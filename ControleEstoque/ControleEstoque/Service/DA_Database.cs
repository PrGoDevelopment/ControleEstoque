﻿using PCLExt.FileStorage;
using PCLExt.FileStorage.Folders;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Service
{
    public class DA_Database
    {
        // DEFINE UMA CONEXAO E O  NOME DO BANCO DE DADOS
        public static SQLiteAsyncConnection sqliteconnection;
        public static string DbFileName = "DB_ESTOQUE.db3";

        public DA_Database(string dbPath)
        {
            //CRIA UMA PASTA BASE LOCAL PARA O DISPOSITIVO
            var pasta = new LocalRootFolder();
            //cria o arquivo
            var arquivo = pasta.CreateFile(DbFileName, CreationCollisionOption.OpenIfExists);
            //ABRE O BD
            sqliteconnection = new SQLiteAsyncConnection(arquivo.Path);
        }

        public Task<List<Model.T_ESTOQUE>> getEstoque()
        {
            return sqliteconnection.Table<Model.T_ESTOQUE>().ToListAsync();
        }

        public async Task<bool> insertEstoque(Model.T_ESTOQUE estoque)
        {
            try
            {
                await sqliteconnection.QueryAsync<Model.T_ESTOQUE>("INSERT OR IGNORE INTO T_ESTOQUE (Nome, Quantidade) VALUES('" + estoque.Nome + "', '" + estoque.Quantidade + "')");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> updateEstoque(Model.T_ESTOQUE estoque)
        {
            try
            {
                await sqliteconnection.QueryAsync<Model.T_ESTOQUE>("UPDATE T_ESTOQUE SET Quantidade = '" + estoque.Quantidade + "' WHERE Id = '" + estoque.Id.ToString() + "'");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
