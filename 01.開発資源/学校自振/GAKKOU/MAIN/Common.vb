Option Explicit On
Option Strict Off

Imports System.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic 
Module Common

#Region "���ʕϐ�"

    Public GCom As New MenteCommon.clsCommon
    '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
    'Public MainLog As New CASTCommon.BatchLOG("Common", "�N������")
    Public MainLog As New CASTCommon.BatchLOG("Common", "G_GCOM")
    '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

    Private ConString As New CASTCommon.DB
    Public ReadOnly STR_CONNECTION As String = CASTCommon.DB.CONNECT
    '���ʃC�x���g�R���g���[��
    Public CAST As New CASTCommon.Events
    '' �������w��
    Public CASTx As New CASTCommon.Events("0-9a-zA-Z", CASTCommon.Events.KeyMode.GOOD)
    Public CASTxx As New CASTCommon.Events("0-9a-zA-Z._-", CASTCommon.Events.KeyMode.GOOD)
    Public CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Public CASTx02 As New CASTCommon.Events("0-9-", CASTCommon.Events.KeyMode.GOOD)
    Public CASTx03 As New CASTCommon.Events("0-9()-", CASTCommon.Events.KeyMode.GOOD)
    '' �񋖉����ݒ�
    Public CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)
    Public CASTx2 As New CASTCommon.Events("""", CASTCommon.Events.KeyMode.BAD) '2010/11/22.Sakon�@�_�u���N�H�[�e�[�V�������񋖉Ƃ���
    'Public CASTx2 As New CASTCommon.Events("", CASTCommon.Events.KeyMode.BAD)
    '2017/02/23 �^�X�N�j���� DEL �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
    '���g�p�̂��ߍ폜
    ''���
    'Public Const CONST_HIMOKUMAXCNT As Integer = 15 'DB�ő�l
    'Public Const CONST_HIMOKUCNT As Integer = 15 '�g�p����ő�l
    '2017/02/23 �^�X�N�j���� DEL �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END

    '****************************************
    '���ʕϐ���`
    '****************************************  
    Public OBJ_CONNECTION As Data.OracleClient.OracleConnection
    Public OBJ_DATAREADER As Data.OracleClient.OracleDataReader
    Public OBJ_COMMAND As Data.OracleClient.OracleCommand
    Public OBJ_TRANSACTION As Data.OracleClient.OracleTransaction
    Public OBJ_DATAADAPTER As Data.OracleClient.OracleDataAdapter

    'DATAREADER���g�p���Ă��鎞�Ƀf�[�^�o�^���s���ۂɎg�p
    Public OBJ_CONNECTION_DREAD As Data.OracleClient.OracleConnection
    Public OBJ_DATAREADER_DREAD As Data.OracleClient.OracleDataReader
    Public OBJ_COMMAND_DREAD As Data.OracleClient.OracleCommand

    'DATAREADER���g�p���Ă��鎞�Ƀf�[�^�o�^���s���ۂɎg�p
    Public OBJ_CONNECTION_DREAD2 As Data.OracleClient.OracleConnection
    Public OBJ_DATAREADER_DREAD2 As Data.OracleClient.OracleDataReader
    Public OBJ_COMMAND_DREAD2 As Data.OracleClient.OracleCommand

    'DATAREADER���g�p���Ă��鎞�Ƀf�[�^�o�^���s���ۂɎg�p
    Public OBJ_CONNECTION_DREAD3 As Data.OracleClient.OracleConnection
    Public OBJ_DATAREADER_DREAD3 As Data.OracleClient.OracleDataReader
    Public OBJ_COMMAND_DREAD3 As Data.OracleClient.OracleCommand

    'Oracle�ڑ�(M_FUSION)
    Public gdbcCONNECT As New OracleClient.OracleConnection  'CONNECTION
    Public gdbrREADER As OracleClient.OracleDataReader 'READER
    Public gdbCOMMAND As OracleClient.OracleCommand  'COMMAND�֐�
    Public gdbTRANS As OracleClient.OracleTransaction 'TRANSACTION

    Public STR_SQL As String
    Public STR_GCOAD() As String

    '�V�����o�^
    Public str���q�w�Z�R�[�h As String
    Public str���q���w�N�x As String
    Public str���q�ʔ� As String
    Public str���q�w�N As String
    Public str���q�N���X As String
    Public str���q���k�ԍ� As String

    Public STR_FURIKAE_DATE(1) As String

    '���k���ד��͗p
    Public STR_���k���׊w�Z�R�[�h As String
    Public STR_���k���׊w�Z�� As String
    Public STR_���k���אU�֓� As String
    Public STR_���k���ד��o�敪 As String
    Public STR_���k���׏����l As String
    Public STR_���k���׃\�[�g�� As String
    Public STR_���k���׈���敪 As String

    '�Z���^�[�R�[�h
    Public ReadOnly STR_CENTER_CODE As String = GFUNC_INI_READ("COMMON", "CENTER")
    '�R���{�{�b�N�X�ݒ�t�@�C���i�[PATH�擾
    Public ReadOnly STR_JIKINKO_CODE As String = GFUNC_INI_READ("COMMON", "KINKOCD")
    Public ReadOnly STR_TXT_PATH As String = GFUNC_INI_READ("COMMON", "TXT")
    'DAŢ�ٍ쐬��PATH�擾
    Public ReadOnly STR_DAT_PATH As String = GFUNC_INI_READ("COMMON", "DAT")
    'REPORŢ�يi�[��PATH�擾
    Public ReadOnly STR_LST_PATH As String = GFUNC_INI_READ("COMMON", "LST")
    Public ReadOnly STR_LOG_PATH As String = GFUNC_INI_READ("COMMON", "LOG")
    Public ReadOnly STR_CSV_PATH As String = GFUNC_INI_READ("COMMON", "CSV")
    Public ReadOnly STR_IFL_PATH As String = GFUNC_INI_READ("COMMON", "IRAIFL")
    Public ReadOnly STR_TAKIFL_PATH As String = GFUNC_INI_READ("COMMON", "TAKIRAIFL")
    '�`�F�b�N�f�B�W�b�g0:���Ȃ� 1:����
    Public ReadOnly STR_CHK_DJT As String = GFUNC_INI_READ("COMMON", "CHKDJT")
    '�`�F�b�N�\���
    Public ReadOnly STR_JIFURI_CHECK As String = GFUNC_INI_READ("JIFURI", "CHECK")
    '�s�\�敪
    Public ReadOnly STR_JIFURI_FUNOU As String = GFUNC_INI_READ("JIFURI", "FUNOU")
    '�z�M�敪
    Public ReadOnly STR_JIFURI_HAISIN As String = GFUNC_INI_READ("JIFURI", "HAISIN")
    '����敪
    Public ReadOnly STR_JIFURI_KAISYU As String = GFUNC_INI_READ("JIFURI", "KAISYU")
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
    '��ڃp�^�[��(0:�P�O�A1:�P�T��)
    Public ReadOnly STR_HIMOKU_PTN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HIMOKU_PTN")
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
    '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
    Public ReadOnly STR_KINGAKU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_KINGAKU_CHK")
    '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END
    '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���q�ݒ荀�ڂ̕\���^��\������j---------------------- START
    Public ReadOnly STR_TYOUSI_KBN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_TYOUSI_KBN")
    '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���q�ݒ荀�ڂ̕\���^��\������j---------------------- END

    '�N���X�ւ������p
    Public STR_�N���X�֊w�Z�R�[�h As String
    Public STR_�N���X�֊w�Z�� As String
    Public STR_�N���X�֊w�N�R�[�h As String
    Public STR_�N���X�֊w�N�� As String
    Public blnSingaku As Boolean '�i�w���k�t���O

    '���O�������p
    Public STR_SYORI_NAME As String
    Public STR_COMMAND As String
    Public STR_LOG_GAKKOU_CODE As String
    Public STR_LOG_FURI_DATE As String

    '�R���{�{�b�N�X�p�e�L�X�g�t�@�C����
    Public Const STR_NYUSYUTU2_TXT As String = "GFJ_���o���敪.TXT"
    Public Const STR_NYUSYUTU_TXT As String = "GFJ_���o�敪.TXT"
    Public Const STR_TAKOU_TXT As String = "GFJ_���s�敪.TXT"
    Public Const STR_KAMOKU_TXT As String = "GFJ_�Ȗ�.TXT"
    Public Const STR_BAITAI_TXT As String = "GFJ_�}��.TXT"
    Public Const STR_TAKO_BAITAI_TXT As String = "KZFMAST021_���s�}��.TXT"
    Public Const STR_TAKO_CODE_TXT As String = "KZFMAST021_�R�[�h�敪.TXT"
    Public Const STR_CODE_KBN_TXT As String = "GFJ_�R�[�h�敪.TXT"
    Public Const STR_FURIKAE_TXT As String = "GFJ_�U�֕��@.TXT"
    Public Const STR_SEIBETU_TXT As String = "GFJ_����.TXT"
    Public Const STR_SINKYU_TXT As String = "GFJ_�i���敪.TXT"
    Public Const STR_KAIYAKU_TXT As String = "GFJ_���敪.TXT"
    Public Const STR_TESUURYO_TXT As String = "GFJ_�萔���e�[�u��.TXT"
    Public Const STR_TESUURYO_KAMOKU_TXT As String = "GFJ_�萔��_�Ȗ�.TXT"
    Public Const STR_KESSAI_TXT As String = "GFJ_���ϋ敪.TXT"
    Public Const STR_HIMOKU_KAMOKU_TXT As String = "GFJ_���_�Ȗ�.TXT"
    Public Const STR_SEITO_KAMOKU_TXT As String = "GFJ_���k_�Ȗ�.TXT"
    Public Const STR_FURIKETUCODE_TXT As String = "GFJ_�U�֌��ʃR�[�h.TXT"
    Public Const STR_LOGPTN_TXT As String = "GFJ_���O���.TXT"
    Public Const STR_FURI_SFURI_KBN_TXT As String = "GFJ_���U�ĐU�敪.TXT"

    '------------------------------------------
    '�S��t�H�[�}�b�g
    '------------------------------------------
    '�w�b�_���R�[�h
    Structure gstrZG_Record1
        <VBFixedString(1)> Public ZG1 As String  '�f�[�^�敪(=1)
        <VBFixedString(2)> Public ZG2 As String  '��ʃR�[�h
        <VBFixedString(1)> Public ZG3 As String  '�R�[�h�敪
        <VBFixedString(10)> Public ZG4 As String  '�U���˗��l�R�[�h
        <VBFixedString(40)> Public ZG5 As String  '�U���˗��l��
        <VBFixedString(4)> Public ZG6 As String  '�戵��
        <VBFixedString(4)> Public ZG7 As String  '�d����s����
        <VBFixedString(15)> Public ZG8 As String  '�d����s��
        <VBFixedString(3)> Public ZG9 As String  '�d���x�X����
        <VBFixedString(15)> Public ZG10 As String '�d���x�X��
        <VBFixedString(1)> Public ZG11 As String  '�a�����
        <VBFixedString(7)> Public ZG12 As String  '�����ԍ�
        <VBFixedString(17)> Public ZG13 As String '�_�~�[

        Public ReadOnly Property Data() As String
            Get
                Return String.Concat(ZG1, _
                                     ZG2, _
                                     ZG3, _
                                     ZG4, _
                                     ZG5.Trim.PadRight(40, " "c), _
                                     ZG6, _
                                     ZG7, _
                                     ZG8.Trim.PadRight(15, " "c), _
                                     ZG9, _
                                     ZG10.Trim.PadRight(15, " "c), _
                                     ZG11, _
                                     ZG12, _
                                     ZG13.Trim.PadRight(17, " "c))
            End Get
        End Property
    End Structure
    Public gZENGIN_REC1 As gstrZG_Record1
    '�f�[�^���R�[�h
    Structure gstrZG_Record2
        <VBFixedString(1)> Public ZG1 As String  '�f�[�^�敪(=2)
        <VBFixedString(4)> Public ZG2 As String  '��d����s�ԍ�
        <VBFixedString(15)> Public ZG3 As String  '��d����s���@
        <VBFixedString(3)> Public ZG4 As String  '��d���x�X�ԍ�
        <VBFixedString(15)> Public ZG5 As String  '��d���x�X��
        <VBFixedString(4)> Public ZG6 As String  '��`�������ԍ�
        <VBFixedString(1)> Public ZG7 As String  '�a�����
        <VBFixedString(7)> Public ZG8 As String  '�����ԍ�
        <VBFixedString(30)> Public ZG9 As String  '���l
        <VBFixedString(10)> Public ZG10 As String '�U�����z
        <VBFixedString(1)> Public ZG11 As String  '�V�K�R�[�h
        <VBFixedString(10)> Public ZG12 As String '�ڋq�R�[�h�P
        <VBFixedString(10)> Public ZG13 As String '�ڋq�R�[�h�Q
        <VBFixedString(1)> Public ZG14 As String  '�U���w��敪
        <VBFixedString(8)> Public ZG15 As String  '�_�~�[

        Public Property Data() As String
            Get
                Return String.Concat(ZG1, _
                                     ZG2, _
                                     ZG3.Trim.PadRight(15, " "c), _
                                     ZG4, _
                                     ZG5.Trim.PadRight(15, " "c), _
                                     ZG6, _
                                     ZG7, _
                                     ZG8, _
                                     ZG9.Trim.PadRight(30, " "c), _
                                     ZG10, _
                                     ZG11, _
                                     ZG12.Trim.PadRight(10, " "c), _
                                     ZG13.Trim.PadRight(10, " "c), _
                                     ZG14, _
                                     ZG15.Trim.PadRight(8, " "c))
            End Get
            Set(ByVal Value As String)
                ZG1 = Mid(Value, 1, 1)
                ZG2 = Mid(Value, 2, 4)
                ZG3 = Mid(Value, 6, 15)
                ZG4 = Mid(Value, 21, 3)
                ZG5 = Mid(Value, 24, 15)
                ZG6 = Mid(Value, 39, 4)
                ZG7 = Mid(Value, 43, 1)
                ZG8 = Mid(Value, 44, 7)
                ZG9 = Mid(Value, 51, 30)
                ZG10 = Mid(Value, 81, 10)
                ZG11 = Mid(Value, 91, 1)
                ZG12 = Mid(Value, 92, 10)
                ZG13 = Mid(Value, 102, 10)
                ZG14 = Mid(Value, 112, 1)
                ZG15 = Mid(Value, 113, 8)
            End Set
        End Property
    End Structure
    Public gZENGIN_REC2 As gstrZG_Record2
    '�g���[�����R�[�h
    Structure gstrZG_Record8
        <VBFixedString(1)> Public ZG1 As String  '�f�[�^�敪(=8)
        <VBFixedString(6)> Public ZG2 As String  '���v����
        <VBFixedString(12)> Public ZG3 As String  '���v���z
        <VBFixedString(6)> Public ZG4 As String  '�U�֍ό���
        <VBFixedString(12)> Public ZG5 As String  '�U�֍ϋ��z
        <VBFixedString(6)> Public ZG6 As String  '�U�֕s�\����
        <VBFixedString(12)> Public ZG7 As String  '�U�֕s�\���z
        <VBFixedString(65)> Public ZG8 As String  '�_�~�[

        Public ReadOnly Property Data() As String
            Get
                Return String.Concat(ZG1, _
                                     ZG2, _
                                     ZG3, _
                                     ZG4, _
                                     ZG5, _
                                     ZG6, _
                                     ZG7, _
                                     ZG8.Trim.PadRight(65, " "c))
            End Get
        End Property
    End Structure
    Public gZENGIN_REC8 As gstrZG_Record8
    '�G���h���R�[�h
    Structure gstrZG_Record9
        <VBFixedString(1)> Public ZG1 As String  '�f�[�^�敪(=9)
        <VBFixedString(119)> Public ZG2 As String '�_�~�[��

        Public ReadOnly Property Data() As String
            Get
                Return String.Concat(ZG1, _
                                     ZG2.Trim.PadRight(119, " "c))
            End Get
        End Property
    End Structure
    Public gZENGIN_REC9 As gstrZG_Record9

#End Region

    'Public Sub Main()
    '    '*****************************************
    '    '�N������
    '    '*****************************************
    '    If UBound(Diagnostics.Process.GetProcessesByName( _
    '      Diagnostics.Process.GetCurrentProcess.ProcessName)) > 0 Then
    '        End
    '    End If

    '    'fskj.ini���݃`�F�b�N
    '    If Not CheckIniFile() Then
    '        End
    '    End If

    '    GCom.GetUserID = Microsoft.VisualBasic.Command()
    '    GCom.GetSysDate = Date.Now

    '    GCom.GetLogFolder = STR_LOG_PATH
    '    GCom.GetTXTFolder = STR_TXT_PATH
    '    GCom.GetLSTFolder = STR_LST_PATH
    '    '******************************
    '    '�N����ʕ\��
    '    '****************************** 
    '    Dim StartUpForm As New KFGMENU010

    '    '���j���[��ʂ���
    '    StartUpForm.StartPosition = FormStartPosition.CenterScreen
    '    System.Windows.Forms.Application.Run(StartUpForm)

    'End Sub

#Region " INI̧�ٓ��l�擾�֐�"
    Public Function GFUNC_INI_READ(ByVal pApName As String, ByVal pKeyName As String) As String
        Dim ret As String = CASTCommon.GetFSKJIni("G" & pApName, pKeyName).Trim

        If ret.ToUpper = "ERR" Then
            Return ""
        Else
            Return ret
        End If

    End Function
    Public Function CheckIniFile() As Boolean

        If System.IO.File.Exists(Application.StartupPath & "\fskj.ini") = False Then
            MessageBox.Show(Application.StartupPath & "\fskj.ini�����o�^�B�V�X�e�����~���܂��B", "Main", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Select Case ""
            Case STR_JIFURI_CHECK
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[JIFURI, CHECK]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_JIFURI_FUNOU
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[JIFURI, FUNOU]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_JIFURI_HAISIN
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[JIFURI, HAISIN]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_JIFURI_KAISYU
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[JIFURI, KAISYU]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_JIKINKO_CODE
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, KINKOCD]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_TXT_PATH
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, TXT]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_DAT_PATH
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, DAT]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_LST_PATH
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, LST]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_LOG_PATH
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, LOG]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_CSV_PATH
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, CSV]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_IFL_PATH
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, IRAIFL]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_TAKIFL_PATH
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, TAKIRAIFL]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case STR_CHK_DJT
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���[Common, CHKDJT]", "Main", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
        End Select

        Return True

    End Function
#End Region
#Region " �e�L�X�gFILE����R���{�{�b�N�X�ݒ�֐�"
    Public Function GFUNC_INDEX_GET_COMBO_CODE(ByVal pComboIndex As Integer, _
                          ByRef pDbCombo As ComboBox) As String
        '*****************************************
        'Combo�Ŏg�p���Ă���SelectedIndex��n���ăR���{�{�b�N�X���̃R�[�h�����擾
        '*****************************************
        Dim sSplit() As String

        GFUNC_INDEX_GET_COMBO_CODE = ""

        If pComboIndex < 0 Then
            Exit Function
        End If

        If pDbCombo.Items.Count = 0 Then
            Exit Function
        End If

        Select Case pDbCombo.Items.Count
            Case Is >= (pComboIndex - 1)

            Case Else
                Exit Function
        End Select

        pDbCombo.SelectedIndex = pComboIndex

        sSplit = Split(pDbCombo.SelectedItem.ToString, "�F")

        GFUNC_INDEX_GET_COMBO_CODE = sSplit(0)

    End Function
    Public Function GFUNC_INDEX_GET_COMBO_NAME(ByVal pComboIndex As Integer, _
                          ByRef pDbCombo As ComboBox) As String
        '*****************************************
        'Combo�Ŏg�p���Ă���SelectedIndex��n���ăR���{�{�b�N�X���̖��̕����擾
        '*****************************************
        Dim sSplit() As String

        GFUNC_INDEX_GET_COMBO_NAME = ""

        If pComboIndex < 0 Then
            Exit Function
        End If

        If pDbCombo.Items.Count = 0 Then
            Exit Function
        End If

        Select Case pDbCombo.Items.Count
            Case Is >= (pComboIndex - 1)

            Case Else
                Exit Function
        End Select

        pDbCombo.SelectedIndex = pComboIndex
        sSplit = Split(pDbCombo.SelectedItem.ToString, "�F")

        GFUNC_INDEX_GET_COMBO_NAME = sSplit(1)

    End Function
    Public Function GFUNC_CODE_GET_COMBO_INDEX(ByVal pCode As String, _
                          ByRef pDbCombo As ComboBox) As Integer
        '*****************************************
        'DB�Ŏg�p���Ă���CODE��n���ăR���{�{�b�N�X��INDEX���擾
        '*****************************************
        Dim iIndex As Integer
        Dim sSplit() As String

        GFUNC_CODE_GET_COMBO_INDEX = -1

        If Trim(pCode) = "" Then
            Exit Function
        End If

        If pDbCombo.Items.Count = 0 Then
            Exit Function
        End If

        For iIndex = 0 To pDbCombo.Items.Count - 1
            pDbCombo.SelectedIndex = iIndex

            sSplit = Split(pDbCombo.SelectedItem.ToString, "�F")
            Select Case Mid(pDbCombo.SelectedItem.ToString, 1, Len(sSplit(0)))
                Case pCode
                    GFUNC_CODE_GET_COMBO_INDEX = iIndex
                    Exit For
                Case Else

            End Select
        Next iIndex

    End Function
    Public Function GFUNC_CODE_GET_COMBO_NAME(ByVal pCode As String, _
                         ByRef pDbCombo As ComboBox) As String
        '*****************************************
        'DB�Ŏg�p���Ă���CODE��n���ăR���{�{�b�N�X���̖��̕����擾
        '*****************************************
        Dim iIndex As Integer
        Dim sSplit() As String

        GFUNC_CODE_GET_COMBO_NAME = ""

        If Trim(pCode) = "" Then
            Exit Function
        End If

        If pDbCombo.Items.Count = 0 Then
            Exit Function
        End If

        For iIndex = 0 To pDbCombo.Items.Count - 1
            pDbCombo.SelectedIndex = iIndex

            sSplit = Split(pDbCombo.SelectedItem.ToString, "�F")
            Select Case Mid(pDbCombo.SelectedItem.ToString, 1, Len(sSplit(0)))
                Case pCode
                    GFUNC_CODE_GET_COMBO_NAME = sSplit(1)
                    Exit For
                Case Else

            End Select
        Next iIndex

    End Function

    '�V�`���R���{�p
    Public Function GFUNC_CODE_TO_NAME(ByVal pTxtFile As String, _
                      ByVal pCode As String) As String
        '*****************************************
        '�e�L�X�gFILE����R���{�{�b�N�X�ݒ�
        '*****************************************
        Dim sSplit() As String

        GFUNC_CODE_TO_NAME = ""

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", _
                    "GFUNC_TXT_TO_DBCOMBO", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, _
                        Encoding.GetEncoding("Shift_JIS"))
        Dim Str_Line As String

        If Trim(pCode) = "" Then
            Exit Function
        End If

        'COMBOBOX ADD
        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                sSplit = Split(Str_Line, ",")
                Select Case UBound(sSplit)
                    Case Is >= 1
                        If Trim(sSplit(0)) = Trim(pCode) Then
                            GFUNC_CODE_TO_NAME = Trim(sSplit(1))
                        End If
                End Select
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

    End Function
    Public Function GFUNC_CODE_TO_INDEX(ByVal pTxtFile As String, _
                      ByVal pCode As String) As Integer
        '*****************************************
        'DB�Ŏg�p���Ă���CODE��n���ăR���{�{�b�N�X��INDEX���擾
        '*****************************************
        Dim iIndex As Integer
        Dim sSplit() As String

        GFUNC_CODE_TO_INDEX = -1

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", _
                    "GFUNC_TXT_TO_DBCOMBO", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, _
                        Encoding.GetEncoding("Shift_JIS"))
        Dim Str_Line As String

        If Trim(pCode) = "" Then
            Exit Function
        End If

        iIndex = -1
        'COMBOBOX ADD
        Do
            iIndex += 1
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                sSplit = Split(Str_Line, ",")
                Select Case UBound(sSplit)
                    Case Is >= 1
                        If Trim(sSplit(0)) = pCode Then
                            GFUNC_CODE_TO_INDEX = iIndex
                        End If
                End Select
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

    End Function
    Public Function GFUNC_NAME_TO_CODE(ByVal pTxtFile As String, _
                      ByVal pDbCombo As ComboBox) As String
        '*****************************************
        '�e�L�X�gFILE����R���{�{�b�N�X�ݒ�
        '*****************************************
        Dim sSplit() As String

        GFUNC_NAME_TO_CODE = ""

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", _
                    "GFUNC_TXT_TO_DBCOMBO", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, _
                        Encoding.GetEncoding("Shift_JIS"))
        Dim Str_Line As String

        If Trim(pDbCombo.Text) = "" Then
            Exit Function
        End If

        'COMBOBOX ADD
        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                sSplit = Split(Str_Line, ",")
                Select Case UBound(sSplit)
                    Case Is >= 1
                        If Trim(sSplit(1)) = pDbCombo.Text Then
                            GFUNC_NAME_TO_CODE = Trim(sSplit(0))
                        End If
                End Select
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

    End Function
    Public Function GFUNC_NAME_TO_CODE2(ByVal pTxtFile As String, _
                      ByVal pValue As String) As String
        '*****************************************
        '�e�L�X�gFILE����R���{�{�b�N�X�ݒ�
        '*****************************************
        Dim sSplit() As String

        GFUNC_NAME_TO_CODE2 = ""

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", _
                    "GFUNC_TXT_TO_DBCOMBO", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, _
                        Encoding.GetEncoding("Shift_JIS"))
        Dim Str_Line As String

        If Trim(pValue) = "" Then
            Exit Function
        End If

        'COMBOBOX ADD
        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                sSplit = Split(Str_Line, ",")
                Select Case UBound(sSplit)
                    Case Is >= 1
                        If Trim(sSplit(1)) = pValue Then
                            GFUNC_NAME_TO_CODE2 = Trim(sSplit(0))
                        End If
                End Select
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

    End Function


    Public Function GFUNC_TXT_TO_DBCOMBO(ByVal pTxtFile As String, _
                       ByRef pDbCombo As ComboBox) As Boolean
        '*****************************************
        '�e�L�X�gFILE����R���{�{�b�N�X�ݒ�
        '*****************************************
        Dim sSplit() As String

        GFUNC_TXT_TO_DBCOMBO = False

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", _
                    "GFUNC_TXT_TO_DBCOMBO", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, _
                        Encoding.GetEncoding("Shift_JIS"))
        Dim Str_Line As String

        pDbCombo.Items.Clear()

        'COMBOBOX ADD
        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                sSplit = Split(Str_Line, ",")
                Select Case UBound(sSplit)
                    Case 0
                        pDbCombo.Items.Add(Str_Line)
                    Case Is >= 1
                        pDbCombo.Items.Add(sSplit(1))
                End Select
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

        '�R���{�{�b�N�X�̐擪�Ɉʒu�Â�
        If pDbCombo.Items.Count <> 0 Then
            pDbCombo.SelectedIndex = 0
        End If

        GFUNC_TXT_TO_DBCOMBO = True
    End Function
    Public Function GFUNC_TXT_TO_DBCOMBO_TESUURYOU(ByVal pTxtFile As String, _
                            ByRef pDbCombo As ComboBox) As Boolean
        '*****************************************
        '�e�L�X�gFILE����R���{�{�b�N�X�ݒ�
        '*****************************************
        GFUNC_TXT_TO_DBCOMBO_TESUURYOU = False

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", _
                    "GFUNC_TXT_TO_DBCOMBO", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, _
                        Encoding.GetEncoding("Shift_JIS"))
        Dim Str_Line As String
        Dim Str_TESUURYOU() As String

        'COMBOBOX ADD
        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Str_TESUURYOU = Str_Line.Split(","c)
                pDbCombo.Items.Add(Str_TESUURYOU(0) & "�F" & Str_TESUURYOU(1))
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

        '�R���{�{�b�N�X�̐擪�Ɉʒu�Â�
        If pDbCombo.Items.Count <> 0 Then
            pDbCombo.SelectedIndex = 0
        End If

        GFUNC_TXT_TO_DBCOMBO_TESUURYOU = True
    End Function
    Public Function GFUNC_TXT_TO_DBCOMBO_KESSAI(ByVal pTxtFile As String, _
                          ByRef pDbCombo As ComboBox) As Boolean
        '*****************************************
        '�e�L�X�gFILE����R���{�{�b�N�X�ݒ�
        '*****************************************
        GFUNC_TXT_TO_DBCOMBO_KESSAI = False

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", _
                    "GFUNC_TXT_TO_DBCOMBO", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, _
                        Encoding.GetEncoding("Shift_JIS"))
        Dim Str_Line As String
        Dim Str_Kessai() As String

        'COMBOBOX ADD
        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Str_Kessai = Split(Str_Line, ",")
                Select Case UBound(Str_Kessai)
                    Case 0
                        pDbCombo.Items.Add(Str_Line)
                    Case Is >= 1
                        pDbCombo.Items.Add(Str_Kessai(1))
                End Select
            End If

        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

        '�R���{�{�b�N�X�̐擪�Ɉʒu�Â�
        If pDbCombo.Items.Count <> 0 Then
            pDbCombo.SelectedIndex = 0
        End If

        GFUNC_TXT_TO_DBCOMBO_KESSAI = True

    End Function


#End Region
#Region " MessageBox�ďo���ʊ֐�"
    Public Sub GSUB_MESSAGE_WARNING(ByVal pMESSAGE As String)
        '*****************************************
        '���b�Z�[�W�\���i�G���[�j
        '*****************************************
        MessageBox.Show(pMESSAGE, _
                STR_SYORI_NAME, _
                MessageBoxButtons.OK, _
                MessageBoxIcon.Warning)
    End Sub

    Public Sub GSUB_MESSAGE_INFOMATION(ByVal pMESSAGE As String)
        '*****************************************
        '���b�Z�[�W�\���i���j
        '*****************************************
        MessageBox.Show(pMESSAGE, _
                STR_SYORI_NAME, _
                MessageBoxButtons.OK, _
                MessageBoxIcon.Information)
    End Sub

    Public Function GFUNC_MESSAGE_QUESTION(ByVal pMESSAGE As String) As Integer
        '*****************************************
        '���b�Z�[�W�\���i�m�F�j
        '*****************************************
        GFUNC_MESSAGE_QUESTION = MessageBox.Show(pMESSAGE, _
                             STR_SYORI_NAME, _
                             MessageBoxButtons.OKCancel, _
                             MessageBoxIcon.Question)
    End Function
    Public Function GFUNC_MESSAGE_YESNO(ByVal pMESSAGE As String) As Integer
        '*****************************************
        '���b�Z�[�W�\���i�m�F�j
        '*****************************************
        GFUNC_MESSAGE_YESNO = MessageBox.Show(pMESSAGE, _
                             STR_SYORI_NAME, _
                             MessageBoxButtons.YesNo, _
                             MessageBoxIcon.Question)
    End Function
#End Region
#Region " OracleDB����֐�"
    Public Function GFUNC_EXECUTESQL_TRANS(ByVal p_Sql As String, ByVal p_Index As Integer) As Boolean
        '********************************************
        '�f�[�^����(�g�����U�N�V����)
        '********************************************
        GFUNC_EXECUTESQL_TRANS = True

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.GFUNC_EXECUTESQL_TRANS", "p_Index=" & p_Index)
            If MainLog.IS_LEVEL4() = True Then
                MainLog.Write_LEVEL4("Common.GFUNC_EXECUTESQL_TRANS", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            Select Case p_Index
                Case 0
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "BeginTransaction")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '�g�����U�N�V���������J�n(BeginTrans)
                    OBJ_TRANSACTION = OBJ_CONNECTION.BeginTransaction
                Case 1
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "ExecuteNonQuery")
                    End If
                    If MainLog.IS_SQLLOG() = True Then
                        MainLog.Write_SQL("Common.GFUNC_EXECUTESQL_TRANS", p_Sql)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '�g�����U�N�V�����������f�[�^����
                    OBJ_COMMAND = New Data.OracleClient.OracleCommand
                    OBJ_COMMAND.Connection = OBJ_CONNECTION
                    OBJ_COMMAND.Transaction = OBJ_TRANSACTION
                    OBJ_COMMAND.CommandText = p_Sql

                    OBJ_COMMAND.ExecuteNonQuery()
                Case 2
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "Commit")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '�g�����U�N�V���������I��(CommitTrans)
                    '*** Str Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***
                    'OBJ_TRANSACTION.Commit()
                    If Not OBJ_TRANSACTION Is Nothing Then
                        OBJ_TRANSACTION.Commit()
                        OBJ_TRANSACTION = Nothing
                    End If
                    '*** End Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***

                Case 3
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "Rollback")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '�蓮RollBack
                    '*** Str Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***
                    'OBJ_TRANSACTION.Rollback()
                    If Not OBJ_TRANSACTION Is Nothing Then
                        OBJ_TRANSACTION.Rollback()
                        OBJ_TRANSACTION = Nothing
                    End If
                    '*** End Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***

                Case 4
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "ExecuteNonQuery")
                    End If
                    If MainLog.IS_SQLLOG() = True Then
                        MainLog.Write_SQL("Common.GFUNC_EXECUTESQL_TRANS", p_Sql)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '���z�G���[
                    OBJ_COMMAND = New Data.OracleClient.OracleCommand
                    OBJ_COMMAND.Connection = OBJ_CONNECTION
                    OBJ_COMMAND.Transaction = OBJ_TRANSACTION
                    OBJ_COMMAND.CommandText = p_Sql
                    OBJ_COMMAND.ExecuteNonQuery()

                    GFUNC_EXECUTESQL_TRANS = False

                    OBJ_TRANSACTION.Rollback()
                Case 5
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "ExecuteReader")
                    End If
                    If MainLog.IS_SQLLOG() = True Then
                        MainLog.Write_SQL("Common.GFUNC_EXECUTESQL_TRANS", p_Sql)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    OBJ_COMMAND = New Data.OracleClient.OracleCommand
                    OBJ_COMMAND.Connection = OBJ_CONNECTION
                    OBJ_COMMAND.Transaction = OBJ_TRANSACTION
                    OBJ_COMMAND.CommandText = p_Sql

                    OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
                    OBJ_DATAREADER.Read()

                    If OBJ_DATAREADER.HasRows = False Then

                        GFUNC_EXECUTESQL_TRANS = False
                    End If
                    OBJ_DATAREADER.Close()
            End Select

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.GFUNC_EXECUTESQL_TRANS")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch ex As Exception

            GFUNC_EXECUTESQL_TRANS = False

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("Common.GFUNC_EXECUTESQL_TRANS", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            MessageBox.Show(ex.Message, _
                    "GFUNC_EXECUTESQL_TRANS", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Warning)

            '�g�����U�N�V����������COMMIT���̴װ��RollBack���Ȃ���
            Select Case p_Index
                Case 0, 2, 5
                Case Else
                    '*** Str Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***
                    'OBJ_TRANSACTION.Rollback()
                    If Not OBJ_TRANSACTION Is Nothing Then
                        OBJ_TRANSACTION.Rollback()
                        OBJ_TRANSACTION = Nothing
                    End If
                    '*** End Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***
            End Select
        End Try

    End Function
    Public Function GFUNC_EXECUTE(ByVal pStr_Sql As String) As Boolean
        '********************************************
        'SQL�̎��s
        '********************************************
        GFUNC_EXECUTE = False

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.GFUNC_EXECUTE")
            If MainLog.IS_LEVEL4() = True Then
                MainLog.Write_LEVEL4("Common.GFUNC_EXECUTE", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        If MainLog.IS_SQLLOG() = True Then
            MainLog.Write_SQL("Common.GFUNC_EXECUTE", pStr_Sql)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        'Oracle COMMAND CONECTION
        OBJ_COMMAND = New Data.OracleClient.OracleCommand(pStr_Sql)
        OBJ_COMMAND.Connection = OBJ_CONNECTION

        Try
            OBJ_COMMAND.ExecuteNonQuery()

            GFUNC_EXECUTE = True

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.GFUNC_EXECUTE")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch e As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("Common.GFUNC_EXECUTE", e)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            MessageBox.Show(e.Message, _
                    "GFUNC_EXECUTE", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
        End Try

        OBJ_COMMAND.Dispose()

    End Function

    Public Function GFUNC_SELECT_SQL2(ByVal p_Sql As String, ByVal pIndex As Integer) As Boolean

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.GFUNC_SELECT_SQL2", "pIndex=" & pIndex)
            If MainLog.IS_LEVEL4() = True Then
                MainLog.Write_LEVEL4("Common.GFUNC_SELECT_SQL2", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        GFUNC_SELECT_SQL2 = False
        '*****************************************
        'Oracle Conection
        '*****************************************
        Try
            Select Case pIndex
                Case 0
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL2", "ExecuteReader")
                    End If
                    If MainLog.IS_SQLLOG() = True Then
                        MainLog.Write_SQL("Common.GFUNC_SELECT_SQL2", p_Sql)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE OPEN
                    OBJ_COMMAND = New Data.OracleClient.OracleCommand(p_Sql)
                    OBJ_COMMAND.Connection = OBJ_CONNECTION
                    'OBJ_COMMAND.CommandText = STR_SQL
                    OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
                Case 1
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL2", "Close")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE CLOSE
                    OBJ_DATAREADER.Close()
                    OBJ_COMMAND.Dispose()

            End Select

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.GFUNC_SELECT_SQL2")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch e As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("Common.GFUNC_SELECT_SQL2", e)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            MessageBox.Show(e.Message, _
                    "GFUNC_SELECT_SQL2", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End Try

        GFUNC_SELECT_SQL2 = True

    End Function
    Public Function GFUNC_SELECT_SQL3(ByVal p_Sql As String, ByVal pIndex As Integer) As Boolean

        GFUNC_SELECT_SQL3 = False

        '������
        '�������A���O��OracleConnection�ڑ���K�v�Ƃ���
        '�ڑ��̃^�C�~���O�͎g�p�����ʂ̃t�H�[�����[�h��
        '�ؒf�̃^�C�~���O�͎g�p�����ʂ̃t�H�[���A�����[�h��
        'MAIN�ōŏ��ɐڑ����Ă��܂��Ƃ��̉�ʂ��g�p���Ȃ��Ă�����N�����ɐڑ�����̂�
        '�K�v�Ƃ��鎞�݂̂̐ڑ��E�ؒf�̂ق����������x�̌y���ɂȂ��

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.GFUNC_SELECT_SQL3", "pIndex=" & pIndex)
            If MainLog.IS_LEVEL4() = True Then
                MainLog.Write_LEVEL4("Common.GFUNC_SELECT_SQL3", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            Select Case pIndex
                Case 0
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL3", "ExecuteReader")
                    End If
                    If MainLog.IS_SQLLOG() = True Then
                        MainLog.Write_SQL("Common.GFUNC_SELECT_SQL3", p_Sql)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE OPEN
                    OBJ_COMMAND_DREAD = New Data.OracleClient.OracleCommand(p_Sql)
                    OBJ_COMMAND_DREAD.Connection = OBJ_CONNECTION_DREAD
                    'OBJ_COMMAND.CommandText = STR_SQL
                    OBJ_DATAREADER_DREAD = OBJ_COMMAND_DREAD.ExecuteReader()
                Case 1
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL3", "Close")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE CLOSE
                    OBJ_DATAREADER_DREAD.Close()
                    OBJ_COMMAND_DREAD.Dispose()
            End Select

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.GFUNC_SELECT_SQL3")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch e As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("Common.GFUNC_SELECT_SQL3", e)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            MessageBox.Show(e.Message, _
                    "GFUNC_SELECT_SQL3", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End Try

        GFUNC_SELECT_SQL3 = True

    End Function
    Public Function GFUNC_SELECT_SQL4(ByVal p_Sql As String, ByVal pIndex As Integer) As Boolean

        '������
        '�������A���O��OracleConnection�ڑ���K�v�Ƃ���
        '�ڑ��̃^�C�~���O�͎g�p�����ʂ̃t�H�[�����[�h��
        '�ؒf�̃^�C�~���O�͎g�p�����ʂ̃t�H�[���A�����[�h��
        'MAIN�ōŏ��ɐڑ����Ă��܂��Ƃ��̉�ʂ��g�p���Ȃ��Ă�����N�����ɐڑ�����̂�
        '�K�v�Ƃ��鎞�݂̂̐ڑ��E�ؒf�̂ق����������x�̌y���ɂȂ��

        GFUNC_SELECT_SQL4 = False

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.GFUNC_SELECT_SQL4", "pIndex=" & pIndex)
            If MainLog.IS_LEVEL4() = True Then
                MainLog.Write_LEVEL4("Common.GFUNC_SELECT_SQL4", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            Select Case pIndex
                Case 0
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL4", "ExecuteReader")
                    End If
                    If MainLog.IS_SQLLOG() = True Then
                        MainLog.Write_SQL("Common.GFUNC_SELECT_SQL4", p_Sql)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE OPEN
                    OBJ_COMMAND_DREAD2 = New Data.OracleClient.OracleCommand(p_Sql)
                    OBJ_COMMAND_DREAD2.Connection = OBJ_CONNECTION_DREAD2
                    'OBJ_COMMAND.CommandText = STR_SQL
                    OBJ_DATAREADER_DREAD2 = OBJ_COMMAND_DREAD2.ExecuteReader()
                Case 1
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL4", "Close")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE CLOSE
                    OBJ_DATAREADER_DREAD2.Close()
                    OBJ_COMMAND_DREAD2.Dispose()
            End Select

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.GFUNC_SELECT_SQL4")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch e As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("Common.GFUNC_SELECT_SQL4", e)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            MessageBox.Show(e.Message, _
                    "GFUNC_SELECT_SQL4", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End Try

        GFUNC_SELECT_SQL4 = True

    End Function
    Public Function GFUNC_SELECT_SQL5(ByVal p_Sql As String, ByVal pIndex As Integer) As Boolean

        '������
        '�������A���O��OracleConnection�ڑ���K�v�Ƃ���
        '�ڑ��̃^�C�~���O�͎g�p�����ʂ̃t�H�[�����[�h��
        '�ؒf�̃^�C�~���O�͎g�p�����ʂ̃t�H�[���A�����[�h��
        'MAIN�ōŏ��ɐڑ����Ă��܂��Ƃ��̉�ʂ��g�p���Ȃ��Ă�����N�����ɐڑ�����̂�
        '�K�v�Ƃ��鎞�݂̂̐ڑ��E�ؒf�̂ق����������x�̌y���ɂȂ��

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.GFUNC_SELECT_SQL5", "pIndex=" & pIndex)
            If MainLog.IS_LEVEL4() = True Then
                MainLog.Write_LEVEL4("Common.GFUNC_SELECT_SQL5", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        GFUNC_SELECT_SQL5 = False

        Try
            Select Case pIndex
                Case 0
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL5", "ExecuteReader")
                    End If
                    If MainLog.IS_SQLLOG() = True Then
                        MainLog.Write_SQL("Common.GFUNC_SELECT_SQL5", p_Sql)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE OPEN
                    OBJ_COMMAND_DREAD3 = New Data.OracleClient.OracleCommand(p_Sql)
                    OBJ_COMMAND_DREAD3.Connection = OBJ_CONNECTION_DREAD3
                    'OBJ_COMMAND.CommandText = STR_SQL
                    OBJ_DATAREADER_DREAD3 = OBJ_COMMAND_DREAD3.ExecuteReader()
                Case 1
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If MainLog.IS_LEVEL3() = True Then
                        MainLog.Write_LEVEL3("clsCommon.GFUNC_SELECT_SQL5", "Close")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'TABLE CLOSE
                    OBJ_DATAREADER_DREAD3.Close()
                    OBJ_COMMAND_DREAD3.Dispose()
            End Select

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.GFUNC_SELECT_SQL5")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch e As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("Common.GFUNC_SELECT_SQL5", e)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            MessageBox.Show(e.Message, _
                    "GFUNC_SELECT_SQL5", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
            Exit Function
        End Try

        GFUNC_SELECT_SQL5 = True

    End Function

    Public Function GFUNC_ISEXIST(ByVal pStr_Sql As String) As Boolean
        '********************************************
        '���R�[�h���݃`�F�b�N
        '********************************************
        GFUNC_ISEXIST = False

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.GFUNC_ISEXIST")
            If MainLog.IS_LEVEL4() = True Then
                MainLog.Write_LEVEL4("Common.GFUNC_ISEXIST", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        If MainLog.IS_SQLLOG() = True Then
            MainLog.Write_SQL("Common.GFUNC_ISEXIST", pStr_Sql)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        'Oracle COMMAND CONECTION
        OBJ_COMMAND = New Data.OracleClient.OracleCommand(pStr_Sql)
        OBJ_COMMAND.Connection = OBJ_CONNECTION

        Try
            OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
            OBJ_DATAREADER.Read()
            If OBJ_DATAREADER.HasRows = True Then

                GFUNC_ISEXIST = True
            End If

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.GFUNC_ISEXIST")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch e As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("Common.GFUNC_ISEXIST", e)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            MessageBox.Show(e.Message, _
                    "GFUNC_ISEXIST", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Error)
        End Try

        OBJ_DATAREADER.Close()
        OBJ_COMMAND.Dispose()

    End Function

    Public Function GFUNC_GET_SELECTSQL_ITEM(ByVal p_Sql As String, ByVal pItemName As String) As String

        GFUNC_GET_SELECTSQL_ITEM = ""

        If GFUNC_SELECT_SQL2(p_Sql, 0) = False Then
            Exit Function
        End If

        With OBJ_DATAREADER
            .Read()

            If .HasRows = False Then
                If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                    Exit Function
                End If
                Exit Function
            End If

            GFUNC_GET_SELECTSQL_ITEM = .Item(pItemName).ToString
        End With

        If GFUNC_SELECT_SQL2(p_Sql, 1) = False Then
            Exit Function
        End If

    End Function

    '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    'KFGMENU010�������ڐA
    '�e��ʂ̃��[�h��ʂŌĂяo���悤�ɂ���
    Public Sub GSUB_CONNECT()
        If Not OBJ_CONNECTION Is Nothing AndAlso OBJ_CONNECTION.State = ConnectionState.Open Then
        Else
            Try
                'Oracle �ڑ�
                OBJ_CONNECTION = New Data.OracleClient.OracleConnection(STR_CONNECTION)
                'Oracle OPEN
                OBJ_CONNECTION.Open()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "main", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End
            End Try
        End If
    End Sub
    '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    '�e��ʂ̃I���N���N���[�Y�p����
    Public Sub GSUB_CLOSE()
        Try
            If Not OBJ_CONNECTION Is Nothing AndAlso OBJ_CONNECTION.State = ConnectionState.Open Then
                OBJ_CONNECTION.Close()
                OBJ_CONNECTION = Nothing
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "main", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End Try
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#End Region
#Region "�_�C�A���O�ďo�֐�"
    Public Function GFUNC_FD_Copy(ByVal pForm As Form, ByVal pTitleName As String, ByVal pSouceFilePath As String, ByVal pInitialFileName As String, ByVal pBaitai As String, Optional ByVal pGinko_CD As String = "") As Integer

        Dim ret As Integer = -1

        Try
            Dim sPath As String = ""
            Dim sBuff As String = ""

            '�U�փf�[�^�ۑ���p�X
            If pBaitai = "0" Then
                If pGinko_CD = "" Then
                    sPath = STR_IFL_PATH
                Else
                    sPath = STR_TAKIFL_PATH
                End If

                If sPath = "" Then
                    If pGinko_CD = "" Then
                        Call GSUB_MESSAGE_WARNING("ini�t�@�C����IRAIFL�i�[���񂪐ݒ肳��Ă��܂���")
                    Else
                        Call GSUB_MESSAGE_WARNING("ini�t�@�C����TAKIRAIFL�i�[���񂪐ݒ肳��Ă��܂���")
                    End If
                    Exit Try
                End If
            Else
                sPath = "A:\"
            End If

            Select Case StrConv(Mid(sPath, 1, 1), vbProperCase)
                Case "A", "B"
                    '�m�F���b�Z�[�W�\��
                    If GFUNC_MESSAGE_QUESTION("�e�c���Z�b�g���ĉ������B") <> vbOK Then
                        ret = 1
                        Exit Try
                    End If
            End Select

            Dim oDlg As New SaveFileDialog
            oDlg.Filter = "���ׂẴt�@�C�� (*.*)|*.*"
            oDlg.FilterIndex = 1
            oDlg.InitialDirectory = sPath

            If Trim(pInitialFileName) <> "" Then
                oDlg.FileName = pInitialFileName
            End If
            oDlg.Title = "[" & pTitleName.Trim & ":" & pGinko_CD & "]�U�փf�[�^�ۑ�"

            If oDlg.ShowDialog = DialogResult.Cancel Then '�L�����Z���̂Ƃ�
                ret = 1
                Exit Try
            End If
            sBuff = oDlg.FileName

            If sBuff = pInitialFileName Then
                ret = 1
                Exit Try
            End If

            If Dir(sBuff, vbNormal) <> "" Then Kill(sBuff)

            FileCopy(pSouceFilePath, sBuff)

            ret = 0

        Catch ex As Exception
            ret = -1
        End Try

        Return ret

    End Function
#End Region
    '*** Str Add 2016/01/05 sys)mori for V2�p�C�� ***
    '#Region " ���[����p���ʊ֐�"
    '    Public Sub GSUB_PRINT_CRYSTALREPORT(ByVal pReportFileName As String, ByVal pJyoken As String)

    '        Try
    '            'Dim CRXApplication As New CRAXDDRT.Application
    '            'Dim CRXReport As CRAXDDRT.Report
    '            'Dim CPProperty As CRAXDDRT.ConnectionProperty
    '            'Dim DBTable As CRAXDDRT.DatabaseTable

    '            'CRXReport = CRXApplication.OpenReport(STR_LST_PATH & pReportFileName, 1)

    '            'DBTable = CRXReport.Database.Tables(1)

    '            'CPProperty = DBTable.ConnectionProperties("Password")
    '            'CPProperty.Value = "KZFMAST"
    '            'CRXReport.RecordSelectionFormula = pJyoken

    '            'CRXReport.PrintOut(False, 1)

    '        Catch ex As Exception
    '            Call GSUB_LOG(0, "���[������s�F" & pReportFileName & "," & ex.ToString)
    '            Call GSUB_MESSAGE_WARNING(pReportFileName & "�̈���Ɏ��s���܂���")
    '        End Try


    '    End Sub
    '    Public Sub GSUB_PRINT_CRYSTALREPORT2(ByVal pReportFileName As String, ByVal pQuery As String)

    '        Try
    '            'Dim CRXApplication As New CRAXDDRT.Application
    '            'Dim CRXReport As CRAXDDRT.Report
    '            'Dim CPProperty As CRAXDDRT.ConnectionProperty
    '            'Dim DBTable As CRAXDDRT.DatabaseTable

    '            'CRXReport = CRXApplication.OpenReport(STR_LST_PATH & pReportFileName, 1)

    '            'DBTable = CRXReport.Database.Tables(1)

    '            'CPProperty = DBTable.ConnectionProperties("Password")
    '            'CPProperty.Value = "KZFMAST"
    '            'CRXReport.SQLQueryString = pQuery

    '            'CRXReport.PrintOut(False, 1)

    '        Catch ex As Exception
    '            Call GSUB_LOG(0, "���[������s�F" & pReportFileName & "," & ex.ToString)
    '            Call GSUB_MESSAGE_WARNING(pReportFileName & "�̈���Ɏ��s���܂���")
    '        End Try

    '    End Sub
    '#End Region
    '*** End Add 2016/01/05 sys)mori for V2�p�C�� ***


    Public Sub GSUB_LOG(ByVal pIndex As Integer, ByVal pLog As String)
        Select Case pIndex
            Case 0
                '���s�����O
                Call GSUB_LOG_OUT(STR_LOG_GAKKOU_CODE, STR_LOG_FURI_DATE, STR_SYORI_NAME, STR_COMMAND, "���s", pLog)
            Case 1
                '�������O
                Call GSUB_LOG_OUT(STR_LOG_GAKKOU_CODE, STR_LOG_FURI_DATE, STR_SYORI_NAME, STR_COMMAND, "����", pLog)
        End Select
    End Sub
    Public Sub GSUB_LOG_OUT(ByVal strGAKKOU_CODE As String, ByVal strFURI_DATE As String, ByVal strSYORI As String, ByVal strCOMMAND As String, ByVal strKEKKA As String, ByVal strERR As String)
        MainLog.Write(GCom.GetUserID, strGAKKOU_CODE, strFURI_DATE, strCOMMAND, strKEKKA, strERR)
    End Sub
    Public Function GFUNC_CHK_DEJIT(ByVal astrKINKO As String, ByVal astrSITEN As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String) As Boolean
        '============================================================================
        'NAME      :PFUNC_CHK_DEJIT
        'Parameter   :astrKINKO:���ɃR�[�h�@astrSITEN:�x�X�R�[�hastrKAMOKU�F�Ȗ�astrKOUZA�F�����ԍ�
        'Description  :�`�F�b�N�f�W�b�g
        'Return     :True=OK,False=NG
        'Create     :2007/02/12
        'UPDATE     :
        '============================================================================
        Dim IN_KINKO1, IN_KINKO2, IN_KINKO3, IN_KINKO4 As Integer
        Dim IN_SITEN1, IN_SITEN2, IN_SITEN3 As Integer
        Dim IN_KOUZA1, IN_KOUZA2, IN_KOUZA3, IN_KOUZA4, IN_KOUZA5, IN_KOUZA6, IN_KOUZA7 As Integer
        Dim CHG_KAMOKU As Integer
        Dim KINKO_OMOMI, TENPO_OMOMI, KAMOKU_OMOMI, KOUZA_OMOMI As String
        Dim KINKO_OMOMI_1, KINKO_OMOMI_2, KINKO_OMOMI_3, KINKO_OMOMI_4 As Integer
        Dim TENPO_OMOMI_1, TENPO_OMOMI_2, TENPO_OMOMI_3 As Integer
        Dim KAMOKU_OMOMI_1, KAMOKU_OMOMI_2 As Integer
        Dim KOUZA_OMOMI_1, KOUZA_OMOMI_2, KOUZA_OMOMI_3, KOUZA_OMOMI_4, KOUZA_OMOMI_5, KOUZA_OMOMI_6 As Integer
        Dim ATAI, ATAI_A, ATAI_B, ATAI1, ATAI2, ATAI3, ATAI4, ATAI5, ATAI6, ATAI7, ATAI8, ATAI9, ATAI10, ATAI11, ATAI12, ATAI13 As Integer

        GFUNC_CHK_DEJIT = False

        KINKO_OMOMI = "9874"
        TENPO_OMOMI = "732"
        KAMOKU_OMOMI = "19"
        KOUZA_OMOMI = "387432"

        KINKO_OMOMI_1 = CInt(KINKO_OMOMI.Substring(0, 1))        '���ɃR�[�h�d��
        KINKO_OMOMI_2 = CInt(KINKO_OMOMI.Substring(1, 1))
        KINKO_OMOMI_3 = CInt(KINKO_OMOMI.Substring(2, 1))
        KINKO_OMOMI_4 = CInt(KINKO_OMOMI.Substring(3, 1))

        TENPO_OMOMI_1 = CInt(TENPO_OMOMI.Substring(0, 1))           '�X�܃R�[�h�d��
        TENPO_OMOMI_2 = CInt(TENPO_OMOMI.Substring(1, 1))
        TENPO_OMOMI_3 = CInt(TENPO_OMOMI.Substring(2, 1))

        KAMOKU_OMOMI_1 = CInt(KAMOKU_OMOMI.Substring(0, 1))          '�ȖڃR�[�h�d��
        KAMOKU_OMOMI_2 = CInt(KAMOKU_OMOMI.Substring(1, 1))

        KOUZA_OMOMI_1 = CInt(KOUZA_OMOMI.Substring(0, 1))            '�����ԍ��d��
        KOUZA_OMOMI_2 = CInt(KOUZA_OMOMI.Substring(1, 1))
        KOUZA_OMOMI_3 = CInt(KOUZA_OMOMI.Substring(2, 1))
        KOUZA_OMOMI_4 = CInt(KOUZA_OMOMI.Substring(3, 1))
        KOUZA_OMOMI_5 = CInt(KOUZA_OMOMI.Substring(4, 1))
        KOUZA_OMOMI_6 = CInt(KOUZA_OMOMI.Substring(5, 1))

        IN_KINKO1 = CInt(astrKINKO.Substring(0, 1))             '���ɃR�[�h
        IN_KINKO2 = CInt(astrKINKO.Substring(1, 1))
        IN_KINKO3 = CInt(astrKINKO.Substring(2, 1))
        IN_KINKO4 = CInt(astrKINKO.Substring(3, 1))

        IN_SITEN1 = CInt(astrSITEN.Substring(0, 1))                '�x�X�R�[�h
        IN_SITEN2 = CInt(astrSITEN.Substring(1, 1))
        IN_SITEN3 = CInt(astrSITEN.Substring(2, 1))

        IN_KOUZA1 = CInt(astrKOUZA.Substring(0, 1))             '�����ԍ�
        IN_KOUZA2 = CInt(astrKOUZA.Substring(1, 1))
        IN_KOUZA3 = CInt(astrKOUZA.Substring(2, 1))
        IN_KOUZA4 = CInt(astrKOUZA.Substring(3, 1))
        IN_KOUZA5 = CInt(astrKOUZA.Substring(4, 1))
        IN_KOUZA6 = CInt(astrKOUZA.Substring(5, 1))
        IN_KOUZA7 = CInt(astrKOUZA.Substring(6, 1))

        ATAI1 = CInt(CStr(IN_KINKO1 * KINKO_OMOMI_1).Substring(CInt(CStr(IN_KINKO1 * KINKO_OMOMI_1).Length - 1), 1))
        ATAI2 = CInt(CStr(IN_KINKO2 * KINKO_OMOMI_2).Substring(CInt(CStr(IN_KINKO2 * KINKO_OMOMI_2).Length - 1), 1))
        ATAI3 = CInt(CStr(IN_KINKO3 * KINKO_OMOMI_3).Substring(CInt(CStr(IN_KINKO3 * KINKO_OMOMI_3).Length - 1), 1))
        ATAI4 = CInt(CStr(IN_KINKO4 * KINKO_OMOMI_4).Substring(CInt(CStr(IN_KINKO4 * KINKO_OMOMI_4).Length - 1), 1))
        ATAI5 = CInt(CStr(IN_SITEN1 * TENPO_OMOMI_1).Substring(CInt(CStr(IN_SITEN1 * TENPO_OMOMI_1).Length - 1), 1))
        ATAI6 = CInt(CStr(IN_SITEN2 * TENPO_OMOMI_2).Substring(CInt(CStr(IN_SITEN2 * TENPO_OMOMI_2).Length - 1), 1))
        ATAI7 = CInt(CStr(IN_SITEN3 * TENPO_OMOMI_3).Substring(CInt(CStr(IN_SITEN3 * TENPO_OMOMI_3).Length - 1), 1))
        ATAI8 = CInt(CStr(IN_KOUZA1 * KOUZA_OMOMI_1).Substring(CInt(CStr(IN_KOUZA1 * KOUZA_OMOMI_1).Length - 1), 1))
        ATAI9 = CInt(CStr(IN_KOUZA2 * KOUZA_OMOMI_2).Substring(CInt(CStr(IN_KOUZA2 * KOUZA_OMOMI_2).Length - 1), 1))
        ATAI10 = CInt(CStr(IN_KOUZA3 * KOUZA_OMOMI_3).Substring(CInt(CStr(IN_KOUZA3 * KOUZA_OMOMI_3).Length - 1), 1))
        ATAI11 = CInt(CStr(IN_KOUZA4 * KOUZA_OMOMI_4).Substring(CInt(CStr(IN_KOUZA4 * KOUZA_OMOMI_4).Length - 1), 1))
        ATAI12 = CInt(CStr(IN_KOUZA5 * KOUZA_OMOMI_5).Substring(CInt(CStr(IN_KOUZA5 * KOUZA_OMOMI_5).Length - 1), 1))
        ATAI13 = CInt(CStr(IN_KOUZA6 * KOUZA_OMOMI_6).Substring(CInt(CStr(IN_KOUZA6 * KOUZA_OMOMI_6).Length - 1), 1))


        Select Case CInt(astrKAMOKU)
            Case 2 '����
                CHG_KAMOKU = 9   '"01"*19
            Case 1 '����
                CHG_KAMOKU = 8   '"02"*19
            Case 3, 4
                CHG_KAMOKU = 8   '"02"*19
            Case Else  '�`�F�b�N�f�W�b�g�ΏۊO
                Exit Function
        End Select

        ATAI = ATAI1 + ATAI2 + ATAI3 + ATAI4 + ATAI5 + ATAI6 + ATAI7 + ATAI8 + ATAI9 + ATAI10 + ATAI11 + ATAI12 + ATAI13 + CHG_KAMOKU

        ATAI_A = ATAI Mod 10

        ATAI_B = 10 - ATAI_A

        If ATAI_B = 10 Then
            ATAI_B = 0
        End If
        If ATAI_B = IN_KOUZA7 Then
            GFUNC_CHK_DEJIT = True
        Else
            GFUNC_CHK_DEJIT = False
        End If

    End Function
    Public Function GFUNC_DB_COMBO_SET(ByVal pcmbKana As ComboBox, ByVal pcmbToriName As ComboBox, Optional ByVal TakouOnlyFlg As Boolean = False) As Boolean
        '*****************************************
        '�w�Z���R���{�{�b�N�X�ݒ�
        '*****************************************
        Dim Int_Counter As Integer

        GFUNC_DB_COMBO_SET = False

        Dim SQL As String = ""

        SQL = ""
        SQL += " SELECT GAKKOU_CODE_G , GAKKOU_NNAME_G FROM GAKMAST1,GAKMAST2 "
        SQL += " WHERE GAKKOU_CODE_G = GAKKOU_CODE_T "
        If TakouOnlyFlg = True Then
            SQL += " AND TAKO_KBN_T ='1'"
        End If
        If pcmbKana.Text <> "" Then
            SQL += " AND GAKKOU_KNAME_G LIKE '" & pcmbKana.Text & "%'"
        End If
        SQL += " GROUP by GAKKOU_CODE_G , GAKKOU_NNAME_G "
        SQL += " ORDER BY GAKKOU_CODE_G "

        '�R���{�N���A
        pcmbToriName.Items.Clear()

        If GFUNC_SELECT_SQL2(SQL, 0) = False Then
            Exit Function
        End If

        '�w�Z�������݂��Ȃ��ꍇ�̓G���[�Ƃ��ďI�����Ȃ�
        If OBJ_DATAREADER.HasRows = True Then
            Int_Counter = 0
            '�R���{�ݒ�
            While (OBJ_DATAREADER.Read = True)
                With OBJ_DATAREADER
                    pcmbToriName.Items.Add(.Item("GAKKOU_NNAME_G"))

                    ReDim Preserve STR_GCOAD(Int_Counter)
                    STR_GCOAD(Int_Counter) = CStr(.Item("GAKKOU_CODE_G"))
                    Int_Counter += 1
                End With
            End While
        End If


        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        GFUNC_DB_COMBO_SET = True

    End Function

    '
    '�@�֐����@-�@fn_GetEigyoubi
    '
    '�@�@�\    -  �c�Ɠ��擾
    '
    '�@����    -  TargetDate(�Ώۓ�),HoseiDate(�␳��),FrontBackkbn("-":�O�c�Ɠ��@"+"�F���c�Ɠ�)
    '
    '�@���l    -  
    '
    '�@
    Public Function fn_GetEigyoubi(ByVal TargetDate As String, ByVal HoseiDate As String, ByVal FrontBackkbn As String) As String

        Dim ret As String = ""

        Try
            '***���O���***
            STR_COMMAND = "fn_GetEigyoubi"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = TargetDate
            '***���O���**

            Dim YEAR As String = ""
            Dim MONTH As String = ""
            Dim DAY As String = ""
            Dim WorkDate As String = ""

            '���l
            If Not IsNumeric(TargetDate) Then
                Return ret
            End If
            '����
            If TargetDate.Length = 8 Then
                YEAR = TargetDate.Substring(0, 4)
                MONTH = TargetDate.Substring(4, 2)
                DAY = TargetDate.Substring(6)
                WorkDate = YEAR & "/" & MONTH & "/" & DAY
            Else
                Return ret
            End If

            '���t�^
            If Not IsDate(WorkDate) Then
                '���t�̂݃G���[�Ȃ�Ζ����G���[�Ƃ��␳����
                If Not IsDate(YEAR & "/" & MONTH & "/" & "01") Then
                    Return ret
                Else
                    '���� AddMonths(1).AddDays(-1)
                    TargetDate = CDate(YEAR & "/" & MONTH & "/" & "01").AddMonths(1).AddDays(-1).ToString("yyyyMMdd")
                End If
            End If

            Dim Bret As Boolean = False
            Bret = GCom.CheckDateModule(Nothing, 1)

            If Bret = False Then
                ret = ""
            End If

            Dim IntDate As Integer = CInt(HoseiDate)
            Dim IntFrontBackKbn As Integer = 0

            If FrontBackkbn = "-" Then
                IntFrontBackKbn = 1 '�O�c�Ɠ�
            Else
                IntFrontBackKbn = 0 '���c�Ɠ�
            End If

            If IntDate = 0 Then
                Bret = GCom.CheckDateModule(TargetDate, ret, IntFrontBackKbn)
            Else
                Bret = GCom.CheckDateModule(TargetDate, ret, IntDate, IntFrontBackKbn)
            End If

        Catch ex As Exception
            ret = ""
            Call GSUB_LOG(0, "�\�����ʃG���[:" & ex.ToString)
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@fn_JOBMAST_TOUROKU_CHECK
    '
    '�@�@�\    -  �W���u�}�X�^�ɃW���u��o�^����O�Ɋ��ɓo�^����Ă��Ȃ�������
    '
    '�@����    -  strJOBID�AstrUSERID�AstrPARAMETA
    '
    '�@���l    -  
    '
    '�@
    Public Function fn_JOBMAST_TOUROKU_CHECK(ByVal strJOBID As String, ByVal strUSERID As String, ByVal strPARAMETA As String) As Boolean

        Dim ret As Boolean = False

        Try
            Dim SQL As String = ""
            SQL = "SELECT * FROM JOBMAST "
            SQL &= " WHERE JOBID_J = '" & Trim(strJOBID) & "'"
            SQL &= " AND PARAMETA_J = '" & Trim(strPARAMETA) & "'"
            SQL &= " AND STATUS_J IN('0','1')"

            Dim OraConn As New OracleClient.OracleConnection(STR_CONNECTION)
            OraConn.Open()

            Dim OraCommnad As New OracleClient.OracleCommand
            OraCommnad.CommandText = SQL
            OraCommnad.Connection = OraConn

            Dim OraReader As OracleClient.OracleDataReader = Nothing
            OraReader = OraCommnad.ExecuteReader

            Dim COUNT As Integer = 0
            While (OraReader.Read)
                COUNT += 1
                ret = False
            End While

            If COUNT = 0 Then
                ret = True
            Else
                Call GSUB_MESSAGE_WARNING("�o�b�`�W���u�Ǘ��}�X�^�ɓo�^�ςł�")
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "�W���u�Ď��o�^�`�F�b�N���s�F" & ex.ToString)
            Call GSUB_MESSAGE_WARNING("�W���u�Ď��o�^�`�F�b�N�Ɏ��s���܂���")
        End Try

        Return ret

    End Function
    '
    '�@�֐����@-�@fn_INSERT_JOBMAST
    '
    '�@�@�\    -  �W���u�}�X�^�ɃW���u��o�^����
    '
    '�@����    -  strJOBID�AstrUSERID�AstrPARAMETA
    '
    '�@���l    -  
    '
    '�@
    Public Function fn_INSERT_JOBMAST(ByVal strJOBID As String, ByVal strUSERID As String, ByVal strPARAMETA As String) As Boolean

        Dim ret As Boolean = False

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If MainLog.IS_LEVEL3() = True Then
            sw = MainLog.Write_Enter3("Common.fn_INSERT_JOBMAST")
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
        Dim OraMain As CASTCommon.MyOracle = Nothing
        Dim dblock As CASTCommon.CDBLock = New CASTCommon.CDBLock

        Dim LockWaitTime As Integer = 30
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

        Try
            Dim JOB(12) As String
            JOB(0) = " "
            JOB(1) = 1                                              'TUUBAN_J
            JOB(2) = Format(System.DateTime.Today, "yyyyMMdd")      'TOUROKU_DATE_J
            JOB(3) = Format(System.DateTime.Now, "HHmmss")          'TOUROKU_TIME_J
            JOB(4) = "00000000"                                     'STA_DATE_J
            JOB(5) = "000000"                                       'STA_TIME_J
            JOB(6) = "00000000"                                     'END_DATE_J
            JOB(7) = "000000"                                       'END_TIME_J
            JOB(8) = strJOBID                                       'JOBID_J
            JOB(9) = "0"                                            'STATUS_J  '�I�k�M���J�X�^�}�C�Y�j�X�e�[�^�X�P�A���s��
            JOB(10) = strUSERID                                     'USERID_J
            JOB(11) = strPARAMETA                                   'PARAMETA_J
            JOB(12) = " "                                           'ERRMSG_J

            Dim SQL As String = ""
            SQL = " INSERT INTO JOBMAST"
            SQL &= " VALUES ( "
            SQL &= " '" & JOB(1) & "'"
            SQL &= ",'" & JOB(2) & "'"
            SQL &= ",'" & JOB(3) & "'"
            SQL &= ",'" & JOB(4) & "'"
            SQL &= ",'" & JOB(5) & "'"
            SQL &= ",'" & JOB(6) & "'"
            SQL &= ",'" & JOB(7) & "'"
            SQL &= ",'" & JOB(8) & "'"
            SQL &= ",'" & JOB(9) & "'"
            SQL &= ",'" & JOB(10) & "'"
            SQL &= ",'" & JOB(11) & "'"
            SQL &= ",'" & JOB(12) & "'"
            SQL &= " ) "

            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
'            Dim OraConn As New OracleClient.OracleConnection(STR_CONNECTION)
'            OraConn.Open()
'
'            Dim OraCommnad As New OracleClient.OracleCommand
'            OraCommnad.CommandText = SQL
'            OraCommnad.Connection = OraConn
'
'            Dim OraTran As OracleClient.OracleTransaction = OraConn.BeginTransaction
'            OraCommnad.Transaction = OraTran
'
'            If OraCommnad.ExecuteNonQuery() = 1 Then
'                OraTran.Commit()
'                ret = True
'            Else
'                Call GSUB_LOG(0, "�W���u�}�X�^�o�^���s")
'                Call GSUB_MESSAGE_WARNING("�W���u�}�X�^�̓o�^�Ɏ��s���܂���")
'                OraTran.Rollback()
'            End If
'
'            OraConn.Close()

            OraMain = New CASTCommon.MyOracle

            ' �W���u�o�^���b�N
            If dblock.InsertJOBMAST_Lock(OraMain, LockWaitTime) = False Then
                Throw New Exception("�W���u�}�X�^�o�^�^�C���A�E�g")
            End If

            ret = OraMain.ExecuteNonQuery(SQL)

            ' �W���u�o�^�A�����b�N
            dblock.InsertJOBMAST_UnLock(OraMain)

            OraMain.Commit()
            OraMain.Close()

            If MainLog.IS_LEVEL3() = True Then
                MainLog.Write_Exit3(sw, "Common.fn_INSERT_JOBMAST")
            End If
            '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
'            Call GSUB_LOG(0, "�W���u�}�X�^�o�^���s�F" & ex.ToString)
             MainLog.Write_Err("�W���u�}�X�^�o�^", "���s", ex)

            ' �W���u�o�^�A�����b�N
            dblock.InsertJOBMAST_UnLock(OraMain)

            If Not OraMain Is Nothing Then
                ' ���[���o�b�N
                OraMain.Rollback()
                OraMain.Close()
            End If
            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

            Call GSUB_MESSAGE_WARNING("�W���u�}�X�^�̓o�^�Ɏ��s���܂���")
        End Try

        Return ret

    End Function
    Public Function ConvNullToString(ByVal Value1 As Object, Optional ByVal Value2 As Object = Nothing) As String
        If IsDBNull(Value1) Then
            If Not Value2 Is Nothing Then
                Return Value2.ToString
            Else
                Return ""
            End If
        Else
            Return Value1.ToString
        End If
    End Function
    Public Function ConvKamoku1To2(ByVal strKAMOKU As String) As String
        Select Case strKAMOKU
            Case "1"
                Return "02"
            Case "2"
                Return "01"
            Case "3"
                Return "05"
            Case "4"
                Return "37"
            Case "9"
                Return "02"
            Case Else
                Return "02"
        End Select
    End Function
    Public Function ConvKanaNGToKanaOK(ByVal strTXT As String, ByRef strRETURN As String) As Boolean

        Dim ret As Boolean = False

        Try
            strRETURN = ""

            For i As Integer = 0 To strTXT.Length - 1
                Select Case strTXT.Chars(i)
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "�"
                    Case "�"
                        strRETURN = strRETURN + "-"
                    Case "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", _
                         "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
                        strRETURN = strRETURN + strTXT.Chars(i)
                    Case "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�", "�"
                        strRETURN = strRETURN + strTXT.Chars(i)
                    Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                        strRETURN = strRETURN + strTXT.Chars(i)
                    Case "�", "�"
                        strRETURN = strRETURN + strTXT.Chars(i)
                        '2010/10/20 "*","&","$"�͋K��O�����Ƃ���
                        'Case "\", ",", ".", "�", "�", "-", "/", "*", "&", "$", "(", ")"
                    Case "\", ",", ".", "�", "�", "-", "/", "(", ")"
                        '---------------------------------------------------------------
                        strRETURN = strRETURN + strTXT.Chars(i)
                    Case " "
                        strRETURN = strRETURN + strTXT.Chars(i)
                    Case Else
                        Exit Try
                End Select

            Next i

            ret = True

        Catch ex As Exception
            ret = False
        End Try

        Return ret

    End Function
End Module
