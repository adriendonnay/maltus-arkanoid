using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;

namespace Maltus_Arkanoïd
{
    public partial class menu : Form
    {
        public static bool OPEN = true;
        public static string NomProfil = "";
        private parametres param = new parametres(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        System.Media.SoundPlayer fond1;

        public menu()
        {
            InitializeComponent();

            fond1 = new System.Media.SoundPlayer(param.ObtenirPosition() + "\\Sons\\1.wav");
            fond1.PlayLooping();
            Init_Graphics();
            if (NomProfil == "") { Init_Profil(); } //Profil initialisé?
        }

        private void label2_Click(object sender, EventArgs e) //Jouer
        {
            GC.Collect();
            if (OPEN)
            {
                Jeu Jeu_Niveau = new Jeu(NomProfil, Extraire_Niveau_Max(NomProfil), "", true, false);
                OPEN = false;
                Jeu_Niveau.Show();
            }
        }

        private void label3_Click(object sender, EventArgs e) //Sélectionner un niveau
        {
            GC.Collect();
            if (OPEN)
            {
                SelectionneurNiveau SN = new SelectionneurNiveau(NomProfil);
                OPEN = false;
                SN.Show();
            }
        }

        private void label4_Click(object sender, EventArgs e) //Créateur de niveau
        {
            GC.Collect();
            if (OPEN)
            {
                Form4 Formulaire4 = new Form4();
                OPEN = false;
                Formulaire4.Show();
            }
        }

        private void label5_Click(object sender, EventArgs e) //Changer de profil
        {
            GC.Collect();
            if (OPEN)
            {
                Init_Profil();
            }
        }

        private void label6_Click(object sender, EventArgs e) //Multi-joueur local
        {
            GC.Collect();
            if (OPEN)
            {
                Jeu Jeu_Niveau = new Jeu(NomProfil, Extraire_Niveau_Max(NomProfil), "", true, true);
                OPEN = false;
                Jeu_Niveau.Show();
            }
        }

        private void label7_Click(object sender, EventArgs e) //Quitter
        {
            GC.Collect();
            Application.Exit();
        }

        private void label9_Click(object sender, EventArgs e) //Multi-joueurs réseau
        {
            GC.Collect();
            if (OPEN)
            {
                Configuration_Reseau cr = new Configuration_Reseau();
                OPEN = false;
                cr.Show();
            }
        }

        public void Init_Profil()
        {
            if (OPEN)
            {
                Profil Formulaire2 = new Profil(label8);
                OPEN = false;
                Formulaire2.Show();
                Formulaire2.BringToFront();
                /*
                if (NomProfil == " " ||NomProfil == "")
                {
                    Init_Profil();
                }*/
            }
        }

        public void Init_Graphics()
        {
            label1.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label1.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height/16) * 1));
            label2.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label2.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height/16) * 3));
            label3.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label3.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height/16) * 5));
            label4.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label4.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height/16) * 7));
            label5.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label5.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height/16) * 9));
            label6.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label6.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height/16) * 11));
            label7.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label7.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height/16) * 15));
            label8.Location = new Point(10, 10);
            label9.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label7.Width) / 2), ((Screen.PrimaryScreen.Bounds.Height / 16) * 13));
        }

        private void menu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Application.Exit(); }
        }

        public int Extraire_Niveau_Max(string NomDuJoueur)
        {
            int Nm = 1;
            string User = param.ObtenirPosition() + "\\Profiles\\" + NomDuJoueur + ".txt";
            string[] lignes = System.IO.File.ReadAllLines(User);
            string[] split;

            foreach (string ligne in lignes)
            {
                split = ligne.Split(new Char[] { '=' });
                if (split[0] == "NM")
                {
                    try
                    {
                        if (Convert.ToInt32(split[1]) != 0)
                        {
                            Nm = Convert.ToInt32(split[1]);
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }
                    break;
                }
            }
            return Nm;
        }
    }
}