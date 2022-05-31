using System;

public class TetrisModel
{
	public int[,] CurrentPos;
	public int FallSpeed;
	public const int[,] StartPos;
	public const int[,] StandbyPos;

}
 public abstract class Tetrimino
{
	int pivotX,pivotY;
	int mode;

	public List<int[,]> points;
	public abstract List<int[,]> GetPoints(int[,] point, int mode);
	public void UpdatePoints(List<int[,]> points)
    {
		this.points = points;
    }
	public void UpdateMode(int newMode)
    {
		this.mode= newMode;
    }
}
public class OShapeTetrimino:Tetrimino
{

}
public class TShapeTetrimino : Tetrimino
{

}
public class ZShapeTetrimino : Tetrimino
{

}
public class SShapeTetrimino : Tetrimino
{

}
public class LShapeTetrimino : Tetrimino
{

}
public class JShapeTetrimino : Tetrimino
{

}
public class IShapeTetrimino : Tetrimino
{

}