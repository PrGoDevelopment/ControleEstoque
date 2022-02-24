using ControleEstoque.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Service
{
    public static class SyncAPI_DB
    {
        private static List<T_ESTOQUE> lst_Estoque_DB = new List<T_ESTOQUE>();
        private static List<T_ESTOQUE> lst_Estoque_API = new List<T_ESTOQUE>();
        private static Model.T_ESTOQUE estoque;

        public static async Task<bool> syncInformacao_OutIn()
        {
            bool produtoExiste = false;

            estoque = new T_ESTOQUE();

            lst_Estoque_DB = await App.Database.getEstoque();
            lst_Estoque_API = await API.ApiEstoque.getEstoque("http://192.168.15.20:5000/api/Estoque");

            if (lst_Estoque_DB.Count() == 0)
            {
                await carregarBanco(lst_Estoque_API);
                lst_Estoque_DB = await App.Database.getEstoque();
            }

            // SINCRONIZAÇÃO
            foreach (var item in lst_Estoque_API)
            {
                for (int i = 0; i < lst_Estoque_DB.Count(); i++)
                {
                    if (lst_Estoque_DB[i].Id == item.Id)
                    {
                        produtoExiste = true;
                        break;
                    }
                }

                if (!produtoExiste)
                {
                    estoque.Nome = item.Nome;
                    estoque.Quantidade = item.Quantidade;

                    await App.Database.insertEstoque(estoque);
                }

                produtoExiste = false;
            }

            return true;
        }

        private static async Task<bool> carregarBanco(List<T_ESTOQUE> lst_Estoque_API)
        {
            foreach (var item in lst_Estoque_API)
            {
                estoque.Nome = item.Nome;
                estoque.Quantidade = item.Quantidade;

                await App.Database.insertEstoque(estoque);
            }

            return true;
        }

        public static async Task<bool> syncProduto_InOut(T_ESTOQUE estoque)
        {
            await API.ApiEstoque.postProduto("http://192.168.15.20:5000/api/Estoque", estoque);
            return true;
        }
    }
}
