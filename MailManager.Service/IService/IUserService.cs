using MailManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.IService
{
    public interface IUserService
    {
        User Login(string username);
    }
}
