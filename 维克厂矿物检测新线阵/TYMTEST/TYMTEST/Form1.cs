using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYMDetector;
using System.Threading;
using TYMTest;


namespace TYMTest
{
    public partial class Form1 : Form
    {
        const int DISCONNECTED = -1;

        int instanceId_board1 = DISCONNECTED;
        SdkInterface.tymfn_datacallback tymcallback;
        SdkInterface.tymfn_status_callback tym_status_callback;

        const int maxchannels = 6;
        TextBox[] tbx_6channel = new TextBox[maxchannels];



        private delegate void ImageDelegate(short[] buffer);
        ImageDelegate imgUpdate;

        private delegate void chartDelegate(short[] buffer);
        chartDelegate cS;


        private delegate void StatusDelegate();
        StatusDelegate SD;



        int _imgwidth = 256;
        int _imgheight = 512;
        int _blockheight = 128;
        int _bytes_per_pixel = 2;
        int _bits_range = 16;
        int _systemtype = 0;
        int _channel_number = 4;

        short[] recv_data_store = null;
        byte[] recv_data_store_grey = null;
        IntPtr recv_data_store_grey_ptr;
        byte[] whole_image_store_grey = null;
        ushort[] frame_data_buffer = null;
        ushort[] block_data_buffer = null;

        bool _single_energy = false;

        int[] _gainlow_everycard = null;
        int[] _gainhigh_everycard = null;

        GainList _gainlist = null;
        string[] _card_num_range = null;

        //StreamWriter _streamw;

        enum SystemType
        {
            DAQ = 1,
            DAQII = 2,
            DAQIIEX = 4,
            PANGUC = 5,//single energy
            FCM_G = 10,//single energy
            _04X8 = 11//single energy
        }

        public Form1()
        {
            InitializeComponent();

            init_UI();

            tymcallback = callback;

            tym_status_callback = status_callback;

            imgUpdate = image_update;
            cS = chartShua;
            SD = status_delegate;

            set_controls_enable(false);

            _gainlist = new GainList();
            _gainlist.Hide();


            // _streamw = File.CreateText(@"E:\test3.txt");


            /*
            for(int i=0;i<100;i++)
            {
                if (instanceId_board1 != DISCONNECTED)
                {
                    SdkInterface.tymscan_cleanup(instanceId_board1);
                    //Thread.Sleep(1000);
                    instanceId_board1 = DISCONNECTED;
                }

                SdkInterface.tymscan_init("192.168.10.10", "192.168.10.1", 7171, 7474, ref instanceId_board1);
                //Thread.Sleep(1000);
            }
            
            int a = 0;*/
        }


        private void status_delegate()
        {

            tbx_status_callback.Clear();
            int var_size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(SdkInterface.TYM_Variant));
            bool attr_is_temperature_all_fcms = false;
            SdkInterface.tymscan_is_attribute_exist((int)SdkInterface.ATTRIBUTE_ID.attr_id_temperature_all_fcms, ref attr_is_temperature_all_fcms, instanceId_board1);
            if (attr_is_temperature_all_fcms)
            {

                IntPtr var = Marshal.AllocHGlobal(var_size);
                SdkInterface.tymscan_get_attribute_value((int)SdkInterface.ATTRIBUTE_ID.attr_id_temperature_all_fcms, var, instanceId_board1);
                SdkInterface.TYM_Variant strucut_var = (SdkInterface.TYM_Variant)Marshal.PtrToStructure(var, typeof(SdkInterface.TYM_Variant));
                double[] temperature_all_fcms_store = new double[strucut_var.arr_length];
                Marshal.Copy(strucut_var.val.val_doubleArr, temperature_all_fcms_store, 0, strucut_var.arr_length);

                string temp_all = "temperature_all_fcms:  ";
                for (int i = 0; i < temperature_all_fcms_store.Length; i++)
                {
                    temp_all += "FCM" + (i + 1).ToString() + ":  " + Math.Round(temperature_all_fcms_store[i], 2).ToString() + "   ";
                }

                tbx_status_callback.AppendText(temp_all + "\r\n");

            }


            bool attr_is_humidity_all_fcms = false;
            SdkInterface.tymscan_is_attribute_exist((int)SdkInterface.ATTRIBUTE_ID.attr_id_humidity_all_fcms, ref attr_is_humidity_all_fcms, instanceId_board1);
            if (attr_is_humidity_all_fcms)
            {

                IntPtr var = Marshal.AllocHGlobal(var_size);
                SdkInterface.tymscan_get_attribute_value((int)SdkInterface.ATTRIBUTE_ID.attr_id_humidity_all_fcms, var, instanceId_board1);
                SdkInterface.TYM_Variant strucut_var = (SdkInterface.TYM_Variant)Marshal.PtrToStructure(var, typeof(SdkInterface.TYM_Variant));
                double[] humidity_all_fcms_store = new double[strucut_var.arr_length];
                Marshal.Copy(strucut_var.val.val_doubleArr, humidity_all_fcms_store, 0, strucut_var.arr_length);

                string temp_all = "humidity_all_fcms:  ";
                for (int i = 0; i < humidity_all_fcms_store.Length; i++)
                {
                    temp_all += "FCM" + (i + 1).ToString() + ":  " + Math.Round(humidity_all_fcms_store[i], 2).ToString() + "   ";
                }

                tbx_status_callback.AppendText(temp_all + "\r\n");

            }





        }


