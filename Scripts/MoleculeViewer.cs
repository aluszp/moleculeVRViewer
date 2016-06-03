using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using Assets.Code.Sources;

public class MoleculeViewer : MonoBehaviour
{
    public GameObject atomPrefab;
    public GameObject bondPrefab;
    //public GameObject cartoonLinePrefab;
    public GameObject pipePrefab;
    public GameObject cylinderHelixPrefab;
    public GameObject arrowSheetPrefab;
    public Camera mainCamera;
    public Camera leapMotionCamera;

    Vector3 target; //geometric center of molecule
    bool rotating; //initial rotating

    FileReader fileReader;
    ColouringApplier colouringApplier;

    bool hasHydrogens;
    Dictionary<string, Color> chainColorDictionary;
    Dictionary<string, Color> residueColorDictionary;
    Dictionary<string, Color> atomColorDictionary;
    Dictionary<string, List<Vector3>> dictionaryOfBackbone;
    Dictionary<string, List<Vector3>> dictionaryOfNucleicBackbone;
    Dictionary<SecondaryStructureParser, List<Vector3>> dictionaryOfAlphaHelix;
    Dictionary<SecondaryStructureParser, List<AtomParser>> dictionaryOfBetaSheet;
    List<AtomParser> listOfAtoms;
    List<List<int>> listOfConectPairs; //pairs that are connected in CONECT section in pdb
    int numberOfConects;
    int numberOfChains;
    int numberOfAtoms; //number of atoms and heteroatoms
    string chains; //for dictionaries
    string residues; //for dictionaries
  
    Vector3 sumOfPositions;

