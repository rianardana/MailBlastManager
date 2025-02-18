using MailManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.IService
{
    public interface IMasterMailService
    {
        List<MasterMail> GetSentEmails();
        List<MasterMail> GetReadyReceipent();

        void UpdateEmailStatus(MasterMail mail);

        //void SendEmails();

        void Insert(MasterMail obj);
        void Update(MasterMail obj);

        List<MasterMail> GetAll();
        //void SendQueuedEmails();
        List<MasterMail> GetReady();
        void UpdateSubject(MasterMail mail);
        void UpdatePdfStatus(MasterMail email);
        bool EncryptPdf(int id);
        (int success, int failed, List<string> errors) EncryptAllPdfs();
        List<MasterMail> GetReadyAndEncryptedReceipent();
    }
}
