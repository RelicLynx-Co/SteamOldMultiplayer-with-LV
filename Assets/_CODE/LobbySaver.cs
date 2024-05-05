using Steamworks.Data;
using UnityEngine;

public class LobbySaver : MonoBehaviour
{
	public Lobby? CurrentLobby { get; set; }
	public static LobbySaver instance;

	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(this.gameObject);
	}
}