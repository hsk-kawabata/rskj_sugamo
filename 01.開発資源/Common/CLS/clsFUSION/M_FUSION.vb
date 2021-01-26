Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Module M_FUSION
    '2018/02/16 saitou 広島信金(RSV2標準) DEL サーバー処理対応(64ビット対応) -------------------- START
    'Public vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/16 saitou 広島信金(RSV2標準) DEL --------------------------------------------------- END
    Public lngEXITCODE As Long
    Public clsFUSION As New clsFUSION.clsMain

    'Oracle接続
    Public gstrSSQL As String  'SQL文
    Public gdbcCONNECT As New OracleClient.OracleConnection   'CONNECTION
    Public gdbrREADER As OracleClient.OracleDataReader 'READER
    Public gdbCOMMAND As OracleClient.OracleCommand   'COMMAND関数
    Public gdbTRANS As OracleClient.OracleTransaction 'TRANSACTION

    Public gstrFTR_OPENDIR As String
    Public gstrFTRANP_OPENDIR As String
    Public gstrFTRAN2000_OPENDIR As String
    '*****20120710 mubuchi DVD追加対応*******
    Public DVD_DRIVE As String
    '*****20120710 mubuchi DVD追加対応*******

End Module
