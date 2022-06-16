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
        private const int BlockFrameWidth = 3;
        private const int widthOfPlayboardFrame = 3;


        private readonly Point NextTetrisPos = new Point(12, 7);

        private List<string> keying = new List<string>();

        public Form1()
        {
            InitializeComponent();
            ResetGame();
            test();
        }
        private void test()
        {
            for(int i = 0; i < 8; i++)
            {
                tm.FallenGridMap[19, i] = 1;
            }
            
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
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Draw main playboard
            Pen playboardFramePen = new Pen(Color.Gray, widthOfPlayboardFrame);
            e.Graphics.DrawLine(playboardFramePen, rightOfPlayboard, topOfPlayboard, rightOfPlayboard, bottomOfPlayboard);

            //Draw next block
            if (tm.NextTetrimino != null)
            {
                int[][] nextTetriminoPointsLocations = tm.NextTetrimino.GetPoints(NextTetrisPos.X, NextTetrisPos.Y, tm.NextTetrimino.Mode);
                for (int i = 0; i < nextTetriminoPointsLocations.Length; i++)
                {
                    e.Graphics.DrawRectangle(new Pen(BlockColor(tm.NextTetrimino.ColorIndex), BlockFrameWidth), CoordinateMapping(nextTetriminoPointsLocations[i][0]), CoordinateMapping(nextTetriminoPointsLocations[i][1]), GridSize, GridSize);
                }
            }

            //Draw stack and falling block
            for (int i = 0; i < TetrisModel.GridCountY; i++)
            {
                for (int j = 0; j < TetrisModel.GridCountX; j++)
                {
                    if (tm.FallenGridMap[i, j] != 0)
                    {
                        e.Graphics.FillRectangle(BlockColor(tm.FallenGridMap[i, j]), CoordinateMapping(j), CoordinateMapping(i), GridSize, GridSize);
                    }
                    if (tm.FallingGridMap[i, j] != 0)
                    {
                        e.Graphics.DrawRectangle(new Pen(BlockColor(tm.FallingGridMap[i, j]),3), CoordinateMapping(j), CoordinateMapping(i), GridSize, GridSize);
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tm.ReadyToFall();
            //printGridMapVal(tm.FallenGridMap);
            if (tm.IsGameOver(tm.CurrentTetrimino.points))
            {
                GameOver();
                //Debug.WriteLine(tm.CurrentPos.X.ToString()+", "+ tm.CurrentPos.Y.ToString());
            }
            tm.ControlTetrimino(Directions.Down);
            UpdateScore(tm.Score);
            pictureBox1.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (keying.Contains("Left") && tm.CurrentTetrimino != null)
            {
                tm.ControlTetrimino(Directions.Left);
            }
            if (keying.Contains("Right") && tm.CurrentTetrimino != null)
            {
                tm.ControlTetrimino(Directions.Right);
            }
            if (keying.Contains("Down") && tm.CurrentTetrimino != null)
            {
                tm.ControlTetrimino(Directions.Down);
            }
            if (keying.Contains("Up") && tm.CurrentTetrimino != null && tm.CurrentTetrimino.Mode != -1)
            {
                tm.ControlTetrimino(Directions.Up);
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

        private void button1_Click(object sender, EventArgs e)
        {
            ResetGame();
        }
        private void UpdateScore(int currentScore)
        {
            label1.Text = "Score:\n" + currentScore;
        }
        private void GameOver()
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
        private static void printGridMapVal(int[,] gridMap)
        {
            for(int i = 0; i < TetrisModel.GridCountY; i++)
            {
                for(int j = 0; j < TetrisModel.GridCountX; j++)
                {
                    Debug.Write(gridMap[i,j]+" ");
                }
                Debug.Write("\n");
            }
            Debug.WriteLine("-------------------");
        }
    }
}