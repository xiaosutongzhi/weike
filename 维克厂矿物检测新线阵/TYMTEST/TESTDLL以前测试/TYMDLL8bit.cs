#define _CORRECT
#define _QUEUE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TYMDetector;
using System.Numerics;
using static TYMDLL.TYMPANGU;
using TYMDLL;
//#define _WHITE_BALANCE
//#define _BLOCK
//#define _RELEASE // 使能tra catch
//#define _SINGLE //_SINGLE表示单能模式


namespace TYMDLL8B
{
    public class TYMPANGU8B : TYMCard
    {
        private static object locker = new object();

        static int instanceId_PANGU = -1;
        //here
        static public TYMPANGU8B TYMCardFactory(int imgHeight=512, int deathHeight=200)
        {
            return new TYMPANGU8B(imgHeight, deathHeight);
        }

        #region Base
        //###新的变量here
        private bool connected_PANGU = false;
        private int restart_num = 5;


        const uint channelPortsNum_1 = 5;
        public virtual uint Channel1s { get { return channelPortsNum_1; } }

        const uint channelPortsNum_2 = 5;
        public virtual uint Channel2s { get { return channelPortsNum_2; } }

        const uint cardSum = channelPortsNum_1 + channelPortsNum_2;
        public virtual uint CardSum { get { return cardSum; } }

        const uint pixelNum = 128;
        public virtual uint PixelNum { get { return pixelNum; } }

        const int gain = 60;//1-63
        public virtual int Gain { get { return gain; } }

        const uint deepth = 8;
        public virtual uint Deepth { get { return deepth; } }//像素深度，就是一个像素多少字节



        //public bool Inverse = true;

        public PositonType ImgPosiontType = PositonType.Inverse;

        public GrayType ImgGrayType = GrayType.Common;

        //public bool ItemTexture = false;



        internal bool isInitFinished = false;//初始化完成标志，完成后才真正开始采样
        public bool IsInitFinished
        {
            get { return isInitFinished; }
        }
        private int jumpCunts = 0;
        public void IgnoreLines(int num)
        {
            jumpCunts = num;
            isInitFinished = false;
        }


        public TYMPANGU8B(int imageHeight = 512, int deathHeight = 0,int integral=1000)
        {
            imgHeight = imageHeight;
            deadHeight = deathHeight;
            integral_time = integral;
        }

        ~TYMPANGU8B()
        {
            DisConnect();
        }
        #endregion


        //here
        #region connect
        SdkInterface.tymfn_datacallback callback;
        public void Connect(string host, string remote, int recvPort, int sendPort, int remoteControlPort, int remoteRecallPort)
        {
            //创建底层通信实例
            SdkInterface.tymscan_init(host, remote, remoteControlPort, remoteRecallPort, ref instanceId_PANGU);
            SdkInterface.tymscan_connected(ref connected_PANGU, instanceId_PANGU);

            //连接硬件，建立回调
            while (!connected_PANGU&&restart_num != 0)
            {
                SdkInterface.tymscan_connected(ref connected_PANGU, instanceId_PANGU);
                restart_num--;
                Console.WriteLine("Connect Error |  RECONNECT...");
            }

            SdkInterface.tymscan_set_gain_low(0, gain, instanceId_PANGU); // 设置低能增益
#if !_SINGLE
            SdkInterface.tymscan_set_gain_high(0, gain, instanceId_PANGU); // 设置高能增益
#endif
            SdkInterface.tymscan_set_integral_time(integral_time, instanceId_PANGU); // 设置积分时间
            callback = new SdkInterface.tymfn_datacallback(PraseData);
            SdkInterface.tymscan_set_datacallback(callback, IntPtr.Zero, imgHeight, instanceId_PANGU);//32行数据存放回调一次

          
            Start();
        }

        public void PraseData(IntPtr bufferPtr, IntPtr extra_info, IntPtr user_data, int instance)
        {
            if (instance == instanceId_PANGU)
            {
                // 将缓冲区指针转换为可用的结构体
                // 注意：需要根据实际结构体定义调整
                TymdataBuffer buffer = Marshal.PtrToStructure<TymdataBuffer>(bufferPtr);//marshal的内置函数
                //获取数据，不用解析了，直接放到buffer队列里面
                Handle(buffer);
            }
        }

        // 定义数据缓冲区结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct TymdataBuffer//结构体成员排列要符合文档
        {
            public int line_num;              // 行数
            public int pixels_per_line;       // 每行像素数
            public int bytes_per_pixel;       //像素深度
            public int bytes_per_line;        // 每行字节数
            public IntPtr data;               // 图像数据指针

        }


        public class A_linedata
        {

            private byte[] byteArray;
            private DateTime time;
            private int Length;//字节长度
            private int dataindex;//第几行
            public A_linedata()
            {
                byteArray = new byte[0];  // 初始化为空数组
                time = DateTime.MinValue;  // 初始化时间为最小时间
            }
            public A_linedata(byte[] byteData, int idx)
            {
                byteArray = byteData;
                Length = byteArray.Length;
                time = DateTime.Now;
                dataindex = idx;
            }
            public DateTime GetTime()
            {
                return time;
            }
            public byte[] Getbytedata()
            {
                return byteArray;
            }
           
