Imports System
Imports System.IO
Imports System.Text
Imports System.Diagnostics
Imports System.Collections

Imports CAstBatch
Imports CAstSystem
Imports CASTCommon.ModPublic

' 伝送連携処理
Public Class ClsDensouRenkei

    ' ログ処理クラス
    Private LOG As CASTCommon.BatchLOG

    Private mDenParam As CAstSystem.ClsDensou.DENSOUPARAM

    ' 起動パラメータ 共通情報
    Private mArgumentData As CommData

    ' 依頼データファイル名
    Private mDataFileName As String

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    ' メッセージ
    Public Message As String = ""

    ' FSKJ.INI セクション名
    Private ReadOnly AppTOUROKU As String = "OTHERSYS"

    ' New
    Public Sub New()
    End Sub

    ' 機能　 ： メイン
    '
    ' 戻り値 ： 0 - 正常 ， 0以外 - 異常
    '
    ' 備考　 ： 
    Public Function Main(ByVal command As String) As Integer
        Dim oDenso As New CAstSystem.ClsDensou  ' 業務ヘッダ

        ' ジョブ通番設定
        LOG = New CASTCommon.BatchLOG("他シス連携", AppTOUROKU)
        LOG.ToriCode = ""

        ' 業務ヘッダ
        oDenso.GyoumuHeadName = command

        ' 業務ヘッダファイル読込
        Try
            ' パラメータ分解
            mDenParam.Data = oDenso.ReadHeader
        Catch ex As Exception
            ' 業務ヘッダ読み込み失敗
            Message = "伝送業務ヘッダ読み込み 失敗"
            Call LOG.Write("伝送業務ヘッダ読み込み", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
            Return 1
        End Try

        ' オラクル
        MainDB = New CASTCommon.MyOracle

        ' 起動パラメータ共通情報
        mArgumentData = New CommData(MainDB)

        ' パラメータ情報を設定
        Dim InfoParam As CommData.stPARAMETER
        InfoParam.TORI_CODE = ""
        InfoParam.BAITAI_CODE = ""
        ' レコード長からフォーマット区分を判定する
        Select Case CASTCommon.CAInt32(mDenParam.RecordLen)
            Case 120    ' 全銀
                InfoParam.FMT_KBN = "00"
            Case 350    ' 地公体３５０
                InfoParam.FMT_KBN = "01"
            Case 390    ' 国税
                InfoParam.FMT_KBN = "02"
            Case 130    ' 年金
                InfoParam.FMT_KBN = "03"
            Case 300    ' 地公体３００
                InfoParam.FMT_KBN = "06"
            Case 165    ' センタ返還ファイル
                InfoParam.FMT_KBN = "MT"
            Case 210    ' ＳＳＣ結果データ
                InfoParam.FMT_KBN = "SC"
            Case Else
                InfoParam.FMT_KBN = "00"
        End Select

        LOG.Write("レコード長取得", "成功", "レコード長：" & mDenParam.RecordLen & " フォーマット区分：" & InfoParam.FMT_KBN)

        InfoParam.FURI_DATE = ""
        InfoParam.CODE_KBN = ""
        InfoParam.FSYORI_KBN = ""
        InfoParam.JOBTUUBAN = 0
        InfoParam.RENKEI_KBN = "00"     ' 伝送固定
        InfoParam.RENKEI_FILENAME = mDenParam.FileName
        InfoParam.ENC_KBN = ""
        InfoParam.ENC_KEY1 = ""
        InfoParam.ENC_KEY2 = ""
        InfoParam.ENC_OPT1 = ""
        InfoParam.CYCLENO = ""
        mArgumentData.INFOParameter = InfoParam

        ' 連携用クラス作成
        Dim oRenkei As New CAstSystem.ClsRenkei(mArgumentData)

        Dim bRet As Boolean
        ' 連携処理
        bRet = MainDensou(oRenkei)
        If bRet = False Then
            ' ロールバック
            MainDB.Rollback()
        Else
            ' コミット
            MainDB.Commit()
        End If

        MainDB.Close()

        If bRet = False Then
            Return 2
        End If
        Return 0

    End Function

    ' 機能　 ： 伝送
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    ' encode.exe  -I "C:\ZEN01.dat" -l "120" -O "C:\ZEN01.dat.AES"    -a 8          -k "31323334353637383930313233343536" -v "31323334353637383930313233343536" -rl "120" -t 1 -b "120" -g 1 -m 0 -ak 0 -p 0 > "D:\WORK\AES.LOG"
    ' encode.exe  -I "C:\ZEN01.dat" -l "120" -O "C:\ZEN01.dat.暗号化" -a 2 -n "256" -k "3132333435363738"                 -v "3132333435363738"                           -t 0          -g 0                 > "D:\WORK\ENCODE.LOG"
    ' decode.exe  -I "C:\ZEN01.dat.AES"      -O "C:\ZEN01.dat.暗号化,復号化.DAT"    -k "31323334353637383930313233343536" -v "31323334353637383930313233343536" -lf 0                                        > "D:\WORK\DECODE.LOG"
    '
    Private Function MainDensou(ByVal renkei As ClsRenkei) As Boolean
        LOG.Write("伝送連携開始", "成功")

        Try
            Dim DensouPath As String = CASTCommon.GetFSKJIni(AppTOUROKU, "DENSOUREAD").Replace("%yyyyMMdd%", CASTCommon.Calendar.Now.ToString("yyyyMMdd"))
            renkei.InFileName = Path.Combine(DensouPath, mDenParam.FileName)
            LOG.Write("伝送ファイル取得", "成功", renkei.InFileName)

            ' 伝送データ取得成功
            ' ＪＯＢ登録
            If TourokuFile(renkei) = True Then
                LOG.Write("伝送データ登録", "成功", "入力ファイル：" & renkei.InFileName)
            Else
                If Message = "" Then
                    Message = "伝送データ登録 失敗 入力ファイルなし：" & renkei.InFileName
                    LOG.Write("伝送データ登録", "失敗", "入力ファイルなし：" & renkei.InFileName)
                End If
                Return False
            End If
        Catch ex As Exception
            Message = "伝送 失敗"
            Call LOG.Write("伝送", "失敗", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
        End Try

        LOG.Write("伝送連携終了", "成功")

        Return True
    End Function

    ' 機能　 ： ファイルから情報を読み込んでＪＯＢへ登録する
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： メディアコンバータ，学校，ＣＭＴ 
    '
    Private Function TourokuFile(ByRef oRenkei As ClsRenkei) As Boolean

        '媒体読み込み
        'フォーマット　
        Dim oReadFMT As CAstFormat.CFormat
        Dim FSyoriKbn As String = "1"
        Dim FormatKbn As String = mArgumentData.INFOParameter.FMT_KBN
        Dim FuriDate As String

        Try
            ' フォーマット区分から，フォーマットを特定する
            oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
            oReadFMT.ToriData = mArgumentData

            ' 伝送ファイルをローカルにコピーする
            Dim sWorkFile As String = oRenkei.CopyToWork()
            If sWorkFile = oRenkei.InFileName Then
                ' 伝送ファイルなし
                Return False
            End If
            If File.Exists(sWorkFile) = False Then
                sWorkFile = oRenkei.InFileName
            End If

            If mArgumentData.INFOParameter.FMT_KBN = "SC" Then
                ' ＳＳＣ結果 ＪＯＢ登録
                Call InsertJOBMAST("E207", sWorkFile)
                oReadFMT.Close()
                oReadFMT = Nothing
                Return True
            End If

            ' 入力ファイルをコピーする
            If mDenParam.EncodeKubun = "1" Then
                ' 復号化してコピー
                oRenkei.InFileName = sWorkFile
                Dim dmmyFile As String = oRenkei.FileDecodeMove(mDenParam.RecordLen, mDenParam.AES, mDenParam.EncodeKey, mDenParam.EncodeVIKey)

                If File.Exists(dmmyFile) = False Then
                    Message = "復号化 失敗 " & oRenkei.Message
                    LOG.Write("複合化", "失敗", "複合化に失敗しました。" & mDenParam.FileName)
                    Return False
                Else
                    sWorkFile = dmmyFile
                End If
            Else
                ' そのまま使用
            End If

            Dim JobID As String
            Dim Param() As String
            ' 2008.04.21 ADD >>
            Dim OutFileName As String = ""
            ' 2008.04.21 ADD <<

            If FormatKbn = "MT" Then
                ' センタ不能明細ＭＴ

                ' 振替処理区分，フォーマット区分のフォルダーへ移動する
                Dim oKekkaRenkei As New ClsRenkei(oRenkei.InfoArg, 1)       ' 結果用連携クラス 
                'Dim OutFileName As String
                OutFileName = oKekkaRenkei.MoveToGet(sWorkFile)
                LOG.Write("伝送ファイルコピー", "成功", mDenParam.FileName & "->" & OutFileName)

                ' 結果更新用パラメータ
                Param = GetParam(OutFileName)

                ' 結果更新処理
                JobID = "K101"

            Else
                ' 依頼データ，元受不能データ

                '*** 修正 mitsu 2008/08/25 委託者コード読替処理 ***
                '全銀フォーマットの場合のみ
                If FormatKbn = "00" AndAlso oRenkei.ConvertItakuCode(sWorkFile, LOG) = False Then
                    Message = oRenkei.Message
                    Return False
                End If
                '**************************************************

                ' 伝送ファイルから情報を読み込む
                If oReadFMT.FirstRead(sWorkFile) = 1 Then
                    Call oReadFMT.CheckDataFormat()
                    ' 2008.04.21 ADD 変なデータ対応 >>
                    Call oReadFMT.CheckRecord1()
                    ' 2008.04.21 ADD 変なデータ対応 <<

                    ' 振替日判定
                    FuriDate = oReadFMT.InfoMeisaiMast.FURIKAE_DATE
                    Dim dFuriDate As Date = ConvertDate(FuriDate)
                    Dim d10MaeDate As Date
                    Dim Fmt As New CAstFormat.CFormat
                    Fmt.Oracle = MainDB
                    d10MaeDate = Fmt.GetEigyobiFmt(dFuriDate, -10)
                    If dFuriDate.Subtract(d10MaeDate).Days >= 30 Then
                        ' 落し込み処理
                        JobID = "T101"
                    Else
                        If Not (FuriDate Is Nothing) And CASTCommon.Calendar.Now >= dFuriDate And dFuriDate >= d10MaeDate Then
                            ' 結果更新処理
                            JobID = "K101"
                        Else
                            ' 落し込み処理
                            JobID = "T101"
                        End If
                    End If

                    If JobID = "T101" Then
                        Select Case FormatKbn
                            Case "02"
                                ' 国税の場合
                                Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
                                KokuzeiFmt.KOKUZEI_REC1.Data = oReadFMT.RecordData
                                Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
                                KokuzeiFmt.Close()
                                Dim TORICODE As String
                                ' 科目コード　020:申告所得税, 300:消費税及地方消費税
                                TORICODE = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
                                If TORICODE = "err" Then
                                    TORICODE = ""
                                End If

                                Call oReadFMT.GetTorimastFromToriCode(TORICODE, MainDB)
                                If Not oReadFMT.ToriData Is Nothing Then
                                    oReadFMT.InfoMeisaiMast.ITAKU_CODE = oReadFMT.ToriData.INFOToriMast.ITAKU_CODE_T
                                End If
                            Case "03"
                                ' 年金の場合

                                Dim NenkinFmt As New CAstFormat.CFormatNenkin
                                NenkinFmt.NENKIN_REC1.Data = oReadFMT.RecordData
                                Dim Syubetu As String = NenkinFmt.NENKIN_REC1.NK2
                                NenkinFmt.Close()

                                Dim TORICODE As String
                                ' 年金種別から判断 61:旧厚生年金,62:旧船員年金,63:旧国民年金,64:労災年金,65:新国民年金・厚生年金,66:新船員年金,67:旧国民年金短期
                                TORICODE = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & Syubetu)
                                If TORICODE = "err" Then
                                    TORICODE = ""
                                    LOG.Write("年金　取引先コード取得", "失敗", "科目：" & Syubetu)
                                Else
                                    LOG.ToriCode = TORICODE
                                    LOG.Write("年金　取引先コード取得", "成功", "科目：" & Syubetu)
                                End If

                                Call oReadFMT.GetTorimastFromToriCode(TORICODE, MainDB)
                                If Not oReadFMT.ToriData Is Nothing Then
                                    oReadFMT.InfoMeisaiMast.ITAKU_CODE = oReadFMT.ToriData.INFOToriMast.ITAKU_CODE_T
                                End If
                            Case Else
                                ' 委託者コードから，取引先マスタ情報を取得する
                                Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                        End Select
                        'Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                        FSyoriKbn = oReadFMT.ToriData.INFOToriMast.FSYORI_KBN_T
                        FormatKbn = oReadFMT.ToriData.INFOToriMast.FMT_KBN_T
                    Else
                        ' 結果更新　判定
                        ' 自金庫コード
                        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
                        If oReadFMT.InfoMeisaiMast.ITAKU_KIN = Jikinko Then
                            ' SSS委託者検索
                            If oReadFMT.GetTorimastFromItakuCodeSSS(MainDB) = False Then
                                ' 取引先が見つからないので，落し込み処理を実行
                                JobID = "T101"
                                Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                            End If
                        Else
                            If oReadFMT.GetTorimastFromItakuCodeTAKO(MainDB) = False Then
                                ' 取引先が見つからないので，落し込み処理を実行
                                JobID = "T101"
                                Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                            End If
                        End If
                        FSyoriKbn = oReadFMT.ToriData.INFOToriMast.FSYORI_KBN_T
                        FormatKbn = oReadFMT.ToriData.INFOToriMast.FMT_KBN_T
                    End If

                Else
                    ' 落し込み処理
                    JobID = "T101"
                End If
                oReadFMT.Close()

                Dim InfoParam As CommData.stPARAMETER = mArgumentData.INFOParameter
                InfoParam.FSYORI_KBN = FSyoriKbn
                InfoParam.FMT_KBN = FormatKbn
                mArgumentData.INFOParameter = InfoParam

                ' ＪＯＢマスタ正常登録
                If JobID = "T101" Then
                    ' 振替処理区分，フォーマット区分のフォルダーへ移動する
                    ' 2008.04.21 DELETE >>
                    'Dim OutFileName As String
                    ' 2008.04.21 DELETE <<
                    OutFileName = oRenkei.MoveToGet(sWorkFile)
                    LOG.Write("伝送ファイルコピー", "成功", mDenParam.FileName & "->" & OutFileName)

                    ' 落し込み用パラメータ
                    Param = GetParam(oReadFMT, OutFileName)
                Else
                    Dim oKekkaRenkei As New ClsRenkei(oRenkei.InfoArg, 1)       ' 結果用連携クラス
                    ' 2008.04.21 DELETE >>
                    'Dim OutFileName As String
                    ' 2008.04.21 DELETE <<
                    OutFileName = oKekkaRenkei.MoveToGet(sWorkFile)
                    LOG.Write("伝送ファイルコピー", "成功", sWorkFile & "->" & OutFileName)

                    ' 結果更新用パラメータ
                    Param = GetParamZenginKekka(oReadFMT, OutFileName)
                End If

                ' 2008.04.21 ADD 取引先情報取得失敗処理 >>
                If Param.Length >= 1 And Param(0).StartsWith("000000000") = True Then
                    If File.Exists(OutFileName) = True Then
                        File.Delete(OutFileName)
                    End If
                    OutFileName = oRenkei.InFileName

                    ' 取引先情報取得失敗
                    Message = "取引先情報取得失敗 委託者コード：" & oReadFMT.InfoMeisaiMast.ITAKU_CODE & " ファイル名：" & OutFileName
                    LOG.Write("取引先情報取得", "失敗", "委託者コード：" & oReadFMT.InfoMeisaiMast.ITAKU_CODE & " ファイル名:" & OutFileName)

                    Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                    If Prn.OutputCSVKekkaSysError(oReadFMT.InfoMeisaiMast.ITAKU_CODE, mArgumentData.INFOParameter.FSYORI_KBN, OutFileName, MainDB) = False Then
                        LOG.Write("処理結果確認表", "失敗", "ファイル名:" & Prn.FileName & " メッセージ:" & Prn.ReportMessage)
                    End If
                    Prn = Nothing
                    oReadFMT.Close()
                    Return False
                End If
                ' 2008.04.21 ADD 取引先情報取得失敗処理 <<
            End If

            If InsertJOBMAST(Param, JobID) = True Then
                If (oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing) Then
                    LOG.Write("依頼データ読み取り", "成功")
                Else
                    LOG.Write("依頼データ読み取り", "成功", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                End If
            Else
                Message = "依頼データ読み取り 失敗 委託者コード：" & oReadFMT.InfoMeisaiMast.ITAKU_CODE
                LOG.Write("依頼データ読み取り", "失敗", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                Return False
            End If
            oReadFMT = Nothing
        Catch ex As Exception
            MainDB.Rollback()

            If Not oReadFMT Is Nothing Then
                oReadFMT.Close()
            End If
            Message = "依頼書データジョブ登録 失敗"
            LOG.Write("依頼データジョブ登録", "失敗", ex.Message & ":" & ex.StackTrace)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： ＪＯＢＭＡＳＴ作成
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertJOBMAST(ByVal param() As String, Optional ByVal jobId As String = "T101") As Boolean
        Dim SQL As New StringBuilder(128)

        SQL.Append("INSERT INTO JOBMAST(")
        SQL.Append(" TOUROKU_DATE_J")
        SQL.Append(",TOUROKU_TIME_J")
        SQL.Append(",STA_DATE_J")
        SQL.Append(",STA_TIME_J")
        SQL.Append(",END_DATE_J")
        SQL.Append(",END_TIME_J")
        SQL.Append(",JOBID_J")
        SQL.Append(",STATUS_J")
        SQL.Append(",USERID_J")
        SQL.Append(",PARAMETA_J")
        SQL.Append(",ERRMSG_J")
        SQL.Append(",DENSO_CNT_CODE_J")
        SQL.Append(",TOHO_CNT_CODE_J")
        SQL.Append(",ZENGIN_F_NAME_J")
        SQL.Append(",DENSO_F_NAME_J")
        SQL.Append(",DENSO_TIME_STAMP_J")
        SQL.Append(") VALUES (")
        SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
        SQL.Append(",'00000000'")                                           ' STA_DATE_J
        SQL.Append(",'000000'")                                             ' STA_TIME_J
        SQL.Append(",'00000000'")                                           ' END_DATE_J
        SQL.Append(",'000000'")                                             ' END_TIME_J
        SQL.Append("," & SQ(jobId))                                         ' JOBID_J
        SQL.Append(",'0'")                                                  ' STATUS_J
        SQL.Append("," & SQ(LOG.UserID))                                      ' USERID_J
        Dim work() As String
        If param.Length = 18 Then
            ' 落とし込み用
            '*** 修正 mitsu 2008/09/30 処理高速化 ***
            'work = CType(Array.CreateInstance(GetType(String), 13), String())
            work = New String(12) {}
            '****************************************
            Array.Copy(param, work, 13)
        Else
            ' 結果更新用
            '*** 修正 mitsu 2008/09/30 処理高速化 ***
            'work = CType(Array.CreateInstance(GetType(String), param.Length - 6), String())
            work = New String(param.Length - 7) {}
            '****************************************
            Array.Copy(param, work, param.Length - 6)
        End If
        Dim sJoin As String = String.Join(",", work)
        SQL.Append("," & SQ(sJoin))                                         ' PARAMETA_J
        SQL.Append(",' '")                                                  ' ERRMSG_J
        SQL.Append("," & SQ(param(param.Length - 5)))                       ' DENSO_CNT_CODE_J
        SQL.Append("," & SQ(param(param.Length - 4)))                       ' TOHO_CNT_CODE_J
        SQL.Append("," & SQ(param(param.Length - 3)))                       ' ZENGIN_F_NAME_J
        SQL.Append("," & SQ(param(param.Length - 2)))                       ' DENSO_F_NAME_J
        SQL.Append("," & SQ(param(param.Length - 1)))                       ' DENSO_TIME_STAMP_S
        SQL.Append(")")

        '*** 修正 mitsu 2008/08/04 失敗時リトライする ***
        'If MainDB.ExecuteNonQuery(SQL) <= 0 Then
        '    Message = "JOBMAST登録 失敗"
        '    LOG.Write("JOBMAST登録", "失敗", MainDB.Message)
        '    Return False
        'End If
        Dim cnt As Integer = 0
        While True
            Try
                If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                    Message = "JOBMAST登録 失敗"
                    LOG.Write("JOBMAST登録", "失敗", MainDB.Message)
                    Return False
                Else
                    '成功時は抜ける
                    Exit While
                End If

            Catch ex As Exception
                cnt += 1
                '3回以上失敗時は強制終了
                If cnt >= 3 Then
                    Message = "JOBMAST登録 失敗 " & ex.Message
                    LOG.Write("JOBMAST登録", "失敗", MainDB.Message)
                    Return False
                End If
                '0.5秒待機
                Threading.Thread.Sleep(500)
            End Try
        End While
        '************************************************

        Return True
    End Function

    ' 機能　 ： ＪＯＢＭＡＳＴ作成
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertJOBMAST(ByVal jobId As String, ByVal fileName As String) As Boolean
        Dim SQL As New StringBuilder(128)

        SQL.Append("INSERT INTO JOBMAST(")
        SQL.Append(" TOUROKU_DATE_J")
        SQL.Append(",TOUROKU_TIME_J")
        SQL.Append(",STA_DATE_J")
        SQL.Append(",STA_TIME_J")
        SQL.Append(",END_DATE_J")
        SQL.Append(",END_TIME_J")
        SQL.Append(",JOBID_J")
        SQL.Append(",STATUS_J")
        SQL.Append(",USERID_J")
        SQL.Append(",PARAMETA_J")
        SQL.Append(",ERRMSG_J")
        SQL.Append(",DENSO_CNT_CODE_J")
        SQL.Append(",TOHO_CNT_CODE_J")
        SQL.Append(",ZENGIN_F_NAME_J")
        SQL.Append(",DENSO_F_NAME_J")
        SQL.Append(",DENSO_TIME_STAMP_J")
        SQL.Append(") VALUES (")
        SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
        SQL.Append(",'00000000'")                                           ' STA_DATE_J
        SQL.Append(",'000000'")                                             ' STA_TIME_J
        SQL.Append(",'00000000'")                                           ' END_DATE_J
        SQL.Append(",'000000'")                                             ' END_TIME_J
        SQL.Append("," & SQ(jobId))                                         ' JOBID_J
        SQL.Append(",'0'")                                                  ' STATUS_J
        SQL.Append("," & SQ(LOG.UserID))                                      ' USERID_J
        SQL.Append("," & SQ("""" & fileName & """"))                        ' PARAMETA_J
        SQL.Append(",' '")                                                  ' ERRMSG_J
        SQL.Append("," & SQ(mDenParam.CenterCode))                          ' DENSO_CNT_CODE_J
        SQL.Append("," & SQ(mDenParam.TouhouCode))                          ' TOHO_CNT_CODE_zJ
        SQL.Append("," & SQ(mDenParam.ZenginName))                          ' ZENGIN_F_NAME_J
        SQL.Append("," & SQ(mDenParam.FileName))                            ' DENSO_F_NAME_J
        SQL.Append("," & SQ(mDenParam.DensouNitiji))                        ' DENSO_TIME_STAMP_S
        SQL.Append(")")

        '*** 修正 mitsu 2008/08/04 失敗時リトライする ***
        'If MainDB.ExecuteNonQuery(SQL) <= 0 Then
        '    Message = "JOBMAST登録 失敗"
        '    LOG.Write("JOBMAST登録", "失敗", MainDB.Message)
        '    Return False
        'End If
        Dim cnt As Integer = 0
        While True
            Try
                If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                    Message = "JOBMAST登録 失敗"
                    LOG.Write("JOBMAST登録", "失敗", MainDB.Message)
                    Return False
                Else
                    '成功時は抜ける
                    Exit While
                End If

            Catch ex As Exception
                cnt += 1
                '3回以上失敗時は強制終了
                If cnt >= 3 Then
                    Message = "JOBMAST登録 失敗 " & ex.Message
                    LOG.Write("JOBMAST登録", "失敗", MainDB.Message)
                    Return False
                End If
                '0.5秒待機
                Threading.Thread.Sleep(500)
            End Try
        End While
        '************************************************

        Return True
    End Function

    ' 機能　 ： 結果更新用パラメータ作成（センタ不能結果）
    '
    ' 戻り値 ： パラメータ
    '
    ' 備考　 ： 
    '
    Private Function GetParam(ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' フォーマット区分
        Param.Add(mArgumentData.INFOParameter.FMT_KBN)

        ' 連携区分
        Param.Add(mArgumentData.INFOParameter.RENKEI_KBN)

        ' 更新キー：企業シーケンス
        Param.Add("0")

        ' 連携ファイル名
        Param.Add(Path.GetFileName(filename).TrimEnd)

        ' ----- 上記までがパラメータ

        '' サイクル№
        Param.Add(mDenParam.HostTuuban)

        ' 相手センタ確認コード
        Param.Add(mDenParam.CenterCode)

        ' 当方センター確認コード
        Param.Add(mDenParam.TouhouCode)

        ' 全銀ファイル名
        Param.Add(mDenParam.ZenginName)

        ' 伝送ファイル名
        Param.Add(mDenParam.FileName)

        ' 伝送日時
        Param.Add(mDenParam.DensouNitiji)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' 機能　 ： 結果更新用パラメータ作成（他行，ＳＳＳ）
    '
    ' 戻り値 ： パラメータ
    '
    ' 備考　 ： 
    '
    Private Function GetParamZenginKekka(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' 取引先コード
        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            Param.Add(readFmt.ToriData.INFOToriMast.TORIS_CODE_T & readFmt.ToriData.INFOToriMast.TORIF_CODE_T)
        Else
            Param.Add(New String("0"c, 10))
        End If

        ' 振替日
        Param.Add(readFmt.InfoMeisaiMast.FURIKAE_DATE)

        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            ' フォーマット区分
            Param.Add(readFmt.ToriData.INFOToriMast.FMT_KBN_T)
        Else
            Param.Add("")
        End If

        ' 連携区分
        Param.Add(mArgumentData.INFOParameter.RENKEI_KBN)

        ' 更新キー：企業シーケンス
        Param.Add("0")

        ' 連携ファイル名
        Param.Add(Path.GetFileName(filename).TrimEnd)

        ' ----- 上記までがパラメータ

        '' サイクル№
        Param.Add(mDenParam.HostTuuban)

        ' 相手センタ確認コード
        Param.Add(mDenParam.CenterCode)

        ' 当方センター確認コード
        Param.Add(mDenParam.TouhouCode)

        ' 全銀ファイル名
        Param.Add(mDenParam.ZenginName)

        ' 伝送ファイル名
        Param.Add(mDenParam.FileName)

        ' 伝送日時
        Param.Add(mDenParam.DensouNitiji)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' 機能　 ： 落とし込み用パラメータ作成
    '
    ' 戻り値 ： パラメータ
    '
    ' 備考　 ： 
    '
    Private Function GetParam(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' 委託者コードから，取引先マスタ情報を取得する
        Call readFmt.GetTorimastFromItakuCode(MainDB)

        ' 取引先コード
        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            Param.Add(readFmt.ToriData.INFOToriMast.TORIS_CODE_T & readFmt.ToriData.INFOToriMast.TORIF_CODE_T)
        Else
            Param.Add(New String("0"c, 10))
        End If

        ' 振替日
        Param.Add(readFmt.InfoMeisaiMast.FURIKAE_DATE)

        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            ' コード区分
            Param.Add(readFmt.ToriData.INFOToriMast.CODE_KBN_T)

            ' フォーマット区分
            Param.Add(readFmt.ToriData.INFOToriMast.FMT_KBN_T)

            ' 媒体コード
            Param.Add(readFmt.ToriData.INFOToriMast.BAITAI_CODE_T)
        Else
            Param.Add("")
            Param.Add("")
            Param.Add("")
        End If

        ' ラベル区分
        Param.Add("0")

        ' 連携区分
        Param.Add(mArgumentData.INFOParameter.RENKEI_KBN)

        ' 連携ファイル名
        Param.Add(Path.GetFileName(filename).TrimEnd)

        ' 暗号化区分
        Param.Add(mDenParam.EncodeKubun)
        If mDenParam.EncodeKubun = "1" Then
            ' 暗号化キー
            Param.Add(mDenParam.EncodeKey.TrimEnd)
            ' 暗号化IVキー
            Param.Add(mDenParam.EncodeVIKey.TrimEnd)
            ' ＡＥＳ
            Param.Add(mDenParam.AES.TrimEnd)
        Else
            ' 暗号化キー
            Param.Add("")
            ' 暗号化IVキー
            Param.Add("")
            ' ＡＥＳ
            Param.Add("")
        End If

        '' サイクル№
        Param.Add(mDenParam.HostTuuban)

        ' 相手センタ確認コード
        Param.Add(mDenParam.CenterCode)

        ' 当方センター確認コード
        Param.Add(mDenParam.TouhouCode)

        ' 全銀ファイル名
        Param.Add(mDenParam.ZenginName)

        ' 伝送ファイル名
        Param.Add(mDenParam.FileName)

        ' 伝送日時
        Param.Add(mDenParam.DensouNitiji)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' 機能　 ： ＪＯＢＭＡＳＴ登録
    '
    ' 戻り値 ： ARG1 - パラメータ
    '           ARG2 - メッセージ
    '
    ' 備考　 ： 
    '
    Public Function InsertJOBMASTbyError(ByVal command As String) As Boolean
        Dim SQL As New StringBuilder(128)

        SQL.Append("INSERT INTO JOBMAST(")
        SQL.Append(" TOUROKU_DATE_J")
        SQL.Append(",TOUROKU_TIME_J")
        SQL.Append(",STA_DATE_J")
        SQL.Append(",STA_TIME_J")
        SQL.Append(",END_DATE_J")
        SQL.Append(",END_TIME_J")
        SQL.Append(",JOBID_J")
        SQL.Append(",STATUS_J")
        SQL.Append(",USERID_J")
        SQL.Append(",PARAMETA_J")
        SQL.Append(",ERRMSG_J")
        SQL.Append(",DENSO_F_NAME_J")
        SQL.Append(",DENSO_TIME_STAMP_J")
        SQL.Append(") VALUES (")
        SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' STA_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' STA_TIME_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' END_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' END_TIME_J
        SQL.Append(",'T102'")                                               ' JOBID_J
        SQL.Append(",'3'")          ' 異常終了                              ' STATUS_J
        SQL.Append("," & SQ("SYSTEM"))                                      ' USERID_J
        SQL.Append("," & SQ(command))                                       ' PARAMETA_J
        SQL.Append("," & SQ(Message))                                       ' ERRMSG_J
        SQL.Append(",' '")                                                  ' DENSO_F_NAME_J
        SQL.Append(",TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')")                  ' DENSO_TIME_STAMP_J
        SQL.Append(")")
        Dim DB As New CASTCommon.MyOracle
        If DB.ExecuteNonQuery(SQL) <= 0 Then
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("他シス連携　失敗", Diagnostics.EventLogEntryType.Error)
            Return False
        End If
        DB.Commit()
        DB.Close()

        Return True
    End Function
End Class
