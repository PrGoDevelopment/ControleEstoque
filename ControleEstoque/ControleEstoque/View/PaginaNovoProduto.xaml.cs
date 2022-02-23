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
    public partial class PaginaNovoProduto : ContentPage
    {
        public PaginaNovoProduto()
        {
            InitializeComponent();
        }

        private void salvarProduto(object sender, EventArgs e)
        {

        }
    }
}