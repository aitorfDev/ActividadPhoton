using Fusion;
using UnityEngine;

public class LocalVisibilityHandler : NetworkBehaviour
{
    // Drag the MeshRenderer component of the "nose" object here in the Inspector.
    [SerializeField] private MeshRenderer noseRenderer;
    [SerializeField] private MeshRenderer eyeRenderer;
    [SerializeField] private MeshRenderer eye2Renderer;
    [SerializeField] private GameObject camera;


    // This method is called when the object is spawned and set up on the network.
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {

            if (noseRenderer != null)
            {
                noseRenderer.enabled = false;
                eyeRenderer.enabled = false;
                eye2Renderer.enabled = false;
                camera.SetActive(true);
            }
        }

        else
            {
      
                if (noseRenderer != null)
                {
                    noseRenderer.enabled = true;
                    eyeRenderer.enabled = true;
                    eye2Renderer.enabled = true;
                    camera.SetActive(false);
                }
            }
    }
}