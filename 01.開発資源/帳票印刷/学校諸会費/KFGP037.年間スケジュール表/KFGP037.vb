Imports System
Imports System.Collections
Imports System.Text
Imports Microsoft.VisualBasic
Imports CASTCommon

''' <summary>
''' 年間スケジュール表印刷　メインモジュール
''' </summary>
''' <remarks>2017/04/28 saitou RSV2 added for 標準機能追加(年間スケジュール表)</remarks>
Public Class KFGP037
    Inherits CAstReports.ClsReportBase

#Region "クラス変数"

    Public Property GAKKOU_CODE As String
        Get
            Return strGAKKOU_CODE
        End Get
        Set(value As String)
            strGAKKOU_CODE = value
        End Set
    End Property
    Private strGAKKOU_CODE As String

    Public Property NENDO As String
        Get
            Return strNENDO
        End Get
        Set(value As String)
            strNENDO = value
        End Set
    End Property
    Private strNENDO As String

    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ''' <summary>
    ''' 印刷内容格納構造体（ヘッダ部）
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure strcCommonInfo
        Dim GakkouCode As String
        Dim GakkouName As String
        Dim SiyouGakunen As String

        Public Sub Init()
            GakkouCode = ""
            GakkouName = ""
            SiyouGakunen = ""
        End Sub

        Public Sub SetOraData(ByVal OraReader As CASTCommon.MyOracleReader)
            GakkouCode = OraReader.GetString("GAKKOU_CODE_G")
            GakkouName = OraReader.GetString("GAKKOU_NNAME_G")
            SiyouGakunen = OraReader.GetString("SIYOU_GAKUNEN_T")
        End Sub
    End Structure
    Private CommonInfo As strcCommonInfo

    ''' <summary>
    ''' 印刷内容格納構造体（年間スケジュール）
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure strcNenkanInfo
        Dim FuriDate_1 As String
        Dim FuriDate_2 As String
        Dim FuriDate_3 As String
        Dim FuriDate_4 As String
        Dim FuriDate_5 As String
        Dim FuriDate_6 As String
        Dim FuriDate_7 As String
        Dim FuriDate_8 As String
        Dim FuriDate_9 As String
        Dim FuriDate_10 As String
        Dim FuriDate_11 As String
        Dim FuriDate_12 As String
        Dim SFuriDate_1 As String
        Dim SFuriDate_2 As String
        Dim SFuriDate_3 As String
        Dim SFuriDate_4 As String
        Dim SFuriDate_5 As String
        Dim SFuriDate_6 As String
        Dim SFuriDate_7 As String
        Dim SFuriDate_8 As String
        Dim SFuriDate_9 As String
        Dim SFuriDate_10 As String
        Dim SFuriDate_11 As String
        Dim SFuriDate_12 As String

        Public Sub Init()
            FuriDate_1 = ""
            FuriDate_2 = ""
            FuriDate_3 = ""
            FuriDate_4 = ""
            FuriDate_5 = ""
            FuriDate_6 = ""
            FuriDate_7 = ""
            FuriDate_8 = ""
            FuriDate_9 = ""
            FuriDate_10 = ""
            FuriDate_11 = ""
            FuriDate_12 = ""
            SFuriDate_1 = ""
            SFuriDate_2 = ""
            SFuriDate_3 = ""
            SFuriDate_4 = ""
            SFuriDate_5 = ""
            SFuriDate_6 = ""
            SFuriDate_7 = ""
            SFuriDate_8 = ""
            SFuriDate_9 = ""
            SFuriDate_10 = ""
            SFuriDate_11 = ""
            SFuriDate_12 = ""
        End Sub

        Public Sub SetOraData(ByVal Month As Integer, _
                              ByVal OraReader As CASTCommon.MyOracleReader)
            Select Case Month
                Case 1
                    FuriDate_1 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_1 = OraReader.GetString("SFURI_DATE_S")
                    End If
                Case 2
                    FuriDate_2 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_2 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 3
                    FuriDate_3 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_3 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 4
                    FuriDate_4 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_4 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 5
                    FuriDate_5 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_5 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 6
                    FuriDate_6 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_6 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 7
                    FuriDate_7 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_7 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 8
                    FuriDate_8 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_8 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 9
                    FuriDate_9 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_9 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 10
                    FuriDate_10 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_10 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 11
                    FuriDate_11 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_11 = OraReader.GetString("SFURI_DATE_S")
                    End If

                Case 12
                    FuriDate_12 = OraReader.GetString("FURI_DATE_S")
                    If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                        SFuriDate_12 = OraReader.GetString("SFURI_DATE_S")
                    End If
            End Select
        End Sub
    End Structure
    Private NenkanInfo As strcNenkanInfo

    ''' <summary>
    ''' 印刷内容格納構造体（特別スケジュール）
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure strcSpecialInfo
        Dim Tuki As String
        Dim FuriDate As String
        Dim SFuriDate As String
        Dim Gakunen1Flg As String
        Dim Gakunen2Flg As String
        Dim Gakunen3Flg As String
        Dim Gakunen4Flg As String
        Dim Gakunen5Flg As String
        Dim Gakunen6Flg As String
        Dim Gakunen7Flg As String
        Dim Gakunen8Flg As String
        Dim Gakunen9Flg As String

        Public Sub Init()
            Tuki = ""
            FuriDate = ""
            SFuriDate = ""
            Gakunen1Flg = ""
            Gakunen2Flg = ""
            Gakunen3Flg = ""
            Gakunen4Flg = ""
            Gakunen5Flg = ""
            Gakunen6Flg = ""
            Gakunen7Flg = ""
            Gakunen8Flg = ""
            Gakunen9Flg = ""
        End Sub

        Public Sub SetOraData(ByVal OraReader As CASTCommon.MyOracleReader)
            Tuki = OraReader.GetString("NENGETUDO_S").Substring(4, 2)
            FuriDate = OraReader.GetString("FURI_DATE_S")
            If OraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                SFuriDate = OraReader.GetString("SFURI_DATE_S")
            End If
            Gakunen1Flg = OraReader.GetString("GAKUNEN1_FLG_S")
            Gakunen2Flg = OraReader.GetString("GAKUNEN2_FLG_S")
            Gakunen3Flg = OraReader.GetString("GAKUNEN3_FLG_S")
            Gakunen4Flg = OraReader.GetString("GAKUNEN4_FLG_S")
            Gakunen5Flg = OraReader.GetString("GAKUNEN5_FLG_S")
            Gakunen6Flg = OraReader.GetString("GAKUNEN6_FLG_S")
            Gakunen7Flg = OraReader.GetString("GAKUNEN7_FLG_S")
            Gakunen8Flg = OraReader.GetString("GAKUNEN8_FLG_S")
            Gakunen9Flg = OraReader.GetString("GAKUNEN9_FLG_S")
        End Sub
    End Structure
    Private SpecialInfo As strcSpecialInfo
    Private SpecialInfoArray As ArrayList

    ''' <summary>
    ''' 印刷内容格納構造体（随時スケジュール）
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure strcAnyTimeInfo
        Dim NSKbn As String
        Dim FuriDate As String
        Dim Gakunen1Flg As String
        Dim Gakunen2Flg As String
        Dim Gakunen3Flg As String
        Dim Gakunen4Flg As String
        Dim Gakunen5Flg As String
        Dim Gakunen6Flg As String
        Dim Gakunen7Flg As String
        Dim Gakunen8Flg As String
        Dim Gakunen9Flg As String

        Public Sub Init()
            NSKbn = ""
            FuriDate = ""
            Gakunen1Flg = ""
            Gakunen2Flg = ""
            Gakunen3Flg = ""
            Gakunen4Flg = ""
            Gakunen5Flg = ""
            Gakunen6Flg = ""
            Gakunen7Flg = ""
            Gakunen8Flg = ""
            Gakunen9Flg = ""
        End Sub

        Public Sub SetOraData(ByVal OraReader As CASTCommon.MyOracleReader)
            NSKbn = OraReader.GetString("FURI_KBN_S")
            FuriDate = OraReader.GetString("FURI_DATE_S")
            Gakunen1Flg = OraReader.GetString("GAKUNEN1_FLG_S")
            Gakunen2Flg = OraReader.GetString("GAKUNEN2_FLG_S")
            Gakunen3Flg = OraReader.GetString("GAKUNEN3_FLG_S")
            Gakunen4Flg = OraReader.GetString("GAKUNEN4_FLG_S")
            Gakunen5Flg = OraReader.GetString("GAKUNEN5_FLG_S")
            Gakunen6Flg = OraReader.GetString("GAKUNEN6_FLG_S")
            Gakunen7Flg = OraReader.GetString("GAKUNEN7_FLG_S")
            Gakunen8Flg = OraReader.GetString("GAKUNEN8_FLG_S")
            Gakunen9Flg = OraReader.GetString("GAKUNEN9_FLG_S")
        End Sub
    End Structure
    Private AnyTimeInfo As strcAnyTimeInfo
    Private AnyTimeInfoArray As ArrayList

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' ＣＳＶファイル名セット
        InfoReport.ReportName = "KFGP037"
        ' 定義体名セット
        ReportBaseName = "KFGP037_年間スケジュール表.rpd"
    End Sub

    ''' <summary>
    ''' 印刷内容の設定を行います。（ヘッダ部）
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function MakeRecordCommon() As Boolean
        Dim dbReader As CASTCommon.MyOracleReader = Nothing

        Try
            dbReader = New CASTCommon.MyOracleReader(MainDB)
            If dbReader.DataReader(CreateGetGakkouSQL()) = True Then
                While dbReader.EOF = False
                    CommonInfo.Init()
                    CommonInfo.SetOraData(dbReader)
                    dbReader.NextRead()
                End While
            Else
                '対象学校なし
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校マスタ参照", "失敗", "学校マスタ登録無し")
                RecordCnt = -1
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷レコード設定", "失敗", ex.Message)
            Return False

        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 印刷内容の設定を行います。（年間スケジュール）
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function MakeRecordNenkan() As Boolean
        Dim dbMainReader As CASTCommon.MyOracleReader = Nothing
        Dim dbSubReader As CASTCommon.MyOracleReader = Nothing

        Try
            dbMainReader = New CASTCommon.MyOracleReader(MainDB)
            If dbMainReader.DataReader(CreateGetGakkouSQL()) = True Then
                dbSubReader = New CASTCommon.MyOracleReader(MainDB)
                NenkanInfo.Init()

                While dbMainReader.EOF = False
                    For i As Integer = 1 To 12 Step 1

                        '年月度の設定
                        Dim Nengetudo As String = ""
                        Select Case i
                            Case 1 To 3
                                Nengetudo = CStr(CInt(Me.NENDO) + 1) & i.ToString.PadLeft(2, "0"c)
                            Case Else
                                Nengetudo = Me.NENDO & i.ToString.PadLeft(2, "0"c)
                        End Select

                        '年間スケジュールの取得
                        If dbSubReader.DataReader(CreateNenkanListSQL(Nengetudo)) = True Then
                            RecordCnt += 1
                            NenkanInfo.SetOraData(i, dbSubReader)
                        End If

                        dbSubReader.Close()
                    Next

                    dbMainReader.NextRead()
                End While
            Else
                '対象学校なし
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校マスタ参照", "失敗", "学校マスタ登録無し")
                RecordCnt = -1
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷レコード設定", "失敗", ex.Message)
            Return False

        Finally
            If Not dbMainReader Is Nothing Then
                dbMainReader.Close()
                dbMainReader = Nothing
            End If
            If Not dbSubReader Is Nothing Then
                dbSubReader.Close()
                dbSubReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 印刷内容の設定を行います。（特別スケジュール）
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function MakeRecordSpecial() As Boolean
        Dim dbMainReader As CASTCommon.MyOracleReader = Nothing
        Dim dbSubReader As CASTCommon.MyOracleReader = Nothing

        Try
            SpecialInfoArray = New ArrayList
            dbMainReader = New CASTCommon.MyOracleReader(MainDB)
            If dbMainReader.DataReader(CreateGetGakkouSQL()) = True Then
                dbSubReader = New CASTCommon.MyOracleReader(MainDB)

                While dbMainReader.EOF = False
                    If dbSubReader.DataReader(CreateSpecialListSQL()) = True Then
                        While dbSubReader.EOF = False
                            RecordCnt += 1
                            SpecialInfo.Init()
                            SpecialInfo.SetOraData(dbSubReader)
                            SpecialInfoArray.Add(SpecialInfo)
                            dbSubReader.NextRead()
                        End While
                    End If

                    dbSubReader.Close()

                    dbMainReader.NextRead()
                End While
            Else
                '対象学校なし
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校マスタ参照", "失敗", "学校マスタ登録無し")
                RecordCnt = -1
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷レコード設定", "失敗", ex.Message)
            Return False

        Finally
            If Not dbMainReader Is Nothing Then
                dbMainReader.Close()
                dbMainReader = Nothing
            End If
            If Not dbSubReader Is Nothing Then
                dbSubReader.Close()
                dbSubReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 印刷内容の設定を行います。（随時スケジュール）
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function MakeRecordAnyTime() As Boolean
        Dim dbMainReader As CASTCommon.MyOracleReader = Nothing
        Dim dbSubReader As CASTCommon.MyOracleReader = Nothing

        Try
            AnyTimeInfoArray = New ArrayList
            dbMainReader = New CASTCommon.MyOracleReader(MainDB)
            If dbMainReader.DataReader(CreateGetGakkouSQL()) = True Then
                dbSubReader = New CASTCommon.MyOracleReader(MainDB)

                While dbMainReader.EOF = False
                    If dbSubReader.DataReader(CreateAnyTimeListSQL()) = True Then
                        While dbSubReader.EOF = False
                            RecordCnt += 1
                            AnyTimeInfo.Init()
                            AnyTimeInfo.SetOraData(dbSubReader)
                            AnyTimeInfoArray.Add(AnyTimeInfo)
                            dbSubReader.NextRead()
                        End While
                    End If

                    dbSubReader.Close()

                    dbMainReader.NextRead()
                End While
            Else
                '対象学校なし
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校マスタ参照", "失敗", "学校マスタ登録無し")
                RecordCnt = -1
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷レコード設定", "失敗", ex.Message)
            Return False

        Finally
            If Not dbMainReader Is Nothing Then
                dbMainReader.Close()
                dbMainReader = Nothing
            End If
            If Not dbSubReader Is Nothing Then
                dbSubReader.Close()
                dbSubReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 印刷内容の設定を行います。（印刷用ＣＳＶの作成）
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function MakeRecord() As Boolean
        Try
            '----------------------------------------
            '年間スケジュール設定
            '----------------------------------------
            If Me.SetListCsvNenkan() = False Then
                Return False
            End If

            '----------------------------------------
            '特別スケジュール設定
            '----------------------------------------
            If Me.SetListCsvSpecial() = False Then
                Return False
            End If

            '----------------------------------------
            '随時スケジュール設定
            '----------------------------------------
            If Me.SetListCsvAnyTime() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールレコード設定", "失敗", ex.Message)
            Return False

        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 印刷対象の学校を取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetGakkouSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     GAKKOU_CODE_G")
            .Append("    ,GAKKOU_NNAME_G")
            .Append("    ,SIYOU_GAKUNEN_T")
            .Append(" from")
            .Append("     GAKMAST1")
            .Append(" inner join")
            .Append("     GAKMAST2")
            .Append(" on  GAKKOU_CODE_G = GAKKOU_CODE_T")
            .Append(" where")
            .Append("     GAKKOU_CODE_G = " & SQ(GAKKOU_CODE))
            .Append(" group by")
            .Append("     GAKKOU_CODE_G")
            .Append("    ,GAKKOU_NNAME_G")
            .Append("    ,SIYOU_GAKUNEN_T")
        End With

        Return SQL

    End Function

    ''' <summary>
    ''' 年間スケジュール情報を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="Nengetudo">年月度</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateNenkanListSQL(ByVal Nengetudo As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     FURI_DATE_S")
            .Append("    ,SFURI_DATE_S")
            .Append(" from")
            .Append("     G_SCHMAST")
            .Append(" where")
            .Append("     GAKKOU_CODE_S = " & SQ(GAKKOU_CODE))
            .Append(" and NENGETUDO_S = " & SQ(Nengetudo))
            .Append(" and SCH_KBN_S = '0'")     '年間
            .Append(" and FURI_KBN_S = '0'")    '初振
        End With

        Return SQL

    End Function

    ''' <summary>
    ''' 特別スケジュール情報を取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateSpecialListSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     NENGETUDO_S")
            .Append("    ,FURI_DATE_S")
            .Append("    ,SFURI_DATE_S")
            .Append("    ,GAKUNEN1_FLG_S")
            .Append("    ,GAKUNEN2_FLG_S")
            .Append("    ,GAKUNEN3_FLG_S")
            .Append("    ,GAKUNEN4_FLG_S")
            .Append("    ,GAKUNEN5_FLG_S")
            .Append("    ,GAKUNEN6_FLG_S")
            .Append("    ,GAKUNEN7_FLG_S")
            .Append("    ,GAKUNEN8_FLG_S")
            .Append("    ,GAKUNEN9_FLG_S")
            .Append(" from")
            .Append("     G_SCHMAST")
            .Append(" where")
            .Append("     GAKKOU_CODE_S = " & SQ(GAKKOU_CODE))
            .Append(" and NENGETUDO_S")
            .Append(" between")
            .Append("     " & SQ(Me.NENDO & "04"))
            .Append(" and " & SQ(CStr(CInt(Me.NENDO) + 1) & "03"))
            .Append(" and SCH_KBN_S = '1'")     '特別
            .Append(" and FURI_KBN_S = '0'")    '初振
            .Append(" order by")
            .Append("     NENGETUDO_S")
            .Append("    ,FURI_DATE_S")
            .Append("    ,SFURI_DATE_S")
        End With

        Return SQL

    End Function

    ''' <summary>
    ''' 随時スケジュール情報を取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateAnyTimeListSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     FURI_DATE_S")
            .Append("    ,FURI_KBN_S")
            .Append("    ,GAKUNEN1_FLG_S")
            .Append("    ,GAKUNEN2_FLG_S")
            .Append("    ,GAKUNEN3_FLG_S")
            .Append("    ,GAKUNEN4_FLG_S")
            .Append("    ,GAKUNEN5_FLG_S")
            .Append("    ,GAKUNEN6_FLG_S")
            .Append("    ,GAKUNEN7_FLG_S")
            .Append("    ,GAKUNEN8_FLG_S")
            .Append("    ,GAKUNEN9_FLG_S")
            .Append(" from")
            .Append("     G_SCHMAST")
            .Append(" where")
            .Append("     GAKKOU_CODE_S = " & SQ(GAKKOU_CODE))
            .Append(" and NENGETUDO_S")
            .Append(" between")
            .Append("     " & SQ(Me.NENDO & "04"))
            .Append(" and " & SQ(CStr(CInt(Me.NENDO) + 1) & "03"))
            .Append(" and SCH_KBN_S = '2'")     '特別
            .Append(" order by")
            .Append("     NENGETUDO_S")
            .Append("    ,FURI_DATE_S")
            .Append("    ,FURI_KBN_S")
        End With

        Return SQL

    End Function

    ''' <summary>
    ''' 年間スケジュール情報を設定します。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function SetListCsvNenkan() As Boolean
        Try
            '共通部
            Me.SetListCsvCommon()
            '年間スケジュール情報
            Me.SetListCsvNenkan("0")
            '特別スケジュール情報(空白)
            Me.SetListCsvSpecial(True)
            '随時スケジュール情報(空白)
            Me.SetListCsvAnyTime(True)
            '改行
            OutputCsvData("", False, True)

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "年間スケジュール設定", "失敗", ex.Message)
            Return False

        End Try
    End Function

    ''' <summary>
    ''' 特別スケジュール情報を設定します。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function SetListCsvSpecial() As Boolean
        Try
            If Me.SpecialInfoArray.Count = 0 Then
                '対象なし
                Return True
            Else
                For i As Integer = 0 To Me.SpecialInfoArray.Count - 1
                    '構造体初期化
                    SpecialInfo.Init()
                    SpecialInfo = DirectCast(Me.SpecialInfoArray(i), strcSpecialInfo)

                    '共通部
                    Me.SetListCsvCommon()
                    '年間スケジュール情報
                    Me.SetListCsvNenkan("1")
                    '特別スケジュール情報
                    Me.SetListCsvSpecial(False)
                    '随時スケジュール情報(空白)
                    Me.SetListCsvAnyTime(True)
                    '改行
                    OutputCsvData("", False, True)
                Next
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "特別スケジュール設定", "失敗", ex.Message)
            Return False

        End Try
    End Function

    ''' <summary>
    ''' 随時スケジュール情報を設定します。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function SetListCsvAnyTime() As Boolean
        Try
            If Me.AnyTimeInfoArray.Count = 0 Then
                '対象なし
                Return True
            Else
                For i As Integer = 0 To Me.AnyTimeInfoArray.Count - 1
                    '構造体初期化
                    AnyTimeInfo.Init()
                    AnyTimeInfo = DirectCast(Me.AnyTimeInfoArray(i), strcAnyTimeInfo)

                    '共通部
                    Me.SetListCsvCommon()
                    '年間スケジュール情報
                    Me.SetListCsvNenkan("2")
                    '特別スケジュール情報(空白)
                    Me.SetListCsvSpecial(True)
                    '随時スケジュール情報
                    Me.SetListCsvAnyTime(False)
                    '改行
                    OutputCsvData("", False, True)
                Next
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "随時スケジュール設定", "失敗", ex.Message)
            Return False

        End Try

    End Function

    ''' <summary>
    ''' 共通部設定
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetListCsvCommon()
        OutputCsvData(strDate, True)
        OutputCsvData(strTime, True)
        OutputCsvData(CommonInfo.GakkouCode, True)
        OutputCsvData(CommonInfo.GakkouName, True)
        OutputCsvData(NENDO, True)
    End Sub

    ''' <summary>
    ''' 年間スケジュール部設定
    ''' </summary>
    ''' <param name="SCH_KBN"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsvNenkan(ByVal SCH_KBN As String)
        OutputCsvData(SCH_KBN)                              'スケジュール区分
        OutputCsvData(NenkanInfo.FuriDate_4)                '4月振替日
        OutputCsvData(NenkanInfo.SFuriDate_4)               '4月再振日
        OutputCsvData(NenkanInfo.FuriDate_5)                '5月振替日
        OutputCsvData(NenkanInfo.SFuriDate_5)               '5月再振日
        OutputCsvData(NenkanInfo.FuriDate_6)                '6月振替日
        OutputCsvData(NenkanInfo.SFuriDate_6)               '6月再振日
        OutputCsvData(NenkanInfo.FuriDate_7)                '7月振替日
        OutputCsvData(NenkanInfo.SFuriDate_7)               '7月再振日
        OutputCsvData(NenkanInfo.FuriDate_8)                '8月振替日
        OutputCsvData(NenkanInfo.SFuriDate_8)               '8月再振日
        OutputCsvData(NenkanInfo.FuriDate_9)                '9月振替日
        OutputCsvData(NenkanInfo.SFuriDate_9)               '9月再振日
        OutputCsvData(NenkanInfo.FuriDate_10)               '10月振替日
        OutputCsvData(NenkanInfo.SFuriDate_10)              '10月再振日
        OutputCsvData(NenkanInfo.FuriDate_11)               '11月振替日
        OutputCsvData(NenkanInfo.SFuriDate_11)              '11月再振日
        OutputCsvData(NenkanInfo.FuriDate_12)               '12月振替日
        OutputCsvData(NenkanInfo.SFuriDate_12)              '12月再振日
        OutputCsvData(NenkanInfo.FuriDate_1)                '1月振替日
        OutputCsvData(NenkanInfo.SFuriDate_1)               '1月再振日
        OutputCsvData(NenkanInfo.FuriDate_2)                '2月振替日
        OutputCsvData(NenkanInfo.SFuriDate_2)               '2月再振日
        OutputCsvData(NenkanInfo.FuriDate_3)                '3月振替日
        OutputCsvData(NenkanInfo.SFuriDate_3)               '3月再振日
    End Sub

    ''' <summary>
    ''' 特別スケジュール部設定
    ''' </summary>
    ''' <param name="NoDataFlg"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsvSpecial(ByVal NoDataFlg As Boolean)
        If NoDataFlg = True Then
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
        Else
            '学年フラグの設定　使用学年まで値の設定を行う
            Dim GakunenFlg(8) As String
            For i As Integer = 0 To 8
                GakunenFlg(i) = ""
            Next
            For i As Integer = 0 To 8
                Select Case i + 1
                    Case Is = 1
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen1Flg = "1", "○", "")
                    Case Is = 2
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen2Flg = "1", "○", "")
                    Case Is = 3
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen3Flg = "1", "○", "")
                    Case Is = 4
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen4Flg = "1", "○", "")
                    Case Is = 5
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen5Flg = "1", "○", "")
                    Case Is = 6
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen6Flg = "1", "○", "")
                    Case Is = 7
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen7Flg = "1", "○", "")
                    Case Is = 8
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen8Flg = "1", "○", "")
                    Case Is = 9
                        GakunenFlg(i) = IIf(SpecialInfo.Gakunen9Flg = "1", "○", "")
                End Select

                If i = CInt(CommonInfo.SiyouGakunen) - 1 Then
                    Exit For
                End If
            Next

            OutputCsvData(SpecialInfo.Tuki)                                         '請求月
            OutputCsvData(SpecialInfo.FuriDate)                                     '振替日
            OutputCsvData(SpecialInfo.SFuriDate)                                    '再振日
            '学年フラグ
            For i As Integer = 0 To 8
                OutputCsvData(GakunenFlg(i))
            Next
        End If
    End Sub

    ''' <summary>
    ''' 随時スケジュール部設定
    ''' </summary>
    ''' <param name="NoDataFlg"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsvAnyTime(ByVal NoDataFlg As Boolean)
        If NoDataFlg = True Then
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
            OutputCsvData("")
        Else
            '学年フラグの設定　使用学年まで値の設定を行う
            Dim GakunenFlg(8) As String
            For i As Integer = 0 To 8
                GakunenFlg(i) = ""
            Next
            For i As Integer = 0 To 8
                Select Case i + 1
                    Case Is = 1
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen1Flg = "1", "○", "")
                    Case Is = 2
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen2Flg = "1", "○", "")
                    Case Is = 3
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen3Flg = "1", "○", "")
                    Case Is = 4
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen4Flg = "1", "○", "")
                    Case Is = 5
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen5Flg = "1", "○", "")
                    Case Is = 6
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen6Flg = "1", "○", "")
                    Case Is = 7
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen7Flg = "1", "○", "")
                    Case Is = 8
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen8Flg = "1", "○", "")
                    Case Is = 9
                        GakunenFlg(i) = IIf(AnyTimeInfo.Gakunen9Flg = "1", "○", "")
                End Select

                If i = CInt(CommonInfo.SiyouGakunen) - 1 Then
                    Exit For
                End If
            Next

            OutputCsvData(IIf(AnyTimeInfo.NSKbn = "2", "入金", "出金"))       '入出金区分
            OutputCsvData(AnyTimeInfo.FuriDate)                               '振替日
            '学年フラグ
            For i As Integer = 0 To 8
                OutputCsvData(GakunenFlg(i))
            Next
        End If
    End Sub

    ''' <summary>
    ''' CSVファイルに内容を書き込みます。
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="dq"></param>
    ''' <param name="crlf"></param>
    ''' <remarks></remarks>
    Private Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

#End Region

End Class
