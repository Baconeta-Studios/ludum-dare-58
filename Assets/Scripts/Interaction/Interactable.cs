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
        var playerSync = initiatingPlayer.GetComponent<CoherenceSync>();
        if (playerSync == null) { Debug.LogError("Initiating player missing CoherenceSync"); return; }

        // TODO Tell all clients about the item being collected
        // TODO Make sure the right client has received the item.

        // for now tell ALL clients to process it like they collected it.
        _sync.SendOrderedCommandToChildren<Interactable>(
           nameof(OnCollect),
           Coherence.MessageTarget.All,
           playerSync
       );
    }

    [Command]
    public void RequestInspect(GameObject initiatingPlayer)
    {
        Debug.Log("client requesting to inspect from server");
        var playerSync = initiatingPlayer.GetComponent<CoherenceSync>();
        if (playerSync == null) { Debug.LogError("Initiating player missing CoherenceSync"); return; }

        // Tell all clients about the murder.
        _sync.SendOrderedCommandToChildren<Interactable>(
           nameof(OnInspect),
           Coherence.MessageTarget.StateAuthorityOnly,
           playerSync
       );
    }

    [Command]
    public void RequestMurder(GameObject initiatingPlayer)
    {
        Debug.Log("client requesting to murder from server");
        var playerSync = initiatingPlayer.GetComponent<CoherenceSync>();
        if (playerSync == null) { Debug.LogError("Initiating player missing CoherenceSync"); return; }

        // Tell all clients about the murder.
        _sync.SendOrderedCommandToChildren<Interactable>(
           nameof(OnMurder),
           Coherence.MessageTarget.All, // TODO server only knows murderers.
           playerSync
       );
    }

    [Command]
    public void RequestScare(GameObject initiatingPlayer)
    {
        Debug.Log("client requesting to scare from server");
        var playerSync = initiatingPlayer.GetComponent<CoherenceSync>();
        if (playerSync == null) { Debug.LogError("Initiating player missing CoherenceSync"); return; }

        // Tell only the AI owner about the scare to run the movements.
        _sync.SendOrderedCommandToChildren<Interactable>(
            nameof(OnScare),
            Coherence.MessageTarget.StateAuthorityOnly,
            playerSync
        );
    }

    /* Server receives the request and processes and replies with a Command also. */
    [Command]
    public virtual void OnScare(CoherenceSync initiatingPlayer)
    {
        // Server updates the one AI to move and informs no one about the scare.
        Debug.Log($": Scared {name}");
        // TO USE THIS Override it in a subclass and call base to keep the logs
    }

    [Command]
    public virtual void OnMurder(CoherenceSync initiatingPlayer)
    {
        // Server tells ALL clients about the murder
        Debug.Log($" Murdered {name}");
        // TO USE THIS Override it in a subclass and call base to keep the logs
    }

    [Command]
    public virtual void OnInspect(CoherenceSync initiatingPlayer)
    {
        // Server tells only the owner Client whats inspected 
        Debug.Log($" Inspected {name}");
        // TO USE THIS Override it in a subclass and call base to keep the logs
    }

    [Command]
    public virtual void OnCollect(CoherenceSync initiatingPlayer)
    {
        // Server tell ALL about the item being collected.
        // TODO BUT doesn't make all the players animate

        Debug.Log($": Collected {name}");
        // TODO behaviour for collect

        // TODO move this into a collectables class, but for now whatevers.
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