        private void init_UI()
        {
            StringBuilder str_buider = new StringBuilder();
            SdkInterface.tymscan_version(str_buider, 20);
            string sdkversion = str_buider.ToString();
            tbx_sdkversion.Text = sdkversion;

            tbx_hostip.Text = "192.168.10.100";
            tbx_daqip.Text = "192.168.10.1";
            tbx_cmdport.Text = "7171";
            tbx_imgport.Text = "7474";

            // chart1.ChartAreas[0].AxisY.Maximum = 65535;
        }

        private void init_data_storage()
        {
            try
            {
                recv_data_store = new short[_imgwidth * _blockheight];
                recv_data_store_grey = new byte[_imgwidth * _blockheight];

                whole_image_store_grey = new byte[_imgwidth * _imgheight];
                Marshal.FreeHGlobal(recv_data_store_grey_ptr);
                recv_data_store_grey_ptr = Marshal.AllocHGlobal(_imgwidth * _imgheight);

                frame_data_buffer = new ushort[_imgwidth * _imgheight];
                block_data_buffer = new ushort[_imgwidth * _blockheight];
            }
            catch
            {
                MessageBox.Show("Not enough space to allocate memory!");
                return;
            }

        }



        private void update_presettingUI()
        {
            for (int i = 0; i < 6; i++)
            {
                tbx_6channel[i] = null;
            }
            tbx_pixelsperdetector.Text = "";
            tbx_frameheight.Text = "";
            tbx_blockheight.Text = "";


            switch (_systemtype)
            {
                case (int)SystemType.DAQ:
                case (int)SystemType.DAQII:
                    presettingUI_2Channel();
                    _channel_number = 2;
                    break;
                case (int)SystemType.DAQIIEX:
                    presettingUI_6Channel();
                    _channel_number = 6;
                    break;
                default:
                    presettingUI_4Channel();
                    _channel_number = 4;
                    break;
            }
            tbx_frameheight.Text = (_imgheight).ToString();
            tbx_blockheight.Text = (_blockheight).ToString();


            for (int i = 1; i <= _channel_number; i++)
            {
                int cardnum_thischannel = 0;
                SdkInterface.tymscan_get_cardnumber(i, ref cardnum_thischannel, instanceId_board1);

                tbx_6channel[i - 1].Text = cardnum_thischannel.ToString();

            }

            int pixel_perdetector = 0;
            SdkInterface.tymscan_get_pixelnum_percard(ref pixel_perdetector, instanceId_board1);

            tbx_pixelsperdetector.Text = pixel_perdetector.ToString();

            int pixelnumber = 0;
            SdkInterface.tymscan_get_pixelnum(ref pixelnumber, instanceId_board1);
            tbx_pixelnumber.Text = pixelnumber.ToString();
            _imgwidth = pixelnumber;

            int energymode = 0;
            SdkInterface.tymscan_get_energy_mode(ref energymode, instanceId_board1);
            cbx_energymode.SelectedIndex = energymode - 1;

            init_data_storage();
        }
        private void presettingUI_2Channel()
        {
            Label lbl_ch1 = new Label() { Location = new Point(5, 5), Width = 77, Height = 12, Text = "CH1 CardNum:" };

            TextBox tbx_ch1 = new TextBox() { Location = new Point(85, 5), Width = 39, Height = 21 };
            tbx_6channel[0] = tbx_ch1;

            Label lbl_ch2 = new Label() { Location = new Point(127, 5), Width = 77, Height = 12, Text = "CH2 CardNum:" };

            TextBox tbx_ch2 = new TextBox() { Location = new Point(207, 5), Width = 39, Height = 21 };
            tbx_6channel[1] = tbx_ch2;



            panel_presetting.Controls.Clear();
            panel_presetting.Controls.Add(lbl_ch1);
            panel_presetting.Controls.Add(tbx_ch1);
            panel_presetting.Controls.Add(lbl_ch2);
            panel_presetting.Controls.Add(tbx_ch2);




        }


