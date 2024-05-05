using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
	[SerializeField] private TMP_InputField _edt_lobbyId;
	[SerializeField] private TextMeshProUGUI _lbl_lobbyID;

	[SerializeField] private GameObject _mainMenu;
	[SerializeField] private GameObject _inLobbyMenu;

	private void OnEnable()
	{
		SteamMatchmaking.OnLobbyCreated += LobbyCreated;
		SteamMatchmaking.OnLobbyEntered += LobbyEntered;
		SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
	}

	private void OnDisable()
	{
		SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
		SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
		SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
	}

	private void LobbyEntered( Lobby lobby )
	{
		LobbySaver.instance.CurrentLobby = lobby;
		_lbl_lobbyID.text = lobby.Id.ToString();
		_edt_lobbyId.text = lobby.Id.ToString();
		if ( NetworkManager.Singleton.IsHost )
		{
			return;
		}
		NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = lobby.Owner.Id;
		NetworkManager.Singleton.StartClient();
		//SwapMenuIfLobbyIsNull();
	}

	private void SwitchGUI( string option )
	{
		switch ( option )
		{
			case "HostLobby":
				break;
			default:
				break;
		}
	}

	//private void SwapMenuIfLobbyIsNull()
	//{
	//	bool isSwap = LobbySaver.instance.CurrentLobby == null;
	//	_mainMenu.SetActive( isSwap );
	//	_inLobbyMenu.SetActive( !isSwap );
	//}

	private async void GameLobbyJoinRequested( Lobby lobby, SteamId steamId )
	{
		await lobby.Join();
	}

	private void LobbyCreated( Result result, Lobby lobby )
	{
		if ( result == Result.OK )
		{
			lobby.SetPublic();
			lobby.SetJoinable( true );
			NetworkManager.Singleton.StartHost();
		}
	}

	public async void HostLobby()
	{
		await SteamMatchmaking.CreateLobbyAsync( 4 );
		SwitchGUI( "HostLobby" );
	}

	public async void JoinLobbywithID()
	{
		ulong lobbyID = 0;
		if ( !ulong.TryParse( _edt_lobbyId.text, out lobbyID ) )
		{
			return;
		}

		Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable( 1 ).RequestAsync();

		foreach ( Lobby lobby in lobbies )
		{
			if ( lobby.Id == lobbyID )
			{
				await lobby.Join();
				return;
			}
		}
	}

	public void CopyID()
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = _edt_lobbyId.text;
		textEditor.SelectAll();
		textEditor.Copy();
	}

	public void LeaveLobby()
	{
		LobbySaver.instance.CurrentLobby?.Leave();
		LobbySaver.instance.CurrentLobby = null;
		NetworkManager.Singleton.Shutdown();

		//_edt_lobbyId.text = "";
		//SwapMenuIfLobbyIsNull();
	}

	public void StartGameServer()
	{

		if ( NetworkManager.Singleton.IsHost )
		{
			NetworkManager.Singleton.SceneManager.LoadScene( "GamePlay", UnityEngine.SceneManagement.LoadSceneMode.Single );
		}
	}
}