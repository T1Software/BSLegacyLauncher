Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Diagnostics
Imports System.IO
Imports UnityEditor
Imports UnityEngine
Imports UnityEngine.UI
Imports System.Security.Principal
Imports System.Runtime.InteropServices
Imports Yggdrasil.Logging
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Linq
Imports System.Net
Imports System.IO.Compression
Imports System.Net.Http
Imports SteamKit2
Imports SteamKit2.Internal
Imports Debug = UnityEngine.Debug

<Serializable>
Public Class SymLinkLocations
    Public folders As New List(Of String)()
    Public customLevels As Boolean = False
    Public customWipLevels As Boolean = False
    Public customSongs As Boolean = False
    Public dlcs As Boolean = False

    Public Sub SaveToFile(Path As String)
        Dim FileStream As New StreamWriter(Path, False)
        FileStream.Write(JsonUtility.ToJson(Me))
        FileStream.Close()
    End Sub

    Public Shared Function LoadFile(Path As String) As SymLinkLocations
        Dim Reader As New StreamReader(Path)
        Dim Value As String = Reader.ReadToEnd()
        Reader.Close()
        Return JsonUtility.FromJson(Of SymLinkLocations)(Value)
    End Function
End Class

Public Class AdvancedButtons
    Inherits MonoBehaviour

    <Header("Text")>
    Public ErrorText As Text
    Public FeedbackText As Text
    Public InfoText As Text

    <Header("Text Objects")>
    Public ErrorTextObject As GameObject
    Public FeedbackTextObject As GameObject
    Public InfoTextObject As GameObject
    Public InstallingTextObject As GameObject

    <Header("Button Objects")>
    Public IPA4Button As GameObject
    Public IPA3Button As GameObject
    Public UninstallIPAButton As GameObject

    <Header("Buttons")>
    Public RelinkFoldersButton As Button

    <Header("Toggles & Fields")>
    Public CustomLevelsToggle As Toggle
    Public CustomWIPLevelsToggle As Toggle
    Public CustomSongsToggle As Toggle
    Public DLCToggle As Toggle
    Public OtherToggle As Toggle
    Public OtherField As InputField

    <Header("Audio & Others")>
    Public ErrorSound As AudioSource
    Public FeedbackSound As AudioSource

    Public Const jsonLocation As String = "Beat Saber Legacy Launcher_Data/Settings/LinkedFolders.json"
    Public settingsLocation As String = "Beat Saber Legacy Launcher_Data/Settings"
    Public Shared locations As New SymLinkLocations()

    Public Shared Sub Save()
        If Not Directory.Exists(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory & jsonLocation)) Then Directory.CreateDirectory(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory & jsonLocation))
        File.WriteAllText(InstalledVersionToggle.BaseDirectory & jsonLocation, JsonConvert.SerializeObject(locations))
    End Sub

    Public Shared Sub LoadSettings()
        If Not File.Exists(InstalledVersionToggle.BaseDirectory & jsonLocation) Then Save()
        Try
            locations = JsonConvert.DeserializeObject(Of SymLinkLocations)(File.ReadAllText(InstalledVersionToggle.BaseDirectory & jsonLocation))
        Catch
            Save()
        End Try
    End Sub

    Private Sub DisplayErrorText(text As String)
        ' Set to false to restart popup animation DON'T CHANGE
        FeedbackTextObject.SetActive(False)
        ErrorTextObject.SetActive(False)
        ErrorTextObject.SetActive(True)
        ErrorText.text = text
        ErrorSound.Play()
    End Sub

    Private Sub DisplayFeedbackText(text As String)
        ' Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(False)
        FeedbackTextObject.SetActive(False)
        FeedbackTextObject.SetActive(True)
        FeedbackText.text = text
        FeedbackSound.Play()
    End Sub

    Private Sub DisplayInfoText(text As String)
        ' Set to false to restart popup animation DON'T CHANGE
        InfoTextObject.SetActive(False)
        InfoTextObject.SetActive(True)
        InfoText.text = text
    End Sub

    Public Sub DirectoryCopy(sourceDirName As String, destDirName As String, copySubDirs As Boolean)
        ' Get the subdirectories for the specified directory.
        Dim dir As New DirectoryInfo(sourceDirName)

        If Not dir.Exists Then
            Throw New DirectoryNotFoundException(
                "Source directory does not exist or could not be found: " &
                sourceDirName)
        End If

        Dim dirs As DirectoryInfo() = dir.GetDirectories()

        ' If the destination directory doesn't exist, create it.
        Directory.CreateDirectory(destDirName)

        ' Get the files in the directory and copy them to the new location.
        Dim files As FileInfo() = dir.GetFiles()
        For Each file As FileInfo In files
            Dim tempPath As String = Path.Combine(destDirName, file.Name)
            file.CopyTo(tempPath, False)
        Next

        ' If copying subdirectories, copy them and their contents to new location.
        If copySubDirs Then
            For Each subdir As DirectoryInfo In dirs
                Dim tempPath As String = Path.Combine(destDirName, subdir.Name)
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs)
            Next
        End If
    End Sub

    Public Sub MoveDirectory(source As String, target As String)
        Dim sourcePath = source.TrimEnd("\"c, " "c)
        Dim targetPath = target.TrimEnd("\"c, " "c)
        Dim files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories).
                              GroupBy(Function(s) Path.GetDirectoryName(s))
        For Each folder In files
            Dim targetFolder = folder.Key.Replace(sourcePath, targetPath)
            Directory.CreateDirectory(targetFolder)
            For Each file In folder
                Dim targetFile = Path.Combine(targetFolder, Path.GetFileName(file))
                If File.Exists(targetFile) Then File.Delete(targetFile)
                File.Move(file, targetFile)
            Next
        Next
        Directory.Delete(source, True)
    End Sub

    Public InputString As New List(Of String)()

    Private Sub Delayfunc(delay As Single, action As Action)
        StartCoroutine(DelayCoroutine(delay, action))
    End Sub

    Private Shared Iterator Function DelayCoroutine(delay As Single, action As Action) As IEnumerator
        Yield New WaitForSeconds(delay)
        action.Invoke()
    End Function

    Public Sub BrowseAppdata()
        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\AppData\LocalLow\Hyperbolic Magnetism")
    End Sub

    Private Sub Start()
        If File.Exists(jsonLocation) Then
            locations = SymLinkLocations.LoadFile(jsonLocation)
        Else
            RelinkFoldersButton.interactable = False
        End If
    End Sub

    Public Sub BackupAppdata()
        Dim thisDay As DateTime = DateTime.Today

        Dim lastBackupPath = "Beat Saber AppData Backups\Latest Backup\Beat Saber"
        Dim sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\AppData\LocalLow\Hyperbolic Magnetism\Beat Saber"))
        Dim targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\" & thisDay.ToString("M")) & "\Beat Saber")

        If Directory.Exists(targetDirectoryPath) Then
            DisplayErrorText("BACKUP ON THAT DATE ALREADY EXISTS")
            Throw New Exception("Backup on that date already exists")
        Else
            If Not Directory.Exists(targetDirectoryPath) Then
                Directory.CreateDirectory(targetDirectoryPath)
            End If

            If Directory.Exists(lastBackupPath) Then
                Directory.Delete(lastBackupPath, True)
            End If

            If Directory.Exists(sourceDirectoryPath) Then
                DirectoryCopy(sourceDirectoryPath, targetDirectoryPath, True)
                DirectoryCopy(sourceDirectoryPath, lastBackupPath, True)
            End If

            DisplayFeedbackText("BACKUP CREATED")
        End If
    End Sub

    Public Sub BrowseGameFiles()
        Process.Start(InstalledVersionToggle.BSDirectory)
    End Sub

    Public Sub RevertAppdata()
        Dim thisDay As DateTime = DateTime.Today

        Dim lastBackupPath = "Beat Saber AppData Backups\Latest Backup\Beat Saber"
        Dim sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\AppData\LocalLow\Hyperbolic Magnetism\Beat Saber"))
        Dim targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\" & thisDay.ToString("M")) & "\Beat Saber")

        If Directory.Exists("Beat Saber AppData Backups") Then
            If Directory.Exists(sourceDirectoryPath) Then
                Directory.Delete(sourceDirectoryPath, True)
            End If
            DirectoryCopy(lastBackupPath, sourceDirectoryPath, True)
            DisplayFeedbackText("APPDATA RESTORED TO LATEST BACKUP")
        Else
            DisplayErrorText("LAST BACKUP NOT FOUND")
            Throw New Exception("Last backup not found")
        End If
    End Sub

    Public Sub ClearAppdata()
        Dim thisDay As DateTime = DateTime.Today

        Dim lastBackupPath = "Beat Saber AppData Backups\Latest Backup\Beat Saber"
        Dim sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\AppData\LocalLow\Hyperbolic Magnetism\Beat Saber"))
        Dim targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\" & thisDay.ToString("M")) & "\Beat Saber")

        If Directory.Exists("Beat Saber AppData Backups") Then
            Try
                Directory.Delete(sourceDirectoryPath, True)
                DisplayErrorText("APPDATA CLEARED")
            Catch
                DisplayErrorText("APPDATA NOT FOUND")
                Throw New Exception("AppData does not exist")
            End Try
        Else
            DisplayErrorText("CREATE A BACKUP FIRST")
            Throw New Exception("No backups found, cannot clear AppData")
        End If
    End Sub

    Public Sub PixelModPackLink()
        Application.OpenURL("https://github.com/iPixelGalaxy/iPixelGalaxy-Beat-Saber-Modpack")
    End Sub

    Public Sub PixelModPack()
        InstallingTextObject.SetActive(True)

        Delayfunc(0.3F, Sub()
            If Not Directory.Exists("Temp Files") Then
                Directory.CreateDirectory("Temp Files")
            End If

            Dim modpackversions As New List(Of String)()

            Dim parsed As JObject

            ' Check Version
            Using client As New WebClient()
                Dim json As String = client.DownloadString("https://raw.githubusercontent.com/iPixelGalaxy/iPixelGalaxy-Beat-Saber-Modpack/main/BSLegacyVersionCheck.json")
                parsed = JObject.Parse(json)

                For Each version As JProperty In parsed.Properties()
                    modpackversions.Add(version.Name)
                Next
            End Using

            Dim Version As String = File.ReadAllText($"{InstalledVersionToggle.BSBaseDir}\BeatSaberVersion.txt")
            Dim VersionIndex As Integer = modpackversions.IndexOf(Version)

            If VersionIndex = -1 Then
                ' Version is incompatible
                DisplayErrorText("INCOMPATIBLE BEAT SABER VERSION")
                DisplayInfoText($"Latest Compatible Version:{vbLf}{modpackversions(0)}")
                If Directory.Exists("Temp Files") Then
                    Directory.Delete("Temp Files", True)
                End If

                If File.Exists($"{InstalledVersionToggle.BSDirectory}\ModpackInfo") Then
                    File.Delete($"{InstalledVersionToggle.BSDirectory}\ModpackInfo")
                End If

                InstallingTextObject.SetActive(False)
                Throw New Exception("Incompatible Version")
            End If

            Dim externalMods As String() = parsed(Version).SelectToken("externalmods").ToObject(Of String())()
            Dim modpackurl As String = parsed(Version)("modpackurl").ToObject(Of String)()
            Dim message As String = parsed(Version)("message").ToObject(Of String)()

            ' Display Message
            If message <> "" Then
                Dim messageBuffer As String = "Installed Modpack for Beat Saber " & Version & vbLf & message
                DisplayInfoText(messageBuffer)
            Else
                Dim messageBuffer As String = "Installed Modpack for Beat Saber " & Version
                DisplayInfoText(messageBuffer)
            End If

            ' Download Modpack
            Using client As New WebClient()
                client.DownloadFile(modpackurl, "BeatSaberModpack.zip")
            End Using
            ZipFile.ExtractToDirectory("BeatSaberModpack.zip", "Temp Files")
            File.Delete("BeatSaberModpack.zip")

            For i As Integer = 1 To externalMods.Length - 1
                Dim CutIndex As Integer = externalMods(i).LastIndexOf("/"c) + 1
                Dim FileName As String = externalMods(i).Substring(CutIndex, externalMods(i).Length - CutIndex)

                If FileName.Contains(".zip") Then
                    Using client As New WebClient()
                        client.DownloadFile(externalMods(i), FileName)
                    End Using
                    ZipFile.ExtractToDirectory(FileName, "Temp Files")
                    File.Delete(FileName)
                ElseIf FileName.Contains(".dll") Then
                    Using client As New WebClient()
                        client.DownloadFile(externalMods(i), $"Temp Files\Plugins\{FileName}")
                    End Using
                Else
                    DisplayErrorText("PIXEL IS A DUMBASS")
                    Throw New Exception("Yep, Pixel is a dumbass")
                End If
            Next

            MoveDirectory("Temp Files", InstalledVersionToggle.BSDirectory)

            If File.Exists($"{InstalledVersionToggle.BSDirectory}\ModpackInfo") Then
                File.Delete($"{InstalledVersionToggle.BSDirectory}\ModpackInfo")
            End If

            If Directory.Exists("Temp Files") Then
                Directory.Delete("Temp Files", True)
            End If

            InstallingTextObject.SetActive(False)
            DisplayFeedbackText("INSTALLED MODPACK")
        End Sub)
    End Sub

    Public Sub InstallNewIPA()
        Try
            DirectoryCopy(InstalledVersionToggle.BaseDirectory & "Resources\BSIPA-4.2.2", InstalledVersionToggle.BSDirectory, True)

            Dim IPA = FindObjectOfType(Of CheckIPA)()
            IPA.IPAInstalled()

            ErrorTextObject.SetActive(False)
            DisplayFeedbackText("IPA 4.2.2 INSTALLED")
            IPA3Button.SetActive(False)
            IPA4Button.SetActive(False)
            UninstallIPAButton.SetActive(True)
        Catch
            DisplayErrorText("IPA ALREADY INSTALLED")
            Throw New Exception("An IPA version is already installed")
        End Try

        Dim bspath As String = InstalledVersionToggle.BSDirectory
        Process.Start(New ProcessStartInfo With {
            .WorkingDirectory = bspath,
            .FileName = bspath & "IPA.exe",
            .Arguments = "-n"
        })
    End Sub

    Public Sub InstallLegacyIPA()
        Try
            DirectoryCopy(InstalledVersionToggle.BaseDirectory & "Resources\BSIPA-Legacy", InstalledVersionToggle.BSDirectory, True)
            DisplayFeedbackText("LEGACY IPA INSTALLED")
            Dim IPA = FindObjectOfType(Of CheckIPA)()
            IPA.IPAInstalled()
        Catch
            DisplayErrorText("IPA ALREADY INSTALLED")
            Throw New Exception("An IPA version is already installed")
        End Try

        Dim bspath As String = InstalledVersionToggle.BSDirectory
        Process.Start(New ProcessStartInfo With {
            .WorkingDirectory = bspath,
            .FileName = "IPA.exe",
            .Arguments = """Beat Saber.exe"""
        })
    End Sub

    Public Sub UninstallIPA()
        Try
            If Directory.Exists(InstalledVersionToggle.BSDirectory) Then
                Dim bspath As String = InstalledVersionToggle.BSDirectory
                Dim IPADir As String = $"{InstalledVersionToggle.BSDirectory}IPA"
                Dim IPAexe As String = $"{InstalledVersionToggle.BSDirectory}IPA.exe"
                Dim IPAconfig As String = $"{InstalledVersionToggle.BSDirectory}IPA.exe.config"
                Dim IPAruntime As String = $"{InstalledVersionToggle.BSDirectory}IPA.runtimeconfig.json"
                Dim MonoCecildll As String = $"{InstalledVersionToggle.BSDirectory}Mono.Cecil.dll"

                Dim i As New ProcessStartInfo With {
                    .WorkingDirectory = bspath,
                    .FileName = "IPA.exe",
                    .Arguments = "--revert --nowait"
                }
                Dim p As Process = Process.Start(i)
                p.WaitForExit(5000)
                Try
                    If Directory.Exists(IPADir) Then
                        Directory.Delete(IPADir, True)
                        File.Delete(IPAexe)
                    End If

                    If File.Exists(IPAconfig) Then
                        File.Delete(IPAconfig)
                    End If

                    If File.Exists(IPAruntime) Then
                        File.Delete(IPAruntime)
                    End If

                    If File.Exists(MonoCecildll) Then
                        File.Delete(MonoCecildll)
                    End If

                    DisplayFeedbackText("IPA UNINSTALLED")

                    Dim IPA = FindObjectOfType(Of CheckIPA)()
                    IPA.IPANotInstalled()
                Catch
                    DisplayErrorText("FAILED TO DELETE IPA FILES")
                End Try
            End If
        Catch
            DisplayErrorText("IPA NOT INSTALLED")
            Throw New Exception("No BSIPA Installation has been found")
        End Try
    End Sub

    <DllImport("shell32.dll", SetLastError:=True)>
    Public Shared Function IsUserAnAdmin() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Public Sub CreateLinkedFolders()
        ' Your turn, ComputerElite
        ' Thanks Risk, headpats

        For Each d As String In Directory.GetDirectories(InstalledVersionToggle.BSBaseDir)
            Dim customSongsFolder As String = d & Path.DirectorySeparatorChar & "CustomSongs"
            Dim customLevelsFolder As String = d & Path.DirectorySeparatorChar & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomLevels"
            Dim customWIPLevelsFolder As String = d & Path.DirectorySeparatorChar & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomWIPLevels"
            Dim dLCsFolder As String = d & Path.DirectorySeparatorChar & "DLC"

            If CustomLevelsToggle.isOn Then
                If Not Directory.Exists(InstalledVersionToggle.CustomLevelsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomLevelsDirectory)
                ProcessLink(customLevelsFolder, InstalledVersionToggle.CustomLevelsDirectory)
                locations.customLevels = True
                Save()
            End If
            If CustomWIPLevelsToggle.isOn Then
                If Not Directory.Exists(InstalledVersionToggle.CustomWIPLevelsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomWIPLevelsDirectory)
                ProcessLink(customWIPLevelsFolder, InstalledVersionToggle.CustomWIPLevelsDirectory)
                locations.customWipLevels = True
                Save()
            End If
            If CustomSongsToggle.isOn Then
                If Not Directory.Exists(InstalledVersionToggle.CustomSongsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomSongsDirectory)
                ProcessLink(customSongsFolder, InstalledVersionToggle.CustomSongsDirectory)
                locations.customSongs = True
                Save()
            End If
            If DLCToggle.isOn Then
                If Not Directory.Exists(InstalledVersionToggle.DLCDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.DLCDirectory)
                ProcessLink(dLCsFolder, InstalledVersionToggle.DLCDirectory)
                locations.dlcs = True
                Save()
            End If
            If OtherToggle.isOn Then
                If OtherField.text.Contains("CustomLevels") Then
                    DisplayErrorText("USE CUSTOMLEVELS TOGGLE")
                    Throw New Exception("Use the CustomLevels Toggle to link CustomLevels")
                End If

                If OtherField.text.Contains("CustomWIPLevels") Then
                    DisplayErrorText("USE CUSTOMWIPLEVELS TOGGLE")
                    Throw New Exception("Use the CustomWIPLevels Toggle to link CustomLevels")
                End If

                If OtherField.text.Contains("Plugins") Then
                    DisplayErrorText("CANNOT LINK PLUGINS")
                    Throw New Exception("Sharing this folder is Forbidden")
                End If

                If Not locations.folders.Any(Function(x) x = OtherField.text) Then
                    locations.folders.Add(OtherField.text)
                    Save()
                End If

                If Not Directory.Exists($"{d}\{OtherField.text}") Then Directory.CreateDirectory($"{d}\{OtherField.text}")
                ProcessLink($"{d}\{OtherField.text}", $"{OtherField.text}")

                Dim inputStrings As String() = New String() {""}
                Dim input As String = ""

                For Each x As String In inputStrings
                    input &= vbLf & OtherField.text
                Next
            End If
        Next

        Log.Debug("Folders Linked")
        locations.SaveToFile(jsonLocation)
        RelinkFoldersButton.interactable = True
        DisplayFeedbackText("FOLDERS CREATED")
    End Sub

    Public Shared Sub AddAllSymlinksToSelectedBeatSaberFolder()
        Dim d As String = InstalledVersionToggle.BSDirectory
        Dim customSongsFolder As String = d & Path.DirectorySeparatorChar & "CustomSongs"
        Dim customLevelsFolder As String = d & Path.DirectorySeparatorChar & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomLevels"
        Dim customWIPLevelsFolder As String = d & Path.DirectorySeparatorChar & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomWIPLevels"
        Dim dLCsFolder As String = d & Path.DirectorySeparatorChar & "DLC"

        If locations.customLevels Then
            If Not Directory.Exists(InstalledVersionToggle.CustomLevelsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomLevelsDirectory)
            ProcessLink(customLevelsFolder, InstalledVersionToggle.CustomLevelsDirectory)
        End If
        If locations.customWipLevels Then
            If Not Directory.Exists(InstalledVersionToggle.CustomWIPLevelsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomWIPLevelsDirectory)
            ProcessLink(customWIPLevelsFolder, InstalledVersionToggle.CustomWIPLevelsDirectory)
        End If
        If locations.customSongs Then
            If Not Directory.Exists(InstalledVersionToggle.CustomSongsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomSongsDirectory)
            ProcessLink(customSongsFolder, InstalledVersionToggle.CustomSongsDirectory)
        End If
        If locations.dlcs Then
            If Not Directory.Exists(InstalledVersionToggle.DLCDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.DLCDirectory)
            ProcessLink(dLCsFolder, InstalledVersionToggle.DLCDirectory)
        End If
        For Each s As String In locations.folders
            If Not Directory.Exists($"{d}\{s}") Then Directory.CreateDirectory($"{d}\{s}")
            ProcessLink($"{d}\{s}", $"{s}")
        Next
    End Sub

    Public Shared Sub ProcessLink(path As String, target As String)
        If Directory.Exists(path) AndAlso Not New DirectoryInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint) Then
            DepotDownloaderObject.MoveDirectory(path, target)
        End If
        If Not Directory.Exists(path) Then
            CreateLink(path, target)
        End If
    End Sub

    Public Shared Sub CreateLink(path As String, target As String)
        Dim i As New ProcessStartInfo With {
            .FileName = "cmd.exe",
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .UseShellExecute = False,
            .Arguments = "/c ""mklink /J """ & path & """ """ & target & "\" & """"""
        }
        Dim p As Process = Process.Start(i)
        p.WaitForExit()
    End Sub

    Public Sub RelinkFolders()
        Dim d As String = InstalledVersionToggle.BSDirectory
        Dim customSongsFolder As String = d & Path.DirectorySeparatorChar & "CustomSongs"
        Dim customLevelsFolder As String = d & Path.DirectorySeparatorChar & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomLevels"
        Dim customWIPLevelsFolder As String = d & Path.DirectorySeparatorChar & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomWIPLevels"
        Dim dLCsFolder As String = d & Path.DirectorySeparatorChar & "DLC"

        If locations.customLevels Then
            If Not Directory.Exists(InstalledVersionToggle.CustomLevelsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomLevelsDirectory)
            ProcessLink(customLevelsFolder, InstalledVersionToggle.CustomLevelsDirectory)
        End If
        If locations.customWipLevels Then
            If Not Directory.Exists(InstalledVersionToggle.CustomWIPLevelsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomWIPLevelsDirectory)
            ProcessLink(customWIPLevelsFolder, InstalledVersionToggle.CustomWIPLevelsDirectory)
        End If
        If locations.customSongs Then
            If Not Directory.Exists(InstalledVersionToggle.CustomSongsDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.CustomSongsDirectory)
            ProcessLink(customSongsFolder, InstalledVersionToggle.CustomSongsDirectory)
        End If
        If locations.dlcs Then
            If Not Directory.Exists(InstalledVersionToggle.DLCDirectory) Then Directory.CreateDirectory(InstalledVersionToggle.DLCDirectory)
            ProcessLink(dLCsFolder, InstalledVersionToggle.DLCDirectory)
        End If

        For Each folder As String In locations.folders
            If Not Directory.Exists($"{d}\{folder}") Then Directory.CreateDirectory($"{d}\{folder}")
            ProcessLink($"{d}\{folder}", $"{folder}")
        Next

        DisplayInfoText("The YeetMods variable in Beat Saber IPA.json" & vbLf & "has been set to false to prevent mod removal")

        DisplayFeedbackText("FOLDERS RELINKED")
    End Sub

    Public Sub OpenPatreonURL()
        Application.OpenURL("https://patreon.com/RiskiVR")
    End Sub

    Public Sub OpenKofiURL()
        Application.OpenURL("https://ko-fi.com/U7U114VMM")
    End Sub
End Class