        private void presettingUI_4Channel()
        {
            Label lbl_ch1 = new Label() { Location = new Point(5, 5), Width = 77, Height = 12, Text = "CH1 CardNum:" };

            TextBox tbx_ch1 = new TextBox() { Location = new Point(85, 5), Width = 39, Height = 21 };

            Label lbl_ch2 = new Label() { Location = new Point(127, 5), Width = 77, Height = 12, Text = "CH2 CardNum:" };

            TextBox tbx_ch2 = new TextBox() { Location = new Point(207, 5), Width = 39, Height = 21 };

            Label lbl_ch3 = new Label() { Location = new Point(249, 5), Width = 77, Height = 12, Text = "CH3 CardNum:" };

            TextBox tbx_ch3 = new TextBox() { Location = new Point(329, 5), Width = 39, Height = 21 };

            Label lbl_ch4 = new Label() { Location = new Point(371, 5), Width = 77, Height = 12, Text = "CH4 CardNum:" };

            TextBox tbx_ch4 = new TextBox() { Location = new Point(451, 5), Width = 39, Height = 21 };



            panel_presetting.Controls.Clear();
            panel_presetting.Controls.Add(lbl_ch1);
            panel_presetting.Controls.Add(tbx_ch1);
            panel_presetting.Controls.Add(lbl_ch2);
            panel_presetting.Controls.Add(tbx_ch2);
            panel_presetting.Controls.Add(lbl_ch3);
            panel_presetting.Controls.Add(tbx_ch3);
            panel_presetting.Controls.Add(lbl_ch4);
            panel_presetting.Controls.Add(tbx_ch4);


            tbx_6channel[0] = tbx_ch1;
            tbx_6channel[1] = tbx_ch2;
            tbx_6channel[2] = tbx_ch3;
            tbx_6channel[3] = tbx_ch4;


        }


        private void presettingUI_6Channel()
        {
            Label lbl_ch1 = new Label() { Location = new Point(5, 5), Width = 77, Height = 12, Text = "CH1 CardNum:" };

            TextBox tbx_ch1 = new TextBox() { Location = new Point(85, 5), Width = 39, Height = 21 };

            Label lbl_ch2 = new Label() { Location = new Point(127, 5), Width = 77, Height = 12, Text = "CH2 CardNum:" };

            TextBox tbx_ch2 = new TextBox() { Location = new Point(207, 5), Width = 39, Height = 21 };

            Label lbl_ch3 = new Label() { Location = new Point(249, 5), Width = 77, Height = 12, Text = "CH3 CardNum:" };

            TextBox tbx_ch3 = new TextBox() { Location = new Point(329, 5), Width = 39, Height = 21 };

            Label lbl_ch4 = new Label() { Location = new Point(371, 5), Width = 77, Height = 12, Text = "CH4 CardNum:" };

            TextBox tbx_ch4 = new TextBox() { Location = new Point(451, 5), Width = 39, Height = 21 };


            Label lbl_ch5 = new Label() { Location = new Point(493, 5), Width = 77, Height = 12, Text = "CH5 CardNum:" };

            TextBox tbx_ch5 = new TextBox() { Location = new Point(573, 5), Width = 39, Height = 21 };

            Label lbl_ch6 = new Label() { Location = new Point(615, 5), Width = 77, Height = 12, Text = "CH6 CardNum:" };

            TextBox tbx_ch6 = new TextBox() { Location = new Point(695, 5), Width = 39, Height = 21 };



            panel_presetting.Controls.Clear();
            panel_presetting.Controls.Add(lbl_ch1);
            panel_presetting.Controls.Add(tbx_ch1);
            panel_presetting.Controls.Add(lbl_ch2);
            panel_presetting.Controls.Add(tbx_ch2);
            panel_presetting.Controls.Add(lbl_ch3);
            panel_presetting.Controls.Add(tbx_ch3);
            panel_presetting.Controls.Add(lbl_ch4);
            panel_presetting.Controls.Add(tbx_ch4);
            panel_presetting.Controls.Add(lbl_ch5);
            panel_presetting.Controls.Add(tbx_ch5);
            panel_presetting.Controls.Add(lbl_ch6);
            panel_presetting.Controls.Add(tbx_ch6);


            tbx_6channel[0] = tbx_ch1;
            tbx_6channel[1] = tbx_ch2;
            tbx_6channel[2] = tbx_ch3;
            tbx_6channel[3] = tbx_ch4;
            tbx_6channel[4] = tbx_ch5;
            tbx_6channel[5] = tbx_ch6;

        }

