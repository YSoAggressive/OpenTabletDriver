using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using TabletDriverLib;
using TabletDriverLib.Component;

namespace OpenTabletDriverGUI.Views
{
    public class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            AsyncInit();
        }

        private async void AsyncInit()
        {
            Log.Output += (s, msg) => ScrollToBottom();
            var sv = await GetScrollViewerAsync();
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        public void ScrollToBottom()
        {
            Dispatcher.UIThread.Post(async () => 
            {
                ListBox listbox = (ListBox)this.Content;
                var scrollViewer = await GetScrollViewerAsync();
                var maxVal = scrollViewer.GetValue(ScrollViewer.VerticalScrollBarMaximumProperty);
                scrollViewer.SetValue(ScrollViewer.VerticalScrollBarValueProperty, maxVal);
            });
        }

        private async Task<ScrollViewer> GetScrollViewerAsync() => await ((ListBox)this.Content)
            .GetObservable(ListBox.ScrollProperty)
            .OfType<ScrollViewer>()
            .FirstAsync();

        public static readonly StyledProperty<ObservableCollection<LogMessage>> MessagesProperty =
            AvaloniaProperty.Register<LogView, ObservableCollection<LogMessage>>(nameof(Messages));
        
        public ObservableCollection<LogMessage> Messages
        {
            set => SetValue(MessagesProperty, value);
            get => GetValue(MessagesProperty);
        }

        private async Task CopyMessage(LogMessage message)
        {
            var text = string.Format("[{0}:{1}] {2}", message.Group, message.IsError ? "Error" : "Normal", message.Message);
            await App.Current.Clipboard.SetTextAsync(text);
        }
    }
}