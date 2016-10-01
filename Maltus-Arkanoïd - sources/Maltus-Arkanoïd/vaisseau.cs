using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Maltus_Arkanoïd
{
    class Vaisseau
    {
        private int _TAILLEVAISSEAUX, _TAILLEVAISSEAUY, _x, _y;
        
        public bool SensGauche = true;
        private bool ReseauActif = false;

        private PictureBox _VaisseauConteneur;
        private parametres param = new parametres(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        Communication _com;

        delegate void MAJIntIntdel(int x, int y);

        /****************************************
         * 
         *              Démarrage
         * 
        ****************************************/

        public void INIT(PictureBox Vaisseau)
        {
            _VaisseauConteneur = Vaisseau;
            _x = (param.ObtenirTEBX() - param.ObtenirTailleVaisseauBaseX()) / 2;
            _y = param.ObtenirTEBY() - param.ObtenirTailleVaisseauBaseY();
            
            Redimentionner_Vaisseau_INIT();
            Deplacer_Vaisseau(Convert.ToInt32(_x * param.ObtenirRAPPORTX()), Convert.ToInt32(_y * param.ObtenirRAPPORTY()));
        }

        public void Redimentionner_Vaisseau_INIT()
        {
            _TAILLEVAISSEAUX = Convert.ToInt32(param.ObtenirTailleVaisseauBaseX() * param.ObtenirRAPPORTX());
            _TAILLEVAISSEAUY = Convert.ToInt32(param.ObtenirTailleVaisseauBaseY() * param.ObtenirRAPPORTY());

            Redimentionner_Vaisseau(_TAILLEVAISSEAUX, _TAILLEVAISSEAUY);
        }

        public void Activer_Reseau(Communication com)
        {
            _com = com;
            ReseauActif = true;
        }
        /****************************************
         * 
         *            Maintenance
         * 
        ****************************************/
        
        public Point Position_Vaisseau() // Position du vaisseau sans RAPPORT
        {
            Point Position = new Point(_x, _y);
            return Position;
        }
        
        public void Deplacer_vaisseau_brut(int X, int Y)
        {
            int xt = Convert.ToInt32(X * param.ObtenirRAPPORTX());
            int yt = Convert.ToInt32(Y * param.ObtenirRAPPORTY());

            if (X > 0 && X <= param.ObtenirTEJX() && Y > 0 && Y <= param.ObtenirTEJY())
            {
                Deplacer_Vaisseau(xt, yt);
                _x = X;
                _y = Y;
            }
        }

        public void Deplacer_Vaisseau_Mouse(int X, int Y)
        {
            int xt = Convert.ToInt32(X / param.ObtenirRAPPORTX());
            int yt = Convert.ToInt32(Y / param.ObtenirRAPPORTY());

            if (xt > 0 && xt <= param.ObtenirTEJX() && yt > 0 && yt <= param.ObtenirTEJY())
            {
                Deplacer_Vaisseau(X, Y);

                _x = xt;
                _y = yt;
            }
        }

        public void Deplacer_Vaisseau_OUT(int X, int Y)
        {
            if (X > 0 && X <= param.ObtenirTEJX() && Y > 0 && Y <= param.ObtenirTEJY())
            {
                Deplacer_Vaisseau(Convert.ToInt32(X * param.ObtenirRAPPORTX()), Convert.ToInt32(Y * param.ObtenirRAPPORTY()));

                _x = X;
                _y = Y;
            }
        }

        public void Glisser_Vaisseau_OUT()
        {
            int bordureDroite = Convert.ToInt32(param.ObtenirMargeX() - (param.ObtenirTailleVaisseauBaseX() * param.ObtenirRAPPORTX()));
            int x = _VaisseauConteneur.Location.X;
            if (SensGauche)
            {
                if (_VaisseauConteneur.Location.X - 5 >= 0)
                {
                    x = _VaisseauConteneur.Location.X - 5;
                }
                else
                {
                    if (_VaisseauConteneur.Location.X != 0)
                    {
                        x = 0;
                    }
                }
            }
            else
            {
                if (_VaisseauConteneur.Location.X + 5 <= bordureDroite)
                {
                    x = _VaisseauConteneur.Location.X + 5;

                }
                else
                {
                    if (_VaisseauConteneur.Location.X != bordureDroite)
                    {
                        x = bordureDroite;
                    }
                }
            }

            Deplacer_Vaisseau(x, _VaisseauConteneur.Location.Y);

            _x = Convert.ToInt32(_VaisseauConteneur.Location.X / param.ObtenirRAPPORTX());
            _y = Convert.ToInt32(_VaisseauConteneur.Location.Y / param.ObtenirRAPPORTY());
        }

        private void Deplacer_Vaisseau(int x, int y)
        {
            if (_VaisseauConteneur.InvokeRequired)
            {
                MAJIntIntdel a = new MAJIntIntdel(Deplacer_Vaisseau);
                _VaisseauConteneur.Invoke(a, new object[] { x, y });
            }
            else
            {
                _VaisseauConteneur.Location = new Point(x, y);

                if (ReseauActif)
                {
                    _com.Envoyer_DEPLACER(Convert.ToInt32(_x), Convert.ToInt32(_y));
                }
            }
        }

        private void Redimentionner_Vaisseau(int x, int y)
        {
            if (_VaisseauConteneur.InvokeRequired)
            {
                MAJIntIntdel a = new MAJIntIntdel(Redimentionner_Vaisseau);
                _VaisseauConteneur.Invoke(a, new object[] { x, y });
            }
            else
            {
                _VaisseauConteneur.Size = new Size(x, y);
            }
        }

    }
}
