using System;
using System.Runtime.Serialization;

namespace Training.Entities
{
    [Serializable]
    [DataContract]
    public class UserProfile
    {
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastName")]
        public string LastName { get; set; }

        [DataMember(Name = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [DataMember(Name = "gender")]
        public char Gender { get; set; }
    }
}
