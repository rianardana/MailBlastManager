using MailManager.Service.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailManager.Data;

namespace MailManager.Service.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private string _errorMessage = string.Empty;
        private readonly MailManagerEntities entities = null;
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        private DbContextTransaction _objTran;
        public UnitOfWork(MailManagerEntities entities = null)
        {

            this.entities = entities ?? new MailManagerEntities();
            //  this.entities = entities;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var temp = typeof(T);

            foreach (var dc in repositories)
            {
                if (dc.Key == temp)
                {
                    return repositories[typeof(T)] as IRepository<T>;
                }
            }



            IRepository<T> repo = new EfRepository<T>(entities);
            repositories.Add(typeof(T), repo);
            return repo;
        }

        public void CreateTransaction()
        {
            _objTran = entities.Database.BeginTransaction();
        }

        public void Commit()
        {
            _objTran.Commit();
        }

        public void Rollback()
        {
            _objTran.Rollback();
            _objTran.Dispose();
        }

        public void SaveChanges()
        {
            try
            {
                entities.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        _errorMessage += $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}" + Environment.NewLine;
                throw new Exception(_errorMessage, dbEx);
            }

        }

        public void CloseConnection()
        {
            entities.Dispose();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    entities.Dispose();
            _disposed = true;
        }
    }
}
