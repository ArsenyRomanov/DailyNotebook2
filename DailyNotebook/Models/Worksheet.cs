using DailyNotebookApp.Models;
using System;
using System.Collections.ObjectModel;

namespace DailyNotebook.Models
{
    public class Worksheet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastOpenedDate { get; set; } = DateTime.Now;
        public ObservableCollection<Task> Tasks { get; set; }
    }
}
