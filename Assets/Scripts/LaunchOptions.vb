Imports Newtonsoft.Json
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class LaunchOptions
    Inherits MonoBehaviour

    Public oculusToggle As Toggle
    Public FPFCToggle As Toggle
    Public VerboseToggle As Toggle

    Public Shared vars As New LaunchOptionsVars()
    Private Const jsonLocation As String = "Beat Saber Legacy Launcher_Data/Settings/launchoptions.json"

    Public Shared Sub LoadSettings()
        If Not File.Exists(InstalledVersionToggle.BaseDirectory & jsonLocation) Then Save()
        Try
            vars = JsonConvert.DeserializeObject(Of LaunchOptionsVars)(File.ReadAllText(InstalledVersionToggle.BaseDirectory & jsonLocation))
        Catch
            Save()
        End Try
    End Sub

    ' Start is called before the first frame update
    Private Sub Start()
        LoadSettings()
        oculusToggle.isOn = vars.oculus
        oculusToggle.onValueChanged.AddListener(Sub(value)
            vars.oculus = value
            Save()
        End Sub)
        FPFCToggle.isOn = vars.fpfc
        FPFCToggle.onValueChanged.AddListener(Sub(value)
            vars.fpfc = value
            Save()
        End Sub)
        VerboseToggle.isOn = vars.verbose
        VerboseToggle.onValueChanged.AddListener(Sub(value)
            vars.verbose = value
            Save()
        End Sub)
    End Sub

    Public Shared Sub Save()
        If Not Directory.Exists(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory & jsonLocation)) Then Directory.CreateDirectory(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory & jsonLocation))
        File.WriteAllText(InstalledVersionToggle.BaseDirectory & jsonLocation, JsonConvert.SerializeObject(vars))
    End Sub

    ' Update is called once per frame
    Private Sub Update()

    End Sub
End Class

Public Class LaunchOptionsVars
    Public Property oculus As Boolean = False
    Public Property fpfc As Boolean = False
    Public Property verbose As Boolean = False
End Class
