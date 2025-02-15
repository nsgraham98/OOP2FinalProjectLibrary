﻿using OOP2FinalProjectLibrary.Data.Objects.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
	public class BookManager : IItemManager<Book>
	{
		public static List<Book> books;

		private readonly DBHandler _dbHan;

		public BookManager(DBHandler dbHan)
		{
			_dbHan = dbHan;
		}

		public string AddItem(int itemId, string title, string category, string publisher, string genre,
			string location, string status, float replaceCost, DateTime pubDate,
			params object[] additionalParam)
		{

			if (additionalParam.Length < 3)
			{
				return "Error: Missing additional parameters for Book.";
			}

			string isbn = additionalParam[0] as string;
			string author = additionalParam[1] as string;
			string format = additionalParam[2] as string;

			try
			{
				_dbHan.InsertBookDB(
					title,
					category,
					publisher,
					genre,
					location,
					status,
					replaceCost,
					pubDate,
					isbn,
					author,
					format);
				return "Book Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"ERROR: Book cannot be null - {ex.Message}";
			}
		}

		public string UpdateItem(Book b)
		{
			if (b == null)
			{
				return "ERROR! Book cannot be null.";
			}

			try
			{
				return _dbHan.UpdateBookDB(b);
			}

			catch (Exception ex)
			{
				return $"An error occurred while updating the book: {ex.Message}";

			}
		}

		public string DeleteItem(Book b)
		{
			if (b == null)
			{
				return "ERROR! Book cannot be null.";
			}

			try
			{
				return _dbHan.DeleteBookDB(b);
			}
			catch (Exception ex)
			{
				return $"An error occurred while deleting the book: {ex.Message}";
			}
		}

		public List<Book> LoadAllItems()
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Book>().ToList();
		}

		public Book LoadItemById(int itemId)
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Book>().FirstOrDefault(b => b.ItemId == itemId);
		}
		public List<Book> SearchItem(string searchField, bool isTitle, bool isCategory, bool isStatus, bool isLocation)
		{
			Book b = new Book();
			var allBooks = LoadAllItems();
			var filteredBooks = allBooks.Where(Dvd =>
				(isTitle && b.Title.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
				(isCategory && b.Category.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
				(isStatus && b.Status.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
				(isLocation && b.Location.Contains(searchField, StringComparison.OrdinalIgnoreCase))
				).ToList();
			return filteredBooks;
		}
	}
}
