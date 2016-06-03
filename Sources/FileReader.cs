using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private List<AtomParser> listOfAtoms;
        private Vector3 sumOfPositions;
        private string chains; //for dictionaries
        private int numberOfConects;
        private string residues; //for dictionaries
        private List<List<int>> listOfConectPairs; //pairs that are connected in CONECT section in pdb

        public FileReader()
        {
            client = new WebClient();
            listOfAtoms = new List<AtomParser>();
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
            return chains;
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

        public void ReadFile()
        {
            //System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Dell 15z\Studia\NOWA PRACA MGR\4QRV.pdb");
            StreamReader file = new StreamReader(client.OpenRead(URL));
            listOfConectPairs = new List<List<int>>();

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



            }

            file.Close();
        }


        public List<SecondaryStructureParser> StrideAnalize()
        {
            List<SecondaryStructureParser> listOfSecondaryStructures = new List<SecondaryStructureParser>();
            StreamReader strideFile = new StreamReader(client.OpenRead("http://webclu.bio.wzw.tum.de/cgi-bin/stride/stridecgi.py?pdbid=" + Configurator.GetPdbID() + "&action=compute"));
            while (!strideFile.EndOfStream)
            {
                string strideLine = strideFile.ReadLine();
                if (strideLine.Contains("LOC"))
                {

                    SecondaryStructureParser thisSecondaryStructure = new SecondaryStructureParser(strideLine);
                    listOfSecondaryStructures.Add(thisSecondaryStructure);
                }
            }
            strideFile.Close();
            return listOfSecondaryStructures;
        }
    }
}
