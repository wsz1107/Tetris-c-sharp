using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace teris
{
    public partial class Form1 : Form
    {
        private TerisModel model;

        // Drawing parameters
        public const int FallSpeed = 20;
        public const int LengthOfCell = 20;
        public const int PuzzleFrameWidth = 3;
        private int topOfPlayboard = 0;
        private int bottomOfPlayboard = LengthOfCell * TerisModel.CellCountY;
        private int leftOfPlayboard = 0;
        private int rightOfPlayboard = LengthOfCell * TerisModel.CellCountX;

        // Input keys
        private List<string> keying = new List<string>();
        
        
        public Form1()
        {
            InitializeComponent();
            resetGame();
        }
        private void resetGame()
        {
            this.model.Reset();

            //Intialize color of cells
            Cells = new int[20, 10];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Cells[i, j] = 0;
                }
            }

            PuzzlePos = StartPos;
            timer1.Enabled = true;
            timer2.Enabled = true;

            //test
            for (int i = 2; i < 10; i++)
            {
                Cells[19, i] = 1;
                Cells[18, i] = 1;
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
                    if (Cells[i, j] != 0)
                    {
                        e.Graphics.DrawRectangle(PuzzlePen(Cells[i,j]), j * LengthOfCell, i * LengthOfCell, LengthOfCell, LengthOfCell);
                    }
                }
            }

            //Draw falling puzzle
            if (CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.CellCoordinates.Length; i++)
                {
                    e.Graphics.DrawRectangle(PuzzlePen(CurrentPuzzle.colorIndex), CurrentPuzzle.CellCoordinates[i].X, CurrentPuzzle.CellCoordinates[i].Y, LengthOfCell, LengthOfCell);
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
            //var entry = CurrentPuzzle;
            for (int i = 0; i < CurrentPuzzle.CellCoordinates.Length; i++)
            {
                if (CurrentPuzzle.CellCoordinates[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell + 1, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell] != 0)
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
                CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.currentMode);
                //Debug.WriteLine(PuzzlePos.Y);
            }
            pictureBox1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Control the falling puzzle
            bool flag = true;
            if (keying.Contains("Left") && CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.CellCoordinates.Length; i++)
                {
                    if (CurrentPuzzle.CellCoordinates[i].X <= leftOfPlayboard || Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell - 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    PuzzlePos.X -= LengthOfCell;
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.currentMode);
                    //Debug.WriteLine(PuzzlePos.X);
                }
            }
            if (keying.Contains("Right") && CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.CellCoordinates.Length; i++)
                {
                    if (CurrentPuzzle.CellCoordinates[i].X + LengthOfCell >= rightOfPlayboard || Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell + 1] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    PuzzlePos.X += LengthOfCell;
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.currentMode);
                    Debug.WriteLine(PuzzlePos.X);
                }
            }
            if (keying.Contains("Down") && CurrentPuzzle != null)
            {
                for (int i = 0; i < CurrentPuzzle.CellCoordinates.Length; i++)
                {
                    if (CurrentPuzzle.CellCoordinates[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell + 1, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell] != 0)
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
                    PuzzlePos.Y += LengthOfCell;
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.currentMode);

                }
            }
            if (keying.Contains("Up") && CurrentPuzzle.currentMode != -1)
            {
                for (int i = 0; i < CurrentPuzzle.CellCoordinates.Length; i++)
                {
                    if ((CurrentPuzzle.CellCoordinates[i].X <= leftOfPlayboard || Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell - 1] != 0 || CurrentPuzzle.CellCoordinates[i].X + LengthOfCell >= rightOfPlayboard || Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell + 1] != 0)
                        || (CurrentPuzzle.CellCoordinates[i].Y + LengthOfCell >= bottomOfPlayboard || Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell + 1, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell] != 0))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    if (CurrentPuzzle.currentMode == 0)
                    {
                        CurrentPuzzle.currentMode = CurrentPuzzle.ModeMaxIndex;
                    }
                    else
                    {
                        CurrentPuzzle.currentMode--;
                        Debug.WriteLine("Mode" + CurrentPuzzle.currentMode);
                    }
                    CurrentPuzzle.SetPoints(PuzzlePos, CurrentPuzzle.currentMode);
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
            for (int i = 0; i < CurrentPuzzle.CellCoordinates.Length; i++)
            {
                Cells[CurrentPuzzle.CellCoordinates[i].Y / LengthOfCell, CurrentPuzzle.CellCoordinates[i].X / LengthOfCell] = CurrentPuzzle.colorIndex;
            }
        }
        private void checkLine()
        {
            int i = 19;
            while (i >= 0)
            {
                int res = 1;
                for(int j = 0; j < 10; j++)
                {
                    res *= Cells[i, j];
                }
                if (res != 0)
                {
                    for(int k = i; k > 0; k--)
                    {
                        for(int l = 0; l < 10; l++)
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
            testPrintCellsVal();
        }
        private Puzzle CreateFallingPuzzle(Point point, int ind)
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
        public Pen PuzzlePen(int ind)
        {
            switch (ind)
            {
                case 1:
                    return new Pen(Color.Yellow, PuzzleFrameWidth);
                case 2:
                    return new Pen(Color.Violet, PuzzleFrameWidth);
                case 3:
                    return new Pen(Color.Red, PuzzleFrameWidth);
                case 4:
                    return new Pen(Color.Green, PuzzleFrameWidth);
                case 5:
                    return new Pen(Color.Orange, PuzzleFrameWidth);
                case 6:
                    return new Pen(Color.Blue, PuzzleFrameWidth);
                case 7:
                    return new Pen(Color.SkyBlue, PuzzleFrameWidth);
                default:
                    return new Pen(Color.Black, PuzzleFrameWidth);
            }
        }

        private void testPrintCellsVal()
        {
            for(int i = 19; i >= 0; i--)
            {
                for(int j = 0; j < 10; j++)
                {
                    Debug.Write(Cells[i, j]);
                }
                Debug.Write(Environment.NewLine);
            }
        }
    }

}
