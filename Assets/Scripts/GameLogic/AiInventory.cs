using System.Collections;
using UnityEngine;

// TODO we should be setting this data and everything from the GameManager so that we can
// track everything in the world (probably)
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
    public GameObject bloodSpatter;

    private void Awake()
    {
        Invoke(nameof(SetupAIItem), 0.5f);
    }

    private void SetupAIItem()
    {
        var itemPrefab = Managers.GameManager.Instance.GetNextItemForAI();
        if (itemPrefab != null)
        {
            heldItem = Instantiate(itemPrefab.gameObject, transform);
            heldItem.transform.position = inspectOrigin.position;
            heldItem.SetActive(false);
            initialScale = heldItem.transform.localScale;
            Debug.Log($"{name} is holding {itemPrefab.ItemName}");
        }
        else
        {
            Debug.LogWarning($"{name} could not get an item to hold.");
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

    public void DropCollectable()
    {
        if (inspectionCoroutine != null)
        {
            StopCoroutine(inspectionCoroutine);
        }

        // drop the held collectable
        DropItem(heldItem, Vector3.zero);

        // drop the COLLECTABLE evidence (blood spatter)
        DropItem(bloodSpatter, new Vector3(90, 0, 0));

    }

    private void DropItem(GameObject thingToDrop, Vector3 orientation)
    {
        // Set the position of the COLLECTABLE item.
        thingToDrop.transform.position = transform.position;
        thingToDrop.transform.rotation = Quaternion.Euler(orientation);
        thingToDrop.transform.localScale = Vector3.one;
        thingToDrop.transform.SetParent(null);
        thingToDrop.SetActive(true);
        Collider collider = thingToDrop.GetComponent<Collider>();
        if (collider)
        {
            collider.enabled = true;
        }
        Interactable interactComponent = thingToDrop.GetComponent<Interactable>();
        if (interactComponent)
        {
            interactComponent.canBeInteracted = true;
        }
    }
}
