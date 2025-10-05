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
    protected override void OnMurder(GameObject initiatingPlayer) 
    {
        movement.StopMovement();
        animator.SetTrigger("Die");
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
}
