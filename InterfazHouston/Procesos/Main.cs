﻿using Biblioteca_InterfazHouston.Data;
using Biblioteca_InterfazHouston.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazHouston.Procesos
{
    public class Main
    {
        Encriptacion encriptacion;
        FuncionesBD bd;

        bool envio_recibido;
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
        string conexion_usr;
        string conexion_pswd;
        string iniciales_inicial;
    


        public Main()
        {
            encriptacion = new Encriptacion();


            this.SetParametrosIniciales();

        }

        public void SetParametrosIniciales()
        {

            this.envio_recibido = (Funcion.getValueAppConfig("realizado", "ENVIO") == "1") ? true : false;
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
            this.conexion_usr =  encriptacion.Decrypt(Funcion.getValueAppConfig("usuario", "CONEXION"));
            this.conexion_pswd = encriptacion.Decrypt(Funcion.getValueAppConfig("contrasenia", "CONEXION"));
            this.iniciales_inicial =  Funcion.getValueAppConfig("Inicial", "INICIALES");
        }

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
    }
}