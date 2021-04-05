using System.Drawing;

namespace SequenceReader
{
    public class Frame
    {
        public double GrayRatio { get; set; }
        public int DetectionCount { get; set; }
        public Bitmap RawData { get; set; }
        public byte[] Content { get; set; }
    }
}