using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaçãoTEL.ViewModel
{
    public class BTViewModel
    {
        private ObservableCollection<Item> _itens = new ObservableCollection<Item>();
        public ObservableCollection<Item> Itens { get { return this._itens; } }
        public bool TsItemsIsOn { get; set; }

        readonly List<string> namesAssayBT = new List<string> { "Selecionar Todos", "Largura de Faixa a 20 db", "Potência de Pico Máxima", "Emissão Fora da Faixa", "Separação de Canais de Salto", "Numero de Frequencia de Salto", "Numero de Ocupações", "Tempo de Ocupação" };
        readonly List<string> namesModulations = new List<string> { "Selecionar Todos", "GFSK", "PI/4 DQPSK", "8DPSK" };
        public void AttList()
        {
            this.Itens.Clear();
            if (TsItemsIsOn)
            {
                for (int i = 0; i < namesAssayBT.Count; i++)
                {
                    this._itens.Add(new Item()
                    {
                        Name = namesAssayBT[i]
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
