using MailManager.Service.IService;
using MailManager.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;

namespace MailManager.Web.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private readonly IUserService _service;
        public LoginController(IUserService service)
        {
            _service = service;
        }
        public ActionResult Index(string returnUrl)
        {
            var model = new LoginVM();
            ViewBag.ReturnUrl = returnUrl;
            FormsAuthentication.SignOut();
            Session.Clear();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginVM model, string returnUrl)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    var user = _service.Login(model.UserName);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User Name is Incorrect!");
                        return View(model);
                    }
                    if (user.Password != model.Password)
                    {
                        ModelState.AddModelError("", "Password is Incorrect!");
                        return View(model);
                    }
                   
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                       
                            return RedirectToAction("Index", "Home");
                        
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}