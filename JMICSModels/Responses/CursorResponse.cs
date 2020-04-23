using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Responses
{
    public class CursorResponse
    {
        public Cursor cursor { get; set; }
        public class Cursor
        {
            private string prev;
            private string next;
            private bool hasPrev;
            private bool hasNext;
            private long totalRows;


            [JsonProperty(PropertyName = "prev")]
            public string Prev { get { return prev; } set { prev = value; } }
            [JsonProperty(PropertyName = "next")]
            public string Next { get { return next; } set { next = value; } }
            [JsonProperty(PropertyName = "hasPrev")]
            public bool HasPrev { get { return hasPrev; } set { hasPrev = value; } }
            [JsonProperty(PropertyName = "hasNext")]
            public bool HasNext { get { return hasNext; } set { hasNext = value; } }
            [JsonProperty(PropertyName = "totalRows")]
            public long TotalRows { get { return totalRows; } set { totalRows = value; } }


        }

        public CursorResponse(string requestPath, string prev, string next, long totalRows, long limit, bool hasPrev = true, bool hasNext = true, string query = "")
        {
            Cursor cursor = new Cursor();
            if (!string.IsNullOrEmpty(query))
                //    query = "&" + query;

                if (hasPrev)
                    cursor.Prev = requestPath + (requestPath.IndexOf("?") >= 0 ? "&" : "?") + prev + "&limit=" + limit + "&" + query;
                else
                    cursor.Prev = null;


            if (hasNext)
                cursor.Next = requestPath + (requestPath.IndexOf("?") >= 0 ? "&" : "?") + next + "&limit=" + limit + "&" + query;
            else
                cursor.Next = null;
            cursor.HasPrev = hasPrev;
            cursor.HasNext = hasNext;
            cursor.TotalRows = totalRows;


            this.cursor = cursor;
        }
    }
}

