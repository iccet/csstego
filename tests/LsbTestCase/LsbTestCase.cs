using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using CsStg;
using Xunit;
using Xunit.Abstractions;

namespace LsbTestCase
{
    public unsafe class LsbTestCase
    {
        private readonly ITestOutputHelper _output;
        private readonly RNGCryptoServiceProvider _crypto;
        private const string Data = "test";

        public LsbTestCase(ITestOutputHelper output)
        {
            _output = output;
            _crypto = new RNGCryptoServiceProvider();
        }

        [Fact]
        public void EncodeBytesTest()
        {
            var random = new byte[100];
            _crypto.GetBytes(random);
            
            fixed (byte* p = random)
            {
                Assert.True(Lsb.Encode(Data, p, random.Length));
            }
        }
        
        [Fact]
        public void EncodeImageTest()
        {
            var bitmap = new Bitmap(10, 10);

            using var stream = new MemoryStream();
            
            bitmap.Save(stream, ImageFormat.Png);
            var bytes = stream.ToArray();
            
            fixed (byte* p = bytes)
            {
                Assert.True(Lsb.Encode(Data, p, bytes.Length));
            }
        }

        [Fact]
        public void DecodeBytesTest()
        {
            var bytes = new byte[100];
            _crypto.GetBytes(bytes);
            
            fixed (byte* p = bytes)
            {
                Assert.True(Lsb.Decode(p, bytes.Length, out var data));
                Assert.Equal(Data, Marshal.PtrToStringUTF8(data));
            }
        }
        
    }
}