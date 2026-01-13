using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TYMTest
{
    public partial class GainList : Form
    {
        public GainList()
        {
            InitializeComponent();
        }

        public void refresh_gainlist(bool single_energy, string[] card_no, int[] gainlow_every_card, int[] gainhigh_every_card)
        {
            gain_table.Rows.Clear();
            if (single_energy)
            {
                gain_table.Columns[2].Visible = false;
                gain_table.Columns[1].HeaderText = "Gain";
                for (int i = 0; i < card_no.Length; i++)
                {
                    gain_table.Rows.Add(card_no[i], gainlow_every_card[i], "");
                }

            }
            else
            {
                gain_table.Columns[2].Visible = true;
                gain_table.Columns[1].HeaderText = "Low Gain";
            }
        }
    }
}
