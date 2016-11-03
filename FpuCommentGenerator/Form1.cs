using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FPU
{
    public partial class Form1 : Form
    {
        CommentGenerator fpu = new CommentGenerator();
        // 256 space symbols
        string whitespace = "                                                                                                                                                                                                                                                                                ";
        string filename = "";

        public Form1()
        {
            InitializeComponent();
            GenerateComments();
        }

        private void UpdateTitle()
        {
            if (filename == "")
                this.Text = "FPU Comment Generator";
            else
                this.Text = "FPU Comment Generator - " + Path.GetFileName(filename);
        }

        private void GenerateComments()
        {
            fpu.Init();
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(rtf.Text);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string fpucomment = fpu.ParseLine(line.Trim());
                sb.AppendLine(fpucomment);
            }
            rtf2.Text = sb.ToString();
        }

        private void GenerateMergedComments()
        {
            fpu.Init();
            StringBuilder sb_merged = new StringBuilder();
            StringReader sr = new StringReader(rtf.Text);
            string line = "";
            int max_length = 0;
            while ((line = sr.ReadLine()) != null)
            {
                max_length = Math.Max(max_length, line.Length);
            }
            max_length++;

            sr = new StringReader(rtf.Text);
            while ((line = sr.ReadLine()) != null)
            {
                string fpucomment = fpu.ParseLine(line.Trim());
                sb_merged.Append(line);
                sb_merged.Append(";" + whitespace.Substring(0, max_length - line.Length));
                sb_merged.AppendLine(fpucomment);
            }
            Clipboard.SetText(sb_merged.ToString());
        }

        private void startToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            GenerateComments();
        }

        private void copyMergedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateMergedComments();
        }

        private void rtf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Shift)
            {
                GenerateComments();
                e.Handled = true;
            }
        }

        private void clearFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = rtf.Text;
            rtf.Clear();
            rtf.Text = s;
        }

        private void clearCommentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string line = "";
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(rtf.Text);
            while ((line = sr.ReadLine()) != null)
            {
                int pos = line.IndexOf(";");
                if (pos < 0)
                    sb.AppendLine(line);
                else
                    sb.AppendLine(line.Substring(0, pos).Trim());
            }
            rtf.Text = sb.ToString();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filename = "";
            UpdateTitle();
            rtf.Text = "";
            rtf2.Text = "";
            fpu.Init();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = Path.GetFileName(filename);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    rtf.Text = sr.ReadToEnd();
                }
                UpdateTitle();
                GenerateComments();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filename == "") saveAsToolStripMenuItem_Click(null, null);
            else
                using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
                {
                    sw.Write(rtf.Text);
                }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = Path.GetFileName(filename);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = saveFileDialog1.FileName;
                using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
                {
                    sw.Write(rtf.Text);
                }
                UpdateTitle();
            }
        }
    }
}
