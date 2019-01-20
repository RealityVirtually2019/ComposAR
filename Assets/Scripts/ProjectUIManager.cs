/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using UnityEngine;
using UnityEngine.UI;

public class ProjectUIManager : MonoBehaviour {
    public GameObject defaultPanel;
    public Button loadProjButton;
    public Button newProjButton;

    public GameObject newProjPanel;
    public InputField newProjNameInput;
    public Button cancelNewProjButton;
    public Button createAndLoadProjButton;

    public GameObject loadProjPanel;
    public Button cancelLoadProjButton;
    public Button loadSelectedProjButton;
    public Dropdown projDropdown;

    enum ProjectUIFsm
    {
        Default,
        NewProj,
        LoadProj,
    }
    ProjectUIFsm currentState = ProjectUIFsm.Default;

	// Use this for initialization
	void Start () {
        defaultPanel.SetActive(true);
        newProjPanel.SetActive(false);
        loadProjPanel.SetActive(false);

        //TODO tom
        //projDropdown.options = get projects from global app state

        loadProjButton.onClick.AddListener(() => LoadProjState());
        newProjButton.onClick.AddListener(() => NewProjState());

        cancelLoadProjButton.onClick.AddListener(() => Default());
        cancelNewProjButton.onClick.AddListener(() => Default());

        createAndLoadProjButton.onClick.AddListener(() => CreateAndLoad());
        loadSelectedProjButton.onClick.AddListener(() => LoadSelected());
    }

    public void LoadProjState(){
        print("clicked load ");
        ChangeState(ProjectUIFsm.LoadProj);
    }
	
    public void NewProjState(){
        print("clicked new");
        ChangeState(ProjectUIFsm.NewProj);
    }


    public void OpenSequenceScene(){
        //TODO tom
        // loadScene ("Sequence")
        print("\nOpening Seqeunce scene i hope");
    }

    public void LoadSelected(){
        string selected = projDropdown.options[projDropdown.value].text;
        print("\nLoading selected");

        print(selected);
        //TODO TOM
        //Composar.StateManager.Shared.LoadSequece(name)selected);
        OpenSequenceScene();
    }

    public void CreateAndLoad(){
        string n = newProjNameInput.text;
        if (n.Length > 0){
            print("\nMAKING proj");
            print(name);
            // TODO TOM 
            //ComposarStateManager.Shared.MakeSequence(name);
            //Composar.StateManager.Shared.LoadSequece(name);
        } else {
            print("proj name is empty");
            return;
        }
        OpenSequenceScene();
    }


    public void Default(){
        ChangeState(ProjectUIFsm.Default);
    }

    void ChangeState(ProjectUIFsm next){
        print("\nCHANGING STATE");
        print(next.ToString());
        switch (next)
        {
            case ProjectUIFsm.Default:
                defaultPanel.SetActive(true);
                newProjPanel.SetActive(false);
                loadProjPanel.SetActive(false);
                currentState = ProjectUIFsm.Default;
                break;

            case ProjectUIFsm.NewProj:
                defaultPanel.SetActive(false);
                newProjPanel.SetActive(true);
                loadProjPanel.SetActive(false);
                currentState = ProjectUIFsm.NewProj;
                break;

            case ProjectUIFsm.LoadProj:
                defaultPanel.SetActive(false);
                newProjPanel.SetActive(false);
                loadProjPanel.SetActive(true);
                currentState = ProjectUIFsm.LoadProj;
                break;

            default:
                break;
        }



    }

}

