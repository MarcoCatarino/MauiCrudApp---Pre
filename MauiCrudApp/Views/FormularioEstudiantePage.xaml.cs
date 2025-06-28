using MauiCrudApp.Data;
using MauiCrudApp.Models;
using Microsoft.Maui.Controls;

namespace MauiCrudApp.Views;

[QueryProperty(nameof(EstudianteToEdit), "estudiante")]
public partial class FormularioEstudiantePage : ContentPage
{
    private readonly EstudianteService _estudianteService;
    private Estudiante? _estudianteToEdit;
    private bool _isEditMode = false;

    public Estudiante? EstudianteToEdit
    {
        get => _estudianteToEdit;
        set
        {
            _estudianteToEdit = value;
            ConfigurarModoEdicion();
        }
    }

    public FormularioEstudiantePage()
    {
        InitializeComponent();
        _estudianteService = new EstudianteService();
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
        if (_estudianteToEdit != null)
        {
            _isEditMode = true;

            // Cambiar título y botones
            tituloLabel.Text = "Editar Estudiante";
            guardarButton.Text = "Actualizar Estudiante";
            guardarButton.BackgroundColor = Color.FromArgb("#facc15");

            // Mostrar información de edición
            infoEdicionLayout.IsVisible = true;
            cancelarButton.IsVisible = true;
            infoEstudianteLabel.Text = $"ID: {_estudianteToEdit.Id} | Nombre: {_estudianteToEdit.Nombre}";

            // Cargar datos en el formulario
            CargarDatosEstudiante(_estudianteToEdit);
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
        tituloLabel.Text = "Nuevo Estudiante";
        guardarButton.Text = "Guardar Estudiante";
        guardarButton.BackgroundColor = Color.FromArgb("#22c55e");

        // Ocultar elementos de edición
        infoEdicionLayout.IsVisible = false;
        cancelarButton.IsVisible = false;

        LimpiarFormulario();
    }

    private void CargarDatosEstudiante(Estudiante estudiante)
    {
        nombreEntry.Text = estudiante.Nombre;
        correoEntry.Text = estudiante.Correo;
        carreraEntry.Text = estudiante.Carrera;
    }

    private void LimpiarFormulario()
    {
        nombreEntry.Text = string.Empty;
        correoEntry.Text = string.Empty;
        carreraEntry.Text = string.Empty;
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

            if (_isEditMode && _estudianteToEdit != null)
            {
                await ActualizarEstudiante();
            }
            else
            {
                await CrearEstudiante();
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

    private async Task CrearEstudiante()
    {
        var nuevoEstudiante = new Estudiante
        {
            Nombre = nombreEntry.Text?.Trim() ?? string.Empty,
            Correo = correoEntry.Text?.Trim() ?? string.Empty,
            Carrera = carreraEntry.Text?.Trim() ?? string.Empty
        };

        await Task.Run(() => _estudianteService.AgregarEstudiante(nuevoEstudiante));

        await DisplayAlert("Éxito", "Estudiante creado correctamente", "OK");
        LimpiarFormulario();
    }

    private async Task ActualizarEstudiante()
    {
        if (_estudianteToEdit == null) return;

        _estudianteToEdit.Nombre = nombreEntry.Text?.Trim() ?? string.Empty;
        _estudianteToEdit.Correo = correoEntry.Text?.Trim() ?? string.Empty;
        _estudianteToEdit.Carrera = carreraEntry.Text?.Trim() ?? string.Empty;

        await Task.Run(() => _estudianteService.ActualizarEstudiante(_estudianteToEdit));

        await DisplayAlert("Éxito", "Estudiante actualizado correctamente", "OK");

        // Volver a la lista después de actualizar
        await Shell.Current.GoToAsync("//ListaEstudiantes");
    }

    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(nombreEntry.Text))
        {
            DisplayAlert("Validación", "El nombre es requerido", "OK");
            nombreEntry.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(correoEntry.Text))
        {
            DisplayAlert("Validación", "El correo es requerido", "OK");
            correoEntry.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(carreraEntry.Text))
        {
            DisplayAlert("Validación", "La carrera es requerida", "OK");
            carreraEntry.Focus();
            return false;
        }

        // Validación básica de email
        if (!correoEntry.Text.Contains("@") || !correoEntry.Text.Contains("."))
        {
            DisplayAlert("Validación", "Por favor ingresa un correo válido", "OK");
            correoEntry.Focus();
            return false;
        }

        return true;
    }

    private void OnLimpiarClicked(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    private async void OnVerListaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ListaEstudiantes");
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
            await Shell.Current.GoToAsync("//ListaEstudiantes");
        }
    }
}