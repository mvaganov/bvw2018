using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TransformValue {
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;

	public static implicit operator TransformValue(Transform t) {
		return new TransformValue (t);
	}

	public TransformValue(Transform t) {
		position = t.position;
		rotation = t.rotation;
		scale = t.lossyScale;
	}

	public void Set(Transform t) {
		t.position = position;
		t.rotation = rotation;
		Vector3 globalScale = (t.parent == null)?Vector3.one : t.parent.lossyScale;
		t.localScale = new Vector3(scale.x/globalScale.x, scale.y/globalScale.y, scale.z/globalScale.z);
	}

	public static Quaternion RollUpright(Transform transform, Vector3 upright) {
		Vector3 right = (transform.forward != upright && transform.forward != -upright)
			?Vector3.Cross (upright, transform.forward):transform.right;
		Vector3 cameraUp = Vector3.Cross (transform.forward, right);
		float dot = Vector3.Dot (cameraUp, transform.up);
		if (dot < 0) { return transform.rotation; }
		return Quaternion.LookRotation (transform.forward, cameraUp);
	}
}
