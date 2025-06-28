using Microsoft.Maui.Controls;

namespace MauiCrudApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas para navegación programática
            Routing.RegisterRoute("ListaEstudiantes", typeof(Views.ListaEstudiantesPage));
            Routing.RegisterRoute("FormularioEstudiante", typeof(Views.FormularioEstudiantePage));
        }
    }
}