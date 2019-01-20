/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    public bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Draw());
    }

    IEnumerator Draw()
    {
        while (active) {
            if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0))) {
                Plane objPlane = new Plane(Camera.main.transform.forward*-1, this.transform.position);

                Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                float rayDistance;
                if (objPlane.Raycast(mRay, out rayDistance)) {
                    this.transform.position = mRay.GetPoint(rayDistance);
                }
            } else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0)) {
                active = false;
            }
            yield return null;
        }

    }
}
