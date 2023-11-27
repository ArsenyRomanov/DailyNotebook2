using DailyNotebookApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace DailyNotebookApp.Services
{
    public class CheckNAssignService
    {
        public static string CheckNAssignFinishTo(DateTime? finishToDate, double? finishToHour, double? finishToMinutes)
        {
            if (finishToDate != null)
            {
                var result = finishToDate.Value;

                if (finishToHour != null)
                {
                    result = result.AddHours(finishToHour.Value);

                    if (finishToMinutes != null)
                    {
                        result = result.AddMinutes(finishToMinutes.Value);
                    }
                }

                return HelpService.FormatDateTimeOutput(result);
            }

            return "-";
        }

        public static DateRange CheckNAssignDateRange(string creationDate, DateTime? start, DateTime? end)
        {
            if (start != null && end != null)
            {
                return new DateRange(creationDate, start.Value, end.Value);
            }
            return null;
        }

        public static ObservableCollection<Subtask> CheckNAssignSubtasks(ObservableCollection<Subtask> subtasks, DateRange dateRange)
        {
            var count = subtasks.Count;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrWhiteSpace(subtasks[i].Description) || subtasks[i].Date == null)
                {
                    subtasks.RemoveAt(i);
                    i--;
                    count--;
                }
            }

            foreach (var subtask in subtasks)
            {
                subtask.DateRange = dateRange;
            }

            return subtasks;
        }
    }
}
