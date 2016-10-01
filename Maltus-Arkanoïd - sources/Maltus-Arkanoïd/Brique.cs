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
    class Brique
    {
        private int _type = 0, _x, _y;

        private bool unlock = false;

        private PictureBox _pb;
        private Form _formulaireCible;
        static Label _l1;
        private parametres param = new parametres(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        delegate void MAJColordel(Color couleur);
        delegate void MAJVoiddel();

        /****************************************
                       Création
         ****************************************/

        public void ChargerLabel8(Label l8)
        {
            _l1 = l8;
        }

        public void Charger_Brique(int type, int postionX, int positionY, Form formulaireCible) // Jeu
        {
            _type = type;
            _x = postionX;
            _y = positionY;
            _formulaireCible = formulaireCible;

            if (_type > 0 && _type <= 4)
            {
                _pb = new PictureBox();
                _pb.BackgroundImageLayout = ImageLayout.Stretch;
                _pb.Visible = true;
                _pb.BringToFront();
                _pb.Size = new Size(Convert.ToInt32(param.ObtenirTailleBriqueX() * param.ObtenirRAPPORTX()), Convert.ToInt32(param.ObtenirTailleBriqueY() * param.ObtenirRAPPORTY()));
                _pb.Location = new Point(Convert.ToInt32(postionX * param.ObtenirTailleBriqueX() * param.ObtenirRAPPORTX()), Convert.ToInt32(positionY * param.ObtenirTailleBriqueY() * param.ObtenirRAPPORTY()));
                Changer_Brique(_type);
                _pb.BorderStyle = BorderStyle.FixedSingle;
                if (unlock)
                {
                    _pb.MouseClick += new MouseEventHandler(DestroyClick);
                }
                _formulaireCible.Controls.Add(_pb);
                AugmenterNombreBrique();
            }
        }

        /****************************************
                          Set
         ****************************************/

        public void setUnlock()
        {
            unlock = true;
        }

        public void Changer_Brique(int typeBrique) // Changer le type de brique
        {
            switch (typeBrique)
            {
                case 0:
                    cassebriqueappel();
                    break;

                case 1:
                    ChangerCouleur(Color.Blue);
                    break;

                case 2:
                    ChangerCouleur(Color.Red);
                    break;

                case 3:
                    ChangerCouleur(Color.Green);
                    break;

                case 4:
                    ChangerCouleur(Color.Yellow);
                    break;
            }
        }

        /****************************************
                          GET
         ****************************************/

        public int gettype()
        {
            return _type;
        }

        /****************************************
                       Nettoyage
         ****************************************/

        public void Clear()
        {
            _type = 0;
            cassebriqueappel();
            DiminuerNombreBrique();
        }

        /****************************************
                       Destruction
         ****************************************/

        public void detruire() // Jeu
        {
            switch (_type)
            {
                case 1:
                    _type -= 1;
                    cassebriqueappel();
                    DiminuerNombreBrique();
                    Changer_Brique(_type);
                    break;

                case 2:
                    _type -= 1;
                    Changer_Brique(_type);
                    break;

                case 3:
                    _type -= 1;
                    Changer_Brique(_type);
                    break;

                case 4:
                    ChangerCouleur(Color.Yellow);
                    break;
            }
        }

        public void DestroyClick(object sender, MouseEventArgs e) //créateur de cartes
        {
            if (e.Button == MouseButtons.Left)
            {
                cassebriqueappel();
            }
        }

        public void detruireConstr() //créateur de cartes
        {
            cassebriqueappel();
        }

        /****************************************
                         Delegate
         ****************************************/

        private void DiminuerNombreBrique()
        {
            if (_l1.InvokeRequired)
            {
                MAJVoiddel a = new MAJVoiddel(DiminuerNombreBrique);
                _l1.Invoke(a);
            }
            else
            {
                _l1.Text = (Convert.ToInt32(_l1.Text) - 1).ToString();
            }
        }

        private void AugmenterNombreBrique()
        {
            if (_l1.InvokeRequired)
            {
                MAJVoiddel a = new MAJVoiddel(AugmenterNombreBrique);
                _l1.Invoke(a);
            }
            else
            {
                _l1.Text = (Convert.ToInt32(_l1.Text) + 1).ToString();
            }
        }

        private void ChangerCouleur(Color couleur)
        {
            if (_pb.InvokeRequired)
            {
                MAJColordel a = new MAJColordel(ChangerCouleur);
                _pb.Invoke(a, new object[] { couleur });
            }
            else
            {
                _pb.BackColor = couleur;
            }
        }

        public void cassebriqueappel()
        {
            try
            {
                if (_pb.InvokeRequired)
                {
                    MAJVoiddel a = new MAJVoiddel(cassebriqueappel);
                    _pb.Invoke(a);
                }
                else
                {
                    _pb.Visible = false;
                    _pb.Dispose();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }
    }
}
