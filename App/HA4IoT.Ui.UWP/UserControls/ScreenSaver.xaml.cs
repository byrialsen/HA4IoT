using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace HA4IoT.Ui.UWP.UserControls
{
    // The screensaver class triggers automatically after a certain amount of inactivity
    // from keyboard or pointer events.
    // Note: You should only use this screen saver if your device has a pointer or keyboard,
    // or you won't be able to see the app after the screensaver starts.
    //
    // To re-use this screensaver, simply include this class and add the following line
    // to the end of App.OnLaunched:
    // Screensaver.InitializeScreensaver();
    public sealed partial class ScreenSaver : UserControl
    {
        const int IDLE_TRIGGER_DURATION = 60;

        private static DispatcherTimer _idleCheckTimer;
        private static Popup _container;
        private static DateTime _lastAction = DateTime.Now;
        private static CoreCursor _lastCursor = null;
        private static int _idleTriggerDurationInSeconds;

        /// <summary>
        /// Initializes the screensaver
        /// </summary>
        public static void InitializeScreensaver(int idleTriggerDurationInSeconds = IDLE_TRIGGER_DURATION)
        {
            // init
            _idleTriggerDurationInSeconds = idleTriggerDurationInSeconds;

            // create container containg the screen saver user control
            _container = new Popup()
            {
                Child = new ScreenSaver(),
                Margin = new Thickness(0),
                IsOpen = false
            };

            // set screen saver to activate after a period of time
            Window.Current.Content.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(App_KeyDown), true);
            Window.Current.Content.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(App_PointerEvent), true);
            Window.Current.Content.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(App_PointerEvent), true);
            Window.Current.Content.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(App_PointerEvent), true);
            Window.Current.Content.AddHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(App_PointerEvent), true);

            // start idle check timer
            _idleCheckTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            _idleCheckTimer.Tick += OnIdleTimerTick;
            _idleCheckTimer.Start();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the screen saver should listen for inactivity and 
        /// show after a little while. The default is <c>false</c>.
        /// </summary>
        public static bool IsScreensaverEnabled
        {
            get
            {
                return true;

                //if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Constants.EnableScreensaverKey))
                //{
                //    return (bool)ApplicationData.Current.LocalSettings.Values[Constants.EnableScreensaverKey];
                //}
                //return false;
            }
            set
            {
                //ApplicationData.Current.LocalSettings.Values[Constants.EnableScreensaverKey] = value;
                if (_idleCheckTimer != null)
                {
                    if (value)
                    {
                        _idleCheckTimer.Start();
                    }
                    else
                    {
                        _idleCheckTimer.Stop();
                    }
                }
            }
        }

        public static bool IsScreensaverActive
        {
            get { return _container.IsOpen; }
        }

        // Triggered when there hasn't been any key or pointer events in a while
        private static void OnIdleTimerTick(object sender, object e)
        {
            TimeSpan idleDuration = DateTime.Now.Subtract(_lastAction);

            System.Diagnostics.Debug.WriteLine($"Idle timer check ({Convert.ToInt32(idleDuration.TotalMilliseconds)})");

            _idleCheckTimer.Stop();
            try
            {
                if (IsScreensaverActive)
                    return;

                // check if it is time to show screensaver
                if (idleDuration > TimeSpan.FromSeconds(_idleTriggerDurationInSeconds))
                {
                    ShowScreensaver();
                }
            }
            finally
            {
                if (!IsScreensaverActive)
                    _idleCheckTimer.Start();
            }
        }

        private static void ShowScreensaver()
        {
            System.Diagnostics.Debug.WriteLine($"Enable Screensaver");

            var bounds = Windows.UI.Core.CoreWindow.GetForCurrentThread().Bounds;
            var view = (ScreenSaver)_container.Child;
            view.Width = bounds.Width;
            view.Height = bounds.Height;
            view.content.Width = view.Width / 5; //Make screensaver image 1/5 the width of the screen
            _lastCursor = Window.Current.CoreWindow.PointerCursor;
            Window.Current.CoreWindow.PointerCursor = null; // Hide the cursor
            _container.IsOpen = true;
        }

        private static void App_KeyDown(object sender, KeyRoutedEventArgs args)
        {
            ResetScreensaverTimeout();
        }

        private static void App_PointerEvent(object sender, PointerRoutedEventArgs e)
        {
            ResetScreensaverTimeout();
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            ResetScreensaverTimeout();
        }

        // Resets the timer and starts over.
        private static void ResetScreensaverTimeout()
        {
            _lastAction = DateTime.Now;

            if (IsScreensaverEnabled && IsScreensaverActive)
            {
                System.Diagnostics.Debug.WriteLine($"Disable Screensaver");

                Window.Current.CoreWindow.PointerCursor = _lastCursor; // new CoreCursor(CoreCursorType.Arrow, 1);
                _idleCheckTimer.Start();
                _container.IsOpen = false;
            }
        }

        private DispatcherTimer moveTimer;
        private Random randomizer = new Random();

        /// <summary>
        /// private constructor
        /// </summary>
        private ScreenSaver()
        {
            this.InitializeComponent();

            moveTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(10) };
            moveTimer.Tick += OnMoveTimerTick;

            this.Loaded += async (sender, e) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    ScreenSaver_Loaded(sender, e);
                });
            };
            this.Unloaded += ScreenSaver_Unloaded;
            //image.Source = new BitmapImage(DeviceInfoPresenter.GetBoardImageUri());
        }

        private void OnMoveTimerTick(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine($"Move time tick");

            var left = randomizer.NextDouble() * (this.ActualWidth - content.ActualWidth);
            var top = randomizer.NextDouble() * (this.ActualHeight - content.ActualHeight);
            content.Margin = new Thickness(left, top, 0, 0);
        }

        private void ScreenSaver_Unloaded(object sender, RoutedEventArgs e)
        {
            moveTimer.Stop();
        }

        private void ScreenSaver_Loaded(object sender, RoutedEventArgs e)
        {
            moveTimer.Start();
            OnMoveTimerTick(moveTimer, null);
        }
    }
}
