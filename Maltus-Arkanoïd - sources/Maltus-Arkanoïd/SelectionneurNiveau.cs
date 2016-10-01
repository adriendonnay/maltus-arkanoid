using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Maltus_Arkanoïd
{
    public partial class SelectionneurNiveau : Form
    {
        private string POSITIONAPPLICATION = AppDomain.CurrentDomain.BaseDirectory;
        private int NiveauMaxJoueur = 1;
        
        public SelectionneurNiveau(string NomJoueur)
        {
            InitializeComponent();

            Init_Affichage();

            NiveauMaxJoueur = Extraire_Niveau_Max(NomJoueur);

            Charger_Liste_Niveaux(POSITIONAPPLICATION + "\\niveaux\\niveaux\\", dataGridView1, true, NiveauMaxJoueur);
            Charger_Liste_Niveaux(POSITIONAPPLICATION + "\\niveaux\\niveaux utilisateur\\", dataGridView2, false, 1);
        }

        public int Extraire_Niveau_Max(string NomDuJoueur)
        {
            int Nm = 1;
            string User = POSITIONAPPLICATION + "\\Profiles\\" + NomDuJoueur + ".txt";
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

        public void Charger_Liste_Niveaux(string repertoire, DataGridView datagrid, bool natif, int NiveauMax)
        {
            if (Directory.Exists(repertoire))
            {
                string[] split;
                DirectoryInfo Repertoire = new DirectoryInfo(repertoire);

                datagrid.Rows.Clear();

                FileInfo[] fichiers = Repertoire.GetFiles();

                foreach (FileInfo fichier in fichiers)
                {
                    split = fichier.Name.Split(new Char[] { '.' });

                    if (natif)
                    {
                        if(Convert.ToInt32(split[0]) <= NiveauMax)
                        {
                            datagrid.Rows.Add(split[0]);
                        }
                    }
                    else
                    {
                        datagrid.Rows.Add(split[0]);
                    }
                }

            }
            else
            {
                if (!Creer_Dossier(repertoire))
                {
                    MessageBox.Show("Erreur de récupération des niveaux");
                }
            }
        }

        public bool Creer_Dossier(string PositionDossier)
        {
            bool ArretOK = false;
            int i = 0;

            while (!Directory.Exists(PositionDossier) && i < 3)
            {
                Directory.CreateDirectory(PositionDossier);
                i += 1;
            }

            if (i >= 3)
            {
                ArretOK = true;
            }

            return ArretOK;
        }
        
        private void SelectionneurNiveau_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                menu.OPEN = true;
                this.Close();
            }
        }

        private void Init_Affichage()
        {
            label1.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label1.Width) / 2), label1.Location.Y);
            label2.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label2.Width) / 2), label2.Location.Y);
            label3.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - label3.Width) / 2), label3.Location.Y);

            dataGridView1.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - dataGridView1.Width) / 2), dataGridView1.Location.Y);
            dataGridView2.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - dataGridView2.Width) / 2), dataGridView2.Location.Y);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Gestion_Clique_Cellule(dataGridView1, true, e.RowIndex);
            
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Gestion_Clique_Cellule(dataGridView2, false, e.RowIndex);
        }

        private void Gestion_Clique_Cellule(DataGridView datagrid, bool Natif, int ligne)
        {
            string niveau;

            try
            {
                niveau = datagrid.Rows[ligne].Cells[0].Value.ToString();
                if (Natif)
                {
                    Jeu Jeu_Niveau = new Jeu(menu.NomProfil, Convert.ToInt32(niveau), "", true, false);
                    menu.OPEN = false;
                    Jeu_Niveau.Show();
                }
                else
                {
                    Jeu Jeu_Niveau = new Jeu(menu.NomProfil, 0, niveau, false, false);
                    menu.OPEN = false;
                    Jeu_Niveau.Show();
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
    }
}
