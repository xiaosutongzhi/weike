//using System;
//using System.Runtime.InteropServices;
//using System.Threading;
//using TYMDetector;
//using SixLabors.ImageSharp.PixelFormats;
//using SixLabors.ImageSharp;

//////直接测试用的
//public class Program
//{


//    // 定义数据缓冲区结构体
//    [StructLayout(LayoutKind.Sequential)]
//    public struct TymdataBuffer//结构体成员排列要符合文档
//    {
//        public int line_num;              // 行数
//        public int pixels_per_line;       // 每行像素数
//        public int bytes_per_pixel;       //像素深度
//        public int bytes_per_line;        // 每行字节数
//        public IntPtr data;               // 图像数据指针

//    }

//    private static int instanceId_PANGU = -1;
//    private static int frameCount = 0;    // 帧计数器

//    public static void Main(string[] args)
//    {
//        // Initialize the scanner
//        instanceId_PANGU = SdkInterface.tymscan_init("192.168.10.100", "192.168.10.1", 7171, 7474, ref instanceId_PANGU);

//        bool connected_PANGU = false;
//        SdkInterface.tymscan_connected(ref connected_PANGU, instanceId_PANGU);

//        if (connected_PANGU)
//        {
//            // Configure the scanner
//            //SdkInterface.tymscan_set_cardnumber(1, 5, instanceId_PANGU);
//            //SdkInterface.tymscan_set_cardnumber(2, 5, instanceId_PANGU);
//            //SdkInterface.tymscan_set_pixelnum_percard(256, instanceId_PANGU);
//            SdkInterface.tymscan_set_gain_low(0, 50, instanceId_PANGU);
//            SdkInterface.tymscan_set_gain_high(0, 50, instanceId_PANGU);
//            SdkInterface.tymscan_set_integral_time(266, instanceId_PANGU);

//            // Set up the callback
//            SdkInterface.tymfn_datacallback callback;
//            callback = new SdkInterface.tymfn_datacallback(Callback);
//            SdkInterface.tymscan_set_datacallback(callback, IntPtr.Zero, 512, instanceId_PANGU);


//            SdkInterface.tymscan_grab_start(instanceId_PANGU);
//            Thread.Sleep(TimeSpan.FromMinutes(1));
//            SdkInterface.tymscan_grab_stop(instanceId_PANGU);
//            SdkInterface.tymscan_cleanup(instanceId_PANGU);
//        }

//        Console.WriteLine("Press any key to exit...");
//        Console.ReadKey();
//    }
//    private static int currentLine = 0;
//    private static ushort[,] lowEnergyImageBuffer = new ushort[512, 1280];  // First half of each line (low energy)
//    private static ushort[,] highEnergyImageBuffer = new ushort[512, 1280]; // Second half of each line (high energy)
//    private static int testIdx = 0; // Added test index counter
//    private static readonly object fileLock = new object();
//    private const int ExpectedLines = 512;

//    private static void Handle(TymdataBuffer buf)
//    {
//        int width = buf.pixels_per_line;
//        int halfWidth = width / 2;
//        int totalPixels = ExpectedLines * halfWidth;

//        // 创建缓冲区
//        L16[] lowEnergyPixels = new L16[totalPixels];
//        L16[] highEnergyPixels = new L16[totalPixels];

//        // 计算总字节数
//        int totalBytes = ExpectedLines * width * buf.bytes_per_pixel;
//        byte[] imageData = new byte[totalBytes];

//        // 复制数据
//        Marshal.Copy(buf.data, imageData, 0, imageData.Length);

//        // 处理数据
//        for (int line = 0; line < ExpectedLines; line++)
//        {
//            int lineOffset = line * width * buf.bytes_per_pixel;

//            for (int pixel = 0; pixel < halfWidth; pixel++)
//            {
//                int lowEnergyIndex = line * halfWidth + pixel;
//                int highEnergyIndex = line * halfWidth + pixel;

//                // 低能数据（前半部分）
//                lowEnergyPixels[lowEnergyIndex] = new L16(BitConverter.ToUInt16(imageData, lineOffset + pixel * buf.bytes_per_pixel));

//                // 高能数据（后半部分）
//                highEnergyPixels[highEnergyIndex] = new L16(BitConverter.ToUInt16(imageData, lineOffset + (pixel + halfWidth) * buf.bytes_per_pixel));
//            }
//        }

//        // 保存图像（使用锁防止多线程冲突）
//        lock (fileLock)
//        {
//            SaveImage(lowEnergyPixels, halfWidth, ExpectedLines, "low_energy");
//            SaveImage(highEnergyPixels, halfWidth, ExpectedLines, "high_energy");
//            frameCount++;
//            Console.WriteLine($"已成功处理第 {frameCount} 帧数据");
//        }
//    }

