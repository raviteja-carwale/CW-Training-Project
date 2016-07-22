using System.Collections.Generic;
using Training.Entities;

namespace Training.DTO
{
    public class UsersList
    {
        public int count { get; set; }
        public List<UserProfile> userList;
    }
}
