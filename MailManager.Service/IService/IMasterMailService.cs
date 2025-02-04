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
        List<MasterMail> GetUnsentEmails();

        void UpdateEmailStatus(MasterMail mail);

        //void SendEmails();

        List<MasterMail> GetAll();
        void SendQueuedEmails();
    }
}
