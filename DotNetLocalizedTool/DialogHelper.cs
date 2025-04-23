using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DotNetLocalizedTool.ViewModels;
using DotNetLocalizedTool.Views;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

namespace DotNetLocalizedTool;

public class DialogHelper
{
    private static Dialog? _loadingDialog;
    public static Dialog ShowLoading(string title = "请稍等...")
    {
        _loadingDialog = Dialog.Show(new Border()
        {
            CornerRadius = new CornerRadius(20),
            Height = 180,
            Background = new SolidColorBrush( (Color)ColorConverter.ConvertFromString("#ffffff")),
            Width = 180,
            Child = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock()
                    {
                        Text = title,
                        FontSize = 30,
                        TextAlignment = TextAlignment.Center
                    },
                    new LoadingCircle()
                    {
                        Width = 100,
                        Margin = new Thickness(0, 10, 0, 0),
                        Height = 100,
                        DotDiameter = 13
                    }
                }
            }
        }, "123");
        return _loadingDialog;
    }

    public static void CloseLoading()
    {
        _loadingDialog?.Close();
    }
}