using System.Collections.Generic;
using Hikari.Mvvm;

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

        private string _currentVersion;  // 当前选中的sdk版本
        /// <summary>
        /// 当前选中的sdk版本
        /// </summary>
        public string CurrentVersion
        {
            get => _currentVersion;
            set { _currentVersion = value; NotifyPropertyChanged(); }
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