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
    public Vector3 target = new Vector3(0, 0, 0); //geometric center of molecule
    bool rotating = true; //initial rotating
    Dictionary<string, Color> chainColorDicitonary = new Dictionary<string, Color>();



    //initialization
    void Start()
    {


        WebClient client = new WebClient();
        //System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Dell 15z\Studia\NOWA PRACA MGR\4QRV.pdb");
        StreamReader file = new StreamReader(client.OpenRead("http://files.rcsb.org/download/" + MainMenu.pdbID + ".pdb"));
        int numberOfAtoms = 0;
        Vector3 sumOfPositions = new Vector3();
        while (!file.EndOfStream)
        {
            string line = file.ReadLine();

            if (line.Substring(0, 6).Trim() == "ATOM" || line.Substring(0, 6).Trim() == "HETATM")
            {
                AtomParser thisAtom = new AtomParser(line);

                Draw(thisAtom);
                numberOfAtoms++;
                sumOfPositions += thisAtom.GetAtomPosition();
            }

            //creating dictionary of subunits and respective colours for subunits coloring method
            else if ((MainMenu.colouring == "Subunits") && (line.Substring(0, 6).Trim() == "COMPND")
                && (line.Contains("CHAIN:")))
            {

                string chains = line.Substring(18, 60).Trim().Trim(';').Replace(" ", "");
                print(chains);

                foreach (string chain in chains.Split(','))
                {
                    chainColorDicitonary.Add(chain, new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
                }

            }


        }
        target = sumOfPositions / numberOfAtoms;
        transform.position = target - new Vector3(0, 0, 50);
        transform.LookAt(target);
        file.Close();

    }





    void Draw(AtomParser thisAtom)
    {
        if ((!MainMenu.hideHydrogens) || (MainMenu.hideHydrogens && thisAtom.GetElementType() != "H"))
        {
            if (MainMenu.representationStyle == "Van der Waals")
            {
                GameObject atomBall = VdwRepresentation(thisAtom);

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



    GameObject VdwRepresentation(AtomParser thisAtom)
    {
        GameObject atomBall = (GameObject)Instantiate(atomPrefab, thisAtom.GetAtomPosition(), Quaternion.identity);
        atomBall.name = thisAtom.GetAtomName();

        switch (thisAtom.GetElementType())
        {
            case "H":
                atomBall.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f) * 2;
                break;
            case "C":
                atomBall.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f) * 2;
                break;
            case "N":
                atomBall.transform.localScale = new Vector3(1.55f, 1.55f, 1.55f) * 2;
                break;
            case "O":
                atomBall.transform.localScale = new Vector3(1.52f, 1.52f, 1.52f) * 2;
                break;
            case "F":
                atomBall.transform.localScale = new Vector3(1.47f, 1.47f, 1.47f) * 2;
                break;
            case "CL":
                atomBall.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f) * 2;
                break;
            case "P":
                atomBall.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f) * 2;
                break;
            case "S":
                atomBall.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f) * 2;
                break;
            case "MG":
                atomBall.transform.localScale = new Vector3(1.73f, 1.73f, 1.73f) * 2;
                break;
        }
        return atomBall;
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

    void SubunitsColouring(AtomParser thisAtom, GameObject atomBall)
    {

        atomBall.GetComponent<Renderer>().material.color = chainColorDicitonary[thisAtom.GetChainID()];

    }

    void ResiduesColouring(AtomParser thisAtom, GameObject atomBall)
    {
        //http://life.nthu.edu.tw/~fmhsu/rasframe/COLORS.HTM

        print(thisAtom.GetResidueName());
        switch (thisAtom.GetResidueName())
        {
            case "ALA":
                atomBall.GetComponent<Renderer>().material.color = new Color32(200, 200, 200, 1);
                break;
            case "ASN":
                atomBall.GetComponent<Renderer>().material.color = new Color32(0, 220, 220, 1);
                break;
            case "ASP":
                atomBall.GetComponent<Renderer>().material.color = new Color32(230, 10, 10, 1);
                break;
            case "ARG":
                atomBall.GetComponent<Renderer>().material.color = new Color32(20, 90, 255, 1);
                break;
            case "CYS":
                atomBall.GetComponent<Renderer>().material.color = new Color32(230, 230, 0, 1);
                break;
            case "GLN":
                atomBall.GetComponent<Renderer>().material.color = new Color32(0, 220, 220, 1);
                break;
            case "GLU":
                atomBall.GetComponent<Renderer>().material.color = new Color32(230, 10, 10, 1);
                break;
            case "GLY":
                atomBall.GetComponent<Renderer>().material.color = new Color32(235, 235, 235, 1);
                break;
            case "HIS":
                atomBall.GetComponent<Renderer>().material.color = new Color32(130, 130, 210, 1);
                break;
            case "ILE":
                atomBall.GetComponent<Renderer>().material.color = new Color32(15, 130, 15, 1);
                break;
            case "LEU":
                atomBall.GetComponent<Renderer>().material.color = new Color32(15, 130, 15, 1);
                break;
            case "LYS":
                atomBall.GetComponent<Renderer>().material.color = new Color32(20, 90, 255, 1);
                break;
            case "MET":
                atomBall.GetComponent<Renderer>().material.color = new Color32(230, 230, 0, 1);
                break;
            case "PHE":
                atomBall.GetComponent<Renderer>().material.color = new Color32(50, 50, 170, 1);
                break;
            case "PRO":
                atomBall.GetComponent<Renderer>().material.color = new Color32(220, 150, 130, 1);
                break;
            case "SER":
                atomBall.GetComponent<Renderer>().material.color = new Color32(250, 150, 0, 1);
                break;
            case "THR":
                atomBall.GetComponent<Renderer>().material.color = new Color32(250, 150, 0, 1);
                break;
            case "TYR":
                atomBall.GetComponent<Renderer>().material.color = new Color32(50, 50, 170, 1);
                break;
            case "TRP":
                atomBall.GetComponent<Renderer>().material.color = new Color32(180, 90, 180, 1);
                break;
            case "VAL":
                atomBall.GetComponent<Renderer>().material.color = new Color32(15, 130, 15, 1);
                break;

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
    }



}
