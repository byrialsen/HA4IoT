using HA4IoT.TraceReceiver.UWP;

namespace HA4IoT.Ui.UWP.ViewModels
{
    public class LogViewModel  : MVVM.BaseViewModel
    {      
        public LogViewModel()
        {
            TraceItemReceiverClient log = new TraceItemReceiverClient();
            log.Start();
        }
    }
}
