Imports System
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

''' <summary>
''' 取引先ツリービュー部品化クラス
''' </summary>
''' <remarks>2016/10/06 saitou 八十二銀行 added for 取引先マスタ画面変更対応</remarks>
Public Class GakkouTreeView

#Region "クラス変数"

    ''' <summary>
    ''' 学校コードテキストボックスプロパティ
    ''' </summary>
    ''' <value>フォーム設定取引先主コードテキストボックス</value>
    ''' <returns>取引先主コードテキストボックス</returns>
    ''' <remarks>コントロール配置時のインスタンス生成でフォームに設定されている取引先主コードテキストボックスを設定する</remarks>
    Public Property ITV_GAKKOU_CODE_TEXTBOX As TextBox
        Get
            Return txtGAKKOU_CODE
        End Get
        Set(value As TextBox)
            txtGAKKOU_CODE = value
        End Set
    End Property
    Private txtGAKKOU_CODE As TextBox

    ''' <summary>
    ''' 参照ボタンプロパティ
    ''' </summary>
    ''' <value>フォーム設定参照ボタン</value>
    ''' <returns>参照ボタン</returns>
    ''' <remarks>コントロール配置時のインスタンス生成でフォームに設定されている参照ボタンを設定する</remarks>
    Public Property ITV_SELECT_BUTTON As Button
        Get
            Return btnSELECT
        End Get
        Set(value As Button)
            btnSELECT = value
        End Set
    End Property
    Private btnSELECT As Button

    ''' <summary>
    ''' 取消ボタンプロパティ
    ''' </summary>
    ''' <value>フォーム設定参照ボタン</value>
    ''' <returns>取消参照ボタン</returns>
    ''' <remarks>コントロール配置時のインスタンス生成でフォームに設定されている取消ボタンを設定する</remarks>
    Public Property ITV_CANCEL_BUTTON As Button
        Get
            Return btnCANCEL
        End Get
        Set(value As Button)
            btnCANCEL = value
        End Set
    End Property
    Private btnCANCEL As Button

    ''' <summary>
    ''' Ｎｅｘｔフォーカスプロパティ
    ''' </summary>
    ''' <value>次フォーカステキストボックス</value>
    ''' <returns>コントロール</returns>
    ''' <remarks>ツリービューのダブルクリック後の移動するフォーカスを指定する</remarks>
    Public Property ITV_NEXT_FOCUS As Control
        Get
            Return txtNEXT_FOCUS
        End Get
        Set(value As Control)
            txtNEXT_FOCUS = value
        End Set
    End Property
    Private txtNEXT_FOCUS As Control
    '2017/03/08 タスク）西野 CHG 飯田信金 カスタマイズ対応(機能改善) -------------------- START
    'テキストボックスしか指定できないのでコントロール全て選択できるように変更
    'Public Property ITV_NEXT_FOCUS As TextBox
    '    Get
    '        Return txtNEXT_FOCUS
    '    End Get
    '    Set(value As TextBox)
    '        txtNEXT_FOCUS = value
    '    End Set
    'End Property
    'Private txtNEXT_FOCUS As TextBox
    '2017/03/08 タスク）西野 CHG 飯田信金 カスタマイズ対応(機能改善) -------------------- END

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' カスタムツリービューロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GakkouTreeView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '----------------------------------------
        'ツリービューに追加
        '----------------------------------------
        Call ControlInitializa()
    End Sub

    ''' <summary>
    ''' 参照ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CmdCIFForm_Click(sender As Object, e As EventArgs) Handles CmdCIFForm.Click
        '----------------------------------------
        'ツリービューに追加
        '----------------------------------------
        Call ControlInitializa(False)
    End Sub

    ''' <summary>
    ''' クリアボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CmdCIFClear_Click(sender As Object, e As EventArgs) Handles CmdCIFClear.Click
        '----------------------------------------
        'ツリービューに追加
        '----------------------------------------
        TextBoxItakuCode.Text = ""
        TextBoxItakuKName.Text = ""
        Call ControlInitializa()
        TextBoxItakuKName.Focus()
    End Sub

    ''' <summary>
    ''' ノードマウスダブルクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TreeView_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs) Handles TreeView.NodeMouseDoubleClick
        Try
            'ダブルクリックされたノードから取引先コードを取得する
            Dim NodeGakkouCode As String = String.Empty
            NodeGakkouCode = e.Node.Name

            If NodeGakkouCode.Trim = String.Empty Then
                'ノードキーが入っていない場合は親ノードクリックと判断する
            Else
                '取消ボタン押下イベント実行
                If Not Me.ITV_CANCEL_BUTTON Is Nothing Then
                    Me.ITV_CANCEL_BUTTON.PerformClick()
                End If

                '画面の取引先コードにノードキーの取引先コードを設定
                Me.ITV_GAKKOU_CODE_TEXTBOX.Text = NodeGakkouCode
                '2017/03/08 タスク）西野 ADD 飯田信金 カスタマイズ対応(機能改善) -------------------- START
                '学校コードのValidatingが動かないのでフォーカスを当てる
                If Not Me.ITV_GAKKOU_CODE_TEXTBOX Is Nothing Then
                    Me.ITV_GAKKOU_CODE_TEXTBOX.Focus()
                End If
                '2017/03/08 タスク）西野 ADD 飯田信金 カスタマイズ対応(機能改善) -------------------- END

                '参照ボタン押下イベント実行
                If Not Me.ITV_SELECT_BUTTON Is Nothing Then
                    Me.ITV_SELECT_BUTTON.PerformClick()
                End If

                If Not Me.ITV_NEXT_FOCUS Is Nothing Then
                    Me.ITV_NEXT_FOCUS.Focus()
                End If

            End If

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' 委託者名カナバリデイティングイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBoxItakuKName_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextBoxItakuKName.Validating
        Call NzCheckString(CType(sender, TextBox))
        CmdCIFForm.Focus()
    End Sub

    ''' <summary>
    ''' 委託者コードバリデイティングイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBoxItakuCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextBoxItakuCode.Validating
        Call NzNumberString(CType(sender, TextBox), True)
        CmdCIFForm.Focus()
    End Sub

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 取引先ツリービューのコントロールを初期化します。
    ''' </summary>
    ''' <param name="ClearItakuText">委託者名カナ検索テキストボックスの初期化フラグ（デフォルト：True）</param>
    ''' <remarks></remarks>
    Public Sub ControlInitializa(Optional ClearItakuText As Boolean = True)
        '----------------------------------------
        'ツリービューの内容をクリア
        '----------------------------------------
        Me.TreeView.Nodes.Clear()

        '委託者検索　カナテキストをクリアする設定の場合はクリア
        If ClearItakuText = True Then
            Me.TextBoxItakuKName.Clear()
        End If

        '----------------------------------------
        'ツリービュー設定
        '----------------------------------------
        Me.SetGakkouTreeView()

    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 取引先ツリービューを設定します。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function SetGakkouTreeView() As Boolean
        Dim DB As CASTCommon.MyOracle = Nothing
        Dim DBReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim NodeKey As String = String.Empty
            Dim NodeCode As String = String.Empty
            Dim NodeText As String = String.Empty

            DB = New CASTCommon.MyOracle
            DBReader = New CASTCommon.MyOracleReader(DB)

            If DBReader.DataReader(Me.GetGakkouTreeViewSQL) = True Then
                While DBReader.EOF = False

                    '配下委託者のキーと表示項目設定
                    NodeCode = DBReader.GetString("GAKKOU_CODE_G")
                    NodeText = String.Concat(New String() {DBReader.GetString("ITAKU_CODE_T"), " (", DBReader.GetString("GAKKOU_NNAME_G"), ")"})

                    '代表委託者のノードに取引先コードをキーとして追加
                    TreeView.Nodes.Add(NodeCode, NodeText)

                    DBReader.NextRead()
                End While

            End If

            Return True

        Catch ex As Exception
            Return False

        Finally
            If Not DBReader Is Nothing Then
                DBReader.Close()
                DBReader = Nothing
            End If

            If Not DB Is Nothing Then
                DB.Close()
                DB = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 取引先ツリービューを設定するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetGakkouTreeViewSQL() As System.Text.StringBuilder
        Dim SQL As New System.Text.StringBuilder

        With SQL
            .Append("select")
            .Append("     GAKKOU_CODE_G")
            .Append("   , ITAKU_CODE_T")
            .Append("   , GAKKOU_NNAME_G")
            .Append("   , GAKKOU_KNAME_G")
            .Append(" from")
            .Append("     GAKMAST1 , GAKMAST2")
            .Append(" where")
            .Append("     GAKKOU_CODE_G = GAKKOU_CODE_T")
            If Me.TextBoxItakuCode.Text.Trim <> "" Then
                .Append(" and ITAKU_CODE_T = '" & Me.TextBoxItakuCode.Text & "'")
            Else
                If Me.TextBoxItakuKName.Text.Trim <> "" Then
                    .Append(" and GAKKOU_KNAME_G like '%")
                    .Append(Me.TextBoxItakuKName.Text.Trim)
                    .Append("%'")
                End If
            End If
            .Append(" group by")
            .Append("     GAKKOU_CODE_G")
            .Append("   , ITAKU_CODE_T")
            .Append("   , GAKKOU_NNAME_G")
            .Append("   , GAKKOU_KNAME_G")
            .Append(" order by")
            .Append("    GAKKOU_KNAME_G")
        End With

        Return SQL
    End Function

    Private Sub NzCheckString(ByVal avTextBox As TextBox)
        Dim Ret As String = ""
        Try
            With avTextBox
                If Not IsNothing(.Text) Then

                    Dim Temp As String = StrConv(.Text, VbStrConv.Narrow)

                    For Idx As Integer = 0 To Temp.Length - 1 Step 1

                        Ret &= GetLimitString(Temp.Substring(Idx, 1), 1)
                    Next Idx
                End If
            End With
        Catch ex As Exception

        Finally
            avTextBox.Text = Ret
        End Try
    End Sub

    Private Function GetLimitString(ByVal avTargetData As String, ByVal avLength As Integer) As String
        Try
            Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")
            Dim onByte() As Byte = JISEncoding.GetBytes(avTargetData)

            Do While onByte.Length > avLength
                avTargetData = avTargetData.Substring(0, avTargetData.Length - 1)
                onByte = JISEncoding.GetBytes(avTargetData)
            Loop
            Return avTargetData
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' 機　能 : TextBoxの整数値を評価する
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 評価対象値
    ' 　　　   ARG2 - Format する／しない
    '
    ' 備　考 : 画面で使用
    '    
    Private Sub NzNumberString(ByVal avTextBox As TextBox, Optional ByVal avSEL As Boolean = False)
        With avTextBox
            If IsNothing(.Text) OrElse Not IsNumber(.Text) Then
                .Text = ""
            Else
                Select Case avSEL
                    Case True
                        Dim Temp As String = New String("0"c, .MaxLength)
                        .Text = String.Format("{0:" & Temp & "}", NzDec(.Text))
                    Case Else
                        .Text = NzDec(.Text).ToString
                End Select
            End If
        End With
    End Sub
    '
    ' 機能　 ： 数値評価
    '
    ' 引数　 ： ARG1 - 対象値
    '
    ' 戻り値 ： 数値 = True
    '
    ' 備考　 ： なし
    '
    Private Function IsNumber(ByVal avValue As Object) As Boolean
        Return (New System.Text.RegularExpressions.Regex("^[-]*\d+$")).IsMatch(avValue.ToString)
    End Function
    '
    ' 機　能 : 数値を評価する
    '
    ' 戻り値 : 数値
    '
    ' 引き数 : ARG1 - 評価対象値
    '
    ' 備　考 : テキスト処理系関数
    '    
    Private Function NzDec(ByVal avValue As Object) As Decimal
        Dim Ret As Decimal = 0
        If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) AndAlso IsNumber(avValue) Then

            Ret = CType(avValue, Decimal)
        End If
        Return Ret
    End Function

#End Region

End Class