        private void update_settingArea()
        {
            if (_systemtype == (int)SystemType.PANGUC)
            {
                lbl_triggertime.Visible = true;
                tbx_triggertime.Visible = true;
                lbl_delaytime.Visible = true;
                tbx_delaytime.Visible = true;
                lbl_opmode.Visible = true;
                cbx_opmode.Visible = true;

                int trigger_time = 0;
                int integration_time = 0;
                int delay_time = 0;
                SdkInterface.tymscan_get_integral_time(ref trigger_time, instanceId_board1, 0);
                SdkInterface.tymscan_get_integral_time(ref integration_time, instanceId_board1, 1);
                SdkInterface.tymscan_get_integral_time(ref delay_time, instanceId_board1, 2);

                tbx_integrationTime.Text = integration_time.ToString();
                tbx_triggertime.Text = trigger_time.ToString();
                tbx_delaytime.Text = delay_time.ToString();

                int op_mode = 0;
                SdkInterface.tymscan_special_get(0x06, ref op_mode, 1, instanceId_board1);
                cbx_opmode.SelectedIndex = op_mode - 1;
            }
            else
            {
                lbl_triggertime.Visible = false;
                tbx_triggertime.Visible = false;
                lbl_delaytime.Visible = false;
                tbx_delaytime.Visible = false;
                lbl_opmode.Visible = false;
                cbx_opmode.Visible = false;

                int integration_time = 0;
                SdkInterface.tymscan_get_integral_time(ref integration_time, instanceId_board1);
                tbx_integrationTime.Text = integration_time.ToString();

            }

            int test_pattern_min = 0;
            int test_pattern_max = 0;
            SdkInterface.tymscan_get_test_pattern_range(ref test_pattern_min, ref test_pattern_max, instanceId_board1);
            string[] test_pattern_range = new string[test_pattern_max - test_pattern_min + 1];
            for (int i = 0; i < test_pattern_range.Length; i++)
            {
                test_pattern_range[i] = i.ToString();
            }
            cbx_testpattern.Items.Clear();
            cbx_testpattern.Items.AddRange(test_pattern_range);

            int testpattern = 0;
            SdkInterface.tymscan_get_test_pattern(ref testpattern, instanceId_board1);
            cbx_testpattern.SelectedIndex = testpattern;


            SdkInterface.tymscan_set_test_pattern(cbx_testpattern.SelectedIndex, instanceId_board1);


            int tigger_mode = 0;
            SdkInterface.tymscan_get_trigger_mode(ref tigger_mode, instanceId_board1);

            cbx_triggermode.SelectedIndex = tigger_mode;



            string[] card_number_range = get_cardnum_range();
            _card_num_range = card_number_range;

            cbx_cardno_low.Items.Clear();
            cbx_cardno_low.Items.Add("All");
            cbx_cardno_low.Items.AddRange(card_number_range);

            cbx_cardno_high.Items.Clear();
            cbx_cardno_high.Items.Add("All");
            cbx_cardno_high.Items.AddRange(card_number_range);

            cbx_cardno_low.SelectedIndex = 0;
            cbx_cardno_high.SelectedIndex = 0;

            int gain_min = 0;
            int gain_max = 0;
            SdkInterface.tymscan_get_gain_range(ref gain_min, ref gain_max, instanceId_board1);
            string[] gain_range = new string[gain_max - gain_min + 1];
            for (int i = 1; i <= gain_range.Length; i++)
            {
                gain_range[i - 1] = i.ToString();
            }

            cbx_lowgain.Items.Clear();
            cbx_highgain.Items.Clear();
            cbx_lowgain.Items.AddRange(gain_range);
            cbx_highgain.Items.AddRange(gain_range);

            if (_single_energy)
            {
                label14.Visible = false;
                label13.Visible = false;
                cbx_cardno_high.Visible = false;
                cbx_highgain.Visible = false;
                lbl_lowgain.Text = "Gain:";


                _gainlow_everycard = new int[_card_num_range.Length];

                change_gain_status();

                if (is_all_gain_same(_gainlow_everycard))
                {
                    cbx_lowgain.SelectedIndex = _gainlow_everycard[0] - 1;
                }
                else
                {
                    cbx_lowgain.SelectedIndex = -1;
                }


            }
            else
            {
                label14.Visible = true;
                label13.Visible = true;
                cbx_cardno_high.Visible = true;
                cbx_highgain.Visible = true;
                lbl_lowgain.Text = "LowGain:";


                _gainlow_everycard = new int[_card_num_range.Length];
                _gainhigh_everycard = new int[_card_num_range.Length];

                change_gain_status();

                if (is_all_gain_same(_gainlow_everycard))
                {
                    cbx_lowgain.SelectedIndex = _gainlow_everycard[0] - 1;
                }
                else
                {
                    cbx_lowgain.SelectedIndex = -1;
                }
                if (is_all_gain_same(_gainhigh_everycard))
                {
                    cbx_highgain.SelectedIndex = _gainhigh_everycard[0] - 1;//ADAPT
                }
                else
                {
                    cbx_highgain.SelectedIndex = -1;
                }

            }


        }

