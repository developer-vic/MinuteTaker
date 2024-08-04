namespace MinuteTaker.Views;

public partial class AddEditBookPage : ContentPage
{
	public AddEditBookPage()
	{
		InitializeComponent();
        BindingContext = new AddEditBookVM();
	} 
    private class AddEditBookVM : BaseViewModel
    {
        private bool showRecording = true;
        private bool showReading = true;

        public bool ShowRecording { get => showRecording; set { SetProperty(ref showRecording, value); } }
        public bool ShowReading { get => showReading; set { SetProperty(ref showReading, value); } }
        public AddEditBookVM()
        {
            ShowRecording = true; ShowReading = true;
        }
    }
}