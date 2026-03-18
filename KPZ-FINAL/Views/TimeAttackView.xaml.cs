using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfLibrary.viewmodels;

namespace KPZ_FINAL.Views
{
    public partial class TimeAttackView : UserControl
    {
        public TimeAttackView()
        {
            InitializeComponent();
        }

        private void Cell_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe &&
                fe.DataContext is CellViewModel cellVm &&
                DataContext is TimeAttackViewModel taVm)
            {
                taVm.ToggleFlagCommand.Execute(cellVm);
                e.Handled = true;
            }
        }
    }
}