using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public enum Months 
    {
        [EnumMember] JANUAR, 
        [EnumMember] FEBRUAR, 
        [EnumMember] MART, 
        [EnumMember] APRIL, 
        [EnumMember] MAJ,
        [EnumMember] JUN,
        [EnumMember] JUL,
        [EnumMember] AVGUST,
        [EnumMember] SEPTEMBAR,
        [EnumMember] OKTOBAR,
        [EnumMember] NOVEMBAR,
        [EnumMember] DECEMBAR
    }
}
