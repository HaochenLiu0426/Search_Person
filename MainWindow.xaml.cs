using ClosedXML.Excel;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppVorlage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // ⭐关键：把Viewmodel挂到UI, 主窗口里所有 {Binding ...} 默认都去 EmployeeViewModel 里找
            DataContext = new EmployeeViewModel();
        }

        private void ExecuteSearch()
        {
            var vm = (EmployeeViewModel)DataContext;
            vm.Search();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSearch();
        }


        // 下面是对于日期搜索输入框的PreviewKeyDown事件的处理器，确保在输入框内按下Enter键能触发搜索
        private void SearchControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is DatePicker datePicker)
                {
                    var binding = BindingOperations.GetBindingExpression(
                        datePicker,
                        DatePicker.SelectedDateProperty);

                    binding?.UpdateSource();
                }
                ExecuteSearch();
                e.Handled = true;
            }
        }

        //下面是窗口KeyDown事件的处理器，确保在窗口内按下Enter键也能触发搜索
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteSearch();
            }
        }

        private void Delete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && sender is ComboBox comboBox)
            {
                comboBox.SelectedItem = "";
                e.Handled = true;
            }
        }



        // 导出excel
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not EmployeeViewModel vm)
                return;

            if (vm.Employees.Count == 0)
            {
                MessageBox.Show("Keine Daten zum Exportieren vorhanden.");
                return;
            }

            var selectedEmployees = vm.Employees.Where(x => x.IsSelected).ToList();
            var exportEmployees = selectedEmployees.Count > 0
                ? selectedEmployees
                : vm.Employees.ToList();

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel-Arbeitsmappe|*.xlsx",
                FileName = "Mitarbeiter.xlsx"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Mitarbeiter");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Vorname";
            worksheet.Cell(1, 3).Value = "Nachname";
            worksheet.Cell(1, 4).Value = "Berufsbezeichnung";
            worksheet.Cell(1, 5).Value = "Eintrittsdatum";

            int row = 2;

            foreach (var employee in exportEmployees)
            {
                worksheet.Cell(row, 1).Value = employee.BusinessEntityID;
                worksheet.Cell(row, 2).Value = employee.FirstName;
                worksheet.Cell(row, 3).Value = employee.LastName;
                worksheet.Cell(row, 4).Value = employee.JobTitle;
                worksheet.Cell(row, 5).Value = employee.HireDate;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(saveFileDialog.FileName);

            if (selectedEmployees.Count > 0)
            {   
                MessageBox.Show($"{selectedEmployees.Count} Zeilen wurden erfolgreich exportiert.");
            }
            else
            {
                MessageBox.Show($"{vm.Employees.Count} Zeilen wurden erfolgreich exportiert.");
            }
        }

    }
}