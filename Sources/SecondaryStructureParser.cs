using System;
using UnityEngine;


namespace Assets.Code.Sources
{
    public class SecondaryStructureParser
    {
        string typeOfStructure, chainID;
        int startingResidue, endingResidue, length;
        public SecondaryStructureParser(string line, bool ifStride)
        {
            if (ifStride)
            {
                typeOfStructure = line.Substring(3, 12).Trim();
                startingResidue = Int32.Parse(line.Substring(22, 5).Trim());
                endingResidue = Int32.Parse(line.Substring(40, 5).Trim());
                chainID = line.Substring(27, 1).Trim();
                length = endingResidue - startingResidue + 1;
            }
            else
            {
                if (line.Substring(0, 6).Trim() == "HELIX" && line.Substring(38, 2).Trim() == "1")
                {
                    typeOfStructure = SecondaryStructures.alphaHelix;
                    startingResidue = Int32.Parse(line.Substring(21, 4).Trim());
                    endingResidue = Int32.Parse(line.Substring(33, 4).Trim());
                    chainID = line.Substring(19, 1).Trim();
                    length = Int32.Parse(line.Substring(71, 5).Trim());
                   
                }
                else if (line.Substring(0, 6).Trim() == "SHEET")
                {
                    typeOfStructure = SecondaryStructures.betaSheet;
                    startingResidue = Int32.Parse(line.Substring(22, 4).Trim());
                    endingResidue = Int32.Parse(line.Substring(33, 4).Trim());
                    chainID = line.Substring(21, 1).Trim();
                    length = endingResidue - startingResidue + 1;
                }

            }
            



        }

        public string GetTypeOfStructure()
        {
            return typeOfStructure;
        }
        public string GetChainID()
        {
            return chainID;
        }
        public int GetStartingResidue()
        {
            return startingResidue;
        }
        public int GetEndingResidue()
        {
            return endingResidue;
        }
        public int GetLength()
        {
            return length;
        }
    }
}

