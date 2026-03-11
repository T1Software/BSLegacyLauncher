Imports Newtonsoft.Json
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports UnityEngine
Imports System.Linq
Imports DepotDownloader
Imports SteamKit2
Imports UnityEngine.UI

Public Class VersionVar
    Inherits MonoBehaviour

    Public Shared instance As VersionVar

    Public VersionText As TextMesh

    Public DiscordController As DiscordController

    <HideInInspector>
    Public version As String

    Public Sub ListVersion(Version As String)
        Me.version = Version
        VersionText.text = Version
        VersionText.gameObject.SetActive(False)
        VersionText.gameObject.SetActive(True)
        DiscordController.BSVersion = $"{Version}"
        DiscordController.SelectVersion()
    End Sub

    Private Sub Start()
        instance = Me
    End Sub
End Class
