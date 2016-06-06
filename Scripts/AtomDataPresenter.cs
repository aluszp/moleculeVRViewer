using UnityEngine;
using System.Collections;

public class AtomDataPresenter : MonoBehaviour {
    private IEnumerator atomCoroutine;
    public float delayInterval = .15f;
    // Use this for initialization

    void Awake () {
        atomCoroutine = PresentData();
    }

    private IEnumerator PresentData()
    {

       
        while (true)
        {
            print("no hejka");
            yield return new WaitForSeconds(delayInterval);
        }
    }

    public void StartPresenting()
    {
        StartCoroutine(atomCoroutine);
    }

    public void StopPresenting()
    {
        StopAllCoroutines();
    }
}
