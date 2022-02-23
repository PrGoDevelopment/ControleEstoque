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
    public partial class PaginaAlterarEstoque : ContentPage
    {
        private Model.T_ESTOQUE estoque;

        public PaginaAlterarEstoque()
        {
            InitializeComponent();
        }

        private void AlterarEstoque(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                estoque = new Model.T_ESTOQUE();

                estoque.Id =  int.Parse(lbl_Id.Text);
                estoque.Nome = lbl_Nome.Text;
                estoque.Quantidade = int.Parse(entr_alterarEstoque.Text);

                if(await App.Database.updateEstoque(estoque))
                {
                    await DisplayAlert("Controle Estoque", "Alteração feita com sucesso!", "Ok");
                    await Navigation.PopModalAsync();
                }
                else
                    await DisplayAlert("Controle Estoque", "Erro ao tentar alterar o estoque do produto " + estoque.Nome, "Ok");
            });
        }
    }
}