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
using MobileMilk.Model;
using System.Collections.Generic;

namespace MobileMilk.Store
{
    public class TaskList : List<Task>
    {
        public TaskList()
        {
            this.LastSyncDate = string.Empty;
        }
        
        public string LastSyncDate { get; set; }
    }
}
