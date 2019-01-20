using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DrawBackgroundController2 : MonoBehaviour
{

    private LineController lineController;

    private enum State { None, Front, Side, Done };
    private State currentState = State.None;

    // public GameObject BackgroundPlane;
    public Material frontPlane;
    public RenderTexture SaveTexture;

    void Start() {
        Debug.Log("State starts as: " + currentState);
        lineController = LineController.Shared; 
        setBackgroundImage("Assets/test.jpg");
        currentState = State.None;
    }

    bool doneSave = false;
    public void Next() {
        Debug.Log("Next() CS: " + currentState);
        if (currentState == State.None) {
            Debug.Log("None -> Front");
            // Save front
            SaveAndClear("sketch-F-0");
            while (!doneSave) {}
            setBackgroundImage("/SavedImages/sketch-F-0.png");
            currentState = State.Front;
        } else if (currentState == State.Front) {
            // Save("sketch-S-0");
            currentState = State.Side;
        } else if (currentState == State.Side) {
            // SendToServer();
            currentState = State.Done;
        } else {
            // Reset
            currentState = State.None;
        }
    }

    public void ICanSeeClearlyNowThatTheRainIsGone() {
        if (lineController != null) lineController.clearAll();
    }

    public void SendToServer() {
        StartCoroutine(helpiboi());
    }

    IEnumerator helpiboi() {
        if (!System.IO.File.Exists("/SavedImages/sketch-F-0.png") || !System.IO.File.Exists("/SavedImages/sketch-S-0.png")) {
            Debug.Log("Missing one or more sketches");
            yield return null;
        }
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add( new MultipartFormFileSection("file[0]", "Assets/SavedImages/sketch-F-0.png") );
        formData.Add( new MultipartFormFileSection("file[1]", "Assets/SavedImages/sketch-S-0.png") );

        UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.49:5000/submit", formData);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Form upload complete!");
        }
    }

    public void Save(string filename) {
        setBackgroundImage("Assets/reset.png");
        StartCoroutine(CoSave(filename));
    }

    public void SaveAndClear(string filename) {
        setBackgroundImage("Assets/reset.png");
        StartCoroutine(CoSaveClear(filename));
    }

    public void Save() {
        setBackgroundImage("Assets/reset.png");
        // TODO: add some unique number or date/time to filename
        StartCoroutine(CoSave("SavedImage"));
    }

    private IEnumerator CoSave(string filename) {
        Debug.Log("Start CoSave");
        yield return new WaitForEndOfFrame();

        RenderTexture.active = SaveTexture;

        var texture2D = new Texture2D(SaveTexture.width, SaveTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, SaveTexture.width, SaveTexture.height), 0, 0);
        texture2D.Apply();
        Debug.Log("2");
        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/SavedImages/" + filename + ".png", data);
        Debug.Log("3");
    }

    private IEnumerator CoSaveClear(string filename) {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = SaveTexture;

        var texture2D = new Texture2D(SaveTexture.width, SaveTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, SaveTexture.width, SaveTexture.height), 0, 0);
        texture2D.Apply();
        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/SavedImages/" + filename + ".png", data);

        doneSave = true;

        ICanSeeClearlyNowThatTheRainIsGone();
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
        // Debug.Log(currentState);
    }
}
