using System;
using System.IO;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

class ImageSharpTest
{
    const string InputDir = "input-images";
    const string Gradient = " ._^a&#@";
    static void Main(string[] args)
    {
        System.IO.Directory.CreateDirectory("output");

        string[] inputImages = System.IO.Directory.GetFiles(InputDir);
        foreach(var imageName in inputImages)
        {
            ConvertToAscii(imageName);
        }
    }

    static void ConvertToAscii(string imageName)
    {
        string outputFile = Path.Combine("output", 
                Path.GetFileNameWithoutExtension(imageName) + ".txt");

        using(Image<Rgba32> image = Image.Load<Rgba32>(imageName))
        {
            image.Mutate(x => x.Resize(800, 800));
            var fullOutput = new StringBuilder();

            for(var y = 0; y < image.Height; ++y)
            {
                var row = new StringBuilder();
                for(var x = 0; x < image.Width; ++x)
                {
                    var pixel = image[x,y];
                    double red = pixel.R / 255.0;
                    double green = pixel.G / 255.0;
                    double blue = pixel.B / 255.0;
                    
                    
                    double luminance = 
                                RGBtoLin(red)   * 0.2126 +
                                RGBtoLin(green) * 0.7152 +
                                RGBtoLin(blue)  * 0.0722 ;
                    row.Append(
                            luminance switch
                            {
                                >= 0.0 and < 0.125 => Gradient[0] ,
                                >= 0.125 and < 0.25 => Gradient[1] ,
                                >= 0.25 and < 0.375 => Gradient[2] ,
                                >= 0.375 and < 0.5 => Gradient[3] ,
                                >= 0.5 and < 0.625 => Gradient[4] ,
                                >= 0.625 and < 0.75 => Gradient[5] ,
                                >= 0.75 and < 0.875 => Gradient[6] ,
                                _ => Gradient[7] ,
                            });
                }
                fullOutput.AppendLine(row.ToString());
            }
            File.WriteAllText(outputFile, fullOutput.ToString());
        }
        

    }
    public static double RGBtoLin(double colorChannel)
    {
        if(colorChannel <= 0.04045) return colorChannel / 12.92;

        return Math.Pow((colorChannel + 0.055)/1.055,2.4);
    }
}
