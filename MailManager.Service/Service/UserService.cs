using MailManager.Data;
using MailManager.Service.IService;
using MailManager.Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        public UserService(IUnitOfWork unit)
        {
            _uow = unit;
        }
        public User Login(string username)
        {
            return _uow.Repository<User>().Get(c=>c.Username.Equals(username));
        }
    }
}
