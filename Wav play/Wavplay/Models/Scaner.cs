using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wavplay.Models
{
    public class Scaner
    {
        List<ScanList> ScanList { get; set; }
        List<WavList> WavList { get; set; }
        public List<ScanStat> ScanStat { get; set; }
        public string Text { get; set; }

        public Scaner(List<ScanList> scanList)
        {
            ScanList = scanList;
        }
        public Scaner(List<WavList> WavList)
        {
            ConvertWavListToScanList(WavList);
        }

        public Scaner(List<WavList> wavList, int FScan, int TScan)
        {
            WavList = wavList;
            GetScanListFromData(wavList, FScan, TScan);
        }

        public Scaner(List<WavList> wavList, int FScan, int TScan, int hScrollBarValue, int delta)
        {
            WavList = wavList;
            GetScanListFromData(wavList, FScan, TScan, hScrollBarValue, delta);
        }

        private List<ScanList> GetScanListFromData(List<WavList> WavList, int FScan, int TScan, int hScrollBarValue, int delta)
        {
            if (WavList.Count != 0)
            {
                if (FScan == 0 && TScan == 0 )
                {
                    FScan = hScrollBarValue;
                    TScan = hScrollBarValue + delta;
                }

                GetScanListFromData(WavList,FScan, TScan);
            }
            return ScanList;
        }

        private List<ScanList> ConvertWavListToScanList(List<WavList> WavList)
        {
            if (ScanList == null) ScanList = new List<ScanList>();
            else ScanList.Clear();
            foreach (var elem in WavList) ScanList.Add(new ScanList(elem.UpLf, 0, false, false, false));
            return ScanList;
        }

        private List<ScanList> GetScanListFromData(List<WavList> WavList, int FScan, int TScan)
        {
            if (WavList.Count != 0 && WavList.Count >= TScan)
            {
                WavList = WavList.GetRange(FScan, TScan - FScan);
                Text = "\n FScan " + FScan + " TScan " + TScan;
                ConvertWavListToScanList(WavList);
            }
            return ScanList;
        }

        public List<ScanList> Fscan()
        {
            int n = 0, i = 0, k = 0, k2 = 0, So = 5;
            double max = -32767, Max = -32767;

            for (i = 0; i < ScanList.Count; i++)
            {
                n++;
                if (ScanList[i].El > max) { max = ScanList[i].El; k = i; }
                if (n > So && k != 0 && k + 1 < ScanList.Count && k - 1 > 0)
                {
                    n = 0;
                    if ((ScanList[k + 1].El <= ScanList[k].El) && ScanList[k - 1].El <= ScanList[k].El)
                    {
                        ScanList[k].Fl1 = true;
                        if (k2 != 0) ScanList[k].Rad = k - k2;
                        k2 = k;
                    }
                    if (Max < max) Max = max;
                    max = -32767;
                }
            }
            return ScanList;
        }

        public List<ScanList> ScanMetod1(int Par)
        {
            if (ScanList.Count != 0)
            {
                bool fl = true;
                while (fl != false)
                {
                    double KEl = 0, IEl = 0;
                    int k = 0, i = 0, El = 0;
                    fl = false;
                    for (i = 0; i < ScanList.Count; i++)
                    {
                        if (ScanList[i].Fl1 == true)
                        {
                            IEl = ScanList[i].El;
                            if (k != 0)
                            {
                                if (ScanList[k].El < ScanList[i].El)
                                {
                                    El = (int)(KEl / IEl * 100);
                                    if (!ScanRadTest(El, ScanList[i].Rad, Par))
                                    {
                                        ScanList[k].Fl1 = false;
                                        ScanList[k].Fl2 = true;
                                        fl = true;
                                    }
                                }
                                else
                                {
                                    El = (int)(IEl / KEl * 100);
                                    if (!ScanRadTest(El, ScanList[i].Rad, 0))
                                    {
                                        ScanList[i].Fl1 = false;
                                        ScanList[i].Fl2 = true;
                                        fl = true;
                                    }
                                }
                            }
                            k = i;
                            KEl = IEl;
                        }
                    }
                    ScanRad();
                }
            }
            return ScanList;
        }

        public List<ScanList> ScanMetod2(int Rad)
        {
            bool Fl = true;
            Text = string.Empty;

            while (Fl != false)
            {
                Fl = false;

                int ScR = ScanRad(), i, i2 = 0;
                Text = "ScanRad " + ScR;
                if (Rad != 0) ScR = Rad;
                else ScR = ScR / 20;
                Text += "\nSrR 10% " + ScR;

                if (ScanList.Count != 0)
                {
                    for (i = 0; i < ScanList.Count; i++)
                    {
                        if (ScanList[i].Fl1 == true && ScanList[i].Rad != 0)
                        {
                            if (ScanList[i].Rad < ScR && i2 != 0)
                            {
                                Fl = true;
                                Text += "\n\nI2 " + i2 + " Rad " + ScanList[i2].Rad + " El " + ScanList[i2].El + "\nI " + i + " Rad " + ScanList[i].Rad + " El " + ScanList[i].El + " <ScR";
                                if (ScanList[i2].El > ScanList[i].El)
                                {
                                    ScanList[i].Fl1 = false;
                                    ScanList[i].Fl2 = true;
                                }
                                else
                                {
                                    ScanList[i2].Fl1 = false;
                                    ScanList[i2].Fl2 = true;
                                }
                            }
                            i2 = i;
                        }
                    }
                }
                Text += "\nFl = " + Fl;
            }
            return ScanList;
        }

        public double ScanMetod3()
        {
            Text = string.Empty;
            double Rez = -1;

            if (ScanList.Count != 0)
            {
                ScanStat = new List<ScanStat>();
                int max = 0, k = -1, j = 0;
                for (int i = 0; i < ScanList.Count; i++)
                {
                    if (ScanList[i].Fl1 == true && ScanList[i].Rad > 4)
                    {
                        if (ScanStat.Count == 0)
                        {
                            ScanStat.Add(new ScanStat(new List<int>()));
                            ScanStat[0].AddListElm(ScanList[i].Rad);
                        }
                        else
                        {
                            double x = ScanList[i].Rad, ScanStatEl = 0, Fr = 0, To = 0;
                            bool Fl2 = false;
                            for (j = 0; j < ScanStat.Count; j++)
                            {
                                ScanStatEl = ScanStat[j].SrRad;
                                Fr = ScanStatEl - ScanStatEl / 60;
                                To = ScanStatEl + ScanStatEl / 60;
                                Text += "\n" + j + " SrRad" + ScanStatEl + " " + Fr + " > " + x + " < " + To;
                                if (x > Fr && x < To)
                                {
                                    Fl2 = true;
                                    ScanStat[j].AddListElm(ScanList[i].Rad);
                                    break;
                                }
                                //Text += " " + Fl2;
                            }

                            if (Fl2 == false)
                            {
                                ScanStat.Add(new ScanStat(new List<int>()));
                                ScanStat[ScanStat.Count - 1].AddListElm(ScanList[i].Rad);
                            }
                        }
                    }
                }


                for (j = 0; j < ScanStat.Count; j++)
                {
                    if (ScanStat[j].Count > max) { max = ScanStat[j].Count; k = j; }
                }
                if (k != -1) Rez = ScanStat[k].SrRad;
            }

            //ScanStatListShow();
            return Rez;
        }
        public int ScanTest()
        {
            int d = -1;
            if (ScanList.Count != 0)
            {

                ScanRad();
                double Sr = 0, j = 0, k = 0, Del = 0;
                for (int i = 0; i < ScanList.Count; i++)
                    if (ScanList[i].Fl1 == true && ScanList[i].Rad != 0) { k++; j += ScanList[i].Rad; }

                Sr = j / k;
                for (int i = 0; i < ScanList.Count; i++)
                {
                    if (ScanList[i].Fl1 == true && ScanList[i].Rad != 0)
                    {
                        j = ScanList[i].Rad;
                        Del += Math.Abs(j - Sr);
                    }
                }
                d = (int)(Del / k / Sr * 100);
            }
            return d;
        }

        private int ScanRad()
        {
            int i, j = 0, k = -1, Sr = 0;

            if (ScanList.Count != 0)
            {
                for (i = 0; i < ScanList.Count; i++)
                {
                    if (ScanList[i].Fl1 == true)
                    {
                        if (k != 0)
                        {
                            ScanList[i].Rad = i - k;
                            j++;
                            Sr += ScanList[i].Rad;
                        }
                        else ScanList[i].Rad = 0;
                        k = i;
                    }
                }
                k = Sr / j;
            }
            return k;
        }

        private bool ScanRadTest(int Prsnt, int Rad, int Par)
        {
            bool Fl = false;
            double Rd = Rad;
            if (ScanList.Count != 0)
            {
                Par = 100 - Par - (int)(100 * Math.Sin(Rd / 1000) + 1);
                if (Par <= Prsnt) Fl = true;
            }
            return Fl;
        }

        public string ScanTxt()
        {
            string txt = "", txt2 = "";

            if (ScanList.Count != 0)
            {
                double KEl = 0, IEl = 0;
               
                int k = 0, i = 0, El = 0;
                for (i = 0; i < ScanList.Count; i++)
                {
                    if (ScanList[i].Fl1 == true)
                    {
                        IEl = ScanList[i].El;
                        txt += "\n ^ \nRad " + ScanList[i].Rad;
                        if (k != 0)
                        {
                            if (ScanList[k].El < ScanList[i].El) El = (int)(KEl / IEl * 100);
                            else El = (int)(IEl / KEl * 100);
                        }
                        txt += " | " + El + "\n v \n" + i + " | " + ScanList[i].El;
                        txt2 += "\n |i " + i + " | " + ScanList[i].El + " |Rad " + ScanList[i].Rad;
                        k = i;
                        KEl = IEl;
                    }
                }
            }
            return txt2 + "\n\n++++++++++++++\n\n" + txt;
        }

        public string ScanStatListShow()
        {
            Text = string.Empty;
            if (ScanStat.Count() != 0)
            {
                for (int i = 0; i < ScanStat.Count(); i++)
                {
                    Text += "\n<<" + i + ">>\n |Count " + ScanStat[i].Count + " |ListCount " + ScanStat[i].ListCount() + " |SrRad " + ScanStat[i].SrRad + "|\n";
                    for (int j = 0; j < ScanStat[i].Count; j++) Text += "|" + ScanStat[i].GetListElm(j);
                    Text += "|";
                }
            }
            else Text = "ScanStat is Empty!";
            return Text;
        }

        public short GetMaxVolum()
        {
            short volum = short.MaxValue;
            if(ScanList.Count>0)volum = (short)ScanList.Max(x => x.El);
            return volum;
        }

        public int GetStartUpPoint()
        {
            int N = -1;
            for (int i = 0; i < ScanList.Count; i++)
            {
                if (i != 0 && i != ScanList.Count - 1)
                {
                    if ((ScanList[i - 1].El < 0 && ScanList[i].El > 0) || (ScanList[i - 1].El < 0 && ScanList[i].El == 0 && ScanList[i + 1].El > 0))
                    {
                        N = i;
                        break;
                    }
                }
            }
            return N;
        }

        public int GetStartUpPointFrDataList(int FScan, int TScan)
        {
            var TempWavList = WavList.GetRange(FScan, TScan - FScan);

            int N = -1;
            for (int i=0; i< TempWavList.Count; i++)
            {
                if (i != 0 && i != TempWavList.Count - 1)
                {
                    if ((TempWavList[i - 1].UpLf < 0 && TempWavList[i].UpLf > 0) || (TempWavList[i - 1].UpLf < 0 && TempWavList[i].UpLf == 0 && TempWavList[i + 1].UpLf > 0))
                    {
                        N = i;
                        break;
                    }
                }
            }
            return N;
        }

        //public int GetStartDnPoint()
        //{
        //    int N = -1;
        //    for (int i = 0; i < WavList.Count-1; i++)
        //    {
        //        if (i != 0 && i != WavList.Count - 1)
        //        {
        //            if ((WavList[i - 1].UpLf > 0 && WavList[i].UpLf < 0) || (WavList[i - 1].UpLf > 0 && WavList[i].UpLf == 0 && WavList[i+1].UpLf < 0))
        //            {
        //                N = i;
        //                break;
        //            }
        //        }
        //    }
        //    return N;
        //}
    }
}
