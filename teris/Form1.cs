using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace teris
{
    public partial class Form1 : Form
    {
        public Point PuzzlePos;
        public Point StartPos = new Point(100, 100);
        //public RectPuzzle rp;
        //public Puzzles testPuzzles;
        //public Puzzles otherPuzzles;
        public Puzzles CurrentPuzzle;
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

        private Random random = new Random();

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

            PuzzlePos = StartPos;
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
            if (CurrentPuzzle != null)
            {
                //otherPuzzles = new StickPuzzles(PuzzlePos, FallingPuzzleMode);
                Pen blackPen = new Pen(Color.Black, 2);
                for (int i = 0; i < CurrentPuzzle.points.Length; i++)
                {
                    e.Graphics.DrawRectangle(blackPen, CurrentPuzzle.points[i].X, CurrentPuzzle.points[i].Y, LengthOfCell, LengthOfCell);
                }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Create a puzzle if not exist
            if (CurrentPuzzle == null)
            {
                CurrentPuzzle = CreateFallingPuzzle(PuzzlePos, random.Next(7));
            }

            //Puzzle falls
            bool flag = true;
            var entry = CurrentPuzzle;
            for (int i = 0; i < CurrentPuzzle.points.Length; i++)
            {
                if (CurrentPuzzle.points[i].Y + LengthOfCell >= TopOfCells[CurrentPuzzle.points[i].X / LengthOfCell])
                {
                    addStack();
                    PuzzlePos = StartPos;
                    CurrentPuzzle = null;
                    checkLine();
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                PuzzlePos.Y += FallSpeed;
                CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.mode);
                //Debug.WriteLine(PuzzlePos.Y);
            }
            pictureBox1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Control falling puzzle
            bool flag = true;
            if (keying.Contains("Left") && CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.points.Length; i++)
                {
                    if (CurrentPuzzle.points[i].X <= leftOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell - 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    PuzzlePos.X -= LengthOfCell;
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.mode);
                    //Debug.WriteLine(PuzzlePos.X);
                }
            }
            if (keying.Contains("Right") && CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.points.Length; i++)
                {
                    if (CurrentPuzzle.points[i].X + LengthOfCell >= rightOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell + 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    PuzzlePos.X += LengthOfCell;
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.mode);
                    Debug.WriteLine(PuzzlePos.X);
                }
            }
            if (keying.Contains("Down") && CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.points.Length; i++)
                {
                    if (CurrentPuzzle.points[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell + 1, CurrentPuzzle.points[i].X / LengthOfCell] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    PuzzlePos.Y += LengthOfCell;
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.mode);
                }
            }
            if (keying.Contains("Up") && CurrentPuzzle.mode != -1)
            {
                for (int i = 0; i < CurrentPuzzle.points.Length; i++)
                {
                    if ((CurrentPuzzle.points[i].X <= leftOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell - 1] != 0 || CurrentPuzzle.points[i].X + LengthOfCell >= rightOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell + 1] != 0)
                        || (CurrentPuzzle.points[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell + 1, CurrentPuzzle.points[i].X / LengthOfCell] != 0))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    if (CurrentPuzzle.mode == CurrentPuzzle.modeMaxIndex)
                    {
                        CurrentPuzzle.mode = 0;
                    }
                    else
                    {
                        CurrentPuzzle.mode++;
                        Debug.WriteLine("Mode" + CurrentPuzzle.mode);
                    }
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.mode);
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
            for (int i = 0; i < CurrentPuzzle.points.Length; i++)
            {
                Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell] = 1;
                TopOfCells[CurrentPuzzle.points[i].X / LengthOfCell] = Math.Min(TopOfCells[CurrentPuzzle.points[i].X / LengthOfCell], CurrentPuzzle.points[i].Y);
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
                    for (int k = i; k > 0; k--)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            Cells[k, l] = 0;
                            Cells[k, l] = Cells[k - 1, l];
                        }
                    }
                    for (int k = 0; k < 10; k++)
                    {
                        TopOfCells[k] += LengthOfCell;
                    }
                }
            }
        }
        private Puzzles CreateFallingPuzzle(Point point, int ind)
        {
            switch (ind)
            {
                case 0:
                    RectPuzzle rectP = new RectPuzzle();
                    rectP.SetPoints(point, -1);
                    return rectP;
                case 1:
                    TrianglePuzzle triangleP = new TrianglePuzzle();
                    triangleP.SetPoints(point, 0);
                    return triangleP;
                case 2:
                    ZPuzzles zP = new ZPuzzles();
                    zP.SetPoints(point, 0);
                    return zP;
                case 3:
                    XZPuzzles xzP = new XZPuzzles();
                    xzP.SetPoints(point, 0);
                    return xzP;
                case 4:
                    LPuzzles lP = new LPuzzles();
                    lP.SetPoints(point, 0);
                    return lP;
                case 5:
                    XLPuzzles xlP = new XLPuzzles();
                    xlP.SetPoints(point, 0);
                    return xlP;
                case 6:
                    StickPuzzles stickP = new StickPuzzles();
                    stickP.SetPoints(point, 0);
                    return stickP;
                default:
                    return null;
            }
        }
    }
    public abstract class Puzzles
    {
        public const int LengthOfCell = 20;
        public Point[] points;
        public Pen Pen;
        public int mode;
        public int modeMaxIndex;
        public abstract void SetPoints(Point _point, int _mode);

    }
    public class RectPuzzle : Puzzles
    {
        public override void SetPoints(Point _point, int _mode)
        {
            this.points = new Point[]
            {
                new Point(_point.X, _point.Y),
                new Point(_point.X+LengthOfCell, _point.Y),
                new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell),
                new Point(_point.X,_point.Y+LengthOfCell)
            };
            this.mode = _mode;
        }
    }
    public class TrianglePuzzle : Puzzles
    {
        
        public override void SetPoints(Point _point, int _mode)
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
    public class ZPuzzles : Puzzles
    {
        public override void SetPoints(Point _point, int _mode)
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
        public override void SetPoints(Point _point, int _mode)
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
        public override void SetPoints(Point _point, int _mode)
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
    public class XLPuzzles : Puzzles
    {
        public override void SetPoints(Point _point, int _mode)
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
        public override void SetPoints(Point _point, int _mode)
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
