using System.Collections.Generic;

namespace Islands
{
    public class GraphTraversal
    {
        public unsafe static int CountIslands(int* pData, int w, int h)
        {
            var counter = 1;

            var queue = new Queue<(int x, int y)>();

            int* pointer = pData;

            var w1 = w - 1;
            var h1 = h - 1;

            for (var j = 0; j < h; j++)
            {
                for (var i = 0; i < w; i++, pointer++)
                {
                    if (*pointer == 1)
                    {
                        counter++;
                        queue.Enqueue((i, j));

                        while (queue.TryDequeue(out var tuple))
                        {
                            var (x, y) = tuple;
                            var index = x + y * w;
                            if (pData[index] == 1)
                            {
                                pData[index] = counter;

                                if (x > 0) queue.Enqueue((x - 1, y));
                                if (x < w1) queue.Enqueue((x + 1, y));
                                if (y > 0) queue.Enqueue((x, y - 1));
                                if (y < h1) queue.Enqueue((x, y + 1));
                            }
                        }
                    }
                }
            }

            return counter - 1;
        }
    }
}
