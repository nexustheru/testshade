Imports System.Drawing.Imaging
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Assimp
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class modelimport

    Public Structure Model
        Dim Vertex() As Single
        Dim Faces() As UInteger
        Dim Normals() As Single
        Dim Colors() As Single
        Dim Uv() As Single
        Dim textures As Bitmap
        Dim sce As Scene
        Dim vertexbuffer, vertexobject, facebuffer, faceobject, normalbuffer, normalobject, texcoordbuffer, texcoordobject, colorbuffer, colorobject, tex As Integer

    End Structure
    Public util As utility
    Private m_sceneCenter, m_sceneMin, m_sceneMax As Vector3
    Public m_angle As Single
    Public m_model As Model

    Dim _shader As Shader
    Public matri As Matrix4

    Public Sub New(loadmodel As String)

        Dim asa As AssimpContext = New AssimpContext()
        asa.SetConfig(New Configs.NormalSmoothingAngleConfig(66.0F))
        m_model = New Model()
        m_model.sce = asa.ImportFile(loadmodel, PostProcessPreset.TargetRealTimeMaximumQuality And PostProcessSteps.Triangulate And PostProcessSteps.FlipUVs AndAlso PostProcessSteps.GenerateSmoothNormals)
        util = New utility(m_model.sce)
        UploadBuffers()
        ComputeBoundingBox()

    End Sub

    Public Sub UploadBuffers()
        For Each m In m_model.sce.Meshes
            Dim vertices((m.VertexCount * 3) - 1) As Single
            Dim uv((m.VertexCount * 2) - 1) As Single
            Dim normals((m.VertexCount * 3) - 1) As Single
            Dim color((m.VertexCount * 4) - 1) As Single
            Dim faces((m.FaceCount * 3) - 1) As UInteger

            If m.HasVertices = True Then
                For i As Integer = 0 To m.VertexCount - 1
                    vertices(i * 3) = m.Vertices(i).X
                    vertices(i * 3 + 1) = m.Vertices(i).Y
                    vertices(i * 3 + 2) = m.Vertices(i).Z
                Next i

            End If

            If m.HasNormals = True Then
                If m.HasNormals = True Then
                    For i As Integer = 0 To m.VertexCount - 1
                        normals(i * 3) = m.Normals(i).X
                        normals(i * 3 + 1) = m.Normals(i).Y
                        normals(i * 3 + 2) = m.Normals(i).Z
                    Next i

                End If

            End If

            If m.HasTextureCoords(0) = True Then
                For i As Integer = 0 To m.VertexCount - 1
                    uv(i * 2) = m.TextureCoordinateChannels(0)(i).X
                    uv(i * 2 + 1) = m.TextureCoordinateChannels(0)(i).Y

                Next i

            End If

            If m.HasVertexColors(0) = True Then
                For i As Integer = 0 To m.VertexCount - 1
                    color(i * 4) = m.VertexColorChannels(0)(i).R
                    color(i * 4 + 1) = m.VertexColorChannels(0)(i).G
                    color(i * 4 + 2) = m.VertexColorChannels(0)(i).B
                    color(i * 4 + 3) = m.VertexColorChannels(0)(i).A
                Next


            End If

            If m.HasFaces = True Then
                For i As Integer = 0 To m.FaceCount - 1
                    faces(i * 3) = m.Faces(i).Indices(0)
                    faces(i * 3 + 1) = m.Faces(i).Indices(1)
                    faces(i * 3 + 2) = m.Faces(i).Indices(2)
                Next i

            End If
            m_model.Vertex = vertices
            m_model.Normals = normals
            m_model.Uv = uv
            m_model.Colors = color
            m_model.Faces = faces
            Console.WriteLine("vertex and faces contains= " & vbNewLine & m_model.Vertex.Length.ToString + " " & m_model.Faces.Length.ToString)
        Next
    End Sub

    Public Sub CleanVbo()
        GL.DeleteBuffer(m_model.vertexbuffer)
        GL.DeleteVertexArray(m_model.vertexobject)
        GL.DeleteProgram(_shader.Handle)
        GL.DeleteShader(_shader._vertexObj)
        GL.DeleteShader(_shader._fragObj)


    End Sub

    Public Sub LoadAttributes()

        GL.EnableVertexAttribArray(0)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, False, Vector3.SizeInBytes, 0) 'vertex

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, False, Vector3.SizeInBytes, 0) 'normal
        GL.EnableVertexAttribArray(1)

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, False, Vector2.SizeInBytes, 0) 'tex
        GL.EnableVertexAttribArray(2)

        GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, False, Vector4.SizeInBytes, 0) 'color
        GL.EnableVertexAttribArray(3)


    End Sub

    Public Sub VboVertex()
        If m_model.Vertex.Count > 0 = True Then
            GL.EnableClientState(ArrayCap.VertexArray)
            m_model.vertexbuffer = GL.GenBuffer()

            GL.BindBuffer(BufferTarget.ArrayBuffer, m_model.vertexbuffer)
            GL.BufferData(BufferTarget.ArrayBuffer, m_model.Vertex.Length * 4, m_model.Vertex, BufferUsageHint.StaticDraw)

            m_model.vertexobject = GL.GenVertexArray()
            GL.BindVertexArray(m_model.vertexobject)

        End If
    End Sub '0'

    Public Sub VboNormal()
        If m_model.Normals.Count > 0 = True Then
            GL.EnableClientState(ArrayCap.NormalArray)
            m_model.normalbuffer = GL.GenBuffer() '2
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_model.normalbuffer)
            GL.BufferData(BufferTarget.ArrayBuffer, m_model.Normals.Length * 4, m_model.Normals, BufferUsageHint.StaticDraw)

            GL.BindVertexArray(m_model.vertexobject)

            ' GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_model.normalbuffer)
        End If
    End Sub '1'

    Public Sub VboTexcoord()
        If m_model.Uv.Count > 0 = True = True Then
            GL.EnableClientState(ArrayCap.TextureCoordArray)
            m_model.texcoordbuffer = GL.GenBuffer() '3
            GL.BindBuffer(BufferTarget.TextureBuffer, m_model.texcoordbuffer)
            GL.BufferData(BufferTarget.TextureBuffer, m_model.Vertex.Length * 2, m_model.Uv, BufferUsageHint.StaticDraw)

            GL.BindVertexArray(m_model.vertexobject)

            'GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_model.texcoordbuffer)
        End If
    End Sub '2'

    Public Sub VboColor()
        If m_model.Colors.Count > 0 = True Then
            GL.EnableClientState(ArrayCap.ColorArray)
            m_model.colorbuffer = GL.GenBuffer() '3
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_model.colorbuffer)
            GL.BufferData(BufferTarget.ArrayBuffer, m_model.Colors.Length * 4, m_model.Colors, BufferUsageHint.StaticDraw)

            GL.BindVertexArray(m_model.vertexobject)

            ' GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_model.colorbuffer)
        End If
    End Sub '3'

    Public Sub VboFace()
        If m_model.Faces.Count > 0 = True Then
            m_model.facebuffer = GL.GenBuffer() '4
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_model.facebuffer)
            GL.BufferData(BufferTarget.ElementArrayBuffer, m_model.Faces.Length * 4, m_model.Faces, BufferUsageHint.StaticDraw)
            'GL.BindVertexArray(0) ''should be here?
            'GL.BindVertexArray(m_model.vertexobject)
            'GL.VertexAttribPointer(4, 3, VertexAttribPointerType.UnsignedInt, False, Vector3.SizeInBytes, 0)
            'GL.EnableVertexAttribArray(4)
        End If
    End Sub '4'

    Public Sub VboTexture()
        If m_model.sce.HasMaterials = True Then
            GL.Enable(EnableCap.Texture2D)
            'GL.Enable(EnableCap.TextureCoordArray)
            m_model.tex = GL.GenTexture()
            For i As Integer = 0 To m_model.sce.TextureCount - 1
                Dim textt = m_model.sce.Textures.ElementAt(i)
                If textt.IsCompressed = True Then
                    Dim img As Bitmap = New Bitmap(New MemoryStream(textt.CompressedData))
                    util.loadimageFromCompressed(img, m_model.tex)
                    ' Else
                    ' util.loadimageFromNonCompressed(textt.NonCompressedData, textt.NonCompressedDataSize)

                End If

                'loadimage(tslot.FilePath)
            Next
        Else
            Console.WriteLine("found no texture")
        End If


    End Sub

    Public Sub loadVbo()
        _shader = New Shader("vert.vert", "frag.frag")
        getShadermatrix(matri)

        VboVertex()

        'VboColor()
        'VboTexcoord()
        'VboNormal()
        VboFace()
        'VboTexture()


    End Sub

    Public Sub getShadermatrix(mat As Matrix4)
        For Each posi In _shader.locations
            If posi.Key = "perspective" Then
                _shader.setMatrix(mat, posi.Value)
            End If
        Next

    End Sub

    Public Sub drawVbo()

        Dim ma = util.FromMatrix(m_model.sce.RootNode.Transform)
        ma.Transpose()
        GL.PushMatrix()
        GL.MultMatrix(ma)

        GL.DrawElements(OpenGL.PrimitiveType.Triangles, m_model.Faces.Length * 4, DrawElementsType.UnsignedInt, 0)
        ' GL.BindVertexArray(0) ''should be here?
    End Sub 'draw elementvbo


    Public Sub domodel()
        Try
            Dim fcount As Integer = 0
            Dim m = util.FromMatrix(m_model.sce.RootNode.Transform)
            m.Transpose()
            GL.PushMatrix()
            GL.MultMatrix(m)

            For Each mo In m_model.sce.Meshes

                Dim culor = mo.HasVertexColors(0)
                For i As Integer = 0 To mo.FaceCount - 1

                    Dim fac = mo.Faces.ElementAt(i)
                    Select Case fac.IndexCount
                        Case 3
                            GL.Begin(Graphics.OpenGL.PrimitiveType.Triangles)
                            fcount = 3
                        Case 4
                            GL.Begin(Graphics.OpenGL.PrimitiveType.Polygon)
                            fcount = 4
                    End Select

                    For ii = 0 To fcount - 1
                        Dim index = fac.Indices.ElementAt(ii)
                        If culor = True Then
                            GL.Color4(util.FromColor(mo.VertexColorChannels.ElementAt(0).ElementAt(index)))
                        End If
                        GL.TexCoord2(mo.TextureCoordinateChannels(0).ElementAt(index).X, mo.TextureCoordinateChannels(0).ElementAt(index).Y)
                        GL.Vertex3(util.FromVector(mo.Vertices.ElementAt(index)))
                        GL.Normal3(util.FromVector(mo.Normals.ElementAt(index)))

                    Next
                    GL.End()
                Next

                'If sce.HasMaterials = True Then

                '    For i As Integer = 0 To sce.TextureCount - 1
                '        Dim textt = sce.Textures.ElementAt(i)
                '        If textt.IsCompressed = True Then
                '            Dim img As Bitmap = New Bitmap(New MemoryStream(textt.CompressedData))
                '            util.loadimageFromCompressed(img, tex)
                '        Else
                '            loadimageFromNonCompressed(textt.NonCompressedData, textt.NonCompressedDataSize)

                '        End If

                '        'loadimage(tslot.FilePath)
                '    Next

                'End If

            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Sub 'manual per vertex drawing

    Public Sub domatrix()
        GL.MatrixMode(MatrixMode.Modelview)
        Dim lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0)
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

End Class
