using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailManager.Web.ViewModel
{
    public class MasterMailVM
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string RecipientName { get; set; } 
        public string EmailTo { get; set; }      
        public string Subject { get; set; }      
        public string Body { get; set; }          
        public bool IsSent { get; set; }        
        public DateTime? SentOnUTC { get; set; }
    }
}