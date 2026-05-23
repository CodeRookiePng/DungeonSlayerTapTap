using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    // Tu v Unity do listu pretiahni všetkých 7 panelov v poradí (0 až 6)
    public List<GameObject> panels;

    void Start()
    {
      
    }

    // Táto funkcia prepne na panel podľa indexu (volaj ju z tlačidiel)
    public void SwitchToPanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            // Zapne iba ten panel, ktorého index sa zhoduje, ostatné vypne
            panels[i].SetActive(i == index);
        }
    }
}