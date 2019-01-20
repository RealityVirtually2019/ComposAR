using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DrawBackgroundController : MonoBehaviour
{

    private LineController lineController;
    public enum Side {F, S};
    private int currentState = 0;
    // 0 = None
    // 1 = Front
    // 2 = Side
    // 3 = Done

    // private enum State { None, Front, Side, Done };
    // private State currentState = State.None;

    // public GameObject BackgroundPlane;
    public Material frontPlane;
    public RenderTexture SaveTexture;

    void Start() {
        Debug.Log("State starts as: " + currentState);
        lineController = LineController.Shared; 
        setBackgroundImage("Assets/test.jpg");
    }

    //getters for state
    bool isFront() {
        return currentState == 1;
    }
    bool isSide() {
        return currentState == 2;
    }

    public void Next() {
        Debug.Log("Next() CS: " + currentState);
        if (currentState == 0) {

            // Save front
            Save("sketch-F-0");
            // Debug.Log("Saved");
            // ICanSeeClearlyNowThatTheRainIsGone();
            // setBackgroundImage("/SavedImages/sketch-F-0.png");
            currentState = 1;
        } else if (currentState == 1) {
            // Save("sketch-S-0");
            currentState = 2;
        } else if (currentState == 2) {
            // SendToServer();
            currentState = 3;
        } else {
            // Reset
            currentState = 0;
        }
    }

    public void ICanSeeClearlyNowThatTheRainIsGone() {
        if (lineController != null) lineController.clearAll();
    }

    public void SendToServer() {
        StartCoroutine(helpiboi());
    }

    IEnumerator helpiboi() {
        if (!System.IO.File.Exists(Application.dataPath + "/SavedImages/SavedImage.png")) {
        //if (!System.IO.File.Exists("/SavedImages/sketch-F-0.png") || !System.IO.File.Exists("/SavedImages/sketch-S-0.png")) {
            Debug.Log("Missing sketch");
            yield return null;
        }
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add( new MultipartFormFileSection("file[0]", Application.dataPath + "/SavedImages/SavedImage.png") );

        Debug.Log("Setting...");
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.43.7:5000/submit", formData);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Form upload complete!");
        }
    }

    private void SaveUp(string filename, bool setBackground) {
        StartCoroutine(CoSaveUp(filename, setBackground));
    }

    public void Save(string filename) {
        setBackgroundImage("Assets/reset.png");
        StartCoroutine(CoSave(filename));
    }

    public void Save() {
        setBackgroundImage("Assets/reset.png");
        // TODO: add some unique number or date/time to filename
        StartCoroutine(CoSave("SavedImage"));
    }

    public void SaveWithBg() {
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

        // SendToServer();
        // ComposarStateManager.Shared.SetMode(ComposarMode.Layout);
    }

    private IEnumerator CoSaveUp(string filename, bool setBackground) {
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

        if (setBackground) {
            setBackgroundImage(Application.dataPath + "/SavedImages/" + filename + ".png");
        }
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
        Debug.Log(currentState);
    }
}
