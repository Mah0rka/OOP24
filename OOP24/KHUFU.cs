using System;

namespace OOP24
{
    internal class Khufu
    {
        private const int BLOCK_SIZE = 64; // Розмір блоку в бітах
        private const int ROUNDS = 16;     // Кількість раундів
        private readonly byte[] SBox = new byte[256];
        private readonly ulong[] KeySchedule;

        public Khufu(byte[] key)
        {
            KeySchedule = GenerateKeySchedule(key);
            InitializeSBox();
        }

        private void InitializeSBox()
        {
            // Ініціалізація S-Box деякою перестановкою, наприклад, ідентичною перестановкою
            for (int i = 0; i < 256; i++)
            {
                SBox[i] = (byte)i;
            }
        }

        private ulong[] GenerateKeySchedule(byte[] key)
        {
            ulong[] schedule = new ulong[ROUNDS];
            // Логіка генерації розкладу ключів
            // Для спрощення, використовуємо безпосередній ключовий матеріал. У реальному світі розклад ключів був би складнішим.
            Buffer.BlockCopy(key, 0, schedule, 0, Math.Min(key.Length, ROUNDS * sizeof(ulong)));
            return schedule;
        }

        private ulong Substitute(ulong value)
        {
            ulong substituted = 0;
            for (int i = 0; i < 8; i++)
            {
                byte b = (byte)((value >> (i * 8)) & 0xFF);
                substituted |= ((ulong)SBox[b]) << (i * 8);
            }
            return substituted;
        }

        private ulong FeistelFunction(ulong halfBlock, ulong subkey)
        {
            return Substitute(halfBlock ^ subkey);
        }

        public ulong EncryptBlock(ulong block)
        {
            ulong left = block >> 32;
            ulong right = block & 0xFFFFFFFF;

            for (int i = 0; i < ROUNDS; i++)
            {
                ulong temp = right;
                right = left ^ FeistelFunction(right, KeySchedule[i]);
                left = temp;
            }

            return (right << 32) | left; // Комбінуємо дві половини
        }

        private byte[] ApplyPadding(byte[] plaintext)
        {
            int paddingSize = (BLOCK_SIZE / 8) - (plaintext.Length % (BLOCK_SIZE / 8));
            byte[] paddedPlaintext = new byte[plaintext.Length + paddingSize];
            Array.Copy(plaintext, paddedPlaintext, plaintext.Length);
            for (int i = plaintext.Length; i < paddedPlaintext.Length; i++)
            {
                paddedPlaintext[i] = (byte)paddingSize;
            }
            return paddedPlaintext;
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            byte[] paddedPlaintext = ApplyPadding(plaintext);
            int blockCount = paddedPlaintext.Length / (BLOCK_SIZE / 8);
            byte[] ciphertext = new byte[blockCount * (BLOCK_SIZE / 8)];

            for (int i = 0; i < blockCount; i++)
            {
                ulong block = BitConverter.ToUInt64(paddedPlaintext, i * (BLOCK_SIZE / 8));
                ulong encryptedBlock = EncryptBlock(block);
                byte[] encryptedBytes = BitConverter.GetBytes(encryptedBlock);
                Array.Copy(encryptedBytes, 0, ciphertext, i * (BLOCK_SIZE / 8), BLOCK_SIZE / 8);
            }

            return ciphertext;
        }
    }
}

