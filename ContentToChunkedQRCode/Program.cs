using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;
using QRCoder;

namespace ContentToChunkedQRCode
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("please pass the file as argument");
                return 1;
            }

            string fileName = args[0];
            string extractDirectory = Path.Combine(Path.GetDirectoryName(fileName),"extraction");
            Directory.CreateDirectory(extractDirectory);
            using (FileStream stream = File.OpenRead(fileName))
            {
                int index = 0;
                while (stream.Position<stream.Length)
                {
                    byte[] buffer = new byte[1024+ sizeof(int)];

                    int readSize = stream.Read(buffer, sizeof(int), buffer.Length- sizeof(int));
                    byte[] bufferToWrite = buffer;
                    if (readSize != buffer.Length)
                        bufferToWrite = buffer.Take(readSize).ToArray();
                    byte[] bytes = BitConverter.GetBytes(index);
                    Array.Copy(bytes,0,bufferToWrite,0, sizeof(int));
                    WriteTo(index++, extractDirectory, bufferToWrite);
                }
                WriteTo(index, extractDirectory, BitConverter.GetBytes(-1).Concat(BitConverter.GetBytes(index)).ToArray());
            }

            
            return 0;
        }

        private static void WriteTo(int index, string extractDirectory, byte[] buffer)
        {
            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(buffer, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap graphic = qrCode.GetGraphic(5);
            graphic.Save(Path.Combine(extractDirectory,$"{index}.bmp"), ImageFormat.Bmp);
        }
    }
}
