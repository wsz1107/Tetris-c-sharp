using System;
using System.Diagnostics;
using tetris;

namespace tetris
{
    public struct PointCoordinate
    {
        public PointCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
    public enum Directions
    {
        Right,
        Left,
        Down,
        Up
    }
    public enum GridColors
    {
        NullColor,
        Yellow,
        Violet,
        Red,
        Green,
        Orange,
        Blue,
        SkyBlue
    }
    public class TetrisModel
    {
        public Tetrimino CurrentTetrimino;
        public Tetrimino NextTetrimino;

        public const int GridCountX = 10;
        public const int GridCountY = 20;
        public int[,] FallenGridMap = new int[GridCountY, GridCountX];
        public int[,] FallingGridMap = new int[GridCountY, GridCountX];

        public int Score = 0;
        private const int ScorePerLine = 100;

        public PointCoordinate CurrentPos;
        public const int FallSpeed = 1;

        public readonly int[,] DirectionCoordinate = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 } };

        public static readonly PointCoordinate StartPos = new PointCoordinate(5, 2);

        private readonly Random random = new Random();
        private int TetriminoType = 0;



        public void ReadyToFall()
        {
            TetriminoType = random.Next(7);
            int testTetriminoType = 1;
            if (NextTetrimino == null)
            {
                NextTetrimino = CreateTetrimino(testTetriminoType);
            }
            if (CurrentTetrimino == null)
            {
                CurrentPos = StartPos;
                CurrentTetrimino = NextTetrimino;
                CurrentTetrimino.UpdatePoints(CurrentTetrimino.GetPoints(CurrentPos.X, CurrentPos.Y, CurrentTetrimino.Mode));
                NextTetrimino = null;
            }
        }


        private Tetrimino CreateTetrimino(int index)
        {
            switch (index)
            {
                case 0:
                    return new OShapeTetrimino(-1);
                case 1:
                    return new TShapeTetrimino(0);
                case 2:
                    return new ZShapeTetrimino(0);
                case 3:
                    return new SShapeTetrimino(0);
                case 4:
                    return new LShapeTetrimino(0);
                case 5:
                    return new JShapeTetrimino(0);
                case 6:
                    return new IShapeTetrimino(0);
                default:
                    return null;
            }
        }
        public void IntializeGridMap(int[,] gridMap)
        {
            for(int i = 0; i < GridCountY; i++)
            {
                for(int j=0; j < GridCountX; j++)
                {
                    gridMap[i, j] = 0;
                }
            }
        }
        public void IntializeGridMap(int[,] gridMap, int[][] oldLocations)
        {
            for (int i = 0; i < oldLocations.Length; i++)
            {
                gridMap[oldLocations[i][1], oldLocations[i][0]] = 0;
            }
        }
        private void UpdateGridMap(int[,] gridMap)
        {
            for (int i = 0; i < CurrentTetrimino.points.Length; i++)
            {
                gridMap[CurrentTetrimino.points[i][1], CurrentTetrimino.points[i][0]] = CurrentTetrimino.ColorIndex;
            }
        }

