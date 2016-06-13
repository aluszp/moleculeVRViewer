using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using UnityEngine;

namespace Assets.Code.Sources
{
    public class FileReader
    {
        private string URL = "http://files.rcsb.org/download/" + Configurator.GetPdbID() + ".pdb";

        private WebClient client;
        private bool hasHydrogens;
        private bool ifStride;
        private List<AtomParser> listOfAtoms;
        private Vector3 sumOfPositions;
        private string chains; //for dictionaries
        private string title;
        private int numberOfConects;
        private string residues; //for dictionaries
        private List<List<int>> listOfConectPairs; //pairs that are connected in CONECT section in pdb
        private List<SecondaryStructureParser> listOfSecondaryStructures;
        private List<SecondaryStructureParser> listOfAlphaHelices;
        private List<SecondaryStructureParser> listOfBetaSheets;

        public FileReader()
        {
            client = new WebClient();
            listOfAtoms = new List<AtomParser>();
            ifStride = true;
        }

        public bool GetHasHydrogens()
        {
            return hasHydrogens;
        }

        public List<AtomParser> GetListOfAtoms()
        {
            return listOfAtoms;
        }

        public Vector3 GetSumOfPositions()
        {
            return sumOfPositions;
        }

        public string GetChains()
        {
            chains = chains.Remove(chains.Length - 1);
            return chains;
        }

        public string GetTitle()
        {
            title = Regex.Replace(title, ";", "");
            return title;
        }

        public int GetNumberOfConects()
        {
            return numberOfConects;
        }

        public string GetResidues()
        {
            return residues;
        }

        public List<List<int>> GetListOfConectPairs()
        {
            return listOfConectPairs;
        }

        public List<SecondaryStructureParser> GetListOfSecondaryStructures()
        {
            return listOfSecondaryStructures;
        }

        public List<SecondaryStructureParser> GetListOfAlphaHelices()
        {
            return listOfAlphaHelices;
        }

        public List<SecondaryStructureParser> GetListOfBetaSheets()
        {
            return listOfBetaSheets;
        }


