using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections; //para el arraylist

namespace DAL
{
    public class Acceso
    {
        public SqlConnection oCnn = new SqlConnection(@"Data Source=DESKTOP-94B70F5;Initial Catalog=literaryhub;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        public SqlTransaction Tranx;
        public SqlCommand Cmd;


        public string TestConnection()
        {
            oCnn.Open();
            if (oCnn.State == ConnectionState.Open)
            {
                return "Conexion con la bdd establecida";
            }
            else
            {
                return "No se pudo establecer la conexion con la base de datos";
            }
        }


        public int LeerCantidad(string Consulta, Hashtable Hdatos)
        {
            oCnn.Open();
            Cmd = new SqlCommand(Consulta, oCnn);
            Cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if ((Hdatos != null))
                {
                    foreach (string dato in Hdatos.Keys)
                    {
                        Cmd.Parameters.AddWithValue(dato, Hdatos[dato]);
                    }
                }

                int Respuesta = Convert.ToInt32(Cmd.ExecuteScalar());
                oCnn.Close();
                return Respuesta;
            }
            catch (SqlException ex)
            { throw ex; }
            catch (Exception ex)
            { throw ex; }
        }

        public bool LeerScalar(string Consulta, Hashtable Hdatos)
        {
            oCnn.Open();
            Cmd = new SqlCommand(Consulta, oCnn);
            Cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                if ((Hdatos != null))
                {
                    foreach (string dato in Hdatos.Keys)
                    {
                        Cmd.Parameters.AddWithValue(dato, Hdatos[dato]);
                    }
                }

                int Respuesta = Convert.ToInt32(Cmd.ExecuteScalar());
                oCnn.Close();
                if (Respuesta > 0)
                { return true; }
                else
                { return false; }
            }
            catch (SqlException ex)
            { throw ex; }
            catch (Exception ex)
            { throw ex; }
        }
        public DataTable Leer(string Consulta, Hashtable Hdatos)
        {
            DataTable Dt = new DataTable();
            SqlDataAdapter Da;
            Cmd = new SqlCommand(Consulta, oCnn);
            Cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                Da = new SqlDataAdapter(Cmd);

                if ((Hdatos != null))
                {
                    foreach (string dato in Hdatos.Keys)
                    {
                        Cmd.Parameters.AddWithValue(dato, Hdatos[dato]);
                    }
                }

            }
            catch (SqlException ex)
            { throw ex; }
            catch (Exception ex)
            { throw ex; }
            Da.Fill(Dt);
            return Dt;


        }
        public bool Escribir(string consulta, Hashtable Hdatos)
        {

            if (oCnn.State == ConnectionState.Closed)
            {
                oCnn.Open();
            }

            try
            {
                Tranx = oCnn.BeginTransaction();
                Cmd = new SqlCommand(consulta, oCnn, Tranx);
                Cmd.CommandType = CommandType.StoredProcedure;

                if ((Hdatos != null))
                {
                    foreach (string dato in Hdatos.Keys)
                    {
                        Cmd.Parameters.AddWithValue(dato, Hdatos[dato]);
                    }
                }

                int respuesta = Cmd.ExecuteNonQuery();
                Tranx.Commit();
                return true;

            }

            catch (SqlException ex)
            {
                Tranx.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                Tranx.Rollback();
                return false;
            }
            finally
            { oCnn.Close(); }

        }

    }
}