using System.Collections;
using UnityEngine;
 
[RequireComponent(typeof(AudioSource))]
public class AudioSpectrumToEmission2 : MonoBehaviour
{
    public Material material;  // The material whose emission will change
    public Color baseEmissionColor = Color.white;  // Base emission color
    public float emissionMultiplier = 1.0f;  // Multiplier for emission intensity
 
    private AudioSource audioSource;
    private float[] spectrumData = new float[64];  // Array to hold audio spectrum data
 
    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
 
        // Ensure the material is assigned
        if (material == null)
        {
            Debug.LogError("No material assigned! Please assign a material.");
        }
        else
        {
            // Enable emission on the material
            material.EnableKeyword("_EMISSION");
        }
    }
 
    void Update()
    {
        // If the audio is playing, adjust emission based on spectrum data
        if (audioSource.isPlaying)
        {
            // Get the audio spectrum data (64 frequency bands)
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);
 
            // Calculate the average spectrum value (intensity based on audio)
            float averageSpectrumValue = 0f;
            for (int i = 0; i < spectrumData.Length; i++)
            {
                averageSpectrumValue += spectrumData[i];
            }
            averageSpectrumValue /= spectrumData.Length;
 
            // Calculate the emission intensity, clamped between 0.5 and 1.0
            float intensity = Mathf.Clamp(averageSpectrumValue * emissionMultiplier, 0.5f, 5f);
 
            // Set the material's emission color with the updated intensity
            Color emissionColor = baseEmissionColor * intensity;
            material.SetColor("_EmissionColor", emissionColor);
        }
        else
        {
            // If the audio has stopped, turn off emission
            material.SetColor("_EmissionColor", Color.black);
            material.DisableKeyword("_EMISSION");
        }
    }
}