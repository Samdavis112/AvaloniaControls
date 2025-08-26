using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AOSMvvm.Views;


public partial class MessageBox : Window
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
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        AvaloniaXamlLoader.Load(this);
    }

    public static Task<MessageBoxResult> Show(Window parent, string text, string title, MessageBoxButtons buttons)
    {
        var msgbox = new MessageBox()
        {
            Title = title,
        };

        msgbox.FindControl<TextBlock>("msg")!.Text = text;
        var res = MessageBoxResult.Ok;

        // Remove entry tb as not needed:
        msgbox.FindControl<StackPanel>("root")!.Children.Remove(msgbox.FindControl<TextBox>("tb")!);

        void AddButton(string caption, MessageBoxResult r, bool def = false)
        {
            var btn = new Button { Content = caption };
            btn.Click += (_, __) =>
            {
                res = r;
                msgbox.Close();
            };
            msgbox.FindControl<StackPanel>("buttonPanel")!.Children.Add(btn);
            if (def)
                res = r;
        }

        if (buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
            AddButton("Ok", MessageBoxResult.Ok, true);
        if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
        {
            AddButton("Yes", MessageBoxResult.Yes);
            AddButton("No", MessageBoxResult.No, true);
        }
        if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
            AddButton("Cancel", MessageBoxResult.Cancel, true);

        var tcs = new TaskCompletionSource<MessageBoxResult>();
        msgbox.Closed += delegate { tcs.TrySetResult(res); };
        if (parent != null)
            msgbox.ShowDialog(parent);
        else msgbox.Show();
        return tcs.Task;
    }

    public static Task<Tuple<MessageBoxResult, string>> Ask(Window parent, string text, string title, MessageBoxButtons buttons, string? PresetText = null)
    {
        var msgbox = new MessageBox()
        {
            Title = title,
        };

        var res = MessageBoxResult.Ok;
        msgbox.FindControl<TextBlock>("msg")!.Text = text;
        if (!string.IsNullOrEmpty(PresetText))
            msgbox.FindControl<TextBox>("tb")!.Text = PresetText;

        void AddButton(string caption, MessageBoxResult r, bool def = false)
        {
            var btn = new Button { Content = caption };
            btn.Click += (_, __) =>
            {
                res = r;
                msgbox.Close();
            };
            msgbox.FindControl<StackPanel>("buttonPanel")!.Children.Add(btn);
            if (def)
                res = r;
        }

        if (buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
            AddButton("Ok", MessageBoxResult.Ok, true);
        if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
        {
            AddButton("Yes", MessageBoxResult.Yes);
            AddButton("No", MessageBoxResult.No, true);
        }
        if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
            AddButton("Cancel", MessageBoxResult.Cancel, true);

        var tcs = new TaskCompletionSource<Tuple<MessageBoxResult, string>>();
        msgbox.Closed += delegate { tcs.TrySetResult(Tuple.Create(res, msgbox.FindControl<TextBox>("tb")!.Text!)); };

        if (parent != null)
            msgbox.ShowDialog(parent);
        else msgbox.Show();

        return tcs.Task;
    }
}