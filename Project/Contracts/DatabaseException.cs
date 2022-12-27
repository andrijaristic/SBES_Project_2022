using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public class DatabaseException
    {
        string message;
        [DataMember]
        public string Message { get => message; set => message = value; }

        public DatabaseException(string message)
        {
            Message = message;
        }

        public DatabaseException() : this("") { }
    }
}
