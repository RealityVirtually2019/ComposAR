/// animaid @ MIT Reality Virtually Hacakthon 2019 ///
/// Thomas Suarez, Matt Kelsey, Ryan Reede, Sam Roquitte, Nick Grana ///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComposarHelper {

    public static string SerializeTransform(Transform trans) {
        string data = "";

		Vector3 pos = trans.position;
		Vector3 rot = trans.eulerAngles;
		Vector3 scale = trans.localScale;

		data += pos.x + "," + pos.y + "," + pos.z + "/";
		data += rot.x + "," + rot.y + "," + rot.z + "/";
		data += scale.x + "," + scale.y + "," + scale.z;

		return data;
    } 

}