using ControleEstoque.Controllers;
using ControleEstoque.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleEstoque.Service
{
    public static class SyncAPI_DB
    {
        private static readonly EstoqueController estoqueController = new EstoqueController();

        private static List<ESTOQUE> lst_Estoque_DB = new List<ESTOQUE>();
        private static List<ESTOQUE> lst_Estoque_API = new List<ESTOQUE>();

        // VARIÁVEIS DE COMUNICAÇÃO
        private static bool busy = false;
        public static bool Busy
        {
            get { return busy; }
            set { busy = value; }
        }

        private static string msgSincronizacao;
        public static string MsgSincronizacao
        {
            get { return msgSincronizacao; }
            set { msgSincronizacao = value; }
        }

        private static bool refresh = false;
        public static bool Refresh
        {
            get { return refresh; }
            set { refresh = value; }
        }

        private static void FinalizarOperacao()
        {
            Refresh = true; // PERMITE ATUALIZAÇÃO LISTA NA TELA PRINCIPAL, QUANRO TRUE, SAI DO LAÇO E MATA A THEAD
            Busy = false; // LIBERA PARA UM NOVO PROCESSO DE SINCRONIZAÇÃO
        }

        // SINCRONIZAÇÃO DO BANCO DE DADOS LOCAL PARA O API
        public static async Task<bool> SyncProduto_DB_to_API()
        {
            Busy = true; // IMPEDE A INICIALIZAÇÃO DE 2 OU MAIS PROCESSOS DE SINCRONIZAÇÃO
            MsgSincronizacao = "Tentando conexão...";
            await Task.Delay(1000);
            
            List<ESTOQUE> lst_Estoque = new List<ESTOQUE>();
            lst_Estoque = await estoqueController.GetListEstoque();

            List<ESTOQUE> lst_Estoque_API = new List<ESTOQUE>();
            lst_Estoque_API = await API.ApiEstoque.GetEstoque("http://" + App.EnderecoIP + ":5000/api/Estoque");

            MsgSincronizacao = "Enviando dados para o API";

            List<ESTOQUE> lst_Estoque_UPDATE = new List<ESTOQUE>();

            try
            {
                if (lst_Estoque.Count() > 0)
                {
                    int newId = lst_Estoque_API.Count() > 0 ? lst_Estoque_API[lst_Estoque_API.Count() - 1].Id : 0; // MAIOR ID DA LISTA

                    foreach (var item_API in lst_Estoque_API)
                    {
                        // ELIMINANDO CONFLITO DE IDs
                        var aux = lst_Estoque.Where(x => (x.Id == item_API.Id)).FirstOrDefault();

                        if (aux != null)
                        {
                            var aux2 = lst_Estoque_API.Where(x => (x.Nome == aux.Nome)).FirstOrDefault();

                            if (aux2 == null)
                            {
                                // SE EXISTE PRODUTO DIFERENTE COM UM MESMO ID NO SERVIDOR, O ITEM DO BANCO LOCAL RECEBE UM NOVO ID E É ENVIADO
                                lst_Estoque.Remove(aux);
                                newId++;
                                aux.Id = newId; // NOVO ID
                                lst_Estoque.Add(aux); // NOVOS ITENS A SER ENVIADOS
                            }
                            else
                            {
                                // ITEM REMOVIDO DA LISTA lst_Estoque QUE NO CONTEXTO REPRESENTA NOVOS ITENS
                                // CASO A QUANTIDADE SEJA DIFERENTE, O ITEM É ADICIONADO A LISTA E ENVIADO AO API PARA UPDATE DE ESTOQUÉ
                                lst_Estoque.Remove(aux);
                                if (item_API.Quantidade != aux.Quantidade)
                                {
                                    lst_Estoque_UPDATE.Add(aux); // ITENS QUE JÁ EXISTEM MAS O ESTOQUE SERÁ ATUALIZADO
                                }
                            }
                        }
                    }

                    if (lst_Estoque.Count() > 0) // ENVIA SE HÁ NOVOS ITENS
                        await API.ApiEstoque.PostListaProdutos("http://" + App.EnderecoIP + ":5000/api/Estoque/listaEstoquePost", lst_Estoque);

                    if (lst_Estoque_UPDATE.Count > 0) // ENVIA SE HÁ ESTOQUE A SER ATUALIZADO
                        await API.ApiEstoque.PutListaProdutos("http://" + App.EnderecoIP + ":5000/api/Estoque/listaEstoquePut", lst_Estoque_UPDATE);

                    if (lst_Estoque.Count() == 0 && lst_Estoque_UPDATE.Count == 0)
                        MsgSincronizacao = "Nada para fazer!";
                    else
                        MsgSincronizacao = "Dados enviados!";
                    await Task.Delay(2000);

                    // SINCRONIZAÇÃO SE INVERTE
                    await SyncInformacao_API_to_DB();
                }
                else
                {
                    MsgSincronizacao = "Nada para fazer!";
                    // SINCRONIZAÇÃO SE INVERTE (SE NÃO CONSEGUI ENVIAR DO DB LOCAL PARA O API, TENTRA TRAZER DO API PARA O DB LOCAL)
                    if (lst_Estoque_API != null)
                        await SyncInformacao_API_to_DB();
                    else
                        FinalizarOperacao();
                }
                return true;
            }
            catch (System.Exception)
            {
                MsgSincronizacao = "Erro de sincronização ou servidor offline, tentando buscar as informações do API!";
                await Task.Delay(2000);

                // SINCRONIZAÇÃO SE INVERTE (SE NÃO CONSEGUI ENVIAR DO DB LOCAL PARA O API, TENTRA TRAZER DO API PARA O DB LOCAL)
                if (lst_Estoque_API != null)
                    await SyncInformacao_API_to_DB();
                else
                    FinalizarOperacao();
                return false;
            }
        }

        // SINCRONIZAÇÃO INVERSA, DO API PARA O BANCO DE DADOS LOCAL
        public static async Task<bool> SyncInformacao_API_to_DB()
        {
            bool produtoExiste = false;

            ESTOQUE estoque = new ESTOQUE();

            lst_Estoque_DB = await estoqueController.GetListEstoque();
            lst_Estoque_API = await API.ApiEstoque.GetEstoque("http://" + App.EnderecoIP + ":5000/api/Estoque");

            MsgSincronizacao = "Buscando os dados do API";

            try
            {
                foreach (var item_API in lst_Estoque_API)
                {
                    for (int i = 0; i < lst_Estoque_DB.Count(); i++)
                    {
                        if (lst_Estoque_DB[i].Id == item_API.Id)
                        {
                            produtoExiste = true;
                            if ((!lst_Estoque_DB[i].Nome.Equals(item_API.Nome)) || lst_Estoque_DB[i].Quantidade != item_API.Quantidade)
                                // PARA PRIORIZAR IS ITENS NO API, TODOS OS ITENS DE IDs IGUAIS SÃO ATUALIZADOS NA BASE DE DADOS LOCAL NÃO IMPORTANDO O NOME OU QUANTIDADE EM ESTOQUE
                                await estoqueController.UpdateEstoque(item_API);
                            break;
                        }
                    }

                    // VERIFICANDO POR ID, PRODUTO NÃO EXISTE NO BANCO LOCAL, UM NOVO É INSERIDO COM O ID VINDO DO API
                    if (!produtoExiste)
                    {
                        estoque.Id = item_API.Id;
                        estoque.Nome = item_API.Nome;
                        estoque.Quantidade = item_API.Quantidade;

                        await estoqueController.InsertEstoque(new ESTOQUE() { Id = item_API.Id, Nome = item_API.Nome, Quantidade = item_API.Quantidade });
                    }

                    produtoExiste = false;
                }

                await Task.Delay(1000);
                MsgSincronizacao = "Concluído!";
                FinalizarOperacao();
                return true;
            }
            catch (System.Exception)
            {
                MsgSincronizacao = "Erro de sincronização!";
                FinalizarOperacao();
                return false;
            }
        }

        // SINCRONIZANDO ÚLTIMO PRODUTO REGISTRADO NO BANCO DE DADOS COM O API.
        public static async Task<int> PostProduto_API()
        {
            ESTOQUE ultimoProdutoInserido = await estoqueController.GetUltumoProdutoGravado();
            int statusCode = await API.ApiEstoque.PostProduto("http://" + App.EnderecoIP + ":5000/api/Estoque", ultimoProdutoInserido);
            return statusCode;
        }

        // SINCRONIZANDO A ÚLTIMA QUANTIDADE EM ESTOQUE COM O API
        public static async Task<int> PutProduto_API(ESTOQUE estoque)
        {
            int statusCode = await API.ApiEstoque.PutProduto("http://" + App.EnderecoIP + ":5000/api/Estoque?id=" + estoque.Id, estoque);
            return statusCode;
        } 
    }
}
