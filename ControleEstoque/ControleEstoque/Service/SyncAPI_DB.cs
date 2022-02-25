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

        public static async Task<bool> syncInformacao_API_to_DB()
        {
            bool produtoExiste = false;

            estoque = new T_ESTOQUE();

            lst_Estoque_DB = await App.Database.getEstoque();
            lst_Estoque_API = await API.ApiEstoque.getEstoque("http://" + App.EnderecoIP + ":5000/api/Estoque");

            if (lst_Estoque_DB.Count() == 0)
            {
                await carregarBanco(lst_Estoque_API);
                lst_Estoque_DB = await App.Database.getEstoque();
            }

            // SINCRONIZAÇÃO
            foreach (var item_API in lst_Estoque_API)
            {
                for (int i = 0; i < lst_Estoque_DB.Count(); i++)
                {
                    if (lst_Estoque_DB[i].Id == item_API.Id)
                    {
                        produtoExiste = true;
                        if((!lst_Estoque_DB[i].Nome.Equals(item_API.Nome)) || lst_Estoque_DB[i].Quantidade != item_API.Quantidade)
                            await App.Database.updateEstoque(item_API);
                        break;
                    }
                }

                if (!produtoExiste)
                {
                    estoque.Id = item_API.Id;
                    estoque.Nome = item_API.Nome;
                    estoque.Quantidade = item_API.Quantidade;

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

        public static async Task<bool> syncProduto_DB_to_API()
        {
            List<T_ESTOQUE> lst_Estoque = new List<T_ESTOQUE>();
            lst_Estoque = await App.Database.getEstoque();

            List<T_ESTOQUE> lst_Estoque_API = new List<T_ESTOQUE>();
            lst_Estoque_API = await API.ApiEstoque.getEstoque("http://" + App.EnderecoIP + ":5000/api/Estoque");

            foreach (var item_API in lst_Estoque_API)
            {
                var aux = lst_Estoque.Where(x => (x.Id == item_API.Id)).FirstOrDefault();

                if (aux != null)
                    lst_Estoque.Remove(aux);
                else
                    break;
            }

            await API.ApiEstoque.postListaProdutos("http://" + App.EnderecoIP + ":5000/api/Estoque/listaProdutos", lst_Estoque);
            return true;
        }
    }
}
