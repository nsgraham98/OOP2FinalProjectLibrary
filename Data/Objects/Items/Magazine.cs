using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Objects.Items
{
    public class Magazine : Item
    {
        string issn;
        DateTime coverDate;

        public Magazine(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string issn, DateTime coverDate)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.issn = issn;
            this.coverDate = coverDate;
        }
    }
}
