/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using UnityEngine;

public class AppStateManager : MonoBehaviour
{
    public static AppStateManager instance;

    public Project[] projects = { new Project("demo") };
    public Project activeProject;

    // is this dumb? im tirrd
    public enum appNavState { Project, Sequence, Layout, SketchAI, SketchDetail};
    public appNavState currentAppNavState = appNavState.Project;


    void Awake()
    {
        // If the instance reference has not been set, yet, 
        if (instance == null)
        {
            // Set this instance as the instance reference.
            instance = this;
        }
        else if (instance != this)
        {
            // If the instance reference has already been set, and this is not the
            // the instance reference, destroy this game object.
            Destroy(gameObject);
        }

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(gameObject);
        print(instance.projects[0]);
        instance.activeProject = projects[0];
    }

    public void SetActiveSequence(string sqName){
        instance.activeProject.SetCurrentSequence(sqName);
    }
}

  
