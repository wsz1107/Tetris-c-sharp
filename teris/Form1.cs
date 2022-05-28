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
        private int score = 0;
        private const int lineVal = 100;
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

            button1.Visible = false;
            button1.Enabled = false;
            label2.Visible = false;

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
            score = 0;
            timer1.Enabled = true;
            timer2.Enabled = true;
            UpdateScore();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Draw main playboard
            int widthOfPlayboardFrame = 3;
            Pen playboardFramePen = new Pen(Color.Gray, widthOfPlayboardFrame);
            e.Graphics.DrawLine(playboardFramePen, rightOfPlayboard, topOfPlayboard, rightOfPlayboard, bottomOfPlayboard);

            //Draw fallen puzzules
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
                    e.Graphics.DrawRectangle(new Pen(PuzzleColor(CurrentPuzzle.colorIndex), PuzzleFrameWidth), CurrentPuzzle.points[i].X, CurrentPuzzle.points[i].Y, LengthOfCell, LengthOfCell);
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
            if (IsGameOver())
            {
                GameOver();
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
                    CheckRow();
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                Debug.WriteLine(CurrentPuzzle.mode);
                PuzzlePos.Y += FallSpeed;
                CurrentPuzzle.UpdatePoints(CurrentPuzzle.GetPoints(PuzzlePos, CurrentPuzzle.mode));
            }
            pictureBox1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Control the falling puzzle
            if (keying.Contains("Left") && CurrentPuzzle != null)
            {
                bool flag = true;
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
                    CurrentPuzzle.UpdatePoints(CurrentPuzzle.GetPoints(PuzzlePos, CurrentPuzzle.mode));
                }
            }
            if (keying.Contains("Right") && CurrentPuzzle != null)
            {
                bool flag = true;
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
                    CurrentPuzzle.UpdatePoints(CurrentPuzzle.GetPoints(PuzzlePos, CurrentPuzzle.mode));
                }
            }
            if (keying.Contains("Down") && CurrentPuzzle != null)
            {
                bool flag = true;
                for (int i = 0; i < CurrentPuzzle.points.Length; i++)
                {
                    if (CurrentPuzzle.points[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentPuzzle.points[i].Y / LengthOfCell + 1, CurrentPuzzle.points[i].X / LengthOfCell] != 0)
                    {
                        AddStack();
                        PuzzlePos = StartPos;
                        CurrentPuzzle = null;
                        CheckRow();
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    PuzzlePos.Y += LengthOfCell;
                    CurrentPuzzle.UpdatePoints(CurrentPuzzle.GetPoints(PuzzlePos, CurrentPuzzle.mode));
                }
            }
            if (keying.Contains("Up") && CurrentPuzzle != null && CurrentPuzzle.mode != -1)
            {
                int nextMode;
                Point[] nextModePoints;
                bool flag = true;
                if (CurrentPuzzle.mode == 0)
                {
                    nextMode = CurrentPuzzle.modeMaxIndex;
                }
                else
                {
                    nextMode = CurrentPuzzle.mode - 1;
                }
                nextModePoints = CurrentPuzzle.GetPoints(PuzzlePos, nextMode);
                for (int i = 0; i < nextModePoints.Length; i++)
                {
                    Debug.WriteLine(nextModePoints[i]);
                    if (nextModePoints[i].X < leftOfPlayboard
                        || (nextModePoints[i].X > 0 && Cells[nextModePoints[i].Y / LengthOfCell, nextModePoints[i].X / LengthOfCell - 1] != 0))
                    {
                        flag = false;
                        break;
                    }
                    if (nextModePoints[i].X + LengthOfCell > rightOfPlayboard
                        || (nextModePoints[i].X / LengthOfCell + 1 < CellCountX && Cells[nextModePoints[i].Y / LengthOfCell, nextModePoints[i].X / LengthOfCell + 1] != 0))
                    {
                        flag = false;
                        break;
                    }
                    if (nextModePoints[i].Y + LengthOfCell > bottomOfPlayboard
                        || (nextModePoints[i].Y / LengthOfCell + 1 < CellCountY && Cells[nextModePoints[i].Y / LengthOfCell + 1, nextModePoints[i].X / LengthOfCell] != 0))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    CurrentPuzzle.UpdatePoints(nextModePoints);
                    CurrentPuzzle.UpdateMode(nextMode);
                }
            }
            pictureBox1.Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

            keying.Remove(e.KeyCode.ToString());
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!keying.Contains(e.KeyCode.ToString()))
            {
                keying.Add(e.KeyCode.ToString());
            }
        }
        private void AddStack()
        {
            for (int i = 0; i < CurrentPuzzle.points.Length; i++)
            {
                Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell] = CurrentPuzzle.colorIndex;
            }
        }
        private void CheckRow()
        {
            int combo = 0;
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
                    combo++;
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
            if (combo != 0)
            {
                score += (combo ^ combo) * lineVal;
                UpdateScore();
            }
        }
        private Puzzles CreateFallingPuzzle(Point point, int ind)
        {
            switch (ind)
            {
                case 0:
                    return new RectPuzzle(point,-1);
                case 1:
                    TrianglePuzzle triangleP = new TrianglePuzzle();
                    triangleP.UpdatePoints(triangleP.GetPoints(point, 0));
                    triangleP.UpdateMode(0);
                    return triangleP;
                case 2:
                    ZPuzzles zP = new ZPuzzles();
                    zP.UpdatePoints(zP.GetPoints(point, 0));
                    zP.UpdateMode(0);
                    return zP;
                case 3:
                    XZPuzzles xzP = new XZPuzzles();
                    xzP.UpdatePoints(xzP.GetPoints(point, 0));
                    xzP.UpdateMode(0);
                    return xzP;
                case 4:
                    LPuzzles lP = new LPuzzles();
                    lP.UpdatePoints(lP.GetPoints(point, 0));
                    lP.UpdateMode(0);
                    return lP;
                case 5:
                    XLPuzzles xlP = new XLPuzzles();
                    xlP.UpdatePoints(xlP.GetPoints(point, 0));
                    xlP.UpdateMode(0);
                    return xlP;
                case 6:
                    StickPuzzles stickP = new StickPuzzles();
                    stickP.UpdatePoints(stickP.GetPoints(point, 0));
                    stickP.UpdateMode(0);
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

        private void button1_Click(object sender, EventArgs e)
        {
            ResetGame();
        }
        private void UpdateScore()
        {
            label1.Text = "Score:\n" + score;
        }
        private bool IsGameOver()
        {
            for (int i = 0; i < CurrentPuzzle.points.Length; i++)
            {
                if (Cells[CurrentPuzzle.points[i].Y / LengthOfCell, CurrentPuzzle.points[i].X / LengthOfCell] != 0)
                {
                    return true;
                }
            }
            return false;
        } private void GameOver()
        {
            label2.Visible = true;
            timer1.Enabled = false;
            timer2.Enabled = false;
            button1.Visible = true;
            button1.Enabled = true;
        }
    }
    public abstract class Puzzles
    {
        public const int LengthOfCell = 20;
        public Point[] points;
        public int mode;
        public abstract int modeMaxIndex { get; }
        public abstract int colorIndex { get; }
        public abstract Point[] GetPoints(Point _point, int _mode);
        public void UpdatePoints(Point[] points)
        {
            this.points = points;
        }
        public void UpdateMode(int mode)
        {
            this.mode = mode;
        }
    }
    public class RectPuzzle : Puzzles
    {
        public override int colorIndex { get { return 1; } }
        public override int modeMaxIndex { get { return 0; } }
        public override Point[] GetPoints(Point _point, int _mode)
        {
            return new Point[]
            {
                new Point(_point.X, _point.Y),
                new Point(_point.X+LengthOfCell, _point.Y),
                new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell),
                new Point(_point.X,_point.Y+LengthOfCell)
            };
        }
        public RectPuzzle(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }
    }
    public class TrianglePuzzle : Puzzles
    {
        public override int colorIndex { get { return 2; } }
        public override int modeMaxIndex { get { return 3; } }

        public override Point[] GetPoints(Point _point, int _mode)
        {
            switch (_mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                case 2:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                case 3:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y)
                    };
            }
        }
    }
    public class ZPuzzles : Puzzles
    {
        public override int colorIndex { get { return 3; } }
        public override int modeMaxIndex { get { return 1; } }

        public override Point[] GetPoints(Point _point, int _mode)
        {
            switch (_mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
            }
        }
    }
    public class XZPuzzles : Puzzles
    {
        public override int colorIndex {  get { return 4; } }
        public override int modeMaxIndex { get { return 1; } }

        public override Point[] GetPoints(Point _point, int _mode)
        {
            switch (_mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y),
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
            }
        }
    }
    public class LPuzzles : Puzzles
    {
        public override int colorIndex { get { return 5; } }
        public override int modeMaxIndex { get { return 3; } }

        public override Point[] GetPoints(Point _point, int _mode)
        {
            switch (_mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell, _point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                case 2:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                case 3:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell,_point.Y)
                    };
                default:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
            }
        }
    }
    public class XLPuzzles : Puzzles
    {
        public override int colorIndex { get { return 6; } }
        public override int modeMaxIndex { get { return 3; } }

        public override Point[] GetPoints(Point _point, int _mode)
        {
            switch (_mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y)
                    };
                case 2:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X-LengthOfCell,_point.Y),
                        new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell)
                    };
                case 3:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X-LengthOfCell, _point.Y+LengthOfCell),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
                        new Point(_point.X+LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell),
                        new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
                    };
            }
        }

    }
    public class StickPuzzles : Puzzles
    {
        public override int colorIndex { get { return 7; } }
        public override int modeMaxIndex { get { return 1; } }
        public override Point[] GetPoints(Point _point, int _mode)
        {
            switch (_mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell*2),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(_point.X-LengthOfCell*2, _point.Y),
                        new Point(_point.X-LengthOfCell, _point.Y),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X+LengthOfCell,_point.Y)
                    };
                default:
                    return new Point[]
                    {
                        new Point(_point.X, _point.Y-LengthOfCell*2),
                        new Point(_point.X, _point.Y-LengthOfCell),
                        new Point(_point.X,_point.Y),
                        new Point(_point.X,_point.Y+LengthOfCell)
                    };
            }
        }
    }
}
