using MailManager.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.Repository
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        // private readonly DbProductionManagerEntities _dbContext;
        private readonly MailManagerEntities entities = null;
        private DbSet<T> _objectSet;
        public EfRepository(MailManagerEntities _entities = null)
        {
            // this._dbContext = dbContext;//?? new DbProductionManagerEntities();
            entities = _entities;
            _objectSet = entities.Set<T>();
        }

        public T GetById(int id)
        {
            return _objectSet.Find(id);
        }

        /// <summary>
        /// Get all the list
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAll(Func<T, bool> predicate = null)
        {
            if (predicate != null)
            {
                return _objectSet.Where(predicate);
            }

            return _objectSet.AsEnumerable();
        }
        /// <summary>
        /// Get a certain element of the list
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T Get(Func<T, bool> predicate)
        {
            return _objectSet.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Get a certain element of the list
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T GetLast(Func<T, object> predicate)
        {

            //  if (predicate != null)
            // {
            return _objectSet.OrderByDescending(predicate).FirstOrDefault();
            //   }


            //   return _objectSet.LastOrDefault();
        }

        /// <summary>
        /// Add a new element to the list
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            _objectSet.Add(entity);
        }
        /// <summary>
        /// Update a certain element
        /// </summary>
        /// <param name="entity"></param>
        public void Attach(T entity)
        {
            _objectSet.AddOrUpdate(entity);
        }
        /// <summary>
        /// Delete an element from the list
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            _objectSet.Remove(entity);
        }

        public IQueryable<T> Table()
        {
            return _objectSet;
        }

        public IQueryable<T> TableNoTracking()
        {
            return _objectSet.AsNoTracking();
        }

        public void ExecuteSql(string sql)
        {
            entities.Database.ExecuteSqlCommand(sql);

        }

        public List<T> SqlQueryList(string sql)
        {
            return entities.Database.SqlQuery<T>(sql).ToList();

        }

        public T SqlQuery(string sql)
        {
            return entities.Database.SqlQuery<T>(sql).FirstOrDefault();
        }

        public List<T> AddRange(List<T> items)
        {
            foreach (var itm in items)
            {
                entities.Set<T>().Add(itm);
                entities.Entry(itm).State = EntityState.Added;
                entities.SaveChanges();
            }

            return items;
        }

        public T GetByName(string Username)
        {
            return _objectSet.Find(Username);
        }


        public void RemoveRange(List<T> items)
        {
            foreach (var itm in items)
            {
                entities.Entry(itm).State = EntityState.Deleted;
                entities.SaveChanges();
            }
        }



    }
}
