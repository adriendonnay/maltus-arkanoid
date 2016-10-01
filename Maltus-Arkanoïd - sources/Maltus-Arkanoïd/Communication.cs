using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Maltus_Arkanoïd
{
    class Communication
    {
        private string _IP;
        private int _PORT;
        private int _prioriteIN = 0;
        private int _prioriteOUT = 0;
        private int _niveau = 0;
        private bool _ecouter = true;
        private bool _ready = false;

        TcpClient _clientSocket = new TcpClient();

        private TcpListener _serveurSocket;
        private TcpClient _srvSocketCli = default(TcpClient);
        private IPAddress _AdresseIP;

        private Form _FormulaireRetours;
        private Label _L2;
        private Label _L3;
        private Label _L4;

        delegate void MAJL(string instr, int x, int y, int z, int vi);

        /****************************************
         *          Initialisation
        ****************************************/

        public Communication(string IP, int PORT, Form FormulaireRetours)
        {
            _IP = IP;
            _PORT = PORT;
            _FormulaireRetours = FormulaireRetours;

            DefinirPrioriteIN();

            _AdresseIP = IPAddress.Any;
            _serveurSocket = new TcpListener(_AdresseIP, _PORT);
        }

        public void initL2(Label L2)
        {
            _L2 = L2;
        }

        public void initL3(Label L3)
        {
            _L3 = L3;
        }

        public void initL4(Label L4)
        {
            _L4 = L4;
        }

        public void demarrer()
        {
            _ecouter = true;
        }

        private void DefinirPrioriteIN()
        {
            Random rnd = new Random();
            int Random = rnd.Next(1, 1001);
            _prioriteIN = Random + DateTime.Now.Millisecond;
        }

        /****************************************
         *         Fonctions d'accès
        ****************************************/

        public int ObtenirPrioriteIN()
        {
            return _prioriteIN;
        }

        public int ObtenirPrioriteOUT()
        {
            return _prioriteOUT;
        }

        public bool ObtenirEtat()
        {
            return _ecouter;
        }

        public bool EstPrioritaire()
        {
            if (_prioriteIN > _prioriteOUT) { return true; }
            else { return false; }
        }

        public int SynchroniserPriorite()
        {
            if (_prioriteOUT == 0)
            {
                Envoyer_Priorite();
                return 2;
            }
            else if (_prioriteIN == _prioriteOUT)
            {
                DefinirPrioriteIN();
                Envoyer_Priorite();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool NiveauSync()
        {
            return _ready;
        }

        public int ReturnNiveauSync()
        {
            return _niveau;
        }

        public void arreter()
        {
            _ecouter = false;
        }

        /****************************************
         *        Fonctions envoi
        ****************************************/

        public void Envoyer_Priorite()
        {
            Envoyer(":GETPRIOR:" + _prioriteIN.ToString() + ":");
        }

        public void Envoyer_PrioriteII()
        {
            Envoyer(":PRIOR:" + _prioriteIN.ToString() + ":");
        }

        public void Envoyer_Niveau(int niveau)
        {
            Envoyer(":SYNC_NIV:" + niveau.ToString() + ":");
        }

        public void Envoyer_NiveauII(int niveau)
        {
            Envoyer(":NIV:" + niveau.ToString() + ":");
        }

        public void Envoyer_START()
        {
            Envoyer(":START:");
        }

        public void Envoyer_DETRUIRE(int XIN, int YIN)
        {
            Envoyer(":DETRUIRE:" + XIN.ToString() + ":" + YIN.ToString() + ":");
        }

        public void Envoyer_BALLE(int XIN, int YIN, int VECT, int VITESSE)
        {
            Envoyer(":BALLE:" + XIN.ToString() + ":" + YIN.ToString() + ":" + VECT.ToString() + ":" + VITESSE.ToString());
        }

        public void Envoyer_DEPLACER(int XIN, int YIN)
        {
            Envoyer(":DEPLACER:" + XIN.ToString() + ":" + YIN.ToString() + ":");
        }

        public void Envoyer_BONUS(int BONUS)
        {
            Envoyer(":BONUS:" + BONUS.ToString() + ":");
        }

        public void Envoyer_NEXT(int NIV)
        {
            Envoyer(":NEXT:" + NIV.ToString() + ":");
        }

        public void Envoyer_EFF()
        {
            Envoyer(":EFF:");
        }

        public void Envoyer_DIE()
        {
            Envoyer(":DIE:");
        }

        public void Envoyer_CLOSE()
        {
            Envoyer(":CLOSE:");
        }

        public void Envoyer_KILL()
        {
            Envoyer(":KILL:");
        }

        /****************************************
         *         Fonctions reception
        ****************************************/

        private void Recevoir_Priorite(int PRIOR)
        {
            if (_prioriteOUT == 0)
            {
                _prioriteOUT = PRIOR;
            }
        }

        private void Recevoir_Niveau_SYNC(int niveau)
        {
            if (_ready == false)
            {
                if (niveau > _niveau)
                {
                    _niveau = niveau;
                }
                _ready = true;
            }
        }

        private void Reception_donnees(string instr, int x, int y, int z, int vi)
        {
            if(instr == "BALLE")
            {
                if (_L3.InvokeRequired)
                {
                    MAJL a = new MAJL(Reception_donnees);
                    _L3.Invoke(a, new object[] { instr, x, y, z, vi });
                }
                else
                {
                    _L3.Text = instr + ":" + x.ToString() + ":" + y.ToString() + ":" + z.ToString() + ":" + vi.ToString();
                }
            }
            else if (instr == "DEPLACER")
            {
                if (_L4.InvokeRequired)
                {
                    MAJL a = new MAJL(Reception_donnees);
                    _L4.Invoke(a, new object[] { instr, x, y, z, vi });
                }
                else
                {
                    _L4.Text = instr + ":" + x.ToString() + ":" + y.ToString() + ":" + z.ToString() + ":" + vi.ToString();
                }
            }
            else
            {
                if (_L2.InvokeRequired)
                {
                    MAJL a = new MAJL(Reception_donnees);
                    _L2.Invoke(a, new object[] { instr, x, y, z, vi});
                }
                else
                {
                    _L2.Text = instr + ":" + x.ToString() + ":" + y.ToString() + ":" + z.ToString() + ":" + vi.ToString();
                }
            }
        }

        /****************************************
         *             Envoyer
        ****************************************/

        public void Envoyer(string message)
        {
            if (_clientSocket.Connected == false)
            {
                try
                {
                    _clientSocket.Connect(_IP, _PORT);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
            }
            else
            {
                try
                {
                    NetworkStream serverStream = _clientSocket.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message + "$");
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                }
                catch (Exception err) { Console.WriteLine(err.ToString()); }
            }
        }

        /****************************************
         *             Recevoir
        ****************************************/

        public void Serveur_Start()
        {
            _ecouter = true;
            string[] split;
            try
            {
                _serveurSocket.Start();
                _srvSocketCli = _serveurSocket.AcceptTcpClient();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                _ecouter = false;
            }

            while (_ecouter)
            { 
                try
                {
                    NetworkStream networkStream = _srvSocketCli.GetStream();
                    byte[] donnees = new byte[4096000];

                    networkStream.Read(donnees, 0, (int)_srvSocketCli.ReceiveBufferSize);
                    string message = System.Text.Encoding.ASCII.GetString(donnees);
                    message = message.Substring(0, message.IndexOf("$"));
                    
                    split = message.Split(new Char[] { ':' });

                    if (split[1] == "PRIOR") // Recevoir et envoyer la priorité
                    {
                        if (_prioriteOUT == 0)
                        {
                            Recevoir_Priorite(Convert.ToInt32(split[2]));
                        }
                    }
                    else if (split[1] == "GETPRIOR") // Recevoir et envoyer la priorité
                    {
                        if (_prioriteOUT == 0)
                        {
                            Recevoir_Priorite(Convert.ToInt32(split[2]));
                        }
                        Envoyer_PrioriteII();
                    }
                    else if (split[1] == "SYNC_NIV") // Synchroniser le niveau
                    {
                        Recevoir_Niveau_SYNC(Convert.ToInt32(split[2]));
                        Envoyer_NiveauII(_niveau);
                    }
                    else if (split[1] == "NIV") // Synchroniser le niveau
                    {
                        Recevoir_Niveau_SYNC(Convert.ToInt32(split[2]));
                    }
                    else if (split[1] == "START" || split[1] == "DIE" || split[1] == "EFF" || split[1] == "CLOSE" || split[1] == "KILL") // Démarrer le jeu
                    {
                        Reception_donnees(split[1], 0, 0, 0, 0);
                    }
                    else if(split[1] == "BONUS" || split[1] == "NEXT")
                    {
                        Reception_donnees(split[1], Convert.ToInt32(split[2]), 0, 0, 0);
                    }
                    else if (split[1] == "DETRUIRE" || split[1] == "DEPLACER")
                    {
                        Reception_donnees(split[1], Convert.ToInt32(split[2]), Convert.ToInt32(split[3]), 0, 0);
                    }
                    else if(split[1] == "BALLE")
                    {
                        Reception_donnees(split[1], Convert.ToInt32(split[2]), Convert.ToInt32(split[3]), Convert.ToInt32(split[4]), Convert.ToInt32(split[5]));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            _clientSocket.Close();

            _srvSocketCli.Close();
            _serveurSocket.Stop();
            GC.Collect();
        }

    }
}