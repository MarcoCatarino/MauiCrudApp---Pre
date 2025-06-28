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

        // Si no estamos en modo edici�n, limpiar el formulario
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

            // Cambiar t�tulo y botones
            tituloLabel.Text = "Editar Estudiante";
            guardarButton.Text = "Actualizar Estudiante";
            guardarButton.BackgroundColor = Color.FromArgb("#facc15");

            // Mostrar informaci�n de edici�n
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

        // Configurar para modo creaci�n
        tituloLabel.Text = "Nuevo Estudiante";
        guardarButton.Text = "Guardar Estudiante";
        guardarButton.BackgroundColor = Color.FromArgb("#22c55e");

        // Ocultar elementos de edici�n
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

        await DisplayAlert("�xito", "Estudiante creado correctamente", "OK");
        LimpiarFormulario();
    }

    private async Task ActualizarEstudiante()
    {
        if (_estudianteToEdit == null) return;

        _estudianteToEdit.Nombre = nombreEntry.Text?.Trim() ?? string.Empty;
        _estudianteToEdit.Correo = correoEntry.Text?.Trim() ?? string.Empty;
        _estudianteToEdit.Carrera = carreraEntry.Text?.Trim() ?? string.Empty;

        await Task.Run(() => _estudianteService.ActualizarEstudiante(_estudianteToEdit));

        await DisplayAlert("�xito", "Estudiante actualizado correctamente", "OK");

        // Volver a la lista despu�s de actualizar
        await Shell.Current.GoToAsync("//ListaEstudiantes");
    }

    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(nombreEntry.Text))
        {
            DisplayAlert("Validaci�n", "El nombre es requerido", "OK");
            nombreEntry.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(correoEntry.Text))
        {
            DisplayAlert("Validaci�n", "El correo es requerido", "OK");
            correoEntry.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(carreraEntry.Text))
        {
            DisplayAlert("Validaci�n", "La carrera es requerida", "OK");
            carreraEntry.Focus();
            return false;
        }

        // Validaci�n b�sica de email
        if (!correoEntry.Text.Contains("@") || !correoEntry.Text.Contains("."))
        {
            DisplayAlert("Validaci�n", "Por favor ingresa un correo v�lido", "OK");
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
            "Cancelar edici�n",
            "�Est�s seguro de que deseas cancelar? Los cambios no guardados se perder�n.",
            "S�",
            "No");

        if (confirm)
        {
            await Shell.Current.GoToAsync("//ListaEstudiantes");
        }
    }
}