Imports System
Imports System.Windows

Module Program
    Public Sub Main(args As String())
        Dim rend As renderer = New renderer(800, 600, "vbOpen_TK")
        rend.Run(60)
    End Sub
End Module
