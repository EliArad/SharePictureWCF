
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.IO;
using System.Timers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.ServiceModel.Channels;
using System.Xml;

 
namespace ShareServiceServiceLib
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TestService" in both code and config file together.
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ShareService : IShareService 
    {
        public static IDuplexServiceCallback Callback;
        public Dictionary<string, IDuplexServiceCallback> listCallback = new Dictionary<string, IDuplexServiceCallback>();

        List<string> removeList = new List<string>();
        string m_fieldGuid;
        public ShareService()
        {
            
      
        }
     
        public void Registration(string baseFieldGuid)
        {

            Callback = OperationContext.Current.GetCallbackChannel<IDuplexServiceCallback>();
            if (listCallback.ContainsKey(baseFieldGuid) == false)
                listCallback.Add(baseFieldGuid, Callback);
            else
                listCallback[baseFieldGuid] = Callback;
        }
        public void BroadcastPicture(string FieldGuid, string fileName)
        {
            removeList.Clear();
            foreach (KeyValuePair<string, IDuplexServiceCallback> p  in listCallback)
            {
                if (p.Key != FieldGuid)
                {
                    try
                    {
                        p.Value.NotifyCallbackMessage("base guid", fileName, DateTime.Now);
                        Console.WriteLine("Sedning data back..");
                    }
                    catch (Exception err)
                    {
                        removeList.Add(p.Key);
                        Console.WriteLine(err.Message);
                    }
                }
            }
            foreach (string s in removeList)
            {
                listCallback.Remove(s);
            }
        }
    }
}
