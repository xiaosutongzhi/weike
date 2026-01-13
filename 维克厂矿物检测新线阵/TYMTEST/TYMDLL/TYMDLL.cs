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
using ERROR;
using TYMCARD;
using System.Collections.Concurrent;

//#define _WHITE_BALANCE
//#define _RELEASE // 使能tra catch
//#define _SINGLE //_SINGLE表示单能模式


namespace TYMDLL
{
    public class TYMPANGU : TYMCard
    {
        private static object locker = new object();

        static int instanceId_PANGU = -1;
        //here
        static public TYMPANGU TYMCardFactory(int imgHeight = 512, int deathHeight = 200)
        {
            return new TYMPANGU(imgHeight, deathHeight);
        }

        #region Base
        //###新的变量here
        private bool connected_PANGU = false;
        private int restart_num = 5;
        private int integral_time = 0;




        const uint channelPortsNum_1 = 5;
        public virtual uint Channel1s { get { return channelPortsNum_1; } }

        const uint channelPortsNum_2 = 4;
        public virtual uint Channel2s { get { return channelPortsNum_2; } }

        const uint channelPortsNum_3 = 4;
        public virtual uint Channel3s { get { return channelPortsNum_3; } }

        const uint channelPortsNum_4 = 4;
        public virtual uint Channel4s { get { return channelPortsNum_4; } }

        const uint cardSum = channelPortsNum_1 + channelPortsNum_2 + channelPortsNum_3 + channelPortsNum_4;
        public virtual uint CardSum { get { return cardSum; } }

        const uint pixelNum = 128;//和文档不匹配，得和他们软件初始化匹配，单能像素128，单卡像素256
        public virtual uint PixelNum { get { return pixelNum; } }

        const int gain = 16;//1-63
        public virtual int Gain { get { return gain; } }

        const uint deepth = 16;
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


        private TYMPANGU(int imageHeight = 512, int deathHeight = 0)
        {
            imgHeight = imageHeight;
            deadHeight = deathHeight;
        }

        ~TYMPANGU()
        {
            DisConnect();
        }
        #endregion


        //here
        #region connect
        SdkInterface.tymfn_datacallback callback;
        public void Connect(string host, string remote, int recvPort, int sendPort, int remoteControlPort, int remoteRecallPort)
        {
#if RELEASE
            try
            {
#endif

            //创建底层通信实例
            SdkInterface.tymscan_init(host, remote, remoteControlPort, remoteRecallPort, ref instanceId_PANGU);
            SdkInterface.tymscan_connected(ref connected_PANGU, instanceId_PANGU);

            //连接硬件，建立回调
            while (!connected_PANGU && restart_num != 0)
            {
                SdkInterface.tymscan_connected(ref connected_PANGU, instanceId_PANGU);
                restart_num--;
                Console.WriteLine("Connect Error |  RECONNECT...");
            }

            callback = new SdkInterface.tymfn_datacallback(PraseData);
            SdkInterface.tymscan_set_datacallback(callback, IntPtr.Zero, imgHeight, instanceId_PANGU);//32行数据存放回调一次
            Start();
#if RELEASE
        }
            catch (Exception ex)
            {
                string msg = " Line Img Sender Round Err: " + ex.Message;
                Console.WriteLine(msg);
                ErrLog.Recorder.AddErrMsg(msg);
                ErrLog.Recorder.AddErrMsg(ex.StackTrace);
                ErrLog.Recorder.AddErrMsg("========");
                ErrLog.Recorder.WriteMsg();
            }
#endif
        }

