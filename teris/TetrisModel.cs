using System;

namespace tetris
{
    public class TetrisModel
    {
        public Tetrimino CurrentTetrimino;
        //public Tetrimino NextTetrimino;

        const int GridCountX = 10;
        const int GridCountY = 20;
        int[,] FallenGrid = new int[GridCountY, GridCountX];
        int[,] FallingGrid = new int[GridCountY, GridCountX];

        public int[,] CurrentPos;
        public const int FallSpeed=1;

        public readonly int StartPosX = 5, StartPosY = 0;
        //public readonly int StandbyPosX, StandbyPosY;

        private readonly Random random = new Random();

        public Tetrimino CreateTetrimino(int posX, int posY, int randomIndex)
        {
            switch (randomIndex)
            {
                case 0:
                    return new OShapeTetrimino(posX,posY, -1);
                case 1:
                    return new TShapeTetrimino(posX, posY, 0);
                case 2:
                    return new ZShapeTetrimino(posX, posY, 0);
                case 3:
                    return new SShapeTetrimino(posX, posY, 0);
                case 4:
                    return new LShapeTetrimino(posX, posY, 0);
                case 5:
                    return new JShapeTetrimino(posX, posY, 0);
                case 6:
                    return new IShapeTetrimino(posX, posY, 0);
                default:
                    return null;
            }
        }

    }
    public abstract class Tetrimino
    {
        int pivotX, pivotY;
        int mode;
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
        public OShapeTetrimino(int x, int y, int mode)
        {
            UpdatePoints(GetPoints(x, y, mode));
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

