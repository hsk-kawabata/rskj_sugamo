Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Public Class KFGP024

    Inherits CAstReports.ClsReportBase
    Sub New()

        ' CSVファイルセット
        InfoReport.ReportName = "KFGP024"
        ' 定義体名セット
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        If STR_HIMOKU_PTN = "1" Then
            ReportBaseName = "KFGP024_収納状況一覧表(費目別合計)(費目15).rpd"
        Else
            ReportBaseName = "KFGP024_収納状況一覧表(費目別合計).rpd"
        End If
        'ReportBaseName = "KFGP024_収納状況一覧表(費目別合計).rpd"
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END

    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： CSVファイルに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '
    ' 機能　 ： 印刷データの作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim SUB_SQL As New StringBuilder(128)
        Dim KEN(3) As Decimal '1:依頼件数 2:振替件数 3:不能件数
        Dim KIN(3) As Decimal '1:依頼金額 2:振替金額 3:不能金額
        Dim HimokuKen(15) As Decimal '費目1～15 依頼件数
        Dim HimokuKin(15) As Decimal '費目1～15 依頼金額
        Dim HimokuFunouKen(15) As Decimal '費目1～15 不能件数
        Dim HimokuFunouKin(15) As Decimal '費目1～15 不能金額
        Dim HimokuFuriKen(15) As Decimal '費目1～15 依頼件数
        Dim HimokuFuriKin(15) As Decimal '費目1～15 依頼件数
        Dim HimoNameUse(15) As String '費目名使用判定
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)



            '集計件数・金額計算用サブクエリ
            SUB_SQL.Append(" SELECT")
            SUB_SQL.Append(" COUNT(SEIKYU_KIN_M) IRAI_KEN")                                             '依頼件数
            SUB_SQL.Append(",SUM(SEIKYU_KIN_M) IRAI_KIN")                                               '依頼金額
            SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 1 ELSE 0 END ) FURI_KEN")             '振替済件数
            SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN SEIKYU_KIN_M ELSE 0 END ) FURI_KIN")  '振替済金額
            SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 0 ELSE 1 END ) FUNOU_KEN")            '不能件数
            SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 0 ELSE SEIKYU_KIN_M END ) FUNOU_KIN") '不能金額
            For No As Integer = 1 To 15
                SUB_SQL.Append(",SUM(CASE HIMOKU" & No.ToString & "_KIN_M WHEN 0 THEN 0 ELSE 1 END) HIMO" & No.ToString & "_IRAI_KEN")  '費目依頼件数
                SUB_SQL.Append(",SUM(HIMOKU" & No.ToString & "_KIN_M) HIMO" & No.ToString & "_IRAI_KIN")                                '費目依頼金額

                SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN " & _
                           "(CASE HIMOKU" & No.ToString & "_KIN_M WHEN 0 THEN 0 ELSE 1 END) ELSE 0 END ) HIMO" & No.ToString & "_FURI_KEN") '振替済件数
                SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN HIMOKU" & No.ToString & "_KIN_M ELSE 0 END ) HIMO" & No.ToString & "_FURI_KIN") '振替済金額

                SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN " & _
                           "0 ELSE (CASE HIMOKU" & No.ToString & "_KIN_M WHEN 0 THEN 0 ELSE 1 END) END ) HIMO" & No.ToString & "_FUNOU_KEN") '不能件数
                SUB_SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 0 ELSE HIMOKU" & No.ToString & "_KIN_M END ) HIMO" & No.ToString & "_FUNOU_KIN") '不能金額
            Next
            SUB_SQL.Append(",GAKKOU_CODE_M GAKKOU_CODE_W")
            SUB_SQL.Append(",GAKUNEN_CODE_M GAKUNEN_CODE_W")
            SUB_SQL.Append(",FURI_DATE_M FURI_DATE_W")
            SUB_SQL.Append(",MIN(HIMOKU_ID_M) HIMOKU_ID_W")
            SUB_SQL.Append(" FROM ")
            SUB_SQL.Append(" KZFMAST.GAKMAST2  ")
            SUB_SQL.Append(",KZFMAST.G_MEIMAST ")
            SUB_SQL.Append(",KZFMAST.GAKMAST1 ")
            SUB_SQL.Append(" WHERE GAKMAST2.GAKKOU_CODE_T = G_MEIMAST.GAKKOU_CODE_M")
            SUB_SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = GAKMAST1.GAKKOU_CODE_G")
            SUB_SQL.Append(" AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G")
            SUB_SQL.Append(" AND G_MEIMAST.SEIKYU_KIN_M > 0")
            SUB_SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = " & SQ(GakkouCode))
            SUB_SQL.Append(" AND G_MEIMAST.FURI_DATE_M = " & SQ(FuriDate))
            SUB_SQL.Append(" AND G_MEIMAST.SAIFURI_SUMI_M ='0'")
            '2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '随時に同じ振替日が存在した場合に随時の費目IDを取ってしまう
            SUB_SQL.Append(" AND G_MEIMAST.FURI_KBN_M IN ('0','1')")
            '2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
            SUB_SQL.Append(" GROUP BY GAKKOU_CODE_M,GAKUNEN_CODE_M,FURI_DATE_M")
            SUB_SQL.Append(" ORDER BY GAKKOU_CODE_M,GAKUNEN_CODE_M,FURI_DATE_M")

            If oraReader.DataReader(SUB_SQL) = False Then
                'この時点で失敗する場合はデータなし
                Return False
            End If
            'SQLここから
            SQL.Append(" SELECT")
            SQL.Append(" GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKKOU_CODE_G ")
            SQL.Append(",WORK.FURI_DATE_W ")
            SQL.Append(",WORK.HIMOKU_ID_W ")

            For No As Integer = 1 To 15
                SQL.Append(",HIMOMAST.HIMOKU_NAME" & No.ToString("00") & "_H ")
                SQL.Append(",WORK.HIMO" & No.ToString & "_IRAI_KEN")
                SQL.Append(",WORK.HIMO" & No.ToString & "_IRAI_KIN")
                SQL.Append(",WORK.HIMO" & No.ToString & "_FURI_KEN")
                SQL.Append(",WORK.HIMO" & No.ToString & "_FURI_KIN")
                SQL.Append(",WORK.HIMO" & No.ToString & "_FUNOU_KEN")
                SQL.Append(",WORK.HIMO" & No.ToString & "_FUNOU_KIN")
            Next
            SQL.Append(",WORK.IRAI_KEN ")
            SQL.Append(",WORK.IRAI_KIN ")
            SQL.Append(",WORK.FURI_KEN ")
            SQL.Append(",WORK.FURI_KIN")
            SQL.Append(",WORK.FUNOU_KEN")
            SQL.Append(",WORK.FUNOU_KIN")

            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G ")
            SQL.Append(",GAKMAST1.GAKUNEN_CODE_G ")
            SQL.Append(",GAKMAST2.JIFURI_KBN_T ")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQL.Append(" FROM ")
            SQL.Append(" KZFMAST.GAKMAST2  ")
            SQL.Append(",KZFMAST.GAKMAST1 ")
            SQL.Append(",KZFMAST.HIMOMAST ")
            SQL.Append(",(" & SUB_SQL.ToString & ") WORK")

            SQL.Append(" WHERE GAKMAST2.GAKKOU_CODE_T = GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND WORK.GAKKOU_CODE_W = HIMOMAST.GAKKOU_CODE_H")
            SQL.Append(" AND WORK.GAKUNEN_CODE_W = HIMOMAST.GAKUNEN_CODE_H")
            SQL.Append(" AND WORK.HIMOKU_ID_W = HIMOMAST.HIMOKU_ID_H")
            SQL.Append(" AND GAKMAST1.GAKKOU_CODE_G = WORK.GAKKOU_CODE_W")
            SQL.Append(" AND GAKMAST1.GAKUNEN_CODE_G = WORK.GAKUNEN_CODE_W")

            SQL.Append(" AND GAKMAST2.GAKKOU_CODE_T = " & SQ(GakkouCode))
            SQL.Append(" AND WORK.FURI_DATE_W = " & SQ(FuriDate))
            SQL.Append(" AND HIMOMAST.TUKI_NO_H =" & SQ(Mid(SeikyuTuki, 5, 2)))

            SQL.Append(" ORDER BY WORK.GAKKOU_CODE_W,WORK.GAKUNEN_CODE_W,WORK.FURI_DATE_W")

            If oraReader.DataReader(SQL) = True Then
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                User_ID = oraReader.GetString("YOBI1_T")
                BaitaiCode = oraReader.GetString("BAITAI_CODE_T")
                ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                While oraReader.EOF = False
                    OutputCsvData(mMatchingDate)                                                '処理日
                    OutputCsvData(mMatchingTime)                                                'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))             '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))            '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_W")))               '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_G")))            '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))            '学年名

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("JIFURI_KBN_T")))            '自振区分
                    OutputCsvData("0")            '学校集計
                    For No As Integer = 1 To 15                                                 '費目名1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H")))
                        If GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H")).Trim <> "" Then
                            HimoNameUse(No) = "1"
                        End If
                    Next
                    KEN(1) += GCOM.NzDec(oraReader.GetString("IRAI_KEN")) '請求件数
                    KEN(2) += GCOM.NzDec(oraReader.GetString("FURI_KEN")) '振替済件数
                    KEN(3) += GCOM.NzDec(oraReader.GetString("FUNOU_KEN")) '振替不能件数
                    KIN(1) += GCOM.NzDec(oraReader.GetString("IRAI_KIN")) '請求金額
                    KIN(2) += GCOM.NzDec(oraReader.GetString("FURI_KIN")) '振替済金額
                    KIN(3) += GCOM.NzDec(oraReader.GetString("FUNOU_KIN")) '振替不能金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("IRAI_KEN"))) '請求件数
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_KEN"))) '振替済件数
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FUNOU_KEN"))) '振替不能件数
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("IRAI_KIN"))) '請求金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_KIN"))) '振替済金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FUNOU_KIN"))) '振替不能金額

                    For No As Integer = 1 To 15 '費目1～15請求件数
                        If oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H").Trim <> "" Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMO" & No.ToString & "_IRAI_KEN")))
                        Else
                            OutputCsvData("")
                        End If
                        HimokuKen(No) += GCOM.NzDec(oraReader.GetString("HIMO" & No.ToString & "_IRAI_KEN"))
                    Next

                    For No As Integer = 1 To 15 '費目1～15請求金額
                        If oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H").Trim <> "" Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMO" & No.ToString & "_IRAI_KIN")))
                        Else
                            OutputCsvData("")
                        End If
                        HimokuKin(No) += GCOM.NzDec(oraReader.GetString("HIMO" & No.ToString & "_IRAI_KIN"))
                    Next

                    For No As Integer = 1 To 15 '費目1～15振替済件数
                        If oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H").Trim <> "" Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMO" & No.ToString & "_FURI_KEN")))
                        Else
                            OutputCsvData("")
                        End If
                        HimokuFuriKen(No) += GCOM.NzDec(oraReader.GetString("HIMO" & No.ToString & "_FURI_KEN"))
                    Next

                    For No As Integer = 1 To 15 '費目1～15振替済金額
                        If oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H").Trim <> "" Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMO" & No.ToString & "_FURI_KIN")))
                        Else
                            OutputCsvData("")
                        End If
                        HimokuFuriKin(No) += GCOM.NzDec(oraReader.GetString("HIMO" & No.ToString & "_FURI_KIN"))
                    Next

                    For No As Integer = 1 To 15 '費目1～15振替不能件数
                        If oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H").Trim <> "" Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMO" & No.ToString & "_FUNOU_KEN")))
                        Else
                            OutputCsvData("")
                        End If
                        HimokuFunouKen(No) += GCOM.NzDec(oraReader.GetString("HIMO" & No.ToString & "_FUNOU_KEN"))
                    Next

                    For No As Integer = 1 To 15 '費目1～15振替不能金額
                        If oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H").Trim <> "" Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMO" & No.ToString & "_FUNOU_KIN")))
                        Else
                            OutputCsvData("")
                        End If
                        HimokuFunouKin(No) += GCOM.NzDec(oraReader.GetString("HIMO" & No.ToString & "_FUNOU_KIN"))
                    Next
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    ' 振替コード、企業コード追加（企業コードで改行するため改行用ダミーは削除）
                    'OutputCsvData("", False, True)  '改行用ダミー
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))                   '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), False, True)     '企業コード
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

                    RecordCnt += 1
                    oraReader.NextRead()
                End While
                '学校計出力
                oraReader.DataReader(SQL)   '項目取得のため再読み込み
                OutputCsvData(mMatchingDate)                                                '処理日
                OutputCsvData(mMatchingTime)                                                'タイムスタンプ
                OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))             '学校コード
                OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))            '学校名
                OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_W")))               '振替日
                OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_G")))            '学年
                OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))            '学年名

                OutputCsvData(GCOM.NzStr(oraReader.GetString("JIFURI_KBN_T")))            '自振区分
                OutputCsvData("1")            '学校集計
                For No As Integer = 1 To 15                                               '費目名1～15
                    If HimoNameUse(No) = "1" Then
                        OutputCsvData("費目" & No.ToString & "合計")
                    Else
                        OutputCsvData("")
                    End If
                Next
                OutputCsvData(GCOM.NzStr(KEN(1))) '請求件数
                OutputCsvData(GCOM.NzStr(KEN(2))) '振替済件数
                OutputCsvData(GCOM.NzStr(KEN(3))) '振替不能件数
                OutputCsvData(GCOM.NzStr(KIN(1))) '請求金額
                OutputCsvData(GCOM.NzStr(KIN(2))) '振替済金額
                OutputCsvData(GCOM.NzStr(KIN(3))) '振替不能金額
                For No As Integer = 1 To 15 '費目1～15請求件数
                    If HimoNameUse(No) = "1" Then
                        OutputCsvData(GCOM.NzStr(HimokuKen(No)))
                    Else
                        OutputCsvData("")
                    End If
                Next

                For No As Integer = 1 To 15 '費目1～15請求金額
                    If HimoNameUse(No) = "1" Then
                        OutputCsvData(GCOM.NzStr(HimokuKin(No)))
                    Else
                        OutputCsvData("")
                    End If
                Next

                For No As Integer = 1 To 15 '費目1～15振替済件数
                    If HimoNameUse(No) = "1" Then
                        OutputCsvData(GCOM.NzStr(HimokuFuriKen(No)))
                    Else
                        OutputCsvData("")
                    End If
                Next

                For No As Integer = 1 To 15 '費目1～15振替済金額
                    If HimoNameUse(No) = "1" Then
                        OutputCsvData(GCOM.NzStr(HimokuFuriKin(No)))
                    Else
                        OutputCsvData("")
                    End If
                Next

                For No As Integer = 1 To 15 '費目1～15振替不能件数
                    If HimoNameUse(No) = "1" Then
                        OutputCsvData(GCOM.NzStr(HimokuFunouKen(No)))
                    Else
                        OutputCsvData("")
                    End If
                Next

                For No As Integer = 1 To 15 '費目1～15振替不能金額
                    If HimoNameUse(No) = "1" Then
                        OutputCsvData(GCOM.NzStr(HimokuFunouKin(No)))
                    Else
                        OutputCsvData("")
                    End If
                Next
                ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                ' 振替コード、企業コード追加（企業コードで改行するため改行用ダミーは削除）
                'OutputCsvData("", False, True)  '改行用ダミー
                OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))                   '振替コード
                OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), False, True)     '企業コード
                ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

                RecordCnt += 1
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
End Class
