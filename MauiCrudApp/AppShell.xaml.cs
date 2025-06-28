using Microsoft.Maui.Controls;

namespace MauiCrudApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rutas para navegación programática
            //Routing.RegisterRoute("ListaEstudiantes", typeof(Views.ListaEstudiantesPage));
            //Routing.RegisterRoute("FormularioEstudiante", typeof(Views.FormularioEstudiantePage));
            Routing.RegisterRoute("ListaProductos", typeof(Views.ListaProductosPage));
            Routing.RegisterRoute("FormularioProducto", typeof(Views.FormularioProductoPage));
        }
    }
}