        void change_gain_status()
        {
            int switch_code = 0;
            if (_single_energy)
            {

                IntPtr gainget_ptr = Marshal.AllocHGlobal(_card_num_range.Length * sizeof(int));
                SdkInterface.tymscan_get_gain_low(0, gainget_ptr, instanceId_board1, _card_num_range.Length, ref switch_code);
                Marshal.Copy(gainget_ptr, _gainlow_everycard, 0, _card_num_range.Length);
            }
            else
            {
                IntPtr gainlowget_ptr = Marshal.AllocHGlobal(_card_num_range.Length * sizeof(int));
                IntPtr gainhighget_ptr = Marshal.AllocHGlobal(_card_num_range.Length * sizeof(int));
                SdkInterface.tymscan_get_gain_low(0, gainlowget_ptr, instanceId_board1, _card_num_range.Length, ref switch_code);
                SdkInterface.tymscan_get_gain_high(0, gainhighget_ptr, instanceId_board1, _card_num_range.Length, ref switch_code);
                Marshal.Copy(gainlowget_ptr, _gainlow_everycard, 0, _card_num_range.Length);
                Marshal.Copy(gainhighget_ptr, _gainhigh_everycard, 0, _card_num_range.Length);
            }
        }

        bool is_all_gain_same(int[] gainbuf)
        {
            bool gain_same = true;
            for (int i = 1; i < gainbuf.Length; i++)
            {
                if (gainbuf[i] != gainbuf[0])
                {
                    gain_same = false;
                    break;
                }
            }
            return gain_same;
        }

