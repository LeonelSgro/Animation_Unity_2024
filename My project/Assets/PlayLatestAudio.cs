using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;

public class PlayLatestAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private string folderPath; // Path to the audio folder in Assets
    private float checkInterval = 2f; // Check interval in seconds
    private string lastPlayedFile;
    private float[] audioData = new float[256]; // Array to store audio data

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing.");
            return;
        }

        // Define the path of the audio folder in the Assets folder
        folderPath = Path.Combine(Application.dataPath, "AudioFiles");

        // Check if the folder exists; if not, create it
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created missing directory: " + folderPath);
        }

        StartCoroutine(CheckForNewAudioFiles());
    }

    IEnumerator CheckForNewAudioFiles()
    {
        while (true)
        {
            string latestAudioFile = GetLatestAudioFile();
            if (!string.IsNullOrEmpty(latestAudioFile) && latestAudioFile != lastPlayedFile)
            {
                lastPlayedFile = latestAudioFile;
                yield return StartCoroutine(LoadAndPlayAudio(latestAudioFile));
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    string GetLatestAudioFile()
    {
        var audioFiles = Directory.GetFiles(folderPath, "*.*")
                                  .Where(f => f.EndsWith(".wav") || f.EndsWith(".mp3"))
                                  .OrderByDescending(File.GetLastWriteTime)
                                  .ToList();

        return audioFiles.FirstOrDefault(); // Return the latest file or null if there are no files
    }

    IEnumerator LoadAndPlayAudio(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load audio file: " + www.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip == null)
            {
                Debug.LogError("Failed to create AudioClip from file.");
                yield break;
            }

            audioSource.clip = clip;
            audioSource.Play();

            // Wait until the audio has finished playing
            yield return new WaitWhile(() => audioSource.isPlaying);

            try
            {
                File.Delete(filePath);
                Debug.Log("Audio file deleted: " + filePath);
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to delete audio file: " + e.Message);
            }
        }
    }

    void Update()
    {
        if (audioSource.isPlaying)
        {
            audioSource.GetOutputData(audioData, 0); // Fill audioData array with audio data
            //ScaleObject(audioData); // Use the data to scale the object or other purposes
        }
    }

  /*  // Function to scale the object based on audio data
    private void ScaleObject(float[] spectrumData)
    {
        // Calculate the average spectrum value
        float averageSpectrumValue = spectrumData.Average();
        
        // Scale factor (adjust this value to suit your needs)
        float scaleFactor = 1000f;

        // Scale the object based on the average spectrum value
        float newScale = Mathf.Clamp(averageSpectrumValue * scaleFactor, 0.1f, 5f); // Keep the scale within reasonable limits

        // Apply the new scale to the object
        transform.localScale = new Vector3(newScale, newScale, newScale); // Uniform scaling
    }*/
}
