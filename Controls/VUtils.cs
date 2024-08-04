using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Text.RegularExpressions; 

namespace MinuteTaker
{
    public class VUtils
    {
        ////To Save data inside shared preference
        //internal static async void SaveSharedPreference<T>(string key, T value)
        //{
        //    Application.Current.Properties[key] = value;
        //    await Application.Current.SavePropertiesAsync();
        //}

        ////To retrieve user saved data
        //internal static T LoadSharedPreference<T>(string key)
        //{
        //    return (T)Application.Current.Properties[key];
        //}

        ////To check if key available
        //internal static bool ContainsKey(string key)
        //{
        //    return Application.Current.Properties.ContainsKey(key);
        //}

        ////To clear user value with key
        //internal static bool ClearValueUsingKey(string key)
        //{
        //    return Application.Current.Properties.Remove(key);
        //}

        ////To clear user saved files
        //internal static void ClearAllSharedPreferences()
        //{
        //    Application.Current.Properties.Clear();
        //}

        ////To logout
        //internal static void LogOutUser()
        //{
        //    VUtils.SaveSharedPreference<bool>(VConstants.USERTYPE_LOGIN_DB_KEY, false);
        //    if(Application.Current.MainPage != null)
        //    Application.Current.MainPage = new WelcomePage();
        //}

        internal async static Task TryReadAloud(string book_content, Locale? locale, CancellationTokenSource? cts)
        {
            try
            { 
                var settings = new SpeechOptions()
                {
                    Volume = .75f,
                    Pitch = 1.0f,
                    Locale = locale
                };  
                await TextToSpeech.SpeakAsync(book_content, settings, cts?.Token ?? default); 
                //await TextToSpeech.Default.SpeakAsync(inputEntry.Text, new()
                //{
                //    Locale = CURRENT_LOCALE,
                //    Pitch = 1,
                //    Volume = 1
                //}, _cancellationTokenSource.Token); 
            }
            catch (Exception ex)
            {
                ToastText("ReadAloud Error: " + ex.Message);
            }
        }

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
            if(Application.Current != null)
            {
                if(isMainPage)
                    Application.Current.MainPage = new NavigationPage(contentPage);
                else if(Application.Current.MainPage != null)
                    Application.Current.MainPage.Navigation.PushAsync(contentPage);
            }
        }
        internal static void GoBack()
        {
            if (Application.Current?.MainPage != null) 
                Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
