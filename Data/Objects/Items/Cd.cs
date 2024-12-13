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


        public Cd(int itemId, string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate, string artist, string label, string duration)
            : base(itemId, title, category, publisher, genre, location, status, replaceCost, pubDate)
        {
            this.artist = artist;
            this.label = label;
            this.duration = duration;
        }
    }
}
