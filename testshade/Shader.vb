Imports System
Imports System.IO
Imports System.Text
Imports System.Collections.Generic
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class Shader
    Public ReadOnly Handle As Integer
    Public _fragObj, _vertexObj, statusCode As Integer
    Dim info As String
    Public locations As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)

    Public Sub compileVertex(ByVal vertPath As String)
        GL.ShaderSource(_vertexObj, File.ReadAllText(vertPath))

        GL.CompileShader(_vertexObj)
        info = GL.GetShaderInfoLog(_vertexObj)


        GL.GetShader(_vertexObj, ShaderParameter.CompileStatus, statusCode)
        If statusCode <> 1 Then
            Console.Write("vert compiling error ")
            Throw New ApplicationException(info)
        End If
    End Sub
    Public Sub compileFragment(fragPath As String)
        GL.ShaderSource(_fragObj, File.ReadAllText(fragPath))

        GL.CompileShader(_fragObj)
        info = GL.GetShaderInfoLog(_fragObj)

        GL.GetShader(_fragObj, ShaderParameter.CompileStatus, statusCode)
        If statusCode <> 1 Then
            Console.Write("frag compiling error ")
            Throw New ApplicationException(info)
        End If
    End Sub

    Public Sub linkandUse()
        Dim stats As Integer

        GL.EnableVertexAttribArray(0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, False, Vector3.SizeInBytes, 0) 'vertex

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, False, Vector3.SizeInBytes, 0) 'normal
        GL.EnableVertexAttribArray(1)

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, False, Vector2.SizeInBytes, 0) 'tex
        GL.EnableVertexAttribArray(2)

        GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, False, Vector4.SizeInBytes, 0) 'color
        GL.EnableVertexAttribArray(3)


        GL.LinkProgram(Handle)
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, stats)

        If stats = All.True Then
            checkBindingd(Handle)
            GL.UseProgram(Handle)
            Console.Write("Linked and using shadeprogram" + vbNewLine)
        Else
            Console.Write("programs is not linked" + vbNewLine)
        End If

    End Sub

    Public Sub setMatrix(ByVal mat As Matrix4, position As Integer)
        GL.UniformMatrix4(position, True, mat)
        ' GL.UniformMatrix4(projv, True, perspective)
    End Sub
    Public Sub New(ByVal vertPath As String, ByVal fragPath As String)
        Try
            _vertexObj = GL.CreateShader(ShaderType.VertexShader)
            _fragObj = GL.CreateShader(ShaderType.FragmentShader)

            compileVertex(vertPath)
            compileFragment(fragPath)

            Handle = GL.CreateProgram()
            OpenTK.Graphics.OpenGL.GL.AttachShader(Handle, _vertexObj)
            OpenTK.Graphics.OpenGL.GL.AttachShader(Handle, _fragObj)

            linkandUse()

            OpenTK.Graphics.OpenGL.GL.DetachShader(Handle, _vertexObj)
            OpenTK.Graphics.OpenGL.GL.DetachShader(Handle, _fragObj)
            OpenTK.Graphics.OpenGL.GL.DeleteShader(_fragObj)
            OpenTK.Graphics.OpenGL.GL.DeleteShader(_vertexObj)

            Console.Write("created")
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Sub
    Public Sub Use()
        OpenTK.Graphics.OpenGL.GL.UseProgram(Handle)
    End Sub

    Public Sub checkBindingd(shader_program As Integer)
        Dim positionLocation = GL.GetAttribLocation(shader_program, "position")
        If positionLocation = -1 Then
            Console.WriteLine("positionLocation is not bound" + vbNewLine)
        Else
            GL.EnableVertexAttribArray(positionLocation)
            locations.Add("position", positionLocation)
            Console.WriteLine("positionLocation bound to " & positionLocation.ToString() + vbNewLine)
        End If


        Dim normalLocation = GL.GetAttribLocation(shader_program, "normal")
        If normalLocation = -1 Then
            Console.WriteLine("normallocation is not bound" + vbNewLine)
        Else
            GL.EnableVertexAttribArray(normalLocation)
            locations.Add("normal", normalLocation)
            Console.WriteLine("normalLocation bound to " & normalLocation.ToString() + vbNewLine)
        End If


        Dim texLocation = GL.GetAttribLocation(shader_program, "uv")
        If texLocation = -1 Then
            Console.WriteLine("texLocation is not bound" + vbNewLine)
        Else
            GL.EnableVertexAttribArray(texLocation)
            locations.Add("uv", texLocation)
            Console.WriteLine("texLocation bound to " & texLocation.ToString() + vbNewLine)
        End If


        Dim colorLocation = GL.GetAttribLocation(shader_program, "colors")
        If colorLocation = -1 Then
            Console.WriteLine("colorLocation is not bound" + vbNewLine)
        Else
            GL.EnableVertexAttribArray(colorLocation)
            locations.Add("colors", colorLocation)
            Console.WriteLine("colorLocation bound to " & colorLocation.ToString() + vbNewLine)
        End If

        Dim modelv = GL.GetUniformLocation(shader_program, "models")
        If modelv = -1 Then
            Console.WriteLine("model is not bound" + vbNewLine)
        Else
            locations.Add("model", modelv)
            Console.WriteLine("model bound to " & modelv.ToString() + vbNewLine)
        End If

        Dim projv = GL.GetUniformLocation(shader_program, "projection")
        If projv = -1 Then
            Console.WriteLine("projection is not bound" + vbNewLine)
        Else
            locations.Add("projection", projv)
            Console.WriteLine("projection bound to " & projv.ToString() + vbNewLine)
        End If

        Dim view = GL.GetUniformLocation(shader_program, "view")
        If view = -1 Then
            Console.WriteLine("view is not bound" + vbNewLine)
        Else
            locations.Add("view", view)
            Console.WriteLine("view bound to " & view.ToString() + vbNewLine)
        End If


    End Sub
End Class