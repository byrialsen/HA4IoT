using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace HA4IoT.TraceReceiver.UWP
{
    public class TraceItemReceiverClient : IDisposable
    {
        #region Properties

        DatagramSocket listenerSocket = null;

        #endregion Properties

        #region Constructors / destructors

        ~TraceItemReceiverClient()
        {
            Dispose();
        }

        #endregion Constructors / destructors

        #region Public methods

        public void Dispose()
        {
            Stop();
        }

        public async Task Start()
        {
            await StartListener();
        }

        public void Stop()
        {
            if (listenerSocket != null)
            {
                try
                {
                    listenerSocket.MessageReceived -= MessageReceived;
                    listenerSocket?.Dispose();
                    listenerSocket = null;
                }
                catch { }
            }
        }

        #endregion Public methods

        #region Private methods

        private async Task StartListener()
        {
            listenerSocket = new DatagramSocket();
            listenerSocket.MessageReceived += MessageReceived;

            // Start listen operation.
            try
            {
                await listenerSocket.BindServiceNameAsync("19227");
            }
            catch (Exception exception)
            {
                Stop();
                
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                //rootPage.NotifyUser(
                //    "Start listening failed with error: " + exception.Message,
                //    NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Message received handler
        /// </summary>
        /// <param name="socket">The socket object</param>
        /// <param name="eventArguments">The datagram event information</param>
        void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            try
            {
                // Interpret the incoming datagram's entire contents as a string.
                uint stringLength = eventArguments.GetDataReader().UnconsumedBufferLength;
                string data = eventArguments.GetDataReader().ReadString(stringLength);

                IList<TraceItem> traceItems;
                if (!TryParseNotifications(data, out traceItems))
                {
                    return;
                }

                foreach (var notification in traceItems)
                {
                    System.Diagnostics.Debug.WriteLine(notification.Message);
                    //TraceItemReceived?.Invoke(this, new TraceItemReceivedEventArgs(senderAddress, notification));
                }

                //NotifyUserFromAsyncThread(
                //    "Received data from remote peer (Remote Address: " +
                //    eventArguments.RemoteAddress.CanonicalName +
                //    ", Remote Port: " +
                //    eventArguments.RemotePort + "): \"" +
                //     receivedMessage + "\"",
                //    NotifyType.StatusMessage);
            }
            catch (Exception exception)
            {
                SocketErrorStatus socketError = SocketError.GetStatus(exception.HResult);

                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                //rootPage.NotifyUser(
                //    "Error happened when receiving a datagram:" + exception.Message,
                //    NotifyType.ErrorMessage);
            }
        }

        private bool TryParseNotifications(string data, out IList<TraceItem> traceItems)
        {
            try
            {
                var parser = new TraceItemsParser();
                traceItems = parser.Parse(data);

                return true;
            }
            catch
            {
                traceItems = null;
                return false;
            }
        }

        #endregion Private methods
    }
}
