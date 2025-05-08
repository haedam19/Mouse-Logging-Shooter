/* 
 * Cloned from WobbrockLib.dll
 * C# .NET 2.0 library of application-agnostic formulae, widgets, data structures, and algorithms
 * 
 *      Jacob O. Wobbrock, Ph.D.
 * 		The Information School
 *		University of Washington
 *		Mary Gates Hall, Box 352840
 *		Seattle, WA 98195-2840
 *		wobbrock@uw.edu
 */

/// <summary>
/// 2차원 x, y좌표를 double로 저장합니다.
/// </summary>
using System;

public struct PointR
{
    public static readonly PointR Empty;

    private double _x;

    private double _y;

    public double X
    {
        get
        {
            return _x;
        }
        set
        {
            _x = value;
        }
    }

    public double Y
    {
        get
        {
            return _y;
        }
        set
        {
            _y = value;
        }
    }

    public PointR(double x, double y)
    {
        _x = x;
        _y = y;
    }

    public PointR(PointR pt)
    {
        _x = pt._x;
        _y = pt._y;
    }

    public override string ToString()
    {
        return $"{{X={_x}, Y={_y}}}";
    }

    public static PointR FromString(string s)
    {
        PointR result = Empty;
        try
        {
            int num = s.IndexOf('X');
            int num2 = s.IndexOf(',', num + 2);
            double x = double.Parse(s.Substring(num + 2, num2 - (num + 2)));
            int num3 = s.IndexOf('Y');
            num2 = s.IndexOf('}', num3 + 2);
            double y = double.Parse(s.Substring(num3 + 2, num2 - (num3 + 2)));
            result = new PointR(x, y);
        }
        catch
        {
        }

        return result;
    }

    public static double Distance(PointR p1, PointR p2)
    {
        double num = p2.X - p1.X;
        double num2 = p2.Y - p1.Y;
        return Math.Sqrt(num * num + num2 * num2);
    }

    public static bool operator ==(PointR p1, PointR p2)
    {
        if (p1._x == p2._x)
        {
            return p1._y == p2._y;
        }

        return false;
    }

    public static bool operator !=(PointR p1, PointR p2)
    {
        if (p1._x == p2._x)
        {
            return p1._y != p2._y;
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        if (obj is PointR pointR)
        {
            if (X == pointR.X)
            {
                return Y == pointR.Y;
            }

            return false;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}