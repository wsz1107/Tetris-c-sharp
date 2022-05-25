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
        //public RectPuzzle rp;
        public Puzzles testPuzzles;
        public Puzzles otherPuzzles;
        public int FallSpeed = 20;
        public int LengthOfCell = 20;
        public int FallingPuzzleMode = 0;

        public int[,] Cells;//Save the color of cells
        public int[] TopOfCells;//Save the height of every column

        private List<string> keying = new List<string>();

        private int topOfPlayboard = 0;
        private int bottomOfPlayboard = 400;
        private int leftOfPlayboard = 0;
        private int rightOfPlayboard = 200;

        public Form1()
        {
            InitializeComponent();
            resetGame();
        }
        private void resetGame()
        {

            //Intialize color of cells
            Cells = new int[20, 10];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Cells[i, j] = 0;
                }
            }

            //Intialize top of columns
            TopOfCells = new int[10];
            for (int i = 0; i < 10; i++)
            {
                TopOfCells[i] = 400;
            }

            puzzlesPos = new Point(100, 200);
            timer1.Enabled = true;
            timer2.Enabled = true;

            //test
            for (int i = 2; i < 10; i++)
            {
                Cells[19, i] = 1;
                TopOfCells[i] = 380;
            }
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Draw main playboard
            int widthOfPlayboardFrame = 3;
            Pen playboardFramePen = new Pen(Color.Gray, widthOfPlayboardFrame);
            e.Graphics.DrawLine(playboardFramePen, rightOfPlayboard, topOfPlayboard, rightOfPlayboard, bottomOfPlayboard);

            //Draw fallen puzzules
            int widthOfFallenPuzzles = 3;
            Pen fallenPuzzlesPen = new Pen(Color.Red, widthOfFallenPuzzles);
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Cells[i, j] == 1)
                    {
                        e.Graphics.DrawRectangle(fallenPuzzlesPen, j * LengthOfCell, i * LengthOfCell, LengthOfCell, LengthOfCell);
                    }
                }
            }

            //Draw falling puzzle
            //rp = new RectPuzzle(puzzlesPos);

            //Pen bluePen = new Pen(Color.Blue, 2);
            //for (int i = 0; i < rp.points.Length; i++)
            //{
            //    e.Graphics.DrawRectangle(bluePen, rp.points[i].X, rp.points[i].Y, LengthOfCell, LengthOfCell);
            //}


            //Draw test puzzle
            //int testMode = 2;
            //testPuzzles = new TrianglePuzzle(puzzlesPos, testMode);
            //Pen blackPen = new Pen(Color.Black, 2);
            //for (int i = 0; i < testPuzzles.points.Length; i++)
            //{
            //    e.Graphics.DrawRectangle(blackPen, testPuzzles.points[i].X, testPuzzles.points[i].Y, LengthOfCell, LengthOfCell);
            //}

            //Draw other puzzle
            //FallingPuzzleMode = 0;
            otherPuzzles = new StickPuzzles(puzzlesPos, FallingPuzzleMode);
            Pen blackPen = new Pen(Color.Black, 2);
            for (int i = 0; i < otherPuzzles.points.Length; i++)
            {
                e.Graphics.DrawRectangle(blackPen, otherPuzzles.points[i].X, otherPuzzles.points[i].Y, LengthOfCell, LengthOfCell);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Puzzule falls
            bool flag = true;
            for (int i = 0; i < otherPuzzles.points.Length; i++)
            {
                if (otherPuzzles.points[i].Y + LengthOfCell >= TopOfCells[otherPuzzles.points[i].X / LengthOfCell])
                {
                    
                    addStack();
                    puzzlesPos = new Point(100, 100);
                    otherPuzzles = null;
                    otherPuzzles = new TrianglePuzzle(puzzlesPos,FallingPuzzleMode);
                    checkLine();
                    flag = false;                  
                }
            }
            if (flag)
            {
                puzzlesPos.Y += FallSpeed;
            }

            //Debug.WriteLine(puzzlesPos.Y);
            pictureBox1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Control falling puzzle
            //rp = new RectPuzzle(puzzlesPos);
            bool flag = true;
            if (keying.Contains("Left"))
            {
                for (int i = 0; i < otherPuzzles.points.Length; i++)
                {
                    if (otherPuzzles.points[i].X <= leftOfPlayboard || Cells[otherPuzzles.points[i].Y / LengthOfCell, otherPuzzles.points[i].X / LengthOfCell - 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    puzzlesPos.X -= LengthOfCell;
                    Debug.WriteLine(puzzlesPos.X);
                }
            }
            if (keying.Contains("Right"))
            {
                for (int i = 0; i < otherPuzzles.points.Length; i++)
                {
                    if (otherPuzzles.points[i].X + LengthOfCell >= rightOfPlayboard || Cells[otherPuzzles.points[i].Y / LengthOfCell, otherPuzzles.points[i].X / LengthOfCell + 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    puzzlesPos.X += LengthOfCell;
                    Debug.WriteLine(puzzlesPos.X);
                }
            }
            if (keying.Contains("Down"))
            {
                for(int i = 0; i < otherPuzzles.points.Length; i++)
                {
                    if(otherPuzzles.points[i].Y+LengthOfCell >= bottomOfPlayboard || Cells[otherPuzzles.points[i].Y / LengthOfCell+1, otherPuzzles.points[i].X/LengthOfCell] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    puzzlesPos.Y += LengthOfCell;
                }
            }
            if (keying.Contains("Up"))
            {
                for(int i=0;i< otherPuzzles.points.Length; i++)
                {
                    if((otherPuzzles.points[i].X <= leftOfPlayboard || Cells[otherPuzzles.points[i].Y / LengthOfCell, otherPuzzles.points[i].X / LengthOfCell - 1] != 0 || otherPuzzles.points[i].X + LengthOfCell >= rightOfPlayboard || Cells[otherPuzzles.points[i].Y / LengthOfCell, otherPuzzles.points[i].X / LengthOfCell + 1] != 0)
                        ||(otherPuzzles.points[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[otherPuzzles.points[i].Y / LengthOfCell + 1, otherPuzzles.points[i].X / LengthOfCell] != 0))
                    {
                        flag =false;
                        break;
                    }
                }
                if (flag)
                {
                    if (FallingPuzzleMode == otherPuzzles.modeMaxIndex)
                    {
                        FallingPuzzleMode = 0;
                    }
                    else
                    {
                        FallingPuzzleMode++;
                        Debug.WriteLine("Mode"+FallingPuzzleMode);
                    }
                }
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
        private void addStack()
        {
            for (int i = 0; i < otherPuzzles.points.Length; i++)
            {
                Cells[otherPuzzles.points[i].Y / LengthOfCell, otherPuzzles.points[i].X / LengthOfCell] = 1;
                TopOfCells[otherPuzzles.points[i].X / LengthOfCell] = Math.Min(TopOfCells[otherPuzzles.points[i].X / LengthOfCell], otherPuzzles.points[i].Y);
            }
        }
        private void checkLine()
        {
            for (int i = 19; i >= 0; i--)
            {
                int res = 1;
                for (int j = 0; j < 10; j++)
                {
                    res *= Cells[i, j];
                }
                if (res != 0)
                {
                    for (int k = i; k >0; k--)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            Cells[k, l] = 0;
                            Cells[k, l] = Cells[k - 1, l];
                        }
                    }
                }
            }
        }
    }
    public class Puzzles
    {
        public const int LengthOfCell = 20;
        public Point[] points;
        public Pen Pen;
        public int mode;
        public int modeMaxIndex;

    }
    public class RectPuzzle : Puzzles
    {
        Pen pen;
        public RectPuzzle(Point _point)
        {
            this.points = new Point[]
            {
                new Point(_point.X, _point.Y),
                new Point(_point.X+LengthOfCell, _point.Y),
                new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell),
                new Point(_point.X,_point.Y+LengthOfCell)
            };
            pen = new Pen(Color.Blue, 3);

        }
    }
    public class TrianglePuzzle : Puzzles
    {
        public TrianglePuzzle(Point _point, int _mode)
        {
            this.mode = _mode;
            modeMaxIndex = 3;
            switch (mode)
            {
                case 0:
                    this.points = new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y)
                    };
                    break;
                case 1:
                    this.points = new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                    break;
                case 2:
                    this.points = new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                    break;
                case 3:
                    this.points = new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                    break;
            }
        }
    }
    public class ZPuzzles:Puzzles
    {
        public ZPuzzles(Point _point, int _mode)
        {
            this.mode = _mode;
            modeMaxIndex = 1;
            switch (mode)
            {
                case 0:
                    this.points = new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                    break;
                case 1:
                    this.points = new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                    break;
            }
        }
    }
    public class XZPuzzles : Puzzles
    {
        public XZPuzzles(Point _point, int _mode)
        {
            this.mode = _mode;
            modeMaxIndex = 1;
            switch (mode)
            {
                case 0:
                    this.points = new Point[]
                    {
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                    break;
                case 1:
                    this.points = new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                    break;
            }
        }
    }
    public class LPuzzles : Puzzles
    {
        public LPuzzles(Point _point, int _mode)
        {
            this.mode = _mode;
            modeMaxIndex = 3;
            switch (mode)
            {
                case 0:
                    this.points = new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                    break;
                case 1:
                    this.points = new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell, _point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                    break;
                case 2:
                    this.points = new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                    break;
                case 3:
                    this.points = new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell,_point.Y)
                    };
                    break;
            }
        }

    }
    public class ZLPuzzles : Puzzles
    {
        public ZLPuzzles(Point _point, int _mode)
        {
            this.mode = _mode;
            modeMaxIndex = 3;
            switch (mode)
            {
                case 0:
                    this.points = new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                    break;
                case 1:
                    this.points = new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y)
                    };
                    break;
                case 2:
                    this.points = new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell,_point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell)
                    };
                    break;
                case 3:
                    this.points = new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell, _point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                    break;
            }
        }

    }
    public class StickPuzzles : Puzzles
    {
        public StickPuzzles(Point _point, int _mode)
        {
            this.mode = _mode;
            modeMaxIndex = 1;
            switch (mode)
            {
                case 0:
                    this.points = new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell*2),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                    break;
                case 1:
                    this.points = new Point[]
                    {
                        new Point(_point.X-LengthOfCell*2, _point.Y),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y)
                    };
                    break;
            }
        }
    }
}
