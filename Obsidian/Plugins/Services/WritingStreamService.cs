﻿using System.IO;
using System.Threading.Tasks;

namespace Obsidian.Plugins.Services
{
    public class WritingStreamService : StreamService
    {
        private StreamWriter writer;

        public WritingStreamService(FileStream fileStream)
        {
            stream = fileStream;
            writer = new StreamWriter(fileStream);
            name = fileStream.Name;
        }

        public WritingStreamService(Stream stream)
        {
            this.stream = stream;
            writer = new StreamWriter(stream);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count)
        {
            return stream.WriteAsync(buffer, offset, count);
        }

        public override void Write(object value)
        {
            writer.Write(value);
        }

        public override Task WriteAsync(object value)
        {
            return writer.WriteAsync(value.ToString());
        }

        public override void Write(string value)
        {
            writer.Write(value);
        }

        public override Task WriteAsync(string value)
        {
            return writer.WriteAsync(value);
        }

        public override void WriteByte(byte value)
        {
            writer.Write(value);
        }

        public override void WriteLine(object value)
        {
            writer.WriteLine(value);
        }

        public override Task WriteLineAsync(object value)
        {
            return writer.WriteLineAsync(value.ToString());
        }

        public override void WriteLine(string value)
        {
            writer.WriteLine(value);
        }

        public override Task WriteLineAsync(string value)
        {
            return writer.WriteLineAsync(value);
        }

        public override void Flush()
        {
            writer.Flush();
        }

        public override Task FlushAsync()
        {
            return writer.FlushAsync();
        }

        public override void Dispose()
        {
            writer.Dispose();
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            writer.Dispose();
            return base.DisposeAsync();
        }
    }
}
