using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

namespace Maltus_Arkanoïd
{
    public partial class ClientOuServeur : Form
    {
        string _IP;
        string _Nom_Profil;
        int _PORT;
        int _niveau;

        private Communication Communiquer;
        Thread serveurCommunication;

        public ClientOuServeur(string Nom_Profil, int niveau, string IP, int PORT)
        {
            InitializeComponent();

            _Nom_Profil = Nom_Profil;
            _niveau = niveau;
            _IP = IP;
            _PORT = PORT;

            Communiquer = new Communication(IP, PORT, this);
            serveurCommunication = new Thread(Communiquer.Serveur_Start);
            serveurCommunication.Start();

            timer1.Enabled = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Communiquer.SynchroniserPriorite() == 0)
            {
                if (Communiquer.NiveauSync())
                {
                    LancerJeu();
                }
                else
                {
                    Communiquer.Envoyer_Niveau(_niveau);
                }
            }
        }

        private void LancerJeu()
        { 
            timer1.Stop();
            timer1.Dispose();
            
            Communiquer.arreter();

            try
            {
                serveurCommunication.Join();
            }
            catch (Exception err)
            {
                serveurCommunication.Abort();
                Console.WriteLine(err.ToString());
            }
            
            if (Communiquer.EstPrioritaire())
            {
                Jeu_Serveur MASERV = new Jeu_Serveur(_Nom_Profil, Communiquer.ReturnNiveauSync(), " ", true, _IP, _PORT);
                MASERV.Show();
            }
            else
            {
                Jeu_Client MACLI = new Jeu_Client(_Nom_Profil, Communiquer.ReturnNiveauSync(), " ", true, _IP, _PORT);
                MACLI.Show();
            }

            this.Close();
            GC.Collect();
        }

        private void ClientOuServeur_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) 
            {
                menu.OPEN = true;
                timer1.Stop();
                timer1.Dispose();
                this.Close();
                GC.Collect();
            }
        }
    }
}
