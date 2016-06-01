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
    public Vector3 target = new Vector3(0, 0, 0); //geometric center of molecule
    bool rotating = true; //initial rotating
    bool hasHydrogens = false;
    Dictionary<string, Color> chainColorDictionary = new Dictionary<string, Color>();
    Dictionary<string, Color> residueColorDictionary = new Dictionary<string, Color>();
    Dictionary<string, Color> atomColorDictionary = new Dictionary<string, Color>();
    List<AtomParser> listOfAtoms = new List<AtomParser>();
    List<List<int>> listOfConectPairs = new List<List<int>>(); //pairs that are connected in CONECT section in pdb
    int numberOfConects;
    int numberOfChains;
    int numberOfAtoms; //number of atoms and heteroatoms
    string chains; //for dictionaries
    string residues; //for dictionaries
    

    //initialization
    void Start()
    {


        WebClient client = new WebClient();
        //System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Dell 15z\Studia\NOWA PRACA MGR\4QRV.pdb");
        StreamReader file = new StreamReader(client.OpenRead("http://files.rcsb.org/download/" + MainMenu.pdbID + ".pdb"));

        Vector3 sumOfPositions = new Vector3();

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
            else if (MainMenu.colouring == "Subunits"
                && (line.Substring(0, 6).Trim() == "COMPND")
                && (line.Contains("CHAIN:")))
            {

                chains = chains + line.Substring(18, 60).Trim().Trim(';').Replace(" ", "") + ",";
                print(chains);

            }

            //creating dictionary of residues and respective colours for residue coloring method
            else if ((MainMenu.colouring == "Residues") && (line.Substring(0, 6).Trim() == "SEQRES"))

            {
                residues = residues + line.Substring(19, 51).Trim() + " ";
                print(residues);

            }

            else if ((MainMenu.representationStyle == "Lines" || MainMenu.representationStyle == "Balls and Sticks")
                && line.Substring(0, 6).Trim() == "CONECT") //counting number of bonds assigned in CONECT section
            {
                if (line.Substring(11, 5).Trim() != ""
                    && Int32.Parse(line.Substring(11, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                    && Int32.Parse(line.Substring(11, 5).Trim()) != 0)
                {
                    listOfConectPairs.Add
                        (new List<int> { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(11, 5).Trim()) });
                    numberOfConects++;
                }
                if (line.Substring(16, 5).Trim() != ""
                    && Int32.Parse(line.Substring(16, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                    && Int32.Parse(line.Substring(16, 5).Trim()) != 0)
                {
                    listOfConectPairs.Add
                        (new List<int> { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(16, 5).Trim()) });
                    numberOfConects++;
                }
                if (line.Substring(21, 5).Trim() != ""
                    && Int32.Parse(line.Substring(21, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                    && Int32.Parse(line.Substring(21, 5).Trim()) != 0)
                {
                    listOfConectPairs.Add
                        (new List<int> { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(21, 5).Trim()) });
                    numberOfConects++;
                }
                if (line.Substring(26, 5).Trim() != ""
                    && Int32.Parse(line.Substring(26, 5).Trim()) > Int32.Parse(line.Substring(6, 5).Trim())
                    && Int32.Parse(line.Substring(26, 5).Trim()) != 0)
                {
                    listOfConectPairs.Add
                        (new List<int> { Int32.Parse(line.Substring(6, 5).Trim()), Int32.Parse(line.Substring(26, 5).Trim()) });
                    numberOfConects++;
                }
            }


        }

        file.Close();
        numberOfChains = chainColorDictionary.Keys.Count;
        numberOfAtoms = listOfAtoms.Count;
        target = sumOfPositions / numberOfAtoms;
        transform.position = target - new Vector3(0, 0, 50);
        transform.LookAt(target);

        switch (MainMenu.colouring)
        {
            case ("Subunits"):
                PrepareForSubunitsColouring();
                break;

            case ("Residues"):
                PrepareForResiduesColouring();
                break;

            case ("CPK"):
                PrepareForCpkColouring();
                break;
        }

        Draw(listOfAtoms);
    }


    void PrepareForSubunitsColouring()
    {
        chains = chains.Remove(chains.Length - 1);
        foreach (string chain in chains.Split(','))
        {
            if (!chainColorDictionary.ContainsKey(chain))
            {
                chainColorDictionary.Add(chain, new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
            }
        }
    }

    void PrepareForResiduesColouring()
    {
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
    }

    void PrepareForCpkColouring()
    {
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
        
    }


    void Draw(List<AtomParser> listOfAtoms)
    {
        if (MainMenu.representationStyle == "Van der Waals")
        {
            VdwRepresentation(listOfAtoms, 2f);
        }
        else if (MainMenu.representationStyle == "Lines")
        {
            LinesRepresentation(listOfAtoms);
        }
        else if (MainMenu.representationStyle == "Cartoon")
        {
            CartoonRepresentation(listOfAtoms);
        }
        else if (MainMenu.representationStyle == "Balls and Sticks")
        {
            VdwRepresentation(listOfAtoms, 0.6f);
            LinesRepresentation(listOfAtoms);
        }
    }

    void VdwRepresentation(List<AtomParser> listOfAtoms, float radiusMultiplier)
    {
        foreach (AtomParser thisAtom in listOfAtoms)
        {
            if ((!MainMenu.hideHydrogens) || (MainMenu.hideHydrogens && thisAtom.GetElementType() != "H"))
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
                if (MainMenu.hideHydrogens || !hasHydrogens)
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
                else if (!MainMenu.hideHydrogens && hasHydrogens)
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
                if (MainMenu.colouring == "CPK")
                {
                    CpkColouring(thisAtom, atomBall);
                }
                else if (MainMenu.colouring == "Subunits")
                {
                    SubunitsColouring(thisAtom, atomBall);
                }
                else if (MainMenu.colouring == "Residues")
                {
                    ResiduesColouring(thisAtom, atomBall);
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
                            || (!MainMenu.hideHydrogens
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
                        if (((!MainMenu.hideHydrogens) ||
                            (MainMenu.hideHydrogens &&
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
                if ((!MainMenu.hideHydrogens) ||
                    (MainMenu.hideHydrogens &&
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

        if (MainMenu.colouring == "Residues" && thisAtom1.GetResidueName() == thisAtom2.GetResidueName())
        {
            ResiduesColouring(thisAtom1, bond);
        }

        if (MainMenu.colouring == "Subunits" && thisAtom1.GetChainID() == thisAtom2.GetChainID())
        {
            SubunitsColouring(thisAtom1, bond);
        }

        if (MainMenu.colouring == "CPK")
        {
            CpkColouringForLines(thisAtom1, thisAtom2, bondLine);
        }
    }

    void CartoonRepresentation(List<AtomParser> listOfAtoms)
    {
        Dictionary<string, List<Vector3>> dictionaryOfBackbone = new Dictionary<string, List<Vector3>>();
        Dictionary<string, List<Vector3>> dictionaryOfNucleicBackbone = new Dictionary<string, List<Vector3>>();

        foreach (AtomParser thisAtom in listOfAtoms)
        {

            if (thisAtom.GetAtomName() == "CA" || thisAtom.GetAtomName() == "N")
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
            Vector3[] aCarbons = dictionaryOfBackbone[chainKey].ToArray();

            //foreach (Vector3 wektor in aCarbons)
            //{ print(wektor); }
            //GameObject backbone = (GameObject)Instantiate(cartoonLinePrefab, aCarbons[0], Quaternion.identity);
            //LineRenderer cartoonLine = backbone.GetComponent<LineRenderer>();
            Vector3[] aCarbonsNew = CurvesSmoother.MakeSmoothCurve(aCarbons, 2.0f);
            
            
            //cartoonLine.SetVertexCount(aCarbonsNew.Length);
            //cartoonLine.SetPositions(aCarbonsNew);
            //cartoonLine.SetWidth(0.5f, 0.5f);

            //Vector3[] array = { new Vector3(0, 0, 0), new Vector3(0, 0, 2), new Vector3(0, 0, 4), new Vector3(0, 0, 6), new Vector3(0, 0, 8) };
            PipeTheLine backbonePipe = new PipeTheLine(aCarbonsNew, pipePrefab);


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
        
    


        //for (int i = 0; i < aCarbonsNew.Length; i++)
        //{

        //    GameObject cylinder = Instantiate(cylinderPrefab);

        //    cylinder.transform.localPosition = aCarbonsNew[i];

        //    if (i < aCarbonsNew.Length - 1)
        //    {
        //        cylinder.transform.LookAt(aCarbonsNew[i + 1]);

        //    }
        //    else
        //    {
        //        cylinder.transform.LookAt(aCarbonsNew[i - 1]);
        //    }
        //    cylinder.transform.Rotate(90, 0, 0);



        //}

    }
}
    



    void CpkColouring(AtomParser thisAtom, GameObject atomBall)
    {
        atomBall.GetComponent<Renderer>().material.color = atomColorDictionary[thisAtom.GetElementType()];
        
    }

    void CpkColouringForLines(AtomParser thisAtom1, AtomParser thisAtom2, LineRenderer lineToColour)
    {
        Color colour1, colour2;
        colour1 = atomColorDictionary[thisAtom1.GetElementType()];
        colour2 = atomColorDictionary[thisAtom2.GetElementType()];
        lineToColour.SetColors(colour1, colour2);
        lineToColour.material = new Material(Shader.Find("Particles/Alpha Blended"));
    }

    void SubunitsColouring(AtomParser thisAtom, GameObject thingToColour)
    {

        thingToColour.GetComponent<Renderer>().material.color = chainColorDictionary[thisAtom.GetChainID()];

    }

    void ResiduesColouring(AtomParser thisAtom, GameObject thingToColour)
    {
        //http://life.nthu.edu.tw/~fmhsu/rasframe/COLORS.HTM

        thingToColour.GetComponent<Renderer>().material.color = residueColorDictionary[thisAtom.GetResidueName()];

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
    }



}
