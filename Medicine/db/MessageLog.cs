using System;
using System.Collections.Generic;
using System.Text;

namespace Medicine.db
{
    public class MessageLog
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; }
        public DateTime SendTime { get; set; }
    }
}
