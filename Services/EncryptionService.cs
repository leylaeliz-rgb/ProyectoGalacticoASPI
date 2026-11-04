using System.Security.Cryptography;
using System.Text;

namespace ProyectoGalactico.Services
{
    public class EncryptionService : IEncryptionService
    {
        private const int TamanoNonce = 12;
        private const int TamanoTag = 16;
        private readonly byte[] _clave;

        public EncryptionService(IConfiguration configuration)
        {
            var claveBase64 = configuration["Cifrado:Clave"] // hay que settear en el secret manager uououououououououououououououou, DEBE VERSE CIFRADO EN LA BDD
                ?? throw new InvalidOperationException("Falta el secreto Cifrado:Clave");

            _clave = Convert.FromBase64String(claveBase64);
            if (_clave.Length != 32)
            {
                throw new InvalidOperationException("La clave debe tener 32 bytes (256 bits) en Base64");
            }
        }

        public string Encrypt(string plainText)
        {
            var datos = Encoding.UTF8.GetBytes(plainText);
            var nonce = RandomNumberGenerator.GetBytes(TamanoNonce);
            var cifrado = new byte[datos.Length];
            var tag = new byte[TamanoTag];

            using var aes = new AesGcm(_clave, TamanoTag);
            aes.Encrypt(nonce, datos, cifrado, tag);

            var resultado = new byte[TamanoNonce + TamanoTag + cifrado.Length];
            Buffer.BlockCopy(nonce, 0, resultado, 0, TamanoNonce);
            Buffer.BlockCopy(tag, 0, resultado, TamanoNonce, TamanoTag);
            Buffer.BlockCopy(cifrado, 0, resultado, TamanoNonce + TamanoTag, cifrado.Length);

            return Convert.ToBase64String(resultado);
        }

        public string Decrypt(string encryptedText)
        {
            var datos = Convert.FromBase64String(encryptedText);
            var nonce = datos[..TamanoNonce];
            var tag = datos[TamanoNonce..(TamanoNonce + TamanoTag)];
            var cifrado = datos[(TamanoNonce + TamanoTag)..];
            var plano = new byte[cifrado.Length];

            using var aes = new AesGcm(_clave, TamanoTag);
            aes.Decrypt(nonce, cifrado, tag, plano);

            return Encoding.UTF8.GetString(plano);
        }
    }
}
