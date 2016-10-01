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
using System.Net;
using System.Net.Sockets;
namespace Maltus_Arkanoïd
{
    public partial class Jeu_Serveur : Form
    {
        private string _nomNiveau;
        private string _IP;

        public int _niveau;
        public int porte = 0;
        /*
         * 0 => init
         * 1 => jeu en cour
         * 2 => pause
         * 3 => menu
         * 4 => demarrage / redemarrage
         * 5 => vie perdue
         * 6 => Mort
         * 7 => gagne natif
         * 9 => gagne utilisateur
         */
        //public int[,] carte = new int[15, 24];
        public int ModeBonus = 0;
        private int _PORT;
        private bool mode = true;
        private bool _natif;
        private bool loaded = false;
        private bool claviersouristf = false;
        
        static Balle balleObjet = new Balle();
        static Vaisseau vaisseauObjet = new Vaisseau();
        static Vaisseau vaisseauObjet2 = new Vaisseau();
        private Joueur joueurEnCour = new Joueur();
        private parametres param = new parametres(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        private Brique[,] tableauBrique = new Brique[15, 24];
        private PictureBox vaisseauII;

        private Thread BalleBouger;

        private Communication Communiquer;
        Thread serveurCommunication;

        public Jeu_Serveur(string nomJoueur, int niveau, string nomNiveau, bool natif, string IP, int PORT)
        {
            InitializeComponent();

            _niveau = niveau;
            _nomNiveau = nomNiveau;
            _natif = natif;
            _IP = IP;
            _PORT = PORT;

            Communiquer = new Communication(IP, PORT, this);
            serveurCommunication = new Thread(Communiquer.Serveur_Start);
            Communiquer.initL2(label2);
            Communiquer.initL4(label4);
            serveurCommunication.Start();

            porte = 0;
            Init_ecran();
            ChargementDesObjets();
            joueurEnCour.Changer_NomJoueur(nomJoueur);
            Demarrage();
        }

        /****************************************
         *
         *         Fonction diverses
         * 
        ****************************************/

        /****************************************
         *    Fonctions démarrage / relance
        ****************************************/

        public void Init_ecran()
        {
            int rapport = Convert.ToInt32(10 * param.ObtenirRAPPORTX());

            label1.Text = "3";

            label5.Location = new Point(Convert.ToInt32(1015 * param.ObtenirRAPPORTX()), Convert.ToInt32(20 * param.ObtenirRAPPORTY()));
            label6.Location = new Point(Convert.ToInt32(1015 * param.ObtenirRAPPORTX()), Convert.ToInt32(40 * param.ObtenirRAPPORTY()));
            
            label7.Location = new Point(Convert.ToInt32(1015 * param.ObtenirRAPPORTX()), Convert.ToInt32(60 * param.ObtenirRAPPORTY()));
            label8.Location = new Point(Convert.ToInt32(1015 * param.ObtenirRAPPORTX()), Convert.ToInt32(80 * param.ObtenirRAPPORTY()));

            label7.Visible = true;
            label8.Visible = true;
            label8.Text = "0";

            BonusBox.Size = new Size(Convert.ToInt32(100 * param.ObtenirRAPPORTX()), Convert.ToInt32(100 * param.ObtenirRAPPORTY()));
            BonusBox.Location = new Point(Convert.ToInt32((param.ObtenirTEJX() + (((param.ObtenirTEBX() - param.ObtenirTEJX()) - BonusBox.Size.Width) / 2)) * param.ObtenirRAPPORTX()), Convert.ToInt32(100 * param.ObtenirRAPPORTY()));

            label9.Location = new Point(Convert.ToInt32(1015 * param.ObtenirRAPPORTX()), Convert.ToInt32(220 * param.ObtenirRAPPORTY()));

            pictureBox2.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\souris.jpg");
            pictureBox2.Size = new Size(Convert.ToInt32(100 * param.ObtenirRAPPORTX()), Convert.ToInt32(100 * param.ObtenirRAPPORTY()));
            pictureBox2.Location = new Point(Convert.ToInt32((param.ObtenirTEJX() + (((param.ObtenirTEBX() - param.ObtenirTEJX()) - pictureBox2.Size.Width) / 2)) * param.ObtenirRAPPORTX()), Convert.ToInt32(240 * param.ObtenirRAPPORTY()));

            label9.Visible = true;
            pictureBox2.Visible = true;

            vaisseauII = new PictureBox();
            this.Controls.Add(vaisseauII);
            vaisseauII.Visible = true;
            vaisseauII.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\VaisseauII.png");
            vaisseauII.BackgroundImageLayout = ImageLayout.Stretch;

            label10.Location = new Point(Convert.ToInt32(1015 * param.ObtenirRAPPORTX()), Convert.ToInt32(360 * param.ObtenirRAPPORTY()));

            pictureBox3.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\az.jpg");
            pictureBox3.Size = new Size(Convert.ToInt32(100 * param.ObtenirRAPPORTX()), Convert.ToInt32(100 * param.ObtenirRAPPORTY()));
            pictureBox3.Location = new Point(Convert.ToInt32((param.ObtenirTEJX() + (((param.ObtenirTEBX() - param.ObtenirTEJX()) - pictureBox3.Size.Width) / 2)) * param.ObtenirRAPPORTX()), Convert.ToInt32(380 * param.ObtenirRAPPORTY()));

            label10.Visible = true;
            pictureBox3.Visible = true;
        }

        private void ChargementDesObjets()
        {
            vaisseauObjet.INIT(pictureBox1);
            vaisseauObjet.Activer_Reseau(Communiquer);
            balleObjet.INIT(balleConteneur, joueurEnCour, vaisseauObjet, vaisseauObjet2, this, tableauBrique, param.ObtenirVitesseBalleBase(), ModeBonus);
            balleObjet.INIT_Com(Communiquer);

            vaisseauObjet2.INIT(vaisseauII);
            BalleBouger = new Thread(balleObjet.ALIVE);

            joueurEnCour.INITAFF(label5, label6);
            joueurEnCour.Reprendre();
            joueurEnCour.ResetNombreVie();
        }

        public void Demarrage()
        {
            RendrePret();
            
            balleObjet.re_init();
            BonusBox.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\bonus box.jpg");
            label1.Visible = true;
            Cursor.Hide();
            timer1.Enabled = true;
            timer1.Stop();
            timer5.Enabled = true;
            timer5.Stop();
        }

        public void RendrePret()
        {
            Charger_niveau();
            porte = 0;
            label1.Text = "Appuyez sur S pour commencer";
            label1.Location = new Point(Convert.ToInt32(((param.ObtenirTEBX() - label1.Size.Width) / 2) * param.ObtenirRAPPORTX()), Convert.ToInt32(((param.ObtenirTEBY() - label1.Size.Height) / 2) * param.ObtenirRAPPORTY()));
            label1.Visible = true;
        }

        public void booting()
        {
            try
            {
                label1.Visible = false;
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }

            porte = 1;
            balleObjet.re_init();

            if (!BalleBouger.IsAlive)
            {
                BalleBouger.Start();
            }
            else
            {
                try
                {
                    BalleBouger.Resume();
                }
                catch (Exception err) { Console.WriteLine(err.ToString()); }
            }

            timer1.Start();
            timer5.Start();
        }

        public void PreparationReprise()
        {
            label1.Text = "Vous avez perdu une vie!\nAppuyez sur espace pour continuer ^^";
            label1.Location = new Point(Convert.ToInt32(((param.ObtenirTEBX() - label1.Size.Width) / 2) * param.ObtenirRAPPORTX()), Convert.ToInt32(((param.ObtenirTEBY() - label1.Size.Height) / 2) * param.ObtenirRAPPORTY()));
            label1.Visible = true;
            
            balleObjet.VitesseBase();
            porte = 5;
        }

        public void RepriseApresPerteVie()
        {
            if (Convert.ToInt32(label6.Text) > 0)
            {
                label1.Visible = false;
                balleObjet.re_init();
                BonusBox.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\bonus box.jpg");
                joueurEnCour.Reprendre();
                timer1.Start();
                timer5.Start();
                porte = 1;
            }
        }

        public void ReDemarrage()
        {
            porte = 0;
            timer1.Enabled = true;
            timer1.Stop();
            timer5.Stop();

            balleObjet.VitesseBase();

            label1.Text = "Appuyez sur S pour commencer";
            label1.Location = new Point(Convert.ToInt32(((param.ObtenirTEBX() - label1.Size.Width) / 2) * param.ObtenirRAPPORTX()), Convert.ToInt32(((param.ObtenirTEBY() - label1.Size.Height) / 2) * param.ObtenirRAPPORTY()));
            label1.Visible = true;

            Stopper_Bonus();
        }

        public void Bonus_GEN(bool PasAleatoire, int NoBonus, bool ControleActif)
        {
            /*
             * 0 => Aucun bonus
             * 1 => Vitesse de base
             * 2 => Superballe
             * 3 => BombeAtomique
             */

            bool ControlesOK = true;
            
            if (ControleActif)
            {
                if (Convert.ToInt32(label8.Text) != 0 && Convert.ToInt32(label8.Text) % 10 == 0 && ModeBonus == 0) { }
                else { ControlesOK = false; }
            }

            if (ControlesOK)
            {
                if (!PasAleatoire)
                {
                    Random RandomBonus = new Random();
                    ModeBonus = RandomBonus.Next(1, 4);
                }
                else
                {
                    ModeBonus = NoBonus;
                }

                switch (ModeBonus)
                {
                    case 0:
                        BonusBox.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\bonus box.jpg");
                        timer2.Stop();
                        break;

                    case 1:
                        balleObjet.VitesseBase();
                        timer2.Start();
                        BonusBox.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\slow.jpg");
                        Communiquer.Envoyer_BONUS(1);
                        break;

                    case 2:
                        timer2.Start();
                        balleObjet.Super_Balle();
                        BonusBox.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\SuperBalle.jpg");
                        Communiquer.Envoyer_BONUS(2);
                        break;

                    case 3:
                        BonusBox.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\BombeA.jpg");
                        Vider_Carte(true, true);
                        Communiquer.Envoyer_BONUS(3);
                        break;
                }
            }
        }


        /****************************************
         *  Fonctions contrôle / maintenance
        ****************************************/

        public bool Charger_niveau()
        {
            bool Ok = true;
            string line;
            string[] split;
            string FichierNiveau = "";
            int x = 0;
            int y = 0;
            int splitc;

            if (_natif)
            {
                if (_niveau > 0) { FichierNiveau = param.ObtenirPosition() + "\\niveaux\\niveaux\\" + _niveau.ToString() + ".txt"; }
            }
            else { FichierNiveau = param.ObtenirPosition() + "\\niveaux\\niveaux utilisateur\\" + _nomNiveau + ".txt"; }

            if (File.Exists(FichierNiveau))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(FichierNiveau);

                for (y = 0; y < 24; y++)
                {
                    line = file.ReadLine().ToString();
                    split = line.Split(new Char[] { ';' });

                    for (x = 0; x < 15; x++)
                    {
                        splitc = Convert.ToInt32(split[x]);
                        
                        tableauBrique[x, y] = new Brique();
                        tableauBrique[x, y].ChargerLabel8(label8);
                        tableauBrique[x, y].Charger_Brique(splitc, x, y, this);
                    }
                }
            }
            else { Ok = false; }
            loaded = true;
            label8.Text = CompterBriquesRestantes().ToString();

            return Ok;
        }

