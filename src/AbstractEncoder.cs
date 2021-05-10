using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace CsStg
{
    public abstract class AbstractEncoder
    {
        protected const string CsStgLib =
#if WINDOWS
    "libCsStg.dll"
#else
    "libCsStg.so"
#endif
    ;

        protected abstract unsafe bool Encode(string data, byte *container, int size);

        protected abstract unsafe int Decode(byte *container, int size, out IntPtr data);

        public unsafe string Decode(byte[] container)
        {
            fixed (byte* p = container)
            {
                _ = Decode(p, container.Length, out var data);
                return Marshal.PtrToStringAnsi(data);
            }
        }

        public unsafe bool Encode(string data, byte[] container)
        {
            fixed (byte* p = container)
            {
                return Encode(data, p, container.Length);
            }
        }
        
        public string Decode(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bmpData = bitmap.LockBits(
                rect, 
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            var ptr = bmpData.Scan0;
            var length = bmpData.Stride * bitmap.Height;
            var bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);

            var decoded = Decode(bytes);

            bitmap.UnlockBits(bmpData);
            return decoded;
        }

        public bool Encode(string data, Bitmap bitmap, MemoryStream stream)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bmpData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            var ptr = bmpData.Scan0;
            var length = bmpData.Stride * bitmap.Height;
            var bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);

            var success = Encode(data, bytes);

            Marshal.Copy(bytes, 0, ptr, length);
            bitmap.UnlockBits(bmpData);
            bitmap.Save(stream, bitmap.RawFormat);
            stream.Position = 0;
            
            return success;
        }

    }
}