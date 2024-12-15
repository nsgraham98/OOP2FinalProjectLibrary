using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using OOP2FinalProjectLibrary.Data.Objects.Items;

namespace OOP2FinalProjectLibrary.Data.Objects
{
    public class Rental : Item
    {
        int rentalId;
        int memberId;
        DateTime startDate;
        DateTime dueDate;
        DateTime? returnedDate;
        string rentStatus;

        public Rental(int rentalId, int memberId, DateTime startDate, DateTime dueDate, DateTime? returnedDate, string rentStatus)
        {
            this.rentalId = rentalId;
            this.memberId = memberId;
            this.startDate = startDate;
            this.dueDate = dueDate;
            this.returnedDate = returnedDate;
            this.rentStatus = rentStatus;
        }
        public Rental() { }

        [PrimaryKey]
        public int RentalId { get => rentalId; set => rentalId = value; }
        public int MemberId { get => memberId; set => memberId = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime DueDate { get => dueDate; set => dueDate = value; }
        public DateTime? ReturnedDate { get => returnedDate; set => returnedDate = value; }
        public string RentStatus { get => rentStatus; set => rentStatus = value; }
    }
}
