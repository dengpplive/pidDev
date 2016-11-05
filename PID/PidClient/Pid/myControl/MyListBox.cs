using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PBPid.myControl
{
    public class myListBox : ListBox
    {
        public myListBox()
        {
            ForeColor = Color.Lime;
            BackColor = Color.Black;
            Font = new Font("宋体", 9);
            DoubleBuffered = true;

            Control.CheckForIllegalCrossThreadCalls = false;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
    }
}
