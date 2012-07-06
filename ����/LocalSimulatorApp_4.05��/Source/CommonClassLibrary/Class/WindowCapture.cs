using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SymbolLibrary
{
    /// <summary>
    /// GetSystemMetrics関数のフラグ
    /// </summary>
    enum GetMetrics : uint
    {
        SM_CXSCREEN = 0x00000000,
        SM_CYSCREEN = 0x00000001,

    }

    /// <summary>
    /// BitBlt関数のフラグ
    /// </summary>
    enum RasterOperation : uint
    {
        SRCCOPY     = 0x00CC0020,
        SRCPAINT    = 0x00EE0086,
        SRCAND      = 0x008800C6,
        SRCINVERT   = 0x00660046,
        SRCERASE    = 0x00440328,
        NOTSRCCOPY  = 0x00330008,
        NOTSRCERASE = 0x001100A6,
        MERGECOPY   = 0x00C000CA,
        MERGEPAINT  = 0x00BB0226,
        PATCOPY     = 0x00F00021,
        PATPAINT    = 0x00FB0A09,
        PATINVERT   = 0x005A0049,
        DSTINVERT   = 0x00550009,
        BLACKNESS   = 0x00000042,
        WHITENESS   = 0x00FF0062,
    }

    public class GraphicsAPI
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDst, int xDst, int yDst,
            int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, int rasterOp);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobjBmp);

        [DllImport("gdi32.dll")]
        public static extern Int32 GetPixel(IntPtr hdc, int XPos, int YPos);


        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

    }

    public class WindowCapture
    {
        /// <summary>
        /// 画面全体のビットマップを取得
        /// </summary>
        /// <returns>画面全体のビットマップ</returns>
        public static Bitmap GetDesktop()
        {
            int screenX;
            int screenY;
            IntPtr hBmp;
            IntPtr hdcScreen = GraphicsAPI.GetDC(GraphicsAPI.GetDesktopWindow());
            IntPtr hdcCompatible = GraphicsAPI.CreateCompatibleDC(hdcScreen);

            screenX = GraphicsAPI.GetSystemMetrics((int)GetMetrics.SM_CXSCREEN);
            screenY = GraphicsAPI.GetSystemMetrics((int)GetMetrics.SM_CYSCREEN);
            hBmp = GraphicsAPI.CreateCompatibleBitmap(hdcScreen, screenX, screenY);

            if (hBmp != IntPtr.Zero)
            {
                IntPtr hOldBmp = (IntPtr)GraphicsAPI.SelectObject(hdcCompatible, hBmp);
                GraphicsAPI.BitBlt(hdcCompatible, 0, 0, screenX, screenY,
                                    hdcScreen, 0, 0, (int)RasterOperation.SRCCOPY);

                GraphicsAPI.SelectObject(hdcCompatible, hOldBmp);
                GraphicsAPI.DeleteDC(hdcCompatible);
                GraphicsAPI.ReleaseDC(GraphicsAPI.GetDesktopWindow(), hdcScreen);

                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBmp);

                GraphicsAPI.DeleteObject(hBmp);
                GC.Collect();

                return bmp;
            }

            return null;
        }

        /// <summary>
        /// 指定ダイアログのビットマップを取得
        /// </summary>
        /// <param name="captureForm">ビットマップを取得するダイアログ</param>
        /// <returns>指定ダイアログのビットマップ</returns>
        public static Bitmap GetForm(Form captureForm)
        {
            if (captureForm == null) return null;

            int screenX;
            int screenY;
            IntPtr hBmp;
            IntPtr hdcScreen = GraphicsAPI.GetDC(captureForm.Handle);
            IntPtr hdcCompatible = GraphicsAPI.CreateCompatibleDC(hdcScreen);

            screenX = captureForm.ClientSize.Width;
            screenY = captureForm.ClientSize.Height;
            hBmp = GraphicsAPI.CreateCompatibleBitmap(hdcScreen, screenX, screenY);

            if (hBmp != IntPtr.Zero)
            {
                IntPtr hOldBmp = (IntPtr)GraphicsAPI.SelectObject(hdcCompatible, hBmp);
                GraphicsAPI.BitBlt(hdcCompatible, 0, 0, screenX, screenY,
                                        hdcScreen, 0, 0, (int)RasterOperation.SRCCOPY);

                GraphicsAPI.SelectObject(hdcCompatible, hOldBmp);
                GraphicsAPI.DeleteDC(hdcCompatible);
                GraphicsAPI.ReleaseDC(GraphicsAPI.GetDesktopWindow(), hdcScreen);

                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBmp);

                GraphicsAPI.DeleteObject(hBmp);
                GC.Collect();

                return bmp;
            }

            return null;
        }

        /// <summary>
        /// 指定ピクセルの色を取得 /* 未実装 */
        /// </summary>
        /// <param name="xPos">X座標(グローバル座標)</param>
        /// <param name="yPos">Y座標(クライアント座標)</param>
        /// <returns>指定ピクセルの色</returns>
        public static Color GetPixel(int xPos, int yPos)
        {
            return GetPixel(GraphicsAPI.GetDesktopWindow(), xPos, yPos);
        }

        /// <summary>
        /// 指定ピクセルの色を取得
        /// </summary>
        /// <param name="captureForm">対象のダイアログ</param>
        /// <param name="xPos">X座標(クライアント座標)</param>
        /// <param name="yPos">Y座標(クライアント座標)</param>
        /// <returns>指定ピクセルの色</returns>
        public static Color GetPixel(Form captureForm, int xPos, int yPos)
        {
            return GetPixel(captureForm.Handle, xPos, yPos);
        }

        /// <summary>
        /// 指定ピクセルの色を取得
        /// </summary>
        /// <param name="capFormHandle">対象ダイアログのハンドル</param>
        /// <param name="xPos">X座標(クライアント座標)</param>
        /// <param name="yPos">Y座標(クライアント座標)</param>
        /// <returns>指定ピクセルの色</returns>
        public static Color GetPixel(IntPtr capFormHandle, int xPos, int yPos)
        {
            IntPtr hdcScreen = GraphicsAPI.GetDC(capFormHandle);
            int scrColor = GraphicsAPI.GetPixel(hdcScreen, xPos, yPos);

            GraphicsAPI.ReleaseDC(capFormHandle, hdcScreen);

            return System.Drawing.ColorTranslator.FromOle(scrColor);
        }


    }

}
