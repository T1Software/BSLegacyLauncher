' From https://github.com/ComputerElite/ComputerUtils/blob/dd623c0b1d2c7063bea27b83558df7ecadc6d416/ComputerUtils/ComputerUtils.CommandLine.cs
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace ComputerUtils.CommandLine

    Public Class ParsedCommand

        Public absoluteApplicationPath As String = ""
        Public command As String = ""
        Public argsStr As String = ""
        Public args As New List(Of String)()
        Public success As Boolean = False
        Public commandParseError As String = ""

        Public Overrides Function ToString() As String
            Return "execute " & absoluteApplicationPath & " with args:" & vbLf & String.Join(vbLf, args)
        End Function

        Public Sub New(command As String)
            Dim c As ParsedCommand = parseCommand(command)
            Me.absoluteApplicationPath = c.absoluteApplicationPath
            Me.command = c.command
            Me.argsStr = c.argsStr
            Me.args = c.args
            Me.success = c.success
            Me.commandParseError = c.commandParseError
        End Sub

        Public Sub New()
        End Sub

        Public Shared Function parseCommand(command As String, Optional removeFirstArgument As Boolean = False, Optional location As String = "") As ParsedCommand
            Dim c As New ParsedCommand()
            Console.WriteLine("Parsing " & command)
            Dim quotationMarks As Integer = 0
            Dim currentCommand As String = ""
            For Each s As String In command.Split(" "c)
                Dim tmp As String = s
                If tmp.StartsWith("""") Then
                    Dim add As String = """"
                    If quotationMarks = 0 Then
                        tmp = tmp.Substring(1)
                    Else
                        add = ""
                    End If
                    quotationMarks += (add & tmp).TakeWhile(Function(ch) ch = """"c).Count()
                End If
                If tmp.EndsWith("""") Then
                    quotationMarks -= tmp.Reverse().TakeWhile(Function(ch) ch = """"c).Count()
                    If quotationMarks = 0 Then tmp = tmp.Substring(0, tmp.Length - 1)
                End If
                currentCommand &= (If(currentCommand = "", "", " ")) & tmp
                If quotationMarks = 0 Then
                    c.args.Add(currentCommand)
                    currentCommand = ""
                End If
                If quotationMarks < 0 Then
                    c.success = False
                    c.commandParseError = "too many quotation marks"
                    Return c
                End If
            Next
            If quotationMarks > 0 Then
                c.success = False
                c.commandParseError = "too little quotation marks"
            Else
                c.success = True
            End If
            If c.args.Count >= 1 AndAlso removeFirstArgument Then
                c.absoluteApplicationPath = If(Path.IsPathRooted(c.args(0)) OrElse File.Exists(c.args(0)) OrElse File.Exists(c.args(0) & ".exe"), c.args(0), location & (If(location.EndsWith("\"), "", "\")) & c.args(0))
                c.command = c.args(0)
                c.args.RemoveAt(0)
                For Each s As String In c.args
                    c.argsStr &= """" & s & """ "
                Next
            End If
            Return c
        End Function

    End Class

    Public Class CommandLineArgument

        Public aliases As New List(Of String)()
        Public isToggle As Boolean = False
        Public description As String = ""
        Public defaultValue As String = ""
        Public valueName As String = "value"

    End Class

    Public Class CommandLineCommandContainer

        Public arguments As New List(Of CommandLineArgument)()
        Public parsedCommand As String() = Nothing

        Public Sub New(arguments As String)
            parsedCommand = New ParsedCommand(arguments).args.ToArray()
        End Sub

        Public Sub New(arguments As String())
            parsedCommand = arguments
        End Sub

        Public Sub AddCommandLineArgument(aliases As List(Of String), Optional isToggle As Boolean = False, Optional description As String = "no description", Optional valueName As String = "value", Optional defaultValue As String = "")
            Dim a As New CommandLineArgument()
            a.aliases = aliases
            a.isToggle = isToggle
            a.description = description
            a.valueName = valueName
            a.defaultValue = defaultValue
            arguments.Add(a)
        End Sub

        Public Function HasArgument(name As String, Optional ignoreCase As Boolean = True) As Boolean
            For Each s As String In parsedCommand
                For Each a As CommandLineArgument In arguments
                    If a.aliases.FirstOrDefault(Function(x) (If(ignoreCase, x.ToLower(), x)) = (If(ignoreCase, name.ToLower(), name))) IsNot Nothing AndAlso a.aliases.FirstOrDefault(Function(x) (If(ignoreCase, x.ToLower(), x)) = (If(ignoreCase, s.ToLower(), s))) IsNot Nothing Then Return True
                Next
                If (ignoreCase AndAlso s.ToLower() = name.ToLower()) OrElse s = name Then Return True
            Next
            Return False
        End Function

        Public Function GetValue(name As String, Optional ignoreCase As Boolean = True) As String
            For i As Integer = 0 To parsedCommand.Length - 1
                For Each a As CommandLineArgument In arguments
                    If a.aliases.FirstOrDefault(Function(x) (If(ignoreCase, x.ToLower(), x)) = (If(ignoreCase, name.ToLower(), name))) IsNot Nothing AndAlso a.aliases.FirstOrDefault(Function(x) (If(ignoreCase, x.ToLower(), x)) = (If(ignoreCase, parsedCommand(i).ToLower(), parsedCommand(i)))) IsNot Nothing AndAlso parsedCommand.Length > i + 1 Then Return parsedCommand(i + 1)
                Next
                If ((ignoreCase AndAlso parsedCommand(i).ToLower() = name.ToLower()) OrElse parsedCommand(i) = name) AndAlso parsedCommand.Length > i + 1 Then Return parsedCommand(i + 1)
            Next
            For Each a As CommandLineArgument In arguments
                If ignoreCase AndAlso a.aliases.FirstOrDefault(Function(x) (If(ignoreCase, x.ToLower(), x)) = name) IsNot Nothing AndAlso Not a.isToggle AndAlso a.defaultValue <> "" Then Return a.defaultValue
            Next
            Return ""
        End Function

        Public Sub ShowHelp(Optional exeName As String = "", Optional extraInfo As String = "")
            Console.ForegroundColor = ConsoleColor.White
            Dim args As New Dictionary(Of Integer, List(Of CommandLineArgument))()
            Console.WriteLine()
            Console.Write(If(exeName.Contains(" "), """" & exeName & """", exeName) & " ")
            Dim length As Integer = 0
            For Each arg As CommandLineArgument In arguments
                If String.Join(" / ", arg.aliases).Length > length Then length = String.Join(" / ", arg.aliases).Length
                Console.ForegroundColor = ConsoleColor.White
                Console.Write("<")
                Console.ForegroundColor = ConsoleColor.DarkYellow
                Console.Write(arg.aliases(0))
                Console.ForegroundColor = ConsoleColor.DarkGreen
                Console.Write(If(arg.isToggle, "", " " & arg.valueName))
                Console.ForegroundColor = ConsoleColor.White
                Console.Write("> ")
            Next
            Console.WriteLine()
            Console.WriteLine()
            For Each arg As CommandLineArgument In arguments
                Console.ForegroundColor = ConsoleColor.DarkYellow
                Console.Write(String.Join(" / ", arg.aliases).PadRight(length) & " :   ")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write(arg.description)
                Console.ForegroundColor = ConsoleColor.Yellow
                If Not arg.isToggle AndAlso arg.defaultValue <> "" Then Console.Write("   default: " & arg.defaultValue)
                Console.Write(vbLf)
            Next
            Console.ForegroundColor = ConsoleColor.White

            Console.WriteLine()
            Console.WriteLine(extraInfo)
            Console.WriteLine()
        End Sub

    End Class

End Namespace
