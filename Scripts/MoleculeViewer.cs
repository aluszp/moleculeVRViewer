using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;
using Assets.Code.Sources;

public class MoleculeViewer : MonoBehaviour
{
    public GameObject atomPrefab;
    public Vector3 target = new Vector3(0,0,0); //geometric center of molecule
    bool rotating = true; //initial rotating
    


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

            if (line.Substring(0, 6).Trim() == "ATOM")
            {
                AtomParser thisAtom = new AtomParser(line);
                
                Draw(thisAtom);
                numberOfAtoms++;
                sumOfPositions += thisAtom.GetAtomPosition();
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
            GameObject atomBall = (GameObject)Instantiate(atomPrefab, thisAtom.GetAtomPosition(), Quaternion.identity);
            atomBall.name = thisAtom.GetAtomName();

            if (MainMenu.hideHydrogens)
            {
                atomBall.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f); //without hydrogens there is more space
            }

            PckColouring(thisAtom, atomBall);
        }

        print(thisAtom.GetResidueName());
        
       
    }

    void PckColouring(AtomParser thisAtom, GameObject atomBall)
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
