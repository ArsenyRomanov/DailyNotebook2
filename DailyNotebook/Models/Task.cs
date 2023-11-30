using DailyNotebook.Models;
using DailyNotebookApp.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DailyNotebookApp.Models
{
    public class Task : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string shortDescription;
        private bool isCompleted;
        private string finishTo;
        private DateTime? finishToDate;
        private double? finishToHour;
        private double? finishToMinutes;
        private PriorityEnum priority;
        private TypeOfTaskEnum typeOfTask;
        private string? detailedDescription;
        private DateRange? dateRange;
        private ObservableCollection<Subtask>? subtasks;

        private readonly Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        public int Id { get; set; }
        public int WorksheetId { get; set; }
        public Worksheet Worksheet { get; set; }

        public bool CanCreate { get; set; } = false;

        public bool HasErrors
        {
            get
            {
                var subtasksHasErrors = false;
                if (Subtasks.Any(item => item.HasErrors))
                    subtasksHasErrors = true;
                if (!propertyErrors.Any() && DateRange != null && !DateRange.HasErrors && !subtasksHasErrors)
                    return false;
                else
                    return true;
            }
        }

        public string CreationDate { get; set; }

        public string FinishTo
        {
            get { return finishTo; }
            set
            {
                finishTo = value;
                OnPropertyChanged(nameof(FinishTo));
            }
        }

        public DateTime? FinishToDate
        {
            get { return finishToDate; }
            set
            {
                finishToDate = value;
                OnPropertyChanged(nameof(FinishToDate));
                if (DateRange != null)
                    DateRange.FinishToDate = finishToDate;
                RemoveError(nameof(FinishToDate));
                if (finishToDate < DateTime.Parse(CreationDate))
                    AddError(nameof(FinishToDate), "Finish to date cannot be earlier than creation date");
                if ((FinishToHour != null || FinishToMinutes != null) && finishToDate == null)
                    AddError(nameof(FinishToDate), "Finish To time is specified, specify Finish To date");
                if (DateRange != null && (DateRange.Start != null || DateRange.End != null) && finishToDate == null)
                    AddError(nameof(FinishToDate), "Date range is specified, specify Finish To date");
            }
        }

        public double? FinishToHour
        {
            get { return finishToHour; }
            set
            {
                finishToHour = value;
                OnPropertyChanged(nameof(FinishToHour));
                RemoveError(nameof(FinishToHour));
                if (finishToHour < 0 || finishToHour > 24)
                    AddError(nameof(FinishToHour), "Invalid hour input");
                if (FinishToMinutes != null && finishToHour == null)
                    AddError(nameof(FinishToHour), "Finish To minutes is specified, specify Finish To hour");
            }
        }

        public double? FinishToMinutes
        {
            get { return finishToMinutes; }
            set
            {
                finishToMinutes = value;
                OnPropertyChanged(nameof(FinishToMinutes));
                RemoveError(nameof(FinishToMinutes));
                if (finishToMinutes < 0 || finishToMinutes > 60)
                    AddError(nameof(FinishToMinutes), "Invalid minutes input");
            }
        }

        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                if (shortDescription == value) return;
                shortDescription = value;
                OnPropertyChanged(nameof(ShortDescription));
                RemoveError(nameof(ShortDescription));
                if (string.IsNullOrWhiteSpace(shortDescription))
                    AddError(nameof(ShortDescription), "Short description cannot be empty");
                if (shortDescription.Length < 3 || shortDescription.Length > 50)
                    AddError(nameof(ShortDescription), "Short description should be between 3 and 50 symbols");
            }
        }

        public PriorityEnum Priority
        {
            get { return priority; }
            set 
            {
                if (Enum.IsDefined(typeof(PriorityEnum), value))
                {
                    priority = value;
                    OnPropertyChanged(nameof(Priority));
                }
            }
        }

        public TypeOfTaskEnum TypeOfTask
        {
            get { return typeOfTask; }
            set
            {
                if (Enum.IsDefined(typeof(TypeOfTaskEnum), value))
                {
                    typeOfTask = value;
                    OnPropertyChanged(nameof(TypeOfTask));
                }
            }
        }

        public string? DetailedDescription
        {
            get { return detailedDescription; }
            set 
            {
                detailedDescription = value;
                OnPropertyChanged(nameof(DetailedDescription));
                RemoveError(nameof(DetailedDescription));
                if (detailedDescription != null && detailedDescription.Length > 210)
                    AddError(nameof(DetailedDescription), "Detailed description cannot be of more than 210 symbols");
            }
        }

        public string? DateRangeString { get; set; }

        public DateRange? DateRange
        {
            get { return dateRange; }
            set
            {
                dateRange = value;
                if (dateRange != null)
                    DateRangeString = dateRange.ToString();
                OnPropertyChanged(nameof(DateRange));
            }
        }

        public ObservableCollection<Subtask>? Subtasks
        {
            get { return subtasks; }
            set
            {
                subtasks = value;
                OnPropertyChanged(nameof(Subtasks));
            }
        }

        public Task()
        {
            CreationDate = HelpService.FormatDateTimeOutput();
            DateRange = new DateRange(CreationDate, null, null);
            Subtasks = new ObservableCollection<Subtask>();
        }

        public void Assign(Task task)
        {
            DateRange = task.DateRange;
            Subtasks = task.Subtasks;
            CanCreate = task.CanCreate;
            FinishTo = task.FinishTo;
            FinishToDate = task.FinishToDate;
            FinishToHour = task.FinishToHour;
            FinishToMinutes = task.FinishToMinutes;
            ShortDescription = task.ShortDescription;
            Priority = task.Priority;
            TypeOfTask = task.TypeOfTask;
            DetailedDescription = task.DetailedDescription;
            IsCompleted = task.IsCompleted;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyErrors.TryGetValue(propertyName, out _))
                return propertyErrors[propertyName];
            return default;
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (!propertyErrors.ContainsKey(propertyName))
            {
                propertyErrors.Add(propertyName, new List<string>());
            }

            propertyErrors[propertyName].Add(errorMessage);
            OnErrorsChanged(propertyName);
        }

        public void RemoveError(string propertyName)
        {
            if (propertyErrors.Remove(propertyName))
                OnErrorsChanged(propertyName);
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool Coequals(Task task)
        {
            var subtaskListsEquals = true;
            if (Subtasks.Count == task.Subtasks.Count)
                for (int i = 0; i < Subtasks.Count; i++)
                {
                    if (!Subtasks[i].Coequals(task.Subtasks[i]))
                        subtaskListsEquals = false;
                }
            else
                subtaskListsEquals = false;

            bool dateRangesEquals;
            if (DateRange != null && task.DateRange != null)
            {
                if (DateRange.Coequals(task.DateRange))
                    dateRangesEquals = true;
                else
                    dateRangesEquals = false;
            }
            else if (DateRange == null && task.DateRange == null)
                dateRangesEquals = true;
            else
                dateRangesEquals = false;


            return (this.CreationDate == task.CreationDate) &&
                   (dateRangesEquals) &&
                   (subtaskListsEquals) &&
                   (this.FinishToDate == task.FinishToDate) &&
                   (this.FinishToHour == task.FinishToHour) &&
                   (this.FinishToMinutes == task.FinishToMinutes) &&
                   (this.IsCompleted == task.IsCompleted) &&
                   (this.ShortDescription == task.ShortDescription) &&
                   (this.Priority == task.Priority) &&
                   (this.TypeOfTask == task.TypeOfTask) &&
                   (this.DetailedDescription == task.DetailedDescription);
        }
    }

    public enum PriorityEnum
    {
        None,
        Lowest,
        Low,
        Medium,
        High,
        Highest
    }

    public enum TypeOfTaskEnum
    {
        None,
        Home,
        Business,
        Study,
        Hobby,
        Entertainment,
        Meeting
    }
}
