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
using Microsoft.Maui.ApplicationModel;

namespace OOP2FinalProjectLibrary.Data
{
    public class DBHandler
    {
        // C:\Users\[your user name]\AppData\Local\Packages\com.companyname.[project file name]\LocalState\library_rental.db
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

        // function gets run on app startup (in App.xaml.cs)
        /* checks if local copy of DB exists
         *  - if not it creates it in users AppData directory
         *  - template DB is located in Resources/Raw/library_rental.db */
        public static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("library_rental.db").Result; // template db
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
                connection.Open();
                string selectQuery = $"SELECT MAX({idColumnName}) AS MaxId FROM {tableName}";

                using (SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, connection))
                {
                    var result = selectCmd.ExecuteScalar();
                    return result == DBNull.Value ? 1 : Convert.ToInt32(result) + 1;
                }
            }  
        }

/* =====================================================================================
 * LOAD FUNCTIONS 
 *      - all Load functions return a List of all entries in DB of the specified type
 */
        public List<Member> LoadMembersFromDB()
        {
            List<Member> memberList = new List<Member>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "Select * from member";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
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
                connection.Open();
                string sql = "Select * from rental";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rentalId = reader.GetInt32(0);
                            int memberId = reader.GetInt32(1);
                            DateTime startDate = reader.GetDateTime(2);
                            DateTime dueDate = reader.GetDateTime(3);
                            DateTime? returnedDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
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
                connection.Open();
                string sql = "Select * from iteminrental";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
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
        public List<Item> LoadGenericItemsFromDB()
        {
            List<Item> itemList = new List<Item>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "Select * from item";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
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

        // return List of all typed Item objects (Book, Audiobook, etc)
        // function could be easily modified to return a specific Object List (see commented code within)
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

                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
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
                            else if (item.Category == "magazine")
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
        
/* =====================================================================================
 * INSERT FUNCTIONS 
 *      - most functions return a string as "proof" the function succesfully happened
 *      - can use string as a front end pop up message or something
 *      - insert functions do not require ID in parameter, ID is automatically generated (except ItemInRental, ID is not generated)
 */
        public string InsertMemberDB(string lastName, string firstName, string phone, string email, string address)
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

        // InsertItemDB is only used and accessed from Insert{child}DB functions
        public Item InsertItemDB(string title, string category, string publisher, string genre,
            string location, string status, float replaceCost, DateTime pubDate)
        {
            try
            {
                int newId = GetNextId("item", "itemId");
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into item values(@id, @t, @c, @p, @g, @l, @s, @rc, @pd)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", newId);
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
                return null;
            }
        }

        public string InsertBookDB(
            string title, string category, string publisher, string genre, 
            string location, string  status, float replaceCost, DateTime pubDate,
                string isbn, string author, string format)
        {
            try
            {
                Item? item = InsertItemDB(title, category, publisher, genre, location, status, replaceCost, pubDate);
                if (item != null)
                {
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
                else { return "Error. Item was not Added"; }    
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
                Item? item = InsertItemDB(title, category, publisher, genre, location, status, replaceCost, pubDate);
                if (item != null)
                {
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
                else { return "Error. Item was not Added"; }              
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
                Item? item = InsertItemDB(title, category, publisher, genre, location, status, replaceCost, pubDate);
                if (item != null)
                {
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
                else { return "Error. Item was not added"; }             
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
                Item? item = InsertItemDB(title, category, publisher, genre, location, status, replaceCost, pubDate);
                if (item != null)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT into magazine values(@id, @title, @magPub, @magIssn, @magCDate)";

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
                    return "Magazine Successfully Added";
                }
                else { return "Error. Item was not added"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string InsertAudiobookDB(
            string title, string category, string publisher, string genre, string location, string status, float replaceCost, DateTime pubDate,string isbn, string author, string duration, string narrator)
        {
            try
            {
                Item? item = InsertItemDB(title, category, publisher, genre, location, status, replaceCost, pubDate);
                if (item != null)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT into audiobook values(@id, @title, @abIsbn, @abAuthor, @abDuration, @abNarrator)";

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
                        return "Audiobook Successfully Added";
                    }
                }
                else { return "Error. Item was not Added"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }               
        }

        // used in conjunction with DeleteItemDB child functions to keep DB consistent
        public Item InsertItemWhenErrorDB(Item item)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT into item values(@id, @t, @c, @p, @g, @l, @s, @rc, @pd)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", item.ItemId);
                        cmd.Parameters.AddWithValue("@t", item.Title);
                        cmd.Parameters.AddWithValue("@c", item.Category);
                        cmd.Parameters.AddWithValue("@p", item.Publisher);
                        cmd.Parameters.AddWithValue("@g", item.Genre);
                        cmd.Parameters.AddWithValue("@l", item.Location);
                        cmd.Parameters.AddWithValue("@s", item.Status);
                        cmd.Parameters.AddWithValue("@rc", item.ReplaceCost);
                        cmd.Parameters.AddWithValue("@pd", item.PubDate);

                        cmd.ExecuteNonQuery();
                    }
                }
                return item;
            }
            catch (Exception e)
            {
                return null;
            }
        }

/* =====================================================================================
 * UPDATE FUNCTIONS
 *      - most functions return a string as "proof" the function succesfully happened
 *      - can use string as a front end pop up message or something
 */
        public string UpdateMemberDB(Member m)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE member SET lastname=@lastName, firstname=@firstName, phone=@phone, Email=@email, address=@address WHERE memberId=@memberId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@lastName", m.LastName);
                        cmd.Parameters.AddWithValue("@firstName", m.FirstName);
                        cmd.Parameters.AddWithValue("@phone", m.Phone);
                        cmd.Parameters.AddWithValue("@email",m.Email);
                        cmd.Parameters.AddWithValue("@address", m.Address);
                        cmd.Parameters.AddWithValue("@memberId", m.MemberId);

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
        public string UpdateRentalDB(Rental r)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE rental SET memberId=@memberId, startDate=@startDate, dueDate=@dueDate, returnedDate=@returnedDate WHERE rentalId=@rentalId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@memberId", r.MemberId);
                        cmd.Parameters.AddWithValue("@startDate", r.StartDate);
                        cmd.Parameters.AddWithValue("@dueDate", r.DueDate);
                        cmd.Parameters.AddWithValue("@returnedDate", r.ReturnedDate);
                        cmd.Parameters.AddWithValue("@rentStatus", r.RentStatus);
                        cmd.Parameters.AddWithValue("@rentalId", r.RentalId);

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
        public string UpdateItemInRentalDB(ItemInRental ir)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE iteminrental SET itemId=@itemId, itemStatus=@itemStatus WHERE itemId=@itemId AND rentalId=@rentalId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@itemId", ir.ItemId);
                        cmd.Parameters.AddWithValue("@rentalId", ir.RentalId);
                        cmd.Parameters.AddWithValue("@itemStatus", ir.ItemStatus);

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

        // UpdateItemDB is only used and accessed from Update{child}DB functions
        public bool UpdateItemDB(Item i)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE item SET title=@title, category=@category, publisher=@publisher, genre=@genre, location=@location, status=@status, replaceCost=@replaceCost, pubDate=@pubDate WHERE itemId=@itemId";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@title", i.Title);
                        cmd.Parameters.AddWithValue("@category", i.Category);
                        cmd.Parameters.AddWithValue("@publisher", i.Publisher);
                        cmd.Parameters.AddWithValue("@genre", i.Genre);
                        cmd.Parameters.AddWithValue("@location", i.Location);
                        cmd.Parameters.AddWithValue("@status", i.Status);
                        cmd.Parameters.AddWithValue("@replaceCost", i.ReplaceCost);
                        cmd.Parameters.AddWithValue("@pubDate", i.PubDate);
                        cmd.Parameters.AddWithValue("@itemId", i.ItemId);

                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public string UpdateAudiobookDB(Audiobook ab)
        {
            try
            {
                bool wasUpdated = UpdateItemDB(ab);

                if (wasUpdated)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE audiobook SET title=@title, isbn=@isbn, author=@author, duration=@duration, narrator=@narrator WHERE itemId=@itemId";

                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@title", ab.Title);
                            cmd.Parameters.AddWithValue("@isbn", ab.Isbn);
                            cmd.Parameters.AddWithValue("@author", ab.Author);
                            cmd.Parameters.AddWithValue("@duration", ab.Duration);
                            cmd.Parameters.AddWithValue("@narrator", ab.Narrator);

                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    return "Audiobook Updated Successfully";
                }
                else { return "Error. Item Was Not Updated"; }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string UpdateBookDB(Book b)
        {
            try
            {
                bool wasUpdated = UpdateItemDB(b);

                if (wasUpdated)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE book SET title=@title, isbn=@isbn, author=@author, format=@format WHERE itemId=@itemId";

                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@title", b.Title);
                            cmd.Parameters.AddWithValue("@isbn", b.Isbn);
                            cmd.Parameters.AddWithValue("@author", b.Author);
                            cmd.Parameters.AddWithValue("@format", b.Format);
                            cmd.Parameters.AddWithValue("@itemId", b.ItemId);

                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    return "Book Updated Successfully";
                }
                else { return "Error. Item Was Not Updated"; }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string UpdateCdDB(Cd cd)
        {
            try
            {
                bool wasUpdated = UpdateItemDB(cd);

                if (wasUpdated)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE book SET title=@title, artist=@artist, label=@label, duration=@duration WHERE itemId=@itemId";

                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@title", cd.Title);
                            cmd.Parameters.AddWithValue("@label", cd.Label);
                            cmd.Parameters.AddWithValue("@artist", cd.Artist);
                            cmd.Parameters.AddWithValue("@duration", cd.Duration);
                            cmd.Parameters.AddWithValue("@itemId", cd.ItemId);

                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    return "CD Updated Successfully";
                }
                else { return "Error. Item Was Not Updated"; }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string UpdateDvdDB(Dvd dvd)
        {
            try
            {
                bool wasUpdated = UpdateItemDB(dvd);

                if (wasUpdated)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE book SET title=@title, director=@director, duration=@duration, format=@format WHERE itemId=@itemId";

                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@title", dvd.Title);
                            cmd.Parameters.AddWithValue("@director", dvd.Director);
                            cmd.Parameters.AddWithValue("@duration", dvd.Duration);
                            cmd.Parameters.AddWithValue("@format", dvd.Format);
                            cmd.Parameters.AddWithValue("@itemId", dvd.ItemId);

                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    return "DVD Updated Successfully";
                }
                else { return "Error. Item Was Not Updated"; }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string UpdateMagazineDB(Magazine m)
        {
            try
            {
                bool wasUpdated = UpdateItemDB(m);

                if (wasUpdated)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE book SET title=@title, publication=@publication, issn=@issn, coverDate=@coverDate WHERE itemId=@itemId";

                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@title", m.Title);
                            cmd.Parameters.AddWithValue("@publication", m.Publication);
                            cmd.Parameters.AddWithValue("@issn", m.Issn);
                            cmd.Parameters.AddWithValue("@coverDate", m.CoverDate);
                            cmd.Parameters.AddWithValue("@itemId", m.ItemId);

                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    return "Magazine Updated Successfully";
                }
                else { return "Error. Item Was Not Updated"; }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

/* =====================================================================================
 * DELETE FUNCTIONS
 *      - most functions return a string as "proof" the function succesfully happened
 *      - can use string as a front end pop up message or something
 */
        public string DeleteMemberDB(int memberId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM member WHERE memberId=@memberId";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@memberId", memberId);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return "Member Deleted Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string DeleteRentalDB(int rentalId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM rental WHERE rentalId=@rentalId";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@rentalId", rentalId);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return "Rental Deleted Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string DeleteItemInRentalDB(int rentalId, int itemId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM iteminrental WHERE rentalId=@rentalId AND itemId=@itemId";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@rentalId", rentalId);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return "Item in Rental Deleted Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // DeleteItemDB is only used and accessed from Delete{child}DB functions
        // Delete Item functions need the full Obj as parameter so they can rebuild item 
        // table in DB in case of error halfway through the function
        public bool DeleteItemDB(Item item)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM item WHERE itemId=@itemId";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@itemId", item.ItemId);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }                   
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public string DeleteAudiobookDB(Audiobook ab)
        {
            bool wasDeleted = DeleteItemDB(ab);
            if (wasDeleted)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "DELETE FROM audiobook WHERE itemId=@itemId";
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@itemId", ab.ItemId);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    return "Audiobook Deleted Successfully";
                }
                catch (Exception e)
                {
                    InsertItemWhenErrorDB(ab);
                    return e.Message;
                }
            }
            else { return "Error. Item was not Deleted"; }
        }
        public string DeleteBookDB(Book book)
        {
            bool wasDeleted = DeleteItemDB(book);
            if (wasDeleted)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "DELETE FROM book WHERE itemId=@itemId";
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@itemId", book.ItemId);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    return "Book Deleted Successfully";
                }
                catch (Exception e)
                {
                    InsertItemWhenErrorDB(book);
                    return e.Message;
                }
            }
            else { return "Error. Item was not Deleted"; }
        }
        public string DeleteCdDB(Cd cd)
        {
            bool wasDeleted = DeleteItemDB(cd);
            if (wasDeleted)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "DELETE FROM cd WHERE itemId=@itemId";
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@itemId", cd.ItemId);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    return "CD Deleted Successfully";
                }
                catch (Exception e)
                {
                    InsertItemWhenErrorDB(cd);
                    return e.Message;
                }
            }
            else { return "Error. Item was not Deleted"; }
        }
        public string DeleteDvdDB(Dvd dvd)
        {
            bool wasDeleted = DeleteItemDB(dvd);
            if (wasDeleted)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "DELETE FROM dvd WHERE itemId=@itemId";
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@itemId", dvd.ItemId);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    return "DVD Deleted Successfully";
                }
                catch (Exception e)
                {
                    InsertItemWhenErrorDB(dvd);
                    return e.Message;
                }
            }
            else { return "Error. Item was not Deleted"; }
        }
        public string DeleteMagazineDB(Magazine mag)
        {
            bool wasDeleted = DeleteItemDB(mag);
            if (wasDeleted)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "DELETE FROM magazine WHERE itemId=@itemId";
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@itemId", mag.ItemId);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    return "Magazine Deleted Successfully";
                }
                catch (Exception e)
                {
                    InsertItemWhenErrorDB(mag);
                    return e.Message;
                }
            }
            else { return "Error. Item was not Deleted"; }
        }

