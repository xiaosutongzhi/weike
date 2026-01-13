//首先测试用，看返回是否成功，然后测试用那个dataframe用双缓冲装下去，直接调用回到原本上层软件，不要改什么东西了

//using System;
//using System.Runtime.InteropServices;
//using System.Threading;
//using TYMDetector;
////测一下接收32次数据多少时间，用快排思想找中间值，然后计算处理一行要多少时间，处理的时间刚好能做并行？，不用这样，依然双缓冲就行
////测双缓冲的情况下，多少行回调比较合适
//namespace TESTDLL
//{

//    class Program
//    {
//        private static int instanceId_PANGU = -1; // 数据采集板实例ID

//        static void Main(string[] args)
//        {
//            // 初始化数据采集板
//            SdkInterface.tymscan_init("192.168.10.100", "192.168.10.1", 7171, 7474, ref instanceId_PANGU);

//            bool connected_PANGU = false;
//            SdkInterface.tymscan_connected(ref connected_PANGU, instanceId_PANGU);

//            if (connected_PANGU)
//            {
//                // 注意：如果产品是单能级产品，只需设置tymscan_set_gain_low，无需设置tymscan_set_gain_high
//                SdkInterface.tymscan_set_gain_low(0, 6, instanceId_PANGU); // 设置低能增益
//                SdkInterface.tymscan_set_gain_high(0, 6, instanceId_PANGU); // 设置高能增益
//                SdkInterface.tymscan_set_integral_time(3000, instanceId_PANGU); // 设置积分时间

//                // 设置数据回调函数(每接收32行数据触发一次回调)
//                SdkInterface.tymfn_datacallback callback = new SdkInterface.tymfn_datacallback(Callback);
//                SdkInterface.tymscan_set_datacallback(callback, IntPtr.Zero, 32, instanceId_PANGU);

//                SdkInterface.tymscan_grab_start(instanceId_PANGU); // 启动采集
//                Thread.Sleep(TimeSpan.FromMinutes(1)); // 等待1分钟
//                SdkInterface.tymscan_grab_stop(instanceId_PANGU); // 停止采集
//                SdkInterface.tymscan_cleanup(instanceId_PANGU); // 清理实例
//            }

//            Console.WriteLine("按任意键退出...");
//            Console.ReadKey();
//        }

//        // 回调函数
//        public static void Callback(IntPtr bufferPtr, IntPtr extra_info, IntPtr user_data, int instance)
//        {
//            if (instance == instanceId_PANGU)
//            {
//                // 将缓冲区指针转换为可用的结构体
//                // 注意：需要根据实际结构体定义调整
//                TymdataBuffer buffer = Marshal.PtrToStructure<TymdataBuffer>(bufferPtr);
//                Handle(buffer);
//            }
//        }

//        // 定义数据缓冲区结构体
//        [StructLayout(LayoutKind.Sequential)]
//        public struct TymdataBuffer
//        {
//            public IntPtr data;               // 图像数据指针
//            public int line_num;              // 行数
//            public int pixels_per_line;       // 每行像素数
//            public int bytes_per_line;        // 每行字节数
//                                              // 可根据原始结构体添加其他字段
//        }


//        // 处理图像数据的函数
//        public static void Handle(TymdataBuffer buf)
//        {
//            int height = buf.line_num;
//            int width = buf.pixels_per_line;

//            for (int i = 0; i < height; i++)
//            {
//                // 计算行首地址
//                IntPtr lineAddress = IntPtr.Add(buf.data, i * buf.bytes_per_line);

//                for (int j = 0; j < width; j++)
//                {
//                    // 读取像素值(16位无符号)
//                    ushort val = (ushort)Marshal.ReadInt16(lineAddress, j * 2);
//                    Console.Write("{0}\t", val);
//                }
//                Console.WriteLine();
//            }

//            Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
//        }
//    }
//}