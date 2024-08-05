using MinuteTaker.Controls;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Windows.Input;

namespace MinuteTaker.Views;

public partial class DashboardPage : ContentPage
{
    DashboardVM vM;
    public DashboardPage()
    {
        InitializeComponent();
        vM = new DashboardVM();
        BindingContext = vM;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        vM.ShowLoading = true;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        colBookList.SelectedItem = (sender as HorizontalStackLayout)?.BindingContext;
        colBookList.SelectedItem = null;
    }
    private void EditImageButton_Clicked(object sender, EventArgs e)
    {
        BookModel? SelectedItem = (sender as ImageButton)?.BindingContext as BookModel;
        VUtils.GetoPage(new AddEditBookPage(SelectedItem));
    }
    private void DeleteImageButton_Clicked(object sender, EventArgs e)
    {
        BookModel? SelectedItem = (sender as ImageButton)?.BindingContext as BookModel;
        vM.DeleteBook(SelectedItem);
    }

    internal class DashboardVM : BaseViewModel
    {
        private List<BookModel> FullBookList = new List<BookModel>();
        private ObservableCollection<BookModel> bookList = new ObservableCollection<BookModel>();
        private BookModel? selectedBook;
        private int meetingsViewed;
        private int meetingsCreated;
        private int meetingsAvailable;
        private bool showLoading;
        private string searchTitle = "";

        public ICommand? MyCommand { get; set; }
        public string? UserFullName { get; set; } = VUtils.LoggedInUser?.FullName;
        public string? UserEmail { get; set; } = VUtils.LoggedInUser?.Email;
        public string? UserType { get; set; } = VUtils.LoggedInUser?.UserType;
        public bool IsWriter { get; set; } = VUtils.LoggedInUser?.UserType == "Writer";
        public bool IsNotWriter { get; set; } = VUtils.LoggedInUser?.UserType != "Writer";
        public int MeetingsViewed { get => meetingsViewed; set { SetProperty(ref meetingsViewed, value); } }
        public int MeetingsCreated { get => meetingsCreated; set { SetProperty(ref meetingsCreated, value); } }
        public int MeetingsAvailable { get => meetingsAvailable; set { SetProperty(ref meetingsAvailable, value); } }
        public ObservableCollection<BookModel> BookList { get => bookList; set { SetProperty(ref bookList, value); } }
        public BookModel? SelectedBook { get => selectedBook; set { SetProperty(ref selectedBook, value); OnBookSelected(value); } }
        private void OnBookSelected(BookModel? value) { if (value != null) VUtils.GetoPage(new BookDetailsPage(value)); }
        public bool ShowLoading { get => showLoading; set { SetProperty(ref showLoading, value); } }
        public string SearchTitle { get => searchTitle; set { SetProperty(ref searchTitle, value); OnSearchTitle(value); } }

        private void OnSearchTitle(string value)
        {
            if (!string.IsNullOrEmpty(value))
                BookList = new ObservableCollection<BookModel>(FullBookList.Where(p => p.title.ToLower().Contains(searchTitle.ToLower()))); 
            else BookList = new ObservableCollection<BookModel>(FullBookList);
        }

        public DashboardVM()
        {
            RunCommands();
        }

        private async void InitializeData()
        {
            BookList = new ObservableCollection<BookModel>();
            FullBookList = await VUtils.GetBookList();
            foreach (var book in FullBookList)
            {
                book.isRead = book.readUsers.Contains(VUtils.LoggedInUser?.UserId);
                book.isNotRead = !book.readUsers.Contains(VUtils.LoggedInUser?.UserId);
                book.isAdmin = book.authorId == VUtils.LoggedInUser?.UserId;
                book.isNotAdmin = book.authorId != VUtils.LoggedInUser?.UserId;
            } 
            MeetingsViewed = FullBookList.Where(p => p.isRead).Count();
            MeetingsCreated = MeetingsAvailable = FullBookList.Count();
            BookList = new ObservableCollection<BookModel>(FullBookList);
            ShowLoading = false;
        }

        private void RunCommands()
        {
            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "logout":
                        TryLogOut();
                        break;
                    case "reload":
                        InitializeData();
                        break;
                    case "add":
                        VUtils.GetoPage(new AddEditBookPage(null));
                        break;
                    case "history":
                        VUtils.GetoPage(new BookListPage());
                        break;
                    case "gwin":
                        Launcher.TryOpenAsync("https://programmergwin.com");
                        break;
                }
            });
        }

        private async void TryLogOut()
        {
            if (Application.Current?.MainPage == null) return;
            bool canLogout = await Application.Current.MainPage.DisplayAlert("Log Out Confirmation", "Are you sure you want to log out?", "YES, LOG OUT", "NO, CLOSE");
            if (canLogout) VUtils.LogOut();
        }

        internal async void DeleteBook(BookModel? delBook)
        {
            if (delBook == null || Application.Current?.MainPage == null) return;
            bool canDel = await Application.Current.MainPage.DisplayAlert("Delete Confirmation", "Are you sure you want to delete?", "YES, DELETE", "NO, CLOSE");
            if (canDel)
            {
                await VUtils.DeleteBook(delBook);
                ShowLoading = true;
            }
        }
    }

}