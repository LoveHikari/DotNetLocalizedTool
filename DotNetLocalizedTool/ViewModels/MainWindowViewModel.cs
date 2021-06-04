using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DotNetLocalizedTool.Models;
using GalaSoft.MvvmLight.Threading;
using Hikari.Common;
using Hikari.Common.Net.Http;
using NSoup.Nodes;

namespace DotNetLocalizedTool.ViewModels
{
    public class MainWindowViewModel
    {
        public Models.MainWindowModel Model { get; set; }

        private List<string> _pathList;
        private IDictionary<string, IDictionary<string, string>> _languageLinkList;

        public MainWindowViewModel()
        {
            this.Model = new MainWindowModel();

            _pathList = new List<string>()
            {
                @"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref",
                @"C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref"
            };
        }
        /// <summary>
        /// 窗口启动时
        /// </summary>
        public ICommand LoadedCommand
        {
            get
            {
                return new DelegateCommand<object>(async delegate (object obj)
                {
                    Model.Versions = new List<string>();

                    var dirs = new DirectoryInfo(_pathList[0]).GetDirectories();
                    foreach (var dir in dirs)
                    {
                        Model.Versions.Add(dir.Name.Split('-')[0]);
                    }

                    _languageLinkList = await GetLanguageLinkList();
                });
            }
        }
        /// <summary>
        /// 选择版本命令
        /// </summary>
        public ICommand VersionSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<object>(delegate (object obj)
                {
                    if (_languageLinkList != null)
                    {
                        string version = obj.ToString();
                        var packs = new List<string>();

                        foreach (string path in _pathList)
                        {
                            var dir = new DirectoryInfo(path).GetDirectories(version + "*")[0].FullName;
                            dir = System.IO.Path.Combine(dir, "ref");
                            dir = new DirectoryInfo(dir).GetDirectories()[0].FullName;

                            packs.Add(dir);
                        }
                        Model.Packs = packs;

                        version = version.Substring(0, 3);
                        if (_languageLinkList.ContainsKey(version))
                        {
                            Model.LanguageList = _languageLinkList[version].Keys.ToList();
                        }
                        else
                        {
                            var v = _languageLinkList.Keys.ToList();
                            Model.LanguageList = _languageLinkList[v[^1]].Keys.ToList();
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("语言包未加载，请稍后！");
                    }

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
                return new DelegateCommand<object>(async delegate (object obj)
                {
                    object[] objs = obj as object[];
                    string version = objs[0].ToString();
                    version = version.Substring(0, 3);
                    string language = objs[1].ToString();

                    string url;
                    if (_languageLinkList.ContainsKey(version))
                    {
                        url = _languageLinkList[version][language];
                    }
                    else
                    {
                        var v = _languageLinkList.Keys.ToList();
                        url = _languageLinkList[v[^1]][language];
                    }


                    string fileName = Path.GetFileName(url);
                    string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName);
                    fileName = Path.GetFileNameWithoutExtension(url);
                    A(fileName);
                    //// 下载
                    //ThreadPool.QueueUserWorkItem(async state =>
                    //{
                    //    IProgress<HttpDownloadProgress> progress = new Progress<HttpDownloadProgress>(progress =>
                    //    {
                    //        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    //        {
                    //            Model.DownloadProgress = progress.BytesReceived * 100.0 / progress.TotalBytesToReceive ?? 1;
                    //        });
                            
                    //        if (progress.BytesReceived == progress.TotalBytesToReceive)
                    //        {
                    //            // 解压
                    //            new ZipLibHelper().UnzipZip(path, System.AppDomain.CurrentDomain.BaseDirectory);
                    //            // 覆盖
                    //            path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName, fileName, _pathList[0]);

                    //            MessageBox.Show("完成");
                    //        }
                    //    });
                    //    await new HttpClientExtensions().GetByteArrayAsync(url, path, progress, CancellationToken.None);
                    //});
                    



                });
            }
        }

        private async Task<IDictionary<string, IDictionary<string, string>>> GetLanguageLinkList()
        {
            IDictionary<string, IDictionary<string, string>> dic = new Dictionary<string, IDictionary<string, string>>();
            string url = "https://dotnet.microsoft.com/download/intellisense";
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

                dic.Add(v, dic1);
            }

            return dic;
        }

        private void A(string fileName)
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName, fileName, "Microsoft.NETCore.App.Ref");
            var dir1 = new DirectoryInfo(path).GetDirectories()[0].FullName;
            path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, fileName, fileName, "Microsoft.WindowsDesktop.App.Ref");
            var dir2 = new DirectoryInfo(path).GetDirectories()[0].FullName;
            FileHelper.CopyDir2(dir1, Model.Packs[0]);
            FileHelper.CopyDir2(dir2, Model.Packs[1]);
        }

    }
}