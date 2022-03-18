using Biblioteca_InterfazHouston.Data;
using Biblioteca_InterfazHouston.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazHouston.Procesos
{
    public class Main
    {
        Encriptacion encriptacion;
        FuncionesBD bd;

        bool envio_realizado;
        string envio_mes;
        string envio_anio;
        string envio_diainicio;
        string envio_diafin;
        string as400_dns;
        string as400_server;
        string as400_libreria;
        string as400_tipoarchivo;
        string as400_archdtf;
        string as400_dirsave;
        string as400_dirsavepc;
        string as400_dirhouston;
        string as400_pathdatos;
        string as400_pathtransfer;
        string conexion_usr;
        string conexion_pswd;
        string iniciales_inicial;

        bool paso1, paso2;
        DateTime hoy;
    


        public Main()
        {
            encriptacion = new Encriptacion();


            this.SetParametrosIniciales();

            if(this.envio_realizado == false)
            {
                if (paso1)
                {
                    this.DescargaAS400();
                }
                if (paso2)
                {
                    this.TransferenciaArchivos();
                }
            }

        }

        private void TransferenciaArchivos()
        {
            try
            {
                if (Int32.Parse(hoy.ToString("dd")) >= Int32.Parse(this.envio_diainicio) && Int32.Parse(hoy.ToString("dd")) <= Int32.Parse(this.envio_diafin))
                {

                    this.as400_tipoarchivo = this.as400_tipoarchivo.Replace("yyMM", RegresaFechaArch(hoy) + ".txt");

                    string ruta_origen = this.as400_pathdatos + this.as400_tipoarchivo;
                    string ruta_copiado = this.as400_pathtransfer + this.as400_tipoarchivo;
                    if (File.Exists(ruta_origen))
                    {
                        //Shell("xcopy.exe", new string[] { ruta_origen, ruta_copiado});
                        File.Copy(ruta_origen, ruta_copiado);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }
        }

        private void DescargaAS400()
        {
            try
            {
               
                
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }
        }

        /// <summary>
        /// Establece los parametros inciales para funcion de la aplicacion
        /// </summary>
        public void SetParametrosIniciales()
        {

            this.envio_realizado = (Funcion.getValueAppConfig("realizado", "ENVIO") == "1") ? true : false;
            this.envio_mes =  Funcion.getValueAppConfig("mes", "ENVIO");
            this.envio_anio =  Funcion.getValueAppConfig("anio", "ENVIO");
            this.envio_diainicio =  Funcion.getValueAppConfig("diainicio", "ENVIO");
            this.envio_diafin =  Funcion.getValueAppConfig("diafin", "ENVIO");
            this.as400_dns =  Funcion.getValueAppConfig("dns", "AS400");
            this.as400_server =  Funcion.getValueAppConfig("server", "AS400");
            this.as400_libreria =  Funcion.getValueAppConfig("libreria", "AS400");
            this.as400_tipoarchivo =  Funcion.getValueAppConfig("tipoarchivo", "AS400");
            this.as400_archdtf =  Funcion.getValueAppConfig("archdtf", "AS400");
            this.as400_dirsave =  Funcion.getValueAppConfig("dirsave", "AS400");
            this.as400_dirsavepc =  Funcion.getValueAppConfig("dirsavepc", "AS400");
            this.as400_dirhouston =  Funcion.getValueAppConfig("dirhouston", "AS400");
            this.as400_pathdatos =  Funcion.getValueAppConfig("pathdatos", "AS400");
            this.as400_pathtransfer =  Funcion.getValueAppConfig("pathtransfer", "AS400");
            this.conexion_usr =  encriptacion.Decrypt(Funcion.getValueAppConfig("usuario", "CONEXION"));
            this.conexion_pswd = encriptacion.Decrypt(Funcion.getValueAppConfig("contrasenia", "CONEXION"));
            this.iniciales_inicial =  Funcion.getValueAppConfig("Inicial", "INICIALES");

            this.paso1 = (Funcion.getValueAppConfig("1", "paso") == "1") ? true : false;
            this.paso2 = (Funcion.getValueAppConfig("2", "paso") == "1") ? true : false;

            hoy = DateTime.Now;
        }

        /// <summary>
        /// Establece los parametros iniciales para la conexion a la base de datos e inicializa
        /// instancia de conexion
        /// </summary>
        /// <returns>Resultado verdadero o falso de la ocnexion</returns>
        public bool ConectDB()
        {
            bool ConectDB = false;
            string section = "conexion";
            try
            {
                string a = Funcion.getValueAppConfig("DBCata", section);
                string gsCataDB = encriptacion.Decrypt(a);
                string gsDSNDB = encriptacion.Decrypt(Funcion.getValueAppConfig("DBDSN", section));
                string gsSrvr = encriptacion.Decrypt(Funcion.getValueAppConfig("DBSrvr", section));
                string gsUserDB = encriptacion.Decrypt(Funcion.getValueAppConfig("DBUser", section));
                string gsPswdDB = encriptacion.Decrypt(Funcion.getValueAppConfig("DBPswd", section));
                string gsNameDB = encriptacion.Decrypt(Funcion.getValueAppConfig("DBName", section));

               
                string conn_str = $"Data source ={gsSrvr}; uid ={gsUserDB}; PWD ={gsPswdDB}; initial catalog = {gsNameDB}";

                bd = new FuncionesBD(conn_str);

                ConectDB = true;

                return ConectDB;
            }
            catch (Exception ex)
            {
                ConectDB = false;
                Log.Escribe(ex, "Error");

                return ConectDB;
            }

        }

        /// <summary>
        /// Ejecuta un comando en el CMD
        /// </summary>
        /// <param name="cmd">comando</param>
        /// <param name="args">argumentos</param>
        public void Shell(string cmd, string[] args)
        {
            try
            {
                string argumentos = "";
                if(args.Length >= 1)
                {
                    for(int i = 0; i < args.Length; i++)
                    {
                        argumentos += args[i] + " ";
                    }
                }
                Process p = new Process();
                p.EnableRaisingEvents = false;
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = argumentos;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
                p.WaitForExit();

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }
        }

        /// <summary>
        /// En caso de que sea Enero, cambia la fecha a un mes antes
        /// </summary>
        /// <param name="fecha">fecha a validar</param>
        /// <returns>Fecha con un mes antes</returns>
        private static string RegresaFechaArch(DateTime fecha)
        {
            try
            {
                int mes;
                int year;

                string month;
                string ano;

                mes = Int32.Parse(fecha.ToString("MM"));
                year = Int32.Parse(fecha.ToString("yyyy"));

                if (mes == 1)
                {
                    ano = (year - 1).ToString();
                    month = "12";
                }
                else
                {
                    ano = year.ToString();
                    month = (mes - 1).ToString("00");
                }

                string fech = DateTime.Parse(ano + "/" + month + "/" + Funcion.getValueAppConfig("primerdia", "ENVIO")).ToString("yyMM");

                return fech;

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return null;
            }
        }

        /// <summary>
        /// Te indica el numero de la dia de la semana despues de indicar cual es el primer dia de la semana
        /// </summary>
        /// <param name="fecha">fecha de la que se quiere saber que numero de la semana es</param>
        /// <param name="str_primerdiasemana">primer dia de la semana a establecer</param>
        /// <returns></returns>
        private static string DiaSemana(DateTime fecha, string str_primerdiasemana)
        {
            int numero_dia = 0;
            try
            {
                DateTime nuevaFecha;
                String diasemana;

                DateTime bandera_fecha = DateTime.Now;

                for (int i = 0; i <= 7; i++)
                {
                    nuevaFecha = fecha.AddDays(-i);
                    diasemana = nuevaFecha.ToString("dddd");

                    if (diasemana == str_primerdiasemana.ToLower().Trim())
                    {
                        bandera_fecha = nuevaFecha;
                        break;
                    }
                }
                int contador = 1;
                for (int j = 0; j <= 7; j++)
                {
                    if (bandera_fecha.AddDays(j).ToString("ddMMyyyy") == fecha.ToString("ddMMyyyy"))
                    {
                        numero_dia = contador;
                        break;
                    }
                    contador++;
                }

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }
            return numero_dia.ToString();
        }


        public static string FechaAnterior(DateTime fecha)
        {
            try
            {
                int mes, year;
                string month, ano;

                mes = Int32.Parse(fecha.ToString("MM"));
                year = Int32.Parse(fecha.ToString("yyyy"));

                if (mes == 1)
                {
                    ano = (year - 1).ToString();
                    month = "12";
                }
                else
                {
                    ano = year.ToString();
                    month = (mes - 1).ToString();
                }

                string fech;
                return fech = new DateTime(Int32.Parse(ano), Int32.Parse(month), Int32.Parse(Funcion.getValueAppConfig("primerdiadelmes", "CargaEnvioA"))).ToString("dd/MM/yyyy");


            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return null;
            }

        }
    }
}
