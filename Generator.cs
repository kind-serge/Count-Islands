using System;

namespace Islands
{
    public class PlasmaFractalGenerator
    {
        double gRoughness;
        double gBigSize;
        Random rnd;

        public static int[] NewIslandMap(int w, int h, int seed, double roughness = 200)
        {
            var map = new PlasmaFractalGenerator { rnd = new Random(seed) }.Generate(w, h, roughness);
            var result = new int[w * h];

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    if (map[x, y] > .7)
                        result[x + y * w] = 1;
                }
            }

            return result;
        }

        double[,] Generate(int iWidth, int iHeight, double iRoughness)
        {
            double c1, c2, c3, c4;
            double[,] points = new double[iWidth + 1, iHeight + 1];
            c1 = rnd.NextDouble();
            c2 = rnd.NextDouble();
            c3 = rnd.NextDouble();
            c4 = rnd.NextDouble();
            gRoughness = iRoughness;
            gBigSize = iWidth + iHeight;
            DivideGrid(ref points, 0, 0, iWidth, iHeight, c1, c2, c3, c4);
            return points;
        }

        void DivideGrid(ref double[,] points, double x, double y, double width, double height, double c1, double c2, double c3, double c4)
        {
            double Edge1, Edge2, Edge3, Edge4, Middle;

            double newWidth = Math.Floor(width / 2);
            double newHeight = Math.Floor(height / 2);

            if (width > 1 || height > 1)
            {
                Middle = ((c1 + c2 + c3 + c4) / 4) + Displace(newWidth + newHeight);
                Edge1 = ((c1 + c2) / 2);
                Edge2 = ((c2 + c3) / 2);
                Edge3 = ((c3 + c4) / 2);
                Edge4 = ((c4 + c1) / 2);
                Middle = Rectify(Middle);
                Edge1 = Rectify(Edge1);
                Edge2 = Rectify(Edge2);
                Edge3 = Rectify(Edge3);
                Edge4 = Rectify(Edge4);
                DivideGrid(ref points, x, y, newWidth, newHeight, c1, Edge1, Middle, Edge4);
                DivideGrid(ref points, x + newWidth, y, width - newWidth, newHeight, Edge1, c2, Edge2, Middle);
                DivideGrid(ref points, x + newWidth, y + newHeight, width - newWidth, height - newHeight, Middle, Edge2, c3, Edge3);
                DivideGrid(ref points, x, y + newHeight, newWidth, height - newHeight, Edge4, Middle, Edge3, c4);
            }
            else
            {
                double c = (c1 + c2 + c3 + c4) / 4;
                points[(int)(x), (int)(y)] = c;
                if (width == 2)
                    points[(int)(x + 1), (int)(y)] = c;
                if (height == 2)
                    points[(int)(x), (int)(y + 1)] = c;
                if ((width == 2) || (height == 2))
                    points[(int)(x + 1), (int)(y + 1)] = c;
            }
        }

        double Rectify(double iNum)
        {
            if (iNum < 0)
                iNum = 0;
            else if (iNum > 1.0)
                iNum = 1.0;
            return iNum;
        }

        double Displace(double SmallSize)
        {
            double Max = SmallSize / gBigSize * gRoughness;
            return (rnd.NextDouble() - 0.5) * Max;
        }
    }
}
