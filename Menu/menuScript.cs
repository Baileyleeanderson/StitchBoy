using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menuScript : MonoBehaviour {

	public AudioSource play;

	public void PlayGame (){
		SceneManager.LoadScene("Level1");
	}

	public void PlaySound(){
		play.Play();
	}

}
