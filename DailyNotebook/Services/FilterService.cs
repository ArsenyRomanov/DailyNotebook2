using DailyNotebookApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DailyNotebook.Services
{
    public static class FilterService
    {
        public static bool FiltersOn { get; set; } = true;

        public delegate void FiltersHandler(IEnumerable<Task> collection);

        public static event Action? FiltersDisabled;
        public static event FiltersHandler? FiltersEnabled;

        public static string? ShortDescription { get; set; }
        public static IsCompletedEnum? IsCompleted { get; set; }
        public static DateTime? CreationDate { get; set; }
        public static DateTime? FinishToDate { get; set; }
        public static int? FinishToHour { get; set; }
        public static int? FinishToMinutes { get; set; }
        public static PriorityEnum? Priority { get; set; }
        public static TypeOfTaskEnum? TypeOfTask { get; set; }
        public static string? DetailedDescription { get; set; }
        public static DateRange? DateRange { get; set; } = new DateRange(DateTime.Now.ToString(), null, null);
        public static SubtasksFilterEnum? HasSubtasks { get; set; }

        public static void ClearFilters()
        {
            ShortDescription = null;
            IsCompleted = IsCompletedEnum.All;
            CreationDate = null;
            FinishToDate = null;
            FinishToHour = null;
            FinishToMinutes = null;
            Priority = PriorityEnum.None;
            TypeOfTask = TypeOfTaskEnum.None;
            DetailedDescription = null;
            DateRange.Start = null;
            DateRange.End = null;
            HasSubtasks = SubtasksFilterEnum.NoFilter;
        }

        public static void FilterCollection(BindingList<Task> tasks)
        {
            IEnumerable<Task> collection = tasks;

            if (!string.IsNullOrWhiteSpace(ShortDescription))
                collection = collection.Where(item => item.ShortDescription.Contains(ShortDescription));

            switch (IsCompleted)
            {
                case IsCompletedEnum.All:
                    collection = collection.Where(item => true);
                    break;
                case IsCompletedEnum.Completed:
                    collection = collection.Where(item => item.IsCompleted);
                    break;
                case IsCompletedEnum.Uncompleted:
                    collection = collection.Where(item => !item.IsCompleted);
                    break;
                default:
                    break;
            }

            if (CreationDate is not null)
                collection = collection.Where(item => DateTime.Parse(item.CreationDate[..10]) == CreationDate);
            if (FinishToDate is not null)
                collection = collection.Where(item => item.FinishToDate == FinishToDate);
            if (FinishToHour is not null)
                collection = collection.Where(item => item.FinishToHour == FinishToHour);
            if (FinishToMinutes is not null)
                collection = collection.Where(item => item.FinishToMinutes == FinishToMinutes);

            switch (Priority)
            {
                case PriorityEnum.None:
                    collection = collection.Where(item => true);
                    break;
                case PriorityEnum.Lowest:
                    collection = collection.Where(item => item.Priority == PriorityEnum.Lowest);
                    break;
                case PriorityEnum.Low:
                    collection = collection.Where(item => item.Priority == PriorityEnum.Low);
                    break;
                case PriorityEnum.Medium:
                    collection = collection.Where(item => item.Priority == PriorityEnum.Medium);
                    break;
                case PriorityEnum.High:
                    collection = collection.Where(item => item.Priority == PriorityEnum.High);
                    break;
                case PriorityEnum.Highest:
                    collection = collection.Where(item => item.Priority == PriorityEnum.Highest);
                    break;
                default:
                    break;
            }

            switch (TypeOfTask)
            {
                case TypeOfTaskEnum.None:
                    collection = collection.Where(item => true);
                    break;
                case TypeOfTaskEnum.Home:
                    collection = collection.Where(item => item.TypeOfTask == TypeOfTaskEnum.Home);
                    break;
                case TypeOfTaskEnum.Business:
                    collection = collection.Where(item => item.TypeOfTask == TypeOfTaskEnum.Business);
                    break;
                case TypeOfTaskEnum.Study:
                    collection = collection.Where(item => item.TypeOfTask == TypeOfTaskEnum.Study);
                    break;
                case TypeOfTaskEnum.Hobby:
                    collection = collection.Where(item => item.TypeOfTask == TypeOfTaskEnum.Hobby);
                    break;
                case TypeOfTaskEnum.Entertainment:
                    collection = collection.Where(item => item.TypeOfTask == TypeOfTaskEnum.Entertainment);
                    break;
                case TypeOfTaskEnum.Meeting:
                    collection = collection.Where(item => item.TypeOfTask == TypeOfTaskEnum.Meeting);
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(DetailedDescription))
                collection = collection.Where(item => item.DetailedDescription.Contains(DetailedDescription));

            if (DateRange.Start is not null && DateRange.End is not null)
                collection = collection.Where(item => item.DateRange is not null &&
                                                      item.DateRange?.Start == DateRange.Start &&
                                                      item.DateRange?.End == DateRange.End);
            else if (DateRange.Start is not null && DateRange.End is null)
                collection = collection.Where(item => item.DateRange is not null && item.DateRange?.Start == DateRange.Start);
            else if (DateRange.Start is null && DateRange.End is not null)
                collection = collection.Where(item => item.DateRange is not null && item.DateRange?.End == DateRange.End);

            switch (HasSubtasks)
            {
                case SubtasksFilterEnum.NoFilter:
                    collection = collection.Where(item => true);
                    break;
                case SubtasksFilterEnum.HaveSubtasks:
                    collection = collection.Where(item => item.Subtasks is not null && item.Subtasks.Any());
                    break;
                case SubtasksFilterEnum.NoSubtasks:
                    collection = collection.Where(item => item.Subtasks is not null && !item.Subtasks.Any());
                    break;
                default:
                    break;
            }

            if (collection.ToList().Count == tasks.Count)
            {
                FiltersOn = false;
                FiltersDisabled?.Invoke();
                return;
            }
            else
            {
                FiltersOn = true;
                FiltersEnabled?.Invoke(collection);
                return;
            }
        }
    }

    public enum SubtasksFilterEnum
    {
        NoFilter,
        HaveSubtasks,
        NoSubtasks
    }
}
