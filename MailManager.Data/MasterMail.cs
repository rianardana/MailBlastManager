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
    
    public partial class MasterMail
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string RecipientName { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Nullable<bool> IsSent { get; set; }
        public Nullable<System.DateTime> SentOnUTC { get; set; }
        public bool IsEncrypted { get; set; }
        public System.DateTime DateofBirth { get; set; }
    }
}
