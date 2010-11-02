using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using MobileMilk.Common;

namespace MobileMilk.ViewModels.Task
{
    [DataContract]
    public class TaskViewModel : ViewModel
    {
        #region Members
        #endregion Members

        public TaskViewModel(Model.Task taskItem, INavigationService navigationService) 
            : base(navigationService)
        {
            this.TaskItem = taskItem;

            this.IsBeingActivated();
        }

        #region Properties

        [DataMember]
        public Model.Task TaskItem { get; set; }

        [DataMember]
        public string Name { get { return this.TaskItem.Name; } }

        [DataMember]
        public int Priority { get { return this.TaskItem.Priority; } }
        public string PriorityAsString { get { return this.TaskItem.Priority.ToString(); } }
        public string PriorityColour
        {
            get
            {
                switch (this.Priority)
                {
                    case 1:
                        return "#EA5200";
                    case 2:
                        return "#0060BF";
                    case 3:
                        return "#359AFF";
                    default:
                        return Colors.Transparent.ToString();
                }
            }
        }

        [DataMember]
        public DateTime? Due { get { return this.TaskItem.Due; } }
        public bool HasDue { get { return this.TaskItem.Due != null && this.TaskItem.Due != DateTime.MinValue; } }
        public string DueAsString
        {
            get
            {
                var dueString = "";
                if (this.HasDue)
                {
                    var due = this.Due ?? DateTime.MinValue;
                    if (due == DateTime.Today)
                        dueString += "Today";
                    else if (DateTime.Today.AddDays(1) == due)
                        dueString += "Tomorrow";
                    else if (DateTime.Today < due && DateTime.Today.AddDays(6) >= due)
                        dueString += due.ToString("dddd");
                    else
                        dueString += due.ToString("ddd d MMM");

                    //if (this.HasDueTime)
                    //    dueString += " " + due.ToString("t");
                }

                return dueString;
            }
        }

        [DataMember]
        public DateTime? Created { get { return this.TaskItem.Created; } }

        [DataMember]
        public DateTime? Completed { get { return this.TaskItem.Completed; } }

        [DataMember]
        public DateTime? Deleted { get { return this.TaskItem.Deleted; } }

        public bool HasNotes { get { return this.TaskItem.Notes.Count != 0; } }
        public int NoteCount { get { return this.TaskItem.Notes.Count; } }

        public string TagsAsString { get { return string.Join(",", this.TaskItem.Tags.ToArray()); } }
        public bool HasTags { get { return this.TaskItem.Tags.Count != 0; } }
        public int TagCount { get { return this.TaskItem.Tags.Count; } }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated() {}

        #endregion Methods
    }
}
