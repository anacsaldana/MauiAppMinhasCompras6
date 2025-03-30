using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    private ObservableCollection<Produto> produtos = new();
    private ObservableCollection<Produto> produtosFiltrados = new();
    private List<string> categorias = new(); // Lista de categorias

    public ListaProduto()
    {
        InitializeComponent();
        listaProdutosView.ItemsSource = produtosFiltrados;
    }

    private async void ListaProduto_Appearing(object sender, EventArgs e)
    {
        await AtualizarLista();
    }

    private async Task AtualizarLista()
    {
        var lista = await App.Db.GetAll();
        produtos.Clear();
        produtosFiltrados.Clear();

        // Preencher a lista de categorias para o Picker e adicionar a opção "Todas as Categorias"
        categorias = lista.Select(p => p.Categoria).Distinct().ToList();
        categorias.Insert(0, "Todas as Categorias"); // Adiciona a opção "Todas as Categorias"
        categoriaPicker.ItemsSource = categorias;

        foreach (var produto in lista)
        {
            produtos.Add(produto);
            produtosFiltrados.Add(produto);
        }
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textoBusca = e.NewTextValue?.ToLower() ?? "";

        produtosFiltrados.Clear();
        foreach (var produto in produtos)
        {
            if (produto.Descricao.ToLower().Contains(textoBusca))
            {
                produtosFiltrados.Add(produto);
            }
        }
    }

    private async void SomarButton_Clicked(object sender, EventArgs e)
    {
        if (categoriaPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Erro", "Por favor, selecione uma categoria.", "OK");
            return;
        }

        string categoriaSelecionada = categoriaPicker.SelectedItem.ToString();
        double total = 0;

        var produtosDaCategoria = produtos.Where(p => p.Categoria == categoriaSelecionada).ToList();

        foreach (var produto in produtosDaCategoria)
        {
            total += produto.Quantidade * produto.Preco;
        }

        await DisplayAlert("Total", $"O valor total para a categoria '{categoriaSelecionada}' é: R$ {total:F2}", "OK");
    }

    private void CategoriaPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (categoriaPicker.SelectedIndex == 0) // "Todas as Categorias"
        {
            // Exibe todos os produtos
            produtosFiltrados.Clear();
            foreach (var produto in produtos)
            {
                produtosFiltrados.Add(produto);
            }
        }
        else
        {
            // Filtra os produtos pela categoria selecionada
            string categoriaSelecionada = categoriaPicker.SelectedItem.ToString();
            produtosFiltrados.Clear();

            foreach (var produto in produtos)
            {
                if (produto.Categoria == categoriaSelecionada)
                {
                    produtosFiltrados.Add(produto);
                }
            }
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
        {
            Produto produtoSelecionado = (Produto)e.CurrentSelection[0];
            await DisplayAlert("Produto Selecionado", produtoSelecionado.Descricao, "OK");

            listaProdutosView.SelectedItem = null;
        }
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int id)
        {
            await App.Db.Delete(id);
            await AtualizarLista();
        }
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int id)
        {
            // Busca o produto pelo ID
            var produto = await App.Db.GetById(id);

            if (produto != null)
            {
                // Navega para a página de edição, passando o produto como parâmetro
                await Navigation.PushAsync(new EditarProduto(produto));
            }
            else
            {
                await DisplayAlert("Erro", "Produto não encontrado.", "OK");
            }
        }
    }
}
