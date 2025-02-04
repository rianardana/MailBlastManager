using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Service.Repository
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll(Func<T, bool> predicate = null);
        T Get(Func<T, bool> predicate);
        T GetLast(Func<T, object> predicate);
        void Add(T entity);
        void Attach(T entity);
        void Delete(T entity);

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<T> Table();

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking();

        void ExecuteSql(string sql);

        List<T> SqlQueryList(string sql);
        T SqlQuery(string sql);


        //IQueryable<T> List();
        //T GetById(int id);

        ////Test Get By Name
        T GetByName(string Username);
        //T GetByIdAsNoTracking(int id);

        //T Add(T newItem);

        List<T> AddRange(List<T> items);

        //void Update(T item);

        //void Remove(T item);

        void RemoveRange(List<T> items);
        //IQueryable<T> TableNoTracking();


        //T GetLatestData();
        // Test Get Selected Column

    }
}
