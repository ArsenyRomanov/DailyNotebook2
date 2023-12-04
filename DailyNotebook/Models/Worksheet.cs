using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DailyNotebook.Models
{
    public class Worksheet : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }
        private string name;
        public DateTime LastOpenedDate { get; set; } = DateTime.Now;
        public string? LastOpenedString { get; set; }
        public string? TasksCount { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }

        private readonly Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        public string Name
        {
            get {  return name; }
            set
            {
                if (name == value) return;
                name = value;
                RemoveError(nameof(Name));
                if (string.IsNullOrWhiteSpace(name))
                    AddError(nameof(Name), "Worksheet name cannot be empty");
                if (name.Length < 3 || name.Length > 50)
                    AddError(nameof(Name), "Worksheet name should bw between 3 and 50 symbols");
            }
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (!propertyErrors.ContainsKey(propertyName))
                propertyErrors.Add(propertyName, new List<string>());

            propertyErrors[propertyName].Add(errorMessage);
            OnErrorsChanged(propertyName);
        }

        public void RemoveError(string propertyName)
        {
            if (propertyErrors.Remove(propertyName))
                OnErrorsChanged(propertyName);
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool HasErrors
        {
            get
            {
                if (!propertyErrors.Any())
                    return false;
                else
                    return true;
            }
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyErrors.TryGetValue(propertyName, out _))
                return propertyErrors[propertyName];
            return default;
        }
    }
}
