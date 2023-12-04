using DailyNotebook.Models;
using DailyNotebook.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DailyNotebook
{
    /// <summary>
    /// Логика взаимодействия для EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        public Task EditedTask { get; set; } = new Task();
        ObservableCollection<Subtask> Subtasks { get; set; } = new ObservableCollection<Subtask>();
        DateRange DateRange { get; set; }

        public EditTaskWindow(Task taskToEdit, double mainTop, double mainLeft, double mainHeight, double mainWidth)
        {
            InitializeComponent();

            Top = mainTop + mainHeight / 2 - Height / 2;
            Left = mainLeft + mainWidth / 2 - Width / 2;

            PriorityComboBox.ItemsSource = Enum.GetValues(typeof(PriorityEnum));
            TypeOfTaskComboBox.ItemsSource = Enum.GetValues(typeof(TypeOfTaskEnum));

            ShortDescriptionTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "ShortDescription"));
            IsCompletedCheckBox.SetBinding(CheckBox.IsCheckedProperty, HelpService.AddBinding(EditedTask, "IsCompleted"));
            FinishToDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(EditedTask, "FinishToDate"));
            FinishToHoursTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "FinishToHour"));
            FinishToMinutesTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "FinishToMinutes"));
            PriorityComboBox.SetBinding(ComboBox.SelectedItemProperty, HelpService.AddBinding(EditedTask, "Priority"));
            TypeOfTaskComboBox.SetBinding(ComboBox.SelectedItemProperty, HelpService.AddBinding(EditedTask, "TypeOfTask"));
            DetailedDescriptionTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "DetailedDescription"));

            CreationDateTextBlock.Text = taskToEdit.CreationDate;
            ShortDescriptionTextBox.Text = taskToEdit.ShortDescription;
            IsCompletedCheckBox.IsChecked = taskToEdit.IsCompleted;

            if (taskToEdit.DateRange != null )
            {
                DateRangeFirstDatePicker.SelectedDate = taskToEdit.DateRange.Start;
                DateRangeLastDatePicker.SelectedDate = taskToEdit.DateRange.End;
                DateRangeFirstDatePicker.Tag = "true";
                DateRangeLastDatePicker.Tag = "true";
                DateRange = new DateRange(CreationDateTextBlock.Text, DateRangeFirstDatePicker.SelectedDate, DateRangeLastDatePicker.SelectedDate);

                if (taskToEdit.Subtasks.Any())
                {
                    SubtasksControlService.AddSubtaskControls(SubtasksGrid,
                                                              DateDatePicker_SelectedDateChanged,
                                                              Subtasks,
                                                              taskToEdit.Subtasks,
                                                              DateRange);
                }
                else
                {
                    SubtasksControlService.AddSubtaskControls(SubtasksGrid,
                                                              DateDatePicker_SelectedDateChanged,
                                                              Subtasks,
                                                              DateRange);
                }
            }
            else
            {
                DateRangeFirstDatePicker.SelectedDate = null;
                DateRangeLastDatePicker.SelectedDate = null;
                DateRange = new DateRange(CreationDateTextBlock.Text, null, null);
            }

            FinishToDatePicker.SelectedDate = taskToEdit.FinishToDate;
            if (taskToEdit.FinishToHour != null )
                FinishToHoursTextBox.Text = taskToEdit.FinishToHour.ToString();
            if (taskToEdit.FinishToMinutes != null)
                FinishToMinutesTextBox.Text = taskToEdit.FinishToMinutes.ToString();
            PriorityComboBox.SelectedItem = taskToEdit.Priority;
            TypeOfTaskComboBox.SelectedItem = taskToEdit.TypeOfTask;
            DetailedDescriptionTextBox.Text = taskToEdit.DetailedDescription;

            EditedTask.DateRange = DateRange;
            EditedTask.Subtasks = Subtasks;

            DateRangeFirstDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(EditedTask.DateRange, "Start"));
            DateRangeLastDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(EditedTask.DateRange, "End"));

            DataContext = EditedTask;
            DateRangeGrid.DataContext = EditedTask.DateRange;
        }

        private void ElementGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HelpService.UpdateProperty(EditedTask, nameof(EditedTask.ShortDescription));
            HelpService.UpdateProperty(EditedTask, nameof(EditedTask.FinishToDate));
            HelpService.UpdateProperty(EditedTask, nameof(EditedTask.FinishToHour));
            HelpService.UpdateProperty(EditedTask, nameof(EditedTask.FinishToMinutes));
            HelpService.UpdateProperty(EditedTask.DateRange, nameof(EditedTask.DateRange.Start));
            HelpService.UpdateProperty(EditedTask.DateRange, nameof(EditedTask.DateRange.End));
            HelpService.UpdateProperty(EditedTask.Subtasks);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //EditedTask.CreationDate = CreationDateTextBlock.Text;
            //EditedTask.ShortDescription = ShortDescriptionTextBox.Text;
            //EditedTask.DateRange = DateRange;
            //EditedTask.Subtasks = Subtasks;
            //EditedTask.FinishToDate = FinishToDatePicker.SelectedDate;

            //if (double.TryParse(FinishToHoursTextBox.Text, out var finishToHours))
            //    EditedTask.FinishToHour = finishToHours;
            //else
            //    EditedTask.FinishToHour = null;

            //if (double.TryParse(FinishToMinutesTextBox.Text, out var finishToMinutes))
            //    EditedTask.FinishToMinutes = finishToMinutes;
            //else
            //    EditedTask.FinishToMinutes = null;

            //EditedTask.Priority = (PriorityEnum)PriorityComboBox.SelectedItem;
            //EditedTask.TypeOfTask = (TypeOfTaskEnum)TypeOfTaskComboBox.SelectedItem;
            //EditedTask.DetailedDescription = DetailedDescriptionTextBox.Text;

            //ShortDescriptionTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "ShortDescription"));
            //DateRangeFirstDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(EditedTask.DateRange, "Start"));
            //DateRangeLastDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(EditedTask.DateRange, "End"));
            //FinishToDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(EditedTask, "FinishToDate"));
            //FinishToHoursTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "FinishToHour"));
            //FinishToMinutesTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "FinishToMinutes"));
            //PriorityComboBox.SetBinding(ComboBox.SelectedItemProperty, HelpService.AddBinding(EditedTask, "Priority"));
            //TypeOfTaskComboBox.SetBinding(ComboBox.SelectedItemProperty, HelpService.AddBinding(EditedTask, "TypeOfTask"));
            //DetailedDescriptionTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(EditedTask, "DetailedDescription"));

            DateRangeFirstDatePicker.SelectedDateChanged += DateRangeFirstDatePicker_SelectedDateChanged;
            DateRangeLastDatePicker.SelectedDateChanged += DateRangeLastDatePicker_SelectedDateChanged;

            EditTaskWindowGrid.GotKeyboardFocus += ElementGotKeyboardFocus;

            Keyboard.ClearFocus();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditedTask.CanCreate = false;
            Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!EditedTask.HasErrors)
            {
                EditedTask.CanCreate = true;
                EditedTask.FinishTo = CheckNAssignService.CheckNAssignFinishTo(EditedTask.FinishToDate,
                                                                            EditedTask.FinishToHour,
                                                                            EditedTask.FinishToMinutes);
                EditedTask.DateRange = CheckNAssignService.CheckNAssignDateRange(EditedTask.CreationDate,
                                                                              DateRangeFirstDatePicker.SelectedDate,
                                                                              DateRangeLastDatePicker.SelectedDate);
                EditedTask.Subtasks = CheckNAssignService.CheckNAssignSubtasks(EditedTask.Subtasks, EditedTask.DateRange);

                Close();
            }
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
            HelpService.ManageControlsOnDateRangeChanged(DateRangeFirstDatePicker, DateRangeLastDatePicker, SubtasksGrid, DateDatePicker_SelectedDateChanged, EditedTask.Subtasks, EditedTask.DateRange);
        }

        private void DateRangeLastDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            HelpService.ManageControlsOnDateRangeChanged(DateRangeLastDatePicker, DateRangeFirstDatePicker, SubtasksGrid, DateDatePicker_SelectedDateChanged, EditedTask.Subtasks, EditedTask.DateRange);
        }

        private void DateDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DatePicker).Tag == null)
            {
                SubtasksControlService.AddSubtaskControls(SubtasksGrid, DateDatePicker_SelectedDateChanged, EditedTask.Subtasks, EditedTask.DateRange);
                (sender as DatePicker).Tag = "true";
            }
        }
    }
}
