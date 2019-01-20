using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private enum State { None, Front, Side, Done };
    private State currentState = State.None;

    // Start is called before the first frame update
    void Start() {
        
    }

    public void Ye() {
        Debug.Log(currentState);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
