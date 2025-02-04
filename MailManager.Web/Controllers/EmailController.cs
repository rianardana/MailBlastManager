using MailManager.Service.IService;
using MailManager.Web.Extension;
using MailManager.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MailManager.Web.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        private readonly IMasterMailService _service;
        public EmailController(IMasterMailService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            var model = new List<MasterMailVM>();
            try
            {
                model = _service.GetAll().Select(item=>item.ToModel()).ToList();    
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("",ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SendEmails()
        {
            try
            {
                _service.SendQueuedEmails();  
                TempData["Message"] = "Email berhasil dikirim!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Terjadi kesalahan: {ex.Message}";
            }

            return RedirectToAction("Index");  
        }
    }
}