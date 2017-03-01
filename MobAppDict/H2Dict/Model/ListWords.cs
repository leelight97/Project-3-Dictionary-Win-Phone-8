using H2Dict.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2Dict.Model
{
    public class ListWords
    {
        public ListWords()
        {
            _lstKey = new List<string>();
            _lstLength = new List<string>();
            _lstOffset = new List<string>();
        }

        private List<string> _lstKey;

        public List<string> LstKey
        {
            get { return _lstKey; }
            set { _lstKey = value; }
        }

        private List<string> _lstOffset;

        public List<string> LstOffset
        {
            get { return _lstOffset; }
            set { _lstOffset = value; }
        }

        private List<string> _lstLength;

        public List<string> LstLength
        {
            get { return _lstLength; }
            set { _lstLength = value; }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propName));
            }
        }

        // Method Execute
        public string Search(string key)
        {
            string res = null;



            return res;
        }
    }
}
