using Coherence.Toolkit;
using FMODUnity;
using Movement;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AiInteractable : Interactable
{
    Animator animator;
    AiMovementController movement;
    public ParticleSystem smokeParticleSystem;
    public ParticleSystem murderParticleSystem;

    private void Awake()
    {
        movement = GetComponent<AiMovementController>();
        animator = GetComponent<Animator>();
    }

    public void PlaySmokePoof()
    {
        smokeParticleSystem.transform.SetParent(null);
        smokeParticleSystem.Play();
    }

    public void OnEndMurder()
    {
        Destroy(gameObject);
    }

    [Command]
    public override void OnInspect(CoherenceSync initiatingPlayerSync)
    {
        //base.OnInspect(initiatingPlayerSync);
        GameObject initiatingPlayer = initiatingPlayerSync.gameObject;
        Debug.Log("Inspecting the AI");

        // TODO behaviour for inspect
        AiInventory inventory = GetComponent<AiInventory>();
        if (inventory)
        {
            inventory.OnInspect();
        }
    }

    [Command]
    public override void OnCollect(CoherenceSync initiatingPlayerSync)
    {
        GameObject initiatingPlayer = initiatingPlayerSync.gameObject;
        //base.OnCollect(initiatingPlayerSync);
        // Should be impossible to get here anyhow.
        Debug.Log("Called On Collect but Ai cannot be collected");
    }

    [Command]
    public override void OnMurder(CoherenceSync initiatingPlayerSync)
    {
        // Play card interaction sound with FMOD.
        RuntimeManager.PlayOneShot("event:/SFX/Weapons/Knife", transform.position);
        
        //base.OnMurder(initiatingPlayerSync);
        GameObject initiatingPlayer = initiatingPlayerSync.gameObject;

        Debug.Log("Making the AI Die.");
        movement.StopMovement();
        animator.SetTrigger("Die");
        murderParticleSystem.Play();
    }

    [Command]
    public override void OnScare(CoherenceSync initiatingPlayerSync)
    {
        //base.OnScare(initiatingPlayerSync);
        GameObject initiatingPlayer = initiatingPlayerSync.gameObject;

        Debug.Log("Making the AI scared.");

        var movement = GetComponent<AiMovementController>();
        if (movement != null)
        {
            // Move 0 cells in X (east), 10 cells in Y (north)
            movement.MoveBy(0, 10);
        }
    }



}
