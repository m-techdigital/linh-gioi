using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace LinhGioi.UI
{
    internal static class RuntimePngWriter
    {
        public static int WriteRgbTexture(string path, Texture2D texture)
        {
            WritePng(path, texture.GetPixels32(), texture.width, texture.height);
            return File.Exists(path) ? checked((int)new FileInfo(path).Length) : 0;
        }

        private static void WritePng(string path, Color32[] pixels, int width, int height)
        {
            using (var output = File.Create(path))
            {
                output.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, 0, 8);
                var ihdr = new byte[13];
                WriteInt(ihdr, 0, width);
                WriteInt(ihdr, 4, height);
                ihdr[8] = 8;
                ihdr[9] = 6;
                WriteChunk(output, "IHDR", ihdr);
                WriteChunk(output, "IDAT", BuildZlibRgba(pixels, width, height));
                WriteChunk(output, "IEND", Array.Empty<byte>());
            }
        }

        private static byte[] BuildZlibRgba(Color32[] pixels, int width, int height)
        {
            var stride = width * 4 + 1;
            var raw = new byte[stride * height];
            var offset = 0;
            for (var y = height - 1; y >= 0; y--)
            {
                raw[offset++] = 0;
                for (var x = 0; x < width; x++)
                {
                    var pixel = pixels[y * width + x];
                    raw[offset++] = pixel.r;
                    raw[offset++] = pixel.g;
                    raw[offset++] = pixel.b;
                    raw[offset++] = pixel.a;
                }
            }

            var adler = Adler32(raw);
            using (var stream = new MemoryStream())
            {
                stream.WriteByte(0x78);
                stream.WriteByte(0x9c);
                using (var deflate = new DeflateStream(stream, System.IO.Compression.CompressionLevel.Fastest, true))
                {
                    deflate.Write(raw, 0, raw.Length);
                }
                WriteUInt(stream, adler);
                return stream.ToArray();
            }
        }

        private static uint Adler32(byte[] data)
        {
            const uint mod = 65521;
            uint a = 1;
            uint b = 0;
            foreach (var value in data)
            {
                a = (a + value) % mod;
                b = (b + a) % mod;
            }
            return (b << 16) | a;
        }

        private static void WriteChunk(Stream output, string type, byte[] data)
        {
            var typeBytes = Encoding.ASCII.GetBytes(type);
            WriteUInt(output, (uint)data.Length);
            output.Write(typeBytes, 0, typeBytes.Length);
            output.Write(data, 0, data.Length);
            var crcInput = new byte[typeBytes.Length + data.Length];
            Buffer.BlockCopy(typeBytes, 0, crcInput, 0, typeBytes.Length);
            Buffer.BlockCopy(data, 0, crcInput, typeBytes.Length, data.Length);
            WriteUInt(output, Crc32(crcInput));
        }

        private static uint Crc32(byte[] data)
        {
            uint crc = 0xffffffff;
            foreach (var value in data)
            {
                crc ^= value;
                for (var i = 0; i < 8; i++)
                    crc = (crc & 1) != 0 ? 0xedb88320 ^ (crc >> 1) : crc >> 1;
            }
            return crc ^ 0xffffffff;
        }

        private static void WriteInt(byte[] data, int offset, int value)
        {
            data[offset] = (byte)((value >> 24) & 0xff);
            data[offset + 1] = (byte)((value >> 16) & 0xff);
            data[offset + 2] = (byte)((value >> 8) & 0xff);
            data[offset + 3] = (byte)(value & 0xff);
        }

        private static void WriteUInt(Stream output, uint value)
        {
            output.WriteByte((byte)((value >> 24) & 0xff));
            output.WriteByte((byte)((value >> 16) & 0xff));
            output.WriteByte((byte)((value >> 8) & 0xff));
            output.WriteByte((byte)(value & 0xff));
        }
    }
}
