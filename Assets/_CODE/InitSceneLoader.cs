using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneLoader : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI _tmp;
	private void Start()
	{
		StartCoroutine( LoadMainScene() );
	}

	IEnumerator LoadMainScene()
	{
		_tmp.text += ".";
		Debug.Log( $" ST » Setup Scene waiting for NetworkManager.Singleton", this );
		yield return new WaitUntil( () => NetworkManager.Singleton != null );
		_tmp.text += ".";
		Debug.Log( $" ED » Setup Scene waiting for NetworkManager.Singleton", this );
		yield return new WaitForSeconds( 1 );
		_tmp.text += ".";
		SceneManager.LoadScene( "MainMenu", LoadSceneMode.Single );
	}
}