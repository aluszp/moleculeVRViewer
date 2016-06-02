using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code.Sources
{
    public class Configurator
    {
        private static string pdbID;
        private static bool hideHydrogens = false;
        private static string representationStyle;
        private static string colouring;

        public static string GetPdbID()
        {
            return pdbID;
        }

        public static void SetPdbID(string value)
        {
            pdbID = value;
        }

        public static string GetRepresentationStyle()
        {
            return representationStyle;
        }

        public static void SetRepresentationStyle(string value)
        {
            representationStyle = value;
        }

        public static string GetColouring()
        {
            return colouring;
        }

        public static void SetColouring(string value)
        {
            colouring = value;
        }

        public static bool GetHideHydrogens()
        {
            return hideHydrogens;
        }

        public static void SetHideHydrogens(bool value)
        {
            hideHydrogens = value;
        }

    }
}
