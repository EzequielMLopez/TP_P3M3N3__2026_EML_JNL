using System;
using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using MySqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones a completar:
        static void MenuEmitirTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR NUEVA TARJETA ---");
            
            EmisionTarjeta();

            Console.WriteLine("Funcionalidad en desarrollo...");
            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();

        }

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-11} {4,-16} {5, -15} {6, -10}", "Nombre", "Apellido", "Email", "DNI Titular", "Nro Tarjeta", "Banco Emisor", "Saldo");
            Console.WriteLine("=====================================================================================================================");
            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion(){
          Console.Clear();
          Console.WriteLine("--- EMITIR NUEVA LIQUIDACION MENSUAL ---");
          Console.Write("Ingrese el Número de Cuenta de la tarjeta para mostrar su liquidacion mensual: ");
          int numCuenta = Convert.ToInt32(Console.ReadLine());
          Console.WriteLine("{0, -12} {1, -18} {2, -20} {3, -5} {4, -10} {5, -10}", 
                            "ID", "Nro. Cuenta", "Periodo", "Fecha_Vencimiento", "Total_a_Pagar", "Pago_Minimo");
          Console.WriteLine("====================================================================================================");

          LiquidacionTarjeta(numCuenta);

          Console.WriteLine("\nPresione una tecla para volver al menú...");
          Console.ReadKey();
        }


        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================

        static void EmisionTarjeta(){
            string nombre, apellido, email, banco_emisor, tipo_documento, fecha_nacimiento, usuario, query_sql, query_sql1, cadenaConexion,
                   nro_pasaporte = "", password, nro_tarjeta;
            int nro_dni = 0;
            DateTime fecha_emision = DateTime.Now;
            Boolean fecha_nacimiento_valida = false, nro_dni_valido = false, nro_pasaporte_valido = false;

            Console.Write("Ingrese Nombre del Cliente: ");
            nombre = LeerNombreuApellido();
            Console.Write("Ingrese Apellido del Cliente: ");
            apellido = LeerNombreuApellido();

            Console.Write("Ingrese Tipo de Documento (DNI o PASAPORTE): ");
            tipo_documento = LeerTipoDocumento();

            // Se valida en esta seccion el tipo de documento y se solicita su numero en funcion del mismo
            if (tipo_documento == "DNI")
            {
              Console.Write("Ingrese Número de Documento (sin puntos): ");
              while(!nro_dni_valido)
              {
                string input = Console.ReadLine();

                //Valida que el DNI sea un numero de 8 digitos y que no tenga puntos ni letras
                if (Regex.IsMatch(input, @"^[0-9]{8}$"))
                {
                  nro_dni = int.Parse(input);
                  nro_dni_valido = true;
                }
                else
                {
                    Console.Write("Número de Documento inválido. Ingrese nuevamente: ");
                }
              }
            }else if (tipo_documento == "PASAPORTE")
            {
              Console.Write("Ingrese Numero de Documento (alfanumerico): ");
              while(!nro_pasaporte_valido)
              {
                string input = Console.ReadLine().ToUpper();

                //Valida que el pasaporte sea alfanumérico y tenga un formato específico (ejemplo: ABC123456)
                if (Regex.IsMatch(input, @"^[A-Z]{3}[0-9]{6}$"))
                {
                    nro_pasaporte_valido = true;
                    nro_pasaporte = input;
                }
                else
                {
                    Console.Write("Número de Pasaporte inválido. Ingrese nuevamente (Formato: ABC123456): ");
                }
              }
            }
            // Aca finaliza la verificacion con respecto al tipo de documento y su numero correspondiente

            Console.Write("Ingrese Fecha de Nacimiento (YYYY-MM-DD): ");
            fecha_nacimiento = LeerFechaNacimiento();

            Console.Write("Ingrese Email del Cliente: ");
            email = LeerEmail();

            banco_emisor = LeerBancoEmisor();

            nro_tarjeta = GenerarNumeroTarjeta();
            
            usuario = GenerarUsuario(nombre, apellido);
            password = GenerarPassword();
            // Aquí deben insertar los datos en la base de datos (tabla 'usuarios' y 'tarjetas') 

            cadenaConexion = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";

            if (tipo_documento.ToUpper() == "DNI")
            {
              query_sql = "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password, creado_el)" +
                          "VALUES (@documento, @tipo_doc, @nombre, @apellido, @fecha_nacimiento, @email, @usuario, @password, @creado_el)";

              query_sql1 = "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, estado, saldo, dni_titular)" +
                           "VALUES (@nro_tarjeta, @banco_emisor, 'ACTIVA', 0.00, @documento)";

              using (MySqlConnection conexion = new MySqlConnection(cadenaConexion)){
                try{
                  conexion.Open();

                  using (MySqlCommand comando = new MySqlCommand(query_sql, conexion)){
                    comando.Parameters.AddWithValue("@documento", nro_dni);
                    comando.Parameters.AddWithValue("@tipo_doc", tipo_documento);
                    comando.Parameters.AddWithValue("@nombre", nombre);
                    comando.Parameters.AddWithValue("@apellido", apellido);
                    comando.Parameters.AddWithValue("@fecha_nacimiento", fecha_nacimiento);
                    comando.Parameters.AddWithValue("@email", email);
                    comando.Parameters.AddWithValue("@usuario", usuario);
                    comando.Parameters.AddWithValue("@password", password);
                    comando.Parameters.AddWithValue("@creado_el", fecha_emision);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0){
                      Console.WriteLine("Usuario creado correctamente.");
                    }
                  }
  
                  using (MySqlCommand comando1 = new MySqlCommand(query_sql1, conexion)){
                    comando1.Parameters.AddWithValue("@nro_tarjeta", nro_tarjeta);
                    comando1.Parameters.AddWithValue("@banco_emisor", banco_emisor);
                    comando1.Parameters.AddWithValue("@documento", nro_dni);

                    int filasAfectadas1 = comando1.ExecuteNonQuery();

                    if (filasAfectadas1 > 0){
                      Console.WriteLine("Tarjeta creada correctamente.");
                    }
                  }

              }catch (Exception ex){
                  Console.WriteLine("Error al crear el usuario con dni: " + ex.Message);
                }
              }
            } else if (tipo_documento.ToUpper() == "PASAPORTE"){
              query_sql = "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password, creado_el)" +
                          "VALUES (@nro_pasaporte, @tipo_doc, @nombre, @apellido, @fecha_nacimiento, @email, @usuario, @password, @creado_el)";

              query_sql1 = "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, estado, saldo, dni_titular)" +
                           "VALUES (@nro_tarjeta, @banco_emisor, 'ACTIVA', 0.00, @nro_pasaporte)";

              using (MySqlConnection conexion = new MySqlConnection(cadenaConexion)){
                try{
                  conexion.Open();

                  using (MySqlCommand comando = new MySqlCommand(query_sql, conexion)){
                    comando.Parameters.AddWithValue("@nro_pasaporte", nro_pasaporte);
                    comando.Parameters.AddWithValue("@tipo_doc", tipo_documento);
                    comando.Parameters.AddWithValue("@nombre", nombre);
                    comando.Parameters.AddWithValue("@apellido", apellido);
                    comando.Parameters.AddWithValue("@fecha_nacimiento", fecha_nacimiento);
                    comando.Parameters.AddWithValue("@email", email);
                    comando.Parameters.AddWithValue("@usuario", usuario);
                    comando.Parameters.AddWithValue("@password", password);
                    comando.Parameters.AddWithValue("@creado_el", fecha_emision);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if(filasAfectadas > 0){
                      Console.WriteLine("Usuario creado correctamente.");
                    }
                  }

                  using (MySqlCommand comando1 = new MySqlCommand(query_sql1, conexion)){
                    comando1.Parameters.AddWithValue("@nro_tarjeta", nro_tarjeta);
                    comando1.Parameters.AddWithValue("@banco_emisor", banco_emisor);
                    comando1.Parameters.AddWithValue("@nro_pasaporte", nro_pasaporte);

                    int filasAfectadas1 = comando1.ExecuteNonQuery();

                    if(filasAfectadas1 > 0){
                      Console.WriteLine("Tarjeta creada correctamente.");
                    }
                  }

                }catch (Exception ex){
                  Console.WriteLine("Error al crear el usuario con pasaporte: " + ex.Message);
                }
              }
            }
        }

        static string LeerNombreuApellido(){
          string input;
          bool valido = false;

          do{
            input = Console.ReadLine();

            if (Regex.IsMatch(input, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")){
              input = input.Trim(); // Elimina espacios al inicio y al final
              input = char.ToUpper(input[0]) + input.Substring(1).ToLower(); // Capitaliza la primera letra
              valido = true;
            } else {
              Console.Write("Nombre o Apellido invalido. Ingrese nuevamente (solo letras y espacios): ");
            }
          }while(!valido);

          return input;
        }

        static string LeerTipoDocumento(){
          string input;
          bool valido = false;

          do{
            input = Console.ReadLine().ToUpper();

            if (input == "DNI" || input == "PASAPORTE"){
              input = input.Trim(); // Elimina espacios al inicio y al final
              valido = true;
            }else{
              Console.Write("Tipo de documento invalido. Ingrese nuevamente (DNI o PASAPORTE): ");
            }
          }while(!valido);

          return input;
        }

        static string LeerFechaNacimiento(){
          string fecha_nacimiento = "";
          bool fecha_nacimiento_valida = false;

          while(!fecha_nacimiento_valida){
            fecha_nacimiento = Console.ReadLine();

            if (DateTime.TryParseExact(
                  fecha_nacimiento, 
                  "yyyy-MM-dd",
                  null,
                  System.Globalization.DateTimeStyles.None,
                  out DateTime fechaValidada)) {
                fecha_nacimiento_valida = true;

            // System.Globalization.DateTimeStyles.None sirve para que no se realicen
            // ajustes de zona horaria ni de calendario al analizar la fecha.

            }else {
                Console.Write("Fecha invalida u formato invalido. Ingrese nuevamente (YYYY-MM-DD): ");
            }
          }

          return fecha_nacimiento;
        }

        static string LeerEmail(){
          string input;
          bool valido = false;

          do{
            input = Console.ReadLine();

            if (Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")){
              input = input.Trim(); // Elimina espacios al inicio y al final
              valido = true;
            }else{
              Console.Write("Email invalido. Ingrese nuevamente: ");
            }
          }while(!valido);

          return input;
        }

        static string LeerBancoEmisor(){
          string input;
          bool valido = false;
          string[] Bancos = { "Banco Nación", "Banco Provincia", "Banco Ciudad", "Banco Galicia", "Banco Santander", "Banco Macro", "Banco Patagonia"                                , "Banco Comafi" };

          Console.Write("Banco emisor permitidos: ");

          for (int i = 0; i < Bancos.Length; i++){
            Console.Write("\n" + Bancos[i]);
          }

          do{
            Console.Write("Ingrese Banco Emisor deseado: ");
            input = Console.ReadLine();

            if (Regex.IsMatch(input, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")){
              input = input.Trim(); // Elimina espacios al inicio y al final
              input = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower()); // Capitaliza la primera letra de cada palabra

              if(Bancos.Contains(input)){
                valido = true;
              }else{
                Console.Write("Banco Emisor invalido. Ingrese nuevamente (solo letras y espacio y que sean bancos que funcionen en Argentina): ");
              }
            }else{
              Console.Write("No se enconetro dicho Banco, Reingrese (solo letras y espacio y que sean bancos que funcionen en Argentina): ");
            }
            }while(!valido);

          return input;
        }

        static string GenerarNumeroTarjeta(){
          Random random = new Random();
          string nro_tarjeta =
            $"{random.Next(1000, 9999)}" + // 4 dígitos
            $"{random.Next(1000, 9999)}" + // 4 dígitos
            $"{random.Next(1000, 9999)}" + // 4 dígitos
            $"{random.Next(1000, 9999)}";  // 4 dígitos
          return nro_tarjeta; // Ejemplo de número de tarjeta
        }

        static string GenerarUsuario(string nombre, string apellido){
          string usuario = nombre.Substring(0).ToLower() + apellido.Substring(0, 3).ToLower() + new Random().Next(0, 100);

          return usuario;
        }

        static string GenerarPassword(){
          Random random = new Random();
          string password = 
            $"{random.Next(1000, 9999)}" + // 4 dígitos
            $"{random.Next(1000, 9999)}" + // 4 dígitos
            $"{random.Next(1000, 9999)}"; // 4 dígitos
          return password;
        }

        static void ObtenerYMostrarTarjetas() {
          string cadenaConexion = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";
          string query_sql = "SELECT * FROM tarjetas";

          MySqlCommand comando = null;
          MySqlDataReader reader = null;

          using (MySqlConnection conexion = new MySqlConnection(cadenaConexion)){
            try{
              conexion.Open();
              comando = new MySqlCommand(query_sql, conexion);

              reader = comando.ExecuteReader();

              while (reader.Read()){
                string num_cuenta = reader["num_cuenta"].ToString() ?? "";
                string numero_tarjeta = reader["numero_tarjeta"].ToString() ?? "";
                string banco_emisor = reader["banco_emisor"].ToString() ?? "";
                string dni_titular = reader["dni_titular"].ToString() ?? "";

                Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", num_cuenta, numero_tarjeta, banco_emisor, dni_titular);
              }

            } catch (Exception ex) {
              Console.Write("Error al obtener las tarjetas: " + ex.Message);

            } finally {
              reader?.Close(); // Cierro manualmente el reader para liberar recursos
              reader?.Dispose(); // Cierro manualmente el reader para liberar recursos
              comando?.Dispose(); // Cierro manualmente el comando para liberar recursos
            }
          }
        }

        static void MostrarDetalleCompleto(int cuenta) {
          string cadenaConexion = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";
          string query_sql = "SELECT u.nombre, u.apellido, u.email, u.documento, t.numero_tarjeta," +
                             "t.banco_emisor, t.saldo FROM usuarios u JOIN tarjetas t ON " +
                             "u.documento = t.dni_titular WHERE t.num_cuenta = @cuenta";

          MySqlCommand comando = null;
          MySqlDataReader reader = null;

          using (MySqlConnection conexion = new MySqlConnection(cadenaConexion)){
            try{
              conexion.Open();

              comando = new MySqlCommand(query_sql, conexion);
              comando.Parameters.AddWithValue("@cuenta", cuenta);

              reader = comando.ExecuteReader();

              while (reader.Read()){
                string nombre = reader["nombre"].ToString() ?? "";
                string apellido = reader["apellido"].ToString() ?? "";
                string email = reader["email"].ToString() ?? "";
                string dni_titular = reader["documento"].ToString() ?? "";
                string numero_tarjeta = reader["numero_tarjeta"].ToString() ?? "";
                string banco_emisor = reader["banco_emisor"].ToString() ?? "";
                string saldo = reader["saldo"].ToString() ?? "";

                Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-11} {4,-16} {5, -15} {6, -10}", nombre, apellido, email, dni_titular, numero_tarjeta, banco_emisor, saldo);
              }
            } catch (Exception ex) {
              Console.Write("Error al obtener el detalle de la tarjeta: " + ex.Message);
            } finally {
              reader?.Close(); // Cierro manualmente el reader para liberar recursos
              reader?.Dispose(); // Cierro manualmente el reader para liberar recursos
              comando?.Dispose(); // Cierro manualmente el comando para liberar recursos
            }
          }

        }

        static bool DarDeBajaTarjeta(int cuenta) {
          string cadenaConexion = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";
          string query_sql = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";

          MySqlCommand comando = null;

          using (MySqlConnection conexion = new MySqlConnection(cadenaConexion)){
            try {
              conexion.Open();

              comando = new MySqlCommand(query_sql, conexion);
              comando.Parameters.AddWithValue("@cuenta", cuenta);

              int filasAfectadas = comando.ExecuteNonQuery();

              if (filasAfectadas > 0){
                Console.Write("Tarjeta eliminada correctamente.");
                return true;
              } else {
                return false;
              }

             } catch (Exception ex) {
               Console.Write("Error al eliminar la tarjeta: " + ex.Message);
               return false;

            } finally {
              comando?.Dispose(); // Cierro manualmente el comando para liberar recursos
            }
          }
        }

        static void LiquidacionTarjeta(int cuenta){
          string cadenaConexion = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";
          string query_sql = "SELECT * FROM liquidaciones WHERE num_cuenta = @cuenta";

          MySqlCommand comando = null;
          MySqlDataReader reader = null;

          using (MySqlConnection conexion = new MySqlConnection(cadenaConexion)){
            try {
              conexion.Open();

              comando = new MySqlCommand(query_sql, conexion);
              comando.Parameters.AddWithValue("@cuenta", cuenta);

              reader = comando.ExecuteReader();

              while (reader.Read()){
                string id_liquidacion = reader["id_liquidacion"].ToString() ?? "";
                string num_cuenta = reader["num_cuenta"].ToString() ?? "";
                string periodo = reader["periodo"].ToString() ?? "";
                string fecha_vencimiento = reader["fecha_vencimiento"].ToString() ?? "";
                string total_a_pagar = reader["total_a_pagar"].ToString() ?? "";
                string pago_minimo = reader["pago_minimo"].ToString() ?? "";

                Console.WriteLine("{0, -12} {1, -18} {2, -20} {3, -5} {4, -13} {5, -10}", id_liquidacion, num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo);
              }

            } catch (Exception ex) {
              Console.Write("Error al obtener la liquidacion de la tarjeta: " + ex.Message);

            } finally {
              reader?.Close(); // Cierro manualmente el reader para liberar recursos
              reader?.Dispose(); // Cierro manualmente el reader para liberar recursos
              comando?.Dispose(); // Cierro manualmente el comando para liberar recursos
            }
          }
        }
      }
}
