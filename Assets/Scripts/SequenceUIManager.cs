/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SequenceUIManager : MonoBehaviour {
    public Button backButton;
    public Text currProjText; 

    public GameObject defaultPanel; 
    public Dropdown sequenceDropdown;
    public Button layoutSelectedSeqButton;
    public Button newSeqPanelButton;
    public Button load3DModelsPanelButton;
    public Button sketchCharsButton;
    public Button sketchDeepButton;

    public GameObject addSeqPanel;
    public InputField newSeqNameInput;
    public Button addNewSeqButton;
    public Button cancelNewSeq;

    public GameObject modelsPanel;
    public Text currSeqText;
    public Text modelsListText;
    public Button cancelListModels;

    enum SequenceUIFsm
    {
        Default,
        NewSeq,
        ViewModels
    }
    SequenceUIFsm currentState = SequenceUIFsm.Default;

    // Use this for initialization
    void Start() {
        print("started sq ui mgr");
        defaultPanel.SetActive(true);
        addSeqPanel.SetActive(false);
        modelsPanel.SetActive(false);

        // Get curr proj name from state and set
        currProjText.text = ComposarStateManager.Shared.GetCurrentProject().GetName();

        backButton.onClick.AddListener(() => Back());
        layoutSelectedSeqButton.onClick.AddListener(() => Layout());

        cancelNewSeq.onClick.AddListener(() => Default());
        cancelListModels.onClick.AddListener(() => Default());

        newSeqPanelButton.onClick.AddListener(() => NewSequencePanel());
        load3DModelsPanelButton.onClick.AddListener(() => ListModelPanel());

        sketchCharsButton.onClick.AddListener(() => SketchChars());
        sketchDeepButton.onClick.AddListener(() => SketchModels());

        addNewSeqButton.onClick.AddListener(() => CreateSequence());
       
        //TODO tom
        //sequenceDropdown.options = getSequences from app state, REFRESH after we ADD a new seq
    }

    public void SketchChars()
    {
        ComposarStateManager.Shared.SetMode(ComposarMode.SketchChars);
    }

    public void SketchModels()
    {
        ComposarStateManager.Shared.SetMode(ComposarMode.SketchModels);
    }

    public string GenerateModelText(List<Model3D> models){
        string str = "";
        foreach (Model3D model in models) {
            str += model.GetName() + "\n";
        }
        return str;
    }

    public void ListModelPanel(){
        ChangeState(SequenceUIFsm.ViewModels);
        string selectedSeq = sequenceDropdown.options[sequenceDropdown.value].text;
        //TODO TOM:
        currSeqText.text = selectedSeq;

        // reset text
        modelsListText.text = "";

        // todo tom
        // Model[] models = getModels(sequence) from global state
        //modelsListText.text = GenerateModelText(models)
    }

    public void Back() {
        // Head back to Project
        ComposarStateManager.Shared.SetMode(ComposarMode.SetupProject);
    }

    public void Layout(){
        //TODO tom, set state for selected Sequence
        string selectedSeq = sequenceDropdown.options[sequenceDropdown.value].text;

        // load layout editor
        ComposarStateManager.Shared.SetMode(ComposarMode.Layout);
    }

    public void NewSequencePanel() {
        ChangeState(SequenceUIFsm.NewSeq);
    }

    public void CreateSequence(){
        string n = newSeqNameInput.text;
        if (n.Length > 0)
        {
            print("\nMAKING sq");
            print(name);
            // TODO TOM add new sq to global state
            //Composar.StateManager.Shared.MakeSequence(name);
            //Composar.StateManager.Shared.LoadSequece(name);
        }
        else
        {
            print("sq name is empty");
            return;
        }
        //TODO tom refresh this list now that there is A NEW SQ
        //sequenceDropdown.options = getSequences from app state, REFRESH after we ADD a new seq

    }

    public void Default(){
        ChangeState(SequenceUIFsm.Default);
    }

    void ChangeState(SequenceUIFsm next)
    {
        print("\nCHANGING STATE");
        print(next.ToString());
        switch (next)
        {
            case SequenceUIFsm.Default:
                defaultPanel.SetActive(true);
                addSeqPanel.SetActive(false);
                modelsPanel.SetActive(false);
                currentState = SequenceUIFsm.Default;
                break;

            case SequenceUIFsm.NewSeq:
                defaultPanel.SetActive(false);
                addSeqPanel.SetActive(true);
                modelsPanel.SetActive(false);
                currentState = SequenceUIFsm.NewSeq;
                break;

            case SequenceUIFsm.ViewModels:
                defaultPanel.SetActive(false);
                addSeqPanel.SetActive(false);
                modelsPanel.SetActive(true);
                currentState = SequenceUIFsm.ViewModels;
                break;             

            default:
                break;
        }

    }
}
