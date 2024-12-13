using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Objects.Items
{
    public class Book : Item
    {
        string isbn;
        string author;
        string format;

        public string Isbn { get => isbn; set => isbn = value; }
        public string Author { get => author; set => author = value; }
        public string Format { get => format; set => format = value; }

        // full constructor
        public Book(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string isbn, string author, string format)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.Isbn = isbn;
            this.Author = author;
            this.Format = format;       
        }

        // constructor for how Book table is in DB
        public Book(int itemId, string title, string isbn, string author, string format)
        {
            ItemId = itemId;
            Title = title;
            this.Isbn = isbn;
            this.Author = author;
            this.Format = format;
        }

        // constructor with Item obj parameter
        public Book(Item item, string isbn, string author, string format)
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
            this.format = format;
        }
        public Book() { }
    }
}
