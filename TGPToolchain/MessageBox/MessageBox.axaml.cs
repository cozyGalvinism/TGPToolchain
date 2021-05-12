using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TGPToolchain.MessageBox
{
    public class MessageBox : Window
    {
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        public enum MessageBoxResult
        {
            Ok,
            Cancel,
            Yes,
            No
        }
        
        public MessageBox()
        {
            InitializeComponent();
        }

        public static Task<MessageBoxResult> ShowDialog(Window? parent, string text, string title, MessageBoxButtons buttons)
        {
            var messageBox = new MessageBox
            {
                Title = title
            };
            messageBox.FindControl<TextBlock>("Text").Text = text;
            var buttonPanel = messageBox.FindControl<StackPanel>("Buttons");

            var res = MessageBoxResult.Ok;

            void AddButton(string caption, MessageBoxResult r, bool def = false)
            {
                var btn = new Button {Content = caption};
                btn.Click += (_, __) =>
                {
                    res = r;
                    messageBox.Close();
                };
                buttonPanel.Children.Add(btn);
                if (def)
                    res = r;
            }
            
            if(buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
                AddButton("OK", MessageBoxResult.Ok, true);
            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                AddButton("Yes", MessageBoxResult.Yes);
                AddButton("No", MessageBoxResult.No, true);
            }
            if(buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
                AddButton("Cancel", MessageBoxResult.Cancel, true);

            var taskCompletionSource = new TaskCompletionSource<MessageBoxResult>();
            messageBox.Closed += delegate { taskCompletionSource.TrySetResult(res); };
            if (parent != null)
                messageBox.ShowDialog(parent);
            else messageBox.Show();
            return taskCompletionSource.Task;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}