using System.Collections.Generic;
using System.Linq;

namespace Wavplay.Models
{
    public class HzConverter
    {
        static double[] NHz = { 1975.5, 2093, 2217.4, 2349.2, 2489, 2637, 2793.8, 2960, 3136, 3332.4, 3440, 3729.2, 3951, 4186 };
        public static List<_NHz> NHzList { get; set; }
        public static string Text { get; set; }
        static HzConverter()
        {
            double D = 0, I = 0;
            int i, j = 0, k = 0, k6 = -1, k5 = -1, k4 = -1, k3 = -1, k2 = -1, k1 = -1;
            string txt = "";
            NHzList = new List<_NHz>();
            for (i = NHz.Count() - 1; i > -1; i--)
            {
                if (i < NHz.Count() - 2)
                {
                    I = 12 - i;
                    D = 1 + I / 100;
                    _NHz el = new _NHz((NHz[i] + NHz[i + 1]) / 2, NHz[i + 1], (NHz[i + 1] + NHz[i + 2]) / 2, 12 - i, D, "TXT");
                    NHzList.Add(el);
                }
            }

            for (i = 0; i < 96; i++)
            {
                j++;
                I = j;
                D = i / 12 + 2 + I / 100;

                if (NHzList[i].ToHz < 165.2) { txt = " |6." + --k + "| "; }
                _NHz el = new _NHz(NHzList[i].FrHz / 2, NHzList[i].SrHz / 2, NHzList[i].ToHz / 2, i + 13, D, txt);
                NHzList.Add(el);
                if (j == 12) j = 0;
            }

            for (i = NHzList.Count() - 1; i > -1; i--)
            {
                txt = " |";
                if (NHzList[i].FrHz < 82.6 && NHzList[i].ToHz > 82.6) k6 = 0;
                if (k1 > -1) { txt += "1." + k1 + "| "; k1++; }
                if (k2 > -1 && k2 < 15) { txt += "2." + k2 + "| "; k2++; if (k2 == 5) k1 = 0; }
                if (k3 > -1 && k3 < 15) { txt += "3." + k3 + "| "; k3++; if (k3 == 4) k2 = 0; }
                if (k4 > -1 && k4 < 15) { txt += "4." + k4 + "| "; k4++; if (k4 == 5) k3 = 0; }
                if (k5 > -1 && k5 < 15) { txt += "5." + k5 + "| "; k5++; if (k5 == 5) k4 = 0; }
                if (k6 > -1 && k6 < 15) { txt += "6." + k6; k6++; if (k6 == 5) k5 = 0; }
                if (k6 > -1) NHzList[i].Nt = txt;
            }
            Text = txt;
        }

        public string NHzShow()
        {
            Text = string.Empty;
            for (int i = 0; i < NHzList.Count; i++) Text += "\n| " + NHzList[i].GTR + " | " + NHzList[i].DGTR + " | " + NHzList[i].FrHz + " | " + NHzList[i].SrHz + " | " + NHzList[i].ToHz + " | " + NHzList[i].Nt + " | ";
            return Text;
        }

        public string NHzShowMini()
        {
            Text = "\n";
            int x = 0, y = 0;
            for (int i = 0; i < NHzList.Count; i++)
            {
                y++;
                x = (int)NHzList[i].SrHz;
                Text += "| " + x;
                if (y == 12) { y = 0; Text += " |\n"; }
            }
            return Text;
        }

        public string GetHzFromScanList(List<ScanList> ScanList)
        {
            if (ScanList.Count != 0)
            {
                int hz = -1, k = 0, St = 0, Fn = 0;
                double tmp = 0;
                for (int i = 0; i < ScanList.Count; i++)
                {
                    if (ScanList[i].Fl1 == true)
                    {
                        k++;
                        if (Fn != 0) ScanList[i].Rad = i - Fn;
                        else St = i;
                        Fn = i;
                    }
                }
                tmp = Fn - St;

                if (k > 3)
                {
                    tmp = tmp / (k - 1);
                    hz = (int)(44100 / tmp);
                }
                return hz.ToString() + " Hz";
            }
            else return "ScanList is Empty";
        }

        public double GetHzByNt(string Nt)
        {
            double Hz=-1;
            var NHz = NHzList.Where(x => x.Nt.IndexOf(Nt) != -1).FirstOrDefault();
            if(NHz!=null)
            {
                Hz = NHz.SrHz;
                var asd = HzInfo(Hz);
            }
            return Hz;
        }

        public string GetNtFromHz(double Hz)
        {
            string Nt = string.Empty;
            foreach(var elem in NHzList)
            {
                var Fr = elem.FrHz;
                var To = elem.ToHz;
                if(Fr< Hz && Hz < To)
                {
                    Nt = elem.Nt;
                    break;
                }
            }
            return Nt;
        }

        public string GetNtFromHz(string strHz)
        {
            double Hz = 0;
            //bool fl = double.TryParse(strHz, out Hz);
            
            if(double.TryParse(strHz, out Hz))
            {
                string Nt = string.Empty;
                foreach (var elem in NHzList)
                {
                    var Fr = elem.FrHz;
                    var To = elem.ToHz;
                    if (Fr < Hz && Hz < To)
                    {
                        Nt = elem.Nt;
                        break;
                    }
                }
                return Nt;
            }
            else return "Errr!!!";
        }

        public string HzInfo(double Hz)
        {
            string txt = " - ";

            for (int i = 0; i < NHzList.Count; i++) { if (NHzList[i].FrHz < Hz && NHzList[i].ToHz > Hz) txt = NHzList[i].Nt; }
            return txt;
        }
    }
}