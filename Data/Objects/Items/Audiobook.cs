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

        public Audiobook(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string isbn, string author, string duration, string narrator)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.author = author;
            this.isbn = isbn;
            this.duration = duration;
            this.narrator = narrator;
        }
    }
}
