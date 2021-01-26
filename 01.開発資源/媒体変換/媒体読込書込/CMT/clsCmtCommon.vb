Option Explicit On 
Option Strict On

Imports System.IO
Imports CASTCommon
Imports System.text
Imports System.Data.OracleClient
Imports System.Math
Imports CMT.ClsCMTCTRL

' 機　能 : CMT変換機能用共通関数クラス
'
' 備　考 : なるべく他の共通関数のものを利用し、使えるものが無い場合にのみ
'       　 ここに追加する。
'
Public Class clsCmtCommon
    Public gstrSysDate As String    ' 起動引数のシステム日付(yyyymmddの形式)
    Public gstrStationNo As String   ' 受付PCの機番
    'Public gstrStationName As String ' PC名

    Private Const ThisModuleName As String = "clsCmtCommon.vb"

    ' 機能　 ： 初期化関数
    '
    ' 引数　 ： なし
    ' 戻り値 ：
    Public Sub New()
        'gstrStationName = System.Environment.MachineName()
    End Sub

    ' 機　能 : CMT読込処理
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - フォーマット区分
    ' 　　　   ARG2 - 強制読込フラグ
    '          ARG3 - ListView(参照渡)
    ' 　　　   ARG4 - 読込CMT本数
    '
    ' 備 考  : 
    Public Function CmtReader(ByVal nFormatKbn As Integer _
        , ByVal bOverrideFlag As Boolean _
        , ByRef listview As ListView _
        , ByVal nReadQuantity As Integer _
        ) As Boolean

        ' 内部処理用変数の宣言
        Dim nReceptionNo, nFirstReceptionNo As Integer      ' 受付番号
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing
        'Dim cmtFormat As CAstFormat.CFormat
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim strSQL As String                                ' SQL文格納用変数
        Dim strPreRecord As String = ""                     ' 一個前のレコードのデータ区分
        Dim cmtReadData(nReadQuantity) As clsCmtData
        Dim filename As String = ""
        Dim serverpath As String = ""
        Dim bJSFlag As Boolean = False                      ' 自振/総振のフラグ
        Dim bCMTExist As Boolean = False                    ' CMTが一本でも存在したらTrue

        Try
            GCom.GLog.Job2 = "CMT読込"

            For i As Integer = 0 To nReadQuantity - 1
                cmtReadData(i) = New clsCmtData
            Next i

            ' フォーマット区分に応じてデータ保持クラスのインスタンス生成
            If Not GetFormat(nFormatKbn, cmtFormat) Then
                Return False
            End If

            ' ローカルフォルダの削除
            If Not Me.LocalFileDelete(True) Then
                MessageBox.Show("CMT読取機の一時ファイル用フォルダの消去に失敗しました。", "CMT読取失敗" _
                    , MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            If Not ReadStacker(11, MAXSTACKER) Then ' CMT読込関数を呼び出し
                Return False
            End If

            ' 受付番号(nReceptionNo)を取得
            If Not CmtCom.GetReceptionNo(True, nReceptionNo) Then
                ' 受付番号取得失敗
                ' ログ出力は関数内部で行っているため何もしないで終了
                Return False
            End If

            nFirstReceptionNo = nReceptionNo ' 先頭の受付番号を後で使うためコピー

            For i As Integer = 0 To nReadQuantity - 1
                With cmtReadData(i)
                    .ReadSucceedFlag = False ' CMT読込フラグをあらかじめFalseで初期化
                    .bListUpFlag = False
                    If GetCmtReadResult(i + 1) <> 1 Then
                        ' ListViewに表示すべきメッセージが存在するとき(CMTが空以外)
                        ' CMT読込データ保持クラスの初期化
                        .Init(nReceptionNo, i + 1, False, True)
                        .bListUpFlag = True

                        If GetCmtReadResult(i + 1) = 0 Then
                            ' CMTファイルの正常読込成功
                            .ReadSucceedFlag = True
                            bCMTExist = True

                            ' CMTの読込.解析しながらデータ保持クラスに溜め込み
                            Try
                                '*** 修正 mitsu 2009/04/?? センター直接持込暗号化CMT対応 ***
                                If nFormatKbn = 4 Then
                                    Dim AngouFileName As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DATBK"), "COMPLOCK")
                                    Dim FukugouFileName As String = Path.Combine(MakeServerFileName(True, True, nFormatKbn), "NCOMPLOCK.dat")

                                    'DATBKに暗号化ファイルをコピーする
                                    File.Copy(gstrCMTReadFileName(i), AngouFileName, True)

                                    '複合化EXEをジョブ登録する
                                    Dim clsFUSION As clsFUSION.clsMain = New clsFUSION.clsMain
                                    clsFUSION.fn_INSERT_JOBMAST("T104", GCom.GetUserID, AngouFileName)

                                    '複合化ファイルが出来るまで待機する(3分待っても終わらなければエラー)
                                    Dim waitCnt As Integer = 0
                                    While File.Exists(FukugouFileName) = False
                                        Threading.Thread.Sleep(1000)
                                        waitCnt += 1

                                        If waitCnt > 180 Then
                                            MessageBox.Show("CMTファイルが見つかりません。" & vbCrLf & "ファイルの複合化に失敗している可能性があります。", _
                                                "CMT読込", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                            Return False
                                        End If
                                    End While

                                    'ファイルをローカルのCMTフォルダに移動して通常のCMTファイルと同様に処理する
                                    File.Delete(gstrCMTReadFileName(i))
                                    File.Move(FukugouFileName, gstrCMTReadFileName(i))
                                End If
                                '***********************************************************

                                If (nFormatKbn = 5) Then
                                    ' センター持込(結果データの場合)
                                    .InitEmptyItakuDataNoErr(True)
                                    .ErrCode = 0
                                    .bListUpFlag = True
                                    bJSFlag = True
                                ElseIf cmtFormat.FirstRead(gstrCMTReadFileName(i)) = 0 Then ' 1.CMTファイルの物理的読込
                                    ' 読込失敗
                                    .InitEmptyItakuData(True)
                                    ' エラー内容をDBに保存
                                    strSQL = .GetInsertSQL(0) ' ファイルSEQ単位でSQL文生成
                                    GCom.DBExecuteProcess(4, strSQL)
                                    Exit Try
                                Else ' 物理的読込成功
                                    ' 2.フォーマットの解析開始
                                    ParseCMTFormat(nFormatKbn, cmtFormat, cmtReadData(i), True)

                                    ' 3.自振・総振の自動判別
                                    If .ItakuCounter >= 0 Then
                                        GetJSFlag(.ItakuData(0), cmtReadData(i), bJSFlag)

                                        '***Astar酒井 2008/05/30
                                        .JSFlag = bJSFlag
                                        '***

                                        '*** ASTAR.S.S 2008.05.23 媒体区分相違対応  ***
                                        If .NotError() Then
                                            '*** 修正 mitsu 2008/10/21 .ItakuData(0)の表記省略 ***
                                            Dim id As clsItakuData = .ItakuData(0)
                                            '*****************************************************

                                            If id.strBaitaiCode <> "06" Then
                                                '*** 修正 mitsu 2008/10/21 センター直接持込フォーマット国税データ対応 ***
                                                'センター直接持込かつ国税の場合は媒体区分相違を無視する
                                                If Not (nFormatKbn = 4 AndAlso _
                                                    (id.strToriSCode & id.strToriFCode = GetFSKJIni("TOUROKU", "KOKUZEI020") Or _
                                                     id.strToriSCode & id.strToriFCode = GetFSKJIni("TOUROKU", "KOKUZEI300"))) Then
                                                    '********************************************************************
                                                    cmtReadData(i).setError(9, "媒体区分相違")
                                                With GCom.GLog
                                                        .Result = MenteCommon.clsCommon.NG
                                                    .Discription = "媒体区分相違 委託者コード：" & id.strItakuCode & ", 媒体区分：" & id.strBaitaiCode
                                                End With
                                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                                End If
                                                '*** 修正 mitsu 2008/10/21 センター直接持込フォーマット国税データ対応 ***
                                            End If
                                            '****************************************************************************
                                        End If
                                    Else
                                        ' 委託者が存在しない場合 = レコードがゼロ
                                        .ComplockInit("", "")     ' 読取できなかったため、委託者不明でデータ生成
                                        .ComplockFlag = False
                                        .Override = False
                                        .bListUpFlag = True
                                        .setError(9, "CMT読込失敗")
                                        With GCom.GLog
                                            .Result = MenteCommon.clsCommon.NG
                                            .Discription = "CMTファイル中の委託者数０ CMTファイル中にヘッダレコードが存在しない可能性があります"
                                        End With
                                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                    End If
                                End If

                                If .NotError() Then
                                    ' 4.ファイルのアップロード処理(.tmpでコピー)
                                    ' -- ファイル名作成

                                    '***伝送扱いのファイル名を想定
                                    If nFormatKbn <> 7 Then
                                        If .ItakuData(0).strMultiKbn = "0" Then
                                            filename = "D" & .ItakuData(0).strToriSCode & .ItakuData(0).strToriFCode
                                        Else
                                            filename = "D" & .ItakuData(0).strItakuKanriCode
                                        End If
                                    Else
                                        If .ItakuData(0).strMultiKbn = "0" Then
                                            filename = "SD" & .ItakuData(0).strToriSCode & .ItakuData(0).strToriFCode
                                        Else
                                            filename = "SD" & .ItakuData(0).strItakuKanriCode
                                        End If
                                    End If


                                    serverpath = MakeServerFileName(True, bJSFlag, nFormatKbn)

                                    If serverpath = "ERR" Then
                                        .setError(6, "サーバへのアップロードに失敗しました。")
                                        With GCom.GLog
                                            .Result = MenteCommon.clsCommon.NG
                                            .Discription = "サーバのフォルダ名生成に失敗しました 自振総振区分：" & bJSFlag.ToString & ", フォーマット区分：" & nFormatKbn.ToString
                                        End With
                                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                        .bListUpFlag = True
                                    Else
                                        .FileName = filename & ".dat" ' ファイル名設定

                                        ' -- アップロード - 
                                        If (nFormatKbn <> 5) Then
                                            .bUploadSucceedFlg = upload(gstrCMTReadFileName(i), serverpath & filename & ".tmp", cmtReadData(i), cmtFormat.RecordLen)
                                        Else ' センター持込(結果データ)
                                            .bUploadSucceedFlg = upload(gstrCMTReadFileName(i), serverpath & filename & ".tmp", cmtReadData(i), 165)
                                        End If

                                        If Not .bUploadSucceedFlg Then
                                            .setError(6, "サーバへのアップロード失敗")
                                            With GCom.GLog
                                                .Result = MenteCommon.clsCommon.NG
                                                .Discription = "サーバ上へのアップロード失敗 " & cmtReadData(i).FileName & ".tmpから .dat"
                                            End With
                                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                        Else
                                            If Not Me.RenameFileName(serverpath & filename & ".tmp", serverpath & filename & ".dat") Then
                                                cmtReadData(i).setError(6, "サーバへのアップロードに失敗しました。")
                                                With GCom.GLog
                                                    .Result = MenteCommon.clsCommon.NG
                                                    .Discription = "サーバ上でのリネーム失敗 " & cmtReadData(i).FileName & ".tmpから .dat"
                                                End With
                                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                                DeleteFile(serverpath & filename & ".tmp") ' リネーム失敗ファイルの削除
                                            End If
                                        End If
                                    End If

                                    ' cmtReadData(i)の内容をテーブルに書込み
                                    For j As Integer = 0 To .ItakuCounter
                                        strSQL = .GetInsertSQL(j) ' ファイルSEQ単位でSQL文生成
                                        GCom.DBExecuteProcess(4, strSQL)
                                    Next
                                Else
                                    ' エラー内容をDBに出力
                                    strSQL = .GetInsertSQL(0) ' ファイルSEQ単位でSQL文生成
                                    GCom.DBExecuteProcess(4, strSQL)
                                End If
                                cmtFormat.Close()
                            Catch ex As Exception
                                With GCom.GLog
                                    .Job2 = "CMT読込"
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = ex.Message
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                'GCom.DBExecuteProcess(16) ' rollback
                            End Try
                        End If
                        .bListUpFlag = True
                        nReceptionNo += 1
                    Else
                        ' CMTが空、あるいはCTM実機無しでファイルも無し →　エラーではないのでこのまま素通し
                        .bListUpFlag = False
                    End If
                End With
            Next i

            ' ListView に項目を追加
            For i As Integer = 0 To nReadQuantity - 1
                If cmtReadData(i).bListUpFlag Then
                    Call AddListView(listview, cmtReadData(i), nFirstReceptionNo)
                    nFirstReceptionNo += 1
                End If
            Next i

            ' cmtFormatインスタンスの削除
            cmtFormat.Close()
            cmtFormat.Dispose()

            For i As Integer = 0 To nReadQuantity - 1 ' cmtReadDataインスタンスの削除
                cmtReadData(i).Dispose()
            Next i

        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT読込"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

        Return True
    End Function

    ' 機　能 : CMTが挿入されていないときエラーメッセージを表示
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - CMT有無フラグ
    '
    ' 備 考  :
    Private Sub CMTNotExistErr()
        MessageBox.Show("CMTリーダにCMTがセットされていません", "CMT読取エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
        With GCom.GLog
            .Result = MenteCommon.clsCommon.NG
            .Discription = "CMT無し(スタッカ未セット,CMT未挿入)"
        End With
        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    End Sub

    ' 機　能 : ComplockCMT読込処理
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - 委託者コード
    '          ARG2 - 取引先主コード
    '          ARG3 - 取引先副コード
    '          ARG4 - ListView(参照渡)
    '
    ' 備 考  : 
    Public Function ComplockCmtReader(ByVal strItakuCd As String _
        , ByVal strToriSCd As String _
        , ByVal strToriFCd As String _
        , ByRef listview As ListView _
        ) As Boolean

        ' 内部処理用変数の宣言
        Dim nReceptionNo As Integer                         ' 受付番号
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim cmtFormat As CAstFormat.CFormat = Nothing
        'Dim cmtFormat As CAstFormat.CFormat
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim strSQL As String                                ' SQL文格納用変数
        Dim cmtReadData As clsCmtData = New clsCmtData
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim fkbn As String = String.Empty
        Dim toris As String = String.Empty
        Dim torif As String = String.Empty
        'Dim fkbn, toris, torif As String                    ' F処理区分, 取引先主コード、副コード
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim bJSFlag As Boolean
        Dim serverpath As String                            ' アップロード先のパス
        Dim filename As String                              ' アップロード先のファイル名(拡張子抜き)
        Dim nFormatKbn As Integer                           ' フォーマット区分
        Dim cmtc As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL

        Try
            If Not DB.GetFormatKbn(strItakuCd, nFormatKbn) Then
                Return False
            End If
            GCom.GLog.Job2 = "Complock読込"

            If Not DB.GetFToriCode(strItakuCd, "", toris, torif, fkbn, nFormatKbn) Then
                '*** 修正 mitsu 2008/09/01 メッセージボックス化 ***
                'With GCom.GLog
                '    .Result = "委託者コードに該当する委託者なし"
                '    .Discription = "委託者コード:" & strItakuCd
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("委託者コードに該当する委託者なし")
                '**************************************************
                Return False
            Else
                Select Case fkbn
                    Case "1" '自振
                        bJSFlag = True
                    Case "3" ' 総振
                        bJSFlag = False
                    Case Else
                        '*** 修正 mitsu 2008/09/01 メッセージボックス化 ***
                        'With GCom.GLog
                        '    .Result = "F処理区分が1,3以外"
                        '    .Discription = "F処理区分:" & fkbn
                        'End With
                        'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        MessageBox.Show("処理区分が1,3以外")
                        '**************************************************
                        Return False
                End Select
            End If

            LocalFileDelete(True) ' ローカルPCのファイル削除

            ' フォーマット区分に応じてデータ保持クラスのインスタンス生成
            GetFormat(nFormatKbn, cmtFormat)

            '*** 修正 mitsu 2008/09/01 CMT接続確認 ***
            If Not CheckConnectCMT() Then 'CMTが接続されていない場合
                Return False
            End If
            '*****************************************

            If Me.IsExistStacker(cmtc, True) Then ' スタッカの有無をチェック.
                Return False
            ElseIf Not ReadStacker(12, 1) Then ' CMT読込関数を呼び出し
                Return False '読込失敗時は異常終了
            ElseIf Not CmtCom.GetReceptionNo(True, nReceptionNo) Then ' 受付番号(nReceptionNo)を取得
                ' 受付番号取得失敗
                ' ログ出力は関数内部で行っているため何もしないで終了
                Return False
            End If

            With cmtReadData
                .ReadSucceedFlag = False ' CMT読込フラグをあらかじめFalseで初期化
                .bListUpFlag = False
                If GetCmtReadResult(1) = 1 Then
                    ' CMTが空
                    Me.CMTNotExistErr()
                    Return False
                ElseIf GetCmtReadResult(1) = 0 Then
                    ' ListViewに表示すべきメッセージが存在するとき(CMTが空以外)
                    ' CMT読込データ保持クラスの初期化
                    .Init(nReceptionNo, 1, False, True)
                    .bListUpFlag = True

                    ' CMTファイルの正常読込成功
                    .ReadSucceedFlag = True

                    ' CMTの読込.解析しながらデータ保持クラスに溜め込み
                    Try
                        ' ファイルのアップロード処理(.tmpでコピー)
                        ' -- ファイル作成
                        filename = "C" & CmtCom.gstrStationNo & CmtCom.gstrSysDate & (nReceptionNo.ToString).PadLeft(5, "0"c) _
                            & "." & fkbn & "." & toris & "." & torif ' 拡張子抜きのファイル名作成
                        serverpath = MakeServerFileName(True, bJSFlag, nFormatKbn)
                        .FileName = filename & ".dat"
                        ' -- ファイルコピー
                        upload(gstrCMTReadFileName(0), serverpath & filename & ".tmp", cmtReadData, cmtFormat.RecordLen)
                        ' サーバにアップロードしたファイルを.tmp→.datにリネーム
                        If .NotError Then
                            ' 正常に読み取れた場合のみをアップロード
                            If Not Me.RenameFileName(serverpath & filename & ".tmp", serverpath & filename & ".dat") Then
                                ' リネーム失敗時
                                cmtReadData.setError(6, "サーバへのアップロードに失敗しました。")
                                DeleteFile(serverpath & filename & ".tmp") ' リネーム失敗ファイルの削除
                            End If
                        End If
                    Catch ex As Exception
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "ファイルコピー失敗 " & ex.Message
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    End Try
                Else
                    ' CMTが異常
                    .bListUpFlag = True ' エラーを表示するためTrueに設定
                    CheckCmtReadResult(1, cmtReadData)
                End If
                ' エラーの有無を問わず、表示用・DB保存用のデータを作成
                .ComplockInit(strItakuCd, "00000000")
                .ComplockFlag = True
                ' cmtReadData(i)の内容をテーブルに書込み
                strSQL = .GetInsertSQL(0) ' ファイルSEQ単位でSQL文生成
                GCom.DBExecuteProcess(4, strSQL)
            End With

            ' ListView に項目を追加
            If cmtReadData.bListUpFlag Then
                Call AddListView(listview, cmtReadData, nReceptionNo)
            End If

            If listview.Items.Count > 0 AndAlso _
                MessageBox.Show("印刷を実行しますか？", "ComplockCMT読込結果印刷", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                Me.PrintButton(listview, "ComplockCMT読込結果")
            End If

            ' cmtFormatインスタンスの削除
            cmtFormat.Close()
            cmtFormat.Dispose()
            cmtReadData.Dispose()
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "Complock読込"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return True
    End Function


    ' 機　能 : TORIMASTから自振・総振のフラグを取得
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - clsItakuDataのインスタンス
    '        : ARG2 - エラーを格納するためのclsCmtDataのインスタンス
    '          ARG3 - 取得結果を格納するBool値フラグ
    '
    ' 備 考  :
    Private Function GetJSFlag(ByVal itakudata As clsItakuData, ByVal cmtd As clsCmtData, ByRef bJSFlag As Boolean) As Boolean
        Select Case itakudata.strFSyoriKbn
            Case "1"
                bJSFlag = True
            Case "3"
                bJSFlag = False
            Case Else
                'cmtd.setError(9, "データ区分不正")         ' 2008/04/06 前田 修正
                cmtd.setError(9, "取引先マスタなし")        ' 2008/04/06 前田 修正
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "処理区分不正 委託者コード：" & itakudata.strItakuCode & ", 処理区分：" & itakudata.strFSyoriKbn
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
        End Select
        Return True
    End Function


    ' 機  能 : Listview追加
    '
    ' 戻り値 : エラー無 True / エラー有 False
    '
    ' 引き数 : ARG1 - 追加先ListView
    '          ARG2 - clsCmtDataインスタンス
    '          ARG3 - 受付番号
    '          ARG4 - リストカウンタ
    Private Function AddListView(ByVal listv As ListView, ByVal cmtd As clsCmtData, ByVal nReceptionNo As Integer) As Boolean
        Try
            With cmtd
                For j As Integer = 0 To .ItakuCounter
                    Dim lv As New ListViewItem(nReceptionNo.ToString.PadLeft(5, "0"c))
                    .getListViewItem(lv, j)
                    If Not .NotError() Then
                        lv.BackColor = Color.Pink
                    Else
                        lv.BackColor = Color.White
                    End If
                    listv.Items.AddRange(New ListViewItem() {lv})
                Next j
            End With
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ListView追加失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function


    ' 機  能 : CMT読込結果のチェック
    '
    ' 戻り値 : エラー無 True / エラー有 False
    '
    ' 引き数 : ARG1 - スタッカ番号
    '          ARG2 - clsCmtDataのインスタンス
    ' 備 考  : 暦日チェック＆システム日付より未来であることをチェック
    Private Function CheckCmtReadResult(ByVal nStackerNo As Integer, ByVal cmtd As clsCmtData) As Boolean
        If nStackerNo < 1 Or nStackerNo > MAXSTACKER Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "指定されたスタッカ番号は不正です スタッカ番号：" & nStackerNo.ToString
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        With cmtd
            Select Case GetCmtReadResult(nStackerNo)
                Case 2
                    .setError(9, "CMTが空です")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMTが空です err code 9"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Case 3
                    .setError(4, "CMT読取機の中に既に同名のファイルが存在しています")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "ローカルPCに先行ファイルあり err code 4"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Case 4
                    .setError(9, "CMT読取エラー")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMT読取エラー err code 4"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            End Select
        End With
        Return True
    End Function

    ' 機　能 : 振替日のチェック
    '
    ' 戻り値 : エラー無 True / エラー有 False
    '
    ' 引き数 : ARG1 - 振替日付(yyyymmdd形式)
    '          ARG2 - システム日付(yyyymmdd形式)
    ' 備 考  : 暦日チェック＆システム日付より未来であることをチェック
    Public Function CheckFuriDate(ByVal strFuriDate As String, ByVal strSysDate As String, ByVal bShowMsg As Boolean) As Boolean
        Dim furi As Date
        Dim sys As Date

        Try
            furi = DateValue(strFuriDate.Substring(0, 4) & "/" & strFuriDate.Substring(4, 2) & "/" & strFuriDate.Substring(6, 2))
            sys = DateValue(strSysDate.Substring(0, 4) & "/" & strSysDate.Substring(4, 2) & "/" & strSysDate.Substring(6, 2))
            'If furi <= sys Then
            '    ' システム日付より以前の日付が振替日として指定されている
            '    If bShowMsg Then
            '        MessageBox.Show("システム日付:" & strSysDate & ", 振替日:" & strFuriDate, "システム日付より以前の日付が振替日として指定", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    End If

            '    With GCom.GLog
            '        .Result = "システム日付より以前の日付が振替日として指定"
            '        .Discription = "システム日付:" & strSysDate & ", 振替日:" & strFuriDate
            '    End With
            '    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            '    Return False
            'End If
        Catch ex As InvalidCastException
            If bShowMsg Then
                MessageBox.Show("システム日付：" & strSysDate & ", 振替日：" & strFuriDate, "振替日、システム日付が暦日ではありません", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            '*** 修正 mitsu 2008/09/01 不要 ***
            'With GCom.GLog
            '    .Result = "振替日、システム日付が暦日ではありません"
            '    .Discription = "システム日付:" & strSysDate & ", 振替日:" & strFuriDate
            '    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            'End With
            '**********************************
            Return False
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "暦日チェックエラー システム日付：" & strSysDate & ", 振替日：" & strFuriDate & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function


    ' 機　能 : 暦日チェック
    '
    ' 戻り値 : エラー無 True / エラー有 False
    '
    ' 引き数 : ARG1 - 振替日付(yyyymmdd形式)
    '          
    ' 備 考  : 暦日チェックのみ
    Public Function CheckDate(ByVal strFuriDate As String) As Boolean
        Dim furi As Date

        Try
            furi = DateValue(strFuriDate.Substring(0, 4) & "/" & strFuriDate.Substring(4, 2) & "/" & strFuriDate.Substring(6, 2))
        Catch ex As InvalidCastException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "振替日が暦日ではありません 振替日：" & strFuriDate & " " & ex.Message
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            End With
            Return False
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "暦日チェックエラー 振替日：" & strFuriDate & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function

    ' 機　能 : TORIMASTから自振・総振のフラグを取得
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - F処理区分
    '          ARG2 - 取得結果を格納するBool値フラグ
    ' 備 考  : 引数として委託者コードを渡す場合
    Private Function GetJSFlag(ByVal fkbn As String, ByRef bJSFlag As Boolean) As Boolean
        Select Case fkbn
            Case "1"
                bJSFlag = True
            Case "3"
                bJSFlag = False
            Case Else
                '*** 修正 mitsu 2008/09/01 不要 ***
                'With GCom.GLog
                '    .Result = "委託者コードのF処理区分不正"
                '    .Discription = "F処理区分値:" & fkbn
                'End With
                '**********************************
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
        End Select
        Return True
    End Function

    ' 機　能 : サーバにファイルをアップロードして処理結果をログに記録
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - 元ファイル名
    '          ARG2 - 先ファイル名
    '          ARG3 - エラーメッセージ設定先clsCmtData
    '          ARG4 - レコード長
    ' 備 考  : 引数として委託者コードを渡す場合
    Private Function upload(ByVal source As String, ByVal distination As String, _
            ByVal cmtd As clsCmtData, ByVal rlen As Integer) As Boolean
        Dim nRet As Integer = CmtCom.BinaryCopy(source, distination, rlen)
        If nRet = 0 Then
            cmtd.bUploadSucceedFlg = True
            Return True
        End If
        ' サーバへの.tmpファイルのアップロード失敗
        Select Case nRet
            Case 1
                cmtd.setError(6, "サーバへのアップロードに失敗しました。")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "サーバへのアップロード失敗 元ファイル名：" & source & ", 先ファイル名：" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 2
                cmtd.setError(7, "ファイル書込失敗")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "ファイル書込失敗 元ファイル名：" & source & ", 先ファイル名：" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 3
                cmtd.setError(9, "ブロック長不備")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "ブロック長不備 元ファイル名：" & source & ", 先ファイル名：" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Select
        '*** 修正 mitsu 2008/09/01 不要 ***
        'With GCom.GLog
        '    .Result = "サーバへのアップロード失敗"
        '    .Discription = "元:" & source & ", 先:" & distination
        'End With
        'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        '**********************************
        Return False
    End Function

    ' 機　能 : サーバからファイルをダウンロードして処理結果をログに記録
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - 元ファイル名
    '          ARG2 - 先ファイル名
    '          ARG3 - エラーメッセージ設定先clsCmtData
    '          ARG4 - レコード長
    ' 備 考  : 引数として委託者コードを渡す場合
    Private Function download(ByVal source As String, ByVal distination As String, ByVal cmtd As clsCmtData, ByVal rlen As Integer) As Boolean
        ' 3.サーバからローカルPC書込用フォルダへのコピー

        Select Case BinaryCopy(source, distination, rlen)
            ' 戻り値 : 0:成功, 1:ファイルなし, 2:書込み先が既に存在, 3:ファイル長が不正
        Case 0
                ' ダウンロード成功
                cmtd.bUploadSucceedFlg = True
                Return True
            Case 1
                ' ダウンロード失敗
                cmtd.bUploadSucceedFlg = False
                cmtd.setError(9, "返却ファイルコピー失敗")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "返却ファイルコピー失敗 元ファイル名：" & source & ", 先ファイル名：" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 2
                cmtd.bListUpFlag = False
                cmtd.setError(9, "返却ファイルコピー失敗")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "返却ファイルコピー失敗 元ファイル名：" & source & ", 先ファイル名：" & distination
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Case 3
                cmtd.bListUpFlag = False
                cmtd.setError(9, "返却ファイルのレコード長が不正です")
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "返却ファイルレコード長不正 元ファイル名：" & source & ", 先ファイル名：" & distination & " レコード長：" & rlen.ToString
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Select
        Return False

    End Function

    ' 機　能 : CMTファイルのチェック
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - CMTスロット番号
    '          ARG2 - フォーマット区分
    '          ARG3 - 結果を出力するclsCmtReadDataのインスタンス
    '          ARG4 - フォーマットクラスのインスタンス
    '          ARG5 - CMTインスタンス
    ' 備 考  :
    Private Function CheckCMTFile(ByVal nSlotNo As Integer, ByVal nFormatKbn As Integer _
        , ByRef aCmtReadData As clsCmtData, ByRef cmtFormat As CAstFormat.CFormat, ByRef cmt As CMT.ClsCMTCTRL) As Boolean

        Try
            'CMTファイルの読込()
            If Not cmt.ReadCmt(CByte(nSlotNo)) Then
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "スタッカ番号：" & nSlotNo & "で読込失敗"
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMTファイルチェックエラー スタッカ番号：" & nSlotNo & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If (cmtFormat.FirstRead(gstrCMTReadFileName(nSlotNo - 1)) = 0) Then
            aCmtReadData.InitEmptyItakuData(False)
            Return False
        End If

        ' CMTファイルの解析
        Try
            If (Not ParseCMTFormat(nFormatKbn, cmtFormat, aCmtReadData, True)) AndAlso _
                aCmtReadData.ErrCode = 0 Then
                aCmtReadData.setError(9, "ファイルのフォーマットに異常があります")
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMTファイルチェックエラー スタッカ番号：" & nSlotNo & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        '***ASTAR SUSUKI 2008.06.12         ***
        '***ヘッダラベル書き込み対応
        Call LocalHeadCopy(nSlotNo)
        '**************************************

        Return True
    End Function

    ' 機　能 : サーバ返却ファイルのチェック
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - 委託者コード
    '          ARG2 - 振替日
    '          ARG3 - cmtWriteData
    '          ARG4 - cmtFormatのインスタンス
    '          ARG5 - サーバ返却ファイルの名前
    '          ARG6 - フォーマット区分
    '
    ' 備 考  : 
    Private Function ParseServerFile(ByVal strFileName As String _
        , ByRef aCmtWriteData As clsCmtData, ByRef cmtFormat As CAstFormat.CFormat, ByVal nFormatKbn As Integer) As Boolean

        If Not File.Exists(strFileName) Then
            ' 書込対象となる返却ファイルが存在せず
            MessageBox.Show("書込対象となる返却ファイルが存在しません", "返却ファイルチェック")
            Return False
        End If

        If (cmtFormat.FirstRead() = 1) Then ' 読込に成功しているかチェック
            ' CMTファイルの解析
            If Not ParseCMTFormat(nFormatKbn, cmtFormat, aCmtWriteData, False) Then
                If aCmtWriteData.ErrCode = 0 Then
                    aCmtWriteData.setError(9, "返却ファイル読取エラー")
                    Return False
                End If
            End If
        Else
            aCmtWriteData.InitEmptyItakuData(False)
            Return False
        End If

        Return True
    End Function


    ' 機　能 : CMT書込処理
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - フォーマット区分
    ' 　　　   ARG2 - 強制読込フラグ
    '          ARG3 - ListView(参照渡)
    '          ARG4 - 書込CMT本数
    ' 備 考  :
    Public Function CmtWriter(ByVal nFormatKbn As Integer _
        , ByVal bOverrideFlag As Boolean _
        , ByRef listview As ListView _
        , ByVal nWriteQuantity As Integer _
        ) As Boolean

        ' 内部処理用変数の宣言
        Dim nReceptionNo, nFirstReceptionNo As Integer          ' 受付番号
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing           ' フォーマットクラス
        Dim cmtfServer As CAstFormat.CFormat = Nothing          ' フォーマットクラス
        'Dim cmtFormat As CAstFormat.CFormat                     ' フォーマットクラス
        'Dim cmtfServer As CAstFormat.CFormat                    ' フォーマットクラス
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim strSQL As String                                    ' SQL文格納用変数
        Dim strPreRecord As String = ""                         ' 一個前のレコードのデータ区分
        Dim cmtReadData(nWriteQuantity) As clsCmtData           ' CMT読込結果の解析結果
        Dim cmtWriteData(nWriteQuantity) As clsCmtData          ' サーバ返却ファイルの解析結果
        Dim strWriteFileName(nWriteQuantity) As String          ' サーバ返却ファイル名(フルパス)
        Dim cmtCtrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim nWriteCounter As Integer = 0
        Dim bRet As Boolean
        Dim bJSFlag As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim fkbn As String = String.Empty
        Dim toris As String = String.Empty
        Dim torif As String = String.Empty
        'Dim fkbn, toris, torif As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        '***ASTAR 2008.08.07 ブロックサイズ対応 >>
        If nFormatKbn = 0 Then
            '全銀の場合は，ブロックサイズ１８００
            cmtCtrl.BlockSize = 1800
            '*** 修正 mitsu 2008/09/12 各種フォーマットに対応 ***
        ElseIf nFormatKbn = 1 Then
            cmtCtrl.BlockSize = 2100
        ElseIf nFormatKbn = 2 Then
            cmtCtrl.BlockSize = 3000
        ElseIf nFormatKbn = 3 Then
            cmtCtrl.BlockSize = 3900
            'センター直接未対応！
            '****************************************************
        ElseIf nFormatKbn = 6 Then  'NHK
            cmtCtrl.BlockSize = 1800
        Else
            cmtCtrl.BlockSize = -1
        End If
        '***ASTAR 2008.08.07 ブロックサイズ対応 <<

        Try
            GCom.GLog.Job2 = "CMT書込"

            For i As Integer = 0 To nWriteQuantity - 1
                cmtReadData(i) = New clsCmtData
                cmtWriteData(i) = New clsCmtData
            Next i

            ' フォーマット区分に応じてデータ保持クラスのインスタンス生成
            If Not GetFormat(nFormatKbn, cmtFormat) Or Not GetFormat(nFormatKbn, cmtfServer) Then
                MessageBox.Show("書込みボタン処理中にエラーが発生しました。", "書込処理", _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            ' ローカルフォルダの掃除
            If (Not LocalFileDelete(True)) Or (Not LocalFileDelete(False)) Then
                MessageBox.Show("ローカルPC内の不要ファイルの削除に失敗しました。", "書込処理", _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If


            ' 受付番号(nReceptionNo)を取得
            If Not CmtCom.GetReceptionNo(False, nReceptionNo) Then
                ' 受付番号取得失敗
                ' ログ出力は関数内部で行っているため何もしないで終了
                Return False
            End If
            nFirstReceptionNo = nReceptionNo

            If Not Me.CheckConnectCMT() Then ' CMTが接続されていない場合は終了
                Return False
            End If

            ' CMT手差しの検出
            If (cmtCtrl.SelectCmt(1)) Then ' 1本目が存在するかチェック
                '***修正 瀬戸 書き込みが動かないので修正 2008.11.13 start
                'If Not Me.IsExistStacker(cmtCtrl, False) Then ' 2本以上存在しないかをチェック
                'CMTが1本だけの場合
                nWriteQuantity = 1 ' 読取本数を一本だけにする
                'End If
                '***修正 瀬戸 書き込みが動かないので修正 2008.11.13 end
            End If

            ' CMTスタッカ装着
            For i As Integer = 0 To nWriteQuantity - 1
                Try
                    nWriteCounter = 0
                    With cmtReadData(i)
                        .Init(i + 1, i + 1, False, True)
                        If cmtCtrl.SelectCmt(CByte(i + 1)) Then
                            ' cmt書込データ保持インスタンスの初期化
                            cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                            cmtWriteData(i).bListUpFlag = True
                            cmtWriteData(i).bUploadSucceedFlg = False

                            bRet = CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl)

                            If bRet AndAlso .ItakuCounter >= 0 Then
                                ' CMTファイルの読込に成功
                                .ReadSucceedFlag = True

                                ' 返却ファイル名作成
                                If .ItakuData(0).strMultiKbn = "0" Then
                                    strWriteFileName(i) = MakeServerFileName(False, True, nFormatKbn) & "O" & .ItakuData(0).strToriSCode & .ItakuData(0).strToriFCode & ".dat"
                                Else
                                    strWriteFileName(i) = MakeServerFileName(False, True, nFormatKbn) & "O" & .ItakuData(0).strItakuKanriCode & ".dat"
                                End If

                                If File.Exists(strWriteFileName(i)) Then ' 返却ファイルの存在チェック
                                    ' 対応する返却ファイルが存在

                                    cmtWriteData(i).FileName = "N" & .ItakuData(0).strItakuCode & .ItakuData(0).strFuriDate & ".dat"
                                    cmtWriteData(i).bListUpFlag = True

                                    Try
                                        ' 1.返却ファイルの検索と解析
                                        bRet = ParseServerFile(strWriteFileName(i), cmtWriteData(i), cmtFormat, nFormatKbn)
                                        cmtWriteData(i).WriteCounter = nWriteCounter
                                    Catch ex As Exception
                                        cmtWriteData(i).setError(5, "サーバ上の返却ファイルが正しくありません")
                                        With GCom.GLog
                                            .Result = MenteCommon.clsCommon.NG
                                            .Discription = "サーバ上の返却ファイルが正しくありません ファイル名：" & strWriteFileName(i) & ex.Message
                                        End With
                                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                    End Try

                                    If Not bRet Then
                                        ' 等しくないときは何もしない

                                    ElseIf (cmtfServer.FirstRead(strWriteFileName(i)) <> 1) Then ' 先頭レコードを読込
                                        ' 返却ファイルの読込失敗
                                        cmtWriteData(i).InitEmptyItakuData(False)

                                    ElseIf CompareCMTFile(nFormatKbn, cmtFormat, cmtfServer, cmtWriteData(i)) AndAlso _
                                            DB.GetWriteCounter(cmtWriteData(i).ItakuData(0).strItakuCode, _
                                                cmtWriteData(i).ItakuData(0).strFuriDate, nWriteCounter) Then
                                        ' 2返却ファイルとCMTファイルの比較
                                        '   a.クラス同士の比較
                                        '   b.先頭5レコード同士の比較
                                        ' 3.CMT書込実績TBLを参照し、過去に何回書き込んでいるかをチェックする
                                        '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加 START
                                        'DB.GetFToriCode(cmtWriteData(i).ItakuData(0).strItakuCode, "", toris, torif, fkbn, nFormatKbn)
                                        DB.aGetFToriCode(cmtWriteData(i).ItakuData(0).strItakuCode, "", toris, torif, fkbn, nFormatKbn, cmtWriteData(i).ItakuData(0).strFuriDate)
                                        '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加  END
                                        Call GetJSFlag(fkbn, bJSFlag)
                                        If Not bJSFlag Then
                                            cmtWriteData(i).setError(9, "総給振データには対応していません")
                                            cmtWriteData(i).bListUpFlag = True
                                            With GCom.GLog
                                                .Result = MenteCommon.clsCommon.NG
                                                .Discription = "総給振データ ファイル名：" & cmtWriteData(i).FileName
                                            End With
                                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                        Else
                                            '2回以上書き込まれていても、書き込みOK対応 start
                                            'If (nWriteCounter >= 2) Then
                                            '    cmtWriteData(i).setError(9, "同一のCMT返却ファイルが既に二回以上書込み済みです")
                                            '    With GCom.GLog
                                            '        .Result = MenteCommon.clsCommon.NG
                                            '        .Discription = "同一ファイル書込み済 ファイル名：" & cmtWriteData(i).FileName
                                            '    End With
                                            '    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                            'Else
                                            Dim Ret As Boolean
                                            ' 3.サーバからローカルPC書込用フォルダへのコピー
                                            Ret = download(strWriteFileName(i), gstrCMTWriteFileName(i), cmtWriteData(i), cmtFormat.RecordLen)

                                            ' 4.返却ファイルのCMTへの書込
                                            If Not cmtCtrl.WriteCmt(CByte(i + 1)) Then
                                                cmtWriteData(i).setError(9, "CMT書込みエラー")
                                                Ret = False
                                            End If

                                            '5.媒体書き込み検証
                                            If Ret Then
                                                If nFormatKbn <> 6 Then 'NHK以外
                                                    Dim BkCmtWriteData As clsCmtData = cmtWriteData(i)
                                                    Dim ItakuCounter As Integer = cmtWriteData(i).ItakuCounter
                                                    .Init(i + 1, i + 1, False, True)
                                                    cmtCtrl.SelectCmt(CByte(i + 1))
                                                    cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                                                    cmtWriteData(i).bListUpFlag = True
                                                    cmtWriteData(i).bUploadSucceedFlg = False
                                                    File.Delete(cmtFormat.FileName)
                                                    If CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl) = False OrElse _
                                                        ChkCMTFile(nFormatKbn, cmtFormat, cmtWriteData(i)) = False Then
                                                        With GCom.GLog
                                                            .Result = MenteCommon.clsCommon.NG
                                                            .Discription = "媒体書き込み検証失敗"
                                                        End With
                                                        cmtWriteData(i) = BkCmtWriteData
                                                        cmtWriteData(i).ErrCode = 9
                                                        cmtWriteData(i).ItakuCounter = ItakuCounter
                                                        cmtWriteData(i).setError(9, "媒体書き込み検証失敗")
                                                    Else
                                                        cmtWriteData(i) = BkCmtWriteData
                                                        cmtWriteData(i).ItakuCounter = ItakuCounter
                                                    End If
                                                    '2010.03.18 鶴来信金カスタマイズ プロテクト時強制終了 START
                                                End If
                                            Else
                                                MessageBox.Show(MSG0033E, "CMT書き込み", _
                                                           MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                cmtWriteData(i).ErrCode = 9
                                                cmtWriteData(i).setError(9, "正書き込み失敗")
                                                GoTo NEXT_RECORD
                                                '2010.03.18 鶴来信金カスタマイズ プロテクト時強制終了  END
                                            End If
                                            '副書き込み
                                            If MessageBox.Show(String.Format(MSG0069I, toris, torif, .ItakuData(0).strItakuKana), "CMT書き込み", _
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                                                Dim BkCmtWriteData As clsCmtData = cmtWriteData(i)
                                                Dim ItakuCounter As Integer = cmtWriteData(i).ItakuCounter
                                                Try
                                                    cmtCtrl.UnloadCmt()
                                                    MessageBox.Show(MSG0070I, "CMT書き込み", _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                    cmtCtrl.ReadCmt(CByte(1))
                                                    'CMT内のファイルを読み込み
                                                    .Init(i + 1, i + 1, False, True)
                                                    cmtCtrl.SelectCmt(CByte(i + 1))
                                                    cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                                                    cmtWriteData(i).bListUpFlag = True
                                                    cmtWriteData(i).bUploadSucceedFlg = False
                                                    File.Delete(cmtFormat.FileName)
                                                    Ret = CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl)
                                                Catch ex As Exception
                                                    Ret = False
                                                End Try

                                                If Ret AndAlso .ItakuCounter >= 0 AndAlso _
                                                    CompareCMTFile(nFormatKbn, cmtFormat, cmtfServer, cmtWriteData(i)) Then
                                                    ' 2返却ファイルとCMTファイルの比較

                                                    If File.Exists(gstrCMTWriteFileName(i)) Then
                                                        File.Delete(gstrCMTWriteFileName(i))
                                                    End If

                                                    ' 3.サーバからローカルPC書込用フォルダへのコピー
                                                    download(strWriteFileName(i), gstrCMTWriteFileName(i), cmtWriteData(i), cmtFormat.RecordLen)

                                                    ' 4.返却ファイルのCMTへの書込
                                                    If Not cmtCtrl.WriteCmt(CByte(i + 1)) Then
                                                        cmtWriteData(i).setError(9, "CMT書込みエラー")
                                                        Ret = False
                                                    End If

                                                    '5.媒体書き込み検証
                                                    If Ret Then
                                                        If nFormatKbn <> 6 Then 'NHK以外
                                                            .Init(i + 1, i + 1, False, True)
                                                            cmtCtrl.SelectCmt(CByte(i + 1))
                                                            cmtWriteData(i).Init(nReceptionNo, i + 1, False, False)
                                                            cmtWriteData(i).bListUpFlag = True
                                                            cmtWriteData(i).bUploadSucceedFlg = False
                                                            File.Delete(cmtFormat.FileName)
                                                            If CheckCMTFile(i + 1, nFormatKbn, cmtReadData(i), cmtFormat, cmtCtrl) = False OrElse _
                                                                ChkCMTFile(nFormatKbn, cmtFormat, cmtWriteData(i)) = False Then
                                                                With GCom.GLog
                                                                    .Result = MenteCommon.clsCommon.NG
                                                                    .Discription = "副媒体書き込み検証失敗"
                                                                End With
                                                                cmtWriteData(i) = BkCmtWriteData
                                                                cmtWriteData(i).ErrCode = 9
                                                                cmtWriteData(i).ItakuCounter = ItakuCounter
                                                                cmtWriteData(i).setError(9, "副媒体書き込み検証失敗")
                                                            Else
                                                                cmtWriteData(i) = BkCmtWriteData
                                                                cmtWriteData(i).ItakuCounter = ItakuCounter
                                                            End If
                                                            nWriteCounter += 1
                                                        End If
                                                    Else
                                                        If Not Ret AndAlso nFormatKbn <> 6 Then
                                                            MessageBox.Show(MSG0033E, "CMT書き込み", _
                                                                       MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                            cmtWriteData(i) = BkCmtWriteData
                                                            cmtWriteData(i).ErrCode = 9
                                                            cmtWriteData(i).ItakuCounter = ItakuCounter
                                                            cmtWriteData(i).setError(9, "副書き込み失敗")
                                                        End If
                                                    End If
                                                Else
                                                    MessageBox.Show(MSG0033E, "CMT書き込み", _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                    cmtWriteData(i) = BkCmtWriteData
                                                    cmtWriteData(i).ErrCode = 9
                                                    cmtWriteData(i).ItakuCounter = ItakuCounter
                                                    cmtWriteData(i).setError(9, "副読み込み失敗")
                                                End If

                                            End If
                                            '2回以上書き込まれていても、書き込みOK対応 start
                                            nWriteCounter += 1
                                            'End If
                                            '2回以上書き込まれていても、書き込みOK対応 end
                                            End If
                                        cmtWriteData(i).WriteCounter = nWriteCounter
                                        cmtfServer.Close()
                                    End If
                                Else
                                    ' 対応する返却ファイルが未存在
                                    With GCom.GLog
                                        .Result = MenteCommon.clsCommon.NG
                                        .Discription = "返却ファイルなし ファイル名：" & strWriteFileName(i)
                                    End With
                                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                    cmtWriteData(i).InitEmptyItakuData(False)
                                    cmtWriteData(i).setError(9, "返却ファイル無し")
                                End If
                            Else
                                ' CMT読込失敗
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "CMT読込エラー スタッカ番号：" & (i + 1).ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                cmtWriteData(i).InitEmptyItakuData(True)
                            End If
                            cmtFormat.Close()

                            nReceptionNo += 1
                            ' 6.CMT書込実績TBLに書込み - 正常時・エラー時ともに書込み
                            For j As Integer = 0 To cmtWriteData(i).ItakuCounter
                                strSQL = cmtWriteData(i).GetWInsertSQL(j) ' ファイルSEQ単位でSQL文生成
                                GCom.DBExecuteProcess(4, strSQL)
                            Next j
                        End If
                    End With
                    '2010.03.18 鶴来信金カスタマイズ プロテクト時強制終了 START
NEXT_RECORD:
                    '2010.03.18 鶴来信金カスタマイズ プロテクト時強制終了  END
                    ' 7.ListViewに結果を表示
                    If cmtWriteData(i).bListUpFlag Then
                        Me.AddListView(listview, cmtWriteData(i), nFirstReceptionNo)
                        nFirstReceptionNo += 1
                    Else
                        'MessageBox.Show("スタッカ番号:" & (i + 1).ToString & "は表示せず")
                    End If
                Catch ex As Exception
                    With GCom.GLog
                        .Job2 = "CMT書込"
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = ex.Message
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End Try
            Next i

            If nWriteQuantity = 1 Then
                cmtCtrl.UnloadCmt()
            Else
                cmtCtrl.UnloadCmt()
                cmtCtrl.SelectCmt(22) ' CMTスタッカ排出
            End If

            cmtFormat.Close()
            cmtfServer.Close()
            cmtFormat.Dispose()
            cmtfServer.Dispose()


            For i As Integer = 0 To nWriteQuantity - 1
                '2回目以降はDENBKにmoveする。 start
                'If cmtWriteData(i).WriteCounter > 1 Then
                '    ' 書込カウンタが2以上のとき削除
                '    Me.DeleteFile(strWriteFileName(i))
                'End If
                If cmtWriteData(i).ErrCode = 0 Then
                    If cmtWriteData(i).WriteCounter > 0 Then
                        ' 書込カウンタが1以上のときDENBKにmove
                        Dim INI_COMMON_DENBK As String = ""
                        INI_COMMON_DENBK = CASTCommon.GetFSKJIni("COMMON", "DENBK")
                        '前回ファイルを削除
                        If File.Exists(INI_COMMON_DENBK & Path.GetFileName(strWriteFileName(i))) = True Then
                            File.Delete(INI_COMMON_DENBK & Path.GetFileName(strWriteFileName(i)))
                        End If
                        File.Move(strWriteFileName(i), INI_COMMON_DENBK & Path.GetFileName(strWriteFileName(i)))
                    End If
                    '2回目以降はDENBKにmoveする。 end
                End If
            Next i

        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT書込"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function

    ' 機　能 : CMT強制書込書込処理(Complock/非Complock強制書込)
    '
    ' 戻り値 : 処理成功 True / 処理失敗 False
    '
    ' 引き数 : ARG1 - 委託者コード
    '          ARG2 - 取引先主コード
    ' 　　　   ARG3 - 取引先副コード
    '          ARG4 - 振替日
    '          ARG5 - ListView(参照渡)
    '          ARG6 - Complock(True)か平文か(false)
    ' 備 考  : 非Complockの強制書込に対応
    Public Function ComplockCmtWriter(ByVal strItakuCode As String _
        , ByVal strToriSCode As String _
        , ByVal strToriFCode As String _
        , ByVal strFuriDate As String _
        , ByRef listview As ListView _
        , ByVal bComplock As Boolean _
        ) As Boolean

        ' 内部処理用変数の宣言
        Dim nReceptionNo As Integer             ' 受付番号
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing     ' フォーマットクラス
        'Dim cmtFormat As CAstFormat.CFormat     ' フォーマットクラス
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim strSQL As String                    ' SQL文格納用変数　
        Dim strPreRecord As String = ""         ' 一個前のレコードのデータ区分
        Dim cmtReadData As clsCmtData           ' CMT読込結果の解析結果
        Dim cmtWriteData As clsCmtData          ' サーバ返却ファイルの解析結果
        Dim strWriteFileName As String          ' サーバ返却ファイル名(フルパス)
        Dim cmtCtrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim nWriteCounter As Integer = 0
        Dim nFormatKbn As Integer
        Dim cmtc As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim nLabelKbn As Integer

        If (bComplock) Then
            GCom.GLog.Job2 = "CMT暗号化書込"
        Else
            GCom.GLog.Job2 = "強制書込"
        End If

        If (strFuriDate.Length < 8) Then
            MessageBox.Show("振替日が不正です。(" & strFuriDate & ").有効な日付を入力してください", "振替日エラー")
            '*** 修正 mitsu 2008/09/01 不要 ***
            'With GCom.GLog
            '    .Result = "振替日が不正です"
            '    .Discription = strFuriDate
            'End With
            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            '**********************************
            Return False
        End If
        cmtReadData = New clsCmtData
        cmtWriteData = New clsCmtData

        '*** 修正 mitsu 2008/09/01 CMT接続確認 ***
        If Not Me.CheckConnectCMT() Then ' CMTが接続されていない場合は終了
            Return False
        End If
        '*****************************************

        ' TORI_VIEWからフォーマット区分の取得
        If Not DB.GetFormatKbn(strItakuCode, nFormatKbn) Then
            '*** 修正 mitsu 2008/09/01 エラーメッセージ表示 ***
            MessageBox.Show("フォーマット区分取得失敗" & vbCrLf & "委託者コード：" & strItakuCode, _
                GCom.GLog.Job2, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If

        '*** 修正 mitsu 2008/09/12 ブロックサイズ対応 ***
        Select Case nFormatKbn
            Case 0
                cmtCtrl.BlockSize = 1800
            Case 1
                cmtCtrl.BlockSize = 2100
            Case 2
                cmtCtrl.BlockSize = 3000
            Case 3
                cmtCtrl.BlockSize = 3900
            Case Else
                cmtCtrl.BlockSize = -1
        End Select
        '************************************************

        ' TORI_VIEWからラベル区分の取得
        If Not DB.GetFormatKbn(strItakuCode, nLabelKbn) Then
            '*** 修正 mitsu 2008/09/01 エラーメッセージ表示 ***
            MessageBox.Show("ラベル区分取得失敗" & vbCrLf & "委託者コード：" & strItakuCode, _
                GCom.GLog.Job2, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If

        ' フォーマット区分に応じてデータ保持クラスのインスタンス生成
        If Not GetFormat(nFormatKbn, cmtFormat) Then
            Return False
        End If

        ' ローカルフォルダの掃除
        If (Not LocalFileDelete(True)) Or (Not LocalFileDelete(False)) Then
            '*** 修正 mitsu 2008/09/01 エラーメッセージ表示 ***
            MessageBox.Show("ローカルPC内の不要ファイルの削除に失敗しました。", "書込処理", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If

        ' 受付番号(nReceptionNo)を取得
        If Not CmtCom.GetReceptionNo(False, nReceptionNo) Then
            ' 受付番号取得失敗
            ' ログ出力は関数内部で行っているため何もしないで終了
            Return False
        End If

        Try
            ' CMTは1本さし前提
            nWriteCounter = 0
            With cmtReadData
                .Init(1, 1, False, True)
                If Not IsExistStacker(cmtCtrl, True) Then
                    ' cmt書込データ保持インスタンスの初期化
                    cmtWriteData.Init(nReceptionNo, 1, False, False)
                    cmtWriteData.bListUpFlag = True
                    cmtWriteData.bUploadSucceedFlg = False
                    cmtWriteData.Override = True

                    ' 返却ファイル名作成
                    If (bComplock) Then
                        strWriteFileName = MakeServerFileName(False, True, nFormatKbn) & "C" & strItakuCode & strFuriDate & ".dat"
                    Else
                        strWriteFileName = MakeServerFileName(False, True, nFormatKbn) & "N" & strItakuCode & strFuriDate & ".dat"
                    End If
                    If (bComplock) Then
                        cmtWriteData.ComplockInit(strItakuCode, strFuriDate)
                        cmtWriteData.ComplockFlag = bComplock
                    Else
                        If (cmtFormat.FirstRead(strWriteFileName) = 1) Then
                            If Not (Me.ParseCMTFormat(nFormatKbn, cmtFormat, cmtWriteData, False)) Then
                                Return False
                            End If
                            cmtWriteData.ComplockFlag = bComplock
                        Else
                            cmtWriteData.InitEmptyItakuData(False)
                        End If
                    End If
                    ' 返却ファイルの存在チェック

                    If File.Exists(strWriteFileName) Then
                        ' 対応する返却ファイルが存在

                        If (bComplock) Then
                            cmtWriteData.FileName = "C" & strItakuCode & strFuriDate & ".dat"
                        Else
                            cmtWriteData.FileName = "N" & strItakuCode & strFuriDate & ".dat"
                        End If

                        cmtWriteData.bListUpFlag = True

                        ' CMT書込実績TBLを参照し、過去に何回書き込んでいるかをチェックする
                        Call DB.GetWriteCounter(strItakuCode, strFuriDate, nWriteCounter)

                        If (nWriteCounter >= 2) Then
                            MessageBox.Show("既に同じ返却ファイルが二回以上CMTに書込みされています", "CMT書込エラー")
                            cmtWriteData.setError(9, "同一のCMT返却ファイルが既に二回以上書込み済みです")
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "同一ファイル書込み済 ファイル名：" & cmtWriteData.FileName
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        Else
                            ' サーバからローカルPC書込用フォルダへのコピー
                            If (download(strWriteFileName, gstrCMTWriteFileName(0), cmtWriteData, cmtFormat.RecordLen)) Then
                            Else
                                Return False
                            End If

                            ' 返却ファイルのCMTへの書込
                            CMT.PutCMTIni("LABEL-EXIST", "1", nLabelKbn.ToString) ' ラベル区分の書込み
                            If Not cmtc.WriteCmt(1) Then
                                Dim result As String = CMT.GetCMTIni("WRITE-RESULT", "1")
                                cmtWriteData.bUploadSucceedFlg = False
                                cmtWriteData.setError(9, "CMT書込失敗")
                            Else
                                cmtWriteData.bUploadSucceedFlg = True
                                nWriteCounter += 1
                                If (nWriteCounter >= 2) Then
                                    Me.DeleteFile(strWriteFileName) ' 書込成功時にサーバのファイルを削除
                                End If
                            End If
                        End If

                        cmtWriteData.WriteCounter = nWriteCounter

                        ' CMT書込実績TBLに書込み
                        strSQL = cmtWriteData.GetWInsertSQL(0) ' ファイルSEQ単位でSQL文生成
                        GCom.DBExecuteProcess(4, strSQL)

                        ' ListViewに結果を表示
                        If cmtWriteData.bListUpFlag Then
                            Try
                                Dim lv As New ListViewItem(nReceptionNo.ToString)
                                cmtWriteData.getListViewItem(lv, 0)
                                listview.Items.AddRange(New ListViewItem() {lv})
                            Catch ex As Exception
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "ListView追加失敗 " & ex.Message
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                Return False
                            End Try
                        End If
                    Else
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "サーバ返却ファイルなし ファイル名：" & strWriteFileName
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    End If
                End If
            End With
        Catch ex As Exception
            With GCom.GLog
                If bComplock Then
                    .Job2 = "ComplockCMT書込"
                Else
                    .Job2 = "CMT強制書込"
                End If
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If listview.Items.Count > 0 AndAlso _
            MessageBox.Show("印刷を実行しますか？", "CMT読込結果印刷", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            If bComplock Then
                Me.PrintButton(listview, "ComplockCMT書込")
            Else
                Me.PrintButton(listview, "CMT強制書込")
            End If
        End If

        cmtCtrl.UnloadCmt()
        cmtFormat.Dispose()

    End Function


    ' 機能   ： スタッカに複数のCMTが存在するかチェック
    '
    ' 引数    : ARG1 - なし
    ' 戻り値  : スタッカに複数のCMTが存在存在 / スタッカ無し false
    Private Function IsExistStacker(ByVal cmtCtrl As CMT.ClsCMTCTRL, ByVal bComplock As Boolean) As Boolean
        Dim nSlotPosition As Byte        ' スタッカ位置
        Dim nCMTSum As Integer              ' CMT本数

        If cmtCtrl.SelectCmt(21) Then
            cmtCtrl.ChkCmtStat(nSlotPosition, nCMTSum)
            If (nCMTSum > 1) Then
                If bComplock Then ' Complockの場合はエラーを出力
                    MessageBox.Show("Complock書込機能/CMT強制書込みはCMT一本毎に処理を行います。スタッカには対応していません。", _
                        "スタッカエラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.OK
                        .Discription = "複数のCMTが同時にセット CMTセット数：" & nCMTSum.ToString
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    cmtCtrl.SelectCmt(22) ' CMT Eject
                End If
                Return True
            Else
                If (nCMTSum = 0) Then
                    Me.CMTNotExistErr()
                    Return False
                End If
            End If
        Else
            Return True
        End If
    End Function


    ' 機能   ： CMTが接続されているかチェックし、エラーを返す
    '
    ' 引数    : ARG1 - なし
    ' 戻り値  : 成功 true / 失敗 false
    Private Function CheckConnectCMT() As Boolean
        Dim CMTSCSIReader As New CMT.ClsCMTCTRL
        If Not CMTSCSIReader.ChkLoader() Then
            MessageBox.Show("CMTが接続されていないか、あるいは電源が入っていません。", "CMT接続エラー", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMTリーダの電源がOFFか、ケーブルが外れている可能性があります"
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Else
            Return True
        End If
    End Function


    ' 機能   ： スタッカ内のCMTファイルの読込
    '
    ' 引数    : ARG1 - nRead
    ' 戻り値  : 成功 true / 失敗 false
    Private Function ReadStacker(ByVal nRead As Integer, ByVal nMax As Integer) As Boolean
        ' CMT読込関数を呼び出し
        Dim CMTSCSIReader As New CMT.ClsCMTCTRL
        Dim nCmtRet As Integer
        Dim bStackerEmpty As Boolean = True
        Dim bNoCmt As Boolean = True
        Dim bRet As Boolean = True
        Try
            If Not CheckConnectCMT() Then 'CMTが接続されていない場合
                Return False
            End If

            ' CMTクラスの読込関数
            nCmtRet = CMTSCSIReader.CmtCtrl(nRead)

            ' CMTスタッカ空チェック [READ-RESULT]が全部1のときスタッカが空
            For i As Integer = 1 To nMax Step 1
                If CMT.GetCMTIni("READ-RESULT", i.ToString) <> "1" Then ' 各スロットのCMTが空ではないかチェック
                    bStackerEmpty = False
                End If
            Next i

            If bStackerEmpty Then ' 手差しでCMTが一本も入っていない
                Me.CMTNotExistErr() 'CMTが空のエラー
                bRet = False
            End If

            If nCmtRet <> 0 Then
                If nMax = 1 Then
                    MessageBox.Show("CMTの読込に失敗しました", "CMT読込エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "CMT読込失敗 CMT.ClsCMTCTRL.CmtCtrl(11)の返り値:" & nCmtRet
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                bRet = False
            End If

            CMTSCSIReader.UnloadCmt()
            'CMTSCSIReader.SelectCmt(22) ' CMT Eject

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT読込失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

        Return bRet
    End Function


    ' 機能   ： フォーマット区分に応じてデータ保持クラスのインスタンス生成
    '
    ' 引数    : ARG1 - フォーマット区分
    '         : ARG2 - フォーマットのインスタンス
    ' 戻り値  : 成功 true / 失敗 false
    Private Function GetFormat(ByVal nkbn As Integer, ByRef cmtFormat As CAstFormat.CFormat) As Boolean
        Try
            Select Case nkbn
                Case 0, 6, 7
                    ' 全銀フォーマット(全銀・NHK・総振)
                    cmtFormat = New CAstFormat.CFormatZengin
                    Return True
                Case 1
                    ' 地方公共団体1
                    cmtFormat = New CAstFormat.CFormatZeikin350
                    Return True
                Case 2
                    ' 地方公共団体2 岡崎市
                    cmtFormat = New CAstFormat.CFormatZeikin300
                    Return True
                Case 3
                    ' 国税
                    cmtFormat = New CAstFormat.CFormatKokuzei
                    Return True
                Case 4
                    ' センター持込(請求分)
                    cmtFormat = New CAstFormat.CFormatTokCenter
                    Return True
                Case 5
                    ' センター持込(結果分)のフォーマット.実際には使わないが、動作させるため指定
                    cmtFormat = New CAstFormat.CFormatTokCenter
                    Return True
                Case Else
                    ' その他フォーマットを選択
                    MessageBox.Show("未実装のフォーマットが選択されています", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Return False
            End Select
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "インスタンス生成失敗 フォーマット区分：" & nkbn.ToString & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

    End Function

    ' 機能   ： 全銀/地方1/2/国税/センター持込フォーマットの解析
    '
    ' 引数    : ARG1 - CAstFormat.CFormatのインスタンス
    '         : ARG2 - 解析結果を格納するためのclsCmtReadDataのインスタンス
    '
    ' 戻り値  : 成功 true / 失敗 false
    ' 備考    : エラーチェックの実装漏れを追加, 全銀以外のフォーマットに対応 2007/12/10
    Private Function ParseCMTFormat(ByVal nFormatKbn As Integer, ByRef cmtFormat As CAstFormat.CFormat, _
        ByRef cmtData As clsCmtData, ByVal bCheckZumiFlag As Boolean) As Boolean
        Dim strRecordKbn As String
        Dim nRecordCounter As Integer = 0

        Try
            With cmtData
                Do Until cmtFormat.EOF = True ' フォーマットの解析開始
                    strRecordKbn = cmtFormat.CheckDataFormat()

                    '*** ASTAR.S.S 異常時対応       ***
                    If cmtFormat.IsHeaderRecord = True Then
                        Call cmtFormat.CheckRecord1()

                        '***ASTAR SUSUKI 2008.06.12                     ***
                        If nFormatKbn = 3 Then
                            ' 国税の場合
                            strRecordKbn = "H"
                            Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
                            KokuzeiFmt.KOKUZEI_REC1.Data = cmtFormat.RecordData
                            Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
                            KokuzeiFmt.Close()
                            Dim TORICODE As String
                            ' 科目コード　020:申告所得税, 300:消費税及地方消費税
                            TORICODE = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
                            If TORICODE = "err" Then
                                TORICODE = ""
                            End If
                            cmtFormat.InfoMeisaiMast.ITAKU_CODE = TORICODE
                            'Dim MainDB As New CASTCommon.MyOracle
                            'cmtFormat.ToriData = New CAstBatch.CommData(MainDB)
                            'Call cmtFormat.GetTorimastFromToriCode(TORICODE, MainDB)
                            'If Not cmtFormat.ToriData Is Nothing Then
                            '    cmtFormat.InfoMeisaiMast.ITAKU_CODE = cmtFormat.ToriData.INFOToriMast.ITAKU_CODE_T
                            'End If
                            'MainDB.Close()
                        End If
                        '**************************************************

                        cmtFormat.InfoMeisaiMast.FILE_SEQ += 1
                        '***ASTAR SUSUKI 2008.06.12                         ***
                        '***国税の場合はレコード区分３のみカウント または 国税以外
                        If nFormatKbn = 3 Then
                            .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, CASTCommon.ConvertDate(cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO, "yyyyMMdd") _
                                , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT, "", "", "", nFormatKbn)
                            '***************************************************

                            '*** 修正 mitsu 2008/07/17 センター直接持込の場合は振替コード・企業コードも渡す ***
                        ElseIf nFormatKbn = 4 Then
                            .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO _
                                , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                      , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT _
                                      , CType(cmtFormat, CAstFormat.CFormatTokCenter).TOKCENTER_REC1.TC.TC5 _
                                      , CType(cmtFormat, CAstFormat.CFormatTokCenter).TOKCENTER_REC1.TC.TC6 _
                                      , "", nFormatKbn)
                            '**********************************************************************************
                        Else
                            '*** 修正 mitsu 2008/07/23 日付が読み取れない場合対応 ***
                            If cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO.Length < 8 Then
                                .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE _
                                    , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                    , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT, "", "", cmtFormat.InfoMeisaiMast.SYUBETU_CODE, nFormatKbn)
                            Else
                                .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE_MOTO _
                                                      , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                                                      , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT, "", "", cmtFormat.InfoMeisaiMast.SYUBETU_CODE, nFormatKbn)
                            End If
                            '********************************************************
                        End If
                        nRecordCounter += 1
                        If Not Me.CheckFuriDate(cmtFormat.InfoMeisaiMast.FURIKAE_DATE, Me.gstrSysDate, False) Then
                            '*** 修正 mitsu 2008/09/01 不要 ***
                            'With GCom.GLog
                            '    .Result = "CMTフォーマット区分解析処理"
                            '    .Discription = "振替日が不正です" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            'End With
                            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '**********************************
                            Return False
                        End If

                    ElseIf cmtFormat.IsDataRecord = True Then
                        ' データレコードの値を設定
                        '***ASTAR SUSUKI 2008.06.12                         ***
                        '***国税の場合はレコード区分３のみカウント または 国税以外
                        If nFormatKbn <> 3 Or (nFormatKbn = 3 And cmtFormat.InfoMeisaiMast.DATA_KBN = "3") Then
                            .ItakuData(.ItakuCounter).SetData(cmtFormat.InfoMeisaiMast.FURIKIN)
                            nRecordCounter += 1
                        End If
                        '******************************************************

                    ElseIf cmtFormat.IsTrailerRecord = True Then
                        If Not (.AddTrailerRecord(cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KIN _
                            , cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN _
                            , cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN, cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN)) Then
                            '***ASTAR SUSUKI 2008.06.12                     ***
                            '***トレーラチェックをはずす
                            '.setError(9, "トレーラ件数金額照合エラー")
                            'With GCom.GLog
                            '    .Result = "トレーラ件数金額照合エラー"
                            '    .Discription = "振替日が不正です" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            'End With
                            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            'Return False
                            '**************************************************
                        End If
                        If bCheckZumiFlag AndAlso ((cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN > 0) Or _
                           (cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN > 0) Or _
                           (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN > 0) Or _
                           (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN > 0)) Then
                            '***ASTAR SUSUKI 2008.06.12                     ***
                            '***トレーラチェックをはずす
                            '.setError(9, "トレーラ件数金額照合エラー")
                            'With cmtFormat.InfoMeisaiMast
                            '    GCom.GLog.Result = "トレーラに不能件・済件が含まれています"
                            '    GCom.GLog.Discription = "不能件:" & .TOTAL_FUNO_KEN.ToString & ", 不能金:" & .TOTAL_FUNO_KIN.ToString & _
                            '        "済件数:" & .TOTAL_ZUMI_KEN.ToString & "済金額" & .TOTAL_ZUMI_KIN.ToString
                            'End With
                            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            'Return False
                            '**************************************************
                        End If
                        nRecordCounter += 1

                    ElseIf cmtFormat.IsEndRecord = True Then
                        ' なにもしない
                        nRecordCounter += 1
                    End If
                    '**********************************

                    Select Case strRecordKbn
                        Case "ERR"
                            If cmtFormat.ErrorNumber = 1 Then
                                cmtData.setError(9, "CMT読取エラー")
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "CMTレコード区分エラー エラー番号：" & cmtFormat.ErrorNumber.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                Return False
                            End If
                        Case "H", "D", "T", "E"
                            'Case "H"
                            '    If Not Me.CheckFuriDate(cmtFormat.InfoMeisaiMast.FURIKAE_DATE, Me.gstrSysDate, False) Then
                            '        With GCom.GLog
                            '            .Result = "CMTフォーマット区分解析処理"
                            '            .Discription = "振替日が不正です" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            '        End With
                            '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '        Return False
                            '    End If
                            '    cmtFormat.InfoMeisaiMast.FILE_SEQ += 1
                            '    .AddHeaderRecord(cmtFormat.InfoMeisaiMast.FILE_SEQ, cmtFormat.InfoMeisaiMast.FURIKAE_DATE _
                            '        , cmtFormat.InfoMeisaiMast.ITAKU_CODE, cmtFormat.InfoMeisaiMast.ITAKU_KNAME _
                            '        , cmtFormat.InfoMeisaiMast.ITAKU_KIN, cmtFormat.InfoMeisaiMast.ITAKU_SIT)
                            '    nRecordCounter += 1
                            'Case "D"
                            '    ' データレコードの値を設定
                            '    .ItakuData(.ItakuCounter).SetData(cmtFormat.InfoMeisaiMast.FURIKIN)
                            '    nRecordCounter += 1
                            'Case "T"
                            '    If Not (.AddTrailerRecord(cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_IRAI_KIN _
                            '        , cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN, cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN _
                            '        , cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN, cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN)) Then
                            '        .setError(9, "トレーラ件数金額照合エラー")
                            '        With GCom.GLog
                            '            .Result = "トレーラ件数金額照合エラー"
                            '            .Discription = "振替日が不正です" & cmtFormat.InfoMeisaiMast.FURIKAE_DATE
                            '        End With
                            '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '        Return False
                            '    End If
                            '    If bCheckZumiFlag AndAlso ((cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KEN > 0) Or _
                            '       (cmtFormat.InfoMeisaiMast.TOTAL_FUNO_KIN > 0) Or _
                            '       (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KEN > 0) Or _
                            '       (cmtFormat.InfoMeisaiMast.TOTAL_ZUMI_KIN > 0)) Then
                            '        .setError(9, "トレーラ件数金額照合エラー")
                            '        With cmtFormat.InfoMeisaiMast
                            '            GCom.GLog.Result = "トレーラに不能件・済件が含まれています"
                            '            GCom.GLog.Discription = "不能件:" & .TOTAL_FUNO_KEN.ToString & ", 不能金:" & .TOTAL_FUNO_KIN.ToString & _
                            '                "済件数:" & .TOTAL_ZUMI_KEN.ToString & "済金額" & .TOTAL_ZUMI_KIN.ToString
                            '        End With
                            '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            '        Return False
                            '    End If
                            '    nRecordCounter += 1
                            'Case "E"
                            '    ' なにもしない
                            '    nRecordCounter += 1
                        Case "IJO"
                            cmtData.setError(9, "CMT読取エラー")
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "CMTファイル解析時に異常検出"
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            Return False
                        Case "99"   '*** ASTAR SUSUKI 2008.06.05 9レコードがいっぱいに対応

                        Case Else
                            ' CMTファイルの読込は出来たが、ファイルが破損していて全く解析出来なかった場合の処理
                            .setError(9, "CMT読取エラー")
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "不明なレコード区分を検出 レコード区分：" & strRecordKbn
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            Return False
                    End Select
                Loop
            End With
        Catch ex As Exception
            cmtData.setError(9, "CMT読取エラー")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT解析失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        If (nRecordCounter < 4) Then
            cmtData.setError(9, "CMT読取エラー(レコード異常あり)")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "不完全なファイルです。レコード数が不足しています。 レコード数：" & nRecordCounter
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End If

        Return True
    End Function

    '*** 修正 mitsu 2008/09/01 処理見直しのため作り直し ***
#Region "旧CompareCMTFile"
    'Private Function CompareCMTFile(ByVal nFormatKbn As Integer, ByRef cmtfSrc As CAstFormat.CFormat, ByRef cmtfDst As CAstFormat.CFormat, ByRef cmtData As clsCmtData) As Boolean
    '    Dim strSrcRecordKbn As String               ' 比較元のレコード区分
    '    Dim strDstRecordKbn As String               ' 比較先のレコード区分
    '    Dim nRecordCounter As Integer = 1           ' レコード数のカウンタ
    '    Dim tempsrc, tempdst As String               ' 文字列操作用テンポラリ変数

    '    Try
    '        With cmtData
    '            Do Until cmtfSrc.EOF = True ' フォーマットの解析開始
    '                strSrcRecordKbn = cmtfSrc.CheckDataFormat()
    '                If Not cmtfDst.EOF Then
    '                    strDstRecordKbn = cmtfDst.CheckDataFormat()
    '                Else
    '                    .setError(9, "レコード異常あり")
    '                    With GCom.GLog
    '                        .Result = "レコード長が相違"
    '                        .Discription = "フォーマット区分:" & nFormatKbn.ToString & ", レコード数カウンタ:" & nRecordCounter.ToString
    '                    End With
    '                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                    Return False
    '                End If

    '                Select Case strSrcRecordKbn
    '                    Case "ERR"
    '                        If cmtfSrc.ErrorNumber = 1 Then
    '                            .setError(9, "CMT読取エラー")
    '                            With GCom.GLog
    '                                .Result = "CMT先頭文字データ区分エラー"
    '                                .Discription = "エラー番号:" & cmtfSrc.ErrorNumber.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            Return False
    '                        Else
    '                            .setError(9, "CMT読取エラー")
    '                            With GCom.GLog
    '                                .Result = "CMTフォーマット区分解析処理"
    '                                .Discription = "区分エラー"
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            Return False
    '                        End If
    '                    Case "H" ' ヘッダレコード
    '                        If nRecordCounter < 6 AndAlso cmtfSrc.RecordData <> cmtfDst.RecordData Then
    '                            With GCom.GLog
    '                                .Result = "CMTフォーマット区分解析処理"
    '                                .Discription = "ヘッダレコードの同一性違反, レコードカウンタ:" & nRecordCounter.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            .setError(7, "同一ファイルではありません")
    '                            Return False
    '                        End If
    '                    Case "D" ' データレコード
    '                        If nRecordCounter < 6 Then
    '                            ' 結果フラグをカットで埋める
    '                            Select Case nFormatKbn
    '                                Case 0 ' 全銀フォーマット
    '                                    CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 111)
    '                                Case 1 ' 地方公共団体(350byte)
    '                                    CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 91)
    '                                Case 2 ' 地方公共団体(300byte) 岡崎市
    '                                    CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 91)
    '                                Case 3 ' 国税
    '                                    ' TODO 仕様がわからないのでチェックをはずす
    '                                    'CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 111)
    '                                    tempsrc = ""
    '                                    tempdst = ""
    '                                Case 4 ' センター持込(請求分)
    '                                    ' TODO センター持込(請求分)のチェック
    '                                    tempsrc = ""
    '                                    tempdst = ""
    '                                Case 5 ' センター持込(結果分)
    '                                    ' TODO センター持込(結果分)のチェック
    '                                    tempsrc = ""
    '                                    tempdst = ""
    '                            End Select
    '                            ' 結果フラグ以外のデータレコードを比較
    '                            If tempsrc <> tempdst Then
    '                                With GCom.GLog
    '                                    .Result = "CMTフォーマット区分解析処理"
    '                                    .Discription = "データレコードの同一性違反, レコードカウンタ:" & nRecordCounter.ToString
    '                                End With
    '                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                                .setError(7, "同一ファイルではありません")
    '                                Return False
    '                            End If
    '                        End If
    '                    Case "T"
    '                        Select Case nFormatKbn
    '                            Case 0 ' 全銀フォーマット
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 18, 56)
    '                            Case 1 ' 地方公共団体(350byte)
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 18, 56)
    '                            Case 2 ' 地方公共団体(300byte) 岡崎市
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 18, 56)
    '                            Case 3 ' 国税
    '                                CutChar(cmtfSrc.RecordData, cmtfDst.RecordData, tempsrc, tempdst, 69, 124)
    '                            Case 4 ' センター持込(請求分)
    '                                ' TODO センター持込(請求分)のチェックの実装
    '                                tempsrc = ""
    '                                tempdst = ""
    '                            Case 5 ' センター持込(結果分)
    '                                ' TODO センター持込(結果分)のチェックの実装
    '                                tempsrc = ""
    '                                tempdst = ""
    '                        End Select
    '                        ' 処理済・不能文以外のトレーラレコードを比較
    '                        If tempsrc <> tempdst Then
    '                            With GCom.GLog
    '                                .Result = "CMTフォーマット区分解析処理"
    '                                .Discription = "トレーラレコードの同一性違反, レコードカウンタ:" & nRecordCounter.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            .setError(7, "同一ファイルではありません")
    '                            Return False
    '                        End If
    '                    Case "E"
    '                        If nRecordCounter < 6 AndAlso cmtfSrc.RecordData <> cmtfDst.RecordData Then
    '                            With GCom.GLog
    '                                .Result = "CMTフォーマット区分解析処理"
    '                                .Discription = "エンドレコードの同一性違反, レコードカウンタ:" & nRecordCounter.ToString
    '                            End With
    '                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                            .setError(7, "同一ファイルではありません")
    '                            Return False
    '                        End If
    '                    Case "IJO"
    '                        .setError(9, "CMT読取エラー")
    '                        With GCom.GLog
    '                            .Result = "CMTファイル解析時に異常検出"
    '                            .Discription = "CMT読取失敗"
    '                        End With
    '                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                        Return False
    '                    Case Else
    '                        .setError(9, "CMT読取エラー")
    '                        With GCom.GLog
    '                            .Result = "cmtFormat.CheckDataFormat()から想定されていない値が返されました"
    '                            .Discription = "レコード区分:(" & strSrcRecordKbn & ")"
    '                        End With
    '                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '                        Return False
    '                End Select
    '                nRecordCounter += 1
    '            Loop
    '        End With
    '    Catch ex As Exception
    '        cmtData.setError(9, "CMT読取エラー")
    '        With GCom.GLog
    '            .Result = "CMT比較時に例外発生"
    '            .Discription = ex.Message & ex.StackTrace
    '        End With
    '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '        Return False
    '    End Try
    '    Return True
    'End Function
#End Region

    ' 機能   ： 全銀/地方1/2/国税/センター持込フォーマットの先頭5レコードの比較
    '
    ' 引数    : ARG1 - フォーマット区分
    '         : ARG2 - 比較元のCAstFormat.CFormatのインスタンス
    '         : ARG3 - 比較先のCAstFormat.CFormatのインスタンス
    '         : ARG3 - 解析結果を格納するためのclsCmtReadDataのインスタンス
    '
    ' 戻り値  : 成功 true / 失敗 false
    ' 備考    : エラーチェックの実装漏れを追加, 全銀以外のフォーマットに対応 2007/12/12
    '         : ヘッダ、トレーラレコードのみチェックする 規定外文字、複数エンドレコード対応 2008/09/01

    Private Function CompareCMTFile(ByVal nFormatKbn As Integer, ByRef cmtfSrc As CAstFormat.CFormat, ByRef cmtfDst As CAstFormat.CFormat, ByRef cmtData As clsCmtData) As Boolean
        Dim SrcQue As New ArrayList                 ' 比較元のレコードキュー
        Dim DstQue As New ArrayList                 ' 比較先のレコードキュー
        Dim SrcEntry, DstEntry As DictionaryEntry   ' キューに追加するエントリー
        Dim SrcTR(1), DstTR(1) As Decimal           ' エントリーに追加するトレーラ件数、金額
        Dim nRecordCounter As Integer = 1           ' レコード数のカウンタ

        Try
            If nFormatKbn = 6 Then  'NHKの場合、絶対一致しない
                Return True
            End If
            With cmtData
                'ファイルを開きなおし、ストリーム初期化
                cmtfSrc.FirstRead()
                cmtfDst.FirstRead()

                '比較元レコードキュー作成
                Do Until cmtfSrc.EOF = True
                    SrcEntry.Key = cmtfSrc.CheckDataFormat()

                    '既に解析済みなのでレコードチェックはしない
                    Select Case SrcEntry.Key.ToString
                        Case "H" ' ヘッダレコード
                            '規定外文字を全て空白に置換しキューに追加
                            SrcEntry.Value = cmtfSrc.ReplaceString(cmtfSrc.RecordData, -1)
                            SrcQue.Add(SrcEntry)

                        Case "T"
                            '依頼件数、金額をキューに追加
                            SrcTR(0) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KEN
                            SrcTR(1) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KIN
                            SrcEntry.Value = SrcTR
                            SrcQue.Add(SrcEntry)
                    End Select
                Loop

                '比較先レコードキュー作成
                Do Until cmtfDst.EOF = True
                    DstEntry.Key = cmtfDst.CheckDataFormat()

                    '既に解析済みなのでレコードチェックはしない
                    Select Case DstEntry.Key.ToString
                        Case "H" ' ヘッダレコード
                            '規定外文字を全て空白に置換しキューに追加
                            DstEntry.Value = cmtfDst.ReplaceString(cmtfDst.RecordData, -1)
                            DstQue.Add(DstEntry)

                        Case "T"
                            '依頼件数、金額をキューに追加
                            DstTR(0) = cmtfDst.InfoMeisaiMast.TOTAL_IRAI_KEN
                            DstTR(1) = cmtfDst.InfoMeisaiMast.TOTAL_IRAI_KIN
                            DstEntry.Value = DstTR
                            DstQue.Add(DstEntry)
                    End Select

                    nRecordCounter += 1
                Loop

                'レコード件数チェック
                If SrcQue.Count <> DstQue.Count Then
                    .setError(9, "レコード異常あり")
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "レコード数相違 フォーマット区分：" & nFormatKbn.ToString & ", レコード数：" & nRecordCounter.ToString
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If

                'ヘッダ・トレーラレコードのみ比較する
                For i As Integer = 0 To SrcQue.Count - 1
                    SrcEntry = CType(SrcQue.Item(i), DictionaryEntry)
                    DstEntry = CType(DstQue.Item(i), DictionaryEntry)

                    'レコード区分チェック(念のため)
                    If SrcEntry.Key.ToString <> DstEntry.Key.ToString Then
                        .setError(9, "レコード異常あり")
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "レコード区分相違 フォーマット区分：" & nFormatKbn.ToString & ", レコード数：" & i.ToString
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        Return False
                    End If

                    Select Case SrcEntry.Key.ToString
                        Case "H" ' ヘッダレコード
                            If SrcEntry.Value.ToString <> DstEntry.Value.ToString Then
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "ヘッダレコードの同一性違反, レコード数：" & i.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                .setError(7, "同一ファイルではありません")
                                Return False
                            End If

                        Case "T" ' トレーラレコード
                            SrcTR = CType(SrcEntry.Value, Decimal())
                            DstTR = CType(DstEntry.Value, Decimal())

                            If SrcTR(0) <> DstTR(0) OrElse _
                               SrcTR(1) <> DstTR(1) Then
                                With GCom.GLog
                                    .Job2 = "CMTファイル比較"
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "トレーラレコードの同一性違反, レコード数：" & i.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                .setError(7, "同一ファイルではありません")
                                Return False
                            End If
                    End Select
                Next
            End With

        Catch ex As Exception
            cmtData.setError(9, "CMT読取エラー")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT比較失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False

        Finally
            '最後にファイルを解放する
            cmtfSrc.Close()
            cmtfDst.Close()
        End Try
        Return True
    End Function
    ' 機能   ： 全銀/地方1/2/国税/センター持込フォーマットの先頭5レコードの比較
    '
    ' 引数    : ARG1 - フォーマット区分
    '         : ARG2 - 比較元のCAstFormat.CFormatのインスタンス
    '         : ARG3 - 解析結果を格納するためのclsCmtReadDataのインスタンス
    '
    ' 戻り値  : 成功 true / 失敗 false
    ' 備考    : エラーチェックの実装漏れを追加, 全銀以外のフォーマットに対応 2007/12/12
    '         : ヘッダ、トレーラレコードのみチェックする 規定外文字、複数エンドレコード対応 2008/09/01

    Private Function ChkCMTFile(ByVal nFormatKbn As Integer, ByVal cmtfSrc As CAstFormat.CFormat, ByVal cmtData As clsCmtData) As Boolean
        Dim SrcQue As New ArrayList                 ' 比較元のレコードキュー
        Dim SrcEntry As DictionaryEntry             ' キューに追加するエントリー
        Dim SrcTR(5) As Decimal                     ' エントリーに追加するトレーラ件数、金額
        Dim nRecordCounter As Integer = 1           ' レコード数のカウンタ

        Try
            With cmtData
                'ファイルを開きなおし、ストリーム初期化
                cmtfSrc.FirstRead()

                '比較元レコードキュー作成
                Do Until cmtfSrc.EOF = True
                    SrcEntry.Key = cmtfSrc.CheckDataFormat()

                    '既に解析済みなのでレコードチェックはしない
                    Select Case SrcEntry.Key.ToString
                        Case "H" ' ヘッダレコード
                            '規定外文字を全て空白に置換しキューに追加
                            SrcEntry.Value = cmtfSrc.ReplaceString(cmtfSrc.RecordData, -1)
                            SrcQue.Add(SrcEntry)

                        Case "T"
                            '依頼件数、金額をキューに追加
                            SrcTR(0) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KEN
                            SrcTR(1) = cmtfSrc.InfoMeisaiMast.TOTAL_IRAI_KIN
                            SrcTR(2) = cmtfSrc.InfoMeisaiMast.TOTAL_ZUMI_KEN
                            SrcTR(3) = cmtfSrc.InfoMeisaiMast.TOTAL_ZUMI_KIN
                            SrcTR(4) = cmtfSrc.InfoMeisaiMast.TOTAL_FUNO_KEN
                            SrcTR(5) = cmtfSrc.InfoMeisaiMast.TOTAL_FUNO_KIN


                            SrcEntry.Value = SrcTR
                            SrcQue.Add(SrcEntry)
                    End Select
                Loop


                'ヘッダ・トレーラレコードのみ比較する
                For i As Integer = 0 To SrcQue.Count - 1
                    SrcEntry = CType(SrcQue.Item(i), DictionaryEntry)

                    Select Case SrcEntry.Key.ToString
                        Case "T" ' トレーラレコード
                            SrcTR = CType(SrcEntry.Value, Decimal())

                            If SrcTR(0) <> SrcTR(2) + SrcTR(4) OrElse _
                               SrcTR(1) <> SrcTR(3) + SrcTR(5) Then
                                With GCom.GLog
                                    .Job2 = "CMTファイル比較"
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "トレーラレコードの同一性違反, レコード数：" & i.ToString
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                .setError(7, "同一ファイルではありません")
                                Return False
                            End If
                    End Select
                Next
            End With

        Catch ex As Exception
            cmtData.setError(9, "CMT読取エラー")
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT比較失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False

        Finally
            '最後にファイルを解放する
            cmtfSrc.Close()
        End Try
        Return True
    End Function
    '******************************************************

    ' 機能   ： サーバのパス名を返す
    '
    ' 引数    : ARG1 - Read(true)/Write(false)
    '         : ARG2 - 自振(True)/総振(False)フラグ
    '         : ARG3 - フォーマット区分(Integer)
    ' 戻り値  : ファイル名ではなくて実際にはパス名を返す
    Private Function MakeServerFileName(ByVal bRWFlag As Boolean, ByVal bJSFlag As Boolean, ByVal fkbn As Integer) As String
        Dim filename As String

        If bRWFlag Then
            filename = gstrCMTServerRead
        Else
            filename = gstrCMTServerWrite
        End If
        'If bJSFlag Then
        '    filename &= "JIFURI\"
        'Select Case fkbn
        '    Case 0 ' 全銀フォーマット
        '        filename &= "JFR120\"
        '    Case 1 ' 地方公共団体350byte
        '        filename &= "JFR350\"
        '    Case 2 ' 地方公共団体300byte
        '        filename &= "JFR300\"
        '    Case 3 ' 国税
        '        filename &= "JFR390\"
        '    Case 4 ' センター直接持込(請求データ)
        '        filename &= "JFR640\"
        '    Case 5 ' センター直接持込(結果データ)
        '        filename &= "JFR165\"
        '    Case Else
        '        With GCom.GLog
        '            .Result = MenteCommon.clsCommon.NG
        '            .Discription = "未対応のファイル区分番号 自振総振区分：" & bJSFlag.ToString & ", CMTフォーマット区分：" & fkbn.ToString
        '        End With
        '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        '        Return "ERR"
        'End Select
        'Else
        'filename &= "SOUKYUFURI\"
        'Select Case fkbn
        '    Case 0
        '        filename &= "SFR120\"
        '    Case 1
        '        filename &= "SFR350\"
        '    Case 2
        '        filename &= "SFR300\"
        '    Case 3
        '        filename &= "SFR390\"
        '    Case Else
        '        With GCom.GLog
        '            .Result = MenteCommon.clsCommon.NG
        '            .Discription = "未対応のファイル区分番号 自振総振区分：" & bJSFlag.ToString & ", CMTフォーマット区分：" & fkbn.ToString
        '        End With
        '        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        '        Return "ERR"
        'End Select
        'End If

        Return filename
    End Function


    ' 機能   ： 他行向けのサーバのパス名を返す
    '
    ' 引数    : ARG1 請求書込 = true / 結果読取 = false
    '
    ' 戻り値  : ファイル名ではなくて実際にはパス名を返す
    Private Function MakeOtherFileName(ByRef bRWflag As Boolean) As String
        Dim filename As String
        '*** 修正 mitsu 2008/09/01 不要 ***
        'Try
        '**********************************
        If bRWflag Then
            filename = gstrCMTServerWrite
        Else
            filename = gstrCMTServerRead
        End If

        ' 全銀のみ
        filename &= "TAKOU\"

        Return filename
    End Function


    ' 機能　 ： 受付番号の取得
    '
    ' 引数　 ： ARG1 - true → CMT_READ_TBLから読込
    '                  false → CMT_WRITE_TBLから読込
    '           ARG2   OracleDataReader(参照渡)
    '           ARG3   受付番号(参照渡) 
    ' 戻り値 ： 正常終了 = True
    ' 　　　 　 異常終了 = False
    '
    ' 備考　 ： 2007.11.14 追加
    Private Function GetReceptionNo(ByVal bRWFlag As Boolean, ByRef nReceptionNo As Integer) As Boolean
        Dim strSQL As String
        Dim strTbl As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim oraReader As OracleDataReader = Nothing
        'Dim oraReader As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        If bRWFlag Then
            strTbl = "CMT_READ_TBL"
        Else
            strTbl = "CMT_WRITE_TBL"
        End If

        ' 受付番号(nReceptionNo)を取得
        strSQL = "SELECT NVL(MAX(RECEPTION_NO),0)"
        strSQL &= " FROM " & strTbl
        strSQL &= " WHERE SYORI_DATE = '" & gstrSysDate & "'"
        strSQL &= " AND STATION_NO = '" & Me.gstrStationNo & "'"

        If GCom.SetDynaset(strSQL, oraReader) Then
            ' 受付番号は、テーブル中の最大値＋１からスタート
            oraReader.Read()
            nReceptionNo = oraReader.GetInt32(0) + 1
            Return True
        Else
            '*** 修正 mitsu 2008/09/01 エラーメッセージ表示 ***
            'With GCom.GLog
            '    .Result = strTbl & "でエラー発生"
            '    .Discription = "受付番号の取得失敗"
            'End With
            'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            MessageBox.Show("受付番号の取得に失敗しました" & vbCrLf & "テーブル名：" & strTbl, _
                GCom.GLog.Job2, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '**************************************************
            Return False
        End If
    End Function

    ' 機　能 : ファイル名をリネーム
    '
    ' 引き数 : ARG1 - 変更元ファイル名
    ' 　　　   ARG2 - 変更後ファイル名
    '
    ' 戻り値 : True = 成功 / False = 失敗
    '
    ' 備考   : 失敗した場合に各種ログを出力する
    Private Function RenameFileName(ByVal strSource As String, ByVal strDistination As String) As Boolean
        If Not (File.Exists(strSource)) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "変更元のファイル名が存在しないため、リネーム失敗 元ファイル名：" & strSource & ", 先ファイル名：" & strDistination
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        If File.Exists(strDistination) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "変更先のファイル名が既に存在しているため、リネーム失敗 元ファイル名：" & strSource & ", 先ファイル名：" & strDistination
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False

        End If
        Try
            FileSystem.Rename(strSource, strDistination)
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "リネーム失敗 元ファイル名：" & strSource & ", 先ファイル名：" & strDistination & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function


    ' 機　能 : ファイルを削除
    '
    ' 引き数 : ARG1 - 削除ファイル名
    '
    ' 戻り値 : True = 成功 / False = 失敗
    '
    Private Function DeleteFile(ByVal filename As String) As Boolean
        Const strResult As String = "ファイル削除失敗"
        Try
            File.Delete(filename)
        Catch ex As UnauthorizedAccessException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " 指定されたパスはディレクトリであるか、アクセス権限がありませんです。パス名：" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As ArgumentException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " ファイル名が空白です。ファイル名：" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As System.Security.SecurityException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " 削除を実行する権限がありません。ファイル名：" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As DirectoryNotFoundException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " 指定したディレクトリがみつかりません。 ディレクトリ名：" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As IOException
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " 指定したファイルが使用中です。 ファイル名：" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = strResult & " ファイル削除時に不明なエラーが発生しました。 ファイル名：" & filename & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        Return True
    End Function

    ' 機　能 : バイナリでファイルをコピー
    '
    ' 引き数 : ARG1 - 読込元ファイル名
    ' 　　　   ARG2 - 書込先ファイル名
    '          ARG3 - レコード長
    '
    ' 戻り値 : 0:成功, 1:ファイルなし, 2:書込み先が既に存在, 3:ファイル長が不正
    '
    ' 備考   : ファイルサイズが2G超となる場合も処理できるようにLong型で処理している
    Private Function BinaryCopy(ByVal ReadFileName As String, ByVal WriteFileName As String, ByVal nRecordLen As Integer) As Integer
        Dim bytesTemp(nRecordLen) As Byte
        Dim i As Long
        Dim len As Long
        Dim divr As Long
        Dim imax As Long

        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim rfs As FileStream = Nothing
        Dim wfs As FileStream = Nothing
        Dim rs As BinaryReader = Nothing
        Dim ws As BinaryWriter = Nothing
        'Dim rfs As FileStream
        'Dim wfs As FileStream
        'Dim rs As BinaryReader
        'Dim ws As BinaryWriter
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try
            Try
                rfs = New FileStream(ReadFileName, FileMode.Open)
                rs = New BinaryReader(rfs)
            Catch rext As FileNotFoundException
                ' ファイル名が存在しません
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "コピー元ファイルなし ファイル名：" & ReadFileName & " " & rext.Message
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return 1
            End Try

            Try
                wfs = New FileStream(WriteFileName, FileMode.CreateNew)
                ws = New BinaryWriter(wfs)
            Catch wex As IOException
                ' ファイル名が既に存在しています
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "書込み先のファイルが既に存在しています ファイル名：" & WriteFileName & " " & wex.Message
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                rs.Close()
                rfs.Close()
                Return 2
            End Try

            len = rfs.Length()
            imax = Math.DivRem(len, nRecordLen, divr)

            ' 指定したレコード長の整数倍の長さを書込
            For i = 1 To imax
                bytesTemp = rs.ReadBytes(nRecordLen)    ' 指定したレコード長分読込
                ws.Write(bytesTemp, 0, nRecordLen)      ' 指定したレコード長分書込
            Next i

            If (divr > 0) Then
                ' 余り分を書込み
                bytesTemp = rs.ReadBytes(CInt(divr))
                ws.Write(bytesTemp, 0, CInt(divr))
                rs.Close()
                ws.Close()
                rfs.Close()
                wfs.Close()
                Return 0 ' CRの有無があるため、余りは無視
            End If

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "バイナリファイルのコピー失敗 " & ReadFileName & "から" & WriteFileName & "へのコピー " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            rs.Close()
            ws.Close()
            rfs.Close()
            wfs.Close()
            Return 3
        End Try

        rs.Close()
        ws.Close()
        rfs.Close()
        wfs.Close()

        Return 0
    End Function


    ' 機能　 ： 機番設定
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： 正常終了 = True
    ' 　　　 　 異常終了 = False
    '
    ' 備考　 ： 2007.11.16 Insert By Astar
    '
    Public Function SetStationNo() As Boolean

        gstrStationNo = GCom.GetStationNo().Substring(1, 1)

        If (gstrStationNo = "NULL") Then
            With GCom.GLog
                .Job2 = "機番取得"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ""
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Else
            Return True
        End If

    End Function


    ' 機能　 ： CMT.INIから読取読取結果の取得
    '           読み取り元: CMT.INI
    '
    ' 引数　 ： ARG1 - スタッカ番号
    '
    ' 戻り値 ： ステータス(Integer)
    '
    ' 備考　 ： 2007.11.21 作成
    Protected Function GetCmtReadResult(ByVal nStackerNo As Integer) As Integer
        Dim str As String = ""
        If (nStackerNo < 1) Or (nStackerNo > 10) Then

            With GCom.GLog
                .Job2 = "CMT.INI読込"
                .Result = MenteCommon.clsCommon.NG
                .Discription = "不正なスタッカ番号 番号：" & nStackerNo.ToString
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return 4
        End If

        str = CMT.GetCMTIni("READ-RESULT", nStackerNo.ToString)
        If str = "err" Then
            Return 4
        Else
            Return Convert.ToInt32(str)
        End If
    End Function


    ' 機能　 ： CMT.INIからラベル有無の取得
    '           読み取り元: CMT.INI
    '
    ' 引数　 ： ARG1 - スタッカ番号
    '
    ' 戻り値 ： ステータス(Integer)
    '
    ' 備考　 ： 2007.11.21 作成
    Protected Function GetCmtLabelExist(ByVal nStackerNo As Integer) As Boolean
        Dim str As String = ""
        '        Dim cmt As CMT.ClsCMTCTRL

        If (nStackerNo < 0) Or (nStackerNo > 9) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT.INI読込 不正なスタッカ番号が指定されました 番号：" & nStackerNo.ToString
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If
        Try
            If CMT.GetCMTIni("LABEL-EXIST", (nStackerNo + 1).ToString) = "1" Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT.INI読込"
                .Result = MenteCommon.clsCommon.NG
                .Discription = "CMT.INI読込 項目読取エラー [LABEL-EXIST]、スタッカ番号：" & nStackerNo.ToString & " " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
    End Function


    ' 機　能 : ローカルPCのファイルの削除
    '
    ' 戻り値 : 成功 true / 失敗 false
    '
    ' 引き数 : ARG1 - 読込フォルダ削除 = True / 書込フォルダ削除 = False
    Protected Function LocalFileDelete(ByVal bRWflag As Boolean) As Boolean
        Dim nCnt As Integer
        Dim path As String
        Dim head As String
        Dim endlabel As String
        Dim filename As String

        Try
            If bRWflag Then
                filename = CMT.GetCMTIni("FILE-NAME", "READ")
            Else
                filename = CMT.GetCMTIni("FILE-NAME", "WRITE")
            End If
            head = CMT.GetCMTIni("FILE-NAME", "HEAD")
            endlabel = CMT.GetCMTIni("FILE-NAME", "END")
            For nCnt = 1 To 10 Step 1
                If (bRWflag) Then
                    path = CMT.GetCMTIni("READ-DIRECTORY", nCnt.ToString)
                Else
                    path = CMT.GetCMTIni("WRITE-DIRECTORY", nCnt.ToString)
                End If

                ' ファイル削除
                If File.Exists(path & "\" & filename) = True Then
                    File.Delete(path & "\" & filename)
                End If
                ' ヘッダラベル削除
                If File.Exists(path & "\" & head) = True Then
                    File.Delete(path & "\" & head)
                End If
                ' エンドラベル削除
                If File.Exists(path & "\" & endlabel) = True Then
                    File.Delete(path & "\" & endlabel)
                End If
            Next nCnt
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ローカルファイル削除失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function

    '***ASTAR SUSUKI 2008.06.12                     ***
    '***追加
    ' 機　能 : ローカルPCのヘッダファイルをコピー Read から Write へ
    '
    ' 戻り値 : 成功 true / 失敗 false
    '
    ' 引き数 : ARG1 - 読込フォルダ削除 = True / 書込フォルダ削除 = False
    Protected Function LocalHeadCopy(ByVal lot As Integer) As Boolean
        Dim nCnt As Integer
        Dim ReadPath As String
        Dim WritePath As String
        Dim head As String
        Dim endlabel As String

        Try
            head = CMT.GetCMTIni("FILE-NAME", "HEAD")
            endlabel = CMT.GetCMTIni("FILE-NAME", "END")
            nCnt = lot
            ReadPath = CMT.GetCMTIni("READ-DIRECTORY", nCnt.ToString)
            WritePath = CMT.GetCMTIni("WRITE-DIRECTORY", nCnt.ToString)

            ' ヘッダラベルコピー
            If File.Exists(ReadPath & "\" & head) = True Then
                If File.Exists(WritePath & "\" & head) = False Then
                    File.Copy(ReadPath & "\" & head, WritePath & "\" & head)
                End If
            End If
            ' エンドラベルコピー
            If File.Exists(ReadPath & "\" & endlabel) = True Then
                If File.Exists(WritePath & "\" & endlabel) = False Then
                    File.Copy(ReadPath & "\" & endlabel, WritePath & "\" & endlabel)
                End If
            End If

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ヘッダファイルコピー失敗 スタッカ番号：" & lot & " " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function
    '***

    ' 機　能 : 実績テーブルからListView生成
    '
    ' 引  数 : ARG1 - 照会結果を格納するテーブル(CMT_READ_TBL = True / CMT_WRITE_TBL = False)
    '          ARG2 - テーブル選択フラグ 読込実績=True/書込実績=False
    '          ARG3 - 自振=True/総給振=False フラグ
    '          ARG4 - 振替日
    '          ARG5 - 委託者コード
    '
    ' 戻り値 : 成功 true / 失敗 false
    '
    Public Function getRWList(ByRef listview As ListView, ByVal bRWFlg As Boolean, ByVal bJSFlg As Boolean, ByVal strFuriDate As String _
         , ByVal strItakuCD As String, ByVal bJifuri As Boolean, ByVal bsoukyufuri As Boolean) As Boolean
        Dim count As Integer
        Dim SQL As String
        Dim strTableName As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        If bRWFlg Then
            strTableName = "CMT_READ_TBL"
        Else
            strTableName = "CMT_WRITE_TBL"
        End If

        Try
            ' 行数カウント
            SQL = "SELECT COUNT(*)"
            SQL &= " FROM " & strTableName ' テーブル名
            If strFuriDate <> "" Then
                '*** 2008.06.10修正 nishida 検索パターン変更　振替日->処理日 START***
                SQL &= " WHERE SYORI_DATE = '" & strFuriDate & "'"
                'SQL &= " WHERE FURI_DATE = '" & strFuriDate & "'"
                '*** 2008.06.10修正 nishida 検索パターン変更　振替日->処理日 END***
                If strItakuCD <> "" Then
                    SQL &= " AND ITAKU_CODE = '" & strItakuCD & "'"
                End If
            Else
                If strItakuCD <> "" Then
                    SQL &= " WHERE ITAKU_CODE = '" & strItakuCD & "'"
                End If
            End If

            If GCom.SetDynaset(SQL, onReader) Then
                onReader.Read()
                count = onReader.GetInt32(0)
            End If

            If count = 0 Then
                ' 表示項目無し
                Return True
            End If

            ' SQL文生成
            SQL = "SELECT "
            SQL &= "   NVL(ITAKU_CODE, '0000000000')"           ' 00.委託者コード
            SQL &= " , NVL(FURI_DATE,'00000000')"               ' 01.振替日
            SQL &= " , NVL(ERR_CD, 0)           "               ' 02.エラーコード
            SQL &= " , NVL(ERR_INFO,' -- ')     "               ' 03.エラー名
            SQL &= " , RECEPTION_NO"                            ' 04.受付No
            SQL &= " , FILE_SEQ"                                ' 05.FILE_SEQ
            SQL &= " , NVL(SYORI_DATE, ' -- ')"                 ' 06.CMT処理日
            SQL &= " , NVL(STACKER_NO, 0)"                      ' 07.スタッカ番号
            SQL &= " , NVL(STATION_NO, '-')"                    ' 08.CMT読込機番
            SQL &= " , NVL(SYORI_KEN, 0)"                       ' 09.件数
            SQL &= " , NVL(SYORI_KIN, 0)"                       ' 10.金額
            SQL &= " , NVL(FURI_KEN, 0)"                        ' 11.振替済件数
            SQL &= " , NVL(FURI_KIN, 0)"                        ' 12.振替済金額
            SQL &= " , NVL(FUNOU_KEN, 0)"                       ' 13.不能件数
            SQL &= " , NVL(FUNOU_KIN, 0)"                       ' 14.不能金額
            SQL &= " , NVL(FILE_NAME, ' -- ')"                  ' 15.ファイル名
            If Not bRWFlg Then
                SQL &= " , NVL(WRITE_COUNTER, 0)"               ' 16a.ファイル書込回数
            Else
                SQL &= " , NVL(JS_FLG,'-')"                     ' 16b.自振/総振フラグ
            End If
            SQL &= " , NVL(COMPLOCK_FLG,'-')"                   ' 17.暗号化フラグ
            SQL &= " , CREATE_DATE"                             ' 18.生成日
            SQL &= " , UPDATE_DATE"                             ' 19.更新日
            If Not bRWFlg Then
                SQL &= " , OVERRIDE_FLG"                        ' 20.強制書込フラグ
            End If

            SQL &= " FROM " & strTableName                      ' テーブル名

            If strFuriDate <> "" Then
                '*** 2008.06.10修正 nishida 検索パターン変更　振替日->処理日 START***
                SQL &= " WHERE SYORI_DATE = '" & strFuriDate & "'"
                'SQL &= " WHERE FURI_DATE = '" & strFuriDate & "'"
                '*** 2008.06.10修正 nishida 検索パターン変更　振替日->処理日 END***
                If strItakuCD <> "" Then
                    SQL &= " AND ITAKU_CODE = '" & strItakuCD & "'"
                End If
                If Not bJifuri Then
                    SQL &= " AND JS_FLG = '0'"
                Else
                    If Not bsoukyufuri Then
                        SQL &= " AND JS_FLG = '1'"
                    End If
                End If
            Else
                If strItakuCD <> "" Then
                    SQL &= " WHERE ITAKU_CODE = '" & strItakuCD & "'"
                    If Not bJifuri Then
                        SQL &= " AND JS_FLG = '0'"
                    Else
                        If Not bsoukyufuri Then
                            SQL &= " AND JS_FLG = '1'"
                        End If
                    End If
                Else
                    If Not bJifuri Then
                        SQL &= " WHERE JS_FLG = '0'"
                    Else
                        If Not bsoukyufuri Then
                            SQL &= " WHERE JS_FLG = '1'"
                        End If
                    End If
                End If
            End If
            SQL &= " ORDER BY RECEPTION_NO"

            If GCom.SetDynaset(SQL, onReader) Then
                For i As Integer = 0 To count - 1
                    onReader.Read()
                    Ora2List(onReader, listview, bRWFlg)
                Next i
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "実績テーブルからListView生成失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
    End Function

    ' 機　能 : エラーのあるテーブルのリストを返す
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 対象ListView
    ' 備　考 : 
    '    
    Public Function getErrorList(ByRef listview As ListView) As Boolean
        Dim count As Integer
        Dim SQL As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try
            ' 行数カウント
            SQL = "SELECT COUNT(*) "
            SQL &= " FROM"
            SQL &= " ( SELECT " _
                 & "   ITAKU_CODE" _
                 & ", FURI_DATE" _
                 & ", MIN(ERR_CD) ERROR" _
                 & " FROM CMT_READ_TBL" _
                 & " GROUP BY ITAKU_CODE, FURI_DATE)"
            SQL &= " WHERE ERROR > 0"

            If GCom.SetDynaset(SQL, onReader) Then
                onReader.Read()
                count = onReader.GetInt32(0)
            End If

            If count = 0 Then
                ' 表示項目無し
                Return True
            End If

            ' テーブルの内容をコピー
            SQL = "SELECT * "
            SQL &= "FROM ("
            SQL &= " SELECT"
            SQL &= "   NVL(ITAKU_CODE,'0000000000')" ' 0.委託者コード 
            SQL &= ",  NVL(FURI_DATE,'00000000')"    ' 1.振替日
            SQL &= ",  MIN(ERR_CD) ERROR"    ' 2.エラーコード
            SQL &= " , NVL(ERR_INFO, ' -- ')" '3.エラー名
            SQL &= " , RECEPTION_NO"         ' 4.受付No
            SQL &= " , FILE_SEQ"             ' 5.FILE_SEQ
            SQL &= " , SYORI_DATE"           ' 6.CMT処理日
            SQL &= " , NVL(STACKER_NO, 0)"   ' 7.スタッカ番号
            SQL &= " , NVL(STATION_NO, '-')" ' 8.CMT読込機番
            SQL &= " , NVL(SYORI_KEN, 0)"    ' 9.件数
            SQL &= " , NVL(SYORI_KIN, 0)"    ' 10.金額
            SQL &= " , NVL(FURI_KEN, 0)"     ' 11.振替済件数
            SQL &= " , NVL(FURI_KIN, 0)"     ' 12.振替済金額
            SQL &= " , NVL(FUNOU_KEN, 0)"    ' 13.不能件数
            SQL &= " , NVL(FUNOU_KIN, 0)"    ' 14.不能金額
            SQL &= " , NVL(FILE_NAME, ' -- ')" ' 15.ファイル名
            SQL &= " , NVL(JS_FLG, '-')"     ' 16.自振/総振フラグ
            SQL &= " , NVL(COMPLOCK_FLG, '-')" ' 17.暗号化フラグ
            SQL &= " , CREATE_DATE"          ' 18.生成日
            SQL &= " , UPDATE_DATE"          ' 19.更新日
            SQL &= " FROM CMT_READ_TBL"
            SQL &= " GROUP BY"
            SQL &= "  ITAKU_CODE"            ' 0.委託者コード 
            SQL &= ",  FURI_DATE"            ' 1.振替日
            ' 2.エラーコード
            SQL &= " , RECEPTION_NO"         ' 4.受付No
            SQL &= " , FILE_SEQ"             ' 5.FILE_SEQ
            SQL &= " , SYORI_DATE"           ' 6.CMT処理日
            SQL &= " , STACKER_NO"           ' 7.スタッカ番号
            SQL &= " , STATION_NO"           ' 8.CMT読込機番
            SQL &= " , SYORI_KEN"            ' 9.件数
            SQL &= " , SYORI_KIN"            ' 10.金額
            SQL &= " , FURI_KEN"             ' 11.振替済件数
            SQL &= " , FURI_KIN"             ' 12.振替済金額
            SQL &= " , FUNOU_KEN"            ' 13.不能件数
            SQL &= " , FUNOU_KIN"            ' 14.不能金額
            SQL &= " , FILE_NAME"            ' 15.ファイル名
            SQL &= " , ERR_INFO"             ' 3.エラー名
            SQL &= " , JS_FLG"               ' 16.自振/総振フラグ
            SQL &= " , COMPLOCK_FLG"         ' 17.暗号化フラグ
            SQL &= " , CREATE_DATE"          ' 18.生成日
            SQL &= " , UPDATE_DATE)"         ' 19.更新日
            SQL &= " WHERE ERROR > 0"        ' エラーコードが0以外であるとき
            If GCom.SetDynaset(SQL, onReader) Then
                
                For i As Integer = 0 To count - 1
                    onReader.Read()
                    Ora2List(onReader, listview, True)
                Next i
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ListView生成失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try

        Return True
    End Function

    ' 機　能 : OracleDataReaderからListViewを生成
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - OracleReader
    '        : ARG2 - 対象ListView
    '        : ARG3 - RWフラグ (True = CMT読込実績TBL / False = CMT書込実績TBL)
    ' 備　考 : 
    '    
    Public Function Ora2List(ByRef onReader As OracleDataReader, ByRef listview As ListView, ByVal bRWFlg As Boolean) As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim fkbn As String = String.Empty
        Dim toris As String = String.Empty
        Dim torif As String = String.Empty
        'Dim fkbn, toris, torif As String ' F区分, 取引先主コード, 取引先副コード格納用変数
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        DB.GetFToriCode(onReader.GetString(0), "", toris, torif, fkbn, 0)

        Dim lv As New ListViewItem(onReader.GetInt32(4).ToString.PadLeft(5, "0"c))  ' 0.受付No
        If (onReader.GetInt32(2) > 0) Then
            lv.BackColor = Color.Pink
        Else
            lv.BackColor = Color.White
        End If
        lv.SubItems.Add(onReader.GetInt32(5).ToString.PadLeft(2, "0"c))             ' 1.FILE_SEQ
        lv.SubItems.Add(onReader.GetString(6))                      ' 2.処理日
        lv.SubItems.Add(onReader.GetInt32(7).ToString.PadLeft(2, "0"c))            ' 3.スタッカ番号
        If Not bRWFlg Then
            lv.SubItems.Add(onReader.GetInt32(16).ToString)         ' 4.ファイル書込回数
        Else
            lv.SubItems.Add("--")                                   ' 4.ファイル書込回数
        End If
        lv.SubItems.Add(onReader.GetString(0))                      ' 5.委託者コード(FILE_SEQ毎)
        lv.SubItems.Add(onReader.GetString(1))                      ' 6.振替日(FILE_SEQ毎)
        lv.SubItems.Add(DB.GetItakuKana(onReader.GetString(0)))     ' 7.委託者カナ
        lv.SubItems.Add(DB.GetItakuKanji(onReader.GetString(0)))    ' 8.委託者漢字
        lv.SubItems.Add(onReader.GetInt32(9).ToString)              ' 9.件数
        lv.SubItems.Add(onReader.GetInt32(10).ToString)             ' 10.金額
        lv.SubItems.Add(" -- ")                                     ' 11.TR件数
        lv.SubItems.Add(" -- ")                                     ' 12.TR金額
        lv.SubItems.Add(onReader.GetInt32(2).ToString)              ' 13.エラーコード
        lv.SubItems.Add(onReader.GetString(3))                      ' 14.エラー名
        lv.SubItems.Add(" -- ")                                     ' 15.金融機関コード
        lv.SubItems.Add(" -- ")                                     ' 16.金融機関名
        lv.SubItems.Add(" -- ")                                     ' 17.支店コード
        lv.SubItems.Add(" -- ")                                     ' 18.支店名
        If bRWFlg Then
            If (onReader.GetString(16) = "1") Then                  ' 19.自振/総振フラグ
                lv.SubItems.Add("自")
            Else
                lv.SubItems.Add("総")
            End If
        Else
            lv.SubItems.Add("自")
        End If
        lv.SubItems.Add(onReader.GetInt32(11).ToString)             ' 20.振替済件数
        lv.SubItems.Add(onReader.GetInt32(12).ToString)             ' 21.振替済金額
        lv.SubItems.Add(onReader.GetInt32(13).ToString)             ' 22.不能件数
        lv.SubItems.Add(onReader.GetInt32(14).ToString)             ' 23.不能金額
        lv.SubItems.Add(fkbn)                                       ' 24.F処理区分
        lv.SubItems.Add(toris)                                      ' 25.取引先主コード
        lv.SubItems.Add(torif)                                      ' 26.取引先副コード
        lv.SubItems.Add(onReader.GetString(8))                      ' 27.CMT読取機番
        lv.SubItems.Add(onReader.GetString(15))                     ' 28.ファイル名
        If Not bRWFlg Then
            If (onReader.GetString(20) = "1") Then                  ' 29.強制書込フラグフラグ
                lv.SubItems.Add("強制")                             '  - 強制
            Else
                lv.SubItems.Add("通常")                              ' - 通常
            End If
        Else
            lv.SubItems.Add(" - ")                                  '　読込なので関係なし
        End If
        If onReader.GetString(17) = "1" Then                        ' 30.暗号化有無
            lv.SubItems.Add("C")                                    '  - 暗号化あり
        Else
            lv.SubItems.Add("N")                                    '  - 暗号化なし
        End If
        lv.SubItems.Add(onReader.GetOracleDateTime(18).ToString)    ' 31.生成日
        lv.SubItems.Add(onReader.GetOracleDateTime(19).ToString)    ' 32.更新日

        listview.Items.AddRange(New ListViewItem() {lv})
    End Function

    ' 機　能 : サーバに残存する返却ファイルの一覧を返す
    '          
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 対象ListView
    ' 備　考 : 
    '
    Public Function getServerExistFiles(ByRef listview As ListView) As Boolean
        Dim serverpath As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim cmtFormat As CAstFormat.CFormat = Nothing
        'Dim cmtFormat As CAstFormat.CFormat
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim cmtd As clsCmtData
        Dim compflag As Boolean
        Dim itakucd As String

        GCom.GLog.Job2 = "CMT書込未処理分検索"

        Try
            For nFormatKbn As Integer = 0 To 4 Step 1
                serverpath = MakeServerFileName(False, True, nFormatKbn)
                GetFormat(nFormatKbn, cmtFormat)

                If Not Directory.Exists(serverpath) Then ' フォルダが存在するかどうか検索
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "サーバ返却フォルダがみつかりません フォルダ名：" & serverpath
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If

                ' フォルダが存在する場合
                Dim di As New DirectoryInfo(serverpath) ' サーバパスのディレクトリ情報を生成
                Dim fi As FileInfo() = di.GetFiles()    ' ファイル情報の配列を生成

                Dim fiTemp As FileInfo
                Dim i As Integer = 0

                Dim wcounter As Integer = 0
                For Each fiTemp In fi
                    ' 返却ファイルをパースしてListViewを生成

                    ' ファイル名のチェック
                    If fiTemp.Name.Length = 23 AndAlso fiTemp.Name.Substring(19, 4) = ".dat" Then
                        itakucd = fiTemp.Name.Substring(1, 10)
                        Dim furidate As String = fiTemp.Name.Substring(11, 8)
                        If (fiTemp.Name.Substring(0, 1) = "C") Then
                            compflag = True
                        Else
                            compflag = False
                        End If

                        If (GCom.NzLong(itakucd, 10) >= 0L) And _
                            (GCom.NzLong(itakucd, 10) <= 9999999999L) And _
                            (GCom.NzLong(furidate, 8) >= 19000000L) And _
                            (GCom.NzLong(furidate, 8) <= 99999999L) Then

                            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
                            Dim fkbn As String = String.Empty
                            Dim toris As String = String.Empty
                            Dim torif As String = String.Empty
                            'Dim fkbn, toris, torif As String ' F処理区分,取引先主・副コード
                            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
                            If DB.GetFToriCode(itakucd, "", toris, torif, fkbn, nFormatKbn) Then
                                ' 取引先主・副コード検索成功

                                cmtd = New clsCmtData
                                cmtd.Init(0, 1, False, False)

                                DB.GetFormatKbn(itakucd, nFormatKbn)

                                If (compflag) Then
                                    ' Complockで暗号化されている場合の処理
                                    cmtd.ComplockInit(itakucd, furidate)
                                Else
                                    ' 平文の処理
                                    If (cmtFormat.FirstRead(serverpath & fiTemp.Name) = 1) Then
                                        Me.ParseCMTFormat(nFormatKbn, cmtFormat, cmtd, False)
                                    Else
                                        cmtd.InitEmptyItakuData(False)
                                    End If

                                    '*** 修正 mitsu 2008/09/08 ファイルを解放する ***
                                    Try : cmtFormat.Close() : Finally : End Try
                                    '************************************************
                                End If

                                If cmtd.NotError Then
                                    DB.GetWriteCounter(itakucd, furidate, wcounter)
                                    cmtd.WriteCounter = wcounter
                                End If

                                With cmtd
                                    ' ファイル破損のエラーも含めて全部表示
                                    For j As Integer = 0 To .ItakuCounter() Step 1
                                        Dim lv As New ListViewItem((i + 1).ToString.PadLeft(5, "0"c))
                                        If Not .NotError() Then
                                            lv.BackColor = Color.LightPink
                                        Else
                                            lv.BackColor = Color.White
                                        End If
                                        .getListViewItem(lv, j)
                                        listview.Items.AddRange(New ListViewItem() {lv})
                                    Next j
                                End With
                                cmtd.Dispose()
                                i += 1
                            Else
                                ' サーバ上のファイル名が取引先マスタに未登録
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "サーバ上のフォルダに取引先マスタ未登録のデータが見つかりました ファイル名：" & serverpath
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                            End If
                        Else
                            ' サーバ上のファイル名が取引先マスタに未登録
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = "サーバ上のフォルダに取引先マスタ未登録のデータが見つかりました フォルダ名：" & serverpath
                            End With
                            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                        End If
                    Else
                        ' サーバ上のファイル名が取引先マスタに未登録
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = "サーバ上に有効でないファイル名のデータが見つかりました フォルダ名：" & serverpath
                        End With
                        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    End If
                Next fiTemp
            Next nFormatKbn
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "サーバ返却ファイル検索失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

        Return True
    End Function

    ' 機　能 : ListviewからCSVへの変換
    '
    ' 戻り値 : 作成成功 true / 失敗 false
    '
    ' 引き数 : ARG1 - 対象ListView
    '        : ARG2 - 生成したCSVファイル
    ' 備　考 :
    '
    Public Function MakeCsvFile(ByVal listview As ListView, ByRef strCSVpath As String, ByRef strCSVfile As String, ByVal strTitle As String) As Boolean
        Dim sw As StreamWriter
        Dim onString As String

        ' ListViewに印刷対象の項目があるかどうかチェック
        If listview.Items.Count = 0 Then
            MessageBox.Show("印刷対象のデータがありません.", "印刷機能")
            Return False
        End If

        Try
            strCSVpath = GCom.GetPRTFolder
            If strCSVpath = "err" Then
                MessageBox.Show("設定ファイルに問題があります。システム管理者にご連絡ください", "印刷エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "fskj.iniファイルの[COMMON]のPRT項目が設定されていない可能性があります。"
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
            strCSVfile = "CMT変換_" & String.Format("{0:yyyyMMddhhmmss}", Date.Now) & ".csv"
            sw = New StreamWriter(strCSVpath & strCSVfile, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            onString = listview.Columns(0).Text.ToString
            For Col As Integer = 1 To listview.Columns.Count - 1 Step 1
                onString &= "," & listview.Columns(Col).Text
            Next Col
            onString &= ",タイトル"
            sw.WriteLine(onString)

            For row As Integer = 0 To listview.Items.Count - 1
                onString = listview.Items.Item(0).Text.ToString
                For Col As Integer = 1 To listview.Items(row).SubItems.Count - 1 Step 1
                    onString &= "," & listview.Items.Item(row).SubItems(Col).Text.ToString
                Next Col
                onString &= "," & strTitle
                sw.WriteLine(onString)
            Next row
            sw.Close()
            sw = Nothing

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "印刷用CSVファイル作成失敗 " & ex.Message
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        Finally
            sw = Nothing
        End Try
        Return True
    End Function

    ' 機　能 : Listviewの印刷
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 対象ListView
    '
    ' 備　考 : 
    '
    Public Function PrintButton(ByVal lv As ListView, ByVal title As String) As Boolean
        GCom.GLog.Job2 = "印刷ボタン"
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim path As String = String.Empty
        Dim csvname As String = String.Empty
        'Dim path As String
        'Dim csvname As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try
            If Not CmtCom.MakeCsvFile(lv, path, csvname, title) Then   ' 帳票用CSVを生成
                Return False
            End If

            Dim repoAgent As New CAstReports.RAX
            repoAgent.ReportName = "CMT変換結果一覧.rpd"                                ' レポート定義名を指定
            repoAgent.CsvPath = path
            repoAgent.CsvName() = csvname
            If repoAgent.PrintOut() = False Then
                Dim MSG As String
                MSG = String.Format("{0}{2}{1}", "印刷できませんでした", repoAgent.Message, Environment.NewLine)
                MessageBox.Show(MSG, "印刷エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "印刷CSVファイル名：" & repoAgent.CsvPath & ", レポエージェントメッセージ：" & repoAgent.CsvName & " " & repoAgent.Message
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "印刷用CSVファイル作成失敗 " & ex.StackTrace
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
        Return True
    End Function

    ' 機　能 : 他行向け請求データのCMTへの書込
    '
    ' 戻り値 : 成功 true / 失敗 false
    '
    ' 引き数 : なし
    '
    ' 備　考 : 
    '
    Public Function WriteOtherCMT() As Boolean
        Dim serverpath As String
        Dim bankcd As String                                ' 金融機関コード
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim bankname As String = String.Empty               ' 金融機関名
        'Dim bankname As String                              ' 金融機関名
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim cmtctrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL

        '***ASTAR 2008.08.07 ブロックサイズ対応 >>
        'ブロックサイズ１８００
        cmtctrl.BlockSize = 1800
        '***ASTAR 2008.08.07 ブロックサイズ対応 <<

        GCom.GLog.Job2 = "他行向請求データCMT書込"

        serverpath = MakeOtherFileName(True)    ' サーバの返却ファイルの探索pathを生成
        If Not Directory.Exists(serverpath) Then ' フォルダが存在するかどうか検索
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "サーバ返却フォルダがみつかりません フォルダ名：" & serverpath
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        ' フォルダが存在する場合
        Dim di As New DirectoryInfo(serverpath) ' サーバパスのディレクトリ情報を生成
        Dim fi As FileInfo() = di.GetFiles()    ' ファイル情報の配列を生成
        Dim fiTemp As FileInfo

        '*** 修正 mitsu 2008/09/01 処理件数0考慮 ***
        If fi.Length = 0 Then
            MessageBox.Show("書込対象ファイルはありません", "他行向請求データCMT書込", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True
        End If
        '*******************************************

        '*** 修正 mitsu 2008/09/01 CMT接続確認 ***
        If Not Me.CheckConnectCMT() Then ' CMTが接続されていない場合は終了
            Return False
        End If
        '*****************************************

        For Each fiTemp In fi
            ' ローカルファイルの削除
            If (Not Me.LocalFileDelete(True)) Or (Not Me.LocalFileDelete(False)) Then
                '*** 修正 mitsu 2008/09/01 エラーメッセージ表示 ***
                'With GCom.GLog
                '    .Result = "エラー"
                '    .Discription = "ローカルPCのフォルダ初期化失敗"
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("CMT書込機の一時ファイル用フォルダの消去に失敗しました。", GCom.GLog.Job2 _
                    , MessageBoxButtons.OK, MessageBoxIcon.Error)
                '**************************************************
                Return False
            End If

            ' ファイル名のチェック
            If fiTemp.Name.Length > 3 AndAlso _
                (GCom.NzInt(fiTemp.Name.Substring(0, 4)) > 0 And GCom.NzInt(fiTemp.Name.Substring(0, 4)) < 9999) Then
                bankcd = fiTemp.Name.Substring(0, 4) ' 金融機関コードの切り出し
                If Not DB.GetBankName(bankcd, bankname) Then ' 金融機関マスタに存在チェック
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "金融機関コード存在なし 金融機関コード：" & bankcd
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                ElseIf MessageBox.Show("金融機関コード：" & bankcd & ", 金融機関名：" & bankname.TrimEnd & _
                    " の請求データをCMTに書き込みますか?", "他行向請求データCMT書込", MessageBoxButtons.YesNo, MessageBoxIcon.Question) _
                        = DialogResult.No Then
                    ' 書き込まない場合は何も処理を行わない
                ElseIf Not Me.IsEmptyCmt() AndAlso MessageBox.Show("CMTが空ではありません。上書きしますか？", "他行向請求データCMT書込", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then ' 書込実行
                    ' CMTが空ではない場合
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMTがフォーマット済みではないため中断"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    cmtctrl.UnloadCmt()
                    Exit For ' ループを抜ける
                ElseIf Me.BinaryCopy(serverpath & fiTemp.Name, gstrCMTWriteFileName(0), 120) = 0 Then
                    ' 処理成功
                    '*** ASTAR 2008.08.07 書込に失敗した場合にメッセージを表示する >>
                    'Me.DeleteFile(serverpath & fiTemp.Name)
                    'cmtctrl.WriteCmt(1)
                    If cmtctrl.WriteCmt(1) = False Then
                        Call MessageBox.Show("金融機関コード:" & bankcd & ", 金融機関名:" & bankname & Environment.NewLine & Environment.NewLine & _
                    "書込に失敗しました。", "他行向請求データCMT書込", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return False
                    Else
                        If MessageBox.Show(MSG0061I, "他行向請求データCMT書込", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                            cmtctrl.UnloadCmt()
                            MessageBox.Show(MSG0070I, "他行向請求データCMT書込", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                            File.Delete(gstrCMTWriteFileName(0))
                            File.Delete(gstrCMTReadFileName(0))
                            If Not Me.IsEmptyCmt() AndAlso MessageBox.Show("CMTが空ではありません。上書きしますか？", "他行向請求データCMT書込", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then ' 書込実行
                                ' CMTが空ではない場合
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = "CMTがフォーマット済みではないため中断"
                                End With
                                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                                cmtctrl.UnloadCmt()
                                Exit For ' ループを抜ける
                            ElseIf Me.BinaryCopy(serverpath & fiTemp.Name, gstrCMTWriteFileName(0), 120) = 0 Then
                                If cmtctrl.WriteCmt(1) = False Then
                                    Call MessageBox.Show("金融機関コード:" & bankcd & ", 金融機関名:" & bankname & Environment.NewLine & Environment.NewLine & _
                                                         "書込に失敗しました。", "他行向請求データCMT書込", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    Return False
                                End If
                            End If
                        End If
                        Me.DeleteFile(serverpath & fiTemp.Name)
                        End If
                        '*** ASTAR 2008.08.07 書込に失敗した場合にメッセージを表示する <<
                        cmtctrl.UnloadCmt()
                End If
            End If
        Next fiTemp
        MessageBox.Show("処理完了", "他行向請求データCMT書込", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Return True
    End Function

    ' 機　能 : 挿入されているCMTがフォーマット済みかどうかチェック
    '
    ' 戻り値 : 成功 true / 失敗 false
    '
    ' 引き数 : なし
    '
    ' 備　考 : 
    '
    Protected Function IsEmptyCmt() As Boolean
        Dim cmtctrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL

        Try
            ' CMT手差しの検出
            If (cmtctrl.SelectCmt(1)) Then ' 1本目が存在するかチェック
                If Not Me.IsExistStacker(cmtctrl, False) Then ' 2本以上存在しないかをチェック
                    ' CMTが1本だけの場合
                    'MessageBox.Show("一本差し検出")
                Else
                    ' 2本以上ある場合も異常とみなす
                    MessageBox.Show("CMTが2本以上挿入されています。一本ずつしか処理できません", "他行向請求データCMT書込み", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    cmtctrl.UnloadCmt()
                    cmtctrl.SelectCmt(22) ' Eject
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "CMT本数エラー 複数本のCMTが挿入済み"
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If
            Else
                MessageBox.Show("CMTが挿入されていません。", "他行向請求データCMT書込み", MessageBoxButtons.OK, MessageBoxIcon.Information)
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "CMT未挿入エラー"
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            End If

            cmtctrl.ReadCmt(1) ' CMTファイル読込

            If File.Exists(gstrCMTReadFileName(0)) Then
                ' CMTが空でないとき
                'cmtctrl.UnloadCmt()
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "CMTがフォーマット済みではありませんでした"
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function

    ' 機　能 : 他行向け結果データのCMTからの読取
    '
    ' 戻り値 : 成功 true / 失敗 false
    '
    ' 引き数 : なし
    '
    ' 備　考 : 
    '
    Public Function ReadOtherCMT() As Boolean
        Dim cmtctrl As CMT.ClsCMTCTRL = New CMT.ClsCMTCTRL
        Dim filename As String
        Dim serverpath As String
        Const msgtitle As String = "他行分結果データ読取"   '*** 2008.05.29 他行向け→他行分

        Try
            ' ローカルファイルの削除
            If (Not Me.LocalFileDelete(True)) Or (Not Me.LocalFileDelete(False)) Then
                '*** 修正 mitsu 2008/09/01 エラーメッセージ表示 ***
                'With GCom.GLog
                '    .Result = "エラー"
                '    .Discription = "ローカルPCのフォルダ初期化失敗"
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("CMT読取機の一時ファイル用フォルダの消去に失敗しました。", GCom.GLog.Job2 _
                    , MessageBoxButtons.OK, MessageBoxIcon.Error)
                '**************************************************
                Return False
            End If

            serverpath = MakeOtherFileName(False)
            If Not Directory.Exists(serverpath) Then ' フォルダが存在するかどうか検索
                '*** 修正 mitsu 2008/09/01 エラーメッセージ表示 ***
                'With GCom.GLog
                '    .Result = "サーバアップロードフォルダがみつかりません"
                '    .Discription = "フォルダ名:" & serverpath
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("サーバアップロードフォルダがみつかりません" & vbCrLf & _
                                 "フォルダ名：" & serverpath, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '**********************************
                Return False
            End If

            '*** 修正 mitsu 2008/09/01 CMT接続確認 ***
            If Not Me.CheckConnectCMT() Then ' CMTが接続されていない場合は終了
                Return False
            End If
            '*****************************************

            If (Me.IsExistStacker(cmtctrl, False)) Then
                ' スタッカには非対応
                MessageBox.Show("スタッカには対応していません", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                With GCom.GLog
                    .Job2 = msgtitle
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "スタッカが存在していたためエラーとして処理"
                End With
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                cmtctrl.UnloadCmt()
                cmtctrl.SelectCmt(22) ' Eject
                Return False
            End If

            filename = serverpath & "0005" ' 三菱東京UFJ銀行決め打ち
            If Me.ReadStacker(11, 1) Then ' CMTファイルの読込
                ' 読込成功
                '*** ASTAR 2008.05.29 金融機関決めうちをはずす     手差しのみ可能（仕様） ***
                Dim MSG As String
                Dim FMT As New CAstFormat.CFormatZengin
                If FMT.FirstRead(gstrCMTReadFileName(0)) = 1 Then
                    Call FMT.CheckDataFormat()
                    Call FMT.CheckRecord1()
                    filename = serverpath & FMT.InfoMeisaiMast.ITAKU_KIN
                    MSG = "金融機関コード ： " & FMT.InfoMeisaiMast.ITAKU_KIN
                    MSG &= Environment.NewLine & Environment.NewLine
                    MSG &= "よろしいですか？"
                    If MessageBox.Show(MSG, msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
                        FMT.Close()
                        Return False
                    End If
                End If
                FMT.Close()
                FMT = Nothing
                Dim nRet As Integer
                nRet = Me.BinaryCopy(gstrCMTReadFileName(0), filename, 120)
                '**********************************************************
                If nRet = 0 Then
                    MessageBox.Show(filename & "へのアップロード処理完了", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    cmtctrl.UnloadCmt()
                    Return True
                    '*** ASTAR 2008.05.29 エラー処理追加                ***
                ElseIf nRet = 2 Then
                    MessageBox.Show("ファイルが既に存在します。 処理を中断します。" & Environment.NewLine & Environment.NewLine & filename, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                    '**********************************************************
                Else
                    MessageBox.Show("サーバアップロード失敗", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    cmtctrl.UnloadCmt()
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "サーバアップロード失敗 ファイル名：" & filename
                    End With
                    GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return False
                End If
            Else
                MessageBox.Show("CMT読込失敗", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                cmtctrl.UnloadCmt()
                '*** 修正 mitsu 2008/09/01 不要 ***
                'With GCom.GLog
                '    .Result = "CMT読込失敗"
                '    .Discription = " -- "
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "CMT読込"
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End Try
    End Function

End Class
