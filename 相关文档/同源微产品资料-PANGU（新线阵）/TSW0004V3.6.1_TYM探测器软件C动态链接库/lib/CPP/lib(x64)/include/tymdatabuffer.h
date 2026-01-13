#pragma once

typedef struct tymdata_buffer
{
	int line_num;           // line count
	int pixels_per_line;    // pixel count of each scan line
	int bytes_per_pixel;    // byte count of each pixel:1,2,3,4 [default:2, 16 bits data]
	int bytes_per_line;     // pixels_per_line * bytes_per_pixel
	unsigned char* data;    // data buffer
} tymimage_buffer_t;


enum HARDWARE_INFO_ID
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



typedef struct tymscan_status_info_all
{
	double status_data_control_board_temperature;//数据采集板的温度信息
	double status_data_control_board_humidity;//数据采集板的湿度信息

	double* status_data_control_board_voltages;//数据采集板的电压信息，由数组存储
	int num_of_vols_data_control_board;//存储数据采集板电压信息的数组大小

	int num_of_detectors;//探测器卡数量
	double* status_temperature_all_detectors;//每个探测器卡的温度信息，由数组存储，数组大小为探测器数量
	double* status_humidity_all_detectors;//每个探测器卡的湿度信息，由数组存储，数组大小为探测器数量

	int num_of_vols_per_detector;//每个探测器所包含的电压信息数量。
	double** status_voltages_all_detectors;//每个探测器卡的电压信息。第一层指针指向某块探测器卡的电压信息数组，第二层指针指向电压信息数组的某个电压。

}tymscan_status_info_all_t;



typedef struct trigger_info
{
	int pulse_level=1;//表示触发脉冲电平 默认:高电平
	int input_pulse_num = 1;//输入脉冲个数,默认:1
	int output_pulse_width = 50;//输出脉冲宽度，默认:50ms
	int outputLinenum_per_trigger = 512;//表明每触发一次trigger上传多少行数据，默认:512

}trigger_info_t;

const int edge_trigger_default=0x03;//默认为上升沿
const int discarded_image_lines_per_frame_default = 5;//每帧图像丢弃行数，默认丢弃5行
const int image_lines_per_frame_default = 512;//每帧图像行数，默认为512行
const int timestamp_collection_edge_default = 0x00;
const int parity_default = 0x00;

typedef struct param_for_work_mode0
{
	int integration_time_us;//积分时间

}param_for_work_mode0_t;

typedef struct param_for_work_mode1
{
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int external_trigger_filtering_time_threshold_us;//外触发屏蔽阈值

}param_for_work_mode1_t;



typedef struct param_for_work_mode3
{
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int external_trigger_filtering_time_threshold_us;//外触发屏蔽阈值

}param_for_work_mode3_t;

typedef struct param_for_work_mode4
{
	int image_lines_per_frame= image_lines_per_frame_default;//每帧图像行数
	int integration_time_us;//积分时间
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int discarded_image_lines_per_frame  = discarded_image_lines_per_frame_default;// 每帧图像丢弃行数

}param_for_work_mode4_t;

typedef struct param_for_work_mode5
{
	int constant_integration_time_us;//常数积分时间
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int external_trigger_filtering_time_threshold_us;//外触发屏蔽阈值

}param_for_work_mode5_t;

typedef struct param_for_work_mode6
{
	int times_of_frequency_division;//分频倍数
	int constant_integration_time_us;//常数积分时间
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int external_trigger_filtering_time_threshold_us;//外触发屏蔽阈值
	int feedback_pulse_of_image_acquisition_quantity;//每采集多少行输出反馈脉冲
	int feedback_pulse_width_ms;// 反馈脉冲宽度


}param_for_work_mode6_t;

typedef struct param_for_work_mode7
{
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
	int interval_between_xray_offset_sampling;//射线本底积分时间间隔
	int corrected_baseline_average_times;//减本底平均次数，取值1/2/4/8
	int integration_time_us;//积分时间
	//注意：interval_between_xray_offset_sampling与integration_time_us之和，需大于600us

}param_for_work_mode7_t;

typedef struct param_for_work_mode8
{
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
	int integration_time_us;//积分时间

}param_for_work_mode8_t;

typedef struct param_for_work_mode9
{
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
	int interval_between_xray_offset_sampling;//射线本底积分时间间隔
	int integration_time_us;//积分时间
	//注意：interval_between_xray_offset_sampling与integration_time_us之和，需大于600us

}param_for_work_mode9_t;


typedef struct param_for_work_mode10
{
	int timestamp_collection_edge = timestamp_collection_edge_default;//时间戳采集沿：0x03为上升沿，0x00为下降沿
	int parity = parity_default;//奇偶校验设置:0x00为不校验，0x02为偶校验，0x01为奇校验

}param_for_work_mode10_t;


typedef struct param_for_work_mode11
{
	int timestamp_collection_edge = timestamp_collection_edge_default;//时间戳采集沿：0x03为上升沿，0x00为下降沿
	int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
	int interval_between_xray_offset_sampling;//射线本底积分时间间隔
	int corrected_baseline_average_times;//减本底平均次数，取值1/2/4/8
	int integration_time_us;//积分时间
	int parity = parity_default;//奇偶校验设置:0x00为不校验，0x02为偶校验，0x01为奇校验


}param_for_work_mode11_t;

