using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Net;
using System.Collections.ObjectModel;

namespace FKGDataEditor.WikiParser
{
    public class EngWikiParser
    {
        public static void DownloadImage()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            for (int i = 2; i < 7; i++)
            {
                String data = String.Format(@"{0}star.txt", i);
                HtmlDocument doc = new HtmlDocument();
                doc.Load(File.Open(data, FileMode.Open));
                HtmlNodeCollection nodeColle = doc.DocumentNode.SelectNodes(@"//tr");
                Directory.CreateDirectory(@"FKGImages");
                foreach (HtmlNode node in nodeColle)
                {
                    String NamesJPN = node.SelectSingleNode(@"./td[4]").InnerText.Replace("\r\n", "");
                    HtmlNode n = node.SelectSingleNode(@"./td[2]");
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(node.SelectSingleNode(@"./td[2]/a[1]/img[1]").Attributes["src"].Value);
                    MemoryStream ms = new MemoryStream(bytes);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    img.Save(@".\FKGImages\" + NamesJPN + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }


        public static List<GirlInfo> LoadInfoFromTxtData()
        {
            //DownloadImage();

            List<GirlInfo> ret = new List<GirlInfo>();
            for (int i = 2; i < 7; i++)
            {
                String data = String.Format(@"{0}star.txt", i);
                HtmlDocument doc = new HtmlDocument();
                doc.Load(File.Open(data, FileMode.Open));
                HtmlNodeCollection nodeColle = doc.DocumentNode.SelectNodes(@"//tr");
                foreach (HtmlNode node in nodeColle)
                {
                    GirlInfo info = new GirlInfo();
                    int id = Convert.ToInt32(node.SelectSingleNode(@"./td[1]").InnerText);
                    String type = node.SelectSingleNode(@"./td[3]").InnerText.Replace("\r\n", "");
                    String NamesJPN = node.SelectSingleNode(@"./td[4]").InnerText.Replace("\r\n", "");
                    String NamesENU = node.SelectSingleNode(@"./td[5]").InnerText.Replace("\r\n", "");
                    String nationality = node.SelectSingleNode(@"./td[6]").InnerText.Replace("\r\n", "");
                    String imgUrl = node.SelectSingleNode(@"./td[2]/a[1]/img[1]").Attributes["src"].Value;
                    String fkgID = imgUrl.Substring(imgUrl.IndexOf("Icon_") + "Icon_".Length, 6);
                    info.Rare = i;
                    info.ID = id;
                    info.Type = GirlInfoEnum.String2Types[type];
                    info.Names = NamesJPN;
                    info.NamesJPN = NamesJPN;
                    info.NamesENU = NamesENU;
                    info.Nationality = GirlInfoEnum.String2Nationality[nationality];
                    info.FKGID = Convert.ToInt32(fkgID);
                    //info.ImgBase64 = GirlInfo.ImgFileToBase64(@"F:\Open source\FKGImages\" + NamesJPN + ".png");
                    //info.ImageSrc = GirlInfo.Base642Image(info.ImgBase64);

                    ret.Add(info);
                }

            }
            return ret;
        }
    }
}