            public int Getlen()
            {
                return Length;
            }
            public int Getdataidx()
            {
                return dataindex;
            }
        }

        public void Handle(TymdataBuffer buf)
        {
            const int BIT_START = 3;  // 从0开始计数的起始位（对应第4位）

            int height = buf.line_num;
            int width = buf.pixels_per_line; // 应该使用像素数而非字节数
            if (height != 512)
            {
                Console.WriteLine("数据高度不对，应为512行");
                return;
            }

            if (buf.bytes_per_line != width * 2)
            {
                Console.WriteLine($"行字节数不匹配，应为{width * 2}，实际{buf.bytes_per_line}");
                return;
            }

            for (int i = 0; i < height; i++)
            {
                // 获取当前行原始数据
                byte[] rawLine = new byte[buf.bytes_per_line];
                IntPtr lineAddress = IntPtr.Add(buf.data, i * buf.bytes_per_line);
                Marshal.Copy(lineAddress, rawLine, 0, buf.bytes_per_line);


                byte[] processedLine = new byte[width];
                for (int j = 0; j < width; j++)
                {
                    ushort pixel = (ushort)(rawLine[j * 2] | (rawLine[j * 2 + 1] << 8));
                    processedLine[j] = (byte)((pixel >> BIT_START) & 0xFF);//左移3位然后并ff
                }

                //here归一化，这里使用提前计算好的min和scale
                A_linedata lineData = new A_linedata(processedLine, i);
                lock (locker)
                {
                    msgBuffers.Enqueue(lineData);
                }
            }
        }


        // 处理图像数据的函数

        public void DisConnect()
        {
            //stopUdp();
            parseImgEnable = false;
            generateImgFlag = false;
            imgReady = false;
            SdkInterface.tymscan_cleanup(instanceId_PANGU); // 清理实例
        }

        #endregion



        #region DataParse

        #region 图像处理多线程
        Thread generateImgThread;
        bool generateImgFlag = false;
        Thread parseImgThread;
        bool parseImgEnable = false;
        bool imgReady = false;
#if _BLOCK
        ManualResetEventSlim mainParseBlocker = new ManualResetEventSlim(false);
#endif
     
        int imgReadyTaskSourceFlag = -1;
        void imgParseMain2()
        {
#if _RELEASE
            try
            {
#endif
            while (parseImgEnable)
            {
#if _QUEUE
#if _BLOCK
                    mainParseBlocker.Wait();
                    mainParseBlocker.Reset();
#endif
                //Console.WriteLine("Get One Img");
                XImgFrame frame = null;
                XImgFrame rawFrame = null;
                XImgFrame lowPowerFrame = null;
                XImgFrame rawLowPowerFrame = null;
                //Console.WriteLine("ASYNC get One Result :{0}", imgReadyTaskResult);
                if (imgReadyTaskSourceFlag >= 0)
                {
                    imgReadyTaskSourceFlag = -1;
                    maxTime = TimeSpan.MinValue;
                    frame = maskBuffer != null ? (XImgFrame)maskBuffer : null;
                    rawFrame = imgBuffer != null ? (XImgFrame)imgBuffer : null;
                    lowPowerFrame = lowPowerMaskBuffer != null ? (XImgFrame)lowPowerMaskBuffer : null;
                    rawLowPowerFrame = lowPowerImgBuffer != null ? (XImgFrame)lowPowerImgBuffer : null;
#if _Test
                    Console.WriteLine("{0} Clear之前：{1}", frameBufferIndex, frame.Height);
#endif
#if _Test
                    Console.WriteLine("{0} Clear之后：{1}", frameBufferIndex, frame.Height);
                    Console.WriteLine();
#endif


                    if (frame != null || rawFrame != null || lowPowerFrame != null || rawLowPowerFrame != null)
                    {
                        XImgGroup frames = new XImgGroup(new XImgFrame[4] { (XImgFrame)frame,
                                                                        (XImgFrame)rawFrame,
                                                                        (XImgFrame)lowPowerFrame,
                                                                        (XImgFrame)rawLowPowerFrame});
                        int[] showImg = { 0, 0, 0, 0 };
                        for (int i = 0; i < 4; i++)
                        {
                            if (frames.Content[i] != null)
                            {
                                showImg[i] = 1;
                            }
                        }
                        Console.WriteLine("Get One img:{0},{1},{2},{3}", showImg[0], showImg[1], showImg[2], showImg[3]);
                        HxCardFrameReady?.Invoke(this, frames);
                    }

                }

#else
                if (!(buffeReadyFlag < 0) && buffeReadyFlag != trick && imgReady) 
                {
                    trick = buffeReadyFlag;
                    maxTime = TimeSpan.MinValue;
                    XImgFrame frame = ImgFrames[trick];
                    XImgFrame rawFrame = ImgFrames[trick + 2];
                    XImgFrame[] frames = new XImgFrame[2] { (XImgFrame)frame.Clone(), (XImgFrame)rawFrame.Clone() };
#if _Test
                    Console.WriteLine("{0} Clear之前：{1}", frameBufferIndex, frame.Height);
#endif
                    HxCardFrameReady(frames);
                    Console.WriteLine("高：{0}，宽：{1}", frames[0].Height, frames[0].Width);
                    frame.Clear();
                    rawFrame.Clear();
#if _Test
                    Console.WriteLine("{0} Clear之后：{1}", frameBufferIndex, frame.Height);
                    Console.WriteLine();
#endif
                }
#endif


            }
#if _RELEASE
            }
            catch(Exception e)
            {
                string msg = " Line Img Sender Round Err: " + e.Message;
                Console.WriteLine(msg);
                ErrLog.Recorder.AddErrMsg(msg);
                ErrLog.Recorder.AddErrMsg(e.StackTrace);
                ErrLog.Recorder.AddErrMsg("========");
                ErrLog.Recorder.WriteMsg();
            }
#endif
        }
        private Stopwatch mysw = Stopwatch.StartNew();
        private long lastTick = -1;
        private string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "frame_interval_log233.txt");

