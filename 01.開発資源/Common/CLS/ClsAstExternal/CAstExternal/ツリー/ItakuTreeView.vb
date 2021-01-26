Imports System
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

''' <summary>
''' 取引先ツリービュー部品化クラス
''' </summary>
''' <remarks>2016/10/06 saitou 八十二銀行 added for 取引先マスタ画面変更対応</remarks>
Public Class ItakuTreeView

#Region "クラス変数"

    ''' <summary>
    ''' 処理区分プロパティ
    ''' </summary>
    ''' <value>1 - 口振 , 3 - 総振</value>
    ''' <returns>設定処理区分</returns>
    ''' <remarks>コントロール配置時のインスタンス生成で処理区分を文字列で設定する</remarks>
    Public Property ITV_FSYORI_KBN As String
        Get
            Return strFSYORI_KBN
        End Get
        Set(value As String)
            strFSYORI_KBN = value
        End Set
    End Property
    Private strFSYORI_KBN As String

    ''' <summary>
    ''' 表示順プロパティ
    ''' </summary>
    ''' <value>0 - 取引先コード順 , 1 - 委託者コード順 , 2 - 委託者カナ名順</value>
    ''' <returns>表示順区分</returns>
    ''' <remarks>コントロール配置時のインスタンス生成で処理区分を文字列で設定する</remarks>
    Public Property ITV_SORT_PATN As String
        Get
            Return strSORT_PATN
        End Get
        Set(value As String)
            strSORT_PATN = value
        End Set
    End Property
    Private strSORT_PATN As String

    ''' <summary>
    ''' 取引先主コードテキストボックスプロパティ
    ''' </summary>
    ''' <value>フォーム設定取引先主コードテキストボックス</value>
    ''' <returns>取引先主コードテキストボックス</returns>
    ''' <remarks>コントロール配置時のインスタンス生成でフォームに設定されている取引先主コードテキストボックスを設定する</remarks>
    Public Property ITV_TORIS_CODE_TEXTBOX As TextBox
        Get
            Return txtTORIS_CODE
        End Get
        Set(value As TextBox)
            txtTORIS_CODE = value
        End Set
    End Property
    Private txtTORIS_CODE As TextBox

    ''' <summary>
    ''' 取引先副コードテキストボックスプロパティ
    ''' </summary>
    ''' <value>フォーム設定取引先副コードテキストボックス</value>
    ''' <returns>取引先副コードテキストボックス</returns>
    ''' <remarks>コントロール配置時のインスタンス生成でフォームに設定されている取引先副コードテキストボックスを設定する</remarks>
    Public Property ITV_TORIF_CODE_TEXTBOX As TextBox
        Get
            Return txtTORIF_CODE
        End Get
        Set(value As TextBox)
            txtTORIF_CODE = value
        End Set
    End Property
    Private txtTORIF_CODE As TextBox

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
    ''' <returns>テキストボックス</returns>
    ''' <remarks>ツリービューのダブルクリック後の移動するフォーカスを指定する</remarks>
    Public Property ITV_NEXT_FOCUS As TextBox
        Get
            Return txtNEXT_FOCUS
        End Get
        Set(value As TextBox)
            txtNEXT_FOCUS = value
        End Set
    End Property
    Private txtNEXT_FOCUS As TextBox

    ''' <summary>
    ''' Ｎｅｘｔフォーカスの次のフォーカスプロパティ
    ''' </summary>
    ''' <value>次フォーカステキストボックス</value>
    ''' <returns>テキストボックス</returns>
    ''' <remarks>ツリービューのダブルクリック後の移動するフォーカスを指定する</remarks>
    Public Property ITV_NEXT_FOCUS_BUTTON As Button
        Get
            Return btnNEXT_FOCUS
        End Get
        Set(value As Button)
            btnNEXT_FOCUS = value
        End Set
    End Property
    Private btnNEXT_FOCUS As Button

    ''' <summary>
    ''' 追加ＳＱＬプロパティ
    ''' </summary>
    ''' <value>追加ＳＱＬ</value>
    ''' <returns>文字列変数</returns>
    ''' <remarks>ツリービューの表示追加条件を指定する</remarks>
    Public Property ITV_ADD_SQL As String
        Get
            Return strADD_SQL
        End Get
        Set(value As String)
            strADD_SQL = value
        End Set
    End Property
    Private strADD_SQL As String

    '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
    Private MainLOG As New CASTCommon.BatchLOG("ItakuTreeView", "取引先ツリービュー部品化クラス")
    '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END