//    private static void SaveImage(L16[] pixelData, int width, int height, string energyType)
//    {
//        string dirPath = @"./data/";
//        Directory.CreateDirectory(dirPath);
//        string filePath = Path.Combine(dirPath, $"{energyType}_frame{frameCount}.png");

//        try
//        {
//            using (var image = Image.LoadPixelData<L16>(pixelData, width, height))
//            {
//                image.Save(filePath, new SixLabors.ImageSharp.Formats.Png.PngEncoder
//                {
//                    BitDepth = SixLabors.ImageSharp.Formats.Png.PngBitDepth.Bit16,
//                    CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.BestCompression
//                });
//            }
//            Console.WriteLine($"已保存 {energyType} 图像: {filePath}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"保存 {energyType} 图像时出错: {ex.Message}");
//        }
//    }
//    // The callback function
//    private static void Callback(IntPtr bufferPtr, IntPtr extraInfo, IntPtr userData, int instance)
//    {
//        if (instance == instanceId_PANGU)
//        {
//            TymdataBuffer buf = (TymdataBuffer)Marshal.PtrToStructure(bufferPtr, typeof(TymdataBuffer));
//            Handle(buf);
//        }
//    }
//}



























////原始采集实例
////public class Program
////{
////    //定义数据缓冲区结构体
////    [StructLayout(LayoutKind.Sequential)]
////    public struct TymdataBuffer//结构体成员排列要符合文档
////    {
////        public int line_num;              // 行数
////        public int pixels_per_line;       // 每行像素数
////        public int bytes_per_pixel;       //像素深度
////        public int bytes_per_line;        // 每行字节数
////        public IntPtr data;               // 图像数据指针
////    }

////    private static int instanceId_PANGU = -1;
////    private static int frameCount = 0;    // 帧计数器

////    public static void Main(string[] args)
////    {
////        // Initialize the scanner
////        instanceId_PANGU = SdkInterface.tymscan_init("192.168.10.100", "192.168.10.1", 7171, 7474, ref instanceId_PANGU);

////        bool connected_PANGU = false;
////        SdkInterface.tymscan_connected(ref connected_PANGU, instanceId_PANGU);

////        if (connected_PANGU)
////        {
////            // Configure the scanner
////            SdkInterface.tymscan_set_cardnumber(1, 5, instanceId_PANGU);
////            SdkInterface.tymscan_set_cardnumber(2, 5, instanceId_PANGU);
////            SdkInterface.tymscan_set_pixelnum_percard(256, instanceId_PANGU);
////            SdkInterface.tymscan_set_gain_low(0, 50, instanceId_PANGU);
////            SdkInterface.tymscan_set_gain_high(0, 50, instanceId_PANGU);
////            SdkInterface.tymscan_set_integral_time(266, instanceId_PANGU);

////            // Set up the callback
////            SdkInterface.tymfn_datacallback callback;
////            callback = new SdkInterface.tymfn_datacallback(Callback);
////            SdkInterface.tymscan_set_datacallback(callback, IntPtr.Zero, 512, instanceId_PANGU);


////            SdkInterface.tymscan_grab_start(instanceId_PANGU);
////            Thread.Sleep(TimeSpan.FromMinutes(1));
////            SdkInterface.tymscan_grab_stop(instanceId_PANGU);
////            SdkInterface.tymscan_cleanup(instanceId_PANGU);
////        }

////        Console.WriteLine("Press any key to exit...");
////        Console.ReadKey();
////    }
////    private static void Callback(IntPtr bufferPtr, IntPtr extra_info, IntPtr user_data, int instance)
////    {
////        if (instance == instanceId_PANGU)
////        {
////            // Handle data
////            TymdataBuffer buf = (TymdataBuffer)Marshal.PtrToStructure(bufferPtr, typeof(TymdataBuffer));
////            HandleData(buf);
////        }
////    }

////    // Data handler method
////    private static void HandleData(TymdataBuffer buf)
////    {

////        int height = buf.line_num;
////        int width = buf.pixels_per_line;

////        for (int i = 0; i < height; i++)
////        {
////            // Calculate the address of the current line
////            IntPtr lineAddress = IntPtr.Add(buf.data, i * buf.bytes_per_line);

////            for (int j = 0; j < width; j++)
////            {
////                // Read the 16-bit value at the current position
////                ushort val = (ushort)Marshal.ReadInt16(IntPtr.Add(lineAddress, j * 2));
////                Console.Write("{0}\t", val);
////            }
////            Console.WriteLine();
////        }
////        Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

////    }
////}