using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Gps
    {
        public override string ToString() => $"{Latitude.ToLatitude()}-{Longitude.ToLongitude()}";

        /// <summary>
        /// Mille nautique est de 1852 mètres
        /// </summary>
        public const int Mille = 1852;

        /// <summary>
        /// Arc/degré en décimal correspondant à 0.001 MN soit 1.8 mètres
        /// </summary>
        public readonly static double Degré0M001 = Gps.ConvertMilesEnDegré(0.001);

        /// <summary>
        /// Arc/degré en décimal correspondant à 0.01 MN soit 18 mètres
        /// </summary>
        public readonly static double Degré0M01 = Gps.ConvertMilesEnDegré(0.01);

        /// <summary>
        /// Arc/degré en décimal correspondant à 0.05 MN soit 90 mètres
        /// </summary>
        public readonly static double Degré0M05 = Gps.ConvertMilesEnDegré(0.05);

        /// <summary>
        /// Arc/degré en décimal correspondant à 0.1 MN soit 185 mètres
        /// </summary>
        public readonly static double Degré0M1 = Gps.ConvertMilesEnDegré(0.1);

        /// <summary>
        /// Arc/degré en décimal correspondant à 0.3 MN soit 495 mètres
        /// </summary>
        public readonly static double Degré0M3 = Gps.ConvertMilesEnDegré(0.3);

        /// <summary>
        /// Arc/degré en décimal correspondant à 0.5 MN soit 925 mètres
        /// </summary>
        public readonly static double Degré0M5 = Gps.ConvertMilesEnDegré(0.5);

        /// <summary>
        /// Arc/degré en décimal correspondant à 2 MN
        /// </summary>
        public readonly static double Degré2M = Gps.ConvertMilesEnDegré(2);

        /// <summary>
        /// Arc/degré en décimal correspondant à 1 MN
        /// </summary>
        public readonly static double Degré1M = Gps.ConvertMilesEnDegré(1);

        /// <summary>
        /// Arc/degré en décimal correspondant à 5 MN
        /// </summary>
        public readonly static double Degré5M = Gps.ConvertMilesEnDegré(5);

        /// <summary>
        /// Arc/degré en décimal correspondant à 10 MN
        /// </summary>
        public readonly static double Degré10M = Gps.ConvertMilesEnDegré(10);

        /// <summary>
        /// Arc/degré en décimal correspondant à 15 MN
        /// </summary>
        public readonly static double Degré15M = Gps.ConvertMilesEnDegré(15);

        /// <summary>
        /// Arc/degré en décimal correspondant à 30 MN
        /// </summary>
        public readonly static double Degré30M = Gps.ConvertMilesEnDegré(30);

        /// <summary>
        /// Arc/degré en décimal correspondant à 45 MN
        /// </summary>
        public readonly static double Degré45M = Gps.ConvertMilesEnDegré(45);

        /// <summary>
        /// Arc/degré en décimal correspondant à 60 MN
        /// </summary>
        public readonly static double Degré60M = Gps.ConvertMilesEnDegré(60);

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        /// <summary>
        /// Constructeur (pour désiarilzation)
        /// </summary>
        public Gps()
        {
            Latitude = 0;
            Longitude = 0;
        }

        public Gps(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Gps(decimal latitude, decimal longitude)
        {
            Latitude = (double)latitude;
            Longitude = (double)longitude;
        }

        public Gps(string latitude, string longitude)
        {
            Latitude = Parse(latitude);
            Longitude = Parse(longitude);
        }

        /// <summary>
        /// Parse une chaine de caractères de type GPS "047N30.250" ou "002W30.000" pour obtenir sa conversion en double
        /// </summary>
        /// <param name="coordonnee"></param>
        /// <returns></returns>
        public static double Parse(string coordonnee)
        {
            if (coordonnee.Length != 10) return 0;

            int.TryParse(coordonnee.Substring(0, 3), out int degrees);
            int.TryParse(coordonnee.Substring(4, 2), out int minutes);
            int.TryParse(coordonnee.Substring(7, 3), out int secondes);
            double result = degrees + minutes / 60.0 + secondes / 60000.0;

            switch (coordonnee.Substring(3, 1).ToUpper())
            {
                case "W":
                case "O":
                case "S":
                    return -result;
                default:
                    return result;
            }
        }

        /// <summary>
        /// Conversion de miles en unité degré (double Android) => 1 degré = 60 minutes => 1 mile
        /// </summary>
        /// <param name="miles"></param>
        public static double ConvertMilesEnDegré(double miles) => miles / 60.0;

        /// <summary>
        /// Calcul de la distance entre deux points GPS en MN
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public double CalculeMilesTo(double latitude, double longitude)
        {
            const double radius = Math.PI / 180D;
            const double eQuatorialEarthRadius = 6378.1370D;

            double dlong = (longitude - Longitude) * radius;
            double dlat = (latitude - Latitude) * radius;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(latitude * radius) * Math.Cos(Longitude * radius) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            return 1000 * eQuatorialEarthRadius * c / Mille;
        }

        /// <summary>
        /// Calcul approximatif de la distance entre deux points GPS en MN
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public double CalculeMilesTo(Gps to) => CalculeMilesTo(to.Latitude, to.Longitude);

        public int Direction(Gps to) => Direction(to.Latitude, to.Longitude);

        public int Direction(double latitude, double longitude)
        {
            int result;
            double x = 60 * (longitude - Longitude);
            double y = 60 * (latitude - Latitude);
            int angle = (int)(180 * Math.Atan2(y, x) / Math.PI);

            if (x > -0.0001 && x < 0.0001)
            {
                result = y > 0 ? 0 : 180;
            }
            else if (x > 0)
            {
                result = 90 - angle;
            }
            else if (y > 0)
            {
                result = 450 - angle;
            }
            else
            {
                result = 90 - angle;
            }
            return result;
        }

        public Gps GetLocalisationAt(double miles, int direction)
        {
            double angle;
            if (direction > 180)
            {
                angle = Math.PI * (450 - direction) / 180;
            }
            else
            {
                angle = Math.PI * (90 - direction) / 180;
            }

            double latitude = Latitude + miles * Math.Sin(angle);
            double longitude = Longitude + miles * Math.Cos(angle);
            return new Gps(latitude, longitude);
        }
    }

}

