using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class applyTexture : MonoBehaviour {

	[SerializeField]
	private int indexTexture = 0; // index of the texture to apply

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitUntil (() => resourcesManager.texturesLoaded == true);
		Renderer renderer = GetComponent<Renderer>();
		if(resourcesManager.textureArray.Count >=0 && indexTexture>=0 && indexTexture<resourcesManager.textureArray.Count )
			renderer.material.mainTexture = resourcesManager.textureArray[indexTexture];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
