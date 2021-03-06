﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.Sources;
using System.Linq;

namespace Assets.Code.Scripts
{
    public class MoleculeViewer : MonoBehaviour
    {
        public GameObject atomPrefab;
        public GameObject bondPrefab;
        //public GameObject cartoonLinePrefab;
        public GameObject pipePrefab;
        public GameObject helixPrefab;
        public GameObject sheetPrefab;
        public GameObject arrowHeadPrefab;
        public GameObject mainCamera;
        public Text startingText;
        public GameObject leftHand;


        static public Vector3 target; //geometric center of molecule
        bool rotating; //initial rotating
        bool enterPressed = false;
        string startingString;


        FileReader fileReader;
        ColouringApplier colouringApplier;

        bool hasHydrogens;
        Dictionary<string, Color> chainColorDictionary;
        Dictionary<string, Color> residueColorDictionary;
        Dictionary<string, Color> atomColorDictionary;
        Dictionary<string, List<Vector3>> dictionaryOfBackbone;
        Dictionary<string, List<Vector3>> dictionaryOfNucleicBackbone;
        List<AtomParser> listOfAtoms;
        List<List<int>> listOfConectPairs; //pairs that are connected in CONECT section in pdb
        List<SecondaryStructureParser> listOfSecondaryStructures;
        List<SecondaryStructureParser> listOfAlphaHelices;
        List<SecondaryStructureParser> listOfBetaSheets;
        int numberOfConects;
        int numberOfChains;
        int numberOfAtoms; //number of atoms and heteroatoms
        string chains; //for dictionaries
        string residues; //for dictionaries
        string title;

        Vector3 sumOfPositions;

        //initialization
        void Start()
        {

            target = new Vector3(0, 0, 0);
            rotating = true;

            colouringApplier = new ColouringApplier();

            dictionaryOfBackbone = new Dictionary<string, List<Vector3>>();
            dictionaryOfNucleicBackbone = new Dictionary<string, List<Vector3>>();


            //read file
            fileReader = new FileReader();
            fileReader.ReadFile();

            hasHydrogens = fileReader.GetHasHydrogens();
            listOfAtoms = fileReader.GetListOfAtoms();
            sumOfPositions = fileReader.GetSumOfPositions();
            chains = fileReader.GetChains();
            numberOfConects = fileReader.GetNumberOfConects();
            residues = fileReader.GetResidues();
            listOfConectPairs = fileReader.GetListOfConectPairs();
            listOfSecondaryStructures = fileReader.GetListOfSecondaryStructures().OrderBy(o => o.GetStartingResidue()).ToList();
            title = fileReader.GetTitle();

            //analyze
            numberOfChains = chains.Split(',').Length;
            numberOfAtoms = listOfAtoms.Count;
            target = sumOfPositions / numberOfAtoms;
            transform.position = target - new Vector3(0, 0, 50);
            transform.LookAt(target);
            startingString = title;

            switch (Configurator.GetColouring())
            {
                case (Colouring.subunits):
                    chainColorDictionary = colouringApplier.PrepareForSubunitsColouring(chains);
                    break;

                case (Colouring.residues):
                    residueColorDictionary = colouringApplier.PrepareForResiduesColouring(residues);
                    break;

                case (Colouring.cpk):
                    atomColorDictionary = colouringApplier.PrepareForCpkColouring();
                    break;
            }

            Draw(listOfAtoms);

        }

        void Draw(List<AtomParser> listOfAtoms)
        {
            if (Configurator.GetRepresentationStyle() == RepresentationStyles.vanDerWaals)
            {
                VdwRepresentation(listOfAtoms, 2f);
            }
            else if (Configurator.GetRepresentationStyle() == RepresentationStyles.lines)
            {
                LinesRepresentation(listOfAtoms);
            }
            else if (Configurator.GetRepresentationStyle() == RepresentationStyles.backbone)
            {
                BackboneRepresentation(listOfAtoms);
            }
            else if (Configurator.GetRepresentationStyle() == RepresentationStyles.ribbon)
            {
                RibbonRepresentation(listOfAtoms);
            }
            else if (Configurator.GetRepresentationStyle() == RepresentationStyles.ballsAndSticks)
            {
                VdwRepresentation(listOfAtoms, 0.6f);
                LinesRepresentation(listOfAtoms);
            }
        }

