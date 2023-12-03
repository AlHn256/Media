namespace Wavplay.Models
{
    public class ScanList
    {
        public int El { get; set; }
        public int Rad { get; set; }
        public bool Fl1 { get; set; }
        public bool Fl2 { get; set; }
        public bool Fl3 { get; set; }
        public ScanList(int El, int Rad, bool Fl1, bool Fl2, bool Fl3)
        {
            this.El = El;
            this.Rad = Rad;
            this.Fl1 = Fl1;
            this.Fl2 = Fl2;
            this.Fl3 = Fl3;
        }
    }
}
