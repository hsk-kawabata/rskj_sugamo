Imports MenteCommon

Module M_FUSION

    Public GCom As MenteCommon.clsCommon

    Public MainForm As KFUMENU020
    Public GOwnerForm As Form

    'ログインユーザ用
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String

    'Biwareログファイルフォーマット
    Structure gstrBiware_LOG
        <VBFixedString(10)> Public LOG1 As String   '伝送日付
        <VBFixedString(1)> Public SP1 As String     'セパレータ
        <VBFixedString(8)> Public LOG2 As String    '開始時間
        <VBFixedString(1)> Public SP2 As String     'セパレータ
        <VBFixedString(4)> Public LOG3 As String    '発受信
        <VBFixedString(1)> Public SP3 As String     'セパレータ
        <VBFixedString(1)> Public LOG4 As String    'サイクル番号(？)
        <VBFixedString(1)> Public SP4 As String     'セパレータ
        <VBFixedString(20)> Public LOG5 As String   '相手先センタ名
        <VBFixedString(1)> Public SP5 As String     'セパレータ
        <VBFixedString(4)> Public LOG6 As String    '送受信
        <VBFixedString(1)> Public SP6 As String     'セパレータ
        <VBFixedString(12)> Public LOG7 As String   'ファイルＩＤ
        <VBFixedString(1)> Public SP7 As String     'セパレータ
        <VBFixedString(256)> Public LOG8 As String  'ファイルパス名
        <VBFixedString(1)> Public SP8 As String     'セパレータ
        <VBFixedString(5)> Public LOG9 As String    'テキスト数
        <VBFixedString(1)> Public SP9 As String     'セパレータ
        <VBFixedString(8)> Public LOG10 As String   'レコード数
        <VBFixedString(1)> Public SP10 As String     'セパレータ
        <VBFixedString(7)> Public LOG11 As String  'エラーコード
        <VBFixedString(1)> Public SP11 As String     'セパレータ
        <VBFixedString(10)> Public LOG12 As String   '終了日
        <VBFixedString(1)> Public SP12 As String     'セパレータ
        <VBFixedString(8)> Public LOG13 As String  '終了時間
        <VBFixedString(1)> Public SP13 As String     'セパレータ
    End Structure
    Public gstrBiware As gstrBiware_LOG
    Structure gstrBiware_LOG_366
        <VBFixedString(366)> Public LOG As String    'データ区分(=2)
    End Structure
    Public gstrBiware366 As gstrBiware_LOG_366

End Module
