using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalInspectionKia
{
    public partial class Mensaje: Form
    {
        public Mensaje(string mensaje)
        {
            InitializeComponent();
            label1.Text = mensaje;
        }


    }
}
