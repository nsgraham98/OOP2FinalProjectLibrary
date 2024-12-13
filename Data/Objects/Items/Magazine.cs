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
        string publication;
        DateTime coverDate;

        public string Issn { get => issn; set => issn = value; }
        public string Publication { get => publication; set => publication = value; }
        public DateTime CoverDate { get => coverDate; set => coverDate = value; }

        // full constructor
        public Magazine(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string issn, string publication, DateTime coverDate)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.Issn = issn;
            this.Publication = publication;
            this.CoverDate = coverDate;
        }

        // constructor for how Magazine table is in DB
        public Magazine(int itemId, string title, string issn, string publication, DateTime coverDate)
        {
            ItemId = itemId;
            Title = title;
            this.Issn = issn;
            this.Publication = publication;
            this.CoverDate = coverDate;
        }

        // constructor with Item obj parameter
        public Magazine(Item item, string issn, string publication, DateTime coverDate)
        {
            this.ItemId = item.ItemId;
            this.Title = item.Title;
            this.Category = item.Category;
            this.Publisher = item.Publisher;
            this.Genre = item.Genre;
            this.Location = item.Location;
            this.Status = item.Status;
            this.ReplaceCost = item.ReplaceCost;
            this.PubDate = item.PubDate;
            this.issn = issn;
            this.publication = publication;
            this.coverDate = coverDate;
        }
        public Magazine() { }
    }
}