        public int CompterBriquesRestantes()
        {
            int x = 0, y = 0, NBR = 0;

            if (loaded)
            {
                for (y = 0; y < 24; y++)
                {
                    for (x = 0; x < 15; x++)
                    {
                        if (tableauBrique[x, y].gettype() != 0 && tableauBrique[x, y].gettype() != 4)
                        {
                            NBR += 1;
                        }
                    }
                }

                label8.Text = NBR.ToString();
            }
            return NBR;
        }

        public bool Reste_Briques()
        {
            int x, y;
            bool Jeu_termine = true;

            for (y = 0; y < 24; y++)
            {
                for (x = 0; x < 15; x++)
                {
                    if (tableauBrique[x, y].gettype() != 0 && tableauBrique[x, y].gettype() != 4)
                    {
                        Jeu_termine = false;
                        break;
                    }
                }
            }

            return Jeu_termine;
        }

        public void PerdreVie()
        {
            Stopper_Bonus();

            if (joueurEnCour.EstMort())
            {
                timer1.Stop();
                timer5.Stop();
                PreparationReprise();
            }
            else
            {
                label1.Text = "GAME OVER!\nAppuyez sur espace";
                label1.Location = new Point(Convert.ToInt32(((1280 - label1.Size.Width) / 2) * param.ObtenirRAPPORTX()), Convert.ToInt32(((780 - label1.Size.Height) / 2) * param.ObtenirRAPPORTY()));
                label1.Visible = true;
                porte = 6;
            }

            Communiquer.Envoyer_DIE();
        }

