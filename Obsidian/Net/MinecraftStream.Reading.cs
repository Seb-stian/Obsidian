﻿using System;
using System.Text;
using System.Threading.Tasks;

namespace Obsidian.Net
{
    public partial class MinecraftStream
    {
		public async Task<sbyte> ReadByteAsync() => (sbyte)await this.ReadUnsignedByteAsync();

		public async Task<byte> ReadUnsignedByteAsync()
		{
			var buffer = new byte[1];
			await this.ReadAsync(buffer);
			return buffer[0];
		}

		public async Task<bool> ReadBooleanAsync()
		{
			var value = (int)await this.ReadByteAsync();
			if (value == 0x00)
			{
				return false;
			}
			else if (value == 0x01)
			{
				return true;
			}
			else
			{
				throw new ArgumentOutOfRangeException("Byte returned by stream is out of range (0x00 or 0x01)", nameof(BaseStream));
			}
		}

		public async Task<ushort> ReadUnsignedShortAsync()
		{
			var buffer = new byte[2];
			await this.ReadAsync(buffer);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			return BitConverter.ToUInt16(buffer);
		}

		public async Task<short> ReadShortAsync()
		{
			var buffer = new byte[2];
			await this.ReadAsync(buffer);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			return BitConverter.ToInt16(buffer);
		}

		public async Task<int> ReadIntAsync()
		{
			var buffer = new byte[4];
			await this.ReadAsync(buffer);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			return BitConverter.ToInt32(buffer);
		}

		public async Task<long> ReadLongAsync()
		{
			var buffer = new byte[8];
			await this.ReadAsync(buffer);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			return BitConverter.ToInt64(buffer);
		}

		public async Task<ulong> ReadUnsignedLongAsync()
		{
			var buffer = new byte[8];
			await this.ReadAsync(buffer);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			return BitConverter.ToUInt64(buffer);
		}

		public async Task<float> ReadFloatAsync()
		{
			var buffer = new byte[4];
			await this.ReadAsync(buffer);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			return BitConverter.ToSingle(buffer);
		}

		public async Task<double> ReadDoubleAsync()
		{
			var buffer = new byte[8];
			await this.ReadAsync(buffer);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			return BitConverter.ToDouble(buffer);
		}

		public async Task<string> ReadStringAsync(int maxLength = 0)
		{
			var length = await this.ReadVarIntAsync();
			var buffer = new byte[length];
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
			await this.ReadAsync(buffer, 0, length);

			var value = Encoding.UTF8.GetString(buffer);
			if (maxLength > 0 && value.Length > maxLength)
			{
				throw new ArgumentException($"string ({value.Length}) exceeded maximum length ({maxLength})", nameof(value));
			}
			return value;
		}

		public virtual async Task<int> ReadVarIntAsync()
		{
			int numRead = 0;
			int result = 0;
			byte read;
			do
			{
				read = await this.ReadUnsignedByteAsync();
				int value = read & 0b01111111;
				result |= value << (7 * numRead);

				numRead++;
				if (numRead > 5)
				{
					throw new InvalidOperationException("VarInt is too big");
				}
			} while ((read & 0b10000000) != 0);

			return result;
		}

		public async Task<byte[]> ReadUInt8ArrayAsync(int length)
		{
			var result = new byte[length];
			if (length == 0) return result;
			int n = length;
			while (true)
			{
				n -= await this.ReadAsync(result, length - n, n);
				if (n == 0)
					break;
				await Task.Delay(1);
			}
			return result;
		}

		public async Task<byte> ReadUInt8Async()
		{
			int value = await this.ReadByteAsync();
			if (value == -1)
				throw new EndOfStreamException();
			return (byte)value;
		}

		public async Task<long> ReadVarLongAsync()
		{
			int numRead = 0;
			long result = 0;
			byte read;
			do
			{
				read = await this.ReadUnsignedByteAsync();
				int value = (read & 0b01111111);
				result |= (long)value << (7 * numRead);

				numRead++;
				if (numRead > 10)
				{
					throw new InvalidOperationException("VarLong is too big");
				}
			} while ((read & 0b10000000) != 0);

			return result;
		}
	}
}