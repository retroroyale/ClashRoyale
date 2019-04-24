using System;
using System.Text;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Crypto
{
    public class Rc4
    {
        public Rc4(byte[] key)
        {
            Key = Ksa(key);
        }

        public byte[] Key { get; set; }
        public byte I { get; set; }
        public byte J { get; set; }

        public byte Prga()
        {
            I = (byte) ((I + 1) % 256);
            J = (byte) ((J + Key[I]) % 256);

            var temp = Key[I];
            Key[I] = Key[J];
            Key[J] = temp;

            return Key[(Key[I] + Key[J]) % 256];
        }

        public static byte[] Ksa(byte[] key)
        {
            var s = new byte[256];

            for (var i = 0; i != 256; i++)
                s[i] = (byte) i;

            byte j = 0;

            for (var i = 0; i != 256; i++)
            {
                j = (byte) ((j + s[i] + key[i % key.Length]) % 256);

                var temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            return s;
        }
    }

    public class Rc4Core
    {
        public Rc4Core(string key, string nonce)
        {
            InitializeCiphers(Encoding.UTF8.GetBytes(key + nonce));
        }

        public Rc4 Encryptor { get; set; }

        public Rc4 Decryptor { get; set; }

        public static byte[] GenerateNonce
        {
            get
            {
                var random = new Random();
                var buffer = new byte[random.Next(15, 25)];
                random.NextBytes(buffer);
                return buffer;
            }
        }

        public void Encrypt(ref IByteBuffer data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            for (var k = 0; k < data.ReadableBytes; k++)
            {
                var b = data.GetByte(k) ^ Encryptor.Prga();
                data.SetByte(k, b);
            }
        }

        public void Decrypt(ref IByteBuffer data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            for (var k = 0; k < data.ReadableBytes; k++)
            {
                var b = data.GetByte(k) ^ Decryptor.Prga();
                data.SetByte(k, b);
            }
        }

        public void Encrypt(ref byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            for (var k = 0; k < data.Length; k++)
                data[k] ^= Encryptor.Prga();
        }

        public void Decrypt(ref byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            for (var k = 0; k < data.Length; k++) 
                data[k] ^= Decryptor.Prga();
        }

        public void InitializeCiphers(byte[] key)
        {
            Encryptor = new Rc4(key);
            Decryptor = new Rc4(key);

            for (var k = 0; k < key.Length; k++)
            {
                Encryptor.Prga();
                Decryptor.Prga();
            }
        }
    }
}