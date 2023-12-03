namespace Wavplay.Models
{
    public class _NHz
    {
        public double FrHz { get; set; }
        public double SrHz { get; set; }
        public double ToHz { get; set; }
        public int GTR { get; set; }
        public double DGTR { get; set; }
        public string Nt { get; set; }
        public _NHz(double FrHz, double SrHz, double ToHz, int GTR, double DGTR, string Nt)
        {
            this.FrHz = FrHz;
            this.SrHz = SrHz;
            this.ToHz = ToHz;
            this.GTR = GTR;
            this.DGTR = DGTR;
            this.Nt = Nt;
        }
    }
}
