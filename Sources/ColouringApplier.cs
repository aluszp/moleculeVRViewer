using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Sources
{
    public class ColouringApplier
    {
        public Dictionary<string, Color> PrepareForSubunitsColouring(string chains)
        {
            
            Dictionary<string, Color> chainColorDictionary = new Dictionary<string, Color>();
            foreach (string chain in chains.Split(','))
            {
                if (!chainColorDictionary.ContainsKey(chain))
                {
                    chainColorDictionary.Add(chain, new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
                }
            }
            return chainColorDictionary;
        }

        public Dictionary<string, Color> PrepareForResiduesColouring(string residues)
        {
            Dictionary<string, Color> residueColorDictionary = new Dictionary<string, Color>();
            residueColorDictionary.Add("ALA", new Color32(200, 200, 200, 1));
            residueColorDictionary.Add("ASN", new Color32(0, 220, 220, 1));
            residueColorDictionary.Add("ASP", new Color32(230, 10, 10, 1));
            residueColorDictionary.Add("ARG", new Color32(20, 90, 255, 1));
            residueColorDictionary.Add("CYS", new Color32(230, 230, 0, 1));
            residueColorDictionary.Add("GLN", new Color32(0, 220, 220, 1));
            residueColorDictionary.Add("GLU", new Color32(230, 10, 10, 1));
            residueColorDictionary.Add("GLY", new Color32(235, 235, 235, 1));
            residueColorDictionary.Add("HIS", new Color32(130, 130, 210, 1));
            residueColorDictionary.Add("ILE", new Color32(15, 130, 15, 1));
            residueColorDictionary.Add("LEU", new Color32(15, 130, 15, 1));
            residueColorDictionary.Add("LYS", new Color32(20, 90, 255, 1));
            residueColorDictionary.Add("MET", new Color32(230, 230, 0, 1));
            residueColorDictionary.Add("PHE", new Color32(50, 50, 170, 1));
            residueColorDictionary.Add("PRO", new Color32(220, 150, 130, 1));
            residueColorDictionary.Add("SER", new Color32(250, 150, 0, 1));
            residueColorDictionary.Add("THR", new Color32(250, 150, 0, 1));
            residueColorDictionary.Add("TYR", new Color32(50, 50, 170, 1));
            residueColorDictionary.Add("TRP", new Color32(180, 90, 180, 1));
            residueColorDictionary.Add("VAL", new Color32(15, 130, 15, 1));
            residueColorDictionary.Add("HOH", Color.red);

            residues = residues.Remove(residues.Length - 1);
            foreach (string residue in residues.Split(' '))
            {
                if (!residueColorDictionary.ContainsKey(residue))
                {
                    residueColorDictionary.Add(residue, new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
                }
            }
            return residueColorDictionary;
        }

        public Dictionary<string, Color> PrepareForCpkColouring()
        {
            Dictionary<string, Color> atomColorDictionary = new Dictionary<string, Color>();
            atomColorDictionary.Add("H", Color.white);
            atomColorDictionary.Add("C", Color.black);
            atomColorDictionary.Add("N", Color.blue);
            atomColorDictionary.Add("O", Color.red);
            atomColorDictionary.Add("F", Color.green);
            atomColorDictionary.Add("CL", Color.green);
            atomColorDictionary.Add("P", new Color32(255, 153, 0, 1)); //orange
            atomColorDictionary.Add("S", Color.yellow);
            atomColorDictionary.Add("BR", new Color32(153, 0, 0, 1));//dark red
            atomColorDictionary.Add("MG", new Color32(0, 102, 0, 1)); //dark green

            return atomColorDictionary;
        }

        public void CpkColouring(AtomParser thisAtom, GameObject atomBall, Dictionary<string, Color> atomColorDictionary)
        {
            if (atomColorDictionary.ContainsKey(thisAtom.GetElementType())) 
            {
                atomBall.GetComponent<Renderer>().material.color = atomColorDictionary[thisAtom.GetElementType()];
            }
            else
            {
                atomBall.GetComponent<Renderer>().material.color = Color.magenta;
            }


        }

        public void CpkColouringForLines(AtomParser thisAtom1, AtomParser thisAtom2, LineRenderer lineToColour,
            Dictionary<string, Color> atomColorDictionary)
        {
            Color colour1, colour2;
            if (atomColorDictionary.ContainsKey(thisAtom1.GetElementType()))
            {
                colour1 = atomColorDictionary[thisAtom1.GetElementType()];
            }
            else
            {
                colour1 = Color.magenta;
            }
            if (atomColorDictionary.ContainsKey(thisAtom2.GetElementType()))
            {
                colour2 = atomColorDictionary[thisAtom2.GetElementType()];
            }
            else
            {
                colour2 = Color.magenta;
            }
            
            lineToColour.SetColors(colour1, colour2);
            lineToColour.material = new Material(Shader.Find("Particles/Alpha Blended"));
        }

        public void SubunitsColouring(string chainID, GameObject thingToColour, 
            Dictionary<string, Color> chainColorDictionary)
        {

            thingToColour.GetComponent<Renderer>().material.color = chainColorDictionary[chainID];

        }

        public void ResiduesColouring(AtomParser thisAtom, GameObject thingToColour, 
            Dictionary<string, Color> residueColorDictionary)
        {
            //http://life.nthu.edu.tw/~fmhsu/rasframe/COLORS.HTM

            thingToColour.GetComponent<Renderer>().material.color = residueColorDictionary[thisAtom.GetResidueName()];

        }

        public void SecondaryStructuresColouring(GameObject thingToColour)
        {
            if (thingToColour.name == SecondaryStructures.alphaHelix)
            {
                thingToColour.GetComponent<Renderer>().material.color = Color.red;
            }
            else if (thingToColour.name == SecondaryStructures.betaSheet)
            {
                thingToColour.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else
            {
                thingToColour.GetComponent<Renderer>().material.color = Color.white;
            }

        }
    }
}
