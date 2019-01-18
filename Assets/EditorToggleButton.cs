using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum EditorMode { Scale, Rotate, Move, Unselected };
enum RotationMode { X, Y, Z }

public class EditorToggleButton : MonoBehaviour {

    public float maxScale = 5;

    private EditorMode mode = EditorMode.Unselected;
    private GameObject selectedObject;

    public GameObject floor;
    public Slider scaleSlider;
    public Slider rotateSlider;
    public Dropdown rotationDropdown;

	void Start () {
        scaleSlider.maxValue = maxScale;
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChange(); });

        rotateSlider.onValueChanged.AddListener(delegate { RotateValueChange(); });
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
        if (mode == EditorMode.Unselected) {
            floor.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value, scaleSlider.value);
        } else {
            selectedObject.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value, scaleSlider.value);
        }
    }

    public void RotateValueChange() {
        RotationMode rotationMode;

        if (rotationDropdown.value == 0) {
            rotationMode = RotationMode.X;
        } else if (rotationDropdown.value == 1) {
            rotationMode = RotationMode.Y;
        } else {
            rotationMode = RotationMode.Z;
        }

        Vector3 rotationVector = new Vector3(
                rotationMode == RotationMode.X ? rotateSlider.value : 0, 
                rotationMode == RotationMode.Y ? rotateSlider.value : 0,
                rotationMode == RotationMode.Z ? rotateSlider.value : 0);

        if (mode == EditorMode.Unselected) {
            floor.transform.eulerAngles = rotationVector;
        } else {
            selectedObject.transform.eulerAngles = rotationVector;
        }
    }

    // TODO: add selection toggles

}
