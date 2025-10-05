using System;
using System.Collections.Generic;
using System.Linq;
using Movement;
using UnityEngine;
using InteractionType = Movement.PlayerClickInteraction.InteractionType;


public class Interactable : MonoBehaviour
{
    public InteractionType[] supportedInteractions;
    public bool doesMove = false;
    public bool occupiesCell;
    
    public void OnValidate(){
        if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            Debug.LogWarning($"{name}: Layer is not set to Interactable but has an Interactable component.");
        }
    }
    
    public void Interact(GameObject initiatingPlayer, InteractionType interactionType){

        // TODO Fix this array to list - my editor was playing up and was GUI erroring everywhere (even rebooted editor)
        if (supportedInteractions.ToList().Contains(interactionType))
        {
            switch (interactionType)
            {
                case InteractionType.Collect:
                    OnCollect(initiatingPlayer);
                    break;
                
                case InteractionType.Inspect:
                    OnInspect(initiatingPlayer);
                    break;
                
                case InteractionType.Murder:
                    OnMurder(initiatingPlayer);
                    break;
                
                case InteractionType.Scare:
                    OnScare(initiatingPlayer);
                    break;
            }
        }
    }

    private void OnScare(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Scared {name}");
        // TODO behaviour for Scare.
        // Tell the AI Movement Controller to move to another room
    }

    private void OnMurder(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Murdered {name}");
        // TODO behaviour for Murder
        // .... murders them?
    }

    private void OnInspect(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Inspected {name}");
        // TODO behaviour for inspect
    }

    private void OnCollect(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Collected {name}");
        // TODO behaviour for collect
    }
}
