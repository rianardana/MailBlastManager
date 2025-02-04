using MailManager.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        void CreateTransaction();
        void Commit();
        void Rollback();
        void SaveChanges();
        void CloseConnection();
    }
}
