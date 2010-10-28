using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace MobileMilk.Model
{
    public class Task
    {
        public string TaskSeriesId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string LocationId { get; set; }

        public List<string> Tags { get; set; }
        public List<User> Participants { get; set; }
        public List<Note> Notes { get; set; }

        public string Id { get; set; }
        public DateTime? Due { get; set; }
        public bool HasDueTime { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Deleted { get; set; }
        public int Priority { get; set; }
        public int Postponed { get; set; }
        public DateTime? Estimate { get; set; }

        public bool IsNew { get; set; }
    }
}