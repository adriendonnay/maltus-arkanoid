using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Maltus_Arkanoïd
{
    public partial class Configuration_Reseau : Form
    {
        public Configuration_Reseau()
        {
            InitializeComponent();

            label7.Text = "Votre IP : " + ObtenirAdresseIP4(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool erreurOk = true;
            try
            {
                int a = Convert.ToInt32(numericUpDown1.Value);
                int b = Convert.ToInt32(numericUpDown2.Value);
                int c = Convert.ToInt32(numericUpDown3.Value);
                int d = Convert.ToInt32(numericUpDown4.Value);
                int f = Convert.ToInt32(numericUpDown5.Value);
                int g = Convert.ToInt32(numericUpDown6.Value);

                if (a >= 0 && a <= 255 && b >= 0 && b <= 255 && c >= 0 && c <= 255 && d >= 0 && d <= 255 && erreurOk)
                {
                    try
                    {
                        string IP = a.ToString() + "." + b.ToString() + "." + c.ToString() + "." + d.ToString();
                        ClientOuServeur cos = new ClientOuServeur(menu.NomProfil, f, IP, g);
                        menu.OPEN = false;
                        cos.Show();

                        this.Close();

                        GC.Collect();
                    }
                    catch (Exception Err) { MessageBox.Show(Err.ToString()); }
                }
                else
                {
                    MessageBox.Show("Erreur dans l'adresse IP!");
                }
            }
            catch (Exception err) { MessageBox.Show("Un erreur s'est produite:\n" + err.ToString()); }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                menu.OPEN = true;
                this.Close();
                GC.Collect();
            }
            catch (Exception err) { MessageBox.Show("Un erreur s'est produite:\n" + err.ToString()); }
        }

        public static string ObtenirAdresseIP4()
        {
            string IP4Address = "";

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
    }
}
