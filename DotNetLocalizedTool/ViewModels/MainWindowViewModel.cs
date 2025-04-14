﻿using System.Collections.ObjectModel;
using DotNetLocalizedTool.Models;
using GalaSoft.MvvmLight.Threading;
using Hikari.Common;
using Hikari.Common.IO;
using Hikari.Common.Net.Http;
using NSoup.Nodes;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace DotNetLocalizedTool.ViewModels
{
    public class MainWindowViewModel
    {
        public Models.MainWindowModel Model { get; set; }


        private IDictionary<string, IDictionary<string, string>> _languageLinkList;

        public MainWindowViewModel()
        {
            this.Model = new MainWindowModel();

            _languageLinkList = new Dictionary<string, IDictionary<string, string>>();

        }
        /// <summary>
        /// 窗口启动时
        /// </summary>
        public ICommand LoadedCommand
        {
            get
            {
                return new AsyncRelayCommand<object>(async delegate (object? obj)
                {
                    DialogHelper.ShowLoading();
                    await Task.Run(async () =>
                    {
                        //Model.Versions = new List<string>();
                        Model.CurrentVersion = SystemHelper.RunCmd("dotnet --version").Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[0];  // 当前版本
                        string[] sdks = SystemHelper.RunCmd("dotnet --list-sdks").Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var sdk in sdks)
                        {
                            Model.SdkVersions.Add(sdk.Split(' ')[0]);
                            Model.SdkVersions = new ObservableCollection<string>(Model.SdkVersions.Reverse());
                        }

                        //Model.CurrentVersion = currentVersion;

                        //_languageLinkList = await GetLanguageLinkList();
                        //VersionSelectionChangedCommand.Execute(currentVersion);
                        GetPackList();
                        await GetLanguageLinkList();
                    });
                    DialogHelper.CloseLoading();
                });
            }
        }

        /// <summary>
        /// 应用命令
        /// </summary>
        public ICommand ApplyCommand
        {
            get
            {
                return new AsyncRelayCommand<object>(async delegate (object? obj)
                {
                    var version = Model.CurrentVersion.Substring(0, 3);
                    string? language = obj.ToString();
                    if (language == null)
                    {
                        MessageBox.Show("请选择语言");
                        return;
                    }
                    string url;
                    if (_languageLinkList.TryGetValue(version, out var value))
                    {
                        url = value[language];
                    }
                    else
                    {
                        var v = _languageLinkList.Keys.ToList();
                        url = _languageLinkList[v[^1]][language];
                    }


                    string fileName = Path.GetFileName(url);
                    string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName);
                    fileName = Path.GetFileNameWithoutExtension(url);

                    // 下载
                    ThreadPool.QueueUserWorkItem(async state =>
                    {
                        IProgress<HttpDownloadProgress> progress = new Progress<HttpDownloadProgress>(progress =>
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Model.DownloadProgress = progress.BytesReceived * 100.0 / progress.TotalBytesToReceive ?? 1;
                            });

                            if (progress.BytesReceived == progress.TotalBytesToReceive)
                            {
                                // 解压
                                new ZipLibHelper().UnzipZip(path, System.AppDomain.CurrentDomain.BaseDirectory);
                                // 覆盖
                                CopyDirectory(fileName);
                                DeleteDirectory(fileName);
                                MessageBox.Show("完成");
                            }
                        });
                        await new HttpClient().GetByteArrayAsync(url, path, progress, CancellationToken.None);
                    });

                });
            }
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        /// <returns></returns>
        public ICommand OpenDirectoryCommand => new RelayCommand<object>(obj =>
        {
            if (obj is string path)
            {
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
        });
        /// <summary>
        /// 清理无用的运行时目录
        /// </summary>
        public ICommand ClearRuntimeCommand => new AsyncRelayCommand<object>(async obj =>
        {
            DialogHelper.ShowLoading();
            await Task.Run(() =>
            {
                string[] strs = [@"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref", @"C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref"];
                foreach (var str in strs)
                {
                    var dirs = DirectoryHelper.FindDirectories(str);
                    foreach (var dir in dirs)
                    {
                        var list = DirectoryHelper.FindDirectories(dir);
                        if (list.Count < 3)
                        {
                            // 删除目录
                            DirectoryHelper.DeleteDirectory(dir, true);
                        }
                    }
                }
            });

            DialogHelper.CloseLoading();
            
        });

        /// <summary>
        /// 获得语言包列表
        /// </summary>
        private async Task GetLanguageLinkList()
        {
            string url = "https://dotnet.microsoft.com/zh-cn/download/intellisense";
            HttpClientHelper helper = new HttpClientHelper();
            string html = await helper.GetAsync(url);
            NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(html);

            var ele = doc.Select("tbody tr");
            foreach (Element e in ele)
            {
                string v = e.Attr("id");


                IDictionary<string, string> dic1 = new Dictionary<string, string>();
                var lis = e.Select("ul li");
                foreach (Element li in lis)
                {
                    var a = li.Select("a");
                    string title = a.Attr("title");
                    string href = a.Attr("href");
                    dic1.Add(title, href);
                }

                _languageLinkList.Add(v, dic1);
            }

            var version = Model.CurrentVersion.Substring(0, 3);
            if (_languageLinkList.TryGetValue(version, out var value))
            {
                Model.LanguageList = value.Keys.ToList();
            }
            else
            {
                var v = _languageLinkList.Keys.ToList();
                Model.LanguageList = _languageLinkList[v[^1]].Keys.ToList();
            }
        }

        private void CopyDirectory(string fileName)
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName, fileName, "Microsoft.NETCore.App.Ref");
            var dir1 = new DirectoryInfo(path).GetDirectories()[0].FullName;
            path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName, fileName, "Microsoft.WindowsDesktop.App.Ref");
            var dir2 = new DirectoryInfo(path).GetDirectories()[0].FullName;

            if (!System.IO.Directory.Exists(Path.Combine(Model.Packs[0], Path.GetFileName(dir1))))
            {
                DirectoryHelper.CopyDirectory(dir1, Model.Packs[0]);
            }
            if (!System.IO.Directory.Exists(Path.Combine(Model.Packs[1], Path.GetFileName(dir2))))
            {
                DirectoryHelper.CopyDirectory(dir2, Model.Packs[1]);
            }
        }

        private void DeleteDirectory(string fileName)
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName);
            DirectoryHelper.DeleteDirectory(path, true);
            path += ".zip";
            File.Delete(path);
        }
        /// <summary>
        /// 获得运行时清单
        /// </summary>
        private void GetPackList()
        {
            string[] sdks = SystemHelper.RunCmd("dotnet --list-runtimes").Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            IDictionary<string, string> pathList = new Dictionary<string, string>()
            {
                {"Microsoft.NETCore.App", @"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref"},
                {"Microsoft.WindowsDesktop.App", @"C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref"}
            };
            foreach (string sdk in sdks)
            {
                var s = sdk.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (pathList.Keys.Contains(s[0]))
                {
                    var path = Path.Combine(pathList[s[0]], s[1], "ref");
                    var dirs = DirectoryHelper.FindDirectories(path);
                    Model.Packs.Add(dirs[0]);
                }
            }
        }

    }
}