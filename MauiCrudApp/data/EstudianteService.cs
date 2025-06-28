using MySql.Data.MySqlClient;
using MauiCrudApp.Models;
using Microsoft.Maui.Networking; // Necesario para verificar conexión
using System.Collections.Generic;

namespace MauiCrudApp.Data
{
    public class EstudianteService
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=crudmaui;Uid=root;Pwd=;";

        public List<Estudiante> ObtenerEstudiantes()
        {
            var lista = new List<Estudiante>();

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

                string query = "SELECT * FROM estudiantes";
                using var cmd = new MySqlCommand(query, conexion);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Estudiante
                    {
                        Id = reader.GetInt32("id"),
                        Nombre = reader.GetString("nombre"),
                        Correo = reader.GetString("correo"),
                        Carrera = reader.GetString("carrera")
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

        public void AgregarEstudiante(Estudiante estudiante)
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

                string query = "INSERT INTO estudiantes (nombre, correo, carrera) VALUES (@nombre, @correo, @carrera)";
                using var cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@nombre", estudiante.Nombre);
                cmd.Parameters.AddWithValue("@correo", estudiante.Correo);
                cmd.Parameters.AddWithValue("@carrera", estudiante.Carrera);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al agregar estudiante: {ex.Message}", "OK");
                });
            }
        }

        public void ActualizarEstudiante(Estudiante estudiante)
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

                string query = "UPDATE estudiantes SET nombre = @nombre, correo = @correo, carrera = @carrera WHERE id = @id";
                using var cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@nombre", estudiante.Nombre);
                cmd.Parameters.AddWithValue("@correo", estudiante.Correo);
                cmd.Parameters.AddWithValue("@carrera", estudiante.Carrera);
                cmd.Parameters.AddWithValue("@id", estudiante.Id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al actualizar estudiante: {ex.Message}", "OK");
                });
            }
        }

        public void EliminarEstudiante(int id)
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

                string query = "DELETE FROM estudiantes WHERE id = @id";
                using var cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error al eliminar estudiante: {ex.Message}", "OK");
                });
            }
        }
    }
}
