using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoque.Model
{
    public class T_ESTOQUE
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
    }
}
