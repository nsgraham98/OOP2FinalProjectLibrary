using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Objects.Items
{
    public class Dvd : Item
    {
        string director;
        string duration;
        string format;

        public string Director { get => director; set => director = value; }
        public string Duration { get => duration; set => duration = value; }
        public string Format { get => format; set => format = value; }

        // full constructor
        public Dvd(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string director, string duration, string format)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.Director = director;
            this.Duration = duration;
            this.Format = format;
        }

        // constructor for how DVD table is in DB
        public Dvd(int itemId, string title, string director, string duration, string format)
        {
            ItemId = itemId;
            Title = title;
            this.Director = director;
            this.Duration = duration;
            this.Format = format;
        }

        // constructor with Item obj parameter
        public Dvd(Item item, string director, string duration, string format)
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
            this.director = director;
            this.duration = duration;
            this.format = format;
        }
        public Dvd() { }
    }
}
