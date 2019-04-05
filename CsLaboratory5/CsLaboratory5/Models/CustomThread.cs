using System;

namespace CsLaboratory5.Models
{
    public class CustomThread
    {
        public int ThreadId { get; set; }
        public string ThreadState { get; set; }
        public DateTime ThreadStart { get; set; }

        public CustomThread(int id, string state, DateTime start)
        {
            ThreadId = id;
            ThreadState = state;
            ThreadStart = start;
        }
    }
}
