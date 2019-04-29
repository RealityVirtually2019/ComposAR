using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Teleportal;

enum EditorMode { Camera, LookingAtObject, SelectedObject, None }

public class EditorToggleButton : MonoBehaviour {

    public float maxScale = 2;

    private XRItem selectedItem;
    private bool isMovingObject;

    public GameObject floor;

    // singleton reference
    public static EditorToggleButton Shared;
    public bool waitingForDuplication = false;

    // selectedobject items 
    public Slider scaleSlider;
    public Slider rotateSlider;
    public Dropdown rotationDropdown;
    public Dropdown scaleDropdown;

    // looking at object options 
    public Button selectButton;
    public Button duplicateButton;
    public Button deleteButton;
    public Button exitButton;

    // camera options 
    public Button takeShotButton;
    public RawImage cameraDisplay;

    public Text selectButtonText;
    public Toggle snapGridToggle;

    private Vector3 selectedObjectEulerAngles = Vector3.zero;
    private EditorMode currentEditMode = EditorMode.SelectedObject;
    private GameObject actuallySelectedObject;
    private ComposarCamera highlightedCamera;

    void Awake() {
        EditorToggleButton.Shared = this;
    }

	void Start () {
        scaleSlider.value = 1;
        scaleDropdown.value = 3;
        scaleSlider.maxValue = maxScale;
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChange(); });

        rotateSlider.onValueChanged.AddListener(delegate { RotateValueChange(); });
        rotationDropdown.onValueChanged.AddListener(delegate { DropdownValueChange(); });
        scaleDropdown.onValueChanged.AddListener(delegate { ScaleDropdownValueChange(); });

        setEditorMode(EditorMode.None);
	}

    // i guess they never miss, huh?
    void Update() {
        EditorMode newEditorMode = EditorMode.None;

        // they already have an item, so they are moving an object 
        if (selectedItem != null) {
            // Snap position to actualMovePosition

            if (snapGridToggle.isOn) {
                Debug.Log("test");
                float GridSnapSize = 0.25f; // meters

                Vector3 snappedPosition = actuallySelectedObject.transform.position;
                snappedPosition.x = (float) Math.Round(snappedPosition.x / GridSnapSize) * GridSnapSize;
                snappedPosition.y = (float) Math.Round(snappedPosition.y / GridSnapSize) * GridSnapSize;
                snappedPosition.z = (float) Math.Round(snappedPosition.z / GridSnapSize) * GridSnapSize;
            
                /* Experiments:
                float div = 5.0f;
                double k = 0.1;
                Debug.Log("actual transfor x " + Math.Abs(actuallySelectedObject.transform.position.x) % 2);
                Debug.Log("actual transfor y " + Math.Abs(actuallySelectedObject.transform.position.y) % 2);
                Debug.Log("actual transfor z " + Math.Abs(actuallySelectedObject.transform.position.z) % 2);

                Vector3 snappedPosition = new Vector3(
                    Math.Abs(actuallySelectedObject.transform.position.x) % 2 <= k ? actuallySelectedObject.transform.position.x : selectedItem.gameObject.transform.position.x, 
                    Math.Abs(actuallySelectedObject.transform.position.y) % 2 <= k ? actuallySelectedObject.transform.position.y : selectedItem.gameObject.transform.position.y, 
                    Math.Abs(actuallySelectedObject.transform.position.z) % 2 <= k ? actuallySelectedObject.transform.position.z : selectedItem.gameObject.transform.position.z); 

                float absX = Math.Abs(actuallySelectedObject.transform.position.x - actuallySelectedObject.transform.position.x) % div;
                float absY = Math.Abs(actuallySelectedObject.transform.position.y - actuallySelectedObject.transform.position.y) % div;
                float absZ = Math.Abs(actuallySelectedObject.transform.position.z - actuallySelectedObject.transform.position.z) % div;
                print(absX);
                print(absY);
                print(absZ);
                snappedPosition.y = absY < k ? actuallySelectedObject.transform.position.y : selectedItem.gameObject.transform.position.y;
                snappedPosition.z = absZ < k ? actuallySelectedObject.transform.position.z : selectedItem.gameObject.transform.position.z;
                */

                // LOCAL ONLY version - selectedItem.gameObject.transform.position = snappedPosition;
                Teleportal.Ar.Shared.MoveItem(selectedItem.GetId(), snappedPosition.x, snappedPosition.y, snappedPosition.z, selectedObjectEulerAngles.y, selectedObjectEulerAngles.x);
            } else {
                // TODO: rotation z does not work
                // TODO: grab mesh and set alpha
                selectedItem.gameObject.transform.eulerAngles = selectedObjectEulerAngles;
            }

            
            newEditorMode = EditorMode.SelectedObject;
        } else {
            XRItem lookingAtItem = XRItemRaycaster.Shared.GetFocusedItem();

            // if they are looking at an item and not holding one, determine if it is camera/object  
            if (lookingAtItem != null) {
                GameObject lookingAtObject = lookingAtItem.gameObject;

                if (lookingAtObject.transform.name.Contains("camera")) {
                    newEditorMode = EditorMode.Camera;
                } else {
                    newEditorMode = EditorMode.LookingAtObject;
                }
            } else {
                // if they are not looking at an item, they are looking at none
                newEditorMode = EditorMode.None;
            }
        }

        setEditorMode(newEditorMode);
    }

    private void setEditorMode(EditorMode newMode) {
        XRItem lookingAtItem = XRItemRaycaster.Shared.GetFocusedItem();
        
        if (currentEditMode == EditorMode.Camera && newMode != EditorMode.Camera) {
            // disable camera 
            if (highlightedCamera != null) {
                highlightedCamera.DisableRender();
            }
        } else if (newMode == currentEditMode) {
            // double check if looking at floor, dup/delete button are deleted 
            if (newMode == EditorMode.LookingAtObject && lookingAtItem != null && !lookingAtItem.gameObject.transform.name.Contains("Floor")) {
                duplicateButton.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(true);
            }
            
            return; 
        }

        bool showExitButton = false;

        // selected object controls 
        bool showScaleSlider = false;      
        bool showRotateSlider = false;
        bool showRotateDropdown = false;
        bool showScaleDropdown = false;

        // looking at object controls
        bool showSelectButton = false;
        bool showDuplicateButton = false;
        bool showDeleteButton = false;
        
        bool showTakeShotButton = false;
        bool showRawImage = false;

        if (newMode == EditorMode.None) {
            showExitButton = true;
        } else if (newMode == EditorMode.LookingAtObject) {
            showSelectButton = true;
            showExitButton = true;

            // prevent deleting and duplicating of floor 
            if (lookingAtItem != null && !lookingAtItem.gameObject.transform.name.Contains("Floor")) {
                showDuplicateButton = true;
                showDeleteButton = true;
            }
        } else if (newMode == EditorMode.SelectedObject) {
            showSelectButton = true;
            showScaleSlider = true;
            showRotateSlider = true;
            showRotateDropdown = true;
            showScaleDropdown = true;

            // set values back 
            if (selectedItem != null) {
                DropdownValueChange();
                ScaleDropdownValueChange();
            }

            if (lookingAtItem != null && lookingAtItem.gameObject.transform.name.Contains("camera")) {
                highlightedCamera = lookingAtItem.gameObject.GetComponent<ComposarCamera>();
                highlightedCamera.EnableRender();
                cameraDisplay.texture = lookingAtItem.gameObject.GetComponent<ComposarCamera>().GetRenderTexture();
                showRawImage = true;
                showTakeShotButton = true; 
            }
        } else if (newMode == EditorMode.Camera) {
            showSelectButton = true;
            showDuplicateButton = true;
            showDeleteButton = true;
            showRawImage = true;
            showTakeShotButton = true;

            if (lookingAtItem != null) {
                highlightedCamera = lookingAtItem.gameObject.GetComponent<ComposarCamera>();
                highlightedCamera.EnableRender();
                cameraDisplay.texture = highlightedCamera.GetRenderTexture();
            }
        }

        scaleSlider.gameObject.SetActive(showScaleSlider);
        rotateSlider.gameObject.SetActive(showRotateSlider);
        rotationDropdown.gameObject.SetActive(showRotateDropdown);
        scaleDropdown.gameObject.SetActive(showScaleDropdown);

        selectButton.gameObject.SetActive(showSelectButton);
        duplicateButton.gameObject.SetActive(showDuplicateButton);
        deleteButton.gameObject.SetActive(showDeleteButton);

        cameraDisplay.gameObject.SetActive(showRawImage);
        takeShotButton.gameObject.SetActive(showTakeShotButton);

        exitButton.gameObject.SetActive(showExitButton);

        currentEditMode = newMode;
    }

    // Menu Toggles

    public void OnClickSpawn() {
        Teleportal.Inventory.Shared.UseCurrent();
    }

    public void OnClickMove() {
        if (isMovingObject) {
            // place object 
            setIsMoving(false);
        } else {
            // pickup object
            selectedItem = XRItemRaycaster.Shared.GetFocusedItem();
            selectedObjectEulerAngles = selectedItem.transform.eulerAngles;
            setIsMoving(true);
        }
    }

    public void OnClickDelete() {
        XRItem item = XRItemRaycaster.Shared.GetFocusedItem();

        if (item != null) {
            Teleportal.Ar.Shared.DeleteItem(item.GetId());
        }
    }

    public void OnClickDuplicate() {
        /* TODO fix TP refs
        waitingForDuplication = true;

        int lastSlot = Teleportal.Inventory.Shared.CurrentItem.id;
        int slot = 0;
        for (int i = 0; i < Teleportal.Inventory.Shared.InventoryArray.Length; i++) {
            if (XRItemRaycaster.Shared.GetFocusedItem().gameObject.name.Contains(Teleportal.Inventory.Shared.InventoryArray[i].type)) {
                slot = i;
                break;
            }
        }

        Teleportal.Inventory.Shared.SetItem(slot);
        Teleportal.Inventory.Shared.UseCurrent();
        Teleportal.Inventory.Shared.SetItem(lastSlot);
        // goes to OnDuplication
        */
    }

    public void OnClickTakeShot() {
        XRItem lookingAtItem = XRItemRaycaster.Shared.GetFocusedItem();

        if (lookingAtItem != null) {
            lookingAtItem.gameObject.GetComponent<ComposarCamera>().SaveImage();
        }
    }

    public void OnClickExit() {
        ComposarStateManager.Shared.SetMode(ComposarMode.SetupSequence);
    }

    public void OnDuplication(string id, XRItem newItem) {
        XRItem lookingAt = XRItemRaycaster.Shared.GetFocusedItem();

        if (lookingAt == null) {
            return;
        }
        
        Transform selected = lookingAt.gameObject.transform;
        Teleportal.Ar.Shared.MoveItem(id, selected.position.x, selected.position.y, selected.position.z, selected.eulerAngles.y, selected.eulerAngles.x);
        
        newItem.gameObject.transform.eulerAngles = new Vector3(selected.eulerAngles.x, selected.eulerAngles.y, selected.eulerAngles.z);
        newItem.gameObject.transform.localScale = new Vector3(selected.localScale.x, selected.localScale.y, selected.localScale.y);
        
        selectedItem = lookingAt;
        selectedObjectEulerAngles = selectedItem.transform.eulerAngles;
        setIsMoving(true);
    }

    private void setIsMoving(bool isMoving) {
        if (selectedItem == null) {
            isMoving = false;
            return;
        }

        isMovingObject = isMoving;
        
        float alpha = 1.0f;

        if (isMoving) {
            selectButtonText.text = "Unselect";
            // prevent floor from being added 
            if (!selectedItem.gameObject.name.Equals("Floor")) {
                if (snapGridToggle.isOn) {
                    actuallySelectedObject = new GameObject();
                    actuallySelectedObject.transform.position = selectedItem.gameObject.transform.position;
                    actuallySelectedObject.transform.SetParent(Teleportal.Ar.Shared.CurrentCamera.transform);
                } else {
                    selectedItem.ParentToUser(true);
                }
            }
            
            alpha = 0.5f;
        } else {
            selectButtonText.text = "Select";

             if (!snapGridToggle.isOn) {
                selectedItem.ParentToUser(false);
            } 
            
            selectedItem.gameObject.transform.SetParent(null);
            selectedItem.gameObject.transform.SetParent(floor.transform);
        }

        for (int i = 0; i < selectedItem.gameObject.transform.childCount; i++) {
            MeshRenderer renderer = selectedItem.gameObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>();
            if (renderer != null) {
                Color oldColor = renderer.material.color;
                renderer.material.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            }
        }
        
        if (!isMovingObject) {
            selectedItem = null;
        }

    }

    // Slider Changes

    public void ScaleValueChange() {
        if (selectedItem == null) {
            return;
        }

        if (ignoreThisChange) {
            ignoreThisChange = false;
            return;
        }

        Vector3 scale = selectedItem.gameObject.transform.localScale;

        Vector3 scaleVector = new Vector3(
                scaleDropdown.value == 0 || scaleDropdown.value == 3 ? scaleSlider.value : scale.x, 
                scaleDropdown.value == 1 || scaleDropdown.value == 3 ? scaleSlider.value : scale.y,
                scaleDropdown.value == 2 || scaleDropdown.value == 3 ? scaleSlider.value : scale.z);

        if (selectedItem.gameObject.name.Equals("Floor")) {
            floor.transform.localScale = scaleVector;
        } else {
            selectedItem.gameObject.transform.localScale = scaleVector;
        }
    }

    public void RotateValueChange() {
        if (selectedItem == null) {
            return; 
        }

        if (ignoreThisChange) {
            ignoreThisChange = false;
            return;
        }

        Vector3 angles = selectedItem.gameObject.transform.eulerAngles;

        float angleSnap = 30; // degrees 

        float snappedAngle = snapGridToggle.isOn ? (float) Math.Round(rotateSlider.value / angleSnap) * angleSnap : rotateSlider.value;

        Vector3 rotationVector = new Vector3(
                rotationDropdown.value == 0 ? snappedAngle : angles.x, 
                rotationDropdown.value == 1 ? snappedAngle : angles.y,
                rotationDropdown.value == 2 ? snappedAngle : angles.z);

        selectedObjectEulerAngles = rotationVector;
    }

    bool ignoreThisChange = false;
    public void DropdownValueChange() {
        if (selectedItem == null) {
            return;
        }

        float newValue;
                
        if (rotationDropdown.value == 0) {
            newValue = selectedObjectEulerAngles.x;
        } else if (rotationDropdown.value == 1) {
            newValue = selectedObjectEulerAngles.y;
        } else {
            newValue = selectedObjectEulerAngles.z;
        }

        ignoreThisChange = true;
        rotateSlider.value = newValue;
    }

    public void ScaleDropdownValueChange() {
        if (selectedItem == null) {
            return;
        }

        float newValue;
        
        Transform transform = selectedItem.gameObject.name.Equals("Floor") ? floor.transform : selectedItem.gameObject.transform;

        if (scaleDropdown.value == 0) {
            newValue = transform.localScale.x;
        } else if (scaleDropdown.value == 1) {
            newValue = transform.localScale.y;
        } else if (scaleDropdown.value == 2) {
            newValue = transform.localScale.z;
        } else {
            newValue = transform.localScale.x;
        }

        ignoreThisChange = true;
        scaleSlider.value = newValue;
    }

}
