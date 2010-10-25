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

namespace MobileMilk.Data.Entities
{
    public class RtmTask
    {
        public string Id { get; set; }
        public DateTime? Due { get; set; }
        public bool HasDueTime { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Deleted { get; set; }
        public string Priority { get; set; }
        public int Postponed { get; set; }
        public DateTime? Estimate { get; set; }
    }
}