using MailManager.Service.IService;
using MailManager.Service.Service;
using MailManager.Web.Extension;
using MailManager.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MailManager.Web.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        private readonly IMasterMailService _service;
        private readonly IEmailAccountService _serviceAccount;
        public EmailController(IMasterMailService service, IEmailAccountService serviceAccount)
        {
            _service = service;
            _serviceAccount = serviceAccount;

        }
        public ActionResult Index()
        {
            var model = new List<MasterMailVM>();
            try
            {
                model = _service.GetReadyReceipent().Select(item=>item.ToModel()).ToList();    
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("",ex.Message);
            }
            return View(model);
        }

        public ActionResult Report()
        {
            var model = new List<MasterMailVM>();
            try
            {
                model = _service.GetSentEmails().Select(item => item.ToModel()).ToList();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        //[HttpPost]
        //public ActionResult SendEmails()
        //{
        //    try
        //    {
        //        _service.SendQueuedEmails();  
        //        TempData["Message"] = "Email berhasil dikirim!";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Message"] = $"Terjadi kesalahan: {ex.Message}";
        //    }

        //    return RedirectToAction("Index");  
        //}

        //[HttpPost]
        //public ActionResult SendEmails()
        //{
        //    try
        //    {
        //        var emails = _service.GetReadyReceipent();
        //        if (!emails.Any())
        //        {
        //            return Json(new { success = false, message = "No pending emails found." }, JsonRequestBehavior.AllowGet);
        //        }

        //        var emailAccount = _serviceAccount.GetDefaultEmailAccount();
        //        if (emailAccount == null)
        //        {
        //            return Json(new { success = false, message = "No SMTP configuration found." }, JsonRequestBehavior.AllowGet);
        //        }

        //        List<string> failedEmails = new List<string>();

        //        foreach (var email in emails)
        //        {
        //            try
        //            {
        //                _service.UpdateSubject(email);
        //                using (var message = new MailMessage())
        //                using (var smtpClient = new SmtpClient())
        //                {
        //                    smtpClient.Host = emailAccount.Host;
        //                    smtpClient.Port = emailAccount.Port.Value;
        //                    smtpClient.EnableSsl = emailAccount.EnableSSL.Value;
        //                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //                    smtpClient.UseDefaultCredentials = false;
        //                    smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);

        //                    message.From = new MailAddress(emailAccount.Email, "NoReply");
        //                    message.To.Add(email.EmailTo);
        //                    message.Subject = email.Subject;
        //                    message.Body = HttpUtility.HtmlDecode(email.Body);
        //                    message.IsBodyHtml = true;

        //                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        //                    DirectoryInfo dir = new DirectoryInfo(baseDir);
        //                    while (dir != null && dir.Name != "MailManager.Web")
        //                    {
        //                        dir = dir.Parent;
        //                    }

        //                    if (dir == null)
        //                    {
        //                        Console.WriteLine("ERROR: Tidak menemukan folder MailManager.Web!");
        //                        failedEmails.Add(email.EmailTo);
        //                        continue;
        //                    }

        //                    if (!string.IsNullOrEmpty(email.FileName))
        //                    {
        //                        string pdfFileName = email.FileName + ".pdf";
        //                        string pdfPath = Path.Combine(dir.FullName, "FilePDF", pdfFileName);

        //                        if (System.IO.File.Exists(pdfPath))
        //                        {
        //                            message.Attachments.Add(new Attachment(pdfPath));
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine($"WARNING: File NOT FOUND at: {pdfPath}");
        //                        }
        //                    }

        //                    smtpClient.Send(message);

        //                    email.IsSent = true;
        //                    email.SentOnUTC = DateTime.UtcNow;
        //                    _service.UpdateEmailStatus(email);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error sending email to {email.EmailTo}: {ex.Message}");
        //                failedEmails.Add(email.EmailTo);
        //            }
        //        }

        //        if (failedEmails.Any())
        //        {
        //            return Json(new { success = false, message = "Some emails failed: " + string.Join(", ", failedEmails) }, JsonRequestBehavior.AllowGet);
        //        }

        //        return Json(new { success = true, message = "All emails sent successfully." }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error processing emails: " + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult SendEmails()
        {
            try
            {
                var emails = _service.GetReadyReceipent();
                if (!emails.Any())
                {
                    return Json(new { success = false, message = "No pending emails found." }, JsonRequestBehavior.AllowGet);
                }

                var emailAccount = _serviceAccount.GetDefaultEmailAccount();
                if (emailAccount == null)
                {
                    return Json(new { success = false, message = "No SMTP configuration found." }, JsonRequestBehavior.AllowGet);
                }

                List<string> failedEmails = new List<string>();
                // Dapatkan root path aplikasi
                string webRootPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                string pdfFolderPath = Path.Combine(webRootPath, "FilePDF");

                foreach (var email in emails)
                {
                    try
                    {
                        _service.UpdateSubject(email);
                        using (var message = new MailMessage())
                        using (var smtpClient = new SmtpClient())
                        {
                            smtpClient.Host = emailAccount.Host;
                            smtpClient.Port = emailAccount.Port.Value;
                            smtpClient.EnableSsl = emailAccount.EnableSSL.Value;
                            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);

                            message.From = new MailAddress(emailAccount.Email, "NoReply");
                            message.To.Add(email.EmailTo);
                            message.Subject = email.Subject;
                            message.Body = HttpUtility.HtmlDecode(email.Body);
                            message.IsBodyHtml = true;

                            if (!string.IsNullOrEmpty(email.FileName))
                            {
                                string pdfFileName = email.FileName + ".pdf";
                                string pdfPath = Path.Combine(pdfFolderPath, pdfFileName);

                                if (System.IO.File.Exists(pdfPath))
                                {
                                    message.Attachments.Add(new Attachment(pdfPath));
                                }
                                else
                                {
                                    // Gunakan proper logging di production
                                    System.Diagnostics.Trace.WriteLine($"WARNING: File tidak ditemukan di: {pdfPath}");
                                }
                            }

                            smtpClient.Send(message);

                            email.IsSent = true;
                            email.SentOnUTC = DateTime.UtcNow;
                            _service.UpdateEmailStatus(email);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Gunakan proper logging di production
                        System.Diagnostics.Trace.WriteLine($"Error sending email to {email.EmailTo}: {ex.Message}");
                        failedEmails.Add(email.EmailTo);
                    }
                }

                if (failedEmails.Any())
                {
                    return Json(new { success = false, message = "a few email failed to sent: " + string.Join(", ", failedEmails) }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, message = "All emails were sent successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error processing email: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReadyDoc()
        {
            var model = new List<MasterMailVM>();
            try
            {
                model = _service.GetReady().Select(item => item.ToModel()).ToList();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        public ActionResult AllReadyDoc()
        {
            var model = new List<MasterMailVM>();
            try
            {
                model = _service.GetReady().Select(item => item.ToModel()).ToList();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Encrypt(int Id)
        {
            try
            {
                if (Id==0)
                {
                    return Json(new { success = false, message = "Email ID is required." }, JsonRequestBehavior.AllowGet);
                }

                bool result = _service.EncryptPdf(Id);

                if (result)
                {
                    return Json(new { success = true, message = "PDF encrypted successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Failed to encrypt PDF." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EncryptAll()
        {
            try
            {
                var result = _service.EncryptAllPdfs();

                var message = $"Successfully encrypted {result.success} files. ";
                if (result.failed > 0)
                {
                    message += $"Failed to encrypt {result.failed} files. ";
                    message += $"Errors: {string.Join("; ", result.errors)}";
                }

                return Json(new
                {
                    success = true,
                    successCount = result.success,
                    failedCount = result.failed,
                    errors = result.errors,
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error processing batch encryption: {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}