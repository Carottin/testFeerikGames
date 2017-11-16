using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Threading;

public class resourcesManager : MonoBehaviour {

	// Images on internet
	string url1 = "https://www.w3schools.com/howto/img_fjords.jpg";
	string url2 = "https://www.smashingmagazine.com/wp-content/uploads/2015/06/10-dithering-opt.jpg";

	[SerializeField]
	private int maxThread = 3; // maximum number of threads

	int nbThreadActive = 0; // number of active threads

	List<string> urlArray;

	public static List<Texture2D> textureArray; 
	public Texture2D texture;
	public static bool texturesLoaded = false; // check if all textures are loaded
	public string filePath;	
	
	IEnumerator Start()
	{
		urlArray = new List<string>();
		textureArray = new List<Texture2D> ();
		urlArray.Add (url1);
		urlArray.Add (url2);

		// for all images on server
		for (int i = 0; i < urlArray.Count; i++) {
			WWW www = new WWW(urlArray[i]);
			yield return www;

			// Check if the number of active threads is inferior to the maximum number of threads
			yield return new WaitUntil (() => nbThreadActive != maxThread);

			filePath = Application.persistentDataPath + "/Textures/" + Path.GetFileName (urlArray [i]);

			texture = new Texture2D(4, 4); // Create a new texture

			object[] parms = new object[2]{filePath, texture};

			// If file is not on the disk
			if (!File.Exists (filePath)) {
				File.WriteAllBytes (filePath, www.bytes); // Creation of a new image file
				StartCoroutine ("LoadTexture", parms); // start thread to load texture
				nbThreadActive++; // increment the number of active thread
			} else { // File is on the disk
				Debug.Log ("File exist");
				StartCoroutine ("LoadTexture", parms);// start thread to load texture
				nbThreadActive++; // increment the number of active thread
			}

		}
		// All texture are loaded
		texturesLoaded = true;
	}

	// Update is called once per frame
	void Update () {

	}
	void applyTexture(){
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = textureArray[0];
	}

	IEnumerator LoadTexture (object[] parms){
		// parms[0] : path of the image
		// parms[0] : the texture to load

		byte[] imageData = File.ReadAllBytes((string)parms[0]); // read the image
		((Texture2D)parms[1]).LoadImage(imageData); // load image into the texture

		textureArray.Add ((Texture2D)parms[1]); // add the image to the textures array
		nbThreadActive--; // decrement the number of thread

		yield break;
	}		
}
