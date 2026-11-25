using System.Security.Cryptography;
using System.Text;

namespace ProyectoGalactico.Services
{
    public class EncryptionService : IEncryptionService
    {
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            // Clave AES-256 en Base64, guardada en user-secrets. Con ella se cifra el ganador del evento en la BDD.
            var base64Key = configuration["Cifrado:Clave"]
                ?? throw new InvalidOperationException("Falta el secreto Cifrado:Clave");

            _key = Convert.FromBase64String(base64Key);
            if (_key.Length != 32)
            {
                throw new InvalidOperationException("La clave debe tener 32 bytes (256 bits) en Base64");
            }
        }

        public string Encrypt(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);
            var nonce = RandomNumberGenerator.GetBytes(NonceSize);
            var cipherText = new byte[data.Length];
            var tag = new byte[TagSize];

            using var aes = new AesGcm(_key, TagSize);
            aes.Encrypt(nonce, data, cipherText, tag);

            var result = new byte[NonceSize + TagSize + cipherText.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(cipherText, 0, result, NonceSize + TagSize, cipherText.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encryptedText)
        {
            var data = Convert.FromBase64String(encryptedText);
            var nonce = data[..NonceSize];
            var tag = data[NonceSize..(NonceSize + TagSize)];
            var cipherText = data[(NonceSize + TagSize)..];
            var plain = new byte[cipherText.Length];

            using var aes = new AesGcm(_key, TagSize);
            aes.Decrypt(nonce, cipherText, tag, plain);

            return Encoding.UTF8.GetString(plain);
        }
    }
}
