using nightreign_auto_storm_timer.Services;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Direct3D11 = SharpDX.Direct3D11;

namespace nightreign_auto_storm_timer.Utils;

public class ScreenCapturerSharpDX : IDisposable
{
    private readonly Direct3D11.Device _device;
    private readonly OutputDuplication _duplicator;
    private readonly Texture2DDescription _desc;
    private readonly Texture2D _stagingTex;

    public ScreenCapturerSharpDX(int monitorIndex = 0)
    {
        var factory = new Factory1();
        var adapter = factory.GetAdapter1(monitorIndex);
        var output = adapter.GetOutput(monitorIndex).QueryInterface<Output1>();

        _device = new Direct3D11.Device(adapter);
        _duplicator = output.DuplicateOutput(_device);

        var bounds = output.Description.DesktopBounds;
        _desc = new Texture2DDescription
        {
            Width = bounds.Right - bounds.Left,
            Height = bounds.Bottom - bounds.Top,
            Format = Format.B8G8R8A8_UNorm,
            ArraySize = 1,
            MipLevels = 1,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Staging,
            BindFlags = BindFlags.None,
            CpuAccessFlags = CpuAccessFlags.Read,
            OptionFlags = ResourceOptionFlags.None
        };

        _stagingTex = new Texture2D(_device, _desc);
    }

    public Bitmap Capture(Rectangle region)
    {
        try
        {
            _duplicator.TryAcquireNextFrame(1000, out var info, out var screenResource);
            using (screenResource)
            using (var screenTex = screenResource.QueryInterface<Texture2D>())
            {
                _device.ImmediateContext.CopyResource(screenTex, _stagingTex);

                var dataBox = _device.ImmediateContext.MapSubresource(
                    _stagingTex, 0, MapMode.Read, Direct3D11.MapFlags.None);

                Bitmap bitmap = new(region.Width, region.Height, PixelFormat.Format32bppArgb);

                for (int y = 0; y < region.Height; y++)
                {
                    int sourceOffset = (region.Y + y) * dataBox.RowPitch + region.X * 4;
                    nint srcPtr = dataBox.DataPointer + sourceOffset;

                    var bmpData = bitmap.LockBits(
                        new Rectangle(0, y, region.Width, 1),
                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                    int byteCount = region.Width * 4;
                    byte[] buffer = new byte[byteCount];

                    Marshal.Copy(srcPtr, buffer, 0, byteCount);
                    Marshal.Copy(buffer, 0, bmpData.Scan0, byteCount);

                    bitmap.UnlockBits(bmpData);
                }

                _device.ImmediateContext.UnmapSubresource(_stagingTex, 0);
                _duplicator.ReleaseFrame();
                return bitmap;
            }
        }
        catch (Exception ex)
        {
            LogService.Exception(ex, "[ScreenCapturerSharpDX] -> Capture");
        }

        // Fallback
        Bitmap bmp = new Bitmap(100, 100);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Black);
        }
        return bmp;
    }

    public void Dispose()
    {
        _duplicator.Dispose();
        _stagingTex.Dispose();
        _device.Dispose();
    }
}
