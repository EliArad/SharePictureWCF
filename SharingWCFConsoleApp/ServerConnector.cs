using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.Net;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Microsoft.Win32;
//using elialog;

namespace WCFSharePictureServerConsoleApp
{

    using System.Threading;
    using ShareServiceServiceLib;
    public class ServerConnector
    {
        static bool m_open = false;
        static string urlMeta, urlService = "";
        static ServiceHost host = null;      
    
        ShareService m_shareService = new ShareService();
       
        public ServerConnector()
        {

            try
            { 
                string ipAddress = "localhost";
            
                urlService = "net.tcp://" + ipAddress + ":8092/WCFSharePictureService";
                host = new ServiceHost(m_shareService, new Uri(urlService));
                //host = new ServiceHost(typeof(WatsonService));
                host.Opening += new EventHandler(host_Opening);
                host.Opened += new EventHandler(host_Opened);
                host.Closing += new EventHandler(host_Closing);
                host.Closed += new EventHandler(host_Closed);

                // The binding is where we can choose what
                // transport layer we want to use. HTTP, TCP ect.
                NetTcpBinding tcpBinding = new NetTcpBinding();
                tcpBinding.ReliableSession.InactivityTimeout = new TimeSpan(1, 0, 0, 0);
                tcpBinding.TransactionFlow = false;
                //tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                int x = tcpBinding.MaxConnections;
                tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;


                tcpBinding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                tcpBinding.SendTimeout = new TimeSpan(0, 5, 0);
                //tcpBinding.OpenTimeout = new TimeSpan(0, 0, 2);
                //tcpBinding.CloseTimeout = new TimeSpan(0, 0, 2);
                //tcpBinding.Security.Mode = SecurityMode.None;
                tcpBinding.Security.Mode = SecurityMode.Transport;
                // <- Very crucial

                // Add a endpoint
                host.AddServiceEndpoint(typeof(
                   IShareService), tcpBinding, urlService);
             

                // A channel to describe the service.
                // Used with the proxy scvutil.exe tool
                ServiceMetadataBehavior metadataBehavior;
                metadataBehavior =
                  host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadataBehavior == null)
                {
                    metadataBehavior = new ServiceMetadataBehavior();
                    metadataBehavior.HttpGetUrl = new Uri("http://" + ipAddress + ":8093/WCFSharePictureService");
                    metadataBehavior.HttpGetEnabled = true;
                    metadataBehavior.ToString();
                    host.Description.Behaviors.Add(metadataBehavior);
                    urlMeta = metadataBehavior.HttpGetUrl.ToString();
                }

                m_open = false;
                host.Open();
                m_open = true;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        void NotifyCallbackMessage(string ipAddress, int code, string msg)
        {
            
        }
        void NotifyCallbackData(string ipAddress, byte[] data, int size)
        {
           
        }
        public void Close()
        {            
            if (m_open == true)
                host.Close();
        }
        void host_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Service closed");
        }

        void host_Closing(object sender, EventArgs e)
        {
            Console.WriteLine("Service closing ... stand by");
        }

        void host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Service opened.");
            Console.WriteLine("Service URL:\t" + urlService);
            Console.WriteLine("Meta URL:\t" + urlMeta + " (Not that relevant)");
            Console.WriteLine("Waiting for clients...");
        }
        void host_Opening(object sender, EventArgs e)
        {
            Console.WriteLine("Service opening ... Stand by");
        }
    }
}
