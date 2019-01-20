using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{

    public GameObject line;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    enum ClickState { NoClick, StartClick, InClick, ReleaseClick};
    ClickState currentState = ClickState.NoClick;

    Line currentLine;

    // Update is called once per frame
    void Update() {
        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0))) {
            if (currentState == ClickState.StartClick) {
                currentState = ClickState.InClick;
            }
            if (currentState == ClickState.NoClick) {
                currentState = ClickState.StartClick;
            }

            Plane objPlane = new Plane(Camera.main.transform.forward*-1, this.transform.position);

            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;

            if (currentState == ClickState.StartClick) {
                if (objPlane.Raycast(mRay, out rayDistance)) {
                    Debug.Log("Drawing new line");
                    Line newLine = Instantiate(line, Input.mousePosition, Quaternion.identity, transform).GetComponent<Line>();
                    if (currentLine != null) {
                        currentLine.active = false;
                    }
                    currentLine = newLine;
                    newLine.transform.position = mRay.GetPoint(rayDistance);
                    newLine.active = true;
                    return;
                }
            }
        } else {
            if (currentState == ClickState.InClick) {
                currentState = ClickState.ReleaseClick;
            } else {
                currentState = ClickState.NoClick;
            }
        }
    }
}
