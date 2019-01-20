/// ComposAR @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Model3DType {
    Character, Environment, DeepLearning
}

public class Model3D {

    private string name;
    private XRItem XRI;
    private string filePath;
    private Model3DType type;

    public Model3D() : this("", null, null, Model3DType.Environment) {}

    public Model3D(string name, XRItem item, string filePath, Model3DType type) {
        this.name = name;
        this.XRI = item;
        this.filePath = filePath;
        this.type = type;

        // TODO spawn object if not null
    }

    public string GetName() {
        return this.name;
    }

    public void SetName(string name) {
        this.name = name;
    }

    public XRItem GetXRItem() {
        return this.XRI;
    }

    public void SetXRItem(XRItem item) {
        this.XRI = item;
    }

    public string GetId() {
        return this.XRI.Id;
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
