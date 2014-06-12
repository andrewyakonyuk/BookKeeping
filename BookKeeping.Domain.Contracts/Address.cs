using System;
using System.Runtime.Serialization;

namespace BookKeeping.Domain.Contracts
{
    
    [Serializable]
    [DataContract(Namespace = "BookKeeping")]
    public class Address
    {
        [DataMember(Order = 1)]
        public string Country { get; set; }
        [DataMember(Order = 2)]
        public string City { get; set; }
        [DataMember(Order = 3)]
        public string ZipCode { get; set; }
        [DataMember(Order = 4)]
        public string Street { get; set; }
    }
}
