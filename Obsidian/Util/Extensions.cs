﻿using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Obsidian.Util
{
    public static class Extensions
    {
        //this is for ints
        public static int GetUnsignedRightShift(this int value, int s) => value >> s;

        //this is for longs

        public static long GetUnsignedRightShift(this long value, int s)
        {
            return (long)((ulong)value >> s);
        }

        public static string Capitalize(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static int GetVarIntLength(this int val)
        {
            int amount = 0;
            do
            {
                var temp = (sbyte)(val & 0b01111111);
                // Note: >>> means that the sign bit is shifted with the rest of the number rather than being left alone
                val >>= 7;
                if (val != 0)
                {
                    temp |= 127;
                }
                amount++;
            } while (val != 0);
            return amount;
        }

        //https://gist.github.com/ammaraskar/7b4a3f73bee9dc4136539644a0f27e63
        public static string MinecraftShaDigest(this byte[] data)
        {
            var hash = new SHA1Managed().ComputeHash(data);
            // Reverse the bytes since BigInteger uses little endian
            Array.Reverse(hash);

            var b = new BigInteger(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            if (b < 0)
            {
                // toss in a negative sign if the interpreted number is negative
                return "-" + (-b).ToString("x").TrimStart('0');
            }
            else
            {
                return b.ToString("x").TrimStart('0');
            }
        }
    }
}