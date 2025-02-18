using MailManager.Data;
using MailManager.Service.IService;
using MailManager.Service.Service;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MailManager.Web.Helpers
{
    public static class ExcelHelper
    {
        public static void UploadExcelFile(Stream MyExcelStream)
        {
            using (var package = new ExcelPackage(MyExcelStream))
            {
                IMasterMailService mailService = new MasterMailService();
                var currenSheet = package.Workbook.Worksheets;
                var workSheet = currenSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;
                var obj = (object[,])workSheet.Cells.Value;
                var datas = new List<MasterMail>();
                var existingData = mailService.GetAll();

                for (int k = 1; k < noOfRow; k++)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(obj[k, 0]))) continue;
                    if (string.IsNullOrEmpty(Convert.ToString(obj[k, 1]))) continue;


                    var mail = new MasterMail();

                    mail.NoBadge= Convert.ToString(obj[k, 0]);
                    mail.RecipientName = Convert.ToString(obj[k, 1]);
                    mail.EmailTo = Convert.ToString(obj[k, 2]);
                    string dateString = Convert.ToString(obj[k, 3]);
                    DateTime result;

                    // Mencoba beberapa metode parsing
                    if (DateTime.TryParseExact(dateString.Trim(), "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out result))
                    {
                        mail.DateofBirth = result;
                    }
                    else if (double.TryParse(dateString, out double excelDate))
                    {
                        mail.DateofBirth = DateTime.FromOADate(excelDate);
                    }
                    else
                    {
                        throw new Exception($"Convert Date Error : {dateString} for {mail.RecipientName}");
                    }

                    datas.Add(mail);


                }
                foreach (var mail in datas)
                {
                    var existingFile = existingData.FirstOrDefault(c=>c.FileName == mail.FileName);
                    if (existingFile != null)
                    {
                        mail.Id = existingFile.Id;
                        mailService.Update(mail);
                    }
                    else
                    {
                        mailService.Insert(mail);
                    }
                       
                    

                }
            }
        }
    }

}