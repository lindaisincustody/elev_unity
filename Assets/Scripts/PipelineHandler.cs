using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PipelineHandler : MonoBehaviour
{
    public RenderPipelineAsset asset;

    // Update is called once per frame
    private void Start()
    {
        GraphicsSettings.renderPipelineAsset = asset;
    }
}
