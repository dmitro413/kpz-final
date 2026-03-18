using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfLibrary.viewmodels;

namespace KPZ_FINAL.Views
{
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
        }

        private void Cell_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not CellViewModel cellVm) return;

            var gameVm = FindGameViewModel(fe);
            if (gameVm == null) return;

            gameVm.ToggleFlagCommand.Execute(cellVm);
            e.Handled = true;
        }

        private GameViewModel? FindGameViewModel(DependencyObject element)
        {
            var current = VisualTreeHelper.GetParent(element);
            while (current != null)
            {
                if (current is FrameworkElement fw &&
                    fw.DataContext is GameViewModel gvm)
                    return gvm;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}