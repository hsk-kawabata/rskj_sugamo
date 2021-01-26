Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' ���t����͉��
''' </summary>
''' <remarks></remarks>
Public Class KFSMAIN141

#Region "�N���X�萔"
    Private Const msgTitle As String = "���t����͉��(KFSMAIN141)"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Const JYOUKEN As String = " AND SYOUGOU_KBN_T = '1'"    '�ƍ��v�̐�̂ݑΏ�
#End Region

#Region "�N���X�ϐ�"
    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN141", "���t����͉��")
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U����
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    Private Structure TORIMAST_INF
        Dim FSYORI_KBN As String
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim BAITAI_CODE As String
        Dim BAITAI_KANRI_CODE As String
        Dim SYUBETU_CODE As String
        Dim ITAKU_CODE As String
        Dim ITAKU_NNAME As String
    End Structure
    Private TR As TORIMAST_INF

    '���t����тs�a�k�L�[���ڕێ�
    Private Structure RecordInformation
        Dim ListView As ListView
        Dim psFSYORI_KBN As String        '�U�֏����敪
        Dim psTORIS_CODE As String        '������
        Dim psTORIF_CODE As String        '����敛
        Dim psENTRY_DATE As String        '���t����͓�
        Dim psBAITAI_KANRI_CODE As String '�}�̊Ǘ��R�[�h�@�����P�[�X�ԍ�
        Dim psITAKU_CODE As String        '�ϑ��҃R�[�h
        Dim psSTATION_ENTRY_NO As String  '�[���ԍ�
        Dim psSYUBETU_CODE As String      '��ʃR�[�h
        Dim psFURI_DATE As String         '�U����
        Dim psBAITAI_CODE As String       '�}�̃R�[�h
        Dim psENTRY_NO As Integer         '�ʔ�
        Dim psSYORI_KEN As Long           '����
        Dim psSYORI_KIN As Long           '���z
        Dim psDELETE_FLG As Integer       '�폜�e�k�f
        Dim psFORCE_FLG As Integer        '�����o�^�e�k�f
        Dim psUPLOAD_DATE As String       '�A�b�v���[�h���t
        Dim psUPLOAD_FLG As Integer       '�A�b�v���[�h�e�k�f
        Dim psCHECK_FLG As Integer        '�ƍ�FLG
        Dim psENTRY_OP As String          '�S���҂h�c(���t����́j
        Dim psUPDATE_OP As String         '�S���҂h�c�i���t��ύX�j
        Dim psDELETE_OP As String         '�S���҂h�c�i���t��폜�j
        Dim psNewFuriDate As String       '�ύX��U����
        Dim psITAKUNNAME As String        '�ϑ��Җ�
        Dim psIN_DATE As String           '�}�̓��ɓ�
        Dim psSTATION_IN_NO As String     '���ɒ[���ԍ�
        Dim pnIN_COUNTER As Integer       '��t�ʔ�
        Dim pnHENKOU_SW As Integer        '�ύX�L��
    End Structure
    Private RecInf As RecordInformation

    'ListView Control
    Public WriteOnly Property SetListView() As ListView
        Set(ByVal Value As ListView)
            RecInf.ListView = Value
        End Set
    End Property

    '�U�֏����敪
    Public WriteOnly Property SetpsFSYORI_KBN() As String
        Set(ByVal Value As String)
            RecInf.psFSYORI_KBN = Value
        End Set
    End Property

    '������
    Public WriteOnly Property SetpsTORIS_CODE() As String
        Set(ByVal Value As String)
            RecInf.psTORIS_CODE = Value
        End Set
    End Property

    '����敛
    Public WriteOnly Property SetpsTORIF_CODE() As String
        Set(ByVal Value As String)
            RecInf.psTORIF_CODE = Value
        End Set
    End Property

    '���t����͓�
    Public WriteOnly Property SetpsENTRY_DATE() As String
        Set(ByVal Value As String)
            RecInf.psENTRY_DATE = Value
        End Set
    End Property

    '�}�̊Ǘ��R�[�h�@�����P�[�X�ԍ�
    Public WriteOnly Property SetpsBAITAI_KANRI_CODE() As String
        Set(ByVal Value As String)
            RecInf.psBAITAI_KANRI_CODE = Value
        End Set
    End Property

    '�ϑ��҃R�[�h
    Public WriteOnly Property SetpsITAKU_CODE() As String
        Set(ByVal Value As String)
            RecInf.psITAKU_CODE = Value
        End Set
    End Property

    '�[���ԍ�
    Public WriteOnly Property SetpsSTATION_ENTRY_NO() As String
        Set(ByVal Value As String)
            RecInf.psSTATION_ENTRY_NO = Value
        End Set
    End Property

    '��ʃR�[�h
    Public WriteOnly Property SetpsSYUBETU_CODE() As String
        Set(ByVal Value As String)
            RecInf.psSYUBETU_CODE = Value
        End Set
    End Property

    '�U����
    Public WriteOnly Property SetpsFURI_DATE() As String
        Set(ByVal Value As String)
            RecInf.psFURI_DATE = Value
        End Set
    End Property

    '�}�̃R�[�h
    Public WriteOnly Property SetpsBAITAI_CODE() As String
        Set(ByVal Value As String)
            RecInf.psBAITAI_CODE = Value
        End Set
    End Property

    '�ʔ�
    Public WriteOnly Property SetpsENTRY_NO() As Integer
        Set(ByVal Value As Integer)
            RecInf.psENTRY_NO = Value
        End Set
    End Property

    '����
    Public WriteOnly Property SetpsSYORI_KEN() As Long
        Set(ByVal Value As Long)
            RecInf.psSYORI_KEN = Value
        End Set
    End Property

    '���z
    Public WriteOnly Property SetpsSYORI_KIN() As Long
        Set(ByVal Value As Long)
            RecInf.psSYORI_KIN = Value
        End Set
    End Property

    '�폜�e�k�f
    Public WriteOnly Property SetpsDELETE_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psDELETE_FLG = Value
        End Set
    End Property

    '�����o�^�e�k�f
    Public WriteOnly Property SetpsFORCE_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psFORCE_FLG = Value
        End Set
    End Property

    '�A�b�v���[�h��
    Public WriteOnly Property SetpsUPLOAD_DATE() As String
        Set(ByVal Value As String)
            RecInf.psUPLOAD_DATE = Value
        End Set
    End Property

    '�A�b�v���[�h�e�k�f
    Public WriteOnly Property SetpsUPLOAD_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psUPLOAD_FLG = Value
        End Set
    End Property

    '�ƍ��e�k�f
    Public WriteOnly Property SetpsCHECK_FLG() As Integer
        Set(ByVal Value As Integer)
            RecInf.psCHECK_FLG = Value
        End Set
    End Property

    '�S���҂h�c(���t��V�K���́j
    Public WriteOnly Property SetpsENTRY_OP() As String
        Set(ByVal Value As String)
            RecInf.psENTRY_OP = Value
        End Set
    End Property

    '�S���҂h�c(���t��ύX���́j
    Public WriteOnly Property SetpsUPDATE_OP() As String
        Set(ByVal Value As String)
            RecInf.psUPDATE_OP = Value
        End Set
    End Property

    '�S���҂h�c(���t��폜�j
    Public WriteOnly Property SetpsDELETE_OP() As String
        Set(ByVal Value As String)
            RecInf.psDELETE_OP = Value
        End Set
    End Property

    'NEW�U����
    Public WriteOnly Property SetpsNewFuriDate() As String
        Set(ByVal Value As String)
            RecInf.psNewFuriDate = Value
        End Set
    End Property

    '�ϑ��Җ�
    Public WriteOnly Property SetpsITAKUNNAME() As String
        Set(ByVal Value As String)
            RecInf.psITAKUNNAME = Value
        End Set
    End Property

    '�}�̓��ɓ�
    Public WriteOnly Property SetpsIN_DATE() As String
        Set(ByVal Value As String)
            RecInf.psIN_DATE = Value
        End Set
    End Property

    '���ɒ[���ԍ�
    Public WriteOnly Property SetpsSTATION_IN_NO() As String
        Set(ByVal Value As String)
            RecInf.psSTATION_IN_NO = Value
        End Set
    End Property

    '��t�ʔ�
    Public WriteOnly Property SetpsIN_COUNTER() As Integer
        Set(ByVal Value As Integer)
            RecInf.pnIN_COUNTER = Value
        End Set
    End Property

    '�ύX�L��
    Public WriteOnly Property SetpnHENKOU_SW() As Integer
        Set(ByVal Value As Integer)
            RecInf.pnHENKOU_SW = Value
        End Set
    End Property

    Public nHeadKoumokuCnt As Integer = 0
    Private nKyouseitouroku As Integer               '�����o�^�e�k�f
    Private sSetDateTime As String                   '�����J�n����

    '�ʔԌ�
    Private TUUBAN_CNT As Integer = 4   '�����l�S��

    '�ʔԕ\���p�R���g���[��
    Private TUUBAN_LBL() As Label
    Private TUUBAN_VAL() As Label

    '�ʔԏ��
    Private Structure TYP_INFO_TUUBAN
        Dim LABEL As String
        Dim BAITAI As String
    End Structure
    Private INFO_TUUBAN() As TYP_INFO_TUUBAN

    '�ʔԏ��i���̑��j�̔}�̎w��(True:���̒ʔԂŎw�肳�ꂽ�}�̈ȊO�AFalse:INI�t�@�C���Ŏw��(""�ȊO))
    Private BAITAI_OTHER_FLG As Boolean = False
    Private OTHER_BAITAI As String = "" '���̑��p�̔}��

