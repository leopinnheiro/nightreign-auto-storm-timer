using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace nightreign_auto_storm_timer.Utils;

public static class NumericValidationBehavior
{
    public static readonly DependencyProperty AllowOnlyPositiveIntegersProperty =
        DependencyProperty.RegisterAttached(
            "AllowOnlyPositiveIntegers",
            typeof(bool),
            typeof(NumericValidationBehavior),
            new UIPropertyMetadata(false, OnAllowOnlyPositiveIntegersChanged));

    public static bool GetAllowOnlyPositiveIntegers(DependencyObject obj)
        => (bool)obj.GetValue(AllowOnlyPositiveIntegersProperty);

    public static void SetAllowOnlyPositiveIntegers(DependencyObject obj, bool value)
        => obj.SetValue(AllowOnlyPositiveIntegersProperty, value);

    private static void OnAllowOnlyPositiveIntegersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                DataObject.AddPastingHandler(textBox, OnPaste);
            }
            else
            {
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                DataObject.RemovePastingHandler(textBox, OnPaste);
            }
        }
    }

    private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = sender as TextBox;
        string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        if (!int.TryParse(fullText, out int number) || number <= 0)
            e.Handled = true;
    }

    private static void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var pastedText = (string)e.DataObject.GetData(typeof(string));
            var textBox = sender as TextBox;
            string fullText = textBox.Text.Insert(textBox.SelectionStart, pastedText);

            if (!int.TryParse(fullText, out int number) || number <= 0)
                e.CancelCommand();
        }
        else
        {
            e.CancelCommand();
        }
    }
}
