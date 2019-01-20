using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrawBackgroundController : MonoBehaviour
{

    // public GameObject BackgroundPlane;
    public Material frontPlane;
    public RenderTexture SaveTexture;

    void Start() {
        setBackgroundImage("Assets/test.jpg");
    }

    public void Save() {
        setBackgroundImage("Assets/reset.png");
        StartCoroutine(CoSave());
    }

    private IEnumerator CoSave() {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = SaveTexture;

        var texture2D = new Texture2D(SaveTexture.width, SaveTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, SaveTexture.width, SaveTexture.height), 0, 0);
        texture2D.Apply();

        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/savedImage.png", data);
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
        MeshRenderer mr = this.GetComponent<MeshRenderer>();
        mr.material = frontPlane;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
