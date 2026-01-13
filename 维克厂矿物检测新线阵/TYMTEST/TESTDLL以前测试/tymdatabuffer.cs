using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TYMDetector
{
    public partial class SdkInterface
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct tymdata_buffer
        {
            public int line_num;
            public int pixels_per_line;
            public int bytes_per_pixel;
            public int bytes_per_line;
            public IntPtr data;//unsigned char*
        };



        public enum HARDWARE_INFO_ID
        {
            fcminfo_id_smt_pcba_sns,//此id用于获取所有探测器卡(FCM)的SMT生产的PCBA序列号
            fcminfo_id_smt_tpns,//此id用于获取所有探测器卡(FCM)的SMT生产的TYM部件号
            fcminfo_id_smt_hardware_versions,//此id用于获取所有探测器卡(FCM)的SMT生产的硬件版本号
            fcminfo_id_smt_manufact_dates,//此id用于获取所有探测器卡(FCM)的SMT生产的生产日期
            fcminfo_id_smt_supplier_names,//此id用于获取所有探测器卡(FCM)的SMT生产的供应商缩写

            fcminfo_id_tym_detector_module_sns,//此id用于获取所有探测器卡(FCM)的TYM生产的探测器模块序列号
            fcminfo_id_tym_hardware_versions,//此id用于获取所有探测器卡(FCM)的TYM生产的硬件版本号
            fcminfo_id_tym_manufact_dates,//此id用于获取所有探测器卡(FCM)的TYM生产的生产日期
            fcminfo_id_tym_supplier_names,//此id用于获取所有探测器卡(FCM)的TYM生产的供应商缩写
            fcminfo_id_tym_detector_module_classs,//此id用于获取所有探测器卡(FCM)的TYM生产的探测器模块等级


            dcminfo_id_smt_pcba_sn,//此id用于获取数据采集板(DCM)的SMT生产的PCBA序列号
            dcminfo_id_smt_tpn,//此id用于获取数据采集板(DCM)的SMT生产的TYM部件号
            dcminfo_id_smt_hardware_version,//此id用于获取数据采集板(DCM)的SMT生产的硬件版本号
            dcminfo_id_smt_manufact_date,//此id用于获取数据采集板(DCM)的SMT生产的生产日期
            dcminfo_id_smt_supplier_name,//此id用于获取数据采集板(DCM)的SMT生产的供应商缩写
        };




        [StructLayout(LayoutKind.Sequential)]
        public struct trigger_info
        {
            public int pulse_level;//表示触发脉冲电平 默认:高电平
            public int input_pulse_num;//输入脉冲个数,默认:1
            public int output_pulse_width;//输出脉冲宽度，默认:50ms
            public int outputLinenum_per_trigger;//表明每触发一次trigger上传多少行数据，默认:512

        };


        const int edge_trigger_default = 0x03;//默认为上升沿
        const int discarded_image_lines_per_frame_default = 5;//每帧图像丢弃行数，默认丢弃5行
        const int image_lines_per_frame_default = 512;//每帧图像行数，默认为512行


        public struct param_for_work_mode0
        {
            public int integration_time_us;//积分时间

        };

        public struct param_for_work_mode1
        {
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
        };

        public struct param_for_work_mode3
        {
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
        };

        public struct param_for_work_mode4
        {
            public int image_lines_per_frame;//每帧图像行数
            public int integration_time_us;//积分时间
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
            public int discarded_image_lines_per_frame;// 每帧图像丢弃行数
        };

        public struct param_for_work_mode5
        {
            public int constant_integration_time_us;//常数积分时间
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
            public int external_trigger_filtering_time_threshold_us;//外触发屏蔽阈值
        };

        public struct param_for_work_mode6
        {
            public int times_of_frequency_division;//分频倍数
            public int constant_integration_time_us;//常数积分时间
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
            public int external_trigger_filtering_time_threshold_us;//外触发屏蔽阈值
            public int feedback_pulse_of_image_acquisition_quantity;//每采集多少行输出反馈脉冲
            public int feedback_pulse_width_ms;// 反馈脉冲宽度
        };

        public struct param_for_work_mode7
        {
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
            public int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
            public int integration_time_us;//积分时间
        };

        public struct param_for_work_mode8
        {
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
            public int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
            public int integration_time_us;//积分时间
        };

        public struct param_for_work_mode9
        {
            public int edge_trigger;//帧/外触发沿：0x03为上升沿，0x00为下降沿
            public int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
            public int integration_time_us;//积分时间
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct work_mode_param
        {
            public param_for_work_mode0 param_mode0;
            public param_for_work_mode1 param_mode1;
            public param_for_work_mode3 param_mode3;
            public param_for_work_mode4 param_mode4;
            public param_for_work_mode5 param_mode5;
            public param_for_work_mode6 param_mode6;
            public param_for_work_mode7 param_mode7;
            public param_for_work_mode8 param_mode8;
            public param_for_work_mode9 param_mode9;
        };



        public enum TYM_VAR_TYPE
        {
            TYM_INT = 0,
            TYM_DOUBLE = 1,
            TYM_USHORT = 2,
            TYM_UINT_ARR = 3,
            TYM_UCHAR_ARR = 4,
            TYM_DOUBLE_ARR = 5,
            TYM_UCHAR = 6
        };


        public struct VAL
        {
            public int val_int;
            public double val_double;
            public ushort val_ushort;
            public IntPtr val_uintArr;//unsigned int*
            public IntPtr val_ucharArr;//unsigned char*
            public IntPtr val_doubleArr;//double*
            public byte val_uchar;//unsigned char        
        };



        [StructLayout(LayoutKind.Sequential)]
        public struct TYM_Variant
        {
            public TYM_VAR_TYPE vt;
            public int arr_length;
            public VAL val;

        }

        public enum ATTRIBUTE_ID
        {
            attr_id_lineNumber,
            attr_id_data_control_board_temperature,
            attr_id_data_control_board_humidity,
            attr_id_xray_flag,
            attr_id_data_mode,
            attr_id_voltage1_dcm,
            attr_id_voltage2_dcm,
            attr_id_voltage3_dcm,
            attr_id_voltage4_dcm,
            atrr_id_trigger_pass_reg,
            attr_id_voltage1_all_fcms,
            attr_id_voltage2_all_fcms,
            attr_id_voltage3_all_fcms,
            attr_id_voltage4_all_fcms,
            attr_id_voltage5_all_fcms,
            attr_id_voltage6_all_fcms,
            attr_id_voltage7_all_fcms,
            attr_id_temperature_all_fcms,
            attr_id_humidity_all_fcms,
        };



        [StructLayout(LayoutKind.Sequential)]
        public struct userdata_pangu
        {
            public int height;
            public int blocknum;
            public IntPtr lineNumber; //unsigned int*
            public IntPtr temperature; //unsigned short*
            public IntPtr humidity; //unsigned short*
            public IntPtr voltage1; //unsigned short*
            public IntPtr voltage2; //unsigned short*
            public IntPtr voltage3; //unsigned short*
            public IntPtr voltage4; //unsigned short*

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct userdata_panguc
        {
            public userdata_pangu usrdata_pangu;
            public IntPtr xray_flag;//unsigned char*
            public IntPtr data_mode;//unsigned char*

        }

        [StructLayout(LayoutKind.Sequential)]//非托管代码（就是非net生成的代码），指定调用约定
        public struct info_header_setting
        {

            public int null_byte_num;
            public IntPtr buf_item_order;//unsigned char*
            public int bufsize_item_order;
            public enum item_id { id_linenum, id_null_byte };

        }

        public const int id_linenum = 0;
        public const int id_null_byte = 1;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void tymfn_datacallback(IntPtr buffer, IntPtr extra_info, IntPtr user_data, int instance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void tymfn_status_callback(IntPtr user_data, int instance);

    }
}
