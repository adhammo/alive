using UnityEngine;
using UnityEngine.Rendering;

public class Underworld : MonoBehaviour
{
    [Header("Upworld")]
    public Color UpworldFogColor;
    public Material UpworldSkybox;
    public GameObject[] UpworldObjects;

    [Header("Underworld")]
    public Color UnderworldFogColor;
    public Material UnderworldSkybox;
    public Color UnderworldAmbientColor;
    public GameObject[] UnderworldObjects;

    private void OnEnable()
    {
        RenderSettings.fogColor = UnderworldFogColor;
        RenderSettings.skybox = UnderworldSkybox;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = UnderworldAmbientColor;
        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        foreach (var obj in UpworldObjects)
        {
            if (obj)
                obj.SetActive(false);
        }
        foreach (var obj in UnderworldObjects)
        {
            if (obj)
                obj.SetActive(true);
        }
    }

    private void OnDisable()
    {
        RenderSettings.fogColor = UpworldFogColor;
        RenderSettings.skybox = UpworldSkybox;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
        foreach (var obj in UnderworldObjects)
        {
            if (obj)
                obj.SetActive(false);
        }
        foreach (var obj in UpworldObjects)
        {
            if (obj)
                obj.SetActive(true);
        }
    }
}
