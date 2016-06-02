using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Sources
{
    public class SecondaryStructureParser
    {
        string typeOfStructure;
        int startingResidue, endingResidue;
        public SecondaryStructureParser(string strideLine)
        {
            typeOfStructure = strideLine.Substring(3,12).Trim();
            startingResidue = Int32.Parse(strideLine.Substring(22, 5).Trim());
            endingResidue = Int32.Parse(strideLine.Substring(40, 5).Trim());
        }

        public string GetTypeOfStructure ()
        {
            return typeOfStructure;
        }
        public int GetStartingResidue()
        {
            return startingResidue;
        }
        public int GetEndingResidue()
        {
            return endingResidue;
        }
    }
}

