using Svg;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ekona.Helper
{
    public class SVGLoader
    {
        static float screenDPI = 0.0f;

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static float GetScreenDpi()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            int dpi = GetDeviceCaps(hdc, 88);
            ReleaseDC(IntPtr.Zero, hdc);
            return dpi == 0 ? 96f : (float)dpi;
        }

        private static Icon BitmapToIcon(Bitmap bitmap)
        {
            IntPtr hIcon = bitmap.GetHicon();
            try
            {
                Icon icon = Icon.FromHandle(hIcon);
                return (Icon)icon.Clone();
            }
            finally
            {
                DestroyIcon(hIcon);
            }
        }

        public static Bitmap LoadSvgFromStream(Stream s, int width, int height)
        {
            SvgDocument svgDoc = SvgDocument.Open<SvgDocument>(s);
            Bitmap bmp = svgDoc.Draw(width, height);
            return bmp;
        }

        public static Bitmap LoadSvg(string svgName, int baseSize)
        {
            if (screenDPI == 0.0f) screenDPI = GetScreenDpi();

            float scaleFactor = screenDPI / 96f;
            int iconSize = (int)(baseSize * scaleFactor);

            string resourceName = "Ekona.Icons." + svgName + ".svg";
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                return LoadSvgFromStream(stream, iconSize, iconSize);
            }
        }

        public static Icon LoadSvgToIcon(string svgName, int baseSize)
        {
            return BitmapToIcon(LoadSvg(svgName, baseSize));
        }

        public static int GetRealIconSize(int baseSize)
        {
            if (screenDPI == 0.0f) screenDPI = GetScreenDpi();

            float scaleFactor = screenDPI / 96f;
            int iconSize = (int)(baseSize * scaleFactor);

            return iconSize;
        }
    }
}
