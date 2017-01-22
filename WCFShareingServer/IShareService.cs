using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ShareServiceServiceLib
{

    [ServiceContract(SessionMode = SessionMode.Required,
                      CallbackContract = typeof(IDuplexServiceCallback))]
    public interface IShareService
    {

        [OperationContract]
        void Registration(string fieldGuid);

        [OperationContract]
        void BroadcastPicture(string fileName, string FieldGuid);
 
    }

}
