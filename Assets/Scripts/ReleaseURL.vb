Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports UnityEngine
Imports Yggdrasil.Logging

Public Class ReleaseURL
    Inherits MonoBehaviour

    Public Shared versions As New List(Of Version)()

    Private Sub Start()
        Dim versionList As String = File.ReadAllText(InstalledVersionToggle.BaseDirectory & "Resources/BSVersions.json")
        versions = JsonConvert.DeserializeObject(Of List(Of Version))(versionList)
    End Sub

    Public Sub OpenURL()
        Dim selectedVersion As Version = versions.First(Function(x) x.BSVersion.Equals(VersionVar.instance.version))
        Log.Info($"Opened Release info for {selectedVersion.BSVersion} : {selectedVersion.ReleaseURL}")
        Application.OpenURL($"{selectedVersion.ReleaseURL}")
    End Sub
End Class
