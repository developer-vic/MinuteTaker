namespace MinuteTaker.Views;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
    } 
    void btnLogin_Clicked(System.Object sender, System.EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
    void btnRegister_Clicked(System.Object sender, System.EventArgs e)
    {
       Application.Current.MainPage = new NavigationPage(new RegisterPage());
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Launcher.TryOpenAsync("https://programmergwin.com");
    }
}