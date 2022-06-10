using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using tetris;

namespace teris
{
    public partial class Form1 : Form
    {
        TetrisModel tm = new TetrisModel();

        private const int GridSize = 20;

        private readonly int topOfPlayboard = 0;
        private readonly int bottomOfPlayboard = CoordinateMapping(TetrisModel.GridCountY);
        private readonly int leftOfPlayboard = 0;
        private readonly int rightOfPlayboard = CoordinateMapping(TetrisModel.GridCountX);

        private readonly Point StartPos=new Point(CoordinateMapping(TetrisModel.GridCountY), CoordinateMapping(TetrisModel.GridCountY));
        private readonly Point NextTetrisPos = new Point(230, 150);

        private List<string> keying = new List<string>();

        



        ///old code
        private Point CurrentBlockPos;
        private Block CurrentBlock;
        private Block NextBlock;

        private const int FallSpeed = 20;
        private const int LengthOfCell = 20;
        private const int BlockFrameWidth = 3;

        private int[,] Cells;//Save the colorIndex of fallen puzzles
        private const int CellCountX = 10;
        private const int CellCountY = 20;

        private Random random = new Random();
        private int typeOfNextBlock = 0;
        private int score = 0;
        private const int scorePerRow = 100;
        public Form1()
        {
            InitializeComponent();
            ResetGame();
        }
        private void ResetGame()
        {
            //Clear the screen
            if (tm.CurrentTetrimino != null)
            {
                tm.CurrentTetrimino = null;
            }
            button1.Visible = false;
            button1.Enabled = false;
            label2.Visible = false;

            //Intialize color of cells
            tm.IntializeGridMap(tm.FallingGridMap);
            tm.IntializeGridMap(tm.FallenGridMap);

            tm.Score = 0;
            UpdateScore(tm.Score);

            timer1.Enabled = true;
            timer2.Enabled = true;
            

            //Cells = new int[CellCountY, CellCountX];
            //for (int i = 0; i < CellCountY; i++)
            //{
            //    for (int j = 0; j < CellCountX; j++)
            //    {
            //        Cells[i, j] = 0;
            //    }
            //}

            //Start new game
            //CurrentBlockPos = StartPos;
            //score = 0;
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Draw main playboard
            int widthOfPlayboardFrame = 3;
            Pen playboardFramePen = new Pen(Color.Gray, widthOfPlayboardFrame);
            e.Graphics.DrawLine(playboardFramePen, rightOfPlayboard, topOfPlayboard, rightOfPlayboard, bottomOfPlayboard);

            //Draw next block
            if (tm.NextTetrimino != null)
            {
                for (int i = 0; i < tm.NextTetrimino.points.Length; i++)
                {
                    e.Graphics.DrawRectangle(new Pen(BlockColor(tm.NextTetrimino.ColorIndex), BlockFrameWidth), NextTetrisPos.X, NextTetrisPos.Y, GridSize, GridSize);
                }
            }

            //Draw stack and falling block
            for (int i = 0; i < TetrisModel.GridCountY; i++)
            {
                for (int j = 0; j < TetrisModel.GridCountX; j++)
                {
                    if (tm.FallenGridMap[i, j] != 0)
                    {
                        e.Graphics.FillRectangle(BlockColor(tm.FallenGridMap[i, j]), CoordinateMapping(i), CoordinateMapping(j), GridSize, GridSize);
                    }
                    if(tm.FallingGridMap[i,j] != 0)
                    {
                        e.Graphics.DrawRectangle(new Pen(BlockColor(tm.FallenGridMap[i, j])), CoordinateMapping(i), CoordinateMapping(j), GridSize, GridSize);
                    }
                }
            }

            //old Draw falling block code
            //if (CurrentBlock != null)
            //{
            //    for (int i = 0; i < CurrentBlock.points.Length; i++)
            //    {
            //        e.Graphics.DrawRectangle(new Pen(BlockColor(CurrentBlock.ColorIndex), BlockFrameWidth), CurrentBlock.points[i].X, CurrentBlock.points[i].Y, LengthOfCell, LengthOfCell);
            //    }
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tm.ReadyToFall();

            if (tm.IsGameOver(tm.CurrentTetrimino.points))
            {
                GameOver();
            }
            tm.MoveCurrentTetrimino(Directions.Down);
            pictureBox1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Control the falling block
            if (keying.Contains("Left") && CurrentBlock != null)
            {
                bool flag = true;
                for (int i = 0; i < CurrentBlock.points.Length; i++)
                {
                    if (CurrentBlock.points[i].X <= leftOfPlayboard || Cells[CurrentBlock.points[i].Y / LengthOfCell, CurrentBlock.points[i].X / LengthOfCell - 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    CurrentBlockPos.X -= LengthOfCell;
                    CurrentBlock.UpdatePoints(CurrentBlock.GetPoints(CurrentBlockPos, CurrentBlock.mode));
                }
            }
            if (keying.Contains("Right") && CurrentBlock != null)
            {
                bool flag = true;
                for (int i = 0; i < CurrentBlock.points.Length; i++)
                {
                    if (CurrentBlock.points[i].X + LengthOfCell >= rightOfPlayboard || Cells[CurrentBlock.points[i].Y / LengthOfCell, CurrentBlock.points[i].X / LengthOfCell + 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    CurrentBlockPos.X += LengthOfCell;
                    CurrentBlock.UpdatePoints(CurrentBlock.GetPoints(CurrentBlockPos, CurrentBlock.mode));
                }
            }
            if (keying.Contains("Down") && CurrentBlock != null)
            {
                bool flag = true;
                for (int i = 0; i < CurrentBlock.points.Length; i++)
                {
                    if (CurrentBlock.points[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentBlock.points[i].Y / LengthOfCell + 1, CurrentBlock.points[i].X / LengthOfCell] != 0)
                    {
                        AddStack();
                        CurrentBlockPos = StartPos;
                        CurrentBlock = null;
                        CheckRow();
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    CurrentBlockPos.Y += LengthOfCell;
                    CurrentBlock.UpdatePoints(CurrentBlock.GetPoints(CurrentBlockPos, CurrentBlock.mode));
                }
            }
            //Roll the block
            if (keying.Contains("Up") && CurrentBlock != null && CurrentBlock.mode != -1)
            {
                int nextMode;
                Point[] nextModePoints;
                bool flag = true;
                if (CurrentBlock.mode == 0)
                {
                    nextMode = CurrentBlock.ModeMaxIndex;
                }
                else
                {
                    nextMode = CurrentBlock.mode - 1;
                }
                nextModePoints = CurrentBlock.GetPoints(CurrentBlockPos, nextMode);
                for (int i = 0; i < nextModePoints.Length; i++)
                {
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
                    CurrentBlock.UpdatePoints(nextModePoints);
                    CurrentBlock.UpdateMode(nextMode);
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

        private void BlockFalls()
        {
            bool flag = true;
            for (int i = 0; i < CurrentBlock.points.Length; i++)
            {
                if (CurrentBlock.points[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentBlock.points[i].Y / LengthOfCell + 1, CurrentBlock.points[i].X / LengthOfCell] != 0)
                {
                    AddStack();
                    CurrentBlockPos = StartPos;
                    CurrentBlock = null;
                    CheckRow();
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                CurrentBlockPos.Y += FallSpeed;
                CurrentBlock.UpdatePoints(CurrentBlock.GetPoints(CurrentBlockPos, CurrentBlock.mode));
            }
        }

        //Add value to the cells where the falling block was to be
        private void AddStack()
        {
            for (int i = 0; i < CurrentBlock.points.Length; i++)
            {
                Cells[CurrentBlock.points[i].Y / LengthOfCell, CurrentBlock.points[i].X / LengthOfCell] = CurrentBlock.ColorIndex;
            }
        }

        //If non cell's value in a row is 0, clear the row and take in the upper row's value
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
                score += (combo * combo) * scorePerRow;
                UpdateScore(tm.Score);
            }
        }
        private Block CreateBlock(Point point, int ind)
        {
            switch (ind)
            {
                case 0:
                    return new OShapeBlock(point,-1);
                case 1:
                    return new TShapeBlock(point,0);
                case 2:
                    return new ZShapeBlock(point, 0);
                case 3:
                    return new SShapeBlock(point, 0);
                case 4:
                    return new LShapeBlock(point, 0);
                case 5:
                    return new JShapeBlock(point, 0);
                case 6:
                    return new IShapeBlock(point, 0);
                default:
                    return null;
            }
        }
        private SolidBrush BlockColor(int ind)
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
        private void UpdateScore(int currentScore)
        {
            label1.Text = "Score:\n" + currentScore;
        }
        private bool IsGameOver()
        {
            for (int i = 0; i < CurrentBlock.points.Length; i++)
            {
                if (Cells[CurrentBlock.points[i].Y / LengthOfCell, CurrentBlock.points[i].X / LengthOfCell] != 0)
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
        private static int CoordinateMapping(int gridCordinate)
        {
            return gridCordinate * GridSize;
        }
    }
    public abstract class Block
    {
        public const int LengthOfCell = 20;
        public Point[] points;
        public int mode;
        public abstract int ModeMaxIndex { get; }
        public abstract int ColorIndex { get; }
        public abstract Point[] GetPoints(Point point, int mode);
        public void UpdatePoints(Point[] points)
        {
            this.points = points;
        }
        public void UpdateMode(int mode)
        {
            this.mode = mode;
        }
    }
    public class OShapeBlock : Block
    {
        public override int ColorIndex { get { return 1; } }
        public override int ModeMaxIndex { get { return 0; } }
        public override Point[] GetPoints(Point point, int mode)
        {
            return new Point[]
            {
                new Point(point.X, point.Y),
                new Point(point.X+LengthOfCell, point.Y),
                new Point(point.X+LengthOfCell,point.Y+LengthOfCell),
                new Point(point.X,point.Y+LengthOfCell)
            };
        }
        public OShapeBlock(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }
    }
    public class TShapeBlock : Block
    {
        public override int ColorIndex { get { return 2; } }
        public override int ModeMaxIndex { get { return 3; } }

        public override Point[] GetPoints(Point point, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X,point.Y),
                        new Point(point.X+LengthOfCell,point.Y)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X,point.Y),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
                case 2:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X, point.Y),
                        new Point(point.X+LengthOfCell,point.Y),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
                case 3:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X, point.Y),
                        new Point(point.X+LengthOfCell,point.Y),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X,point.Y),
                        new Point(point.X+LengthOfCell,point.Y)
                    };
            }
        }
        public TShapeBlock(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }
    }
    public class ZShapeBlock : Block
    {
        public override int ColorIndex { get { return 3; } }
        public override int ModeMaxIndex { get { return 1; } }

        public override Point[] GetPoints(Point point, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X, point.Y),
                        new Point(point.X,point.Y+LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(point.X+LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X, point.Y),
                        new Point(point.X+LengthOfCell,point.Y),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X, point.Y),
                        new Point(point.X,point.Y+LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
            }
        }
        public ZShapeBlock(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }
    }
    public class SShapeBlock : Block
    {
        public override int ColorIndex {  get { return 4; } }
        public override int ModeMaxIndex { get { return 1; } }

