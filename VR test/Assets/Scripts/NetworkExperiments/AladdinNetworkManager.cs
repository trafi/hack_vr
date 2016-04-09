using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AladdinNetworkManager : NetworkManager {

	public NetworkDiscovery discovery;

	private enum State {
		DISCONNECTED,
		HOST,
		CLIENT
	}
	private State state;

	// Use this for initialization
	void Start () {
		state = State.DISCONNECTED;

		discovery.Initialize ();
		StartCoroutine (EstablishConnection ());
	}

	IEnumerator EstablishConnection() {
		Debug.Log ("Establishing connection: Searching for host");
		discovery.StartAsClient ();
		yield return new WaitForSeconds (5);
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

	// called when a new player is added for a client
	public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		var player = (GameObject)GameObject.Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
	}

	// called when a player is removed for a client
	public virtual void OnServerRemovePlayer(NetworkConnection conn, short playerControllerId)
	{
		PlayerController player;
		if (conn.GetPlayer(playerControllerId, out player))
		{
			if (player.NetworkIdentity != null && player.NetworkIdentity.gameObject != null)
				NetworkServer.Destroy(player.NetworkIdentity.gameObject);
		}
	}

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
