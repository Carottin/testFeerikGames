using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class applyTexture : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitUntil (() => resourcesHandler.texturesLoaded == true);
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = ressourcesHandler.textureArray[1];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
