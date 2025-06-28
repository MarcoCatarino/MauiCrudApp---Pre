using MauiCrudApp.Data;
using MauiCrudApp.Models;
using Microsoft.Maui.Controls;

namespace MauiCrudApp.Views;

public partial class ListaEstudiantesPage : ContentPage
{
    private readonly EstudianteService _estudianteService;

    public ListaEstudiantesPage()
    {
        InitializeComponent();
        _estudianteService = new EstudianteService();
        LoadStudents();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStudents();
    }

    private async void LoadStudents()
    {
        try
        {
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            // Ejecutar en background thread
            var estudiantes = await Task.Run(() => _estudianteService.ObtenerEstudiantes());

            estudiantesCollectionView.ItemsSource = estudiantes;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar estudiantes: {ex.Message}", "OK");
        }
        finally
        {
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;
        }
    }

    private async void OnNuevoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//FormularioEstudiante");
    }

    private async void OnActualizarListaClicked(object sender, EventArgs e)
    {
        LoadStudents();
    }

    private async void OnEstudianteTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Estudiante estudiante)
        {
            string action = await DisplayActionSheet(
                $"¿Qué deseas hacer con {estudiante.Nombre}?",
                "Cancelar",
                null,
                "Ver detalles",
                "Editar",
                "Eliminar");

            switch (action)
            {
                case "Ver detalles":
                    await MostrarDetalles(estudiante);
                    break;
                case "Editar":
                    await EditarEstudiante(estudiante);
                    break;
                case "Eliminar":
                    await EliminarEstudiante(estudiante);
                    break;
            }
        }
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Estudiante estudiante)
        {
            await EditarEstudiante(estudiante);
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Estudiante estudiante)
        {
            await EliminarEstudiante(estudiante);
        }
    }

    private async Task EditarEstudiante(Estudiante estudiante)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            ["estudiante"] = estudiante
        };

        await Shell.Current.GoToAsync("//FormularioEstudiante", navigationParameter);
    }

    private async Task EliminarEstudiante(Estudiante estudiante)
    {
        bool confirm = await DisplayAlert(
            "Confirmar eliminación",
            $"¿Estás seguro de que deseas eliminar a {estudiante.Nombre}?",
            "Sí",
            "No");

        if (confirm)
        {
            try
            {
                loadingIndicator.IsVisible = true;
                loadingIndicator.IsRunning = true;

                await Task.Run(() => _estudianteService.EliminarEstudiante(estudiante.Id));

                await DisplayAlert("Éxito", "Estudiante eliminado correctamente", "OK");
                LoadStudents();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar estudiante: {ex.Message}", "OK");
            }
            finally
            {
                loadingIndicator.IsVisible = false;
                loadingIndicator.IsRunning = false;
            }
        }
    }

    private async Task MostrarDetalles(Estudiante estudiante)
    {
        string mensaje = $"ID: {estudiante.Id}\n" +
                        $"Nombre: {estudiante.Nombre}\n" +
                        $"Correo: {estudiante.Correo}\n" +
                        $"Carrera: {estudiante.Carrera}";

        await DisplayAlert("Detalles del Estudiante", mensaje, "OK");
    }
}