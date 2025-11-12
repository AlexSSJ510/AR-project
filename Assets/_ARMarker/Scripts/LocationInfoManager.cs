using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocationInfoManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationData
    {
        public string markerName;
        public string locationName;
        [TextArea(5, 10)]
        public string description;
        public AudioClip audioGuide;
    }

    [Header("Información por Ubicación")]
    public LocationData[] locations;

    [Header("Referencias UI")]
    public GameObject infoPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public AudioSource audioSource;
    public Button closeButton;
    public Button audioButton;

    private string currentLocation;

    void Start()
    {
        // VERIFICACIONES DE SEGURIDAD
        if (audioSource == null)
        {
            Debug.LogError("? AudioSource no asignado! Buscando automáticamente...");
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("? AudioSource creado automáticamente");
            }
        }

        // Configurar botones
        if (closeButton != null)
            closeButton.onClick.AddListener(HideInfo);
        else
            Debug.LogWarning("?? CloseButton no asignado");

        if (audioButton != null)
            audioButton.onClick.AddListener(ToggleAudio);
        else
            Debug.LogWarning("?? AudioButton no asignado");

        HideInfo();
    }

    public void ShowLocationInfo(string markerName)
    {
        if (string.IsNullOrEmpty(markerName))
        {
            Debug.LogWarning("?? Marker name está vacío");
            return;
        }

        foreach (LocationData location in locations)
        {
            if (markerName == location.markerName)
            {
                currentLocation = markerName;

                if (titleText != null)
                    titleText.text = location.locationName;

                if (descriptionText != null)
                    descriptionText.text = location.description;

                if (infoPanel != null)
                    infoPanel.SetActive(true);

                // Preparar audio
                if (audioSource != null && location.audioGuide != null)
                {
                    audioSource.clip = location.audioGuide;
                }

                Debug.Log("?? Mostrando info: " + location.locationName);
                return;
            }
        }

        Debug.LogWarning("? No se encontró información para: " + markerName);
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        StopAudio();
    }

    public void ToggleAudio()
    {
        if (audioSource == null)
        {
            Debug.LogError("? AudioSource no asignado!");
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            Debug.Log("?? Audio pausado");
        }
        else
        {
            if (audioSource.clip != null)
            {
                audioSource.Play();
                Debug.Log("?? Reproduciendo audio");
            }
            else
            {
                Debug.LogWarning("?? No hay clip de audio asignado para esta ubicación");
            }
        }
    }

    public void StopAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}