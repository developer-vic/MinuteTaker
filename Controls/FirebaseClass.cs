using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace MinuteTaker.Controls
{
    public class FirebaseClass
    {
        private readonly FirebaseClient _firebaseClient;
        private const string MATRIC_NO = "CS-HND-F22-3320";

        public FirebaseClass()
        {
            _firebaseClient = new FirebaseClient("https://fedpoffacs-default-rtdb.firebaseio.com/");
        }
        internal async Task<string> GetResponse(string actionname, string key, string value)
        { 
            try
            { 
                if (actionname == "/noteTakerGet")
                {
                    if(key== "savedUserList")
                    { 
                        var users = await _firebaseClient.Child(MATRIC_NO)
                            .Child("users").OnceAsync<UserModel>(); 
                        var listRes = users.Select(item => item.Object).ToList();
                        return JsonConvert.SerializeObject(listRes);
                    }
                    else if (key == "savedBookList")
                    {
                        var meetings = await _firebaseClient.Child(MATRIC_NO)
                            .Child("meetings").OnceAsync<BookModel>();
                        var listRes = meetings.Select(item => item.Object).ToList();
                        return JsonConvert.SerializeObject(listRes);
                    }
                }
                else if (actionname == "/noteTakerSet")
                {
                    if (key == "savedUserList")
                    {
                        var userListModel = JsonConvert.DeserializeObject<List<UserModel>>(value);
                        if (userListModel != null)
                        {
                            await _firebaseClient.Child(MATRIC_NO).Child("users").DeleteAsync();
                            foreach (var user in userListModel)
                            { 
                                await _firebaseClient.Child(MATRIC_NO).Child("users").Child(user.UserId).PutAsync(user);
                            }
                        }
                        return "true";
                    }
                    else if (key == "savedBookList")
                    {
                        var userListModel = JsonConvert.DeserializeObject<List<BookModel>>(value);
                        if (userListModel != null)
                        {
                            await _firebaseClient.Child(MATRIC_NO).Child("meetings").DeleteAsync();
                            foreach (var user in userListModel)
                            {
                                await _firebaseClient.Child(MATRIC_NO).Child("meetings").Child(user.bookId).PutAsync(user);
                            }
                        } 
                        return "true";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }
         
         
    }
}
