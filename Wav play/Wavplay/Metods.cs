using System;
using System.Collections.Generic;

namespace Wavplay
{
    //class Metods
    //{
    //    public List<WavPlay._ScanList> ScanList = new List<WavPlay._ScanList>();
    //    public void SetList(List<WavPlay._ScanList> List){ScanList = new List<WavPlay._ScanList>(List);}
    //    public List<WavPlay._ScanList> GetList(){return ScanList;}
    //    public int ListCount(){return ScanList.Count;}

    //    public int Hz()
    //    {
    //        int i, hz = -1, k = 0, St = 0, Fn = 0;

    //        if (ScanList.Count != 0)
    //        {
    //            double tmp = 0;
    //            for (i = 0; i < ScanList.Count; i++)
    //            {
    //                if (ScanList[i].Fl1 == true)
    //                {
    //                    k++;
    //                    if (Fn != 0) ScanList[i].Rad = i - Fn;
    //                    else St = i;
    //                    Fn = i;
    //                }
    //            }
    //            tmp = Fn - St;

    //            if (k > 3)
    //            {
    //                tmp = tmp / (k - 1);
    //                hz = (int)(44100 / tmp);
    //            }
    //        }
    //        return hz;
    //    }

    //    private int ScanListTest()
    //    {
    //        int i, d = -1;
    //        if (ScanList.Count != 0)
    //        {
    //            double Sr = 0, j = 0, k = 0, Del = 0;
    //            for (i = 0; i < ScanList.Count; i++)
    //            {
    //                if (ScanList[i].Rad != 0)
    //                {
    //                    k++;
    //                    j += ScanList[i].Rad;
    //                }
    //            }

    //            Sr = j / k;

    //            for (i = 0; i < ScanList.Count; i++)
    //            {
    //                if (ScanList[i].Rad != 0)
    //                {
    //                    j = ScanList[i].Rad;
    //                    Del += Math.Abs(j - Sr);
    //                }
    //            }
    //            d = (int)(Del / k / Sr * 100);
    //        }
    //        return d;
    //    }

    //}
}
