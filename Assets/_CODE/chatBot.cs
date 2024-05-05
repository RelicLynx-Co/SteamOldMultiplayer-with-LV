using Steamworks;
using Steamworks.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class chatBot : MonoBehaviour
{

	[SerializeField] private TMP_InputField _messageInputField;
	[SerializeField] private TextMeshProUGUI _messageTemplate;
	[SerializeField] private GameObject _messageContainer;

	private void Start()
	{
		_messageTemplate.text = "";
	}

	private void OnEnable()
	{
		SteamMatchmaking.OnChatMessage += ChatMessage;
		SteamMatchmaking.OnLobbyEntered += LobbyEntered;
		SteamMatchmaking.OnLobbyMemberJoined += LobbyMemberJoined;
		SteamMatchmaking.OnLobbyMemberLeave += LobbyMemberLeave;
	}

	private void OnDisable()
	{
		SteamMatchmaking.OnChatMessage -= ChatMessage;
		SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
		SteamMatchmaking.OnLobbyMemberJoined -= LobbyMemberJoined;
		SteamMatchmaking.OnLobbyMemberLeave -= LobbyMemberLeave;
	}

	private void LobbyMemberLeave( Lobby lobby, Friend friend )
	{
		AddMessage( "has left the lobby", friend, lobby.Owner.Id == friend.Id );
	}

	private void LobbyMemberJoined( Lobby lobby, Friend friend )
	{
		AddMessage( "has joined the lobby", friend, lobby.Owner.Id == friend.Id );
	}

	private void LobbyEntered( Lobby lobby )
	{
		AddMessage( "You have joined the lobby" );
	}

	private void ChatMessage( Lobby lobby, Friend friend, string msg )
	{
		AddMessage( msg, friend, lobby.Owner.Id == friend.Id );
	}

	private void AddMessage( string msg, Friend? friend = null, bool isOwner = true )
	{
		GameObject message = Instantiate( _messageTemplate.gameObject, _messageContainer.transform );

		string friendName = "";
		if ( friend != null )
		{
			friendName = $"<color=#{( isOwner ? "12FA12" : "FA1212" )}>{friend?.Name}</color>:";
		}


		message.GetComponent<TextMeshProUGUI>().text = $"[{DateTime.Now:HH:mm:ss}] {friendName}{msg}";
	}

	private void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Return ) )
		{
			ToggleChatBox();
		}
	}

	private void ToggleChatBox()
	{
		if ( _messageInputField.gameObject.activeSelf )
		{
			if ( !string.IsNullOrEmpty( _messageInputField.text ) )
			{
				LobbySaver.instance.CurrentLobby?.SendChatString( _messageInputField.text );
				_messageInputField.text = "";
			}
			_messageInputField.gameObject.SetActive( false );
			EventSystem.current.SetSelectedGameObject( null );

		}
		else
		{
			_messageInputField.gameObject.SetActive( true );
			EventSystem.current.SetSelectedGameObject( _messageInputField.gameObject );
		}
	}
}
