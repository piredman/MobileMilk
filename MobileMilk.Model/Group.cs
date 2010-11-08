using System.Collections.Generic;

namespace MobileMilk.Model
{
    public class Group
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public int Count { get { return Tasks.Count; } }
        public List<Task> Tasks { get; set; }
    }
}