using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Objects.Items
{
    public class Audiobook : Item
    {
        string isbn;
        string author;
        string duration;
        string narrator;

        public string Isbn { get => isbn; set => isbn = value; }
        public string Author { get => author; set => author = value; }
        public string Duration { get => duration; set => duration = value; }
        public string Narrator { get => narrator; set => narrator = value; }

        // full constructor
        public Audiobook(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string isbn, string author, string duration, string narrator)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.Author = author;
            this.Isbn = isbn;
            this.Duration = duration;
            this.Narrator = narrator;
        }

        // constructor for how Audiobook table is in DB
        public Audiobook(int itemId, string title, string isbn, string author, string duration, string narrator)
        {
            this.ItemId = itemId;
            this.Title = title;
            this.Isbn = isbn;
            this.Author = author;
            this.Duration = duration;
            this.Narrator = narrator;
        }

        // constructor with Item obj parameter
        public Audiobook(Item item, string isbn, string author, string duration, string narrator)
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
            this.isbn = isbn;
            this.author = author;
            this.duration = duration;
            this.narrator = narrator;
        }
        public Audiobook() { }
    }
}
