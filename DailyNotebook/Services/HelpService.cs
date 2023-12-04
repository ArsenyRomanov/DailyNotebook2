using DailyNotebook.Models;
using DailyNotebook.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DailyNotebook.Services
{
    public static class HelpService
    {
        private static ResourceManager rm = new ResourceManager(typeof(Resources));

        public static string FormatDateTimeOutput()
        {
            return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }

        public static string FormatDateTimeOutput(DateTime dateTime)
        {
            return dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
        }

        public static string FormatDateTimeOutput(string dateTimeString)
        {
            if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
            {
                return dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
            }
            return "-";
        }

        public static void MarkDateRangeInCalendar(Calendar calendar, DateRange dateRange, string finishToDate)
        {
            calendar.SelectedDates.AddRange(dateRange.Start.Value, dateRange.End.Value);
            DateTime.TryParse(finishToDate, out DateTime finishToDateTime);

            if (!dateRange.Contains(finishToDateTime))
            {
                calendar.SelectedDates.Add(finishToDateTime.Date);
            }
        }

        public static void UpdateProperty(Task task, string propertyName)
        {
            var taskType = typeof(Task);
            var propertyInfo = taskType.GetProperty(propertyName);
            var prevValue = propertyInfo.GetValue(task);

            if (prevValue != null)
                propertyInfo.SetValue(task, prevValue);
            else
            {
                if (propertyInfo.PropertyType == typeof(string))
                    propertyInfo.SetValue(task, string.Empty);
                if (propertyInfo.PropertyType == typeof(DateTime?) || propertyInfo.PropertyType == typeof(double?))
                    propertyInfo.SetValue(task, null);
            }
        }

        public static void UpdateProperty(DateRange dateRange, string propertyName)
        {
            var dateRangeType = typeof(DateRange);
            var propertyInfo = dateRangeType.GetProperty(propertyName);
            var prevValue = propertyInfo.GetValue(dateRange);
            propertyInfo.SetValue(dateRange, prevValue);
        }

        public static void UpdateProperty(ObservableCollection<Subtask> subtasks)
        {
            foreach (var subtask in subtasks)
            {
                var prevDescription = subtask.Description;
                var prevDate = subtask.Date;

                if (prevDescription != null)
                    subtask.Description = prevDescription;
                else
                    subtask.Description = string.Empty;

                if (prevDate != null)
                    subtask.Date = prevDate;
                else
                    subtask.Date = null;
            }
        }

        public static void ManageControlsOnDateRangeChanged(DatePicker current, DatePicker second, Grid subtasksGrid, EventHandler<SelectionChangedEventArgs> selectedDateChanged, ObservableCollection<Subtask> subtasks, DateRange dateRange)
        {
            if (current.SelectedDate == null && subtasksGrid.RowDefinitions.Count != 0)
            {
                subtasksGrid.Children.RemoveRange(0, subtasksGrid.Children.Count);
                subtasksGrid.RowDefinitions.RemoveRange(0, subtasksGrid.RowDefinitions.Count);
                subtasks.Clear();
                current.Tag = null;
                second.Tag = null;
                return;
            }

            if (current.Tag != null)
                return;

            if (current.SelectedDate != null && second.SelectedDate != null)
            {
                SubtasksControlService.AddSubtaskControls(subtasksGrid, selectedDateChanged, subtasks, dateRange);
                current.Tag = "true";
                second.Tag = "true";
            }
        }

        public static Binding AddBinding(Task source, string propertyName)
        {
            return new Binding
            {
                Source = source,
                Path = new PropertyPath(propertyName),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }

        public static Binding AddBinding(Subtask source, string propertyName)
        {
            return new Binding
            {
                Source = source,
                Path = new PropertyPath(propertyName),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }

        public static Binding AddBinding(DateRange source, string propertyName)
        {
            return new Binding
            {
                Source = source,
                Path = new PropertyPath(propertyName),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }

        public static void InsertToListBox(ListBox listBox, string message)
        {
            listBox.Items.Add(message);

            if (listBox.Items.Count > 1)
            {
                for (int i = listBox.Items.Count - 1; i > 0; i--)
                {
                    listBox.Items[i] = listBox.Items[i - 1];
                }

                listBox.Items[0] = message;
            }
        }

        public static void SaveNInsertToListBox(ListBox listBox, string savePath, string key, string taskName, string worksheetName)
        {
            string message = $"{DateTime.Now.ToShortTimeString()}\t{rm.GetString(key)} \"{taskName}\"# worksheet- {worksheetName}";
            using (var sw = new StreamWriter(savePath, true)) sw.WriteLine(message);
            InsertToListBox(listBox, message.Split('#')[0]);
        }

        public static void RefreshWorksheetControls(ListBox listBox, TextBlock textBlock, List<Worksheet> worksheets)
        {
            listBox.Items.Clear();

            foreach (var worksheet in worksheets.OrderBy(x => x.LastOpenedDate))
                InsertToListBox(listBox, $"{worksheet.Name}\t\t" +
                    $"Tasks: {(worksheet.Tasks == null ? 0 : worksheet.Tasks.Count)}\t\t" +
                    $"{(worksheet.LastOpenedDate == DateTime.Today ? 0 : (DateTime.Today - worksheet.LastOpenedDate).Days)} days ago");

            textBlock.Text = $"Worksheets: {worksheets.Count.ToString()}";
        }
    }
}
