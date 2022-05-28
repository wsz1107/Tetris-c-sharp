using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teris
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class PuzzleCell
    {
        public int CoordX;
        public int CoordY;
        public int ScreenCoordX { get { return this.CoordX * Form1.LengthOfCell; } }
        public int ScreenCoordY { get { return this.CoordY * Form1.LengthOfCell; } }

        public void SetCoords(int x, int y)
        {
            this.CoordX = x;
            this.CoordY = y;
        }
    }

    /// <summary>
    /// An abstract teris block
    /// </summary>
    public abstract class Puzzle
    {
        public enum PuzzleType
        {
            Stick,
            Rect,
            Triangle,
            LShape,
            ReverseL,
            ZShape,
            ReverseZ,
        }
        public abstract PuzzleType Type { get; }
        public abstract int ModeMaxIndex { get; }

        public PuzzleCell[] Cells;
        public abstract int PivotX { get; }
        public abstract int PivotY { get; }

        public int currentMode;

        public abstract void SetPoints(int pivotX, int pivotY);

        public void NextMode()
        {
            this.currentMode++;
            if (this.currentMode >= this.ModeMaxIndex)
                this.currentMode = 0;
        }
    }

    public class RectPuzzle : Puzzle
    {
        public override PuzzleType Type => PuzzleType.Rect;
        public override int ModeMaxIndex => 0;
        public override int PivotX => this.Cells[0].ScreenCoordX;
        public override int PivotY => this.Cells[0].ScreenCoordY;

        public override void SetPoints(int pivotX, int pivotY)
        {
            if (this.Cells == null)
                this.Cells = new PuzzleCell[4];

            this.Cells[0].SetCoords(pivotX, pivotY);
            this.Cells[1].SetCoords(pivotX + 1, pivotY);
            this.Cells[2].SetCoords(pivotX, pivotY + 1);
            this.Cells[3].SetCoords(pivotX + 1, pivotY + 1);
        }
    }

    //public class TrianglePuzzle : Puzzle
    //{

    //    public override void SetPoints(Point _point, int _mode)
    //    {
    //        this.currentMode = _mode;
    //        modeMaxIndex = 3;
    //        colorIndex = 2;
    //        pen = new Pen(Color.Violet, FrameWidth);
    //        switch (currentMode)
    //        {
    //            case 0:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X-LengthOfCell, _point.Y),
    //                    new Point(_point.X,_point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y)
    //                };
    //                break;
    //            case 1:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X-LengthOfCell, _point.Y),
    //                    new Point(_point.X,_point.Y),
    //                    new Point(_point.X,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 2:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X-LengthOfCell, _point.Y),
    //                    new Point(_point.X, _point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y),
    //                    new Point(_point.X,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 3:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X, _point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y),
    //                    new Point(_point.X,_point.Y+LengthOfCell)
    //                };
    //                break;
    //        }
    //    }
    //}
    //public class ZPuzzles : Puzzle
    //{
    //    public override void SetPoints(Point _point, int _mode)
    //    {
    //        this.currentMode = _mode;
    //        modeMaxIndex = 1;
    //        colorIndex = 3;
    //        pen = new Pen(Color.Red, FrameWidth);
    //        switch (currentMode)
    //        {
    //            case 0:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X-LengthOfCell, _point.Y),
    //                    new Point(_point.X, _point.Y),
    //                    new Point(_point.X,_point.Y+LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 1:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
    //                    new Point(_point.X, _point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y),
    //                    new Point(_point.X,_point.Y+LengthOfCell)
    //                };
    //                break;
    //        }
    //    }
    //}
    //public class XZPuzzles : Puzzle
    //{
    //    public override void SetPoints(Point _point, int _mode)
    //    {
    //        this.currentMode = _mode;
    //        modeMaxIndex = 1;
    //        colorIndex = 4;
    //        pen = new Pen(Color.Green, FrameWidth);
    //        switch (currentMode)
    //        {
    //            case 0:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X, _point.Y),
    //                    new Point(_point.X+LengthOfCell, _point.Y),
    //                    new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
    //                    new Point(_point.X,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 1:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X, _point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
    //                };
    //                break;
    //        }
    //    }
    //}
    //public class LPuzzles : Puzzle
    //{
    //    public override void SetPoints(Point _point, int _mode)
    //    {
    //        this.currentMode = _mode;
    //        modeMaxIndex = 3;
    //        colorIndex = 5;
    //        pen = new Pen(Color.Orange, FrameWidth);
    //        switch (currentMode)
    //        {
    //            case 0:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
    //                    new Point(_point.X-LengthOfCell, _point.Y),
    //                    new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell),
    //                    new Point(_point.X,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 1:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X+LengthOfCell, _point.Y),
    //                    new Point(_point.X-LengthOfCell, _point.Y+LengthOfCell),
    //                    new Point(_point.X,_point.Y+LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 2:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 3:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y-LengthOfCell),
    //                    new Point(_point.X-LengthOfCell,_point.Y)
    //                };
    //                break;
    //        }
    //    }
    //}
    //public class XLPuzzles : Puzzle
    //{
    //    public override void SetPoints(Point _point, int _mode)
    //    {
    //        this.currentMode = _mode;
    //        modeMaxIndex = 3;
    //        colorIndex = 6;
    //        pen = new Pen(Color.Blue, FrameWidth);
    //        switch (currentMode)
    //        {
    //            case 0:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X+LengthOfCell, _point.Y-LengthOfCell),
    //                    new Point(_point.X+LengthOfCell, _point.Y),
    //                    new Point(_point.X,_point.Y+LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 1:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y-LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y)
    //                };
    //                break;
    //            case 2:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X-LengthOfCell, _point.Y-LengthOfCell),
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X-LengthOfCell,_point.Y),
    //                    new Point(_point.X-LengthOfCell,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 3:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X+LengthOfCell, _point.Y),
    //                    new Point(_point.X-LengthOfCell, _point.Y+LengthOfCell),
    //                    new Point(_point.X,_point.Y+LengthOfCell),
    //                    new Point(_point.X+LengthOfCell,_point.Y+LengthOfCell)
    //                };
    //                break;
    //        }
    //    }

    //}
    //public class StickPuzzles : Puzzle
    //{
    //    public override void SetPoints(Point _point, int _mode)
    //    {
    //        this.currentMode = _mode;
    //        modeMaxIndex = 1;
    //        colorIndex = 7;
    //        pen = new Pen(Color.SkyBlue, FrameWidth);
    //        switch (currentMode)
    //        {
    //            case 0:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X, _point.Y-LengthOfCell*2),
    //                    new Point(_point.X, _point.Y-LengthOfCell),
    //                    new Point(_point.X,_point.Y),
    //                    new Point(_point.X,_point.Y+LengthOfCell)
    //                };
    //                break;
    //            case 1:
    //                this.CellCoordinates = new Point[]
    //                {
    //                    new Point(_point.X-LengthOfCell*2, _point.Y),
    //                    new Point(_point.X-LengthOfCell, _point.Y),
    //                    new Point(_point.X,_point.Y),
    //                    new Point(_point.X+LengthOfCell,_point.Y)
    //                };
    //                break;
    //        }
    //    }
    //}


    public class TerisModel
    {
        public Puzzle CurrentPuzzle;
        public int[,] StackedCells;

        private Random random = new Random();

        public const int CellCountX = 10;
        public const int CellCountY = 20;
        private static readonly int[] spawnPoint = new int[2] { 5, 5 };


        public void Reset()
        {

        }
    }
}
