using System;
using System.Collections.Generic;

public static class StatsEx
{
    public static double Mean(double[] d)
    {
        double num = 0.0;
        for (int i = 0; i < d.Length; i++)
        {
            num += d[i];
        }

        return num / (double)d.Length;
    }

    public static double StdDev(double[] d)
    {
        if (d.Length == 0)
        {
            return double.NaN;
        }

        double num = 0.0;
        double num2 = Mean(d);
        for (int i = 0; i < d.Length; i++)
        {
            double num3 = d[i] - num2;
            num += num3 * num3;
        }

        return Math.Sqrt(num / (double)(d.Length - 1));
    }

    public static double StdDev2D(List<PointR> points)
    {
        if (points.Count == 0)
        {
            return double.NaN;
        }

        double num = 0.0;
        PointR p = PointR.Centroid(points);
        for (int i = 0; i < points.Count; i++)
        {
            double num2 = PointR.Distance(p, points[i]);
            num += num2 * num2;
        }

        return Math.Sqrt(num / (double)(points.Count - 1));
    }
}
