using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MailManager.Web.ViewModel
{
    public class MasterMailVM
    {
        public int Id { get; set; }
        [Display(Name ="No Badge")]
        public string NoBadge { get; set; }
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        [Display(Name = "Employee Name")]
        public string RecipientName { get; set; }
        [Display(Name = "Email")]
        public string EmailTo { get; set; }      
        public string Subject { get; set; }      
        public string Body { get; set; }          
        public bool IsSent { get; set; }
        public bool IsEncrypted { get; set; }
        public DateTime? SentOnUTC { get; set; }
        public DateTime DateofBirth { get; set; }
    }
}