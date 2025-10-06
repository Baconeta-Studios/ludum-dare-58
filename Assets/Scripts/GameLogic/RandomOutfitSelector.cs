using UnityEngine;

[System.Serializable]
public class Outfit
{
    [Tooltip("Name of this outfit for clarity")]
    public string outfitName;

    [Tooltip("All objects that should be enabled when this outfit is chosen")]
    public GameObject[] parts;
}

public class RandomOutfitSelector : MonoBehaviour
{
    [Header("Define all possible outfits here")]
    public Outfit[] outfits;

    [Header("Config")]
    public bool randomizeOnStart = true;

    private Outfit currentOutfit;

    private void Start()
    {
        if (randomizeOnStart)
        {
            SelectRandomOutfit();
        }
    }

    public void SelectRandomOutfit()
    {
        // Disable all objects first
        foreach (var outfit in outfits)
        {
            foreach (var obj in outfit.parts)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        // Pick a random outfit
        if (outfits.Length > 0)
        {
            int randomIndex = Random.Range(0, outfits.Length);
            currentOutfit = outfits[randomIndex];

            // Enable all parts of that outfit
            foreach (var obj in currentOutfit.parts)
            {
                if (obj != null) obj.SetActive(true);
            }

            Debug.Log("Outfit selected: " + currentOutfit.outfitName);
        }
    }
}