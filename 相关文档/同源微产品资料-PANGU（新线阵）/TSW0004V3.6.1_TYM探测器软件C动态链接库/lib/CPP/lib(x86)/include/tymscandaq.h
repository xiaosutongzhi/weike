#pragma once
#  ifdef _WIN32
#    define TYM_DECL_EXPORT     extern "C" __declspec(dllexport)
#    define TYM_DECL_IMPORT     extern "C" __declspec(dllimport)
#    define TYMSCANAPI            __stdcall
#  else
#    define TYM_DECL_EXPORT     __attribute__((visibility("default")))
#    define TYM_DECL_IMPORT     __attribute__((visibility("default")))
#    define TYM_DECL_HIDDEN     __attribute__((visibility("hidden")))
#    define TYMSCANAPI
#  endif

#  if defined(TYMSCANDAQ_LIBRARY)
#    define TYMSCAN_EXPORT TYM_DECL_EXPORT
#  else
#    define TYMSCAN_EXPORT TYM_DECL_IMPORT
#  endif






#include "tymdatabuffer.h"


const int TYM_NOERROR = 0;
const int TYM_TIMEOUT = -1;
const int TYM_RETURNERR = -2;
const int TYM_PRODUCT_NO_SUCH_FUNCTION = 0XEE;//表示该产品类型无该功能
const int TYM_CMDSENDFORBIDDEN = 0XFB;
const int TYM_CMDDATAOUTOFRANGE = 0XAA;
const int TYM_CMDWRONGDATALENGTH = 0XAE;
const int TYM_INSTANCE_INVALID = 0XCC;
const int TYM_WRITE_READ_MISMATCH = 0XAB;
const int TYM_VAR_IS_NULL = 0XAC;


const int TYM_FCM_SYSYTYPE_NOT_SAME = 0XF01;//表示连接的所有FCM中，该参数状态不一致
const int TYM_FCM_PIXNUM_PERCARD_NOT_SAME = 0XF02;
const int TYM_FCM_ENERGYMODE_NOT_SAME = 0XF03;
const int TYM_FCM_OPMODE_NOT_SAME = 0XF04;
const int TYM_FCM_PIXREVERSE_NOT_SAME = 0XF05;
const int TYM_FCM_LEDCTRL_NOT_SAME = 0XF06;
const int TYM_FCM_DETECTOR_TYPE_NOT_SAME = 0XF07;
const int TYM_FCM_ASICMODE_NOT_SAME = 0XF08;

const int TYM_CALIB_GET_CORRECT_DIR_FAILED = 1;//获取校准文件夹失败
const int TYM_CALIB_CANT_OPEN_OFFSET_FILE = 2;//无法打开offset校准文件
const int TYM_CALIB_OFFSET_PIXNUM_DIFF = 3;//当前的像素数与offset校准文件中的像素数不一致
const int TYM_CALIB_CANT_OPEN_COE_FILE = 4;//无法打开coe校准文件
const int TYM_CALIB_COE_PIXNUM_DIFF = 5;//当前的像素数与coe校准文件中的像素数不一致
const int TYM_CALIB_CANT_OPEN_BASELINE_FILE = 6;//无法打开baseline校准文件
const int TYM_CALIB_MASK_INDEX_OUT_OF_RANGE = 7;//要屏蔽的像素索引超出了最大范围

