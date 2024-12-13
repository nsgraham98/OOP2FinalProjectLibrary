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


        public Book(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string isbn, string author, string format)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.isbn = isbn;
            this.author = author;
            this.format = format;       
        }
    }
}
