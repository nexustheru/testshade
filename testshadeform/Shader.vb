Imports OpenTK
Imports OpenTK.Graphics.OpenGL

Public Class Shader
    Private vert_shader_source, frag_shader_source, info As String
    Private vertex_shader, fragment_shader, shader_program, stats As Integer
    Public locations As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
    Public RichTextBox1 As RichTextBox
    Public mat As Matrix4

    Public Sub DebugMessage(message As String)
        RichTextBox1.AppendText(message)
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
        linkShader()
        checkAndSetLocation()
        useShader()
        setUni()
    End Sub
    Public Sub compileShader()
        'vertex shader
        GL.CompileShader(vertex_shader)
        info = GL.GetShaderInfoLog(vertex_shader)
        GL.GetShader(vertex_shader, ShaderParameter.CompileStatus, stats)
        If stats <> 1 Then
            RichTextBox1.AppendText("vert compiling error" + vbNewLine)
            Throw New ApplicationException(info)
        End If

        'fragment shader
        GL.CompileShader(fragment_shader)
        info = GL.GetShaderInfoLog(fragment_shader)
        GL.GetShader(fragment_shader, ShaderParameter.CompileStatus, stats)
        If stats <> 1 Then
            RichTextBox1.AppendText("frag compiling error" + vbNewLine)
            Throw New ApplicationException(info)
        End If
    End Sub
    Public Sub linkShader()
        GL.LinkProgram(shader_program)
        GL.GetProgram(shader_program, GetProgramParameterName.LinkStatus, stats)
    End Sub
    Public Sub useShader()
        GL.UseProgram(shader_program)
        RichTextBox1.AppendText("using shaderprogram" + vbNewLine)
    End Sub
    Public Sub checkAndSetLocation()
        If stats = All.True Then

            Dim positionLocation = GL.GetAttribLocation(shader_program, "position")
            If positionLocation = -1 Then
                RichTextBox1.AppendText("positionLocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(positionLocation)
                locations.Add("position", positionLocation)
                RichTextBox1.AppendText("positionLocation bound to " & positionLocation.ToString() + vbNewLine)
            End If


            Dim normalLocation = GL.GetAttribLocation(shader_program, "normals")
            If normalLocation = -1 Then
                RichTextBox1.AppendText("normallocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(normalLocation)
                locations.Add("normals", normalLocation)
                RichTextBox1.AppendText("normalLocation bound to " & normalLocation.ToString() + vbNewLine)
            End If


            Dim texLocation = GL.GetAttribLocation(shader_program, "texCoord")
            If texLocation = -1 Then
                RichTextBox1.AppendText("texcoordLocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(texLocation)
                locations.Add("texcoord", texLocation)
                RichTextBox1.AppendText("texcoordLocation bound to " & texLocation.ToString() + vbNewLine)
            End If


            Dim colorLocation = GL.GetAttribLocation(shader_program, "colorr")
            If colorLocation = -1 Then
                RichTextBox1.AppendText("colorLocation is not bound" + vbNewLine)
            Else
                GL.EnableVertexAttribArray(colorLocation)
                locations.Add("colorr", colorLocation)
                RichTextBox1.AppendText("colorLocation bound to " & colorLocation.ToString() + vbNewLine)
            End If


            Dim modelv = GL.GetUniformLocation(shader_program, "models")
            If modelv = -1 Then
                RichTextBox1.AppendText("model is not bound" + vbNewLine)
            Else
                locations.Add("models", modelv)
                RichTextBox1.AppendText("model bound to " & modelv.ToString() + vbNewLine)
            End If

            Dim projv = GL.GetUniformLocation(shader_program, "projection")
            If projv = -1 Then
                RichTextBox1.AppendText("projection is not bound" + vbNewLine)
            Else
                locations.Add("projection", projv)
                RichTextBox1.AppendText("projection bound to " & projv.ToString() + vbNewLine)
            End If

            Dim view = GL.GetUniformLocation(shader_program, "view")
            If view = -1 Then
                RichTextBox1.AppendText("view is not bound" + vbNewLine)
            Else
                locations.Add("view", view)
                RichTextBox1.AppendText("view bound to " & view.ToString() + vbNewLine)
            End If

            Dim texturee = GL.GetUniformLocation(shader_program, "pic")
            If texturee = -1 Then
                RichTextBox1.AppendText("pic is not bound" + vbNewLine)
            Else

                locations.Add("pic", texturee)
                RichTextBox1.AppendText("picbound to " & texturee.ToString() + vbNewLine)
            End If

            RichTextBox1.AppendText("Linked " + vbNewLine)
        Else
            RichTextBox1.AppendText("Program is not linked" + vbNewLine)
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
