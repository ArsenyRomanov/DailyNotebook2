using DailyNotebook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
