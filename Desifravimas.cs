using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DesifravimoSifravimoKodai
{
    class DesifravimoKodas
    {
        // Desifravimo funkcija naudojant AES
        public static void DesifruotiBylaAES(string uzsifruotaByla, string naujaByla, string slaptazodis)
        {
            // Skaitomas saltas iš užšifruotos bylos
            using (FileStream fsCrypt = new FileStream(uzsifruotaByla, FileMode.Open))
            {
                byte[] salt = new byte[16];
                fsCrypt.Read(salt, 0, salt.Length);

                // Konvertuojamas slaptažodis į baitų masyvą
                byte[] slaptazodzioBytes = Encoding.UTF8.GetBytes(slaptazodis);

                // Sukuriamas AES šifravimo objektas
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Padding = PaddingMode.PKCS7;

                    // Sukuriamas raktas iš slaptažodžio, salt ir iteracijų skaičiaus
                    var raktas = new Rfc2898DeriveBytes(slaptazodzioBytes, salt, 50000);
                    aes.Key = raktas.GetBytes(aes.KeySize / 8);
                    aes.IV = raktas.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CFB;

                    // Naudodami CryptoStream dešifruojame bylą
                    using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        // Sukuriama nauja byla dešifravimui
                        using (FileStream fsOut = new FileStream(naujaByla, FileMode.Create))
                        {
                            byte[] buferis = new byte[1048576];
                            int skaityti;

                            // Skaitome dešifruotus duomenis iš CryptoStream
                            while ((skaityti = cs.Read(buferis, 0, buferis.Length)) > 0)
                            {
                                // Rašome dešifruotus duomenis į naują bylą
                                fsOut.Write(buferis, 0, skaityti);
                            }

                            fsOut.Close();
                        }
                    }
                }

                fsCrypt.Close();
            }
        }

        // Desifravimo funkcija naudojant 3DES
        public static void DesifruotiByla3DES(string uzsifruotaByla, string naujaByla, string slaptazodis)
        {
            // Konvertuojamas slaptažodis į baitų masyvą
            byte[] slaptazodzioBytes = Encoding.UTF8.GetBytes(slaptazodis);
            // Sukuriama baitų masyvo vieta saltui
            byte[] salt = new byte[16];

            // Nuskaitomas saltas iš užšifruotos bylos
            using (FileStream fsCrypt = new FileStream(uzsifruotaByla, FileMode.Open))
            {
                fsCrypt.Read(salt, 0, salt.Length);

                // Sukuriamas 3DES šifravimo objektas
                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                tripleDES.KeySize = 128;
                tripleDES.BlockSize = 64;
                tripleDES.Padding = PaddingMode.PKCS7;

                // Sukuriamas raktas iš slaptažodžio ir salto naudojant PasswordDeriveBytes
                var raktas = new PasswordDeriveBytes(slaptazodzioBytes, salt);
                tripleDES.Key = raktas.GetBytes(tripleDES.KeySize / 8);
                tripleDES.IV = raktas.GetBytes(tripleDES.BlockSize / 8);
                tripleDES.Mode = CipherMode.CBC;

                // Naudodami CryptoStream dešifruojame bylą
                using (CryptoStream cs = new CryptoStream(fsCrypt, tripleDES.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    // Sukuriama nauja byla dešifravimui
                    using (FileStream fsOut = new FileStream(naujaByla, FileMode.Create))
                    {
                        int skaityti;
                        byte[] buferis = new byte[1048576];

                        try
                        {
                            // Skaitome dešifruotus duomenis iš CryptoStream
                            while ((skaityti = cs.Read(buferis, 0, buferis.Length)) > 0)
                            {
                                // Rašome dešifruotus duomenis į naują bylą
                                fsOut.Write(buferis, 0, skaityti);
                            }
                        }
                        catch (CryptographicException ex_CryptographicException)
                        {
                            // Jei įvyko kriptografinė klaida
                            Console.WriteLine("Nepavyko " + ex_CryptographicException.Message);
                        }
                        catch (Exception ex)
                        {
                            // Jei įvyko kitokia klaida
                            Console.WriteLine("Nepavyko " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
