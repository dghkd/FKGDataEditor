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
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<GirlInfo> _girls = new List<GirlInfo>();
        private ObservableCollection<GirlInfoVM> _girlColle = new ObservableCollection<GirlInfoVM>();
        
        public MainWindow()
        {
            InitializeComponent();
            
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
