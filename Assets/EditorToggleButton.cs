using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum RotationMode { X, Y, Z }

public class EditorToggleButton : MonoBehaviour {

    public float maxScale = 2;

    private XRItem selectedItem;
    private bool isMovingObject;

    public GameObject floor;
    public Slider scaleSlider;
    public Slider rotateSlider;
    public Dropdown rotationDropdown;
    public Text selectButtonText;


	void Start () {
        scaleSlider.maxValue = maxScale;
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChange(); });

        rotateSlider.onValueChanged.AddListener(delegate { RotateValueChange(); });
        rotationDropdown.onValueChanged.AddListener(delegate { DropdownValueChange(); });

        setShouldShowEditor(false);
	}

    private void setShouldShowEditor(bool shouldShow) {
        scaleSlider.gameObject.SetActive(shouldShow);
        rotateSlider.gameObject.SetActive(shouldShow);
        rotationDropdown.gameObject.SetActive(shouldShow);
    }

    // Menu Toggles

    public void OnClickSpawn() {
        TeleportalInventory.Shared.UseCurrent();
    }

    public void OnClickMove() {
        if (isMovingObject) {
            // place object 
            setIsMoving(false);
        } else {
            // pickup object
            selectedItem = XRItemRaycaster.Shared.ItemFocus;
            setIsMoving(true);
        }
    }

    private void setIsMoving(bool isMoving) {
        if (selectedItem == null) {
            isMoving = false;
            return;
        }

        isMovingObject = isMoving;
        setShouldShowEditor(isMoving);
        
        float alpha = 1.0f;

        if (isMoving) {
            selectButtonText.text = "Unselect";
            TeleportalAr.Shared.HoldItem(selectedItem);
            //selectedItem.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            alpha = 0.5f;
        } else {
            selectButtonText.text = "Select";
            // selectedItem.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            TeleportalAr.Shared.ReleaseItem(selectedItem);
            selectedItem.gameObject.transform.SetParent(null);
            selectedItem.gameObject.transform.SetParent(floor.transform);
            selectedItem = null;
        }

        selectedItem.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        for (int i = 0; i < selectedItem.gameObject.transform.childCount; i++) {
            selectedItem.gameObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    // Slider Changes

    public void ScaleValueChange() {
        if (selectedItem != null) {
            if (selectedItem.gameObject.name.Equals("Floor")) {
                floor.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value, scaleSlider.value);
            } else {
                selectedItem.gameObject.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value, scaleSlider.value);
            }
        }
    }

    public void RotateValueChange() {
        if (ignoreThisChange) {
            ignoreThisChange = false;
            return;
        }

        RotationMode rotationMode = getRotationMode();

        Vector3 rotationVector = new Vector3(
                rotationMode == RotationMode.X ? rotateSlider.value : 0, 
                rotationMode == RotationMode.Y ? rotateSlider.value : 0,
                rotationMode == RotationMode.Z ? rotateSlider.value : 0);

        if (selectedItem != null) {
            if (selectedItem.gameObject.name.Equals("Floor")) {
                floor.transform.eulerAngles = rotationVector;
            } else {
                selectedItem.gameObject.transform.eulerAngles = rotationVector;
            }
        }
    }

    // TODO: select floor, affect all world
    // TODO: nothing selected, disable menu
    // TODO: transluecent for all children 
    // TODO: let go is not being set back to prev parent  ?

    bool ignoreThisChange = false;
    public void DropdownValueChange() {
        if (selectedItem == null) {
            return;
        }

        RotationMode rotationMode = getRotationMode();

        float newValue;
        
        Transform transform = selectedItem.gameObject.name.Equals("Floor") ? floor.transform : selectedItem.gameObject.transform;

        if (rotationMode == RotationMode.X) {
            newValue = transform.eulerAngles.x;
        } else if (rotationMode == RotationMode.Y) {
            newValue = transform.eulerAngles.y;
        } else {
            newValue = transform.eulerAngles.z;
        }

        Debug.Log(newValue);

        ignoreThisChange = true;
        rotateSlider.value = newValue;
    }

    private RotationMode getRotationMode() {
        RotationMode rotationMode;

        if (rotationDropdown.value == 0) {
            rotationMode = RotationMode.X;
        } else if (rotationDropdown.value == 1) {
            rotationMode = RotationMode.Y;
        } else {
            rotationMode = RotationMode.Z;
        }

        return rotationMode;
    }

    // TODO: Move should be a toggle

}
