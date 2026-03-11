Imports UnityEngine

<System.Serializable>
Public Class DiscordJoinEvent
    Inherits UnityEngine.Events.UnityEvent(Of String)
End Class

<System.Serializable>
Public Class DiscordSpectateEvent
    Inherits UnityEngine.Events.UnityEvent(Of String)
End Class

<System.Serializable>
Public Class DiscordJoinRequestEvent
    Inherits UnityEngine.Events.UnityEvent(Of DiscordRpc.DiscordUser)
End Class

Public Class DiscordController
    Inherits MonoBehaviour

    Public presence As New DiscordRpc.RichPresence()
    Public applicationId As String
    Public optionalSteamId As String
    Public onConnect As UnityEngine.Events.UnityEvent
    Public onDisconnect As UnityEngine.Events.UnityEvent
    Public hasResponded As UnityEngine.Events.UnityEvent

    Private handlers As DiscordRpc.EventHandlers

    Public BSVersion As String
    Public Installed As String
    Public DownloadProgress As String

    Private Shared Timestamp As Long = 0

    Public Sub ReadyCallback(ByRef connectedUser As DiscordRpc.DiscordUser)
        Debug.Log(String.Format("Discord: connected to {0}#{1}: {2}", connectedUser.username, connectedUser.discriminator, connectedUser.userId))
        onConnect.Invoke()
    End Sub

    Public Sub DisconnectedCallback(errorCode As Integer, message As String)
        Debug.Log(String.Format("Discord: disconnect {0}: {1}", errorCode, message))
        onDisconnect.Invoke()
    End Sub

    Public Sub ErrorCallback(errorCode As Integer, message As String)
        Debug.Log(String.Format("Discord: error {0}: {1}", errorCode, message))
    End Sub

    Public Sub SelectVersion()
        presence.details = "Selecting a version"
        presence.state = $"Beat Saber {BSVersion}"
        DiscordRpc.UpdatePresence(presence)
    End Sub

    Public Sub DownloadUpdate()
        presence.details = $"{DownloadProgress}"
        presence.state = $"Beat Saber {BSVersion}"
        DiscordRpc.UpdatePresence(presence)
    End Sub

    Public Sub VersionStart()
        presence.details = $"Currently Selected: {InstalledVersionToggle.BSVersion}"
        presence.state = ""
        presence.largeImageKey = "block"
        DiscordRpc.UpdatePresence(presence)
    End Sub

    Public Sub Uninstall()
        Installed = "No version installed"
        VersionStart()
    End Sub

    Private Sub Start()
        presence.largeImageKey = "block"
        '  presence.startTimestamp = 197011000 this shit doesn't work
        DiscordRpc.UpdatePresence(presence)
    End Sub

    Private Sub Update()
        DiscordRpc.RunCallbacks()
    End Sub

    Private Sub OnEnable()
        Debug.Log("Discord: init")
        handlers = New DiscordRpc.EventHandlers()
        handlers.readyCallback = AddressOf ReadyCallback
        handlers.disconnectedCallback = AddressOf DisconnectedCallback
        handlers.errorCallback = AddressOf ErrorCallback
        DiscordRpc.Initialize(applicationId, handlers, True, optionalSteamId)
    End Sub

    Private Sub OnDisable()
        Debug.Log("Discord: shutdown")
        DiscordRpc.Shutdown()
    End Sub

    Private Sub OnDestroy()

    End Sub
End Class
