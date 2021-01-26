Public Class ClsExtendPrintSQL

    Private Const ERRLOG_001 As String = "ファイル '{0}' が見つかりませんでした。"
    Private Const ERRLOG_002 As String = "{0}定義エラー：SQL文中に、置換文字列{1}が定義されていません。"

    Private LOG As New CASTCommon.BatchLOG("CAstExtendPrint", "ClsExtendPrintSQL")

    '------------------------------------------------------------
    ' 機能   ： SQL文を作成する
    ' 引数   ： prtid        - 帳票ID
    '           prtname      - 帳票名
    '           replaceArray - SQL置換文字列配列
    '           isALL9       - ALL9指定判定
    ' 戻り値 ： SQL文字列（エラーの場合はNothingを返す）
    '------------------------------------------------------------
    Public Function Make_SQL(ByVal prtid As String, ByVal prtname As String, ByVal replaceArray As String(), ByVal isALL9 As Boolean) As String

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("ClsExtendPrintSQL.Make_SQL")

        Dim sql As String = ""
        Dim sr As System.IO.StreamReader = Nothing

        Try

            'SQL定義ファイル取得
            Dim filepath As String = CASTCommon.GetFSKJIni("COMMON", "PRT_FLD")
            If filepath.EndsWith("\") = False Then
                filepath = filepath & "\"
            End If

            Dim filename As String = ""
            If isALL9 = False Then
                filename = filepath & prtid & "_" & prtname & "\" & prtid & "_" & prtname & "_SQL.txt"
            Else
                filename = filepath & prtid & "_" & prtname & "\" & prtid & "_" & prtname & "_SQL_ALL9.txt"
            End If

            If Not System.IO.File.Exists(filename) Then
                LOG.Write_Err("ClsExtendPrintSQL.Make_SQL", String.Format(ERRLOG_001, filename))
                Return Nothing
            End If

            sr = New System.IO.StreamReader(filename, System.Text.Encoding.GetEncoding("shift_jis"))
            While sr.Peek >= 0
                Dim strLine As String = sr.ReadLine().Trim
                If strLine <> "" Then
                    'タブスを空白に変換
                    strLine = strLine.Replace(vbTab, " ")
                    sql += strLine
                    sql += " "
                End IF
            End While

            If Not replaceArray Is Nothing Then
                'パラメタ置換
                For i As Integer = 0 To replaceArray.Length - 1

                    If replaceArray(i) Is Nothing Then
                        Continue For
                    End If

                    If sql.IndexOf("{" & i + 1 & "}") < 0 Then
                        LOG.Write_Err("ClsExtendPrintSQL.Make_SQL", String.Format(ERRLOG_002, filename, "{" & i + 1 & "}"))
                        Return Nothing
                    End If

                    sql = sql.Replace("{" & i + 1 & "}", replaceArray(i))
                Next
            End If

        Catch ex As Exception
            LOG.Write_Err("ClsExtendPrintSQL.Make_SQL", ex)
        Finally
            If Not sr Is Nothing Then
                sr.Close()
            End If

            LOG.Write_Exit3(sw, "ClsExtendPrintSQL.Make_SQL")
        End Try

        Return sql
    End Function

End Class