        void generateMain2()
        {
            mysw.Start();
            while (generateImgFlag)
            {
                try
                {
                    if (msgBuffers.Count > 0)
                    {
                        long currentTick = mysw.ElapsedTicks;

                        if (lastTick >= 0)
                        {
                            double microseconds = (currentTick - lastTick) * (1_000_000.0 / Stopwatch.Frequency);
                            try
                            {
                                File.AppendAllText(logPath, $"{microseconds:0.000} us\n");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("写文件失败：" + ex.Message);
                            }
                        }

                        lastTick = currentTick;

                        A_linedata e = new A_linedata();
                        lock (locker)
                        {
                            e = msgBuffers.Dequeue();
                        }
                        int dataIndex = e.Getdataidx();

                        var msg_time = e.GetTime();
                        if (dataBuffer.Length < 1)
                            continue;
                        if (dataBuffer.Length != dataBufferBackground.Length)
                        {
                            Console.WriteLine("here一行数据不齐全？？？");
                            continue;
                        }


                        int result = 0;
                        if (dataBuffer.Length > 0)
                        {
                            jumpCunts--;//跳过数据，用户层调用ignore
                            if (jumpCunts <= 0)
                            {
                                isInitFinished = true;
                                jumpCunts = 0;
                            }

                            if (IsInitFinished)
                            {
                                //sw.Start();
                                byte[] highPowerDataBuffer = new byte[CardSum * PixelNum];//10*64*2*2=2560byte一行（双能再*2）.先算他20张卡
                                uint highPowerStartIndex = CardSum * PixelNum;
                                for (int i = 0; i < CardSum; i++)
                                {
#if !_SINGLE
                                    Array.Copy(dataBuffer, highPowerStartIndex + (i * PixelNum), highPowerDataBuffer, i * PixelNum, PixelNum);
#else
                                    Array.Copy(dataBuffer, i * PixelNum , highPowerDataBuffer, i * PixelNum , PixelNum );
#endif
                                }
                                frameParese(highPowerDataBuffer, msg_time, ref imgBuffer, ref maskBuffer, ref imgFrames,
                                    ref heighFrameBufferIndex, ref heighBuffeReadyFlag, ref result,
                                    ref eachLinesThreshold, ref eachConut, ref threshHighHeight, dataIndex);
#if !_SINGLE
                                byte[] lowPowerDataBuffer = new byte[CardSum * PixelNum];
                                for (int i = 0; i < CardSum; i++)
                                {
                                    Array.Copy(dataBuffer, i * PixelNum, lowPowerDataBuffer, i * PixelNum, PixelNum);
                                }
                                frameParese(lowPowerDataBuffer, msg_time, ref lowPowerImgBuffer, ref lowPowerMaskBuffer,
                                    ref lowPowerImgFrames, ref lowFrameBufferIndex, ref lowBuffeReadyFlag, ref result,
                                    ref eachLowLinesThreshold, ref eachLowConut, ref threshLowHeight, dataIndex);
#endif
                                if (result >= 1)
                                {
                                    imgReadyTaskSourceFlag = result;
                                    mysw.Stop();
                                    TimeSpan elapsed = mysw.Elapsed;
                                    Console.WriteLine($"一帧图片所需时间{elapsed.TotalMilliseconds:F3}ms");
                                    mysw.Restart();


                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrLog.Recorder.AddErrMsg(ex.Message);
                    ErrLog.Recorder.AddErrMsg(ex.StackTrace);
                    ErrLog.Recorder.AddErrMsg("========");
                    ErrLog.Recorder.WriteMsg();
                }
            }

        }
        #endregion

        #region Child Member
        public struct XFrameLine
        {
            public int DataIndex;
            public byte[] Data;
        }

        public struct MsgBuffer
        {
            public byte[] Buffer;
            public DateTime Time;
            public MsgBuffer(byte[] msg, DateTime time)
            {
                Buffer = msg;
                Time = time;
            }
        }

        //public struct SrcLine
        //{
        //    public SrcLine(byte[] high, byte[] low, int index, DateTime time)
        //    {
        //        High = new byte[high.Length];
        //        Array.Copy(high, High, high.Length);
        //        Low = new byte[low.Length];
        //        Array.Copy(low, High, low.Length);
        //        Index = index;
        //        Time = time;
        //    }
        //    public byte[] High;
        //    public byte[] Low;
        //    public int Index;
        //    public DateTime Time;
        //}

        public enum PositonType
        {
            Common = 1, Inverse = 2
        }
        public enum GrayType
        {
            Common = 1, Inverse = 2, CommonByTexture = 3, InverseByTexture = 4, OnlyBackgourdInverse = 5
        }

        #region OutPutDataStruct

        public class XImgFrame : ICloneable
        {
            bool firstFrame = true;
            Stopwatch sw = new Stopwatch();
            byte[] data;
            uint width = 0;
            uint pixelDepth = 8;
            uint[] lineIndexList;
            DateTime headTime;
            uint roiHeight;
            uint deadHeight;
            public bool First
            {
                get
                {
                    return firstFrame;
                }
                set
                {
                    firstFrame = value;
                }
            }
            public DateTime HeadTime
            {
                get
                {
                    return headTime;
                }
            }
            public byte[] Data
            {
                get
                {
                    return data;
                }
            }
            public uint[] LineIndexList
            {
                get
                {
                    return lineIndexList;
                }
            }
            public uint DataOffset
            {
                get; set;
            }
            public uint Size
            {
                get; set;
            }
            public uint PixelDepth
            {
                get
                {
                    return pixelDepth;
                }
            }
            public uint Width
            {
                get
                {
                    return width;
                }
            }

            public uint height;
            public uint Height
            {
                get
                {
                    return height;
                }
            }
            public uint RoiHeight
            {
                get
                {
                    return roiHeight;
                }
            }
            public uint DeadHeight
            {
                get
                {
                    return deadHeight;
                }
            }
            public uint MaxHeight
            {
                get
                {
                    return roiHeight + deadHeight;
                }
            }
            //int frameIndex = 0;
            private void setHeadTime()
            {
                headTime = System.DateTime.Now;
                //Console.WriteLine("ImgIndex{0},ImgTime{1}", frameIndex, (long)(headTime - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds);
                //frameIndex++;
            }
            public XImgFrame(uint _height, uint _deadHeight, uint _width, IntPtr _data, uint _pixelDepth = 8)
            {
                pixelDepth = _pixelDepth;
                roiHeight = _height;
                deadHeight = _deadHeight;
                width = _width;
                lineIndexList = new uint[roiHeight + deadHeight];
                height = 0;
                bufferLine.Data = new byte[width];
                data = new byte[MaxHeight * _width];
                Marshal.Copy(_data, data, 0, (int)((MaxHeight * _width) * (_pixelDepth / 8)));
            }

            public XImgFrame(uint _height, uint _deadHeight, uint _width, IntPtr _data, DateTime _headTime, uint _pixelDepth = 8)
            {
                headTime = _headTime;
                pixelDepth = _pixelDepth;
                roiHeight = _height;
                deadHeight = _deadHeight;
                width = _width;
                lineIndexList = new uint[roiHeight + deadHeight];
                height = 0;
                bufferLine.Data = new byte[width];
                data = new byte[MaxHeight * _width];
                Marshal.Copy(_data, data, 0, (int)((MaxHeight * _width) * (_pixelDepth / 8)));
            }

            public XImgFrame(uint _height, uint _deadHeight, uint _width, uint _pixelDepth = 8)
            {

                pixelDepth = _pixelDepth;
                roiHeight = _height;
                deadHeight = _deadHeight;
                width = _width;
                lineIndexList = new uint[roiHeight + deadHeight];
                height = 0;
                bufferLine.Data = new byte[width];
                data = new byte[MaxHeight * _width];
            }
            XFrameLine bufferLine = new XFrameLine();

            public int AddLine(byte[] lineData, int index, DateTime time)
            {
                int ret = -1;
                //bufferLine.Data = lineData;
                bufferLine.DataIndex = index;

                //Console.WriteLine($"{height}");
                if (height >= MaxHeight)
                {
                    Clear();
                }

                //这个目前不存在丢失帧请款，这个不是我们自己解包的，里面控制好打包了。等出现了丢失帧情况再补充，return 2补充行的时候出发缓冲满

                if (height < 1)
                    headTime = time;
                //setHeadTime();
                lineIndexList[height] = (uint)index;//Add((uint)index);
                height += 1;
                lineData.CopyTo(data, (height - 1) * width);
                lineData.CopyTo(bufferLine.Data, 0);
                if (height >= RoiHeight && height < MaxHeight)
                {
                    ret = 0;
                    //开始补充死区
                }
                else if (height >= MaxHeight)
                {
                    ret = 1;
                }

                return ret;
            }
            public uint GetPixelVal(uint row, uint col)
            {
                return data[row * width + col];
            }
            public void SetPixelVal(uint row, uint col, uint value)
            {

            }
            public int Save(string file)
            {
                return 0;
            }
            public void Clear()
            {
                //Console.WriteLine("Height:{0}", Height);

                //lineIndexList.Clear();
                lineIndexList = new uint[deadHeight + roiHeight];
                height = 0;
                //data = new byte[MaxHeight * Width];

            }
            public object Clone()
            {
                XImgFrame retClone = new XImgFrame(roiHeight, deadHeight, width, PixelDepth);
                retClone.headTime = headTime;
                retClone.lineIndexList = new uint[deadHeight + roiHeight];
                Array.Copy(lineIndexList, retClone.lineIndexList, lineIndexList.Length);
                retClone.height = height;
                retClone.data = new byte[data.Length];
                //data.CopyTo(retClone.data, 0);
                Array.Copy(data, retClone.data, retClone.data.Length);
                return retClone;
            }
        }

        public class XImgGroup : EventArgs
        {
            public XImgFrame[] Content;
            public XImgGroup(XImgFrame[] frames)
            {
                Content = frames;
            }
        }

        XImgFrame imgBuffer;
        XImgFrame maskBuffer;
        XImgFrame lowPowerImgBuffer;
        XImgFrame lowPowerMaskBuffer;


        #endregion

        public void RegisterImgCallback(EventHandler e)
        {
            HxCardFrameReady += e;
        }

        /// <summary>
        /// 图像传参委托
        /// </summary>
        /// <param name="img"></param>
        public delegate void XFrameReady(XImgFrame[] img);

        /// <summary>
        /// 图像准备完毕事件
        /// </summary>
        public event EventHandler HxCardFrameReady;
        private int imgHeight = 512;
        private int deadHeight = 0;
        private int integral_time = 0;
        private int heighBuffeReadyFlag = -1;
        private int lowBuffeReadyFlag = -1;
        public int ImgHeight
        {
            get
            {
                return imgHeight;
            }
        }
        public int DeathHeight
        {
            get
            {
                return deadHeight;
            }
        }
        XImgFrame[] imgFrames;//图像帧缓存，数组长度大于等于2，图像[1]缓存和图像[2]处理同时进行
        XImgFrame[] lowPowerImgFrames;//图像帧缓存，数组长度大于等于2，图像[1]缓存和图像[2]处理同时进行
        byte[] dataBuffer = new byte[0];
        byte[] dataBufferBackground = new byte[0];
        byte[] airValues = new byte[0];//用于校验统一颜色
        byte[] backgroundValues = new byte[0];
        float[] DValues = new float[0];   //校验系数
        int X = 0;
        int dataBufferDataTail = 0;
        int frameLineCounter = 0;//图像帧行组合计数器
        int heighFrameBufferIndex = 0;
        int lowFrameBufferIndex = 0;
        int preIndex = 2;


        #endregion
        static int threshline_data = 2560;//一行数据的像素个数，数组的大小刚好是一行的，用于每列自适应像素阈值调整（直接获取物块图像）
        long iq = 0;
        TimeSpan maxTime = TimeSpan.MinValue;
        long[] eachLinesThreshold = new long[threshline_data];
        long[] eachConut = new long[threshline_data];
        long[] eachLowLinesThreshold = new long[threshline_data];
        long[] eachLowConut = new long[threshline_data];
        int threshHighHeight = 0;
        int threshLowHeight = 0;
        /// <summary>
        /// 设置每列的自适应阈值
        /// </summary>
        /// <param name="lineData"></param>
        /// <param name="index"></param>
        void setThreshold(byte lineData, int index, long[] thresholdList, long[] countList)
        {
            //eachLinesThreshold[index] += lineData;
            //eachConut[index] += 1;
            thresholdList[index] += lineData;
            countList[index] += 1;
        }

        bool checkCmdResponse(byte[] msg, string cmd)
        {
            string response = System.Text.Encoding.UTF8.GetString(msg);
            if (response == cmd)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Stopwatch sw = new Stopwatch();
        int lossCatch = 0;
        //Queue<SrcLine> srcLines = new Queue<SrcLine>();
        Queue<A_linedata> msgBuffers = new Queue<A_linedata>();


        void frameParese(byte[] dataBuffer, DateTime time, ref XImgFrame imgBuffer, ref XImgFrame maskBuffer,
    ref XImgFrame[] ImgFrames, ref int frameBufferIndex, ref int buffeReadyFlag, ref int taskResult,
    ref long[] thresholdList, ref long[] countList, ref int threshHeight, int dataIndex)
        {
#if _RELEASE
            try
            {
#endif
            //Console.WriteLine($"databuffer的长度{dataBuffer.Length}");//这里是1280高能或者低能
            //Console.WriteLine($"当前行高{dataIndex}");

            XFrameLine dataMember = new XFrameLine();
            dataMember.DataIndex = dataIndex;
            XFrameLine rawDataMember = new XFrameLine();
            rawDataMember.DataIndex = dataIndex;
            byte[] lsHightPower = new byte[dataBuffer.Length];
            byte[] lsRawHightPower = new byte[dataBuffer.Length];
            if (threshHeight < ImgHeight / 2)                        
                threshHeight++;

            for (int i = 0; i < lsHightPower.Length; i++)
            {

                byte pixelValue = 0;
                byte background = byte.MaxValue;
                //byte rawPixelValue = 0;
                //byte rawBackground = Byte.MaxValue;
                switch (ImgGrayType)
                {
                    case GrayType.Common:
                        pixelValue = dataBuffer[i];
                        background = byte.MaxValue;
                        break;
                    case GrayType.Inverse:
                        pixelValue = (byte)(byte.MaxValue - dataBuffer[i]);
                        background = 0;
                        break;
                    case GrayType.CommonByTexture:
                        pixelValue = 0;
                        background = byte.MaxValue;
                        break;
                    case GrayType.InverseByTexture:
                        pixelValue = byte.MaxValue;
                        background = 0;
                        break;
                    case GrayType.OnlyBackgourdInverse:
                        pixelValue = dataBuffer[i];
                        background = 0;
                        break;
                }



                if (threshHeight < ImgHeight / 2)
                {
                    switch (ImgPosiontType)
                    {
                        case PositonType.Common:
                            lsHightPower[i] = dataBuffer[i];
                            lsRawHightPower[i] = dataBuffer[i];
                            setThreshold(lsHightPower[i], i, thresholdList, countList);
                            break;
                        case PositonType.Inverse:
                            lsHightPower[lsHightPower.Length - 1 - i] = dataBuffer[i];
                            lsRawHightPower[lsHightPower.Length - 1 - i] = dataBuffer[i];
                            setThreshold(lsHightPower[lsHightPower.Length - 1 - i], i, thresholdList, countList);
                            //if(lsHightPower.Length - 1 - i == 0 && thresholdList == eachLinesThreshold)
                            //    Console.WriteLine("IsHightPower:" + lsHightPower[lsHightPower.Length - 1 - i]);
                            break;
                    }
                }
                else
                {//255-(120-100)
                    var a = threshHeight;
                    switch (ImgPosiontType)
                    {
                        case PositonType.Common:

                            lsHightPower[i] = dataBuffer[i] > thresholdList[i] / threshHeight * 0.9 ? background : pixelValue;//每列自适应阈值控制，直接区分出来物块
#if _WHITE_BALANCE
                                    lsRaw[i] = dataBuffer[i]> thresholdList[i] / threshHeight * 0.9 ? (byte)255 : (byte)(255 - (eachLinesThreshold[i] / threshHeight - dataBuffer[i ]));
#else
                            lsRawHightPower[i] = dataBuffer[i];
#endif
                            break;
                        case PositonType.Inverse:

                            lsHightPower[lsHightPower.Length - 1 - i] = dataBuffer[i] > thresholdList[i] / threshHeight * 0.9 ? background : pixelValue;
#if _WHITE_BALANCE
                                    lsRaw[ls.Length - 1 - i] = dataBuffer[i]> thresHoldList[i] / threshHeight * 0.9 ? (byte)255:(byte)(255 - (eachLinesThreshold[i] / threshHeight - dataBuffer[i ]));
#else
                            lsRawHightPower[lsHightPower.Length - 1 - i] = dataBuffer[i];
#endif
                            break;
                    }
                }


            }
            //Console.WriteLine($"lsHightPower的长度{lsHightPower.Length}");1088
            //Console.WriteLine($"lsRawHightPower的长度{lsRawHightPower.Length}");

            ////dataBuffer.CopyTo(ls, 0);
            dataMember.Data = lsHightPower;
            rawDataMember.Data = lsRawHightPower;//转为字节序，缺省为小端
            int ret = ImgFrames[frameBufferIndex].AddLine(dataMember.Data, dataMember.DataIndex, time);
            ImgFrames[frameBufferIndex + 2].AddLine(rawDataMember.Data, rawDataMember.DataIndex, time);
            int deadFrameBufferIndex = frameBufferIndex == 1 ? 0 : 1;
            if (ImgFrames[deadFrameBufferIndex].Height >= ImgFrames[deadFrameBufferIndex].RoiHeight &&
                ImgFrames[deadFrameBufferIndex].Height < ImgFrames[deadFrameBufferIndex].MaxHeight)
            {
                //Console.WriteLine("DataIndex_Height:{0}", ImgFrames[deadFrameBufferIndex].Height);
                int deadRet = ImgFrames[deadFrameBufferIndex].AddLine(dataMember.Data, dataMember.DataIndex, time);
                ImgFrames[deadFrameBufferIndex + 2].AddLine(rawDataMember.Data, rawDataMember.DataIndex, time);
                if (deadRet > 0)
                {

                    if (!ImgFrames[deadFrameBufferIndex + 2].First)
                    {
                        imgBuffer = (XImgFrame)ImgFrames[deadFrameBufferIndex + 2];
                        maskBuffer = (XImgFrame)ImgFrames[deadFrameBufferIndex];
                        taskResult++;
#if _BLOCK
                                mainParseBlocker.Set();
#endif
                    }
                    else
                    {
                        foreach (var fram in ImgFrames)
                        {
                            fram.First = false;
                        }
                    }
                    buffeReadyFlag = frameBufferIndex == 1 ? 0 : 1;

                    //Console.WriteLine("==================== Get one Img ====================");
                }
            }
            if (ret > -1)//ImgFrames[frameBufferIndex].Height >= ImgFrames[frameBufferIndex].MaxHeight)
            {

                //Console.WriteLine("===============");
                //Thread frameParesThreda = new Thread(FrameReadyEvent);
                //frameParesThreda.Start(ImgFrames[frameBufferIndex]);
                //FrameReadyEvent(ImgFrames[frameBufferIndex]);
                if (imgReady == false && frameBufferIndex != 0)
                {
                    imgReady = true;//第一帧用于白平衡，这里不向外部回调第一幅图像
                }
                frameBufferIndex = frameBufferIndex == 1 ? 0 : 1;
                //Console.WriteLine(frameBufferIndex);
                if (ret == 1)
                {

                    buffeReadyFlag = frameBufferIndex;
                    if (!ImgFrames[deadFrameBufferIndex + 2].First)
                    {
                        imgBuffer = ImgFrames[buffeReadyFlag + 2];
                        maskBuffer = ImgFrames[buffeReadyFlag];
                        taskResult++;

#if _BLOCK
                                mainParseBlocker.Set();
#endif
                    }
                    else
                    {
                        foreach (var fram in ImgFrames)
                        {
                            fram.First = false;
                        }
                    }

                }
                if (ret == 2)                                                                           //丢帧的时候正好跨帧，将新的帧移到下一帧。
                {
                    ImgFrames[frameBufferIndex].AddLine(dataMember.Data, dataMember.DataIndex, time);
                    ImgFrames[frameBufferIndex + 2].AddLine(rawDataMember.Data, rawDataMember.DataIndex, time);

                    buffeReadyFlag = frameBufferIndex;
                    if (!ImgFrames[deadFrameBufferIndex + 2].First)
                    {
                        imgBuffer = ImgFrames[buffeReadyFlag + 2];
                        maskBuffer = ImgFrames[buffeReadyFlag];
                        taskResult++;

#if _BLOCK
                                mainParseBlocker.Set();
#endif
                    }
                    else
                    {
                        foreach (var fram in ImgFrames)
                        {
                            fram.First = false;
                        }
                    }
                }
                dataBuffer = new byte[0];
                dataBufferDataTail = 0;
                ////Console.WriteLine("ImgHeight:{0},ImgWidth:{1}",ImgFrames[0].Height,dataMember.Data.Length);
            }
#if _RELEASE
            }
            catch(Exception e)
            {
                Console.WriteLine(String.Format("ImgFrames:{0},dataBuffer:{1}",ImgFrames==null,dataBuffer==null));
                string msg = " Line Img Sender Round Err: " + e.Message;
                Console.WriteLine(msg);
                ErrLog.Recorder.AddErrMsg(msg);
                ErrLog.Recorder.AddErrMsg(e.StackTrace);
                ErrLog.Recorder.AddErrMsg("========");
                ErrLog.Recorder.WriteMsg();
            }
#endif
        }


        List<List<TimeSpan>> timeLog = new List<List<TimeSpan>>();

        public void debugTimeSpan()
        {
            using (FileStream fs = new FileStream("./timeSpanLog.csv", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < 5000; i++)
                    {
                        string msg = "";
                        var timeSpanList = timeLog[i];
                        foreach (var ms in timeSpanList)
                        {
                            string m = ms.ToString();
                            if (m.Length > 8)
                                m = m.Substring(9, 7);
                            else
                                m = "0";
                            msg += m + ",";
                        }
                        //string msg = String.Format("{0},{1},{2}", m1, m2, m3);
                        sw.WriteLine(msg.Substring(0, msg.Length - 1));
                    }
                }
            }
        }

        //TimeSpan maxTime1 = TimeSpan.MinValue;
        //TimeSpan maxTime2 = TimeSpan.MinValue;
        //TimeSpan maxTime3 = TimeSpan.MinValue;
        void ParseData(byte[] src, out int dataIndex, out int packNum, out int packIndex, out byte[] dataLine)
        {
            //sw.Start();
            int head = 1;
            int prehead = head;
            int tail = head;
            //List<byte[]> parsedData = new List<byte[]>();
            dataIndex = 0;
            packNum = 0;
            packIndex = 0;
            dataLine = new byte[0];

            //sw.Stop();
            //var time1 = sw.Elapsed;
            //if (time1 > maxTime1)
            //{
            //    maxTime1 = time1;
            //}
            //TimeSpan time2 = TimeSpan.Zero;
            //TimeSpan time3 = TimeSpan.Zero;
            //sw.Restart();

            while (head < src.Length)
            {
                if (src[head - 1] == 0xff && src[head] == 0xff)//找表头
                {
                    prehead = head - 1;
                    int dataLength = src[++head] * 0x10 + src[++head] / 0x10;//获取数据长度
                    //Console.WriteLine(dataLength);
                    packNum = src[head] % 0x10 + 1;
                    dataIndex = src[++head] * 0x10 + src[++head] / 0x10;
                    packIndex = src[head] % 0x10;
                    head += 8;//行编号后面还接了8个非数据字节，该8字节含义未在文档中找到说明

                    //sw.Stop();
                    //time2 += sw.Elapsed;
                    //sw.Restart();

                    tail = head;
                    head += dataLength;
                    //dataLine = src.Skip(tail).Take(dataLength).ToArray();
                    dataLine = new byte[dataLength];
                    Array.Copy(src, tail, dataLine, 0, dataLength);
                    //Console.WriteLine("packNum:{0},dataIndex:{1},packIndex:{2},dataLength{3}",
                    //    packNum,dataIndex,packIndex,dataLine.Length);
                    //foreach(byte c in man)
                    //{
                    //    Console.Write(c.ToString("x2"));
                    //}

                    //sw.Stop();
                    //time3 += sw.Elapsed;
                    //sw.Restart();
                }
                head++;
            }
            //sw.Stop();
            //if(time2 > maxTime2)
            //{
            //    maxTime2 = time2;
            //}
            //if(time3 > maxTime3)
            //{
            //    maxTime3 = time3;
            //}
            //Console.WriteLine(String.Format("Init Time:{0},Time2: {1},Time3:{2}", time1, time2, time3));
            //Console.WriteLine(String.Format("MaxTime1:{0},MaxTime2: {1},MaxTime3:{2}", maxTime1, maxTime2, maxTime3));
            //List<TimeSpan> timeSpans= new List<TimeSpan>();
            //timeSpans.Add(time1);
            //timeSpans.Add(time2);
            //timeSpans.Add(time3);
            //timeLog.Add(timeSpans);
            //sw.Reset();
        }

        void FrameReadyEvent(object img)
        {
            XImgFrame[] data = img as XImgFrame[];
            XImgGroup datas = new XImgGroup(data);
            lock (locker)
            {
                HxCardFrameReady(this, datas);
            }
        }

        void clearImgBuffer()
        {
            foreach (XImgFrame man in imgFrames)
            {
                man.Clear();
            }
            foreach (XImgFrame man in lowPowerImgFrames)
            {
                man.Clear();
            }
        }
        #endregion

        #region Controller
        public virtual void Init(int integrationTime)
        {

        }
        public virtual async Task<bool> AsyncInit(int integrationTime)
        {
            await Task.Delay(10);
            return true;
        }

        public void StopSampling()
        {
            SdkInterface.tymscan_grab_stop(instanceId_PANGU);
        }

        public void StartSampling()
        {
            SdkInterface.tymscan_grab_start(instanceId_PANGU);
        }

        public void SetIntegrationTime(int time)
        {

        }


        public void RefreshThresh()
        {
            eachLinesThreshold = new long[threshline_data];
            eachConut = new long[threshline_data];
            eachLowLinesThreshold = new long[threshline_data];
            eachLowConut = new long[threshline_data];
            threshHighHeight = 0;
            threshLowHeight = 0;
        }

        public void Reset()
        {
            SdkInterface.tymscan_grab_stop(instanceId_PANGU); // 停止采集
            SdkInterface.tymscan_cleanup(instanceId_PANGU); // 清理实例
            foreach (XImgFrame frame in lowPowerImgFrames)
                frame.Clear();
        }

        public void Start()
        {
            uint len = CardSum * PixelNum ;//卡数*像素数
            dataBufferBackground = new byte[len];
            Console.WriteLine($"dataBufferBackground的像素长度{len}");//1280才对
            for (int i = 0; i < len; i++)
            {
                dataBufferBackground[i] = byte.MaxValue;
            }
            imgFrames = new XImgFrame[4] { new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum , Deepth),
                                           new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum , Deepth),
                                           new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum  , Deepth),
                                           new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum, Deepth)  };
            lowPowerImgFrames = new XImgFrame[4] { new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum , Deepth),
                                                   new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum , Deepth),
                                                   new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum, Deepth),
                                                   new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum , Deepth)  };
            foreach (var frame in imgFrames)
            {
                frame.First = true;
            }


            generateImgFlag = true;
            generateImgThread = new Thread(generateMain2);
            generateImgThread.IsBackground = true;
            generateImgThread.Start();


            parseImgEnable = true;
            parseImgThread = new Thread(imgParseMain2);
            parseImgThread.IsBackground = true;
            parseImgThread.Start();
        }

        public void ChangeTextureValue(GrayType type)
        {
            ImgGrayType = type;
        }

        public static byte[] ReadBinaryTextFile(string filePath)
        {
            // 读取所有行
            string[] lines = File.ReadAllLines(filePath);

            // 计算总字节数（每行有 len/8 个字节，每字节占8位二进制+1空格）
            int bytesPerRow = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
            byte[] result = new byte[lines.Length * bytesPerRow];

            int byteIndex = 0;
            foreach (string line in lines)
            {
                // 分割每行的二进制字符串
                string[] binaryStrings = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // 将每个二进制字符串转回字节
                foreach (string binaryStr in binaryStrings)
                {
                    if (binaryStr.Length != 8)
                    {
                        throw new FormatException($"无效的二进制格式: {binaryStr}");
                    }
                    result[byteIndex++] = Convert.ToByte(binaryStr, 2);
                }
            }

            return result;
        }
        #endregion

    }
}
