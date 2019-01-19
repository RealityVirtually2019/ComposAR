using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComposarStateManager : MonoBehaviour {

	// singleton reference
	public static ComposarStateManager Shared;

	private List<Project> Projects;
	private Project CurrentProject;
	private string CurrentSceneName;

	void Awake () {
		ComposarStateManager.Shared = this;
	}

	void Start() {
		this.ChangeScene("Project");

		// TMP test
		StartCoroutine(DelayStartSceneIE());
	}

	// TMP
	private IEnumerator DelayStartSceneIE() {
		yield return new WaitForSeconds(2.0f);
		this.ChangeScene("SampleScene");
	}

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

	public void ChangeScene(string sceneName) {
		// Unload existing scene (if there is one)
		if (this.CurrentSceneName != null) {
			SceneManager.UnloadSceneAsync(this.CurrentSceneName);
		}
		
		// Load a new one
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
		this.CurrentSceneName = sceneName;
	}
	
}
