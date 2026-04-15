using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;

namespace AppVorlage
{
    public class EmployeeViewModel : INotifyPropertyChanged //这个类支持属性变化通知，UI里绑定到这个类的属性发生变化时会自动更新显示
    {
        public event PropertyChangedEventHandler? PropertyChanged; // 上述接口必须要实现这个事件，UI会监听这个事件来知道什么时候更新显示

        protected void OnPropertyChanged([CallerMemberName] string? name = null) // CallerMemberName特性: 如果你调用 OnPropertyChanged() 时没有传参数，编译器会自动把“调用它的方法或属性名”填进去
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // 1. 搜索关键词（姓名）
        private string? nameKeyword;
        public string? NameKeyword
        {
            get => nameKeyword;
            set
            {
                nameKeyword = value;
                OnPropertyChanged(); // 实际上OnPropertyChanged("NameKeyword");
            }
        }

        // 2. JobTitle筛选
        public ObservableCollection<string> JobTitles { get; set; } = new();

        private string? selectedJobTitle;
        public string? SelectedJobTitle
        {
            get => selectedJobTitle; // combobox SelectedItem="{Binding SelectedJobTitle}" 选中的项绑定到这个属性
            set
            {
                selectedJobTitle = value;
                OnPropertyChanged();
            }
        }

        // 3. HireDate筛选
        private DateTime? hireDateFrom;
        public DateTime? HireDateFrom
        {
            get => hireDateFrom;
            set
            {
                hireDateFrom = value;
                OnPropertyChanged();
            }
        }

        // 5.ID
        private string? employeeIdKeyword;
        public string? EmployeeIdKeyword
        {
            get => employeeIdKeyword;
            set
            {
                employeeIdKeyword = value;
                OnPropertyChanged();
            }
        }

        // 全选
        private bool selectAll;
        public bool SelectAll
        {
            get => selectAll;
            set
            {
                if (selectAll == value)
                    return;

                selectAll = value;
                OnPropertyChanged();

                foreach (var employee in Employees)
                {
                    employee.IsSelected = value;
                }
            }
        }
        // 7. 搜索结果
        public ObservableCollection<EmployeeInfo> Employees { get; set; } // 它在集合内容变化时会自动通知 UI更新显示
            = new();

        private readonly SQLVerbindung _repo;

        public EmployeeViewModel()
        {
            _repo = new SQLVerbindung(); // 给ViewModel准备一个“数据库工具”
            LoadInitialData(); // 程序一启动就先加载一些初始数据
        }

        private void LoadInitialData()
        {
            JobTitles.Clear(); // 先清空，避免重复加载

            JobTitles.Add("");


            var jobs = _repo.GetJobTitles();

            foreach (var j in jobs)
                JobTitles.Add(j);

            SelectedJobTitle = "";
        }


        public void Search()
        {
            SelectAll = false;
            var jobFilter = SelectedJobTitle == "" ? null : SelectedJobTitle;

                       
            var result = _repo.SearchEmployees(
                EmployeeIdKeyword,
                NameKeyword,
                jobFilter,
                HireDateFrom
            );

            Employees.Clear();

            foreach (var e in result)
                Employees.Add(e);
        }
    }
}
