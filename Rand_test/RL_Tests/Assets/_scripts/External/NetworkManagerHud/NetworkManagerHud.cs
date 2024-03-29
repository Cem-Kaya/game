using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
[DisallowMultipleComponent]
public class NetworkManagerHud : MonoBehaviour
{
    NetworkManager m_NetworkManager;

    UnityTransport m_Transport;

    GUIStyle m_LabelTextStyle;

    // This is needed to make the port field more convenient. GUILayout.TextField is very limited and we want to be able to clear the field entirely so we can't cache this as ushort.
    string m_PortString = "26990";
	string m_ConnectAddress = "127.0.0.1";

	public string GetLocalIPAddress()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				string txt = ip.ToString();
				return txt;
			}
		}
		throw new System.Exception("No network adapters with an IPv4 address in the system!");
	}



	

	public Vector2 DrawOffset = new Vector2(10, 10);

    public Color LabelColor = Color.black;

    void Awake()
    {
        // Only cache networking manager but not transport here because transport could change anytime.
        m_NetworkManager = GetComponent<NetworkManager>();
        m_LabelTextStyle = new GUIStyle(GUIStyle.none);
    }

    void OnGUI()
    {
        m_LabelTextStyle.normal.textColor = LabelColor;

        m_Transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;

        GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(200, 200)));

        if (IsRunning(m_NetworkManager))
        {
            DrawStatusGUI();
        }
        else
        {
            DrawConnectGUI();
        }

        GUILayout.EndArea();
    }

    private void Start()
    {
        m_ConnectAddress = GetLocalIPAddress();

	}
    void DrawConnectGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Address", m_LabelTextStyle);
        GUILayout.Label("Port", m_LabelTextStyle);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        m_ConnectAddress = GUILayout.TextField(m_ConnectAddress);
        m_PortString = GUILayout.TextField(m_PortString);
        if (ushort.TryParse(m_PortString, out ushort port))
        {
            //m_Transport.SetConnectionData(m_ConnectAddress, port);
        }
        else
        {
            //m_Transport.SetConnectionData(m_ConnectAddress, 7777);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Host (Server + Client)"))
        {
            //m_NetworkManager.Start();
            m_NetworkManager.StartHost();
            //NetworkManager.Singleton.SceneManager.LoadScene("Main_scene no_network", LoadSceneMode.Single);

        }

        GUILayout.BeginHorizontal();
                
        if (GUILayout.Button("Client"))
        {            
            m_NetworkManager.StartClient();
        }

        GUILayout.EndHorizontal();
    }

    void DrawStatusGUI()
    {
        if (m_NetworkManager.IsServer)
        {
            var mode = m_NetworkManager.IsHost ? "Host" : "Server";
            GUILayout.Label($"{mode} active on port: {m_Transport.ConnectionData.Port.ToString()}", m_LabelTextStyle);
            GUILayout.Label($"conection ip   {m_Transport.ConnectionData.Address}", m_LabelTextStyle);
        }
        else
        {
            if (m_NetworkManager.IsConnectedClient)
            {
                GUILayout.Label($"Client connected port {m_Transport.ConnectionData.Port.ToString()}", m_LabelTextStyle);
                GUILayout.Label($"Client connected ip   {m_Transport.ConnectionData.Address} ", m_LabelTextStyle);
            }
        }

        if (GUILayout.Button("Shutdown"))
        {
            m_NetworkManager.Shutdown();
        }
    }

    // from now on my additions until the methodImpl curses
    public void start_game_as_host()
    {
        m_NetworkManager.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Start_room", LoadSceneMode.Single);
    }

    public void start_game_as_client()
    {
        m_NetworkManager.StartClient();
        //NetworkManager.Singleton.SceneManager.LoadScene("Main_scene no_network", LoadSceneMode.Single);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsRunning(NetworkManager networkManager) => networkManager.IsServer || networkManager.IsClient;
}