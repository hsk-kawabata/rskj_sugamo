Imports Microsoft.VisualBasic.FileIO
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Data.OracleClient
Imports MenteCommon
Imports System.Runtime.InteropServices


Public Module ModExternal

#Region "共通クラス"
    Public GCom As New MenteCommon.clsCommon
#End Region

#Region "共通変数"
    Public GOwnerForm As Windows.Forms.Form

    Public gstrFURI_DATE As String = ""     '振替日

    '2018/08/09 saitou 広島信金 DEL ---------------------------------------- START
    '文字が出ないのでやめる。
    ''------------------------------
    '' 共通Ini情報
    ''------------------------------
    'Private FrmProg As FrmProgress = Nothing
    '2018/08/09 saitou 広島信金 DEL ---------------------------------------- END
#End Region

    ''' <summary>
    ''' 口振・総振のスケジュールマスタ(追加情報)の作成処理を行います。
    ''' </summary>
    ''' <param name="FSyoriKbn"></param>
    ''' <param name="TorisCode"></param>
    ''' <param name="TorifCode"></param>
    ''' <param name="FuriDate"></param>
    ''' <param name="MotikomiSeq"></param>
    ''' <param name="ReturnMessage"></param>
    ''' <param name="MainDB"></param>
    ''' <param name="ArrayNum"></param>
    ''' <param name="ArrayVcr"></param>
    ''' <remarks></remarks>
    Public Sub Ex_InsertSchmastSub(ByVal FSyoriKbn As String, _
                                   ByVal TorisCode As String, _
                                   ByVal TorifCode As String, _
                                   ByVal FuriDate As String, _
                                   ByVal MotikomiSeq As Integer, _
                                   ByRef ReturnMessage As String, _
                                   Optional ByVal MainDB As CASTCommon.MyOracle = Nothing, _
                                   Optional ByVal ArrayNum As ArrayList = Nothing, _
                                   Optional ByVal ArrayVcr As ArrayList = Nothing)

        Dim SQL As New StringBuilder(128)
        Dim MainDB_NewFlg As Integer = 0

        Try

            '-----------------------------------------------------
            ' 拡張領域(数値型)のチェック
            '-----------------------------------------------------
            If Not ArrayNum Is Nothing Then
                If ArrayNum.Count <> 50 Then
                    ReturnMessage = "拡張領域(数値型)の項目数不正 項目数:" & ArrayNum.Count
                End If
            End If

            '-----------------------------------------------------
            ' 拡張領域(文字列型)のチェック
            '-----------------------------------------------------
            If Not ArrayVcr Is Nothing Then
                If ArrayVcr.Count <> 50 Then
                    ReturnMessage = "拡張領域(文字列型)の項目数不正 項目数:" & ArrayVcr.Count
                End If
            End If

            '-----------------------------------------------------
            ' オラクル接続チェック
            '-----------------------------------------------------
            If MainDB Is Nothing Then
                MainDB = New CASTCommon.MyOracle
                MainDB_NewFlg = 1
            End If

            SQL.Length = 0
            SQL.Append("INSERT INTO ")
            Select Case FSyoriKbn
                Case "1"
                    SQL.Append(" SCHMAST_SUB")
                Case "3"
                    SQL.Append(" S_SCHMAST_SUB")
            End Select
            SQL.Append(" ( ")
            SQL.Append("   FSYORI_KBN_SSUB")
            SQL.Append(" , TORIS_CODE_SSUB")
            SQL.Append(" , TORIF_CODE_SSUB")
            SQL.Append(" , FURI_DATE_SSUB")
            Select Case FSyoriKbn
                Case "3"
                    SQL.Append(" , MOTIKOMI_SEQ_SSUB")
            End Select
            SQL.Append(" , SYOUGOU_FLG_S")
            SQL.Append(" , SYOUGOU_DATE_S")
            SQL.Append(" , UKETUKE_TIME_STAMP_S")
            SQL.Append(" , DENSO_FILENAME_S")
            For Cnt = 1 To 50 Step 1
                SQL.Append(" , CUSTOM_NUM" & Format(Cnt, "00") & "_S")
            Next Cnt
            For Cnt = 1 To 50 Step 1
                SQL.Append(" , CUSTOM_VCR" & Format(Cnt, "00") & "_S")
            Next Cnt

            SQL.Append(" ) VALUES ( ")

            SQL.Append("   '" & FSyoriKbn & "'")
            SQL.Append(" , '" & TorisCode & "'")
            SQL.Append(" , '" & TorifCode & "'")
            SQL.Append(" , '" & FuriDate & "'")
            Select Case FSyoriKbn
                Case "3"
                    SQL.Append(" , " & MotikomiSeq)
            End Select
            SQL.Append(" , '0'")
            SQL.Append(" , '00000000'")
            SQL.Append(" , '00000000000000'")
            SQL.Append(" , ''")
            For Cnt = 1 To 50 Step 1
                If Not ArrayNum Is Nothing Then
                    Select Case ArrayNum(Cnt - 1)
                        Case Nothing
                            SQL.Append(" , ''")
                        Case Else
                            SQL.Append(" , " & CLng(ArrayNum(Cnt - 1)))
                    End Select
                Else
                    SQL.Append(" , ''")
                End If
            Next Cnt
            For Cnt = 1 To 50 Step 1
                If Not ArrayVcr Is Nothing Then
                    Select Case ArrayVcr(Cnt - 1)
                        Case Nothing
                            SQL.Append(" , ''")
                        Case Else
                            SQL.Append(" , '" & ArrayVcr(Cnt - 1) & "'")
                    End Select
                Else
                    SQL.Append(" , ''")
                End If
            Next Cnt
            SQL.Append(" ) ")

            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            ReturnMessage = "結果コード:" & iRet

        Catch ex As Exception
            ReturnMessage = ex.Message
        Finally
            If MainDB_NewFlg = 1 Then
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End If
        End Try

    End Sub

    ''' <summary>
    ''' 口振・総振のスケジュールマスタ(追加情報)の作成SQLを返します。
    ''' </summary>
    ''' <param name="FSyoriKbn"></param>
    ''' <param name="TorisCode"></param>
    ''' <param name="TorifCode"></param>
    ''' <param name="FuriDate"></param>
    ''' <param name="MotikomiSeq"></param>
    ''' <param name="ReturnMessage"></param>
    ''' <param name="RetSQL"></param>
    ''' <param name="ArrayNum"></param>
    ''' <param name="ArrayVcr"></param>
    ''' <remarks></remarks>
    Public Sub Ex_InsertSchmastSub_RetSQL(ByVal FSyoriKbn As String, _
                                          ByVal TorisCode As String, _
                                          ByVal TorifCode As String, _
                                          ByVal FuriDate As String, _
                                          ByVal MotikomiSeq As Integer, _
                                          ByRef ReturnMessage As String, _
                                          ByRef RetSQL As StringBuilder, _
                                          Optional ByVal ArrayNum As ArrayList = Nothing, _
                                          Optional ByVal ArrayVcr As ArrayList = Nothing)

        Try

            '-----------------------------------------------------
            ' 拡張領域(数値型)のチェック
            '-----------------------------------------------------
            If Not ArrayNum Is Nothing Then
                If ArrayNum.Count <> 50 Then
                    ReturnMessage = "拡張領域(数値型)の項目数不正 項目数:" & ArrayNum.Count
                End If
            End If

            '-----------------------------------------------------
            ' 拡張領域(文字列型)のチェック
            '-----------------------------------------------------
            If Not ArrayVcr Is Nothing Then
                If ArrayVcr.Count <> 50 Then
                    ReturnMessage = "拡張領域(文字列型)の項目数不正 項目数:" & ArrayVcr.Count
                End If
            End If

            RetSQL.Length = 0
            RetSQL.Append("INSERT INTO ")
            Select Case FSyoriKbn
                Case "1"
                    RetSQL.Append(" SCHMAST_SUB")
                Case "3"
                    RetSQL.Append(" S_SCHMAST_SUB")
            End Select
            RetSQL.Append(" ( ")
            RetSQL.Append("   FSYORI_KBN_SSUB")
            RetSQL.Append(" , TORIS_CODE_SSUB")
            RetSQL.Append(" , TORIF_CODE_SSUB")
            RetSQL.Append(" , FURI_DATE_SSUB")
            Select Case FSyoriKbn
                Case "3"
                    RetSQL.Append(" , MOTIKOMI_SEQ_SSUB")
            End Select
            RetSQL.Append(" , SYOUGOU_FLG_S")
            RetSQL.Append(" , SYOUGOU_DATE_S")
            RetSQL.Append(" , UKETUKE_TIME_STAMP_S")
            RetSQL.Append(" , DENSO_FILENAME_S")
            For Cnt = 1 To 50 Step 1
                RetSQL.Append(" , CUSTOM_NUM" & Format(Cnt, "00") & "_S")
            Next Cnt
            For Cnt = 1 To 50 Step 1
                RetSQL.Append(" , CUSTOM_VCR" & Format(Cnt, "00") & "_S")
            Next Cnt

            RetSQL.Append(" ) VALUES ( ")

            RetSQL.Append("   '" & FSyoriKbn & "'")
            RetSQL.Append(" , '" & TorisCode & "'")
            RetSQL.Append(" , '" & TorifCode & "'")
            RetSQL.Append(" , '" & FuriDate & "'")
            Select Case FSyoriKbn
                Case "3"
                    RetSQL.Append(" , " & MotikomiSeq)
            End Select
            RetSQL.Append(" , '0'")
            RetSQL.Append(" , '00000000'")
            RetSQL.Append(" , '00000000000000'")
            RetSQL.Append(" , ''")
            For Cnt = 1 To 50 Step 1
                If Not ArrayNum Is Nothing Then
                    Select Case ArrayNum(Cnt - 1)
                        Case Nothing
                            RetSQL.Append(" , ''")
                        Case Else
                            RetSQL.Append(" , " & CLng(ArrayNum(Cnt - 1)))
                    End Select
                Else
                    RetSQL.Append(" , ''")
                End If
            Next Cnt
            For Cnt = 1 To 50 Step 1
                If Not ArrayVcr Is Nothing Then
                    Select Case ArrayVcr(Cnt - 1)
                        Case Nothing
                            RetSQL.Append(" , ''")
                        Case Else
                            RetSQL.Append(" , '" & ArrayVcr(Cnt - 1) & "'")
                    End Select
                Else
                    RetSQL.Append(" , ''")
                End If
            Next Cnt
            RetSQL.Append(" ) ")

            ReturnMessage = "SQL作成成功"

        Catch ex As Exception
            ReturnMessage = ex.Message
        End Try

    End Sub

    ''' <summary>
    ''' 口振・総振のスケジュールマスタ(追加情報)の更新処理を行います。
    ''' </summary>
    ''' <param name="FSyoriKbn"></param>
    ''' <param name="TorisCode"></param>
    ''' <param name="TorifCode"></param>
    ''' <param name="FuriDate"></param>
    ''' <param name="MotikomiSeq"></param>
    ''' <param name="ColumnInfo"></param>
    ''' <param name="ReturnMessage"></param>
    ''' <param name="MainDB"></param>
    ''' <remarks></remarks>
    Public Sub Ex_UpdateSchmastSub(ByVal FSyoriKbn As String, _
                                   ByVal TorisCode As String, _
                                   ByVal TorifCode As String, _
                                   ByVal FuriDate As String, _
                                   ByVal MotikomiSeq As Integer, _
                                   ByVal ColumnInfo As ArrayList, _
                                   ByRef ReturnMessage As String, _
                                   Optional ByVal MainDB As CASTCommon.MyOracle = Nothing)

        Dim SQL As New StringBuilder(128)
        Dim MainDB_NewFlg As Integer = 0

        Try

            '-----------------------------------------------------
            ' オラクル接続チェック
            '-----------------------------------------------------
            If MainDB Is Nothing Then
                MainDB = New CASTCommon.MyOracle
                MainDB_NewFlg = 1
            End If

            SQL.Length = 0
            SQL.Append("UPDATE")
            Select Case FSyoriKbn
                Case "1"
                    SQL.Append(" SCHMAST_SUB")
                Case "3"
                    SQL.Append(" S_SCHMAST_SUB")
            End Select
            SQL.Append(" SET ")
            For i As Integer = 0 To ColumnInfo.Count - 1 Step 1
                Select Case i
                    Case 0
                        SQL.Append("  " & ColumnInfo(i).ToString.Split(","c)(0) & " =")
                        Select Case ColumnInfo(i).ToString.Split(","c)(1)
                            Case "NUMBER"
                                SQL.Append("  " & ColumnInfo(i).ToString.Split(","c)(2))
                            Case Else
                                SQL.Append("  " & SQ(ColumnInfo(i).ToString.Split(","c)(2)))
                        End Select
                    Case Else
                        SQL.Append(", " & ColumnInfo(i).ToString.Split(","c)(0) & " =")
                        Select Case ColumnInfo(i).ToString.Split(","c)(1)
                            Case "NUMBER"
                                SQL.Append("  " & ColumnInfo(i).ToString.Split(","c)(2))
                            Case Else
                                SQL.Append("  " & SQ(ColumnInfo(i).ToString.Split(","c)(2)))
                        End Select
                End Select
            Next
            SQL.Append(" WHERE ")
            SQL.Append("     FSYORI_KBN_SSUB   = " & SQ(FSyoriKbn))
            SQL.Append(" AND TORIS_CODE_SSUB   = " & SQ(TorisCode))
            SQL.Append(" AND TORIF_CODE_SSUB   = " & SQ(TorifCode))
            SQL.Append(" AND FURI_DATE_SSUB    = " & SQ(FuriDate))
            If FSyoriKbn = "3" Then
                SQL.Append(" AND MOTIKOMI_SEQ_SSUB = " & MotikomiSeq)
            End If

            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If MainDB_NewFlg = 1 Then
                If iRet = -1 Then
                    MainDB.Rollback()
                    ReturnMessage = "結果コード(Rollback):" & iRet
                Else
                    MainDB.Commit()
                    ReturnMessage = "結果コード(Commit):" & iRet
                End If
            Else
                ReturnMessage = "結果コード:" & iRet
            End If

        Catch ex As Exception
            ReturnMessage = ex.Message
        Finally
            If MainDB_NewFlg = 1 Then
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End If
        End Try

    End Sub

    ''' <summary>
    ''' 口振・総振のスケジュールマスタ(追加情報)の更新処理(Where句指定)を行います。
    ''' </summary>
    ''' <param name="FSyoriKbn"></param>
    ''' <param name="WhereSQL"></param>
    ''' <param name="ColumnInfo"></param>
    ''' <param name="ReturnMessage"></param>
    ''' <param name="MainDB"></param>
    ''' <remarks></remarks>
    Public Sub Ex_UpdateSchmastSub_Where(ByVal FSyoriKbn As String, _
                                         ByVal WhereSQL As StringBuilder, _
                                         ByVal ColumnInfo As ArrayList, _
                                         ByRef ReturnMessage As String, _
                                         Optional ByVal MainDB As CASTCommon.MyOracle = Nothing)

        Dim SQL As New StringBuilder(128)
        Dim MainDB_NewFlg As Integer = 0

        Try

            '-----------------------------------------------------
            ' オラクル接続チェック
            '-----------------------------------------------------
            If MainDB Is Nothing Then
                MainDB = New CASTCommon.MyOracle
                MainDB_NewFlg = 1
            End If

            SQL.Length = 0
            SQL.Append("UPDATE")
            Select Case FSyoriKbn
                Case "1"
                    SQL.Append(" SCHMAST_SUB")
                Case "3"
                    SQL.Append(" S_SCHMAST_SUB")
            End Select
            SQL.Append(" SET ")
            For i As Integer = 0 To ColumnInfo.Count - 1 Step 1
                Select Case i
                    Case 0
                        SQL.Append("  " & ColumnInfo(i).ToString.Split(","c)(0) & " =")
                        Select Case ColumnInfo(i).ToString.Split(","c)(1)
                            Case "NUMBER"
                                SQL.Append("  " & ColumnInfo(i).ToString.Split(","c)(2))
                            Case Else
                                SQL.Append("  " & SQ(ColumnInfo(i).ToString.Split(","c)(2)))
                        End Select
                    Case Else
                        SQL.Append(", " & ColumnInfo(i).ToString.Split(","c)(0) & " =")
                        Select Case ColumnInfo(i).ToString.Split(","c)(1)
                            Case "NUMBER"
                                SQL.Append("  " & ColumnInfo(i).ToString.Split(","c)(2))
                            Case Else
                                SQL.Append("  " & SQ(ColumnInfo(i).ToString.Split(","c)(2)))
                        End Select
                End Select
            Next
            SQL.Append(" WHERE ")
            SQL.Append("     FSYORI_KBN_SSUB   = " & SQ(FSyoriKbn))
            SQL.Append(" " & WhereSQL.ToString & " ")

            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If MainDB_NewFlg = 1 Then
                If iRet = -1 Then
                    MainDB.Rollback()
                    ReturnMessage = "結果コード(Rollback):" & iRet
                Else
                    MainDB.Commit()
                    ReturnMessage = "結果コード(Commit):" & iRet
                End If
            Else
                ReturnMessage = "結果コード:" & iRet
            End If

        Catch ex As Exception
            ReturnMessage = ex.Message
        Finally
            If MainDB_NewFlg = 1 Then
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End If
        End Try

    End Sub

    ''' <summary>
    ''' 口振・総振のスケジュールマスタ(追加情報)の更新処理を行います。
    ''' </summary>
    ''' <param name="FSyoriKbn"></param>
    ''' <param name="TorisCode"></param>
    ''' <param name="TorifCode"></param>
    ''' <param name="FuriDate"></param>
    ''' <param name="MotikomiSeq"></param>
    ''' <param name="ReturnMessage"></param>
    ''' <param name="MainDB"></param>
    ''' <remarks></remarks>
    Public Sub Ex_DeleteSchmastSub(ByVal FSyoriKbn As String, _
                                   ByVal TorisCode As String, _
                                   ByVal TorifCode As String, _
                                   ByVal FuriDate As String, _
                                   ByVal MotikomiSeq As Integer, _
                                   ByRef ReturnMessage As String, _
                                   Optional ByVal MainDB As CASTCommon.MyOracle = Nothing)

        Dim SQL As New StringBuilder(128)
        Dim MainDB_NewFlg As Integer = 0

        Try

            '-----------------------------------------------------
            ' オラクル接続チェック
            '-----------------------------------------------------
            If MainDB Is Nothing Then
                MainDB = New CASTCommon.MyOracle
                MainDB_NewFlg = 1
            End If

            SQL.Length = 0
            SQL.Append("DELETE")
            Select Case FSyoriKbn
                Case "1"
                    SQL.Append(" SCHMAST_SUB")
                Case "3"
                    SQL.Append(" S_SCHMAST_SUB")
            End Select
            SQL.Append(" WHERE ")
            SQL.Append("     FSYORI_KBN_SSUB   = " & SQ(FSyoriKbn))
            SQL.Append(" AND TORIS_CODE_SSUB   = " & SQ(TorisCode))
            SQL.Append(" AND TORIF_CODE_SSUB   = " & SQ(TorifCode))
            SQL.Append(" AND FURI_DATE_SSUB    = " & SQ(FuriDate))
            If FSyoriKbn = "3" Then
                SQL.Append(" AND MOTIKOMI_SEQ_SSUB = " & MotikomiSeq)
            End If

            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If MainDB_NewFlg = 1 Then
                If iRet = -1 Then
                    MainDB.Rollback()
                    ReturnMessage = "結果コード(Rollback):" & iRet
                Else
                    MainDB.Commit()
                    ReturnMessage = "結果コード(Commit):" & iRet
                End If
            Else
                ReturnMessage = "結果コード:" & iRet
            End If

        Catch ex As Exception
            ReturnMessage = ex.Message
        Finally
            If MainDB_NewFlg = 1 Then
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End If
        End Try

    End Sub

    ''' <summary>
    ''' 口振・総振のスケジュールマスタ(追加情報)の更新処理(Where句指定)を行います。
    ''' </summary>
    ''' <param name="FSyoriKbn"></param>
    ''' <param name="ReturnMessage"></param>
    ''' <param name="MainDB"></param>
    ''' <remarks></remarks>
    Public Sub Ex_DeleteSchmastSub_Where(ByVal FSyoriKbn As String, _
                                         ByVal WhereSQL As StringBuilder, _
                                         ByRef ReturnMessage As String, _
                                         Optional ByVal MainDB As CASTCommon.MyOracle = Nothing)

        Dim SQL As New StringBuilder(128)
        Dim MainDB_NewFlg As Integer = 0

        Try

            '-----------------------------------------------------
            ' オラクル接続チェック
            '-----------------------------------------------------
            If MainDB Is Nothing Then
                MainDB = New CASTCommon.MyOracle
                MainDB_NewFlg = 1
            End If

            SQL.Length = 0
            SQL.Append("DELETE")
            Select Case FSyoriKbn
                Case "1"
                    SQL.Append(" SCHMAST_SUB")
                Case "3"
                    SQL.Append(" S_SCHMAST_SUB")
            End Select
            SQL.Append(" WHERE ")
            SQL.Append("     FSYORI_KBN_SSUB   = " & SQ(FSyoriKbn))
            SQL.Append(" " & WhereSQL.ToString & " ")

            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If MainDB_NewFlg = 1 Then
                If iRet = -1 Then
                    MainDB.Rollback()
                    ReturnMessage = "結果コード(Rollback):" & iRet
                Else
                    MainDB.Commit()
                    ReturnMessage = "結果コード(Commit):" & iRet
                End If
            Else
                ReturnMessage = "結果コード:" & iRet
            End If

        Catch ex As Exception
            ReturnMessage = ex.Message
        Finally
            If MainDB_NewFlg = 1 Then
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End If
        End Try

    End Sub

    ''' <summary>
    ''' 店舗マスタの存在を確認する。
    ''' </summary>
    ''' <param name="MainDB"></param>
    ''' <param name="DataKinNo"></param>
    ''' <param name="DataSitNo"></param>
    ''' <param name="DataKinName"></param>
    ''' <param name="DataSitName"></param>
    ''' <param name="DataFuriDate"></param>
    ''' <param name="RetKinName"></param>
    ''' <param name="RetSitName"></param>
    ''' <remarks>0 - 金融機関なし，1 - 金融機関あり，支店なし，2 - 金融機関，支店あり</remarks>
    Public Function GetTENMASTExistsCustom(ByVal MainDB As CASTCommon.MyOracle, _
                                           ByVal DataKinNo As String, _
                                           ByVal DataSitNo As String, _
                                           ByVal DataKinName As String, _
                                           ByVal DataSitName As String, _
                                           ByVal DataFuriDate As String, _
                                           ByRef RetKinName As String, _
                                           ByRef RetSitName As String) As Integer

        Dim nRet As Integer = 0


        Dim WorkKinCode As String = ""
        Dim WorkKinName As String = DataKinName
        Dim WorkSitCode As String = ""
        Dim WorkSitName As String = DataSitName
        Dim WorkSitDelDate As String = "00000000"
        Dim WorkKinDelDate As String = "00000000"

        Dim SQL As New StringBuilder(256)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            ' ＤＢ接続が存在しない場合，正常値を返す
            If MainDB Is Nothing Then
                Return 2
            End If

            '***************************
            '金融機関を決定
            '***************************
            '金融機関カナで検索
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     KIN_NO_N")
            SQL.Append("   , KIN_KNAME_N")
            SQL.Append(" FROM ")
            SQL.Append("     TENMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TRIM(KIN_KNAME_N)  = " & SQ(DataKinName.Trim))
            SQL.Append(" GROUP BY")
            SQL.Append("     KIN_NO_N")
            SQL.Append("   , KIN_KNAME_N")

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                '金融機関名決定
                WorkKinCode = OraReader.GetString("KIN_NO_N")
                WorkKinName = OraReader.GetString("KIN_KNAME_N")
                OraReader.Close()
            Else
                OraReader.Close()

                '見つからないので金融機関コードで検索
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append("     KIN_NO_N")
                SQL.Append("   , KIN_KNAME_N")
                SQL.Append(" FROM ")
                SQL.Append("     TENMAST")
                SQL.Append(" WHERE ")
                SQL.Append("     KIN_NO_N   = " & SQ(DataKinNo))
                SQL.Append(" GROUP BY")
                SQL.Append("     KIN_NO_N")
                SQL.Append("   , KIN_KNAME_N")

                OraReader = New CASTCommon.MyOracleReader(MainDB)
                If OraReader.DataReader(SQL) = True Then
                    '金融機関名決定
                    WorkKinCode = OraReader.GetString("KIN_NO_N")
                    WorkKinName = OraReader.GetString("KIN_KNAME_N")
                    OraReader.Close()
                Else
                    OraReader.Close()
                    '見つからないので金融機関なしでおわり
                    '金、支店カナともに無条件で明細のカナ名で発信することになる
                    Return 0
                End If
            End If

            '***************************
            '支店を決定
            '***************************
            '金融機関コードは決定しているので金融機関コードと支店カナで検索、このとき削除日も取得
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     SIT_NO_N")
            SQL.Append("   , SIT_KNAME_N")
            SQL.Append("   , SIT_DEL_DATE_N")
            SQL.Append("   , KIN_DEL_DATE_N")
            SQL.Append(" FROM ")
            SQL.Append("     TENMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     KIN_NO_N          = " & SQ(WorkKinCode))
            SQL.Append(" AND TRIM(SIT_KNAME_N) = " & SQ(DataSitName.Trim))
            SQL.Append(" ORDER BY")
            SQL.Append("     SIT_FUKA_N")

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                '最小の枝番の支店名で決定
                WorkSitCode = OraReader.GetString("SIT_NO_N")
                WorkSitName = OraReader.GetString("SIT_KNAME_N")
                WorkSitDelDate = OraReader.GetString("SIT_DEL_DATE_N")
                WorkKinDelDate = OraReader.GetString("KIN_DEL_DATE_N")
                '削除日・・・
                '明細の支店コードと違っていた場合は・・・？
                OraReader.Close()
            Else
                OraReader.Close()
                '見つからないので金融機関コード、支店コードで検索
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append("     SIT_NO_N")
                SQL.Append("   , SIT_KNAME_N")
                SQL.Append("   , SIT_DEL_DATE_N")
                SQL.Append("   , KIN_DEL_DATE_N")
                SQL.Append(" FROM ")
                SQL.Append("     TENMAST")
                SQL.Append(" WHERE ")
                SQL.Append("     KIN_NO_N   = " & SQ(WorkKinCode))
                SQL.Append(" AND SIT_NO_N   = " & SQ(DataSitNo))
                SQL.Append(" ORDER BY")
                SQL.Append("     SIT_FUKA_N")

                OraReader = New CASTCommon.MyOracleReader(MainDB)
                If OraReader.DataReader(SQL) = True Then
                    '最小の枝番の支店名で決定
                    WorkSitCode = OraReader.GetString("SIT_NO_N")
                    WorkSitName = OraReader.GetString("SIT_KNAME_N")
                    WorkSitDelDate = OraReader.GetString("SIT_DEL_DATE_N")
                    WorkKinDelDate = OraReader.GetString("KIN_DEL_DATE_N")
                    '明細の支店コードと違っていた場合は・・・？
                    OraReader.Close()
                Else
                    OraReader.Close()
                    '見つからないので金融機関あり、支店なしでおわり
                    '支店名は無条件で明細のカナ名で発信することになる
                    Return 1
                End If
            End If

            '**********************************************************::

            If WorkKinDelDate.Trim.PadLeft(8, "0"c) <> "00000000" AndAlso WorkKinDelDate < DataFuriDate Then
                ' 振替日が金融機関削除日よりあとになります
                nRet = 3
            Else
                If WorkSitDelDate.Trim.PadLeft(8, "0"c) <> "00000000" AndAlso WorkSitDelDate < DataFuriDate Then
                    ' 振替日が支店削除日よりあとになります
                    nRet = 3
                Else
                    ' すべてあり
                    nRet = 2
                End If
            End If


            '2010.04.19 飯田 最終的には明細の金カナ、支店カナが空白でなければそちらを採用する。
            '                ややこしいが上記処理はそのまま通すことにする。
            If DataKinName.TrimEnd.Length > 0 Then
                WorkKinName = DataKinName
            End If

            If DataSitName.TrimEnd.Length > 0 Then
                WorkSitName = DataSitName
            End If

            Return nRet

        Catch ex As Exception
            Return 0
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            RetKinName = WorkKinName
            RetSitName = WorkSitName
        End Try

    End Function

    '2018/08/09 saitou 広島信金 DEL ---------------------------------------- START
    '文字が出ないのでやめる。
    '''' <summary>
    '''' 「実行中」画面を表示する。
    '''' </summary>
    '''' <param name="Title"></param>
    'Public Sub FrmProgressShow(Optional ByVal Title As String = "処理中")

    '    If FrmProg Is Nothing Then
    '        FrmProg = New FrmProgress
    '        FrmProg.Text = Title
    '        FrmProg.Show()
    '    End If

    'End Sub

    '''' <summary>
    '''' 「実行中」画面を閉じる。
    '''' </summary>
    'Public Sub FrmProgressClose()

    '    If Not FrmProg Is Nothing Then
    '        FrmProg.Close()
    '        FrmProg = Nothing
    '    End If

    'End Sub
    '2018/08/09 saitou 広島信金 DEL ---------------------------------------- END

