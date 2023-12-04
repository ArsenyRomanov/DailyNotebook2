using DailyNotebook.Models;
using DailyNotebook.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DailyNotebook
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private BindingList<Worksheet> worksheets = new BindingList<Worksheet>();
        private Worksheet? worksheet { get; set; }

        public MenuWindow()
        {
            InitializeComponent();

            Top = SystemParameters.FullPrimaryScreenHeight - Height;
            Left = SystemParameters.FullPrimaryScreenWidth - Width;

            try { worksheets = DataBaseIOService.LoadWorksheets(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Close();
            }

            if (worksheets.Count > 0)
            {
                foreach (var worksheet in worksheets)
                {
                    worksheet.LastOpenedString = $"{(DateTime.Now - worksheet.LastOpenedDate).Days} days ago";
                    worksheet.TasksCount = $"Tasks: {worksheet.Tasks.Count}";
                }

                worksheets = HelpService.TreeSort(worksheets);
            }

            WorksheetsDG.ItemsSource = worksheets;
            NumberOfWorksheetsTextBlock.Text = $"Worksheets: {worksheets.Count}";
        }

        private void AccessButton_Click(object sender, RoutedEventArgs e)
        {
            if (worksheet != null)
            {
                Hide();
                MainWindow mainWindow = new MainWindow(worksheet);
                mainWindow.ShowDialog();

                worksheet.LastOpenedDate = DateTime.Now;
                worksheet.LastOpenedString = $"{(DateTime.Now - worksheet.LastOpenedDate).Days} days ago";
                worksheet.TasksCount = $"Tasks: {worksheet.Tasks.Count}";

                try { DataBaseIOService.UpdateWorksheet(worksheet); }
                catch (Exception exception) { MessageBox.Show(exception.Message); }

                worksheets = HelpService.TreeSort(worksheets);
                WorksheetsDG.ItemsSource = worksheets;
                WorksheetsDG.SelectedIndex = 0;
                Show();
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            MenuGrid.Opacity = 0.5;
            Worksheet newWorksheet = new Worksheet();
            CreateWorksheetWindow createWorksheetWindow = new CreateWorksheetWindow(newWorksheet, Top, Left, Height, Width);
            createWorksheetWindow.ShowDialog();
            MenuGrid.Opacity = 1;

            if (!string.IsNullOrWhiteSpace(newWorksheet.Name))
            {
                newWorksheet.LastOpenedString = "0 days ago";
                newWorksheet.TasksCount = "Tasks: 0";
                worksheets.Insert(0, newWorksheet);

                try { DataBaseIOService.AddWorksheet(newWorksheet); }
                catch (Exception exception) { MessageBox.Show(exception.Message); }

                NumberOfWorksheetsTextBlock.Text = $"Worksheets: {worksheets.Count}";
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorksheetsDG.SelectedItem is not Worksheet worksheetToDelit)
                return;

            var currentIndex = WorksheetsDG.SelectedIndex;

            worksheets.Remove(worksheetToDelit);

            try { DataBaseIOService.DeleteWorksheet(worksheetToDelit); }
            catch (Exception exception) { MessageBox.Show(exception.Message); }

            NumberOfWorksheetsTextBlock.Text = $"Worksheets: {worksheets.Count}";

            if (worksheets.Count > 0)
            {
                if (currentIndex != 0)
                    WorksheetsDG.SelectedItem = WorksheetsDG.Items[currentIndex - 1];
                else
                    WorksheetsDG.SelectedItem = WorksheetsDG.Items[currentIndex];
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WorksheetsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            worksheet = WorksheetsDG.SelectedItem as Worksheet;
        }
    }
}
