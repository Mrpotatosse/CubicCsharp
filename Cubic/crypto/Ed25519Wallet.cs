using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Cubic.V1.Crypto
{
    public class Ed25519Wallet
    {
        private static readonly int Ed25519SeedSize = 32;
        private static readonly int Ed25519DerivationIterations = 2048;

        private readonly AsymmetricCipherKeyPair keyPair;

        public byte[] PublicKey
        {
            get
            {
                if (keyPair.Public is Ed25519PublicKeyParameters publicKey)
                {
                    return publicKey.GetEncoded();
                }
                throw new Exception("Bad public key parameters");
            }
        }

        public string PublicKeyHex
        {
            get
            {
                return BitConverter.ToString(PublicKey).Replace("-", "").ToLower();
            }
        }

        public byte[] PrivateKey
        {
            get
            {
                if (keyPair.Private is Ed25519PrivateKeyParameters privateKey)
                {
                    return privateKey.GetEncoded();
                }
                throw new Exception("Bad private key parameters");
            }
        }

        public Ed25519Wallet(AsymmetricCipherKeyPair keyPair)
        {
            this.keyPair = keyPair;
        }

        public byte[] Sign(byte[] message)
        {
            ISigner signer = new Ed25519Signer();
            signer.Init(true, keyPair.Private);
            signer.BlockUpdate(message, 0, message.Length);
            return signer.GenerateSignature();
        }

        public static Ed25519Wallet GenerateKeyPair()
        {
            IAsymmetricCipherKeyPairGenerator generator = new Ed25519KeyPairGenerator();
            KeyGenerationParameters parameters = new Ed25519KeyGenerationParameters(new SecureRandom());
            generator.Init(parameters);
            return new Ed25519Wallet(generator.GenerateKeyPair());
        }

        public static Ed25519Wallet GenerateKeyPairFromPassphrase(string passphrase, string salt)
        {
            byte[] seed = GenerateSeedFromPassphrase(passphrase, salt, Ed25519SeedSize);
            return GenerateKeyPairFromSeed(seed);
        }

        public static Ed25519Wallet GenerateKeyPairFromSeed(byte[] seed)
        {
            IAsymmetricCipherKeyPairGenerator generator = new Ed25519KeyPairGenerator();
            KeyGenerationParameters parameters = new Ed25519KeyGenerationParameters(FixedSecureRandom(seed));
            generator.Init(parameters);
            return new Ed25519Wallet(generator.GenerateKeyPair());
        }

        private static SecureRandom FixedSecureRandom(byte[] seed)
        {
            IRandomGenerator random = new DigestRandomGenerator(new Sha256Digest());
            random.AddSeedMaterial(seed);
            return new SecureRandom(random);
        }

        private static byte[] GenerateSeedFromPassphrase(string passphrase, string salt, int seedSize)
        {
            byte[] passphraseBytes = Encoding.UTF8.GetBytes(passphrase);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(passphraseBytes, saltBytes, Ed25519DerivationIterations))
            {
                return pbkdf2.GetBytes(seedSize);
            }
        }
    }
}
