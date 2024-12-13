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


        public Dvd(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string director, string duration, string format)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.director = director;
            this.duration = duration;
            this.format = format;
        }
    }
}
