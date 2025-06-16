using AForge.Imaging.Filters;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace nightreign_auto_storm_timer.Utils;

public static class ImageProcessor
{
    public static Bitmap ProcessImage(Bitmap image)
    {
        // Converter para escala de cinza
        Grayscale grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
        using Bitmap gray = grayscale.Apply(image);

        // Aplicar threshold
        Threshold threshold = new Threshold(200);
        threshold.ApplyInPlace(gray);

        // Inverter as cores
        Invert invert = new Invert();
        invert.ApplyInPlace(gray);

        // Aplicar filtro mediano
        Median median = new Median();
        Bitmap result = median.Apply(gray);

        return result;
    }

    public static byte[] ProcessImageToBytes(Bitmap image)
    {
        using Bitmap result = ProcessImage(image);
        return ImageToBytes(result);
    }

    public static byte[] ImageToBytes(Bitmap image)
    {
        using var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}