/**
 * get sdk version, ex: v1.0.1
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_version(char* buf, int buf_length);

/*!
 * tymscan_init init daq
 * \param ipaddress_local local ip address
 * \param [in] ipaddress_daq daq ip address
 * \param [in] cmd_port command port
 * \param [in] data_port data port
 * \param [out] return instance ID
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_init(const char* ipaddress_local, const char* ipaddress_daq, int cmd_port, int data_port, int* instance);
/**
 * tymscan_inited query if the daq is initialized
 * \param [out] inited true:initialized false:not initialized
 * \param instance instance ID
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_inited(bool* inited, int instance);
/*!
 * tymscan_cleanup cleanup the daq
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_cleanup(int instance);
/**
 * get firmware version, ex: 1
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_version_firmware(char* version, int buf_length,int instance);
/*!
 * tymscan_connected query if the daq is connected
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_connected(bool* connected,int instance);
/*!
 * tymscan_grab_start start grabing data
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_grab_start(int instance,int count=0);
/*!
 * tymscan_grab_stop stop grabing data
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_grab_stop(int instance);


/*!
 * tymscan_get_gain_range get gain range [ex: 1-63]
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_gain_range(int* gain_min, int* gain_max, int instance);

/*!
 * tymscan_is_support_set_gain_per_card confirm whether the current product can set the gain individually for each detector card
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_is_support_set_gain_per_card(bool* is_support,int instance);

/*!
 * tymscan_set_gain_low set gain of low energy
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_gain_low(int card_no, int gain, int instance,int switch_code = 0);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_gain_low(int card_no, int* gain, int instance, int bufsize=1,int* switch_code = nullptr);
/*!
 * tymscan_set_gain_high set gain of high energy
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_gain_high(int card_no, int gain, int instance, int switch_code = 0);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_gain_high(int card_no, int* gain, int instance, int bufsize = 1, int* switch_code = nullptr);

/*!
 * tymscan_get_test_pattern_range get test pattern range [ex: 0-5, 0 is normal data mode]
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_test_pattern_range(int* pattern_min, int* pattern_max, int instance);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_test_pattern(int pattern, int instance);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_test_pattern(int* pattern, int instance);

/*!
 * tymscan_get_integral_time_range get integral time range [unit:us]
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_integral_time_range(int* time_min, int* time_max, int instance);
/*!
 * tymscan_set_integral_time set integral time [unit:us]
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_integral_time( int time_us, int instance, int switch_code=0);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_integral_time( int* time_us, int instance, int switch_code = 0);

/*!
 * tymscan_set_daq_ip set daq ip address [will save to flash]
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_daq_ip(const char* ipaddress, int instance, int switch_code = 0);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_daq_ip(char* ipaddress, int ipaddress_length, int instance, int switch_code = 0);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_daq_port(int cmd_port,int img_port, int instance);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_daq_port(int* cmd_port, int* img_port, int instance);

/*!
 * tymscan_set_pixelnum_percard set pixel number of each card
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_pixelnum_percard(int pixelnum,int instance);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_pixelnum_percard(int* pixelnum,int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_pixelnum(int* pixelnum,int instance);

/*!
 * tymscan_set_trigger_mode
 * \param mode 0-internal 1-external
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_trigger_mode(int mode,int instance,trigger_info* trig_info=nullptr);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_trigger_mode(int* mode,int instance, trigger_info* trig_info = nullptr);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_energy_mode(int mode,int instance);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_energy_mode(int* mode,int instance);


/*!
 * tymscan_set_cardnumber set card number of each link
 * \param linkid link id, range ex:[1~4] note: different product may different
 * \param number card number, range ex:[1~16] note: different product may different
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_cardnumber(int linkid, int number, int instance);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_cardnumber(int linkid, int* number, int instance);

/*!
 * tymscan_set_datacallback set data callback function
 * \param block_line_num line number  of each callback notification
 * \param userdata user data will t
 */
TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_datacallback(tymfn_datacallback callback,void* user_data, int block_line_num, int instance, void* data_format = nullptr);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_status_callback(tymfn_status_callback callback, void* user_data,int time_us, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_special_set(int cmd_id, int value, int bytes_this_cmd, int instance);
TYMSCAN_EXPORT int TYMSCANAPI tymscan_special_get(int cmd_id, int* value, int bytes_this_cmd, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_send_command(int cmdId, int op, char* data, char* result, int maxResultLen, int instance);


TYMSCAN_EXPORT int TYMSCANAPI tymcan_doOffsetCalibration(int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymcan_doGainCalibration(unsigned int target, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymcan_setBaseLine(unsigned int baseLine, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymcan_set_sendCalibratedData_Enable(bool offset_enable, bool coe_enable, bool baseline_enable, int instance, int* buf_mask_pixs=nullptr, int buf_size=0);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_pixel_order_mode(int mode, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_systype(int *type,int* bytes_per_pixel,int* bits_range, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_save_config(int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_is_attribute_exist(int attribute_id,bool* is_exist, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_attribute_value(int attribute_id, TYM_Variant* var, int instance);


TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_total_lost_lines(unsigned int* total_lost_lines, int instance);


TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_work_mode(int mode, int instance,work_mode_param* mode_param = nullptr) ;

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_work_mode(int* mode, int instance, work_mode_param* mode_param = nullptr,int switch_code = 0);


TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_hardware_info_file(const char* output_filepath, int instance, int auth_code=0);


TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_hardware_info(int hwinfo_id,char* info_buf,int info_buf_length,int instance, int auth_code = 0);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_hardware_info_length(int hwinfo_id, int* info_length, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_fcm_firmware_version(unsigned short* version, int fcm_qty, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_pdc_set_pitch_and_gaps(double pitch, double* gaps, int buf_size, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_pdc_enable(bool enable, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_verify_fcm_info(int instance);


TYMSCAN_EXPORT int TYMSCANAPI tymscan_enable_interpolation(bool enable, int instance,unsigned int max_allow_lost_lines = 5);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_enable_multi_row_product_move(bool enable, int instance);


TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_detector_type_str(char* detector_type, int detector_type_length, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_detector_type_all_fcms_length(int* info_length, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_detector_type_all_fcms(char* detector_type_all_fcms, int detector_type_all_fcms_length, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_status_info_all(tymscan_status_info_all* stinfo_all, int instance);


TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_calib_timeout_for_extrig(int time_out_ms, int instance);