using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using MobileMilk.Common;
using MobileMilk.Model;
using MobileMilk.Service;

namespace MobileMilk.ViewModels
{
    [DataContract]
    public class TaskViewModel : ViewModel
    {
        #region Delegates

        public DelegateCommand MarkCompleteCommand { get; set; }
        public DelegateCommand MarkPostponeCommand { get; set; }
        public DelegateCommand TaskCommand { get; set; }

        #endregion Delegates

        #region Members

        private ISynchronizationService _synchronizationService;

        #endregion Members

        public TaskViewModel(Model.Task taskItem, DelegateCommand taskCommand, 
            INavigationService navigationService, ISynchronizationService synchronizationService) 
            : base(navigationService)
        {
            this.TaskItem = taskItem;

            this._synchronizationService = synchronizationService;

            this.TaskCommand = taskCommand;
            this.MarkCompleteCommand = new DelegateCommand(MarkCompleteCommandDelegate);
            this.MarkPostponeCommand = new DelegateCommand(MarkPostponeCommandDelegate);

            this.IsBeingActivated();
        }

        #region Properties

        [DataMember]
        public Task TaskItem { get; set; }

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
                var dueString = "Whenever";
                if (this.HasDue)
                {
                    dueString = "";
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
        public int Postponed { get { return this.TaskItem.Postponed; } }

        [DataMember]
        public DateTime? Added { get { return this.TaskItem.Added; } }

        [DataMember]
        public DateTime? Created { get { return this.TaskItem.Created; } }

        [DataMember]
        public DateTime? Completed { get { return this.TaskItem.Completed; } }

        [DataMember]
        public DateTime? Deleted { get { return this.TaskItem.Deleted; } }
        
        [DataMember]
        public string Url { get { return this.TaskItem.Url; } }
        public bool HasUrl { get { return !string.IsNullOrEmpty(this.TaskItem.Url); } }

        [DataMember]
        public DateTime? Estimate { get { return this.TaskItem.Estimate; } }
        public bool HasEstimate { get { return this.TaskItem.Estimate != null; } }

        [DataMember]
        public List<string> Tags { get { return this.TaskItem.Tags; } }
        public string TagsAsString { get { return string.Join(",", this.TaskItem.Tags.ToArray()); } }
        public bool HasTags { get { return this.TaskItem.HasTags; } }
        public int TagCount { get { return this.TaskItem.Tags.Count; } }

        [DataMember]
        public List<Note> Notes { get { return this.TaskItem.Notes; } }
        public bool HasNotes { get { return this.TaskItem.HasNotes; } }
        public int NoteCount { get { return this.TaskItem.Notes.Count; } }

        [DataMember]
        public List<User> Participants { get { return this.TaskItem.Participants; } }
        public bool HasParticipants { get { return this.TaskItem.HasParticipants; } }
        public int ParticipantsCount { get { return this.TaskItem.Participants.Count; } }

        #endregion Properties

        #region Methods

        public override void IsBeingActivated() {}

        public void MarkCompleteCommandDelegate()
        {
            this._synchronizationService.CompleteTask(this.TaskItem);
            this.RaisePropertyChanged(() => this.Completed);
        }

        public void MarkPostponeCommandDelegate()
        {
            this._synchronizationService.PostponeTask(this.TaskItem);
            this.RaisePropertyChanged(() => this.Postponed);
            this.RaisePropertyChanged(() => this.Due);
        }

        #endregion Methods
    }
}
