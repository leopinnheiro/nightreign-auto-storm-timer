using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Utils;

public class NumericTextBoxBehavior : Behavior<TextBox>
{
    private static readonly Regex _regex = new(@"^\d*$");

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewTextInput += OnPreviewTextInput;
        DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        AssociatedObject.TextChanged += OnTextChanged;
        AssociatedObject.GotKeyboardFocus += OnGotKeyboardFocus;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
        DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
        AssociatedObject.TextChanged -= OnTextChanged;
        AssociatedObject.GotKeyboardFocus -= OnGotKeyboardFocus;
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !_regex.IsMatch(e.Text);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            string text = (string)e.DataObject.GetData(typeof(string));
            if (!_regex.IsMatch(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (AssociatedObject.Text == "")
        {
            AssociatedObject.Text = "0";
            AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
            AssociatedObject.SelectAll();
        }
    }

    private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        AssociatedObject.SelectAll();
    }
}