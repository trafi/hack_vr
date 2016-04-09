using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AladdinNetworkManager : NetworkManager {

	public NetworkDiscovery discovery;

	public GameObject godPlayerPrefab;
	public GameObject spectatorPlayerPrefab;

	private enum State {
		DISCONNECTED,
		HOST,
		CLIENT
	}
	private State state;

	public bool hostIsHuman = true;
	private int numConnectedPlayers = 0;

	// Use this for initialization
	void Start () {
		state = State.DISCONNECTED;

		discovery.Initialize ();
		StartCoroutine (EstablishConnection ());
	}

	IEnumerator EstablishConnection() {
		Debug.Log ("Establishing connection: Searching for host");
		discovery.StartAsClient ();
		yield return new WaitForSeconds (2);
		if (State.DISCONNECTED != state) {
			yield break;
		}
		Debug.Log ("Establishing connection: No host found, will become host");
		discovery.StopBroadcast ();
		StartHost ();
		discovery.StartAsServer ();
	}

	// called by AladdinNetworkDiscovery
	public void OnReceivedBroadcast(string fromAddress) {
		Debug.Log ("Establishing connection: Host found " + fromAddress);
		discovery.StopBroadcast ();
		networkAddress = fromAddress;
		StartClient();
	}

	public override void OnStartHost ()
	{
		Debug.Log ("OnStartHost");
		state = State.HOST;
	}

	public override void OnStartClient (NetworkClient client)
	{
		if (State.HOST == state) {
			return;
		}
		Debug.Log ("OnStartClient");
		state = State.CLIENT;
	}

	/*
	 * Customization available below.
	 * See http://docs.unity3d.com/Manual/UNetManager.html
	 * ---------------------------------------------------------------------------------------------------
	 */
	/*
	 * HOST / SERVER methods
	 */
	/*
	// called when a client connects 
	public virtual void OnServerConnect(NetworkConnection conn);

	// called when a client disconnects
	public virtual void OnServerDisconnect(NetworkConnection conn)
	{
		NetworkServer.DestroyPlayersForConnection(conn);
	}

	// called when a client is ready
	public virtual void OnServerReady(NetworkConnection conn)
	{
		NetworkServer.SetClientReady(conn);
	}
	*/
	// called when a new player is added for a client
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject player = null;
		if ((hostIsHuman && 0 == numConnectedPlayers) || (!hostIsHuman && 1 == numConnectedPlayers)) {
			player = GameObject.Instantiate (playerPrefab, playerPrefab.transform.position, Quaternion.identity) as GameObject;
			Debug.Log ("Spawning human player");
		} else if ((!hostIsHuman && 0 == numConnectedPlayers) || (hostIsHuman && 1 == numConnectedPlayers)) {
			player = GameObject.Instantiate (godPlayerPrefab, godPlayerPrefab.transform.position, Quaternion.identity) as GameObject;
			Debug.Log ("Spawning god player");
		} else {
			player = GameObject.Instantiate (spectatorPlayerPrefab, spectatorPlayerPrefab.transform.position, Quaternion.identity) as GameObject;
			// all other players become spectators
			Debug.Log ("Spawning spectator");
		}
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		++numConnectedPlayers;
	}

	// called when a player is removed for a client
	public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
	{
		base.OnServerRemovePlayer(conn, player);
		--numConnectedPlayers;
	}
	/*
	// called when a network error occurs
	public virtual void OnServerError(NetworkConnection conn, int errorCode);
	*/

	/*
	 * CLIENT methods
	 */
	/*
	// called when connected to a server
	public virtual void OnClientConnect(NetworkConnection conn)
	{
		ClientScene.Ready(conn);
		ClientScene.AddPlayer(0);
	}

	// called when disconnected from a server
	public virtual void OnClientDisconnect(NetworkConnection conn)
	{
		StopClient();
	}

	// called when a network error occurs
	public virtual void OnClientError(NetworkConnection conn, int errorCode);

	// called when told to be not-ready by a server
	public virtual void OnClientNotReady(NetworkConnection conn);
	*/
}
