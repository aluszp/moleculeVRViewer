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
    public GameObject cartoonLinePrefab;
    public Vector3 target = new Vector3(0, 0, 0); //geometric center of molecule
    bool rotating = true; //initial rotating
    bool hasHydrogens = false;
    Dictionary<string, Color> chainColorDictionary = new Dictionary<string, Color>();
    Dictionary<string, Color> residueColorDictionary = new Dictionary<string, Color>();
    List<AtomParser> listOfAtoms = new List<AtomParser>();
    List<List<int>> listOfConectPairs = new List<List<int>>();
    int numberOfConects;
    int numberOfChains;
    int numberOfAtoms; //number of atoms and heteroatoms
    string chains; 
    string residues;

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

                if (thisAtom.GetElementType() == "H") //important for VdW representation
                {
                    hasHydrogens = true;
                }
                listOfAtoms.Add(thisAtom);
                sumOfPositions += thisAtom.GetAtomPosition();
            }

            //creating dictionary of subunits and respective colours for subunits coloring method
            else if ((MainMenu.colouring == "Subunits" || MainMenu.representationStyle == "Cartoon") 
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

            else if (MainMenu.representationStyle == "Lines" && line.Substring(0, 6).Trim() == "CONECT") //counting number of bonds assigned in CONECT section
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
        }
       
        Draw(listOfAtoms);
    }


    void PrepareForSubunitsColouring()
    {
        
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

        foreach (string residue in residues.Split(' '))
        {
            if (!residueColorDictionary.ContainsKey(residue))
            {
                residueColorDictionary.Add(residue, new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
            }
        }
    }


    void Draw(List<AtomParser> listOfAtoms)
    {
        if (MainMenu.representationStyle == "Van der Waals")
        {
            VdwRepresentation(listOfAtoms);
        }
        else if (MainMenu.representationStyle == "Lines")
        {
            LinesRepresentation(listOfAtoms);
        }
        else if (MainMenu.representationStyle == "Cartoon")
        {
            CartoonRepresentation(listOfAtoms);
        }
    }

    void VdwRepresentation(List<AtomParser> listOfAtoms)
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
                        atomBall.transform.localScale = new Vector3(1.47f, 1.47f, 1.47f) * 2;
                        break;
                    case "CL":
                        atomBall.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f) * 2;
                        break;
                    case "P":
                        atomBall.transform.localScale = new Vector3(1.880f, 1.880f, 1.880f) * 2;
                        break;
                    case "MG":
                        atomBall.transform.localScale = new Vector3(1.73f, 1.73f, 1.73f) * 2;
                        break;
                    case "CA":
                        atomBall.transform.localScale = new Vector3(1.948f, 1.948f, 1.948f) * 2;
                        break;
                    case "FE":
                        atomBall.transform.localScale = new Vector3(1.948f, 1.948f, 1.948f) * 2;
                        break;
                    case "ZN":
                        atomBall.transform.localScale = new Vector3(1.148f, 1.148f, 1.148f) * 2;
                        break;
                    case "CD":
                        atomBall.transform.localScale = new Vector3(1.748f, 1.748f, 1.748f) * 2;
                        break;
                    case "I":
                        atomBall.transform.localScale = new Vector3(1.748f, 1.748f, 1.748f) * 2;
                        break;

                }
                if (MainMenu.hideHydrogens || !hasHydrogens)
                {
                    switch (thisAtom.GetElementType())
                    {
                        case "C":
                            atomBall.transform.localScale = new Vector3(1.872f, 1.872f, 1.872f) * 2;
                            break;
                        case "N":
                            atomBall.transform.localScale = new Vector3(1.507f, 1.507f, 1.507f) * 2;
                            break;
                        case "O":
                            atomBall.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f) * 2;
                            break;
                        case "S":
                            atomBall.transform.localScale = new Vector3(1.848f, 1.848f, 1.848f) * 2;
                            break;

                    }
                }
                else if (!MainMenu.hideHydrogens && hasHydrogens)
                {
                    switch (thisAtom.GetElementType())
                    {
                        case "H":
                            atomBall.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f) * 2;
                            break;
                        case "C":
                            atomBall.transform.localScale = new Vector3(1.548f, 1.548f, 1.548f) * 2;
                            break;
                        case "N":
                            atomBall.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f) * 2;
                            break;
                        case "O":
                            atomBall.transform.localScale = new Vector3(1.348f, 1.348f, 1.348f) * 2;
                            break;
                        case "S":
                            atomBall.transform.localScale = new Vector3(1.808f, 1.808f, 1.808f) * 2;
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
    }

    void CartoonRepresentation(List<AtomParser> listOfAtoms) 
    {
        chains = chains.Remove(chains.Length - 1);
        List<Vector3> aCarbonsList = new List<Vector3>();
        foreach (string chain in chains.Split(','))
        {

            foreach (AtomParser thisAtom in listOfAtoms)
            {

                if (thisAtom.GetAtomName() == "CA" && thisAtom.GetChainID() == chain)
                {
                    
                    aCarbonsList.Add(thisAtom.GetAtomPosition());
                }
                print("łańcuch: " + chain + " chainIDatomu: " + thisAtom.GetChainID());


            }
            Vector3[] aCarbons = aCarbonsList.ToArray();

            //foreach (Vector3 wektor in aCarbons)
            //{ print(wektor); }
            GameObject backbone = (GameObject)Instantiate(cartoonLinePrefab, aCarbons[0], Quaternion.identity);
            LineRenderer cartoonLine = backbone.GetComponent<LineRenderer>();
            Vector3[] aCarbonsNew = CurvesSmoother.MakeSmoothCurve(aCarbons, 3.0f);
            cartoonLine.SetVertexCount(aCarbonsNew.Length);
            cartoonLine.SetPositions(aCarbonsNew);
            cartoonLine.SetWidth(0.2f, 0.2f);

            aCarbonsList.Clear();
        }
        




    }


    void CpkColouring(AtomParser thisAtom, GameObject atomBall)
    {
        switch (thisAtom.GetElementType())
        {
            case "H":
                atomBall.GetComponent<Renderer>().material.color = Color.white;
                break;
            case "C":
                atomBall.GetComponent<Renderer>().material.color = Color.black;
                break;
            case "N":
                atomBall.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case "O":
                atomBall.GetComponent<Renderer>().material.color = Color.red;
                break;
            case "F":
                atomBall.GetComponent<Renderer>().material.color = Color.green;
                break;
            case "CL":
                atomBall.GetComponent<Renderer>().material.color = Color.green;
                break;
            case "P":
                atomBall.GetComponent<Renderer>().material.color = new Color32(255, 153, 0, 1); //orange
                break;
            case "S":
                atomBall.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case "BR":
                atomBall.GetComponent<Renderer>().material.color = new Color32(153, 0, 0, 1); //dark red
                break;
            case "MG":
                atomBall.GetComponent<Renderer>().material.color = new Color32(0, 102, 0, 1); //dark green
                break;

        }
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
