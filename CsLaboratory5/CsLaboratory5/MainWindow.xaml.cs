using CsLaboratory5.ViewModels;
using System.Windows;

namespace CsLaboratory5
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ProcessesViewModel();
        }
    }
}
