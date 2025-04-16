using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DotNetLocalizedTool.Models
{
    public partial class MainWindowModel : ObservableObject
    {
        public MainWindowModel()
        {
            Packs = new ObservableCollection<string>();
            SdkVersions = new ObservableCollection<string>();
            LanguageList = new List<string>();
        }
        [ObservableProperty]
        private ObservableCollection<string> _packs;  // 版本包路径列表

        [ObservableProperty]
        private ObservableCollection<string> _sdkVersions;  // sdk版本列表

        [ObservableProperty]
        private string _currentVersion;  // 当前sdk版本


        private List<string> _languageList;  // 语言列表
        public List<string> LanguageList
        {
            get => _languageList;
            set { _languageList = value; OnPropertyChanged(); }
        }
        [ObservableProperty]
        private string _currentLanguage = "";  // 当前选中的语言
        private double _downloadProgress;  // 下载进度
        public double DownloadProgress
        {
            get => _downloadProgress;
            set { _downloadProgress = value; OnPropertyChanged(); }
        }
        [ObservableProperty]
        private bool _isDownloaded = true;  // 是否下载完成
    }
}