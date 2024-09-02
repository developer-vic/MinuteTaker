using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using MinuteTaker.Controls;
using MinuteTaker.Views;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Security;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MinuteTaker
{
    public class VUtils
    {
        internal static async void ToastText(string text)
        {
            await Toast.Make(text, ToastDuration.Short).Show();
        }

        public static string GetTransactionRef()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmssfff");
        }

        private static Regex _regex = new Regex(
 @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
 RegexOptions.CultureInvariant | RegexOptions.Singleline);

        internal static bool IsValidEmailFormat(string emailInput)
        {
            return _regex.IsMatch(emailInput);
        }

        internal async static void CopyTextToClipBoard(string book_decription)
        {
            try
            {
                await Clipboard.SetTextAsync(book_decription);
                ToastText("copied");
            }
            catch (Exception ex)
            {
                ToastText(ex.Message);
            }
        }

        internal static void GetoPage(Page contentPage, bool isMainPage = false)
        {
            if (Application.Current != null)
            {
                if (isMainPage)
                    Application.Current.MainPage = new NavigationPage(contentPage);
                else if (Application.Current.MainPage != null)
                    Application.Current.MainPage.Navigation.PushAsync(contentPage);
            }
        }
        internal static void GoBack()
        {
            if (Application.Current?.MainPage != null)
                Application.Current.MainPage.Navigation.PopAsync();
        }
        internal static void LogOut()
        {

            if (Application.Current?.MainPage != null)
                Application.Current.MainPage = new NavigationPage(new WelcomePage());
            LoggedInUser = null;
        }
        private static void ShowMessage(string message)
        {
            if (Application.Current?.MainPage != null)
                Application.Current.MainPage.DisplayAlert("Alert", message, "OK");
        }

        internal static async Task<string> GetApiAudio(string meetingContent)
        {
            try
            {
                var client = new HttpClient(); var request = new HttpRequestMessage(HttpMethod.Post, "https://play.ht/api/transcribe");
                var content = new StringContent("{\"userId\":\"public-access\",\"platform\":\"landing_demo\",\"ssml\":\"<speak><p>" + meetingContent + "</p></speak>\",\"voice\":\"en-NG-EzinneNeural\",\"narrationStyle\":\"Neural\",\"method\":\"file\"}", null, "application/json");
                request.Content = content; var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); string resMraw = await response.Content.ReadAsStringAsync();
                FileModel? fileModel = JsonConvert.DeserializeObject<FileModel>(resMraw);
                return fileModel?.file ?? "";
            }
            catch (Exception)
            {
            }
            return "";
        }

        //DATABASE 
        internal static UserModel? LoggedInUser;
        private static HttpClient GetVHttpClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                {
                    return true;
                }
                var certificateValidator = new SelfSignedCertificateValidator();
                bool isValid = false;
                if (certificate != null)
                    isValid = certificateValidator.ValidateCertificate(certificate.GetRawCertData(), "https://programmergwin.com");
                return isValid;
            };
            var mclient = new HttpClient(httpClientHandler);
            return mclient;
        }
        private async static Task<string> PostRequest(string actionname, string key, string value)
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet) return "";
                using (var mclient = GetVHttpClient())
                {
                    string url = "https://programmergwin.com" + actionname;
                    if (!string.IsNullOrEmpty(key)) url += "?key=" + key;
                    if (!string.IsNullOrEmpty(value)) url += "&value=" + value;

                    mclient.Timeout = TimeSpan.FromMinutes(1);
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                    {
                        var response = await mclient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                            return await response.Content.ReadAsStringAsync();
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            return "";
                    }
                }
            }
            catch (Exception) { }
            return "";
        }
        internal static async Task<List<string>> GetAllOrganizations()
        {
            List<string> AllOrgs = new List<string>();
            try
            {
                List<UserModel>? UserList = new List<UserModel>();
                string savedUserList = await PostRequest("/noteTakerGet", "savedUserList", "");
                if (!string.IsNullOrEmpty(savedUserList))
                    UserList = JsonConvert.DeserializeObject<List<UserModel>>(savedUserList);
                if (UserList != null && UserList.Count != 0)
                    AllOrgs = UserList.DistinctBy(p => p.Organization).Select(s => s.Organization).ToList();
            }
            catch (Exception)
            {
            }
            return AllOrgs;
        }
        internal static async Task<bool> RegisterUser(UserModel newUser)
        {
            try
            {
                List<UserModel>? UserList = new List<UserModel>();
                string savedUserList = await PostRequest("/noteTakerGet", "savedUserList", "");
                if (!string.IsNullOrEmpty(savedUserList))
                    UserList = JsonConvert.DeserializeObject<List<UserModel>>(savedUserList);
                if (UserList == null) UserList = new List<UserModel>();
                if (UserList.Where(p => p.Email == newUser.Email).Count() > 0)
                {
                    ShowMessage("Email Already Exist"); return false;
                }
                UserList.Add(newUser); string newMrawList = JsonConvert.SerializeObject(UserList);
                await PostRequest("/noteTakerSet", "savedUserList", newMrawList);
                LoggedInUser = newUser; ShowMessage("Registration Successful");
                return true;
            }
            catch (Exception)
            {
                ShowMessage("Registration Failed"); return false;
            }
        }
        internal static async Task<bool> LoginUser(string email, string password)
        {
            try
            {
                List<UserModel>? UserList = new List<UserModel>(); LoggedInUser = null;
                string savedUserList = await PostRequest("/noteTakerGet", "savedUserList", "");
                if (!string.IsNullOrEmpty(savedUserList))
                    UserList = JsonConvert.DeserializeObject<List<UserModel>>(savedUserList);
                if (UserList != null && UserList.Count != 0)
                    LoggedInUser = UserList.Where(p => p.Email == email && p.Password == password).FirstOrDefault();
                //if (LoggedInUser != null)
                //{
                //    ShowMessage("Login Successful"); return true;
                //} 
                return LoggedInUser != null;
            }
            catch (Exception)
            {
            }
            ShowMessage("Incorrect Email or Password"); return false;
        }

        internal static async Task<bool> AddUpdateBook(BookModel newBook, bool showMsg = true)
        {
            try
            {
                if (AllBookList == null) return false;
                var existBook = AllBookList.Where(p => p.bookId == newBook.bookId).FirstOrDefault();
                if (existBook != null)
                {
                    AllBookList.Remove(existBook);
                    existBook.content = newBook.content; existBook.title = newBook.title;
                    existBook.readUsers = newBook.readUsers; 
                    existBook.dateTime = DateTime.Now.ToString("MMM dd, yyyy - hh:mm tt");
                }
                else existBook = newBook; AllBookList.Add(existBook);

                string mraw = JsonConvert.SerializeObject(AllBookList);
                await PostRequest("/noteTakerSet", "savedBookList", mraw);
                if (showMsg) ShowMessage("Meeting Updated Successfully");
                return true;
            }
            catch (Exception)
            {
                ShowMessage("Error Updating Meeting"); return false;
            }
        }
        static List<BookModel>? AllBookList = new List<BookModel>();
        internal static async Task<List<BookModel>> GetBookList()
        {
            try
            {
                string savedBookList = await PostRequest("/noteTakerGet", "savedBookList", "");
                if (!string.IsNullOrEmpty(savedBookList))
                    AllBookList = JsonConvert.DeserializeObject<List<BookModel>>(savedBookList);
                if (AllBookList == null) AllBookList = new List<BookModel>(); 
                return AllBookList.Where(p => p.organization == LoggedInUser?.Organization).ToList();
            }
            catch (Exception)
            {
                return new List<BookModel>();
            }
        }
        internal static async Task<bool> DeleteBook(BookModel delBook)
        {
            try
            {
                if (AllBookList == null) return false;
                var existBook = AllBookList.Where(p => p.bookId == delBook.bookId).FirstOrDefault();
                if (existBook != null) AllBookList.Remove(existBook);
                else { ShowMessage("Meeting Not Found"); return false; }

                string mraw = JsonConvert.SerializeObject(AllBookList);
                await PostRequest("/noteTakerSet", "savedBookList", mraw);
                ShowMessage("Meeting Deleted Successfully"); return true;
            }
            catch (Exception)
            {
                ShowMessage("Error Deleting Meeting"); return false;
            }
        }

    }
    class FileModel
    {
        public string file { get; set; } = "";
    }
    public class SelfSignedCertificateValidator : ICertificateValidator
    {
        public bool ValidateCertificate(byte[] certificateData, string host)
        {
            // Add logic here to validate the certificate.
            // For development, we'll accept any certificate, but in production,
            // you should implement proper validation logic.

            // For example, you can check the certificate's thumbprint or issuer.
            // Here, we're accepting any certificate.
            return true;
        }
    }

    public interface ICertificateValidator
    {
        bool ValidateCertificate(byte[] certificateData, string host);
    }

}
