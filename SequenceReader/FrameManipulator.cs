using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;

namespace SequenceReader
{
    public class FrameManipulator
    {
        private readonly IFrameSource _frameSource;
        private readonly string _filename;
        private ConcurrentDictionary<int,Frame> _frames = new ConcurrentDictionary<int, Frame>();
        public FrameManipulator(IFrameSource frameSource,string filename)
        {
            _frameSource = frameSource;
            _filename = filename;
        }

        public void Initialize()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                while (true)
                {
                    QRCodeDecoder decoder = new QRCodeDecoder();
                    Frame frame = _frameSource.Pick();
                    if (frame == null)
                        continue;
                    byte[] decodeBytes = null;
                    try
                    {
                        decodeBytes = decoder.DecodeBytes(new QRCodeBitmapImage(frame.RawData)).Select(s => (byte)s).ToArray();
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    var index = BitConverter.ToInt32(decodeBytes, 0);
                    frame.Content = decodeBytes.Skip(sizeof(int)).ToArray();

                    _frames.TryAdd(index, frame);
                    TryFinalize();

                    Thread.Sleep(100);
                }
            });
        }

        private void TryFinalize()
        {
            if (_frames.ContainsKey(-1))
            {
                var size = BitConverter.ToInt32(_frames[-1].Content, 0);
                for (int i = 0; i < size; i++)
                {
                    if (!_frames.TryGetValue(i, out Frame frame))
                    {
                        Console.WriteLine("Lost Frame system is waiting for retrying");
                        _frames.TryRemove(-1, out Frame _);
                        return;
                    }
                }

                using FileStream fileStream = File.Create(_filename);
                for (int i = 0; i < size; i++)
                    fileStream.Write(_frames[i].Content,0, _frames[i].Content.Length);
            }
        }
    }
}