        public void ReadFile()
        {
            //System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Dell 15z\Studia\NOWA PRACA MGR\4QRV.pdb");
            StreamReader file = new StreamReader(client.OpenRead(URL));
            listOfConectPairs = new List<List<int>>();
            listOfSecondaryStructures = new List<SecondaryStructureParser>();
            listOfAlphaHelices = new List<SecondaryStructureParser>();
            listOfBetaSheets = new List<SecondaryStructureParser>();
            int titleNo = -2;
            int titleNo2;

            while (!file.EndOfStream)
            {
                string line = file.ReadLine();

                if (line.Substring(0, 6).Trim() == "ATOM" || line.Substring(0, 6).Trim() == "HETATM")
                {
                    AtomParser thisAtom = new AtomParser(line);

                    if (thisAtom.GetElementType() == "H") //important for VdW and Balls&Sticks representation
                    {
                        hasHydrogens = true;
                    }
                    listOfAtoms.Add(thisAtom);
                    sumOfPositions += thisAtom.GetAtomPosition();
                }

                else if ((line.Substring(0, 6).Trim() == "COMPND")
                    && (line.Contains("MOLECULE:")))
                {
                    if (!(line.Substring(20, 60).Trim().EndsWith(";")))
                    {
                        titleNo = Int32.Parse(line.Substring(9, 1).Trim());
                        Debug.Log(titleNo);
                        title = title + line.Substring(20, 60).Trim();
                    }

                    else
                    {
                        title = title + line.Substring(20, 60).Trim() + " ";
                    }
                }

                else if ((line.Substring(0, 6).Trim() == "COMPND")
                   && (Int32.TryParse(line.Substring(9, 1).Trim(), out titleNo2) && titleNo2-1 == titleNo))
                {
                    title = title + line.Substring(10, 60).Trim() + " ";
                }

                //creating dictionary of subunits and respective colours for subunits coloring method
                else if ((line.Substring(0, 6).Trim() == "COMPND")
                    && (line.Contains("CHAIN:")))
                {

                    chains = chains + line.Substring(18, 60).Trim().Trim(';').Replace(" ", "") + ",";

                }

                //creating dictionary of residues and respective colours for residue coloring method
                else if ((Configurator.GetColouring() == Colouring.residues) && (line.Substring(0, 6).Trim() == "SEQRES"))

                {
                    residues = residues + line.Substring(19, 51).Trim() + " ";

                }

                else if ((Configurator.GetRepresentationStyle() == RepresentationStyles.lines
                    || Configurator.GetRepresentationStyle() == RepresentationStyles.ballsAndSticks)
                    && line.Substring(0, 6).Trim() == "CONECT") //counting number of bonds assigned in CONECT section
                {
                    if (line.Substring(11, 5).Trim() != ""
                        && Int32.Parse(line.Substring(11, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                        && Int32.Parse(line.Substring(11, 5).Trim()) != 0)
                    {
                       
                        listOfConectPairs.Add
                            (new List<int>() { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(11, 5).Trim()) });
                        numberOfConects++;
                    }
                    if (line.Substring(16, 5).Trim() != ""
                        && Int32.Parse(line.Substring(16, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                        && Int32.Parse(line.Substring(16, 5).Trim()) != 0)
                    {
                        listOfConectPairs.Add
                            (new List<int>() { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(16, 5).Trim()) });
                        numberOfConects++;
                    }
                    if (line.Substring(21, 5).Trim() != ""
                        && Int32.Parse(line.Substring(21, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                        && Int32.Parse(line.Substring(21, 5).Trim()) != 0)
                    {
                        listOfConectPairs.Add
                            (new List<int>() { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(21, 5).Trim()) });
                        numberOfConects++;
                    }
                    if (line.Substring(26, 5).Trim() != ""
                        && Int32.Parse(line.Substring(26, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                        && Int32.Parse(line.Substring(26, 5).Trim()) != 0)
                    {
                        listOfConectPairs.Add
                            (new List<int>() { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(26, 5).Trim()) });
                        numberOfConects++;
                    }
                    //foreach (List<int> pair in listOfConectPairs)
                    //{
                    //    foreach (int atom in pair)
                    //    {
                    //        Debug.Log(atom);
                    //    }
                    //}

                }

                else if (Configurator.GetRepresentationStyle() == RepresentationStyles.ribbon
                    && (line.Substring(0, 6).Trim() == "HELIX" || line.Substring(0, 6).Trim() == "SHEET"))
                {
                    ifStride = false;
                    Debug.Log(line);
                    SecondaryStructureParser thisSecondaryStructure = new SecondaryStructureParser(line, ifStride);
                    listOfSecondaryStructures.Add(thisSecondaryStructure);
                    switch (thisSecondaryStructure.GetTypeOfStructure())
                    {
                        case "AlphaHelix":
                            listOfAlphaHelices.Add(thisSecondaryStructure);
                            break;

                        case "Strand":
                            listOfBetaSheets.Add(thisSecondaryStructure);
                            break;
                    }
                }
                


            }

            file.Close();

            if (ifStride)
            {
                StrideAnalize();
            }

            
        }


        void StrideAnalize()
        {
            
            StreamReader strideFile = new StreamReader(client.OpenRead("http://webclu.bio.wzw.tum.de/cgi-bin/stride/stridecgi.py?pdbid=" + Configurator.GetPdbID() + "&action=compute"));
            while (!strideFile.EndOfStream)
            {
                string strideLine = strideFile.ReadLine();
                if (strideLine.Contains("LOC"))
                {

                    SecondaryStructureParser thisSecondaryStructure = new SecondaryStructureParser(strideLine, ifStride);
                    listOfSecondaryStructures.Add(thisSecondaryStructure);
                    switch (thisSecondaryStructure.GetTypeOfStructure())
                    {
                        case "AlphaHelix":
                            listOfAlphaHelices.Add(thisSecondaryStructure);
                            break;

                        case "Strand":
                            listOfBetaSheets.Add(thisSecondaryStructure);
                            break;
                    }
                }
            }
            strideFile.Close();
            
        }
    }
}
