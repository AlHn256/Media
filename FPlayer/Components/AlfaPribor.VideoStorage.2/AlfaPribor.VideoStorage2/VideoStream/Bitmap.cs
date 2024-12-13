using System.IO;

namespace AlfaPribor.VideoStorage2
{
    internal class Bitmap
    {
        private MemoryStream ms;

        public Bitmap(MemoryStream ms)
        {
            this.ms = ms;
        }
    }
}