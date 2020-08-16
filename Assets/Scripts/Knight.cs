using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    public Renderer[] EnvironmentRenderers;
    public float offsetSpeed;


    public void Update()
    {
        foreach (var r in EnvironmentRenderers)
        {
            r.material.mainTextureOffset += Vector2.right * Time.deltaTime * offsetSpeed;
        }
    }
}
