using System;
using System.Collections.Generic;

namespace MobileMilk.Model
{
    public interface IFilter
    {
        string Name { get; }
        string Type { get; }
        object Value { get; set; }
    }

    public class ListFilter : IFilter
    {
        public string Name { get { return "list"; } }
        public string Type { get { return this.GetType().FullName; } }
        public object Value { get; set; }
    }

    public class Filters
    {
        public string List { get; set; }
        public int? Priority { get; set; }
        public bool? IsComplete { get; set; }

        public List<string> IncludeTags { get; set; }
        public List<string> ExcludeTags { get; set; }

        public List<string> IncludePartialTags { get; set; }
        public List<string> ExcludePartialTags { get; set; }

        public bool? IsTaged { get; set; }

        public string Location { get; set; }
        //public string LocatedWithin { get; set; }
        public bool? IsLocated { get; set; }
        public bool? IsRepeating { get; set; }

        public string TaskName { get; set; }

        public string NoteContains { get; set; }
        public bool? HasNotes { get; set; }

        public DateTime? Due { get; set; }
        public DateTime? DueBefore { get; set; }
        public DateTime? DueAfter { get; set; }
        //public DateTime? DueWithin { get; set; }

        public DateTime? Completed { get; set; }
        public DateTime? CompletedBefore { get; set; }
        public DateTime? CompletedAfter { get; set; }
        //public DateTime? CompletedWithin { get; set; }

        public DateTime? Added { get; set; }
        public DateTime? AddedBefore { get; set; }
        public DateTime? AddedAfter { get; set; }
        //public DateTime? AddedWithin { get; set; }

        public int? TimeEstimate { get; set; }
        public int? Postponed { get; set; }

        public bool? IsShared { get; set; }
        public string SharedWith { get; set; }
        public bool? IsRecieved { get; set; }
        public string SentTo { get; set; }
        public string RecievedFrom { get; set; }

        public bool? IncludeArchived { get; set; }
    }
}
