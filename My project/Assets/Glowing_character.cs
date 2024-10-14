using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Glowing_character : MonoBehaviour
{
    public AudioSource audioSource; // Reference to your audio source
    private Bloom bloomLayer; // Reference to the bloom layer
    private float[] spectrumData = new float[256]; // Array for audio spectrum data

    void Start()
    {
        // Get the Post Process Volume from the main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            PostProcessVolume postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
            if (postProcessVolume != null)
            {
                postProcessVolume.profile.TryGetSettings(out bloomLayer);
            }
            else
            {
                Debug.LogError("Post Process Volume is not found on the Main Camera!");
            }
        }
        else
        {
            Debug.LogError("Main Camera is not found!");
        }
    }

    void Update()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);

            // Log the spectrum data
            for (int i = 0; i < spectrumData.Length; i++)
            {
                Debug.Log($"Spectrum[{i}]: {spectrumData[i]}");
            }

            float frequencyValue = spectrumData[0] * 10000; // Adjust multiplier as needed
            Debug.Log("Frequency Value: " + frequencyValue); // Log frequency value

            if (bloomLayer != null)
            {
                bloomLayer.intensity.value = Mathf.Clamp(frequencyValue, 100f, 190f); // Clamp between 100 and 190
            }
        }
    }
}
