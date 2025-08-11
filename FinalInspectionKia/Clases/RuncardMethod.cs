using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FinalInspectionKia.INIFILES;
using FinalInspectionKia.Runcard;

namespace FinalInspectionKia.Clases
{
    public class RuncardMethod
    {

        LogGenerator log = new LogGenerator();

        runcard_wsdlPortTypeClient cliente = new runcard_wsdlPortTypeClient("runcard_wsdlPort");

        // Config
        Configuracion config = new Configuracion();


        public RuncardMethod()
        {
            config.llamarConfig();
        }


        public string ValidarUsuario(string usuario)
        {
            string retro = string.Empty;
            int error;
            string msg = string.Empty;


            var validar = cliente.checkUserStatus(usuario,out error,out msg);

            if (error == 0)
            {

                Console.WriteLine($"Usuario: {validar.username} {validar.firstname} {validar.lastname}");

            }
            else
            {
                Console.WriteLine($"No se encontro ningun usuario {msg}");
                log.generarlog($"No se encontro ningun usuario {msg}");
                retro = $"No se encontro ningun usuario {msg}";
            }

            return retro;
        }


        public string ValidarSerial(string serial, out string retro) {


            int error;
            string msg = string.Empty;
            string noParte = string.Empty;


            var status = cliente.getUnitStatus(serial,out error,out msg);

            if (error == 0)
            {

                if ((status.status == "IN QUEUE" || status.status == "IN PROGRESS") && status.opcode == config.opcode)
                {

                    noParte = status.partnum;
                    retro = string.Empty;
                }
                else
                {
                    retro = $"Unidad en fuera de flujo {status.status}, {status.opcode}";
                    log.generarlog($"Unidad en fuera de flujo {status.status}, {status.opcode}");
                    Console.WriteLine($"Unidad en fuera de flujo {status.status}, {status.opcode}");
                }

            }
            else
            {
                retro = $"Error al consultar el serial {msg}";
                log.generarlog($"Error al consultar el serial {msg}");
               // MessageBox.Show("Error al consultar el serial");
            }

            return noParte;
        
        }


        public void Transaccion(string serial,string movimiento, string defect)
        {

            int error;
            string msg = string.Empty;

            // warehouse
            string houseloc = "SCRAP";
            string housebin = "SCRAP";
            // Recolectamos unicamente elcodigo de defecto
            string defectCode = string.Empty;

            // Recortamos la cadena
            if (defect != string.Empty)
            {
                defectCode = defect.Substring(0,7);
            }
            Console.WriteLine($"Cadena recortada: {defectCode}");
            

            var status = cliente.getUnitStatus(serial, out error, out msg);

            if (error == 0)
            {

                if (movimiento == "MOVE")
                {
                    houseloc = status.warehouseloc;
                    housebin = status.warehousebin;
                }

                transactionItem request = new transactionItem() {
                
                    username = "ftest",
                    transaction = movimiento,
                    workorder = status.workorder,
                    serial = status.serial,
                    trans_qty = 1,
                    seqnum = status.seqnum,
                    opcode = status.opcode,
                    warehouseloc = houseloc,
                    warehousebin = housebin,
                    defect_code = defectCode,
                    



                };

                dataItem[] inputData = new dataItem[] { };
                bomItem[] bomData = new bomItem[] { };


                var transact = cliente.transactUnit(request,inputData,bomData,out msg);


                Console.WriteLine($"Resultado: {msg}");

            }
            else
            {
                Console.WriteLine($"Error: {msg}");
                log.generarlog($"Error: {msg}");
                return;
            }          

        }


        public Dictionary<string,string> ObtenerListaDefectos()
        {
            Dictionary<string, string> diccionario = new Dictionary<string, string>();

            int error;
            string msg = string.Empty;
            string op = "Q200";

            var listaDefectos = cliente.fetchDefectCodeList(op,out error,out msg);

            if (error == 0)
            {
                for (int i = 0; i < listaDefectos.Length; i++)
                {
                    diccionario.Add(listaDefectos[i].defect_code, listaDefectos[i].description);
                }
                
            }
            else
            {
                Console.WriteLine("No se encontro ninguna lista de defectos.");
                log.generarlog("No se encontro ninguna lista de defectos.");
            }

            return diccionario;
        }



    }
}
