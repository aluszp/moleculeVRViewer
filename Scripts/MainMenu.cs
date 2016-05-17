using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Texture2D menuBackground;
    public static string pdbID; //a może użyć PlayerPrefs? tylko trzeba je potem usunąć z dysku!!!
    public bool ifEnterPressed = false;
    public bool improperCode;
    public static bool hideHydrogens = false;

    // Use this for initialization

    void Start()
    {
        pdbID = "Enter the PDB ID...";

    }

    void OnGUI()
    {


        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), menuBackground, ScaleMode.StretchToFill);
        hideHydrogens = GUI.Toggle(new Rect(Screen.width / 2 - 100, 70, 150, 30), hideHydrogens, "Hide hydrogens");

        pdbID = GUI.TextField(new Rect(Screen.width / 2 - 100, 20, 200, 25), pdbID);



        if ((GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height - 40, 100, 30), "Start")) || ifEnterPressed)
        {

            if (pdbID.Length == 4)
            {
                improperCode = false;
                if (SceneManager.GetActiveScene().name != "MoleculeScene")
                {
                    SceneManager.LoadScene("MoleculeScene");
                }
            }

            else
                improperCode = true;
        }

        if (improperCode)
        {
            GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height - 70, 250, 25), "PDB ID should contain 4 characters!");
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            ifEnterPressed = true;
        }

      
    }
}
