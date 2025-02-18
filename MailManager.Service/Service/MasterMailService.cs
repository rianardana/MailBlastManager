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
using iTextSharp.text.pdf;

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

        public static class EmailTemplates
        {
            public static string GetPdfEmailTemplate()
            {
                return @"
            <html>
            <head>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                    }
                    .header {
                        background-color: #f8f9fa;
                        padding: 15px;
                        border-radius: 5px;
                        margin-bottom: 20px;
                    }
                    .warning {
                        color: #dc3545;
                        font-weight: bold;
                        margin: 15px 0;
                    }
                    .section {
                        margin: 20px 0;
                        padding: 15px;
                        border-left: 4px solid #007bff;
                        background-color: #f8f9fa;
                    }
                    .example {
                        background-color: #e9ecef;
                        padding: 10px;
                        border-radius: 5px;
                        margin: 10px 0;
                    }
                    .divider {
                        border-top: 1px solid #dee2e6;
                        margin: 20px 0;
                    }
                </style>
            </head>
            <body>
                <div class='header'>
                    <h2>Bukti Pemotongan PPh Pasal 21</h2>
                </div>

                <!-- English Section -->
                <div class='section'>
                    <p>This Email is sent automatically from the Mail Blast system of PT. DYNACAST INDONESIA.</p>
                    <p class='warning'>DO NOT REPLY TO THIS EMAIL.</p>
                    <p>Your PDF password is ID@Date of Birth:</p>
                    
                    <div class='example'>
                        <strong>Example:</strong><br>
                        ID: AN190001<br>
                        Date of Birth: May 25, 1990<br>
                        Password: AN190001@25051990
                    </div>
                </div>

                <div class='divider'></div>

                <!-- Indonesian Section -->
                <div class='section'>
                    <p>Email ini dikirim secara otomatis dari sistem Mail Blast PT. DYNACAST INDONESIA.</p>
                    <p class='warning'>JANGAN MEMBALAS EMAIL INI.</p>
                    <p>Kata sandi PDF anda adalah ID@tanggal lahir:</p>
                    
                    <div class='example'>
                        <strong>Contoh:</strong><br>
                        ID: AN190001<br>
                        Tanggal lahir: 25 Mei 1990<br>
                        Sandi: AN190001@25051990
                    </div>
                </div>
            </body>
            </html>";
            }
        }
        public static class PasswordGenerator
        {
            public static string GeneratePassword(string employeeId, DateTime? dateOfBirth)
            {
                if (string.IsNullOrEmpty(employeeId) || !dateOfBirth.HasValue)
                {
                    return "default_password";
                }

                // Format tanggal lahir menjadi ddMMyyyy
                string formattedDate = dateOfBirth.Value.ToString("ddMMyyyy");

                // Gabungkan ID dengan tanggal lahir
                return $"{employeeId}@{formattedDate}";
            }
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
                existingEmail.Subject = "Bukti Pemotongan PPh Pasal 21";
                existingEmail.Body = EmailTemplates.GetPdfEmailTemplate();

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

        public List<MasterMail> GetReady()
        {
            return _uow.Repository<MasterMail>().Table().Where(c=>c.IsEncrypted==false).ToList();
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


        //public List<MasterMail> GetReadyReceipent()
        //{
        //    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        //    DirectoryInfo dir = new DirectoryInfo(baseDir);

        //   while (dir != null && dir.Name != "MailManager.Web")
        //    {
        //        dir = dir.Parent;
        //    }

        //    if (dir == null)
        //    {
        //        Console.WriteLine("ERROR: Tidak menemukan folder MailManager.Web!");
        //        return new List<MasterMail>();
        //    }

        //    string pdfFolderPath = Path.Combine(dir.FullName, "FilePDF");

        //    var allEmails = _uow.Repository<MasterMail>().Table().ToList();

        //        var readyEmails = allEmails.Where(email =>
        //        !string.IsNullOrEmpty(email.FileName) &&
        //        File.Exists(Path.Combine(pdfFolderPath, email.FileName + ".pdf"))
        //    ).ToList();

        //    return readyEmails;
        //}

        public List<MasterMail> GetReadyReceipent()
        {
            
            string webRootPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            string pdfFolderPath = Path.Combine(webRootPath, "FilePDF");

            if (!Directory.Exists(pdfFolderPath))
            {
                Console.WriteLine($"ERROR: Folder tidak ditemukan di path: {pdfFolderPath}");
                return new List<MasterMail>();
            }

            var allEmails = _uow.Repository<MasterMail>().Table().ToList();
            var readyEmails = allEmails.Where(email =>
                !string.IsNullOrEmpty(email.NoBadge) &&
                File.Exists(Path.Combine(pdfFolderPath, email.NoBadge + ".pdf"))
            ).ToList();

            return readyEmails;
        }

        public List<MasterMail> GetReadyAndEncryptedReceipent()
        {

            string webRootPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            string pdfFolderPath = Path.Combine(webRootPath, "FilePDF");

            if (!Directory.Exists(pdfFolderPath))
            {
                Console.WriteLine($"ERROR: Folder tidak ditemukan di path: {pdfFolderPath}");
                return new List<MasterMail>();
            }

            var allEmails = _uow.Repository<MasterMail>().Table().Where(c=>c.IsEncrypted==true && c.IsSent==false).ToList();
            var readyEmails = allEmails.Where(email =>
                !string.IsNullOrEmpty(email.NoBadge) &&
                File.Exists(Path.Combine(pdfFolderPath, email.NoBadge +"-" + email.RecipientName+ ".pdf"))
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

        public bool EncryptPdf(int id)
        {
            try
            {
                // Ambil data email dari database
                var email = _uow.Repository<MasterMail>().GetById(id);
                if (email == null) return false;

                // Dapatkan path file
                string webRootPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                string pdfFolderPath = Path.Combine(webRootPath, "FilePDF");

                string sourceFile = Path.Combine(pdfFolderPath, email.FileName + ".pdf");
                string encryptedFile = Path.Combine(pdfFolderPath, email.FileName +"-" + email.RecipientName + ".pdf");

                // Cek apakah file exists
                if (!File.Exists(sourceFile))
                {
                    System.Diagnostics.Trace.WriteLine($"File tidak ditemukan: {sourceFile}");
                    return false;
                }

                // Generate password (contoh: menggunakan EmployeeNumber atau field lain)
                string password = PasswordGenerator.GeneratePassword(
                    email.FileName,  // Pastikan nama field sesuai dengan model Anda
                    email.DateofBirth  // Pastikan nama field sesuai dengan model Anda
                );

                using (Stream input = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Stream output = new FileStream(encryptedFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfReader reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output,
                        true,
                        password,
                        password,
                        PdfWriter.ALLOW_PRINTING | PdfWriter.ALLOW_COPY
                    );
                }

                // Update nama file di database
                //email.FileName = email.FileName;
                email.IsEncrypted = true;
                email.Body = password; // Simpan password jika diperlukan
                UpdatePdfStatus(email);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Error encrypting PDF: {ex.Message}");
                return false;
            }
        }

        public void UpdatePdfStatus(MasterMail email)
        {
            if (email == null) throw new ArgumentNullException("email");
            _uow.Repository<MasterMail>().Attach(email);
            _uow.SaveChanges();
        }


        public (int success, int failed, List<string> errors) EncryptAllPdfs()
        {
            var successCount = 0;
            var failedCount = 0;
            var errors = new List<string>();
            try
            {
                _uow.CreateTransaction();
                var unencryptedEmails = _uow.Repository<MasterMail>()
                    .Table()
                    .Where(c => c.IsEncrypted == false)
                    .ToList();

                foreach (var email in unencryptedEmails)
                {
                    try
                    {
                        string webRootPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                        string pdfFolderPath = Path.Combine(webRootPath, "FilePDF");
                        string sourceFile = Path.Combine(pdfFolderPath, email.NoBadge + ".pdf");
                        string encryptedFile = Path.Combine(pdfFolderPath, email.NoBadge + "-" + email.RecipientName + ".pdf");

                        if (!File.Exists(sourceFile))
                        {
                            errors.Add($"File tidak ditemukan: {email.FileName}.pdf");
                            failedCount++;
                            continue;
                        }

                        string password = PasswordGenerator.GeneratePassword(email.NoBadge, email.DateofBirth);
                        bool encryptionSuccess = false;

                        try
                        {
                            using (Stream input = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            using (Stream output = new FileStream(encryptedFile, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                PdfReader reader = new PdfReader(input);
                                PdfEncryptor.Encrypt(reader, output,
                                    true,
                                    password,
                                    password,
                                    PdfWriter.ALLOW_PRINTING | PdfWriter.ALLOW_COPY
                                );
                                encryptionSuccess = true;
                            }

                            // Jika enkripsi berhasil, update database dan hapus file lama
                            if (encryptionSuccess)
                            {
                                // Hapus file lama
                                try
                                {
                                    File.Delete(sourceFile);
                                }
                                catch (Exception deleteEx)
                                {
                                    // Log error penghapusan tapi jangan hentikan proses
                                    errors.Add($"Warning: Gagal menghapus file lama {email.NoBadge}.pdf: {deleteEx.Message}");
                                }

                                email.FileName = email.NoBadge + "-" + email.RecipientName;
                                email.IsEncrypted = true;
                                email.Body = password;
                                _uow.Repository<MasterMail>().Attach(email);
                                successCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Jika terjadi error saat enkripsi, hapus file hasil enkripsi jika sudah terbuat
                            if (File.Exists(encryptedFile))
                            {
                                try
                                {
                                    File.Delete(encryptedFile);
                                }
                                catch { /* Ignore cleanup errors */ }
                            }
                            throw; // Re-throw untuk ditangkap oleh catch di luar
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error encrypting {email.FileName}: {ex.Message}");
                        failedCount++;
                    }
                }

                // Simpan semua perubahan
                _uow.SaveChanges();
                _uow.Commit();
                return (successCount, failedCount, errors);
            }
            catch (Exception ex)
            {
                _uow.Rollback();
                errors.Add($"Transaction error: {ex.Message}");
                return (successCount, failedCount, errors);
            }
        }
    }


}





