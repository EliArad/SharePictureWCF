using SharingClientLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharingPictureClientApp
{
    public partial class Form1 : Form
    {
        bool m_running = true;
        SharingPictureClient m_client;
        Thread m_thread = null;

        string m_htext = "Sharing picture client ";
        Guid m_guid;
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
         
            pictureBox1.AllowDrop = true;
            SharingPictureClient.ClientCallbackMessage p = new SharingPictureClient.ClientCallbackMessage(ClientCallbackMessage);
            m_client = new SharingPictureClient("localhost", 8092, p);
            Control.CheckForIllegalCrossThreadCalls = false;

            Process[] result = Process.GetProcessesByName("SharingPictureClientApp");
            if (result.Length > 3)
            {
                System.Environment.Exit(0);
                return;
            }
            m_guid = Guid.NewGuid();
            try
            {
                this.Text = m_htext + "Trying to connect to server";
                m_client.Connect();
                m_client.Register(m_guid.ToString());
                this.Text = m_htext + "Connected";
            }
            catch (Exception err)
            {
                m_thread = new Thread(ConnectionThread);
                m_thread.Start();
            }
        }
        void ConnectionThread()
        {
            while (m_running)
            {
                Thread.Sleep(1000);
                try
                {
                    m_client.Connect();
                    m_client.Register(m_guid.ToString());
                    this.Text = m_htext + "Connected";
                    break;
                }
                catch (Exception err)
                {
                    this.Text = m_htext + "Trying to connect to server";   
                }
            }
        }
        void ClientCallbackMessage(string guid, int code, string fileName)
        {
            switch (code)
            {
                case 1:
                    pictureBox1.ImageLocation = fileName;
                break;
            }
        }
        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
           
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            string filename;
            if (GetFilename(out filename, e) == true)
            {
                try
                {
                    pictureBox1.ImageLocation = filename;
                    m_client.BroadcastPicture(filename);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
           
        }

        private void pictureBox1_DragLeave(object sender, EventArgs e)
        {

        }

        private void pictureBox1_DragOver(object sender, DragEventArgs e)
        {

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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_running = false;
            if (m_thread != null && m_thread.IsAlive == true)
                m_thread.Join();
        }
    }
}
