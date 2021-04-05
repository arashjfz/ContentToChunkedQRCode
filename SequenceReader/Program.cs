using System;

namespace SequenceReader
{
    class Program
    {
        static void Main(string[] args)
        {
            FrameLoader frameLoader = new FrameLoader();
            frameLoader.Initialize();
            FrameManipulator frameManipulator = new FrameManipulator(frameLoader,@"c:\file.bin");
            frameManipulator.Initialize();
            Console.ReadLine();
        }

    }
}
