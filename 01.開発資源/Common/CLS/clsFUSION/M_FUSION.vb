Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Module M_FUSION
    '2018/02/16 saitou �L���M��(RSV2�W��) DEL �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
    'Public vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/16 saitou �L���M��(RSV2�W��) DEL --------------------------------------------------- END
    Public lngEXITCODE As Long
    Public clsFUSION As New clsFUSION.clsMain

    'Oracle�ڑ�
    Public gstrSSQL As String  'SQL��
    Public gdbcCONNECT As New OracleClient.OracleConnection   'CONNECTION
    Public gdbrREADER As OracleClient.OracleDataReader 'READER
    Public gdbCOMMAND As OracleClient.OracleCommand   'COMMAND�֐�
    Public gdbTRANS As OracleClient.OracleTransaction 'TRANSACTION

    Public gstrFTR_OPENDIR As String
    Public gstrFTRANP_OPENDIR As String
    Public gstrFTRAN2000_OPENDIR As String
    '*****20120710 mubuchi DVD�ǉ��Ή�*******
    Public DVD_DRIVE As String
    '*****20120710 mubuchi DVD�ǉ��Ή�*******

End Module
