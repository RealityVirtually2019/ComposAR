/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    
    public static LineController Shared;

    public GameObject line;

    void Awake() {
        LineController.Shared = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    enum ClickState { NoClick, StartClick, InClick, ReleaseClick };
    ClickState currentState = ClickState.NoClick;

    Line currentLine;

    List<Line> bigoli = new List<Line>();

    // Update is called once per frame
    void Update() {
        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0))) {
            // 614, 6
            // 190, 420
            // TODO: make dis work
            // var mousePos = Input.mousePosition;
            // if (mousePos.x > 614 || mousePos.x < 190 &&
            //     mousePos.y > 420 || mousePos.y < 6) {
            //         // Debug.Log("BB");
            //         if (currentState == ClickState.InClick) {
            //             currentState = ClickState.ReleaseClick;
            //             return;
            //         } else if (currentState == ClickState.StartClick) {
            //             return;
            //         }
            //     }
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
                    Line newLine = Instantiate(line, Input.mousePosition, Quaternion.identity, transform).GetComponent<Line>();
                    Debug.Log("Adding line " + bigoli.Count);
                    bigoli.Add(newLine);
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

    public void clearAll() {
        Debug.Log("BIGOLI " + bigoli.Count);
        foreach (Line l in bigoli) {
            Destroy(l.gameObject);
        }
        bigoli.Clear();
    }
}
