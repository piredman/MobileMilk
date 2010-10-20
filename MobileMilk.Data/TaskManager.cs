using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MobileMilk.Data
{
    public enum TaskDateScope
    {
        Today,
        Tomorrow,
        ThisWeek,
        NextWeek
    }

    public class TaskManager
    {
        public List<string> GetTasks(TaskDateScope scope)
        {
            switch (scope)
            {
                case TaskDateScope.Today:
                    return new List<string> { "TodayTask1", "TodayTask2", "TodayTask3" };
                case TaskDateScope.Tomorrow:
                    return new List<string> { "TomorrowTask1", "TomorrowTask2", "TomorrowTask3" };
                case TaskDateScope.ThisWeek:
                    return new List<string> { "ThisWeekTask1", "ThisWeekTask2", "ThisWeekTask3" };
                case TaskDateScope.NextWeek:
                    return new List<string> { "NextWeekTask1", "NextWeekTask2", "NextWeekTask3" };
                default:
                    return new List<string>();
            }
        }

        private List<string> GetTasks(DateTime startDate, DateTime endDate)
        {
            //IronCow.
            return new List<string>();
        }
    }
}
