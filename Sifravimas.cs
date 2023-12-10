using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace DesifravimoSifravimoKodai
{
    public class SifravimoKodas
    {
        private static FileStream fsCrypt = null;

        // Šifravimo funkcija naudojant AES
        public static void UzsifruotiBylaAES(string bylosPavadinimas, string slaptazodis)
        {
            // Generuojamas atsitiktinis saltas
            byte[] salt = GeneruotiAtsitiktiniSalt();

            // Sukuriamas failas, kuriame bus saugomas užšifruotas tekstas
            using (FileStream fsCrypt = new FileStream(bylosPavadinimas + ".aes", FileMode.Create))
            {
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

                    // Įrašomas saltas į užšifruoto failo pradžią
                    fsCrypt.Write(salt, 0, salt.Length);

                    // Naudodami CryptoStream šifruojame bylą
                    using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // Atidaromas bylos skaitymo srautas
                        using (FileStream fsIn = new FileStream(bylosPavadinimas, FileMode.Open))
                        {
                            byte[] buferis = new byte[1048576];
                            int skaityti;

                            // Skaitome bylą ir šifruojame duomenis
                            while ((skaityti = fsIn.Read(buferis, 0, buferis.Length)) > 0)
                            {
                                cs.Write(buferis, 0, skaityti);
                            }

                            fsIn.Close();
                        }
                    }
                }

                // Uždaromas šifravimo failas
                fsCrypt.Close();
            }
        }

        // Generuoja atsitiktinį saltą
        public static byte[] GeneruotiAtsitiktiniSalt()
        {
            byte[] duomenys = new byte[16]; // 3DES naudoja 16 baitų ilgio raktą

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(duomenys);
            }

            return duomenys;
        }

        // Šifravimo funkcija naudojant 3DES
        public static void UzsifruotiByla3DES(string bylosPavadinimas, string slaptazodis)
        {
            // Generuojamas atsitiktinis saltas
            byte[] salt = GeneruotiAtsitiktiniSalt();

            // Sukuriamas failas, kuriame bus saugomas užšifruotas tekstas
            FileStream fsCrypt = new FileStream(bylosPavadinimas + ".3des", FileMode.Create);
            byte[] slaptazodzioBytes = Encoding.UTF8.GetBytes(slaptazodis);

            // Sukuriamas 3DES šifravimo objektas
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.KeySize = 128; // 3DES naudoja 128 bitų ilgio raktą
            tripleDES.BlockSize = 64; // 3DES naudoja 64 bitų blokus
            tripleDES.Padding = PaddingMode.PKCS7;

            // Sukuriamas raktas iš slaptažodžio ir salto naudojant PasswordDeriveBytes
            var raktas = new PasswordDeriveBytes(slaptazodzioBytes, salt);
            tripleDES.Key = raktas.GetBytes(tripleDES.KeySize / 8);
            tripleDES.IV = raktas.GetBytes(tripleDES.BlockSize / 8);
            tripleDES.Mode = CipherMode.CBC;

            // Įrašomas saltas į užšifruoto failo pradžią
            fsCrypt.Write(salt, 0, salt.Length);

            // Naudodami CryptoStream šifruojame bylą
            CryptoStream cs = new CryptoStream(fsCrypt, tripleDES.CreateEncryptor(), CryptoStreamMode.Write);
            FileStream fsIn = new FileStream(bylosPavadinimas, FileMode.Open);
            byte[] buferis = new byte[1048576];
            int skaityti;

            try
            {
                // Skaitome bylą ir šifruojame duomenis
                while ((skaityti = fsIn.Read(buferis, 0, buferis.Length)) > 0)
                {
                    cs.Write(buferis, 0, skaityti);
                }

                fsIn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nepavyko: " + ex.Message);
            }
            finally
            {
                // Uždaromas šifravimo failas
                cs.Close();
                fsCrypt.Close();
            }
        }
    }
}
