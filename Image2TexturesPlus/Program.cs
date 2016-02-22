using System;
using System.Drawing;
using System.IO;

namespace Image2TexturesPlus
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                uint processedCount = 0;
                string outputFileName = "Output.txt";

                using (StreamWriter writer = new StreamWriter(outputFileName))
                {
                    // Start from the directory of the executive.
                    processedCount = Image2Textures(writer, Environment.CurrentDirectory);
                }

                if (processedCount == 0)
                {
                    Console.WriteLine("No file processed.");
                }
                else
                {
                    Console.WriteLine(String.Format("{0} file(s) processed, file \"{1}\" output.", processedCount, outputFileName));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot create output file. Application terminated.");
            }

            Console.ReadKey();
        }

        public static uint Image2Textures(StreamWriter writer, string directory)
        {
            uint processedCount = 0;

            // Get files under current directory.
            string[] files = Directory.GetFiles(directory);
            foreach (string fileName in files)
            {
                // Is this a file with extension "png"?
                if (Path.GetExtension(fileName) == ".png")
                {
                    try
                    {
                        // Load image.
                        Image image = Image.FromFile(fileName);

                        // make it a bitmap
                        Bitmap bmp = new Bitmap(image);

                        // Output information to TEXTURES lump.
                        string lumpName = Path.GetFileNameWithoutExtension(fileName);
                        writer.WriteLine(String.Format("texture {0}, {1}, {2}", lumpName, image.Width, image.Height));
                        writer.WriteLine("{");
                        writer.WriteLine("offset 0, 0");

                        // go through all pixels!
                        for (int y = 0; y < image.Width; y++)
                        {
                            for (int x = 0; x < image.Height; x++)
                            {
                                // get color of the pixel
                                Color clr = bmp.GetPixel(x, y);
                                int r = clr.R, g = clr.G, b = clr.B;

                                writer.WriteLine(String.Format("patch pixel, {0}, {1} {{ translation \"0:255=[{2},{3},{4}]:[{2},{3},{4}]\" }}", x, y, r, g, b));
                            }
                        }

                        writer.WriteLine("}");
                        writer.WriteLine(""); // Pad one more line.

                        // Free image.
                        image.Dispose();
                        bmp.Dispose();

                        processedCount++;
                    }
                    catch(Exception)
                    {
                        // Fail to process this image.
                        // We do nothing.
                    }
                }
            }

            // Get directories under current directory.
            string[] directories = Directory.GetDirectories(directory);
            foreach (string dir in directories)
            {
                // Process through subfolders.
                processedCount += Image2Textures(writer, dir);
            }

            return processedCount;
        }
    }
}
