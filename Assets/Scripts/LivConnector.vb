Imports Microsoft.Win32
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports UnityEngine

Public Class LivConnector

    Private Const LivApplicationsKey As String = "Software\LIV.App\ExternalApplications"
    Private Const BSLLIdPrefix As String = "bs-legacy-"

    Public Structure LivEntry
        Public Id As String
        Public Name As String
        Public InstallPath As String
        Public Arguments As String
        Public Executable As String
    End Structure

    Public Shared Sub AddAndUpdateAllRegistryEntries(versions As List(Of String))
        ClearAllLivEntries(BSLLIdPrefix)
        Dim entries As New List(Of LivEntry)()
        Dim exeLoc As String = InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher.exe"
        For Each version As String In versions
            If version Is Nothing Then Continue For
            Debug.Log(version)
            entries.Add(New LivEntry() With {
                .Id = BSLLIdPrefix & version.Replace(".", "-"),
                .Name = "Beat Saber v" & version,
                .InstallPath = InstalledVersionToggle.GetBSDirectory(version),
                .Executable = exeLoc,
                .Arguments = "--version """ & version & """"
            })
        Next
        Debug.Log(CreateLivEntries(entries))
    End Sub

    ''' <returns>Whether the entire operation was successful or not.</returns>
    Public Shared Function ClearAllLivEntries(idPrefix As String) As Boolean
        idPrefix = idPrefix.ToLowerInvariant()

        Try
            Using hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                Using livApplicationsKey = hkcu.CreateSubKey(LivApplicationsKey, True)
                    If livApplicationsKey Is Nothing Then
                        Return False
                    End If

                    Dim applicationEntries = livApplicationsKey.GetSubKeyNames()

                    For Each applicationId In applicationEntries
                        If Not applicationId.StartsWith(idPrefix, StringComparison.InvariantCultureIgnoreCase) Then
                            Continue For
                        End If

                        livApplicationsKey.DeleteSubKeyTree(applicationId, False)
                    Next

                    Return True
                End Using
            End Using
        Catch ex As Exception
            ' You should log this; generally we haven't seen issues.
            Debug.LogError(ex.ToString())
            Return False
        End Try
    End Function

    ''' <returns>Whether the entire operation was successful or not.</returns>
    Public Shared Function CreateLivEntries(entries As IEnumerable(Of LivEntry)) As Boolean
        Try
            Using hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                Using livApplicationsKey = hkcu.CreateSubKey(LivApplicationsKey, True)
                    If livApplicationsKey Is Nothing Then
                        Return False
                    End If

                    For Each entry In entries
                        Using entryKey = livApplicationsKey.CreateSubKey(entry.Id, True)
                            If entryKey Is Nothing Then
                                Return False
                            End If

                            entryKey.SetValue("Name", entry.Name, RegistryValueKind.String)
                            entryKey.SetValue("InstallPath", entry.InstallPath, RegistryValueKind.String)
                            entryKey.SetValue("Executable", entry.Executable, RegistryValueKind.String)
                            entryKey.SetValue("Arguments", entry.Arguments, RegistryValueKind.String)
                        End Using
                    Next

                    Return True
                End Using
            End Using
        Catch ex As Exception
            ' You should log this; generally we haven't seen issues.
            Debug.LogError(ex.ToString())
            Return False
        End Try
    End Function
End Class
