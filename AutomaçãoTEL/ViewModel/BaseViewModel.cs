using AutomaçãoTEL.Views;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace AutomaçãoTEL.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<T>(ref T property, T value,
        [CallerMemberName] string propertyName = "")
        {
            property = value;
            localSettings.Values[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        protected T GetProperty<T>(ref T property, [CallerMemberName] string propertyName = "")
        {
            if((T)localSettings.Values[propertyName] != null)
            {
                property = (T)localSettings.Values[propertyName];
                return property;
            }
            return default;
        }

        protected void SetPropertyWifi<T>(ref T property, T value, string bw,
        [CallerMemberName] string propertyName = "")
        {
            localSettings.CreateContainer(bw as string, ApplicationDataCreateDisposition.Always);
            property = value;
            localSettings.Containers[bw as string].Values[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        protected T GetPropertyWifi<T>(ref T property, T bw, [CallerMemberName] string propertyName = "")
        {
            try
            {
                localSettings.CreateContainer(bw as string, ApplicationDataCreateDisposition.Always);
                if ((T)localSettings.Containers[bw as string].Values[propertyName] != null)
                {
                    property = (T)localSettings.Containers[bw as string].Values[propertyName];
                    return property;
                }
                return default;
            }
            catch
            {
                return default;
            }
            
            
        }

        protected void SetPropertyBw<T>(ref T property, ref string freqI, ref string freqC, ref string freqF, ref string freqEspI, ref string freqEspF, T value ,[CallerMemberName] string propertyName = "")
        {
            try
            {
                localSettings.CreateContainer(value as string, ApplicationDataCreateDisposition.Always);
                property = value;
                if (localSettings.Containers[value as string].Values != null)
                {
                    SetPropertyWifi(ref freqI, (string)localSettings.Containers[value as string].Values["FreqIWifi"] ?? " ", value as string, "FreqIWifi"); 
                    SetPropertyWifi(ref freqC, (string)localSettings.Containers[value as string].Values["FreqCWifi"] ?? " ", value as string, "FreqCWifi");
                    SetPropertyWifi(ref freqF, (string)localSettings.Containers[value as string].Values["FreqFWifi"] ?? " ", value as string, "FreqFWifi");
                    SetPropertyWifi(ref freqEspI, (string)localSettings.Containers[value as string].Values["FreqIEspWifi"] ?? " ", value as string, "FreqIEspWifi");
                    SetPropertyWifi(ref freqEspF, (string)localSettings.Containers[value as string].Values["FreqFEspWifi"] ?? " ", value as string, "FreqFEspWifi");
                }
                else
                {
                    SetPropertyWifi(ref freqI, "", value as string, propertyName);
                    SetPropertyWifi(ref freqC, "", value as string, propertyName);
                    SetPropertyWifi(ref freqF, "", value as string, propertyName);
                    SetPropertyWifi(ref freqEspI, "", value as string, propertyName);
                    SetPropertyWifi(ref freqEspF, "", value as string, propertyName);
                }

            }
            catch 
            {
                localSettings.CreateContainer("Largura de 1 - BLE", ApplicationDataCreateDisposition.Always);
            }
            
        }

    }

    public class Item : BaseViewModel
    {

        private string _name;
        public string Name
        {
            get => _name;

            set => SetProperty(ref _name, value);
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;

            set => SetProperty(ref _isChecked, value);
        }


    }

}
