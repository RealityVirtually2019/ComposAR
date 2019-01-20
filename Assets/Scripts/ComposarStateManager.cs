using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ComposarMode {
	SetupProject, SetupSequence, Layout, SketchChars, SketchModels
}

public class ComposarStateManager : MonoBehaviour {

	// singleton reference
	public static ComposarStateManager Shared;

	// assets
	public Material DrawPlaneMaterial;

	// instance objects
	private List<Project> Projects;
	private Project CurrentProject;
	private string CurrentSceneName;
	private ComposarMode CurrentMode;
	private RenderTexture CurrentScrenshot;

	/* Lifecycle */

	void Awake () {
		ComposarStateManager.Shared = this;
		this.Projects = new List<Project>();
	}

	void Start() {
		// Default to Project scene
		this.ChangeScene("Project");

		// TMP test
		// StartCoroutine(DelayStartSceneIE());
	}

	// TMP
	private IEnumerator DelayStartSceneIE() {
		yield return new WaitForSeconds(2.0f);
		this.ChangeScene("Layout");
	}

	/* Projects */

	public void AddProject(Project project) {
		this.Projects.Add(project);
	}

	public void RemoveProject(Project project) {
		this.Projects.Remove(project);
	}

	public List<Project> GetProjects() {
		return this.Projects;
	}

	public Project GetCurrentProject() {
		return this.CurrentProject;
	}

	public void SetCurrentProject(Project project) {
		this.CurrentProject = project;
	}

	public RenderTexture GetCurrentScreenshot() {
		return this.CurrentScrenshot;
	}

	public void SetCurrentScreenshot(RenderTexture screenshot) {
		this.CurrentScrenshot = screenshot;
		DrawPlaneMaterial.SetTexture("_MainTex", this.CurrentScrenshot);
	}

	/* Scenes */
	protected void ChangeScene(string newSceneName) {
		if (this.CurrentSceneName == newSceneName) {
			print("Trying to change scene to self ; already exists! Skipping...");
			return;
		}

		// Load a new scene
		SceneManager.LoadScene(newSceneName, LoadSceneMode.Additive);

		// Unload existing scene (if there is one)
		if (this.CurrentSceneName != null && newSceneName != "Paint") {
			print("Unloading " + this.CurrentSceneName);
			SceneManager.UnloadSceneAsync(this.CurrentSceneName);
			
			// Handle Teleportal AR
			if (this.CurrentSceneName == "Layout") {
				SceneManager.UnloadSceneAsync("Teleportal");
			}
		}
		
		// Set new scene name
		this.CurrentSceneName = newSceneName;
	}
	
	public ComposarMode GetMode() {
		return this.CurrentMode;
	}

	public void SetMode(ComposarMode mode) {
		switch (mode) {
			case ComposarMode.SetupProject:
				this.ChangeScene("Project");
				break;
			case ComposarMode.SetupSequence:
				this.ChangeScene("Sequence");
				break;
			case ComposarMode.Layout:
				this.ChangeScene("Layout");
				break;
			case ComposarMode.SketchChars:
				this.ChangeScene("Paint");
				break;
			case ComposarMode.SketchModels:
				this.ChangeScene("Paint");
				break;
		}
	}

}
