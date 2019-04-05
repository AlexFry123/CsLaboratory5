using CsLaboratory5.Tools;
using System.Windows;

namespace CsLaboratory5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            Cancellation.RefreshListCancellation.Cancel();
            Cancellation.RefreshMetaDataCancellation.Cancel();
        }
    }
}
