using System;
using System.Collections.Generic;
using Hikari.Common.Net.Http;
using NSoup.Nodes;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;
using Xunit.Abstractions;

namespace TestProject1
{
    public class UnitTest1
    {
        private ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void Test1()
        {
            var v = GetLanguageLinkList();



            Assert.True(true);
        }

        private IDictionary<string, IDictionary<string, string>> GetLanguageLinkList()
        {
            IDictionary<string, IDictionary<string, string>> dic = new Dictionary<string, IDictionary<string, string>>();
            string url = "https://dotnet.microsoft.com/download/intellisense";
            HttpClientHelper helper = new HttpClientHelper();
            string html = helper.GetAsync(url).GetAwaiter().GetResult();
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
    }
}
