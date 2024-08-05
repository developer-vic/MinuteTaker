using MinuteTaker.Controls;
using static MinuteTaker.Views.DashboardPage;

namespace MinuteTaker.Views;

public partial class BookListPage : ContentPage
{ 
    DashboardVM vM;
    public BookListPage()
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
        colBookListFull.SelectedItem = (sender as HorizontalStackLayout)?.BindingContext;
        colBookListFull.SelectedItem = null;
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
}