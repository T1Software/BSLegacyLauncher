Imports ComputerUtils.CommandLine
Imports Newtonsoft.Json
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class Settings
    Inherits MonoBehaviour

    Public Pixel As GameObject
    Public PixelModpackButton As GameObject
    Public ambientToggle As Toggle

    Public Shared vars As New Setting()
    Private Const settingsLocation As String = "Beat Saber Legacy Launcher_Data/Settings/settings.json"

    ' Start is called before the first frame update
    Private Sub Start()
        Dim commands As New CommandLineCommandContainer(Environment.GetCommandLineArgs())

        If commands.HasArgument("--Pixel") Then ' ignores case
            Pixel.SetActive(True)
            PixelModpackButton.SetActive(True)
        End If

        If Not File.Exists(InstalledVersionToggle.BaseDirectory & settingsLocation) Then Save()
        Try
            vars = JsonConvert.DeserializeObject(Of Setting)(File.ReadAllText(InstalledVersionToggle.BaseDirectory & settingsLocation))
        Catch
            Save()
        End Try
        ambientToggle.isOn = vars.ambient
        ambientToggle.onValueChanged.AddListener(Sub(value)
            vars.ambient = value
            Save()
        End Sub)
    End Sub

    Public Sub Save()
        If Not Directory.Exists(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory & settingsLocation)) Then
            Directory.CreateDirectory(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory & settingsLocation))
        End If
        File.WriteAllText(InstalledVersionToggle.BaseDirectory & settingsLocation, JsonConvert.SerializeObject(vars))
    End Sub
End Class

Public Class Setting
    Public Property ambient As Boolean = True
End Class
