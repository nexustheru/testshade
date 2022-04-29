Imports System.Runtime.InteropServices
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class renderer
    Inherits OpenTK.GameWindow

    Dim mof As modelimport
    Dim perspective As Matrix4


    Protected Overrides Sub OnRenderFrame(e As FrameEventArgs)
        Try

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
            'GL.CullFace(CullFaceMode.Back)

            mof.domatrix()

            If IsNothing(mof) = False Then
                mof.drawVbo()
                'mof.domodel()
            End If

            SwapBuffers()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        MyBase.OnRenderFrame(e)

    End Sub
    Public Sub New(width As Integer, height As Integer, title As String)
        Me.Width = width
        Me.Height = height
        Me.Title = title
        mof = New modelimport("ball.fbx")
        mof.matri = perspective
    End Sub
    Protected Overrides Sub OnUnload(e As EventArgs)
        mof.CleanVbo()

        MyBase.OnUnload(e)
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
        GL.Viewport(0, 0, Width, Height)
        Dim aspectRatio = Convert.ToSingle(Width / Height)
        perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 64)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(perspective)
        MyBase.OnResize(e)
    End Sub
    Protected Overrides Sub OnLoad(e As EventArgs)
        Try

            mof.loadVbo()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        MyBase.OnLoad(e)
    End Sub
    Protected Overrides Sub OnUpdateFrame(e As FrameEventArgs)
        MyBase.OnUpdateFrame(e)
        mof.m_angle += 25.0F * Convert.ToSingle(e.Time)
        If mof.m_angle > 360 Then

            mof.m_angle = 0.0F
        End If
    End Sub

End Class
