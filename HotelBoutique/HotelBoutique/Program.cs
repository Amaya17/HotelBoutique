using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace HotelBoutique
{
    class Program
    {
        static String connectionString = ConfigurationManager.ConnectionStrings["conexionHotelBoutique"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;

        static void Main(string[] args)
        {
            int menuChoice = 0;
            do
            {
                Console.WriteLine("HOTEL BOUTIQUE");
                Console.WriteLine("1. Registrar Cliente");
                Console.WriteLine("2. Editar Cliente");
                Console.WriteLine("3. Check-in");
                Console.WriteLine("4. Check-out");
                Console.WriteLine("5. Exit");

                menuChoice = Convert.ToInt32(Console.ReadLine());
                Menu(menuChoice);

            } while (menuChoice != 5);


        }
        public static void RegistrarCliente()
        {
            
            //PEDIR DATOS CLIENTE
            Console.WriteLine("Introduzca el nombre:");
            string Nombre = Console.ReadLine();
            Console.WriteLine("Introduzca apellidos:");
            string Apellidos = Console.ReadLine();
            Console.WriteLine("Introduzca DNI:");
            string DNI = Console.ReadLine();
            //INSERTAR DATOS CLIENTE

            conexion.Open();

            cadena = "INSERT INTO Cliente VALUES('" + DNI + "','" + Nombre + "','" + Apellidos+ "')";
            comando = new SqlCommand(cadena, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();
            Console.WriteLine("El cliente ha sido registrado.");
            return;
        }
        public static void EditarCliente()
        {
            
            //BUSCAMOS EL DNI DEL CLIENTE
            Console.WriteLine("Introduzca DNI");
            string editDNI = Console.ReadLine();

            conexion.Open();

            cadena = "SELECT * FROM Cliente WHERE DNI LIKE '"+ editDNI+"'";
            comando = new SqlCommand(cadena, conexion);
            SqlDataReader registros = comando.ExecuteReader();
            

            if (registros.Read())
            {
                Console.WriteLine("El registro existe");
                Console.WriteLine("Introduzca el nuevo nombre");
                string nuevoNombre = Console.ReadLine();

                Console.WriteLine("Introduzca los apellidos");
                string nuevoApellidos = Console.ReadLine();

                //EDITAMOS EL NOMBRE DEL CLIENTE
                conexion.Close();
                conexion.Open();
                cadena = "UPDATE Cliente SET Nombre = '"+nuevoNombre+ "' WHERE DNI LIKE'"+editDNI+"'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();

                conexion.Close();

                //EDITAMOS LOS APELLIDOS DEL CLIENTE
                conexion.Open();

                cadena = "UPDATE Cliente SET Apellidos = '" + nuevoApellidos + "' WHERE DNI LIKE'" + editDNI + "'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();

                Console.WriteLine("El registro ha sido editado.");
                Console.ReadLine();


            }
            else
            {
                Console.WriteLine("No existe el registro");
            }

            conexion.Close();
        return;
        }


        public static void check_in()
        {
            //BUSCAMOS EL DNI DEL CLIENTE
            Console.WriteLine("Introduzca DNI");
            string DNI = Console.ReadLine();

            conexion.Open();

            cadena = "SELECT * FROM Cliente WHERE DNI LIKE '" + DNI + "'";
            comando = new SqlCommand(cadena, conexion);
            SqlDataReader registros = comando.ExecuteReader();
            

            if (registros.Read())
            {

                conexion.Close();
                Console.WriteLine("Cliente registrado");

                //HACEMOS UNA CONSULTA A LA TABLA HABITACION
                conexion.Open();
                cadena = "SELECT * FROM Habitacion WHERE Estado like 'libre'";
                comando = new SqlCommand(cadena, conexion);
                SqlDataReader habitacion = comando.ExecuteReader();

                while (habitacion.Read())
                {
                    Console.WriteLine(habitacion["CodHabitacion"].ToString() + "\t" + habitacion["Estado"].ToString());
                    
                }                
                habitacion.Close();
                conexion.Close();
                

                Console.WriteLine("Elija su habitación");

                //CREAR EL CÓDIGO DE RESERVA
                conexion.Open();
                cadena = "SELECT max(CodReserva) FROM Reserva";
                comando = new SqlCommand(cadena, conexion);
                SqlDataReader codReservaR = comando.ExecuteReader();
                int codReserva = Convert.ToInt32(codReservaR.Read())+1;
                conexion.Close();
                int codHabitacion = Convert.ToInt32(Console.ReadLine());

                conexion.Open();
                cadena = "UPDATE Habitacion SET Estado = 'ocupado' WHERE CodHabitacion like'" + codHabitacion + "'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();

                conexion.Open();
                cadena = "INSERT INTO Reserva (CodReserva, CodHabitacion, DNI, Check_in ) VALUES ('"+ codReserva +"','"+codHabitacion+"','"+DNI+"','"+DateTime.Now+"') ";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                                               
                conexion.Close();
                Console.WriteLine("Su habitación ha sido reservada");
                
            }
            else
            {
                Console.WriteLine("El cliente no está registrado, no se puede realizar la reserva");
            }

            conexion.Close();
            return;
        }
        public static void check_out()
        {
            Console.WriteLine("Introduzca el DNI");
            string DNI = Console.ReadLine();

            conexion.Open();

            cadena = "SELECT * FROM Reserva WHERE DNI LIKE '" + DNI + "' AND Check_out IS NULL";
            comando = new SqlCommand(cadena, conexion);
            SqlDataReader registros = comando.ExecuteReader();

            
            if (registros.Read())
            {
                conexion.Close();
                Console.WriteLine("Cliente registrado");

                conexion.Open();
                cadena = "UPDATE Reserva SET Check_out = GETDATE() WHERE DNI like '"+DNI+"'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();
                
                //seleccionar el codHab del que estamos haciendo el check_out
                conexion.Open();
                cadena = "SELECT CodHAbitacion FROM Reserva WHERE DNI like '" + DNI + "'";
                comando = new SqlCommand(cadena, conexion);
                SqlDataReader codHabitacionR = comando.ExecuteReader();
                int codHabitacion = Convert.ToInt32(codHabitacionR.Read());
                conexion.Close();
               //

                conexion.Open();
                cadena = "UPDATE Habitacion  SET Estado = 'libre' WHERE CodHabitacion LIKE '" +codHabitacion+"'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();

                Console.WriteLine("Se ha completado el check-out.");

            }
            else
            {
                Console.WriteLine("Introduzca un DNI válido");
            }


        }
        //MÉTODO MENU
        public static void Menu(int Choice)
        {
            bool exit = false;
            do
            {
                switch (Choice)
                {
                    case 1:
                        RegistrarCliente();
                        exit = true;
                        break;

                    case 2:                       
                        EditarCliente();
                        exit = true;
                        break;
                    case 3:
                        check_in();
                        exit = true;
                        break;

                    case 4:
                        check_out();
                        exit = true;
                        break;

                    case 5:
                        Console.WriteLine("Que tenga un buen día");
                        exit = true;
                        break;


                }

            } while (exit == false);
            //return;
        
        }
    }
}
