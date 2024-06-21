//Aldo Junio - 23121
//Leonardo Ribeiro - 23579

using System;
using System.Collections;
using System.Collections.Generic;


public class HashDuplo<Tipo> : ITabelaDeHash<Tipo>
  where Tipo : IRegistro<Tipo>, IComparable<Tipo>
{

    public const int SIZE = 131;
    Tipo[] dados;



    public HashDuplo() //Cosntrutor, que cria a ArrayList que será usada
    {
        dados = new Tipo[SIZE];
    }

    public int HashSimples(string chave)
    {
        int hash = 0;
        foreach (char c in chave) //Soma os numeros da tabela asic da chave
        {
            hash += c;
        }
        int index = hash % SIZE;
        return index;
    }

    public static int EncontrarPrimoMenor(int n)
    {
        // Inicia a busca a partir do número n - 1 para encontrar o maior primo menor que n
        for (int i = n - 1; i >= 2; i--)
        {
            if (ÉPrimo(i)) // Verifica se o número é primo
            {
                return i; // Retorna o maior primo encontrado
            }
        }

        return 2; // Se nenhum primo for encontrado, retorna 2 (o menor primo possível)
    }

    public static bool ÉPrimo(int num)
    {
        if (num <= 1)
        {
            return false; // Números menores ou iguais a 1 não são primos
        }

        // Verifica se o número é divisível por algum número menor que ele 
        for (int i = 2; i * i <= num; i++)
        {
            if (num % i == 0)
            {
                return false; // Se for divisível, não é primo
            }
        }

        return true; // Se não for divisível por nenhum número menor que ele, é primo
    }

    public int Hash(string chave)
    {
        int index = HashSimples(chave);
        int R = EncontrarPrimoMenor(SIZE);
        int hash2 = R - (index % R);
        int offset = 1;
        while (dados[index] != null && !dados[index].Chave.Equals(chave))  //verifica colisões
        {
            index = (index + offset * hash2) % SIZE; // Aplica a sondagem dupla
            offset++;
        }
        return index;
    }

    public void Inserir(Tipo item)
    {
        int valorDeHash = Hash(item.Chave);
        int indice = valorDeHash;
        while (dados[indice] != null)
        {
            indice = (indice + 1) % SIZE;
            if (indice == valorDeHash)
                throw new Exception("Tabela cheia");
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
