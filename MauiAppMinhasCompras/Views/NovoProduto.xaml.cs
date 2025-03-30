using MauiAppMinhasCompras.Models;
using System;
using System.Linq;

namespace MauiAppMinhasCompras.Views
{
    public partial class NovoProduto : ContentPage
    {
        public NovoProduto()
        {
            InitializeComponent();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_id.Text) ||
                    string.IsNullOrWhiteSpace(txt_descricao.Text) ||
                    string.IsNullOrWhiteSpace(txt_quantidade.Text) ||
                    string.IsNullOrWhiteSpace(txt_preco.Text) ||
                    picker_categoria.SelectedItem == null)
                {
                    await DisplayAlert("Aviso", "Todos os campos são obrigatórios.", "OK");
                    return;
                }

                if (!int.TryParse(txt_id.Text, out int id) ||
                    !double.TryParse(txt_quantidade.Text, out double quantidade) ||
                    !double.TryParse(txt_preco.Text, out double preco))
                {
                    await DisplayAlert("Aviso", "Preencha os campos numéricos corretamente.", "OK");
                    return;
                }

                var produtos = await App.Db.GetAll();
                if (produtos.Any(p => p.Descricao.ToLower() == txt_descricao.Text.ToLower()))
                {
                    await DisplayAlert("Aviso", "Já existe um produto com esta descrição.", "OK");
                    return;
                }

                Produto p = new Produto
                {
                    Id = id,
                    Descricao = txt_descricao.Text,
                    Quantidade = quantidade,
                    Preco = preco,
                    Categoria = picker_categoria.SelectedItem.ToString()
                };

                await App.Db.Insert(p);
                await DisplayAlert("Sucesso!", "Produto cadastrado com sucesso!", "OK");

                txt_id.Text = string.Empty;
                txt_descricao.Text = string.Empty;
                txt_quantidade.Text = string.Empty;
                txt_preco.Text = string.Empty;
                picker_categoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}
