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
    public class parametres
    {
        private string _POSITIONAPPLICATION = AppDomain.CurrentDomain.BaseDirectory;

        private const int _TAILLEECRANXBASE = 1280;
        private const int _TAILLEECRANYBASE = 780;
        private const int _TAILLEECRANJEUX = 975;
        private const int _TAILLEECRANJEUY = 770;
        private const int _TailleZoneBriqueY = 480;
        private const int _TailleBriqueX = 65;
        private const int _TailleBriqueY = 20;
        private const int _TAILLEVAISSEAUBASEX = 120;
        private const int _TAILLEVAISSEAUBASEY = 74;
        private const int _TAILLEBALLEBASE = 10;
        private const int _VitesseBalleBase = 25;
        private const int _AccellerationBalle = 5;
        private const int _positionBalleBaseX = 500;
        private const int _positionBalleBaseY = 650;
        private const int _TempsBalleControle = 3000;
        
        private int _NombreBriquesX;
        private int _NombreBriquesY;
        private int _MARGEX;

        private double _RAPPORTX;
        private double _RAPPORTY;

        public parametres(int ecranX, int ecranY)
        {
            _RAPPORTX = (Convert.ToDouble(ecranX) / Convert.ToDouble(_TAILLEECRANXBASE));
            _RAPPORTY = (Convert.ToDouble(ecranY) / Convert.ToDouble(_TAILLEECRANYBASE));
            _MARGEX = Convert.ToInt32(_TAILLEECRANJEUX * _RAPPORTX);

            _NombreBriquesX = _TAILLEECRANJEUX / _TailleBriqueX;
            _NombreBriquesY = _TAILLEECRANJEUY / _TailleBriqueY;
        }

        public int ObtenirPositionBalleBaseX()
        {
            return _positionBalleBaseX;
        }

        public int ObtenirPositionBalleBaseY()
        {
            return _positionBalleBaseY;
        }

        public int ObtenirTempsBalleControle()
        {
            return _TempsBalleControle;
        }

        public int ObtenirNombreBriquesX()
        {
            return _NombreBriquesX;
        }

        public int ObtenirNombreBriquesY()
        {
            return _NombreBriquesY;
        }
        
        public string ObtenirPosition()
        {
            return _POSITIONAPPLICATION;
        }

        public int ObtenirTEBX()
        {
            return _TAILLEECRANXBASE;
        }

        public int ObtenirTEBY()
        {
            return _TAILLEECRANYBASE;
        }

        public double ObtenirRAPPORTX()
        {
            return _RAPPORTX;
        }

        public double ObtenirRAPPORTY()
        {
            return _RAPPORTY;
        }

        public int ObtenirTailleBriqueX()
        {
            return _TailleBriqueX;
        }

        public int ObtenirTailleBriqueY()
        {
            return _TailleBriqueY;
        }

        public int ObtenirTailleVaisseauBaseX()
        {
            return _TAILLEVAISSEAUBASEX;
        }

        public int ObtenirTailleVaisseauBaseY()
        {
            return _TAILLEVAISSEAUBASEY;
        }

        public int ObtenirTEJX()
        {
            return _TAILLEECRANJEUX;
        }

        public int ObtenirTEJY()
        {
            return _TAILLEECRANJEUY;
        }

        public int ObtenirTailleBalleBase()
        { 
            return _TAILLEBALLEBASE;
        }

        public int ObtenirTailleZoneBriqueY()
        { 
            return _TailleZoneBriqueY;
        }

        public int ObtenirMargeX()
        {
            return _MARGEX;
        }

        public int ObtenirVitesseBalleBase()
        {
            return _VitesseBalleBase;
        }

        public int ObtenirAccellerationBalle()
        {
            return _AccellerationBalle;
        }
    }
}
