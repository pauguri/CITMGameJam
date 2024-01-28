using UnityEngine;
using UnityEngine.Rendering;

public class ShadowmapExtractor : MonoBehaviour
{
    [SerializeField] private int projectionLayer = 1;

    void Start()
    {
        var cb = new CommandBuffer();
        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        var _LightShadowmap = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32);
        _LightShadowmap.filterMode = FilterMode.Point;
        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        var id = new RenderTargetIdentifier(_LightShadowmap);
        cb.Blit(shadowmap, id);
        cb.SetGlobalTexture("_LightShadowmap" + projectionLayer, id);
        Light m_Light = GetComponent<Light>();
        m_Light.AddCommandBuffer(LightEvent.AfterShadowMap, cb);
    }
}
