using CommunityToolkit.Maui.Media;
using MinuteTaker.Controls;
using Plugin.Maui.Audio;
using System.Globalization;
using System.Windows.Input;

namespace MinuteTaker.Views;

public partial class BookDetailsPage : ContentPage
{
    AddEditBookVM vM;
    public BookDetailsPage(BookModel value)
    {
        InitializeComponent();
        vM = new AddEditBookVM(value);
        BindingContext = new AddEditBookVM(value);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        vM.StopReading();
    }
    private class AddEditBookVM : BaseViewModel
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private Locale? CURRENT_LOCALE;
        private IAudioPlayer? _audioPlayer;

        private bool showReading;
        private bool showRead = true;
        private string imgReading = "read.png";
        private string meetingContent = "";
        private string meetingTitle = "";
        BookModel _DetailBook;

        public bool ShowReading { get => showReading; set { SetProperty(ref showReading, value); } }
        public bool ShowRead { get => showRead; set { SetProperty(ref showRead, value); } }
        public string ImgReading { get => imgReading; set { SetProperty(ref imgReading, value); } }
        public string MeetingContent { get => meetingContent; set { SetProperty(ref meetingContent, value); } }
        public string MeetingTitle { get => meetingTitle; set { SetProperty(ref meetingTitle, value); } } 
        public bool IsWriter { get; set; } = VUtils.LoggedInUser?.UserType == "Writer";

        public ICommand? MyCommand { get; private set; }

        public AddEditBookVM(BookModel value)
        {
            _DetailBook = value;
            MeetingContent = value.content;
            MeetingTitle = value.title;
            Initialize(); //others fields too TODO
            UpdateRead(value);
        }

        private async void UpdateRead(Controls.BookModel value)
        { 
            if(!value.readUsers.Contains(VUtils.LoggedInUser?.UserId))
            {
                value.readUsers.Add(VUtils.LoggedInUser?.UserId);
                await VUtils.AddUpdateBook(value, false);
            }
        }

        private void Initialize()
        {
            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "read":
                        OnReadClick();
                        break;
                    case "edit":
                        VUtils.GetoPage(new AddEditBookPage(_DetailBook));
                        break;
                }
            });
        }

        private void OnReadClick()
        {
            ShowReading = !ShowReading;
            ShowRead = !ShowReading;
            if (ShowReading) StarReading();
            else StopReading();
        }
        private async void StarReading()
        {
            try
            {
                ImgReading = "reading.gif";
                bool canReadOnline = await StartReadingLineByLine();
                if (!canReadOnline)
                {
                    if (CURRENT_LOCALE == null)
                    {
                        var locales = await TextToSpeech.Default.GetLocalesAsync();
                        if (locales != null && locales.Count() > 0)
                            CURRENT_LOCALE = locales.FirstOrDefault();
                    }
                    var settings = new SpeechOptions()
                    {
                        Volume = .75f,
                        Pitch = 1.0f,
                        Locale = CURRENT_LOCALE
                    };
                    _cancellationTokenSource = new CancellationTokenSource();
                    await TextToSpeech.Default.SpeakAsync(MeetingContent, settings, _cancellationTokenSource?.Token ?? default);
                }
                ImgReading = "read.png";
            }
            catch (Exception ex)
            {
                VUtils.ToastText("Reading Error: " + ex.Message);
                OnReadClick(); //only call when thiere is error
            }
        }

        private Task<bool> StartReadingLineByLine()
        {
            return Task.FromResult(false);
            //string audioUrl = await VUtils.GetApiAudio("testing");
            //if (!string.IsNullOrEmpty(audioUrl))
            //{
            //    var contentArray = meetingContent.Split(" ");
            //    foreach(var content in contentArray)
            //    {
            //        audioUrl = await VUtils.GetApiAudio(content);
            //        _audioPlayer = await CreateAudioPlayerFromUrlAsync(audioUrl);
            //        if (_audioPlayer != null)
            //        {
            //            _audioPlayer.Play(); Thread.Sleep((int)(_audioPlayer.Duration * 1000));
            //        }
            //    }
            //    return true;
            //}
            //else return false;
        }

        private async Task<IAudioPlayer?> CreateAudioPlayerFromUrlAsync(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var audioPlayer = AudioManager.Current.CreatePlayer(stream);
                    return audioPlayer;
                }
            }
            catch (Exception)
            { 
            }
            return null;
        }

        internal void StopReading()
        {
            try
            {
                ImgReading = "read.png";

                if (_audioPlayer != null && _audioPlayer.IsPlaying)
                {
                    _audioPlayer.Stop();
                }

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = null;
            }
            catch (Exception)
            {
            }
        }
    }
}