        public bool IsCollidedWithBorders(int[][] locations)
        {
            bool flag = false;
            for (int i = 0; i < locations.Length; i++)
            {
                if (locations[i][0] < 0 || locations[i][0] >= GridCountX || locations[i][1] >= GridCountY)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        public bool IsCollidedWithGrid(int[][] locations)
        {
            bool flag = false;
            for (int i = 0; i < locations.Length; i++)
            {
                if (FallenGridMap[locations[i][1], locations[i][0]] != 0)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        public bool IsGameOver(int[][] locations)
        {
            bool flag = false;
            for (int i = 0; i < locations.Length; i++)
            {
                if (IsCollidedWithGrid(locations))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        private void CheckLines()
        {
            int combo = 0;
            int ind = GridCountY - 1;
            while (ind >= 0)
            {
                if (IsLineCompleted(ind))
                {
                    combo++;
                    EraseAndPullDownLine(ind);
                }
                else
                {
                    ind--;
                }
            }
            if (combo > 0)
            {
                Score += combo * combo * ScorePerLine;
            }
        }
        private void EraseAndPullDownLine(int lineIndex)
        {
            for (int i = lineIndex; i > 0; i--)
            {
                for (int j = 0; j < GridCountX; j++)
                {
                    FallenGridMap[i, j] = FallenGridMap[i - 1, j];
                }
                
            }

        }
        private bool IsLineCompleted(int lineIndex)
        {
            int res = 1;
            for (int i = 0; i < GridCountX; i++)
            {
                res *= FallenGridMap[lineIndex, i];
            }
            if (res == 0) return false;
            else return true;
        }
        public void ControlTetrimino(Directions direction)
        {
            if (direction != Directions.Up)
            {
                MoveCurrentTetrimino(direction);
            }
            if (direction == Directions.Up)
            {
                RotateCurrentTetrimino(direction);
            }

        }
        private void MoveCurrentTetrimino(Directions direction)
        {
            int[][] nextLocations = CurrentTetrimino.GetPoints(CurrentPos.X + DirectionCoordinate[(int)direction, 0], CurrentPos.Y + DirectionCoordinate[(int)direction, 1], CurrentTetrimino.Mode);
            if (!IsCollidedWithBorders(nextLocations) && !IsCollidedWithGrid(nextLocations))
            {
                CurrentPos.X += DirectionCoordinate[(int)direction, 0];
                CurrentPos.Y += DirectionCoordinate[(int)direction, 1];
                IntializeGridMap(FallingGridMap, CurrentTetrimino.points);
                CurrentTetrimino.UpdatePoints(nextLocations);
                UpdateGridMap(FallingGridMap);
            }
            else
            {
                if(direction == Directions.Down)
                {
                    UpdateGridMap(FallenGridMap);
                    CurrentTetrimino = null;
                    IntializeGridMap(FallingGridMap);
                    CheckLines();
                }
            }
        }

        public void RotateCurrentTetrimino(Directions direction)
        {
            int rotateDirection = 0;
            int nextMode = 0;
            if (direction == Directions.Left)
            {
                rotateDirection = 1;
            }
            if (direction == Directions.Right)
            {
                rotateDirection = -1;
            }
            if (CurrentTetrimino.Mode + rotateDirection < 0)
            {
                nextMode = CurrentTetrimino.ModeMaxIndex;
            }
            else if (CurrentTetrimino.Mode + rotateDirection > CurrentTetrimino.ModeMaxIndex)
            {
                nextMode = 0;
            }
            else
            {
                nextMode += rotateDirection;
            }
            int[][] nextLocations = CurrentTetrimino.GetPoints(CurrentPos.X, CurrentPos.Y, nextMode);
            if (!IsCollidedWithBorders(nextLocations) && !IsCollidedWithGrid(nextLocations))
            {
                CurrentTetrimino.UpdateMode(nextMode);
                CurrentTetrimino.UpdatePoints(nextLocations);
            }
        }
        private static void printGridMapVal(int[,] gridMap)
        {
            for (int i = 0; i < TetrisModel.GridCountY; i++)
            {
                for (int j = 0; j < TetrisModel.GridCountX; j++)
                {
                    Debug.Write(gridMap[i, j] + " ");
                }
                Debug.Write("\n");
            }
            Debug.WriteLine("-------------------");
        }

    }
    public abstract class Tetrimino
    {
        public int Mode;
        public abstract int ModeMaxIndex { get; }
        public abstract int ColorIndex { get; }

        public int[][] points;
        public abstract int[][] GetPoints(int pivotX, int pivotY, int mode);
        public void UpdatePoints(int[][] points)
        {
            this.points = points;
        }
        public void UpdateMode(int newMode)
        {
            this.Mode = newMode;
        }
    }
    public class OShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 1;
        public override int ModeMaxIndex => 0;
        public OShapeTetrimino(int mode)
        {
            this.UpdateMode(mode);
        }
        public override int[][] GetPoints(int pivotX, int pivotY, int mode)
        {
            return new int[4][]
            {
                new int[2]{pivotX,pivotY},
                new int[2]{pivotX+1,pivotY},
                new int[2]{pivotX,pivotY+1},
                new int[2]{pivotX+1,pivotY+1}
            };
        }
    }
    public class TShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 2;
        public override int ModeMaxIndex => 3;
        public TShapeTetrimino(int mode)
        {
            this.UpdateMode(mode);
        }
        public override int[][] GetPoints(int pivotX, int pivotY, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX-1,pivotY},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX+1,pivotY}
                    };
                case 1:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX, pivotY},
                        new int[2]{pivotX+1,pivotY},
                        new int[2]{pivotX,pivotY+1}
                    };
                case 2:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX-1, pivotY},
                        new int[2]{pivotX+1,pivotY},
                        new int[2]{pivotX,pivotY+1}
                    };
                case 3:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX-1, pivotY},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX,pivotY+1}
                    };
                default:
                    return null;
            }
        }
    }
    public class ZShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 3;
        public override int ModeMaxIndex => 1;
        public ZShapeTetrimino(int mode)
        {
            this.UpdateMode(mode);
        }

        public override int[][] GetPoints(int pivotX, int pivotY, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX-1,pivotY},
                        new int[2]{pivotX,pivotY+1},
                        new int[2]{pivotX+1,pivotY+1}
                    };
                case 1:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX-1, pivotY},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX-1,pivotY+1}
                    };
                default:
                    return null;
            }
        }
    }
    public class SShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 4;
        public override int ModeMaxIndex => 1;
        public SShapeTetrimino(int mode)
        {
            this.UpdateMode(mode);
        }

        public override int[][] GetPoints(int pivotX, int pivotY, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX+1,pivotY},
                        new int[2]{pivotX-1,pivotY+1},
                        new int[2]{pivotX,pivotY+1}
                    };
                case 1:
                    return new int[4][]
                    {
                        new int[2]{pivotX-1,pivotY-1},
                        new int[2]{pivotX-1, pivotY},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX,pivotY+1}
                    };
                default:
                    return null;
            }
        }
    }
    public class LShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 5;
        public override int ModeMaxIndex => 3;
        public LShapeTetrimino(int mode)
        {
            this.UpdateMode(mode);
        }
        public override int[][] GetPoints(int pivotX, int pivotY, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX,pivotY+1},
                        new int[2]{pivotX+1,pivotY+1}
                    };
                case 1:
                    return new int[4][]
                    {
                        new int[2]{pivotX-1,pivotY},
                        new int[2]{pivotX, pivotY},
                        new int[2]{pivotX+1,pivotY},
                        new int[2]{pivotX-1,pivotY+1}
                    };
                case 2:
                    return new int[4][]
                    {
                        new int[2]{pivotX-1,pivotY-1},
                        new int[2]{pivotX, pivotY-1},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX,pivotY+1}
                    };
                case 3:
                    return new int[4][]
                    {
                        new int[2]{pivotX+1,pivotY-1},
                        new int[2]{pivotX-1, pivotY},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX+1,pivotY}
                    };
                default:
                    return null;
            }
        }
    }
    public class JShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 6;
        public override int ModeMaxIndex => 3;
        public JShapeTetrimino(int mode)
        {
            this.UpdateMode(mode);
        }
        public override int[][] GetPoints(int pivotX, int pivotY, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX-1,pivotY+1},
                        new int[2]{pivotX,pivotY+1}
                    };
                case 1:
                    return new int[4][]
                    {
                        new int[2]{pivotX-1,pivotY-1},
                        new int[2]{pivotX-1, pivotY},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX+1,pivotY}
                    };
                case 2:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX+1, pivotY-1},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX,pivotY+1}
                    };
                case 3:
                    return new int[4][]
                    {
                        new int[2]{pivotX-1,pivotY},
                        new int[2]{pivotX, pivotY},
                        new int[2]{pivotX+1,pivotY},
                        new int[2]{pivotX+1,pivotY+1}
                    };
                default:
                    return null;
            }
        }
    }
    public class IShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 7;
        public override int ModeMaxIndex => 1;
        public IShapeTetrimino(int mode)
        {
            this.UpdateMode(mode);
        }
        public override int[][] GetPoints(int pivotX, int pivotY, int mode)
        {
            switch (mode)
            {
                case 0:
                    return new int[4][]
                    {
                        new int[2]{pivotX,pivotY-2},
                        new int[2]{pivotX,pivotY-1},
                        new int[2]{pivotX,pivotY},
                        new int[2]{pivotX,pivotY+1}
                    };
                case 1:
                    return new int[4][]
                    {
                        new int[2]{pivotX-1,pivotY},
                        new int[2]{pivotX, pivotY},
                        new int[2]{pivotX+1,pivotY},
                        new int[2]{pivotX+2,pivotY}
                    };
                default:
                    return null;
            }
        }
    }
}

