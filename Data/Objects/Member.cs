using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Objects
{
    public class Member
    {
        int memberId;
        string lastName;
        string firstName;
        string phone;
        string email;
        string address;

        public Member(int memberId, string lastName, string firstName, string phone, string email, string address)
        {
            MemberId = memberId;
            LastName = lastName;
            FirstName = firstName;
            Phone = phone;
            Email = email;
            Address = address;
        }

        [PrimaryKey]
        public int MemberId { get => memberId; set => memberId = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Email { get => email; set => email = value; }
        public string Address { get => address; set => address = value; }
    }
}
