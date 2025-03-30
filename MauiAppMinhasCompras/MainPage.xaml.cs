using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Views;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Produto> _produtos = new ObservableCollection<Produto>();

        public MainPage()
        {
            InitializeComponent();
            collectionViewProdutos.ItemsSource = _produtos;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarProdutos();
        }

        private async Task CarregarProdutos()
        {
            var produtos = await App.Db.GetAll();
            _produtos.Clear();
            foreach (var produto in produtos)
            {
                _produtos.Add(produto);
            }
        }

        private async void ToolbarItem_Adicionar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovoProduto());
        }

        private async void OnProdutoSelecionado(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;

            var produtoSelecionado = (Produto)e.CurrentSelection.FirstOrDefault();
            if (produtoSelecionado != null)
            {
                await Navigation.PushAsync(new EditarProduto(produtoSelecionado));
            }

            collectionViewProdutos.SelectedItem = null;
        }
    }
}