    //initialization
    void Start()
    {
        target = new Vector3(0, 0, 0);
        rotating = true;

        colouringApplier = new ColouringApplier();

        dictionaryOfBackbone = new Dictionary<string, List<Vector3>>();
        dictionaryOfNucleicBackbone = new Dictionary<string, List<Vector3>>();
        dictionaryOfAlphaHelix = new Dictionary<SecondaryStructureParser, List<Vector3>>();
        dictionaryOfBetaSheet = new Dictionary<SecondaryStructureParser, List<AtomParser>>();

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

        //analyze
        chains = chains.Remove(chains.Length - 1);
        print(chains);
        numberOfChains = chains.Split(',').Length;
        numberOfAtoms = listOfAtoms.Count;
        target = sumOfPositions / numberOfAtoms;
        transform.position = target - new Vector3(0, 0, 50);
        transform.LookAt(target);

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
        else if (Configurator.GetRepresentationStyle() == RepresentationStyles.oldRibbon)
        {
            OldRibbonRepresentation(listOfAtoms);
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
                    colouringApplier.SubunitsColouring(thisAtom, atomBall, chainColorDictionary);
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
            colouringApplier.SubunitsColouring(thisAtom1, bond, chainColorDictionary);
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
            else if (thisAtom.GetAtomName() == "P" || thisAtom.GetAtomName() == "O5'" || thisAtom.GetAtomName() == "O3'"
                || thisAtom.GetAtomName() == "C5'" || thisAtom.GetAtomName() == "OP1'" || thisAtom.GetAtomName() == "OP2'")
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

            Vector3[] backbonePointsNew = CurvesSmoother.MakeSmoothCurve(backbonePoints, 5.0f);


            PipeTheLine backbonePipe = pipePrefab.GetComponent<PipeTheLine>();

            backbonePipe.DrawThePipe(backbonePointsNew, pipePrefab, 0.4f, 0.6f);


            //GameObject helix = (GameObject)Instantiate(cartoonLinePrefab, aCarbons[0], Quaternion.identity);
            //LineRenderer helixLine = helix.GetComponent<LineRenderer>();
            //List<Vector3> listOfHelixPoints = new List<Vector3>();

            //for (int c=0; c<aCarbonsNew.Length-1; c++)
            //{
            //    float t = aCarbonsNew[c].z;
            //    listOfHelixPoints.Add(new Vector3((float)(Math.Cos(t)), (float)(Math.Sin(t)), t));
            //    Vector3 middleHelixPoint = Vector3.Lerp(aCarbonsNew[c], aCarbonsNew[c+1], 0.5f);
            //    float u = middleHelixPoint.z;
            //    listOfHelixPoints.Add(new Vector3((float)(Math.Cos(u)), (float)(Math.Sin(u)), u));
            //}

            //Vector3[] helixPoints = listOfHelixPoints.ToArray();
            //helixLine.SetVertexCount(helixPoints.Length);
            //helixLine.SetPositions(helixPoints);
            //helixLine.SetWidth(0.5f, 0.5f);


        }
    }
    void OldRibbonRepresentation(List<AtomParser> listOfAtoms)
    {
        BackboneRepresentation(listOfAtoms);
        if (dictionaryOfBackbone.Count == 0)
        {
            print("This is not a protein! Can't draw secondary structures!");
        }
        else
        {
            AnalizeSecondaryStructure(listOfAtoms);

            foreach (List<Vector3> atomHelixPositions in dictionaryOfAlphaHelix.Values)
            {
                //GameObject cylinderHelix =
                //    (GameObject)Instantiate(cylinderHelixPrefab, atomHelixPositions[0], Quaternion.identity);
                //float lengthOfHelix = Vector3.Distance(atomHelixPositions[0], atomHelixPositions[atomHelixPositions.Count-1]);
                //cylinderHelix.transform.position = new Vector3(atomHelixPositions[0].x, lengthOfHelix / 2, atomHelixPositions[0].z);
                //cylinderHelix.transform.localScale =
                //    new Vector3(2, lengthOfHelix, 2);
                //cylinderHelix.transform.rotation =
                //    Quaternion.LookRotation(atomHelixPositions[atomHelixPositions.Count - 1]);
                
                Vector3 cylinderWidthOffset = atomHelixPositions[2] - atomHelixPositions[0];
                Vector3 startingPointOfHelix = atomHelixPositions[0] + cylinderWidthOffset/2.0f;
                Vector3 endingPointOfHelix = atomHelixPositions[atomHelixPositions.Count - 1] + cylinderWidthOffset / 2.0f;
                Vector3 cylinderLengthOffset = endingPointOfHelix - startingPointOfHelix;
                
                Vector3 scale = new Vector3(4.6f, cylinderLengthOffset.magnitude/2.0f, 4.6f);
                Vector3 position = startingPointOfHelix + (cylinderLengthOffset / 2.0f);


                GameObject cylinderHelix = (GameObject)Instantiate(cylinderHelixPrefab, position, Quaternion.identity);
                cylinderHelix.transform.up = cylinderLengthOffset;
                cylinderHelix.transform.localScale = scale;
                

            }
        }

    }
    void AnalizeSecondaryStructure(List<AtomParser> listOfAtoms)
    {

        List<SecondaryStructureParser> listOfSecondaryStructures = fileReader.StrideAnalize();
        List<SecondaryStructureParser> listOfAlphaHelices = new List<SecondaryStructureParser>();
        List<SecondaryStructureParser> listOfBetaSheets = new List<SecondaryStructureParser>();

        foreach (SecondaryStructureParser thisSecondaryStructure in listOfSecondaryStructures)
        {
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



        foreach (AtomParser thisAtom in listOfAtoms)
        {
            foreach (SecondaryStructureParser alphaHelix in listOfAlphaHelices)
            {
                if (thisAtom.GetAtomName() == "CA"
                    && thisAtom.GetResidueSequence() >= alphaHelix.GetStartingResidue()
                    && thisAtom.GetResidueSequence() <= alphaHelix.GetEndingResidue())
                {
                    if (dictionaryOfAlphaHelix.ContainsKey(alphaHelix))
                    {
                        dictionaryOfAlphaHelix[alphaHelix].Add(thisAtom.GetAtomPosition());
                    }
                    else
                    {
                        List<Vector3> listOfHelixVectors = new List<Vector3>();
                        listOfHelixVectors.Add(thisAtom.GetAtomPosition());
                        dictionaryOfAlphaHelix.Add(alphaHelix, listOfHelixVectors);
                    }
                }
            }

            foreach (SecondaryStructureParser betaSheet in listOfBetaSheets)
            {
                if (thisAtom.GetAtomName() == "CA" || thisAtom.GetAtomName() == "N"
                    && thisAtom.GetResidueSequence() >= betaSheet.GetStartingResidue()
                    && thisAtom.GetResidueSequence() <= betaSheet.GetEndingResidue())
                {
                    if (dictionaryOfBetaSheet.ContainsKey(betaSheet))
                    {
                        dictionaryOfBetaSheet[betaSheet].Add(thisAtom);
                    }
                    else
                    {
                        List<AtomParser> listOfSheetAtoms = new List<AtomParser>();
                        listOfSheetAtoms.Add(thisAtom);
                        dictionaryOfBetaSheet.Add(betaSheet, listOfSheetAtoms);
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
        PerformKeyboardActions(speed);
        
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

        if (Input.GetKey(KeyCode.Return))
        {
            if (mainCamera.isActiveAndEnabled)
            {
                mainCamera.enabled = false;
                leapMotionCamera.enabled = true;
            }
            else
            {
                mainCamera.enabled = true;
                leapMotionCamera.enabled = false;
            }
            

        }
    }



}
