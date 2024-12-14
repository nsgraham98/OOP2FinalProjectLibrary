using OOP2FinalProjectLibrary.Data.Objects.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
	public interface IItemManager<T> where T : Item
	{

		string AddItem(int itemId, string title, string category, string publisher, string genre,
			string location, string status, float replaceCost, DateTime pubDate,
			params object[] additionalParam); // For child-specific attributes

		string UpdateItem(T item);

		string DeleteItem(T item);

		List<T> LoadAllItems();

		T LoadItemById(int itemId);

		List<T> SearchItem(string searchField, bool isTitle, bool isCategory, bool isStatus, bool isLocation);
	}
}