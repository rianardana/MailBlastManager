//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MailManager.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailAccount
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Nullable<int> Port { get; set; }
        public Nullable<bool> EnableSSL { get; set; }
        public Nullable<bool> UseDefaultCredentials { get; set; }
    }
}
