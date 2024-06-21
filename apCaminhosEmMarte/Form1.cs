using apCidadesBacktracking;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
        Cidade[] asCidades;
        int quantasCidades;   // tamanho lógico
        private string cidadeSendoBuscada = "";
        GrafoBacktracking grafo;

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
                while (!arquivo.EndOfStream)
                {
                    Cidade umaCidade = new Cidade();
                    umaCidade.LerRegistro(arquivo);
                    tabela.Inserir(umaCidade);
                }

                lsbCidades.Items.Clear();  // limpa o listBox
                var asCidades = tabela.GetElementosUso();

                foreach (Cidade cid in asCidades)
                {
                    lsbCidades.Items.Add(cid);
                }
                pbMapa.Invalidate();
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
            // Verifica se o usuário deseja salvar o arquivo
            if (dlgSalvar.ShowDialog() == DialogResult.OK)
            {
                string nomeDoArquivoCidades = dlgSalvar.FileName;
                using (StreamWriter arquivoSaida = new StreamWriter(nomeDoArquivoCidades))
                {
                    ArrayList elementosUso = tabela.GetElementosUso();

                    foreach (Cidade cidade in elementosUso)
                    {
                        cidade.GravarDados(arquivoSaida);
                    }
                }

                if (dlgSalvar.ShowDialog() == DialogResult.OK)
                {
                    string nomeDoArquivoCaminhos = dlgSalvar.FileName;
                    using (StreamWriter arquivoSaida = new StreamWriter(nomeDoArquivoCidades))
                    {
                        grafo.SalvarGrafoEmArquivo(nomeDoArquivoCaminhos);
                    }
                }
            }
        }

        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            if (tabela != null)
            {
                ArrayList asCidadesArrayList = tabela.GetElementosUso();  //Cria uma ArrayList com os elementos em uso (Conteudo(), mas retorna dados de Tipo, ao invés de string)
                if (asCidadesArrayList != null)
                {
                    Cidade[] asCidades = (Cidade[])asCidadesArrayList.ToArray(typeof(Cidade)); //conversão explicita

                    //declara como será pintado no mapa
                    var ondeDesenhar = e.Graphics;
                    SolidBrush brush = new SolidBrush(Color.Black);
                    Pen pen = new Pen(Color.Black, 2);
                    var fonte = new Font("Segoe UI", 8);

                    var buscaFonte = new Font("Segoe UI", 10, FontStyle.Bold);
                    SolidBrush buscaBrush = new SolidBrush(Color.Red); // Cor para a cidade buscada


                    foreach (Cidade cidade in asCidades)
                    {
                        // Calcular as coordenadas x e y com base na porcentagem do PictureBox
                        int x = (int)(cidade.X * pbMapa.Width);
                        int y = (int)(cidade.Y * pbMapa.Height);

                        if (cidade.Chave == cidadeSendoBuscada) //se a cidade esta sendo buscada, será evidenciada no mapa com circulo vermelho
                        {
                            ondeDesenhar.DrawString(cidade.NomeCidade.Trim(), buscaFonte, brush, x - 10, y - 20);
                            ondeDesenhar.FillEllipse(buscaBrush, x - 5, y - 5, 8, 8);
                            ondeDesenhar.DrawEllipse(pen, x - 5, y - 5, 8, 8);

                        }

                        else // Preencher o círculo normalmente
                        {
                            ondeDesenhar.FillEllipse(brush, x - 5, y - 5, 8, 8);
                            ondeDesenhar.DrawEllipse(pen, x - 5, y - 5, 8, 8);
                            ondeDesenhar.DrawString(cidade.NomeCidade.Trim(), fonte, brush, x - 10, y - 20);
                        }
                    }
                }
            }
        }

       

        private void FrmCaminhos_Load(object sender, EventArgs e) //Aqui esta dando arquivo não encontrado
        {
            string nomeDoArquivo = "C:\\Users\\aldin\\OneDrive\\Documentos\\GitHub\\CidadesDeMarteII\\apCaminhosEmMarte\\CaminhoEntreCidades.txt";
            grafo = new GrafoBacktracking(nomeDoArquivo);
        }



        private void tpCaminhos_Enter(object sender, EventArgs e)
        {

            asCidades = new Cidade[25];
            quantasCidades = 0;


            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                string nomeDoArq = dlgAbrir.FileName;
                var arquivo = new StreamReader(nomeDoArq);

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
            }
            // Adicionar nome das cidades nos ComboBoxes referenciando o objeto
            for (int i = 0; i < quantasCidades; i++)
            {
                if (asCidades[i] != null)
                {
                    cbxOrigem.Items.Add(asCidades[i].NomeCidade);
                    cbxDestino.Items.Add(asCidades[i].NomeCidade);
                }
            }
            grafo.Exibir(dgvCaminhos);
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
            for (int lento = 0; lento < quantasCidades; lento++)
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
            string nomeCidadeOrigem = cbxOrigem.SelectedItem.ToString();
            string nomeCidadeDestino = cbxDestino.SelectedItem.ToString();
            Cidade cidadeOrigem = null;
            Cidade cidadeDestino = null;

            foreach (Cidade cidade in asCidades)
            {
                if (cidade.NomeCidade == nomeCidadeOrigem)
                {
                    cidadeOrigem = cidade;
                    break;
                }
            }

            foreach (Cidade cidade in asCidades)
            {
                if (cidade.NomeCidade == nomeCidadeDestino)
                {
                    cidadeDestino = cidade;
                    break;
                }
            }

            // Obter a distância informada no controle
            int distancia = Convert.ToInt32(udDistancia.Value);
            if (cidadeOrigem != null && cidadeDestino != null)
            {
                // Adicionar o caminho no grafo
                grafo.AdicionarCaminho(cidadeOrigem, cidadeDestino, distancia);
                MessageBox.Show("Incluido");
            }
            else
            {
                MessageBox.Show("Erro ao obter os objetos Cidade.");
            }
        }


        private void btnAlterarCaminho_Click(object sender, EventArgs e)
        {
            string cidadeOrigem = cbxOrigem.SelectedItem.ToString().Trim();
            string cidadeDestino = cbxDestino.SelectedItem.ToString().Trim();
            int novaDistancia = (int)udDistancia.Value;

            if (cidadeOrigem != null && cidadeDestino != null)
            {
                grafo.AlterarDistancia(cidadeOrigem, cidadeDestino, novaDistancia);
                MessageBox.Show("Caminho alterado com sucesso!");
            }
            else
            {
                MessageBox.Show("Selecione uma origem e um destino para alterar a distância.");
            }
        }

        private void btnExcluirCaminho_Click(object sender, EventArgs e)
        {
            string cidadeOrigem = cbxOrigem.SelectedItem.ToString().Trim();
            string cidadeDestino = cbxDestino.SelectedItem.ToString().Trim();

            if (cidadeOrigem != null && cidadeDestino != null)
            {
                grafo.ExcluirCaminho(cidadeOrigem, cidadeDestino);
                MessageBox.Show("Caminho excluido com sucesso!");
            }
            else
            {
                MessageBox.Show("Selecione uma origem e um destino para excluir o caminho.");
            }
        }

        private void btnBuscarCaminhos_Click(object sender, EventArgs e)
        {
            var origem = cbxOrigem.SelectedIndex;
            var destino = cbxDestino.SelectedIndex;


            grafo.BuscarCaminho(origem, destino, lsbCaminhos, dgvCaminhos);

        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            double valorX = Convert.ToDouble(udX.Value);
            double valorY = Convert.ToDouble(udY.Value);
            Cidade cidadeInserida = new Cidade(txtCidade.Text, valorX, valorY);
            tabela.Inserir(cidadeInserida);

            lsbCidades.Items.Add(cidadeInserida.Chave);
            pbMapa.Invalidate();

           
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (lsbCidades.SelectedIndex != -1)
            {
                int indice = tabela.Hash(lsbCidades.SelectedItem.ToString());
                Cidade cidade = tabela.GetElementoIndice(indice);
                tabela.Remover(cidade);
                lsbCidades.Items.RemoveAt(lsbCidades.SelectedIndex);
                pbMapa.Invalidate();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string chave = txtCidade.Text;

            int indice = tabela.Hash(chave);
            Cidade cidade = tabela.GetElementoIndice(indice);
            if (cidade != null)
            {
                double valorX = cidade.X;
                udX.Value = Convert.ToDecimal(valorX);

                double valorY = cidade.Y;
                udY.Value = Convert.ToDecimal(valorY);

                txtCidade.Text = chave;

                
                cidadeSendoBuscada = chave;
                pbMapa.Invalidate();
            }
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            lsbCaminhos.Items.Clear();

            if (tabela != null)
            {
                ArrayList asCidadesArrayList = tabela.GetElementosUso();

                foreach (Cidade cidade in asCidadesArrayList)
                {
                    lsbCaminhos.Items.Add(cidade.Chave);
                }
            }
        }

        
    }
}
