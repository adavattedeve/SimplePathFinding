using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    private MeshRenderer meshRenderer;
    public Material Material
    {
        set
        {
            meshRenderer.sharedMaterial = value;
        }
    }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

}
