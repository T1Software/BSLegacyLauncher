Imports UnityEngine
Imports System.Net
Imports System.Text
Imports System.Diagnostics
Imports UnityEngine.UI

Public Class UpdateCheck
    Inherits MonoBehaviour

    Public UpdateCheckObject As GameObject
    Public UpdateButton As Button
    Public ErrorText As Text
    Public ErrorTextObject As GameObject

    Public Shared version As String

    Private incomingData As String = String.Empty
    Private lazyTag As String
    Private GitHub As String = "https://api.github.com/repos/RiskiVR/BSLegacyLauncher/releases"

    Private Sub Start()
        If Application.isEditor Then
            UpdateButton.interactable = False
        End If

        version = Application.version
        lazyTag = """tag_name"": ""v" & version & """"
        ' Get latest tag
        GetComponent(Of TextMesh)().text = "v" & version
        Dim web As New WebClient()
        web.Headers("Content-Type") = "application/json"
        web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0")
        web.Encoding = Encoding.UTF8
        incomingData = web.DownloadString(GitHub)

        If incomingData.Contains(lazyTag) Then
            UnityEngine.Debug.Log("Versions are matching")
        Else
            UnityEngine.Debug.Log("GitHub version is different than internal version.")
            UpdateCheckObject.SetActive(True)
        End If
    End Sub

    Private Sub DisplayErrorText(text As String)
        ' Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(False)
        ErrorTextObject.SetActive(True)
        ErrorText.text = text
    End Sub

    Public Sub RunUpdater()
        Try
            Process.Start("Resources\BSLLUpdater.exe")
        Catch
            DisplayErrorText("UNABLE TO RUN UPDATER")
        End Try
    End Sub
End Class
