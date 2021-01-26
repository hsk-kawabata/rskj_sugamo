Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text

Public Class KFGNENJ030

#Region " ���ʕϐ���` "
    Public strDIR As String        '���݂̃p�X
    Public intSinTuuban As Integer '�i�w���k���͗p�ʔ�
#End Region

#Region " �\���̒�` "
    '***********************************************
    '�w�Z���Ɓi�i�w��w�Z�܂ށj�̓��͍��ڏ��\����
    '***********************************************
    Public Structure structureGakkouData
        Public cmbKana As ComboBox       '�J�i�R���{�{�b�N�X
        Public cmbGakkouName As ComboBox '�w�Z���R���{�{�b�N�X
        Public txtGakkouCode As TextBox  '�w�Z�R�[�h�e�L�X�g�{�b�N�X
        Public labGakkouName As Label    '�w�Z���\�����x��
        Public strGakkouCode() As String '�w�Z�R�[�h�擾�p�z��
        Public strKigyoCode As String    '��ƃR�[�h
        Public strFuriCode As String     '�U�փR�[�h
    End Structure
    Public GakkouData(6) As structureGakkouData

    '***********************************************
    '���k�}�X�^�̍ō��w�N���k�i�i�w���k�j���\����
    '***********************************************
    Public Structure structureSeitoData
        '���k�}�X�^�̓��e��S�Ď擾����
        Public GAKKOU_CODE As String
        Public NENDO As String
        Public TUUBAN As Integer
        Public GAKUNEN_CODE As Integer
        Public CLASS_CODE As Integer
        Public SEITO_NO As String
        Public SEITO_KNAME As String
        Public SEITO_NNAME As String
        Public SEIBETU As String
        Public TKIN_NO As String
        Public TSIT_NO As String
        Public KAMOKU As String
        Public KOUZA As String
        Public MEIGI_KNAME As String
        Public MEIGI_NNAME As String
        Public FURIKAE As String
        Public KEIYAKU_NJYU As String
        Public KEIYAKU_DENWA As String
        Public KAIYAKU_FLG As String
        Public SINKYU_KBN As String
        Public HIMOKU_ID As String
        Public TYOUSI_FLG As Integer
        Public TYOUSI_NENDO As String
        Public TYOUSI_TUUBAN As Integer
        Public TYOUSI_GAKUNEN As Integer
        Public TYOUSI_CLASS As Integer
        Public TYOUSI_SEITONO As String
        Public TUKI_NO As String
        Public SEIKYU01 As String
        Public KINGAKU01 As Integer
        Public SEIKYU02 As String
        Public KINGAKU02 As Integer
        Public SEIKYU03 As String
        Public KINGAKU03 As Integer
        Public SEIKYU04 As String
        Public KINGAKU04 As Integer
        Public SEIKYU05 As String
        Public KINGAKU05 As Integer
        Public SEIKYU06 As String
        Public KINGAKU06 As Integer
        Public SEIKYU07 As String
        Public KINGAKU07 As Integer
        Public SEIKYU08 As String
        Public KINGAKU08 As Integer
        Public SEIKYU09 As String
        Public KINGAKU09 As Integer
        Public SEIKYU10 As String
        Public KINGAKU10 As Integer
        Public SEIKYU11 As String
        Public KINGAKU11 As Integer
        Public SEIKYU12 As String
        Public KINGAKU12 As Integer
        Public SEIKYU13 As String
        Public KINGAKU13 As Integer
        Public SEIKYU14 As String
        Public KINGAKU14 As Integer
        Public SEIKYU15 As String
        Public KINGAKU15 As Integer
        Public SAKUSEI_DATE As String
        Public KOUSIN_DATE As String
        Public YOBI1 As String
        Public YOBI2 As String
        Public YOBI3 As String
    End Structure
    Public SeitoData() As structureSeitoData
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '���[�v�p�ϐ�

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = True
        End With

        STR_SYORI_NAME = "�i�w�f�[�^�쐬����"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '�\���̂Ɋe�t�H�[����ݒ肷��
        PSUB_StructureSet()

        '�w�Z�R���{�ݒ�i�S�w�Z�j
        For i As Integer = 1 To 6
            If GFUNC_DB_COMBO_SET(GakkouData(i).cmbKana, GakkouData(i).cmbGakkouName) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�")
                Call GSUB_MESSAGE_WARNING("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���")
                Exit Sub
            Else
                '�R���{�{�b�N�X���w�Z���w�肷��ۂ̔z��̐ݒ�(�z��͊w�Z�R�[�h�ꗗ)
                If STR_GCOAD Is Nothing Then
                    '�w�Z���o�^����Ă��Ȃ�
                    ReDim STR_GCOAD(0)
                Else
                    '�w�Z�R�[�h�ꗗ���擾
                    GakkouData(i).strGakkouCode = STR_GCOAD.Clone
                End If
            End If
        Next

        '���w�N�x�\��
        '===2008/04/13 �N�x���x�����e�L�X�g�{�b�N�X�ɕύX=============
        'If Format(Month(Now), "00") >= 4 Then
        '    '�S�`�P�Q��
        '    labNendo.Text = Format(Year(Now), "0000") + 1
        'Else
        '    '�P�`�R��
        '    labNendo.Text = Format(Year(Now), "0000")
        'End If
        If Format(Month(Now), "00") >= 4 Then
            '�S�`�P�Q��
            txtNendo.Text = Format(Year(Now), "0000") + 1
        Else
            '�P�`�R��
            txtNendo.Text = Format(Year(Now), "0000")
        End If
        '==========================================================

        '���݂̃p�X���擾
        strDIR = CurDir()

        txtGakkouCode1.Focus()

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '==========================
        '���s�{�^��
        '==========================

        '���̓`�F�b�N
        If PFUNC_CommonCheck() = False Then
            Exit Sub
        End If

        '�V�����}�X�^�擾
        If PFUNC_SEITOMAST2_Check() = False Then
            Exit Sub
        End If

        If MessageBox.Show("�������܂����H", STR_SYORI_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Exit Sub
        End If

        '�i�w���k���擾
        If PFUNC_SEITOMAST_Check() = False Then
            Exit Sub
        End If

        '�i�w���k�o�^����
        If PFUNC_InsSinnyusei() = False Then
            Exit Sub
        End If

        '���[�������
        If PFUNC_PrintMeisai() = False Then
            Exit Sub
        End If

        ''2008/03/09 �d�l�ύX�ɂ��R�����g
        ''�b�r�u�t�@�C���o��
        'If PFUNC_CSVFileWrite() = False Then
        '    Exit Sub
        'End If

        GSUB_LOG_OUT(GakkouData(6).txtGakkouCode.Text, "", "�i�w�f�[�^�쐬", "�f�[�^�쐬����", "����", "")  '2008/03/23 �����������O�ǉ�
        GSUB_MESSAGE_INFOMATION("�o�^���܂���")

    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        '������{�^���i��ʃN���A�j

        '���[�v�p�ϐ�

        '�e��������
        For i As Integer = 1 To 6

            '�w�Z�R�[�h�E�w�Z���E�J�i�R���{������
            GakkouData(i).txtGakkouCode.Text = ""
            GakkouData(i).labGakkouName.Text = ""
            GakkouData(i).cmbKana.SelectedIndex = -1

            If GFUNC_DB_COMBO_SET(GakkouData(i).cmbKana, GakkouData(i).cmbGakkouName) = False Then
                Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�")
                Call GSUB_MESSAGE_WARNING("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���")
                Exit Sub
            Else
                '�R���{�{�b�N�X���w�Z���w�肷��ۂ̔z��̐ݒ�(�z��͊w�Z�R�[�h�ꗗ)
                If STR_GCOAD Is Nothing Then
                    '�w�Z���o�^����Ă��Ȃ�
                    ReDim STR_GCOAD(0)
                Else
                    '�w�Z�R�[�h�ꗗ���擾
                    GakkouData(i).strGakkouCode = STR_GCOAD.Clone
                End If
            End If
        Next

        '���w�N�x�\��
        '===2008/04/13 ���x�����e�L�X�g�{�b�N�X�ɕύX=============
        'If Format(Month(Now), "00") >= 4 Then
        '    '�S�`�P�Q��
        '    labNendo.Text = Format(Year(Now), "0000") + 1
        'Else
        '    '�P�`�R��
        '    labNendo.Text = Format(Year(Now), "0000")
        'End If
        If Format(Month(Now), "00") >= 4 Then
            '�S�`�P�Q��
            txtNendo.Text = Format(Year(Now), "0000") + 1
        Else
            '�P�`�R��
            txtNendo.Text = Format(Year(Now), "0000")
        End If
        '=========================================================


    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '�I���{�^��

        Me.Close()
    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GakNameGet(ByVal intIndex As Integer) As Integer
        '�w�Z���̐ݒ�
        PFUNC_GakNameGet = 0

        STR_SQL = "SELECT GAKKOU_NNAME_G,SINKYU_NENDO_T,YOBI1_T,YOBI2_T FROM KZFMAST.GAKMAST1,KZFMAST.GAKMAST2 "
        STR_SQL += "WHERE GAKKOU_CODE_G = GAKKOU_CODE_T "
        STR_SQL += "AND GAKKOU_CODE_G = " & " '" & GakkouData(intIndex).txtGakkouCode.Text & "'"

        '�w�Z�}�X�^���݃`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            GSUB_MESSAGE_WARNING("�w�Z�}�X�^�ɓo�^����Ă��܂���")
            GakkouData(intIndex).labGakkouName.Text = ""
            GakkouData(intIndex).txtGakkouCode.Focus()
            PFUNC_GakNameGet = 1
            Exit Function
        End If

        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
        OBJ_DATAREADER.Read()

        '�i�������`�F�b�N
        '===2008/04/13 �N�x���x�����e�L�X�g�{�b�N�X�ɕύX==================================================================
        If CInt(OBJ_DATAREADER.Item("SINKYU_NENDO_T")) >= CInt(txtNendo.Text) Then
            'GSUB_MESSAGE_WARNING( "���łɐi�������ς݂̊w�Z�ł�")       '2008/03/09�@�C��
            GSUB_MESSAGE_WARNING(GakkouData(intIndex).labGakkouName.Text.Trim & "�@�͊��ɐi�������ς݂̊w�Z�ł�")
        End If
        '==================================================================================================================

        '�w�Z���\��
        GakkouData(intIndex).labGakkouName.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G")

        '��ƃR�[�h�E�U�փR�[�h�擾
        GakkouData(intIndex).strKigyoCode = OBJ_DATAREADER.Item("YOBI1_T")
        GakkouData(intIndex).strFuriCode = OBJ_DATAREADER.Item("YOBI2_T")

        OBJ_DATAREADER.Close()

    End Function

    Private Function PFUNC_SEITOMAST_Check() As Boolean
        '=============================================
        '�i�w���k�̃`�F�b�N�E�擾
        '=============================================
        PFUNC_SEITOMAST_Check = False

        Dim j As Integer

        Try
            For i As Integer = 1 To 5
                If GakkouData(i).txtGakkouCode.Text.Trim <> "" Then
                    '���k�}�X�^����ō��w�N�̐��k(�i�w���鐶�k)���擾
                    STR_SQL = "SELECT * FROM KZFMAST.SEITOMAST "
                    STR_SQL += "WHERE GAKKOU_CODE_O = '" & GakkouData(i).txtGakkouCode.Text.Trim & "' "
                    STR_SQL += "AND GAKUNEN_CODE_O = (SelectSAIKOU_GAKUNEN_T FROM GAKMAST2 "
                    STR_SQL += "WHERE GAKKOU_CODE_T = '" & GakkouData(i).txtGakkouCode.Text.Trim & "') "
                    STR_SQL += " AND KAIYAKU_FLG_O = '0' "      '2008/03/23 ���ςݐ��k�͂͂���
                    STR_SQL += " AND SINKYU_KBN_O = '0' "       '2008/03/23 �i�����Ȃ����k�͂͂���
                    STR_SQL += "AND TUKI_NO_O = '04'" '�S�����̂ݍ̎�
                    STR_SQL += " ORDER BY CLASS_CODE_O , SEITO_NO_O "   '2008/03/23 �N���X�E���k�ԍ����ɐV�ʔԂ��̔�

                    '���k�}�X�^�^�i�w���k���݃`�F�b�N
                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        'GSUB_MESSAGE_WARNING( "�i�w���鐶�k�����݂��܂���")     '2008/03/09 �C��
                        GSUB_MESSAGE_WARNING(GakkouData(i).labGakkouName.Text.Trim & " �ɂ͐i�w���鐶�k�����݂��܂���")
                        Exit Function
                    End If

                    OBJ_COMMAND.Connection = OBJ_CONNECTION
                    OBJ_COMMAND.CommandText = STR_SQL
                    OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

                    While (OBJ_DATAREADER.Read = True)
                        '�\���̔z��̍Ē�`�i�v�f���̍X�V�j
                        ReDim Preserve SeitoData(j)

                        '�ʔԐݒ�
                        intSinTuuban += 1

                        '�i�w���k�����i�[
                        With SeitoData(j)
                            .GAKKOU_CODE = GakkouData(6).txtGakkouCode.Text '�w�ZCODE�F�i�w��w�Z
                            '===2008/04/13 �N�x���x�����e�L�X�g�{�b�N�X�ɕύX======================================
                            '.NENDO = labNendo.Text.Trim                     '���w�N�x�F�\������Ă���l��ݒ�
                            .NENDO = txtNendo.Text.Trim                     '���w�N�x�F�\������Ă���l��ݒ�
                            '======================================================================================
                            .TUUBAN = intSinTuuban                          '�ʁ@�@�ԁF�̔Ԃ��Ȃ���
                            .GAKUNEN_CODE = 0                               '�w�@�@�N�F�O�Œ�(�V�����̂���)
                            .CLASS_CODE = 1                                 '�N �� �X�F�P�Œ�
                            .SEITO_NO = Format(intSinTuuban, "0000000")     '���k�ԍ��F�̔Ԃ��Ȃ���
                            .SEITO_KNAME = OBJ_DATAREADER.Item("SEITO_KNAME_O")
                            .SEITO_NNAME = PFUNC_NullCheckST(OBJ_DATAREADER.Item("SEITO_NNAME_O"))
                            .SEIBETU = OBJ_DATAREADER.Item("SEIBETU_O")
                            .TKIN_NO = OBJ_DATAREADER.Item("TKIN_NO_O")
                            .TSIT_NO = OBJ_DATAREADER.Item("TSIT_NO_O")
                            .KAMOKU = OBJ_DATAREADER.Item("KAMOKU_O")
                            .KOUZA = OBJ_DATAREADER.Item("KOUZA_O")
                            .MEIGI_KNAME = OBJ_DATAREADER.Item("MEIGI_KNAME_O")
                            .MEIGI_NNAME = PFUNC_NullCheckST(OBJ_DATAREADER.Item("MEIGI_NNAME_O"))
                            .FURIKAE = OBJ_DATAREADER.Item("FURIKAE_O")
                            .KEIYAKU_NJYU = PFUNC_NullCheckST(OBJ_DATAREADER.Item("KEIYAKU_NJYU_O"))
                            .KEIYAKU_DENWA = PFUNC_NullCheckST(OBJ_DATAREADER.Item("KEIYAKU_DENWA_O"))
                            .KAIYAKU_FLG = OBJ_DATAREADER.Item("KAIYAKU_FLG_O")
                            .SINKYU_KBN = OBJ_DATAREADER.Item("SINKYU_KBN_O")
                            .HIMOKU_ID = "001"                              '��@�@�ځF�P�Œ�
                            .TYOUSI_FLG = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_FLG_O"))
                            .TYOUSI_NENDO = PFUNC_NullCheckST(OBJ_DATAREADER.Item("TYOUSI_NENDO_O"))    '2007/10/15�C��
                            .TYOUSI_TUUBAN = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_TUUBAN_O"))
                            .TYOUSI_GAKUNEN = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_GAKUNEN_O"))
                            .TYOUSI_CLASS = PFUNC_NullCheck(OBJ_DATAREADER.Item("TYOUSI_CLASS_O"))
                            .TYOUSI_SEITONO = OBJ_DATAREADER.Item("TYOUSI_SEITONO_O")
                            .TUKI_NO = OBJ_DATAREADER.Item("TUKI_NO_O")
                            .SEIKYU01 = "0"                                 '��FLG �F�O�Œ�(��ڂP�`�P�T)
                            .KINGAKU01 = 0                                  '�ʋ��z�F�O�Œ�(��ڂP�`�P�T)
                            .SEIKYU02 = "0"
                            .KINGAKU02 = 0
                            .SEIKYU03 = "0"
                            .KINGAKU03 = 0
                            .SEIKYU04 = "0"
                            .KINGAKU04 = 0
                            .SEIKYU05 = "0"
                            .KINGAKU05 = 0
                            .SEIKYU06 = "0"
                            .KINGAKU06 = 0
                            .SEIKYU07 = "0"
                            .KINGAKU07 = 0
                            .SEIKYU08 = "0"
                            .KINGAKU08 = 0
                            .SEIKYU09 = "0"
                            .KINGAKU09 = 0
                            .SEIKYU10 = "0"
                            .KINGAKU10 = 0
                            .SEIKYU11 = "0"
                            .KINGAKU11 = 0
                            .SEIKYU12 = "0"
                            .KINGAKU12 = 0
                            .SEIKYU13 = "0"
                            .KINGAKU13 = 0
                            .SEIKYU14 = "0"
                            .KINGAKU14 = 0
                            .SEIKYU15 = "0"
                            .KINGAKU15 = 0
                            .SAKUSEI_DATE = Now.ToString("yyyyMMdd")        '�o�^���t�F�V�X�e�����t
                            .KOUSIN_DATE = ""                               '�X�V���t�F�Ȃ�
                            '�\���P�`�R�ɋ��f�[�^��ێ�����
                            '�\���P�F���w�Z�R�[�h
                            .YOBI1 = GakkouData(i).txtGakkouCode.Text.Trim
                            '�\���Q�F����ƃR�[�h�E���U�փR�[�h
                            .YOBI2 = GakkouData(i).strKigyoCode.Trim & GakkouData(i).strFuriCode.Trim
                            '�\���R�F���N���X�E�����k�ԍ�
                            .YOBI3 = OBJ_DATAREADER.Item("CLASS_CODE_O") & OBJ_DATAREADER.Item("SEITO_NO_O")
                        End With

                        j += 1 '�J�E���g�A�b�v

                    End While
                    OBJ_DATAREADER.Close()
                End If
            Next

            PFUNC_SEITOMAST_Check = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�i�w���k���擾")
            GSUB_MESSAGE_WARNING("�i�w���k���̎擾�Ɏ��s���܂���" & vbCrLf & ex.Message)
            OBJ_DATAREADER.Close()
            Exit Function
        End Try

    End Function

    Private Function PFUNC_SEITOMAST2_Check() As Boolean
        '=============================================
        '�V�����}�X�^�̒ʔԎ擾
        '=============================================
        PFUNC_SEITOMAST2_Check = False

        STR_SQL = "SELECT MAX(TUUBAN_O) FROM KZFMAST.SEITOMAST2 "
        STR_SQL += "WHERE GAKKOU_CODE_O = '" & GakkouData(6).txtGakkouCode.Text & "' "
        '===2008/04/13 �N�x���x�����e�L�X�g�{�b�N�X�ɕύX=======
        'STR_SQL += "AND NENDO_O = '" & labNendo.Text & "'"
        STR_SQL += "AND NENDO_O = '" & txtNendo.Text & "'"
        '=======================================================

        Try
            OBJ_COMMAND.Connection = OBJ_CONNECTION
            OBJ_COMMAND.CommandText = STR_SQL
            OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

            OBJ_DATAREADER.Read()

            '�V�����ʔԂ̎擾
            If TypeOf OBJ_DATAREADER.Item("MAX(TUUBAN_O)") Is DBNull Then
                '�o�^�Ȃ��i�c�a�m�t�k�k�j
                intSinTuuban = 0
            Else
                '�ʔԂ̍ő�l���擾
                intSinTuuban = OBJ_DATAREADER.Item("MAX(TUUBAN_O)")
            End If

            OBJ_DATAREADER.Close()
            PFUNC_SEITOMAST2_Check = True

        Catch ex As Exception
            Call GSUB_LOG(0, "�V�����}�X�^�擾")
            GSUB_MESSAGE_WARNING("�V�����}�X�^�̎擾�Ɏ��s���܂���" & vbCrLf & ex.Message)
            OBJ_DATAREADER.Close()
            Exit Function
        End Try

    End Function

    Private Function PFUNC_CommonCheck() As Boolean
        '===================================
        '���̓`�F�b�N
        '===================================
        PFUNC_CommonCheck = False

        Dim j As Integer

        '===2008/04/13===========================================
        '-------------------------------------------
        '�i�w��w�Z�R�[�h���̓`�F�b�N
        '-------------------------------------------
        If txtNendo.Text = "" Then
            GSUB_MESSAGE_WARNING("���w�N�x�������͂ł�")
            txtNendo.Focus()
            Exit Function
        End If
        '========================================================

        '-------------------------------------------
        '�i�w��w�Z�R�[�h���̓`�F�b�N
        '-------------------------------------------
        If GakkouData(6).txtGakkouCode.Text = "" Then
            GSUB_MESSAGE_WARNING("�i�w��w�Z�������͂ł�")
            GakkouData(6).txtGakkouCode.Focus()
            Exit Function
        End If

        '-------------------------------------------
        '�����w�Z�R�[�h�����͂���Ă��Ȃ����`�F�b�N
        '-------------------------------------------
        For i As Integer = 1 To 5
            '�󗓂̏ꍇ�̓`�F�b�N���Ȃ�
            If GakkouData(i).txtGakkouCode.Text.Trim <> "" Then
                For j = i + 1 To 6
                    If GakkouData(i).txtGakkouCode.Text.Trim = GakkouData(j).txtGakkouCode.Text.Trim Then
                        '�����w�Z���Q�ȏ���͂���Ă���
                        GSUB_MESSAGE_WARNING("�����w�Z���Q�ȏ���͂���Ă��܂�")
                        PFUNC_CommonCheck = False
                        GakkouData(i).txtGakkouCode.Focus()
                        Exit Function
                    Else
                        '�P���ł��L���Ȋw�Z�����͂���Ă����ꍇ
                        PFUNC_CommonCheck = True
                    End If
                Next
            End If
        Next

        '-------------------------------------------
        '�i�w���w�Z�R�[�h���̓`�F�b�N
        '-------------------------------------------
        If PFUNC_CommonCheck = False Then
            '�w�Z�R�[�h���ꌏ�����͂���Ă��Ȃ�
            GSUB_MESSAGE_WARNING("�i�w���w�Z�͍Œ�P���K�v�ł�")
            GakkouData(1).txtGakkouCode.Focus()
        End If

    End Function

    Private Function PFUNC_InsSinnyusei() As Boolean
        '===================================
        '�V�����}�X�^�o�^����
        '===================================
        PFUNC_InsSinnyusei = False

        Dim intTuki As Integer

        '�g�����U�N�V�����J�n
        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        For i As Integer = 0 To SeitoData.Length - 1
            '-----------------------------------
            '�\���̂���l�𒊏o�A�r�p�k�����쐬
            '-----------------------------------
            For intTuki = 1 To 12   '�P�Q������
                With SeitoData(i)
                    STR_SQL = "INSERT INTO SEITOMAST2 VALUES ('"
                    STR_SQL += .GAKKOU_CODE & "','"
                    STR_SQL += .NENDO & "',"
                    STR_SQL += .TUUBAN & ","
                    STR_SQL += .GAKUNEN_CODE & ","
                    STR_SQL += .CLASS_CODE & ",'"
                    STR_SQL += .SEITO_NO & "','"
                    STR_SQL += .SEITO_KNAME & "','"
                    STR_SQL += .SEITO_NNAME & "','"
                    STR_SQL += .SEIBETU & "','"
                    STR_SQL += .TKIN_NO & "','"
                    STR_SQL += .TSIT_NO & "','"
                    STR_SQL += .KAMOKU & "','"
                    STR_SQL += .KOUZA & "','"
                    STR_SQL += .MEIGI_KNAME & "','"
                    STR_SQL += .MEIGI_NNAME & "','"
                    STR_SQL += .FURIKAE & "','"
                    STR_SQL += .KEIYAKU_NJYU & "','"
                    STR_SQL += .KEIYAKU_DENWA & "','"
                    STR_SQL += .KAIYAKU_FLG & "','"
                    STR_SQL += .SINKYU_KBN & "','"
                    STR_SQL += .HIMOKU_ID & "',"
                    STR_SQL += .TYOUSI_FLG & ",'"
                    STR_SQL += .TYOUSI_NENDO & "',"
                    STR_SQL += .TYOUSI_TUUBAN & ","
                    STR_SQL += .TYOUSI_GAKUNEN & ","
                    STR_SQL += .TYOUSI_CLASS & ",'"
                    STR_SQL += .TYOUSI_SEITONO & "','"
                    STR_SQL += Format(intTuki, "00") & "','"      '���i�Q���\���j
                    STR_SQL += .SEIKYU01 & "',"
                    STR_SQL += .KINGAKU01 & ",'"
                    STR_SQL += .SEIKYU02 & "',"
                    STR_SQL += .KINGAKU02 & ",'"
                    STR_SQL += .SEIKYU03 & "',"
                    STR_SQL += .KINGAKU03 & ",'"
                    STR_SQL += .SEIKYU04 & "',"
                    STR_SQL += .KINGAKU04 & ",'"
                    STR_SQL += .SEIKYU05 & "',"
                    STR_SQL += .KINGAKU05 & ",'"
                    STR_SQL += .SEIKYU06 & "',"
                    STR_SQL += .KINGAKU06 & ",'"
                    STR_SQL += .SEIKYU07 & "',"
                    STR_SQL += .KINGAKU07 & ",'"
                    STR_SQL += .SEIKYU08 & "',"
                    STR_SQL += .KINGAKU08 & ",'"
                    STR_SQL += .SEIKYU09 & "',"
                    STR_SQL += .KINGAKU09 & ",'"
                    STR_SQL += .SEIKYU10 & "',"
                    STR_SQL += .KINGAKU10 & ",'"
                    STR_SQL += .SEIKYU11 & "',"
                    STR_SQL += .KINGAKU11 & ",'"
                    STR_SQL += .SEIKYU12 & "',"
                    STR_SQL += .KINGAKU12 & ",'"
                    STR_SQL += .SEIKYU13 & "',"
                    STR_SQL += .KINGAKU13 & ",'"
                    STR_SQL += .SEIKYU14 & "',"
                    STR_SQL += .KINGAKU14 & ",'"
                    STR_SQL += .SEIKYU15 & "',"
                    STR_SQL += .KINGAKU15 & ",'"
                    STR_SQL += .SAKUSEI_DATE & "','"
                    STR_SQL += .KOUSIN_DATE & "','"
                    STR_SQL += .YOBI1 & "','"
                    STR_SQL += .YOBI2 & "','"
                    STR_SQL += .YOBI3 & "')"
                End With

                '�f�[�^�x�[�X�o�^
                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                    Call GSUB_LOG(0, "�V�����}�X�^�o�^")
                    Call GSUB_MESSAGE_WARNING("�V�����}�X�^�o�^�Ɏ��s���܂���")
                    Exit Function
                End If
            Next
        Next

        '�b�n�l�l�h�s����
        GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)
        PFUNC_InsSinnyusei = True

    End Function

    Private Function PFUNC_PrintMeisai() As Boolean
        '===================================
        '�i�w�f�[�^�쐬���ו\�o��
        '===================================
        PFUNC_PrintMeisai = False

        Dim strReportPath As String '���[�̃p�X
        Dim strPrintSQL As String   '���[�o�͏����r�p�k��
        '���[�v�p�ϐ�

        '�N�����|�ϐ�
        'Dim CRXApplication As New CRAXDDRT.Application
        'Dim CRXReport As CRAXDDRT.Report
        'Dim CPProperty As CRAXDDRT.ConnectionProperty
        'Dim DBTable As CRAXDDRT.DatabaseTable
        'Dim CRX_FORMULA As CRAXDDRT.FormulaFieldDefinition
        'Dim strFormulaName As String

        Try
            '���[�̃p�X���擾
            strReportPath = STR_LST_PATH
            strReportPath += "�i�w�f�[�^���ו\.RPT"

            If System.IO.File.Exists(strReportPath) = False Then
                Call GSUB_MESSAGE_WARNING("���|�[�g��`�t�@�C�������o�^�ł��B(" & strReportPath & ")")
                Exit Function
            End If

            '�o�͏����ݒ�
            strPrintSQL = "SELECT * FROM GAKMAST1,SEITOMAST2 "
            strPrintSQL += "WHERE GAKKOU_CODE_O = '" & Trim(GakkouData(6).txtGakkouCode.Text) & "' "
            strPrintSQL += "AND GAKUNEN_CODE_G = 1 "
            strPrintSQL += "AND TUKI_NO_O = 4 "
            strPrintSQL += "AND GAKKOU_CODE_G = GAKKOU_CODE_O "
            strPrintSQL += "AND YOBI1_O IS NOT NULL " '�\�����P�i���w�Z�R�[�h�j�������͂łȂ��i�i�w���k�j�ꍇ
            strPrintSQL += "ORDER BY TUUBAN_O"

            '�N�����|�o�͐ݒ�
            'CRXReport = CRXApplication.OpenReport(strReportPath, 1)
            'DBTable = CRXReport.Database.Tables(1)
            'CPProperty = DBTable.ConnectionProperties("Password")
            'CPProperty.Value = "KZFMAST"
            'CRXReport.SQLQueryString = strPrintSQL

            ''�w�Z�P�`�T�̏���t�^
            'For i As Integer = 1 To CRXReport.FormulaFields.Count
            '    CRX_FORMULA = CRXReport.FormulaFields.Item(i)
            '    strFormulaName = CRX_FORMULA.FormulaFieldName

            '    Select Case strFormulaName
            '        Case "�w�Z���P"
            '            CRX_FORMULA.Text = "'" & GakkouData(1).labGakkouName.Text & "'"
            '        Case "�w�Z�R�[�h�P"
            '            CRX_FORMULA.Text = "'" & GakkouData(1).txtGakkouCode.Text & "'"
            '        Case "�w�Z���Q"
            '            CRX_FORMULA.Text = "'" & GakkouData(2).labGakkouName.Text & "'"
            '        Case "�w�Z�R�[�h�Q"
            '            CRX_FORMULA.Text = "'" & GakkouData(2).txtGakkouCode.Text & "'"
            '        Case "�w�Z���R"
            '            CRX_FORMULA.Text = "'" & GakkouData(3).labGakkouName.Text & "'"
            '        Case "�w�Z�R�[�h�R"
            '            CRX_FORMULA.Text = "'" & GakkouData(3).txtGakkouCode.Text & "'"
            '        Case "�w�Z���S"
            '            CRX_FORMULA.Text = "'" & GakkouData(4).labGakkouName.Text & "'"
            '        Case "�w�Z�R�[�h�S"
            '            CRX_FORMULA.Text = "'" & GakkouData(4).txtGakkouCode.Text & "'"
            '        Case "�w�Z���T"
            '            CRX_FORMULA.Text = "'" & GakkouData(5).labGakkouName.Text & "'"
            '        Case "�w�Z�R�[�h�T"
            '            CRX_FORMULA.Text = "'" & GakkouData(5).txtGakkouCode.Text & "'"
            '    End Select
            'Next

            ''���[�o��
            'CRXReport.PrintOut(False, 1)
            Call ChDir(strDIR)

        Catch ex As Exception
            Call GSUB_LOG(0, "�i�w�f�[�^�쐬���ו\�쐬")
            Call GSUB_MESSAGE_WARNING("�i�w�f�[�^�쐬���ו\�̍쐬�Ɏ��s���܂���" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Exit Function
        End Try

        PFUNC_PrintMeisai = True

    End Function

    Private Function PFUNC_CSVFileWrite() As Boolean
        '===================================
        '�b�r�u�t�@�C���o��
        '===================================
        PFUNC_CSVFileWrite = False

        Dim strCSVOutFile As String     '�o�͂b�r�u�t�@�C����
        Dim fnbr As Integer             '�t�@�C���ԍ�
        Dim strCSV_NName As String      '�b�r�u�o�͗p���k������
        Dim strCSV_OldClass As String   '�b�r�u�o�͗p���N���X
        Dim strCSV_OldSeitoNo As String '�b�r�u�o�͗p�����k�ԍ�

        Try
            '�t�@�C���ۑ��̃_�C�A���O�{�b�N�X�̐ݒ�
            SaveFileDialog1.InitialDirectory = STR_CSV_PATH
            SaveFileDialog1.Filter = "csv̧��(*.csv)|*.csv|�S�Ă�̧��(*.*)|*.*"
            SaveFileDialog1.FilterIndex = 1
            SaveFileDialog1.FileName = "CLASS_SINGAKU" & GakkouData(6).txtGakkouCode.Text & ".csv"

            If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                strCSVOutFile = SaveFileDialog1.FileName
            Else
                Call ChDir(strDIR)
                PFUNC_CSVFileWrite = True '�L�����Z���������������Ƃ݂Ȃ�
                Exit Function
            End If

            '�b�r�u�t�@�C���̃I�[�v��
            fnbr = FreeFile()
            FileOpen(fnbr, strCSVOutFile, OpenMode.Output)

            '�^�C�g���o��
            WriteLine(fnbr, "�폜�t���O", "���h�c�i�N���X�j", "���h�c�i���k�ԍ��j", "�w�Z�R�[�h", "�N�x", "�ʔ�", "���k���J�i", "���k��", "�w�N", "���w�Z��", "���N���X", "�����k�ԍ�", "�V�N���X", "�V���k�ԍ�")

            '�i�w���k�f�[�^�̓Ǎ��݁i�V�����͏����j
            STR_SQL = ""
            'STR_SQL += "SELECT * FROM KZFMAST.SEITOMAST2 "     '2008/03/10 ���w�Z�����擾�ǉ�
            STR_SQL += "SELECT SEITOMAST2.* , GAKKOU_NNAME_G FROM KZFMAST.SEITOMAST2 , GAKMAST1 "
            STR_SQL += "WHERE GAKKOU_CODE_O   = " & "'" & GakkouData(6).txtGakkouCode.Text & "' "
            STR_SQL += "and YOBI1_O = GAKKOU_CODE_G "   '2008/03/10 �����ǉ��@ 
            STR_SQL += "AND TUKI_NO_O  ='04' "
            STR_SQL += "AND YOBI1_O IS NOT NULL " '�\�����P�i���w�Z�R�[�h�j�������͂łȂ��i�i�w���k�j�ꍇ
            STR_SQL += "AND YOBI2_O IS NOT NULL " '�\�����Q
            STR_SQL += "AND YOBI3_O IS NOT NULL " '�\�����R
            STR_SQL += "ORDER BY TUUBAN_O"

            OBJ_COMMAND.Connection = OBJ_CONNECTION
            OBJ_COMMAND.CommandText = STR_SQL
            OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

            While (OBJ_DATAREADER.Read = True)
                With OBJ_DATAREADER
                    If IsDBNull(.Item("SEITO_NNAME_O")) = False Then
                        strCSV_NName = Trim(.Item("SEITO_NNAME_O"))
                    Else
                        strCSV_NName = ""
                    End If

                    '�\���R���狌�N���X�R�[�h�E�����k�ԍ��擾�i�N���X�R�[�h�Q���Ή��j
                    If Trim(OBJ_DATAREADER.Item("YOBI3_O")).Length = 8 Then
                        strCSV_OldClass = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(0, 1)
                        strCSV_OldSeitoNo = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(1, 7)
                    Else
                        strCSV_OldClass = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(0, 2)
                        strCSV_OldSeitoNo = Trim(OBJ_DATAREADER.Item("YOBI3_O")).Substring(2, 7)
                    End If

                    '�i�w�f�[�^�o�� 
                    WriteLine(fnbr, "0", .Item("CLASS_CODE_O"), _
                        Trim(.Item("SEITO_NO_O")), _
                        Trim(.Item("GAKKOU_CODE_O")), _
                        Trim(.Item("NENDO_O")), _
                        .Item("TUUBAN_O"), _
                        Trim(.Item("SEITO_KNAME_O")), _
                        strCSV_NName, _
                        "1", _
                        Trim(.Item("GAKKOU_NNAME_G")), _
                        strCSV_OldClass, _
                        strCSV_OldSeitoNo, " ", " ")

                End With
            End While

            OBJ_DATAREADER.Close()

            '�b�r�u�t�@�C���N���[�Y
            FileClose(fnbr)
            GSUB_LOG_OUT("", "", "�i�w�f�[�^�쐬", "�ڏo", "����", "")

        Catch ex As Exception
            Call GSUB_LOG(0, "�b�r�u�t�@�C���o��")
            Call GSUB_MESSAGE_WARNING("�b�r�u�t�@�C���̍쐬�Ɏ��s���܂���" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Exit Function
        End Try

        Call ChDir(strDIR)
        PFUNC_CSVFileWrite = True

    End Function

    Private Function PFUNC_NullCheck(ByVal objData As Object) As Integer
        '===========================================
        '���l�^�̃f�[�^�x�[�X���ڂ̂m�������`�F�b�N
        '===========================================
        Try
            '�m�������̏ꍇ�͂O��Ԃ�
            If objData.GetType.Name = "DBNull" Then
                PFUNC_NullCheck = 0
            Else
                PFUNC_NullCheck = objData
            End If
        Catch ex As Exception
            Call GSUB_MESSAGE_WARNING("�f�[�^�̂m�������`�F�b�N�Ɏ��s���܂���" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Exit Function
        End Try

    End Function

    Private Function PFUNC_NullCheckST(ByVal objData As Object) As String
        '===========================================
        '���l�^�̃f�[�^�x�[�X���ڂ̂m�������`�F�b�N
        '===========================================
        Try
            '�m�������̏ꍇ�͋󔒂�Ԃ�
            If objData.GetType.Name = "DBNull" Then
                PFUNC_NullCheckST = ""
            Else
                PFUNC_NullCheckST = objData
            End If
        Catch ex As Exception
            Call GSUB_MESSAGE_WARNING("�f�[�^�̂m�������`�F�b�N�Ɏ��s���܂���" & vbCrLf & ex.Message)
            Call ChDir(strDIR)
            Return ""
        End Try

    End Function

#End Region

#Region " Private Sub "
    '****************************
    'Private Function
    '****************************
    Private Sub PSUB_ComboNameSet(ByVal intIndex As Integer)
        '===================================
        '�R���{�{�b�N�X�Ɋw�Z�����Z�b�g����
        '===================================
        If GakkouData(intIndex).cmbKana.Text = "" Then
            Exit Sub
        End If

        '�w�Z����
        Call GFUNC_DB_COMBO_SET(GakkouData(intIndex).cmbKana, GakkouData(intIndex).cmbGakkouName)

    End Sub

    Private Sub PSUB_TextNameSet(ByVal intIndex As Integer)
        '=====================================
        '�e�L�X�g�{�b�N�X�Ɋw�Z�����Z�b�g����
        '=====================================
        If GakkouData(intIndex).cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '�w�Z������̊w�Z�R�[�h�ݒ�
        GakkouData(intIndex).txtGakkouCode.Text = _
            GakkouData(intIndex).strGakkouCode(GakkouData(intIndex).cmbGakkouName.SelectedIndex())

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        GakkouData(intIndex).labGakkouName.Text = GakkouData(intIndex).cmbGakkouName.Text
        GakkouData(intIndex).txtGakkouCode.Focus()

    End Sub

    Private Sub PSUB_StructureSet()
        '=====================================
        '�\���̂Ɋe�t�H�[����ݒ肷��
        '=====================================
        GakkouData(1).cmbKana = cmbKana1
        GakkouData(1).cmbGakkouName = cmbGakkouName1
        GakkouData(1).txtGakkouCode = txtGakkouCode1
        GakkouData(1).labGakkouName = labGakkouName1

        GakkouData(2).cmbKana = cmbKana2
        GakkouData(2).cmbGakkouName = cmbGakkouName2
        GakkouData(2).txtGakkouCode = txtGakkouCode2
        GakkouData(2).labGakkouName = labGakkouName2

        GakkouData(3).cmbKana = cmbKana3
        GakkouData(3).cmbGakkouName = cmbGakkouName3
        GakkouData(3).txtGakkouCode = txtGakkouCode3
        GakkouData(3).labGakkouName = labGakkouName3

        GakkouData(4).cmbKana = cmbKana4
        GakkouData(4).cmbGakkouName = cmbGakkouName4
        GakkouData(4).txtGakkouCode = txtGakkouCode4
        GakkouData(4).labGakkouName = labGakkouName4

        GakkouData(5).cmbKana = cmbKana5
        GakkouData(5).cmbGakkouName = cmbGakkouName5
        GakkouData(5).txtGakkouCode = txtGakkouCode5
        GakkouData(5).labGakkouName = labGakkouName5

        GakkouData(6).cmbKana = cmbKana6
        GakkouData(6).cmbGakkouName = cmbGakkouName6
        GakkouData(6).txtGakkouCode = txtGakkouCode6
        GakkouData(6).labGakkouName = labGakkouName6

    End Sub

#End Region

    Private Sub txtGakkouCode1_Validating(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    txtGakkouCode1.Validating, _
    txtGakkouCode2.Validating, _
    txtGakkouCode3.Validating, _
    txtGakkouCode4.Validating, _
    txtGakkouCode5.Validating, _
    txtGakkouCode6.Validating

        '�w�Z�R�[�h
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then

                Select Case .Name
                    Case "txtGakkouCode1"
                        '�w�Z���̎擾
                        If PFUNC_GakNameGet(1) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode2"
                        '�w�Z���̎擾
                        If PFUNC_GakNameGet(2) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode3"
                        '�w�Z���̎擾
                        If PFUNC_GakNameGet(3) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode4"
                        '�w�Z���̎擾
                        If PFUNC_GakNameGet(4) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode5"
                        '�w�Z���̎擾
                        If PFUNC_GakNameGet(5) <> 0 Then
                            Exit Sub
                        End If
                    Case "txtGakkouCode6"
                        '�w�Z���̎擾
                        If PFUNC_GakNameGet(6) <> 0 Then
                            Exit Sub
                        End If
                End Select
            Else
                Select Case .Name
                    Case "txtGakkouCode1"
                        labGakkouName1.Text = ""
                    Case "txtGakkouCode2"
                        labGakkouName2.Text = ""
                    Case "txtGakkouCode3"
                        labGakkouName3.Text = ""
                    Case "txtGakkouCode4"
                        labGakkouName4.Text = ""
                    Case "txtGakkouCode5"
                        labGakkouName5.Text = ""
                    Case "txtGakkouCode6"
                        labGakkouName6.Text = ""
                End Select
            End If
        End With
    End Sub

#Region " SELECT edIndexChanged(ComboBox) "
    '****************************
    'SelectedIndexChanged
    '****************************
    Private Sub cmbKana1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana1.SelectedIndexChanged
        PSUB_ComboNameSet(1)
        GakkouData(1).strGakkouCode = STR_GCOAD.Clone '�w�Z�R�[�h�擾�p�z��̍Ē�`
    End Sub

    Private Sub cmbGakkouName1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName1.SelectedIndexChanged
        PSUB_TextNameSet(1)
    End Sub

    Private Sub cmbKana2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana2.SelectedIndexChanged
        PSUB_ComboNameSet(2)
        GakkouData(2).strGakkouCode = STR_GCOAD.Clone '�w�Z�R�[�h�擾�p�z��̍Ē�`
    End Sub

    Private Sub cmbGakkouName2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName2.SelectedIndexChanged
        PSUB_TextNameSet(2)
    End Sub

    Private Sub cmbKana3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana3.SelectedIndexChanged
        PSUB_ComboNameSet(3)
        GakkouData(3).strGakkouCode = STR_GCOAD.Clone '�w�Z�R�[�h�擾�p�z��̍Ē�`
    End Sub

    Private Sub cmbGakkouName3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName3.SelectedIndexChanged
        PSUB_TextNameSet(3)
    End Sub

    Private Sub cmbKana4_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana4.SelectedIndexChanged
        PSUB_ComboNameSet(4)
        GakkouData(4).strGakkouCode = STR_GCOAD.Clone '�w�Z�R�[�h�擾�p�z��̍Ē�`
    End Sub

    Private Sub cmbGakkouName4_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName4.SelectedIndexChanged
        PSUB_TextNameSet(4)
    End Sub

    Private Sub cmbKana5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana5.SelectedIndexChanged
        PSUB_ComboNameSet(5)
        GakkouData(5).strGakkouCode = STR_GCOAD.Clone '�w�Z�R�[�h�擾�p�z��̍Ē�`
    End Sub

    Private Sub cmbGakkouName5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName5.SelectedIndexChanged
        PSUB_TextNameSet(5)
    End Sub

    Private Sub cmbKana6_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana6.SelectedIndexChanged
        PSUB_ComboNameSet(6)
        GakkouData(6).strGakkouCode = STR_GCOAD.Clone '�w�Z�R�[�h�擾�p�z��̍Ē�`
    End Sub

    Private Sub cmbGakkouName6_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName6.SelectedIndexChanged
        PSUB_TextNameSet(6)
    End Sub

#End Region

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        '==========================
        '���s�{�^��
        '==========================

        '���̓`�F�b�N
        If PFUNC_CommonCheck() = False Then
            Exit Sub
        End If

        '�V�����}�X�^�擾
        If PFUNC_SEITOMAST2_Check() = False Then
            Exit Sub
        End If

        If MessageBox.Show("�Ĉ�����܂����H", STR_SYORI_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Exit Sub
        End If

        '�����͂��Ȃ�
        ''�i�w���k���擾
        'If PFUNC_SEITOMAST_Check() = False Then
        '    Exit Sub
        'End If

        ''�i�w���k�o�^����
        'If PFUNC_InsSinnyusei() = False Then
        '    Exit Sub
        'End If

        '���[�������
        If PFUNC_PrintMeisai() = False Then
            Exit Sub
        End If

        ''2008/03/09 �d�l�ύX�ɂ��R�����g
        ''�b�r�u�t�@�C���o��
        'If PFUNC_CSVFileWrite() = False Then
        '    Exit Sub
        'End If

        GSUB_LOG_OUT(GakkouData(6).txtGakkouCode.Text, "", "�i�w�f�[�^�쐬", "���[�Ĉ������", "����", "")  '2008/03/23 �����������O�ǉ�
        GSUB_MESSAGE_INFOMATION("�Ĉ�����܂���")

    End Sub
End Class
