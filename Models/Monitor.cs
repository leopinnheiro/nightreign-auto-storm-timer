using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace nightreign_auto_storm_timer.Models;

public partial class Monitor
{
    public string Name { get; }
    public string DeviceName { get; }
    public Screen Screen { get; }
    public ImageSource Thumbnail { get; }
    public bool IsPrimary => Screen.Primary;

    public Monitor(Screen screen)
    {
        Screen = screen;
        DeviceName = screen.DeviceName;
        Name = $"{DeviceName} ({screen.Bounds.Width}x{screen.Bounds.Height})";
        Thumbnail = CaptureMonitorThumbnail(screen);
    }

    private ImageSource CaptureMonitorThumbnail(Screen screen)
    {
        // Captura o monitor inteiro
        using var bmp = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
        using var g = Graphics.FromImage(bmp);
        g.CopyFromScreen(screen.Bounds.Location, System.Drawing.Point.Empty, screen.Bounds.Size);

        // Calcula proporções
        double targetAspect = 16.0 / 9.0;
        double originalAspect = (double)bmp.Width / bmp.Height;

        int finalWidth = 300;
        int finalHeight = 169;

        // Cria imagem final 300x169 com fundo preto
        using var finalImage = new Bitmap(finalWidth, finalHeight);
        using var finalGraphics = Graphics.FromImage(finalImage);
        finalGraphics.Clear(Color.Black);

        // Calcula tamanho da imagem encaixada mantendo aspecto original
        int drawWidth, drawHeight;
        if (originalAspect > targetAspect)
        {
            // Imagem mais larga que 16:9 → ajusta largura
            drawWidth = finalWidth;
            drawHeight = (int)(finalWidth / originalAspect);
        }
        else
        {
            // Imagem mais alta que 16:9 → ajusta altura
            drawHeight = finalHeight;
            drawWidth = (int)(finalHeight * originalAspect);
        }

        // Centraliza a imagem com letterbox
        int offsetX = (finalWidth - drawWidth) / 2;
        int offsetY = (finalHeight - drawHeight) / 2;

        finalGraphics.DrawImage(bmp, offsetX, offsetY, drawWidth, drawHeight);

        // Converte para ImageSource
        using var ms = new MemoryStream();
        finalImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        ms.Position = 0;

        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = ms;
        image.EndInit();
        image.Freeze();

        return image;
    }

}