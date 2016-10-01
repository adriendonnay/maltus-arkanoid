using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maltus_Arkanoïd
{
    public partial class Form4 : Form
    {
        private int ItemSelectionne = 1;
        public int[,] carte = new int[15, 24];
        private parametres param = new parametres(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        private PictureBox placeur;
        private Brique[,] tableauBrique = new Brique[15, 24];
        

        public Form4()
        {
            int x = 0, y = 0;
            InitializeComponent();

            placeur = new PictureBox();
            placeur.BackColor = Color.Blue;
            placeur.KeyUp += new KeyEventHandler(Form4_KeyUp);
            this.Controls.Add(placeur);

            GestionnaireAffichage();
            Vider_Carte(false);
        }

        public void GestionnaireAffichage()
        {
            placeur.Size = new Size(Convert.ToInt32(param.ObtenirTailleBriqueX() * param.ObtenirRAPPORTX()), Convert.ToInt32(param.ObtenirTailleBriqueY() * param.ObtenirRAPPORTY()));

            button1.Size = new System.Drawing.Size(Convert.ToInt32(140 * param.ObtenirRAPPORTX()), Convert.ToInt32(60 * param.ObtenirRAPPORTY()));

            label1.Location = new Point(
                    Convert.ToInt32((((param.ObtenirTEBX() * param.ObtenirRAPPORTX()) - param.ObtenirMargeX() - label1.Size.Width) / 2) + param.ObtenirMargeX()),
                    10
                );

            textBox1.Location = new Point(
                    Convert.ToInt32((((param.ObtenirTEBX() * param.ObtenirRAPPORTX()) - param.ObtenirMargeX() - textBox1.Size.Width) / 2) + param.ObtenirMargeX()),
                    10 + label1.Size.Height + 10
                );

            button1.Location = new Point(
                    Convert.ToInt32((((param.ObtenirTEBX() * param.ObtenirRAPPORTX()) - param.ObtenirMargeX() - button1.Size.Width) / 2) + param.ObtenirMargeX()),
                    10 + label1.Size.Height + 10 + textBox1.Size.Height + 10
                );

            button2.Location = new Point(
                    Convert.ToInt32((((param.ObtenirTEBX() * param.ObtenirRAPPORTX()) - param.ObtenirMargeX() - button2.Size.Width) / 2) + param.ObtenirMargeX()),
                    10 + label1.Size.Height + 10 + textBox1.Size.Height + 10 + button1.Size.Height + 10
                );

            button3.Location = new Point(
                    Convert.ToInt32((((param.ObtenirTEBX() * param.ObtenirRAPPORTX()) - param.ObtenirMargeX() - button3.Size.Width) / 2) + param.ObtenirMargeX()),
                    10 + label1.Size.Height + 10 + textBox1.Size.Height + 10 + button1.Size.Height + 10 + button2.Size.Height + 10
                );
        }

        private void Form4_MouseMove(object sender, MouseEventArgs e)
        {
            if (Control.MousePosition.X > 0 && Control.MousePosition.X < Convert.ToInt32(param.ObtenirTEJX() * param.ObtenirRAPPORTX()))
            {
                if (Control.MousePosition.Y > 0 && Control.MousePosition.Y < Convert.ToInt32(param.ObtenirTailleZoneBriqueY() * param.ObtenirRAPPORTY()))
                {
                    placeur.Location = new Point(Control.MousePosition.X + 5, Control.MousePosition.Y + 5);
                    placeur.Visible = true;
                }
            }
        }


        private void Form4_MouseUp(object sender, MouseEventArgs e)
        {
            int x, y;

            if (e.Button == MouseButtons.Left)
            {
                if (ItemSelectionne >= 0 && ItemSelectionne <= 4)
                {
                    if (Control.MousePosition.X > 0 && Control.MousePosition.X < Convert.ToInt32(param.ObtenirTEJX() * param.ObtenirRAPPORTX()))
                    {
                        if (Control.MousePosition.Y > 0 && Control.MousePosition.Y < Convert.ToInt32(param.ObtenirTailleZoneBriqueY() * param.ObtenirRAPPORTY()))
                        {
                            x = Convert.ToInt32(Math.Floor((Control.MousePosition.X / param.ObtenirRAPPORTX()) / ( param.ObtenirTailleBriqueX())));
                            y = Convert.ToInt32(Math.Floor((Control.MousePosition.Y / param.ObtenirRAPPORTY()) / (param.ObtenirTailleBriqueY())));

                            if (ItemSelectionne != 0 && ItemSelectionne < 5)
                            {
                                if (carte[x, y] == 0)
                                {
                                    try
                                    {
                                        tableauBrique[x, y] = new Brique();
                                        tableauBrique[x, y].setUnlock();
                                        tableauBrique[x, y].Charger_Brique(ItemSelectionne, x, y, this);
                                        carte[x, y] = ItemSelectionne;
                                    }
                                    catch (Exception err)
                                    {
                                        Console.WriteLine(err);
                                        try
                                        {
                                            tableauBrique[x, y].Changer_Brique(ItemSelectionne);
                                            carte[x, y] = ItemSelectionne;
                                        }
                                        catch (Exception er) { Console.WriteLine(er); }
                                    }
                                }
                                else
                                {
                                    tableauBrique[x, y].Changer_Brique(ItemSelectionne);
                                }
                            }
                            else
                            {
                                carte[x, y] = 0;
                                try
                                {
                                    tableauBrique[x, y].detruireConstr();
                                }
                                catch (Exception er) { Console.WriteLine(er); }
                            }
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ItemSelectionne += 1;

                switch (ItemSelectionne)
                { 
                    case 0:
                        placeur.BackColor = Color.White;
                        break;
                    
                    case 1:
                        placeur.BackColor = Color.Blue;
                        break;

                    case 2:
                        placeur.BackColor = Color.Red;
                        break;

                    case 3:
                        placeur.BackColor = Color.Green;
                        break;

                    case 4:
                        placeur.BackColor = Color.Yellow;
                        break;

                    default:
                        ItemSelectionne = 0;
                        placeur.BackColor = Color.White;
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox1.Text != " ")
            {
                string ligne = "";
                int x, y;

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(param.ObtenirPosition() + "\\niveaux\\niveaux utilisateur\\" + textBox1.Text + ".txt"))
                {
                    for (y = 0; y < 24; y++)
                    {
                        for (x = 0; x < 15; x++)
                        {
                            if (x != 14)
                            {
                                ligne += carte[x, y].ToString() + ";";
                            }
                            else
                            {
                                ligne += carte[x, y].ToString();
                            }
                        }
                        file.WriteLine(ligne);
                        ligne = "";
                    }
                    file.Close();
                    Quit();
                }
            }
            else
            {
                MessageBox.Show("Veuillez donnez un nom à votre niveau!");
            }
        }

        public void Vider_Carte(bool empreinte)
        {
            int x, y;
            for (y = 0; y < 24; y++)
            {

                for (x = 0; x < 15; x++)
                {
                    if (empreinte)
                    {
                        if (carte[x, y] != 0)
                        {
                            try
                            {
                                tableauBrique[x, y].detruireConstr();
                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                        }
                    }
                    carte[x, y] = 0;
                }
            }
        }

        private void Form4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Quit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Vider_Carte(true);
        }

        private void Quit()
        {
            menu.OPEN = true;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Quit();
            }
        }
    }
}
