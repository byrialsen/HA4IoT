using System;
using Windows.Networking;

namespace HA4IoT.TraceReceiver.UWP
{
    public class TraceItemReceivedEventArgs : EventArgs
        {
            public TraceItemReceivedEventArgs(HostName senderAddress, TraceItem traceItem)
            {
                if (senderAddress == null) throw new ArgumentNullException(nameof(senderAddress));
                if (traceItem == null) throw new ArgumentNullException(nameof(traceItem));

                SenderAddress = senderAddress;
                TraceItem = traceItem;
            }

            public HostName SenderAddress { get; }

            public TraceItem TraceItem { get; }
        }
    
}
