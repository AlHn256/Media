using System.Collections.Generic;
using System.Linq;

namespace Wavplay.Models
{
    public class ScanStat
    {
        public double SrRad { get; set; }
        public int Count { get; set; }
        public List<int> ListRad { get; set; }
        public ScanStat(List<int> ListRad)
        {
            this.ListRad = ListRad;
            Check();
        }

        public int ListCount() { return this.ListRad.Count(); }
        public int GetListElm(int NEl) { return this.ListRad[NEl]; }
        public void AddListElm(int El) { this.ListRad.Add(El); Check(); }

        private void Check()
        {
            this.Count = this.ListRad.Count();
            if (this.Count != 0)
            {
                double Db = 0;
                for (int i = 0; i < this.Count; i++) Db += this.ListRad[i];
                this.SrRad = Db / this.Count;
            }
            else
            {
                this.Count = 0;
                this.SrRad = 0;
            }
        }
    }
}
