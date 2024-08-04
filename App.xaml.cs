using MinuteTaker.Views; 

namespace MinuteTaker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //MainPage = new Views.WelcomePage(); 37256A
            VUtils.GetoPage(new DashboardPage(), true);
        }
    }
}
