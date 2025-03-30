using MauiAppMinhasCompras.Models;
using System;
using System.Linq;

namespace MauiAppMinhasCompras.Views
{
    public partial class EditarProduto : ContentPage
    {
        private Produto _produto;

        public EditarProduto(Produto produto)
        {
            InitializeComponent();
            _produto = produto;

            // Preenche os campos com os dados do produto
            txt_id.Text = _produto.Id.ToString();
            txt_descricao.Text = _produto.Descricao;
            txt_quantidade.Text = _produto.Quantidade.ToString();
            txt_preco.Text = _produto.Preco.ToString();

            // Seleciona a categoria correta no Picker
            if (!string.IsNullOrEmpty(_produto.Categoria))
            {
                int index = picker_categoria.Items.IndexOf(_produto.Categoria);
                if (index >= 0)
                {
                    picker_categoria.SelectedIndex = index;
                }
            }
        }

        private async void ToolbarItem_Salvar_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Validação dos campos
                if (string.IsNullOrWhiteSpace(txt_descricao.Text) ||
                    string.IsNullOrWhiteSpace(txt_quantidade.Text) ||
                    string.IsNullOrWhiteSpace(txt_preco.Text) ||
                    picker_categoria.SelectedItem == null)
                {
                    await DisplayAlert("Aviso", "Todos os campos são obrigatórios.", "OK");
                    return;
                }

                if (!double.TryParse(txt_quantidade.Text, out double quantidade) ||
                    !double.TryParse(txt_preco.Text, out double preco))
                {
                    await DisplayAlert("Aviso", "Preencha os campos numéricos corretamente.", "OK");
                    return;
                }

                // Verifica se já existe um produto com o mesmo nome (exceto o produto atual)
                var produtos = await App.Db.GetAll();
                bool nomeExistente = produtos.Any(p => p.Descricao.ToLower() == txt_descricao.Text.ToLower() && p.Id != _produto.Id);

                if (nomeExistente)
                {
                    await DisplayAlert("Aviso", "Já existe um produto com este nome.", "OK");
                    return;
                }

                // Atualiza os dados do produto
                _produto.Descricao = txt_descricao.Text;
                _produto.Quantidade = quantidade;
                _produto.Preco = preco;
                _produto.Categoria = picker_categoria.SelectedItem.ToString();

                // Atualiza o produto no banco de dados
                await App.Db.Update(_produto);
                await DisplayAlert("Sucesso!", "Registro Atualizado", "OK");

                // Volta para a página anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}
