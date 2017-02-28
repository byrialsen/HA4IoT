using HA4IoT.Ui.UWP.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HA4IoT.Ui.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogView : Page
    {
        public LogViewModel ViewModel => this.DataContext as LogViewModel;

        public LogView()
        {
            this.InitializeComponent();
        }
    }
}
