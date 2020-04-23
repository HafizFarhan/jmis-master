using MTC.JMICS.Models.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.DAL
{
    public class NewsRepository : IRepository<News>, IDisposable
    {
        public int Delete<T>(T entityToDelete)
        {
            return BaseRepository.Delete<T>(entityToDelete).Result;

        }
        public int Delete<T>(object id)
        {
            return BaseRepository.Delete<T>(id).Result;
        }
        public int DeleteList<T>(object whereConditions)
        {
            throw new NotImplementedException();
        }
        public int DeleteList<T>(string conditions, object parameters = null)
        {
            throw new NotImplementedException();
        }
        public T Get<T>(object id)
        {
            return BaseRepository.Get<T>(id).Result;
        }
        public IEnumerable<T> GetList<T>()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<T> GetList<T>(object whereConditions)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<T> GetList<T>(string conditions, object parameters = null)
        {
            return BaseRepository.GetList<T>(conditions).Result;
        }
        public IEnumerable<T> GetListPaged<T>(int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<T> GetListPaged<T>(int pageNumber, int rowsPerPage, Dictionary<string, object> conditions, string orderby, string[] parameters = null)
        {
            return BaseRepository.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderby, parameters).Result;
        }
        public new int Insert<T>(T entityToInsert)
        {
            return BaseRepository.Insert<T>(entityToInsert).Result;
        }
        public new TKey Insert<TKey, TEntity>(TEntity entityToInsert)
        {
            throw new NotImplementedException();
        }
        public int RecordCount<T>(string conditions = "", object parameters = null)
        {
            throw new NotImplementedException();
        }
        public int RecordCount<T>(object whereConditions)
        {
            return BaseRepository.RecordCount<T>(whereConditions).Result;
        }
        public int Update<T>(T entityToUpdate)
        {
            return BaseRepository.Update<T>(entityToUpdate).Result;
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NewsRepository()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
