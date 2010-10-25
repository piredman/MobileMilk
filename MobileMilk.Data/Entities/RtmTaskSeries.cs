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
    public class RtmTaskSeries
    {
        public string Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string LocationId { get; set; }

        public List<string> Tags { get; set; }
        public List<RtmUser> Participants { get; set; }
        public List<RtmNote> Notes { get; set; }

        public RtmTask Task { get; set; }
    }
}