Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports MenteCommon.clsCommon
Public Class KFJP063
    Inherits CAstReports.ClsReportBase

    '自金庫コード
    Private Jikinko As String
    '自金庫名
    Private JikinkoNm As String
    '部署名
    Private Busyo As String
    'NHK コード
    Private NHKCODE As String

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP063"

        ' 定義体名セット
        ReportBaseName = "KFJP063_預金口座振替変更通知書.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Return MyBase.CreateCsvFile
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
        Dim SQL As New StringBuilder(128)    ' SQL
        MainDB = New CASTCommon.MyOracle ' オラクル
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim OraReader2 As New CASTCommon.MyOracleReader(MainDB) '2010/10/04.Sakon　追加
        Try
            'INIファイル取得
            '自金庫コード
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" Or Jikinko = "" Then
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return False
            End If
            '自金庫名
            JikinkoNm = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
            If JikinkoNm.ToUpper = "ERR" Or JikinkoNm = "" Then
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:自金庫名 分類:PRINT 項目:KINKONAME")
                Return False
            End If
            '部署名
            Busyo = CASTCommon.GetFSKJIni("PRINT", "KINKOBUSYO")
            If Busyo.ToUpper = "ERR" Then '空白は許可する
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:部署名 分類:PRINT 項目:KINKOBUSYO")
                Return False
            End If
            'NHK コード
            NHKCODE = CASTCommon.GetFSKJIni("COMMON", "NHKCD")
            If NHKCODE.ToUpper = "ERR" Then '空白は許可する
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:NHKコード 分類:COMMON 項目:NHKCD")
                Return False
            End If

            SQL = New StringBuilder(128)


            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '2010/02/08.Sakon　店舗統廃合考慮－各金融機関・店舗名取得は別途行う

            SQL.Append("SELECT")
            SQL.Append(" TORIMAST.ITAKU_CODE_T      ITAKU_CODE_TORIMAST ")      '会社コード  
            SQL.Append(",TORIMAST.ITAKU_NNAME_T     ITAKU_NNAME_TORIMAST ")     '会社名   
            SQL.Append(",TORIMAST.TSIT_NO_T         TSIT_NO ")                  '取りまとめ店番
            SQL.Append(",TORIMAST.FMT_KBN_T         FMT_KBN_TORIMAST ")         'フォーマット区分 
            SQL.Append(",MEIMAST.JYUYOUKA_NO_K      JYUYOUKA_NO_MEIMAST ")      '契約者番号    
            SQL.Append(",MEIMAST.KEIYAKU_KNAME_K    KEIYAKU_KNAME_MEIMAST ")    '契約者名     
            SQL.Append(",MEIMAST.KEIYAKU_KNAME_K    YOKINSYA_NAME ")            '預金者名    
            SQL.Append(",MEIMAST.KEIYAKU_SIT_K      KEIYAKU_SIT_MEIMAST ")      '変更前店番     
            SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K   KEIYAKU_KAMOKU_MEIMAST ")   '変更前種目     
            SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K    KEIYAKU_KOUZA_MEIMAST ")    '変更前口座番号    
            SQL.Append(",MEIMAST.TEISEI_SIT_K       TEISEI_SIT_MEIMAST ")       '変更後店番    
            SQL.Append(",MEIMAST.TEISEI_KAMOKU_K    TEISEI_KAMOKU_MEIMAST ")    '変更後種目     
            SQL.Append(",MEIMAST.TEISEI_KOUZA_K     TEISEI_KOUZA_MEIMAST")      '変更後口座番号   
            SQL.Append(",MEIMAST.TORIS_CODE_K       TORIS_CODE_MEIMAST ")       '取引先主コード   
            SQL.Append(",MEIMAST.TORIF_CODE_K       TORIF_CODE_MEIMAST ")       '取引先副コード
            SQL.Append(",MEIMAST.FURI_DATA_K        IRAI_DATA_MEIMAST ")        '依頼データ
            SQL.Append(" FROM ")
            SQL.Append(" MEIMAST ")
            SQL.Append(",TORIMAST ")
            SQL.Append(" WHERE ")
            SQL.Append(" MEIMAST.TORIS_CODE_K = TORIMAST.TORIS_CODE_T ")
            SQL.Append(" AND MEIMAST.TORIF_CODE_K = TORIMAST.TORIF_CODE_T ")
            SQL.Append(" AND MEIMAST.TORIS_CODE_K = '" & strToriSCd & "' ")
            SQL.Append(" AND MEIMAST.TORIF_CODE_K = '" & strToriFCd & "' ")
            SQL.Append(" AND MEIMAST.FURI_DATE_K = '" & strFuriDate & "' ")
            '国税の場合を考慮
            SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
            SQL.Append(" AND (NVL(TRIM(MEIMAST.TEISEI_SIT_K),'000') <> '000' ")
            SQL.Append("      OR NVL(TRIM(MEIMAST.TEISEI_KAMOKU_K),'00') <> '00' ")
            SQL.Append("      OR NVL(TRIM(MEIMAST.TEISEI_KOUZA_K),'0000000') <> '0000000') ")
            SQL.Append(" ORDER BY TORIMAST.TORIMATOME_SIT_T")
            SQL.Append(",MEIMAST.TORIS_CODE_K")
            SQL.Append(",MEIMAST.TORIF_CODE_K")
            SQL.Append(",MEIMAST.RECORD_NO_K")
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'SQL.Append("SELECT")
            'SQL.Append(" TORIMAST.ITAKU_CODE_T   ITAKU_CODE_TORIMAST ")     ' 会社コード  
            'SQL.Append(",TORIMAST.ITAKU_NNAME_T   ITAKU_NNAME_TORIMAST ")   ' 会社名   
            'SQL.Append(",TORIMATOME.SIT_NNAME_N   SIT_NNAME_TORIMATOME ")   ' 取りまとめ店     
            'SQL.Append(",MEIMAST.JYUYOUKA_NO_K   JYUYOUKA_NO_MEIMAST ")     ' 契約者番号    
            'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K   KEIYAKU_KNAME_MEIMAST ") ' 契約者名     
            'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K   YOKINSYA_NAME ")         ' 預金者名    
            'SQL.Append(",KEIYAKU.SIT_KNAME_N   SIT_KNAME_KEIYAKU ")         ' 変更前店名    
            'SQL.Append(",MEIMAST.KEIYAKU_SIT_K   KEIYAKU_SIT_MEIMAST ")     ' 変更前店番     
            'SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K   KEIYAKU_KAMOKU_MEIMAST ") ' 変更前種目     
            'SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K   KEIYAKU_KOUZA_MEIMAST ") ' 変更前口座番号    
            'SQL.Append(",TEISEI.SIT_KNAME_N   SIT_KNAME_TEISEI ")           ' 変更後店名   
            'SQL.Append(",MEIMAST.TEISEI_SIT_K   TEISEI_SIT_MEIMAST ")       ' 変更後店番    
            'SQL.Append(",MEIMAST.TEISEI_KAMOKU_K   TEISEI_KAMOKU_MEIMAST ") ' 変更後種目     
            'SQL.Append(",MEIMAST.TEISEI_KOUZA_K    TEISEI_KOUZA_MEIMAST")   ' 変更後口座番号   
            'SQL.Append(",MEIMAST.TORIS_CODE_K   TORIS_CODE_MEIMAST ")       ' 取引先主コード   
            'SQL.Append(",MEIMAST.TORIF_CODE_K   TORIF_CODE_MEIMAST ")       ' 取引先副コード
            'SQL.Append(",TORIMAST.FMT_KBN_T   FMT_KBN_TORIMAST ")           'フォーマット区分 
            'SQL.Append(",MEIMAST.FURI_DATA_K   IRAI_DATA_MEIMAST ")         '依頼データ
            'SQL.Append(" FROM ")
            'SQL.Append(" MEIMAST ")
            'SQL.Append(",TORIMAST ")
            'SQL.Append(",TENMAST TORIMATOME ")
            'SQL.Append(",TENMAST KEIYAKU ")
            'SQL.Append(",TENMAST TEISEI ")
            'SQL.Append(" WHERE ")
            'SQL.Append(" MEIMAST.TORIS_CODE_K = TORIMAST.TORIS_CODE_T ")
            'SQL.Append(" AND MEIMAST.TORIF_CODE_K = TORIMAST.TORIF_CODE_T ")
            'SQL.Append(" AND MEIMAST.FURI_DATE_K = '" & FURI_DATE & "' ")
            ''国税の場合を考慮
            'SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
            'SQL.Append(" AND (MEIMAST.TEISEI_SIT_K <> '000' ")
            'SQL.Append("      OR MEIMAST.TEISEI_KAMOKU_K <> '00' ")
            'SQL.Append("      OR MEIMAST.TEISEI_KOUZA_K <> '0000000') ")
            'SQL.Append(" AND '" & Jikinko & "' = TORIMATOME.KIN_NO_N ")
            'SQL.Append(" AND TORIMAST.TORIMATOME_SIT_T = TORIMATOME.SIT_NO_N ")
            'SQL.Append(" AND '" & Jikinko & "' = KEIYAKU.KIN_NO_N ")
            'SQL.Append(" AND MEIMAST.KEIYAKU_SIT_K = KEIYAKU.SIT_NO_N ")
            'SQL.Append(" AND '" & Jikinko & "' = TEISEI.KIN_NO_N ")
            'SQL.Append(" AND MEIMAST.TEISEI_SIT_K = TEISEI.SIT_NO_N(+) ")
            'SQL.Append(" ORDER BY TORIMAST.TORIMATOME_SIT_T")
            'SQL.Append(",MEIMAST.TORIS_CODE_K")
            'SQL.Append(",MEIMAST.TORIF_CODE_K")
            'SQL.Append(",MEIMAST.RECORD_NO_K")

        Catch ex As Exception
            BatchLog.Write("", "エラー", ex.Message)
        End Try

        If OraReader.DataReader(SQL) = True Then
            Try
                Do
                    '処理日
                    OutputCsvData(mMatchingDate)
                    '自金庫コード
                    OutputCsvData(Jikinko)
                    '自金庫名
                    OutputCsvData(JikinkoNm)
                    '部署名
                    OutputCsvData(Busyo)
                    '会社コード	
                    OutputCsvData(OraReader.GetString("ITAKU_CODE_TORIMAST"))
                    '会社名	
                    OutputCsvData(OraReader.GetString("ITAKU_NNAME_TORIMAST"))
                    '取りまとめ店	
                    '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++
                    OutputCsvData(GetTenName(OraReader.GetString("TSIT_NO"), OraReader2))
                    'OutputCsvData(OraReader.GetString("SIT_NNAME_TORIMATOME"))
                    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '区分
                    OutputCsvData("ヘンコウ")
                    '契約者番号	
                    OutputCsvData(OraReader.GetString("JYUYOUKA_NO_MEIMAST"))
                    Dim IRAI_DATA As String = OraReader.GetString("IRAI_DATA_MEIMAST", False)

                    Select Case OraReader.GetString("FMT_KBN_TORIMAST")
                        Case "01", "06" '地公体
                            '取得した依頼データから、義務者名を取得
                            Dim KEIYAKU_KNAME As String = IRAI_DATA.Substring(198, 30)

                            '契約者名1	
                            OutputCsvData(KEIYAKU_KNAME.Substring(0, 15))
                            '契約者名2	
                            OutputCsvData(KEIYAKU_KNAME.Substring(15))
                        Case Else
                            'NHK専用契約者名印字
                            If OraReader.GetString("ITAKU_CODE_TORIMAST").Equals(NHKCODE) Then
                                Dim KEIYAKU_KNAME As String = IRAI_DATA.Substring(65, 15).Trim
                                '契約者名1	
                                OutputCsvData(KEIYAKU_KNAME)
                                '契約者名2	
                                OutputCsvData("")
                            Else
                                '契約者名1	
                                OutputCsvData("")
                                '契約者名2	
                                OutputCsvData("")
                            End If
                    End Select

                    ' NHK専用預金者名印字
                    If OraReader.GetString("ITAKU_CODE_TORIMAST").Equals(NHKCODE) Then
                        Dim YOKINSYA_NAME As String = IRAI_DATA.Substring(50, 15).Trim
                        '預金者名1	
                        OutputCsvData(YOKINSYA_NAME)
                        '預金者名2	
                        OutputCsvData("")
                    Else
                        '預金者名1	
                        OutputCsvData(OraReader.GetString("YOKINSYA_NAME"))
                        '預金者名2	
                        OutputCsvData("")
                    End If

                    '変更前店番	
                    OutputCsvData(OraReader.GetString("KEIYAKU_SIT_MEIMAST"))
                    '変更前店名
                    '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    OutputCsvData(GetTenName(OraReader.GetString("KEIYAKU_SIT_MEIMAST"), OraReader2))
                    'OutputCsvData(OraReader.GetString("SIT_KNAME_KEIYAKU"))
                    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '変更前種目	
                    OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("KEIYAKU_KAMOKU_MEIMAST")))
                    '変更前口座番号	
                    OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_MEIMAST"))

                    If OraReader.GetString("TEISEI_SIT_MEIMAST").Equals("000") Then
                        '変更前店番
                        OutputCsvData(OraReader.GetString("KEIYAKU_SIT_MEIMAST"))
                        '変更前店名
                        '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        OutputCsvData(GetTenName(OraReader.GetString("KEIYAKU_SIT_MEIMAST"), OraReader2))
                        'OutputCsvData(OraReader.GetString("SIT_KNAME_KEIYAKU"))
                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    Else
                        '変更後店番
                        OutputCsvData(OraReader.GetString("TEISEI_SIT_MEIMAST"))
                        '変更後店名
                        '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        OutputCsvData(GetTenName(OraReader.GetString("TEISEI_SIT_MEIMAST"), OraReader2))
                        'OutputCsvData(OraReader.GetString("SIT_KNAME_TEISEI"))
                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    End If

                    '2011/06/16 標準版修正 店舗のみ変更の場合を考慮 ------------------START
                    '変更後種目
                    '変換後科目・・・00の場合は""何も印字しない。  
                    If OraReader.GetString("TEISEI_KAMOKU_MEIMAST").Equals("00") Then
                        OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("KEIYAKU_KAMOKU_MEIMAST")))
                    Else
                        OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("TEISEI_KAMOKU_MEIMAST")))
                    End If

                    '変更後口座番号	
                    If OraReader.GetString("TEISEI_KOUZA_MEIMAST").Equals("0000000") Then
                        OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_MEIMAST"))
                    Else
                        OutputCsvData(OraReader.GetString("TEISEI_KOUZA_MEIMAST"))
                    End If

                    ''変更後種目
                    ''変換後科目・・・00の場合は""何も印字しない。  
                    'If OraReader.GetString("TEISEI_KAMOKU_MEIMAST").Equals("00") Then
                    '    OutputCsvData("")
                    'Else
                    '    OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("TEISEI_KAMOKU_MEIMAST")))
                    'End If

                    ''変更後口座番号	
                    'OutputCsvData(OraReader.GetString("TEISEI_KOUZA_MEIMAST"))
                    '2011/06/16 標準版修正 店舗のみ変更の場合を考慮 ------------------END
                    '該当データ有無	
                    OutputCsvData("")
                    '取引先主副	
                    OutputCsvData(OraReader.GetString("TORIS_CODE_MEIMAST") & OraReader.GetString("TORIF_CODE_MEIMAST"), False, True)
                    RecordCnt += 1
                    OraReader.NextRead()
                Loop Until OraReader.EOF
                OraReader.Close()
            Catch ex As Exception
                BatchLog.Write("データ検索", "失敗", ex.Message)
            End Try
            Return True
        Else
            '念のため残す ===============================================
            'If name = "" Then
            '    ' ＣＳＶを作成する
            '    name = List.CreateCsvFile
            'End If
            'OutputCsvData("")    '会社コード
            'OutputCsvData("")    '会社名
            'OutputCsvData("")    '取りまとめ店
            'OutputCsvData("")    '区分
            'OutputCsvData("")    '契約者番号
            ''契約者名、預金者名を二段に分けて印字
            'OutputCsvData("")    '契約者名1
            'OutputCsvData("")    '契約者名2
            'OutputCsvData("")    '預金者名1
            'OutputCsvData("")    '預金者名2
            'OutputCsvData("")    '変更前店番
            'OutputCsvData("")    '変更前店名
            'OutputCsvData("")    '変更前種目
            'OutputCsvData("")    '変更前口座番号
            'OutputCsvData("")    '変更後店番
            'OutputCsvData("")    '変更後店名
            'OutputCsvData("")    '変更後種目
            'OutputCsvData("")    '変更後口座番号
            'OutputCsvData("該当データなし")    '該当データ有無
            'OutputCsvData("", False, True)    '取引先主副
            '==============================================================
            RecordCnt = -1
            BatchLog.Write("印刷", "成功", "印刷対象0件")
            Return False
        End If
    End Function

    '========================================================================================
    ' 機　能：店舗名取得
    ' 引　数：支店コード
    ' 戻り値：店舗名
    ' 備　考：
    ' 作　成：2010/10/04
    ' 更　新：
    '========================================================================================
    Private Function GetTenName(ByVal pstrSitNo As String, ByVal OraReader2 As CASTCommon.MyOracleReader) As String
        Dim SQL As New StringBuilder(128)    ' SQL

        SQL = New StringBuilder(128)

        SQL.Append("SELECT")
        SQL.Append(" SIT_NNAME_N")
        SQL.Append(" FROM TENMAST")
        SQL.Append(" WHERE KIN_NO_N = '" & Jikinko & "'")
        SQL.Append(" AND SIT_NO_N = '" & pstrSitNo.Trim & "'")

        If OraReader2.DataReader(SQL) = True Then
            Return OraReader2.GetString("SIT_NNAME_N")
        Else
            Return ""
        End If

    End Function

End Class
