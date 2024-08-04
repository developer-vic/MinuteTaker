using CommunityToolkit.Maui.Media;
using System.Globalization;

namespace MinuteTaker
{
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private Locale? CURRENT_LOCALE;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnStartListeningClicked(object sender, EventArgs e)
        {
            var isGranted = await SpeechToText.Default.RequestPermissions(CancellationToken.None);
            if (!isGranted)
            {
                VUtils.ToastText("Permission not granted");
                return;
            }
            if (_cancellationTokenSource != null) return;
            _cancellationTokenSource = new CancellationTokenSource();
            await StartListeningAsync();
        }
        private async Task StartListeningAsync()
        {
            try
            {
                // outputLabel.Text = "Listening...";
                if (_cancellationTokenSource == null)
                    _cancellationTokenSource = new CancellationTokenSource();
                var recognitionResult = await SpeechToText.Default.ListenAsync(CultureInfo.GetCultureInfo("en-NG"),
                    new Progress<string>(partialText =>
                    {
                        outputLabel.Text += partialText + " ";
                    }), _cancellationTokenSource.Token);
                if (recognitionResult.IsSuccessful)
                {
                    outputLabel.Text = recognitionResult.Text;
                    await StartListeningAsync();
                }
            }
            catch (Exception)
            {
                VUtils.ToastText("error listening to voice");
            }
        }
        private async void OnStartReadingClicked(object sender, EventArgs e)
        {
            try
            {  
                if (CURRENT_LOCALE == null)
                {
                    var locales = await TextToSpeech.Default.GetLocalesAsync();
                    if (locales != null && locales.Count() > 0) CURRENT_LOCALE = locales.FirstOrDefault();
                } 
                if (string.IsNullOrEmpty(inputEntry.Text)) VUtils.ToastText("Enter text to read"); 
                else await VUtils.TryReadAloud(inputEntry.Text, CURRENT_LOCALE, _cancellationTokenSource);
            }
            catch (Exception ex)
            {
                VUtils.ToastText("StartRead Error: " + ex.Message);
            }
        }
        private void OnStopListenReadClicked(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }


    }
}
