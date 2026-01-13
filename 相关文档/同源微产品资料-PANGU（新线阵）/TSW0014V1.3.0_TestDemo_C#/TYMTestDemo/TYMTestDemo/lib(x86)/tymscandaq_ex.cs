using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TYMDetector
{
    public partial class SdkInterface
    {

        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_set_infopage", CallingConvention = CallingConvention.StdCall)]
        public static extern int tymscan_set_infopage(int channel, int page, string filePath, int instance);


        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_get_infopage", CallingConvention = CallingConvention.StdCall)]
        public static extern int tymscan_get_infopage(int channel, int page, string filePath, int instance);


        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_set_TH_enable_XH", CallingConvention = CallingConvention.StdCall)]
        public static extern int tymscan_set_TH_enable_XH(bool enable, int instance);

        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_set_custom_pixnum", CallingConvention = CallingConvention.StdCall)]
        public static extern int tymscan_set_custom_pixnum(int pixelnum, int instance);


        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_set_detector_type", CallingConvention = CallingConvention.StdCall)]
        public static extern int  tymscan_set_detector_type(int detector_type, int instance);

        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_get_detector_type", CallingConvention = CallingConvention.StdCall)]
        public static extern int  tymscan_get_detector_type(ref int detector_type, int instance);

        enum DetectorType {//固定配置，请勿修改
        	PCM08_1DOUT = 1,
        	PCM04_1DOUT = 2,
        	PCM02_1DOUT = 3,
        	PCM01_1DOUT = 4,
        
        	PCM08DH_2DOUT = 8,
        	PCM04_2DOUT = 9,
        	PCM04DH_2DOUT = 10,
        	PCM02_2DOUT = 11,
        	PCM02DH_2DOUT = 12,
        	PCM01_2DOUT = 13,
        };


        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_set_auth_bit", CallingConvention = CallingConvention.StdCall)]
        public static extern int  tymscan_set_auth_bit(int auth_id, int value, int instance);


        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_enable_auto_get_cardnum", CallingConvention = CallingConvention.StdCall)]
        public static extern int  tymscan_enable_auto_get_cardnum(int enable, int instance);


        [DllImport("TYMScannerLib.dll", EntryPoint = "tymscan_is_auto_get_cardnum", CallingConvention = CallingConvention.StdCall)]
        public static extern int  tymscan_is_auto_get_cardnum(ref int is_auto,int instance);




    }
}
