using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;
using FinalInspectionKia.Clases;


namespace FinalInspectionKia
{
    public partial class Login: Form
    {
        public Login()
        {
            InitializeComponent();
        }



        public void Ingresar()
        {
            if (txtUsuario.Text != string.Empty)
            {

                RuncardMethod runcard = new RuncardMethod();

                string ingresar = runcard.ValidarUsuario(txtUsuario.Text);

                if (ingresar == string.Empty)
                {
                    Sesion.UsuarioActual = txtUsuario.Text.Trim();
                    this.DialogResult = DialogResult.OK;
                    this.Close();

                }
                else
                {
                    MessageBox.Show("El usuario no tiene permiso de Runcard " +ingresar);
                    return;
                }
              
            }
            else
            {
                MessageBox.Show("No se llenaron todos los campos!");
            }
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {

                Ingresar();
            }

        }

    }
}
