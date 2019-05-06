using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Islands
{
    public class ParallelReconciliation
    {
        public unsafe static int CountIslands(int* pData, int w, int h, int threadCount)
        {
            // Sub-divide top to bottom just for the demo's sake.
            // Ideally, needs to be partitioned both horizontally and vertically.
            var sectionHeight = h / threadCount;

            var threads = new Thread[threadCount];
            var sectionCounts = new int[threadCount];

            for (var i = 0; i < threadCount; i++)
            {
                var sectionIndex = i;
                var thread = new Thread(() =>
                {
                    bool isFirstSection = sectionIndex == 0;
                    bool isLastSection = sectionIndex == threadCount - 1;
                    var thisSectionHeight = sectionHeight;
                    if (isLastSection)
                        thisSectionHeight = h - (threadCount - 1) * sectionHeight;

                    sectionCounts[sectionIndex] = CountIslandsInSection(
                        pData + w * (sectionIndex * sectionHeight),
                        w, thisSectionHeight,
                        normalizeTop: !isFirstSection,
                        normalizeBottom: !isLastSection);
                });
                threads[i] = thread;
                thread.Start();
            }

            for (var i = 0; i < threadCount; i++)
                threads[i].Join();

            // Reconcile sections

            var stride = int.MaxValue / threadCount;
            var sRecMap = new Dictionary<int, int>();
            for (var s = 0; s < threadCount - 1; s++)
            {
                var bottomPtr = pData + w * sectionHeight * (s + 1);
                var topPtr = bottomPtr - w;

                var topOffset = s * stride;
                var bottomOffset = topOffset + stride;

                for (var x = 0; x < w; x++, topPtr++, bottomPtr++)
                {
                    var top = *topPtr;
                    var bottom = *bottomPtr;

                    if (top == 0 || bottom == 0)
                        continue;

                    top += topOffset;
                    bottom += bottomOffset;

                    while (true)
                    {
                        if (top == bottom)
                        {
                            break;
                        }
                        else
                        {
                            if (top > bottom)
                            {
                                if (sRecMap.TryGetValue(top, out var p))
                                {
                                    if (p != bottom)
                                    {
                                        top = p;
                                        continue;
                                    }
                                }
                                else
                                {
                                    sRecMap.Add(top, bottom);
                                }
                                break;
                            }
                            else
                            {
                                if (sRecMap.TryGetValue(bottom, out var q))
                                {
                                    if (q != top)
                                    {
                                        bottom = q;
                                        continue;
                                    }
                                }
                                else
                                {
                                    sRecMap.Add(bottom, top);
                                }
                                break;
                            }
                        }
                    }

                }
            }

            return sectionCounts.Sum() - sRecMap.Count;
        }

        private unsafe static int CountIslandsInSection(int* pData, int w, int h, bool normalizeTop, bool normalizeBottom)
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

            if (normalizeTop)
                Normalize(pData, w, 1, recMap);

            if (normalizeBottom)
                Normalize(pData + w * (h - 1), w, 1, recMap);

            return counter - 1 - recMap.Count;
        }

        static unsafe void Normalize(int* pData, int w, int h, Dictionary<int, int> recMap)
        {
            var ptr = pData;
            var end = ptr + w * h;
            while (ptr != end)
            {
                var v = *ptr;
                if (v > 0)
                {
                    while (recMap.TryGetValue(v, out var p))
                        v = p;
                    *ptr = v;
                }
                ptr++;
            }
        }
    }
}
