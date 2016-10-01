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
using System.Timers;

namespace Maltus_Arkanoïd
{
    class Balle
    {
        private int _TAILLEBALLEX, _TAILLEBALLEY, _vecteur, _vitesseBalle;
        private int _ModeBonus = 0;
        private int X1 = 1, X2 = 2, X3 = 3;
        private int Y1 = 1, Y2 = 2, Y3 = 3;
        
        private bool _EnMouvement = true;
        private bool _RESTART = false;
        private bool _SuperBalle = false;
        private bool _EnReseau = false;

        private System.Timers.Timer aTimer;

        private PictureBox _balleConteneur = new PictureBox();
        private Joueur _JoueurEnCour;
        private Vaisseau _Vaisseau, _VaisseauII;
        private Point _PositionVaisseau, _position_balle_ecran_calcul = new Point(500, 650);
        private Form _Jeu;
        private Brique[,] _tableauBrique;
        private parametres param = new parametres(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        private Communication _com;

        delegate void MAJIntIntdel(int x, int y);
        
        /****************************************
         * 
         *              Démarrage
         * 
        ****************************************/

        public void INIT(PictureBox pictureBox, Joueur joueurEnCour, Vaisseau Vaisseau, Vaisseau VaisseauII, Form JeuJ, Brique[,] tableauBrique, int vitesseBalle, int ModeBonus)
        {
            _balleConteneur = pictureBox;
            _JoueurEnCour = joueurEnCour;
            _Vaisseau = Vaisseau;
            _VaisseauII = VaisseauII;
            _Jeu = JeuJ;
            _tableauBrique = tableauBrique;
            _vitesseBalle = vitesseBalle;
            _ModeBonus = ModeBonus;
            
            _balleConteneur.Visible = true;
            _balleConteneur.BackgroundImageLayout = ImageLayout.Stretch;
            _balleConteneur.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "Images\\Jeu\\balle.png");
            _balleConteneur.Location = new Point(Convert.ToInt32(_position_balle_ecran_calcul.X * param.ObtenirRAPPORTX()), Convert.ToInt32(_position_balle_ecran_calcul.Y * param.ObtenirRAPPORTY()));
            Redimentionner_Balle();

            _vecteur = 2;

            aTimer = new System.Timers.Timer(param.ObtenirTempsBalleControle());
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public void re_init()
        {
            _RESTART = true;
            _vecteur = 2;
            _position_balle_ecran_calcul = new Point(param.ObtenirPositionBalleBaseX(), param.ObtenirPositionBalleBaseY());
            
            DeplacerBalle(Convert.ToInt32(param.ObtenirPositionBalleBaseX() * param.ObtenirRAPPORTX()), Convert.ToInt32(param.ObtenirPositionBalleBaseY() * param.ObtenirRAPPORTY()));
            aTimer = new System.Timers.Timer(param.ObtenirTempsBalleControle());
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            ActiverTimerCTRL(true);
        }

        public void INIT_Com(Communication com)
        {
            _com = com;
            _EnReseau = true;
        }

        public void Redimentionner_Balle()
        {
            _TAILLEBALLEX = Convert.ToInt32(param.ObtenirTailleBalleBase() * param.ObtenirRAPPORTX());
            _TAILLEBALLEY = Convert.ToInt32(param.ObtenirTailleBalleBase() * param.ObtenirRAPPORTY());

            ChangerTailleBalle(_TAILLEBALLEX, _TAILLEBALLEY);
        }

        /****************************************
         * 
         *          Maintenance
         * 
        ****************************************/

        public void VitesseBase()
        {
            _vitesseBalle = param.ObtenirVitesseBalleBase();
        }

        public void Accelerer_Balle()
        {
            if (_vitesseBalle > 10) { _vitesseBalle -= param.ObtenirAccellerationBalle(); }
        }

        public void Super_Balle()
        {
            if (_SuperBalle) { _SuperBalle = false; }
            else { _SuperBalle = true; }
        }

        private void GererCollision_Brique()
        {
            int positionCarteX = Convert.ToInt32((_position_balle_ecran_calcul.X + 5) / param.ObtenirTailleBriqueX());
            int positionCarteY = Convert.ToInt32((_position_balle_ecran_calcul.Y + 5) / param.ObtenirTailleBriqueY());
            int moduloX = (_position_balle_ecran_calcul.X + 5) % param.ObtenirTailleBriqueX();
            int moduloY = (_position_balle_ecran_calcul.Y + 5) % param.ObtenirTailleBriqueY();

            System.Media.SoundPlayer casserBrique = new System.Media.SoundPlayer(param.ObtenirPosition() + "\\sons\\casser brique.wav");
            if (_tableauBrique[positionCarteX, positionCarteY].gettype() != 0)
            {
                casserBrique.Play();

                int tx1 = 5, tx2 = (param.ObtenirTailleBriqueX() - 5),
                    ty1 = 2, ty2 = (param.ObtenirTailleBriqueY() - 2);


                if (!_SuperBalle)
                {
                    if ((moduloX >= tx1 && moduloX <= tx2) && (moduloY <= ty1 || moduloY >= ty2))
                    {
                        Inverser_Sens(false, true);
                    }
                    else if ((moduloX < tx1 && moduloX > tx2) && (moduloY >= ty1 || moduloY <= ty2))
                    {
                        Inverser_Sens(true, false);
                    }
                    else
                    {
                        Inverser_Sens(true, true);
                    }

                }
                    
                if (_tableauBrique[positionCarteX, positionCarteY].gettype() - 1 >= 0 && _tableauBrique[positionCarteX, positionCarteY].gettype() != 4)
                {
                    _tableauBrique[positionCarteX, positionCarteY].detruire();

                    if (_EnReseau)
                    {
                        _com.Envoyer_DETRUIRE(positionCarteX, positionCarteY);
                    }
                }        
            }
        }
        /*
        public void GererCollision_Brique_Reseau()
        {
            GererCollision_Brique();
        }
        */
        public int ObtenirVecteur()
        {
            return _vecteur;
        }

        public void definirVecteur(int vect)
        {
            _vecteur = vect;
        }

        private bool Changeur_Vecteur_rebond(Vaisseau mouv, bool rebondOK)
        {
            System.Media.SoundPlayer plaque = new System.Media.SoundPlayer(param.ObtenirPosition() + "\\Sons\\Plaque.wav");
            _PositionVaisseau = mouv.Position_Vaisseau();

            if (_position_balle_ecran_calcul.Y >= _PositionVaisseau.Y - Convert.ToInt32(param.ObtenirTailleBalleBase() / 2) && _position_balle_ecran_calcul.Y < _PositionVaisseau.Y + Convert.ToInt32(param.ObtenirTailleBalleBase() / 2))
            {
                if (_position_balle_ecran_calcul.X >= _PositionVaisseau.X && _position_balle_ecran_calcul.X < (_PositionVaisseau.X + param.ObtenirTailleVaisseauBaseX()))
                {
                    if (_position_balle_ecran_calcul.X >= _PositionVaisseau.X && _position_balle_ecran_calcul.X < (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 1)))
                    {
                        _vecteur = 1; plaque.Play(); rebondOK = true;
                    }
                    else if (_position_balle_ecran_calcul.X >= (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 1)) && _position_balle_ecran_calcul.X < (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 2)))
                    {
                        _vecteur = 2; plaque.Play(); rebondOK = true;
                    }
                    else if (_position_balle_ecran_calcul.X >= (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 2)) && _position_balle_ecran_calcul.X < (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 3)))
                    {
                        _vecteur = 3; plaque.Play(); rebondOK = true;
                    }
                    else if (_position_balle_ecran_calcul.X >= (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 3)) && _position_balle_ecran_calcul.X < (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 4)))
                    {
                        _vecteur = 4; plaque.Play(); rebondOK = true;
                    }
                    else if (_position_balle_ecran_calcul.X >= (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 4)) && _position_balle_ecran_calcul.X < (_PositionVaisseau.X + Convert.ToInt32((param.ObtenirTailleVaisseauBaseX() / 5) * 5)))
                    {
                        _vecteur = 5; plaque.Play(); rebondOK = true;
                    }
                }
            }
            return rebondOK;
        }

        public void Inverser_Sens(bool AxeX, bool AxeY)
        {
            switch (_vecteur)
            {
                case 1:
                    if (AxeX && AxeY) { _vecteur = 6; }
                    else if (AxeX) { _vecteur = 5; }
                    else if (AxeY) { _vecteur = 10; }
                    break;
                case 2:
                    if (AxeX && AxeY) { _vecteur = 7; }
                    else if (AxeX) { _vecteur = 4; }
                    else if (AxeY) { _vecteur = 9; }
                    break;

                case 3:
                    _vecteur = 8;
                    break;

                case 4:
                    if (AxeX && AxeY) { _vecteur = 9; }
                    else if (AxeX) { _vecteur = 2; }
                    else if (AxeY) { _vecteur = 7; }
                    break;

                case 5:
                    if (AxeX && AxeY) { _vecteur = 10; }
                    else if (AxeX) { _vecteur = 1; }
                    else if (AxeY) { _vecteur = 6; }
                    break;

                case 6:
                    if (AxeX && AxeY) { _vecteur = 1; }
                    else if (AxeX) { _vecteur = 10; }
                    else if (AxeY) { _vecteur = 5; }
                    break;

                case 7:
                    if (AxeX && AxeY) { _vecteur = 2; }
                    else if (AxeX) { _vecteur = 9; }
                    else if (AxeY) { _vecteur = 4; }
                    break;

                case 8:
                    _vecteur = 3;
                    break;

                case 9:
                    if (AxeX && AxeY) { _vecteur = 4; }
                    else if (AxeX) { _vecteur = 7; }
                    else if (AxeY) { _vecteur = 2; }
                    break;

                case 10:
                    if (AxeX && AxeY) { _vecteur = 5; }
                    else if (AxeX) { _vecteur = 6; }
                    else if (AxeY) { _vecteur = 1; }
                    break;
            }
        }

        public void ActiverTimerCTRL(bool MarcheArret)
        {
            if (MarcheArret)
            {
                aTimer.Start();
            }
            else
            {
                aTimer.Stop();
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            int x = _position_balle_ecran_calcul.X;
            int y = _position_balle_ecran_calcul.Y;

            if ((x == X1 && y == Y1) || (x == X2 && y == Y2) || (x == X3 && y == Y3))
            {
                re_init();
            }

            X3 = X2;
            Y3 = Y2;

            X2 = X1;
            Y2 = Y1;

            X1 = x;
            Y1 = y;
        }

        /****************************************
         * 
         *              Arrêt
         * 
        ****************************************/

        private void killed()
        {
            re_init();
            _JoueurEnCour.PerdreVie();
            ActiverTimerCTRL(false);
        }

        public void ArreterBalle() 
        { 
            _EnMouvement = false; 
        }

        /****************************************
         * 
         *     Communication Thread
         * 
        ****************************************/

        public void DeplacerBalleReseau(int xIN, int yIN, int vectIN, int vitesseBalleIN)
        {
            if (xIN >= 0 && xIN <= param.ObtenirTEJX() && yIN >= 0 && yIN <= param.ObtenirTEJY())
            {
                _position_balle_ecran_calcul = new Point(xIN, yIN);
                DeplacerBalle(Convert.ToInt32(xIN * param.ObtenirRAPPORTX()), Convert.ToInt32(yIN * param.ObtenirRAPPORTY()));
            }
            if (vectIN >= 1 && vectIN <= 10)
            {
                _vecteur = vectIN;
            }
            _vitesseBalle = vitesseBalleIN;
        }

        public void Envoyer_Balle_Reseau()
        {
            _com.Envoyer_BALLE(_position_balle_ecran_calcul.X, _position_balle_ecran_calcul.Y, _vecteur, _vitesseBalle);
        }

        /****************************************
         * 
         *              Delegate
         * 
        ****************************************/

        private void ChangerTailleBalle(int x, int y)
        {
            if (_balleConteneur.InvokeRequired)
            {
                MAJIntIntdel a = new MAJIntIntdel(ChangerTailleBalle);
                _balleConteneur.Invoke(a, new object[] { x, y });
            }
            else
            {
                _balleConteneur.Size = new Size(x, y);
            }
        }

        private void DeplacerBalle(int x, int y)
        {
            try
            {
                if (_balleConteneur.InvokeRequired)
                {
                    MAJIntIntdel a = new MAJIntIntdel(DeplacerBalle);
                    _balleConteneur.Invoke(a, new object[] { x, y });
                }
                else
                {
                    _balleConteneur.Location = new Point(x, y);
                }
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
        }

        /****************************************
         * 
         *              Moteur
         * 
        ****************************************/
        
        public void ALIVE()
        {
            int x = _position_balle_ecran_calcul.X, y = _position_balle_ecran_calcul.Y;
            int a = 2, b = 3, c = 5;
            bool rebondOK = false;

            System.Media.SoundPlayer mort = new System.Media.SoundPlayer(param.ObtenirPosition() + "\\Sons\\Mort.wav");
            System.Media.SoundPlayer rebond = new System.Media.SoundPlayer(param.ObtenirPosition() + "\\Sons\\coup bords.wav");
            
            while (_EnMouvement == true)
            {
                x = _position_balle_ecran_calcul.X;
                y = _position_balle_ecran_calcul.Y;
                rebondOK = false;

                aTimer.Start();

                if (_JoueurEnCour.EstEnCour())
                {
                    switch (_vecteur)
                    {
                        case 1:
                            if (x - b <= 0 && y - a <= 0) { x += b; y += a; _vecteur = 6; rebond.Play(); }
                            else if (x - b <= 0) { x += b; y -= a; _vecteur = 5; rebond.Play(); }
                            else if (y - a <= 0) { x -= b; y += a; _vecteur = 10; rebond.Play(); }
                            else { x -= b; y -= a; }
                            break;

                        case 2:
                            if (x - a <= 0 && y - b <= 0) { x += a; y += b; _vecteur = 7; rebond.Play(); }
                            else if (x - a <= 0) { x += a; y -= b; _vecteur = 4; rebond.Play(); }
                            else if (y - b <= 0) { x -= a; y += b; _vecteur = 9; rebond.Play(); }
                            else { x -= a; y -= b; }
                            break;

                        case 3:
                            if (y - c >= 0) { y -= c;}
                            else { y += c; _vecteur = 8; rebond.Play(); }
                            break;

                        case 4:
                            if (x + a >= param.ObtenirTEJX() && y - b <= 0) { x -= a; y += b; _vecteur = 9; rebond.Play(); }
                            else if (x + a >= param.ObtenirTEJX()) { x -= a; y -= b; _vecteur = 2; rebond.Play(); }
                            else if (y - b <= 0) { x += a; y += b; _vecteur = 7; rebond.Play(); }
                            else { x += a; y -= b; }
                            break;

                        case 5:
                            if (x + b >= param.ObtenirTEJX() && y - a <= 0) { x -= b; y += a; _vecteur = 10; rebond.Play(); }
                            else if (x + b >= param.ObtenirTEJX()) { x -= b; y -= a; _vecteur = 1; rebond.Play(); }
                            else if (y - a <= 0) { x += b; y += a; _vecteur = 6; rebond.Play(); }
                            else { x += b; y -= a; }
                            break;

                        case 6:
                            if (x + b >= param.ObtenirTEJX() && y + a >= param.ObtenirTEJY()) { x -= b; y -= a; _vecteur = 1; mort.Play(); killed(); }
                            else if (x + b >= param.ObtenirTEJX()) { x -= b; y += a; _vecteur = 10; rebond.Play(); }
                            else if (y + a >= param.ObtenirTEJY()) { x += b; y -= a; _vecteur = 5; mort.Play(); killed(); }
                            else { x += b; y += a; }
                            break;

                        case 7:
                            if (x + a >= param.ObtenirTEJX() && y + b >= param.ObtenirTEJY()) { x -= a; y -= b; _vecteur = 2; killed(); mort.Play(); }
                            else if (x + a >= param.ObtenirTEJX()) { x -= a; y += b; _vecteur = 9; rebond.Play(); }
                            else if (y + b >= param.ObtenirTEJY()) { x += a; y -= b; _vecteur = 4; mort.Play(); killed(); }
                            else { x += a; y += b; }
                            break;

                        case 8:
                            if (y + c <= param.ObtenirTEJY()) { y += c; }
                            else { y -= c; _vecteur = 3; mort.Play(); killed(); }
                            break;

                        case 9:
                            if (x - a <= 0 && y + b >= param.ObtenirTEJY()) { x += a; y -= b; _vecteur = 4; mort.Play(); killed(); }
                            else if (x - a <= 0) { x += a; y += b; _vecteur = 7; rebond.Play(); }
                            else if (y + b >= param.ObtenirTEJY()) { x -= a; y -= b; _vecteur = 2; mort.Play(); killed(); }
                            else { x -= a; y += b; }
                            break;

                        case 10:
                            if (x - b <= 0 && y + a >= param.ObtenirTEJY()) { x += b; y -= a; _vecteur = 5; mort.Play(); killed(); }
                            else if (x - b <= 0) { x += b; y += a; _vecteur = 6; rebond.Play(); }
                            else if (y + a >= param.ObtenirTEJY()) { x -= b; y -= a; _vecteur = 1; mort.Play(); killed(); }
                            else { x -= b; y += a; }
                            break;
                    }

                    if (!_RESTART)
                    {
                        _position_balle_ecran_calcul = new Point(x, y);
                        rebondOK = Changeur_Vecteur_rebond(_Vaisseau, rebondOK);

                        if (!rebondOK) 
                        { 
                            Changeur_Vecteur_rebond(_VaisseauII, rebondOK); 
                        }
                    }
                    else
                    {
                        x = param.ObtenirPositionBalleBaseX();
                        y = param.ObtenirPositionBalleBaseY();
                        _position_balle_ecran_calcul = new Point(param.ObtenirPositionBalleBaseX(), param.ObtenirPositionBalleBaseY());
                        _RESTART = false;
                    }

                    DeplacerBalle(Convert.ToInt32(x * param.ObtenirRAPPORTX()), Convert.ToInt32(y * param.ObtenirRAPPORTY()));

                    if ((y >= 0 && y < param.ObtenirTailleZoneBriqueY() - param.ObtenirTailleBriqueY() + 5) && (x >= 0 && x < param.ObtenirTEJX() - 5))
                    {
                        GererCollision_Brique();
                    }
                }

                if (_EnMouvement == true) { Thread.Sleep(_vitesseBalle); }
            }
        }

    }
}
