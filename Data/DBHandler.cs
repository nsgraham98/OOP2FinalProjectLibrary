using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Xml.Linq;
using OOP2FinalProjectLibrary.Data.Objects;
using OOP2FinalProjectLibrary.Data.Objects.Items;

namespace OOP2FinalProjectLibrary.Data
{
    public class DBHandler
    {
        static string dbPath = Path.Combine(FileSystem.AppDataDirectory, "library_rental.db");
        static SQLiteConnection database;
        static string connectionString = $"Data Source={dbPath}";

        public static SQLiteConnection GetInitialDatabaseConnection()
        {
            if (database == null)
            {
                database = new SQLiteConnection($"Data Source={dbPath}");
            }
            return database;
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("library_rental.db").Result;
                using var newFile = File.Create(dbPath);
                stream.CopyTo(newFile);
            }
            GetInitialDatabaseConnection();
            database.Close();
        }

        public static int GetNextId(string tableName, string idColumnName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string selectQuery = $"SELECT MAX({idColumnName}) AS MaxId FROM {tableName}";

                using (SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, connection))
                {
                    var result = selectCmd.ExecuteScalar();
                    return result == DBNull.Value ? 1 : Convert.ToInt32(result) + 1;
                }
            }  
        }

        // ==================================================================================================
        // LOAD FUNCTIONS
        public List<Member> LoadMembersFromDB()
        {
            List<Member> memberList = new List<Member>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string sql = "Select * from member";
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                using (cmd)
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int memberId = reader.GetInt32(0);
                            string lastName = reader.GetString(1);
                            string firstName = reader.GetString(2);
                            string phone = reader.GetString(3);
                            string email = reader.GetString(4);
                            string address = reader.GetString(5);

                            Member member = new Member(memberId, lastName, firstName, phone, email, address);
                            memberList.Add(member);
                        }
                    }
                }
            }          
            return memberList;
        }
        public List<Rental> LoadRentalsFromDB()
        {
            List<Rental> rentalList = new List<Rental>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string sql = "Select * from rental";
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                using (cmd)
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rentalId = reader.GetInt32(0);
                            int memberId = reader.GetInt32(1);
                            DateTime startDate = reader.GetDateTime(2);
                            DateTime dueDate = reader.GetDateTime(3);
                            DateTime? returnedDate = reader.GetDateTime(4);
                            string rentStatus = reader.GetString(5);

                            Rental rental = new Rental(rentalId, memberId, startDate, dueDate, returnedDate, rentStatus);
                            rentalList.Add(rental);
                        }
                    }
                }
            }
            return rentalList;
        }
        public List<ItemInRental> LoadItemInRentalFromDB()
        {
            List<ItemInRental> iirList = new List<ItemInRental>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string sql = "Select * from iteminrental";
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                using (cmd)
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int itemId = reader.GetInt32(0);
                            int rentalId = reader.GetInt32(1);
                            int itemNum = reader.GetInt32(2);
                            string itemStatus = reader.GetString(3);

                            ItemInRental iir = new ItemInRental(itemId, rentalId, itemNum, itemStatus);
                            iirList.Add(iir);
                        }
                    }
                }
            }
            return iirList;
        }
        public List<Item> LoadGenItemsFromDB()
        {
            List<Item> itemList = new List<Item>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string sql = "Select * from item";
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                using (cmd)
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Item item = new Item
                            {
                                ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Category = reader.GetString(reader.GetOrdinal("Category")),
                                Publisher = reader.GetString(reader.GetOrdinal("Publisher")),
                                Genre = reader.GetString(reader.GetOrdinal("Genre")),
                                Location = reader.GetString(reader.GetOrdinal("Location")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                ReplaceCost = reader.GetFloat(reader.GetOrdinal("ReplaceCost")),
                                PubDate = reader.GetDateTime(reader.GetOrdinal("PubDate"))
                            };
                            itemList.Add(item);
                        }
                    }
                }
            }
            return itemList;
        }
        public List<Item> LoadTypedItemsFromDB()
        {
            //List<Audiobook> abList = new List<Audiobook>();
            //List<Book> bookList = new List<Book>();
            //List<Cd> cdList = new List<Cd>();
            //List<Dvd> dvdList = new List<Dvd>();
            //List<Magazine> magList = new List<Magazine>();

            List<Item> itemList = new List<Item>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                //string sql = "Select * from item";
                string query = @"
                    SELECT i.*, 
                        b.Isbn as bookisbn, b.Author as bookauthor, b.Format as bookformat, 
                        ab.Duration as abduration, ab.Narrator, ab.isbn as abisbn, ab.author as abauthor,
                        cd.artist, cd.label, cd.duration as cdduration,
                        m.issn, m.coverdate, m.publication,
                        dvd.director, dvd.duration as dvdduration, dvd.format as dvdformat             
                    FROM Item i
                    LEFT JOIN Book b ON i.ItemId = b.ItemId
                    LEFT JOIN Audiobook ab ON i.ItemId = ab.ItemId
                    LEFT JOIN cd on i.itemId = cd.itemId
                    LEFT JOIN dvd on i.itemId = dvd.itemId
                    LEFT JOIN magazine m on i.itemId = m.itemId";

                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                using (cmd)
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Item item = new Item
                            {
                                ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Category = reader.GetString(reader.GetOrdinal("Category")),
                                Publisher = reader.GetString(reader.GetOrdinal("Publisher")),
                                Genre = reader.GetString(reader.GetOrdinal("Genre")),
                                Location = reader.GetString(reader.GetOrdinal("Location")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                ReplaceCost = reader.GetFloat(reader.GetOrdinal("ReplaceCost")),
                                PubDate = reader.GetDateTime(reader.GetOrdinal("PubDate"))
                            };

                            if (item.Category == "book")
                            {
                                string isbn = reader.GetString(reader.GetOrdinal("bookisbn"));
                                string author = reader.GetString(reader.GetOrdinal("bookauthor"));
                                string format = reader.GetString(reader.GetOrdinal("bookformat"));

                                Book book = new Book(item, isbn, author, format);
 
                                itemList.Add(book);
                                //bookList.Add(book);
                            }
                            else if (item.Category == "cd")
                            {
                                string artist = reader.GetString(reader.GetOrdinal("artist"));
                                string label = reader.GetString(reader.GetOrdinal("label"));
                                string duration = reader.GetString(reader.GetOrdinal("cdduration"));

                                Cd cd = new Cd(item, artist, label, duration);

                                itemList.Add(cd);
                                //cdList.Add(cd);
                            }
                            else if (item.Category == "audiobook")
                            {
                                string author = reader.GetString(reader.GetOrdinal("abauthor"));
                                string isbn = reader.GetString(reader.GetOrdinal("abisbn"));
                                string duration = reader.GetString(reader.GetOrdinal("abduration"));
                                string narrator = reader.GetString(reader.GetOrdinal("narrator"));

                                Audiobook audiobook = new Audiobook(item, author, isbn, duration, narrator);
                                
                                itemList.Add(audiobook);
                                //abList.Add(audiobook);
                            }
                            else if (item.Category == "dvd")
                            {
                                string director = reader.GetString(reader.GetOrdinal("director"));
                                string duration = reader.GetString(reader.GetOrdinal("dvdduration"));
                                string format = reader.GetString(reader.GetOrdinal("dvdformat"));

                                Dvd dvd = new Dvd(item, director, duration, format);
                                
                                itemList.Add(dvd);
                                //dvdList.Add(dvd);
                            }
                            else
                            {
                                string issn = reader.GetString(reader.GetOrdinal("issn"));
                                string publication = reader.GetString(reader.GetOrdinal("publication"));
                                DateTime coverDate = reader.GetDateTime(reader.GetOrdinal("coverdate"));

                                Magazine mag = new Magazine(item, issn, publication, coverDate);

                                itemList.Add(mag);
                                //magList.Add(mag);
                            }
                        }
                    }
                }
            }
            return itemList;
        }
        //public List<Audiobook> LoadPartialAudiobookFromDB()
        //{
        //    List<Audiobook> abList = new List<Audiobook>();
        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        string sql = "Select * from audiobook";
        //        SQLiteCommand cmd = new SQLiteCommand(sql, connection);
        //        using (cmd)
        //        {
        //            using (SQLiteDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    int itemId = reader.GetInt32(0);
        //                    string title = reader.GetString(1);
        //                    string isbn = reader.GetString(2);
        //                    string author = reader.GetString(3);
        //                    string duration = reader.GetString(4);
        //                    string narrator = reader.GetString(5);

        //                    Audiobook audiobook = new Audiobook(itemId, title, isbn, author, duration, narrator);
        //                    abList.Add(audiobook);
        //                }
        //            }
        //        }
        //    }
        //    return abList;
        //}
        //public List<Book> LoadPartialBookFromDB()
        //{
        //    List<Book> bookList = new List<Book>();
        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        string sql = "Select * from book";
        //        SQLiteCommand cmd = new SQLiteCommand(sql, connection);
        //        using (cmd)
        //        {
        //            using (SQLiteDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    int itemId = reader.GetInt32(0);
        //                    string title = reader.GetString(1);
        //                    string isbn = reader.GetString(2);
        //                    string author = reader.GetString(3);
        //                    string format = reader.GetString(4);

        //                    Book book = new Book(itemId, title, isbn, author, format);
        //                    bookList.Add(book);
        //                }
        //            }
        //        }
        //    }
        //    return bookList;
        //}
        //public List<Cd> LoadPartialCdFromDB()
        //{
        //    List<Cd> cdList = new List<Cd>();
        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        string sql = "Select * from cd";
        //        SQLiteCommand cmd = new SQLiteCommand(sql, connection);
        //        using (cmd)
        //        {
        //            using (SQLiteDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    int itemId = reader.GetInt32(0);
        //                    string title = reader.GetString(1);
        //                    string artist = reader.GetString(2);
        //                    string label = reader.GetString(3);
        //                    string duration = reader.GetString(4);

        //                    Cd cd = new Cd(itemId, title, artist, label, duration);
        //                    cdList.Add(cd);
        //                }
        //            }
        //        }
        //    }
        //    return cdList;
        //}
        //public List<Dvd> LoadPartialDvdFromDB()
        //{
        //    List<Dvd> dvdList = new List<Dvd>();
        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        string sql = "Select * from dvd";
        //        SQLiteCommand cmd = new SQLiteCommand(sql, connection);
        //        using (cmd)
        //        {
        //            using (SQLiteDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    int itemId = reader.GetInt32(0);
        //                    string title = reader.GetString(1);
        //                    string director = reader.GetString(2);
        //                    string duration = reader.GetString(3);
        //                    string format = reader.GetString(4);

        //                    Dvd dvd = new Dvd(itemId, title, director, duration, format);
        //                    dvdList.Add(dvd);
        //                }
        //            }
        //        }
        //    }
        //    return dvdList;
        //}
        //public List<Magazine> LoadPartialMagazineFromDB()
        //{
        //    List<Magazine> magList = new List<Magazine>();
        //    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        string sql = "Select * from magazine";
        //        SQLiteCommand cmd = new SQLiteCommand(sql, connection);
        //        using (cmd)
        //        {
        //            using (SQLiteDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    int itemId = reader.GetInt32(0);
        //                    string title = reader.GetString(1);
        //                    string publication = reader.GetString(2);
        //                    string issn = reader.GetString(3);
        //                    DateTime coverDate = reader.GetDateTime(4);

        //                    Magazine mag = new Magazine(itemId, title, publication, issn, coverDate);
        //                    magList.Add(mag);
        //                }
        //            }
        //        }
        //    }
        //    return magList;
        //}

        public static string InsertMemberDB(string lastName, string firstName, string phone, string email, string address)
        {
            try
            {
                int newId = GetNextId("member", "memberId");
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into member values(@mId, @mLastName, @mFirstName, @mPhone, @mEmail, @mAddress)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@mId", newId);
                        cmd.Parameters.AddWithValue("@mLastName", lastName);
                        cmd.Parameters.AddWithValue("@mFirstName", firstName);
                        cmd.Parameters.AddWithValue("@mPhone", phone);
                        cmd.Parameters.AddWithValue("@mEmail", email);
                        cmd.Parameters.AddWithValue("@mAddress", address);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "Member Successfully Added";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string InsertRentalDB(int memberId, DateTime startDate, DateTime dueDate, DateTime? returnedDate, string status)
        {
            try
            {
                int newId = GetNextId("rental", "rentalId");
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into rental values(@rRId, @rMId, @rStart, @rDue, @rReturned, @rStatus)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@rRId", newId);
                        cmd.Parameters.AddWithValue("@rMId", memberId);
                        cmd.Parameters.AddWithValue("@rStart", startDate);
                        cmd.Parameters.AddWithValue("@rDue", dueDate);
                        cmd.Parameters.AddWithValue("@rReturned", returnedDate);
                        cmd.Parameters.AddWithValue("@rStatus", status);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "Rental Successfully Added";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string InsertItemInRentalDB(int rentalId, int itemId, int itemNum, string itemStatus)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into iteminrental values(@iirRId, @iirIId, @iirNum, @iirStatus)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@iirRId", rentalId);
                        cmd.Parameters.AddWithValue("@iirIId", itemId);
                        cmd.Parameters.AddWithValue("@iirNum", itemNum);
                        cmd.Parameters.AddWithValue("@iirStatus", itemStatus);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "Item Added to Rental";
            }           
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private Item InsertItemDB(string title, string category, string publisher, string genre,
            string location, string status, float replaceCost, DateTime pubDate)
        {
            try
            {
                int newId = GetNextId("rental", "rentalId");
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into item values(@t, @c, @p, @g, @l, @s, @rc, @pd)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@t", title);
                        cmd.Parameters.AddWithValue("@c", category);
                        cmd.Parameters.AddWithValue("@p", publisher);
                        cmd.Parameters.AddWithValue("@g", genre);
                        cmd.Parameters.AddWithValue("@l", location);
                        cmd.Parameters.AddWithValue("@s", status);
                        cmd.Parameters.AddWithValue("@rc", replaceCost);
                        cmd.Parameters.AddWithValue("@pd", pubDate);

                        cmd.ExecuteNonQuery();
                    }
                }
                return new Item(newId, title, category, publisher, genre, location, status, replaceCost, pubDate);
            }       
            catch (Exception e) 
            {
                throw;
            }
        }

        public string InsertBookDB(
            string title, string category, string publisher, string genre, 
            string location, string  status, float replaceCost, DateTime pubDate,
                string isbn, string author, string format)
        {
            try
            {
                Item item = InsertItemDB(title, category, publisher, genre,
            location, status, replaceCost, pubDate);

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into book values(@id, @title, @bIsbn, @bAuthor, @bFormat)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", item.ItemId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@bIsbn", isbn);
                        cmd.Parameters.AddWithValue("@bAuthor", author);
                        cmd.Parameters.AddWithValue("@bFormat", format);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "Book Successfully Added";
            }      
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string InsertCdDB(
            string title, string category, string publisher, string genre,
            string location, string status, float replaceCost, DateTime pubDate,
                string artist, string label, string duration)
        {
            try
            {
                Item item = InsertItemDB(title, category, publisher, genre,
            location, status, replaceCost, pubDate);

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into cd values(@id, @title, @cdArtist, @cdLabel, @cdDuration)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", item.ItemId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@cdArtist", artist);
                        cmd.Parameters.AddWithValue("@cdLabel", label);
                        cmd.Parameters.AddWithValue("@cdDuration", duration);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "CD Successfully Added";
            }           
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string InsertDvdDB(
            string title, string category, string publisher, string genre,
            string location, string status, float replaceCost, DateTime pubDate,
                string director, string duration, string format)
        {
            try
            {
                Item item = InsertItemDB(title, category, publisher, genre,
            location, status, replaceCost, pubDate);

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into dvd values(@id, @title, @dvddirector, @dvdduration, @dvdformat)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", item.ItemId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@dvddirector", director);
                        cmd.Parameters.AddWithValue("@dvdduration", duration);
                        cmd.Parameters.AddWithValue("@dvdformat", format);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "DVD Successfully Added";
            }          
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string InsertMagazineDB(
            string title, string category, string publisher, string genre,
            string location, string status, float replaceCost, DateTime pubDate,
                string publication, string issn, DateTime coverDate)
        {
            try
            {
                Item item = InsertItemDB(title, category, publisher, genre, location, status, replaceCost, pubDate);

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into dvd values(@id, @title, @magPub, @magIssn, @magCDate)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", item.ItemId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@magPub", pubDate);
                        cmd.Parameters.AddWithValue("@magIssn", issn);
                        cmd.Parameters.AddWithValue("@magCDate", coverDate);

                        cmd.ExecuteNonQuery();
                    }
                }
                return $"Magazine Successfully Added";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string InsertAudiobookDB(
            string title, string category, string publisher, string genre,
            string location, string status, float replaceCost, DateTime pubDate,
                string isbn, string author, string duration, string narrator)
        {
            Item item = InsertItemDB(title, category, publisher, genre,
            location, status, replaceCost, pubDate);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "INSERT into dvd values(@id, @title, @abIsbn, @abAuthor, @abDuration, @abNarrator)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", item.ItemId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@abIsbn", isbn);
                        cmd.Parameters.AddWithValue("@abAuthor", author);
                        cmd.Parameters.AddWithValue("@abDuration", duration);
                        cmd.Parameters.AddWithValue("@abNarrator", narrator);

                        cmd.ExecuteNonQuery();
                    }
                    return $"Audiobook Successfully Added";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }            
        }

        public static string UpdateMemberDB(int memberId, string lastName, string firstName, string phone, string email, string address)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE member SET lastname=@lastName, firstname=@firstName, phone=@phone, Email=@email, address=@address WHERE memberId=@memberId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@memberId", memberId);

                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return "Member Updated Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string UpdateRentalDB(int rentalId, int memberId, DateTime startDate, DateTime dueDate, DateTime? returnedDate, string rentStatus)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE rental SET memberId=@memberId, startDate=@startDate, dueDate=@dueDate, returnedDate=@returnedDate WHERE rentalId=@rentalId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@memberId", memberId);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@dueDate", dueDate);
                        cmd.Parameters.AddWithValue("@returnedDate", returnedDate);
                        cmd.Parameters.AddWithValue("@rentStatus", rentStatus);
                        cmd.Parameters.AddWithValue("@rentalId", rentalId);

                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return "Rental Updated Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string UpdateItemInRentalDB(int itemId, int rentalId, string itemStatus)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE iteminrental SET itemId=@itemId, itemStatus=@itemStatus WHERE itemId=@itemId AND rentalId=@rentalId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@itemId", itemId);
                        cmd.Parameters.AddWithValue("@rentalId", rentalId);
                        cmd.Parameters.AddWithValue("@itemStatus", itemStatus);

                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return "Rental Item Updated Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}
