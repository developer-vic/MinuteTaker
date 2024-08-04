using MinuteTaker.Controls;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Windows.Input;

namespace MinuteTaker.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
		BindingContext = new DashboardVM();
    }
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    { 
        colBookList.SelectedItem = (sender as Frame)?.BindingContext;
        colBookList.SelectedItem = null;
    }

    private class DashboardVM : BaseViewModel
    {
        private ObservableCollection<BookModel> bookList = new ObservableCollection<BookModel>();
        private BookModel? selectedBook;

        public ObservableCollection<BookModel> BookList { get => bookList; set { SetProperty(ref bookList, value); } }
        public BookModel? SelectedBook { get => selectedBook; set { SetProperty(ref selectedBook, value); } }

        public ICommand? MyCommand { get; set; }

        public DashboardVM()
        {
            BookList = new ObservableCollection<BookModel>()
            {
                new BookModel()
                {
                    authorId = "test", authorName = "Vic", bookId = VUtils.GetTransactionRef(),
                    content = "messagess", dateTime = DateTime.Now, organization = "Home", 
                    title = "Test 1", isRead = true, isNotRead =false, isAdmin = true, isNotAdmin = false,
                },
                new BookModel()
                {
                    authorId = "test", authorName = "Vic 2", bookId = VUtils.GetTransactionRef(),
                    content = "messagess 2", dateTime = DateTime.Now, organization = "Home 2",
                    title = "Test 2", isRead = false, isNotRead=true, isAdmin = false, isNotAdmin = true,
                }
            };
            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "add":
                        VUtils.GetoPage(new AddEditBookPage());
                        break;
                }
            });
        }
    }

}