        public void PraseData(IntPtr bufferPtr, IntPtr extra_info, IntPtr user_data, int instance)
        {
            if (instance == instanceId_PANGU)
            {
#if RELEASE
                try
                {
#endif
                // 将缓冲区指针转换为可用的结构体
                TymdataBuffer buffer = Marshal.PtrToStructure<TymdataBuffer>(bufferPtr);
                uint total_lost_lines = 0;
                SdkInterface.tymscan_get_total_lost_lines(ref total_lost_lines, instance);

                if (total_lost_lines != 0)
                {
                    Console.WriteLine($"丢失{total_lost_lines}行");
                    string errorMsg = $"Line Img Sender Round Err: total lines lost - {total_lost_lines}";
                    //throw new DataLossException(errorMsg, total_lost_lines);
                }

                // 获取数据，放到buffer队列
                Handle(buffer);
#if RELEASE
                }
                catch (Exception ex)
                {
                    // 记录内部错误（如果有）
                    string msg = $"Error in PraseData: {ex.Message}";
                    Console.WriteLine(msg);
                    ErrLog.Recorder.AddErrMsg(msg);
                    ErrLog.Recorder.AddErrMsg("========");
                    ErrLog.Recorder.WriteMsg();

                }
#endif

            }
        }
        public class DataLossException : Exception
        {
            public uint LostLines { get; }

