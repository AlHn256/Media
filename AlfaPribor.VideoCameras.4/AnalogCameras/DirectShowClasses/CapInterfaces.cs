using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace AnalogNamespace
{

    [ComImport, Guid("56A868A9-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IGraphBuilder
    {
        [PreserveSig]
        int AddFilter([In] IBaseFilter filter, [In, MarshalAs(UnmanagedType.LPWStr)] string name);

        [PreserveSig]
        int RemoveFilter([In] IBaseFilter filter);

        [PreserveSig]
        int EnumFilters([Out] out IntPtr enumerator);

        [PreserveSig]
        int FindFilterByName([In, MarshalAs(UnmanagedType.LPWStr)] string name, [Out] out IBaseFilter filter);

        [PreserveSig]
        int ConnectDirect([In] IPin pinOut, [In] IPin pinIn, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int Reconnect([In] IPin pin);

        [PreserveSig]
        int Disconnect([In] IPin pin);

        [PreserveSig]
        int SetDefaultSyncSource();

        [PreserveSig]
        int Connect([In] IPin pinOut, [In] IPin pinIn);

        [PreserveSig]
        int Render([In] IPin pinOut);

        [PreserveSig]
        int RenderFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string file,
            [In, MarshalAs(UnmanagedType.LPWStr)] string playList);

        [PreserveSig]
        int AddSourceFilter(
            [In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string filterName,
            [Out] out IBaseFilter filter);

        [PreserveSig]
        int SetLogFile(IntPtr hFile);

        [PreserveSig]
        int Abort();

        [PreserveSig]
        int ShouldOperationContinue();
    }

    [ComImport, Guid("56A86895-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBaseFilter
    {
        [PreserveSig]
        int GetClassID([Out] out Guid ClassID);

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Run(long start);

        [PreserveSig]
        int GetState(int milliSecsTimeout, [Out] out int filterState);

        [PreserveSig]
        int SetSyncSource([In] IntPtr clock);

        [PreserveSig]
        int GetSyncSource([Out] out IntPtr clock);

        [PreserveSig]
        int EnumPins([Out] out IEnumPins enumPins);

        [PreserveSig]
        int FindPin([In, MarshalAs(UnmanagedType.LPWStr)] string id, [Out] out IPin pin);

        [PreserveSig]
        int QueryFilterInfo([Out] FilterInfo filterInfo);

        [PreserveSig]
        int JoinFilterGraph([In] IFilterGraph graph, [In, MarshalAs(UnmanagedType.LPWStr)] string name);

        [PreserveSig]
        int QueryVendorInfo([Out, MarshalAs(UnmanagedType.LPWStr)] out string vendorInfo);
    }

    [ComImport, Guid("56A86891-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPin
    {
        [PreserveSig]
        int Connect([In] IPin receivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int ReceiveConnection([In] IPin receivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int ConnectedTo([Out] out IPin pin);

        [PreserveSig]
        int ConnectionMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int QueryPinInfo([Out, MarshalAs(UnmanagedType.LPStruct)] PinInfo pinInfo);

        [PreserveSig]
        int QueryDirection(out PinDirection pinDirection);

        [PreserveSig]
        int QueryId([Out, MarshalAs(UnmanagedType.LPWStr)] out string id);

        [PreserveSig]
        int QueryAccept([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int EnumMediaTypes(IntPtr enumerator);

        [PreserveSig]
        int QueryInternalConnections(IntPtr apPin, [In, Out] ref int nPin);

        [PreserveSig]
        int EndOfStream();

        [PreserveSig]
        int BeginFlush();

        [PreserveSig]
        int EndFlush();

        [PreserveSig]
        int NewSegment(long start, long stop, double rate);
    }

    [ComImport, Guid("56A86892-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumPins
    {
        [PreserveSig]
        int Next([In] int cPins,
           [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IPin[] pins,
           [Out] out int pinsFetched);

        [PreserveSig]
        int Skip([In] int cPins);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumPins enumPins);
    }

    [ComImport,Guid("56A8689F-0AD4-11CE-B03A-0020AF0BA770"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFilterGraph
    {
        [PreserveSig]
        int AddFilter([In] IBaseFilter filter, [In, MarshalAs(UnmanagedType.LPWStr)] string name);

        [PreserveSig]
        int RemoveFilter([In] IBaseFilter filter);

        [PreserveSig]
        int EnumFilters([Out] out IntPtr enumerator);

        [PreserveSig]
        int FindFilterByName([In, MarshalAs(UnmanagedType.LPWStr)] string name, [Out] out IBaseFilter filter);

        [PreserveSig]
        int ConnectDirect([In] IPin pinOut, [In] IPin pinIn, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int Reconnect([In] IPin pin);

        [PreserveSig]
        int Disconnect([In] IPin pin);

        [PreserveSig]
        int SetDefaultSyncSource();
    }

    [ComImport,Guid("55272A00-42CB-11CE-8135-00AA004BB851"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyBag
    {
        [PreserveSig]
        int Read(
            [In, MarshalAs(UnmanagedType.LPWStr)] string propertyName,
            [In, Out, MarshalAs(UnmanagedType.Struct)] ref object pVar,
            [In] IntPtr pErrorLog);

         [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPWStr)] string propertyName,
            [In, MarshalAs(UnmanagedType.Struct)] ref object pVar);
    }

    [ComImport,Guid("6B652FFF-11FE-4FCE-92AD-0266B5D7C78F"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISampleGrabber
    {
        [PreserveSig]
        int SetOneShot([In, MarshalAs(UnmanagedType.Bool)] bool oneShot);

        [PreserveSig]
        int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int GetConnectedMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);

        [PreserveSig]
        int SetBufferSamples([In, MarshalAs(UnmanagedType.Bool)] bool bufferThem);

        [PreserveSig]
        int GetCurrentBuffer(ref int bufferSize, IntPtr buffer);

        [PreserveSig]
        int GetCurrentSample(IntPtr sample);

        [PreserveSig]
        int SetCallback(ISampleGrabberCB callback, int whichMethodToCallback);
    }

    [ComImport, Guid("0579154A-2B53-4994-B0D0-E773148EFF85"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISampleGrabberCB
    {
        [PreserveSig]
        int SampleCB(double sampleTime, IntPtr sample);

        [PreserveSig]
        int BufferCB(double sampleTime, IntPtr buffer, int bufferLen);
    }

    [ComImport, Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator([In] ref Guid type, [Out] out IEnumMoniker enumMoniker, [In] int flags);
    }

    [ComImport,Guid("56A868B4-0AD4-11CE-B03A-0020AF0BA770"),InterfaceType(ComInterfaceType.InterfaceIsDual)]
    internal interface IVideoWindow
    {
        [PreserveSig]
        int put_Caption(string caption);

        [PreserveSig]
        int get_Caption([Out] out string caption);

         [PreserveSig]
        int put_WindowStyle(int windowStyle);

        [PreserveSig]
        int get_WindowStyle(out int windowStyle);

        [PreserveSig]
        int put_WindowStyleEx(int windowStyleEx);

        [PreserveSig]
        int get_WindowStyleEx(out int windowStyleEx);

        [PreserveSig]
        int put_AutoShow([In, MarshalAs(UnmanagedType.Bool)] bool autoShow);

        [PreserveSig]
        int get_AutoShow([Out, MarshalAs(UnmanagedType.Bool)] out bool autoShow);

        [PreserveSig]
        int put_WindowState(int windowState);

        [PreserveSig]
        int get_WindowState(out int windowState);

        [PreserveSig]
        int put_BackgroundPalette([In, MarshalAs(UnmanagedType.Bool)] bool backgroundPalette);

        [PreserveSig]
        int get_BackgroundPalette([Out, MarshalAs(UnmanagedType.Bool)] out bool backgroundPalette);

        [PreserveSig]
        int put_Visible([In, MarshalAs(UnmanagedType.Bool)] bool visible);

        [PreserveSig]
        int get_Visible([Out, MarshalAs(UnmanagedType.Bool)] out bool visible);

        [PreserveSig]
        int put_Left(int left);

        [PreserveSig]
        int get_Left(out int left);

        [PreserveSig]
        int put_Width(int width);

        [PreserveSig]
        int get_Width(out int width);

        [PreserveSig]
        int put_Top(int top);

        [PreserveSig]
        int get_Top(out int top);

        [PreserveSig]
        int put_Height(int height);

        [PreserveSig]
        int get_Height(out int height);

        [PreserveSig]
        int put_Owner(IntPtr owner);

        [PreserveSig]
        int get_Owner(out IntPtr owner);

        [PreserveSig]
        int put_MessageDrain(IntPtr drain);

        [PreserveSig]
        int get_MessageDrain(out IntPtr drain);

        [PreserveSig]
        int get_BorderColor(out int color);

        [PreserveSig]
        int put_BorderColor(int color);

        [PreserveSig]
        int get_FullScreenMode(
            [Out, MarshalAs(UnmanagedType.Bool)] out bool fullScreenMode);

        [PreserveSig]
        int put_FullScreenMode([In, MarshalAs(UnmanagedType.Bool)] bool fullScreenMode);

        [PreserveSig]
        int SetWindowForeground(int focus);

        [PreserveSig]
        int NotifyOwnerMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [PreserveSig]
        int SetWindowPosition(int left, int top, int width, int height);

        [PreserveSig]
        int GetWindowPosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int GetMinIdealImageSize(out int width, out int height);

        [PreserveSig]
        int GetMaxIdealImageSize(out int width, out int height);

        [PreserveSig]
        int GetRestorePosition(out int left, out int top, out int width, out int height);

        [PreserveSig]
        int HideCursor([In, MarshalAs(UnmanagedType.Bool)] bool hideCursor);

        [PreserveSig]
        int IsCursorHidden([Out, MarshalAs(UnmanagedType.Bool)] out bool hideCursor);
    }

    [ComImport, Guid("56A868B1-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    internal interface IMediaControl
    {
        [PreserveSig]
        int Run();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int GetState(int timeout, out int filterState);

        [PreserveSig]
        int RenderFile(string fileName);

        [PreserveSig]
        int AddSourceFilter([In] string fileName, [Out, MarshalAs(UnmanagedType.IDispatch)] out object filterInfo);

        [PreserveSig]
        int get_FilterCollection(
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object collection);

        [PreserveSig]
        int get_RegFilterCollection(
            [Out, MarshalAs(UnmanagedType.IDispatch)] out object collection);

        [PreserveSig]
        int StopWhenReady();
    }

    [ComImport, Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAMStreamConfig
    {
        [PreserveSig]
        int SetFormat([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int GetFormat([Out] out AMMediaType pmt);

        [PreserveSig]
        int GetNumberOfCapabilities(out int piCount, out int piSize);

        [PreserveSig]
        int GetStreamCaps(
            [In] int iIndex,
            [Out] out AMMediaType ppmt,
            [In] IntPtr pSCC
            );
    }

    [ComImport, Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileSinkFilter
    {
        [PreserveSig]
        int SetFileName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );

        [PreserveSig]
        int GetCurFile(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszFileName,
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
            );
    }

    [ComImport, Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICaptureGraphBuilder2
    {
        [PreserveSig]
        int SetFiltergraph([In] IGraphBuilder pfg);

        [PreserveSig]
        int GetFiltergraph([Out] out IGraphBuilder ppfg);

        [PreserveSig]
        int SetOutputFileName(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pType,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile,
            [Out] out IBaseFilter ppbf,
            [Out] out IFileSinkFilter ppSink
            );

        [PreserveSig]
        int FindInterface(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid pType,
            [In] IBaseFilter pbf,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppint
            );

        [PreserveSig]
        int RenderStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid PinCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid MediaType,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pSource,
            [In] IBaseFilter pfCompressor,
            [In] IBaseFilter pfRenderer
            );

        [PreserveSig]
        int ControlStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid PinCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid MediaType,
            [In, MarshalAs(UnmanagedType.Interface)] IBaseFilter pFilter,
            [In] DsLong pstart,
            [In] DsLong pstop,
            [In] short wStartCookie,
            [In] short wStopCookie
            );

        [PreserveSig]
        int AllocCapFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile,
            [In] long dwlSize
            );

        [PreserveSig]
        int CopyCaptureFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrOld,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrNew,
            [In, MarshalAs(UnmanagedType.Bool)] bool fAllowEscAbort,
            [In] IAMCopyCaptureFileProgress pFilter
            );

        [PreserveSig]
        int FindPin(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pSource,
            [In] PinDirection pindir,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid PinCategory,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid MediaType,
            [In, MarshalAs(UnmanagedType.Bool)] bool fUnconnected,
            [In] int ZeroBasedIndex,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPin ppPin
            );
    }

    /// <summary>
    /// DirectShowLib.DsGuid is a wrapper class around a System.Guid value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public class DsGuid
    {
        [FieldOffset(0)]
        private Guid guid;

        public static readonly DsGuid Empty = Guid.Empty;

        /// <summary>
        /// Empty constructor. 
        /// Initialize it with System.Guid.Empty
        /// </summary>
        public DsGuid()
        {
            this.guid = Guid.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid string representation.
        /// </summary>
        /// <param name="g">A valid System.Guid as string</param>
        public DsGuid(string g)
        {
            this.guid = new Guid(g);
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid.
        /// </summary>
        /// <param name="g">A System.Guid value type</param>
        public DsGuid(Guid g)
        {
            this.guid = g;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.guid.ToString();
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance with a specific format.
        /// </summary>
        /// <param name="format"><see cref="System.Guid.ToString"/> for a description of the format parameter.</param>
        /// <returns>A string representing this instance according to the format parameter</returns>
        public string ToString(string format)
        {
            return this.guid.ToString(format);
        }

        public override int GetHashCode()
        {
            return this.guid.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsGuid and System.Guid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.ToGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new DsGuid instance
        ///   DsGuid dsG = new DsGuid("{33D57EBF-7C9D-435e-A15E-D300B52FBD91}");
        ///   // Do implicit cast between DsGuid and Guid
        ///   Guid g = dsG;
        ///
        ///   Console.WriteLine(g.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsGuid to be cast</param>
        /// <returns>A casted System.Guid</returns>
        public static implicit operator Guid(DsGuid g)
        {
            return g.guid;
        }

        /// <summary>
        /// Define implicit cast between System.Guid and DirectShowLib.DsGuid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new Guid instance
        ///   Guid g = new Guid("{B9364217-366E-45f8-AA2D-B0ED9E7D932D}");
        ///   // Do implicit cast between Guid and DsGuid
        ///   DsGuid dsG = g;
        ///
        ///   Console.WriteLine(dsG.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Guid to be cast</param>
        /// <returns>A casted DirectShowLib.DsGuid</returns>
        public static implicit operator DsGuid(Guid g)
        {
            return new DsGuid(g);
        }

        /// <summary>
        /// Get the System.Guid equivalent to this DirectShowLib.DsGuid instance.
        /// </summary>
        /// <returns>A System.Guid</returns>
        public Guid ToGuid()
        {
            return this.guid;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsGuid instance for a given System.Guid
        /// </summary>
        /// <param name="g">The System.Guid to wrap into a DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsGuid FromGuid(Guid g)
        {
            return new DsGuid(g);
        }
    }

    /// <summary>
    /// DirectShowLib.DsLong is a wrapper class around a <see cref="System.Int64"/> value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public class DsLong
    {
        private long Value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsLong with the Value parameter
        /// </summary>
        /// <param name="Value">Value to assign to this new instance</param>
        public DsLong(long Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsLong Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsLong and System.Int64 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsLong.ToInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new DsLong instance
        ///   DsLong dsL = new DsLong(9876543210);
        ///   // Do implicit cast between DsLong and Int64
        ///   long l = dsL;
        ///
        ///   Console.WriteLine(l.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsLong to be cast</param>
        /// <returns>A casted System.Int64</returns>
        public static implicit operator long(DsLong l)
        {
            return l.Value;
        }

        /// <summary>
        /// Define implicit cast between System.Int64 and DirectShowLib.DsLong for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new Int64 instance
        ///   long l = 9876543210;
        ///   // Do implicit cast between Int64 and DsLong
        ///   DsLong dsl = l;
        ///
        ///   Console.WriteLine(dsl.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Int64 to be cast</param>
        /// <returns>A casted DirectShowLib.DsLong</returns>
        public static implicit operator DsLong(long l)
        {
            return new DsLong(l);
        }

        /// <summary>Get the System.Int64 equivalent to this DirectShowLib.DsLong instance.</summary>
        /// <returns>A System.Int64</returns>
        public long ToInt64()
        {
            return this.Value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsLong instance for a given System.Int64
        /// </summary>
        /// <param name="g">The System.Int64 to wrap into a DirectShowLib.DsLong</param>
        /// <returns>A new instance of DirectShowLib.DsLong</returns>
        public static DsLong FromInt64(long l)
        {
            return new DsLong(l);
        }
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("670d1d20-a068-11d0-b3f0-00aa003761c5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAMCopyCaptureFileProgress
    {
        [PreserveSig]
        int Progress(int iProgress);
    }

    sealed public class PinCategory
    {
        private PinCategory()
        {
            // Prevent people from trying to instantiate this class
        }

        /// <summary> PIN_CATEGORY_CAPTURE </summary>
        public static readonly Guid Capture = new Guid(0xfb6c4281, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_PREVIEW </summary>
        public static readonly Guid Preview = new Guid(0xfb6c4282, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_ANALOGVIDEOIN </summary>
        public static readonly Guid AnalogVideoIn = new Guid(0xfb6c4283, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_VBI </summary>
        public static readonly Guid VBI = new Guid(0xfb6c4284, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_VIDEOPORT </summary>
        public static readonly Guid VideoPort = new Guid(0xfb6c4285, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_NABTS </summary>
        public static readonly Guid NABTS = new Guid(0xfb6c4286, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_EDS </summary>
        public static readonly Guid EDS = new Guid(0xfb6c4287, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_TELETEXT </summary>
        public static readonly Guid TeleText = new Guid(0xfb6c4288, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_CC </summary>
        public static readonly Guid CC = new Guid(0xfb6c4289, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_STILL </summary>
        public static readonly Guid Still = new Guid(0xfb6c428a, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_TIMECODE </summary>
        public static readonly Guid TimeCode = new Guid(0xfb6c428b, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> PIN_CATEGORY_VIDEOPORT_VBI </summary>
        public static readonly Guid VideoPortVBI = new Guid(0xfb6c428c, 0x0353, 0x11d1, 0x90, 0x5f, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

    }

    sealed public class MediaType
    {
        private MediaType()
        {
            // Prevent people from trying to instantiate this class
        }

        public static readonly Guid Null = Guid.Empty;

        /// <summary> MEDIATYPE_Video 'vids' </summary>
        public static readonly Guid Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Interleaved 'iavs' </summary>
        public static readonly Guid Interleaved = new Guid(0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Audio 'auds' </summary>
        public static readonly Guid Audio = new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Text 'txts' </summary>
        public static readonly Guid Text = new Guid(0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_Stream </summary>
        public static readonly Guid Stream = new Guid(0xe436eb83, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIATYPE_VBI </summary>
        public static readonly Guid VBI = new Guid(0xf72a76e1, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> MEDIATYPE_Midi </summary>
        public static readonly Guid Midi = new Guid(0x7364696D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_File </summary>
        public static readonly Guid File = new Guid(0x656c6966, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_ScriptCommand </summary>
        public static readonly Guid ScriptCommand = new Guid(0x73636d64, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_AUXLine21Data </summary>
        public static readonly Guid AuxLine21Data = new Guid(0x670aea80, 0x3a82, 0x11d0, 0xb7, 0x9b, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIATYPE_Timecode </summary>
        public static readonly Guid Timecode = new Guid(0x0482dee3, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_LMRT </summary>
        public static readonly Guid LMRT = new Guid(0x74726c6d, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_URL_STREAM </summary>
        public static readonly Guid URLStream = new Guid(0x736c7275, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIATYPE_AnalogVideo </summary>
        public static readonly Guid AnalogVideo = new Guid(0x0482dde1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_AnalogAudio </summary>
        public static readonly Guid AnalogAudio = new Guid(0x0482dee1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIATYPE_MPEG2_SECTIONS </summary>
        public static readonly Guid Mpeg2Sections = new Guid(0x455f176c, 0x4b06, 0x47ce, 0x9a, 0xef, 0x8c, 0xae, 0xf7, 0x3d, 0xf7, 0xb5);

        /// <summary> MEDIATYPE_DTVCCData </summary>
        public static readonly Guid DTVCCData = new Guid(0xfb77e152, 0x53b2, 0x499c, 0xb4, 0x6b, 0x50, 0x9f, 0xc3, 0x3e, 0xdf, 0xd7);

        /// <summary> MEDIATYPE_MSTVCaption </summary>
        public static readonly Guid MSTVCaption = new Guid(0xB88B8A89, 0xB049, 0x4C80, 0xAD, 0xCF, 0x58, 0x98, 0x98, 0x5E, 0x22, 0xC1);

    }

    /// <summary>Цветовые пространства</summary>
    sealed public class MediaSubType
    {
        private MediaSubType()
        {
            // Prevent people from trying to instantiate this class
        }

        public static readonly Guid Null = Guid.Empty;

        /// <summary> MEDIASUBTYPE_CLPL </summary>
        public static readonly Guid CLPL = new Guid(0x4C504C43, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YUYV </summary>
        public static readonly Guid YUYV = new Guid(0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IYUV </summary>
        public static readonly Guid IYUV = new Guid(0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YVU9 </summary>
        public static readonly Guid YVU9 = new Guid(0x39555659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y411 </summary>
        public static readonly Guid Y411 = new Guid(0x31313459, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y41P </summary>
        public static readonly Guid Y41P = new Guid(0x50313459, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YUY2 </summary>
        public static readonly Guid YUY2 = new Guid(0x32595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YVYU </summary>
        public static readonly Guid YVYU = new Guid(0x55595659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_UYVY </summary>
        public static readonly Guid UYVY = new Guid(0x59565955, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Y211 </summary>
        public static readonly Guid Y211 = new Guid(0x31313259, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CLJR </summary>
        public static readonly Guid CLJR = new Guid(0x524a4c43, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IF09 </summary>
        public static readonly Guid IF09 = new Guid(0x39304649, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CPLA </summary>
        public static readonly Guid CPLA = new Guid(0x414c5043, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_MJPG </summary>
        public static readonly Guid MJPG = new Guid(0x47504A4D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_TVMJ </summary>
        public static readonly Guid TVMJ = new Guid(0x4A4D5654, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_WAKE </summary>
        public static readonly Guid WAKE = new Guid(0x454B4157, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_CFCC </summary>
        public static readonly Guid CFCC = new Guid(0x43434643, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IJPG </summary>
        public static readonly Guid IJPG = new Guid(0x47504A49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Plum </summary>
        public static readonly Guid PLUM = new Guid(0x6D756C50, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DVCS </summary>
        public static readonly Guid DVCS = new Guid(0x53435644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DVSD </summary>
        public static readonly Guid DVSD = new Guid(0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_MDVF </summary>
        public static readonly Guid MDVF = new Guid(0x4656444D, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB1 </summary>
        public static readonly Guid RGB1 = new Guid(0xe436eb78, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB4 </summary>
        public static readonly Guid RGB4 = new Guid(0xe436eb79, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB8 </summary>
        public static readonly Guid RGB8 = new Guid(0xe436eb7a, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB565 </summary>
        public static readonly Guid RGB565 = new Guid(0xe436eb7b, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB555 </summary>
        public static readonly Guid RGB555 = new Guid(0xe436eb7c, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB24 </summary>
        public static readonly Guid RGB24 = new Guid(0xe436eb7d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_RGB32 </summary>
        public static readonly Guid RGB32 = new Guid(0xe436eb7e, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_ARGB1555 </summary>
        public static readonly Guid ARGB1555 = new Guid(0x297c55af, 0xe209, 0x4cb3, 0xb7, 0x57, 0xc7, 0x6d, 0x6b, 0x9c, 0x88, 0xa8);

        /// <summary> MEDIASUBTYPE_ARGB4444 </summary>
        public static readonly Guid ARGB4444 = new Guid(0x6e6415e6, 0x5c24, 0x425f, 0x93, 0xcd, 0x80, 0x10, 0x2b, 0x3d, 0x1c, 0xca);

        /// <summary> MEDIASUBTYPE_ARGB32 </summary>
        public static readonly Guid ARGB32 = new Guid(0x773c9ac0, 0x3274, 0x11d0, 0xb7, 0x24, 0x00, 0xaa, 0x00, 0x6c, 0x1a, 0x01);

        /// <summary> MEDIASUBTYPE_A2R10G10B10 </summary>
        public static readonly Guid A2R10G10B10 = new Guid(0x2f8bb76d, 0xb644, 0x4550, 0xac, 0xf3, 0xd3, 0x0c, 0xaa, 0x65, 0xd5, 0xc5);

        /// <summary> MEDIASUBTYPE_A2B10G10R10 </summary>
        public static readonly Guid A2B10G10R10 = new Guid(0x576f7893, 0xbdf6, 0x48c4, 0x87, 0x5f, 0xae, 0x7b, 0x81, 0x83, 0x45, 0x67);

        /// <summary> MEDIASUBTYPE_AYUV </summary>
        public static readonly Guid AYUV = new Guid(0x56555941, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_AI44 </summary>
        public static readonly Guid AI44 = new Guid(0x34344941, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IA44 </summary>
        public static readonly Guid IA44 = new Guid(0x34344149, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB32_D3D_DX7_RT </summary>
        public static readonly Guid RGB32_D3D_DX7_RT = new Guid(0x32335237, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB16_D3D_DX7_RT </summary>
        public static readonly Guid RGB16_D3D_DX7_RT = new Guid(0x36315237, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB32_D3D_DX7_RT </summary>
        public static readonly Guid ARGB32_D3D_DX7_RT = new Guid(0x38384137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB4444_D3D_DX7_RT </summary>
        public static readonly Guid ARGB4444_D3D_DX7_RT = new Guid(0x34344137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB1555_D3D_DX7_RT </summary>
        public static readonly Guid ARGB1555_D3D_DX7_RT = new Guid(0x35314137, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB32_D3D_DX9_RT </summary>
        public static readonly Guid RGB32_D3D_DX9_RT = new Guid(0x32335239, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RGB16_D3D_DX9_RT </summary>
        public static readonly Guid RGB16_D3D_DX9_RT = new Guid(0x36315239, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB32_D3D_DX9_RT </summary>
        public static readonly Guid ARGB32_D3D_DX9_RT = new Guid(0x38384139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB4444_D3D_DX9_RT </summary>
        public static readonly Guid ARGB4444_D3D_DX9_RT = new Guid(0x34344139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_ARGB1555_D3D_DX9_RT </summary>
        public static readonly Guid ARGB1555_D3D_DX9_RT = new Guid(0x35314139, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_YV12 </summary>
        public static readonly Guid YV12 = new Guid(0x32315659, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_NV12 </summary>
        public static readonly Guid NV12 = new Guid(0x3231564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC1 </summary>
        public static readonly Guid IMC1 = new Guid(0x31434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC2 </summary>
        public static readonly Guid IMC2 = new Guid(0x32434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC3 </summary>
        public static readonly Guid IMC3 = new Guid(0x33434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IMC4 </summary>
        public static readonly Guid IMC4 = new Guid(0x34434D49, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_S340 </summary>
        public static readonly Guid S340 = new Guid(0x30343353, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_S342 </summary>
        public static readonly Guid S342 = new Guid(0x32343353, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Overlay </summary>
        public static readonly Guid Overlay = new Guid(0xe436eb7f, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Packet </summary>
        public static readonly Guid MPEG1Packet = new Guid(0xe436eb80, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Payload </summary>
        public static readonly Guid MPEG1Payload = new Guid(0xe436eb81, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1AudioPayload </summary>
        public static readonly Guid MPEG1AudioPayload = new Guid(0x00000050, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary> MEDIATYPE_MPEG1SystemStream </summary>
        public static readonly Guid MPEG1SystemStream = new Guid(0xe436eb82, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1System </summary>
        public static readonly Guid MPEG1System = new Guid(0xe436eb84, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1VideoCD </summary>
        public static readonly Guid MPEG1VideoCD = new Guid(0xe436eb85, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Video </summary>
        public static readonly Guid MPEG1Video = new Guid(0xe436eb86, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_MPEG1Audio </summary>
        public static readonly Guid MPEG1Audio = new Guid(0xe436eb87, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_Avi </summary>
        public static readonly Guid Avi = new Guid(0xe436eb88, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_Asf </summary>
        public static readonly Guid Asf = new Guid(0x3db80f90, 0x9412, 0x11d1, 0xad, 0xed, 0x00, 0x00, 0xf8, 0x75, 0x4b, 0x99);

        /// <summary> MEDIASUBTYPE_QTMovie </summary>
        public static readonly Guid QTMovie = new Guid(0xe436eb89, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_QTRpza </summary>
        public static readonly Guid QTRpza = new Guid(0x617a7072, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTSmc </summary>
        public static readonly Guid QTSmc = new Guid(0x20636d73, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTRle </summary>
        public static readonly Guid QTRle = new Guid(0x20656c72, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_QTJpeg </summary>
        public static readonly Guid QTJpeg = new Guid(0x6765706a, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_PCMAudio_Obsolete </summary>
        public static readonly Guid PCMAudio_Obsolete = new Guid(0xe436eb8a, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_PCM </summary>
        public static readonly Guid PCM = new Guid(0x00000001, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary> MEDIASUBTYPE_WAVE </summary>
        public static readonly Guid WAVE = new Guid(0xe436eb8b, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_AU </summary>
        public static readonly Guid AU = new Guid(0xe436eb8c, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_AIFF </summary>
        public static readonly Guid AIFF = new Guid(0xe436eb8d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_dvhd </summary>
        public static readonly Guid dvhd = new Guid(0x64687664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dvsl </summary>
        public static readonly Guid dvsl = new Guid(0x6c737664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dv25 </summary>
        public static readonly Guid dv25 = new Guid(0x35327664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dv50 </summary>
        public static readonly Guid dv50 = new Guid(0x30357664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_dvh1 </summary>
        public static readonly Guid dvh1 = new Guid(0x31687664, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_Line21_BytePair </summary>
        public static readonly Guid Line21_BytePair = new Guid(0x6e8d4a22, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_Line21_GOPPacket </summary>
        public static readonly Guid Line21_GOPPacket = new Guid(0x6e8d4a23, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_Line21_VBIRawData </summary>
        public static readonly Guid Line21_VBIRawData = new Guid(0x6e8d4a24, 0x310c, 0x11d0, 0xb7, 0x9a, 0x00, 0xaa, 0x00, 0x37, 0x67, 0xa7);

        /// <summary> MEDIASUBTYPE_TELETEXT </summary>
        public static readonly Guid TELETEXT = new Guid(0xf72a76e3, 0xeb0a, 0x11d0, 0xac, 0xe4, 0x00, 0x00, 0xc0, 0xcc, 0x16, 0xba);

        /// <summary> MEDIASUBTYPE_WSS </summary>
        public static readonly Guid WSS = new Guid(0x2791D576, 0x8E7A, 0x466F, 0x9E, 0x90, 0x5D, 0x3F, 0x30, 0x83, 0x73, 0x8B);

        /// <summary> MEDIASUBTYPE_VPS </summary>
        public static readonly Guid VPS = new Guid(0xa1b3f620, 0x9792, 0x4d8d, 0x81, 0xa4, 0x86, 0xaf, 0x25, 0x77, 0x20, 0x90);

        /// <summary> MEDIASUBTYPE_DRM_Audio </summary>
        public static readonly Guid DRM_Audio = new Guid(0x00000009, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_IEEE_FLOAT </summary>
        public static readonly Guid IEEE_FLOAT = new Guid(0x00000003, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DOLBY_AC3_SPDIF </summary>
        public static readonly Guid DOLBY_AC3_SPDIF = new Guid(0x00000092, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_RAW_SPORT </summary>
        public static readonly Guid RAW_SPORT = new Guid(0x00000240, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_SPDIF_TAG_241h </summary>
        public static readonly Guid SPDIF_TAG_241h = new Guid(0x00000241, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_DssVideo </summary>
        public static readonly Guid DssVideo = new Guid(0xa0af4f81, 0xe163, 0x11d0, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_DssAudio </summary>
        public static readonly Guid DssAudio = new Guid(0xa0af4f82, 0xe163, 0x11d0, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_VPVideo </summary>
        public static readonly Guid VPVideo = new Guid(0x5a9b6a40, 0x1a22, 0x11d1, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_VPVBI </summary>
        public static readonly Guid VPVBI = new Guid(0x5a9b6a41, 0x1a22, 0x11d1, 0xba, 0xd9, 0x00, 0x60, 0x97, 0x44, 0x11, 0x1a);

        /// <summary> MEDIASUBTYPE_AnalogVideo_NTSC_M </summary>
        public static readonly Guid AnalogVideo_NTSC_M = new Guid(0x0482dde2, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_B </summary>
        public static readonly Guid AnalogVideo_PAL_B = new Guid(0x0482dde5, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_D </summary>
        public static readonly Guid AnalogVideo_PAL_D = new Guid(0x0482dde6, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_G </summary>
        public static readonly Guid AnalogVideo_PAL_G = new Guid(0x0482dde7, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_H </summary>
        public static readonly Guid AnalogVideo_PAL_H = new Guid(0x0482dde8, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_I </summary>
        public static readonly Guid AnalogVideo_PAL_I = new Guid(0x0482dde9, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_M </summary>
        public static readonly Guid AnalogVideo_PAL_M = new Guid(0x0482ddea, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_N </summary>
        public static readonly Guid AnalogVideo_PAL_N = new Guid(0x0482ddeb, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_PAL_N_COMBO </summary>
        public static readonly Guid AnalogVideo_PAL_N_COMBO = new Guid(0x0482ddec, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_B </summary>
        public static readonly Guid AnalogVideo_SECAM_B = new Guid(0x0482ddf0, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_D </summary>
        public static readonly Guid AnalogVideo_SECAM_D = new Guid(0x0482ddf1, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_G </summary>
        public static readonly Guid AnalogVideo_SECAM_G = new Guid(0x0482ddf2, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_H </summary>
        public static readonly Guid AnalogVideo_SECAM_H = new Guid(0x0482ddf3, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_K </summary>
        public static readonly Guid AnalogVideo_SECAM_K = new Guid(0x0482ddf4, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_K1 </summary>
        public static readonly Guid AnalogVideo_SECAM_K1 = new Guid(0x0482ddf5, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> MEDIASUBTYPE_AnalogVideo_SECAM_L </summary>
        public static readonly Guid AnalogVideo_SECAM_L = new Guid(0x0482ddf6, 0x7817, 0x11cf, 0x8a, 0x03, 0x00, 0xaa, 0x00, 0x6e, 0xcb, 0x65);

        /// <summary> not in uuids.h </summary>
        public static readonly Guid I420 = new Guid(0x30323449, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> WMMEDIASUBTYPE_VIDEOIMAGE </summary>
        public static readonly Guid VideoImage = new Guid(0x1d4a45f2, 0xe5f6, 0x4b44, 0x83, 0x88, 0xf0, 0xae, 0x5c, 0x0e, 0x0c, 0x37);

        /// <summary> WMMEDIASUBTYPE_MPEG2_VIDEO </summary>
        public static readonly Guid Mpeg2Video = new Guid(0xe06d8026, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> WMMEDIASUBTYPE_WebStream </summary>
        public static readonly Guid WebStream = new Guid(0x776257d4, 0xc627, 0x41cb, 0x8f, 0x81, 0x7a, 0xc7, 0xff, 0x1c, 0x40, 0xcc);

        /// <summary> MEDIASUBTYPE_MPEG2_AUDIO </summary>
        public static readonly Guid Mpeg2Audio = new Guid(0xe06d802b, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_DOLBY_AC3 </summary>
        public static readonly Guid DolbyAC3 = new Guid(0xe06d802c, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_DVB_SI </summary>
        public static readonly Guid DvbSI = new Guid(0xe9dd31a3, 0x221d, 0x4adb, 0x85, 0x32, 0x9a, 0xf3, 0x09, 0xc1, 0xa4, 0x08);

        /// <summary> MEDIASUBTYPE_ATSC_SI </summary>
        public static readonly Guid AtscSI = new Guid(0xb3c7397c, 0xd303, 0x414d, 0xb3, 0x3c, 0x4e, 0xd2, 0xc9, 0xd2, 0x97, 0x33);

        /// <summary> MEDIASUBTYPE_MPEG2DATA </summary>
        public static readonly Guid Mpeg2Data = new Guid(0xc892e55b, 0x252d, 0x42b5, 0xa3, 0x16, 0xd9, 0x97, 0xe7, 0xa5, 0xd9, 0x95);

        /// <summary> MEDIASUBTYPE_MPEG2_PROGRAM </summary>
        public static readonly Guid Mpeg2Program = new Guid(0xe06d8022, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_MPEG2_TRANSPORT </summary>
        public static readonly Guid Mpeg2Transport = new Guid(0xe06d8023, 0xdb46, 0x11cf, 0xb4, 0xd1, 0x00, 0x80, 0x5f, 0x6c, 0xbb, 0xea);

        /// <summary> MEDIASUBTYPE_MPEG2_TRANSPORT_STRIDE </summary>
        public static readonly Guid Mpeg2TransportStride = new Guid(0x138aa9a4, 0x1ee2, 0x4c5b, 0x98, 0x8e, 0x19, 0xab, 0xfd, 0xbc, 0x8a, 0x11);

        /// <summary> MEDIASUBTYPE_None </summary>
        public static readonly Guid None = new Guid(0xe436eb8e, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);

        /// <summary> MEDIASUBTYPE_H264 </summary>
        public static readonly Guid H264 = new Guid(0x34363248, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_NV24 </summary>
        public static readonly Guid NV24 = new Guid(0x3432564E, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        /// <summary> MEDIASUBTYPE_708_608Data </summary>
        public static readonly Guid Data708_608 = new Guid(0xaf414bc, 0x4ed2, 0x445e, 0x98, 0x39, 0x8f, 0x9, 0x55, 0x68, 0xab, 0x3c);

        /// <summary> MEDIASUBTYPE_DtvCcData </summary>
        public static readonly Guid DtvCcData = new Guid(0xF52ADDAA, 0x36F0, 0x43F5, 0x95, 0xEA, 0x6D, 0x86, 0x64, 0x84, 0x26, 0x2A);

    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity, 
    Guid("C6E13380-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAMCrossbar
    {
        [PreserveSig]
        int get_PinCounts(
            [Out] out int OutputPinCount,
            [Out] out int InputPinCount
            );

        [PreserveSig]
        int CanRoute(
            [In] int OutputPinIndex,
            [In] int InputPinIndex
            );

        [PreserveSig]
        int Route(
            [In] int OutputPinIndex,
            [In] int InputPinIndex
            );

        [PreserveSig]
        int get_IsRoutedTo(
            [In] int OutputPinIndex,
            [Out] out int InputPinIndex
            );

        [PreserveSig]
        int get_CrossbarPinInfo(
            [In, MarshalAs(UnmanagedType.Bool)] bool IsInputPin,
            [In] int PinIndex,
            [Out] out int PinIndexRelated,
            [Out] out PhysicalConnectorType PhysicalType
            );
    }

    /// <summary>Тип физического подключения</summary>
    public enum PhysicalConnectorType
    {
        /// <summary>Тюнер</summary>
        Video_Tuner = 1,
        /// <summary>Композитный вход</summary>
        Video_Composite,
        /// <summary>Вход S-Video</summary>
        Video_SVideo,
        /// <summary>Вход RGB</summary>
        Video_RGB,
        /// <summary>Вход YRYBY</summary>
        Video_YRYBY,
        /// <summary>Вход цифровой последовательный</summary>
        Video_SerialDigital,
        /// <summary>Вход параллельный цифровой</summary>
        Video_ParallelDigital,
        /// <summary>Вход SCSI</summary>
        Video_SCSI,
        /// <summary>Вход внешний</summary>
        Video_AUX,
        /// <summary>Вход FireWire</summary>
        Video_1394,
        /// <summary>Вход USB</summary>
        Video_USB,
        /// <summary>Вход видео декодер</summary>
        Video_VideoDecoder,
        /// <summary>Вход видео энкодер</summary>
        Video_VideoEncoder,
        /// <summary>Вход SCART</summary>
        Video_SCART,
        /// <summary>Вход Black</summary>
        Video_Black,

        /// <summary>Вход Audio тюнер</summary>
        Audio_Tuner = 0x1000,
        /// <summary>Вход Audio линейный</summary>
        Audio_Line,
        /// <summary>Вход микрофон</summary>
        Audio_Mic,
        /// <summary>Вход цифровой AES</summary>
        Audio_AESDigital,
        /// <summary>Вход цифровой SPDIF</summary>
        Audio_SPDIFDigital,
        /// <summary>Вход аудио цифровой SCSI</summary>
        Audio_SCSI,
        /// <summary>Вход аудио внешний</summary>
        Audio_AUX,
        /// <summary>Вход аудио FireWire</summary>
        Audio_1394,
        /// <summary>Вход аудио USB</summary>
        Audio_USB,
        /// <summary>Вход аудио декодер</summary>
        Audio_AudioDecoder,
    }

    /// <summary>Поиск направления потока данных</summary>
    sealed public class FindDirection
    {
        private FindDirection()
        {
            // Prevent people from trying to instantiate this class
        }

        /// <summary> LOOK_UPSTREAM_ONLY </summary>
        public static readonly Guid UpstreamOnly = new Guid(0xac798be0, 0x98e3, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);

        /// <summary> LOOK_DOWNSTREAM_ONLY </summary>
        public static readonly Guid DownstreamOnly = new Guid(0xac798be1, 0x98e3, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C6E13350-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAMAnalogVideoDecoder
    {
        [PreserveSig]
        int get_AvailableTVFormats([Out] out AnalogVideoStandard lAnalogVideoStandard);

        [PreserveSig]
        int put_TVFormat([In] AnalogVideoStandard lAnalogVideoStandard);

        [PreserveSig]
        int get_TVFormat([Out] out AnalogVideoStandard plAnalogVideoStandard);

        [PreserveSig]
        int get_HorizontalLocked([Out, MarshalAs(UnmanagedType.Bool)] out bool plLocked);

        [PreserveSig]
        int put_VCRHorizontalLocking([In, MarshalAs(UnmanagedType.Bool)] bool lVCRHorizontalLocking);

        [PreserveSig]
        int get_VCRHorizontalLocking([Out, MarshalAs(UnmanagedType.Bool)] out bool plVCRHorizontalLocking);

        [PreserveSig]
        int get_NumberOfLines([Out] out int plNumberOfLines);

        [PreserveSig]
        int put_OutputEnable([In, MarshalAs(UnmanagedType.Bool)] bool lOutputEnable);

        [PreserveSig]
        int get_OutputEnable([Out, MarshalAs(UnmanagedType.Bool)] out bool plOutputEnable);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C6E13360-30AC-11d0-A18C-00A0C9118956"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMVideoProcAmp
    {
        [PreserveSig]
        int GetRange(
            [In] VideoProcAmpProperty Property,
            [Out] out int pMin,
            [Out] out int pMax,
            [Out] out int pSteppingDelta,
            [Out] out int pDefault,
            [Out] out VideoProcAmpFlags pCapsFlags
            );

        [PreserveSig]
        int Set(
            [In] VideoProcAmpProperty Property,
            [In] int lValue,
            [In] VideoProcAmpFlags Flags
            );

        [PreserveSig]
        int Get(
            [In] VideoProcAmpProperty Property,
            [Out] out int lValue,
            [Out] out VideoProcAmpFlags Flags
            );
    }


}
