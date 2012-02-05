using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Juego_Hotel
{
    class ComboItemImagen
    {
        private int _imageIndex;
        private string _etiqueta;

        public string Etiqueta
        {
            get { return _etiqueta; }
            set { _etiqueta = value; }
        }

        public int ImageIndex
        {
            get { return _imageIndex; }
            set { _imageIndex = value; }
        }

        public ComboItemImagen(string etiqueta, int imageIndex)
        {
            this.Etiqueta = etiqueta;
            this.ImageIndex = imageIndex;
        }

        public override string ToString()
        {
            return Etiqueta;
        }
    }
}
