Imports System
Imports System.IO
Imports System.Windows.Forms

Public Class ClsLogin

    Private BatchLog As New BatchLOG("ClsLogin", "���O�C��")

    '���[�U�[�h�c�}�X�^�\����
    Private Structure stUIDMAST                 ' ���[�U�[�h�c�}�X�^
        Public LOGINID_U As String               ' ���[�U�[��
        Public PASSWORD_U As String             ' �p�X���[�h
        Public KENGEN_U As String               ' ����
        Public PASSWORD_1S_U As String         ' �P����O�p�X���[�h
        Public PASSWORD_DATE_U As String        ' �p�X���[�h�X�V���t
        Public SAKUSEI_DATE_U As String         ' �쐬���t
        Public KOUSIN_DATE_U As String          ' �X�V���t
    End Structure
    Private UIDMAST As stUIDMAST

    '�G���[���b�Z�[�W

    Private OraDB As MyOracle

    '2017/11/13 �^�X�N�j���� ADD (�W���ŏC��(��174)) -------------------- START
    Private PASS_SEDAI_CHK As Boolean = True

    Public Sub New()
        '�p�X���[�h�̐���`�F�b�N���s����INI�t�@�C�����擾����
        Dim WK As String = ""
        Dim MSG As String = ""

        Try
            WK = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "PASS_SEDAI_CHK")
            If WK.ToUpper = "ERR" OrElse WK = "" Then
                '���ڂȂ��A���ݒ�̏ꍇ�́A�����l�̂܂܁i����`�F�b�N���s���j
            Else
                If WK.Trim = "0" Then PASS_SEDAI_CHK = False
            End If

        Catch ex As Exception
            BatchLog.Write_Err("0000000000-00", "00000000", "ClsLogin����", "���s", ex)
        End Try
    End Sub
    '2017/11/13 �^�X�N�j���� ADD (�W���ŏC��(��174)) -------------------- END

    '�@�\   :   ���O�C���`�F�b�N
    '
    '����   :   
    '
    '�߂�l :  0 - �����C1 - ���[�U�h�c�G���[, 2 - �p�X���[�h�G���[
    '
    '���l   :   
    '
    Public Function LoginCheck(ByVal userid As String, ByVal password As String) As Integer
        Dim nRet As Integer

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���`�F�b�N(�J�n)", "����", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "���O�C���`�F�b�N", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            If OraDB Is Nothing Then
                OraDB = New MyOracle
            End If

            OraDB.BeginTrans()

            nRet = CheckUIDMAST(userid, password)
            If nRet <> 0 And nRet <> 2 Then
                OraDB.Rollback()
            Else
                OraDB.Commit()
            End If

        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���`�F�b�N(�I��)", "���s", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "���O�C���`�F�b�N", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            nRet = 1

        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���`�F�b�N(�I��)", "����", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "���O�C���`�F�b�N", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        End Try

        Return nRet
    End Function

    '�@�\   :   �p�X���[�h�ύX�`�F�b�N
    '
    '����   :   
    '
    '�߂�l :  0 - �����C1 - ���[�U�h�c�G���[, 2 - �p�X���[�h�G���[
    '
    '���l   :   
    '
    Protected Friend Function PasswordCheck(ByVal userid As String, ByVal oldpass As String, ByVal newpass As String, ByVal exactpass As String) As Integer
        Dim nRet As Integer

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���`�F�b�N(�J�n)", "����", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "�p�X���[�h�ύX�`�F�b�N", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            If OraDB Is Nothing Then
                OraDB = New MyOracle
            End If

            OraDB.BeginTrans()

            nRet = CheckPassword(userid, oldpass, newpass, exactpass)
            If nRet <> 0 Then
                OraDB.Rollback()
            Else
                OraDB.Commit()
            End If

        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���`�F�b�N", "���s", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "�p�X���[�h�ύX�`�F�b�N", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            nRet = 1

        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���`�F�b�N(�I��)", "����", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "�p�X���[�h�ύX�`�F�b�N", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        End Try

        Return nRet

    End Function

    Private Function CheckUIDMAST(ByVal userid As String, ByVal password As String) As Integer
        Dim SQL As String
        Dim OraReader As New MyOracleReader(OraDB)

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- START
        Dim INI_RSV2_PASS_LIMIT As String = GetRSKJIni("RSV2_V1.0.0", "PASS_LIMIT")
        Dim INI_RSV2_PASS_CHANGE As String = GetRSKJIni("RSV2_V1.0.0", "PASS_CHANGE")
        ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- END

        Try
            userid = userid.TrimEnd
            password = password.TrimEnd
            BatchLog.UserID = userid
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���F��(�J�n)", "����", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "���O�C���F��", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            ' ���[�UID�}�X�^�擾
            If GetUIDMAST(userid) > 0 Then
                Return 1
            End If

            If password.CompareTo(UIDMAST.PASSWORD_U) <> 0 Then
                '���[�UID�}�X�^�u�p�X���[�h�v�Ɖ�ʁu�p�X���[�h�v����v���Ȃ��ꍇ
                'MSG0006W:�p�X���[�h����v���܂���B
                MessageBox.Show(MSG0006W, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
                'BatchLog.Write("0000000000-00", "00000000", "���O�C���F��", "���s", "�p�X���[�h�G���[")
                BatchLog.Write_Err("0000000000-00", "00000000", "���O�C���F��", "���s", "�p�X���[�h�G���[")
                '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
                Return 2
            Else
                If UIDMAST.PASSWORD_DATE_U = "" OrElse UIDMAST.PASSWORD_U = "" Then
                    '���[�UID�}�X�^�u�p�X���[�h�v�@���@��ʁu�p�X���[�h�v����v���Ă���ꍇ�A
                    '���@���[�UID�}�X�^�u�p�X���[�h�X�V���t�v����̏ꍇ

                    'MSG0007W:�p�X���[�h�����ݒ�ł��B�ݒ肵�Ă��������B
                    MessageBox.Show(MSG0007W, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    ' �p�X���[�h�ύX��ʕ\��
                    Dim oFrm As New FrmPassChange(Me)
                    oFrm.UserID = userid
                    Call oFrm.ShowDialog()
                    If oFrm.DialogResult = DialogResult.OK Then
                        UIDMAST.PASSWORD_DATE_U = Calendar.Now.ToString("yyyyMMdd")
                        'Return 0
                    Else
                        Return 1
                    End If
                End If

                Try
                    Dim sCompDate As String = ""

                    ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- START
                    ''���[�UID�}�X�^�u�p�X���[�h�v�@���@��ʁu�p�X���[�h�v����v���Ă���ꍇ�A
                    ''���@���[�UID�}�X�^�u�p�X���[�h�X�V���t�v����100���o�߂��Ă���ꍇ
                    'SQL = "SELECT SYSDATE - 100 HIDUKE FROM DUAL"
                    ' ���[�UID�}�X�^�u�p�X���[�h�X�V���t�v����w�����([RSV2_V1.0.0]-[PASS_LIMIT])���o�߂��Ă���ꍇ
                    SQL = "SELECT SYSDATE - " & INI_RSV2_PASS_LIMIT & " HIDUKE FROM DUAL"
                    ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- END

                    If OraReader.DataReader(SQL) = True Then
                        sCompDate = OraReader.GetDate(0).ToString("yyyyMMdd")
                    End If
                    OraReader.Close()

                    If sCompDate >= UIDMAST.PASSWORD_DATE_U Then
                        '�{�����t - 100�� > ���R�[�h���p�X���[�h�X�V���t  �� �ŏI���O�C�������100���ȏ�o���������f

                        'MSG0011I:�p�X���[�h�̗L������������܂����B�p�X���[�h��ݒ肵�Ă��������B
                        MessageBox.Show(MSG0011I, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' �p�X���[�h�ύX��ʕ\��
                        Dim oFrm As New FrmPassChange(Me)
                        oFrm.UserID = userid
                        Call oFrm.ShowDialog()
                        If oFrm.DialogResult = DialogResult.OK Then
                            'Return 0
                            UIDMAST.PASSWORD_DATE_U = Calendar.Now.ToString("yyyyMMdd")
                        Else
                            Return 1
                        End If
                    End If

                Catch ex As Exception
                    MessageBox.Show(MSG0006E, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
                    'BatchLog.Write("0000000000-00", "00000000", "���O�C���F��", "���s", ex.Message)
                    BatchLog.Write_Err("0000000000-00", "00000000", "���O�C���F��", "���s", ex)
                    '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
                    Return 1
                End Try

                Try
                    Dim sCompDate As String = ""

                    ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- START
                    ''���[�UID�}�X�^�u�p�X���[�h�v�@���@��ʁu�p�X���[�h�v����v���Ă���ꍇ�A
                    ''���@���[�UID�}�X�^�u�p�X���[�h�X�V���t�v����X�R���o�߂��Ă���ꍇ
                    'SQL = "SELECT SYSDATE - 93 HIDUKE FROM DUAL"
                    ' ���[�UID�}�X�^�u�p�X���[�h�X�V���t�v����ύX����([RSV2_V1.0.0]-[PASS_CHANGE])���o�߂��Ă���ꍇ
                    SQL = "SELECT SYSDATE - " & INI_RSV2_PASS_CHANGE & " HIDUKE FROM DUAL"
                    ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- END

                    If OraReader.DataReader(SQL) = True Then
                        sCompDate = OraReader.GetDate(0).ToString("yyyyMMdd")
                    End If
                    OraReader.Close()

                    If UIDMAST.PASSWORD_DATE_U < sCompDate Then
                        ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- START
                        ''���R�[�h���p�X���[�h�X�V���t <  �{�����t - 93��  �� �p�X���[�h�X�V���t���93���ȏ�o���������f
                        ''�p�X���[�h�X�V���t+101���i���t�̍��ِ����v�Z����j
                        'Dim dAddPasswordCompDate As Date = ConvertDate(UIDMAST.PASSWORD_DATE_U).AddDays(101)
                        '���R�[�h���p�X���[�h�X�V���t <  �{�����t - 93��  �� �p�X���[�h�X�V���t���ύX�����ȏ�o���������f
                        '�p�X���[�h�X�V���t+(�w�����+1)�i���t�̍��ِ����v�Z����j
                        Dim PASS_LIMIT_Days As Integer = CInt(INI_RSV2_PASS_LIMIT) + 1
                        Dim dAddPasswordCompDate As Date = ConvertDate(UIDMAST.PASSWORD_DATE_U).AddDays(PASS_LIMIT_Days)
                        ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-13(RSV2�Ή�) -------------------- END
                        Dim nNissu As Integer

                        '�{�����t�i�T�[�o�[���j�Q��  Date.Now�͂���
                        SQL = "SELECT SYSDATE HIDUKE FROM DUAL"

                        If OraReader.DataReader(SQL) = True Then
                            nNissu = dAddPasswordCompDate.Subtract(OraReader.GetDate(0)).Days
                        End If

                        OraReader.Close()

                        'MSG0012I:�p�X���[�h�̗L���������؂��܂ŁB
                        MessageBox.Show(String.Format(MSG0012I, nNissu), "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                Catch ex As Exception
                    MessageBox.Show(MSG0006E, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
                    'BatchLog.Write("0000000000-00", "00000000", "���O�C���F��", "���s", ex.Message)
                    BatchLog.Write_Err("0000000000-00", "00000000", "���O�C���F��", "���s", ex)
                    '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
                    Return 1
                End Try

                Return 0
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���F��", "���s", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "���O�C���F��", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 1
        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���O�C���F��(�I��)", "����", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "���O�C���F��", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        End Try

    End Function

    Protected Friend Function CheckPassword(ByVal userid As String, ByVal oldpass As String, ByVal newpass As String, ByVal exactpass As String) As DialogResult
        Dim SQL As String

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            Dim newpassW As String, exactpassW As String
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "�p�X���[�h�`�F�b�N(�J�n)", "����", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "�p�X���[�h�`�F�b�N", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            If UIDMAST.LOGINID_U Is Nothing Then
                ' ���[�UID�}�X�^�擾
                Call GetUIDMAST(userid)
            End If

            '�u�o�^�v�������Ƀ`�F�b�N�������s���B
            If oldpass.CompareTo(UIDMAST.PASSWORD_U) <> 0 Then
                '���p�X���[�h�����R�[�h���p�X���[�h�ƈ�v���Ȃ��ꍇ
                'MSG0010W:���p�X���[�h�Ɍ�肪����܂��B
                MessageBox.Show(MSG0010W, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 1
            End If

            newpassW = newpass.Trim.Replace(" ", "")

            If newpass.Length <> newpassW.Length Then
                MessageBox.Show(MSG0341W, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 2
            End If

            exactpassW = exactpass.Trim.Replace(" ", "")

            If exactpass.Length <> exactpassW.Length Then
                MessageBox.Show(MSG0341W, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 3
            End If

            If newpass.CompareTo(oldpass) = 0 Then
                '�V�p�X���[�h�����p�X���[�h�ƈ�v����ꍇ
                'MSG0012W:���p�X���[�h�Ɠ����p�X���[�h�͎w��ł��܂���B
                MessageBox.Show(MSG0012W, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 2
            End If

            If newpass.CompareTo(exactpass) <> 0 Then
                '�V�p�X���[�h�ƐV�p�X���[�h�m�F����v���Ȃ��ꍇ
                'MSG0011W:�V�p�X���[�h�Ɍ�肪����܂��B
                MessageBox.Show(MSG0011W, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 2
            End If

            '2017/11/13 �^�X�N�j���� CHG (�W���ŏC��(��174)) -------------------- START
            If PASS_SEDAI_CHK = True Then
                If newpass.CompareTo(UIDMAST.PASSWORD_1S_U) = 0 Then
                    '�V�p�X���[�h������p�X���[�h�ƈ�v�����ꍇ
                    'MSG0013W:�ߋ��ɓo�^�����p�X���[�h�͎g�p�ł��܂���B
                    MessageBox.Show(MSG0013W, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 2
                End If
            End If
            'If newpass.CompareTo(UIDMAST.PASSWORD_1S_U) = 0 Then
            '    '�V�p�X���[�h������p�X���[�h�ƈ�v�����ꍇ
            '    'MSG0013W:�ߋ��ɓo�^�����p�X���[�h�͎g�p�ł��܂���B
            '    MessageBox.Show(MSG0013W, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return 2
            'End If
            '2017/11/13 �^�X�N�j���� CHG (�W���ŏC��(��174)) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "�p�X���[�h�`�F�b�N", "���s", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "�p�X���[�h�`�F�b�N", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 1
        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "�p�X���[�h�`�F�b�N(�I��)", "����", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "�p�X���[�h�`�F�b�N", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        End Try

        '�G���[�`�F�b�N�̌��ʂ�����̏ꍇ�A�X�V�������s�����O�C����ʁiKFJLGIN0001)�֑J�ڂ���B
        '���펞�A�b�v�f�[�g
        Try
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "�p�X���[�h�ύX(�J�n)", "����", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "�p�X���[�h�ύX", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            SQL = "UPDATE UIDMAST SET"
            SQL &= " PASSWORD_U    = " & SQ(newpass)                    '�V�p�X���[�h�����R�[�h�ɐݒ�
            SQL &= ",PASSWORD_1S_U = " & SQ(UIDMAST.PASSWORD_U)         '���p�X���[�h��1����O�p�X���[�h�ɐݒ�
            SQL &= ",PASSWORD_DATE_U  = TO_CHAR(SYSDATE,'YYYYMMDD') "   '�p�X���[�h�X�V���t�Ɍ��݂̓��t��ݒ�
            SQL &= ",KOUSIN_DATE_U  = TO_CHAR(SYSDATE,'YYYYMMDD') "     '�X�V���t�Ɍ��݂̓��t��ݒ�
            SQL &= " WHERE"
            SQL &= " LOGINID_U = " & SQ(UIDMAST.LOGINID_U)

            If OraDB.ExecuteNonQuery(SQL) = 1 Then
                Return 0
            End If

            Return 1

        Catch ex As Exception
            MessageBox.Show(MSG0006E, "�p�X���[�h�ύX(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "�p�X���[�h�ύX", "���s", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "�p�X���[�h�ύX", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 1

        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "�p�X���[�h�ύX(�I��)", "����", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "�p�X���[�h�ύX", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        End Try

    End Function

    Private Function GetUIDMAST(ByVal userid As String) As Integer
        Dim SQL As String
        Dim OraReader As New MyOracleReader(OraDB)

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        ' ���[�UID�}�X�^�̊Y���f�[�^���擾����B
        Try
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���[�U�Q��(�J�n)", "����")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "���[�U�Q��", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            SQL = "SELECT"
            SQL &= "  LOGINID_U"             '���[�U�[��
            SQL &= ", PASSWORD_U"           '�p�X���[�h
            SQL &= ", KENGEN_U"             '����
            SQL &= ", PASSWORD_1S_U"        '�P����O�p�X���[�h
            SQL &= ", PASSWORD_DATE_U"      '�p�X���[�h�ύX���t
            SQL &= ", SAKUSEI_DATE_U"       '�쐬���t
            SQL &= ", KOUSIN_DATE_U"        '�X�V���t
            SQL &= " FROM UIDMAST"          '���[�UID�}�X�^
            SQL &= " WHERE LOGINID_U = " & SQ(userid)    '�L�[ = ���[�U�[ID(��ʓ��͒l)

            If OraReader.DataReader(SQL) = False Then
                '���[�UID�}�X�^�ɊY�����R�[�h�����݂��Ȃ��ꍇ
                ' ���O�C���h�c�Ɍ�肪����܂��B
                MessageBox.Show(MSG0006W, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
                'BatchLog.Write("0000000000-00", "00000000", "���[�U�Q��", "���s", "���[�U���}�X�^���o�^")
                BatchLog.Write_Err("0000000000-00", "00000000", "���[�U�Q��", "���s", "���[�U���}�X�^���o�^")
                '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
                Return 1
            End If

            With UIDMAST
                .LOGINID_U = OraReader.GetString("LOGINID_U")
                .PASSWORD_U = OraReader.GetString("PASSWORD_U")
                .PASSWORD_1S_U = OraReader.GetString("PASSWORD_1S_U")
                .PASSWORD_DATE_U = OraReader.GetString("PASSWORD_DATE_U")
                .KOUSIN_DATE_U = OraReader.GetString("KOUSIN_DATE_U")
            End With

            OraReader.Close()
            Return 0

        Catch ex As Exception
            MessageBox.Show(MSG0006E, "���O�C��(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���[�U�Q��", "���s", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "���[�U�Q��", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 1

        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("0000000000-00", "00000000", "���[�U�Q��(�I��)", "����")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "���[�U�Q��", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        End Try
    End Function

    Protected Overrides Sub Finalize()
        If Not OraDB Is Nothing Then
            OraDB.Close()
        End If
        OraDB = Nothing

        MyBase.Finalize()
    End Sub

End Class