            public DataLossException(string message, uint lostLines)
                : base(message)
            {
                LostLines = lostLines;
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
            //public ushort[] Getushortdata()
            //{
            //    if (byteArray.Length % 2 != 0)
            //        throw new ArgumentException("字节数组长度必须是2的倍数");
            //    ushort[] pixels = new ushort[byteArray.Length / 2];
            //    for (int i = 0; i < pixels.Length; i++)
            //    {
            //        int byteIndex = i * 2;
            //        // 小端序：低位在前
            //        pixels[i] = (ushort)(byteArray[byteIndex] | (byteArray[byteIndex + 1] << 8));
            //    }
            //    return pixels;
            //}
            public ushort[] Getushortdata()
            {
                //if (byteArray.Length % 2 != 0)
                //    throw new ArgumentException("字节数组长度必须是2的倍数");

                ushort[] pixels = new ushort[byteArray.Length / 2];

                // 直接将 byteArray 的内存块复制到 pixels 数组中
                // 参数：源数组, 源偏移, 目标数组, 目标偏移, 总字节数
                Buffer.BlockCopy(byteArray, 0, pixels, 0, byteArray.Length);

                return pixels;
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
        public void SetGain(int gain)
        {
            if (gain < 1 || gain > 63)
            {
                throw new ArgumentOutOfRangeException("gain范围为1-63");
            }
            SdkInterface.tymscan_set_gain_low(0, gain, instanceId_PANGU);
#if !_SINGLE
            SdkInterface.tymscan_set_gain_high(0, gain, instanceId_PANGU);
#endif

        }

        public void Handle(TymdataBuffer buf)
        {
            int height = buf.line_num;
            int width = buf.bytes_per_line;
            if (height != 512) return;

            // 1. 准备一个列表装这512行
            List<A_linedata> batchLines = new List<A_linedata>(height);

            // 2. 循环读取，但先不锁
            for (int i = 0; i < height; i++)
            {
                IntPtr lineAddress = IntPtr.Add(buf.data, i * width);
                byte[] lineBytes = new byte[width];
                // 优化点：如果追求极致，这里应该用内存池复用 lineBytes，但目前先只优化锁
                Marshal.Copy(lineAddress, lineBytes, 0, width);

                batchLines.Add(new A_linedata(lineBytes, i));
            }

            lock (locker)
            {
                foreach (var line in batchLines)
                {
                    msgBuffers.Enqueue(line);
                }
            }
        }
        static void AnalyzePixelDistribution(byte[] pixelData)
        {
            // 1. 将字节数组转换为ushort数组（小端序）
            ushort[] pixels = new ushort[pixelData.Length / 2];
            for (int i = 0; i < pixels.Length; i++)
            {
                int offset = i * 2;
                pixels[i] = (ushort)(pixelData[offset] | (pixelData[offset + 1] << 8)); // 小端序解析
            }

            // 2. 初始化统计参数
            const int groupCount = 16;
            const int groupSize = ushort.MaxValue / groupCount + 1; // 4096 per group
            var stats = new (long count, long sum)[groupCount];

            // 3. 统计像素分布
            foreach (var pixel in pixels)
            {
                int groupIndex = pixel / groupSize;
                if (groupIndex >= groupCount) groupIndex = groupCount - 1; // 处理边界情况

                stats[groupIndex].count++;
                stats[groupIndex].sum += pixel;
            }

            // 4. 打印统计结果
            Console.WriteLine($"\n16位像素值分布统计（共 {pixels.Length} 像素）");
            Console.WriteLine("|-----------|------------|------------|------------|");
            Console.WriteLine("| 值范围     | 像素数量    | 占比(%)    | 组内均值   |");
            Console.WriteLine("|-----------|------------|------------|------------|");

            for (int i = 0; i < groupCount; i++)
            {
                int lowerBound = i * groupSize;
                int upperBound = (i + 1) * groupSize - 1;
                if (upperBound > ushort.MaxValue) upperBound = ushort.MaxValue;

                double percentage = (double)stats[i].count / pixels.Length * 100;
                double average = stats[i].count > 0 ? (double)stats[i].sum / stats[i].count : 0;

                Console.WriteLine($"| 0x{lowerBound:X4}-0x{upperBound:X4} " +
                                  $"| {stats[i].count,10} " +
                                  $"| {percentage,7:F2}% " +
                                  $"| {average,9:F1} |");
            }
            Console.WriteLine("|-----------|------------|------------|------------|");

            // 5. 附加统计（最小值、最大值、总均值）
            Console.WriteLine($"\n全局统计：");
            Console.WriteLine($"总像素均值: {pixels.Average(x => (int)x):F1}");
            Console.WriteLine($"最小像素值: 0x{pixels.Min():X4}");
            Console.WriteLine($"最大像素值: 0x{pixels.Max():X4}");
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
                XImgFrame frame = null;
                XImgFrame rawFrame = null;
                XImgFrame lowPowerFrame = null;
                XImgFrame rawLowPowerFrame = null;

                //Console.WriteLine("ASYNC get One Result :{0}", imgReadyTaskResult);
                if (imgReadyTaskSourceFlag >= 0)
                {
                    imgReadyTaskSourceFlag = -1;
                    maxTime = TimeSpan.MinValue;
                    Stopwatch sw1 = Stopwatch.StartNew();

                    if (_readyHighFrameIndices.TryDequeue(out int highIndex))
                    {
                        frame = imgFrames[highIndex] != null ? (XImgFrame)imgFrames[highIndex] : null;
                        rawFrame = imgFrames[highIndex + BUFFER_DEPTH] != null ? (XImgFrame)imgFrames[highIndex + BUFFER_DEPTH] : null;
                        Interlocked.Increment(ref frame.RefCount);

                    }

                    if (_readyLowFrameIndices.TryDequeue(out int lowIndex))
                    {
                        lowPowerFrame = lowPowerImgFrames[lowIndex] != null ? (XImgFrame)lowPowerImgFrames[lowIndex] : null;
                        rawLowPowerFrame = lowPowerImgFrames[lowIndex + BUFFER_DEPTH] != null ? (XImgFrame)lowPowerImgFrames[lowIndex + BUFFER_DEPTH] : null;
                    }


#if _Test
                    Console.WriteLine("{0} Clear之前：{1}", frameBufferIndex, frame.Height);
#endif
#if _Test
                    Console.WriteLine("{0} Clear之后：{1}", frameBufferIndex, frame.Height);
                    Console.WriteLine();
#endif


                    if (frame != null || rawFrame != null || lowPowerFrame != null || rawLowPowerFrame != null)
                    {
                        XImgGroup frames = new XImgGroup(new XImgFrame[4] { (XImgFrame)frame.Clone(),
                                                                        (XImgFrame)rawFrame.Clone(),
                                                                        (XImgFrame)lowPowerFrame.Clone(),
                                                                        (XImgFrame)rawLowPowerFrame.Clone()});
                        Interlocked.Decrement(ref frame.RefCount);


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
                        //c#托管机制无需手动释放资源，除了句柄，数据库链接，GDI对象等
                    }
                    sw1.Stop();
                    long tConsume = sw1.ElapsedMilliseconds;
                    Console.WriteLine($"消费者耗时{tConsume}ms");

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
        private ushort[] highPowerDataBuffer;
        private ushort[] lowPowerDataBuffer;
        private ConcurrentQueue<int> _readyHighFrameIndices = new ConcurrentQueue<int>();
        private ConcurrentQueue<int> _readyLowFrameIndices = new ConcurrentQueue<int>();

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
                        ushort[] dataBuffer = e.Getushortdata();

                        var msg_time = e.GetTime();
                        if (dataBuffer.Length < 1)
                            continue;
                        if (dataBuffer.Length != dataBufferBackground.Length * 2)
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
                                if (highPowerDataBuffer == null)
                                    highPowerDataBuffer = new ushort[CardSum * PixelNum];//17*128=1280pixel一行（双能再*2）.
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
                                    ref eachLinesThreshold, ref eachConut, ref threshHighHeight, dataIndex, ref _readyHighFrameIndices);
#if !_SINGLE
                                if (lowPowerDataBuffer == null)
                                    lowPowerDataBuffer = new ushort[CardSum * PixelNum];
                                for (int i = 0; i < CardSum; i++)
                                {
                                    Array.Copy(dataBuffer, i * PixelNum, lowPowerDataBuffer, i * PixelNum, PixelNum);
                                }
                                frameParese(lowPowerDataBuffer, msg_time, ref lowPowerImgBuffer, ref lowPowerMaskBuffer,
                                    ref lowPowerImgFrames, ref lowFrameBufferIndex, ref lowBuffeReadyFlag, ref result,
                                    ref eachLowLinesThreshold, ref eachLowConut, ref threshLowHeight, dataIndex, ref _readyLowFrameIndices);
#endif
                                if (result >= 1)
                                {
                                    //这用队列不行
                                    //if (maskBuffer != null || imgBuffer != null || lowPowerMaskBuffer != null || lowPowerImgBuffer != null)
                                    //{
                                    //    XImgGroup frames = new XImgGroup(new XImgFrame[4] { (XImgFrame)maskBuffer,
                                    //                                    (XImgFrame)imgBuffer,
                                    //                                    (XImgFrame)lowPowerMaskBuffer,
                                    //                                    (XImgFrame)lowPowerImgBuffer});
                                    //    groupQueue.Enqueue(frames);
                                    //    maskBuffer = null;
                                    //    imgBuffer = null;
                                    //    lowPowerMaskBuffer = null;
                                    //    lowPowerImgBuffer = null;
                                    //}
                                    //semaphore1.Wait(); 
                                    imgReadyTaskSourceFlag = result;
                                    //semaphore2.Release(); 

                                    mysw.Stop();
                                    TimeSpan elapsed = mysw.Elapsed;
                                    //Console.WriteLine($"一帧图片所需时间{elapsed.TotalMilliseconds:F3}ms");
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
            public int RefCount = 0;//引用计数，判断循环缓冲是否存在问题
            private readonly SemaphoreSlim dataSemaphore = new SemaphoreSlim(1, 1);
            bool firstFrame = true;
            Stopwatch sw = new Stopwatch();
            XFrameLine bufferLine = new XFrameLine();
            byte[] data;
            uint width = 0;
            uint pixelDepth = 16;
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
            public uint ByteWidth
            {
                get
                {
                    return width;
                }
            }
            public uint PixelWidth
            {
                get
                {
                    return width / (pixelDepth / 8);
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
            public XImgFrame(uint _height, uint _deadHeight, uint _width, IntPtr _data, uint _pixelDepth = 16)
            {
                pixelDepth = _pixelDepth;
                roiHeight = _height;
                deadHeight = _deadHeight;
                width = _width;
                lineIndexList = new uint[roiHeight + deadHeight];
                height = 0;
                bufferLine.Data = new byte[width];
                data = new byte[MaxHeight * _width];
                Marshal.Copy(_data, data, 0, (int)((MaxHeight * _width)));
            }

            public XImgFrame(uint _height, uint _deadHeight, uint _width, IntPtr _data, DateTime _headTime, uint _pixelDepth = 16)
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
                Marshal.Copy(_data, data, 0, (int)((MaxHeight * _width)));
            }

            public XImgFrame(uint _height, uint _deadHeight, uint _width, uint _pixelDepth = 16)
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

            public int AddLine(byte[] lineData, int index, DateTime time)
            {
                //dataSemaphore.Wait();
                int ret = -1;
                //bufferLine.Data = lineData;
                bufferLine.DataIndex = index;

                //Console.WriteLine($"{height}");
                if (height >= MaxHeight)
                {
                    Clear();
                }

                //这个目前不存在丢失帧情况，这个不是我们自己解包的，里面控制好打包了。等出现了丢失帧情况再补充，return 2补充行的时候出发缓冲满
                //if (height > 0 && index != lineIndexList[height - 1] + 1 &&
                //   !((index == 0) && (lineIndexList[height - 1] == 0xfff + 1)))
                //{
                //    for (uint i = lineIndexList[height - 1] + 1; i < (index == 0 ? MaxHeight : (uint)index); i++)
                //    {
                //        if (height >= MaxHeight)
                //        {
                //            Clear();
                //        }
                //        if (height < 1)
                //            //setHeadTime();
                //            headTime = time;
                //        lineIndexList[height] = i + 1;
                //        height += 1;
                //        //Console.WriteLine("丢失第{0}帧", lineIndexList[lineIndexList.Count - 1]);


                //        bufferLine.Data.CopyTo(data, (height - 1) * width);
                //        if (height == MaxHeight)
                //        {
                //            return 2;
                //        }
                //    }
                //}

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
                //if (height < MaxHeight)
                //{
                //    dataSemaphore.Release();//没满就释放锁，满了就要读取后释放
                //}

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

                // 2. 复制值类型属性 (纳秒级，忽略不计)
                retClone.headTime = this.headTime;
                retClone.height = this.height;
                retClone.First = this.First;
                if (this.lineIndexList != null)
                {
                    Buffer.BlockCopy(this.lineIndexList, 0, retClone.lineIndexList, 0, this.lineIndexList.Length * 4);
                }
                if (this.data != null)
                {
                    Buffer.BlockCopy(this.data, 0, retClone.data, 0, this.data.Length);
                }

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

        XImgFrame imgBuffer;//?表示可以为null
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
        XImgFrame[] imgFrames;
        XImgFrame[] lowPowerImgFrames;
        byte[] dataBuffer = new byte[0];
        ushort[] dataBufferBackground = new ushort[0];
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
        void setThreshold(ushort lineData, int index, long[] thresholdList, long[] countList)
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

        public static ushort[] ConvertBytesToUInt16Array(byte[] byteArray, bool isLittleEndian = true)
        {
            if (byteArray == null)
                throw new ArgumentNullException(nameof(byteArray));
            if (byteArray.Length % 2 != 0)
                throw new ArgumentException("字节数组长度必须是偶数");
            ushort[] result = new ushort[byteArray.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                int byteIndex = i * 2;
                result[i] = isLittleEndian
                    ? (ushort)(byteArray[byteIndex] | (byteArray[byteIndex + 1] << 8))  // 小端序
                    : (ushort)((byteArray[byteIndex] << 8) | byteArray[byteIndex + 1]); // 大端序
            }
            return result;
        }
        public static byte[] ConvertUInt16ArrayToBytes(ushort[] ushortArray, bool isLittleEndian = false)
        {
            if (ushortArray == null)
                throw new ArgumentNullException(nameof(ushortArray));
            byte[] byteArray = new byte[ushortArray.Length * 2];
            for (int i = 0; i < ushortArray.Length; i++)
            {
                int byteIndex = i * 2;
                ushort value = ushortArray[i];
                if (isLittleEndian)
                {
                    byteArray[byteIndex] = (byte)(value & 0xFF);       // 低字节
                    byteArray[byteIndex + 1] = (byte)(value >> 8);      // 高字节
                }
                else
                {
                    byteArray[byteIndex] = (byte)(value >> 8);          // 高字节
                    byteArray[byteIndex + 1] = (byte)(value & 0xFF);   // 低字节
                }
            }
            return byteArray;
        }
        private ushort[] lsHightPower;
        private ushort[] lsRawHightPower;
        private byte[] cacheByteBuffer;//用于转换字节
        private byte[] cacheRawByteBuffer;//用于转换字节


        void frameParese(ushort[] dataBuffer, DateTime time, ref XImgFrame imgBuffer, ref XImgFrame maskBuffer,
    ref XImgFrame[] ImgFrames, ref int frameBufferIndex, ref int buffeReadyFlag, ref int taskResult,
    ref long[] thresholdList, ref long[] countList, ref int threshHeight, int dataIndex, ref ConcurrentQueue<int> readyQueue)
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

            if (lsHightPower == null || lsHightPower.Length != dataBuffer.Length)
            {
                lsHightPower = new ushort[dataBuffer.Length];
                lsRawHightPower = new ushort[dataBuffer.Length];
                cacheByteBuffer = new byte[dataBuffer.Length * 2];
                cacheRawByteBuffer = new byte[dataBuffer.Length * 2];

            }
            //这里获取的是ushort数组，但xframe里面使用的是字节存储的
            if (threshHeight < ImgHeight / 2)                        //也就是处理数据是按像素处理，存储传输用的是字节处理
                threshHeight++;

            for (int i = 0; i < lsHightPower.Length; i++)
            {

                ushort pixelValue = 0;
                ushort background = ushort.MaxValue;
                //byte rawPixelValue = 0;
                //byte rawBackground = Byte.MaxValue;
                switch (ImgGrayType)
                {
                    case GrayType.Common:
                        pixelValue = dataBuffer[i];
                        background = ushort.MaxValue;
                        break;
                    case GrayType.Inverse:
                        pixelValue = (ushort)(ushort.MaxValue - dataBuffer[i]);
                        background = 0;
                        break;
                    case GrayType.CommonByTexture:
                        pixelValue = 0;
                        background = ushort.MaxValue;
                        break;
                    case GrayType.InverseByTexture:
                        pixelValue = ushort.MaxValue;
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

                            lsHightPower[lsHightPower.Length - 1 - i] = dataBuffer[i] > thresholdList[i] / threshHeight * 0.9 ? background : pixelValue;//原数据U16小端，取高8位
#if _WHITE_BALANCE
                                    lsRaw[ls.Length - 1 - i] = dataBuffer[i]> thresHoldList[i] / threshHeight * 0.9 ? (byte)255:(byte)(255 - (eachLinesThreshold[i] / threshHeight - dataBuffer[i ]));
#else
                            lsRawHightPower[lsHightPower.Length - 1 - i] = dataBuffer[i];
#endif
                            break;
                    }
                    //if (Inverse)
                    //    ls[ls.Length - 1 - i] = dataBuffer[i * 2 + 1] > eachLinesThreshold[i] / threshHeight * 0.9 ? Byte.MaxValue : pixelValue;//取高位字节，文档有误，低字节和高字节的顺序反了？
                    //else
                    //    ls[i] = dataBuffer[i * 2 + 1] > eachLinesThreshold[i] / threshHeight * 0.9 ? Byte.MaxValue : pixelValue;//取高位字节，文档有误，低字节和高字节的顺序反了？
                    //ls[i] = dataBuffer[i * 2 + 1] > Byte.MaxValue ? Byte.MaxValue : dataBuffer[i * 2 + 1];//取高位字节，文档有误，低字节和高字节的顺序反了？
                }


            }
            //Console.WriteLine($"lsHightPower的长度{lsHightPower.Length}");1088
            //Console.WriteLine($"lsRawHightPower的长度{lsRawHightPower.Length}");

            ////dataBuffer.CopyTo(ls, 0);

            Buffer.BlockCopy(lsHightPower, 0, cacheByteBuffer, 0, lsHightPower.Length * 2);
            Buffer.BlockCopy(lsRawHightPower, 0, cacheRawByteBuffer, 0, lsRawHightPower.Length * 2);
            dataMember.Data = cacheByteBuffer;
            rawDataMember.Data = cacheRawByteBuffer;//转为字节序

            //组当前帧,BUFFER_DEPTH是缓冲区个数，BUFFER_DEPTH*2 = 掩码图+原图
            int ret = ImgFrames[frameBufferIndex].AddLine(dataMember.Data, dataMember.DataIndex, time);
            ImgFrames[frameBufferIndex + BUFFER_DEPTH].AddLine(rawDataMember.Data, rawDataMember.DataIndex, time);

            //当前帧的死区帧组上一帧，上一帧已经填满512行，开始填死区
            int deadFrameBufferIndex = (frameBufferIndex - 1 + BUFFER_DEPTH) % BUFFER_DEPTH;
            if (ImgFrames[deadFrameBufferIndex].Height >= ImgFrames[deadFrameBufferIndex].RoiHeight &&
                ImgFrames[deadFrameBufferIndex].Height < ImgFrames[deadFrameBufferIndex].MaxHeight)
            {
                //Console.WriteLine("DataIndex_Height:{0}", ImgFrames[deadFrameBufferIndex].Height);
                int deadRet = ImgFrames[deadFrameBufferIndex].AddLine(dataMember.Data, dataMember.DataIndex, time);
                ImgFrames[deadFrameBufferIndex + BUFFER_DEPTH].AddLine(rawDataMember.Data, rawDataMember.DataIndex, time);
                if (deadRet > 0)
                {

                    if (!ImgFrames[deadFrameBufferIndex + BUFFER_DEPTH].First)//第一帧无法填充死区，故省略第一张
                    {
                        //imgBuffer = (XImgFrame)ImgFrames[deadFrameBufferIndex + BUFFER_DEPTH];//c#对象拷贝只有浅拷贝，深拷贝得自己实现
                        //maskBuffer = (XImgFrame)ImgFrames[deadFrameBufferIndex];
                        readyQueue.Enqueue(deadFrameBufferIndex);
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
                    buffeReadyFlag = deadFrameBufferIndex;

                    //Console.WriteLine("==================== Get one Img ====================");
                }
            }///

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
                frameBufferIndex = (frameBufferIndex + 1) % BUFFER_DEPTH;
                if (ImgFrames[frameBufferIndex].RefCount > 0)
                {
                    // 🚨 严重警报：发生追尾！
                    Console.WriteLine($"[严重错误] 缓冲区溢出！消费者太慢了，还在读 Index {frameBufferIndex}，生产者就要覆盖它了！");
                    ErrLog.Recorder.AddErrMsg("Buffer Overflow detected!");
                }
                //Console.WriteLine(frameBufferIndex);
                if (ret == 1)
                {

                    buffeReadyFlag = frameBufferIndex;
                    if (!ImgFrames[frameBufferIndex + BUFFER_DEPTH].First)
                    {
                        //imgBuffer = (XImgFrame)ImgFrames[buffeReadyFlag + BUFFER_DEPTH];
                        //maskBuffer = (XImgFrame)ImgFrames[buffeReadyFlag]);
                        readyQueue.Enqueue(buffeReadyFlag);

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
                    //这个线阵不是自己抓包获取的帧包，无法很好判断丢帧，故这个ret=2没用到
                    ImgFrames[frameBufferIndex].AddLine(dataMember.Data, dataMember.DataIndex, time);
                    ImgFrames[frameBufferIndex + BUFFER_DEPTH].AddLine(rawDataMember.Data, rawDataMember.DataIndex, time);

                    buffeReadyFlag = frameBufferIndex;
                    if (!ImgFrames[deadFrameBufferIndex + BUFFER_DEPTH].First)
                    {
                        //imgBuffer = (XImgFrame)ImgFrames[buffeReadyFlag + BUFFER_DEPTH];
                        //maskBuffer = (XImgFrame)ImgFrames[buffeReadyFlag];
                        readyQueue.Enqueue(buffeReadyFlag);
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
                dataBuffer = new ushort[0];
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

        #endregion

        #region Controller
        public void InitSample(int integrationTime, uint target = 65535, bool correct_flag = true, bool background = true)
        {
            SetGain(gain);
            SetIntegrationTime(integrationTime);
            SdkInterface.tymscan_set_cardnumber(1, (int)Channel1s, instanceId_PANGU);
            SdkInterface.tymscan_set_cardnumber(2, (int)Channel2s, instanceId_PANGU);
            SdkInterface.tymscan_set_cardnumber(3, (int)Channel3s, instanceId_PANGU);
            SdkInterface.tymscan_set_cardnumber(4, (int)Channel4s, instanceId_PANGU);

            SdkInterface.tymscan_set_pixelnum_percard(256, instanceId_PANGU);//单能128

            if (correct_flag)
            {
                //Console.WriteLine("注意需要分别执行gain和本底值校准，获取不同数据，请手动设置代码");
                //校正卡数不匹配会有The current data pixel number is different from the offset calibration data pixel number!的问题
                if (background)
                {
                    SdkInterface.tymcan_doOffsetCalibration(instanceId_PANGU);//本底校准（两个校准要分别执行一次）
                }
                //Thread.Sleep(3000);
                else
                {
                    SdkInterface.tymcan_doGainCalibration(target, instanceId_PANGU);//空场校准
                }
                //SdkInterface.tymcan_setBaseLine(0, instanceId_PANGU);//对于噪声大的用来纠正偏移
                SdkInterface.tymcan_set_sendCalibratedData_Enable(true, true, true, instanceId_PANGU);//在每次获取完物料值的时候，把数组输入进去，可以筛选出不进行校准的值
            }
        }
        //public void InitSample(int integrationTime)
        //{
        //}
        public void SetIntegrationTime(int time)
        {
            integral_time = time;
            SdkInterface.tymscan_set_integral_time(integral_time, instanceId_PANGU); // 设置积分时间
        }
        public void StopSampling()
        {
            SdkInterface.tymscan_grab_stop(instanceId_PANGU);
        }

        public void StartSampling()
        {
            SdkInterface.tymscan_grab_start(instanceId_PANGU);
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

        //组帧缓冲区设置（双缓冲得用到深拷贝，效率太慢，那边发太快）
        const int BUFFER_DEPTH = 6;
        public void Start()
        {
            uint len = CardSum * PixelNum;//卡数*像素数，存储单能
            dataBufferBackground = new ushort[len];
            Console.WriteLine($"dataBufferBackground的像素长度{len}");
            for (int i = 0; i < len; i++)
            {
                dataBufferBackground[i] = ushort.MaxValue;
            }
            //imgFrames = new XImgFrame[4] { new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth),//这里乘2是因为ushort转byte存储
            //                               new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth),
            //                               new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth),
            //                               new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth)  };
            //lowPowerImgFrames = new XImgFrame[4] { new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth),
            //                                       new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth),
            //                                       new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth),
            //                                       new XImgFrame((uint)ImgHeight,(uint)deadHeight,CardSum * PixelNum * Deepth / 8, Deepth)  };
            //foreach (var frame in imgFrames)
            //{
            //    frame.First = true;
            //}

            int totalSize = BUFFER_DEPTH * 2;
            imgFrames = new XImgFrame[totalSize];
            lowPowerImgFrames = new XImgFrame[totalSize];
            for (int i = 0; i < totalSize; i++)
            {
                // 你的初始化参数
                imgFrames[i] = new XImgFrame((uint)ImgHeight, (uint)deadHeight, CardSum * PixelNum * Deepth / 8, Deepth);
                imgFrames[i].First = true;

                lowPowerImgFrames[i] = new XImgFrame((uint)ImgHeight, (uint)deadHeight, CardSum * PixelNum * Deepth / 8, Deepth);
                lowPowerImgFrames[i].First = true;
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


        #endregion

    }

}