        /****************************************
         *         Fonctions Fermeture
        ****************************************/

        public void Stopper_Bonus() // Arrêter le bonus
        {
            switch (ModeBonus)
            {
                case 2:
                    balleObjet.Super_Balle();
                    break;
            }

            ModeBonus = 0;
            BonusBox.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\bonus box.jpg");
            timer2.Stop();
        }

        public void Vider_Carte(bool empreinte, bool winopt)
        {
            int x, y;

            Communiquer.Envoyer_EFF();

            BalleBouger.Suspend();

            for (y = 0; y < 24; y++)
            {
                for (x = 0; x < 15; x++)
                {
                    if (empreinte)
                    {
                        if (tableauBrique[x, y].gettype() != 0)
                        {
                            try { tableauBrique[x, y].detruireConstr(); }
                            catch (Exception e) { MessageBox.Show(e.ToString()); }
                        }
                    }
                    tableauBrique[x, y].Clear();
                }
            }
            if (winopt) { label8.Text = "0"; }
        }

        public void TerminerNiveau()
        {
            timer1.Stop();
            timer5.Stop();
            try
            {
                BalleBouger.Suspend();
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }

            Vider_Carte(true, false);

            loaded = false;

            if (_natif)
            {
                porte = 7;
                _niveau += 1;

                if (_niveau <= 20)
                {
                    joueurEnCour.MAJ_NiveauMax(_niveau);
                    label1.Text = "Vous avez gagné!\nAppuyez sur espace pour continuer ^^\nESC pour quitter";
                    porte = 7;
                }
                else
                {
                    label1.Text = "Félicitations! Vous avez terminé le jeu!\nESC pour quitter";
                }

            }
            else
            {
                porte = 9;
                label1.Text = "Vous avez gagné!\nESC pour quitter";
            }
        }

