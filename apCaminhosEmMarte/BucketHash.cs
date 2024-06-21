//Aldo Junio - 23121
//Leonardo Ribeiro - 23579

using apCaminhosEmMarte;
using System;
using System.Collections;
using System.Collections.Generic;

class BucketHash<Tipo> : ITabelaDeHash<Tipo>
            where Tipo : IRegistro<Tipo>,
                         IComparable<Tipo>
{
    private const int SIZE = 131;  // para gerar mais colisões; o ideal é primo > 100
    public ArrayList[] dados;

    public BucketHash()
    {
        dados = new ArrayList[SIZE];
        for (int i = 0; i < SIZE; i++)
            dados[i] = new ArrayList(1);
    }

    public int Hash(string chave)
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
        if (!dados[valorDeHash].Contains(item)) // Contains procura o item e retorna True ou False
            dados[valorDeHash].Add(item);
    }

    public bool Remover(Tipo item)
    {
        int onde = 0;
        if (!Existe(item, out onde))
            return false;
        dados[onde].Remove(item);
        return true;
    }

    public bool Existe(Tipo item, out int onde)
    {
        onde = Hash(item.Chave);
        return dados[onde].Contains(item);
    }

    public List<string> Conteudo()
    {
        List<string> saida = new List<string>();
        for (int i = 0; i < dados.Length; i++)
            if (dados[i].Count > 0)
            {
                string linha = $"{i,5} : ";
                foreach (Tipo item in dados[i])
                    linha += " | " + item.Chave;
                saida.Add(linha);
            }
        return saida;
    }

    public Tipo GetElementoIndice(int valorDeHash)
    {
        ArrayList lista = dados[valorDeHash];
        if (lista.Count > 0)
        {
            object elementoObjeto = lista[0];
            Tipo elemento = (Tipo)elementoObjeto;
            return elemento;
        }
        else return default(Tipo);
    }



    public ArrayList GetElementosUso()
    {
        ArrayList lista = new ArrayList();

        for (int i = 0; i < dados.Length; i++)
        {
            if (dados[i] != null)
            {
                lista.AddRange(dados[i]);
            }
        }
        return lista;
    }
}
