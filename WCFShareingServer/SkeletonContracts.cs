//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShareServiceServiceLib
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class PData1
    {
        public bool data ;
        public string str ;

        [DataMember]
        public bool valueps
        {
            get { return data; }
            set { value = data; }
        }

        [DataMember]
        public string strps
        {
            get { return str; }
            set { str = value; }
        }

    

    }
}