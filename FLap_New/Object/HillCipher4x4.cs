using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLap_New.Object
{
    public class HillCipher4x4
    {
        // 🔑 Khóa 4x4 (phải khả nghịch mod 37)
        int[,] key = {
            { 2, 4, 5, 9 },
            { 3, 7, 2, 5 },
            { 5, 6, 9, 4 },
            { 4, 2, 7, 3 }
        };
        const int N = 4;
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        const int MOD = 37;

        private int Mod(int x) => (x % MOD + MOD) % MOD;

        private int GCD(int a, int b)
        {
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        public int ModInverse(int a)
        {
            a = Mod(a);
            for (int i = 1; i < MOD; i++)
                if (Mod(a * i) == 1)
                    return i;
            throw new Exception("Không có nghịch đảo modular!");
        }

        public int[] Multiply(int[,] matrix, int[] vector)
        {
            int[] result = new int[N];
            for (int i = 0; i < N; i++)
            {
                int sum = 0;
                for (int j = 0; j < N; j++)
                    sum += matrix[i, j] * vector[j];
                result[i] = Mod(sum);
            }
            return result;
        }

        public int Determinant3x3(int[,] m)
        {
            int det = m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1])
                    - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
                    + m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
            return Mod(det);
        }

        public int Determinant(int[,] m)
        {
            int det = 0;
            for (int j = 0; j < N; j++)
            {
                int[,] minor = new int[3, 3];
                for (int mi = 1; mi < N; mi++)
                {
                    int mj2 = 0;
                    for (int mj = 0; mj < N; mj++)
                    {
                        if (mj == j) continue;
                        minor[mi - 1, mj2++] = m[mi, mj];
                    }
                }

                int sign = (j % 2 == 0) ? 1 : -1;
                det += sign * m[0, j] * Determinant3x3(minor);
            }

            return Mod(det);
        }

        public int Cofactor(int[,] m, int row, int col)
        {
            int[,] minor = new int[N - 1, N - 1];
            int mi = 0, mj = 0;

            for (int i = 0; i < N; i++)
            {
                if (i == row) continue;
                mj = 0;
                for (int j = 0; j < N; j++)
                {
                    if (j == col) continue;
                    minor[mi, mj++] = m[i, j];
                }
                mi++;
            }

            int detMinor = Determinant3x3(minor);
            return Mod(detMinor * (((row + col) % 2 == 0) ? 1 : -1));
        }

        public int[,] Adjugate(int[,] m)
        {
            int[,] adj = new int[N, N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    adj[j, i] = Cofactor(m, i, j);
            return adj;
        }

        public int[,] InverseMatrix(int[,] matrix)
        {
            int det = Determinant(matrix);
            if (GCD(det, MOD) != 1)
                throw new Exception("Ma trận không khả nghịch mod 37!");

            int invDet = ModInverse(det);
            int[,] adj = Adjugate(matrix);
            int[,] inv = new int[N, N];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    inv[i, j] = Mod(adj[i, j] * invDet);

            return inv;
        }

        public string Encrypt(string text)
        {
            StringBuilder result = new StringBuilder();
            while (text.Length % N != 0)
                text += ' ';

            for (int i = 0; i < text.Length; i += N)
            {
                int[] block = new int[N];
                for (int j = 0; j < N; j++)
                {
                    int index = alphabet.IndexOf(text[i + j]);
                    if (index == -1)
                        throw new Exception($"Ký tự '{text[i + j]}' không nằm trong bảng mã!");
                    block[j] = index;
                }

                int[] enc = Multiply(key, block);
                for (int j = 0; j < N; j++)
                    result.Append(alphabet[enc[j]]);
            }

            return result.ToString();
        }

        public string Decrypt(string text)
        {
            int[,] invKey = InverseMatrix(key);
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < text.Length; i += N)
            {
                int[] block = new int[N];
                for (int j = 0; j < N; j++)
                {
                    int index = alphabet.IndexOf(text[i + j]);
                    if (index == -1)
                        throw new Exception($"Ký tự '{text[i + j]}' không nằm trong bảng mã!");
                    block[j] = index;
                }

                int[] dec = Multiply(invKey, block);
                for (int j = 0; j < N; j++)
                    result.Append(alphabet[dec[j]]);
            }

            return result.ToString().TrimEnd();
        }
        public HillCipher4x4(){}
    }
}
