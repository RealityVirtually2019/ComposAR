using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum EditorMode { Scale, Rotate, Move, Unselected };

public class EditorToggleButton : MonoBehaviour {

    private EditorMode mode = EditorMode.Unselected;

    public GameObject floor;
    public Slider scaleSlider;

	void Start () {
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChange(); });
	}

    // Menu Toggles

    public void OnClickScale() {
        mode = EditorMode.Scale;
    }

    public void OnClickRotate() {
        mode = EditorMode.Rotate;
    }

    public void OnClickMove() {
        mode = EditorMode.Move;
    }

    // Slider Changes

    public void ScaleValueChange() {
        floor.transform.localScale += new Vector3(0.1F, 0, 0);
    }

}
