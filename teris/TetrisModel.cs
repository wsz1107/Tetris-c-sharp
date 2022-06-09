﻿using System;

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

        const int GridCountX = 10;
        const int GridCountY = 20;
        int[,] FallenGridMap = new int[GridCountY, GridCountX];
        int[,] FallingGridMap = new int[GridCountY, GridCountX];

        public int Score = 0;
        private const int ScorePerLine = 100;

        public PointCoordinate CurrentPos;
        public const int FallSpeed = 1;

        public readonly int[,] DirectionCoordinate = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 } };

        public readonly PointCoordinate StartPos = new PointCoordinate(5, 0);

        private readonly Random random = new Random();
        private int TetriminoType = 0;



        public void ReadyToFall()
        {
            TetriminoType = random.Next(7);
            if(NextTetrimino == null)
            {
                CurrentPos = StartPos;
                CreateTetrimino(CurrentPos.X, CurrentPos.Y, TetriminoType);
            }
            if (CurrentTetrimino == null)
            {
                CurrentTetrimino = NextTetrimino;
                NextTetrimino = null;
            }
        }


        private void CreateTetrimino(int posX, int posY, int index)
        {
            if (CurrentTetrimino == null)
            {
                switch (index)
                {
                    case 0:
                        CurrentTetrimino = new OShapeTetrimino(posX, posY, -1);
                        break;
                    case 1:
                        CurrentTetrimino = new TShapeTetrimino(posX, posY, 0);
                        break;
                    case 2:
                        CurrentTetrimino = new ZShapeTetrimino(posX, posY, 0);
                        break;
                    case 3:
                        CurrentTetrimino = new SShapeTetrimino(posX, posY, 0);
                        break;
                    case 4:
                        CurrentTetrimino = new LShapeTetrimino(posX, posY, 0);
                        break;
                    case 5:
                        CurrentTetrimino = new JShapeTetrimino(posX, posY, 0);
                        break;
                    case 6:
                        CurrentTetrimino = new IShapeTetrimino(posX, posY, 0);
                        break;
                    default:
                        CurrentTetrimino = null;
                        break;
                }
            }
        }
        private void IntializeGridMap(int[,] gridMap)
        {
            gridMap = new int[GridCountY, GridCountX];
        }
        private void IntializeGridMap(int[,] gridMap, int[][] oldLocations)
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
                if (locations[i][0] < 0 || locations[i][0] > GridCountX || locations[i][1] > GridCountY)
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
            for(int i = 0; i < locations.Length; i++)
            {
                if (IsCollidedWithGrid(locations))
                {
                    flag=true;
                    break;
                }
            }
            return flag;
        }
        public void CheckLines()
        {
            int combo = 0;
            int ind = GridCountY - 1;
            while(ind >= 0)
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
            if(combo > 0)
            {
                Score += combo * combo * ScorePerLine;
            }
        }
        private void EraseAndPullDownLine(int lineIndex)
        {
            for(int i = lineIndex; i > 0; i++)
            {
                for (int j = 0; j < GridCountX; j++)
                {
                    FallenGridMap[lineIndex, j] = FallenGridMap[lineIndex - 1, j];
                }
            }
            
        }
        private bool IsLineCompleted(int lineIndex)
        {
            int res = 1;
            for(int i = 0; i < GridCountX; i++)
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
        public void MoveCurrentTetrimino(Directions direction)
        {
            int[][] nextLocations = CurrentTetrimino.GetPoints(CurrentPos.X + DirectionCoordinate[(int)direction, 0], CurrentPos.Y + DirectionCoordinate[(int)direction, 1], CurrentTetrimino.mode);
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
                UpdateGridMap(FallenGridMap);
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
            if (CurrentTetrimino.mode + rotateDirection < 0)
            {
                nextMode = CurrentTetrimino.ModeMaxIndex;
            }
            else if (CurrentTetrimino.mode + rotateDirection > CurrentTetrimino.ModeMaxIndex)
            {
                nextMode = 0;
            }
            else
            {
                nextMode += rotateDirection;
            }
            int[][] nextLocations = CurrentTetrimino.GetPoints(CurrentPos.X,CurrentPos.Y, nextMode);
            if(!IsCollidedWithBorders(nextLocations) && !IsCollidedWithGrid(nextLocations))
            {
                CurrentTetrimino.UpdateMode(nextMode);
                CurrentTetrimino.UpdatePoints(nextLocations);
            }
        }


    }
    public abstract class Tetrimino
    {
        public int mode;
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
            this.mode = newMode;
        }
    }
    public class OShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 1;
        public override int ModeMaxIndex => 0;

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
        public OShapeTetrimino(int pivotX, int pivotY, int mode)
        {
            UpdatePoints(GetPoints(pivotX, pivotY, mode));
            UpdateMode(mode);
        }
    }
    public class TShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 2;
        public override int ModeMaxIndex => 3;

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
        public TShapeTetrimino(int x, int y, int mode)
        {
            UpdatePoints(GetPoints(x, y, mode));
            UpdateMode(mode);
        }

    }
    public class ZShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 3;
        public override int ModeMaxIndex => 1;
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
        public ZShapeTetrimino(int x, int y, int mode)
        {
            UpdatePoints(GetPoints(x, y, mode));
            UpdateMode(mode);
        }

    }
    public class SShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 4;
        public override int ModeMaxIndex => 1;
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
        public SShapeTetrimino(int x, int y, int mode)
        {
            UpdatePoints(GetPoints(x, y, mode));
            UpdateMode(mode);
        }

    }
    public class LShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 5;
        public override int ModeMaxIndex => 3;
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
        public LShapeTetrimino(int x, int y, int mode)
        {
            UpdatePoints(GetPoints(x, y, mode));
            UpdateMode(mode);
        }

    }
    public class JShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 6;
        public override int ModeMaxIndex => 3;
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
        public JShapeTetrimino(int x, int y, int mode)
        {
            UpdatePoints(GetPoints(x, y, mode));
            UpdateMode(mode);
        }

    }
    public class IShapeTetrimino : Tetrimino
    {
        public override int ColorIndex => 7;
        public override int ModeMaxIndex => 1;
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
        public IShapeTetrimino(int x, int y, int mode)
        {
            UpdatePoints(GetPoints(x, y, mode));
            UpdateMode(mode);
        }
    }
}

