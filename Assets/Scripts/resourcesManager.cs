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

	[SerializeField]
	private int maxCoroutine = 3; // maximum number of coroutine
	int nbCoroutineActive = 0; // number of active coroutine

	List<string> urlArray;

	List<byte[]> imagesData;

	public static List<Texture2D> textureArray; 
	Texture2D texture;
	public static bool texturesLoaded = false; // check if all textures are loaded
	string filePath;	
	
	IEnumerator Start()
	{
		urlArray = new List<string>(); // images' url list
		imagesData = new List<byte[]>(); // images list
		textureArray = new List<Texture2D> (); // texture list
		urlArray.Add (url1);
		urlArray.Add (url2);

		// for all images on server
		for (int i = 0; i < urlArray.Count; i++) {
			WWW www = new WWW(urlArray[i]);
			yield return www;

			// Check if the number of active threads is inferior to the maximum number of threads
			yield return new WaitUntil (() => nbThreadActive != maxThread);

			filePath = Application.persistentDataPath + "/Textures/" + Path.GetFileName (urlArray [i]);

			byte[] data = www.bytes;

			// If file is not on the disk
			if (!File.Exists (filePath)) {
				Thread thread = new Thread(() => writeOnDisk(filePath, data)); 
				thread.Start(); // start the thread to write on the disk from server
				nbThreadActive++; // increment the number of active threads
			} else { // File is on the disk
				Thread thread = new Thread(() => readOnDisk(filePath));
				thread.Start(); // start the thread to read the images from the disk
				nbThreadActive++; // increment the number of active threads
			}
		}

		for (int i = 0; i < imagesData.Count; i++) {
			yield return new WaitUntil (() => nbCoroutineActive != maxCoroutine);
			texture = new Texture2D(4, 4); // Create a new texture
			object[] parms = new object[2]{i, texture};
			StartCoroutine ("LoadTexture", parms);
			nbCoroutineActive++;
		}
		// All texture are loaded
		texturesLoaded = true;
	}

	// Update is called once per frame
	void Update () {

	}
	void writeOnDisk(string filePath, byte[]data){
		Debug.Log ("zer");
		File.WriteAllBytes (filePath, data); // write the images on the disk
		readOnDisk (filePath); // start the thread to read the images from the disk
	}

	void readOnDisk(string filePath){
		imagesData.Add (File.ReadAllBytes (filePath)); // read the images file and add it to the images list
		nbThreadActive--;
	}

	void applyTexture(){
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = textureArray[0];
	}

	IEnumerator LoadTexture (object[] parms){
		((Texture2D)parms[1]).LoadImage(imagesData[(int)parms[0]]); // load image in the texture
		textureArray.Add ((Texture2D)parms[1]); // add the image to the textures array
		nbCoroutineActive--; // decrement the number of coroutine

		yield break;
	}		
}
