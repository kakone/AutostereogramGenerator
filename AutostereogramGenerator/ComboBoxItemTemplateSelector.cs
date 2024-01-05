using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutostereogramGenerator;

/// <summary>
/// ComboBox item template selector to have a different template for selected items
/// </summary>
public class ComboBoxItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the selected item template
    /// </summary>
    public DataTemplate? SelectedItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the item template
    /// </summary>
    public DataTemplate? ItemTemplate { get; set; }

    private static T? FindAscendant<T>(DependencyObject element) where T : notnull, DependencyObject
    {
        while (true)
        {
            var parent = VisualTreeHelper.GetParent(element);
            if (parent is null)
            {
                return null;
            }
            if (parent is T result)
            {
                return result;
            }
            element = parent;
        }
    }

    /// <inheritdoc/>
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return FindAscendant<ComboBoxItem>(container) == null ? SelectedItemTemplate : ItemTemplate;
    }
}
