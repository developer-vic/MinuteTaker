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
    public class BookModel
    { 
        public string bookId { get; set; }
        public DateTime dateTime { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string organization { get; set; }
        public bool isRead { get; set; }
        public bool isNotRead { get; set; }
        public bool isAdmin { get; set; }
        public bool isNotAdmin { get; set; }
    }
}
