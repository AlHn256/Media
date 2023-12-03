using System;

namespace Wavplay.Models
{
    public class WavList
    {
        public Int16 UpLf { get; set; }
        public Int16 DnRt { get; set; }
        public bool FRL { get; set; }
        public bool FDR { get; set; }
        public WavList(Int16 upLf, Int16 dnRt, bool fRL, bool fDR)
        {
            UpLf = upLf;
            DnRt = dnRt;
            FRL = fRL;
            FDR = fDR;
        }

        public WavList(Int16 upLf, Int16 dnRt)
        {
            UpLf = upLf;
            DnRt = dnRt;
            FRL = false;
            FDR = false;
        }
    }
}
