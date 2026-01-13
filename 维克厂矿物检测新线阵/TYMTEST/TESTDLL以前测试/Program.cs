//using System;
//using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using TYMDetector;
using static TYMDetector.SdkInterface;
using TYMDLL;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using SixLabors.ImageSharp;//这个16b得用png
using SixLabors.ImageSharp.PixelFormats; // 必须引用，否则找不到 L16
using SixLabors.ImageSharp.Processing;
using System.ComponentModel.DataAnnotations;
using System;
using OpenCvSharp;

class Program
{

    static public string ArrayServer = "192.168.10.100";       // 线阵本地IP
    static public string ArrayRemoteIp = "192.168.10.1";      // 线阵远端IP
    static public int ArrayRemoteCmdPort = 7171;      // 线阵远端指令接收端口
    static public int ArrayRemoteDataPort = 7474;     // 线阵远端信息发送端口
    static int imgHeight = 512;
    static int deathHeight = 200;
    static int integral_time = 230;//积分时间
    static int gain = 16; //增益
    static void Main(string[] args)
    {

        TYMPANGU LineScaner = TYMPANGU.TYMCardFactory(imgHeight, deathHeight, integral_time);
        LineScaner.ImgGrayType = TYMPANGU.GrayType.InverseByTexture;
        LineScaner.ImgPosiontType = TYMPANGU.PositonType.Inverse;
        LineScaner.RegisterImgCallback(getData);

        LineScaner.Connect(ArrayServer, ArrayRemoteIp, -1, -1, ArrayRemoteCmdPort, ArrayRemoteDataPort);
        LineScaner.InitSample(integral_time, 50000, true);
        LineScaner.SetGain(gain);



        LineScaner.StartSampling();
        while (true)
        {

        }

    }


    static int testIdx = 0;
    static void getData(object sender, EventArgs e)
    {
        //int[] ret = new int[4];

        TYMPANGU.XImgGroup data = e as TYMPANGU.XImgGroup;


        string dirPath = @"./data1/";
        Directory.CreateDirectory(dirPath);

        bool hasObjects1 = false;
        bool hasObjects2 = false;

        if (data.Content[2] != null)
        {
            // 检测物块（使用第3个图像作为掩码）
            hasObjects1 = DetectObjectsInImage(
                data.Content[2].Data,
                (int)data.Content[2].Width / 2,
                (int)data.Content[2].MaxHeight
            );
            // 检测物块（使用第3个图像作为掩码）
            hasObjects2 = DetectObjectsInImage(
                data.Content[0].Data,
                (int)data.Content[0].Width / 2,
                (int)data.Content[0].MaxHeight
            );
        }

        // 如果有物块，保存所有4个图像
        if (hasObjects1 && hasObjects2)
        {
            for (int i = 0; i < 4; i++)
            {
                if (data.Content[i] != null)
                {
                    string filePath = Path.Combine(dirPath, $"img{i}_test{testIdx}.png");
                    Save16bitGrayscalePng(
                        data.Content[i].Data,
                        (int)data.Content[i].Width / 2,
                        (int)data.Content[i].MaxHeight,
                        filePath
                    );
                }
            }
            testIdx++;
        }

        //Console.WriteLine($"\n=== 图像统计 (test{testIdx}) ===");
        //for (int i = 0; i < 4; i++)
        //{
        //    if (data.Content[i] != null && i != 0 && i != 2)
        //    {
        //        Console.WriteLine($"\nContent[{i}] 像素分布:");
        //        AnalyzePixelDistribution(data.Content[i].Data);
        //    }
        //}
    }
    static bool DetectObjectsInImage(byte[] raw16bit, int width, int height)
    {
        // 1. 使用 FromPixelData 创建 Mat（16-bit 灰度）
        // 使用 unsafe 代码块和指针
        unsafe
        {
            fixed (byte* ptr = raw16bit)
            {
                Mat img16 = Mat.FromPixelData(height, width, MatType.CV_16UC1, (IntPtr)ptr, width * 2); // 16-bit 每个像素2字节

                // 2. 转换为 8-bit 进行检测
                Mat img8 = new Mat();
                img16.ConvertTo(img8, MatType.CV_8UC1, 1.0 / 256);

                // 3. 二值化
                Mat binary = new Mat();
                Cv2.Threshold(img8, binary, 30, 255, ThresholdTypes.Binary);

                // 4. 查找轮廓
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchy;
                Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                // 5. 释放资源
                img16.Dispose();
                img8.Dispose();
                binary.Dispose();

                return contours.Length > 0;
            }
        }
    }
    static byte[] ConvertBigEndianToLittleEndian(byte[] raw16bit)
    {
        for (int i = 0; i < raw16bit.Length; i += 2)
        {
            (raw16bit[i], raw16bit[i + 1]) = (raw16bit[i + 1], raw16bit[i]); // 交换字节
        }
        return raw16bit;
    }


    /// <summary>
    /// 将16位灰度数据保存为PNG
    /// </summary>
    static void Save16bitGrayscalePng(byte[] raw16bit, int width, int height, string path)
    {
        // 方法1：使用ImageSharp（推荐，需安装NuGet包 SixLabors.ImageSharp）
        using (var image = SixLabors.ImageSharp.Image.LoadPixelData<L16>(raw16bit, width, height))
        {
            image.Save(path);
        }

    }

    // 分析16位像素值分布
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


    /// <summary>
    /// 将数组转换成彩色图片
    /// </summary>
    /// <param name = "rawValues" > 图像的byte数组 </ param >
    /// < param name="width">图像的宽</param>
    /// <param name = "height" > 图像的高 </ param >
    /// < returns > Bitmap对象 </ returns >
    //static public Bitmap ToColorBitmap(byte[] rawValues, int width, int height)
    //{
    //    Bitmap m_currBitmap;
    //    Rectangle m_rect;
    //    BitmapData m_bitmapData;
    //    //// 申请目标位图的变量，并将其内存区域锁定
    //    m_currBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);//这里16位映射为8位了的
    //    ColorPalette palette = m_currBitmap.Palette;
    //    for (int i = 0; i < 256; i++)
    //    {
    //        palette.Entries[i] = Color.FromArgb(i, i, i);
    //    }
    //    m_currBitmap.Palette = palette;
    //    m_rect = new Rectangle(0, 0, width, height);
    //    m_bitmapData = m_currBitmap.LockBits(m_rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

    //    IntPtr iptr = m_bitmapData.Scan0;  // 获取bmpData的内存起始位置  

    //    //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中  
    //    System.Runtime.InteropServices.Marshal.Copy(rawValues, 0, iptr, width * height);


    //    return m_currBitmap;

    //}




}

