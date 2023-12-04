using DailyNotebook.Models;
using DailyNotebook.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DailyNotebook
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private List<Worksheet> worksheets = new List<Worksheet>();
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

            HelpService.RefreshWorksheetControls(WorksheetsListBox, NumberOfWorksheetsTextBlock, worksheets);
        }

        private void AccessButton_Click(object sender, RoutedEventArgs e)
        {
            if (worksheet != null)
            {
                Hide();
                MainWindow mainWindow = new MainWindow(worksheet);
                mainWindow.ShowDialog();

                worksheet.LastOpenedDate = DateTime.Now;

                try { DataBaseIOService.UpdateWorksheet(worksheet); }
                catch (Exception exception) { MessageBox.Show(exception.Message); }

                HelpService.RefreshWorksheetControls(WorksheetsListBox, NumberOfWorksheetsTextBlock, worksheets);
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
                worksheets.Add(newWorksheet);

                try { DataBaseIOService.AddWorksheet(newWorksheet); }
                catch (Exception exception) { MessageBox.Show(exception.Message); }

                HelpService.RefreshWorksheetControls(WorksheetsListBox, NumberOfWorksheetsTextBlock, worksheets);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (worksheet != null)
            {
                var currentIndex = WorksheetsListBox.SelectedIndex;

                worksheets.Remove(worksheet);

                try { DataBaseIOService.DeleteWorksheet(worksheet); }
                catch (Exception exception) { MessageBox.Show(exception.Message); }

                HelpService.RefreshWorksheetControls(WorksheetsListBox, NumberOfWorksheetsTextBlock, worksheets);

                if (worksheets.Count > 0)
                {
                    if (currentIndex != 0)
                        WorksheetsListBox.SelectedItem = WorksheetsListBox.Items[currentIndex - 1];
                    else
                        WorksheetsListBox.SelectedItem = WorksheetsListBox.Items[currentIndex];
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WorksheetsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem is string worksheetString)
                worksheet = worksheets.First(x => x.Name == worksheetString.Split('\t')[0]);
            else worksheet = null;
        }
    }
}
