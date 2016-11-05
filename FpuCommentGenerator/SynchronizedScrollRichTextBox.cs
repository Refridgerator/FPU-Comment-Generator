﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FPU
{
    public class SynchronizedScrollRichTextBox : System.Windows.Forms.RichTextBox
    {
        public event vScrollEventHandler vScroll;
        public delegate void vScrollEventHandler(System.Windows.Forms.Message message);

        public const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
            if (msg.Msg == WM_VSCROLL)
            {
                if (vScroll != null)
                {
                    vScroll(msg);
                }
            }
            base.WndProc(ref msg);
        }

        public void PubWndProc(ref System.Windows.Forms.Message msg)
        {
            base.WndProc(ref msg);
        }
    }
}
