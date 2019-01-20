/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence {

	public string name;
	public int timestamp;
	private List<Shot> shots;
    private List<Model3D> models;

	public Sequence() : this("New Sequence") {}

	public Sequence(string name) : this(name, 0) {}

	public Sequence(string name, int timestamp) {
		this.name = name;
		this.timestamp = timestamp;
		this.shots = new List<Shot>();
		this.models = new List<Model3D>();
	}

	public void ChangeName(string name) {
		this.name = name;
	}

	public void ChangeTimestamp(int timestamp)  {
		this.timestamp = timestamp;
	}

	public List<Shot> GetShots() {
		return this.shots;
	}

	public void AddShot(Shot shot) {
		this.shots.Add(shot);
	}

	public void RemoveShot(Shot shot) {
		this.shots.Remove(shot);
	}
	
}
