using FinalInspectionKia.Clases;
using FinalInspectionKia.INIFILES;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FinalInspectionKia
{
    public partial class Form1: Form
    {

        // Instancia de RuncardMethod
        RuncardMethod runcardMethod = new RuncardMethod();

        // EWO-99EQ
        // Etiqueta de aprobacion P8818000

        string usuario = string.Empty;
        // Retro
        string retro = string.Empty;
        // Lista de defectos
        Dictionary<string, string> diccionarioDefectos = new Dictionary<string, string>();
        // Log generator
        LogGenerator log = new LogGenerator();
        // Config
        //  Configuracion config;
        Configuracion config = new Configuracion();

        public Form1()
        {
            InitializeComponent();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                //Get App Version
                Version ver = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                lblVersion.Text = ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            config.llamarConfig();

            if (config.opcode == "Q200")
            {
                lblTitulo.Text = "Final Inspection KIA";
                lblOpcode.Text = $"{config.opcode}";
            }
            else if (config.opcode == "S210")
            {
                lblTitulo.Text = "EPC KIA";
                lblOpcode.Text = $"{config.opcode}";

            }
            else
            {
                lblTitulo.Text = "Error config";
                lblOpcode.Text = $"{config.opcode}";

            }


            lblUsuario.Text = Sesion.UsuarioActual;

            // Llenar defectos
            diccionarioDefectos = runcardMethod.ObtenerListaDefectos();

            if (diccionarioDefectos != null)
            {

                foreach (var item in diccionarioDefectos)
                {
                    cbxRechazar.Items.Add($"{item.Key} {item.Value}");

                }

            }
            else
            {
                Console.WriteLine("Error al encontrar la lista de defectos");
                log.generarlog("Error al encontrar la lista de defectos");
                Msg("Error al encontrar la lista de defectos",2);
            }

            
        }


        public void Salir()
        {
            this.Hide();

            Login login = new Login();
            login.ShowDialog();

            if (login.DialogResult == DialogResult.OK)
            {
                this.Show();

                lblUsuario.Text = Sesion.UsuarioActual;
                usuario = Sesion.UsuarioActual;
                Limpiar();
                LimpiarMsg();
                
            }
            else
            {
                Application.Exit();
            }
        }

        private void pictureSalir_Click(object sender, EventArgs e)
        {
            Salir();
        }


        private void txtEtiquetaViajera_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {

                if (txtEtiquetaViajera.Text != string.Empty)
                {

                    /*
                     
                     string serial = string.Empty;

                    // Expresion regular XXXX#251915GK0149

                    bool ok = Regex.IsMatch(txtEtiquetaViajera.Text.Trim(),"^XXXX#[0-9]{6}[A-Z]{2}[0-9]{4}$",RegexOptions.IgnoreCase);

                    if (ok)
                    {
                        Console.WriteLine("Si coincide");
                        serial = txtEtiquetaViajera.Text.Substring(5,12);
                        Console.WriteLine($"Serial: {serial}");
                        
                    }
                    else
                    {
                        Console.WriteLine("No coincide");

                        Msg("Error con serial!!",2);
                        return;
                       
                    }
                     */


                    // Validamos que el serial este en la estacion correcta

                    string ValidacionSerial = runcardMethod.ValidarSerial(txtEtiquetaViajera.Text.Trim(), out retro);

                    if (retro == string.Empty)
                    {
                        // Si es numero de parte 2145930610 se necesita la etiqueta de aprobación
                        if (ValidacionSerial == "2145930610")
                        {
                            txtEtiquetaViajera.Enabled = false;
                            txtAprobacion.Enabled = true;
                            txtAprobacion.Visible = true;
                            lblAprobacion.Visible = true;
                         //   timerAprobacion.Enabled = true;
                            LimpiarMsg();
                            Clipboard.SetText("");

                        }
                        else
                        {
                            PBBad.Visible = true;
                            PBGood.Visible = true;
                            PBGood.Enabled = true;
                            PBBad.Enabled = true;
                            txtEtiquetaViajera.Enabled = false;
                            LimpiarMsg();
                        }                       

                    }
                    else
                    {
                        Console.WriteLine($"Error: {retro}");
                        Msg($"Error: {retro}",2);
                        log.generarlog($"Error: {retro}");
                        txtEtiquetaViajera.Text = string.Empty;
                        return;
                    }

                }
                else
                {
                    Msg("Campo vacio",2);
                    log.generarlog("Campo vacio");
                }

            }

        }


        public bool ValidarLabelAprob(string serial)
        {
            string serialAprobacion = "P8818000";

            if (serial == serialAprobacion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BorrarTxTaprobacion()
        {
            txtAprobacion.Text = string.Empty;
        }


        private void txtAprobacion_KeyDown(object sender, KeyEventArgs e)
        {
            timerAprobacion.Start();
            if (e.KeyCode == Keys.Enter)
            {
                timerAprobacion.Stop();
                
                if (txtAprobacion.Text != string.Empty)
                {
                    bool etiquetaAprobacion = ValidarLabelAprob(txtAprobacion.Text);

                    if (etiquetaAprobacion)
                    {
                        txtAprobacion.Enabled = false;
                        PBGood.Enabled = true;
                        PBBad.Enabled = true;
                        PBBad.Visible = true;
                        PBGood.Visible = true;
                       // timerAprobacion.Enabled = false;

                    }
                    else
                    {
                        Console.WriteLine("El serial no coincide con la etiqueta de aprobacion");
                        Msg("El serial no coincide con la etiqueta de aprobacion",2);
                    }

                    


                }


            }

        }

        private void PBGood_Click(object sender, EventArgs e)
        {
            Transaccion();
        }


        public void Transaccion()
        {


            if (cbxRechazar.Text == string.Empty)
            {
                runcardMethod.Transaccion(txtEtiquetaViajera.Text,"MOVE","");
                Msg("Pieza Completada",1);
                Limpiar();
            }
            else
            {
                runcardMethod.Transaccion(txtEtiquetaViajera.Text,"SCRAP",cbxRechazar.Text);
                Msg("Unidad enviada a SCRAP",2);
                Limpiar();
            }
          
        }

        public void Limpiar()
        {
            txtEtiquetaViajera.Enabled = true;
            txtEtiquetaViajera.Text = string.Empty;

            txtAprobacion.Enabled = false;
            txtAprobacion.Visible = false;
            txtAprobacion.Text = string.Empty;

            PBBad.Visible = false;
            PBGood.Visible = false;

            cbxRechazar.SelectedIndex = -1;
            cbxRechazar.Visible = false;

            btnRechazar.Visible = false;

            lblDefecto.Visible = false;
            lblAprobacion.Visible = false;


        }

        private void PBBad_Click(object sender, EventArgs e)
        {
            cbxRechazar.Enabled = true;
            cbxRechazar.Enabled = true;
            cbxRechazar.Visible = true;
            PBGood.Visible = false;
            lblDefecto.Visible = true;
            
        }

        private void cbxRechazar_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRechazar.Enabled = true;
            btnRechazar.Visible = true;
        }

        private void btnRechazar_Click(object sender, EventArgs e)
        {
            Transaccion();
            

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        public void Msg(string mensaje,int color)
        {
            foreach (Control control in panel1.Controls)
            {
                if (control is Form)
                {
                    control.Dispose();
                }
            }

            Mensaje msg = new Mensaje(mensaje);

            Console.WriteLine($"Color {color}");

            if (color == 1)
            {
                msg.BackColor = System.Drawing.Color.FromArgb(31, 120, 14); // Verde
            }
            else
            {
                msg.BackColor = System.Drawing.Color.FromArgb(173, 24, 24); // Rojo

            }


            msg.TopLevel = false;
            msg.Parent = panel1;
            msg.Size = panel1.ClientSize;
            msg.Show();

            txtEtiquetaViajera.Focus();

        }

        public void LimpiarMsg()
        {
            foreach (Control control in panel1.Controls)
            {
                if (control is Form)
                {
                    control.Dispose();
                }
            }

            Mensaje msg = new Mensaje("");

            msg.BackColor = System.Drawing.Color.FromArgb(229, 225, 225);

            msg.TopLevel = false;
            msg.Parent = panel1;
            msg.Size = panel1.ClientSize;
            msg.Show();

        }

        private void timerAprobacion_Tick(object sender, EventArgs e)
        {
            BorrarTxTaprobacion();
            timerAprobacion.Stop();
        }
    }
}
