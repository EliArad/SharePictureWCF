using MemoryMappedFilesApiLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryMappedFilesDemoApp
{
    public partial class Form1 : Form
    {
        Bitmap m_bitmap;
        //ShareImagePlaceInFile m_sharePlace = new ShareImagePlaceInFile("MySharePlace");
        ShareImagePlaceInMemory m_sharePlace = new ShareImagePlaceInMemory("MySharePlace");
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int RF_TESTMESSAGE = 0xA123;


        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            pictureBox1.AllowDrop = true;
             
            Process[] _myProcesses = Process.GetProcessesByName("MemoryMappedFilesDemoApp");
            if (_myProcesses.Length > 5)
            {
                System.Environment.Exit(0);
                return;
            }
              
        }

        void SendMessageToAllClients()
        {
            //get this running process
            Process proc = Process.GetCurrentProcess();
            //get all other (possible) running instances
            Process[] processes = Process.GetProcessesByName(proc.ProcessName);

            if (processes.Length > 1)
            {
                //iterate through all running target applications
                foreach (Process p in processes)
                {
                    if (p.Id != proc.Id)
                    {
                        //now send the RF_TESTMESSAGE to the running instance
                        SendMessage(p.MainWindowHandle, RF_TESTMESSAGE, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }
         
        void ShowPicture()
        {
            byte[] data = null;
            data = m_sharePlace.Read();
            m_bitmap = ByteToImage(data);
            pictureBox1.Image = m_bitmap;
        }
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == RF_TESTMESSAGE)
            {
                ShowPicture();
            }
            //be sure to pass along all messages to the base also
            base.WndProc(ref message);
        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
             
            Bitmap bm = new Bitmap(mStream);
            mStream.Dispose();
            return bm;
        }
        protected bool GetFilename(out string filename, DragEventArgs e)
        {
            bool ret = false;
            filename = String.Empty;

            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                Array data = ((IDataObject)e.Data).GetData("FileName") as Array;
                if (data != null)
                {
                    if ((data.Length == 1) && (data.GetValue(0) is String))
                    {
                        filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                        {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            string filename;
            if (GetFilename(out filename, e) == true)
            {
                try
                {
                    pictureBox1.ImageLocation = filename;
                    m_sharePlace.Create(filename);
                    ShowPicture();
                    SendMessageToAllClients();
   
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
