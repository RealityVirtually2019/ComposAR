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

    public void MakeSequence(string name){
        Sequence s = new Sequence(name);
        sequenceMap.Add(name, s);
        //return s;
    }

    public void LoadSequence(string name) {
        ComposarStateManager.Shared.SetMode(ComposarMode.Layout);
    }
     
    public List<Sequence> GetSequences(){
        return new List<Sequence>(sequenceMap.Values);
    }

    public void SetCurrentSequence(string name){
        currentSequence = sequenceMap[name];
    }
}