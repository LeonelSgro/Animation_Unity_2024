using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSpectrumToEmission : MonoBehaviour
{
    public Renderer targetRenderer; // El Renderer del objeto que contiene los materiales
    public Color baseEmissionColor = Color.white;  // Color base de la emisión
    public float emissionMultiplier = 4000;  // Factor de escalabilidad para la intensidad de emisión
    public float spectrumScaleFactor = 100.0f; // Factor para escalar el valor del espectro
    public float smoothingFactor = 0.15f; // Factor de suavizado (ajusta según sea necesario)

    private AudioSource audioSource;
    private float[] spectrumData = new float[64];  // Array para datos de espectro de audio
    private Material whiteEmissiveMaterial;

    private float currentIntensity = 0f; // Intensidad actual de la emisión

    void Start()
    {
        // Obtener el AudioSource adjunto al GameObject
        audioSource = GetComponent<AudioSource>();

        // Buscar el material llamado "Whiteemissive" entre los materiales del renderer
        if (targetRenderer != null)
        {
            whiteEmissiveMaterial = targetRenderer.materials.FirstOrDefault(m => m.name.Contains("Whiteemissive"));
            if (whiteEmissiveMaterial != null)
            {
                whiteEmissiveMaterial.EnableKeyword("_EMISSION");
            }
            else
            {
                Debug.LogError("Material 'Whiteemissive' no encontrado en los materiales asignados.");
            }
        }
        else
        {
            Debug.LogError("No se ha asignado un Renderer. Asigna el Renderer del objeto.");
        }
    }

    void Update()
    {
        if (audioSource.isPlaying && whiteEmissiveMaterial != null)
        {
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);

            // Calcular el valor promedio del espectro
            float averageSpectrumValue = Mathf.Sqrt(spectrumData.Average());
            Debug.Log("Average Spectrum Value: " + averageSpectrumValue);

            // Escalar el valor promedio del espectro
            float scaledSpectrumValue = averageSpectrumValue * spectrumScaleFactor; // Aplicar factor de escalado
            Debug.Log("Scaled Spectrum Value: " + scaledSpectrumValue);

            // Calcular la nueva intensidad usando el valor escalado
            float newIntensity = scaledSpectrumValue * emissionMultiplier; // Usar el valor escalado para la intensidad

            // Suavizar la transición entre la intensidad actual y la nueva intensidad
            currentIntensity = Mathf.Lerp(currentIntensity, newIntensity, smoothingFactor * Time.deltaTime); // Interpolación

            // Limitar la intensidad para evitar que se vuelva excesiva
            float clampedIntensity = Mathf.Clamp(currentIntensity, 0f, 5f); // Ajustar el límite superior según sea necesario

            // Ajustar el color de emisión según la intensidad limitada
            Color emissionColor = baseEmissionColor * clampedIntensity; // Asegúrate de que la intensidad no sea negativa
            whiteEmissiveMaterial.SetColor("_EmissionColor", emissionColor);
        }
        else if (whiteEmissiveMaterial != null)
        {
            whiteEmissiveMaterial.SetColor("_EmissionColor", Color.black);
            whiteEmissiveMaterial.DisableKeyword("_EMISSION");
        }
    }
}
