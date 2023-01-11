using AutomaçãoTEL.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace AutomaçãoTEL.ViewModel
{

    public class ConfigViewModel : BaseViewModel
    {
        private LocalConfig _configs = new LocalConfig();
        public LocalConfig Configs { get { return this._configs; } set { this._configs = value; } }
    }


    public class LocalConfig : BaseViewModel
    {
        private ConfigWifi _wifiConfigs = new ConfigWifi();
        public ConfigWifi WifiConfigs { get { return this._wifiConfigs; } }

        private ConfigBT _btConfigs = new ConfigBT();
        public ConfigBT BTConfigs { get { return this._btConfigs; } }

        private string _refLevel;
        public string RefLevel
        {
            get => GetProperty(ref _refLevel);

            set 
            {
                SetProperty(ref _refLevel, value); 
            } 
        }

        private string _att;
        public string Att
        {
            get => GetProperty(ref _att);

            set
            {
                SetProperty(ref _att, value);
            }
        }

        private bool _isCheckedPrintI;
        public bool IsCheckedPrintI
        {
            get => GetProperty(ref _isCheckedPrintI);

            set
            {
                SetProperty(ref _isCheckedPrintI, value);
            }
        }

        private bool _isCheckedPrintC;
        public bool IsCheckedPrintC
        {
            get => GetProperty(ref _isCheckedPrintC);

            set
            {
                SetProperty(ref _isCheckedPrintC, value);
            }
        }

        private bool _isCheckedPrintF;
        public bool IsCheckedPrintF
        {
            get => GetProperty(ref _isCheckedPrintF);

            set
            {
                SetProperty(ref _isCheckedPrintF, value);
            }
        }

        private bool _isCheckedFreqI;
        public bool IsCheckedFreqI
        {
            get => GetProperty(ref _isCheckedFreqI);

            set
            {
                SetProperty(ref _isCheckedFreqI, value);
            }
        }

        private bool _isCheckedFreqC;
        public bool IsCheckedFreqC
        {
            get => GetProperty(ref _isCheckedFreqC);

            set
            {
                SetProperty(ref _isCheckedFreqC, value);
            }
        }

        private bool _isCheckedFreqF;
        public bool IsCheckedFreqF
        {
            get => GetProperty(ref _isCheckedFreqF);

            set
            {
                SetProperty(ref _isCheckedFreqF, value);
            }
        }

        
    }

    public class ConfigBT : BaseViewModel
    {
        private string _freqIBT;
        public string FreqIBT
        {
            get => GetProperty(ref _freqIBT);

            set
            {
                SetProperty(ref _freqIBT, value);
            }
        }

        private string _freqCBT;
        public string FreqCBT
        {
            get => GetProperty(ref _freqCBT);

            set
            {
                SetProperty(ref _freqCBT, value);
            }
        }

        private string _freqFBT;
        public string FreqFBT
        {
            get => GetProperty(ref _freqFBT);

            set
            {
                SetProperty(ref _freqFBT, value);
            }
        }

        private string _freqIEspBT;
        public string FreqIEspBT
        {
            get => GetProperty(ref _freqIEspBT);

            set
            {
                SetProperty(ref _freqIEspBT, value);
            }
        }

        private string _freqFEspBT;
        public string FreqFEspBT
        {
            get => GetProperty(ref _freqFEspBT);

            set
            {
                SetProperty(ref _freqFEspBT, value);
            }
        }

        private string _freqISalto;
        public string FreqISalto
        {
            get => GetProperty(ref _freqISalto);

            set
            {
                SetProperty(ref _freqISalto, value);
            }
        }

        private string _freqMSalto;
        public string FreqMSalto
        {
            get => GetProperty(ref _freqMSalto);

            set
            {
                SetProperty(ref _freqMSalto, value);
            }
        }

        private string _freqFSalto;
        public string FreqFSalto
        {
            get => GetProperty(ref _freqFSalto);

            set
            {
                SetProperty(ref _freqFSalto, value);
            }
        }
    }

    public class ConfigWifi : BaseViewModel
    {
        private string _bw;
        public string Bw
        {
            get => GetPropertyBw(ref _bw);

            set
            {
                SetPropertyBw(ref _bw, ref _freqIWifi, ref _freqCWifi, ref _freqFWifi, ref _freqIEspWifi, ref _freqFEspWifi, value);
            }
        }

        private string _freqIWifi;
        public string FreqIWifi
        {
            get => GetPropertyWifi(ref _freqIWifi, _bw);

            set
            {
                SetPropertyWifi(ref _freqIWifi, value, _bw);
            }
        }

        private string _freqCWifi;
        public string FreqCWifi
        {
            get => GetPropertyWifi(ref _freqCWifi, _bw);

            set
            {
                SetPropertyWifi(ref _freqCWifi, value, _bw);
            }
        }

        private string _freqFWifi;
        public string FreqFWifi
        {
            get => GetPropertyWifi(ref _freqFWifi, _bw);

            set
            {
                SetPropertyWifi(ref _freqFWifi, value, _bw);
            }
        }

        private string _freqIEspWifi;
        public string FreqIEspWifi
        {
            get => GetPropertyWifi(ref _freqIEspWifi, _bw);

            set
            {
                SetPropertyWifi(ref _freqIEspWifi, value, _bw);
            }
        }

        private string _freqFEspWifi;
        public string FreqFEspWifi
        {
            get => GetPropertyWifi(ref _freqFEspWifi, _bw);

            set
            {
                SetPropertyWifi(ref _freqFEspWifi, value, _bw);
            }
        }
    }

}
