using DesifravimoSifravimoKodai;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesifravimoSifravimoPrograma
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Vykdoma, kai paspaudžiamas mygtukas "Vykdyti"
        private void VykdytiButtonClick(object sender, RoutedEventArgs e)
        {
            // Gaunami pasirinkimai iš ComboBox
            string veiksmas = ((ComboBoxItem)VeiksmoComboBox.SelectedItem).Content.ToString();
            string algoritmas = ((ComboBoxItem)AlgoritmoComboBox.SelectedItem).Content.ToString();
            string slaptazodis = SlaptazodisTextBox.Text;
            string bylosPavadinimas = BylosPavadinimasTextBox.Text;

            if (veiksmas == "Šifruoti")
            {
                // Šifruojama pagal pasirinktą algoritmą
                if (algoritmas == "AES")
                {
                    SifravimoKodas.UzsifruotiBylaAES(bylosPavadinimas, slaptazodis);
                }
                else if (algoritmas == "3DES")
                {
                    SifravimoKodas.UzsifruotiByla3DES(bylosPavadinimas, slaptazodis);
                }
                else
                {
                    // Jei pasirinktas netinkamas algoritmas
                    MessageBox.Show("Neteisingas algoritmo pasirinkimas.");
                }
            }
            else if (veiksmas == "Dešifruoti")
            {
                // Gaunamos iš šifravimo įvesties bylos pavadinimo informacijos
                string uzsifruotaByla = BylosPavadinimasTextBox.Text;
                string naujaByla = "Atkoduota_" + uzsifruotaByla;

                if (algoritmas == "AES" && uzsifruotaByla.EndsWith(".aes"))
                {
                    // Dešifruojama pagal pasirinktą algoritmą
                    DesifravimoKodas.DesifruotiBylaAES(uzsifruotaByla, naujaByla, slaptazodis);
                }
                else if (algoritmas == "3DES" && uzsifruotaByla.EndsWith(".3des"))
                {
                    // Dešifruojama pagal pasirinktą algoritmą
                    DesifravimoKodas.DesifruotiByla3DES(uzsifruotaByla, naujaByla, slaptazodis);
                }
                else
                {
                    // Jei pasirinktas netinkamas algoritmas arba bylos plėtinys
                    MessageBox.Show("Neteisingas algoritmo pasirinkimas arba bylos plėtinys.");
                }
            }
            else
            {
                // Jei pasirinktas netinkamas veiksmas
                MessageBox.Show("Neteisingas veiksmas.");
            }

            // Informacinis pranešimas apie operacijos pabaigą
            MessageBox.Show("Operacija baigta.");
        }
    }
}
