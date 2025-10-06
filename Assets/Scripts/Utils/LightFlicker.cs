using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float flickerSpeed = 0.1f;
    public float minIntensity = 1f;
    public float maxIntensity = 3f;
    public float minRange = 1f;
    public float maxRange = 3f;
    public float hueVariation = 0.02f;
    public float satVariation = 0.05f;
    
    private Light lightSource;
    float h, s, v;
    
    private void Start()
    {
        lightSource = GetComponent<Light>();
        Color.RGBToHSV(lightSource.color, out h, out s, out v);
    }

    private void Update()
    {
        // Smooth flicker using Perlin noise (natural feel)
        var noiseA = Mathf.Clamp(Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f), 0.0f, 1.0f);
        var noiseB = Mathf.Clamp(Mathf.PerlinNoise(Time.time * flickerSpeed, 0.5f), 0.0f, 1.0f);
        var noiseC = Mathf.Clamp(Mathf.PerlinNoise(Time.time * flickerSpeed, 1.0f), 0.0f, 1.0f);
        
        lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, noiseA);
        lightSource.range = Mathf.Lerp(minRange, maxRange, noiseA);

        float hNew = Mathf.Repeat(h + (noiseB - 0.5f) * 2f * hueVariation, 1f);
        float sNew = Mathf.Clamp01(s + (noiseC - 0.5f) * 2f * satVariation);
        lightSource.color = Color.HSVToRGB(hNew, sNew, v);
    }
}