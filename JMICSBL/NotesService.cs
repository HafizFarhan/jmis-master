using MTC.JMICS.DAL;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Utility.Cache;
using MTC.JMICS.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTC.JMICS.BL
{
    public class NotesService : BaseService, IDisposable
    {
        public Notes GetById(int NoteID)
        {
            try
            {
                if (MemCache.IsIncache("AllNotesKey"))
                {
                    return MemCache.GetFromCache<List<Notes>>("AllNotesKey").Where<Notes>(x => x.NoteId == NoteID).FirstOrDefault();
                }
                using (NotesRepository notesRepo = new NotesRepository())
                {
                    Notes noteModel = new Notes();
                    {
                        noteModel = notesRepo.Get<Notes>(NoteID);
                        return noteModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Notes Add(int SubscriberId, string UserName, Notes NoteModel)
        {
            try
            {
                if (NoteModel == null)
                    throw new Exception("Notes model is null");

                using (NotesRepository notesRepo = new NotesRepository())
                {
                    NoteModel.CreatedBy = UserName;
                    NoteModel.CreatedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
                    var rowId = notesRepo.Insert<Notes>(NoteModel);
                    NoteModel.NoteId = rowId;

                    if (MemCache.IsIncache("AllNotesKey"))
                        MemCache.GetFromCache<List<Notes>>("AllNotesKey").Add(NoteModel);
                    else
                    {
                        List<Notes> notesList = new List<Notes>();
                        notesList.Add(NoteModel);
                        MemCache.AddToCache("AllNotesKey", notesList);
                    }
                    return NoteModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public bool Update(int SubscriberId, string UserName, Notes NoteModel)
        //{
        //    try
        //    {
        //        using (NotesRepository notesRepo = new NotesRepository())
        //        {
        //            if (MemCache.IsIncache("AllNotesKey"))
        //            {
        //                List<Notes> notesModel = MemCache.GetFromCache<List<Notes>>("AllNotesKey");
        //                if (notesModel.Count > 0)
        //                    notesModel.Remove(notesModel.Find(x => x.NoteId == NoteModel.NoteId));
        //            }
        //            NoteModel.LastModifiedBy = UserName;
        //            NoteModel.LastModifiedOn = Common.GetLocalDateTime(MemCache.GetFromCache<string>("Timezone_" + SubscriberId));
        //            notesRepo.Update<Notes>(NoteModel);

        //            if (MemCache.IsIncache("AllNotesKey"))
        //                MemCache.GetFromCache<List<Notes>>("AllNotesKey").Add(NoteModel);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public bool Delete(int NotesID)
        {
            try
            {
                using (NotesRepository notesRepo = new NotesRepository())
                {
                    var notesExisting = notesRepo.Get<Notes>(NotesID);
                    if (notesExisting == null)
                        return false;
                    
                    else
                    {
                        notesRepo.Delete<Notes>(NotesID);
                        if (MemCache.IsIncache("AllNotesKey"))
                            MemCache.GetFromCache<List<Notes>>("AllNotesKey").Remove(MemCache.GetFromCache<List<Notes>>("AllNotesKey").Where(x => x.NoteId == notesExisting.NoteId).ToList().FirstOrDefault());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Notes> List(Dictionary<string, string> Dic = null)
        {
            try
            {
                List<Notes> lstNotes = new List<Notes>();
                if (MemCache.IsIncache("AllNotesKey"))
                    return MemCache.GetFromCache<List<Notes>>("AllNotesKey");
                
                else
                {
                    if (Dic == null)
                        Dic = new Dictionary<string, string>();

                    Dic.Add("orderby", "Created_On");
                    Dic.Add("offset", "1");
                    Dic.Add("limit", "200");

                    var parameters = this.ParseParameters(Dic);
                    using (NotesRepository notesRepo = new NotesRepository())
                    {
                        lstNotes = notesRepo.GetListPaged<Notes>(Convert.ToInt32(Dic["offset"]), Convert.ToInt32(Dic["limit"]), parameters, Dic["orderby"]).ToList();
                        MemCache.AddToCache("AllNotesKey", lstNotes);
                        return lstNotes;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Dictionary<string, object> ParseParameters(Dictionary<string, string> dic)
        {
            Dictionary<string, object> dicAux = new Dictionary<string, object>();

            string offset;
            string limit;
            string orderby;
            string sort;
            string keyfilter;
            string notesid;
            string keyword;

            if (dic.TryGetValue("offset", out offset))
            {
                dicAux.Add("@offset", offset);
            }

            if (dic.TryGetValue("limit", out limit))
            {
                dicAux.Add("@limit", limit);
            }

            if (dic.TryGetValue("orderby", out orderby))
            {
                dicAux.Add("@orderby", orderby);
            }

            if (dic.TryGetValue("sortorder", out sort))
            {
                dicAux.Add("@sortorder", sort);
            }
            if (dic.TryGetValue("keyfilter", out keyfilter))
            {
                dicAux.Add("@keyfilter", keyfilter);
            }
            if (dic.TryGetValue("Notesid", out notesid))
            {
                dicAux.Add("@Notesid", notesid);
            }
            if (dic.TryGetValue("Keyword", out keyword))
            {
                dicAux.Add("@Description", keyword);
            }
            return dicAux;
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
        // ~Noteservice()
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
