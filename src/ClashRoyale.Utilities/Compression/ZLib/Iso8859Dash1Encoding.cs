using System;
using System.Text;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    public class Iso8859Dash1Encoding : Encoding
    {
        public static int CharacterCount => 256;

        public override string WebName => "iso-8859-1";

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        public override int GetBytes(char[] chars, int start, int count, byte[] bytes, int byteIndex)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars), "null array");

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes), "null array");

            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (chars.Length - start < count)
                throw new ArgumentOutOfRangeException(nameof(chars));

            if (byteIndex < 0 || byteIndex > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteIndex));

            for (var i = 0; i < count; i++)
            {
                var c = chars[start + i]; 

                if (c >= '\x00FF') 
                    bytes[byteIndex + i] = (byte) '?';
                else
                    bytes[byteIndex + i] = (byte) c;
            }

            return count;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        public override int GetChars(byte[] bytes, int start, int count, char[] chars, int charIndex)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars), "null array");

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes), "null array");

            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (bytes.Length - start < count)
                throw new ArgumentOutOfRangeException(nameof(bytes));

            if (charIndex < 0 || charIndex > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charIndex));

            for (var i = 0; i < count; i++)
                chars[charIndex + i] = (char) bytes[i + start];

            return count;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }
    }
}