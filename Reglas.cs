using System.Windows.Forms;

namespace Juego_Hotel
{
    public partial class Reglas : Form
    {
        public Reglas()
        {
            InitializeComponent();
        }

        private void Reglas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Owner == null)
                return;
            if (Owner is Principal)
                (Owner as Principal).bNormas.Enabled = true;
        }
    }
}
