Imports Microsoft.Win32
Imports Newtonsoft.Json
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports UnityEngine
Imports UnityEngine.UI
Imports Yggdrasil.Logging

Public Class Steam
    Inherits MonoBehaviour

    Public Shared versions As New List(Of Version)()
    Public DownloadingText As TextMesh

    Private Sub Start()
        Dim versionList As String = File.ReadAllText(InstalledVersionToggle.BaseDirectory & "Resources/BSVersions.json")
        versions = JsonConvert.DeserializeObject(Of List(Of Version))(versionList)
    End Sub

    Public Sub SteamDownload()
        DownloadingText.text = $"Downloading {VersionVar.instance.version}..."

        Dim selectedVersion As Version = versions.First(Function(x) x.BSVersion.Equals(VersionVar.instance.version))

        Dim path = "C:\Program Files (x86)\Steam\steam.exe"
        Dim args = $"+download_depot 620980 620981 [{selectedVersion.BSManifest}]"

        Process.Start(path, args)
    End Sub
End Class
