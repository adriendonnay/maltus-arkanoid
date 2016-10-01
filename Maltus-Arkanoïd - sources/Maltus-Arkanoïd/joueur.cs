using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Maltus_Arkanoïd
{
    class Joueur
    {
        private string _NomJoueur;
        private int _NombreVieJoueur = 3;
        private bool _Alive = true, STOPPED = false;
        private Label _l1 = new Label(), _l6 = new Label();
        private parametres param = new parametres(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        public void INITAFF(Label l1, Label l2)
        {
            _l1 = l1;
            _l6 = l2;

            _l1.Text = "Nombre de vies restantes :";
            _l6.Text = _NombreVieJoueur.ToString();

            _l1.Visible = true;
            _l6.Visible = true;
        }

        public void Changer_NomJoueur(string NomJoueur)
        {
            _NomJoueur = NomJoueur;
        }

        public void MAJ_NiveauMax(int niveau)
        {
            System.IO.StreamWriter file;
            int y = 0, ymax;
            string User = param.ObtenirPosition() + "\\Profiles\\" + _NomJoueur + ".txt";
            string[] lignes = System.IO.File.ReadAllLines(User);
            string[] split;
            string[,] donnees = new string[10, 2];

            foreach (string ligne in lignes)
            {
                split = ligne.Split(new Char[] { '=' });
                y += 1;

                if (split[0] == "NM")
                {
                    try
                    {
                        if (niveau > Convert.ToInt32(split[1]))
                        {
                            donnees[y, 0] = "NM";
                            donnees[y, 1] = niveau.ToString();
                        }
                        else
                        {
                            donnees[y, 0] = "NM";
                            donnees[y, 1] = Convert.ToString(1);
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }
                }
                else
                {
                    donnees[y, 0] = split[0];
                    donnees[y, 1] = split[1];
                }
            }

            ymax = y;
            file = new System.IO.StreamWriter(User);

            for (y = 0; y <= ymax; y++)
            {
                if (donnees[y, 0] != "" && donnees[y, 0] != null)
                {
                    file.WriteLine(donnees[y, 0] + "=" + donnees[y, 1]);
                }
            }
            file.Close();
        }

        public void ResetNombreVie()
        {
            _Alive = true;
            _NombreVieJoueur = 3;
            _l1.Text = "Nombre de vies restantes :";
            _l6.Text = _NombreVieJoueur.ToString();
            _l1.Visible = true;
            _l6.Visible = true;
        }

        public int NombreVie()
        {
            return _NombreVieJoueur;
        }

        public void Reprendre()
        {
            STOPPED = false;
        }

        public bool EstEnCour()
        {
            return !STOPPED;
        }

        public void AjouterVie()
        {
            _NombreVieJoueur += 1;

            if (_NombreVieJoueur > 0) { _Alive = true; }
            else { _Alive = false; }
        }

        public void PerdreVie()
        {
            _NombreVieJoueur -= 1;

            if (_NombreVieJoueur > 0)
            {
                _Alive = true;
            }
            else
            {
                _NombreVieJoueur = 0;
                _Alive = false;
            }

            _l6.Invoke(new Action(() => _l6.Text = _NombreVieJoueur.ToString()));
            STOPPED = true;
        }

        public bool EstMort()
        {
            return _Alive;
        }
    }
}
