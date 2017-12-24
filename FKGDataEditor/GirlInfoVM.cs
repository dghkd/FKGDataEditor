using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;

namespace FKGDataEditor
{
    public class GirlInfoVM : INotifyPropertyChanged
    {
        #region Private Member
        private GirlInfo _info;
        #endregion


        #region Constructor
        public GirlInfoVM(GirlInfo info)
        {
            _info = info;
        }
        #endregion


        #region Public Member
        public int ID
        {
            get { return _info.ID; }
            set { _info.ID = value; OnPropertyChanged("ID"); }
        }

        public ImageSource ImageSrc
        {
            get
            {
                if (_info.ImageSrc == null)
                {
                    _info.ImageSrc = GirlInfo.Base642Image(this.ImgBase64);
                }
                return _info.ImageSrc;
            }
            set { _info.ImageSrc = value; OnPropertyChanged("ImageSrc"); }
        }

        public String ImgBase64
        {
            get { return _info.ImgBase64; }
            set
            {
                _info.ImgBase64 = value;
                _info.ImageSrc = GirlInfo.Base642Image(this.ImgBase64);
                OnPropertyChanged("ImgBase64"); OnPropertyChanged("ImageSrc");
            }
        }
        public String Names
        {
            get { return _info.Names; }
            set { _info.Names = value; OnPropertyChanged("Names"); }
        }

        public String NamesJPN
        {
            get { return _info.NamesJPN; }
            set { _info.NamesJPN = value; OnPropertyChanged("NamesJPN"); }
        }

        public String NamesCHT
        {
            get { return _info.NamesCHT; }
            set { _info.NamesCHT = value; OnPropertyChanged("NamesCHT"); }
        }
        public String NamesCHS
        {
            get { return _info.NamesCHS; }
            set { _info.NamesCHS = value; OnPropertyChanged("NamesCHS"); }
        }
        public String NamesENU
        {
            get { return _info.NamesENU; }
            set { _info.NamesENU = value; OnPropertyChanged("NamesENU"); }
        }

        public int Rare
        {
            get { return _info.Rare; }
            set { _info.Rare = value; OnPropertyChanged("Rare"); }
        }
        public GirlInfoEnum.Types Type
        {
            get { return _info.Type; }
            set { _info.Type = value; OnPropertyChanged("Type"); }
        }
        public GirlInfoEnum.Nationalities Nationality
        {
            get { return _info.Nationality; }
            set { _info.Nationality = value; OnPropertyChanged("Nationality"); }
        }
        
        public String Note
        {
            get { return _info.Note; }
            set { _info.Note = value; OnPropertyChanged("Note"); }
        }


        public override string ToString()
        {
            return String.Format("{0} {1} {2}", this.NamesJPN, this.NamesENU, this.NamesCHT);

        }
        #endregion


        #region Public Method
        public GirlInfo GetDataInfo()
        {
            return _info;
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
