using MauiCrudApp.Data;
using MauiCrudApp.Models;
using Microsoft.Maui.Controls;

namespace MauiCrudApp.Views;

public partial class ListaProductosPage : ContentPage
{
    private readonly ProductoService _productoService;

    public ListaProductosPage()
    {
        InitializeComponent();
        _productoService = new ProductoService();
        LoadProductos();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadProductos();
    }

    private async void LoadProductos()
    {
        try
        {
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            // Ejecutar en background thread
            var productos = await Task.Run(() => _productoService.ObtenerProductos());

            productosCollectionView.ItemsSource = productos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar productos: {ex.Message}", "OK");
        }
        finally
        {
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }

    private async void OnNuevoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//FormularioProducto");
    }

    private async void OnActualizarListaClicked(object sender, EventArgs e)
    {
        LoadProductos();
    }

    private async void OnProductoTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Producto producto)
        {
            string action = await DisplayActionSheet(
                $"¿Qué deseas hacer con {producto.Nombre}?",
                "Cancelar",
                null,
                "Ver detalles",
                "Editar",
                "Eliminar");

            switch (action)
            {
                case "Ver detalles":
                    await MostrarDetalles(producto);
                    break;
                case "Editar":
                    await EditarProducto(producto);
                    break;
                case "Eliminar":
                    await EliminarProducto(producto);
                    break;
            }
        }
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Producto producto)
        {
            await EditarProducto(producto);
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Producto producto)
        {
            await EliminarProducto(producto);
        }
    }

    private async Task EditarProducto(Producto producto)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            ["producto"] = producto
        };

        await Shell.Current.GoToAsync("//FormularioProducto", navigationParameter);
    }

    private async Task EliminarProducto(Producto producto)
    {
        bool confirm = await DisplayAlert(
            "Confirmar eliminación",
            $"¿Estás seguro de que deseas eliminar a {producto.Nombre}?",
            "Sí",
            "No");

        if (confirm)
        {
            try
            {
                loadingIndicator.IsVisible = true;
                loadingIndicator.IsRunning = true;

                await Task.Run(() => _productoService.EliminarProducto(producto.Id));

                await DisplayAlert("Éxito", "Producto eliminado correctamente", "OK");
                LoadProductos();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar producto: {ex.Message}", "OK");
            }
            finally
            {
                loadingIndicator.IsVisible = false;
                loadingIndicator.IsRunning = false;
            }
        }
    }

    private async Task MostrarDetalles(Producto producto)
    {
        string mensaje = $"ID: {producto.Id}\n" +
                        $"Nombre: {producto.Nombre}\n" +
                        $"Descripcion: {producto.Descripcion}\n" +
                        $"Precio: {producto.Precio}";

        await DisplayAlert("Detalles del Producto", mensaje, "OK");
    }
}