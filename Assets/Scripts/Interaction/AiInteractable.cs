using Coherence.Toolkit;
using Movement;
using UnityEngine;

public class AiInteractable : Interactable
{
    Animator animator;
    AiMovementController movement;
    public ParticleSystem smokeParticleSystem;

    private void Awake()
    {
        movement = GetComponent<AiMovementController>();
        animator = GetComponent<Animator>();
    }
    //protected override void OnMurder(GameObject initiatingPlayer) 
    //{
    //    _sync.SendCommand<AiInteractable>(nameof(RequestMurder), Coherence.MessageTarget.AuthorityOnly);
    //}

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
    public void RequestMurder()
    {
        // Owner decides to actually broadcast - here always broadcast
        //_sync.SendCommand<AiInteractable>(nameof(RpcOnMurder), Coherence.MessageTarget.All);
    }

    [Command]
    public void RpcOnMurder()
    {
        movement.StopMovement();
        animator.SetTrigger("Die");
        PlaySmokePoof();
    }
}