        void VdwRepresentation(List<AtomParser> listOfAtoms, float radiusMultiplier)
        {
            foreach (AtomParser thisAtom in listOfAtoms)
            {
                if ((!Configurator.GetHideHydrogens()) || (Configurator.GetHideHydrogens() && thisAtom.GetElementType() != "H"))
                {

                    GameObject atomBall = (GameObject)Instantiate(atomPrefab, thisAtom.GetAtomPosition(), Quaternion.identity);
                    atomBall.name = thisAtom.GetAtomName();
                    AtomData atomData = atomBall.GetComponent<AtomData>();
                    atomData.chainID = thisAtom.GetChainID();
                    atomData.elementType = thisAtom.GetElementType();
                    atomData.resName = thisAtom.GetResidueName();
                    atomData.resSeq = thisAtom.GetResidueSequence();
                    atomData.serial = thisAtom.GetSerial();
                    atomData.atomName = thisAtom.GetAtomName();

                    switch (thisAtom.GetElementType())
                    {
                        case "F":
                            atomBall.transform.localScale = new Vector3(1.47f, 1.47f, 1.47f) * radiusMultiplier;
                            break;
                        case "CL":
                            atomBall.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f) * radiusMultiplier;
                            break;
                        case "P":
                            atomBall.transform.localScale = new Vector3(1.880f, 1.880f, 1.880f) * radiusMultiplier;
                            break;
                        case "MG":
                            atomBall.transform.localScale = new Vector3(1.73f, 1.73f, 1.73f) * radiusMultiplier;
                            break;
                        case "CA":
                            atomBall.transform.localScale = new Vector3(1.948f, 1.948f, 1.948f) * radiusMultiplier;
                            break;
                        case "FE":
                            atomBall.transform.localScale = new Vector3(1.948f, 1.948f, 1.948f) * radiusMultiplier;
                            break;
                        case "ZN":
                            atomBall.transform.localScale = new Vector3(1.148f, 1.148f, 1.148f) * radiusMultiplier;
                            break;
                        case "CD":
                            atomBall.transform.localScale = new Vector3(1.748f, 1.748f, 1.748f) * radiusMultiplier;
                            break;
                        case "I":
                            atomBall.transform.localScale = new Vector3(1.748f, 1.748f, 1.748f) * radiusMultiplier;
                            break;

                    }

                    if (Configurator.GetHideHydrogens() || !hasHydrogens)
                    {
                        switch (thisAtom.GetElementType())
                        {
                            case "C":
                                atomBall.transform.localScale = new Vector3(1.872f, 1.872f, 1.872f) * radiusMultiplier;
                                break;
                            case "N":
                                atomBall.transform.localScale = new Vector3(1.507f, 1.507f, 1.507f) * radiusMultiplier;
                                break;
                            case "O":
                                atomBall.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f) * radiusMultiplier;
                                break;
                            case "S":
                                atomBall.transform.localScale = new Vector3(1.848f, 1.848f, 1.848f) * radiusMultiplier;
                                break;

                        }
                    }
                    else if (!Configurator.GetHideHydrogens() && hasHydrogens)
                    {
                        switch (thisAtom.GetElementType())
                        {
                            case "H":
                                atomBall.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f) * radiusMultiplier;
                                break;
                            case "C":
                                atomBall.transform.localScale = new Vector3(1.548f, 1.548f, 1.548f) * radiusMultiplier;
                                break;
                            case "N":
                                atomBall.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f) * radiusMultiplier;
                                break;
                            case "O":
                                atomBall.transform.localScale = new Vector3(1.348f, 1.348f, 1.348f) * radiusMultiplier;
                                break;
                            case "S":
                                atomBall.transform.localScale = new Vector3(1.808f, 1.808f, 1.808f) * radiusMultiplier;
                                break;
                        }
                    }
                    if (Configurator.GetColouring() == Colouring.cpk)
                    {
                        colouringApplier.CpkColouring(thisAtom, atomBall, atomColorDictionary);
                    }
                    else if (Configurator.GetColouring() == Colouring.subunits)
                    {
                        colouringApplier.SubunitsColouring(thisAtom.GetChainID(), atomBall, chainColorDictionary);
                    }
                    else if (Configurator.GetColouring() == Colouring.residues)
                    {
                        colouringApplier.ResiduesColouring(thisAtom, atomBall, residueColorDictionary);
                    }
                }

            }
        }

        void LinesRepresentation(List<AtomParser> listOfAtoms) //https://www.umass.edu/microbio/rasmol/rasbonds.htm
        {
            if (numberOfAtoms - numberOfChains > numberOfConects)
            {
                if (numberOfAtoms > 255)
                {
                    foreach (AtomParser thisAtom1 in listOfAtoms)
                    {
                        foreach (AtomParser thisAtom2 in listOfAtoms)
                        {
                            if (((thisAtom1.GetElementType() != "H" && thisAtom2.GetElementType() != "H")
                                && Vector3.Distance(thisAtom1.GetAtomPosition(), thisAtom2.GetAtomPosition()) >= 0.4f
                                && Vector3.Distance(thisAtom1.GetAtomPosition(), thisAtom2.GetAtomPosition()) <= 1.9f)
                                || (!Configurator.GetHideHydrogens()
                                && ((thisAtom1.GetElementType() == "H" || thisAtom2.GetElementType() == "H")
                                && Vector3.Distance(thisAtom1.GetAtomPosition(), thisAtom2.GetAtomPosition()) >= 0.4f
                                && Vector3.Distance(thisAtom1.GetAtomPosition(), thisAtom2.GetAtomPosition()) <= 1.2f)))
                            {
                                DrawLines(thisAtom1, thisAtom2);
                            }

                        }

                    }
                }
                else
                {
                    foreach (AtomParser thisAtom1 in listOfAtoms)
                    {
                        foreach (AtomParser thisAtom2 in listOfAtoms)
                        {
                            if (((!Configurator.GetHideHydrogens()) ||
                                (Configurator.GetHideHydrogens() &&
                                (thisAtom1.GetElementType() != "H" || thisAtom2.GetElementType() != "H")))
                                && Vector3.Distance(thisAtom1.GetAtomPosition(), thisAtom2.GetAtomPosition()) >= 0.4f
                                && Vector3.Distance(thisAtom1.GetAtomPosition(), thisAtom2.GetAtomPosition()) <= (thisAtom1.GetCovalentRadii() + thisAtom2.GetCovalentRadii() + 0.56))
                            {
                                DrawLines(thisAtom1, thisAtom2);
                            }


                        }
                    }
                }

            }
            else
            {
                foreach (List<int> pairOfAtoms in listOfConectPairs)
                {
                    if ((!Configurator.GetHideHydrogens()) ||
                        (Configurator.GetHideHydrogens() &&
                        (listOfAtoms[pairOfAtoms[0]].GetElementType() != "H"
                        || listOfAtoms[pairOfAtoms[1]].GetElementType() != "H")))
                    {
                        DrawLines(listOfAtoms[pairOfAtoms[0]], listOfAtoms[pairOfAtoms[1]]);
                    }
                }
            }
        }

        void DrawLines(AtomParser thisAtom1, AtomParser thisAtom2)
        {
            GameObject bond = (GameObject)Instantiate(bondPrefab, thisAtom1.GetAtomPosition(), Quaternion.identity);
            LineRenderer bondLine = bond.GetComponent<LineRenderer>();
            bondLine.SetWidth(0.2F, 0.2F);
            Vector3[] atoms = new Vector3[2];
            atoms[0] = thisAtom1.GetAtomPosition();
            atoms[1] = thisAtom2.GetAtomPosition();
            bondLine.SetPositions(atoms);

            if (Configurator.GetColouring() == Colouring.residues && thisAtom1.GetResidueName() == thisAtom2.GetResidueName())
            {
                colouringApplier.ResiduesColouring(thisAtom1, bond, residueColorDictionary);
            }

            if (Configurator.GetColouring() == Colouring.subunits && thisAtom1.GetChainID() == thisAtom2.GetChainID())
            {
                colouringApplier.SubunitsColouring(thisAtom1.GetChainID(), bond, chainColorDictionary);
            }

            if (Configurator.GetColouring() == Colouring.cpk)
            {
                colouringApplier.CpkColouringForLines(thisAtom1, thisAtom2, bondLine, atomColorDictionary);
            }
        }

        void BackboneRepresentation(List<AtomParser> listOfAtoms)
        {


            foreach (AtomParser thisAtom in listOfAtoms)
            {

                if (thisAtom.GetAtomName() == "CA")
                {
                    if (dictionaryOfBackbone.ContainsKey(thisAtom.GetChainID()))
                    {
                        dictionaryOfBackbone[thisAtom.GetChainID()].Add(thisAtom.GetAtomPosition());

                    }
                    else
                    {
                        List<Vector3> listOfVectors = new List<Vector3>();
                        listOfVectors.Add(thisAtom.GetAtomPosition());
                        dictionaryOfBackbone.Add(thisAtom.GetChainID(), listOfVectors);
                    }
                }
                else if (thisAtom.GetAtomName() == "P" || thisAtom.GetAtomName() == "OP1'" || thisAtom.GetAtomName() == "OP2'")
                {
                    if (dictionaryOfNucleicBackbone.ContainsKey(thisAtom.GetChainID()))
                    {
                        dictionaryOfNucleicBackbone[thisAtom.GetChainID()].Add(thisAtom.GetAtomPosition());

                    }
                    else
                    {
                        List<Vector3> listOfVectors = new List<Vector3>();
                        listOfVectors.Add(thisAtom.GetAtomPosition());
                        dictionaryOfNucleicBackbone.Add(thisAtom.GetChainID(), listOfVectors);
                    }
                }



            }

            if (dictionaryOfBackbone.Count == 0)
            {
                dictionaryOfBackbone = dictionaryOfNucleicBackbone;
            }


            foreach (string chainKey in dictionaryOfBackbone.Keys)
            {
                Vector3[] backbonePoints = dictionaryOfBackbone[chainKey].ToArray();

                Vector3[] backbonePointsNew = CurvesSmoother.MakeSmoothCurve(backbonePoints, 2.0f);

                GameObject pipe = (GameObject)Instantiate(pipePrefab, Vector3.zero, Quaternion.identity);
                PipeTheLine backbonePipe = pipe.GetComponent<PipeTheLine>();
                backbonePipe.DrawThePipe(backbonePointsNew, pipe, 0.4f, 0.4f);
                if (Configurator.GetColouring() == Colouring.subunits)
                {
                    colouringApplier.SubunitsColouring(chainKey, pipe, chainColorDictionary);
                }

            }
        }
        void RibbonRepresentation(List<AtomParser> listOfAtoms)
        {

            List<AtomParser> listOfAlphaCarbons = new List<AtomParser>();
            List<Vector3> singleBackboneFragments = new List<Vector3>();
            Dictionary<string, List<List<Vector3>>> dictionaryOfBackboneFragments = new Dictionary<string, List<List<Vector3>>>();
            Dictionary<string, List<List<Vector3>>> dictionaryOfHelixFragments = new Dictionary<string, List<List<Vector3>>>();
            Dictionary<string, List<List<Vector3>>> dictionaryOfSheetFragments = new Dictionary<string, List<List<Vector3>>>();

            foreach (AtomParser thisAtom in listOfAtoms)
            {
                if (thisAtom.GetAtomName() == "CA")
                {
                    listOfAlphaCarbons.Add(thisAtom);

                }
            }

            if (listOfAlphaCarbons.Count == 0)
            {
                startingString = title + " is not a protein! Switched to backbone representation.";
                BackboneRepresentation(listOfAtoms);
                return;
            }

            int ssi = 0; //secondary structure index


            for (int i = 0; i < listOfAlphaCarbons.Count; i++)
            {
                if (ssi == listOfSecondaryStructures.Count) //loop already passed through the last secondary structure- get tail
                {
                    singleBackboneFragments.Add(listOfAlphaCarbons[i].GetAtomPosition());
                }
                if (i == listOfAlphaCarbons.Count - 1 && ssi == listOfSecondaryStructures.Count) //add tail to dictionary
                {
                    if (dictionaryOfBackboneFragments.ContainsKey(listOfAlphaCarbons[i].GetChainID()))
                    {
                        dictionaryOfBackboneFragments[listOfAlphaCarbons[i].GetChainID()].Add(singleBackboneFragments);

                    }
                    else
                    {
                        List<List<Vector3>> listOfBackboneFragments = new List<List<Vector3>>();
                        listOfBackboneFragments.Add(singleBackboneFragments);
                        dictionaryOfBackboneFragments.Add(listOfAlphaCarbons[i].GetChainID(), listOfBackboneFragments);
                    }
                }
                if (ssi < listOfSecondaryStructures.Count
                    && listOfAlphaCarbons[i].GetResidueSequence() <= listOfSecondaryStructures[ssi].GetStartingResidue())
                //if position before beginning of secondary structure, add to list of backbone fragment
                {
                    singleBackboneFragments.Add(listOfAlphaCarbons[i].GetAtomPosition());
                }

                else if (ssi < listOfSecondaryStructures.Count
                    && listOfAlphaCarbons[i].GetResidueSequence() > listOfSecondaryStructures[ssi].GetStartingResidue())
                //if position belongs to secondary structure, add list to dictionary and start new list, counting from the
                //end of this secondary structure
                {

                    if (dictionaryOfBackboneFragments.ContainsKey(listOfAlphaCarbons[i].GetChainID()))
                    {
                        dictionaryOfBackboneFragments[listOfAlphaCarbons[i].GetChainID()].Add(singleBackboneFragments);

                    }
                    else
                    {
                        List<List<Vector3>> listOfBackboneFragments = new List<List<Vector3>>();
                        listOfBackboneFragments.Add(singleBackboneFragments);
                        dictionaryOfBackboneFragments.Add(listOfAlphaCarbons[i].GetChainID(), listOfBackboneFragments);
                    }

                    if (listOfSecondaryStructures[ssi].GetTypeOfStructure() == SecondaryStructures.alphaHelix)
                    {
                        List<Vector3> singleHelixFragment = new List<Vector3>();
                        for (int z = i - 1; z < i + listOfSecondaryStructures[ssi].GetLength() - 1; z++) //-1, cause we need index
                        {
                            if (i == 0)
                                z = 0;

                            if (z == listOfAlphaCarbons.Count)
                                break;

                            singleHelixFragment.Add(listOfAlphaCarbons[z].GetAtomPosition());

                            if (z == i + listOfSecondaryStructures[ssi].GetLength() - 2
                                && z != listOfAlphaCarbons.Count - 1 && listOfAlphaCarbons[z + 1].GetChainID() == listOfAlphaCarbons[z].GetChainID()
                                && (ssi == listOfSecondaryStructures.Count - 1 ||
                                (ssi != listOfSecondaryStructures.Count - 1
                                && listOfSecondaryStructures[ssi + 1].GetStartingResidue() != listOfSecondaryStructures[ssi].GetEndingResidue())))
                            {

                                List<Vector3> mediatingBackboneFragments = new List<Vector3>();
                                mediatingBackboneFragments.Add(listOfAlphaCarbons[z].GetAtomPosition());
                                mediatingBackboneFragments.Add(listOfAlphaCarbons[z + 1].GetAtomPosition());

                                if (dictionaryOfBackboneFragments.ContainsKey(listOfAlphaCarbons[z].GetChainID()))
                                {
                                    dictionaryOfBackboneFragments[listOfAlphaCarbons[z].GetChainID()].Add(mediatingBackboneFragments);

                                }
                                else
                                {
                                    List<List<Vector3>> listOfBackboneFragments = new List<List<Vector3>>();
                                    listOfBackboneFragments.Add(mediatingBackboneFragments);
                                    dictionaryOfBackboneFragments.Add(listOfAlphaCarbons[z].GetChainID(), listOfBackboneFragments);
                                }
                            }
                        }
                        if (dictionaryOfHelixFragments.ContainsKey(listOfAlphaCarbons[i].GetChainID()))
                        {
                            dictionaryOfHelixFragments[listOfAlphaCarbons[i].GetChainID()].Add(singleHelixFragment);

                        }
                        else
                        {
                            List<List<Vector3>> listOfHelixFragments = new List<List<Vector3>>();
                            listOfHelixFragments.Add(singleHelixFragment);
                            dictionaryOfHelixFragments.Add(listOfAlphaCarbons[i].GetChainID(), listOfHelixFragments);
                        }
                    }
                    if (listOfSecondaryStructures[ssi].GetTypeOfStructure() == SecondaryStructures.betaSheet)
                    {
                        List<Vector3> singleSheetFragment = new List<Vector3>();
                        for (int z = i - 1; z < i + listOfSecondaryStructures[ssi].GetLength() - 1; z++)
                        {
                            if (i == 0)
                            {
                                z = 0;
                            }
                            if (z == listOfAlphaCarbons.Count)
                            {
                                break;
                            }
                            singleSheetFragment.Add(listOfAlphaCarbons[z].GetAtomPosition());
                            if (z == i + listOfSecondaryStructures[ssi].GetLength() - 2 && z != listOfAlphaCarbons.Count - 1
                                && listOfAlphaCarbons[z + 1].GetChainID() == listOfAlphaCarbons[z].GetChainID())
                            {

                                List<Vector3> mediatingBackboneFragments = new List<Vector3>();
                                mediatingBackboneFragments.Add(listOfAlphaCarbons[z].GetAtomPosition());
                                mediatingBackboneFragments.Add(listOfAlphaCarbons[z + 1].GetAtomPosition());


                                if (dictionaryOfBackboneFragments.ContainsKey(listOfAlphaCarbons[z].GetChainID()))
                                {
                                    dictionaryOfBackboneFragments[listOfAlphaCarbons[z].GetChainID()].Add(mediatingBackboneFragments);

                                }
                                else
                                {
                                    List<List<Vector3>> listOfBackboneFragments = new List<List<Vector3>>();
                                    listOfBackboneFragments.Add(mediatingBackboneFragments);
                                    dictionaryOfBackboneFragments.Add(listOfAlphaCarbons[z].GetChainID(), listOfBackboneFragments);
                                }
                            }
                        }
                        if (dictionaryOfSheetFragments.ContainsKey(listOfAlphaCarbons[i].GetChainID()))
                        {
                            dictionaryOfSheetFragments[listOfAlphaCarbons[i].GetChainID()].Add(singleSheetFragment);

                        }
                        else
                        {
                            List<List<Vector3>> listOfSheetFragments = new List<List<Vector3>>();
                            listOfSheetFragments.Add(singleSheetFragment);
                            dictionaryOfSheetFragments.Add(listOfAlphaCarbons[i].GetChainID(), listOfSheetFragments);
                        }
                    }

                    if (listOfSecondaryStructures[ssi].GetLength() >= 2)
                    {
                        i = i + listOfSecondaryStructures[ssi].GetLength() - 2; //-2- to cover shared points
                    }
                    else
                    {
                        i++;
                    }
                    ssi++;
                    singleBackboneFragments = new List<Vector3>();



                }

            }

            foreach (string chainKey in dictionaryOfBackboneFragments.Keys)
            {
                for (int i = 0; i < dictionaryOfBackboneFragments[chainKey].Count; i++)
                {
                    Vector3[] singleBackboneArray = dictionaryOfBackboneFragments[chainKey][i].ToArray();

                    Vector3[] singleBackboneArrayNew = CurvesSmoother.MakeSmoothCurve(singleBackboneArray, 3.0f);

                    if (singleBackboneArrayNew.Length > 1)
                    {
                        GameObject pipe = (GameObject)Instantiate(pipePrefab, Vector3.zero, Quaternion.identity);
                        PipeTheLine fragmentPipe = pipe.GetComponent<PipeTheLine>();
                        pipe.name = "backbone";
                        fragmentPipe.DrawThePipe(singleBackboneArrayNew, pipe, 0.4f, 0.4f);
                        if (Configurator.GetColouring() == Colouring.subunits)
                        {
                            colouringApplier.SubunitsColouring(chainKey, pipe, chainColorDictionary);
                        }
                        else if (Configurator.GetColouring() == Colouring.secondaryStructures)
                        {
                            colouringApplier.SecondaryStructuresColouring(pipe);
                        }
                    }
                }
            }

            foreach (string chainKey in dictionaryOfHelixFragments.Keys)
            {
                for (int i = 0; i < dictionaryOfHelixFragments[chainKey].Count; i++)
                {
                    Vector3[] singleHelixArray = dictionaryOfHelixFragments[chainKey][i].ToArray();

                    Vector3[] singleHelixArrayNew = HelixPointsMaker.MakeHelixCurve(2.3f, 5.4f, singleHelixArray[0], singleHelixArray[singleHelixArray.Length - 1]);


                    if (singleHelixArrayNew.Length > 1)
                    {
                        Vector3 helixOffset = singleHelixArray[singleHelixArray.Length - 1] - singleHelixArray[0];
                        Vector3 helixPosition = singleHelixArray[0];
                        GameObject helixPipe = (GameObject)Instantiate(helixPrefab, helixPosition, Quaternion.identity);
                        PipeTheLine fragmentPipe = helixPipe.GetComponent<PipeTheLine>();
                        helixPipe.name = SecondaryStructures.alphaHelix;
                        fragmentPipe.DrawThePipe(singleHelixArrayNew, helixPipe, 0.8f, 0.4f);
                        helixPipe.transform.forward = helixOffset;
                        if (Configurator.GetColouring() == Colouring.subunits)
                        {
                            colouringApplier.SubunitsColouring(chainKey, helixPipe, chainColorDictionary);
                        }
                        else if (Configurator.GetColouring() == Colouring.secondaryStructures)
                        {
                            colouringApplier.SecondaryStructuresColouring(helixPipe);
                        }
                    }
                }
            }
            foreach (string chainKey in dictionaryOfSheetFragments.Keys)
            {
                for (int i = 0; i < dictionaryOfSheetFragments[chainKey].Count; i++)
                {
                    Vector3[] singleSheetArray = dictionaryOfSheetFragments[chainKey][i].ToArray();

                    Vector3[] singleSheetArrayNew = CurvesSmoother.MakeSmoothCurve(singleSheetArray, 3.0f);

                    if (singleSheetArrayNew.Length > 1)
                    {
                        GameObject sheetPipe = (GameObject)Instantiate(sheetPrefab, Vector3.zero, Quaternion.identity);
                        PipeTheLine fragmentPipe = sheetPipe.GetComponent<PipeTheLine>();
                        sheetPipe.name = SecondaryStructures.betaSheet;

                        //cutting last two points, as they will be covered with arrow head
                        Vector3[] cutSingleSheetArrayNew;
                        cutSingleSheetArrayNew = singleSheetArrayNew.Take(singleSheetArrayNew.Count() - 2).ToArray();

                        fragmentPipe.DrawThePipe(cutSingleSheetArrayNew, sheetPipe, 0.5f, 0.5f);


                        Vector3 offset = singleSheetArrayNew[singleSheetArrayNew.Length - 1] - singleSheetArrayNew[singleSheetArrayNew.Length - 3];
                        Vector3 scale = new Vector3(0.6f, offset.magnitude / 4F, 0.6f);
                        Vector3 position = singleSheetArrayNew[singleSheetArrayNew.Length - 3]; //- (offset / 2.0f);

                        GameObject arrowHead = (GameObject)Instantiate
                            (arrowHeadPrefab, position, Quaternion.identity);
                        arrowHead.transform.up = offset;
                        arrowHead.transform.localScale = scale;
                        arrowHead.name = SecondaryStructures.betaSheet;
                        if (Configurator.GetColouring() == Colouring.subunits)
                        {
                            colouringApplier.SubunitsColouring(chainKey, sheetPipe, chainColorDictionary);
                            colouringApplier.SubunitsColouring(chainKey, arrowHead, chainColorDictionary);
                        }
                        else if (Configurator.GetColouring() == Colouring.secondaryStructures)
                        {
                            colouringApplier.SecondaryStructuresColouring(sheetPipe);
                            colouringApplier.SecondaryStructuresColouring(arrowHead);
                        }

                    }

                }
            }

        }

        // once per frame:
        void Update()
        {
            const float speed = 40f;
            if (rotating)
            {
                transform.RotateAround(target, new Vector3(0, 1, 1), speed * Time.deltaTime);
                if (Input.anyKey || Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    rotating = false;
                }
            }

            if (Time.time < 15)
            {
                startingText.text = startingString;
                
            }
            else
            {
                startingText.enabled = false;
                rotating = false;
            }

            PerformKeyboardActions(speed);

            if(Configurator.GetRepresentationStyle() != RepresentationStyles.ballsAndSticks && Configurator.GetRepresentationStyle() != RepresentationStyles.vanDerWaals)
            {
                leftHand.SetActive(false);
            }

        }


        private void PerformKeyboardActions(float speed)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.RotateAround(target, transform.TransformDirection(Vector3.down), speed * Time.deltaTime);

            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.RotateAround(target, transform.TransformDirection(Vector3.up), speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.RotateAround(target, transform.TransformDirection(Vector3.left), speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.RotateAround(target, transform.TransformDirection(Vector3.right), speed * Time.deltaTime);

            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.Translate(Vector3.back);

            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                transform.Translate(Vector3.forward);
            }

         


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }


        }
    }
}




