using System.Collections.Generic;

namespace DotNetLocalizedTool.Models
{
    public class MainWindowModel : NotificationObject
    {
        private List<string> _packs;  // 版本包路径列表

        public List<string> Packs
        {
            get => _packs;
            set { _packs = value; NotifyPropertyChanged(); }
        }

        private List<string> _versions;  // 所有版本列表

        public List<string> Versions
        {
            get => _versions;
            set { _versions = value; NotifyPropertyChanged(); }
        }

        private List<string> _languageList;  // 语言列表
        public List<string> LanguageList
        {
            get => _languageList;
            set { _languageList = value; NotifyPropertyChanged(); }
        }

        private double _downloadProgress;  // 下载进度
        public double DownloadProgress
        {
            get => _downloadProgress;
            set { _downloadProgress = value; NotifyPropertyChanged(); }
        }
    }
}