using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Objects.Items
{
    public class Item
    {
        int itemId;
        string title;
        string category;
        string publisher;
        string genre;
        string location;
        string status;
        float replaceCost;
        DateTime pubDate;

        public Item(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate)
        {
            this.itemId = itemId;
            this.title = title;
            this.category = category;
            this.publisher = publisher;
            this.genre = genre;
            this.location = location;
            this.status = status;
            this.replaceCost = replaceCost;
            this.pubDate = pubDate;
        }
        public Item() { }

        [PrimaryKey]
        public int ItemId { get => itemId; set => itemId = value; }
        public string Title { get => title; set => title = value; }
        public string Category { get => category; set => category = value; }
        public string Publisher { get => publisher; set => publisher = value; }
        public string Genre { get => genre; set => genre = value; }
        public string Location { get => location; set => location = value; }
        public string Status { get => status; set => status = value; }
        public float ReplaceCost { get => replaceCost; set => replaceCost = value; }
        public DateTime PubDate { get => pubDate; set => pubDate = value; }
    }
}
