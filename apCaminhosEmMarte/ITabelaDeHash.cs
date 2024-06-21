//Aldo Junio - 23121
//Leonardo Ribeiro - 23579

using System;
using System.Collections;
using System.Collections.Generic;

interface ITabelaDeHash<Tipo>
    where Tipo : IRegistro<Tipo>, IComparable<Tipo>
{
    int Hash(string chave);

    //Metodos criado para faciltar a implementação
    Tipo GetElementoIndice(int valorDeHash);   //método que retorna o Objeto, passando o índice como parâmetro
    ArrayList GetElementosUso();          //método quen retorna uma arrayList do tipo "Tipo"


    void Inserir(Tipo item);
    bool Remover(Tipo item);
    bool Existe(Tipo item, out int onde);
    List<string> Conteudo();


}
