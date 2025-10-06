using System;
using System.Collections.Generic;
using System.Linq;
using Movement;
using UnityEngine;
using Coherence.Toolkit;

public class Interactable : MonoBehaviour
{
    public bool canBeInteracted = true;
    public PlayerClickInteraction.InteractionType[] supportedInteractions;
    public bool doesMove = false;
    public bool occupiesCell;
    public CoherenceSync _sync;
    private void Awake()
    {
        if (_sync == null)
        {
            Debug.LogError($"{name} has Interactable but no CoherenceSync attached!");
        }
    }

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
                // All interactions are requests to the server, which then decides what happesn and calls back to On* methods below for the client to do updates.
                case PlayerClickInteraction.InteractionType.Collect:
                    RequestCollect(initiatingPlayer);
                    break;
                
                case PlayerClickInteraction.InteractionType.Inspect:
                    RequestInspect(initiatingPlayer);
                    break;
                
                case PlayerClickInteraction.InteractionType.Murder:
                    RequestMurder(initiatingPlayer);
                    break;
                
                case PlayerClickInteraction.InteractionType.Scare:
                    RequestScare(initiatingPlayer);
                    break;
            }
        }
    }

    /* Client Calls Host to request iteractions occur and server then receives and processes */
    [Command]
    public void RequestCollect(GameObject initiatingPlayer)
    {
        Debug.Log("client requesting to collect from server");
        _sync.SendCommand<AiInteractable>(nameof(OnCollect), Coherence.MessageTarget.StateAuthorityOnly);
    }

    [Command]
    public void RequestInspect(GameObject initiatingPlayer)
    {
        Debug.Log("client requesting to inspect from server");
        _sync.SendCommand<AiInteractable>(nameof(OnInspect), Coherence.MessageTarget.StateAuthorityOnly);
    }

    [Command]
    public void RequestMurder(GameObject initiatingPlayer)
    {
        Debug.Log("client requesting to murder from server");
        // TODO do we need to send the playerid?
        _sync.SendCommand<AiInteractable>(nameof(OnMurder), Coherence.MessageTarget.StateAuthorityOnly);
    }

    [Command]
    public void RequestScare(GameObject initiatingPlayer)
    {
        Debug.Log("client requesting to scare from server");
        _sync.SendCommand<AiInteractable>(nameof(OnScare), Coherence.MessageTarget.StateAuthorityOnly);
    }

    /* Server receives the request and processes and replies with a Command also. */
    [Command]
    public void OnScare(){
        // Server updates the AI to move and informs no one about the scare.

        Debug.Log($": Scared {name}");
        // TODO behaviour for Scare.
        // Tell the AI Movement Controller to move to another room
    }

    [Command]
    protected void OnMurder(GameObject initiatingPlayer){
        // Server tells ALL about the murder

        Debug.Log($"{initiatingPlayer.gameObject.name}: Murdered {name}");
        // TODO behaviour for Murder
        // .... murders them?

    }

    [Command]
    protected void OnInspect(GameObject initiatingPlayer){
        // Server tells Client whats inspected 

        Debug.Log($"{initiatingPlayer.gameObject.name}: Inspected {name}");
        // TODO behaviour for inspect
        AiInventory inventory = GetComponent<AiInventory>();
        if(inventory)
        {
            inventory.OnInspect();
        }
    }

    [Command]
    protected void OnCollect(GameObject initiatingPlayer){
        // Server tell ALL about the item being collected.

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
