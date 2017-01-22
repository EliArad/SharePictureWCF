using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ShareServiceServiceLib
{

  [ServiceContract]
  public interface IDuplexServiceCallback {

    [OperationContract(IsOneWay = true)]
      void NotifyCallbackMessage(string fieldGuid, string filename , DateTime date);

  }
}
