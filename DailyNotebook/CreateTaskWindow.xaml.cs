using DailyNotebook.Models;
using DailyNotebook.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DailyNotebook
{
    /// <summary>
    /// Логика взаимодействия для CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        private Task NewTask { get; set; }

        public CreateTaskWindow(Task newTask, double mainTop, double mainLeft, double mainHeight, double mainWidth)
        {
            InitializeComponent();

            Top = mainTop + mainHeight / 2 - Height / 2;
            Left = mainLeft + mainWidth / 2 - Width / 2;

            NewTask = newTask;
            DataContext = NewTask;
            DateRangeGrid.DataContext = NewTask.DateRange;

            //ShortDescriptionTextBox.GotKeyboardFocus += ElementGotKeyboardFocus;
            //FinishToDatePicker.GotKeyboardFocus += ElementGotKeyboardFocus;
            //FinishToHoursTextBox.GotKeyboardFocus += ElementGotKeyboardFocus;
            //FinishToMinutesTextBox.GotKeyboardFocus += ElementGotKeyboardFocus;
            //PriorityComboBox.GotKeyboardFocus += ElementGotKeyboardFocus;
            //TypeOfTaskComboBox.GotKeyboardFocus += ElementGotKeyboardFocus;
            //DetailedDescriptionTextBox.GotKeyboardFocus += ElementGotKeyboardFocus;
            //DateRangeFirstDatePicker.GotKeyboardFocus += ElementGotKeyboardFocus;
            //DateRangeLastDatePicker.GotKeyboardFocus += ElementGotKeyboardFocus;
            CreateTaskWindowGrid.GotKeyboardFocus += ElementGotKeyboardFocus;
        }

        private void ElementGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HelpService.UpdateProperty(NewTask, nameof(NewTask.ShortDescription));
            HelpService.UpdateProperty(NewTask, nameof(NewTask.FinishToDate));
            HelpService.UpdateProperty(NewTask, nameof(NewTask.FinishToHour));
            HelpService.UpdateProperty(NewTask, nameof(NewTask.FinishToMinutes));
            HelpService.UpdateProperty(NewTask.DateRange, nameof(NewTask.DateRange.Start));
            HelpService.UpdateProperty(NewTask.DateRange, nameof(NewTask.DateRange.End));
            HelpService.UpdateProperty(NewTask.Subtasks);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void СreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ShortDescriptionTextBox.Text) && !NewTask.HasErrors)
            {
                HelpService.UpdateProperty(NewTask, nameof(NewTask.ShortDescription));
            }

            if (!NewTask.HasErrors)
            {
                NewTask.CanCreate = true;
                NewTask.IsCompleted = false;
                NewTask.FinishTo = CheckNAssignService.CheckNAssignFinishTo(NewTask.FinishToDate,
                                                                            NewTask.FinishToHour,
                                                                            NewTask.FinishToMinutes);
                NewTask.DateRange = CheckNAssignService.CheckNAssignDateRange(NewTask.CreationDate,
                                                                              DateRangeFirstDatePicker.SelectedDate,
                                                                              DateRangeLastDatePicker.SelectedDate);

                NewTask.Subtasks = CheckNAssignService.CheckNAssignSubtasks(NewTask.Subtasks, NewTask.DateRange);

                //if (NewTask.DateRange != null)
                //    NewTask.DateRange.Task = NewTask;

                //if (NewTask.Subtasks.Any())
                //{
                //    foreach (var subtask in NewTask.Subtasks)
                //    {
                //        subtask.Task = NewTask;
                //        subtask.DateRange = NewTask.DateRange;
                //    }

                //    NewTask.DateRange.Subtasks = NewTask.Subtasks.ToList();
                //}

                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreationDateTextBlock.Text = NewTask.CreationDate;
            PriorityComboBox.ItemsSource = Enum.GetValues(typeof(PriorityEnum));
            PriorityComboBox.SelectedItem = PriorityEnum.None;
            TypeOfTaskComboBox.ItemsSource = Enum.GetValues(typeof(TypeOfTaskEnum));
            TypeOfTaskComboBox.SelectedItem = TypeOfTaskEnum.None;
        }

        private void AdditionalInfoExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Width += 422;
            Left -= 211;
        }

        private void AdditionalInfoExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            Width -= 422;
            Left += 211;
        }

        private void FinishToHoursTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse((sender as TextBox).Text, out _))
                (sender as TextBox).Text = null;

            if ((sender as TextBox).Text.Length == 2)
            {
                FinishToMinutesTextBox.Focus();
                FinishToMinutesTextBox.Select(0, 0);
            }
        }

        private void FinishToMinutesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse((sender as TextBox).Text, out _))
                (sender as TextBox).Text = null;

            if ((sender as TextBox).Text.Length == 2)
            {
                Keyboard.ClearFocus();
            }
        }

        private void DateRangeFirstDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            HelpService.ManageControlsOnDateRangeChanged(DateRangeFirstDatePicker, DateRangeLastDatePicker, SubtasksGrid, DateDatePicker_SelectedDateChanged, NewTask.Subtasks, NewTask.DateRange);
        }

        private void DateRangeLastDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            HelpService.ManageControlsOnDateRangeChanged(DateRangeLastDatePicker, DateRangeFirstDatePicker, SubtasksGrid, DateDatePicker_SelectedDateChanged, NewTask.Subtasks, NewTask.DateRange);
        }

        private void DateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DatePicker).Tag == null)
            {
                SubtasksControlService.AddSubtaskControls(SubtasksGrid, DateDatePicker_SelectedDateChanged, NewTask.Subtasks, NewTask.DateRange);
                (sender as DatePicker).Tag = "true";
            }
        }
    }
}
