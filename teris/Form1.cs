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
        public Point StartPos = new Point(100, 40);
        public int FallingPuzzleMode = 0;
        public Puzzles CurrentPuzzle;

        public const int FallSpeed = 20;
        public const int LengthOfCell = 20;
        public const int PuzzleFrameWidth = 3;

        public int[,] Cells;//Save the colorIndex of fallen puzzles
        public const int CellCountX = 10;
        public const int CellCountY = 20;

        private List<string> keying = new List<string>();

        private const int topOfPlayboard = 0;
        private const int bottomOfPlayboard = LengthOfCell * CellCountY;
        private const int leftOfPlayboard = 0;
        private const int rightOfPlayboard = LengthOfCell * CellCountX;

        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            ResetGame();
        }
        private void ResetGame()
        {
            if (CurrentPuzzle != null)
            {
                CurrentPuzzle = null;
            }
            //Intialize color of cells
            Cells = new int[CellCountY, CellCountX];
            for (int i = 0; i < CellCountY; i++)
            {
                for (int j = 0; j < CellCountX; j++)
                {
                    Cells[i, j] = 0;
                }
            }

            PuzzlePos = StartPos;
            timer1.Enabled = true;
            timer2.Enabled = true;
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Draw main playboard
            int widthOfPlayboardFrame = 3;
            Pen playboardFramePen = new Pen(Color.Gray, widthOfPlayboardFrame);
            e.Graphics.DrawLine(playboardFramePen, rightOfPlayboard, topOfPlayboard, rightOfPlayboard, bottomOfPlayboard);

            //Draw fallen puzzules
            int widthOfFallenPuzzles = 3;
            for (int i = 0; i < CellCountY; i++)
            {
                for (int j = 0; j < CellCountX; j++)
                {
                    if (Cells[i, j] != 0)
                    {
                        e.Graphics.FillRectangle(PuzzleColor(Cells[i, j]), j * LengthOfCell, i * LengthOfCell, LengthOfCell, LengthOfCell);
                    }
                }
            }

            //Draw falling puzzle
            if (CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.points.Length; i++)
                {
                    e.Graphics.DrawRectangle(new Pen(PuzzleColor(CurrentPuzzle.colorIndex), widthOfFallenPuzzles), CurrentPuzzle.points[i].X, CurrentPuzzle.points[i].Y, LengthOfCell, LengthOfCell);
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
            for (int i = 0; i < CurrentPuzzle.points.Length; i++)
            {
                if (CurrentPuzzle.points[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell + 1, CurrentPuzzle.points[i].X / LengthOfCell] != 0)
                {
                    AddStack();
                    PuzzlePos = StartPos;
                    CurrentPuzzle = null;
                    CheckLine();
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                PuzzlePos.Y += FallSpeed;
                CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.mode);
            }
            pictureBox1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Control the falling puzzle
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
                        AddStack();
                        PuzzlePos = StartPos;
                        CurrentPuzzle = null;
                        CheckLine();
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
                    if (CurrentPuzzle.mode == 0)
                    {
                        CurrentPuzzle.mode = CurrentPuzzle.modeMaxIndex;
                    }
                    else
                    {
                        CurrentPuzzle.mode--;
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
        private void AddStack()
        {
            for (int i = 0; i < CurrentPuzzle.points.Length; i++)
            {
                Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell] = CurrentPuzzle.colorIndex;
            }
        }
        private void CheckLine()
        {
            int i = CellCountY - 1;
            while (i >= 0)
            {
                int res = 1;
                for (int j = 0; j < CellCountX; j++)
                {
                    res *= Cells[i, j];
                }
                if (res != 0)
                {
                    for (int k = i; k > 0; k--)
                    {
                        for (int l = 0; l < CellCountX; l++)
                        {
                            Cells[k, l] = 0;
                            Cells[k, l] = Cells[k - 1, l];
                        }
                    }
                }
                else
                {
                    i--;
                }
            }
            TestPrintCellsVal();
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
        public SolidBrush PuzzleColor(int ind)
        {
            switch (ind)
            {
                case 1:
                    return new SolidBrush(Color.Yellow);
                case 2:
                    return new SolidBrush(Color.Violet);
                case 3:
                    return new SolidBrush(Color.Red);
                case 4:
                    return new SolidBrush(Color.Green);
                case 5:
                    return new SolidBrush(Color.Orange);
                case 6:
                    return new SolidBrush(Color.Blue);
                case 7:
                    return new SolidBrush(Color.SkyBlue);
                default:
                    return new SolidBrush(Color.Black);
            }
        }

        private void TestPrintCellsVal()
        {
            for (int i = 19; i >= 0; i--)
            {
                for (int j = 0; j < 10; j++)
                {
                    Debug.Write(Cells[i, j]);
                }
                Debug.Write(Environment.NewLine);
            }
        }
    }
    public abstract class Puzzles
    {
        public const int LengthOfCell = 20;
        public Point[] points;
        public int mode;
        public int modeMaxIndex;
        public int colorIndex;
        public abstract void SetPoints(Point _point, int _mode);

    }
    public class RectPuzzle : Puzzles
    {
        public override void SetPoints(Point _point, int _mode)
        {
            this.mode = _mode;
            colorIndex = 1;
            this.points = new Point[]
            {
                new Point(_point.X, _point.Y),
                new Point(_point.X+LengthOfCell, _point.Y),
                new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell),
                new Point(_point.X,_point.Y+LengthOfCell)
            };

        }
    }
    public class TrianglePuzzle : Puzzles
    {
        public override void SetPoints(Point _point, int _mode)
        {
            this.mode = _mode;
            modeMaxIndex = 3;
            colorIndex = 2;
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
            colorIndex = 3;
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
            colorIndex = 4;
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
            colorIndex = 5;
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
            colorIndex = 6;
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
                        new Point(_point.X-LengthOfCell, _point.Y),
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
            colorIndex = 7;
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
