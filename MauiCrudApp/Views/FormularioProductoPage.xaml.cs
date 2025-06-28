using MauiCrudApp.Data;
using MauiCrudApp.Models;
using Microsoft.Maui.Controls;

namespace MauiCrudApp.Views;

[QueryProperty(nameof(ProductoToEdit), "producto")]
public partial class FormularioProductoPage : ContentPage
{
    private readonly ProductoService _productoService;
    private Producto? _productoToEdit;
    private bool _isEditMode = false;

    public Producto? ProductoToEdit
    {
        get => _productoToEdit;
        set
        {
            _productoToEdit = value;
            ConfigurarModoEdicion();
        }
    }

    public FormularioProductoPage()
    {
        InitializeComponent();
        _productoService = new ProductoService();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Si no estamos en modo edición, limpiar el formulario
        if (!_isEditMode)
        {
            LimpiarFormulario();
        }
    }

    private void ConfigurarModoEdicion()
    {
        if (_productoToEdit != null)
        {
            _isEditMode = true;

            // Cambiar título y botones
            tituloLabel.Text = "Editar Producto";
            guardarButton.Text = "Actualizar Producto";
            guardarButton.BackgroundColor = Color.FromArgb("#facc15");

            // Mostrar información de edición
            infoEdicionLayout.IsVisible = true;
            cancelarButton.IsVisible = true;
            infoProductoLabel.Text = $"ID: {_productoToEdit.Id} | Nombre: {_productoToEdit.Nombre}";

            // Cargar datos en el formulario
            CargarDatosProducto(_productoToEdit);
        }
        else
        {
            ConfigurarModoCreacion();
        }
    }

    private void ConfigurarModoCreacion()
    {
        _isEditMode = false;

        // Configurar para modo creación
        tituloLabel.Text = "Nuevo Producto";
        guardarButton.Text = "Guardar Producto";
        guardarButton.BackgroundColor = Color.FromArgb("#22c55e");

        // Ocultar elementos de edición
        infoEdicionLayout.IsVisible = false;
        cancelarButton.IsVisible = false;

        LimpiarFormulario();
    }

    private void CargarDatosProducto(Producto producto)
    {
        nombreEntry.Text = producto.Nombre;
        descripcionEntry.Text = producto.Descripcion;
        precioEntry.Text = producto.Precio.ToString(); // Convertir float a string
    }

    //private void LimpiarFormulario()
    //{
    //    nombreEntry.Text = string.Empty;
    //    descripcionEntry.Text = string.Empty;
    //    precioEntry = string.Empty;
    //}

    private void LimpiarFormulario()
    {
        nombreEntry.Text = "";
        descripcionEntry.Text = "";
        precioEntry.Text = "0.00";
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        // Validar campos
        if (!ValidarCampos())
            return;

        try
        {
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;
            guardarButton.IsEnabled = false;

            if (_isEditMode && _productoToEdit != null)
            {
                await ActualizarProducto();
            }
            else
            {
                await CrearProducto();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
        }
        finally
        {
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
            guardarButton.IsEnabled = true;
        }
    }

    private async Task CrearProducto()
    {
        var nuevoProducto = new Producto
        {
            Nombre = nombreEntry.Text?.Trim() ?? string.Empty,
            Descripcion = descripcionEntry.Text?.Trim() ?? string.Empty,
            Precio = float.TryParse(precioEntry.Text, out var precio) ? precio : 0.0f
        };

        await Task.Run(() => _productoService.AgregarProducto(nuevoProducto));

        await DisplayAlert("Éxito", "Producto creado correctamente", "OK");
        LimpiarFormulario();
    }

    private async Task ActualizarProducto()
    {
        if (_productoToEdit == null) return;

        _productoToEdit.Nombre = nombreEntry.Text?.Trim() ?? string.Empty;
        _productoToEdit.Descripcion = descripcionEntry.Text?.Trim() ?? string.Empty;
        _productoToEdit.Precio = float.TryParse(precioEntry.Text, out var precio) ? precio : 0.0f;

        await Task.Run(() => _productoService.ActualizarProducto(_productoToEdit));

        await DisplayAlert("Éxito", "Producto actualizado correctamente", "OK");

        // Volver a la lista después de actualizar
        await Shell.Current.GoToAsync("//ListaProductos");
    }

    private bool ValidarCampos()
    {
        // Validación del nombre
        if (string.IsNullOrWhiteSpace(nombreEntry.Text))
        {
            DisplayAlert("Validación", "El nombre es requerido", "OK");
            nombreEntry.Focus();
            return false;
        }

        // Validación de la descripción
        if (string.IsNullOrWhiteSpace(descripcionEntry.Text))
        {
            DisplayAlert("Validación", "La descripción es requerida", "OK");
            descripcionEntry.Focus();
            return false;
        }

        // Validación del precio (que no esté vacío y sea un número válido)
        if (string.IsNullOrWhiteSpace(precioEntry.Text))
        {
            DisplayAlert("Validación", "El precio es requerido", "OK");
            precioEntry.Focus();
            return false;
        }

        // Verificar que el precio sea un número válido (float)
        if (!float.TryParse(precioEntry.Text, out float precio) || precio <= 0)
        {
            DisplayAlert("Validación", "El precio debe ser un número mayor a 0", "OK");
            precioEntry.Focus();
            return false;
        }

        // Si todas las validaciones pasan
        return true;
    }

    private void OnLimpiarClicked(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    private async void OnVerListaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ListaProductos");
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Cancelar edición",
            "¿Estás seguro de que deseas cancelar? Los cambios no guardados se perderán.",
            "Sí",
            "No");

        if (confirm)
        {
            await Shell.Current.GoToAsync("//ListaProductos");
        }
    }
}