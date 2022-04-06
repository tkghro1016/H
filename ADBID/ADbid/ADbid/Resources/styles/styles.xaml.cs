using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ADbid.Resources.styles
{
    public partial class styles
    {
        void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.ImeProcessed)
            {
                e.Handled = true;
            }
        }
    }
}