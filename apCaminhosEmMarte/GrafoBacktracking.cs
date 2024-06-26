﻿using apCaminhosEmMarte;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apCidadesBacktracking
{
    class GrafoBacktracking
    {
        const int tamanhoDistancia = 4;
        char tipoGrafo;
        int qtasCidades;
        int[,] matriz;
        public Dictionary<string, int> indicePorNome; //dicionário que relaciona o índice das cidades com o seu nome
        List<string> nomesCidades;

        public GrafoBacktracking(string nomeArquivo)  //Construtor original
        {
            using (var arquivo = new StreamReader(nomeArquivo))
            {
                var linhas = File.ReadAllLines(nomeArquivo).ToList();
                tipoGrafo = linhas[0][0]; // acessa primeiro caracter com tipo do grafo
                qtasCidades = linhas.Count - 1; // subtrai 1 para a linha do tipo de grafo
                matriz = new int[qtasCidades, qtasCidades];
                indicePorNome = new Dictionary<string, int>();
                nomesCidades = new List<string>();

                // Inicializa a matriz com -1 para indicar que não há caminhos inicialmente
                for (int i = 0; i < qtasCidades; i++)
                {
                    for (int j = 0; j < qtasCidades; j++)
                    {
                        matriz[i, j] = -1;
                    }
                }

                string linhaAtual;
                while ((linhaAtual = arquivo.ReadLine()) != null)
                {
                    // Garantir que a linha tem pelo menos 35 caracteres
                    if (linhaAtual.Length < 35)
                    {
                        continue; // Ignora linhas que não têm todas as informações necessárias
                    }

                    // Extrair as informações com base nas posições fixas
                    string cidadeOrigem = linhaAtual.Substring(0, 15).Trim();
                    string cidadeDestino = linhaAtual.Substring(15, 15).Trim();
                    int distancia = int.Parse(linhaAtual.Substring(30, 5).Trim());

                    // Adicionar as cidades e seus índices ao dicionário e lista
                    AdicionarCidade(cidadeOrigem);
                    AdicionarCidade(cidadeDestino);

                    // Adicionar o caminho na matriz com a distância correspondente
                    int indiceOrigem = indicePorNome[cidadeOrigem];
                    int indiceDestino = indicePorNome[cidadeDestino];
                    matriz[indiceOrigem, indiceDestino] = distancia;
                }
            }
        }


        private void AdicionarCidade(string nomeCidade)
        {
            if (!indicePorNome.ContainsKey(nomeCidade))
            {
                int indice = indicePorNome.Count; // Próximo índice disponível
                indicePorNome[nomeCidade] = indice;
                nomesCidades.Add(nomeCidade);
            }
        }

        public char TipoGrafo { get => tipoGrafo; set => tipoGrafo = value; }
        public int QtasCidades { get => qtasCidades; set => qtasCidades = value; }
        public int[,] Matriz { get => matriz; set => matriz = value; }

        public void Exibir(DataGridView dgv)
        {


            dgv.RowCount = dgv.ColumnCount = qtasCidades;
            for (int coluna = 0; coluna < qtasCidades; coluna++)
            {
                dgv.Columns[coluna].HeaderText = coluna.ToString();
                dgv.Rows[coluna].HeaderCell.Value = coluna.ToString();
                dgv.Columns[coluna].Width = 50;
            }
            for (int linha = 0; linha < qtasCidades; linha++)
                for (int coluna = 0; coluna < qtasCidades; coluna++)
                    if (matriz[linha, coluna] != 0)
                        if (matriz[linha, coluna] != -1)
                        {
                            dgv[coluna, linha].Value = matriz[linha, coluna];
                        }

                        else
                        {
                            dgv[coluna, linha].Value = "";  // Deixa em branco se for -1
                        }
        }




        public void AdicionarCaminho(Cidade cidadeOrigem, Cidade cidadeDestino, int distancia)
        {
            // Verificar se as cidades já existem na matriz
            string cidadeOri = cidadeOrigem.Nome().Trim();
            string cidadeDes = cidadeDestino.Nome().Trim();

            if (!indicePorNome.ContainsKey(cidadeOri) || !indicePorNome.ContainsKey(cidadeDes))
            {
                throw new Exception("Cidades não estão presentes no grafo.");
            }

            // Obter os índices das cidades na matriz
            int indiceOrigem = indicePorNome[cidadeOri];
            int indiceDestino = indicePorNome[cidadeDes];

            // Verificar se já existe um caminho entre as cidades
            if (matriz[indiceOrigem, indiceDestino] == -1 || matriz[indiceDestino, indiceOrigem] == -1)
            {
                // Se já existe um caminho, atualiza a distância
                matriz[indiceOrigem, indiceDestino] = distancia;
            }
            else
            {
                // Se não existe caminho, cria um novo caminho
                matriz[indiceOrigem, indiceDestino] = distancia;
                MessageBox.Show("Caminho já existe, distância atualizada");
            }
        }

        public PilhaVetor<Movimento> BuscarCaminho(
                      int origem, int destino, ListBox lsb,
                      DataGridView dgvGrafo
                     )
        {
            int cidadeAtual, saidaAtual;
            bool achouCaminho = false,
                 naoTemSaida = false;
            bool[] passou = new bool[qtasCidades];
            // inicia os valores de “passou” pois ainda não foi em nenhuma cidade
            for (int indice = 0; indice < qtasCidades; indice++)
                passou[indice] = false;
            lsb.Items.Clear();
            cidadeAtual = origem;
            saidaAtual = 0;
            var pilha = new PilhaVetor<Movimento>();

            while (!achouCaminho && !naoTemSaida)
            {
                naoTemSaida = (cidadeAtual == origem && saidaAtual == qtasCidades && pilha.EstaVazia);

                if (!naoTemSaida)
                {
                    while ((saidaAtual < qtasCidades) && !achouCaminho)
                    {
                        // se não há saída pela cidade testada, verifica a próxima
                        if (matriz[cidadeAtual, saidaAtual] == 0)
                            saidaAtual++;
                        else
                        // se já passou pela cidade testada, vê se a próxima cidade permite saída
                        if (passou[saidaAtual])
                            saidaAtual++;
                        else
                        // se chegou na cidade desejada, empilha o local
                        // e termina o processo de procura de caminho
                        if (saidaAtual == destino)
                        {
                            Movimento movim = new Movimento(cidadeAtual, saidaAtual);
                            pilha.Empilhar(movim);
                            achouCaminho = true;
                            lsb.Items.Add($"Saiu de {cidadeAtual} para {saidaAtual}");
                            ExibirUmPasso();
                        }
                        else
                        {
                            lsb.Items.Add($"Saiu de {cidadeAtual} para {saidaAtual}");
                            ExibirUmPasso();
                            Movimento movim = new Movimento(cidadeAtual, saidaAtual);
                            pilha.Empilhar(movim);
                            passou[cidadeAtual] = true;
                            cidadeAtual = saidaAtual; // muda para a nova cidade
                            saidaAtual = 0; // reinicia busca de saídas da nova
                                            // cidade a partir da primeira cidade
                        }
                    }
                }
                if (!achouCaminho)
                    if (!pilha.EstaVazia)
                    {
                        var movim = pilha.Desempilhar();
                        saidaAtual = movim.Destino;
                        cidadeAtual = movim.Origem;
                        lsb.Items.Add($"Voltando de {saidaAtual} para {cidadeAtual}");
                        ExibirUmPasso();
                        saidaAtual++;
                    }
            }

            var saida = new PilhaVetor<Movimento>();
            if (achouCaminho)
            { // desempilha a configuração atual da pilha
              // para a pilha da lista de parâmetros
                while (!pilha.EstaVazia)
                {
                    Movimento movim = pilha.Desempilhar();
                    saida.Empilhar(movim);
                }
            }
            return saida;

            void ExibirUmPasso()
            {
                dgvGrafo.CurrentCell = dgvGrafo[saidaAtual, cidadeAtual];
                //pilha.Exibir(dgvPilha);
                Thread.Sleep(1000);
            }

        }

        public void AlterarDistancia(string cidadeOrigem, string cidadeDestino, int novaDistancia)
        {
            if (indicePorNome.ContainsKey(cidadeOrigem) && indicePorNome.ContainsKey(cidadeDestino))
            {
                //Indice por nome novamente
                int indiceOrigem = indicePorNome[cidadeOrigem];
                int indiceDestino = indicePorNome[cidadeDestino];

                //Altera a distância dos indices acima
                matriz[indiceOrigem, indiceDestino] = novaDistancia;

            }
            else
            {
                MessageBox.Show("nem todas as cidades foram encontradas no grafo.");

            }
        }


        public void ExcluirCaminho(string cidadeOrigem, string cidadeDestino)
        {
            if (indicePorNome.ContainsKey(cidadeOrigem) && indicePorNome.ContainsKey(cidadeDestino))
            {
                int indiceOrigem = indicePorNome[cidadeOrigem];
                int indiceDestino = indicePorNome[cidadeDestino];

                matriz[indiceOrigem, indiceDestino] = -1;

            }
            else
            {
                MessageBox.Show("Nem todas cidades foram encontradas no grafo.");

            }
        }

        public void SalvarGrafoEmArquivo(string nomeArquivo)
        {
            using (var arquivo = new StreamWriter(nomeArquivo))
            {
                // Escreve o tipo do grafo na primeira linha do arquivo
                arquivo.WriteLine(tipoGrafo);
                for (int i = 0; i < qtasCidades; i++)
                {
                    for (int j = 0; j < qtasCidades; j++)
                    {
                        if (matriz[i, j] != -1) // Se a distância não é -1, há um caminho
                        {
                            string nomeCidadeOrigem = nomesCidades[i];
                            string nomeCidadeDestino = nomesCidades[j];
                            int distancia = matriz[i, j];

                            string linha = $"{nomeCidadeOrigem.PadRight(15)}{nomeCidadeDestino.PadRight(15)}{distancia.ToString().PadLeft(5)}";
                            arquivo.WriteLine(linha);
                        }
                    }
                }
            }
        }
        //Metodos que poderiam fazer o papel semelhante ao dicionario

        /* 
     public int ObterIndicePorNome(string nomeCidade)
     {
         for (int i = 0; i < nomesCidades.Count; i++)
         {
             if (CompararStrings(nomesCidades[i], nomeCidade))
             {
                 return i;
             }
         }
         throw new Exception("Cidade não encontrada no grafo.");
     }

     public string ObterNomePorIndice(int indice)
     {
         if (indice >= 0 && indice < nomesCidades.Count)
         {
             return nomesCidades[indice];
         }
         else
         {
             throw new Exception("Índice fora do intervalo válido.");
         }
     }

     public bool CompararStrings(string texto1, string texto2 )
     {
         if (texto1.Length != texto2.Length)
         {
             return false;
         }

         for (int i = 0; i < texto1.Length; i++)
         {
             if (texto1[i] != texto2[i])
             {
                 return false;
             }
         }

         return true;
     }
        */


    }
}
