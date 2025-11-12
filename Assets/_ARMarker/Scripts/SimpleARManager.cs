using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class SimpleARManager : MonoBehaviour
{
    [Header("Modelos para cada marcador")]
    public List<GameObject> modelsToSpawn;

    [Header("Info Manager")]
    public LocationInfoManager infoManager;

    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    void Start()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnImagesChanged;
            SetupModels();
            Debug.Log("? AR Manager inicializado correctamente");
        }
        else
        {
            Debug.LogError("? No se encontró ARTrackedImageManager");
        }
    }

    void SetupModels()
    {
        Debug.Log("?? Iniciando setup de modelos...");

        // Nombres esperados de los marcadores
        string[] expectedNames = { "Biblioteca_Marker", "Cafeteria_Marker", "Laboratorio_Marker", "Auditorio_Marker" };

        for (int i = 0; i < modelsToSpawn.Count && i < expectedNames.Length; i++)
        {
            if (modelsToSpawn[i] != null)
            {
                GameObject newModel = Instantiate(modelsToSpawn[i], Vector3.zero, Quaternion.identity);
                newModel.name = expectedNames[i]; // Usar nombre del marcador
                newModel.SetActive(false);
                spawnedObjects.Add(expectedNames[i], newModel);
                Debug.Log("? Creado: " + expectedNames[i]);
            }
        }
        Debug.Log("?? Setup completo. Modelos creados: " + spawnedObjects.Count);
    }

    void OnImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            UpdateModel(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            UpdateModel(trackedImage);
        }

        foreach (var trackedImage in args.removed)
        {
            HideModel(trackedImage);
        }
    }

    void UpdateModel(ARTrackedImage trackedImage)
    {
        if (trackedImage == null) return;

        string imageName = trackedImage.referenceImage.name;

        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("?? Image name is null or empty!");
            return;
        }

        if (spawnedObjects.ContainsKey(imageName))
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                spawnedObjects[imageName].SetActive(true);
                spawnedObjects[imageName].transform.position = trackedImage.transform.position;
                spawnedObjects[imageName].transform.rotation = trackedImage.transform.rotation;

                if (infoManager != null)
                {
                    infoManager.ShowLocationInfo(imageName);
                }

                Debug.Log("?? Mostrando: " + imageName);
            }
            else
            {
                spawnedObjects[imageName].SetActive(false);
                Debug.Log("?? Ocultando: " + imageName);
            }
        }
        else
        {
            Debug.LogWarning("? No hay modelo para: " + imageName);
        }
    }

    void HideModel(ARTrackedImage trackedImage)
    {
        if (trackedImage == null) return;

        string imageName = trackedImage.referenceImage.name;
        if (!string.IsNullOrEmpty(imageName) && spawnedObjects.ContainsKey(imageName))
        {
            spawnedObjects[imageName].SetActive(false);
            Debug.Log("? Removido: " + imageName);
        }
    }
}