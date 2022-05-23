using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace teris
{
    public partial class Form1 : Form
    {
        public Point puzzlesPos;
        public RectPuzzle rp;
        public int FallSpeed = 20;
        public int LengthOfCell = 20;
        private List<string> keying = new List<string>();

        private int topOfPlayboard = 0;
        private int bottomOfPlayboard = 400;
        private int leftOfPlayboard = 0;
        private int rightOfPlayboard = 200;

        public Form1()
        {
            InitializeComponent();
            puzzlesPos = new Point(100, 100);

            //rp = RectPuzzle(puzzlesPos);
            timer1.Enabled = true;
            timer2.Enabled = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Main playboard
            int widthOfPen = 3;


            Pen grayPen = new Pen(Color.Gray, widthOfPen);
            e.Graphics.DrawLine(grayPen, rightOfPlayboard, topOfPlayboard, rightOfPlayboard, bottomOfPlayboard);

            rp = new RectPuzzle(puzzlesPos);
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            e.Graphics.FillPolygon(blueBrush, rp.points);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Draw falling puzzule
            var entry = puzzlesPos;
            entry.Y += FallSpeed;
            puzzlesPos = entry;
            //Debug.WriteLine(puzzlesPos.Y);
            pictureBox1.Invalidate();



        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Control falling puzzle
            if (keying.Contains("Left") && puzzlesPos.X > leftOfPlayboard)
            {
                puzzlesPos.X -= LengthOfCell;
                Debug.WriteLine(puzzlesPos.X);
            }
            if (keying.Contains("Right") && puzzlesPos.X + 2 * LengthOfCell < rightOfPlayboard)
            {
                puzzlesPos.X += LengthOfCell;
                Debug.WriteLine(puzzlesPos.X);
            }
            pictureBox1.Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

            keying.Remove(e.KeyCode.ToString());
            Debug.WriteLine("Remove{0}", e.KeyCode.ToString());
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!keying.Contains(e.KeyCode.ToString()))
            {
                keying.Add(e.KeyCode.ToString());
                Debug.WriteLine("Add{0}", e.KeyCode.ToString());
            }
        }
    }
    public class Puzzles
    {
        SolidBrush brush;
    }
    public class RectPuzzle : Puzzles
    {
        public Point[] points;
        int rectLength = 40;
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
