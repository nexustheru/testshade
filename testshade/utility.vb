Imports System.Drawing.Imaging
Imports System.IO
Imports System.Reflection
Imports Assimp
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Public Class utility
    Dim sce As Assimp.Scene
    Public Sub New(ByVal scene As Assimp.Scene)
        sce = scene
    End Sub

    Public Function FromMatrix(ByVal mat As Matrix4x4) As Matrix4
        Dim m As Matrix4 = New Matrix4()
        m.M11 = mat.A1
        m.M12 = mat.A2
        m.M13 = mat.A3
        m.M14 = mat.A4
        m.M21 = mat.B1
        m.M22 = mat.B2
        m.M23 = mat.B3
        m.M24 = mat.B4
        m.M31 = mat.C1
        m.M32 = mat.C2
        m.M33 = mat.C3
        m.M34 = mat.C4
        m.M41 = mat.D1
        m.M42 = mat.D2
        m.M43 = mat.D3
        m.M44 = mat.D4
        Return m
    End Function

    Public Function FromVectorUv(ByVal vec As Assimp.Vector3D) As Vector2
        Dim v As Vector2 = New Vector2(vec.X, vec.Y)
        Return v
    End Function

    Public Function FromVector(ByVal vec As Assimp.Vector3D) As Vector3
        Dim v As Vector3 = New Vector3(vec.X, vec.Y, vec.Z)
        Return v
    End Function

    Public Function FromColor(ByVal color As Color4D) As OpenTK.Graphics.Color4
        Dim c As Graphics.Color4
        c.R = color.R
        c.G = color.G
        c.B = color.B
        c.A = color.A
        Return c
    End Function

    Public Function TexelToColor(ByVal tex As Texel) As Color
        Dim col = Color.FromArgb(tex.R, tex.G, tex.B, tex.A)
        Return col
    End Function

    Public Function loadimageFromNonCompressed(ByVal pixels() As Texel, ByVal size As Integer, tex As Integer) As Integer
        Dim bitmap As Bitmap = New Bitmap(500, 500)
        For i As Integer = 0 To size - 1
            Dim tx As Texel = pixels.ElementAt(i)
            For h As Integer = 0 To bitmap.Height - 1
                bitmap.SetPixel(i, h, TexelToColor(tx))
                For w As Integer = 0 To bitmap.Width - 1
                    bitmap.SetPixel(i, w, TexelToColor(tx))
                Next
            Next
        Next

        bitmap.Save("lol.png")
        tex = GL.GenTexture()
        GL.BindTexture(TextureTarget.Texture2D, tex)

        Dim data = bitmap.LockBits(New System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
        bitmap.UnlockBits(data)

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(TextureMagFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))

        Return tex
    End Function

    Public Function loadimage(ByVal filename As String, tex As Integer) As Integer
        Dim bitmap = New Bitmap(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename))
        If File.Exists(filename) = True Then
            tex = GL.GenTexture()
            GL.BindTexture(TextureTarget.Texture2D, tex)

            Dim data = bitmap.LockBits(New System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
            bitmap.UnlockBits(data)

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(TextureMinFilter.Linear))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(TextureMagFilter.Linear))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))

            Return tex
        End If
        Return 0
    End Function

    Public Function loadimageFromCompressed(ByVal bitmap As Bitmap, tex As Integer) As Integer
        tex = GL.GenTexture()

        GL.BindTexture(TextureTarget.Texture2D, tex)

        Dim data = bitmap.LockBits(New System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
        bitmap.UnlockBits(data)

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(TextureMagFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))
        GL.ActiveTexture(tex)
        Return tex
    End Function

End Class
