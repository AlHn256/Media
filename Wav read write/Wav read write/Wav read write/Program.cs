using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        struct WavHeader
        {
            public byte[] riffID;
            public uint size;
            public byte[] wavID;
            public byte[] fmtID;
            public uint fmtSize;
            public ushort format;
            public ushort channels;
            public uint sampleRate;
            public uint bytePerSec;
            public ushort blockSize;
            public ushort bit;
            public byte[] dataID;
            public uint dataSize;
        }
        static void Main(string[] args)
        {
            WavHeader Header = new WavHeader();
            List<Int16> lDataList = new List<Int16>();
            int N = 5658;
            N = 176796;
            using (FileStream fs = new FileStream(@"C:\C#\2.wav", FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {
                    Header.riffID = br.ReadBytes(4);//1
                    Header.size = br.ReadUInt32();//2
                    Header.wavID = br.ReadBytes(4);//3
                    Header.fmtID = br.ReadBytes(4);//4
                    Header.fmtSize = br.ReadUInt32();//5
                    Header.format = br.ReadUInt16();//6
                    Header.channels = br.ReadUInt16();//7
                    Header.sampleRate = br.ReadUInt32();//8
                    Header.bytePerSec = br.ReadUInt32();//9
                    Header.blockSize = br.ReadUInt16();//10
                    Header.bit = br.ReadUInt16();//11
                    Header.dataID = br.ReadBytes(6);//12
                    Header.dataSize = br.ReadUInt32();//13

                    int n = (int)(Header.dataSize / Header.channels * 8 / Header.bit);

                    Console.WriteLine("Header.riffID {0}", Header.riffID);
                    Console.WriteLine("Header.size {0}", Header.size);
                    Console.WriteLine("Header.wavID {0}", Header.wavID);
                    Console.WriteLine("Header.fmtID {0}", Header.fmtID);
                    Console.WriteLine("Header.fmtSize {0}", Header.fmtSize);
                    Console.WriteLine("Header.format {0}", Header.format);
                    Console.WriteLine("Header.channels {0}", Header.channels);
                    Console.WriteLine("Header.sampleRate {0}", Header.sampleRate);
                    Console.WriteLine("Header.bytePerSec {0}", Header.bytePerSec);
                    Console.WriteLine("Header.blockSize {0}", Header.blockSize);
                    Console.WriteLine("Header.bit {0}", Header.bit);
                    Console.WriteLine("Header.dataID {0}", Header.dataID);
                    Console.WriteLine("Header.dataSize {0}", Header.dataSize);



                    Console.WriteLine("Header.dataSize {0}", Header.dataSize);
                    Console.WriteLine("Header.channels {0}", Header.channels);
                    Console.WriteLine("Header.bit {0}", Header.bit);
                    Console.WriteLine("\nN {0}", n);


//                    for (int i = 0; i < n; i++)
                        for (int i = 0; i < N; i++)
                        {
                        Int16 tmp = 0;
                        tmp = br.ReadInt16();
                        lDataList.Add(tmp);
                        //if(i>5500)Console.WriteLine(" {0}    {1}", i, tmp);
                    }
                }
                finally
                {
                    if (br != null)
                    {
                        br.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            Console.WriteLine("Открыт");

            using (FileStream fs = new FileStream(@"C:\C#\111.wav", FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                try
                {
                    bw.Write(Header.riffID);//1
                    bw.Write(Header.size);//2
                    bw.Write(Header.wavID);//3
                    bw.Write(Header.fmtID);//4
                    bw.Write(Header.fmtSize);//5
                    bw.Write(Header.format);//6
                    bw.Write(Header.channels);//7
                    bw.Write(Header.sampleRate);//8
                    bw.Write(Header.bytePerSec);//9
                    bw.Write(Header.blockSize);//10
                    bw.Write(Header.bit);//11
                    bw.Write(Header.dataID);//12
                    bw.Write(Header.dataSize);//13

                    int n = (int)(Header.dataSize / Header.channels * 8 / Header.bit);

                    for (int i = 0; i < N; i++)
                    {
                        Int16 tmp = lDataList[i];
                        bw.Write(tmp);
                         Console.WriteLine("{0}   {1}", i, tmp);
                    }
                }
                finally
                {
                    if (bw != null)
                    {
                        bw.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            Console.ReadKey();
            return;

        }
    }
}