using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AutomaçãoTEL.ViewModel;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutomaçãoTEL.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Config : Page
    {
        
        public ConfigViewModel ControlerConfig { get; set; }
        private ObservableCollection<ComboBoxItem> ComboBoxOptions = new ObservableCollection<ComboBoxItem>();

        public Config()
        {
            this.InitializeComponent();
            ComboBoxOptionsManager.GetComboBoxList(ComboBoxOptions);
            ControlerConfig = new ConfigViewModel();
        }
        private ComboBoxItem _SelectedComboBoxOption;
        public ComboBoxItem SelectedComboBoxOption
        {
            get
            {
                return _SelectedComboBoxOption;
            }
            set
            {
                _SelectedComboBoxOption = value;
            }
        }

        public class ComboBoxOptionsManager
        {
            public static void GetComboBoxList(ObservableCollection<ComboBoxItem> ComboBoxItems)
            {
                var allItems = getComboBoxItems();
                ComboBoxItems.Clear();
                allItems.ForEach(p => ComboBoxItems.Add(p));
            }

            private static List<ComboBoxItem> getComboBoxItems()
            {
                var items = new List<ComboBoxItem>();

                items.Add(new ComboBoxItem() { Content = "Largura de 1 - BLE" });
                items.Add(new ComboBoxItem() { Content = "Largura de 2 - BLE" });
                items.Add(new ComboBoxItem() { Content = "Largura de 20" });
                items.Add(new ComboBoxItem() { Content = "Largura de 40" });
                items.Add(new ComboBoxItem() { Content = "Largura de 80" });
                items.Add(new ComboBoxItem() { Content = "Largura de 160" });

                return items;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControlerConfig.Configs.WifiConfigs.Bw = SelectedComboBoxOption.Content.ToString();
        }
    }
}
