using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MailManager.Web.ViewModel
{
    public class EmailAccountVM
    {
        public int Id { get; set; }
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Host { get; set; }
        [Display(Name = "User Name")]
        public string Username { get; set; }
        public int Port { get; set; }
        [Display(Name = "Enable SSL")]
        public bool EnableSsl { get; set; }
        [Display(Name = "Use Default Credential")]
        public bool UseDefaultCredentials { get; set; }
    }
}