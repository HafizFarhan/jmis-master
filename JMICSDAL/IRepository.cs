using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.DAL
{
    public interface IRepository<T> where T : class
    {
        T Get<T>(object id);
        IEnumerable<T> GetList<T>();
        IEnumerable<T> GetList<T>(object whereConditions);
        IEnumerable<T> GetList<T>(string conditions, object parameters = null);
        IEnumerable<T> GetListPaged<T>(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null);
        IEnumerable<T> GetListPaged<T>(int pageNumber, int rowsPerPage, Dictionary<string, object> conditions, string orderby, string[] parameters = null);
        int Insert<TEntity>(TEntity entityToInsert);
        TKey Insert<TKey, TEntity>(TEntity entityToInsert);
        int Update<TEntity>(TEntity entityToUpdate);
        int Delete<T>(T entityToDelete);
        int Delete<T>(object id);
        int DeleteList<T>(object whereConditions);
        int DeleteList<T>(string conditions, object parameters = null);
        int RecordCount<T>(string conditions = "", object parameters = null);
        int RecordCount<T>(object whereConditions);
    }
}
