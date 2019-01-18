/// ComposAR @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Model3DType {
    Character, Environment, DeepLearning
}

public class Model3D {

    private string id; // XR Item ID of this Shot
    private string filePath;
    private Model3DType type;

    public Model3D() : this("0", null, Model3DType.Environment) {}

    public Model3D(string id, string filePath, Model3DType type) {
        this.id = id;
        this.filePath = filePath;
        this.type = type;
    }
    
    public string GetId() {
        return this.id;
    }

    public void SetId(string id) {
        this.id = id;
    }

    public Model3DType GetType() {
        return this.type;
    }

    public void SetType(Model3DType type) {
        this.type = type;
    }

    public string GetFilePath() {
        return this.filePath;
    }
    
    public void OpenFilePath(string filePath) {
        // Set path
        this.filePath = filePath;

        // Open file (Unity resources directory?)
        // TODO
    }

}
