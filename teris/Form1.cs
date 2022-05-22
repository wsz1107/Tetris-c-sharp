using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace teris
{
    public partial class Form1 : Form
    {
        public List<Point> puzzlesPos=new List<Point>();
        public RectPuzzle rp;
        public int FallSpeed=25;

        public Form1()
        {
            InitializeComponent();
            puzzlesPos.Add(new Point(100, 100));
            for(int i = 0; i < puzzlesPos.Count; i++)
            {
                rp = new RectPuzzle(puzzlesPos[i]);
            }
            timer1.Enabled = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            e.Graphics.FillPolygon(blueBrush, rp.points);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Debug.WriteLine(1);
            for (int i=0; i < puzzlesPos.Count; i++)
            {
                var entry = puzzlesPos[i];
                entry.Y+=FallSpeed;
                puzzlesPos[i] = entry;
                Debug.WriteLine(puzzlesPos[i].Y);
            }
            
            pictureBox1.Invalidate();
        }
    }
    public class Puzzles
    {
        SolidBrush brush;
    }
    public class RectPuzzle:Puzzles
    {
        public Point[] points;
        int rectLength = 50;
        public RectPuzzle(Point _point)
        {
            this.points = new Point[]
            {
                new Point(_point.X, _point.Y),
                new Point(_point.X+rectLength, _point.Y),
                new Point(_point.X+rectLength,_point.Y+rectLength),
                new Point (_point.X,_point.Y+rectLength)
            };
        }
    }

}
