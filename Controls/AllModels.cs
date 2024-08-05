using MinuteTaker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinuteTaker.Controls
{
    class AllModels
    {
    }
    public class UserModel
    {
        public string UserId { get; set; } = "";
        public string UserType { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";
        public string Organization { get; set; } = "";
        public bool isAdmin { get=> UserType=="Writer"; }
    }
    public class BookModel
    {
        public string bookId { get; set; } = "";
        public string dateTime { get; set; } = "";
        public string title { get; set; } = "";
        public string content { get; set; } = "";
        public string authorId { get; set; } = "";
        public string authorName { get; set; } = "";
        public string organization { get; set; } = "";
        public List<string?> readUsers { get; set; } = [];

        public bool isRead { get; set; }
        public bool isNotRead { get; set; }
        public bool isAdmin { get; set; }
        public bool isNotAdmin { get; set; }
    }
}
