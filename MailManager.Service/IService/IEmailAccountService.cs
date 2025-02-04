﻿using MailManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.IService
{
    public interface IEmailAccountService
    {
        List<EmailAccount> GetAll();
        EmailAccount GetDefaultEmailAccount();

        void Update(EmailAccount obj);
        EmailAccount GetById(int id);
        
    }
}
