﻿namespace MobileMilk.Service
{
    public class TaskCompletedSummary
    {
        public string Task { get; set; }

        public TaskSummaryResult Result { get; set; }

        public string Context { get; set; }
    }
}
