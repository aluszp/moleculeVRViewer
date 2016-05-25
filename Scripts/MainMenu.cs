using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System;


public class MainMenu : MonoBehaviour
{
    //public Texture2D menuBackground;
    public static string pdbID; //or PlayerPrefs?
    public static bool hideHydrogens = false;
    public static string representationStyle;
    public static string colouring;

    public InputField pdbInputField;
    public Button startButton;
    public Text errorText;
    public Toggle hideHydrogensToggle;
    public Dropdown representationDropdown;
    public Dropdown colouringDropdown;
    // initialization

    void Start()
    {

        startButton.onClick.AddListener(ButtonFunction);

    }

    //  once per frame
    void Update()
    {
        if (pdbID != pdbInputField.text)
        {
            errorText.text = ""; //clear error message if user starts changing the PDB ID
        }
        pdbID = pdbInputField.text;

        HideHydrogensFunction();
        representationStyle = GetDropdownValue(representationDropdown);
        colouring = GetDropdownValue(colouringDropdown);


    }

    void ButtonFunction()
    {

        if (pdbID.Length == 4)
        {
            if (CheckIfFileExist(pdbID))
            {
                if (SceneManager.GetActiveScene().name != "MoleculeScene")
                {
                    errorText.text = "Wait...";
                    SceneManager.LoadScene("MoleculeScene");
                }
            }
        }

        else
        {
            errorText.text = "ERROR: PDB ID should contain 4 characters! Try again.";

        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);


    }
    bool CheckIfFileExist(string pdbID)
    {
        bool result = false;
        string url = "http://files.rcsb.org/download/" + pdbID + ".pdb";
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        webRequest.Proxy = null;
        webRequest.Timeout = 1200; // miliseconds
        webRequest.Method = "HEAD";

        HttpWebResponse response = null;


        try
        {
            response = (HttpWebResponse)webRequest.GetResponse();
            result = true; ;
        }
        catch (WebException webException)
        {
            errorText.text = "ERROR: " + url + " doesn't exist! " + webException.Message + " Try again.";
            result = false;
        }
        finally
        {
            if (response != null)
            {

                response.Close();
            }
        }

        return result;
    }


    string GetDropdownValue(Dropdown thisDropdown)
    {

        string value = thisDropdown.captionText.text;
        return value;
    }


    void HideHydrogensFunction()
    {
        if (hideHydrogensToggle.isOn)
        {
            hideHydrogens = true;
        }
        else
            hideHydrogens = false;
    }

}


