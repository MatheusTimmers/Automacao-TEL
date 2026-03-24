using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutomaçãoTEL.ViewModel
{
    public class WifiViewModel
    {
        private ObservableCollection<Item> _itens = new ObservableCollection<Item>();
        public ObservableCollection<Item> Itens { get { return this._itens; } }
        public bool TsItemsIsOn { get; set; }

        public readonly List<string> namesAssayWifi = new List<string>
        {
            "Selecionar Todos",
            "Largura de Faixa a 6 dB",
            "Largura de Faixa a 26 dB",
            "Potência de Pico Máxima",
            "Valor Médio da Potência máxima de Saida",
            "Pico da Densidade de Potência",
            "Valor Médio da Densidade Espectral de Potência",
            "Emissão fora de faixa",
            "Potência de Saída",
            "Densidade Espectral de Potência"
        };

        public readonly List<string> namesModulations = new List<string>
        {
            "Selecionar Todos",
            "Bluetooth Low Energy",
            "802.11a",
            "802.11b",
            "802.11g",
            "802.11n (20)",
            "802.11n (40)",
            "802.11n (80)",
            "802.11ac (20)",
            "802.11ac (40)",
            "802.11ac (80)",
            "802.11ax (20)",
            "802.11ax (40)",
            "802.11ax (80)",
            "802.11ax (160)"
        };

        public void AttList()
        {
            this.Itens.Clear();
            var source = TsItemsIsOn ? namesAssayWifi : namesModulations;
            foreach (var name in source)
                this._itens.Add(new Item { Name = name });
        }
    }
}
