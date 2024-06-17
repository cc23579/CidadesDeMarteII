using apCidadesBacktracking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apCaminhosEmMarte
{
  public partial class FrmCaminhos : Form
  {
    public FrmCaminhos()
    {
      InitializeComponent();
    }

    ITabelaDeHash<Cidade> tabela;

    private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
    {

    }

    private void btnLerArquivo_Click(object sender, EventArgs e)
    {
      if (dlgAbrir.ShowDialog() == DialogResult.OK)
      {
        if (rbBucketHash.Checked)
           tabela = new BucketHash<Cidade>();
        else
          if (rbHashLinear.Checked)
             tabela = new HashLinear<Cidade>();
          else 
            if (rbHashQuadratico.Checked)
               tabela = new HashQuadratico<Cidade>();
            else
              if (rbHashDuplo.Checked)
                tabela = new HashDuplo<Cidade>();

        var arquivo = new StreamReader(dlgAbrir.FileName);
        while (! arquivo.EndOfStream) 
        {
          Cidade umaCidade = new Cidade();
          umaCidade.LerRegistro(arquivo);
          tabela.Inserir(umaCidade);
        }
        lsbCidades.Items.Clear();  // limpa o listBox
        var asCidades = tabela.Conteudo();
        foreach (Cidade cid in asCidades)
          lsbCidades.Items.Add(cid);
        arquivo.Close();
      }
    }

    private void FrmCaminhos_FormClosing(object sender, FormClosingEventArgs e)
    {
      // abrir o arquivo para saida, se houver um arquivo selecionado
      // obter todo o conteúdo da tabela de hash
      // percorrer o conteúdo da tabela de hash, acessando
      // cada cidade individualmente e usar esse objeto Cidade
      // para gravar seus próprios dados no arquivo
      // fechar o arquivo ao final do percurso
    }

        //variável que representa o grafo
        GrafoBacktracking grafo;

    private void FrmCaminhos_Load(object sender, EventArgs e)
    {
        //Instancia a classe GrafoBacktracking com o arquivo de caminhos de Marte
        String nomeArquivo = "CaminhosMarte.txt";
        grafo = new GrafoBacktracking(nomeArquivo);

        }

    Cidade[] asCidades;
    int quantasCidades;   // tamanho lógico

        private void tpCaminhos_Enter(object sender, EventArgs e)
        {
            asCidades = new Cidade[25];
            quantasCidades = 0;

            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                var arquivo = new StreamReader(dlgAbrir.FileName);

                // Ler o arquivo de cidades
                while (!arquivo.EndOfStream)
                {
                    // Instanciar um objeto da classe Cidade
                    Cidade cidade = new Cidade();

                    // Ler o registro da cidade
                    cidade.LerRegistro(arquivo);

                    // Adicionar o objeto cidade ao vetor de cidades
                    asCidades[quantasCidades] = cidade;
                    quantasCidades++;
                }

                arquivo.Close(); // Fechar o arquivo após a leitura

                OrdenarCidades();

                // Limpar os ComboBoxes antes de adicionar os novos itens
                cbxOrigem.Items.Clear();
                cbxDestino.Items.Clear();

                // Adicionar apenas o nome das cidades nos ComboBoxes
                foreach (Cidade cidade in asCidades)
                {
                    cbxOrigem.Items.Add(cidade);
                    cbxDestino.Items.Add(cidade);
                }
            }


        }


        private void OrdenarCidades()
    {
      //asCidades[0] = new Cidade("Campinas", 0, 0);
      //asCidades[1] = new Cidade("Americana", 0, 0); 
      //asCidades[2] = new Cidade("Sumaré", 0, 0);
      //asCidades[3] = new Cidade("Estiva Gerbi", 0, 0);
      //asCidades[4] = new Cidade("Rafard", 0, 0); 
      //asCidades[5] = new Cidade("Rifaina", 0, 0);
      //asCidades[6] = new Cidade("Hortolândia", 0, 0);
      //quantasCidades = 7;

      // Ordenação por seleção direta ou
      // Selection Sort
      for (int lento= 0; lento < quantasCidades; lento++)
      {
        int indiceMenorCidade = lento;
        for (int rapido = lento + 1; rapido < quantasCidades; rapido++)
          if (asCidades[rapido].NomeCidade.CompareTo(
                asCidades[indiceMenorCidade].NomeCidade) < 0)
            indiceMenorCidade = rapido;

          if (indiceMenorCidade != lento)
          {
            Cidade auxiliar = asCidades[indiceMenorCidade];
            asCidades[indiceMenorCidade] = asCidades[lento];
            asCidades[lento] = auxiliar;
          }
      }
    }

        private void btnIncluirCaminho_Click(object sender, EventArgs e)
        {
            Cidade cidadeOrigem = (Cidade)cbxOrigem.SelectedItem;
            Cidade cidadeDestino = (Cidade)cbxDestino.SelectedItem;

            int distancia = Convert.ToInt32(udDistancia.Value);

            grafo.AdicionarCaminho(cidadeOrigem, cidadeDestino, distancia);
           
        }

        private void btnAlterarCaminho_Click(object sender, EventArgs e)
        {
            string cidadeOrigem = cbxOrigem.SelectedItem?.ToString();
            string cidadeDestino = cbxDestino.SelectedItem?.ToString();
            int novaDistancia = (int)udDistancia.Value;

            if (cidadeOrigem != null && cidadeDestino != null)
            {
                grafo.AlterarDistancia(cidadeOrigem, cidadeDestino, novaDistancia);
                // Aqui você pode adicionar código adicional após alterar a distância, se necessário
            }
            else
            {
                MessageBox.Show("Selecione uma origem e um destino para alterar a distância.");
            }
        }

        private void btnExcluirCaminho_Click(object sender, EventArgs e)
        {
            string cidadeOrigem = cbxOrigem.SelectedItem?.ToString();
            string cidadeDestino = cbxDestino.SelectedItem?.ToString();

            if (cidadeOrigem != null && cidadeDestino != null)
            {
                grafo.ExcluirCaminho(cidadeOrigem, cidadeDestino);
                // Aqui você pode adicionar código adicional após excluir o caminho, se necessário
            }
            else
            {
                MessageBox.Show("Selecione uma origem e um destino para excluir o caminho.");
            }
        }

       
    }
}
