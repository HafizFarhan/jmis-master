using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Responses
{
    public class MessageWithExceptionResponse
    {
        public MessageResponse Response { get; set; }
        public bool Error { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }

    public class MessageWithExceptionResponse<T>
    {
        public T Response { get; set; }
        public bool Error { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}