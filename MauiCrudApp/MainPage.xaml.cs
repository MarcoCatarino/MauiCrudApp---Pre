using MauiCrudApp.Data;
using MauiCrudApp.Models;
using System.Windows.Input;

namespace MauiCrudApp;

public partial class MainPage : ContentPage
{
    private readonly EstudianteService service = new();
    private Estudiante? estudianteSeleccionado = null;

    public ICommand OnEstudianteTapped { get; }

    public MainPage()
    {
        InitializeComponent();
        OnEstudianteTapped = new Command<Estudiante>(SeleccionarEstudiante);
        BindingContext = this;
    }

    private void OnCargarEstudiantesClicked(object sender, EventArgs e)
    {
        var lista = service.ObtenerEstudiantes();
        estudiantesView.ItemsSource = lista;
    }

    private void OnAgregarClicked(object sender, EventArgs e)
    {
        var nuevo = new Estudiante
        {
            Nombre = nombreEntry.Text ?? "",
            Correo = correoEntry.Text ?? "",
            Carrera = carreraEntry.Text ?? ""
        };

        service.AgregarEstudiante(nuevo);
        OnCargarEstudiantesClicked(sender, e);
        LimpiarCampos();
    }

    private void OnActualizarClicked(object sender, EventArgs e)
    {
        if (estudianteSeleccionado == null)
            return;

        estudianteSeleccionado.Nombre = nombreEntry.Text ?? "";
        estudianteSeleccionado.Correo = correoEntry.Text ?? "";
        estudianteSeleccionado.Carrera = carreraEntry.Text ?? "";

        service.ActualizarEstudiante(estudianteSeleccionado);
        OnCargarEstudiantesClicked(sender, e);
        LimpiarCampos();
    }

    private void OnEliminarClicked(object sender, EventArgs e)
    {
        if (estudianteSeleccionado == null)
            return;

        service.EliminarEstudiante(estudianteSeleccionado.Id);
        OnCargarEstudiantesClicked(sender, e);
        LimpiarCampos();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // se maneja con el TapGestureRecognizer
    }

    private void SeleccionarEstudiante(Estudiante estudiante)
    {
        estudianteSeleccionado = estudiante;

        nombreEntry.Text = estudiante.Nombre;
        correoEntry.Text = estudiante.Correo;
        carreraEntry.Text = estudiante.Carrera;

        estudiantesView.SelectedItem = estudiante;
    }

    private void LimpiarCampos()
    {
        nombreEntry.Text = "";
        correoEntry.Text = "";
        carreraEntry.Text = "";
        estudianteSeleccionado = null;
        estudiantesView.SelectedItem = null;
    }
}
