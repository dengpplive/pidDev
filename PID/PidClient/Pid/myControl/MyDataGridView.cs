using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace PBPid
{
    public class MyDataGridView : DataGridView
    {
        public MyDataGridView()
        {

            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            BackgroundColor = Color.White;

            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;
            RowHeadersVisible = false;
            DoubleBuffered = true;
            ShowRowErrors = false;
            ReadOnly = true;

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

        }
    }
}
