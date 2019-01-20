using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBackgroundController : MonoBehaviour
{

    public GameObject BackgroundPlane;
    public Material frontPlane;


    void Start() {
        setBackgroundImage("Assets/test.jpg");
    }

    public void setBackgroundImage(string filePath) {
        if (!System.IO.File.Exists(filePath)) {
            Debug.Log("File not found '" + filePath + "'");
            return;
        }

        // Load bytes into texture
        var bytes = System.IO.File.ReadAllBytes(filePath);
        var tex = new Texture2D(1, 1);
        tex.LoadImage(bytes);
        frontPlane.mainTexture = tex;

        // Apply to plane
        MeshRenderer mr = BackgroundPlane.GetComponent<MeshRenderer>();
        mr.material = frontPlane;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