        public void kill() // Perte de connection
        {
            MessageBox.Show("Erreur! Perte de la connection!");
            StopperJeu();
        }

        public void stopperBalle()
        {
            try
            {
                BalleBouger.Join();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());

                try
                {
                    BalleBouger.Abort();
                }
                catch (Exception err2) { Console.WriteLine(err2.ToString()); }
            }
        }

        public void StopperJeu()
        {
            Communiquer.Envoyer_KILL();
            Thread.Sleep(5);
            Communiquer.Envoyer_KILL();
            Thread.Sleep(5);
            Communiquer.Envoyer_KILL();


            Communiquer.arreter();

            try
            {
                serveurCommunication.Abort();
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }

            balleObjet.ArreterBalle();
            stopperBalle();
            
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer4.Stop();
            timer5.Stop();

            timer1.Dispose();
            timer2.Dispose();
            timer3.Dispose();
            timer4.Dispose();
            timer5.Dispose();

            Cursor.Show();

            try
            {
                menu.OPEN = true;
                GC.SuppressFinalize(this);
                this.Close();
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
        }
        
        /****************************************
         *
         *      Gestion entrées utilisateur
         *            
        ****************************************/

        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    Communiquer.Envoyer_KILL();

                    StopperJeu();
                    break;
            
                case Keys.Space:
                    if (porte == 5)
                    {
                        RepriseApresPerteVie();
                        Communiquer.Envoyer_START();
                    }
                    else if(porte == 6)
                    {
                        Communiquer.Envoyer_CLOSE();
                        StopperJeu();
                    }
                    else if (porte == 7)
                    {
                        if (_natif)
                        {
                            Demarrage();

                            if (_niveau <= 20)
                            {
                                Vider_Carte(true, false);
                                Charger_niveau();
                                Communiquer.Envoyer_NEXT(_niveau);
                            }
                        }
                        else
                        {
                            StopperJeu();
                        }
                    }
                    break;

                case Keys.Left:
                    if (claviersouristf)
                    {
                        vaisseauObjet.SensGauche = true;
                        if (!timer3.Enabled)
                        {
                            timer3.Start();
                        }
                    }
                    break;

                case Keys.Right:
                    if (claviersouristf)
                    {
                        vaisseauObjet.SensGauche = false;
                        if (!timer3.Enabled)
                        {
                            timer3.Start();
                        }
                    }
                    break;

                case Keys.I:
                    if (claviersouristf) 
                    { 
                        claviersouristf = false;
                        pictureBox2.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\souris.jpg");
                    }
                    else 
                    { 
                        claviersouristf = true;
                        pictureBox2.BackgroundImage = Image.FromFile(param.ObtenirPosition() + "\\Images\\Jeu\\fleches.jpg");
                    }
                    break;

                case Keys.S:
                    if(porte == 0 || porte == 4)
                    {
                        if (loaded)
                        {
                            Communiquer.Envoyer_START();
                            booting();
                        }
                    }
                    break;
            }
        }

        private void Jeu_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                if (claviersouristf) { timer3.Stop(); }
            }
        }

        private void Form3_MouseMove(object sender, MouseEventArgs e)
        {
            if (!claviersouristf)
            {
                if (porte == 1)
                {
                    if (Control.MousePosition.Y < Convert.ToInt32(500 * param.ObtenirRAPPORTY()) && Control.MousePosition.Y > Convert.ToInt32(580 * param.ObtenirRAPPORTY()))
                    {
                        Cursor.Position = new Point(Control.MousePosition.X, Convert.ToInt32(540 * param.ObtenirRAPPORTY()));
                    }
                    if (Control.MousePosition.X > Convert.ToInt32(param.ObtenirMargeX() - (param.ObtenirTailleVaisseauBaseX() * param.ObtenirRAPPORTX())))
                    {
                        Cursor.Position = new Point(Convert.ToInt32(param.ObtenirMargeX() - (param.ObtenirTailleVaisseauBaseX() * param.ObtenirRAPPORTX())), Control.MousePosition.Y);
                    }
                
                    if (Control.MousePosition.X >= 0 && Control.MousePosition.X <= param.ObtenirMargeX() - (Convert.ToInt32(pictureBox1.Size.Width)))
                    {
                        vaisseauObjet.Deplacer_Vaisseau_Mouse(Control.MousePosition.X, Screen.PrimaryScreen.Bounds.Height - pictureBox1.Size.Height);
                    }
                    else
                    {
                        if (Control.MousePosition.X < 0)
                        {
                            Cursor.Position = new Point(0, Control.MousePosition.Y);
                        }
                        else if (Control.MousePosition.X > param.ObtenirMargeX())
                        {
                            Cursor.Position = new Point((param.ObtenirMargeX() - Convert.ToInt32(pictureBox1.Size.Width / param.ObtenirRAPPORTX())), Control.MousePosition.Y);
                        }
                    }
                }
            }
        }

        /****************************************
         *
         *               Form
         *            
        ****************************************/

        private void label1_TextChanged(object sender, EventArgs e)
        {
            label1.Location = new Point(Convert.ToInt32(((1280 - label1.Size.Width) / 2) * param.ObtenirRAPPORTX()), Convert.ToInt32(((780 - label1.Size.Height) / 2) * param.ObtenirRAPPORTY()));
            label1.Visible = true;
        }

        private void label2_TextChanged(object sender, EventArgs e)
        {
            string[] split;
            split = label2.Text.Split(new Char[] { ':' });

            if (split[0] == "KILL")
            {
                MessageBox.Show("Perte de la connection...\nFermeture du jeu.");
                StopperJeu();
            }
        }

        private void label4_TextChanged(object sender, EventArgs e)
        {
            string[] split;
            split = label4.Text.Split(new Char[] { ':' });

            if (split[0] == "DEPLACER")
            {
                vaisseauObjet2.Deplacer_Vaisseau_OUT(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]));
            }
        }

        private void label6_TextChanged(object sender, EventArgs e) // Perdre Vie
        {
            if(porte == 1)
            {
                PerdreVie();
            }
        }

        private void label8_TextChanged(object sender, EventArgs e)
        {
            if(porte == 1 && loaded)
            {
                int NBR = Convert.ToInt32(label8.Text);

                if (NBR <= 0)
                {
                    NBR = CompterBriquesRestantes();
                    label8.Text = NBR.ToString();
                }

                if (NBR > 0)
                {
                    Bonus_GEN(false, 0, true);
                }
                else
                {
                    TerminerNiveau();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            balleObjet.Accelerer_Balle();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Stopper_Bonus();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (porte == 1)
            {
                vaisseauObjet.Glisser_Vaisseau_OUT();
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (porte == 1)
            {
                vaisseauObjet2.Glisser_Vaisseau_OUT();
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            balleObjet.Envoyer_Balle_Reseau();
        }
    }
}
