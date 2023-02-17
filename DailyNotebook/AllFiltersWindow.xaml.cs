using DailyNotebook.Services;
using DailyNotebookApp.Models;
using System;
using System.Windows;

namespace DailyNotebook
{
    /// <summary>
    /// Логика взаимодействия для AllFiltersWindow.xaml
    /// </summary>
    public partial class AllFiltersWindow : Window
    {
        public bool AllowFilters { get; private set; }

        public AllFiltersWindow(double top, double left, double height, double width)
        {
            InitializeComponent();

            Top = top + height - Height - 10;
            Left = left + width - Width - 10;

            IsCompletedComboBox.ItemsSource = Enum.GetValues(typeof(IsCompletedEnum));
            IsCompletedComboBox.SelectedItem = IsCompletedEnum.All;
            PriorityComboBox.ItemsSource = Enum.GetValues(typeof(PriorityEnum));
            PriorityComboBox.SelectedItem = PriorityEnum.None;
            TypeOfTaskComboBox.ItemsSource = Enum.GetValues(typeof(TypeOfTaskEnum));
            TypeOfTaskComboBox.SelectedItem = TypeOfTaskEnum.None;
            HasSubtasksComboBox.ItemsSource = Enum.GetValues(typeof(SubtasksFilterEnum));
            HasSubtasksComboBox.SelectedItem = SubtasksFilterEnum.NoFilter;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AssignElements();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FilterService.ClearFilters();
            AssignElements();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AllowFilters = false;
            Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ShortDescriptionTextBox.Text))
                FilterService.ShortDescription = ShortDescriptionTextBox.Text;
            else FilterService.ShortDescription = null;

            FilterService.IsCompleted = (IsCompletedEnum)IsCompletedComboBox.SelectedItem;

            FilterService.CreationDate = CreationDatePicker.SelectedDate;
            FilterService.FinishToDate = FinishToDatePicker.SelectedDate;

            if (!string.IsNullOrWhiteSpace(FinishToHourTextBox.Text))
                FilterService.FinishToHour = int.Parse(FinishToHourTextBox.Text);
            else FilterService.FinishToHour = null;

            if (!string.IsNullOrWhiteSpace(FinishToMinutesTextBox.Text))
                FilterService.FinishToMinutes = int.Parse(FinishToMinutesTextBox.Text);
            else FilterService.FinishToMinutes = null;

            FilterService.Priority = (PriorityEnum)PriorityComboBox.SelectedItem;
            FilterService.TypeOfTask = (TypeOfTaskEnum)TypeOfTaskComboBox.SelectedItem;

            if (!string.IsNullOrWhiteSpace(DetailedDescriptionTextBox.Text))
                FilterService.DetailedDescription = DetailedDescriptionTextBox.Text;
            else FilterService.DetailedDescription = null;

            if (FilterService.CreationDate is not null)
                FilterService.DateRange = new DateRange(FilterService.CreationDate.ToString(), DateRangeStartDatePicker.SelectedDate, DateRangeEndDatePicker.SelectedDate);
            else FilterService.DateRange = new DateRange(DateTime.Now.ToString(), DateRangeStartDatePicker.SelectedDate, DateRangeEndDatePicker.SelectedDate);

            FilterService.HasSubtasks = (SubtasksFilterEnum)HasSubtasksComboBox.SelectedItem;

            AllowFilters = true;
            Close();
        }

        private void AssignElements()
        {
            ShortDescriptionTextBox.Text = FilterService.ShortDescription;

            if (FilterService.IsCompleted is not null)
                IsCompletedComboBox.SelectedItem = FilterService.IsCompleted;
            else IsCompletedComboBox.SelectedItem = IsCompletedEnum.All;

            CreationDatePicker.SelectedDate = FilterService.CreationDate;
            FinishToDatePicker.SelectedDate = FilterService.FinishToDate;
            FinishToHourTextBox.Text = FilterService.FinishToHour.ToString();
            FinishToMinutesTextBox.Text = FilterService.FinishToMinutes.ToString();

            if (FilterService.Priority is not null)
                PriorityComboBox.SelectedItem = FilterService.Priority;
            else PriorityComboBox.SelectedItem = PriorityEnum.None;

            if (FilterService.TypeOfTask is not null)
                TypeOfTaskComboBox.SelectedItem = FilterService.TypeOfTask;
            else TypeOfTaskComboBox.SelectedItem = TypeOfTaskEnum.None;

            DetailedDescriptionTextBox.Text = FilterService.DetailedDescription;
            DateRangeStartDatePicker.SelectedDate = FilterService.DateRange.Start;
            DateRangeEndDatePicker.SelectedDate = FilterService.DateRange.End;

            if (FilterService.HasSubtasks is not null)
                HasSubtasksComboBox.SelectedItem = FilterService.HasSubtasks;
            else HasSubtasksComboBox.SelectedItem = SubtasksFilterEnum.NoFilter;
        }
    }
}
