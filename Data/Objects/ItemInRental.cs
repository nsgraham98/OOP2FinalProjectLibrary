using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace OOP2FinalProjectLibrary.Data.Objects
{
    public class ItemInRental
    {
        int itemId;
        int rentalId;
        int itemNum;
        string itemStatus;

        public ItemInRental(int itemId, int rentalId, int itemNum, string itemStatus)
        {
            this.itemId = itemId;
            this.rentalId = rentalId;
            this.itemNum = itemNum;
            this.itemStatus = itemStatus;
        }
        public ItemInRental() { }

        [PrimaryKey]
        public int ItemId { get => itemId; set => itemId = value; }
        [PrimaryKey]
        public int RentalId { get => rentalId; set => rentalId = value; }
        [PrimaryKey]
        public int ItemNum { get => itemNum; set => itemNum = value; }
        public string ItemStatus { get => itemStatus; set => itemStatus = value; }
    }
}
