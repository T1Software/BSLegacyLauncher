Imports Newtonsoft.Json
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Net
Imports UnityEngine
Imports UnityEngine.UI

Public Class VersionButtonController
    Inherits MonoBehaviour

    Public Shared versions As New List(Of Version)()
    Public Shared versionTable As New Dictionary(Of String, Dictionary(Of Integer, List(Of Version)))()
    Public Shared toDo As New Dictionary(Of Integer, List(Of Version))()
    Private lastButtonSpawn As Single = 0.0F

    <Header("Prefabs")>
    Public VersionButtonPrefab As GameObject
    Public Shared PublicYearButtonPrefab As GameObject
    Public YearButtonPrefab As GameObject

    <Header("Patreons")>
    Public PatreonL As GameObject
    Public PatreonR As GameObject

    <Header("Other stuff")>
    Public _ClickSound As GameObject
    Public Shared _PublicClickSound As GameObject
    Public Plane As GameObject
    Public Shared PublicPlane As GameObject = Nothing
    Public Versions As GameObject
    Public Shared PublicVersions As GameObject = Nothing
    Public Years As GameObject
    Public Shared PublicYears As GameObject = Nothing
    Public Bar As GameObject
    Public Shared PublicBar As GameObject = Nothing

    Public DownloadButton As GameObject
    Public Shared PublicDownloadButton As GameObject
    Public VersionText2 As GameObject
    Public ReleaseInfoButton As GameObject
    Public Shared PublicReleaseInfoButton As GameObject

    Public Shared yearsAdded As Boolean = False
    Public Shared currentRow As Integer
    Public Shared currentColumn As Integer

    ' Just ignore that variable name. I scambled the sorting Riski made and didn't bother to rename the vars
    Public Shared minors As New List(Of Integer)()

    Private Shared installedVersions As New List(Of String)()

    ' Start is called before the first frame update
    Private Sub Start()
        installedVersions = InstalledVersionToggle.GetInstalledVersions()

        PublicVersions = Versions
        PublicYears = Years
        PublicBar = Bar
        PublicPlane = Plane
        PublicYearButtonPrefab = YearButtonPrefab
        _PublicClickSound = _ClickSound
        PublicDownloadButton = DownloadButton
        PublicReleaseInfoButton = ReleaseInfoButton

        ' Don't forget to add back

        Try
            ' Update cache
            Dim c As New WebClient()
            Dim d As String = c.DownloadString("https://raw.githubusercontent.com/RiskiVR/BSLegacyLauncher/master/Resources/BSVersions.json")
            File.WriteAllText(InstalledVersionToggle.BaseDirectory & "Resources/BSVersions.json", d)
            d = c.DownloadString("https://raw.githubusercontent.com/RiskiVR/BSLegacyLauncher/master/Resources/Patreons.json")
            File.WriteAllText(InstalledVersionToggle.BaseDirectory & "Resources/Patreons.json", d)
        Catch
        End Try

        Dim versionList As String = File.ReadAllText(InstalledVersionToggle.BaseDirectory & "Resources/BSVersions.json")
        versions = JsonConvert.DeserializeObject(Of List(Of Version))(versionList)

        ' Aperture
        If ComputersVars.useApertureDeskJob Then versions.Add(New Version With {.BSManifest = "152863936826529847", .BSVersion = "Aperture", .year = "2022", .row = 3})

        Dim patreonsList As String = File.ReadAllText(InstalledVersionToggle.BaseDirectory & "Resources/Patreons.json")
        Dim patreons As List(Of Patreon) = JsonConvert.DeserializeObject(Of List(Of Patreon))(patreonsList)
        ' Order by alphabet
        patreons = patreons.OrderBy(Function(x) x.name).ToList()
        PatreonL.GetComponent(Of Text)().text = ""
        PatreonR.GetComponent(Of Text)().text = ""
        For i As Integer = 0 To patreons.Count - 1
            If(i < patreons.Count \ 2, PatreonL, PatreonR).GetComponent(Of Text)().text &= patreons(i).name & vbLf
        Next
        GenerateDict()
    End Sub

    Public Shared Sub GetInstalledVersions()
        installedVersions = InstalledVersionToggle.GetInstalledVersions()
    End Sub

    Public Shared Sub ClearVersions()
        currentColumn = 0
        currentRow = 0
        toDo = New Dictionary(Of Integer, List(Of Version))()
        For Each t As Transform In PublicVersions.transform
            Destroy(t.gameObject)
        Next
    End Sub

    Public Shared Sub GenerateDict()
        versionTable.Clear()
        Dim yearTable As New Dictionary(Of String, List(Of Version))()
        ' Group by year
        For Each v As Version In versions
            If Not yearTable.ContainsKey(v.year) Then
                yearTable.Add(v.year, New List(Of Version)())
            End If
            yearTable(v.year).Add(v)
        Next
        ' Group by minor version
        For Each year As KeyValuePair(Of String, List(Of Version)) In yearTable
            Dim minorVersions As New Dictionary(Of Integer, List(Of Version))()
            For Each version As Version In year.Value
                If Not minorVersions.ContainsKey(version.row) Then
                    minorVersions.Add(version.row, New List(Of Version)())
                End If
                minorVersions(version.row).Add(version)
            Next
            versionTable.Add(year.Key, minorVersions)
        Next
        versionTable = versionTable.OrderBy(Function(x) Convert.ToInt32(x.Key)).ToDictionary(Function(x) x.Key, Function(x) x.Value)
        If Not yearsAdded Then
            AddYearButtons()
            yearsAdded = True
        End If
    End Sub

    Public Shared Sub AddYearButtons()
        Dim yearCount As Integer = versionTable.Keys.Count
        Const yearButtonWidth As Integer = 87
        Dim totalWidth As Integer = yearCount * yearButtonWidth
        Dim left As Integer = 0 - totalWidth \ 2
        Dim i As Integer = -1
        For Each year As String In versionTable.Keys
            Dim ii As Integer = i + 1
            Dim yearButton As GameObject = Instantiate(PublicYearButtonPrefab, PublicYears.transform)
            yearButton.GetComponent(Of Button)().onClick.AddListener(Sub() YearClicked(year, ii, yearCount))
            yearButton.GetComponent(Of RectTransform)().anchoredPosition = New Vector2(i * yearButtonWidth + 87, 0)
            yearButton.GetComponentInChildren(Of Text)().text = year
            i += 1
        Next
    End Sub

    Public Shared Sub YearClicked(year As String, barPos As Integer, yearCount As Integer)
        DisableYearButtons()
        PublicBar.GetComponent(Of ButtonController)().set(barPos, yearCount)
        StartVersionDisplay(year)
    End Sub

    Public Shared Sub StartVersionDisplay(year As String)
        ClearVersions()
        For Each y As KeyValuePair(Of Integer, List(Of Version)) In versionTable(year)
            toDo.Add(y.Key, y.Value)
        Next
        minors = toDo.Keys.ToList()
    End Sub

    Public Shared Sub DisableYearButtons()
        GenerateDict()
        PublicVersions.SetActive(True)
        ClearVersions()
        _PublicClickSound.GetComponent(Of AudioSource)().Play()
        PublicBar.SetActive(True)
        PublicPlane.SetActive(True)
    End Sub

    ' Update is called once per frame
    Private Sub Update()
        If minors.Count > 0 AndAlso Time.time - 0.2 > lastButtonSpawn Then
            If toDo.ContainsKey(minors(0)) AndAlso toDo(minors(0)).Count > 0 Then
                Dim version As String = toDo(minors(0))(0).BSVersion
                Dim button As GameObject = Instantiate(VersionButtonPrefab, Versions.transform)
                button.GetComponentInChildren(Of Text)().text = version
                button.GetComponent(Of RectTransform)().anchoredPosition = New Vector2(currentColumn * 90, currentRow * -30)
                button.GetComponentInChildren(Of Button)().onClick.AddListener(Sub()
                    If InstalledVersionToggle.installedVersions Then InstalledVersionToggle.SetBSVersion(version)
                    VersionText2.SetActive(True)
                    DownloadButton.SetActive(True)
                    DownloadButton.GetComponent(Of Button)().interactable = Not InstalledVersionToggle.installedVersions
                    Versions.GetComponent(Of VersionVar)().ListVersion(version)
                    ReleaseInfoButton.SetActive(True)
                    ReleaseInfoButton.GetComponent(Of Button)().interactable = False
                    If versions.FirstOrDefault(Function(x) x.BSVersion = version) IsNot Nothing Then
                        ReleaseInfoButton.GetComponent(Of Button)().interactable = True
                    End If

                    _ClickSound.GetComponent(Of AudioSource)().Play()
                End Sub)

                If (installedVersions.Contains(version) AndAlso Not InstalledVersionToggle.installedVersions) OrElse version.ToLower() = "none" Then
                    button.GetComponentInChildren(Of Button)().interactable = False
                End If
                currentColumn += 1
                toDo(minors(0)).RemoveAt(0)
                If toDo(minors(0)).Count <= 0 Then toDo.Remove(minors(0))
            Else
                currentRow += 1
                currentColumn = 0
                minors.RemoveAt(0)
            End If
        End If
    End Sub

End Class
