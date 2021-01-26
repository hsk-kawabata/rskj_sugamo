Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon.ModPublic

' 一括落とし込み連携処理
Public Class ClsIkkatuRenkei

    ' ログ処理クラス
    Private LOG As CASTCommon.BatchLOG

    ' 一括起動パラメータ 構造体
    Structure IKKATUPARAM
        Dim FSYORI_KBN As String        ' 処理区分（1:口振,2:ＳＳＳ,3:振込）
        Dim RENKEI_KBN As String        ' 連携区分
        Dim FMT_KBN As String           ' フォーマット区分
        Dim JOBTUUBAN As Integer        ' ジョブ通番
        '固定長データ処理用プロパティ
        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                FSYORI_KBN = para(0)
                RENKEI_KBN = para(1).PadLeft(2, "0"c)
                FMT_KBN = para(2)
                JOBTUUBAN = Integer.Parse(para(3))
            End Set
        End Property
    End Structure
    Private mIkParam As IKKATUPARAM

    ' 起動パラメータ 共通情報
    Private mArgumentData As CommData

    ' 依頼データファイル名
    Private mDataFileName As String

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    ' FSKJ.INI セクション名
    Private ReadOnly AppTOUROKU As String = "OTHERSYS"

    ' New
    Public Sub New()
    End Sub

    Public Function Main(ByVal command As String) As Integer

        Dim Comm() As String = command.Split(","c)

        ' オラクル
        MainDB = New CASTCommon.MyOracle

        ' 連携区分から各処理を分岐する
        Select Case Comm(1)
            Case "02"
                ' ＣＭＴ処理
            Case Else
                ' 落し込み処理を呼び出す
                mIkParam.JOBTUUBAN = CAInt32(Comm(3))
                LOG = New CASTCommon.BatchLOG("他シス連携", AppTOUROKU, CType(mIkParam.JOBTUUBAN, String))
                'LOG.JobTuuban = mIkParam.JOBTUUBAN
                'LOG.Write("他シス連携", "成功", command)

                'Dim ProcInfo As New ProcessStartInfo
                'ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), "KFJT101.EXE")
                'ProcInfo.Arguments = command
                'ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "EXE")
                'Call Process.Start(ProcInfo)

                Dim Syori As String = "落し込み"
                If mIkParam.FSYORI_KBN = "10" Then
                    Syori = "返還"
                End If
                Dim Fmt As String = ""
                Select Case Comm(1)
                    Case "01"
                        Fmt = "メディアコンバータ"
                    Case "09"
                        Fmt = "汎用エントリシステム"
                    Case "02"
                        Fmt = "ＣＭＴ"
                    Case "07"
                        Fmt = "学校自振システム"
                End Select
                '*** 修正 mitsu 2008/09/30 処理高速化 ***
                'Dim Param() As String = CType(Array.CreateInstance(GetType(String), Comm.Length - 1), String())
                Dim Param() As String = New String(Comm.Length - 2) {}
                '****************************************
                Array.Copy(Comm, Param, Comm.Length - 1)
                If InsertJOBMAST(Param, "") = True Then
                    LOG.Write("一括" & Syori & "連携(" & Fmt & ")", "成功")
                    LOG.UpdateJOBMASTbyOK("一括" & Syori & "連携(" & Fmt & ")")
                    ' コミット
                    MainDB.Commit()
                    Return 0
                Else
                    LOG.Write("一括" & Syori & "連携(" & Fmt & ")", "失敗")
                    LOG.UpdateJOBMASTbyErr("一括" & Syori & "連携(" & Fmt & ")")
                    ' コミット
                    MainDB.Commit()
                    Return 3
                End If
        End Select

        ' パラメータチェック
        Try
            ' メイン引数設定
            mIkParam.Data = command

            ' ジョブ通番設定
            LOG = New CASTCommon.BatchLOG("他シス連携", AppTOUROKU, CType(mIkParam.JOBTUUBAN, String))
            LOG.JobTuuban = mIkParam.JOBTUUBAN
            LOG.ToriCode = ""
        Catch ex As Exception
            LOG.Write("開始", "失敗", "パラメタ取得失敗[" & command & "]")
            Return 1
        End Try

        ' 起動パラメータ共通情報
        mArgumentData = New CommData(MainDB)

        ' パラメータ情報を設定
        Dim InfoParam As CommData.stPARAMETER
        InfoParam.TORI_CODE = ""
        InfoParam.BAITAI_CODE = ""
        InfoParam.FMT_KBN = mIkParam.FMT_KBN
        InfoParam.FURI_DATE = ""
        InfoParam.CODE_KBN = ""

        Dim TakouCMT As Boolean = False
        If mIkParam.FSYORI_KBN = "10" Then
            TakouCMT = True
        End If
        InfoParam.FSYORI_KBN = mIkParam.FSYORI_KBN
        InfoParam.JOBTUUBAN = mIkParam.JOBTUUBAN
        InfoParam.RENKEI_KBN = mIkParam.RENKEI_KBN
        InfoParam.RENKEI_FILENAME = ""
        InfoParam.ENC_KBN = ""
        InfoParam.ENC_KEY1 = ""
        InfoParam.ENC_KEY2 = ""
        InfoParam.ENC_OPT1 = ""
        InfoParam.CYCLENO = ""
        mArgumentData.INFOParameter = InfoParam

        ' 連携用クラス作成
        Dim oRenkei As CAstSystem.ClsRenkei

        Dim bRet As Boolean
        ' 連携処理
        If TakouCMT = True Then
            oRenkei = New CAstSystem.ClsRenkei(mArgumentData, 1)

            ' 他行結果ＣＭＴ一括更新
            bRet = MainTakouCMT(oRenkei)
        Else
            oRenkei = New CAstSystem.ClsRenkei(mArgumentData)

            ' 企業ＣＭＴ一括落し込み
            bRet = MainCMT(oRenkei)
        End If
        If bRet = False Then
            Call LOG.UpdateJOBMASTbyErr(oRenkei.Message)

            ' ロールバック
            MainDB.Rollback()
        Else
            Call LOG.UpdateJOBMASTbyOK(oRenkei.Message)
            ' コミット
            MainDB.Commit()
        End If

        MainDB.Close()

        If bRet = False Then
            Return 2
        End If
        Return 0

    End Function

    ' 機能　 ： ＣＭＴ 
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function MainCMT(ByRef renkei As ClsRenkei) As Boolean
        LOG.Write("ＣＭＴ連携開始", "成功")

        Try
            Dim FileList() As String = renkei.GetCMTOtherFiles()
            LOG.Write("ＣＭＴファイル取得:" & FileList.Length.ToString & "件", "成功")

            Dim OutFileName As String
            For i As Integer = 0 To FileList.Length - 1
                renkei.InFileName = FileList(i)
                If Path.GetFileName(FileList(i)).ToUpper.StartsWith("C") = True Then
                    ' 復号化してコピー
                    OutFileName = renkei.FileDecodeMove()
                    If OutFileName = "" Then
                        Call LOG.Write("ＣＭＴ復号化", "失敗", FileList(i) & ":" & renkei.Message)

                        ' 失敗したがそのまま次へ連携する
                        OutFileName = renkei.FileCopy()
                    End If
                Else
                    ' そのままコピー
                    OutFileName = renkei.FileCopy()
                End If
                If OutFileName <> "" Then
                    ' ＣＭＴファイル取得成功
                    ' ＪＯＢ登録
                    If TourokuFile(renkei, OutFileName) = True Then
                        LOG.Write("ＣＭＴファイル登録", "成功", OutFileName)
                        File.Delete(renkei.InFileName)

                        MainDB.Commit()

                        MainDB.BeginTrans()
                    Else
                        LOG.Write("ＣＭＴファイル登録", "失敗", OutFileName)
                        renkei.Message = "ＣＭＴファイル登録失敗 " & OutFileName
                        Return False
                    End If
                End If
            Next i

            ' ００：全銀  ０１：地公体（３５０）  ０２：国税  ０３：年金　０４：依頼書　０５：伝票　０６：地公体（３００）　２０：ＳＳＳ
            Dim sRenkei As String
            Select Case mArgumentData.INFOParameter.FMT_KBN
                Case "01"
                    sRenkei = "地公体（３５０）"
                Case "02"
                    sRenkei = "国税"
                Case "03"
                    sRenkei = "年金"
                Case "06"
                    sRenkei = "地公体（３００）"
                Case "00"
                    sRenkei = "全銀"
                    '*** 修正 mitsu 2008/07/17 センター直接持込フォーマット追加 ***
                Case "TO"
                    sRenkei = "センター直接持込"
                    '**************************************************************
                Case Else
                    sRenkei = "なし"
            End Select

            If FileList.Length = 0 Then
                renkei.Message = "ＣＭＴ対象データ（" & sRenkei & "）：０件"
            Else
                renkei.Message = "ＣＭＴ落込登録 " & renkei.Message & " フォーマット区分：" & sRenkei
            End If

        Catch ex As Exception
            Call LOG.Write("ＣＭＴ", "失敗", ex.Message & ":" & ex.StackTrace)
            Call LOG.UpdateJOBMASTbyErr("ＣＭＴ連携 失敗")
            Return False
        Finally
        End Try

        LOG.Write("ＣＭＴ連携終了", "成功")

        Return True
    End Function

    ' 機能　 ： ファイルから情報を読み込んでＪＯＢへ登録する
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： メディアコンバータ，学校，ＣＭＴ 
    '
    Private Function TourokuFile(ByRef oRenkei As ClsRenkei, ByRef filename As String) As Boolean

        '媒体読み込み
        'フォーマット　
        Dim oReadFMT As CAstFormat.CFormat
        Try
            ' フォーマット区分から，フォーマットを特定する
            oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)

            '*** 修正 mitsu 2008/08/25 委託者コード読替処理 ***
            '全銀フォーマットの場合のみ
            If mArgumentData.INFOParameter.FMT_KBN = "00" AndAlso oRenkei.ConvertItakuCode(filename, LOG) = False Then
                Return False
            End If
            '**************************************************

            If oReadFMT.FirstRead(filename) = 1 Then
                Call oReadFMT.CheckDataFormat()
                ' 2008.04.14 ADD 変なデータ対応 >>
                Call oReadFMT.CheckRecord1()
                ' 2008.04.14 ADD <<
            End If
            oReadFMT.ToriData = mArgumentData
            Dim Param() As String = GetParam(oReadFMT, filename)
            If Not oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing Then
                oRenkei.Message = "委託者コード：" & oReadFMT.InfoMeisaiMast.ITAKU_CODE
            End If
            oReadFMT.Close()

            ' 2008.04.21 ADD 取引先情報取得失敗処理 >>
            If Param.Length >= 1 And Param(0).StartsWith("000000000") = True Then

                If File.Exists(filename) = True Then
                    File.Delete(filename)
                End If
                filename = oRenkei.InFileName
                LOG.Write("取引先情報取得", "失敗", "委託者コード：" & oReadFMT.InfoMeisaiMast.ITAKU_CODE & " ファイル名:" & filename)

                ' 取引先情報取得失敗
                Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                If Prn.OutputCSVKekkaSysError(oReadFMT.InfoMeisaiMast.ITAKU_CODE, mArgumentData.INFOParameter.FSYORI_KBN, oRenkei.InFileName, MainDB) = False Then
                    LOG.Write("処理結果確認表", "失敗", "ファイル名:" & Prn.FileName & " メッセージ:" & Prn.ReportMessage)
                End If
                Prn = Nothing
                oReadFMT.Close()
                Return False
            End If
            ' 2008.04.21 ADD 取引先情報取得失敗処理 <<

            ' ＪＯＢマスタ正常登録
            If InsertJOBMAST(Param, oRenkei.InFileName) = True Then
                If (oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing) Then
                    LOG.Write("依頼データ読み取り", "成功")
                Else
                    LOG.Write("依頼データ読み取り", "成功", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                End If
            Else
                LOG.Write("依頼データ読み取り", "失敗", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                Return False
            End If
            oReadFMT = Nothing
        Catch ex As Exception
            If Not oReadFMT Is Nothing Then
                oReadFMT.Close()
            End If
            LOG.Write("依頼データジョブ登録", "失敗", ex.Message & ":" & ex.StackTrace)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： ＣＭＴ (他行結果)
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function MainTakouCMT(ByVal renkei As ClsRenkei) As Boolean
        LOG.Write("ＣＭＴ他行結果連携開始", "成功")

        Try
            Dim FileList() As String = renkei.GetCMTOtherFiles()
            LOG.Write("ＣＭＴファイル取得:" & FileList.Length.ToString & "件", "成功")

            Dim OutFileName As String
            For i As Integer = 0 To FileList.Length - 1
                renkei.InFileName = FileList(i)
                ' そのままコピー
                OutFileName = renkei.FileCopy()
                If OutFileName <> "" Then
                    ' ＣＭＴファイル取得成功
                    ' ＪＯＢ登録
                    If TourokuTakouFile(renkei, OutFileName) = True Then
                        LOG.Write("ＣＭＴファイル登録", "成功", OutFileName)
                        File.Delete(renkei.InFileName)

                        MainDB.Commit()

                        MainDB.BeginTrans()
                    Else
                        LOG.Write("ＣＭＴファイル登録", "失敗", OutFileName)
                        renkei.Message = "ＣＭＴファイル登録失敗 " & OutFileName
                        Return False
                    End If
                End If
            Next i

            If FileList.Length = 0 Then
                Dim sRenkei As String
                sRenkei = "他行結果"
                renkei.Message = "ＣＭＴ対象データ（" & sRenkei & "）：０件"
            End If

        Catch ex As Exception
            Call LOG.Write("ＣＭＴ", "失敗", ex.Message & ":" & ex.StackTrace)
            Call LOG.UpdateJOBMASTbyErr("ＣＭＴ連携 失敗")
            Return False
        Finally
        End Try

        LOG.Write("ＣＭＴ他行結果連携終了", "成功")

        Return True
    End Function

    ' 機能　 ： ファイルから情報を読み込んでＪＯＢへ登録する（他行結果）
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： メディアコンバータ，学校，ＣＭＴ 
    '
    Private Function TourokuTakouFile(ByRef oRenkei As ClsRenkei, ByVal filename As String) As Boolean

        '媒体読み込み
        'フォーマット　
        Dim oReadFMT As CAstFormat.CFormat
        Try
            ' フォーマット区分から，フォーマットを特定する
            oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
            If oReadFMT.FirstRead(filename) = 1 Then
                Call oReadFMT.CheckDataFormat()
                oReadFMT.ToriData = mArgumentData
                Dim Param() As String = GetTakouParam(oReadFMT, filename)
                oReadFMT.Close()

                ' ＪＯＢマスタ正常登録
                If InsertJOBMAST(Param, oRenkei.InFileName) = True Then
                    If (oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing) Then
                        LOG.Write("結果データ読み取り", "成功")
                    Else
                        LOG.Write("結果データ読み取り", "成功", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                    End If
                Else
                    LOG.Write("結果データ読み取り", "失敗", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                    Return False
                End If
            Else
                LOG.Write("結果データ読み取り", "失敗", "全銀フォーマット以外 ファイル名：" & filename)
                oReadFMT.Close()
            End If

            oReadFMT = Nothing
        Catch ex As Exception
            If Not oReadFMT Is Nothing Then
                oReadFMT.Close()
            End If
            LOG.Write("結果データジョブ登録", "失敗", ex.Message & ":" & ex.StackTrace)
            Return False
        End Try

        Return True

    End Function

    Private Function InsertJOBMAST(ByVal param() As String, ByVal inFileName As String) As Boolean
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
        SQL.Append(",'00000000'")                                           ' STA_DATE_J
        SQL.Append(",'000000'")                                             ' STA_TIME_J
        SQL.Append(",'00000000'")                                           ' END_DATE_J
        SQL.Append(",'000000'")                                             ' END_TIME_J
        If mIkParam.FSYORI_KBN = "10" Then
            SQL.Append(",'K101'")                                               ' JOBID_J
        Else
            SQL.Append(",'T101'")                                               ' JOBID_J
        End If
        SQL.Append(",'0'")                                                  ' STATUS_J
        SQL.Append("," & SQ(LOG.UserID))                                      ' USERID_J
        Dim sJoin As String = String.Join(",", param)
        SQL.Append("," & SQ(sJoin))                                         ' PARAMETA_J
        SQL.Append(",' '")                                                  ' ERRMSG_J
        SQL.Append("," & SQ(Path.GetFileName(inFileName)))                  ' DENSO_F_NAME_J
        SQL.Append(",TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')")                  ' DENSO_TIME_STAMP_J
        SQL.Append(")")
        If MainDB.ExecuteNonQuery(SQL) <= 0 Then
            LOG.Write("JOBMAST登録", "失敗", MainDB.Message)
            Return False
        End If

        Return True
    End Function

    Private Function GetParam(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        Dim ItakusyaCode As String
        Select Case mIkParam.FMT_KBN
            Case "02"
                ' 国税の場合
                Dim TORICODE As String = GetKokuzeiTORIMAST(filename).PadRight(9, "0"c)

                Call readFmt.GetTorimastFromToriCode(TORICODE, MainDB)
            Case "03"
                ' 年金の場合
                Dim TORICODE As String = GetNenkinTORIMAST(filename).PadRight(9, "0"c)

                Call readFmt.GetTorimastFromToriCode(TORICODE, MainDB)

                '*** 修正 mitsu 2008/07/17 センター直接持込の場合 ***
            Case "TO"
                '振替コード・企業コードから取引先マスタ情報を取得する
                Dim TORICODE As String = GetToKCenterTORIMAST(filename).PadRight(9, "0"c)

                Call readFmt.GetTorimastFromToriCode(TORICODE, MainDB)

                '委託者コードを正しい値に読替
                readFmt.InfoMeisaiMast.ITAKU_CODE = readFmt.ToriData.INFOToriMast.ITAKU_CODE_T
                '************************************************
            Case Else
                ' 委託者コードから，取引先マスタ情報を取得する
                Call readFmt.GetTorimastFromItakuCode(MainDB)
        End Select

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
        Else
            Param.Add("")
        End If

        ' フォーマット区分
        Param.Add(mIkParam.FMT_KBN)

        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            ' 媒体コード
            Param.Add(readFmt.ToriData.INFOToriMast.BAITAI_CODE_T)
        Else
            Param.Add("")
        End If

        ' ラベル区分
        Param.Add("0")

        ' 連携区分
        Param.Add(mIkParam.RENKEI_KBN)

        ' 連携ファイル名
        Param.Add("""" & Path.GetFileName(filename).TrimEnd & """")

        '' 暗号化区分
        'Param.Add(readFmt.ToriData.INFOToriMast.ENC_KBN_T)
        'If readFmt.ToriData.INFOToriMast.ENC_KBN_T = "1" Then
        '    ' 暗号化キー
        '    Param.Add(readFmt.ToriData.INFOToriMast.ENC_KEY1_T)
        '    ' 暗号化IVキー
        '    Param.Add(readFmt.ToriData.INFOToriMast.ENC_KEY2_T)
        '    ' ＡＥＳ
        '    Param.Add(readFmt.ToriData.INFOToriMast.ENC_OPT1_T)
        'Else
        '    ' 暗号化キー
        '    Param.Add("")
        '    ' 暗号化IVキー
        '    Param.Add("")
        '    ' ＡＥＳ
        '    Param.Add("")
        'End If

        '' サイクル№
        'Param.Add("")

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    Private Function GetTakouParam(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' 取引先コード
        Param.Add("")

        ' 振替日
        Param.Add(readFmt.InfoMeisaiMast.FURIKAE_DATE)

        ' フォーマット区分
        Param.Add(mIkParam.FMT_KBN)

        ' 連携区分
        Param.Add(mIkParam.RENKEI_KBN)

        ' 更新キー：企業シーケンス
        Param.Add("0")

        ' 連携ファイル名
        Param.Add(Path.GetFileName(filename).TrimEnd)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' 機能　 ： 国税　取引先取得
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Protected Friend Function GetKokuzeiTORIMAST(ByVal filename As String) As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
        Call KokuzeiFmt.FirstRead(filename)
        KokuzeiFmt.GetFileData(KokuzeiFmt.KOKUZEI_REC1.Data)
        Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
        KokuzeiFmt.Close()

        Dim ToriCode As String
        ' 科目コード　020:申告所得税, 300:消費税及地方消費税
        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
        If ToriCode <> "err" Then
            Return ToriCode
        End If

        SQL.Append("SELECT ")
        SQL.Append(" ITAKU_CODE_T,TORIS_CODE_T,TORIF_CODE_T")
        SQL.Append(" FROM TORIMAST")
        SQL.Append(" WHERE RENKEI_KBN_T = " & SQ(mIkParam.RENKEI_KBN))
        SQL.Append("   AND FMT_KBN_T = '02'")
        SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")
        SQL.Append(" ORDER BY TORIS_CODE_T ASC, TORIF_CODE_T ASC")
        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                If Kamoku <> "020" Then
                    OraReader.NextRead()
                    If OraReader.EOF = True Then
                        OraReader.DataReader(SQL)
                    End If
                End If
                ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                Return ToriCode.PadRight(9, " "c)
            End If
        Catch ex As Exception
            Return New String("0"c, 9)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            OraReader = Nothing
        End Try

        Return New String("0"c, 9)
    End Function

    ' 機能　 ： 年金　取引先取得
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function GetNenkinTORIMAST(ByVal filename As String) As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        Dim NenkinFmt As New CAstFormat.CFormatNenkin
        Call NenkinFmt.FirstRead(filename)
        NenkinFmt.GetFileData(NenkinFmt.NENKIN_REC1.Data)
        Dim Kamoku As String = NenkinFmt.NENKIN_REC1.NK2
        NenkinFmt.Close()

        Dim ToriCode As String
        ' 年金種別から判断 61:旧厚生年金,62:旧船員年金,63:旧国民年金,64:労災年金,65:新国民年金・厚生年金,66:新船員年金,67:旧国民年金短期
        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & Kamoku)
        If ToriCode <> "err" Then
            Return ToriCode
        End If

        Return New String("0"c, 9)
    End Function

    '*** 修正 mitsu 2008/07/17 センター直接持込フォーマットの取引先コード取得 ***
    ' 機能　 ： センター直接持込　取引先取得
    '
    ' 戻り値 ： 取引先コード
    '
    ' 備考　 ： 
    '
    Private Function GetToKCenterTORIMAST(ByVal filename As String) As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        Dim CenterFmt As New CAstFormat.CFormatTokCenter
        Call CenterFmt.FirstRead(filename)
        CenterFmt.GetFileData(CenterFmt.TOKCENTER_REC1.Data)
        Dim FuriCode As String = CenterFmt.TOKCENTER_REC1.TC.TC5
        Dim KigyoCode As String = CenterFmt.TOKCENTER_REC1.TC.TC6
        CenterFmt.Close()

        Dim ToriCode As String

        SQL.Append("SELECT ")
        SQL.Append(" TORIS_CODE_T,TORIF_CODE_T")
        SQL.Append(" FROM TORIMAST")
        '*** 修正 mitsu 2008/10/27 連携区分ＦＴＰ対応 連携区分を無視する ***
        'SQL.Append(" WHERE RENKEI_KBN_T = " & SQ(mIkParam.RENKEI_KBN))
        SQL.Append(" WHERE (RENKEI_KBN_T = " & SQ(mIkParam.RENKEI_KBN))
        SQL.Append("   OR RENKEI_KBN_T = '11')")
        '*******************************************************************
        SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
        SQL.Append("   AND KIGYO_CODE_T = " & SQ(KigyoCode))
        SQL.Append("   AND FURI_CODE_T = " & SQ(FuriCode))
        SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                Return ToriCode.PadRight(9, " "c)
            End If

        Catch ex As Exception
            Return New String("0"c, 9)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            OraReader = Nothing
        End Try

        Return New String("0"c, 9)
    End Function
    '************************************************************************

End Class
