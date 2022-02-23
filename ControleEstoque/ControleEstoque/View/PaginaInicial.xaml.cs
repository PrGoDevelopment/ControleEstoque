using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControleEstoque.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaInicial : ContentPage
    {
        private Model.T_ESTOQUE estoque = new Model.T_ESTOQUE();
        private List<Model.T_ESTOQUE> lst_Estoque = new List<Model.T_ESTOQUE>();

        public PaginaInicial()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            lst_Estoque = await App.Database.getEstoque();
            ListaProdutos.ItemsSource = lst_Estoque;

            if (lst_Estoque.Count() == 0)
                carregarProdutosTeste();
        }

        public void carregarProdutosTeste()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    estoque.Nome = "Produto " + (i + 1).ToString();
                    estoque.Quantidade = 1;

                    await App.Database.insertEstoque(estoque);
                }
            });
        }

        private async void ListaProdutos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (ListaProdutos.SelectedItem != null)
            {
                var detailPage = new PaginaAlterarEstoque()
                {
                    BindingContext = e.SelectedItem as Model.T_ESTOQUE
                };
                ListaProdutos.SelectedItem = null;
                await Navigation.PushModalAsync(detailPage);
            }
        }

        private void NovoProduto(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PaginaNovoProduto());
        }
    }
}