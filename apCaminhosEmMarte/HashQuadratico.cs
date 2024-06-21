//Aldo Junio - 23121
//Leonardo Ribeiro - 23579

using System;
using System.Collections;
using System.Collections.Generic;


class HashQuadratico<Tipo> : ITabelaDeHash<Tipo>
            where Tipo : IRegistro<Tipo>,
                         IComparable<Tipo>
{

    private const int SIZE = 131;   //declara o tamanho do vetor
    Tipo[] dados;

    public HashQuadratico()
    {
        dados = new Tipo[SIZE];     //instancia o vetor, que armazena o objeto genérico "Tipo"
    }

    public int HashSimples(string chave)
    {
        int hash = 0;
        foreach (char c in chave) // Soma os números da tabela ASCII da chave
        {
            hash += c;
        }
        int index = hash % SIZE;
        return index;
    }



    public int Hash(string chave)
    {
        int index = HashSimples(chave);
        int colisoes = 1;
        while (dados[index] != null && !dados[index].Chave.Equals(chave))  //Verifica colisões, e se não existe um dado igual
                                                                           //na tabela
        {
            index = (index + colisoes * colisoes) % SIZE;                   //caso tenha colisão, soma o hash anterior
                                                                            //com o dobro do numero de colisões
            colisoes++;
        }
        return index;
    }



    public void Inserir(Tipo item)     //insere um objeto na tabela que é passado como parâmetro
    {
        int valorDeHash = Hash(item.Chave); //busca o indice onde será inserido o objeto
        int indice = valorDeHash;
        while (dados[indice] != null)
        {
            indice = (indice + 1) % SIZE;
            if (indice == valorDeHash) // Verifica se voltou ao início
                throw new Exception("Tabela cheia");
        }
        dados[indice] = item;
    }

    public bool Remover(Tipo item)      //Remove o objeto passado como parâmetro
    {
        int onde = 0;
        if (!Existe(item, out onde))
        {
            return false;
        }
        dados[onde] = default(Tipo);
        return true;
    }

    public bool Existe(Tipo item, out int onde)     //metodo que verifica se existe o objeto, e passa onde ele esta como saída
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

    public Tipo GetElementoIndice(int valorDeHash)      //retorna o objeto com índice passado como parâmetro
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


