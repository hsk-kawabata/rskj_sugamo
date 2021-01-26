Public Class frmREPORT
    Inherits System.Windows.Forms.Form

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

    ' Form は dispose をオーバーライドしてコンポーネント一覧を消去します。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    ' Windows フォーム デザイナを使って変更してください。  
    ' コード エディタは使用しないでください。
    Friend WithEvents objAxPowerSORT As AxPowerSORT_Lib.AxPowerSORT
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmREPORT))
        Me.objAxPowerSORT = New AxPowerSORT_Lib.AxPowerSORT
        CType(Me.objAxPowerSORT, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'objAxPowerSORT
        '
        Me.objAxPowerSORT.Enabled = True
        Me.objAxPowerSORT.Location = New System.Drawing.Point(112, 128)
        Me.objAxPowerSORT.Name = "objAxPowerSORT"
        Me.objAxPowerSORT.OcxState = CType(resources.GetObject("objAxPowerSORT.OcxState"), System.Windows.Forms.AxHost.State)
        Me.objAxPowerSORT.Size = New System.Drawing.Size(28, 28)
        Me.objAxPowerSORT.TabIndex = 0
        '
        'frmREPORT
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(292, 273)
        Me.Controls.Add(Me.objAxPowerSORT)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmREPORT"
        Me.Text = "frmREPORT"
        CType(Me.objAxPowerSORT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
