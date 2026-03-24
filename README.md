<h1 align="center">Automação Ensaios TEL</h1>

<p align="center">
  <img src="http://img.shields.io/static/v1?label=STATUS&message=EM+DESENVOLVIMENTO&color=blue&style=for-the-badge"/>
  <img src="http://img.shields.io/static/v1?label=PLATAFORMA&message=Windows+UWP&color=informational&style=for-the-badge"/>
  <img src="http://img.shields.io/static/v1?label=LINGUAGEM&message=C%23&color=purple&style=for-the-badge"/>
</p>

## Sobre o Projeto

Software desenvolvido no LABELO para automatizar ensaios de certificação de dispositivos de telecomunicações, conforme a **Norma 14448 da Anatel** (itens 10, 15 e 20).

O sistema se comunica com analisadores de espectro via TCP/SCPI, executa as sequências de medição automaticamente, salva os resultados em banco de dados Firebird e exporta os dados em CSV por ensaio.

## Arquitetura

O projeto é dividido em quatro bibliotecas com responsabilidades bem definidas:

```
AutomaçãoTEL/          → Interface UWP (XAML + code-behind)
├── Views/             → Páginas: Home, Wifi, Bluetooth, Config, Login
└── ViewModel/         → BTViewModel, WifiViewModel, ConfigViewModel

MainSpecAn/            → Lógica de negócio
├── Interfaces/        → ISpectrumAnalyzer, MeasurementConfig
├── Session/           → TestSession, SpectrumAnalyzerFactory
├── Assays/            → Uma classe por ensaio (sem acoplamento ao instrumento)
└── AssayRunner        → Orquestrador: recebe a sessão, executa e persiste

ConnectLan/            → Comunicação TCP/SCPI (socket raw)
DataBaseClass/         → Acesso ao banco de dados Firebird
```

**Para adicionar suporte a um novo instrumento** (ex: ESR da Rohde & Schwarz):
1. Criar `MainSpecAn/SpecAn/ESR.cs` implementando `ISpectrumAnalyzer`
2. Registrar o novo case em `SpectrumAnalyzerFactory.cs`

Nenhuma outra parte do código precisa ser alterada.

## Instrumentos Suportados

| Fabricante | Modelo | Status |
|---|---|---|
| Keysight | N9010A EXA | Implementado |
| Rohde & Schwarz | ESR | Planejado |

## Ensaios Disponíveis

### WiFi (802.11 a/b/g/n/ac/ax e BLE)

| Ensaio | Norma |
|---|---|
| Occupied Bandwidth at -6 dB | 14448 item 10 |
| Occupied Bandwidth at -26 dB | 14448 item 10 |
| Maximum Peak Power | 14448 item 15 |
| Average Maximum Output Power | 14448 item 15 |
| Peak Power Spectral Density | 14448 item 20 |
| Average Power Spectral Density | 14448 item 20 |
| Output Power | 14448 item 15 |
| Power Spectral Density | 14448 item 20 |
| Out-of-Band Emissions | Planejado |

### Bluetooth (GFSK, PI/4 DQPSK, 8DPSK)

| Ensaio | Norma |
|---|---|
| Occupied Bandwidth at -20 dB | 14448 |
| Maximum Peak Power | 14448 |
| Peak Power Spectral Density | 14448 |
| Number of Occupations | 14448 |
| Occupation Time | 14448 |
| Hopping Channel Separation | 14448 |
| Out-of-Band Emissions | Planejado |

## Requisitos

- Windows 10 / 11 (UWP)
- Conexão LAN com o analisador de espectro
- Banco de dados Firebird (arquivo `.fdb` incluído em `entrypoint/BancoDeDados/`)
- Para descobrir o IP do instrumento: [NI-VISA](https://www.ni.com/pt-br/support/downloads/drivers/download.ni-visa.html)

## Como Usar

1. **Conectar ao instrumento** — Na tela Home, insira o IP do analisador e clique em Conectar
2. **Selecionar pasta de saída** — Escolha onde os arquivos CSV e imagens serão salvos
3. **Configurar parâmetros** — Na tela Config, defina Reference Level, Atenuação e frequências por modulação
4. **Executar ensaios** — Na tela Wifi ou Bluetooth:
   - Selecione as modulações desejadas (toggle em "Modulações")
   - Alterne para "Ensaios" e selecione os itens da norma
   - Clique em **Confirmar** — o software executa tudo automaticamente

Os resultados são salvos em:
```
<pasta_saída>/<alias>/<Wifi|Bluetooth>/<modulação>/<ensaio>.csv
<pasta_saída>/<alias>/<Wifi|Bluetooth>/<modulação>/<freq> <ensaio>.png
```

## Melhorias Implementadas (refatoração atual)

- Separação por camadas: interface `ISpectrumAnalyzer` desacopla instrumento dos ensaios
- `TestSession` substitui estado global espalhado pela aplicação
- Uma classe por ensaio — fácil adicionar, testar e modificar individualmente
- `Thread.Sleep` substituído por `await Task.Delay` (não trava a UI)
- Bugs críticos corrigidos: sintaxe, transação SQL, loop de recebimento TCP
- SQL injection prevenido via whitelist de nomes de tabela
- Nomes dos ensaios padronizados em inglês
