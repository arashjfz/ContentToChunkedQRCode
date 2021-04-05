using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;

namespace SequenceReader
{
    public class FrameLoader : IFrameSource
    {
        private readonly ConcurrentQueue<Frame> _frames = new ConcurrentQueue<Frame>();

        public void Initialize()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {

                bool frameDetected = false;
                while (true)
                {
                    var bitmap = new Bitmap(1920, 1080);
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(0, 0, 0, 0,
                            bitmap.Size, CopyPixelOperation.SourceCopy);
                    }

                    long total = 0;
                    long size = 0;
                    for (int x = 0; x < bitmap.Width; x += 20)
                    {
                        for (int y = 0; y < bitmap.Height; y += 20)
                        {
                            Color color = bitmap.GetPixel(x, y);
                            size++;
                            total += Math.Abs(color.R - color.G);
                        }
                    }

                    double average = (double)total / size;
                    if (average > 0.1)
                    {
                        frameDetected = false;
                        Console.WriteLine($"black skip:{average}");
                        continue;
                    }

                    if (frameDetected)
                    {
                        continue;
                    }
                    _frames.Enqueue(new Frame { RawData = bitmap, GrayRatio = average });
                    frameDetected = true;

                    Thread.Sleep(100);
                }
            });
        }

        public Frame Pick()
        {
            _frames.TryDequeue(out Frame frame);
            return frame;
        }
    }
}