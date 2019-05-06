using System.Collections.Generic;

namespace Islands
{
    public class Reconciliation
    {
        public unsafe static int CountIslands(int* pData, int w, int h)
        {
            var pointer = pData;
            var pointer1 = pData - w;

            var counter = 1;

            var recMap = new Dictionary<int, int>();

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++, pointer++, pointer1++)
                {
                    var c = *pointer;
                    if (c == 0)
                        continue;

                    var left = 0;
                    if (x > 0)
                    {
                        var v = *(pointer - 1);
                        if (v != 0)
                            left = v;
                    }

                    var up = 0;
                    if (y > 0)
                    {
                        var v = *pointer1;
                        if (v != 0)
                            up = v;
                    }

                    if (left != 0)
                    {
                        if (up != 0)
                        {
                            var origLeft = left;
                            var origUp = up;

                            while (true)
                            {
                                if (left == up)
                                {
                                    *pointer = left; // or 'up'
                                    break;
                                }
                                else
                                {
                                    if (left > up)
                                    {
                                        if (recMap.TryGetValue(left, out var p))
                                        {
                                            if (p != up)
                                            {
                                                left = p;
                                                continue;
                                            }
                                            else if (origLeft != left)
                                            {
                                                recMap[origLeft] = up;
                                            }
                                        }
                                        else
                                        {
                                            recMap.Add(left, up);
                                        }

                                        *pointer = left;
                                        break;
                                    }
                                    else
                                    {
                                        if (recMap.TryGetValue(up, out var q))
                                        {
                                            if (q != left)
                                            {
                                                up = q;
                                                continue;
                                            }
                                            else if (origUp != up)
                                            {
                                                recMap[origUp] = left;
                                            }
                                        }
                                        else
                                        {
                                            recMap.Add(up, left);
                                        }

                                        *pointer = up;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            *pointer = left;
                        }
                    }
                    else if (up != 0)
                    {
                        *pointer = up;
                    }
                    else
                    {
                        counter++;
                        *pointer = counter;
                    }
                }
            }

            return counter - 1 - recMap.Count;
        }
    }
}
