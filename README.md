<h1 align="center"> Automacao Ensaios TEL </h1>
<p align="center">
<img src="http://img.shields.io/static/v1?label=STATUS&message=PAUSADO&color=yellow&style=for-the-badge"/>
</p>

## ❓ Sobre o Projeto

Projeto iniciado no meu tempo livre no meu trabalho no LABELO na área de telecomunicações, devido a uma série de melhorias e erros a serem corrigidos da minha primeira automação para os ensaios de WIFI e Bluetooth e
uma necessidade de uma interface que englobasse mais ensaios, além dos de Wifi e bluetooth desenvolvidos no diretório Automacao-Ensaios-WIFI-Bluetooth, assim tendo mais
facilidade na hora de implementar novos ensaios e correções no codigo.

- Programa solicita valores e prints da tela de um analisador de espectro, de acordo com o item 10, 15, 20 da Norma 14448 da Anatel, salva esses valores em um banco de dados
mantendo um registros de quem realizou o ensaio e dos valores de cada um deles, além de gerar um relatório no excel já configurado para enviar para o cliente
(https://informacoes.anatel.gov.br/legislacao/atos-de-certificacao-de-produtos/2017/1139-ato-14448)

## 🛠️ Equipamentos

Automação feita para os modelos de analizadores de espectro, N9010A da Keysight e para o ESR da Rohde e Schwarz

## 💻 Como Utilizar

Para o funcionamento do programa sera necessário:

- Uma conexão LAN com um Analisador de Espectro;

- Um Software da sua escolha para descobrir o IP do Analisador de espectro
(Recomendo: https://www.ni.com/pt-br/support/downloads/drivers/download.ni-visa.html#442805)

- Escolher um ensaio disponivel na tela de navegação;
      (Atualmente disponivel apenas os ensaios de WIFI e Bluetooth, mais tarde quando o software estiver mais consolidado, será adicionado os ensaios de Estabilizadores e de Radios)
- Escolher a modulação solicitada do ensaio desejado;
- Selecionar as frequências de ensaio;
- Selecionar os Items solicitados na norma;
- Confirmar.

## ✨Melhorias e Erros resolvidos - Implementadas e planejadas

- Interface Global (Para futuramente ser adicionado mais ensaios, tudo em um mesmo programa);
- Melhoria no design da tela. Utilizando um projeto UWP a linguagem XAML;
- Integração com Banco de Dados para salvar os ensaios de maneira mais organizada e segura
- Criação de logica de Usuário para salvar o funcionario que realizou o ensaio.
- Erros na logicas de ensaio corrigidos
