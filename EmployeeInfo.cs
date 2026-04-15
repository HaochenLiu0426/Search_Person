using MaterialDesignColors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace AppVorlage
{
    public class EmployeeInfo : INotifyPropertyChanged
    {
        public int BusinessEntityID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? JobTitle { get; set; }
        public DateTime HireDate { get; set; }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public string FullName => $"{FirstName}.{LastName}";
    }

}
// 如果想加一条属性： 首先在EmployeeInfo添加， 接着在SQL Select添加属性的查询和映射，第三步是在Reader里添加属性的读取，最后在UI里Datagrid绑定显示这个属性
// 如果是想添加一个搜索栏位：
// 首先在EmployeeViewModel添加这个搜索栏位的属性，
// 第二步在VM的SearchEmployees添加新的返回值，
// 第三步修改public List<EmployeeInfo> SearchEmployees的方法签名
// 第四步SQL添加动态条件
// 第五步修改Cmd参数
// 最后UI里添加输入控件绑定到这个属性，并且确保按下Enter键能触发搜索