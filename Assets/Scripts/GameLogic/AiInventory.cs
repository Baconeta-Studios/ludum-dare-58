using System.Collections;
using System.Linq;
using UnityEngine;

public class AiInventory : MonoBehaviour
{
    public GameObject[] possibleItems;

    [HideInInspector]
    public GameObject heldItem;

    [Header("Inspection")]
    public Transform inspectOrigin;
    public float inspectionDuration = 3;
    [Header("Rotation")]
    public float inspectionRotationAmount = 180;
    [Header("Scale")]
    public AnimationCurve inspectionCurve;
    private Vector3 initialScale;

    [Header("Particles")] 
    public ParticleSystem inspectionParticles;

    private Coroutine inspectionCoroutine;
    private void Awake(){
        if (possibleItems.Length == 0)
        {
            Debug.LogWarning($"{name} has no possible items");
        }
        else
        {
            
            heldItem = Instantiate(possibleItems[Random.Range(0, possibleItems.Length)], transform);
            heldItem.transform.position = inspectOrigin.position;
            heldItem.SetActive(false);
            initialScale = heldItem.transform.localScale;
            Debug.Log($"{name} is holding {heldItem.name}");
        }
    }

    [ContextMenu("Inspect Item")]
    public void OnInspect(){
        if (inspectionCoroutine == null)
        {
            inspectionCoroutine = StartCoroutine(DoInspection());
        }
    }

    private IEnumerator DoInspection(){
        heldItem.transform.position = inspectOrigin.position;
        heldItem.SetActive(true);
        heldItem.transform.rotation = Quaternion.identity;
        heldItem.transform.localScale = Vector3.zero;
        if (inspectionParticles)
        { 
            inspectionParticles.Play();
        }
        
        
        float t = 0;
        while (t < inspectionDuration)
        {
            // Rotation
            float rotationY = Mathf.Lerp(0, inspectionRotationAmount, t / inspectionDuration);
            heldItem.transform.rotation = Quaternion.Euler(new Vector3(0, rotationY, 0));
            
            // Scale
            heldItem.transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, inspectionCurve.Evaluate(t / inspectionDuration));
            
            t += Time.deltaTime;
            yield return null;
        }
        
        if (inspectionParticles)
        { 
            inspectionParticles.Stop();
        }
        
        heldItem.SetActive(false);
        inspectionCoroutine = null;
        yield return null;
    }
}