typedef struct param_for_work_mode12
{
	int timestamp_collection_edge = timestamp_collection_edge_default;//时间戳采集沿：0x03为上升沿，0x00为下降沿
	int delay_time_from_external_trigger_to_integration_start_us;//积分开始相对外触发延时
	int integration_time_us;//积分时间
	int parity = parity_default;//奇偶校验设置:0x00为不校验，0x02为偶校验，0x01为奇校验


}param_for_work_mode12_t;


typedef struct param_for_work_mode13
{
	int trigger_cycle_us;//触发时间
	int integration_time_us;//积分时间

}param_for_work_mode13_t;

typedef struct param_for_work_mode14
{
	int image_lines_per_frame = image_lines_per_frame_default;//每帧图像行数
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int discarded_image_lines_per_frame = discarded_image_lines_per_frame_default;// 每帧图像丢弃行数

}param_for_work_mode14_t;


typedef struct param_for_work_mode15
{
	int edge_trigger = edge_trigger_default;//帧/外触发沿：0x03为上升沿，0x00为下降沿
	int integration_time_us;//外触发周期
	double jitt_rate;//抖动率。例如：若抖动率为千分之1，请设置为0.001

}param_for_work_mode15_t;


typedef struct param_for_work_mode16
{
	int integration_time_us;//积分时间
	double jitt_rate;//抖动率。例如：若抖动率为千分之1，请设置为0.001

}param_for_work_mode16_t;


typedef struct work_mode_param
{
	param_for_work_mode0 param_mode0;
	param_for_work_mode1 param_mode1;
	param_for_work_mode3 param_mode3;
	param_for_work_mode4 param_mode4;
	param_for_work_mode5 param_mode5;
	param_for_work_mode6 param_mode6;
	param_for_work_mode7 param_mode7;
	param_for_work_mode8 param_mode8;
	param_for_work_mode9 param_mode9;
	param_for_work_mode10 param_mode10;
	param_for_work_mode11 param_mode11;
	param_for_work_mode12 param_mode12;
	param_for_work_mode13 param_mode13;
	param_for_work_mode14 param_mode14;
	param_for_work_mode15 param_mode15;
	param_for_work_mode16 param_mode16;
}work_mode_param_t;




enum TYM_VAR_TYPE
{
	TYM_INT = 0,
	TYM_DOUBLE = 1,
	TYM_USHORT=2,
	TYM_UINT_ARR = 3,
	TYM_UCHAR_ARR=4,
	TYM_DOUBLE_ARR=5,
	TYM_UCHAR=6
};


typedef struct TYM_Variant
{
	TYM_VAR_TYPE vt;
	int arr_length;
	struct {
		int val_int;
		double val_double;
		unsigned short val_ushort;
		unsigned int* val_uintArr;
		unsigned char* val_ucharArr;
		double* val_doubleArr;
		unsigned char val_uchar;

	}val;
	

}TYM_Variant_t;

enum ATTRIBUTE_ID
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
	attr_id_temperature_thermistor_all_fcms,
	attr_id_temperature_all_fcms,
	attr_id_humidity_all_fcms,
	attr_id_timeStamp,

	attr_id_fuxi64_temperature,

	// for dcm se
	attr_id_voltage1_dcm_se,
	attr_id_voltage2_dcm_se,
	attr_id_voltage3_dcm_se,
	attr_id_voltage4_dcm_se,
	attr_id_voltage5_dcm_se,
	attr_id_voltage6_dcm_se,
	attr_id_voltage7_dcm_se,
	attr_id_voltage8_dcm_se,

};




typedef struct userdata_daq
{
	int height;
	int blocknum;

	unsigned int* lineNumber;
	unsigned short* temperature;
	unsigned short* humidity;


}userdata_daq_t;


typedef struct userdata_pangu_se_hlx
{
	int total_QTY;//总卡数
	int ch1_CardQTY;
	int ch2_CardQTY;
	int ch3_CardQTY;
	int ch4_CardQTY;
	int pixel_per_card;
	double* temperature;
	double* humidity;


}userdata_pangu_se_hlx_t;



typedef struct userdata_pangu
{
	int height;
	int blocknum;
	unsigned int* lineNumber;
	unsigned short* temperature;
	unsigned short* humidity;
	unsigned short* voltage1;
	unsigned short* voltage2;
	unsigned short* voltage3;
	unsigned short* voltage4;
	
}userdata_pangu_t;

typedef struct userdata_panguc
{
	userdata_pangu usrdata_pangu;
	unsigned char* xray_flag;
	unsigned char* data_mode;

}userdata_panguc_t;

typedef struct info_header_setting {

	int null_byte_num;//指明填充几个空字节
	unsigned char* buf_item_order;//填入想要的项目顺序
	int bufsize_item_order;//上述数组大小
	enum item_id{ id_linenum,id_null_byte};//项目id,请勿更改

}info_header_setting_t;




#ifdef _WIN32
// Windows平台
typedef void (__stdcall* tymfn_datacallback)(const tymdata_buffer* buffer, void* extra_info, void* user_data, int instance);
typedef void (__stdcall* tymfn_status_callback)(void* user_data, int instance);
#else
// Linux平台
typedef void (__attribute__((__stdcall__))* tymfn_datacallback)(const tymdata_buffer* buffer, void* extra_info, void* user_data, int instance);
typedef void (__attribute__((__stdcall__))* tymfn_status_callback)(void* user_data, int instance);
#endif
