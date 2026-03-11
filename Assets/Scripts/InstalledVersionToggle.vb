Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports UnityEngine
Imports UnityEngine.UI

Public Class InstalledVersionToggle
    Inherits MonoBehaviour

    Public Shared installedVersions As Boolean = False
    Public Shared ReadOnly Property BSBaseDir As String
        Get
            Return BaseDirectory & "Installed Versions" & Path.DirectorySeparatorChar
        End Get
    End Property
    Public Shared BSDirectory As String = ""
    Public Shared ReadOnly Property CustomLevelsDirectory As String
        Get
            Return BaseDirectory & "CustomLevels"
        End Get
    End Property
    Public Shared ReadOnly Property CustomSongsDirectory As String
        Get
            Return BaseDirectory & "CustomSongs"
        End Get
    End Property
    Public Shared ReadOnly Property CustomWIPLevelsDirectory As String
        Get
            Return BaseDirectory & "CustomWIPLevels"
        End Get
    End Property
    Public Shared ReadOnly Property DLCDirectory As String
        Get
            Return BaseDirectory & "DLC"
        End Get
    End Property
    Public Shared BSVersion As String = "1.0.0"
    Public Shared BaseDirectory As String = ""

    Public Shared ReadOnly Property BSInstalledAndSelected As Boolean
        Get
            Return BSVersion <> ""
        End Get
    End Property

    Public TextEnter As RuntimeAnimatorController
    Public SelectVersionsObj As GameObject
    Public SelectVersions As Animator
    Public InstalledVersions As Animator
    Public installedVersionsObj As GameObject

    Public Sub Awake()
        SetBaseDir()
    End Sub

    Public Shared Sub SetBaseDir()
        BaseDirectory = If(Application.isEditor, Environment.CurrentDirectory, System.AppDomain.CurrentDomain.BaseDirectory)
        If Not BaseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()) Then BaseDirectory &= Path.DirectorySeparatorChar
    End Sub

    Public Shared Sub SetBSVersion(version As String, Optional updatedText As Boolean = False)
        SetBaseDir()
        File.WriteAllText(BSBaseDir & "BeatSaberVersion.txt", version)
        BSVersion = version
        BSDirectory = GetBSDirectory(version)
        If Not updatedText Then InstalledVer.UpdateText()
    End Sub

    Public Shared Sub CreateCustomSongsSymLink()
        If Directory.Exists(BSDirectory & "CustomSongs") Then DepotDownloaderObject.MoveDirectory(BSDirectory & "CustomSongs", CustomSongsDirectory)
    End Sub

    Public Shared Function GetBSDirectory(version As String) As String
        Return BSBaseDir & "Beat Saber " & version & Path.DirectorySeparatorChar
    End Function

    Public Shared Function GetInstalledVersions() As List(Of String)
        Dim installedVersions As New List(Of String)()
        If Not Directory.Exists(BSBaseDir) Then Return installedVersions
        For Each s As String In Directory.GetDirectories(BSBaseDir)
            installedVersions.Add(Path.GetFileName(s).Replace("Beat Saber ", ""))
        Next

        ' Optimized now
        LivConnector.AddAndUpdateAllRegistryEntries(installedVersions)
        Return installedVersions
    End Function

    Private showInstalledVersions As Boolean = True

    Public Sub UpdateList()
        Debug.Log("Updating list")
        showInstalledVersions = Directory.GetDirectories(BSBaseDir).Length > 0

        Toggle(Directory.GetDirectories(BSBaseDir).Length > 0)

        VersionButtonController.GetInstalledVersions()

        If Directory.GetDirectories(BSBaseDir).Length > 0 Then
            InstalledVersions.runtimeAnimatorController = TextEnter
        Else
            SelectVersions.runtimeAnimatorController = TextEnter
            SelectVersionsObj.SetActive(True)
        End If
    End Sub

    Public Sub Toggle(value As Boolean)
        Dim versions As List(Of String) = GetInstalledVersions()
        Debug.Log(versions.Count & " version downloaded")
        Dim i As Integer = 0
        Dim ii As Integer = 0
        installedVersions = value

        installedVersionsObj.SetActive(showInstalledVersions)

        gameObject.SetActive(value)
        Debug.Log("show downloaded versions: " & value)
        VersionButtonController.PublicYears.SetActive(Not value)
        VersionButtonController.PublicReleaseInfoButton.SetActive(False)
        VersionButtonController.PublicDownloadButton.SetActive(False)
        If Not value Then
            VersionButtonController.PublicVersions.GetComponent(Of RectTransform)().anchoredPosition = New Vector2(VersionButtonController.PublicVersions.GetComponent(Of RectTransform)().anchoredPosition.x, -8.5F)
            VersionButtonController.YearClicked(VersionButtonController.versionTable.Keys.OrderBy(Function(x) Convert.ToInt32(x)).ToList()(0), 0, VersionButtonController.versionTable.Keys.Count)
            Return
        End If
        VersionButtonController.PublicVersions.GetComponent(Of RectTransform)().anchoredPosition = New Vector2(VersionButtonController.PublicVersions.GetComponent(Of RectTransform)().anchoredPosition.x, 20.0F)
        VersionButtonController.ClearVersions()
        VersionButtonController.PublicVersions.SetActive(True)
        For Each version As String In versions
            If i >= 6 Then
                i = 0
                ii += 1
            End If
            If Not VersionButtonController.toDo.ContainsKey(ii) Then VersionButtonController.toDo.Add(ii, New List(Of Version)())
            VersionButtonController.toDo(ii).Add(New Version With {.year = "0", .BSVersion = version, .row = ii})
            i += 1
        Next
        VersionButtonController.minors = VersionButtonController.toDo.Keys.ToList()
    End Sub

    ' Update is called once per frame
    Private Sub Update()
    End Sub

End Class
