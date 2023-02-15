using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DailyNotebookApp.Models
{
    public class Subtask : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private bool isCompleted;
        private DateTime? date;
        private string description;
        private DateRange dateRange;

        public int Id { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        //public int DateRangeId { get; set; }

        public bool HasErrors => propertyErrors.Any();

        private readonly Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        public string DateRangeString { get; set; }

        public DateRange DateRange
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

        public int OrdinalNumber { get; set; }

        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public DateTime? Date
        {
            get { return date; }
            set
            {
                date = value;
                if (date != null)
                    DateString = date.Value.ToShortDateString();
                RemoveError(nameof(Date));
                if ((DateRange.Start > Date || Date > DateRange.End) && date != null)
                    AddError(nameof(Date), "Date of the subtask cannot exceed the boundaries of the Date range");
                if (!string.IsNullOrWhiteSpace(Description) && date == null)
                    AddError(nameof(Date), "Description of the subtask is specified, specify Date of the subtask");
            }
        }

        public string DateString { get; set; }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RemoveError(nameof(Description));
                if (!string.IsNullOrWhiteSpace(description) && description.Length > 50)
                    AddError(nameof(Description), "Description cannot be of more than 50 symbols");
                if (date != null && string.IsNullOrWhiteSpace(Description))
                    AddError(nameof(Description), "Date of the subtask is specified, specify description of the subtask");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public Subtask(DateRange dateRange)
        {
            DateRange = dateRange;
        }
        public Subtask()
        {
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
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Coequals(Subtask subtask)
        {
            return (this.Date == subtask.Date) &&
                   (this.DateRange.Coequals(subtask.DateRange)) &&
                   (this.Description == subtask.Description) &&
                   (this.IsCompleted == subtask.IsCompleted) &&
                   (this.OrdinalNumber == subtask.OrdinalNumber);
        }
    }
}
