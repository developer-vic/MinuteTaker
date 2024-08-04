using System.Windows.Input;

namespace MinuteTaker.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
		BindingContext = new RegisterVM();
    }
    class RegisterVM : BaseViewModel
    {
        private bool passwordInputType = true;
        private string passwordToggleImage = "eye";
        private string selectedAccType = "";
        private bool showReaderView = false;
        private bool showWriterView = true;
        private List<string> organizationList = new List<string>();

        public List<string> AccTypeList { get; set; }
        public string SelectedAccType { get => selectedAccType; set { SetProperty(ref selectedAccType, value); OnSelectedAccType(value); } }
        private void OnSelectedAccType(string value)
        {
            if (ShowReaderView) { ShowReaderView = false; ShowWriterView = true; LoadOrganizationList(); }
            else if (ShowWriterView) { ShowWriterView = false; ShowReaderView = true; }
        }
        private void LoadOrganizationList()
        {
            if (OrganizationList.Count == 0)
                OrganizationList = new List<string>() { "Load Saved Orgs" };
            Organization = OrganizationList[0];
        }

        public bool ShowReaderView { get => showReaderView; set { SetProperty(ref showReaderView, value); } }
        public bool ShowWriterView { get => showWriterView; set { SetProperty(ref showWriterView, value); } }

        public string? UserFullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public bool PasswordInputType { get => passwordInputType; set { SetProperty(ref passwordInputType, value); } }
        public string PasswordToggleImage { get => passwordToggleImage; set { SetProperty(ref passwordToggleImage, value); } }
        public string? Organization { get; set; }
        public List<string> OrganizationList { get => organizationList; set => organizationList = value; }

        public ICommand? MyCommand { get; protected set; }
        public ICommand? PickerCommand { get; protected set; }

        public RegisterVM()
        {
            AccTypeList = new List<string>() { "Writer", "Reader" };
            SelectedAccType = "Writer"; InitializeCommand();
        }

        private void InitializeCommand()
        {
            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "sign_up":
                        ValidateContiueClicked();
                        break;
                    case "sign_in":
                        VUtils.GetoPage(new LoginPage(), true);
                        break;
                    case "toggle_pw":
                        ShowHidePassword();
                        break;
                }
            });
            PickerCommand = new Command<Picker>((Picker picker) =>
            {
                picker.Focus();
            });
        }

        private void ShowHidePassword()
        {
            PasswordInputType = !PasswordInputType;
            PasswordToggleImage = PasswordInputType ? "eye" : "eyeclosed";
        }


        private void ValidateContiueClicked()
        {
            if (string.IsNullOrEmpty(EmailAddress) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(UserFullName)
                 || string.IsNullOrEmpty(SelectedAccType) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Organization))
                VUtils.ToastText("Pls fill in all fields");
            else if (!VUtils.IsValidEmailFormat(EmailAddress))
                VUtils.ToastText("Incorrect Email Format");
            else
            {
                VUtils.GetoPage(new DashboardPage(), true);
            }
        }

    }
}