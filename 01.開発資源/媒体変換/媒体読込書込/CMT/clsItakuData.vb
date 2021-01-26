Option Explicit On 
Option Strict On

' 機　能 : 全銀フォーマットの委託者毎のデータを保持するクラス
'
'
Public Class clsItakuData

    Private Const ThisModuleName As String = "clsItakuData.vb"

    ' 全銀データのヘッダ、データ、トレイラーから取得できる情報
    Public nFileSeq As Integer = 0      ' FILE_SEQ
    Public strFuriDate As String = ""   ' 振替日
    Public strItakuCode As String = ""  ' 委託者コード 
    Public strItakuKana As String = ""  ' 委託者名カナ(DBには無いが表示用に便利なため保持)
    Public strItakuKanji As String = "" ' 委託者漢字名(DBには無いが表示用に便利なため保持)
    Public strBankCode As String = ""   ' 金融機関コード(DBには無いが表示用に便利なため保持)
    Public strBankName As String = ""   ' 金融機関名
    Public strBranchCode As String = "" ' 支店コード
    Public strBranchName As String = "" ' 支店名
    Public strAccountShubetu As String = "" ' 口座種別
    Public strAccountNo As String = ""  ' 口座番号
    Public nSyoriKin As Decimal = 0D    ' 処理金額
    Public nSyoriKen As Decimal = 0D    ' 処理件数
    Public nFuriKen As Decimal = 0D     ' 振替済件数
    Public nFuriKin As Decimal = 0D     ' 振替済金額
    Public nFunouKen As Decimal = 0D    ' 不能件数
    Public nFunouKin As Decimal = 0D    ' 不能金額

    ' 上記情報から他テーブルを参照して生成される情報
    Public strFSyoriKbn As String        ' F処理区分
    Public strToriSCode As String        ' 取引先主コード
    Public strToriFCode As String        ' 取引先副コード

    '***ASTAR.S.S 2008.05.23 媒体区分追加       ***
    Public strBaitaiCode As String      ' 媒体コード
    '**********************************************

    '*** 修正 mitsu 2008/07/17 振替コード・企業コード追加 ***
    Public strFuriCode As String
    Public strKigyoCode As String
    '********************************************************
    Public strSyubetu As String
    Public strItakuKanriCode As String
    Public strMultiKbn As String

    Public nCheckTotalKin As Decimal = 0D    ' データレコードの引き落とし金額の合計値 
    Public nCheckTotalKen As Decimal = 0D    ' データレコードの件数の合計値 
    Public noBankFlg As Boolean = False  ' 金融機関名・支店名を取得しない場合にtrue
    ' 
    ' 機能　　　: クラスのインスタンスの初期化
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: なし
    '
    ' 備考　　　: initを呼ぶだけで何もしない
    Public Sub New()
        ' 全ての変数を初期化
        init()
    End Sub

    Public Sub New(ByVal nobank As Boolean)
        ' 全ての変数を初期化
        init()
        Me.noBankFlg = nobank
    End Sub

    '*** 修正 mitsu 2008/07/17 センター直接持込の場合は振替コード・企業コードも渡す ***
    'Public Sub New(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String _
    '    , ByVal itakukana As String, ByVal bankcode As String, ByVal branchcode As String)
    Public Sub New(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String _
        , ByVal itakukana As String, ByVal bankcode As String, ByVal branchcode As String _
        , Optional ByVal furicode As String = "", Optional ByVal kigyocode As String = "", Optional ByVal syubetu As String = "", Optional ByVal nFormatkbn As Integer = 0)
        '******************************************************************************

        ' 全ての変数を初期化
        init()

        '*** 修正 mitsu 2008/07/17 振替コード・企業コードも渡す ***
        'SetHeader(fileseq, furiDate, itaku, itakukana, bankcode, branchcode)
        SetHeader(fileseq, furiDate, itaku, itakukana, bankcode, branchcode, furicode, kigyocode, syubetu, nFormatkbn)
        '**********************************************************
    End Sub

    ' 機能　　　: メンバ変数の初期化
    '
    ' 戻り値　　: 成功 True / 失敗 False
    '
    ' 引き数　　: なし
    '
    ' 備考　　　: 
    Public Function init() As Boolean
        nFileSeq = 0                ' FILE_SEQ
        strFuriDate = ""            ' 振替日
        strItakuCode = ""           ' 委託者コード 初期化
        strItakuKana = ""           ' 委託者名カナ(DBには無いが表示用に便利なため保持)初期化
        strItakuCode = ""           ' 委託者コード初期化
        strToriSCode = ""           ' 取引先主コード初期化
        strToriFCode = ""           ' 取引先副コード初期化
        nSyoriKin = 0D              ' 処理金額初期化
        nSyoriKen = 0D              ' 処理件数初期化
        nFuriKen = 0D               ' 振替済件数初期化
        nFuriKin = 0D               ' 振替済金額初期化
        nFunouKen = 0D              ' 不能件数初期化
        nFunouKin = 0D              ' 不能金額初期化

        strFSyoriKbn = ""            ' F処理区分
        strToriSCode = ""            ' 取引先主コード
        strToriFCode = ""            ' 取引先副コード

        '*** 修正 mitsu 2008/07/17 振替コード・企業コード追加 ***
        strFuriCode = ""
        strKigyoCode = ""
        '********************************************************

        nCheckTotalKin = 0D         ' データレコードの引き落とし金額合計値
        nCheckTotalKen = 0D         ' データレコード件数の合計値
        Return False
    End Function

    ' 機能　　　: ヘッダ情報の設定
    '
    ' 戻り値　　: 成功 True / 失敗 False
    '
    ' 引き数　　: ARG1 - FILE_SEQ
    '             ARG2 - 振替日
    '             ARG3 - 委託者コード,
    '             ARG4 - 委託者カナ名
    '             ARG5 - 金融機関コード
    '             ARG6 - 支店コード
    '
    ' 備考　　　: 
    '*** 修正 mitsu 2008/07/17 センター直接持込の場合は振替コード・企業コードも渡す ***
    'Public Function SetHeader(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String, ByVal itakuKana As String, ByVal bankcode As String, ByVal branchcode As String) As Boolean
    Public Function SetHeader(ByVal fileseq As Integer, ByVal furiDate As String, ByVal itaku As String, ByVal itakuKana As String, ByVal bankcode As String, ByVal branchcode As String _
        , Optional ByVal furicode As String = "", Optional ByVal kigyocode As String = "", Optional ByVal syubetu As String = "", Optional ByVal nFormatkbn As Integer = 0) As Boolean
        '******************************************************************************
        Me.strItakuKana = itakuKana
        Me.nFileSeq = fileseq
        Me.strFuriDate = furiDate
        Me.strItakuCode = itaku
        Me.strBankCode = bankcode
        Me.strBranchCode = branchcode

        '*** 修正 mitsu 2008/07/17 振替コード・企業コード追加 ***
        Me.strFuriCode = furicode
        Me.strKigyoCode = kigyocode
        '********************************************************

        nCheckTotalKin = 0D         ' データレコードの引き落とし金額合計値
        nCheckTotalKen = 0D         ' データレコード件数の合計値

        CmtCom.CheckDate(strFuriDate)

        ' F処理区分, 取引先主コード, 取引先副コードの取得

        '*** 修正 mitsu 2008/07/17 振替コード・企業コードを渡された場合 ***
        'strItakuKanji = DB.GetItakuKanji(strItakuCode)
        'センター直接持込対応
        If strFuriCode = "" AndAlso strKigyoCode = "" Then
            strItakuKanji = DB.GetItakuKanji(strItakuCode)
        Else
            strItakuKanji = DB.GetItakuKanji(itaku, strFuriCode, strKigyoCode)
            '委託者コードを正しい値に読替
            Me.strItakuCode = itaku
        End If
        '******************************************************************
        '*** ASTAR.S.S 媒体コード追加   2008.05.23  ***
        '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加 START
        'If Not DB.GetFToriCode(itaku, syubetu, strToriSCode, strToriFCode, strFSyoriKbn, strBaitaiCode, strItakuKanriCode, strMultiKbn, nFormatkbn) Then
        If Not DB.bGetFToriCode(itaku, syubetu, strToriSCode, strToriFCode, strFSyoriKbn, strBaitaiCode, strItakuKanriCode, strMultiKbn, nFormatkbn, furiDate) Then
            '2010.03.18 鶴来信金カスタマイズ 振替日、返還フラグ追加  END
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "処理区分、取引先主コード、副コードの取得失敗 委託者コード：" & itaku
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        ' 金融機関漢字名, 支店漢字名の取得
        If Not noBankFlg Then
            If DB.GetBankAndBranchName(bankcode, branchcode, Me.strBankName, Me.strBranchName) Then
                'MessageBox.Show("金融機関名:" & Me.strBankName & ", 支店名:" & Me.strBranchName)
            Else
                Me.strBankName = " -- "
                Me.strBranchName = " -- "
                ' MessageBox.Show("金融期間情報取得失敗")
                '***ASTAR SUSUKI 2008.06.13                 ***
                '***金融機関チェックをはずす
                'With GCom.GLog
                '    .Result = "金融機関名、支店名取得失敗"
                '    .Discription = "金融機関コード:" & bankcode & ", 支店コード:" & branchcode
                'End With
                'GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                'Return False
                '***                                        ***
            End If
        End If
        Return True
    End Function

    ' 機能　　　: データレコードの件数のカウントと金額の設定
    '
    ' 戻り値　　: 成功 True / 失敗 False
    '
    ' 引き数　　: ARG1 - 引落金額
    '
    ' 備考　　　: 
    Public Function SetData(ByVal hikikin As Decimal) As Boolean
        '
        If (hikikin < 0) Then
            Return False
        End If
        nCheckTotalKin += hikikin
        nCheckTotalKen += 1
        Return True
    End Function

    ' 機能　　　: トレーラ情報の設定
    '
    ' 戻り値　　: 成功 True / 失敗 False
    '
    ' 引き数　　: ARG1 - 処理件数
    '             ARG2 - 処理金額
    '             ARG3 - 振替済件数
    '             ARG4 - 振替済金額
    '             ARG5 - 不能件数
    '             ARG6 - 不能金額
    '                     
    ' 備考　　　: 
    Public Function SetTrailer(ByVal syoriken As Decimal, ByVal syorikin As Decimal _
        , ByVal furiken As Decimal, ByVal furikin As Decimal _
        , ByVal funouken As Decimal, ByVal funoukin As Decimal) As Boolean
        '  数値エラーチェック
        Me.nSyoriKen = syoriken
        Me.nSyoriKin = syorikin
        Me.nFuriKen = furiken
        Me.nFuriKin = furikin
        Me.nFunouKen = funouken
        Me.nFunouKin = funoukin

        If (syoriken < 0) Or (syorikin < 0) Or (furiken < 0) Or (furikin < 0) _
            Or (funouken < 0) Or (funoukin < 0) Then
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "データトレーラ不整合 ヘッダ処理件数：" & nSyoriKen & ",合計件数：" & nCheckTotalKen & ", ヘッダ処理金額：" _
                & nSyoriKin & ",合計処理金額：" & nCheckTotalKin
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If
        ' データレコードの合計値とトレーラレコードの値を比較
        If (Me.nSyoriKen <> Me.nCheckTotalKen) Or (Me.nSyoriKin <> Me.nCheckTotalKin) Then
            ' 不整合発生
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = "データトレーラ不整合 ヘッダ処理件数：" & Me.nSyoriKen & ",合計件数：" & Me.nCheckTotalKen & ", ヘッダ処理金額：" _
                & Me.nSyoriKen & ",合計処理金額：" & Me.nCheckTotalKin
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            Return False
        End If

        Return True
    End Function

End Class