using DailyNotebook.Models;
using DailyNotebookApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace DailyNotebookApp.Services
{
    public class DataBaseIOService
    {
        public static void AddTask(Task task, Worksheet worksheet)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Worksheets.Update(worksheet);
                db.Tasks.Add(task);
                if (task.Subtasks.Any())
                    db.Subtasks.AddRange(task.Subtasks);
                db.SaveChanges();
            }
        }

        public static void AddWorksheet(Worksheet worksheet)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Worksheets.Add(worksheet);
                db.SaveChanges();
            }
        }

        public static List<Worksheet> LoadWorksheets()
        {
            var worksheets = new List<Worksheet>();
            using (ApplicationContext db = new ApplicationContext())
                worksheets = db.Worksheets.Include(x => x.Tasks).ToList();
            return worksheets;
        }

        public static ObservableCollection<Task> LoadData(Worksheet worksheet)
        {
            using(ApplicationContext db = new ApplicationContext())
            {
                var tasksList = db.Tasks.Include(x => x.Subtasks).Where(x => x.WorksheetId == worksheet.Id).ToList();
                ObservableCollection<Task> tasks = new ObservableCollection<Task>();
                foreach (var task in tasksList)
                {
                    if (task.DateRangeString != null)
                    {
                        DateRange dateRange = new DateRange(task.CreationDate,
                            DateTime.Parse(task.DateRangeString.Substring(0, 10)),
                            DateTime.Parse(task.DateRangeString.Substring(13, 10)));

                        dateRange.FinishToDate = task.FinishToDate;

                        task.DateRange = dateRange;

                        if (task.Subtasks.Any())
                        {
                            foreach (var subtask in task.Subtasks)
                            {
                                subtask.DateRange = dateRange;
                            }
                        }
                    }
                    else
                        task.DateRange = null;

                    tasks.Add(task);
                }
                return tasks;
            }
        }

        public static void RemoveTask(Task taskToRemove)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Tasks.Remove(taskToRemove);
                db.SaveChanges();
            }
        }

        public static void DeleteWorksheet(Worksheet worksheet)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Worksheets.Remove(worksheet);
                db.SaveChanges();
            }
        }

        public static void UpdateTask(Task updatedTask)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Task oldTask = db.Tasks.Include(x => x.Subtasks).First(x => x.Id == updatedTask.Id);
                oldTask.Assign(updatedTask);
                db.SaveChanges();
            }
        }

        public static void UpdateWorksheet(Worksheet worksheet)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Worksheets.First(x => x.Id == worksheet.Id).LastOpenedDate = worksheet.LastOpenedDate;
                db.SaveChanges();
            }
        }

        public static void UpdateAll(ObservableCollection<Task> tasks, Worksheet worksheet)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var oldTasks = db.Tasks.Include(x => x.Subtasks).Where(x => x.WorksheetId == worksheet.Id).ToList();

                for (var i = 0; i < oldTasks.Count; i++)
                {
                    oldTasks[i].Assign(tasks[i]);
                }

                db.SaveChanges();
            }
        }
    }
}