#Region " 拡張用関数 "

    Public Function Ex_GetText_CodeToName(ByVal TxtName As String, ByVal Code As String) As String

        Dim ReturnData As String = ""

        Dim sr As StreamReader = Nothing

        Try

            sr = New StreamReader(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), TxtName), Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = Code Then
                    ReturnData = strLineData(1).Trim
                    Exit While
                End If
            End While

            sr.Close()
            sr = Nothing

            Return ReturnData

        Catch ex As Exception
            Return ""
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try

    End Function

    Public Function Ex_GetText_CodeToName(ByVal TxtName As String, ByVal Code As String, ByVal Column As String) As String

        Dim ReturnData As String = ""

        Dim sr As StreamReader = Nothing

        Try

            sr = New StreamReader(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), TxtName), Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = Code Then
                    ReturnData = strLineData(Column).Trim
                    Exit While
                End If
            End While

            sr.Close()
            sr = Nothing

            Return ReturnData

        Catch ex As Exception
            Return ""
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try

    End Function

    Public Function Ex_PutIni(ByVal IniName As String, ByVal AppName As String, ByVal KeyName As String, ByRef Value As String) As Integer

        Dim sIFileName As String = IniName
        Return WritePrivateProfileString(AppName, KeyName, Value, sIFileName)

    End Function

    <DllImport("KERNEL32.DLL")> _
    Public Function WritePrivateProfileString( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, _
        ByVal lpString As String, _
        ByVal lpFileName As String) As Integer
    End Function

    Public Function Ex_GetIni(ByVal IniName As String, ByVal AppName As String, ByVal KeyName As String) As String
        Dim Value As String = ""
        Dim sIFileName As String = IniName
        Dim sIDefault As String = "err"
        Call GetPrivateProfileString(AppName, KeyName, sIDefault, Value, 1024, sIFileName)
        Return Value
    End Function

    Public Function GetPrivateProfileString( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByRef lpReturnedString As String, ByVal nSize As Integer, _
        ByVal lpFileName As String) As Integer
        Dim str As New System.Text.StringBuilder(1024)
        Dim nRet As Integer = GetIniString(lpAppName, lpKeyName, lpDefault, str, str.Capacity, lpFileName)
        lpReturnedString = str.ToString
        Return nRet
    End Function

    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileString", CharSet:=CharSet.Auto)> _
    Public Function GetIniString( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByVal lpReturnedString As System.Text.StringBuilder, ByVal nSize As Integer, _
        ByVal lpFileName As String) As Integer
    End Function

#End Region

End Module
