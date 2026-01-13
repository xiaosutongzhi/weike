#pragma once
#include "tymscandaq.h"

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_infopage(int channel, int page, const char* filePath, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_infopage(int channel, int page, const char* filePath, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_TH_enable_XH(bool enable, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_custom_pixnum(int pixelnum, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_detector_type(int detector_type, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_detector_type(int* detector_type, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymcan_set_sendCalibratedData_Enable_Defaults(bool offset_enable, bool coe_enable, bool baseline_enable, int instance);


enum DetectorType {//¹Ì¶¨ÅäÖÃ£¬ÇëÎðÐÞ¸Ä
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

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_auth_bit(int auth_id, int value, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_enable_auto_get_cardnum(int enable, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_is_auto_get_cardnum(int* is_auto,int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_set_fcm_info(unsigned short fcm_addr, int slot_id, const char* fcm_info, int instance);

TYMSCAN_EXPORT int TYMSCANAPI tymscan_get_fcm_info(unsigned short fcm_addr, int slot_id, char* fcm_info_buf, int buf_length, int instance);



/*
TYMSCAN_EXPORT int TYMSCANAPI set_pixel_gain(const char* file_path, int instance);
TYMSCAN_EXPORT int TYMSCANAPI get_pixel_gain(const char* file_path, int instance) ;

TYMSCAN_EXPORT int TYMSCANAPI set_offset_data(const char* file_path, int instance) ;
TYMSCAN_EXPORT int TYMSCANAPI get_offset_data(const char* file_path, int instance);

*/
/*
TYMSCAN_EXPORT int TYMSCANAPI set_baseline(unsigned int base_line, int instance);
TYMSCAN_EXPORT int TYMSCANAPI get_baseline(unsigned int* base_line, int instance);*/


