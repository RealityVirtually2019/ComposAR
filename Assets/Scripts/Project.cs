/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using UnityEngine;
using System.Collections.Generic;

public class Project {

    private string projectName;
    public Dictionary<string, Sequence> sequenceMap;
    private Sequence currentSequence;
    

    public Project(string n)
    {
        this.projectName = n;
        this.sequenceMap = new Dictionary<string, Sequence>();
    }

    public string GetName() {
        return this.projectName;
    }

    public void MakeSequence(string name){
        Sequence s = new Sequence(name);
        sequenceMap.Add(name, s);
        //return s;
    }

    public void LoadSequence(string name) {
        // load sequence setup
        ComposarStateManager.Shared.SetMode(ComposarMode.SetupSequence);
    }
     
    public List<Sequence> GetSequences(){
        return new List<Sequence>(sequenceMap.Values);
    }

    public void SetCurrentSequence(string name){
        currentSequence = sequenceMap[name];
    }
}