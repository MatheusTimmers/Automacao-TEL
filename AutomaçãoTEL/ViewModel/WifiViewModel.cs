using AutomaçãoTEL.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaçãoTEL.ViewModel
{
    public class WifiViewModel
    {

        private ObservableCollection<Item> _itens = new ObservableCollection<Item>();
        public ObservableCollection<Item> Itens { get { return this._itens; } }
        public bool TsItemsIsOn { get; set; }

        readonly List<string> namesAssayWifi = new List<string> { "Selecionar Todos", "Largura de Faixa a 6 dB", "Largura de Faixa a 26 dB", "Potência de Pico Máxima", "Valor Médio da Potência máxima de Saida", "Pico da Densidade de Potência", "Valor Médio da Densidade Espectral de Potência", "Emissão fora de faixa" };
        readonly List<string> namesModulations = new List<string> { "Selecionar Todos", "Bluetooth Low Energy", "802.11a", "802.11b", "802.11g", "802.11n (20)", "802.11n (40)", "802.11n (80)", "802.11ac (20)", "802.11ac (40)", "802.11ac (80)", "802.11ax (20)", "802.11ax (40)", "802.11ax (80)", "802.11ax (160)" };
        public void AttList()
        {
            this.Itens.Clear();
            if (TsItemsIsOn)
            {
                for (int i = 0; i < namesAssayWifi.Count; i++)
                {
                    this._itens.Add(new Item()
                    {
                        Name = namesAssayWifi[i]
                    });
                }
            }
            else
            {
                for (int i = 0; i < namesModulations.Count; i++)
                {
                    this._itens.Add(new Item()
                    {
                        Name = namesModulations[i]
                    });
                }
            }
        }
    }
}
