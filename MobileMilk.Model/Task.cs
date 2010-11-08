using System;
using System.Collections.Generic;

namespace MobileMilk.Model
{
    public class Task
    {
        public string Id { get; set; }
        public string TaskSeriesId { get; set; }
        public string ListId { get; set; }
        public string LocationId { get; set; }

        public string Name { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public int Priority { get; set; }
        public int Postponed { get; set; }

        public DateTime? Due { get; set; }
        public bool HasDueTime { get; set; }
        public DateTime? Estimate { get; set; }

        public DateTime? Completed { get; set; }
        public bool IsComplete  { get { return (null != Completed) ? true : false; } }

        public bool IsRepeating { get; set; }

        public List<string> Tags { get; set; }
        public bool HasTags { get { return (null != Tags) ? Tags.Count > 0 : false; } }

        public List<User> Participants { get; set; }
        public bool HasParticipants { get { return (null != Participants) ? Participants.Count > 0 : false; } }

        public List<Note> Notes { get; set; }
        public bool HasNotes { get { return (null != Notes) ? Notes.Count > 0 : false; } }

        public DateTime? Added { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public bool IsNew { get; set; }
    }
}