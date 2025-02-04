using MailManager.Data;
using MailManager.Service.IService;
using MailManager.Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.Service
{
    public class EmailAccountService : IEmailAccountService
    {
        private readonly IUnitOfWork _uow;
        public EmailAccountService(IUnitOfWork unit)
        {
            _uow = unit;
        }
        public List<EmailAccount> GetAll()
        {
            return _uow.Repository<EmailAccount>().Table().ToList();
        }

        public EmailAccount GetDefaultEmailAccount()
        {
            return _uow.Repository<EmailAccount>().Table().FirstOrDefault();
        }
    }
}
