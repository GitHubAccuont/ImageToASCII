using System.IO;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageToASCII.Services
{
    public class ImageConverter
    {
        private string asciiCharacters="";


        public string ResizeImage(Stream imageStream, int rows, int method)
        {
            if (method == 1) asciiCharacters = " .:-=+*#%@";
            else asciiCharacters = " .'`^\",:;Il!i><~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";

            using (var image = Image.Load<Rgb24>(imageStream))
            {
                // Calculate the ratio of the original width to the desired rows
                float aspectRatio = (int)((double)image.Width / (double)image.Height) ;
                int columns = (int)(aspectRatio * 2 * rows);

                // Resize the image to the specified size
                image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(columns, rows) }));

                // Convert the image to grayscale
                image.Mutate(x => x.Grayscale());

                // Convert the image to ASCII representation
                if (rows == 0 || columns == 0 || image == null) return "Null exception Error";
                string asciiRepresentation = ConvertToAscii(image, rows, columns);

                return asciiRepresentation;
            }
        }

        private string ConvertToAscii(Image<Rgb24> image, int rows, int columns)
        {
            var asciiBuilder = new StringBuilder();

            // Calculate the width and height ratios for mapping pixels to ASCII characters
            int widthRatio = image.Width / columns;
            int heightRatio = image.Height / rows;

            // Iterate over the rows and columns of the image
            for (int y = 0; y < image.Height; y += heightRatio)
            {
                for (int x = 0; x < image.Width; x += widthRatio)
                {
                    // Get the pixel at the current position
                    var pixel = image[x, y];

                    // Calculate the average intensity of the RGB values
                    int intensity = (pixel.R + pixel.G + pixel.B) / 3;

                    // Map the intensity value to an ASCII character
                    char asciiCharacter = IntensityToAscii(intensity);

                    // Append the ASCII character to the string builder
                    asciiBuilder.Append(asciiCharacter);
                }

                // Append a new line character to start a new row
                asciiBuilder.AppendLine();
            }

            return asciiBuilder.ToString();
        }



        private char IntensityToAscii(int intensity)
        {
            // Scale the intensity to the range of the ASCII characters
            int scaledIntensity = (intensity * (asciiCharacters.Length - 1)) / 255;

            // Return the corresponding ASCII character
            return asciiCharacters[scaledIntensity];
        }
    }
}
