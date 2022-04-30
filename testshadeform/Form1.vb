Imports System.ComponentModel
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK
Imports OpenTK.Graphics.OpenGL
Imports Assimp

Public Class Form1

    Public m_angle, aspectRatio As Single
    Public perspective, lookat, mp As Matrix4
    Public m_sceneCenter, m_sceneMin, m_sceneMax As Vector3
    Public Structure Model
        Dim Vertex As List(Of Vector3)
        Dim Faces As List(Of Vector3)
        Dim Normals As List(Of Vector3)
        Dim Colors As List(Of Vector4)
        Dim Uv As List(Of Vector2)
        Dim textures As List(Of Bitmap)
        Dim materials As List(Of Material)
        Dim sce As Scene
        Dim tex As Integer
    End Structure
    Private vert_shader_source, frag_shader_source, info As String
    Private vertex_shader, fragment_shader, shader_program, stats As Integer
    Private texture As Integer
    Private texture_bmp As New Bitmap(500, 500)

    Public m_model As Model
    Dim vao, vbo As Integer
    Dim util As utility
    Dim _shade As Shader
    Dim time As Timers.Timer

    Private Sub glUpdate(sender As Object, e As System.Timers.ElapsedEventArgs)
        m_angle += 25.0F * Convert.ToSingle(time.Interval)
        If m_angle > 360 Then

            m_angle = 0.0F
        End If

        GlControl1.Invalidate()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        m_model = New Model()
        util = New utility()
        loadmod("ball.fbx")
        UploadBuffers()
        ComputeBoundingBox()
        _shade = New Shader()
        _shade.RichTextBox1 = RichTextBox1
        _shade.mat = perspective
        _shade.createShader()
        time = New Timers.Timer(10)
        AddHandler time.Elapsed, AddressOf glUpdate
        time.Start()
    End Sub

    Public Sub PrepareImage()

        For Each ima In m_model.textures
            GL.BindTexture(TextureTarget.Texture2D, m_model.tex)

            Dim data = ima.LockBits(New System.Drawing.Rectangle(0, 0, ima.Width, ima.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
            ima.UnlockBits(data)

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(TextureMinFilter.Linear))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(TextureMagFilter.Linear))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D)

        Next

    End Sub

    Public Sub UploadBuffers()
        m_model.Vertex = New List(Of Vector3)
        m_model.Normals = New List(Of Vector3)
        m_model.Faces = New List(Of Vector3)
        m_model.Colors = New List(Of Vector4)
        m_model.Uv = New List(Of Vector2)
        m_model.textures = New List(Of Bitmap)
        m_model.materials = New List(Of Material)

        For Each m In m_model.sce.Meshes
            If m.HasVertices = True Then
                For i As Integer = 0 To m.VertexCount - 1
                    Dim vertex As Vector3 = New Vector3
                    vertex.X = m.Vertices(i).X
                    vertex.Y = m.Vertices(i).Y
                    vertex.Z = m.Vertices(i).Z
                    m_model.Vertex.Add(vertex)
                Next i

            End If

            If m.HasNormals = True Then
                If m.HasNormals = True Then
                    For i As Integer = 0 To m.VertexCount - 1
                        Dim normal As Vector3 = New Vector3
                        normal.X = m.Normals(i).X
                        normal.Y = m.Normals(i).Y
                        normal.Z = m.Normals(i).Z
                        m_model.Normals.Add(normal)
                    Next i

                End If

            End If

            If m.HasTextureCoords(0) = True Then
                For i As Integer = 0 To m.VertexCount - 1
                    Dim uv As Vector2 = New Vector2
                    uv.X = m.TextureCoordinateChannels(0)(i).X
                    uv.Y = m.TextureCoordinateChannels(0)(i).Y
                    m_model.Uv.Add(uv)
                Next i

            End If

            If m.HasVertexColors(0) = True Then
                For i As Integer = 0 To m.VertexCount - 1
                    Dim color As Vector4 = New Vector4
                    color.X = m.VertexColorChannels(0)(i).R
                    color.Y = m.VertexColorChannels(0)(i).G
                    color.Z = m.VertexColorChannels(0)(i).B
                    color.W = m.VertexColorChannels(0)(i).A
                    m_model.Colors.Add(color)
                Next


            End If

            If m.HasFaces = True Then
                For i As Integer = 0 To m.FaceCount - 1
                    Dim faces As Vector3 = New Vector3
                    faces.X = m.Faces(i).Indices(0)
                    faces.Y = m.Faces(i).Indices(1)
                    faces.Z = m.Faces(i).Indices(2)
                    m_model.Faces.Add(faces)
                Next i
            End If
        Next


        If m_model.sce.HasMaterials = True Then
            For m As Integer = 0 To m_model.sce.MaterialCount - 1
                m_model.materials.Add(m_model.sce.Materials.ElementAt(m))
            Next

            If m_model.sce.HasTextures = True Then
                m_model.tex = GL.GenTexture()
                For i As Integer = 0 To m_model.sce.TextureCount - 1
                    Dim textt = m_model.sce.Textures.ElementAt(i)
                    If textt.IsCompressed = True Then
                        m_model.textures.Add(util.GenerateBitmap(textt))
                        RichTextBox1.AppendText("image was compressed and saved as bitmap" + vbNewLine)
                    End If
                Next
            End If

        End If

        PrepareImage()

        Label1.Text = "verts =" & m_model.Vertex.Count * 3.ToString
        Label2.Text = "faces =" & m_model.Faces.Count * 3.ToString

    End Sub

    Private Sub dramodel()
        Try
            mp = util.FromMatrix(m_model.sce.RootNode.Transform)
            mp.Transpose()
            GL.PushMatrix()
            GL.MultMatrix(mp)

            For Each mo In m_model.sce.Meshes

                For i As Integer = 0 To mo.FaceCount - 1

                    Dim fac = mo.Faces.ElementAt(i)
                    Dim facm As Graphics.OpenGL.PrimitiveType
                    Select Case fac.IndexCount

                        Case 1
                            facm = Graphics.OpenGL.PrimitiveType.Points
                        Case 2
                            facm = Graphics.OpenGL.PrimitiveType.Lines
                        Case 3
                            facm = Graphics.OpenGL.PrimitiveType.Triangles
                        Case Else
                            facm = Graphics.OpenGL.PrimitiveType.Polygon
                    End Select
                    GL.Begin(facm)

                    For ii As UInteger = 0 To fac.IndexCount - 1
                        Dim index = fac.Indices.ElementAt(ii)
                        If mo.HasVertexColors(0) = True Then
                            GL.Color4(util.FromColor(mo.VertexColorChannels.ElementAt(0).ElementAt(index)))
                        End If
                        GL.TexCoord2(util.FromVector2(mo.TextureCoordinateChannels(0).ElementAt(index)))
                        GL.Vertex3(util.FromVector(mo.Vertices.ElementAt(index)))
                        GL.Normal3(util.FromVector(mo.Normals.ElementAt(index)))
                    Next
                    GL.End()
                Next

            Next
            GL.ActiveTexture(m_model.tex)
            GL.BindTexture(TextureTarget.Texture2D, m_model.tex)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
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

        GL.CullFace(CullFaceMode.Back)

        domatrix()

        dramodel()

        GlControl1.SwapBuffers()

    End Sub

    Public Sub loadmod(loadmodel As String)

        Dim asa As AssimpContext = New AssimpContext()
        asa.SetConfig(New Configs.NormalSmoothingAngleConfig(66.0F))
        m_model = New Model()
        m_model.sce = asa.ImportFile(loadmodel, PostProcessPreset.TargetRealTimeMaximumQuality And PostProcessSteps.Triangulate And PostProcessSteps.FlipUVs AndAlso PostProcessSteps.GenerateSmoothNormals)
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        m_model.Vertex.Clear()
        m_model.Normals.Clear()
        m_model.Faces.Clear()
        m_model.Colors.Clear()
        m_model.Uv.Clear()
        m_model.textures.Clear()
        m_model.materials.Clear()
        m_model = Nothing
    End Sub

    Public Sub domatrix()
        GL.MatrixMode(MatrixMode.Modelview)
        lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0)
        GL.LoadMatrix(lookat)
        GL.Rotate(m_angle, 0.0F, 1.0F, 0.0F)
        Dim tmp = m_sceneMax.X - m_sceneMin.X
        tmp = Math.Max(m_sceneMax.Y - m_sceneMin.Y, tmp)
        tmp = Math.Max(m_sceneMax.Z - m_sceneMin.Z, tmp)
        tmp = 1.0F / tmp
        GL.Scale(tmp * 2, tmp * 2, tmp * 2)
        GL.Translate(-m_sceneCenter)
    End Sub

    Private Sub ComputeBoundingBox()
        m_sceneMin = New Vector3(1.0E+10F, 1.0E+10F, 1.0E+10F)
        m_sceneMax = New Vector3(-1.0E+10F, -1.0E+10F, -1.0E+10F)
        Dim identity As Matrix4 = Matrix4.Identity
        ComputeBoundingBox(m_model.sce.RootNode, m_sceneMin, m_sceneMax, identity)
        m_sceneCenter.X = (m_sceneMin.X + m_sceneMax.X) / 2.0F
        m_sceneCenter.Y = (m_sceneMin.Y + m_sceneMax.Y) / 2.0F
        m_sceneCenter.Z = (m_sceneMin.Z + m_sceneMax.Z) / 2.0F
    End Sub

    Private Sub ComputeBoundingBox(ByVal node As Node, ByRef min As Vector3, ByRef max As Vector3, ByRef trafo As Matrix4)
        Dim prev As Matrix4 = trafo
        trafo = Matrix4.Mult(prev, util.FromMatrix(node.Transform))

        If node.HasMeshes Then

            For Each index As Integer In node.MeshIndices
                Dim mesh As Mesh = m_model.sce.Meshes(index)

                For i As Integer = 0 To mesh.VertexCount - 1
                    Dim tmp As Vector3 = util.FromVector(mesh.Vertices(i))
                    Vector3.TransformPosition(tmp, trafo, tmp)
                    min.X = Math.Min(min.X, tmp.X)
                    min.Y = Math.Min(min.Y, tmp.Y)
                    min.Z = Math.Min(min.Z, tmp.Z)
                    max.X = Math.Max(max.X, tmp.X)
                    max.Y = Math.Max(max.Y, tmp.Y)
                    max.Z = Math.Max(max.Z, tmp.Z)
                Next
            Next
        End If

        For i As Integer = 0 To node.ChildCount - 1
            ComputeBoundingBox(node.Children(i), min, max, trafo)
        Next

        trafo = prev
    End Sub

    Private Sub GlControl1_Resize(sender As Object, e As EventArgs) Handles GlControl1.Resize

        GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height)
        aspectRatio = Convert.ToSingle(GlControl1.Width / GlControl1.Height)
        perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 64)

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(perspective)
    End Sub

End Class
