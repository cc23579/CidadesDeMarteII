//Aldo Junio - 23121
//Leonardo Ribeiro - 23579

using apCaminhosEmMarte;
using System;
using System.Collections;
using System.Collections.Generic;

class HashLinear<Tipo> : ITabelaDeHash<Tipo>
    where Tipo : IRegistro<Tipo>,
                 IComparable<Tipo>
{
    private const int SIZE = 131;  // para gerar mais colisões; o ideal é primo > 100
    public Tipo[] dados;

    public HashLinear()
    {
        dados = new Tipo[SIZE];
    }

    public int Hash(string chave)  //Aplicação do hash Linear
    {
        long tot = 0;
        for (int i = 0; i < chave.Length; i++)
            tot += 37 * tot + (char)chave[i];

        tot = tot % dados.Length;
        if (tot < 0)
            tot += dados.Length;

        return (int)tot;
    }

    public void Inserir(Tipo item)
    {
        int valorDeHash = Hash(item.Chave);
        int indice = valorDeHash;
        while (dados[indice] != null)
        {
            indice = (indice + 1) % SIZE;
            if (indice == valorDeHash) // Verifica se voltou ao início
                throw new Exception("Tabela de Hash cheia");
        }
        dados[indice] = item;
    }

    public bool Remover(Tipo item)
    {
        int onde = 0;
        if (!Existe(item, out onde))
        {
            return false;
        }
        dados[onde] = default(Tipo);
        return true;
    }

    public bool Existe(Tipo item, out int onde)
    {
        onde = Hash(item.Chave);
        return dados[onde].Chave == item.Chave;
    }

    public List<string> Conteudo()
    {
        List<string> saida = new List<string>();
        for (int i = 0; i < dados.Length; i++)
        {
            if (dados[i] != null)
                saida.Add($"{i,5} : | {dados[i].Chave}");
        }
        return saida;
    }

    public Tipo GetElementoIndice(int valorDeHash)
    {
        return dados[valorDeHash];
    }

    public ArrayList GetElementosUso()
    {
        ArrayList lista = new ArrayList();

        for (int i = 0; i < dados.Length; i++)
        {
            if (dados[i] != null)
            {
                lista.Add(dados[i]);
            }
        }
        return lista;
    }
}
