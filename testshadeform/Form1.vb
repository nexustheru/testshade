﻿Imports System.ComponentModel
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports Assimp
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class Form1

    Dim sce As Scene
    Dim Vertex As List(Of Vector3)
    Dim texcoord As List(Of Vector2)
    Dim texcoords1() As Vector2

    Dim colore As List(Of Vector4)
    Dim _indices As List(Of UInteger) = New List(Of UInteger)
    Dim Normals As List(Of Vector3)

    Dim vertexbuffer, vertexobject, facebuffer, faceobject, normalbuffer, normalobject, texbuffer, texobject, colorbuffer, colorobject, textureid As Integer
    Dim projection, modelview, view, MVP As Integer
    Dim vertexpos, normalpos, uvpos, colorpos, texturepos As Integer

    Dim assimMatrix As Matrix4
    Dim _shader As Shader
    Dim util As utility

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim loadfile = New Task(AddressOf loadAssimp)
        loadfile.Start()
        loadfile.Wait()
        setupModel()
        assimMatrix = util.FromMatrix(sce.RootNode.Transform)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        util = New utility()
        _shader = New Shader()
        _shader.createShader()
        _shader.linkShader()
        projection = GL.GetUniformLocation(_shader.shader_program, "projection")
        modelview = GL.GetUniformLocation(_shader.shader_program, "models")
        view = GL.GetUniformLocation(_shader.shader_program, "view")
        MVP = GL.GetUniformLocation(_shader.shader_program, "mvp")
        texturepos = GL.GetUniformLocation(_shader.shader_program, "pic")

        vertexpos = GL.GetAttribLocation(_shader.shader_program, "position")
        normalpos = GL.GetAttribLocation(_shader.shader_program, "normals")
        uvpos = GL.GetAttribLocation(_shader.shader_program, "texCoord")
        colorpos = GL.GetAttribLocation(_shader.shader_program, "colorr")

    End Sub


    Private Sub GlControl1_Paint(sender As Object, e As PaintEventArgs) Handles GlControl1.Paint
        GL.ClearColor(Color.CornflowerBlue)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Clear(ClearBufferMask.DepthBufferBit)
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.Enable(EnableCap.Lighting)
        GL.Enable(EnableCap.Light0)

        GL.Enable(EnableCap.DepthTest)
        GL.Enable(EnableCap.Normalize)
        GL.Enable(EnableCap.PolygonSmooth)
        GL.FrontFace(FrontFaceDirection.Ccw)
        GL.Enable(EnableCap.Texture2D)
        GL.CullFace(CullFaceMode.Back)
        If _indices.Count > 0 Then
            _shader.useShader()
            setMatrix()
            GL.BindVertexArray(vertexobject)
            GL.DrawElements(OpenGL.PrimitiveType.Triangles, _indices.Count, OpenGL.DrawElementsType.UnsignedInt, 0)
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
        End If

        GlControl1.SwapBuffers()

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub GlControl1_Resize(sender As Object, e As EventArgs) Handles GlControl1.Resize

        GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height)

    End Sub

    Private Sub GlControl1_Load(sender As Object, e As EventArgs) Handles GlControl1.Load



    End Sub


    Public Async Sub loadAssimp()
        Dim asa As AssimpContext = New AssimpContext()
        asa.SetConfig(New Configs.NormalSmoothingAngleConfig(66.0F))
        sce = asa.ImportFile("ball.fbx", PostProcessPreset.TargetRealTimeMaximumQuality And PostProcessSteps.Triangulate And PostProcessSteps.FlipUVs AndAlso PostProcessSteps.GenerateSmoothNormals)
        Vertex = New List(Of Vector3)
        Normals = New List(Of Vector3)
        texcoord = New List(Of Vector2)
        colore = New List(Of Vector4)
        For Each m In sce.Meshes
            If m.HasVertices = True Then

                ReDim texcoords1(m.VertexCount - 1)

                For i As Integer = 0 To m.VertexCount - 1

                    Dim vert As Vector3 = New Vector3(util.FromVector(m.Vertices(i)))
                    Vertex.Add(vert)
                    If m.HasNormals = True Then
                        Dim norm As Vector3 = New Vector3(util.FromVector(m.Normals(i)))
                        Normals.Add(norm)
                    End If
                    If m.HasTextureCoords(0) = True Then
                        Dim co As Vector2 = New Vector2(m.TextureCoordinateChannels(0).ElementAt(i).X, m.TextureCoordinateChannels(0).ElementAt(i).Y)
                        texcoords1(i) = co
                        texcoord.Add(co)
                    Else
                        Dim co As Vector2 = New Vector2(0.0F, 0.0F)
                        texcoord.Add(co)
                    End If
                    If m.HasVertexColors(0) = True Then
                        Dim col As Vector4 = New Vector4(m.VertexColorChannels(0).ElementAt(i).R, m.VertexColorChannels(0).ElementAt(i).G, m.VertexColorChannels(0).ElementAt(i).B, m.VertexColorChannels(0).ElementAt(i).A)
                        colore.Add(col)
                    End If
                Next i

            End If

            If m.HasFaces = True Then

                For f As Integer = 0 To m.FaceCount - 1
                    Dim fac = m.Faces.ElementAt(f)
                    Dim indy As UInteger() = m.GetUnsignedIndices()

                    For ind As UInteger = 0 To indy.Length - 1
                        _indices.Add(ind)

                    Next
                Next

            End If

            If sce.HasTextures = True Then
                textureid = GL.GenTexture()
                For i As Integer = 0 To sce.TextureCount - 1
                    Dim textt = sce.Textures.ElementAt(i)
                    If textt.IsCompressed = True Then
                        PrepareImage(util.GenerateBitmap(textt))

                    End If
                Next
            End If

        Next

    End Sub
    Public Sub setMatrix()

        Dim res As Single = Convert.ToSingle(GlControl1.Width / GlControl1.Height)

        Dim model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0F))
        Dim view1 = Matrix4.CreateTranslation(0.0F, 0.0F, -3.0F)
        Dim projection1 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0F), res, 0.1F, 100.0F)
        Dim mvp1 = model * view * projection

        GL.UniformMatrix4(projection, True, projection1)
        GL.UniformMatrix4(modelview, True, model)
        GL.UniformMatrix4(view, True, view1)
        GL.UniformMatrix4(MVP, True, mvp1)

    End Sub
    Public Sub setupModel()
        If Vertex.Count > 0 = True Then

            vertexobject = GL.GenVertexArray()
            GL.BindVertexArray(vertexobject)

            vertexbuffer = GL.GenBuffer()
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexbuffer)
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Count * Marshal.SizeOf(Of Single) * 3, Vertex.ToArray, BufferUsageHint.StaticDraw) 'pos
            GL.EnableVertexAttribArray(vertexpos)
            GL.VertexAttribPointer(vertexpos, 3, VertexAttribPointerType.Float, False, 3 * Marshal.SizeOf(Of Single), 0) 'pos

            normalbuffer = GL.GenBuffer()
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalbuffer)
            GL.BufferData(BufferTarget.ArrayBuffer, Normals.Count * Marshal.SizeOf(Of Single) * 3, Normals.ToArray, BufferUsageHint.StaticDraw) 'pos
            GL.EnableVertexAttribArray(normalpos)
            GL.VertexAttribPointer(normalpos, 3, VertexAttribPointerType.Float, False, 3 * Marshal.SizeOf(Of Single), 0) 'pos

            texbuffer = GL.GenBuffer()
            GL.BindBuffer(BufferTarget.ArrayBuffer, texbuffer)
            GL.BufferData(Of Vector2)(BufferTarget.ArrayBuffer, New IntPtr(texcoords1.Length * Vector2.SizeInBytes), texcoords1, BufferUsageHint.StaticDraw)
            GL.EnableVertexAttribArray(uvpos)
            GL.VertexAttribPointer(uvpos, 2, VertexAttribPointerType.Float, False, Vector2.SizeInBytes, 0)


            colorbuffer = GL.GenBuffer()
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorbuffer)
            GL.BufferData(BufferTarget.ArrayBuffer, colore.Count * Marshal.SizeOf(Of Single) * 4, colore.ToArray, BufferUsageHint.StaticDraw) 'pos
            GL.EnableVertexAttribArray(colorpos)
            GL.VertexAttribPointer(colorpos, 4, VertexAttribPointerType.Float, False, 4 * Marshal.SizeOf(Of Single), 0) 'pos

            facebuffer = GL.GenBuffer()
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, facebuffer)
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * Marshal.SizeOf(Of UInteger), _indices.ToArray, BufferUsageHint.StaticDraw)

            GL.BindVertexArray(0)

        End If
    End Sub
    Public Sub PrepareImage(ima As Bitmap)

        GL.BindTexture(TextureTarget.Texture2D, textureid)

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))
        Dim data As BitmapData = ima.LockBits(New System.Drawing.Rectangle(0, 0, ima.Width, ima.Height), ImageLockMode.[ReadOnly], System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
        ima.UnlockBits(data)

    End Sub
End Class
