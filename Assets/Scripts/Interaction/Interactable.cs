using System;
using System.Collections.Generic;
using System.Linq;
using Movement;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    public bool canBeInteracted = true;
    public PlayerClickInteraction.InteractionType[] supportedInteractions;
    public bool doesMove = false;
    public bool occupiesCell;
    
    public void OnValidate(){
        if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            Debug.LogWarning($"{name}: Layer is not set to Interactable but has an Interactable component.");
        }
    }
    
    public void Interact(GameObject initiatingPlayer, PlayerClickInteraction.InteractionType interactionType){

        if(!canBeInteracted)
        {
            return;
        }
        // TODO Fix this array to list - my editor was playing up and was GUI erroring everywhere (even rebooted editor)
        if (supportedInteractions.ToList().Contains(interactionType))
        {
            switch (interactionType)
            {
                case PlayerClickInteraction.InteractionType.Collect:
                    OnCollect(initiatingPlayer);
                    break;
                
                case PlayerClickInteraction.InteractionType.Inspect:
                    OnInspect(initiatingPlayer);
                    break;
                
                case PlayerClickInteraction.InteractionType.Murder:
                    OnMurder(initiatingPlayer);
                    break;
                
                case PlayerClickInteraction.InteractionType.Scare:
                    OnScare(initiatingPlayer);
                    break;
            }
        }
    }

    protected virtual void OnScare(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Scared {name}");
        // TODO behaviour for Scare.
        // Tell the AI Movement Controller to move to another room
    }

    protected virtual void OnMurder(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Murdered {name}");
        // TODO behaviour for Murder
        // .... murders them?

    }

    protected virtual void OnInspect(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Inspected {name}");
        // TODO behaviour for inspect
        AiInventory inventory = GetComponent<AiInventory>();
        if(inventory)
        {
            inventory.OnInspect();
        }
    }

    protected virtual void OnCollect(GameObject initiatingPlayer){
        Debug.Log($"{initiatingPlayer.gameObject.name}: Collected {name}");
        // TODO behaviour for collect

        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play("Collect");
        }
        else
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
