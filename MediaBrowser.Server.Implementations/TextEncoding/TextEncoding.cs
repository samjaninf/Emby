﻿using System.Text;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.TextEncoding;

namespace MediaBrowser.Server.Implementations.TextEncoding
{
    public class TextEncoding : IEncoding
    {
        private readonly IFileSystem _fileSystem;

        public TextEncoding(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public byte[] GetASCIIBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        public string GetASCIIString(byte[] bytes, int startIndex, int length)
        {
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public Encoding GetFileEncoding(string srcFile)
        {
            // *** Detect byte order mark if any - otherwise assume default
            var buffer = new byte[5];

            using (var file = _fileSystem.OpenRead(srcFile))
            {
                file.Read(buffer, 0, 5);
            }

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                return Encoding.UTF8;
            if (buffer[0] == 0xfe && buffer[1] == 0xff)
                return Encoding.Unicode;
            if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                return Encoding.UTF32;
            if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                return Encoding.UTF7;

            // It's ok - anything aside from utf is ok since that's what we're looking for
            return Encoding.Default;
        }
    }
}
