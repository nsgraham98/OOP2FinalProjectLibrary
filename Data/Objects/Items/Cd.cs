using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Objects.Items
{
    public class Cd : Item
    {
        string artist;
        string label;
        string duration;

        public string Artist { get => artist; set => artist = value; }
        public string Label { get => label; set => label = value; }
        public string Duration { get => duration; set => duration = value; }

        public Cd(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string artist, string label, string duration)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.Artist = artist;
            this.Label = label;
            this.Duration = duration;
        }
        public Cd(int itemId, string title, string artist, string label, string duration)
        {
            ItemId = itemId;
            Title = title;
            this.Artist = artist;
            this.Label = label;
            this.Duration = duration;
        }
        public Cd(Item item, string artist, string label, string duration)
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
            this.Artist = artist;
            this.Label = label;
            this.Duration = duration;
        }
        public Cd() { }
    }
}
