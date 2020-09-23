using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderHandler : MonoBehaviour
{
    // Creates a material from shader and texture references.
    public Shader shader; 
	//public Texture texture;

    void Start()
    {
        Renderer rend = GetComponent<Renderer> ();

        rend.material = new Material(shader);
        var texture = Resources.Load<Texture2D>("Textures/360-2");
		rend.material.mainTexture = texture;
    }
}
