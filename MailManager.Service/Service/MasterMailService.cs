using MailManager.Data;
using MailManager.Service.IService;
using MailManager.Service.UnitOfWork;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using MailKit.Net.Smtp;
//using MimeKit;
using System.Net.Mail;
using System.IO;

namespace MailManager.Service.Service
{
    public class MasterMailService : IMasterMailService
    {
        private readonly IUnitOfWork _uow;

        public MasterMailService()
        {
            _uow = new UnitOfWork.UnitOfWork(new MailManagerEntities());
        }
        public MasterMailService(IUnitOfWork unit)
        {
            _uow = unit;
        }
        public List<MasterMail> GetSentEmails()
        {
            return _uow.Repository<MasterMail>().Table().Where(c=>c.IsSent==true && c.EmailTo != null).ToList();
        }



        public void UpdateEmailStatus(MasterMail mail)
        {
            var existingEmail = _uow.Repository<MasterMail>()
                                .Table()
                                .FirstOrDefault(c => c.Id == mail.Id);
            if (existingEmail != null)
            {
                existingEmail.IsSent = mail.IsSent;
                var indoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                existingEmail.SentOnUTC = TimeZoneInfo.ConvertTimeFromUtc(mail.SentOnUTC.Value, indoTimeZone);
                existingEmail.SentOnUTC = mail.SentOnUTC;
                
                _uow.Repository<MasterMail>().Attach(existingEmail);
                _uow.SaveChanges();
            }
        }

        public void UpdateSubject(MasterMail mail)
        {
            var existingEmail = _uow.Repository<MasterMail>()
                                    .Table()
                                    .FirstOrDefault(c => c.Id == mail.Id);

            if (existingEmail != null)
            {
                existingEmail.Subject = "PDF from PT Dynacast";
                existingEmail.Body = @"This Email is sent automatically from the Mail Blast system of PT. DYNACAST INDONESIA 
                                and no signature is required, DO NOT REPLY TO THIS EMAIL. 
                                Email ini dikirim secara otomatis dari sistem Mail Blast PT. DYNACAST INDONESIA dan tidak memerlukan tanda tangan, 
                                JANGAN MEMBALAS EMAIL INI.";

                _uow.Repository<MasterMail>().Attach(existingEmail); 
                _uow.SaveChanges();
            }
        }


        //public void SendEmails()
        //{
        //    var emailAccount = _uow.Repository<EmailAccount>().Table().FirstOrDefault();
        //    if (emailAccount == null) return;

        //    var unsentEmails = GetUnsentEmails();
        //    if (!unsentEmails.Any()) return;

        //    foreach (var email in unsentEmails)
        //    {
        //        try
        //        {
        //            var message = new MimeMessage();
        //            message.From.Add(new MailboxAddress(emailAccount.DisplayName, emailAccount.Email));
        //            message.To.Add(new MailboxAddress(email.RecipientName, email.EmailTo));
        //            message.Subject = email.Subject;
        //            message.Body = new TextPart("html") { Text = HttpUtility.HtmlDecode(email.Body) };

        //            using (var smtp = new SmtpClient())
        //            {
        //                smtp.Connect(emailAccount.Host, emailAccount.Port.Value, emailAccount.EnableSSL.Value);
        //                smtp.Authenticate(emailAccount.Username, emailAccount.Password);
        //                smtp.Send(message);
        //                smtp.Disconnect(true);
        //            }

        //            email.IsSent = true;
        //            email.SentOnUTC = DateTime.UtcNow;
        //            UpdateEmailStatus(email);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Gagal mengirim email ke {email.EmailTo}: {ex.Message}");
        //        }
        //    }
        //}

        public List<MasterMail> GetAll()
        {
            return _uow.Repository<MasterMail>().Table().ToList();
        }



        //public void SendQueuedEmails()
        //{
        //    try
        //    {
        //        var emails = GetUnsentEmails(); 
        //        if (!emails.Any())
        //        {
        //            Console.WriteLine("No pending emails found.");
        //            return;
        //        }

        //        foreach (var email in emails)
        //        {
        //            try
        //            {
        //                var message = new MailMessage();
        //                var smtpClient = new SmtpClient()
        //                {
        //                    Host = "smtp.gmail.com",
        //                    Port = 587,
        //                    EnableSsl = true,
        //                    DeliveryMethod = SmtpDeliveryMethod.Network,
        //                    UseDefaultCredentials = false,
        //                    Credentials = new NetworkCredential("reminderlicense@gmail.com", "dwzktzxrkmjhjprj")
        //                };

        //                message.From = new MailAddress("reminderlicense@gmail.com", "NoReply");
        //                message.To.Add(email.EmailTo);
        //                message.Subject = email.Subject;
        //                message.Body = HttpUtility.HtmlDecode(email.Body);
        //                message.IsBodyHtml = true;

                        
        //                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                       
                      
        //                DirectoryInfo dir = new DirectoryInfo(baseDir);
        //                while (dir != null && dir.Name != "MailManager.Web")
        //                {
        //                    dir = dir.Parent;
        //                }

        //                if (dir == null)
        //                {
        //                    Console.WriteLine("ERROR: Tidak menemukan folder MailManager.Web!");
        //                    continue;
        //                }

        //                // Path ke FilePDF dan attachment jika ada
        //                if (!string.IsNullOrEmpty(email.FileName))
        //                {
        //                    string pdfFileName = email.FileName + ".pdf";
        //                    string pdfPath = Path.Combine(dir.FullName, "FilePDF", pdfFileName);
        //                    Console.WriteLine($"Final PDF Path: {pdfPath}");

        //                    if (File.Exists(pdfPath))
        //                    {
        //                        message.Attachments.Add(new Attachment(pdfPath));
                                
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"WARNING: File NOT FOUND at: {pdfPath}");
        //                    }
        //                }

        //                smtpClient.Send(message);

                       
        //                email.IsSent = true;
        //                email.SentOnUTC = DateTime.UtcNow;
        //                UpdateEmailStatus(email);

                       
        //            }
        //            catch (Exception ex)
        //            {
        //               if (ex.InnerException != null)
        //                {
        //                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        //                }
                        
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in email processing: {ex.Message}");
        //        throw;
        //    }
        //}

       
        public List<MasterMail> GetReadyReceipent()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(baseDir);

           while (dir != null && dir.Name != "MailManager.Web")
            {
                dir = dir.Parent;
            }

            if (dir == null)
            {
                Console.WriteLine("ERROR: Tidak menemukan folder MailManager.Web!");
                return new List<MasterMail>();
            }

            string pdfFolderPath = Path.Combine(dir.FullName, "FilePDF");

            var allEmails = _uow.Repository<MasterMail>().Table().ToList();

                var readyEmails = allEmails.Where(email =>
                !string.IsNullOrEmpty(email.FileName) &&
                File.Exists(Path.Combine(pdfFolderPath, email.FileName + ".pdf"))
            ).ToList();

            return readyEmails;
        }

        public void Insert(MasterMail obj)
        {
            if(obj==null) throw new ArgumentNullException(nameof(obj));
            _uow.Repository<MasterMail>().Add(obj);
            _uow.SaveChanges();
        }
        public void Update(MasterMail obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            _uow.Repository<MasterMail>().Attach(obj);
            _uow.SaveChanges();
        }
    }

}
