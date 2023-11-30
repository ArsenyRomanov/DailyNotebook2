using DailyNotebook.Models;
using System.Windows;

namespace DailyNotebook
{
    /// <summary>
    /// Логика взаимодействия для CreateWorksheetWindow.xaml
    /// </summary>
    public partial class CreateWorksheetWindow : Window
    {
        private Worksheet worksheet;

        public CreateWorksheetWindow(Worksheet newWorksheet, double menuTop, double menuLeft, double menuHeight, double menuWidth)
        {
            InitializeComponent();

            Top = menuTop + menuHeight/2 - Height/2;
            Left = menuLeft + menuWidth/2 - Width/2;

            worksheet = newWorksheet;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            worksheet.Name = WorksheetNameTextBox.Text;
            Close();
        }
    }
}