        private string[] get_cardnum_range()
        {
            if (_systemtype == (int)SystemType.FCM_G)
            {
                const int max_card_num_per_G = 4;
                int[] cardnum_every_channel = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    SdkInterface.tymscan_get_cardnumber(i + 1, ref cardnum_every_channel[i], instanceId_board1);
                }

                int[] offset_every_channel = new int[4];
                offset_every_channel[0] = 1;
                offset_every_channel[1] = cardnum_every_channel[0] + 1;
                offset_every_channel[2] = cardnum_every_channel[0] + cardnum_every_channel[1] + 1;
                offset_every_channel[3] = cardnum_every_channel[0] + cardnum_every_channel[1] + cardnum_every_channel[2] + 1;

                List<string> card_group_list = new List<string>();

                for (int i = 0; i < 4; i++)
                {
                    int group_num = cardnum_every_channel[i] / max_card_num_per_G;
                    int left_cardnum = cardnum_every_channel[i] % max_card_num_per_G;

                    for (int j = 0; j < group_num; j++)
                    {
                        int card_no_begin = j * max_card_num_per_G + offset_every_channel[i];
                        string[] card_no_this_group = new string[max_card_num_per_G];
                        for (int k = 0; k < max_card_num_per_G; k++)
                        {
                            card_no_this_group[k] = (card_no_begin + k).ToString();
                        }
                        string group_s = "";

                        group_s = card_no_this_group[0] + "~" + card_no_this_group[card_no_this_group.Length - 1];
                        card_group_list.Add(group_s);
                    }

                    if (left_cardnum > 0)
                    {
                        string[] card_no_left = new string[left_cardnum];
                        int card_no_left_begin = group_num * max_card_num_per_G + offset_every_channel[i];
                        for (int k = 0; k < left_cardnum; k++)
                        {
                            card_no_left[k] = (card_no_left_begin + k).ToString();
                        }

                        string left_s = "";

                        if (card_no_left.Length > 1)
                        {
                            left_s = card_no_left[0] + "~" + card_no_left[card_no_left.Length - 1];
                        }
                        else
                        {
                            left_s = card_no_left[0];
                        }

                        card_group_list.Add(left_s);

                    }


                }

                return card_group_list.ToArray();
            }
            else
            {
                int total_cardnumber = getTotal_cardNumber();
                string[] cardnum_range = new string[total_cardnumber];
                for (int i = 1; i <= total_cardnumber; i++)
                {
                    cardnum_range[i - 1] = i.ToString();
                }

                return cardnum_range;
            }
        }

        private int getTotal_cardNumber()
        {
            int total_cardnumber = 0;



            for (int i = 1; i <= _channel_number; i++)
            {
                int cardnum_thischannel = 0;
                SdkInterface.tymscan_get_cardnumber(i, ref cardnum_thischannel, instanceId_board1);
                total_cardnumber += cardnum_thischannel;
            }

            return total_cardnumber;
        }

        private void btn_init_Click(object sender, EventArgs e)
        {
            SdkInterface.tymscan_init(tbx_hostip.Text, tbx_daqip.Text, Convert.ToInt32(tbx_cmdport.Text), Convert.ToInt32(tbx_imgport.Text), ref instanceId_board1);

            changeStatus();

            if (instanceId_board1 != DISCONNECTED)
            {
                btn_init.Enabled = false;

                updateStatus();

                SdkInterface.tymscan_set_datacallback(tymcallback, default(IntPtr), _blockheight, instanceId_board1);
                SdkInterface.tymscan_set_status_callback(tym_status_callback, default(IntPtr), 1000, instanceId_board1);

                set_controls_enable(true);

                //int length_tym_sns=0;
                //SdkInterface.tymscan_get_hardware_info_length((int)SdkInterface.HARDWARE_INFO_ID.fcminfo_id_tym_detector_module_sns,ref length_tym_sns, instanceId_board1);

                //StringBuilder sb_tym_sns = new StringBuilder(length_tym_sns + 1);

                //SdkInterface.tymscan_get_hardware_info((int)SdkInterface.HARDWARE_INFO_ID.fcminfo_id_tym_detector_module_sns, sb_tym_sns, length_tym_sns, instanceId_board1);

                //string tym_sns = sb_tym_sns.ToString();


                //SdkInterface.work_mode_param wk_mode_param=new SdkInterface.work_mode_param();
                //wk_mode_param.param_mode6.constant_integration_time_us = 400;
                //wk_mode_param.param_mode6.external_trigger_filtering_time_threshold_us = 300;
                //wk_mode_param.param_mode6.feedback_pulse_of_image_acquisition_quantity = 270;
                //wk_mode_param.param_mode6.feedback_pulse_width_ms = 450;
                //wk_mode_param.param_mode6.times_of_frequency_division = 3;
                //wk_mode_param.param_mode6.edge_trigger = 0x03;
                //SdkInterface.tymscan_set_work_mode(6, instanceId_board1, SdkInterface.StructToIntPtr(wk_mode_param));

                //SdkInterface.work_mode_param wk_mode_param_get = new SdkInterface.work_mode_param(); ;
                //int mode_get = -1;
                //IntPtr wk_mode_param_get_ptr = SdkInterface.StructToIntPtr(wk_mode_param_get);
                //SdkInterface.tymscan_get_work_mode(ref mode_get, instanceId_board1, wk_mode_param_get_ptr);
                //wk_mode_param_get = (SdkInterface.work_mode_param)Marshal.PtrToStructure(wk_mode_param_get_ptr, typeof(SdkInterface.work_mode_param));


                //调用示例
                /*
                SdkInterface.work_mode_param wk_mode_param=new SdkInterface.work_mode_param();
                wk_mode_param.param_mode5.constant_integration_time_us = 4000;
                wk_mode_param.param_mode5.edge_trigger=0x03;
                wk_mode_param.param_mode5.external_trigger_filtering_time_threshold_us = 4030;
                SdkInterface.tymscan_set_work_mode(5, instanceId_board1, SdkInterface.StructToIntPtr(wk_mode_param));*/




            }
            else
            {
                MessageBox.Show("Connection failed!");
                return;
            }

        }

        private void set_controls_enable(bool enable)
        {
            btn_setting.Enabled = enable;
            btn_preset.Enabled = enable;
            btn_startgrab.Enabled = enable;
            btn_stopgrab.Enabled = enable;
            btn_getgainlist.Enabled = enable;
        }


        private void changeStatus()
        {
            bool inited = false;
            SdkInterface.tymscan_inited(ref inited, instanceId_board1);

            bool connected = false;
            SdkInterface.tymscan_connected(ref connected, instanceId_board1);

            string initedS = (inited == true) ? "init success!" : "init fail!";
            string connectedS = (connected == true) ? "connect success!" : "connect fail!";

            tbx_status.Text = initedS + "   " + connectedS;
        }

        private void updateStatus()
        {

            SdkInterface.tymscan_get_systype(ref _systemtype, ref _bytes_per_pixel, ref _bits_range, instanceId_board1);

            if ((_systemtype == (int)SystemType.FCM_G) || (_systemtype == (int)SystemType._04X8) || (_systemtype == (int)SystemType.PANGUC))
            {
                _single_energy = true;
            }
            else
            {
                _single_energy = false;
            }

            update_presettingUI();
            update_settingArea();

        }

        private void btn_cleanup_Click(object sender, EventArgs e)
        {
            SdkInterface.tymscan_cleanup(instanceId_board1);
            btn_init.Enabled = true;
            changeStatus();
            set_controls_enable(false);
        }


        private void callback(IntPtr buffer, IntPtr extra_info, IntPtr user_data, int instance)
        {
            if (instance == instanceId_board1)
            {

                SdkInterface.tymdata_buffer strucut_buf = (SdkInterface.tymdata_buffer)Marshal.PtrToStructure(buffer, typeof(SdkInterface.tymdata_buffer));
                Marshal.Copy(strucut_buf.data, recv_data_store, 0, _imgwidth * _blockheight);

                if (pictureBox_Img.InvokeRequired)
                {
                    pictureBox_Img.Invoke(imgUpdate, recv_data_store);
                }

                if (chart1.InvokeRequired)
                {
                    chart1.Invoke(cS, recv_data_store);
                }



            }

        }


        private void status_callback(IntPtr user_data, int instance)
        {

            if (instance == instanceId_board1)
            {
                if (tbx_status_callback.InvokeRequired)
                {
                    tbx_status_callback.Invoke(SD);
                }
            }
        }


        //public static IntPtr StructToIntPtr<T>(T req) where T : struct
        //{
        //    int size = Marshal.SizeOf(req);
        //    byte[] bytes = new byte[size];
        //    IntPtr structPtr = Marshal.AllocHGlobal(size);
        //    Marshal.StructureToPtr(req, structPtr, false);
        //    Marshal.Copy(structPtr, bytes, 0, size);
        //    return structPtr;
        //}


        private void btn_preset_Click(object sender, EventArgs e)
        {

            update_presetting(false);

        }

        private void update_presetting(bool enable_header)
        {
            SdkInterface.tymscan_set_energy_mode(cbx_energymode.SelectedIndex + 1, instanceId_board1);//这个需要先设置
            for (int i = 1; i <= _channel_number; i++)
            {
                SdkInterface.tymscan_set_cardnumber(i, Convert.ToInt32(tbx_6channel[i - 1].Text), instanceId_board1);
            }

            SdkInterface.tymscan_set_pixelnum_percard(Convert.ToInt32(tbx_pixelsperdetector.Text), instanceId_board1);



            if (enable_header)
            {
                SdkInterface.info_header_setting header_setting;

                byte[] buf_item_order = new byte[2];
                buf_item_order[0] = (byte)SdkInterface.id_linenum;
                buf_item_order[1] = (byte)SdkInterface.id_null_byte;
                header_setting.buf_item_order = BytesToIntptr2(buf_item_order);
                header_setting.bufsize_item_order = 2;
                header_setting.null_byte_num = 4;
                IntPtr ptr_header_setting = SdkInterface.StructToIntPtr(header_setting);

                //

                SdkInterface.tymscan_set_datacallback(tymcallback, default(IntPtr), Convert.ToInt32(tbx_blockheight.Text), instanceId_board1, ptr_header_setting);
            }
            else
            {
                SdkInterface.tymscan_set_datacallback(tymcallback, default(IntPtr), Convert.ToInt32(tbx_blockheight.Text), instanceId_board1);

            }




            int pixelnumber = 0;
            SdkInterface.tymscan_get_pixelnum(ref pixelnumber, instanceId_board1);
            tbx_pixelnumber.Text = pixelnumber.ToString();


            string[] cardnum_range = get_cardnum_range();
            _card_num_range = cardnum_range;
            cbx_cardno_low.Items.Clear();
            cbx_cardno_low.Items.Add("All");
            cbx_cardno_low.Items.AddRange(cardnum_range);

            cbx_cardno_high.Items.Clear();
            cbx_cardno_high.Items.Add("All");
            cbx_cardno_high.Items.AddRange(cardnum_range);


            cbx_cardno_low.SelectedIndex = 0;
            cbx_cardno_high.SelectedIndex = 0;



            _blockheight = Convert.ToInt32(tbx_blockheight.Text);
            _imgwidth = pixelnumber;
            _imgheight = Convert.ToInt32(tbx_frameheight.Text);

            if (_single_energy)
            {
                _gainlow_everycard = new int[_card_num_range.Length];

                change_gain_status();
                if (is_all_gain_same(_gainlow_everycard))
                {
                    cbx_lowgain.SelectedIndex = _gainlow_everycard[0] - 1;
                }
                else
                {
                    cbx_lowgain.SelectedIndex = -1;
                }

            }
            else
            {
                _gainlow_everycard = new int[_card_num_range.Length];
                _gainhigh_everycard = new int[_card_num_range.Length];
                change_gain_status();
                if (is_all_gain_same(_gainlow_everycard))
                {
                    cbx_lowgain.SelectedIndex = _gainlow_everycard[0] - 1;
                }
                else
                {
                    cbx_lowgain.SelectedIndex = -1;
                }
                if (is_all_gain_same(_gainhigh_everycard))
                {
                    cbx_highgain.SelectedIndex = _gainhigh_everycard[0] - 1;
                }
                else
                {
                    cbx_highgain.SelectedIndex = -1;
                }

            }



            init_data_storage();
        }



        private void btn_setting_Click(object sender, EventArgs e)
        {
            if (_systemtype == (int)SystemType.PANGUC)
            {
                SdkInterface.tymscan_set_integral_time(Convert.ToInt32(tbx_triggertime.Text), instanceId_board1);
                SdkInterface.tymscan_set_integral_time(Convert.ToInt32(tbx_integrationTime), instanceId_board1, 1);
                SdkInterface.tymscan_set_integral_time(Convert.ToInt32(tbx_delaytime), instanceId_board1, 2);
            }
            else
            {
                SdkInterface.tymscan_set_integral_time(Convert.ToInt32(tbx_integrationTime.Text), instanceId_board1);
            }

            SdkInterface.tymscan_set_test_pattern(cbx_testpattern.SelectedIndex, instanceId_board1);
            SdkInterface.tymscan_set_trigger_mode(cbx_triggermode.SelectedIndex, instanceId_board1);

            if (_single_energy)
            {
                SdkInterface.tymscan_set_gain_low(cbx_cardno_low.SelectedIndex, cbx_lowgain.SelectedIndex + 1, instanceId_board1);
            }
            else
            {
                SdkInterface.tymscan_set_gain_low(cbx_cardno_low.SelectedIndex, cbx_lowgain.SelectedIndex + 1, instanceId_board1);
                SdkInterface.tymscan_set_gain_high(cbx_cardno_high.SelectedIndex, cbx_highgain.SelectedIndex + 1, instanceId_board1);
            }



        }

        private void btn_startgrab_Click(object sender, EventArgs e)
        {
            SdkInterface.tymscan_grab_start(instanceId_board1);
            btn_startgrab.Enabled = false;
            btn_preset.Enabled = false;
        }

        private void btn_stopgrab_Click(object sender, EventArgs e)
        {
            SdkInterface.tymscan_grab_stop(instanceId_board1);
            btn_startgrab.Enabled = true;
            btn_preset.Enabled = true;
        }



        private byte[] shortsTobytes(short[] arr_short)
        {

            int values_count = arr_short.Count();

            for (int i = 0; i < values_count; i++)
            {
                recv_data_store_grey[i] = (byte)((ushort)arr_short[i] >> 8);
            }

            return recv_data_store_grey;
        }


        private IntPtr BytesToIntptr(byte[] bytes)
        {
            int size = bytes.Length;
            //IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, recv_data_store_grey_ptr, size);
                return recv_data_store_grey_ptr;
            }
            finally
            {
                //Marshal.FreeHGlobal(buffer);

            }
        }

        private IntPtr BytesToIntptr2(byte[] bytes)
        {
            int size = bytes.Length;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return buffer;
            }
            finally
            {
                //Marshal.FreeHGlobal(buffer);

            }
        }



        private void moveup(byte[] bytes_to_insert)
        {

            Array.Copy(whole_image_store_grey, _imgwidth * _blockheight, whole_image_store_grey, 0, _imgwidth * (_imgheight - _blockheight));
            Array.Copy(bytes_to_insert, 0, whole_image_store_grey, _imgwidth * (_imgheight - _blockheight), bytes_to_insert.Length);

        }

        private void moveup(ushort[] ushorts_to_insert)
        {
            Array.Copy(frame_data_buffer, _imgwidth * _blockheight, frame_data_buffer, 0, _imgwidth * (_imgheight - _blockheight));
            Array.Copy(ushorts_to_insert, 0, frame_data_buffer, _imgwidth * (_imgheight - _blockheight), ushorts_to_insert.Length);
        }

        private void image_update(short[] data)
        {

            /*
            #region 写文本文件

            for (int i = 0; i < _blockheight;i++ )
            {
                ushort byte_10 = (ushort)data[i * _imgwidth + 0];


                ushort byte_32 = (ushort)data[i * _imgwidth + 1];

                UInt32 linenum = byte_32;

                linenum = (linenum << 16) | byte_10;


                _streamw.WriteLine(linenum.ToString());
                
            }
                
            
            #endregion
            */

            byte[] value_bytes = shortsTobytes(data);

            moveup(value_bytes);

            IntPtr valuePtr = BytesToIntptr(whole_image_store_grey);

            Bitmap bmap = new Bitmap(_imgwidth, _imgheight, _imgwidth, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, valuePtr);




            System.Drawing.Imaging.ColorPalette GreyColorPalette = bmap.Palette;

            for (int Index = 0; Index <= byte.MaxValue; Index++)
            {
                GreyColorPalette.Entries[Index] = Color.FromArgb(byte.MaxValue, Index, Index, Index);
            }
            bmap.Palette = GreyColorPalette;


            pictureBox_Img.Image = bmap;
        }


        private void chartShua(short[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                block_data_buffer[i] = (ushort)data[i];
            }

            moveup(block_data_buffer);


            double[] data_avg_line = new double[_imgwidth];


            for (int i = 0; i < _imgheight; i++)
            {

                for (int j = 0; j < _imgwidth; j++)
                {
                    data_avg_line[j] += frame_data_buffer[i * _imgwidth + j];
                }

            }

            for (int i = 0; i < _imgwidth; i++)
            {
                data_avg_line[i] = data_avg_line[i] / _imgheight;
            }

            chart1.Series[0].Points.DataBindY(data_avg_line);

        }

        private void btn_getgainlist_Click(object sender, EventArgs e)
        {
            change_gain_status();
            _gainlist.refresh_gainlist(_single_energy, _card_num_range, _gainlow_everycard, _gainhigh_everycard);
            _gainlist.ShowDialog();

        }

        private void btn_save_config_Click(object sender, EventArgs e)
        {
            SdkInterface.tymscan_save_config(instanceId_board1);
        }

        private void header_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (header_checkbox.Checked)
            {
                update_presetting(true);
            }
            else
            {
                update_presetting(false);

            }
        }



    }
}
