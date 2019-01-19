/// ComposAR @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Model3DType {
    Character, Environment, DeepLearning
}

public class Model3D {

    public string id;
    public string filePath;
    public Model3DType type;

    public Model3D(string id, string filePath, Model3DType type) {
        this.id = id;
        this.filePath = filePath;
        this.type = type;
    }

}
