Imports System.Drawing.Imaging
Imports System.IO
Imports Assimp
Imports OpenTK
Imports OpenTK.Graphics.OpenGL

Public Class utility

    Public Function FromVector(ByVal vec As Assimp.Vector3D) As Vector3
        Dim v As Vector3 = New Vector3(vec.X, vec.Y, vec.Z)
        Return v
    End Function
    Public Function FromVector2(ByVal vec As Assimp.Vector3D) As Vector2
        Dim v As Vector2 = New Vector2(vec.X, vec.Y)
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

    Public Function invertNormal(ByVal norm As Vector3) As Vector3

    End Function

    Public Function TexelToColor(ByVal tex As Texel) As Color
        Dim col = Color.FromArgb(tex.R, tex.G, tex.B, tex.A)
        Return col
    End Function

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

    Public Function GenerateBitmap(ByVal compressed As EmbeddedTexture) As Bitmap
        Dim img As Bitmap = New Bitmap(New MemoryStream(compressed.CompressedData))
        Return img
    End Function

    Public Function loadimageFromModel(ByVal bitmap As Bitmap, tex As Integer) As Integer
        GL.Enable(EnableCap.Texture2D)

        GL.BindTexture(TextureTarget.Texture2D, tex)

        Dim data = bitmap.LockBits(New System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
        bitmap.UnlockBits(data)

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(TextureMagFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat))

        GL.ActiveTexture(tex)
        GL.BindTexture(TextureTarget.Texture2D, tex)
        Return tex
    End Function
End Class
