using CommunityToolkit.Maui.Media;
using System.Globalization;
using System.Threading;
using System.Windows.Input;

namespace MinuteTaker.Views;

public partial class AddEditBookPage : ContentPage
{
    public AddEditBookPage(Controls.BookModel? updatingBook)
    {
        InitializeComponent();
        BindingContext = new AddEditBookVM(updatingBook);
    }
    private class AddEditBookVM : BaseViewModel
    {
        private CancellationTokenSource? _cancellationTokenSource;
        Controls.BookModel? _UpdatingBook = null;

        private bool showRecording;
        private bool showRecord = true;
        private string imgRecording = "recording_stop.png";
        private string meetingContent = "";
        private string meetingTitle = "";
        private bool showLoading;

        public bool ShowRecording { get => showRecording; set { SetProperty(ref showRecording, value); } }
        public bool ShowRecord { get => showRecord; set { SetProperty(ref showRecord, value); } }
        public string ImgRecording { get => imgRecording; set { SetProperty(ref imgRecording, value); } }
        public string MeetingContent { get => meetingContent; set { SetProperty(ref meetingContent, value); } }
        public string MeetingTitle { get => meetingTitle; set { SetProperty(ref meetingTitle, value); } }
        public bool ShowLoading { get => showLoading; set { SetProperty(ref showLoading, value); } }

        public ICommand? MyCommand { get; private set; }

        public AddEditBookVM(Controls.BookModel? updatingBook)
        {
            Initialize(); 
            if(updatingBook != null)
            {
                MeetingContent = updatingBook.content;
                MeetingTitle = updatingBook.title;
                _UpdatingBook = updatingBook;
            }
        }

        private void Initialize()
        {
            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "record":
                        OnRecordClick();
                        break;
                    case "save":
                        TrySaveUpdate();
                        break;
                }
            });
            RequestPermission();
        }

        private async void TrySaveUpdate()
        {
            ShowLoading = true;
            if (_UpdatingBook != null)
            {
                _UpdatingBook.title = MeetingTitle;
                _UpdatingBook.content = MeetingContent;
                //used other fields as it is
            }
            else
            {
                var user = VUtils.LoggedInUser;
                _UpdatingBook = new Controls.BookModel()
                {
                    title = MeetingTitle,
                    content = MeetingContent,
                    authorId = user?.UserId ?? "",
                    authorName = user?.FullName ?? "",
                    organization = user?.Organization ?? "",
                    bookId = VUtils.GetTransactionRef(),
                    dateTime = DateTime.Now.ToString("MMM dd, yyyy - hh:mm tt"),
                    readUsers = new List<string?>()
                };
            }
            bool saveSuccess = await VUtils.AddUpdateBook(_UpdatingBook);
            if (saveSuccess) VUtils.GetoPage(new DashboardPage(), true);
            ShowLoading = false;
        }

        private async void RequestPermission()
        {
            var isGranted = await SpeechToText.Default.RequestPermissions(CancellationToken.None);
            if (!isGranted)
            {
                VUtils.ToastText("Permission not granted");
                return;
            }
        } 
        private void OnRecordClick()
        {
            ShowRecording = !ShowRecording;
            ShowRecord = !ShowRecording;
            if (ShowRecording) StartListening();
            else StopListening();
        } 
        private async void StartListening()
        {
            RequestPermission();
            if (_cancellationTokenSource != null) return;
            _cancellationTokenSource = new CancellationTokenSource();
            await StartListeningAsync();
        }
        private async Task StartListeningAsync()
        {
            try
            {
                if (_cancellationTokenSource == null)
                    _cancellationTokenSource = new CancellationTokenSource();
                var recognitionResult = await SpeechToText.Default.ListenAsync(CultureInfo.GetCultureInfo("en-NG"),
                    new Progress<string>(partialText =>
                    {
                        ImgRecording = "recording.gif"; 
                    }), _cancellationTokenSource.Token);
                if (recognitionResult.IsSuccessful)
                {
                    MeetingContent += recognitionResult.Text + " ";
                    ImgRecording = "recording_stop.png";
                }
                await StartListeningAsync();
            }
            catch (Exception)
            {
                VUtils.ToastText("error listening to voice");
                OnRecordClick();
            }
        }
        private void StopListening()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = null;
            }
            catch (Exception)
            { 
            }
        }
    }
}