        public override Point[] GetPoints(Point point, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(point.X, point.Y),
                        new Point(point.X+LengthOfCell, point.Y),
                        new Point(point.X-LengthOfCell,point.Y+LengthOfCell),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X, point.Y),
                        new Point(point.X+LengthOfCell,point.Y),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(point.X, point.Y),
                        new Point(point.X+LengthOfCell, point.Y),
                        new Point(point.X-LengthOfCell,point.Y+LengthOfCell),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
            }
        }
        public SShapeBlock(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }
    }
    public class LShapeBlock : Block
    {
        public override int ColorIndex { get { return 5; } }
        public override int ModeMaxIndex { get { return 3; } }

        public override Point[] GetPoints(Point point, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X-LengthOfCell,point.Y+LengthOfCell),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(point.X+LengthOfCell, point.Y),
                        new Point(point.X-LengthOfCell, point.Y+LengthOfCell),
                        new Point(point.X,point.Y+LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
                case 2:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X+LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
                case 3:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y-LengthOfCell),
                        new Point(point.X-LengthOfCell,point.Y)
                    };
                default:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X-LengthOfCell,point.Y+LengthOfCell),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
            }
        }
        public LShapeBlock(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }
    }
    public class JShapeBlock : Block
    {
        public override int ColorIndex { get { return 6; } }
        public override int ModeMaxIndex { get { return 3; } }

        public override Point[] GetPoints(Point point, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(point.X+LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X+LengthOfCell, point.Y),
                        new Point(point.X,point.Y+LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y-LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y)
                    };
                case 2:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X-LengthOfCell,point.Y),
                        new Point(point.X-LengthOfCell,point.Y+LengthOfCell)
                    };
                case 3:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X-LengthOfCell, point.Y+LengthOfCell),
                        new Point(point.X,point.Y+LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
                default:
                    return new Point[]
                    {
                        new Point(point.X+LengthOfCell, point.Y-LengthOfCell),
                        new Point(point.X+LengthOfCell, point.Y),
                        new Point(point.X,point.Y+LengthOfCell),
                        new Point(point.X+LengthOfCell,point.Y+LengthOfCell)
                    };
            }
        }
        public JShapeBlock(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }

    }
    public class IShapeBlock : Block
    {
        public override int ColorIndex { get { return 7; } }
        public override int ModeMaxIndex { get { return 1; } }
        public override Point[] GetPoints(Point point, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell*2),
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X,point.Y),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
                case 1:
                    return new Point[]
                    {
                        new Point(point.X-LengthOfCell*2, point.Y),
                        new Point(point.X-LengthOfCell, point.Y),
                        new Point(point.X,point.Y),
                        new Point(point.X+LengthOfCell,point.Y)
                    };
                default:
                    return new Point[]
                    {
                        new Point(point.X, point.Y-LengthOfCell*2),
                        new Point(point.X, point.Y-LengthOfCell),
                        new Point(point.X,point.Y),
                        new Point(point.X,point.Y+LengthOfCell)
                    };
            }
        }
        public IShapeBlock(Point point, int mode)
        {
            UpdatePoints(GetPoints(point, mode));
            UpdateMode(mode);
        }
    }

}
