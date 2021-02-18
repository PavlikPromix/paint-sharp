using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace paint_sharp
{
    public partial class Form1 : Form
    {
        Graphics g;
        Brush b1;
        Pen p1;
        public int shape = 1;
        public bool filled = false;
        List<bool> filleddata = new List<bool> { };
        public int x;
        public int y;
        Point[] pts;
        List<Point[]> fpts = new List<Point[]> { };
        string[] data;
        bool painting = true;

        public Form1()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
            b1 = new SolidBrush(Color.Black);
            p1 = new Pen(Color.Black);
            openFileDialog1.Filter = "Paint-sharp files(*.pts)|*.pts";
            saveFileDialog1.Filter = "Paint-sharp files(*.pts)|*.pts";
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //switch (shape)
            //{
            //    case 1:
            //        g.Clear(Color.White);
            //        g.FillRectangle(b1, new Rectangle(new Point(x, y), new Size(e.X - x, e.Y - y)));
            //        break;
            //    case 2:
            //        g.Clear(Color.White);
            //        Point[] pts = new Point[] { new Point(x, y), new Point((e.X + x) / 2, e.Y), new Point(e.X, y) };
            //        g.FillPolygon(b1, pts);
            //        break;
            //    default:
            //        break;
            //}
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (painting)
            {

                switch (shape)
                {
                    case 1:
                        pts = new Point[] { new Point(x, y), new Point(x, e.Y), new Point(e.X, e.Y), new Point(e.X, y) };
                        if (filled)
                        {
                            g.FillPolygon(b1, pts);
                            fpts.Add(pts);
                            filleddata.Add(true);

                        }
                        else
                        {
                            g.DrawPolygon(p1, pts);
                            fpts.Add(pts);
                            filleddata.Add(false);
                        }
                        break;
                    case 2:
                        pts = new Point[] { new Point(x, y), new Point((e.X + x) / 2, e.Y), new Point(e.X, y) };
                        if (filled)
                        {
                            g.FillPolygon(b1, pts);
                            fpts.Add(pts);
                            filleddata.Add(true);
                        }
                        else
                        {
                            g.DrawPolygon(p1, pts);
                            fpts.Add(pts);
                            filleddata.Add(false);
                        }
                        break;
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            fpts = new List<Point[]> { };
            filleddata = new List<bool> { };
        }

        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filled = true;
        }

        private void outlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filled = false;
        }

        private void squareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 1;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 2;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            File.Delete(saveFileDialog1.FileName);
            TextWriter tw = new StreamWriter(saveFileDialog1.FileName);
            foreach (var item in filleddata)
            {
                tw.Write($"{item} ");
            }
            tw.Close();
            File.AppendAllText(saveFileDialog1.FileName, "\n");
            foreach (var item in fpts)
            {
                foreach (var item1 in item)
                {
                    File.AppendAllText(saveFileDialog1.FileName, $"{item1.X} {item1.Y}\n");
                }
                File.AppendAllText(saveFileDialog1.FileName, "\n");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            g.Clear(Color.White);
            fpts = new List<Point[]> { };
            filleddata = new List<bool> { };
            data = File.ReadAllLines(openFileDialog1.FileName);
            List<Point> heap = new List<Point> { };
            foreach (var item in data)
            {
                if (item.Contains("True") || item.Contains("False"))
                {
                    foreach (var item1 in item.Split(' '))
                    {
                        if (item1 == "True")
                        {
                            filleddata.Add(true);
                        }
                        else
                        {
                            filleddata.Add(false);
                        }
                    }
                    continue;
                }
                if (item == "")
                {
                    fpts.Add(heap.ToArray());
                    heap.Clear();
                    continue;
                }
                heap.Add(new Point(int.Parse(item.Split(' ')[0]), int.Parse(item.Split(' ')[1])));
            }
            foreach (var item in fpts)
            {
                if (filleddata[fpts.IndexOf(item)] == true)
                {
                    g.FillPolygon(b1, item);
                }
                else
                {
                    g.DrawPolygon(p1, item);
                }
            }
        }
    }
}
