

using System;
using System.Collections;

using UnityEngine;



[RequireComponent(typeof(AudioSource))]

public class AudioSpectrumToEmission : MonoBehaviour

{

    public Material material;  // The material whose emission will change

    public float emissionMultiplier = 1.0f;  // Multiplier for emission intensity

    public float minIntensity = 0.5f;  // Minimum intensity value

    public float maxIntensity = 1.8f;   // Maximum intensity value

    public float stoppedIntensity = 0f; // Intensity after audio stops



    private AudioSource audioSource;

    private float[] spectrumData = new float[64];  // Array to hold audio spectrum data

    private Color baseEmissionColor;  // Store the current emission color



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

            // Retrieve the initial emission color from the material

            baseEmissionColor = material.GetColor("_EmissionColor");

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



            // Scale average spectrum value to fit the desired intensity range

            float intensity = Mathf.Lerp(minIntensity, maxIntensity, averageSpectrumValue * emissionMultiplier);
                Debug.Log(intensity);
    
            intensity = Mathf.Clamp(intensity, minIntensity, maxIntensity);



            // Set the material's emission color with the updated intensity

            Color emissionColor = baseEmissionColor * intensity;

            material.SetColor("_EmissionColor", emissionColor);

        }

        else

        {

            // Reset intensity to stopped value when audio stops

            Color emissionColor = baseEmissionColor * stoppedIntensity;

            material.SetColor("_EmissionColor", emissionColor);

        }

    }

}
