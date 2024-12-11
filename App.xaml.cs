using OOP2FinalProjectLibrary.Data;
namespace OOP2FinalProjectLibrary
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            DBHandler.InitializeDatabase();
        }
    }
}
