using Android.Widget;
using ControleEstoque.Controllers;
using ControleEstoque.Model;
using ControleEstoque.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControleEstoque.ViewModel
{
    public class PaginaInicialVM : BaseViewModel
    {
        readonly EstoqueController estoqueController = new EstoqueController();

        private ObservableCollection<ESTOQUE> listEstoque;
        public ObservableCollection<ESTOQUE> ListEstoque
        {
            get { return listEstoque; }
            set { listEstoque = value; RaisePropetyChanged(); }
        }

        private int idProduto;
        public int IdProduto
        {
            get { return idProduto; }
            set { idProduto = value; RaisePropetyChanged(); }
        }

        private string nomeProduto = "";
        public string NomeProduto
        {
            get { return nomeProduto; }
            set { nomeProduto = value; RaisePropetyChanged(); }
        }

        private string quantidadeProduto;
        public string QuantidadeProduto
        {
            get { return quantidadeProduto; }
            set { quantidadeProduto = value; RaisePropetyChanged(); }
        }

        // INFORMAÇÕES NÃO REFERENTES AO BANCO DE DADOS
        private static bool canInsert = false;
        public static bool CanInsert
        {
            get { return canInsert; }
            set { canInsert = value; }
        }

        public ICommand InsertEstoque_IdAuto { get; set; }
        public ICommand UpdateQtdEstoque { get; set; }

        // CONSTRUTOR
        public PaginaInicialVM()
        {
            // COMANDO INSERT
            InsertEstoque_IdAuto = new Command(execute: async () =>
            {
                // VALIDAÇÃO DE INSERTE, SOMENTE SE O CAMPO NOME ESTIVER PREENCHIDO
                if (!NomeProduto.Equals("")) { CanInsert = true; }
                else { CanInsert = false; }

                List<ESTOQUE> lst_Estoque = await estoqueController.GetListEstoque();
                var nomeEncontrado = lst_Estoque.Where(x => (x.Nome == nomeProduto)).FirstOrDefault();

                if (nomeEncontrado != null)
                {
                    CanInsert = false;
                    Toast.MakeText(Android.App.Application.Context, "O produto já exite, por favor tente outro nome!", ToastLength.Long).Show();
                }

                if (canInsert) // BLOQUEIA O INSERT CAJO HAJA ALGO ERRADO
                {
                    if (await estoqueController.InsertEstoque(new ESTOQUE() { Nome = NomeProduto, Quantidade = int.Parse(QuantidadeProduto) }))
                    {
                        Toast.MakeText(Android.App.Application.Context, "O produto " + NomeProduto + " foi adicionado com sucesso ao banco de dados local.", ToastLength.Long).Show();

                        // ENVIANDO DADOS AO API
                        int statusCode = await SyncAPI_DB.PostProduto_API();
                        if (statusCode >= 200 && statusCode <= 207)
                            Toast.MakeText(Android.App.Application.Context, "Sucesso ao enviar dados para o servidor.\nResposta: " + statusCode, ToastLength.Long).Show();
                        else
                            Toast.MakeText(Android.App.Application.Context, "Erro ao enviar dados para o servidor, certifique se de que o servidor está online.\nClique no botão SINCRONIZAR quando o servidor estiver online.\nResposta: " + statusCode, ToastLength.Long).Show();
                    }
                    else
                        Toast.MakeText(Android.App.Application.Context, "Erro ao tentar salvar o produto no banco de dados local!", ToastLength.Long).Show();
                    
                    // LIMPA OS CAMPOS VIA BINDING
                    NomeProduto = "";
                    QuantidadeProduto = "";

                    LoadListEstoque();
                }
            });

            // COMANDO UPDATE
            UpdateQtdEstoque = new Command<ESTOQUE>(execute: async (estoque) =>
            {
                if (!await estoqueController.UpdateEstoque(estoque))
                    Toast.MakeText(Android.App.Application.Context, "Estoque atualizado com sucesso no banco de dados local.", ToastLength.Long).Show();
                else
                    Toast.MakeText(Android.App.Application.Context, "Erro ao tentar atualizar o estoque no banco de dados local!", ToastLength.Long).Show();

                LoadListEstoque();
            });

            LoadListEstoque();
        }

        // CARREHA A LISTA DE ITENS
        public async void LoadListEstoque()
        {
            ListEstoque = new ObservableCollection<ESTOQUE>(await estoqueController.GetListEstoque());
        }
    }
}
