using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalInspectionKia.INIFILES
{
    class Configuracion
    {

        public string opcode { get; set; }

       // Configuracion config;

        /*
         public Configuracion(string rutaConfig)
        {
            var ini = new INIFILE(rutaConfig);
            this.opcode = ini.Read("SETTINGS", "OPCODE");
            // Timeout = int.TryParse(ini.Read("General", "Timeout", "10"), out int t) ? t : 10;
        }
         
         */
        public void llamarConfig()
        {

            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            Console.WriteLine("Ruta completa esperada: " + ruta);
            Console.WriteLine("¿Existe el archivo?: " + System.IO.File.Exists(ruta));

          //  config = new Configuracion(ruta);

            var ini = new INIFILE(ruta);
            this.opcode = ini.Read("SETTINGS", "OPCODE");

        }
    }
}
