using System.Windows.Controls;
using DotNetLocalizedTool.Models;

namespace DotNetLocalizedTool.ViewModels;

public class LoadingDialogViewModel : ViewBase
{
    public LoadingDialogModel Model { get; set; }
    public LoadingDialogViewModel()
    {
        this.Model = new LoadingDialogModel();
    }


    public bool IsClick { get; set; }

    //private LoadingDialogView loadingDialogView;
    //public LoadingDialogView LoadingDialogView
    //{
    //    get => loadingDialogView;
    //    set
    //    {
    //        loadingDialogView = value;
    //        loadingDialogView.MouseLeftButtonDown += (sender, e) =>
    //        {
    //            IsClick = true;
    //        };
    //        loadingDialogView.MouseLeftButtonUp += (sender, e) =>
    //        {
    //            IsClick = false;
    //        };
    //        loadingDialogView.MouseLeave += (sender, e) =>
    //        {
    //            IsClick = false;
    //        };
    //    }
    //}



}