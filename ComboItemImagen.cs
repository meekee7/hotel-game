namespace Juego_Hotel
{
    class ComboItemImagen
    {
        public string Etiqueta { get; set; }

        public int ImageIndex { get; set; }

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
