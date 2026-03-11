Imports Assets.Scripts
Imports DepotDownloader
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Threading
Imports UnityEngine
Imports UnityEngine.UI
Imports Yggdrasil.Logging
Imports SteamKit2.SteamUser
Imports System
Imports System.Diagnostics
Imports System.Globalization

Public Class DepotDownloaderObject
    Inherits MonoBehaviour

    Public Shared instance As DepotDownloaderObject

    <Header("Other scripts")>
    Public DiscordController As DiscordController

    <Header("Scene Objects")>
    Public Username As InputField
    Public Password As InputField
    Public BackButton As Button
    Public ExitButton As Button
    Public UpdateButton As Button
    Public InputFields As GameObject
    Public StartButtonObject As GameObject
    Public DebugText As TextMesh

    <Header("Random Elements We Need")>
    Public VersionText1 As GameObject
    Public VersionText2 As GameObject
    Public ErrorTextObject As GameObject
    Public ErrorText As Text
    Public DownloadingText As TextMesh
    Public DownloadingTextAnim As Animator
    Public DownloadedText As GameObject
    Public DownloadedTextAnim As Animator
    Public PreAllocatingTextAnim As Animator
    Public LoginTextAnim As Animator
    Public DownloadDetailText As TextMesh
    Public ProgressBar As GameObject
    Public InnerProgressBar As Image
    Public InstalledVersionText As Text
    Public InstalledVersionObject As GameObject
    Public LocalGameFilesButton As Button
    Public InstallIPAButton As Button
    Public InvalidPasswordTips As GameObject

    <Header("Audio Sources")>
    Public DownloadSound As AudioSource
    Public ErrorSound As AudioSource
    Public StartSound As AudioSource
    Public StartDownloadSound As GameObject

    <Header("Animators")>
    Public TextDismiss As RuntimeAnimatorController
    Public TextEnter As RuntimeAnimatorController
    Public InstalledVer As RuntimeAnimatorController

    <Header("Popup Handlers")>
    Public PopupPrefab As GameObject
    Public LoadingPopup As GameObject
    Public PopupAnchor As GameObject
    Public LoadingAnchor As GameObject
    <HideInInspector>
    Public LoadingActiveInstance As GameObject

    Private updateDownloading As Boolean = False
    Private isDownloading As Boolean = False
    Public details As LogOnDetails
    Private request As SteamLoginResponse = SteamLoginResponse.NONE
    Private localCurrentDownloadStep As String
    Private downloadPercentage As Single
    Private downloadSmoothened As Single
    Private requestSteamGuardPopUp As Boolean = False
    Private requestLoginPrompt As Boolean = False

    Private downloadFinished As Boolean = False
    Private dd As Process = Nothing
    Public ddStartedTimes As Integer = 0

    ' Start is called before the first frame update
    Private Sub Start()
        instance = Me

        Log.Logger.AddTarget(New UnityConsoleLogTarget())

        Username.onEndEdit.AddListener(Sub(value)
            If Input.GetKey(KeyCode.Return) Then Password.Select()
        End Sub)

        Password.onEndEdit.AddListener(Sub(value)
            If Input.GetKey(KeyCode.Return) Then LoginPressed()
        End Sub)
    End Sub

    ' Update is called once per frame
    Private Sub Update()
        If Input.GetKeyDown(KeyCode.Tab) Then
            If Password.isFocused Then Username.Select() Else Password.Select()
        End If

        If isDownloading Then SetDownloadingLayout()

        If requestLoginPrompt Then
            Log.Info("Login prompt requested")
            SetLoginObjects(True)
            requestLoginPrompt = False
        End If

        If requestSteamGuardPopUp Then
            Log.Info("SteamGuard prompt requested")
            requestSteamGuardPopUp = False
            createSteamCodePopup("Check the code from your 2FA or Steam Guard")
        End If

        If downloadFinished Then
            OnMainThreadDownloadCompleted()
            downloadFinished = False
        End If

        downloadSmoothened = downloadSmoothened + downloadPercentage / 50 - downloadSmoothened / 50
        InnerProgressBar.fillAmount = downloadSmoothened / 100

        If updateDownloading Then
            updateDownloading = False
            DownloadDetailText.text = $"Downloading... {localCurrentDownloadStep}"

            DiscordController.DownloadProgress = $"Downloading... {localCurrentDownloadStep}"
            DiscordController.DownloadUpdate()
        End If

        Select Case request
            Case SteamLoginResponse.NONE
                ' No action
            Case SteamLoginResponse.INVALIDPASSWORD
                DisplayErrorText("INVALID PASSWORD")
                InvalidPasswordTips.SetActive(True)
            Case SteamLoginResponse.PASSWORDUNSET
                DisplayErrorText("INVALID CREDENTIALS")
            Case SteamLoginResponse.RATELIMIT
                DisplayErrorText("LOGIN RATELIMIT EXCEEDED")
            Case SteamLoginResponse.INVALIDLOGINAUTHCODE
                DisplayErrorText("INVALID CODE")
            Case SteamLoginResponse.EXPIREDLOGINAUTHCODE
                DisplayErrorText("CODE EXPIRED, PLEASE TRY AGAIN")
            Case SteamLoginResponse.NOTENOUGHSPACE
                DisplayErrorText("NOT ENOUGH SPACE ON DISK")
            Case SteamLoginResponse.EXCEPTION
                DisplayErrorText("AN UNKNOWN ERROR OCCURED, TRY AGAIN")
            Case SteamLoginResponse.BEATSABERNOTOWNED
                DisplayErrorText("BEAT SABER IS NOT PURCHASED ON THIS ACCOUNT")
            Case SteamLoginResponse.CONNECTIONFAILED
                DisplayErrorText("STEAM CONNECTION FAILED, TRY AGAIN LATER")
            Case SteamLoginResponse.NETNOTINSTALLED
                request = SteamLoginResponse.NONE
                DisplayErrorText("PLEASE INSTALL .NET 6.0")
            Case SteamLoginResponse.PATHDENIED
                DisplayErrorText("PATH IS DENIED")
            Case SteamLoginResponse.UNAUTHORIZED
                DisplayErrorText("UNAUTHORIZED TO DOWNLOAD DEPOT")
            Case SteamLoginResponse.PREALLOCATING
                LoginTextAnim.runtimeAnimatorController = TextDismiss
                PreAllocatingTextAnim.runtimeAnimatorController = TextEnter
        End Select
        request = SteamLoginResponse.NONE
    End Sub

    Private Sub SetDownloadingLayout()
        StartDownloadSound.SetActive(True)
        GameObject.Destroy(LoadingActiveInstance)
        SetLoginObjects(False)
        VersionText1.SetActive(False)
        VersionText2.SetActive(False)
        DownloadingText.text = $"Downloading {VersionVar.instance.version}..."
        DownloadingTextAnim.runtimeAnimatorController = TextEnter
        PreAllocatingTextAnim.runtimeAnimatorController = TextDismiss
        ProgressBar.SetActive(True)
        ExitButton.interactable = False
        UpdateButton.interactable = False
        InstalledVersionObject.SetActive(False)

        isDownloading = False
    End Sub

    Private Sub DisplayErrorText(err As String)
        ' Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(False)
        ErrorTextObject.SetActive(True)
        ErrorText.text = err
        ErrorSound.Play()
    End Sub

    Public Sub LoginPressed()
        If Not File.Exists(InstalledVersionToggle.BaseDirectory & "Resources\DepotDownloader\DepotDownloader.exe") Then
            DisplayErrorText("DEPOTDOWNLOADER NOT FOUND")
            Log.Info("DepotDownloader doesn't exist in /Resources/")
            Return
        End If
        If String.IsNullOrEmpty(Username.text) OrElse String.IsNullOrEmpty(Password.text) Then
            requestLoginPrompt = True
            request = SteamLoginResponse.PASSWORDUNSET
            Return
        End If

        If Not Directory.Exists(InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher_Data\Saved\steamcreds") Then Directory.CreateDirectory(InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher_Data\Saved\steamcreds")
        File.WriteAllText(InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher_Data\Saved\steamcreds\username.txt", $"{Username.text}")

        Log.Info("Triggered login")

        SetLoginObjects(False)

        InvalidPasswordTips.SetActive(False)

        details = New LogOnDetails() With {
            .Username = Username.text,
            .Password = Password.text
        }

        StartDownload()
    End Sub

    Private Sub OnDepotNotOwned()
        Log.Error("This user doesn't own the requested repo!")
        request = SteamLoginResponse.BEATSABERNOTOWNED
        requestLoginPrompt = True
        Directory.Delete(InstalledVersionToggle.GetBSDirectory(VersionVar.instance.version), True)
    End Sub

    Private Sub OnMainThreadDownloadCompleted()
        downloadPercentage = 100
        BackButton.interactable = True
        DownloadDetailText.text = "Download completed! Ready to Launch!"
        DownloadedText.gameObject.SetActive(True)
        DownloadingTextAnim.runtimeAnimatorController = TextDismiss
        DownloadedTextAnim.runtimeAnimatorController = TextEnter
        ExitButton.interactable = True
        UpdateButton.interactable = True
        InstallIPAButton.interactable = True
        LocalGameFilesButton.interactable = True
        InstalledVersionToggle.SetBSVersion(VersionVar.instance.version)
        If ComputersVars.useApertureDeskJob Then
            File.WriteAllText(InstalledVersionToggle.BSDirectory & "Beat Saber.exe", ":D This is real Beat Saber")
        End If
        ' InstalledVersionObject.SetActive(False) this just hides it forever
        DiscordController.DownloadProgress = "Download Finished"
        DiscordController.DownloadUpdate()
        Destroy(LoadingActiveInstance)
        DownloadSound.Play()
    End Sub

    Private Sub OnProgressUpdate(current As String, percentage As Single)
        isDownloading = True
        localCurrentDownloadStep = current
        downloadPercentage = percentage
        updateDownloading = True
    End Sub

    Private Sub createSteamCodePopup(description As String)
        Destroy(LoadingActiveInstance)
        Dim popup As SteamCodePopup = GameObject.Instantiate(PopupPrefab, PopupAnchor.transform).GetComponent(Of SteamCodePopup)()
        popup.callback = AddressOf steamCodePopupCallback
        popup.Description.text = description
        popup.gameObject.SetActive(True)
        VersionText1.SetActive(False)
        VersionText2.SetActive(False)
        InputFields.SetActive(False)
        BackButton.interactable = False
        StartButtonObject.gameObject.SetActive(False)
    End Sub

    Private Sub steamCodePopupCallback(code As String)
        LoadingActiveInstance = GameObject.Instantiate(LoadingPopup, LoadingAnchor.transform)
        VersionText1.SetActive(True)
        VersionText2.SetActive(True)
        details.TwoFactorCode = code
        Log.Debug("Entering code " & details.TwoFactorCode & " into DD")
        dd.StandardInput.WriteLine(details.TwoFactorCode)
    End Sub

    Private Sub SetLoginObjects(state As Boolean)
        BackButton.interactable = state
        StartButtonObject.gameObject.SetActive(state)
        InputFields.SetActive(state)

        If Not state AndAlso Not isDownloading Then
            LoadingActiveInstance = GameObject.Instantiate(LoadingPopup, LoadingAnchor.transform)
        ElseIf LoadingActiveInstance IsNot Nothing Then
            GameObject.Destroy(LoadingActiveInstance)
        End If
    End Sub

    Public Shared Sub MoveDirectory(source As String, target As String)
        Dim stack As New Stack(Of Folders)()
        stack.Push(New Folders(source, target))

        While stack.Count > 0
            Dim folders = stack.Pop()
            If Not Directory.Exists(folders.Target) Then Directory.CreateDirectory(folders.Target)
            For Each file In Directory.GetFiles(folders.Source, "*.*")
                Dim targetFile As String = Path.Combine(folders.Target, Path.GetFileName(file))
                If File.Exists(targetFile) Then File.Delete(targetFile)
                File.Move(file, targetFile)
            Next

            For Each folder In Directory.GetDirectories(folders.Source)
                stack.Push(New Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))))
            Next
        End While
        Directory.Delete(source, True)
    End Sub

    Public Sub StartDownload()
        StartSound.Play()
        ErrorTextObject.SetActive(False)
        StartDownloadSound.SetActive(False)
        Dim selectedVersion As Version = VersionButtonController.versions.First(Function(x) x.BSVersion.Equals(VersionVar.instance.version))
        Log.Info($"You selected version {selectedVersion.BSVersion} : {selectedVersion.BSManifest}")
        DiscordController.BSVersion = $"{selectedVersion.BSVersion}"

        If Directory.Exists(InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion)) Then Directory.Delete(InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion), True)
        Dim ddInfo As New ProcessStartInfo With {
            .FileName = InstalledVersionToggle.BaseDirectory & "Resources\DepotDownloader\DepotDownloader.exe",
            .Arguments = "-username """ & details.Username & """ -password """ & details.Password.Replace("""", "\""") & """ -manifest " & ULong.Parse(selectedVersion.BSManifest) & " -dir """ & InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion).TrimEnd("\"c) & """ -depot 620981 -app 620980",
            .RedirectStandardInput = True,
            .RedirectStandardOutput = True,
            .UseShellExecute = False,
            .CreateNoWindow = True
        }
        If ComputersVars.useApertureDeskJob Then
            ddInfo.Arguments = "-username """ & details.Username & """ -password """ & details.Password.Replace("""", "\""") & """ -manifest " & ULong.Parse(selectedVersion.BSManifest) & " -dir """ & InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion).TrimEnd("\"c) & """ -depot 1902492 -app 1902490"
        End If
        Dim t As New Thread(Sub()
            Try
                ddStartedTimes += 1
                Dim myDDProcess As Integer = ddStartedTimes
                Dim lines As Integer = 0
                downloadFinished = False
                dd = Process.Start(ddInfo)
                Log.Debug("Started dd")
                'dd.WaitForInputIdle()
                Dim line As String = ""
                While Not dd.StandardOutput.EndOfStream AndAlso myDDProcess = ddStartedTimes
                    If line.EndsWith(Environment.NewLine) OrElse line.Contains(" code ") Then
                        Log.Debug(line)
                        ProcessLine(line)
                        line = ""
                        lines += 1
                    End If
                    line &= ChrW(dd.StandardOutput.Read())
                End While

                If lines <= 0 Then
                    request = SteamLoginResponse.NETNOTINSTALLED
                    Process.Start("https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.408-windows-x64-installer")
                    requestLoginPrompt = True
                End If
            Catch ex As Exception
                requestLoginPrompt = True
                request = SteamLoginResponse.EXCEPTION
                Log.Error(ex.ToString())
            End Try
        End Sub)
        t.Start()
    End Sub

    Public Sub ProcessLine(line As String)
        If line.Contains("This account is protected") Then
            ' Invoke prompt here
            requestSteamGuardPopUp = True
        End If
        If line.Contains("LogOn requires a username and password to be set in") Then
            requestLoginPrompt = True
            Log.Debug("Nothing Entered")
            Return
        End If
        If line.Contains("Unset") Then
            requestLoginPrompt = True
            request = SteamLoginResponse.PASSWORDUNSET
            Log.Debug("PASSWORDUNSET")
            Return
        End If
        If line.Contains("InvalidPassword") Then
            requestLoginPrompt = True
            request = SteamLoginResponse.INVALIDPASSWORD
            Log.Debug("INVALIDPASSWORD")
            Return
        End If
        If line.Contains("404 for depot manifest") OrElse line.Contains("App") AndAlso line.Contains("is not available from this account") Then
            requestLoginPrompt = True
            OnDepotNotOwned()
            Log.Debug("DEPOTNOTOWNED")
            Return
        End If
        If line.Contains("401 for depot manifest") Then
            requestLoginPrompt = True
            request = SteamLoginResponse.UNAUTHORIZED
            Directory.Delete(InstalledVersionToggle.GetBSDirectory(VersionVar.instance.version), True)
            Return
        End If
        If line.Contains("Got depot key") Then
            ' Depot is owned
        End If
        If line.Contains("Connection to Steam failed") Then
            requestLoginPrompt = True
            Try
                dd.Kill()
            Catch
            End Try

            request = SteamLoginResponse.CONNECTIONFAILED

            Log.Debug("CONNECTIONFAILED")
            Return
        End If
        If line.Contains("RateLimitExceeded") Then
            requestLoginPrompt = True
            request = SteamLoginResponse.RATELIMIT
        End If
        If line.Contains("InvalidLoginAuthCode") Then
            Try
                dd.Kill()
            Catch
            End Try
            request = SteamLoginResponse.INVALIDLOGINAUTHCODE
            StartDownload()

            Log.Debug("INVALIDLOGINAUTHCODE")
            Return
        End If
        If line.Contains("ExpiredLoginAuthCode") Then
            Try
                dd.Kill()
            Catch
            End Try
            request = SteamLoginResponse.EXPIREDLOGINAUTHCODE
            StartDownload()

            Log.Debug("EXPIREDLOGINAUTHCODE")
            Return
        End If
        If line.Contains("There is not enough space") Then
            requestLoginPrompt = True
            request = SteamLoginResponse.NOTENOUGHSPACE
            Log.Debug("There is not enough space on the disk")
            Return
        End If
        If line.Contains("Access to the path is denied") Then
            requestLoginPrompt = True
            request = SteamLoginResponse.PATHDENIED
            Log.Debug("Access to the path is denied")
            Return
        End If
        If line.Contains("Pre-allocating") Then
            request = SteamLoginResponse.PREALLOCATING
            Log.Debug("Pre-allocating Disk Space for Beat Saber")
            Return
        End If
        If line.Contains("Got session token") Then
            ' Logged in
        End If
        If line.Contains("Total downloaded:") Then
            ' Download finished (maybe only partially but idc)
            downloadFinished = True
        End If
        If line.Contains("%") Then
            Dim percentage As String = line.Split("%"c)(0)
            Try
                Dim per As Single = Single.Parse(percentage.Replace(",", "."), CultureInfo.InvariantCulture)
                OnProgressUpdate(String.Format("{0:0.0}", per) & "%", per)
            Catch ex As Exception
                Log.Debug("Fuck you DD" & ex.ToString())
            End Try
        End If
    End Sub
End Class

Public Class Folders
    Private _source As String
    Private _target As String

    Public Property Source As String
        Get
            Return _source
        End Get
        Private Set(value As String)
            _source = value
        End Set
    End Property

    Public Property Target As String
        Get
            Return _target
        End Get
        Private Set(value As String)
            _target = value
        End Set
    End Property

    Public Sub New(source As String, target As String)
        Me.Source = source
        Me.Target = target
    End Sub
End Class

Friend Enum SteamLoginResponse
    NONE
    TWOFACTOR
    STEAMGUARD
    INVALIDPASSWORD
    PASSWORDUNSET
    RATELIMIT
    EXCEPTION
    INVALIDLOGINAUTHCODE
    EXPIREDLOGINAUTHCODE
    BEATSABERNOTOWNED
    CONNECTIONFAILED
    NETNOTINSTALLED
    NOTENOUGHSPACE
    PREALLOCATING
    PATHDENIED
    UNAUTHORIZED
End Enum
