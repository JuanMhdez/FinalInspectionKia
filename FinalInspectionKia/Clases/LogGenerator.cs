using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalInspectionKia.Clases
{
    class LogGenerator
    {

        // Generar log
        public void generarlog(string log)
        {

            // Obtener la carpeta donde se ejecuta el programa
            string rutaCarpeta = AppDomain.CurrentDomain.BaseDirectory;
            // Definir el nombre del archivo log
            string nombreArchivoLog = "log.txt";
            // Combinar ruta con el nombre del archivo
            string rutaArchivoLog = Path.Combine(rutaCarpeta, nombreArchivoLog);

            // Crear o añadir texto al archivo log
            using (StreamWriter writer = new StreamWriter(rutaArchivoLog, append: true))
            {
                writer.WriteLine($"{DateTime.Now}: {log}");
            }
        }
    }
}
