using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net;
using Assets.Code.Sources;



public class MainMenu : MonoBehaviour
{



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
        if (Configurator.GetPdbID() != pdbInputField.text)
        {
            errorText.text = ""; //clear error message if user starts changing the PDB ID
        }
        Configurator.SetPdbID(pdbInputField.text);

        HideHydrogensFunction();

        Configurator.SetRepresentationStyle(GetDropdownValue(representationDropdown));
        if (Configurator.GetRepresentationStyle() == RepresentationStyles.vanDerWaals 
            || Configurator.GetRepresentationStyle() == RepresentationStyles.ballsAndSticks
            || Configurator.GetRepresentationStyle() == RepresentationStyles.lines)
        {
            colouringDropdown.ClearOptions();
            colouringDropdown.AddOptions(new List<string> { Colouring.cpk, Colouring.residues, Colouring.subunits });
        }
        else
        {
            colouringDropdown.ClearOptions();
            colouringDropdown.AddOptions(new List<string> { Colouring.subunits });
        }

        Configurator.SetColouring(GetDropdownValue(colouringDropdown));


    }

    void ButtonFunction()
    {

        if (Configurator.GetPdbID().Length == 4)
        {
            if (CheckIfFileExist(Configurator.GetPdbID()))
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
            Configurator.SetHideHydrogens(true);
        }
        else
            Configurator.SetHideHydrogens(false);
    }

}


