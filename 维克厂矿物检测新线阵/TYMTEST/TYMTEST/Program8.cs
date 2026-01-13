////using System;
////using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Threading;
//using TYMDetector;
//using static TYMDetector.SdkInterface;
//using TYMDLL;
//using System.Drawing;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Drawing.Imaging;
//using System.Threading;
//using System.IO;
////using SixLabors.ImageSharp;//这个16b得用png
////using SixLabors.ImageSharp.PixelFormats; // 必须引用，否则找不到 L16
////using SixLabors.ImageSharp.Processing;
//using TYMDLL8B;

//class Program
//{

//    static public string ArrayServer = "192.168.10.100";       // 线阵本地IP
//    static public string ArrayRemoteIp = "192.168.10.1";      // 线阵远端IP
//    static public int ArrayRemoteCmdPort = 7171;      // 线阵远端指令接收端口
//    static public int ArrayRemoteDataPort = 7474;     // 线阵远端信息发送端口
//    static int imgHeight = 512;
//    static int deathHeight = 200;
//    static int integral_time = 266;
//    static int gain = 12;
//    static void Main(string[] args)
//    {

//        TYMPANGU8B LineScaner = TYMPANGU8B.TYMCardFactory(imgHeight, deathHeight, integral_time);
//        LineScaner.ImgGrayType = TYMPANGU8B.GrayType.InverseByTexture;
//        LineScaner.ImgPosiontType = TYMPANGU8B.PositonType.Inverse;
//        LineScaner.RegisterImgCallback(getData);

//        LineScaner.Connect(ArrayServer, ArrayRemoteIp, -1, -1, ArrayRemoteCmdPort, ArrayRemoteDataPort);
//        LineScaner.InitSample(integral_time, 50000, true);
//        LineScaner.SetGain(gain);



//        LineScaner.StartSampling();
//        Thread.Sleep(100000);
//        LineScaner.StopSampling();



//        Console.ReadKey();

//    }


//    static int testIdx = 0;
//    static void getData(object sender, EventArgs e)
//    {
//        //int[] ret = new int[4];

//        TYMPANGU8B.XImgGroup data = e as TYMPANGU8B.XImgGroup;


//        string dirPath = @"./data/";
//        Directory.CreateDirectory(dirPath);

//        var img1 = ToColorBitmap(data.Content[1].Data, (int)data.Content[1].Width, (int)(data.Content[1].MaxHeight));
//        var img0 = ToColorBitmap(data.Content[0].Data, (int)data.Content[0].Width, (int)(data.Content[0].MaxHeight));//掩码图
//        var img2 = ToColorBitmap(data.Content[2].Data, (int)data.Content[1].Width, (int)(data.Content[2].MaxHeight));
//        var img3 = ToColorBitmap(data.Content[3].Data, (int)data.Content[0].Width, (int)(data.Content[3].MaxHeight));//掩码图
//        img1.Save(string.Format(@"./data/img1test{0}.bmp", testIdx));
//        img2.Save(string.Format(@"./data/img2test{0}.bmp", testIdx));
//        img3.Save(string.Format(@"./data/img3test{0}.bmp", testIdx));
//        img0.Save(string.Format(@"./data/img0test{0}.bmp", testIdx));



//        testIdx++;

//    }




//    /// <summary>
//    /// 将数组转换成彩色图片
//    /// </summary>
//    /// <param name="rawValues">图像的byte数组</param>
//    /// <param name="width">图像的宽</param>
//    /// <param name="height">图像的高</param>
//    /// <returns>Bitmap对象</returns>
//    static public Bitmap ToColorBitmap(byte[] rawValues, int width, int height)
//    {
//        Bitmap m_currBitmap;
//        Rectangle m_rect;
//        BitmapData m_bitmapData;
//        //// 申请目标位图的变量，并将其内存区域锁定
//        m_currBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
//        ColorPalette palette = m_currBitmap.Palette;
//        for (int i = 0; i < 256; i++)
//        {
//            palette.Entries[i] = Color.FromArgb(i, i, i);
//        }
//        m_currBitmap.Palette = palette;
//        m_rect = new Rectangle(0, 0, width, height);
//        m_bitmapData = m_currBitmap.LockBits(m_rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

//        IntPtr iptr = m_bitmapData.Scan0;  // 获取bmpData的内存起始位置  

//        //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中  
//        System.Runtime.InteropServices.Marshal.Copy(rawValues, 0, iptr, width * height);


//        return m_currBitmap;

//    }

//}

//////测一下接收32次数据多少时间，用快排思想找中间值，然后计算处理一行要多少时间，处理的时间刚好能做并行？，不用这样，依然双缓冲就行
//////测双缓冲的情况下，多少行回调比较合适
////namespace TESTDLL
////{

////    class Program
////    {
////        static public string ArrayServer = "192.168.10.100";       // 线阵本地IP
////        static public string ArrayRemoteIp = "192.168.10.1";      // 线阵远端IP
////        static public int ArrayRemoteCmdPort = 7171;      // 线阵远端指令接收端口
////        static public int ArrayRemoteDataPort = 7474;     // 线阵远端信息发送端口
////        static int imgHeight = 512;
////        static int deathHeight = 200;
////        static int integral_time = 266;
////        static int gain = 1;
////        static void Main(string[] args)
////        {
////            TYMPANGU LineScaner = TYMPANGU.TYMCardFactory(imgHeight, deathHeight, integral_time);
////            LineScaner.ImgGrayType = TYMPANGU.GrayType.InverseByTexture;
////            LineScaner.ImgPosiontType = TYMPANGU.PositonType.Inverse;

