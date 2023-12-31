using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace Netherlands3D.Mutations
{
    public class ImportFileFromURL : MonoBehaviour
    {
        public UnityEvent<float> importProgress;
        public UnityEvent<string> filesImportedEvent;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ImportFromURL(string url, string filename);
#endif
        public void Import(string url)
        {
            Debug.Log($"Import from URL {url}");
            //Get the filename from the url (without any vars)
            var filename = Path.GetFileName(url).Split("?")[0];

#if UNITY_WEBGL && !UNITY_EDITOR
        //Callbacks for WebGL go through FileInputIndexDB        
            ImportFromURL(url, filename);
#else
            StartCoroutine(DownloadAndImport(url, filename));
#endif
        }

        private IEnumerator DownloadAndImport(string url, string filename)
        {
            UnityWebRequest getModelRequest = UnityWebRequest.Get(url);
            yield return getModelRequest.SendWebRequest();

            if (getModelRequest.result == UnityWebRequest.Result.Success)
            {
                var data = getModelRequest.downloadHandler.data;
                var localFile = $"{Application.persistentDataPath}/{filename}";
                Debug.Log($"Wrote local file: {localFile}");
                File.WriteAllBytes(localFile, data);

                filesImportedEvent.Invoke(localFile);
            }
            yield return null;
        }
    }
}