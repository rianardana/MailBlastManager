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
    public class EmailAccountController : Controller
    {
        // GET: EmailAccount
        private readonly IEmailAccountService _service;
        public EmailAccountController(IEmailAccountService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            var model = new List<EmailAccountVM>();
            try
            {
                model = _service.GetAll().Select(item=>item.ToModel()).ToList();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        public ActionResult Edit(int Id)
        {
            var model = new EmailAccountVM();
            try
            {
                if(Id==0)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }
                var entity = _service.GetById(Id);
                
                if(entity == null)
                {
                    return HttpNotFound();
                }
                model = entity.ToModel();   
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmailAccountVM model)
        {
            try
            {
                var entity = model.ToEntity();
                _service.Update(entity);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }
    }
}