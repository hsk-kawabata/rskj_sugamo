Option Explicit On 
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Data.OracleClient

' 機　能 : CMTファイルのデータアクセス用のクラス。
'            
'
'
Public Class clsCmtData
    Implements IDisposable

    Private Const ThisModuleName As String = "clsCmtData.vb"

    ' テーブル(CMT_READ_TBL / CMT_WRITE_TBL)に両方出現する項目をリストアップ
    '          変数名      保持用変数の型   DB項目名
    Protected nReceptionNo As Integer       ' 受付番号
    Protected strSyoriDate As String        ' 処理日(起動時オプションで渡されたシステム稼働日)  
    Protected strStationNo As String        ' CMT読取機番
    'Protected gstrPCName As String          ' DBには無いが便利なため保持
    Protected nErrCode As Integer           ' エラー番号
    Protected strErrName As String          ' エラー名
    Protected nStackerNo As Integer         ' スタッカー番号
    Protected strFileName As String         ' 持込ファイル名
    Protected bComplockFlag As Boolean       ' 暗号化フラグ       
    Public ReadSucceedFlag As Boolean       ' 読込成功フラグ
    Protected CreateDate As Date            ' 作成日
    Protected UpdateDate As Date            ' 更新日
    Protected bJSFlag As Boolean            ' 自振総給振フラグ
    Public bUploadSucceedFlg As Boolean     ' アップロード成否フラグ
    Public bListUpFlag As Boolean           ' ListView表示フラグ
    Protected bRWFlag As Boolean            ' 読込/書込フラグ

    ' テーブルには無い項目
    Protected nItakuCounter As Integer = -1 ' 全銀ファイル中のヘッダの数
    Public ItakuData() As clsItakuData      ' 委託者毎のデータ

    ' CmtWriteDataで追加する項目
    Protected nWriteCounter As Integer ' ファイル書込回数(同名のファイルを何回書き込んだかを記録)
    Protected bOverrideFlg As Boolean ' 強制書込みフラグ trueのとき強制書込み実施

    ' 機能　　　: コンストラクタ 
    ' 引数      : なし、の場合の処理
    Public Sub New()
        Init(1, 1, False, True)
    End Sub

    ' 機能　　　: コンストラクタ
    ' 引数      : ARG1 - 受付番号
    '           : ARG2 - スタッカ番号
    '           : ARG3 - ラベル有無フラグ
    '           : ARG4 - 暗号化フラグ
    Public Sub New(ByVal nReceptionNo As Integer _
    , ByVal nStackerNo As Integer _
    , ByVal ComplockFlag As Boolean _
    , ByVal JSFlag As Boolean _
    , ByVal rwflg As Boolean)
        ' 初期化処理
        Init(nReceptionNo, nStackerNo, ComplockFlag, rwflg)
    End Sub

    ' 機能　　　: 初期化
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - 受付番号
    '       　　: ARG2 - スタッカ番号
    '       　　: ARG3 - ラベル有無フラグ(True:ラベル有 / False:ラベル無し)
    '       　　: ARG4 - 暗号化フラグ (False:平文 / True:暗号)
    '           : ARG5 - Read/Writeフラグ (True : Read / False : Write)
    ' 備考　　　: なし
    Public Sub Init(ByVal recep As Integer _
        , ByVal nStackerNo As Integer _
        , ByVal compFlag As Boolean _
        , ByVal rwflg As Boolean)
        Me.nReceptionNo = recep
        Me.nStackerNo = nStackerNo
        Me.strStationNo = CmtCom.gstrStationNo
        Me.strSyoriDate = CmtCom.gstrSysDate
        Me.strFileName = "none"

        Me.nErrCode = 0
        Me.strErrName = "正常"
        Me.bComplockFlag = compFlag
        Me.bUploadSucceedFlg = False
        Me.bListUpFlag = False
        Me.bOverrideFlg = False
        Me.nWriteCounter = 0
        Me.bJSFlag = True ' デフォルトでは自振
        Me.bRWFlag = rwflg

        nItakuCounter = -1
    End Sub

    ' 受付番号を返す
    Property ReceptionNo() As Integer
        Get
            Return nReceptionNo
        End Get
        Set(ByVal Value As Integer)
            nReceptionNo = Value
        End Set
    End Property

    ' ヘッダをスキャンした際に設定する項目
    ' 機能　　　: ヘッダをスキャンした際に設定する項目
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - 振替日
    '         　: ARG2 - 委託者コード
    '
    ' 備考　　　: なし
    '*** 修正 mitsu 2008/07/17 センター直接持込の場合は振替コード・企業コードも渡す ***
    'Public Function AddHeaderRecord(ByVal fileseq As Integer, ByVal strFuriDate As String, ByVal strItakuCode As String, ByVal strItakuKana As String _
    '    , ByVal strBankCode As String, ByVal strBranchCode As String) As Boolean
    Public Function AddHeaderRecord(ByVal fileseq As Integer, ByVal strFuriDate As String, ByVal strItakuCode As String, ByVal strItakuKana As String _
       , ByVal strBankCode As String, ByVal strBranchCode As String _
        , Optional ByVal strFuriCode As String = "", Optional ByVal strKigyoCode As String = "", Optional ByVal Syubetu As String = "", Optional ByVal nFormatKbn As Integer = 0) As Boolean
        '******************************************************************************
        ' 委託者インスタンスの追加と設定
        Me.nItakuCounter += 1
        ReDim Preserve ItakuData(nItakuCounter)

        '*** 修正 mitsu 2008/07/17 振替コード・企業コードも渡す ***
        'ItakuData(nItakuCounter) = New clsItakuData(fileseq, strFuriDate, strItakuCode, strItakuKana, strBankCode, strBranchCode)
        ItakuData(nItakuCounter) = New clsItakuData(fileseq, strFuriDate, strItakuCode, strItakuKana, strBankCode, strBranchCode, strFuriCode, strKigyoCode, Syubetu, nFormatKbn)
        '**********************************************************

        Return True
    End Function

    ' Complock版init
    ' 機能　　　: complock版の初期化関数
    '
    ' 戻り値　　: 正常終了 true / 異常終了 false
    '
    ' 引き数　　: ARG1 - 委託者コード
    '         　: ARG2 - 取引先主コード
    '           : ARG3 - 取引先副コード
    '
    ' 備考　　　: なし
    Public Function ComplockInit(ByVal itakucd As String, ByVal furidate As String) As Boolean

        Me.bComplockFlag = True

        ' 委託者インスタンスの追加と設定
        nItakuCounter += 1
        ReDim Preserve ItakuData(nItakuCounter)
        ItakuData(nItakuCounter) = New clsItakuData(False)
        If (itakucd = "") Then
            ItakuData(nItakuCounter).strItakuCode = "0000000000"
            ItakuData(nItakuCounter).strItakuKanji = " -- "
            ItakuData(nItakuCounter).strItakuKana = " -- "
            ItakuData(nItakuCounter).strToriSCode = "0000000"
            ItakuData(nItakuCounter).strToriFCode = "00"
            ItakuData(nItakuCounter).strFSyoriKbn = "0"
            ItakuData(nItakuCounter).strFuriDate = "00000000"
            '***ASTAR.S.S 2008.05.23 媒体区分追加       ***
            ItakuData(nItakuCounter).strBaitaiCode = "00"
            '**********************************************
        Else
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
            Dim str1 As String = String.Empty
            Dim str2 As String = String.Empty
            Dim fsyori As String = String.Empty
            'Dim str1, str2, fsyori As String
            '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
            '***ASTAR.S.S 2008.05.23 媒体区分追加       ***
            Dim baitaiCode As String = ""
            Dim itakukanricode As String = ""
            Dim multikbn As String = ""
            '**********************************************
            Call DB.GetFToriCode(itakucd, "", str1, str2, fsyori, 0) ' 委託者コードが正常に設定されていない場合も素通し

            ItakuData(nItakuCounter).strItakuCode = itakucd
            ItakuData(nItakuCounter).strItakuKanji = DB.GetItakuKanji(itakucd.PadLeft(10, "0"c))
            ItakuData(nItakuCounter).strItakuKana = DB.GetItakuKana(itakucd.PadLeft(10, "0"c))
            ItakuData(nItakuCounter).strToriSCode = str1
            ItakuData(nItakuCounter).strToriFCode = str2
            ItakuData(nItakuCounter).strFSyoriKbn = fsyori
            ItakuData(nItakuCounter).strFuriDate = furidate
            '***ASTAR.S.S 2008.05.23 媒体区分追加       ***
            Call DB.GetFToriCode(itakucd, "", str1, str2, fsyori, baitaiCode, itakukanricode, multikbn) ' 委託者コードが正常に設定されていない場合も素通し
            ItakuData(nItakuCounter).strBaitaiCode = baitaiCode
            '**********************************************
        End If
        Return True
    End Function

    ' 機能　　　: 空の委託データを生成
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - RWFlag  (読込結果→True / 書込結果の保持 False)
    '
    ' 備考　　　: なし
    Public Function InitEmptyItakuData(ByVal rw As Boolean) As Boolean
        Me.ComplockInit("", "")     ' 読取できなかったため、委託者不明
        If (rw) Then
            Me.setError(9, "CMT読取エラー")
        Else
            Me.setError(9, "返却ファイル読取エラー")
        End If

        Me.ComplockFlag = False     ' 暗号化されたものは処理対象外のため、全てFalse
        Me.Override = False         ' エラーチェックで引っかかった際の処理のため、全てFalse
        Me.bListUpFlag = True       ' エラー内容を表示するためtrue
        Me.RWFlag = rw
        With GCom.GLog
            .Result = "不明"
            If (rw) Then
                .Discription = "CMTファイル読取エラー"
            Else
                .Discription = "返却ファイル読取エラー"
            End If
            .Discription &= " ファイルが破損している可能性があります"
        End With
        GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    End Function

    ' 機能　　　: 空の委託データを生成(エラー出力せず)
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - RWFlag  (読込結果→True / 書込結果の保持 False)
    '
    ' 備考　　　: なし
    Public Function InitEmptyItakuDataNoErr(ByVal rw As Boolean) As Boolean
        Me.ComplockInit("", "")     ' 読取できなかったため、委託者不明

        Me.ComplockFlag = False     ' 暗号化されたものは処理対象外のため、全てFalse
        Me.Override = False         ' エラーチェックで引っかかった際の処理のため、全てFalse
        Me.bListUpFlag = True       ' 処理結果を表示するためtrue
        Me.RWFlag = rw
    End Function


    ' 機能　　　: データレコードをスキャンした際に設定する項目
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - 引き落とし金額
    '
    ' 備考　　　: なし
    Public Function AddDataRecord(ByVal nHikiKin As Decimal) As Boolean
        ' 引き落とし金額を委託者インスタンスに告知
        Return ItakuData(nItakuCounter).SetData(nHikiKin)
    End Function

    ' 機能　　　: トレーラレコードをスキャンした際に設定する項目
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - 件数
    '             ARG2 - 金額
    '             ARG3 - 振替済件数
    '             ARG4 - 振替済金額
    '             ARG5 - 不能件数
    '             ARG6 - 不能金額
    '
    ' 備考　　　: なし
    Public Function AddTrailerRecord(ByVal ken As Decimal, ByVal kin As Decimal, _
           ByVal furiken As Decimal, ByVal furikin As Decimal, _
           ByVal funouken As Decimal, ByVal funoukin As Decimal) As Boolean
        ' 引き落とし金額を委託者インスタンスに告知
        Return ItakuData(nItakuCounter).SetTrailer(ken, kin, furiken, furikin, funouken, funoukin)
    End Function

    ' 委託カウンタの取得
    Property ItakuCounter() As Integer
        Get
            Return nItakuCounter
        End Get
        Set(ByVal Value As Integer)
            nItakuCounter = Value
        End Set
    End Property

    ' 機能　　　: ファイル名を取得
    Property FileName() As String
        Get
            Return Me.strFileName
        End Get
        Set(ByVal Value As String)
            Me.strFileName = Value
        End Set
    End Property

    ' 機能　　　: 書込回数の取得
    Property WriteCounter() As Integer
        Get
            Return Me.nWriteCounter
        End Get
        Set(ByVal Value As Integer)
            Me.nWriteCounter = Value
        End Set
    End Property




    ' 機能　　　: エラー情報を設定
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - エラー番号
    '             ARG2 - エラー名
    ' 備考　　　: なし
    Public Sub setError(ByVal nerrCode As Integer, ByVal strerrName As String)
        Me.nErrCode = nerrCode
        Me.strErrName = strerrName

    End Sub

    ' 機能　　　: エラー判定
    '
    ' 戻り値　　: エラー番号が0でエラーがないとき True / エラー番号が0以外のとき False
    '
    ' 引き数　　: なし
    '
    ' 備考　　　: なし
    Public Function NotError() As Boolean
        If (nErrCode = 0) Then
            Return True
        Else
            Return False
        End If
    End Function

    ' 機能　　　: クラス破棄時の処理
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: なし
    '
    ' 備考　　　: なし
    Public Sub Dispose() Implements System.IDisposable.Dispose
        ' クラスを破棄するときに行う処理を記述
    End Sub

    ' 機能　　　: CMT読込実績テーブル用のInsertを実行するためのSQL文を生成
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: 成功 true / 失敗 false
    '
    ' 備考　　　: なし
    Public Function GetInsertSQL(ByVal nItakuCounter As Integer) As String
        '*** 修正 mitsu 2008/09/01 処理高速化 ***
        Dim strSQL As New StringBuilder(512)
        strSQL.Append("INSERT INTO CMT_READ_TBL(" _
            & "RECEPTION_NO" _
            & ", SYORI_DATE" _
            & ", FILE_SEQ" _
            & ", STATION_NO" _
            & ", FSYORI_KBN" _
            & ", TORIS_CODE" _
            & ", TORIF_CODE" _
            & ", ITAKU_CODE" _
            & ", FURI_DATE" _
            & ", SYORI_KEN" _
            & ", SYORI_KIN" _
            & ", FURI_KEN" _
            & ", FURI_KIN" _
            & ", FUNOU_KEN" _
            & ", FUNOU_KIN" _
            & ", ERR_CD" _
            & ", ERR_INFO" _
            & ", STACKER_NO" _
            & ", FILE_NAME" _
            & ", COMPLOCK_FLG" _
            & ", JS_FLG" _
            & ")")

        With ItakuData(nItakuCounter)
            strSQL.Append(" VALUES(")
            strSQL.Append(nReceptionNo)                                      ' 1.受付No
            strSQL.Append(", '" & strSyoriDate & "' ")                       ' 2.システム処理日
            strSQL.Append(", " & .nFileSeq)                                  ' 3.FILE_SEQ
            strSQL.Append(", '" & strStationNo & "' ")                       ' 4.受付PCの機番
            strSQL.Append(", '" & .strFSyoriKbn & "' ")                      ' 5.F処理区分
            strSQL.Append(", '" & .strToriSCode.PadLeft(7, "0"c) & "' ")     ' 6.取引先主コード
            strSQL.Append(", '" & .strToriFCode.PadLeft(2, "0"c) & "' ")     ' 7.取引先副コード
            strSQL.Append(", '" & .strItakuCode.PadLeft(10, "0"c) & "' ")    ' 8.委託者コード
            strSQL.Append(", '" & .strFuriDate & "' ")                       ' 9.振替日
            strSQL.Append(", " & .nSyoriKen.ToString)                        ' 10.件数  
            strSQL.Append(", " & .nSyoriKin.ToString)                        ' 11.金額
            strSQL.Append(", " & .nFuriKen.ToString)                         ' 12.振替済件数  
            strSQL.Append(", " & .nFuriKin.ToString)                         ' 13.振替済金額
            strSQL.Append(", " & .nFunouKen.ToString)                        ' 14.不能件数  
            strSQL.Append(", " & .nFunouKin.ToString)                        ' 15.不能金額
            strSQL.Append(", " & nErrCode.ToString)                          ' 16.エラーコード 
            strSQL.Append(", '" & strErrName & "' ")                         ' 17.エラー名
            strSQL.Append(", " & nStackerNo.ToString)                        ' 18.スタッカ番号
            strSQL.Append(", '" & strFileName & "' ")                        ' 19.持込ファイル名
            If bComplockFlag Then                                       ' 20.暗号化ファイル
                strSQL.Append(", '1' ")                            ' 暗号 
            Else
                strSQL.Append(", '0' ")                            ' 平文
            End If
            If Me.bJSFlag Then                                          ' 21.自振・総振
                strSQL.Append(", '1' ")                            ' 自振
            Else
                '***Astar酒井 2008/05/30
                strSQL.Append(", '0' ")                            ' 総振

                ''***Astar酒井 2008/05/30
                ''strSQL.Append(", '0' ")                            ' 総振
                'strSQL.Append(", '3' ")                            ' 総振
                ''***
                '***
            End If
            strSQL.Append(")")
        End With

        Return strSQL.ToString
        '****************************************
    End Function

    ' 機能　　　: CMT書込実績テーブル用のInsertを実行するためのSQL文を生成
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: 成功 true / 失敗 false
    '
    ' 備考　　　: なし
    Public Function GetWInsertSQL(ByVal nItakuCounter As Integer) As String
        '*** 修正 mitsu 2008/09/01 処理高速化 ***
        Dim strSQL As New StringBuilder(512)
        strSQL.Append("INSERT INTO CMT_WRITE_TBL(" _
            & "RECEPTION_NO" _
            & ", SYORI_DATE" _
            & ", FILE_SEQ" _
            & ", STATION_NO" _
            & ", FSYORI_KBN" _
            & ", TORIS_CODE" _
            & ", TORIF_CODE" _
            & ", ITAKU_CODE" _
            & ", FURI_DATE" _
            & ", SYORI_KEN" _
            & ", SYORI_KIN" _
            & ", FURI_KEN" _
            & ", FURI_KIN" _
            & ", FUNOU_KEN" _
            & ", FUNOU_KIN" _
            & ", ERR_CD" _
            & ", ERR_INFO" _
            & ", STACKER_NO" _
            & ", FILE_NAME" _
            & ", COMPLOCK_FLG" _
            & ", WRITE_COUNTER" _
            & ", OVERRIDE_FLG" _
            & ")")

        If (nItakuCounter >= 0) Then
            With ItakuData(nItakuCounter)
                strSQL.Append(" VALUES(")
                strSQL.Append(nReceptionNo)                                      ' 1.受付No
                strSQL.Append(", '" & strSyoriDate & "' ")                       ' 2.システム処理日
                strSQL.Append(", " & .nFileSeq)                                  ' 3.FILE_SEQ
                strSQL.Append(", '" & strStationNo & "' ")                       ' 4.受付PCの機番
                strSQL.Append(", '" & .strFSyoriKbn & "' ")                      ' 5.F処理区分
                strSQL.Append(", '" & .strToriSCode.PadLeft(7, "0"c) & "' ")     ' 6.取引先主コード
                strSQL.Append(", '" & .strToriFCode.PadLeft(2, "0"c) & "' ")     ' 7.取引先副コード
                strSQL.Append(", '" & .strItakuCode & "' ")                      ' 8.委託者コード
                strSQL.Append(", '" & .strFuriDate & "' ")                       ' 9.振替日
                strSQL.Append(", " & .nSyoriKen.ToString)                        ' 10.件数  
                strSQL.Append(", " & .nSyoriKin.ToString)                        ' 11.金額
                strSQL.Append(", " & .nFuriKen.ToString)                         ' 12.振替済件数  
                strSQL.Append(", " & .nFuriKin.ToString)                         ' 13.振替済金額
                strSQL.Append(", " & .nFunouKen.ToString)                        ' 14.不能件数  
                strSQL.Append(", " & .nFunouKin.ToString)                        ' 15.不能金額
                strSQL.Append(", " & nErrCode.ToString)                          ' 16.エラーコード 
                strSQL.Append(", '" & strErrName & "' ")                         ' 17.エラー名
                strSQL.Append(", " & nStackerNo.ToString)                        ' 18.スタッカ番号
                strSQL.Append(", '" & strFileName & "' ")                        ' 19.持込ファイル名
                If (bComplockFlag) Then                                     ' 20.暗号化ファイル
                    strSQL.Append(", '1' ")                            ' 暗号 
                Else
                    strSQL.Append(", '0' ")                            ' 平文
                End If
                strSQL.Append(", " & nWriteCounter.ToString)                     ' 21.書込回数
                If (bOverrideFlg) Then                                      ' 22.強制書込フラグ
                    strSQL.Append(", '1' ")                            ' 強制
                Else
                    strSQL.Append(", '0' ")                            ' 通常
                End If
                strSQL.Append(")")
            End With
        Else
            strSQL.Append(" VALUES(")
            strSQL.Append(nReceptionNo)                                      ' 1.受付No
            strSQL.Append(", '" & strSyoriDate & "' ")                       ' 2.システム処理日
            strSQL.Append(", 0")                                             ' 3.FILE_SEQ
            strSQL.Append(", '" & strStationNo & "' ")                       ' 4.受付PCの機番
            strSQL.Append(", '0' ")                                          ' 5.F処理区分
            strSQL.Append(", '0000000' ")                                    ' 6.取引先主コード
            strSQL.Append(", '00' ")                                         ' 7.取引先副コード
            strSQL.Append(", '0000000000'")                                  ' 8.委託者コード
            strSQL.Append(", '00000000'")                                    ' 9.振替日
            strSQL.Append(", 0")                                             ' 10.件数  
            strSQL.Append(", 0")                                             ' 11.金額
            strSQL.Append(", 0")                                             ' 12.振替済件数  
            strSQL.Append(", 0")                                             ' 13.振替済金額
            strSQL.Append(", 0")                                             ' 14.不能件数  
            strSQL.Append(", 0")                                             ' 15.不能金額
            strSQL.Append(", " & nErrCode.ToString)                          ' 16.エラーコード 
            strSQL.Append(", '" & strErrName & "' ")                         ' 17.エラー名
            strSQL.Append(", " & nStackerNo.ToString)                        ' 18.スタッカ番号
            strSQL.Append(", '" & strFileName & "' ")                        ' 19.持込ファイル名
            If (bComplockFlag) Then                                     ' 20.暗号化ファイル
                strSQL.Append(", '1' ")                            ' 暗号 
            Else
                strSQL.Append(", '0' ")                            ' 平文
            End If
            strSQL.Append(", " & nWriteCounter.ToString)                     ' 21.書込回数
            If (bOverrideFlg) Then                                      ' 22.強制書込フラグ
                strSQL.Append(", '1' ")                            ' 強制
            Else
                strSQL.Append(", '0' ")                            ' 通常
            End If
            strSQL.Append(")")
        End If

        Return strSQL.ToString
        '****************************************
    End Function

    ' 機能　　　: clsCMTDataのメンバ変数の情報からListViewItemを返す
    '
    ' 戻り値　　: 成功 true / 失敗 false
    '
    ' 引き数　　: ListViewItem
    ' 備考　　　: 読込/書込両対応 2007/12/07 前田
    Public Function getListViewItem(ByRef lv As ListViewItem, ByVal idx As Integer) As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim createDate As String = String.Empty
        Dim updateDate As String = String.Empty
        'Dim createDate As String
        'Dim updateDate As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        If (idx < 0) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "レコードに委託者情報無し 受付番号：" & Me.nReceptionNo.ToString
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If
        Try
            lv.SubItems.Add(ItakuData(idx).nFileSeq.ToString.PadLeft(2, "0"c)) ' 01.FILE_SEQ
            lv.SubItems.Add(Me.strSyoriDate)                                    ' 02.処理日
            lv.SubItems.Add(nStackerNo.ToString.PadLeft(2, "0"c))               ' 03.スタッカ番号
            If Me.bRWFlag Then                                                  ' 04.書込回数
                ' 読込
                lv.SubItems.Add("-")                                            '  - 読込なので無効
            Else
                ' 書込
                lv.SubItems.Add(nWriteCounter.ToString)                         '  - 書込回数を出力
            End If

            lv.SubItems.Add(ItakuData(idx).strItakuCode.PadLeft(10, "0"c))  ' 05.委託者コード(FILE_SEQ毎)
            lv.SubItems.Add(ItakuData(idx).strFuriDate)                     ' 06.振替日(FILE_SEQ毎)
            lv.SubItems.Add(ItakuData(idx).strItakuKana)                    ' 07.委託者カナ
            lv.SubItems.Add(ItakuData(idx).strItakuKanji)                   ' 08.委託者カナ
            lv.SubItems.Add(ItakuData(idx).nSyoriKen.ToString)              ' 09.件数
            lv.SubItems.Add(ItakuData(idx).nSyoriKin.ToString)              ' 10.金額
            lv.SubItems.Add(ItakuData(idx).nCheckTotalKen.ToString)         ' 11.データレコード合計件数
            lv.SubItems.Add(ItakuData(idx).nCheckTotalKin.ToString)         ' 12.データレコード合計金額
            lv.SubItems.Add(nErrCode.ToString.PadLeft(1, "0"c))             ' 13.エラーコード
            lv.SubItems.Add(strErrName)                                     ' 14.エラー名
            lv.SubItems.Add(ItakuData(idx).strBankCode.PadLeft(4, "0"c))    ' 15.金融機関コード
            lv.SubItems.Add(ItakuData(idx).strBankName)                     ' 16.金融機関名
            lv.SubItems.Add(ItakuData(idx).strBranchCode.PadLeft(3, "0"c))  ' 17.支店コード
            lv.SubItems.Add(ItakuData(idx).strBranchName)                   ' 18.支店名
            If (Me.bRWFlag) Then                                                                      ' 19.自振・総振フラグ
                If (Me.bJSFlag) Then
                    lv.SubItems.Add("自")                                   '  - 自振
                Else
                    lv.SubItems.Add("総")                                   '  - 総振
                End If
            Else
                ' 書込には自振しかない
                lv.SubItems.Add("自")                                       '  - 自振
            End If

            lv.SubItems.Add(ItakuData(idx).nFuriKen.ToString)               ' 20.振替済件数
            lv.SubItems.Add(ItakuData(idx).nFuriKin.ToString)               ' 21.振替済金額
            lv.SubItems.Add(ItakuData(idx).nFunouKen.ToString)              ' 22.不能件数
            lv.SubItems.Add(ItakuData(idx).nFunouKin.ToString)              ' 23.不能金額
            lv.SubItems.Add(ItakuData(idx).strFSyoriKbn)                    ' 24.F処理区分
            lv.SubItems.Add(ItakuData(idx).strToriSCode)                    ' 25.取引先主コード
            lv.SubItems.Add(ItakuData(idx).strToriFCode)                    ' 26.取引先副コード
            lv.SubItems.Add(Me.strStationNo)                                ' 27.CMT読取機番
            lv.SubItems.Add(Me.FileName)                                 ' 28ファイル名
            If Not bRWFlag Then                                                 ' 29.強制書込みフラグ
                ' 書込時
                If Me.bOverrideFlg Then
                    lv.SubItems.Add("強制")                                 '  - 強制
                Else
                    lv.SubItems.Add("通常")                                 '  - 通常
                End If
            Else
                ' 読込時
                lv.SubItems.Add(" - ")                                      '  - 未使用
            End If

            If Me.bComplockFlag Then                                                                  ' 30.暗号化フラグ
                lv.SubItems.Add("C")
            Else
                lv.SubItems.Add("N")                                        '  - N:平文
            End If

            Me.GetCreateDate(Me.ItakuData(idx).nFileSeq, createDate, updateDate)
            lv.SubItems.Add(createDate)                                     ' 31.作成日
            lv.SubItems.Add(updateDate)                                     ' 32.更新日

        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "ListViewItem生成処理 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try
        Return True
    End Function

    ' 機能　　　: CMT読込実績TBL/CMT書込実績TBLの生成日・更新日を取得
    '
    ' 戻り値　　: 取得成功 true / 取得失敗 false
    '
    ' 引き数    : ARG1 FILE_SEQ
    '           : ARG2 生成日(参照渡し)
    '           : ARG3 更新日(参照渡し)
    ' 備考　　　: 
    Private Function GetCreateDate(ByVal fileseq As Integer, ByRef strCreateDate As String, ByRef strUpdateDate As String) As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim onReader As OracleDataReader = Nothing
        'Dim onReader As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim strTbl As String

        If Me.bRWFlag Then
            strTbl = "CMT_READ_TBL"
        Else
            strTbl = "CMT_WRITE_TBL"
        End If

        If fileseq = 0 Then ' FILE_SEQがゼロのとき検索は実行しない
            strUpdateDate = " --- "
            strCreateDate = " --- "
            Return False
        End If

        Try
            Dim SQL As String = "SELECT "
            SQL &= "  CREATE_DATE"
            SQL &= ", UPDATE_DATE"
            SQL &= "  FROM " & strTbl
            SQL &= "  WHERE RECEPTION_NO = " & Me.nReceptionNo.ToString()
            SQL &= "  AND FILE_SEQ = " & fileseq.ToString
            SQL &= "  AND SYORI_DATE = '" & Me.strSyoriDate & "'"
            SQL &= "  AND STATION_NO = '" & CmtCom.gstrStationNo & "'"
            If GCom.SetDynaset(SQL, onReader) AndAlso onReader.Read Then
                strCreateDate = onReader.GetOracleDateTime(0).ToString
                strUpdateDate = onReader.GetOracleDateTime(1).ToString
                Return True
            Else
                '*** 修正 mitsu 2008/09/01 不要 ***
                'With GCom.GLog
                '    .Result = "生成日・更新日取得失敗"
                '    .Discription = "SQL文:" & SQL
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                '**********************************
                strUpdateDate = " --- "
                strCreateDate = " --- "
                Return False
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "生成日・更新日取得失敗 " & ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            strUpdateDate = " -- "
            strCreateDate = " -- "
            Return False
        End Try
    End Function

    ' 機能　　　: 他のCMTデータ構造体と比較
    '             処理日と委託件数のカウンタを比較した後、委託データの比較を行う。
    '
    ' 戻り値　　: 合致 true / 不一致 false
    '
    ' 引き数　　: clsCmtDataのインスタンス
    ' 備考　　　: 
    Public Function CompareWR(ByRef cmtData As clsCmtData) As Boolean

        If Me.strSyoriDate = cmtData.strSyoriDate And _
            Me.nItakuCounter = cmtData.nItakuCounter And _
            Me.bJSFlag = cmtData.bJSFlag Then
            For i As Integer = 0 To Me.nItakuCounter - 1
                With cmtData.ItakuData(i)
                    If Me.ItakuData(i).nCheckTotalKen = .nCheckTotalKen And _
                        Me.ItakuData(i).nCheckTotalKin = .nCheckTotalKin And _
                        Me.ItakuData(i).nSyoriKen = .nSyoriKen And _
                        Me.ItakuData(i).nSyoriKin = .nSyoriKin And _
                        Me.ItakuData(i).strBankCode = .strBankCode And _
                        Me.ItakuData(i).strBranchCode = .strBranchCode And _
                        Me.ItakuData(i).strItakuCode = .strItakuCode And _
                        Me.ItakuData(i).strItakuKana = .strItakuKana Then
                        Return True
                    Else
                        Return False
                    End If
                End With
            Next
            Return True
        Else
            Return False
        End If
    End Function


    ' 強制書込みフラグの設定
    Property Override() As Boolean
        Get
            Return bOverrideFlg
        End Get
        Set(ByVal Value As Boolean)
            bOverrideFlg = Value
        End Set
    End Property

    ' 自振/総振フラグの設定
    Property JSFlag() As Boolean
        Get
            Return bJSFlag
        End Get
        Set(ByVal Value As Boolean)
            bJSFlag = Value
        End Set
    End Property

    ' 読書フラグの設定
    Property RWFlag() As Boolean
        Get
            Return bRWFlag
        End Get
        Set(ByVal Value As Boolean)
            bRWFlag = Value
        End Set
    End Property

    ' 暗号化フラグの設定
    Property ComplockFlag() As Boolean
        Get
            Return Me.bComplockFlag
        End Get
        Set(ByVal Value As Boolean)
            Me.bComplockFlag = Value
        End Set
    End Property

    ' エラーコード取得
    Property ErrCode() As Integer
        Get
            Return Me.nErrCode
        End Get
        Set(ByVal Value As Integer)
            Me.nErrCode = Value
        End Set
    End Property

    ' エラー名の設定
    Property ErrorName() As String
        Get
            Return Me.strErrName
        End Get
        Set(ByVal Value As String)
            Value = Me.strErrName
        End Set
    End Property
End Class
