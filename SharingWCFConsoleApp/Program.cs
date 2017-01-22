using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace WCFSharePictureServerConsoleApp
{
    using System.IO;
  
    class Program
    {
        

        static void Main(string[] args)
        {
            try
            {
                ServerConnector server = new ServerConnector();
                Console.WriteLine("Sharing server is running at localhost:8092/WCFSharePictureService");
                Console.ReadLine();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
}
