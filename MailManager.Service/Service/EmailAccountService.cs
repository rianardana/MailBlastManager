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

        public void Update(EmailAccount obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            _uow.Repository<EmailAccount>().Attach(obj);
            _uow.SaveChanges();

            
        }

        public List<EmailAccount> GetAll()
        {
            return _uow.Repository<EmailAccount>().Table().ToList();
        }

        public EmailAccount GetById(int id)
        {
            return _uow.Repository<EmailAccount>().GetById(id);
        }

        public EmailAccount GetDefaultEmailAccount()
        {
            return _uow.Repository<EmailAccount>().Table().FirstOrDefault();
        }
    }
}
