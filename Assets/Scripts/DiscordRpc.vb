Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Text
Imports AOT

Public Class DiscordRpc

    <MonoPInvokeCallback(GetType(OnReadyInfo))>
    Public Shared Sub ReadyCallback(ByRef connectedUser As DiscordUser)
        Callbacks.readyCallback(connectedUser)
    End Sub
    Public Delegate Sub OnReadyInfo(ByRef connectedUser As DiscordUser)

    <MonoPInvokeCallback(GetType(OnDisconnectedInfo))>
    Public Shared Sub DisconnectedCallback(errorCode As Integer, message As String)
        Callbacks.disconnectedCallback(errorCode, message)
    End Sub
    Public Delegate Sub OnDisconnectedInfo(errorCode As Integer, message As String)

    <MonoPInvokeCallback(GetType(OnErrorInfo))>
    Public Shared Sub ErrorCallback(errorCode As Integer, message As String)
        Callbacks.errorCallback(errorCode, message)
    End Sub
    Public Delegate Sub OnErrorInfo(errorCode As Integer, message As String)

    <MonoPInvokeCallback(GetType(OnJoinInfo))>
    Public Shared Sub JoinCallback(secret As String)
        Callbacks.joinCallback(secret)
    End Sub
    Public Delegate Sub OnJoinInfo(secret As String)

    <MonoPInvokeCallback(GetType(OnSpectateInfo))>
    Public Shared Sub SpectateCallback(secret As String)
        Callbacks.spectateCallback(secret)
    End Sub
    Public Delegate Sub OnSpectateInfo(secret As String)

    <MonoPInvokeCallback(GetType(OnRequestInfo))>
    Public Shared Sub RequestCallback(ByRef request As DiscordUser)
        Callbacks.requestCallback(request)
    End Sub
    Public Delegate Sub OnRequestInfo(ByRef request As DiscordUser)

    Private Shared Property Callbacks As EventHandlers

    Public Structure EventHandlers
        Public readyCallback As OnReadyInfo
        Public disconnectedCallback As OnDisconnectedInfo
        Public errorCallback As OnErrorInfo
        Public joinCallback As OnJoinInfo
        Public spectateCallback As OnSpectateInfo
        Public requestCallback As OnRequestInfo
    End Structure

    <Serializable, StructLayout(LayoutKind.Sequential)>
    Public Structure RichPresenceStruct
        Public state As IntPtr              ' max 128 bytes
        Public details As IntPtr            ' max 128 bytes
        Public startTimestamp As Long
        Public endTimestamp As Long
        Public largeImageKey As IntPtr      ' max 32 bytes
        Public largeImageText As IntPtr     ' max 128 bytes
        Public smallImageKey As IntPtr      ' max 32 bytes
        Public smallImageText As IntPtr     ' max 128 bytes
        Public partyId As IntPtr            ' max 128 bytes
        Public partySize As Integer
        Public partyMax As Integer
        Public partyPrivacy As Integer
        Public matchSecret As IntPtr        ' max 128 bytes
        Public joinSecret As IntPtr         ' max 128 bytes
        Public spectateSecret As IntPtr     ' max 128 bytes
        Public instance As Boolean
    End Structure

    <Serializable>
    Public Structure DiscordUser
        Public userId As String
        Public username As String
        Public discriminator As String
        Public avatar As String
    End Structure

    Public Enum Reply
        No = 0
        Yes = 1
        Ignore = 2
    End Enum

    Public Enum PartyPrivacy
        [Private] = 0
        [Public] = 1
    End Enum

    Public Shared Sub Initialize(applicationId As String, ByRef handlers As EventHandlers, autoRegister As Boolean, optionalSteamId As String)
        Callbacks = handlers

        Dim staticEventHandlers As New EventHandlers()
        staticEventHandlers.readyCallback = AddressOf DiscordRpc.ReadyCallback
        staticEventHandlers.disconnectedCallback = AddressOf DiscordRpc.DisconnectedCallback
        staticEventHandlers.errorCallback = AddressOf DiscordRpc.ErrorCallback
        staticEventHandlers.joinCallback = AddressOf DiscordRpc.JoinCallback
        staticEventHandlers.spectateCallback = AddressOf DiscordRpc.SpectateCallback
        staticEventHandlers.requestCallback = AddressOf DiscordRpc.RequestCallback

        InitializeInternal(applicationId, staticEventHandlers, autoRegister, optionalSteamId)
    End Sub

    <DllImport("discord-rpc", EntryPoint:="Discord_Initialize", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub InitializeInternal(applicationId As String, ByRef handlers As EventHandlers, autoRegister As Boolean, optionalSteamId As String)
    End Sub

    <DllImport("discord-rpc", EntryPoint:="Discord_Shutdown", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub Shutdown()
    End Sub

    <DllImport("discord-rpc", EntryPoint:="Discord_RunCallbacks", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub RunCallbacks()
    End Sub

    <DllImport("discord-rpc", EntryPoint:="Discord_UpdatePresence", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub UpdatePresenceNative(ByRef presence As RichPresenceStruct)
    End Sub

    <DllImport("discord-rpc", EntryPoint:="Discord_ClearPresence", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub ClearPresence()
    End Sub

    <DllImport("discord-rpc", EntryPoint:="Discord_Respond", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub Respond(userId As String, reply As Reply)
    End Sub

    <DllImport("discord-rpc", EntryPoint:="Discord_UpdateHandlers", CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Sub UpdateHandlers(ByRef handlers As EventHandlers)
    End Sub

    Public Shared Sub UpdatePresence(presence As RichPresence)
        Dim presencestruct = presence.GetStruct()
        UpdatePresenceNative(presencestruct)
        presence.FreeMem()
    End Sub

    Public Class RichPresence

        Private _presence As RichPresenceStruct
        Private ReadOnly _buffers As New List(Of IntPtr)(10)

        Public state As String              ' max 128 bytes
        Public details As String            ' max 128 bytes
        Public startTimestamp As Long
        Public endTimestamp As Long
        Public largeImageKey As String      ' max 32 bytes
        Public largeImageText As String     ' max 128 bytes
        Public smallImageKey As String      ' max 32 bytes
        Public smallImageText As String     ' max 128 bytes
        Public partyId As String            ' max 128 bytes
        Public partySize As Integer
        Public partyMax As Integer
        Public partyPrivacy As PartyPrivacy
        Public matchSecret As String        ' max 128 bytes
        Public joinSecret As String         ' max 128 bytes
        Public spectateSecret As String     ' max 128 bytes
        Public instance As Boolean

        ''' <summary>
        ''' Get the <see cref="RichPresenceStruct"/> reprensentation of this instance
        ''' </summary>
        ''' <returns><see cref="RichPresenceStruct"/> reprensentation of this instance</returns>
        Friend Function GetStruct() As RichPresenceStruct
            If _buffers.Count > 0 Then
                FreeMem()
            End If

            _presence.state = StrToPtr(state)
            _presence.details = StrToPtr(details)
            _presence.startTimestamp = startTimestamp
            _presence.endTimestamp = endTimestamp
            _presence.largeImageKey = StrToPtr(largeImageKey)
            _presence.largeImageText = StrToPtr(largeImageText)
            _presence.smallImageKey = StrToPtr(smallImageKey)
            _presence.smallImageText = StrToPtr(smallImageText)
            _presence.partyId = StrToPtr(partyId)
            _presence.partySize = partySize
            _presence.partyMax = partyMax
            _presence.partyPrivacy = CInt(partyPrivacy)
            _presence.matchSecret = StrToPtr(matchSecret)
            _presence.joinSecret = StrToPtr(joinSecret)
            _presence.spectateSecret = StrToPtr(spectateSecret)
            _presence.instance = instance

            Return _presence
        End Function

        ''' <summary>
        ''' Returns a pointer to a representation of the given string with a size of maxbytes
        ''' </summary>
        ''' <param name="input">String to convert</param>
        ''' <returns>Pointer to the UTF-8 representation of <see cref="input"/></returns>
        Private Function StrToPtr(input As String) As IntPtr
            If String.IsNullOrEmpty(input) Then Return IntPtr.Zero
            Dim convbytecnt = Encoding.UTF8.GetByteCount(input)
            Dim buffer = Marshal.AllocHGlobal(convbytecnt + 1)
            For i As Integer = 0 To convbytecnt
                Marshal.WriteByte(buffer, i, 0)
            Next
            _buffers.Add(buffer)
            Marshal.Copy(Encoding.UTF8.GetBytes(input), 0, buffer, convbytecnt)
            Return buffer
        End Function

        ''' <summary>
        ''' Convert string to UTF-8 and add null termination
        ''' </summary>
        ''' <param name="toconv">string to convert</param>
        ''' <returns>UTF-8 representation of <see cref="toconv"/> with added null termination</returns>
        Private Shared Function StrToUtf8NullTerm(toconv As String) As String
            Dim str = toconv.Trim()
            Dim bytes = Encoding.Default.GetBytes(str)
            If bytes.Length > 0 AndAlso bytes(bytes.Length - 1) <> 0 Then
                str &= vbNullChar & vbNullChar
            End If
            Return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(str))
        End Function

        ''' <summary>
        ''' Free the allocated memory for conversion to <see cref="RichPresenceStruct"/>
        ''' </summary>
        Friend Sub FreeMem()
            For i As Integer = _buffers.Count - 1 To 0 Step -1
                Marshal.FreeHGlobal(_buffers(i))
                _buffers.RemoveAt(i)
            Next
        End Sub

    End Class

End Class
