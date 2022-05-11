Imports OpenTK
Imports OpenTK.Graphics.OpenGL

Public Class Shader
    Private vert_shader_source, frag_shader_source, info As String
    Public vertex_shader, fragment_shader, shader_program, stats As Integer
    Public locations As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
    Public mat As Matrix4

    Public Sub DebugMessage(message As String)
        Console.WriteLine(message)
    End Sub
    Public Sub createShader()

        vert_shader_source = IO.File.ReadAllText("vertex.glsl")
        frag_shader_source = IO.File.ReadAllText("fragment.glsl")
        vertex_shader = GL.CreateShader(ShaderType.VertexShader)
        GL.ShaderSource(vertex_shader, vert_shader_source)

        fragment_shader = GL.CreateShader(ShaderType.FragmentShader)
        GL.ShaderSource(fragment_shader, frag_shader_source)

        compileShader()
        shader_program = GL.CreateProgram()
        GL.AttachShader(shader_program, vertex_shader)
        GL.AttachShader(shader_program, fragment_shader)

        'checkAndSetLocation()
        'useShader()
        'setUni()
    End Sub
    Public Sub compileShader()
        'vertex shader
        GL.CompileShader(vertex_shader)
        info = GL.GetShaderInfoLog(vertex_shader)
        GL.GetShader(vertex_shader, ShaderParameter.CompileStatus, stats)
        If stats <> 1 Then
            Console.WriteLine("vert compiling error" + vbNewLine)
            Throw New ApplicationException(info)
        End If

        'fragment shader
        GL.CompileShader(fragment_shader)
        info = GL.GetShaderInfoLog(fragment_shader)
        GL.GetShader(fragment_shader, ShaderParameter.CompileStatus, stats)
        If stats <> 1 Then
            Console.WriteLine("frag compiling error" + vbNewLine)
            Throw New ApplicationException(info)
        End If
    End Sub
    Public Sub linkShader()
        GL.LinkProgram(shader_program)
        GL.GetProgram(shader_program, GetProgramParameterName.LinkStatus, stats)
        If stats = All.True Then
            Console.WriteLine("Linked " + vbNewLine)
        End If
    End Sub
    Public Sub useShader()
        GL.UseProgram(shader_program)

    End Sub
    Public Sub deleteShader()
        GL.DeleteProgram(shader_program)
        GL.DeleteShader(vertex_shader)
        GL.DeleteShader(fragment_shader)
    End Sub
    Public Sub checkAndSetLocation()
        If stats = All.True Then

            Dim positionLocation = GL.GetAttribLocation(shader_program, "position")
            If positionLocation = -1 Then
                Console.WriteLine("positionLocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(positionLocation)
                locations.Add("position", positionLocation)
                Console.WriteLine("positionLocation bound to " & positionLocation.ToString() + vbNewLine)
            End If


            Dim normalLocation = GL.GetAttribLocation(shader_program, "normals")
            If normalLocation = -1 Then
                Console.WriteLine("normallocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(normalLocation)
                locations.Add("normals", normalLocation)
                Console.WriteLine("normalLocation bound to " & normalLocation.ToString() + vbNewLine)
            End If


            Dim texLocation = GL.GetAttribLocation(shader_program, "texCoord")
            If texLocation = -1 Then
                Console.WriteLine("texcoordLocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(texLocation)
                locations.Add("texcoord", texLocation)
                Console.WriteLine("texcoordLocation bound to " & texLocation.ToString() + vbNewLine)
            End If


            Dim colorLocation = GL.GetAttribLocation(shader_program, "colorr")
            If colorLocation = -1 Then
                Console.WriteLine("colorLocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(colorLocation)
                locations.Add("colorr", colorLocation)
                Console.WriteLine("colorLocation bound to " & colorLocation.ToString() + vbNewLine)
            End If


            Dim modelv = GL.GetUniformLocation(shader_program, "models")
            If modelv = -1 Then
                Console.WriteLine("model is not bound" + vbNewLine)
            Else
                locations.Add("models", modelv)
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

            Dim texturee = GL.GetUniformLocation(shader_program, "pic")
            If texturee = -1 Then
                Console.WriteLine("pic is not bound" + vbNewLine)
            Else

                locations.Add("pic", texturee)
                Console.WriteLine("picbound to " & texturee.ToString() + vbNewLine)
            End If

            Console.WriteLine("Linked " + vbNewLine)
        Else
            Console.WriteLine("Program is not linked" + vbNewLine)
        End If
    End Sub

    Public Sub setUni()
        For Each item In locations
            If item.Key = "projection" Then
                GL.UniformMatrix4(item.Value, True, mat)
            ElseIf item.Key = "pic" Then
                GL.Uniform1(item.Value, 0)
            End If

        Next

    End Sub
End Class
