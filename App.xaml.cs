using MinuteTaker.Views; 

namespace MinuteTaker
{
    public partial class App : Application
    {
        public App()
        { 
            InitializeComponent(); //37256A 
            MainPage = new WelcomePage();
            //VUtils.GetoPage(new DashboardPage(), true); 
        }
    }
}
