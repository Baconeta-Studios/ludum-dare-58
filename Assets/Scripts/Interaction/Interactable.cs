using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool doesMove = false;
    public bool occupiesCell;
    
    public void OnValidate(){
        if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            Debug.LogWarning($"{name}: Layer is not set to Interactable but has an Interactable component.");
        }
    }

    public void Interact(){
        
        // TODO Add functionality to the proper interaction based on the context.
    }
}