#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' カスタムツリービューロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ItakuTreeView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
        MainLOG.Write("", "", "", "(ロード)開始", "成功", "")

        strADD_SQL = String.Empty
        '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END

        '----------------------------------------
        'ツリービューに追加
        '----------------------------------------
        Call ControlInitializa()

        '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
        MainLOG.Write("", "", "", "(ロード)終了", "成功", "")
        '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END
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
        '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
        TextBoxTenpoCode.Text = ""
        '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END
        Call ControlInitializa()
        TextBoxItakuKName.Focus()
    End Sub

    ''' <summary>
    ''' ノードマウスダブルクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TreeView1_NodeMouseDoubleClick(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs) Handles TreeView.NodeMouseDoubleClick
        Try
            'ダブルクリックされたノードから取引先コードを取得する
            Dim NodeToriCode As String = String.Empty
            NodeToriCode = e.Node.Name

            If NodeToriCode.Trim = String.Empty Then
                'ノードキーが入っていない場合は親ノードクリックと判断する
            Else
                '子ノードクリック
                Dim NodeTorisCode As String = String.Empty
                Dim NodeTorifCode As String = String.Empty

                NodeTorisCode = NodeToriCode.Substring(0, 10)
                NodeTorifCode = NodeToriCode.Substring(10)

                '取消ボタン押下イベント実行
                If Not Me.ITV_CANCEL_BUTTON Is Nothing Then
                    Me.ITV_CANCEL_BUTTON.PerformClick()
                End If

                '画面の取引先コードにノードキーの取引先コードを設定
                Me.ITV_TORIS_CODE_TEXTBOX.Text = NodeTorisCode
                Me.ITV_TORIF_CODE_TEXTBOX.Text = NodeTorifCode

                '2017/03/09 タスク）西野 ADD 飯田信金 カスタマイズ対応(機能改善) -------------------- START
                '取引先コードのValidatingが動かないのでフォーカスを当てる
                If Not Me.ITV_TORIS_CODE_TEXTBOX Is Nothing Then
                    Me.ITV_TORIS_CODE_TEXTBOX.Focus()
                End If
                '2017/03/09 タスク）西野 ADD 飯田信金 カスタマイズ対応(機能改善) -------------------- END

                '参照ボタン押下イベント実行
                If Not Me.ITV_SELECT_BUTTON Is Nothing Then
                    Me.ITV_SELECT_BUTTON.PerformClick()
                End If

                If Not Me.ITV_NEXT_FOCUS Is Nothing Then
                    Me.ITV_NEXT_FOCUS.Focus()
                End If

                If Not Me.ITV_NEXT_FOCUS_BUTTON Is Nothing Then
                    Me.ITV_NEXT_FOCUS_BUTTON.Focus()
                End If
            End If

        Catch ex As Exception
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
            MainLOG.Write("", "", "", "(ノードマウスダブルクリック)", "失敗", ex.Message)
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END

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
        '2017/10/17 タスク）日比野 DEL 青梅信金(UI.5-1<PG>) ---------------------------------------- START
        'Call GCom.NzNumberString(CType(sender, TextBox), True)
        '2017/10/17 タスク）日比野 DEL 青梅信金(UI.5-1<PG>) ---------------------------------------- END
        CmdCIFForm.Focus()
    End Sub

    '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
    ''' <summary>
    ''' 店舗コードバリデイティングイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBoxTenpoCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TextBoxTenpoCode.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        CmdCIFForm.Focus()
    End Sub


    ''' <summary>
    ''' 店舗キープレスイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBoxTenpoCode_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxTenpoCode.KeyPress
        Try
            Select Microsoft.VisualBasic.Asc(e.KeyChar)
                Case Keys.Return                            'Enterキー
                    CmdCIFForm.Focus()                      'フォーカス移動
            End Select
        Catch ex As Exception
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
            MainLOG.Write("", "", "", "(店舗キープレス)", "失敗", ex.Message)
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END
        Finally
        End Try
    End Sub
    '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END

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
        Me.SetItakuTreeView()

    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 取引先ツリービューを設定します。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function SetItakuTreeView() As Boolean
        Dim DB As CASTCommon.MyOracle = Nothing
        Dim DBReader As CASTCommon.MyOracleReader = Nothing

        Try

            Dim NodeKey As String = String.Empty
            Dim NodeCode As String = String.Empty
            Dim NodeText As String = String.Empty

            DB = New CASTCommon.MyOracle
            DBReader = New CASTCommon.MyOracleReader(DB)
            If DBReader.DataReader(Me.GetItakuTreeViewSQL) = True Then
                While DBReader.EOF = False

                    '配下委託者のキーと表示項目設定
                    NodeCode = DBReader.GetString("TORIS_CODE_T") & DBReader.GetString("TORIF_CODE_T")
                    If Me.ITV_FSYORI_KBN = "1" Then
                        If DBReader.GetString("SFURI_KBN_T") = "0" Then
                            NodeText = String.Concat(New String() {DBReader.GetString("ITAKU_CODE_T"), " (", DBReader.GetString("ITAKU_NNAME_T"), ")"})
                        Else
                            NodeText = String.Concat(New String() {DBReader.GetString("ITAKU_CODE_T"), " (", DBReader.GetString("ITAKU_NNAME_T"), ") <再振>"})
                        End If
                    Else
                        Select Case DBReader.GetString("SYUBETU_T")
                            Case "21"
                                NodeText = String.Concat(New String() {DBReader.GetString("ITAKU_CODE_T"), " (", DBReader.GetString("ITAKU_NNAME_T"), ") <総振>"})
                            Case "11"
                                NodeText = String.Concat(New String() {DBReader.GetString("ITAKU_CODE_T"), " (", DBReader.GetString("ITAKU_NNAME_T"), ") <給与>"})
                            Case "12"
                                NodeText = String.Concat(New String() {DBReader.GetString("ITAKU_CODE_T"), " (", DBReader.GetString("ITAKU_NNAME_T"), ") <賞与>"})
                            Case Else
                                NodeText = String.Concat(New String() {DBReader.GetString("ITAKU_CODE_T"), " (", DBReader.GetString("ITAKU_NNAME_T"), ") <口振>"})
                        End Select
                    End If

                    '代表委託者のノードに取引先コードをキーとして追加
                    TreeView.Nodes.Add(NodeCode, NodeText)

                    DBReader.NextRead()
                End While
            End If

            Return True

        Catch ex As Exception
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
            MainLOG.Write("", "", "", "(取引先ツリービュー設定)", "失敗", ex.Message)
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END
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
    Private Function GetItakuTreeViewSQL() As System.Text.StringBuilder
        Dim SQL As New System.Text.StringBuilder

        With SQL
            .Append("select")
            .Append("      TORIS_CODE_T")
            .Append("    , TORIF_CODE_T")
            .Append("    , ITAKU_CODE_T")
            .Append("    , ITAKU_NNAME_T")
            If Me.ITV_FSYORI_KBN = "1" Then
                .Append(", SFURI_KBN_T")
            Else
                .Append(", SYUBETU_T")
            End If
            .Append(" from")
            If Me.ITV_FSYORI_KBN = "1" Then
                .Append(" TORIMAST_VIEW2")
            Else
                .Append(" S_TORIMAST")
            End If
            .Append(" where BAITAI_CODE_T <> '15'")
            If strADD_SQL.Trim <> "" Then
                .Append(" and " & strADD_SQL)
            End If
            '2017/10/17 タスク）日比野 CHG 青梅信金(UI.5-1<PG>) ---------------------------------------- START
            'If Me.TextBoxItakuCode.Text.Trim <> "" Then
            '    .Append(" and ITAKU_CODE_T = '" & Me.TextBoxItakuCode.Text & "'")
            'Else
            '    If Me.TextBoxItakuKName.Text.Trim <> "" Then
            '        .Append(" and ITAKU_KNAME_T like '%")
            '        .Append(Me.TextBoxItakuKName.Text.Trim)
            '        .Append("%'")
            '    End If
            'End If
            If Me.TextBoxItakuKName.Text.Trim <> "" Then
                .Append(" and ITAKU_KNAME_T like '%")
                .Append(Me.TextBoxItakuKName.Text.Trim)
                .Append("%'")                                       '部分一致
            End If
            If Me.TextBoxItakuCode.Text.Trim <> "" Then
                .Append(" and ITAKU_CODE_T LIKE '")
                .Append(Me.TextBoxItakuCode.Text)
                .Append("%'")                                       '前方一致
            End If
            If Me.TextBoxTenpoCode.Text.Trim <> "" Then
                .Append(" and TSIT_NO_T = '")
                .Append(Me.TextBoxTenpoCode.Text.Trim)
                .Append("'")                                        '一致
            End If
            '2017/10/17 タスク）日比野 CHG 青梅信金(UI.5-1<PG>) ---------------------------------------- END
            .Append(" order by")
            '2017/10/17 タスク）日比野 CHG 青梅信金(UI.5-1<PG>) ---------------------------------------- START
            'If Me.TextBoxItakuKName.Text.Trim <> "" Then
            '    .Append("      ITAKU_KNAME_T")
            '    .Append("    , ITAKU_CODE_T")
            '    .Append("    , TORIS_CODE_T")
            '    .Append("    , TORIF_CODE_T")
            'Else
            '    Select Case Me.ITV_SORT_PATN
            '        Case "0"
            '            .Append("      TORIS_CODE_T")
            '            .Append("    , TORIF_CODE_T")
            '        Case "1"
            '            .Append("      ITAKU_CODE_T")
            '            .Append("    , TORIS_CODE_T")
            '            .Append("    , TORIF_CODE_T")
            '        Case "2"
            '            .Append("      ITAKU_KNAME_T")
            '            .Append("    , ITAKU_CODE_T")
            '            .Append("    , TORIS_CODE_T")
            '            .Append("    , TORIF_CODE_T")
            '        Case Else
            '            .Append("      TORIS_CODE_T")
            '            .Append("    , TORIF_CODE_T")
            '    End Select
            'End If
            Select Case Me.ITV_SORT_PATN
                Case "0"                            '0 - 取引先コード順 
                    .Append("      TORIS_CODE_T")
                    .Append("    , TORIF_CODE_T")
                Case "1"                            '1 - 委託者コード順 
                    .Append("      ITAKU_CODE_T")
                    .Append("    , TORIS_CODE_T")
                    .Append("    , TORIF_CODE_T")
                Case "2"                            '2 - 委託者カナ名順
                    .Append("      ITAKU_KNAME_T")
                    .Append("    , ITAKU_CODE_T")
                    .Append("    , TORIS_CODE_T")
                    .Append("    , TORIF_CODE_T")
                Case Else                           'x - 取引先コード順
                    .Append("      TORIS_CODE_T")
                    .Append("    , TORIF_CODE_T")
            End Select
            '2017/10/17 タスク）日比野 CHG 青梅信金(UI.5-1<PG>) ---------------------------------------- END
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
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
            MainLOG.Write("", "", "", "(ItakuTreeView-NzCheckString)", "失敗", ex.Message)
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END

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
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- START
            MainLOG.Write("", "", "", "(ItakuTreeView-GetLimitString)", "失敗", ex.Message)
            '2017/10/17 タスク）日比野 ADD 青梅信金(UI.5-1<PG>) ---------------------------------------- END
            Return ""
        End Try
    End Function

#End Region

End Class
