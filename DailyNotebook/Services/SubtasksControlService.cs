using DailyNotebookApp.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DailyNotebookApp.Services
{
    public class SubtasksControlService
    {
        public static void AddSubtaskControls(Grid subtasksGrid,
                                              EventHandler<SelectionChangedEventArgs> selectedDateChanged,
                                              BindingList<Subtask> subtasks,
                                              DateRange dateRange)
        {
            subtasksGrid.RowDefinitions.Add(new RowDefinition());
            int rowCount = subtasksGrid.RowDefinitions.Count;

            var newSubtask = new Subtask(dateRange)
            {
                OrdinalNumber = rowCount
            };
            subtasks.Add(newSubtask);

            var newControls = GetControls(rowCount);

            var newOrdinalNumberTextBlock = newControls.OrdinalNumberTextBlock;
            var newDateDatePicker = newControls.DateDatePicker;
            var newDescriptionTextBox = newControls.DescriptionTextBox;

            newDescriptionTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(subtasks[rowCount - 1], "Description"));
            newDateDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(subtasks[rowCount - 1], "Date"));

            newDateDatePicker.SelectedDateChanged += selectedDateChanged;

            Grid.SetRow(newOrdinalNumberTextBlock, rowCount - 1);
            Grid.SetRow(newDateDatePicker, rowCount - 1);
            Grid.SetRow(newDescriptionTextBox, rowCount - 1);
            subtasksGrid.Children.Add(newOrdinalNumberTextBlock);
            subtasksGrid.Children.Add(newDateDatePicker);
            subtasksGrid.Children.Add(newDescriptionTextBox);
        }

        public static void AddSubtaskControls(Grid subtasksGrid,
                                              EventHandler<SelectionChangedEventArgs> selectedDateChanged,
                                              BindingList<Subtask> subtasks,
                                              BindingList<Subtask> oldSubtasks,
                                              DateRange dateRange)
        {
            for (int i = 0; i <= oldSubtasks.Count; i++)
            {
                subtasksGrid.RowDefinitions.Add(new RowDefinition());
                int rowCount = subtasksGrid.RowDefinitions.Count;

                var newSubtask = new Subtask(dateRange)
                {
                    OrdinalNumber = rowCount
                };

                subtasks.Add(newSubtask);

                var newControls = GetControls(rowCount);

                var newOrdinalNumberTextBlock = newControls.OrdinalNumberTextBlock;
                var newDateDatePicker = newControls.DateDatePicker;
                var newDescriptionTextBox = newControls.DescriptionTextBox;

                newDescriptionTextBox.SetBinding(TextBox.TextProperty, HelpService.AddBinding(subtasks[rowCount - 1], "Description"));
                newDateDatePicker.SetBinding(DatePicker.SelectedDateProperty, HelpService.AddBinding(subtasks[rowCount - 1], "Date"));

                if (i != oldSubtasks.Count)
                {
                    newDateDatePicker.SelectedDate = oldSubtasks[i].Date;
                    newDateDatePicker.Tag = "true";
                    newDescriptionTextBox.Text = oldSubtasks[i].Description;
                }

                newDateDatePicker.SelectedDateChanged += selectedDateChanged;

                Grid.SetRow(newOrdinalNumberTextBlock, rowCount - 1);
                Grid.SetRow(newDateDatePicker, rowCount - 1);
                Grid.SetRow(newDescriptionTextBox, rowCount - 1);
                subtasksGrid.Children.Add(newOrdinalNumberTextBlock);
                subtasksGrid.Children.Add(newDateDatePicker);
                subtasksGrid.Children.Add(newDescriptionTextBox);
            }
        }

        private static (TextBlock OrdinalNumberTextBlock, DatePicker DateDatePicker, TextBox DescriptionTextBox) GetControls(int index)
        {
            var ordinalNumberTextBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 5, 0, 0),
                Text = index.ToString() + ".",
                Style = (Style)Application.Current.FindResource("MaterialDesignBody1TextBlock")
            };

            var dateDatePicker = new DatePicker
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 100,
                IsTodayHighlighted = true,
                FontSize = 16,
                Margin = new Thickness(25, 5, 0, 0)
            };

            var descriptionTextBox = new TextBox
            {
                Margin = new Thickness(130, 5, 5, 0),
                FontSize = 16
            };

            return (ordinalNumberTextBlock, dateDatePicker, descriptionTextBox);
        }
    }
}
