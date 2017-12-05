using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Net;
using System.Collections.ObjectModel;

namespace FKGDataEditor
{
    public class ChName
    {
        public String NamesCHT { get; set; }
        public String NamesCHS { get; set; }
    }

    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<GirlInfo> _girls = new List<GirlInfo>();
        private ObservableCollection<GirlInfoVM> _girlColle = new ObservableCollection<GirlInfoVM>();

        private Dictionary<String, ChName> _dict = new Dictionary<string, ChName>();

        public MainWindow()
        {
            InitializeComponent();

            //List<String> nameContent = new List<string>();
            //nameContent = File.ReadLines(@"F:\Open source\FKGNames.txt").ToList();

            //foreach (String l in nameContent)
            //{
            //    string[] names = l.Split('\t');
            //    ChName cname = new ChName();
            //    cname.NamesCHT = names[1];
            //    cname.NamesCHS = names[2];
            //    _dict.Add(names[0], cname);
            //}

            SQLiteCtrl.Data.Init();
            Task.Factory.StartNew(() =>
            {
                _girls = SQLiteCtrl.Data.LoadData();
            
                foreach (GirlInfo info in _girls)
                {
                    GirlInfoVM vm = new GirlInfoVM(info);
                    _girlColle.Add(vm);
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    CMB_Type.ItemsSource = Enum.GetValues(typeof(GirlInfoEnum.Types));
                    CMB_Nationality.ItemsSource = Enum.GetValues(typeof(GirlInfoEnum.Nationalities));
                    CMB_Girls.ItemsSource = _girlColle;
                    CMB_Girls.SelectedIndex = 0;
                });
            });
            

            
        }

        private void DownloadImage()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            for (int i = 2; i < 7; i++)
            {
                String data = String.Format(@"F:\Open source\{0}star.txt", i);
                HtmlDocument doc = new HtmlDocument();
                doc.Load(File.Open(data, FileMode.Open));
                HtmlNodeCollection nodeColle = doc.DocumentNode.SelectNodes(@"//tr");
                foreach (HtmlNode node in nodeColle)
                {
                    String NamesJPN = node.SelectSingleNode(@"./td[4]").InnerText.Replace("\r\n", "");
                    HtmlNode n = node.SelectSingleNode(@"./td[2]");
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(node.SelectSingleNode(@"./td[2]/a[1]/img[1]").Attributes["src"].Value);
                    MemoryStream ms = new MemoryStream(bytes);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    img.Save(@"F:\Open source\FKGImages\" + NamesJPN + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        private List<GirlInfo> LoadInfoFromTxtData()
        {
            //DownloadImage();

            List<GirlInfo> ret = new List<GirlInfo>();
            for (int i = 2; i < 7; i++)
            {
                String data = String.Format(@"F:\Open source\{0}star.txt", i);
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

                    info.Rare = i;
                    info.ID = id;
                    info.Type = GirlInfoEnum.String2Types[type];
                    info.Names = NamesJPN;
                    info.NamesJPN = NamesJPN;
                    info.NamesENU = NamesENU;
                    info.Nationality = GirlInfoEnum.String2Nationality[nationality];
                    info.ImgBase64 = GirlInfo.ImgFileToBase64(@"F:\Open source\FKGImages\" + NamesJPN + ".png");
                    //info.ImageSrc = GirlInfo.Base642Image(info.ImgBase64);

                    ret.Add(info);
                }

            }
            return ret;
        }

        private void On_BTN_Prev_Click(object sender, RoutedEventArgs e)
        {
            GirlInfoVM curSelVM = CMB_Girls.SelectedItem as GirlInfoVM;
            SQLiteCtrl.Data.InsertData(curSelVM.GetDataInfo());
            if (CMB_Girls.SelectedIndex > 0)
            {
                CMB_Girls.SelectedIndex--;
            }
        }

        private void On_BTN_Next_Click(object sender, RoutedEventArgs e)
        {
            GirlInfoVM curSelVM = CMB_Girls.SelectedItem as GirlInfoVM;
            SQLiteCtrl.Data.InsertData(curSelVM.GetDataInfo());
            if (CMB_Girls.SelectedIndex < _girlColle.Count - 1)
            {
                CMB_Girls.SelectedIndex++;
            }
        }

        private void On_BTN_CreateNew_Click(object sender, RoutedEventArgs e)
        {
            GirlInfoVM vm = new GirlInfoVM(new GirlInfo());
            vm.Rare = 2;
            vm.ID = _girlColle.ElementAt(_girlColle.Count - 1).ID + 1;

            _girlColle.Add(vm);
            CMB_Girls.SelectedItem = vm;

        }

        private void On_BTN_ChangeIcon_Click(object sender, RoutedEventArgs e)
        {
            GirlInfoVM vm = CMB_Girls.SelectedItem as GirlInfoVM;

            using (System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image files (*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.Png|All files (*.*)|*.*";

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    vm.ImgBase64 = GirlInfo.ImgFileToBase64(dlg.FileName);
                }
            }
        }
    }
}
