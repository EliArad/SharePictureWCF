using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.IO;
 using SharingPictureClientApi.SharingPictureReference;

namespace SharingClientLib
{

    public class SharingPictureClient : IShareServiceCallback
    {

        IShareService m_client = null;
        DuplexChannelFactory<IShareService> pipeFactory;
        bool m_IsConnected = false;
        string m_clientIpAdresss;
        string m_fieldGuid;
        public delegate void ClientCallbackMessage(string guid , int code, string fileName);

        string m_ipAddress;
        int m_portNumber;

        ClientCallbackMessage pClientCallback;
        
        public bool IsConnected
        {
            get
            {
                return m_IsConnected;
            }

        }
        public SharingPictureClient(string ipAddress, 
                                    int portNumber, 
                                    ClientCallbackMessage p)
        {
           
            m_ipAddress = ipAddress;
            m_portNumber = portNumber;
            pClientCallback = p;
        }

        public void Connect()
        {
            try
            {
                m_clientIpAdresss = m_ipAddress;
                NetTcpBinding tcpBinding = new NetTcpBinding();
                tcpBinding.OpenTimeout = TimeSpan.FromSeconds(15);
                tcpBinding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                tcpBinding.SendTimeout = new TimeSpan(0, 5, 0);
                tcpBinding.CloseTimeout = TimeSpan.FromSeconds(5);
                tcpBinding.ReliableSession.InactivityTimeout = new TimeSpan(1, 0, 0, 0);

                tcpBinding.TransactionFlow = false;
                tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                //tcpBinding.Security.Mode = SecurityMode.None;
                tcpBinding.Security.Mode = SecurityMode.Transport;


                pipeFactory =
                 new DuplexChannelFactory<IShareService>(
                     new InstanceContext(this),
                    tcpBinding,
                     new EndpointAddress("net.tcp://" + m_ipAddress + ":" + m_portNumber + "/WCFSharePictureService"));

                m_client = pipeFactory.CreateChannel();

                m_IsConnected = true;
                ((ICommunicationObject)m_client).Closed += new EventHandler(delegate
                {
                    ServiceClose(m_ipAddress);
                });

                ((ICommunicationObject)m_client).Faulted += new EventHandler(delegate
                {
                    ServiceFault(m_ipAddress);
                });
                //pipeFactory.Faulted += new EventHandler(Channel_Faulted);
                //pipeFactory.Closed += new EventHandler(Channel_Faulted);
                //m_client = new WatsonServiceClient("BasicHttpBinding_IWatsonService");
            }
            catch (Exception err)
            {
                if (err.InnerException != null)
                    throw (new SystemException("Error#1: " + err.Message + " " + err.InnerException.Message));
                else
                    throw (new SystemException("Error#1: " + err.Message));
            }
        }
       
        public void Disconnect()
        {

        }
       
        private void ServiceClose(string ipAddress)
        {
            m_IsConnected = false;
            Console.WriteLine("Service closed!");
            if (pClientCallback != null)
            {
                //pClientCallback();
            }
        }

        public void SetBroadcastToScada(bool s)
        {
            try
            {
                //m_client.SetBroadcastToScada(s);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }  
        }
        private void ServiceFault(string ipAddress)
        {
            m_IsConnected = false;
            Console.WriteLine("Service faulted!");
            ((ICommunicationObject)m_client).Faulted -= new EventHandler(delegate
            {

            });
            Thread t = new Thread(() =>
            {
                //if (pClientCallback != null)
                {
                    //pClientCallback(m_fieldGuid);
                }
            });
            t.Start();
        } 

        public void Register(string fieldGuid)
        {
            try
            {
                m_client.Registration(fieldGuid);
                m_fieldGuid = fieldGuid;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }     
        }
         
        public void BroadcastPicture(string fileName)
        {
            try
            {
                m_client.BroadcastPicture(m_fieldGuid,fileName);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
         
        }
         
        void IShareServiceCallback.NotifyCallbackMessage(string fieldGuid, string filename, DateTime date)
        {
            // we want to know which server send us data , we can work with multiple servers
            // Code 1: file name has been send;
            pClientCallback(fieldGuid, 1, filename);
        }
    }
}
