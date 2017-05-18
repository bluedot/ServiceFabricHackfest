using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StatelessBackendService.Interfaces.Models
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string Type { get; set; }
        //[DataMember]
        //public Dictionary<string, object> Payload { get; set; }
    }
}