/* =====================================================================================
 * SEARCH FUNCTIONS
 */
        public List<Item> SearchItemsDB(string titleField, string catField, string statusField, string locationField, bool isTitle, bool isCategory, bool isStatus, bool isLocation)
        {
            List<string> sColumnList = new List<string>();
            List<string> sTermsList = new List<string>();  

            if (isTitle)
            {
                sColumnList.Add("title LIKE");
                sTermsList.Add($"%{titleField}%");
            }
            if (isCategory)
            {
                sColumnList.Add("category LIKE");
                sTermsList.Add($"{catField}");
            }
            if (isStatus)
            {
                sColumnList.Add("status LIKE");
                sTermsList.Add($"{statusField}");
            }
            if (isLocation)
            {
                sColumnList.Add("location LIKE");
                sTermsList.Add($"{locationField}");
            }

            if (sColumnList.Count == 0)
            {
                return new List<Item>(); // No search criteria
            }

            string whereClause = $@"{sColumnList[0]} '{sTermsList[0]}' ";
            for (int i = 1; i < sColumnList.Count(); i++)
            {
                whereClause += $@"AND {sColumnList[i]} '{sTermsList[i]}' ";
            }

            // SQL query
            //string query = $"SELECT * FROM Item WHERE {whereClause}";
            string query = $@"
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
                        LEFT JOIN magazine m on i.itemId = m.itemId
                    WHERE {whereClause}";

            try
            {
                List<Item> itemList = new List<Item>();
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
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

                                if (item.Category.ToLower() == "book")
                                {
                                    string isbn = reader.GetString(reader.GetOrdinal("bookisbn"));
                                    string author = reader.GetString(reader.GetOrdinal("bookauthor"));
                                    string format = reader.GetString(reader.GetOrdinal("bookformat"));

                                    Book book = new Book(item, isbn, author, format);

                                    itemList.Add(book);
                                    //bookList.Add(book);
                                }
                                else if (item.Category.ToLower() == "cd")
                                {
                                    string artist = reader.GetString(reader.GetOrdinal("artist"));
                                    string label = reader.GetString(reader.GetOrdinal("label"));
                                    string duration = reader.GetString(reader.GetOrdinal("cdduration"));

                                    Cd cd = new Cd(item, artist, label, duration);

                                    itemList.Add(cd);
                                    //cdList.Add(cd);
                                }
                                else if (item.Category.ToLower() == "audiobook")
                                {
                                    string author = reader.GetString(reader.GetOrdinal("abauthor"));
                                    string isbn = reader.GetString(reader.GetOrdinal("abisbn"));
                                    string duration = reader.GetString(reader.GetOrdinal("abduration"));
                                    string narrator = reader.GetString(reader.GetOrdinal("narrator"));

                                    Audiobook audiobook = new Audiobook(item, author, isbn, duration, narrator);

                                    itemList.Add(audiobook);
                                    //abList.Add(audiobook);
                                }
                                else if (item.Category.ToLower() == "dvd")
                                {
                                    string director = reader.GetString(reader.GetOrdinal("director"));
                                    string duration = reader.GetString(reader.GetOrdinal("dvdduration"));
                                    string format = reader.GetString(reader.GetOrdinal("dvdformat"));

                                    Dvd dvd = new Dvd(item, director, duration, format);

                                    itemList.Add(dvd);
                                    //dvdList.Add(dvd);
                                }
                                else if (item.Category.ToLower() == "magazine")
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
                        cmd.ExecuteNonQuery();
                    }
                }

                return itemList;
            }
            catch (Exception e)
            {
                return new List<Item>();
            }
        }
    }
}
