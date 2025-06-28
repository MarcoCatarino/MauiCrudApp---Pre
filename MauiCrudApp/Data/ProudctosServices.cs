using MySql.Data.MySqlClient;
using MauiCrudApp.Models;
using Microsoft.Maui.Networking; // Necesario para verificar conexión
using System.Collections.Generic;

namespace MauiCrudApp.Data
{
    public class ProductoService
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=crudmaui;Uid=root;Pwd=;";

        public List<Producto> ObtenerProductos()
        {
            var lista = new List<Producto>();

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Sin conexión", "No tienes conexión a Internet.", "OK");
                });
                return lista;
            }

            try
            {
                using var conexion = new MySqlConnection(connectionString);
                conexion.Open();

                string query = "SELECT * FROM productos";
                using var cmd = new MySqlCommand(query, conexion);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Producto
                    {
                        Id = reader.GetInt32("id"),
                        Nombre = reader.GetString("nombre"),
                        Descripcion = reader.GetString("descripcion"),
                        Precio = reader.GetFloat("precio")
                    });
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al obtener estudiantes: {ex.Message}", "OK");
                });
            }

            return lista;
        }

        public void AgregarProducto(Producto producto)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Sin conexión", "No tienes conexión a Internet.", "OK");
                });
                return;
            }

            try
            {
                using var conexion = new MySqlConnection(connectionString);
                conexion.Open();

                string query = "INSERT INTO productos (nombre, descripcion, precio) VALUES (@nombre, @descripcion, @precio)";
                using var cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                cmd.Parameters.AddWithValue("@precio", producto.Precio);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al agregar producto: {ex.Message}", "OK");
                });
            }
        }

        public void ActualizarProducto(Producto producto)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Sin conexión", "No tienes conexión a Internet.", "OK");
                });
                return;
            }

            try
            {
                using var conexion = new MySqlConnection(connectionString);
                conexion.Open();

                string query = "UPDATE productos SET nombre = @nombre, descripcion = @descripcion, precio = @precio WHERE id = @id";
                using var cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                cmd.Parameters.AddWithValue("@precio", producto.Precio);
                cmd.Parameters.AddWithValue("@id", producto.Id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al actualizar producto: {ex.Message}", "OK");
                });
            }
        }

        public void EliminarProducto(int id)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Sin conexión", "No tienes conexión a Internet.", "OK");
                });
                return;
            }

            try
            {
                using var conexion = new MySqlConnection(connectionString);
                conexion.Open();

                string query = "DELETE FROM productos WHERE id = @id";
                using var cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al eliminar producto: {ex.Message}", "OK");
                });
            }
        }
    }
}
