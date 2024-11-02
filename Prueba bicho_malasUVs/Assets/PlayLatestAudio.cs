using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;

public class PlayLatestAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private string folderPath; // Path to the audio folder in persistent data path
    private float checkInterval = 2f; // Check interval in seconds
    private string lastPlayedFile;
    private System.DateTime lastPlayedFileTime; // Track last played file write time

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing.");
            return;
        }

        // Define the path of the audio folder in persistent data path
        folderPath = Path.Combine(Application.persistentDataPath, "AudioFiles");

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
            if (!string.IsNullOrEmpty(latestAudioFile))
            {
                System.DateTime latestFileTime = File.GetLastWriteTime(latestAudioFile);

                // Check if this is a new or updated file
                if (latestAudioFile != lastPlayedFile || latestFileTime > lastPlayedFileTime)
                {
                    lastPlayedFile = latestAudioFile;
                    lastPlayedFileTime = latestFileTime;
                    yield return StartCoroutine(LoadAndPlayAudio(latestAudioFile));
                }
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
        Debug.Log("Loading audio file: " + filePath); // Debug log for file loading

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

            // Unload any unused assets to free up memory
            Resources.UnloadUnusedAssets();

            // Assign the loaded clip to the audio source
            audioSource.clip = clip;
            audioSource.Play();

            Debug.Log("Playing audio clip: " + clip.name); // Debug log for playing audio

            // Wait until the audio has finished playing
            yield return new WaitWhile(() => audioSource.isPlaying);

            // Delete the file after playback
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
}