////            //TYMPANGU LineScaner = TYMPANGU.TYMCardFactory(imgHeight, deathHeight);
////            //LineScaner.ImgGrayType = TYMPANGU.GrayType.InverseByTexture;
////            //LineScaner.ImgPosiontType = TYMPANGU.PositonType.Inverse;

////            LineScaner.RegisterImgCallback(getData);

////            LineScaner.Connect(ArrayServer, ArrayRemoteIp, -1, -1, ArrayRemoteCmdPort, ArrayRemoteDataPort);
////            LineScaner.InitSample(integral_time, 50000, false);
////            LineScaner.SetGain(gain);

////            LineScaner.StartSampling();
////            Thread.Sleep(100000);
////            LineScaner.StopSampling();


////            Console.ReadKey();


////        }


////        static int testIdx = 0;
////        static void getData(object sender, EventArgs e)
////        {
////            TYMPANGU.XImgGroup? data = e as TYMPANGU.XImgGroup;

////            // 计算实际图像高度（总字节数/每行字节数）
////            int height = data.Content[1].Data.Length / (int)(data.Content[1].Width); // 16bit=2字节每像素
////            Console.WriteLine($"一帧图片高度: {height}");

////            string dirPath = @"./data/";
////            Directory.CreateDirectory(dirPath);

////            // 处理每个Content通道
////            for (int i = 0; i < 4; i++)
////            {
////                if (data.Content[i] != null && i % 2 != 0)
////                {
////                    string filePath = Path.Combine(dirPath, $"img{i}test{testIdx}.png");
////                    Save16bitGrayscalePng(data.Content[i].Data,
////                                        (int)data.Content[i].Width ,//宽度减少一半，字节数变像素数
////                                        (int)data.Content[i].MaxHeight,
////                                        filePath);
////                }
////            }

////            testIdx++;
////        }

////        /// <summary>
////        /// 将16位灰度数据保存为PNG
////        /// </summary>
////        static void Save16bitGrayscalePng(byte[] raw16bit, int width, int height, string path)
////        {
////            // 方法1：使用ImageSharp（推荐，需安装NuGet包 SixLabors.ImageSharp）
////            using (var image = SixLabors.ImageSharp.Image.LoadPixelData<L16>(raw16bit, width, height))
////            {
////                image.Save(path);
////            }

////            //    // 方法2：使用System.Drawing（需要.NET 6+）
////            //    unsafe
////            //    {
////            //        fixed (byte* ptr = raw16bit)
////            //        {
////            //            using (var bmp = new Bitmap(width, height, width * 2,
////            //                                      PixelFormat.Format16bppGrayScale,
////            //                                      new IntPtr(ptr)))
////            //            {
////            //                bmp.Save(path, ImageFormat.Png);
////            //            }
////            //        }
////            //    }
////            //}

////            /// <summary>
////            /// 备用方法：当原始数据不是ushort数组时的转换
////            /// </summary>
////            static ushort[] ConvertToUshortArray(byte[] raw16bit)
////            {
////                ushort[] pixels = new ushort[raw16bit.Length / 2];
////                Buffer.BlockCopy(raw16bit, 0, pixels, 0, raw16bit.Length);
////                return pixels;
////            }

////            ///// <summary>
////            ///// 将数组转换成彩色图片
////            ///// </summary>
////            ///// <param name="rawValues">图像的byte数组</param>
////            ///// <param name="width">图像的宽</param>
////            ///// <param name="height">图像的高</param>
////            ///// <returns>Bitmap对象</returns>
////            //static public Bitmap ToColorBitmap(byte[] rawValues, int width, int height)
////            //{
////            //    Bitmap m_currBitmap;
////            //    Rectangle m_rect;
////            //    BitmapData m_bitmapData;
////            //    //// 申请目标位图的变量，并将其内存区域锁定
////            //    m_currBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
////            //    ColorPalette palette = m_currBitmap.Palette;
////            //    for (int i = 0; i < 256; i++)
////            //    {
////            //        palette.Entries[i] = Color.FromArgb(i, i, i);
////            //    }
////            //    m_currBitmap.Palette = palette;
////            //    m_rect = new Rectangle(0, 0, width, height);
////            //    m_bitmapData = m_currBitmap.LockBits(m_rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

////            //    IntPtr iptr = m_bitmapData.Scan0;  // 获取bmpData的内存起始位置  

////            //    //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中  
////            //    System.Runtime.InteropServices.Marshal.Copy(rawValues, 0, iptr, width * height);

////            //    //if (width != oldPicWidth || height != oldPicHeight)
////            //    //{
////            //    //    m_currBitmap.UnlockBits(m_bitmapData);
////            //    //    oldPicWidth = width;
////            //    //    oldPicHeight = height;
////            //    //}

////            //    //// 算法到此结束，返回结果  

////            //    return m_currBitmap;

////            //}

////        }
////    }
////}