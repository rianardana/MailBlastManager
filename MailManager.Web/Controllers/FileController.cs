using MailManager.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MailManager.Web.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        private readonly string uploadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FilePDF");
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadExcel()
        {
            try
            {
                var uploadFile = Request.Files["importexcelfile"];
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    ExcelHelper.UploadExcelFile(uploadFile.InputStream);
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult UploadPDF()
        {
            try
            {
                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No files selected." });
                }

                string uploadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FilePDF");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                int uploadedCount = 0; 

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(uploadPath, fileName);

                        
                        if (System.IO.File.Exists(filePath))
                        {
                            //string uniqueName = Path.GetFileNameWithoutExtension(file.FileName) +
                            //                    "_" + Guid.NewGuid().ToString().Substring(0, 8) +
                            //                    Path.GetExtension(file.FileName);
                            //filePath = Path.Combine(uploadPath, uniqueName);

                            System.IO.File.Delete(filePath);
                        }

                        file.SaveAs(filePath);
                        uploadedCount++; 
                    }
                }

                return Json(new { success = true, message = $"{uploadedCount} files uploaded successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading files: " + ex.Message });
            }
        }

        public ActionResult DownloadTemplate()
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "Template.xlsx");

            if (System.IO.File.Exists(templatePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(templatePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template.xlsx");
            }

            return HttpNotFound("Template file not found");
        }

    }
}