#End Region

#Region "�C�x���g�n���h��"

    ''' <summary>
    ''' ��ʃ��[�h�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFSMAIN141_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '--------------------------------------------------
            '���O�̏����ɕK�v�ȏ��̎擾
            '--------------------------------------------------
            Me.LW.UserID = GCom.GetUserID
            Me.LW.ToriCode = "000000000000"
            Me.LW.FuriDate = "00000000"

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���[�h)�J�n", "����", "")

            '--------------------------------------------------
            '�V�X�e�����t�ƃ��[�U����\��
            '--------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label2, Me.Label3, Me.lblUser, Me.lblDate)

            '--------------------------------------------------
            '�ʔԃR���g���[���̓��I����
            '--------------------------------------------------
            If Not SetTuubanInfo() Then Return

            '�t�H�[���̏�����
            Call FormInitializa()

            '�����J�n����
            sSetDateTime = String.Format("{0:yyyyMMddHHmmss}", System.DateTime.Now)

            '�U�����̔���Ɏg�p
            '�Y�����O��̋x�����̂�����~�ς���B
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�x�����擾)�I��", "���s", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �o�^�{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnTouroku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTouroku.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�o�^)�J�n", "����", "")

            '--------------------------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '--------------------------------------------------
            If Me.fn_check_text() = False Then
                Return
            End If

            RecInf.psNewFuriDate = String.Concat(New String() {Me.txtFuriDateY.Text, Me.txtFuriDateM.Text, Me.txtFuriDateD.Text})
            RecInf.psSYORI_KEN = Me.DeleteComma(Me.txtKensu.Text)
            RecInf.psSYORI_KIN = Me.DeleteComma(Me.txtKingaku.Text)

            '--------------------------------------------------
            '�d�����͂̃`�F�b�N
            '--------------------------------------------------
            Me.MainDB = New CASTCommon.MyOracle
            If Not RecInf.pnHENKOU_SW = 1 Then
                If Me.Double_Input_Judge() = False Then
                    MessageBox.Show(MSG0382W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    Return
                End If
            End If

            '�����}�X�^�̃`�F�b�N
            If Me.CheckTorimast() = False Then
                Return
            End If

            '0:�V�K�o�^  1:�ύX�o�^(KFJMAIN140���I���f�[�^�󂯎��\���������́j
            If RecInf.pnHENKOU_SW = 1 Then
                '�ύX�o�^
                If Me.Update_Section = False Then
                    Me.MainDB.Rollback()
                    Return
                Else
                    Me.MainDB.Commit()
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                '�}�̊Ǘ��͍s��Ȃ��B
                '�����܂ő��t��̓��͏����̂�

                Select Case TR.BAITAI_CODE
                    Case "00"
                        '�}�̃R�[�h���`���̏ꍇ�A�������b�Z�[�W�Ȃ��ł̓o�^

                        '�P�F�����o�^�@�Q�F�ʏ�o�^
                        nKyouseitouroku = 2
                        If Me.Entry_Insert_Section(RecInf.psNewFuriDate) = False Then
                            Me.MainDB.Rollback()
                            Return
                        Else
                            Me.MainDB.Commit()
                            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If

                        'FD�AMT�ACMT�AWEB�`���ADVD�AUSB�AMO�AFDR��������
                    Case "01", "05", "06", "10", "11", "12", "13", "16"

                        'DB�o�^() �P�F�����o�^�@�Q�F�ʏ�o�^
                        nKyouseitouroku = 1
                        If Me.Entry_Insert_Section(RecInf.psNewFuriDate) = False Then
                            Me.MainDB.Rollback()
                            Return
                        Else
                            Me.MainDB.Commit()
                            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If

                        '��L�ȊO�̓G���[
                    Case Else
                        MessageBox.Show(MSG0383W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTorisCode.Focus()
                        Return

                End Select
            End If

            Call FormInitializa()
            Me.txtTorisCode.Focus()

            If RecInf.pnHENKOU_SW = 0 Then
            Else
                '�e��ʂɖ߂�
                Me.Close()
                Me.Dispose()
            End If

        Catch ex As Exception
            Me.MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)

        Finally
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�o�^)�I��", "����", "")

        End Try
    End Sub

    ''' <summary>
    ''' ����{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnTorikesi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTorikesi.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���)�J�n", "����", "")

            '�V�K��ʂւ��ǂ邩�ύX��ʂւ��ǂ邩  
            'RecInf.pnHENKOU_SW = 0 : �V�K���(RecInf.pnHENKOU_SW = 1) : �ύX���
            If RecInf.pnHENKOU_SW = 0 Then
                Call FormInitializa() '������ʂɖ߂�
                Me.txtTorisCode.Focus()
            Else
                '�e��ʂɖ߂�
                RecInf.pnHENKOU_SW = 0
                Me.Close()
                Me.Dispose()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �I���{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�I��)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�I��)��O�G���[", "���s", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�I��)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X���@���f�C�e�B���O�C�x���g�i�[���p�f�B���O�j
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorisCode.Validating, txtTorifCode.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(�[���p�f�B���O)", "���s", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X���@���f�C�e�b�h�C�x���g�i�����擾�j
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FMT_NzNumberString_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles txtTorisCode.Validated, txtTorifCode.Validated

        Me.MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder

        Try
            If (Me.txtTorisCode.Text.Trim & Me.txtTorifCode.Text.Trim).Length <> 12 Then
                Return
            End If

            Call ItakuNameRead(0)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "��ʃ`�F�b�N", "���s", ex.Message)

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
        End Try
    End Sub

    ''' <summary>
    ''' �����J�i�C���f�b�N�X�`�F���W�C�x���g�i�����擾�j
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�I���J�i�Ŏn�܂�ϑ��Җ����擾
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '�����R���{�{�b�N�X�ݒ�
                If GCom.SelectItakuName_View(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "3", JYOUKEN) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            cmbToriName.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��Җ��R���{�{�b�N�X�ݒ�", "���s", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' ����挟���C���f�b�N�X�`�F���W�C�x���g�i�����擾�j
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�����R�[�h���擾
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString.Trim = Nothing Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                Call FMT_NzNumberString_Validated(sender, e)
                Me.txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����R�[�h�ݒ�", "���s", ex.Message)
        End Try
    End Sub

#End Region

#Region "�v���C�x�[�g���\�b�h"

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X�̓��̓`�F�b�N���s���܂��B
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Private Function fn_check_text() As Boolean
        Try
            '������R�[�h�`�F�b�N
            If Me.txtTorisCode.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
                Return False
            End If

            '����敛�R�[�h�`�F�b�N
            If Me.txtTorifCode.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorifCode.Focus()
                Return False
            End If

            '�U�����i�N�j�`�F�b�N
            If Me.txtFuriDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '�U�����i���j�`�F�b�N
            If Me.txtFuriDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            If GCom.NzInt(Me.txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            '�U�����i���j�`�F�b�N
            If Me.txtFuriDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            If GCom.NzInt(Me.txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            '���t�������`�F�b�N
            Dim WORK_DATE As String = Me.txtFuriDateY.Text & "/" & Me.txtFuriDateM.Text & "/" & Me.txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '�c�Ɠ��`�F�b�N
            Dim KyuCode As Integer = 0
            Dim ChangeDate As String = String.Empty
            Dim bRet As Boolean = GCom.CheckDateModule(WORK_DATE, ChangeDate, KyuCode)
            If Not WORK_DATE = ChangeDate Then
                MessageBox.Show(MSG0093W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '�����`�F�b�N
            If Me.txtKensu.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "�˗�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKensu.Focus()
                Return False
            End If

            '���z�`�F�b�N
            If Me.txtKingaku.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "�˗����z"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKingaku.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "�e�L�X�g�{�b�N�X���̓`�F�b�N", "���s", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' ���t��V�K�o�^
    ''' </summary>
    ''' <param name="sSonDateWk">�U����</param>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Private Function Entry_Insert_Section(ByVal sSonDateWk As String) As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim OraReader2 As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���t��V�K�o�^)�J�n", "����", "")

            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

            Dim nTubanMax As Integer
            Dim sIN_DATE_WK As String
            Dim sSTATION_IN_NO_WK As String
            Dim sIN_COUNTER_WK As Integer

            '�ʔԎZ�o(�{���ŏI�ʔ�)
            SQL.Append("SELECT NVL(MAX(ENTRY_NO_ME), 0) + 1 AS NEW_NUMBER FROM MEDIA_ENTRY_TBL")
            '�{�����t
            SQL.Append(" WHERE ENTRY_DATE_ME = " & SQ(String.Format("{0:yyyyMMdd}", GCom.GetSysDate)))
            SQL.Append(" AND STATION_ENTRY_NO_ME = " & SQ(GCom.GetStationNo))
            Dim TARGET_INDEX As Integer = -1
            For i As Integer = 0 To TUUBAN_CNT - 1
                If INFO_TUUBAN(i).LABEL.Trim <> "���̑�" OrElse BAITAI_OTHER_FLG = False Then
                    If INFO_TUUBAN(i).BAITAI.IndexOf(TR.BAITAI_CODE) >= 0 Then
                        TARGET_INDEX = i
                        Exit For
                    End If
                End If
            Next

            If TARGET_INDEX >= 0 Then
                '�}�̂���v�����ꍇ
                SQL.Append(" AND BAITAI_CODE_ME IN (" & INFO_TUUBAN(TARGET_INDEX).BAITAI.Trim & ")")
            Else
                '��v���Ȃ������ꍇ
                If BAITAI_OTHER_FLG Then
                    '�t���O�������Ă���ꍇ�i���̔}�̃R�[�h�ȊO���u���̑��v�Ƃ���ꍇ�j
                    SQL.Append(" AND BAITAI_CODE_ME NOT IN (" & OTHER_BAITAI.Trim & ")")
                    '�u���̑��v�̏ꍇ�A�ŏI�C���f�b�N�X���w��
                    TARGET_INDEX = TUUBAN_CNT - 1
                Else
                    MessageBox.Show(String.Format(MSG0388W, TR.BAITAI_CODE), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "�ʔԎ擾�G���[", "���s", "�}�̃R�[�h�s��v�@�}�̃R�[�h�F" & TR.BAITAI_CODE)
                    Return False
                End If
            End If

            nTubanMax = 1
            If OraReader.DataReader(SQL) = True Then
                nTubanMax = OraReader.GetInt64("NEW_NUMBER")
            End If

            OraReader.Close()

            '�ʔԃ��X�g�p�ɕێ�����B�P:FD�E�U:CMT�D
            nHeadKoumokuCnt = 1       '�P��ł��o�^���s�������ǂ����B

            '�擾�����ʔԂ�\������
            TUUBAN_VAL(TARGET_INDEX).Text = String.Format("{0:0000}", nTubanMax)

            '�����}�X�^�L���m�F
            SQL.Length = 0
            SQL.Append("SELECT * FROM S_TORIMAST_VIEW")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TR.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TR.TORIF_CODE))

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    Me.LW.ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")

                    '�����P�[�X���ɂs�a�k�͂Ȃ�
                    sIN_DATE_WK = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
                    sSTATION_IN_NO_WK = "0"
                    sIN_COUNTER_WK = 0

                    '���t����тs�a�k(---Insert)
                    SQL.Length = 0
                    SQL.Append("INSERT INTO MEDIA_ENTRY_TBL ")
                    SQL.Append("(FSYORI_KBN_ME")               '�U�֏����敪
                    SQL.Append(", TORIS_CODE_ME")              '������
                    SQL.Append(", TORIF_CODE_ME")              '����敛
                    SQL.Append(", ENTRY_DATE_ME")              '���t����͓�
                    SQL.Append(", ITAKU_KANRI_CODE_ME")       '�}�̊Ǘ��R�[�h

                    SQL.Append(", IN_DATE_ME")                 '���ɓ�
                    SQL.Append(", STATION_IN_NO_ME")           '���ɒ[���ԍ�
                    SQL.Append(", IN_COUNTER_ME")              '��t�ʔ�

                    SQL.Append(", ITAKU_CODE_ME")              '�ϑ��҃R�[�h
                    SQL.Append(", STATION_ENTRY_NO_ME")        '�[���ԍ�
                    SQL.Append(", SYUBETU_CODE_ME")            '���
                    SQL.Append(", FURI_DATE_ME")               '�U����
                    SQL.Append(", BAITAI_CODE_ME")             '�}�̃R�[�h
                    SQL.Append(", ENTRY_NO_ME")                '�ʔ�
                    SQL.Append(", SYORI_KEN_ME")               '����
                    SQL.Append(", SYORI_KIN_ME")               '���z
                    SQL.Append(", CYCLE_NO_ME")                '�T�C�N����
                    SQL.Append(", DELETE_FLG_ME")              '�폜�e�k�f
                    SQL.Append(", FORCE_FLG_ME")               '�����o�^�e�k�f
                    SQL.Append(", UPLOAD_DATE_ME")             '�A�b�v���[�h��
                    SQL.Append(", UPLOAD_FLG_ME")              '�A�b�v���[�h�e�k�f
                    SQL.Append(", CHECK_FLG_ME")               '�ƍ��e�k�f
                    SQL.Append(", CHECK_KBN_ME")               '�ƍ��v�ۋ敪
                    SQL.Append(", ENTRY_OP_ME")                '�S���҂h�c�i���t����́j
                    SQL.Append(", UPDATE_OP_ME")               '�S���҂h�c�i���t����͕ύX�j
                    SQL.Append(", DELETE_OP_ME")               '�S���҂h�c�i���t����͍폜�j
                    SQL.Append(", CREATE_DATE_ME")             '�쐬��
                    SQL.Append(", UPDATE_DATE_ME)")            '�X�V��

                    SQL.Append(" Values  (")
                    SQL.Append("'" & GCom.NzDec(OraReader.GetString("FSYORI_KBN_T"), "") & "'")
                    SQL.Append(",'" & GCom.NzDec(OraReader.GetString("TORIS_CODE_T"), "") & "'")
                    SQL.Append(",'" & GCom.NzDec(OraReader.GetString("TORIF_CODE_T"), "") & "'")
                    SQL.Append(",'" & String.Format("{0:yyyyMMdd}", GCom.GetSysDate) & "'")

                    '�}�̊Ǘ��R�[�h�͂Ȃ�
                    SQL.Append(",'0000000000'")

                    SQL.Append(",'" & sIN_DATE_WK & "'")
                    SQL.Append(",'" & sSTATION_IN_NO_WK & "'")
                    SQL.Append(",'" & sIN_COUNTER_WK & "'")

                    SQL.Append(",'" & OraReader.GetString("ITAKU_CODE_T") & "'")
                    SQL.Append(",'" & GCom.GetStationNo & "'")
                    SQL.Append(",'" & OraReader.GetString("SYUBETU_T") & "'")
                    SQL.Append(",'" & RecInf.psNewFuriDate & "'")                '�U����
                    SQL.Append(",'" & String.Format("{0:00}", CType(OraReader.GetString("BAITAI_CODE_T"), Integer)) & "'")
                    SQL.Append(",'" & nTubanMax & "'")
                    SQL.Append(",'" & GCom.NzDec(Me.DeleteComma(Me.txtKensu.Text), "") & "'")       '����
                    SQL.Append(",'" & GCom.NzDec(Me.DeleteComma(Me.txtKingaku.Text), "") & "'")     '���z
                    SQL.Append(",'0'")
                    SQL.Append(",'0'")
                    SQL.Append(",'" & nKyouseitouroku & "'")                           '�����o�^�L��
                    SQL.Append(",'" & Mid(sSetDateTime, 1, 8) & "'")    '�A�b�v���[�h�������g�p���Ȃ����߁A�쐬����ݒ肷�邱�ƂƂ���B
                    SQL.Append(",'1'")                                          '�A�b�v���[�h�������g�p���Ȃ����߁A�t���O�𗧂ĂĂ������ƂƂ���B
                    SQL.Append(",'0'")
                    SQL.Append(",'" & GCom.NzInt(OraReader.GetString("SYOUGOU_KBN_T")) & "'")
                    SQL.Append(",'" & GCom.GetUserID & "'")
                    SQL.Append(",'" & "" & "'")
                    SQL.Append(",'" & "" & "'")
                    SQL.Append(",'" & sSetDateTime & "'")
                    SQL.Append(",'" & sSetDateTime & "'")        '�A�b�v���[�h�������g�p���Ȃ����߁A�쐬����ݒ肷�邱�ƂƂ���B
                    SQL.Append(")")

                    Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
                    If iRet < 1 Then
                        MessageBox.Show(String.Format(MSG0002E, "�o�^"), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "���t����уe�[�u���o�^", "���s", Me.MainDB.Message)
                        Return False
                    End If

                    OraReader.NextRead()
                End While
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "���t����уe�[�u���o�^", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not OraReader2 Is Nothing Then
                OraReader2.Close()
                OraReader2 = Nothing
            End If

            Me.LW.ToriCode = "000000000000"
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���t��V�K�o�^)�I��", "����", "")

        End Try
    End Function

    ''' <summary>
    ''' ���t��X�V
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Private Function Update_Section() As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder
        Dim sFuri_Date As String
        Dim nDec As Integer
        Dim stemp As String

        sFuri_Date = Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text

        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���t��X�V)�J�n", "����", "")

            SQL.Append("SELECT *")
            SQL.Append(" FROM MEDIA_ENTRY_TBL")
            SQL.Append(" WHERE FSYORI_KBN_ME =" & "'" & RecInf.psFSYORI_KBN & "'")
            SQL.Append(" AND TORIS_CODE_ME =" & "'" & RecInf.psTORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_ME =" & "'" & RecInf.psTORIF_CODE & "'")
            SQL.Append(" AND ENTRY_DATE_ME =" & "'" & RecInf.psENTRY_DATE & "'")
            SQL.Append(" AND STATION_ENTRY_NO_ME =" & "'" & RecInf.psSTATION_ENTRY_NO & "'")
            SQL.Append(" AND BAITAI_CODE_ME =" & "'" & RecInf.psBAITAI_CODE & "'")
            SQL.Append(" AND ENTRY_NO_ME =" & "'" & RecInf.psENTRY_NO & "'")

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    Me.LW.ToriCode = RecInf.psTORIS_CODE & RecInf.psTORIF_CODE

                    SQL.Length = 0
                    SQL.Append("UPDATE MEDIA_ENTRY_TBL SET ")
                    SQL.Append(" FURI_DATE_ME = " & "'" & sFuri_Date & "',")                                       '�U����
                    SQL.Append(" STATION_ENTRY_NO_ME = " & GCom.GetStationNo & ",")                                '�[���ԍ�
                    SQL.Append(" SYORI_KEN_ME = " & "'" & GCom.NzDec(Me.DeleteComma(txtKensu.Text), nDec) & "',")                  '����
                    SQL.Append(" SYORI_KIN_ME = " & "'" & GCom.NzDec(Me.DeleteComma(txtKingaku.Text), nDec) & "',")                '���z
                    SQL.Append(" UPDATE_OP_ME = " & "'" & GCom.GetUserID & "',")                                   '�S���҂h�c�i���t���́j
                    SQL.Append(" UPDATE_DATE_ME = " & "'" & String.Format("{0:yyyyMMddHHmmss}", System.DateTime.Now) & "'") '�X�V��
                    SQL.Append(" Where FSYORI_KBN_ME =" & "'" & RecInf.psFSYORI_KBN & "'")
                    SQL.Append(" AND TORIS_CODE_ME =" & "'" & RecInf.psTORIS_CODE & "'")
                    SQL.Append(" AND TORIF_CODE_ME =" & "'" & RecInf.psTORIF_CODE & "'")
                    SQL.Append(" AND ENTRY_DATE_ME =" & "'" & RecInf.psENTRY_DATE & "'")
                    SQL.Append(" AND STATION_ENTRY_NO_ME =" & "'" & RecInf.psSTATION_ENTRY_NO & "'")
                    SQL.Append(" AND BAITAI_CODE_ME =" & "'" & RecInf.psBAITAI_CODE & "'")
                    SQL.Append(" AND ENTRY_NO_ME =" & "'" & RecInf.psENTRY_NO & "'")

                    Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
                    If iRet < 1 Then
                        MessageBox.Show(String.Format(MSG0002E, "�X�V"), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "���t����уe�[�u���X�V", "���s", Me.MainDB.Message)
                        Return False
                    End If

                    OraReader.NextRead()
                End While
            End If

            '�e���ListView�ύX����()
            RecInf.psSYUBETU_CODE = String.Format("{0:00}", lblSyubetuCode.Text)
            Call GCom.SelectedItem(RecInf.ListView, 8, RecInf.psSYUBETU_CODE.ToString)       '���
            stemp = RecInf.psNewFuriDate.Substring(0, 4)
            stemp &= RecInf.psNewFuriDate.Substring(4, 2)
            stemp &= RecInf.psNewFuriDate.Substring(6)
            Call GCom.SelectedItem(RecInf.ListView, 9, stemp)                                '�U����
            stemp = String.Format("{0:#,##0}", RecInf.psSYORI_KEN)
            Call GCom.SelectedItem(RecInf.ListView, 10, stemp)                               '�˗�����
            stemp = String.Format("{0:#,##0}", RecInf.psSYORI_KIN)
            Call GCom.SelectedItem(RecInf.ListView, 11, stemp)

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "���t����уe�[�u���X�V", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

    End Function

    ''' <summary>
    ''' ��ʂ̓��͓��e�����������܂��B
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FormInitializa()
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            GroupBox1.Visible = True
            lblItakuName.Text = ""
            lblSyubetuName.Text = ""

            txtFuriDateY.Text = ""
            txtFuriDateD.Text = ""
            txtFuriDateM.Text = ""
            txtKensu.Text = ""
            txtKingaku.Text = ""
            nKyouseitouroku = 0

            Me.txtTorisCode.Text = String.Empty
            Me.txtTorifCode.Text = String.Empty
            Me.lblItakuCode.Text = String.Empty
            Me.lblSyubetuCode.Text = String.Empty
            If GCom.SelectItakuName_View("", cmbToriName, txtTorisCode, txtTorifCode, "3", JYOUKEN) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            If RecInf.pnHENKOU_SW = 0 Then
                Me.txtTorisCode.ReadOnly = False
                Me.txtTorifCode.ReadOnly = False
                Me.cmbKana.Enabled = True
                Me.cmbToriName.Enabled = True
            Else
                '�ύX�̏ꍇ(KFJMAIN142���f�[�^�󂯓n���j���t����͉�ʂɕ\��
                GroupBox1.Visible = False

                Me.txtTorisCode.ReadOnly = True
                Me.txtTorifCode.ReadOnly = True
                Me.cmbKana.Enabled = False
                Me.cmbToriName.Enabled = False

                SQL.Append("SELECT * ")
                SQL.Append(" FROM MEDIA_ENTRY_TBL")
                SQL.Append(" WHERE ")
                SQL.Append(" FSYORI_KBN_ME =" & "'" & RecInf.psFSYORI_KBN & "'")         '�����敪�i�ύX��ʂ���j
                SQL.Append(" AND TORIS_CODE_ME =" & "'" & RecInf.psTORIS_CODE & "'")     '������CD(�ύX��ʂ���j
                SQL.Append(" AND TORIF_CODE_ME =" & "'" & RecInf.psTORIF_CODE & "'")     '����敛CD(�ύX��ʂ���j
                SQL.Append(" AND ENTRY_DATE_ME =" & "'" & RecInf.psENTRY_DATE & "'")     '���t����͓�(�ύX��ʂ���j
                SQL.Append(" AND STATION_ENTRY_NO_ME =" & "'" & RecInf.psSTATION_ENTRY_NO & "'")    '�[���ԍ��i�ύX��ʂ���j
                SQL.Append(" AND BAITAI_CODE_ME =" & "'" & RecInf.psBAITAI_CODE & "'")   '�}�̃R�[�h�i�ύX��ʂ���j
                SQL.Append(" AND ENTRY_NO_ME =" & "'" & RecInf.psENTRY_NO & "'")         '��t�ʔԁi�ύX��ʂ���j

                '�������̌Ăяo�����͂��ׂĐڑ����Ă��Ȃ��͂������A�O�̂���
                If Not Me.MainDB Is Nothing Then
                    Me.MainDB.Close()
                    Me.MainDB = Nothing
                End If

                Me.MainDB = New CASTCommon.MyOracle
                OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        Me.lblItakuCode.Text = GCom.NzStr(OraReader.GetString("ITAKU_CODE_ME"))
                        Me.lblSyubetuCode.Text = String.Format("{0:00}", GCom.NzStr(OraReader.GetString("SYUBETU_CODE_ME")))
                        lblSyubetuName.Text = Me.SetSyubetuName(Me.lblSyubetuCode.Text)
                        txtFuriDateY.Text = GCom.NzStr(OraReader.GetString("FURI_DATE_ME")).Substring(0, 4)
                        txtFuriDateM.Text = GCom.NzStr(OraReader.GetString("FURI_DATE_ME")).Substring(4, 2)
                        txtFuriDateD.Text = GCom.NzStr(OraReader.GetString("FURI_DATE_ME")).Substring(6, 2)
                        txtKensu.Text = String.Format("{0:#,##0}", GCom.NzDec(OraReader.GetString("SYORI_KEN_ME"), 0))
                        txtKingaku.Text = String.Format("{0:#,##0}", GCom.NzDec(OraReader.GetString("SYORI_KIN_ME"), 0))
                        Me.txtTorisCode.Text = GCom.NzStr(OraReader.GetString("TORIS_CODE_ME"))
                        Me.txtTorifCode.Text = GCom.NzStr(OraReader.GetString("TORIF_CODE_ME"))
                        Call ItakuNameRead(1)

                        OraReader.NextRead()
                    End While
                End If

                OraReader.Close()
                OraReader = Nothing

                Me.MainDB.Close()
                Me.MainDB = Nothing

            End If

            '*** �����\���̎��ɍŏI�ʔԕ\��         
            For i As Integer = 0 To TUUBAN_CNT - 1
                TUUBAN_VAL(i).Text = "0000"
            Next

            '�ʔԎZ�o(�{���ŏI�ʔ�)
            SQL.Length = 0
            For i As Integer = 0 To TUUBAN_CNT - 1
                If i > 0 Then
                    SQL.Append(" UNION")
                End If
                SQL.Append(" SELECT NVL(MAX(ENTRY_NO_ME),0) AS NEW_NUMBER, " & i.ToString & " AS BAITAI_CODE FROM MEDIA_ENTRY_TBL")
                SQL.Append(" WHERE ENTRY_DATE_ME = " & SQ(String.Format("{0:yyyyMMdd}", GCom.GetSysDate)))
                SQL.Append(" AND STATION_ENTRY_NO_ME = " & SQ(GCom.GetStationNo))
                If INFO_TUUBAN(i).LABEL = "���̑�" AndAlso BAITAI_OTHER_FLG Then
                    '�u���̑��v�̏ꍇ
                    SQL.Append(" AND BAITAI_CODE_ME NOT IN (" & OTHER_BAITAI & ")")
                Else
                    SQL.Append(" AND BAITAI_CODE_ME IN (" & INFO_TUUBAN(i).BAITAI.Trim & ")")
                End If
            Next

            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False

                    TUUBAN_VAL(OraReader.GetInt("BAITAI_CODE")).Text = String.Format("{0:0000}", OraReader.GetInt("NEW_NUMBER"))

                    OraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "������", "���s", ex.Message)

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
        End Try
    End Sub

    ''' <summary>
    ''' �ϑ��ҏ����擾���܂��B
    ''' </summary>
    ''' <param name="ErrorSw"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ItakuNameRead(ByVal ErrorSw As Integer) As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            '�����擾���@�ύX
            With TR
                .FSYORI_KBN = ""
                .TORIS_CODE = GCom.NzDec(Me.txtTorisCode.Text, "")
                .TORIF_CODE = GCom.NzDec(Me.txtTorifCode.Text, "")
                .BAITAI_CODE = ""
                .BAITAI_KANRI_CODE = ""
                .SYUBETU_CODE = ""
                .ITAKU_CODE = ""
                .ITAKU_NNAME = ""
            End With

            If (TR.TORIS_CODE.Trim & TR.TORIF_CODE.Trim).Length = 0 Then

                lblItakuName.Text = ""

                TR.BAITAI_CODE = ""

                Return False
            Else
                SQL.Append("SELECT FSYORI_KBN_T")
                SQL.Append(", TORIS_CODE_T")
                SQL.Append(", TORIF_CODE_T")
                SQL.Append(", BAITAI_CODE_T")
                SQL.Append(", '0000000000' AS BAITAI_KANRI_CODE_T")
                SQL.Append(", SYUBETU_T")
                SQL.Append(", ITAKU_CODE_T")
                SQL.Append(", ITAKU_NNAME_T")
                SQL.Append(" FROM S_TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TR.TORIS_CODE))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(TR.TORIF_CODE))

                OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        With TR
                            .FSYORI_KBN = String.Format("{0:0}", GCom.NzDec(OraReader.GetItem("FSYORI_KBN_T"), 0))
                            .BAITAI_CODE = String.Format("{0:00}", GCom.NzDec(OraReader.GetItem("BAITAI_CODE_T"), 0))
                            .BAITAI_KANRI_CODE = String.Format("{0:0000000000}", GCom.NzDec(OraReader.GetItem("BAITAI_KANRI_CODE_T"), 0))
                            .SYUBETU_CODE = String.Format("{0:00}", GCom.NzDec(OraReader.GetItem("SYUBETU_T"), 0))
                            .ITAKU_CODE = String.Format("{0:0000000000}", GCom.NzDec(OraReader.GetItem("ITAKU_CODE_T"), 0))
                            .ITAKU_NNAME = GCom.NzStr(OraReader.GetItem("ITAKU_NNAME_T")).Trim
                        End With

                        lblItakuName.Text = GCom.GetLimitString(TR.ITAKU_NNAME, 50)
                        Me.lblSyubetuCode.Text = TR.SYUBETU_CODE
                        Me.lblItakuCode.Text = TR.ITAKU_CODE

                        Me.lblSyubetuName.Text = SetSyubetuName(TR.SYUBETU_CODE)

                        OraReader.NextRead()
                    End While

                Else
                    lblItakuName.Text = ""
                    lblSyubetuName.Text = ""

                    TR.BAITAI_CODE = ""

                    Me.lblItakuCode.Text = String.Empty
                    Me.lblSyubetuCode.Text = String.Empty

                    If ErrorSw = 0 Then
                    Else
                        MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If
            End If

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return True

    End Function


    Private Function SetSyubetuName(ByVal strSYUBETU As String) As String
        Select Case strSYUBETU
            Case "91" : Return "���U"
            Case "21" : Return "���U"
            Case "11" : Return "���^"
            Case "12" : Return "�ܗ^"
            Case "71" : Return "���������^"
            Case "72" : Return "�������ܗ^"
            Case Else : Return ""
        End Select
    End Function

    ''' <summary>
    ''' ��d���̓`�F�b�N
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>�S���ړ����f�[�^�����݂̏ꍇ�A�d�����͂Ƃ���</remarks>
    Private Function Double_Input_Judge() As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder

        Try
            SQL.Append("SELECT * FROM MEDIA_ENTRY_TBL")
            SQL.Append(" WHERE TORIS_CODE_ME = " & SQ(Me.txtTorisCode.Text))
            SQL.Append(" AND TORIF_CODE_ME = " & SQ(Me.txtTorifCode.Text))
            SQL.Append(" AND FURI_DATE_ME = " & SQ(RecInf.psNewFuriDate))
            SQL.Append(" AND SYORI_KEN_ME = " & GCom.NzDec(Me.txtKensu.Text, ""))
            SQL.Append(" AND SYORI_KIN_ME = " & GCom.NzDec(Me.txtKingaku.Text, ""))
            SQL.Append(" AND DELETE_FLG_ME <> '1'")
            SQL.Append(" AND FSYORI_KBN_ME = '3'")

            If OraReader.DataReader(SQL) = True Then
                '���݂���̂�NG
                Return False
            Else
                '���݂��Ȃ��̂�OK
                Return True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "��d���̓`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' �J���}�폜��̐��l��Ԃ��܂��B
    ''' </summary>
    ''' <param name="strTarget">�ϊ��Ώې��l������</param>
    ''' <returns>�J���}�폜�㐔�l</returns>
    ''' <remarks></remarks>
    Private Function DeleteComma(ByVal strTarget As String) As Long
        Dim strNumber As String = strTarget.Replace(",", "").Replace(" ", "")
        Return GCom.NzLong(strNumber)
    End Function

    ''' <summary>
    ''' �����}�X�^�̃`�F�b�N���s���܂��B
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks>2014/11/27 saitou �L��M�� added</remarks>
    Private Function CheckTorimast() As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("select * from S_TORIMAST_VIEW")
                .Append(" where TORIS_CODE_T = " & SQ(Me.txtTorisCode.Text))
                .Append(" and TORIF_CODE_T = " & SQ(Me.txtTorifCode.Text))
            End With

            If OraReader.DataReader(SQL) = True Then
                If OraReader.GetString("SYOUGOU_KBN_T") <> "1" Then
                    MessageBox.Show(MSG0384W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    Return False
                End If
            Else
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "�����}�X�^�`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try
    End Function


    ''' <summary>
    ''' �ʔԃR���g���[���̓��I����
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SetTuubanInfo() As Boolean
        Try
            Dim WK As String = ""

            '�ʔԌ��擾
            WK = GetRSKJIni("FORM", "KFSMAIN141_MAXTUUBAN").ToUpper
            If WK = "" OrElse Not IsNumeric(WK) Then
                '�����l�̂܂�
            Else
                TUUBAN_CNT = CInt(WK)
            End If

            ReDim TUUBAN_LBL(TUUBAN_CNT - 1)
            ReDim TUUBAN_VAL(TUUBAN_CNT - 1)
            ReDim INFO_TUUBAN(TUUBAN_CNT - 1)

            For i As Integer = 1 To TUUBAN_CNT
                WK = GetRSKJIni("FORM", "KFSMAIN141_TUUBAN" & i.ToString).ToUpper
                If WK = "" OrElse WK = "ERR" Then
                    MessageBox.Show(String.Format(MSG0001E, "�}�̒ʔ�" & i.ToString, "FORM", "KFSMAIN141_TUUBAN" & i.ToString), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�}�̒ʔ�" & i.ToString & " ����:FORM ����:KFSMAIN141_TUUBAN" & i.ToString)
                    Return False
                Else
                    Dim strBAITAI() As String = WK.Split(":"c)
                    If strBAITAI.Length <> 2 Then
                        MessageBox.Show(String.Format(MSG0035E, "�}�̒ʔ�" & i.ToString, "FORM", "KFSMAIN141_TUUBAN" & i.ToString), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�}�̒ʔ�" & i.ToString & " ����:FORM ����:KFSMAIN141_TUUBAN" & i.ToString)
                        Return False
                    Else
                        With INFO_TUUBAN(i - 1)
                            .LABEL = strBAITAI(0)
                            .BAITAI = strBAITAI(1)

                            '�}�̖����u���̑��v�ȊO�̔}�̃R�[�h�����W����
                            If Trim(.LABEL) <> "���̑�" Then
                                If i = 1 Then
                                    OTHER_BAITAI = .BAITAI.Trim
                                Else
                                    OTHER_BAITAI &= ("," & .BAITAI.Trim)
                                End If
                            Else
                                If .BAITAI.Trim = "" Then
                                    '�}�̖��u���̑��v�̔}�̃R�[�h= NOT IN (OTHER_BAITAI)
                                    BAITAI_OTHER_FLG = True
                                End If
                            End If
                        End With
                    End If
                End If
            Next

            For i As Integer = 0 To TUUBAN_CNT - 1
                '���x���̐ݒ�
                TUUBAN_LBL(i) = New Label
                With TUUBAN_LBL(i)
                    .Text = INFO_TUUBAN(i).LABEL
                    .Font = New Font("�l�r �S�V�b�N", 9)
                    .BorderStyle = BorderStyle.None
                    .TextAlign = ContentAlignment.MiddleRight
                    .Size = New System.Drawing.Size(New System.Drawing.Point(102, 20))
                    .Location = New System.Drawing.Point(50, (i * 24) + 28)
                End With

                '�ʔԕ����̐ݒ�
                TUUBAN_VAL(i) = New Label
                With TUUBAN_VAL(i)
                    .Text = "0000"
                    .Font = New Font("�l�r �S�V�b�N", 9)
                    .BorderStyle = BorderStyle.Fixed3D
                    .TextAlign = ContentAlignment.MiddleCenter
                    .Size = New System.Drawing.Size(New System.Drawing.Point(90, 20))
                    .Location = New System.Drawing.Point(180, (i * 24) + 28)
                End With

                GroupBox1.Controls.Add(TUUBAN_LBL(i))
                GroupBox1.Controls.Add(TUUBAN_VAL(i))
            Next

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "�ʔԃR���g���[������", "���s", ex.Message)
            Return False
        End Try
    End Function
#End Region

End Class
