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
    public partial class Profil : Form
    {
        string RepertoireProfiles = AppDomain.CurrentDomain.BaseDirectory + "\\Profiles\\";
        private Label _l8 = new Label();
        
        public Profil(Label L8)
        {
            InitializeComponent();
            _l8 = L8;
            Charger_Profiles();
            if (dataGridView1.Rows.Count <= 0) { MessageBox.Show("Aucun profil trouvé, veuillez en créer un!"); }
        }

        private void button1_Click(object sender, EventArgs e) //Ajout d'un profil
        {
            if (textBox1.Text != "" && textBox1.Text != " ")
            {
                string ProfilPosition = RepertoireProfiles + textBox1.Text.ToString() + ".txt";

                Creer_Fichier(ProfilPosition);

                Charger_Profiles();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) //Sélection d'un profil
        {
            try
            {
                menu.NomProfil = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                _l8.Invoke(new Action(() => _l8.Text = menu.NomProfil));
                menu.OPEN = true;
                this.Close();
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
        }

        public void Charger_Profiles()
        {
            string[] split;
            DirectoryInfo Repertoire = new DirectoryInfo(RepertoireProfiles);

            dataGridView1.Rows.Clear();

            if (Creer_Dossier(RepertoireProfiles)){}
            else
            {
                FileInfo[] fichiers = Repertoire.GetFiles();

                foreach (FileInfo fichier in fichiers)
                {
                    split = fichier.Name.Split(new Char[] { '.' });
                    dataGridView1.Rows.Add(split[0]);
                }
            }
        }

        public void Creer_Fichier(string PositionFichier)
        { 
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(PositionFichier))
                {
                    file.WriteLine("NM=1");
                    file.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur lors de la création du fichier de profil!");
                Console.WriteLine(e.ToString());
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
    }
}
