Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' ���ɏ��ꗗ���
''' </summary>
''' <remarks>2017/11/27 �W���� added for �ƍ��Ή�(MASTPTN2�p)</remarks>
Public Class KFJMAIN122

#Region "�N���X�萔"
    Private Const msgTitle As String = "���ɏ��ꗗ���(KFJMAIN122)"
    Private CAST As New CASTCommon.Events

#End Region

#Region "�N���X�ϐ�"
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN122", "���ɏ��ꗗ���")
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    '���t����тs�a�k�L�[���ڕێ�
    Private Structure RecordInformation
        Dim ListView As ListView
        Dim psFSYORI_KBN As String        '�U�֏����敪
        Dim psTORIS_CODE As String        '������
        Dim psTORIF_CODE As String        '����敛
        Dim psENTRY_DATE As String        '���t����͓�
        Dim psBAITAI_KANRI_CODE As String '�}�̊Ǘ��R�[�h�@�����P�[�X�ԍ�
        Dim psITAKU_CODE As String        '�ϑ��҃R�[�h
        Dim psSTATION_ENTRY_NO As String        '�[���ԍ�
        Dim psSYUBETU_CODE As String      '��ʃR�[�h
        Dim psFURI_DATE As String         '�U�֓�
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
        Dim psNewFuriDate As String       '�ύX��U�֓�
        Dim psITAKUNNAME As String        '�ϑ��Җ�

        Dim psIN_DATE As String           '�}�̓��ɓ�
        Dim psSTATION_IN_NO As String     '���ɒ[���ԍ�
        Dim pnIN_COUNTER As Integer       '��t�ʔ�
    End Structure
    Private RecInf As RecordInformation

    'ListView Control
    Public WriteOnly Property SetListView() As ListView
        Set(ByVal Value As ListView)
            RecInf.ListView = Value
        End Set
    End Property

    Public Property SELECT_DATE() As String
        Get
            Return strSELECT_DATE
        End Get
        Set(ByVal value As String)
            strSELECT_DATE = value
        End Set
    End Property
    Private strSELECT_DATE As String

    '�����ғ����
    Private MyOwnerForm As Form
    '�\�[�g�I�[�_�[�t���O
    Private SortOrderFlag As Boolean = True

    '�N���b�N������̔ԍ�
    Private ClickedColumn As Integer
    Private Const ThisModuleName As String = "KFJUKT0220G.vb"

#End Region

#Region "�C�x���g�n���h��"

    ''' <summary>
    ''' ��ʃ��[�h�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFJMAIN122_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            '�t�H�[���̏�����
            Call FormInitializa()     '��ʏ�����
            Call FormDataLoad()       '������ʃf�[�^���[�h

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �ύX�{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnHenkou_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHenkou.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�ύX)�J�n", "����", "")

            '--------------------------------------------------
            '�I���`�F�b�N
            '--------------------------------------------------
            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    Call MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '�폜����Ă����烁�b�Z�[�W�\��
            If GCom.NzStr(GCom.SelectedItem(Me.ListView1, 12)).Trim = "��" Then
                MessageBox.Show(MSG0385W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '--------------------------------------------------
            '���t����͉�ʂɑJ��
            '--------------------------------------------------
            Dim KFJMAIN121 As New KFJMAIN121
            With KFJMAIN121
                .SetListView = Me.ListView1
                .SetpsFSYORI_KBN = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 0))
                .SetpsTORIS_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 1))
                .SetpsTORIF_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 2))
                .SetpsENTRY_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 18))
                .SetpsBAITAI_KANRI_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 5))
                .SetpsITAKU_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 6))
                .SetpsSTATION_ENTRY_NO = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 3))
                .SetpsSYUBETU_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 8))
                .SetpsFURI_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 9))
                .SetpsBAITAI_CODE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 13))
                .SetpsENTRY_NO = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 4))
                .SetpsSYORI_KEN = GCom.NzLong(GCom.SelectedItem(Me.ListView1, 10))
                .SetpsSYORI_KIN = GCom.NzLong(GCom.SelectedItem(Me.ListView1, 11))
                If GCom.NzStr(GCom.SelectedItem(Me.ListView1, 12)).Trim = "��" Then
                    .SetpsDELETE_FLG = 1
                Else
                    .SetpsDELETE_FLG = 0
                End If
                .SetpsFORCE_FLG = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 14))
                .SetpsUPLOAD_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 15))
                .SetpsUPLOAD_FLG = GCom.NzInt(GCom.SelectedItem(Me.ListView1, 16))
                .SetpsCHECK_FLG = GCom.NzInt(GCom.SelectedItem(Me.ListView1, 17))
                .SetpsENTRY_OP = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 19))
                .SetpsUPDATE_OP = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 20))
                .SetpsDELETE_OP = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 21))
                .SetpsIN_DATE = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 24))
                .SetpsSTATION_IN_NO = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 25))
                .SetpsIN_COUNTER = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 26))
                .SetpnHENKOU_SW = 1

            End With

            CASTCommon.ShowFORM(GCom.GetUserID, CType(KFJMAIN121, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�ύX)��O�G���[", "���s", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�ύX)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �폜�{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�폜)�J�n", "����", "")

            '--------------------------------------------------
            '�I���`�F�b�N
            '--------------------------------------------------
            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    Call MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '--------------------------------------------------
            '���b�Z�[�W�ݒ�
            '--------------------------------------------------
            Dim MSG As String
            Dim bDeleteFlg As Boolean = False
            If GCom.NzStr(GCom.SelectedItem(Me.ListView1, 12)).Trim = "��" Then
                MSG = String.Format(MSG0015I, "�폜���")
                bDeleteFlg = False
            Else
                MSG = String.Format(MSG0015I, "�폜")
                bDeleteFlg = True
            End If
            If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '--------------------------------------------------
            '�폜�^�폜�������
            '--------------------------------------------------
            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
            Dim RecordCount As Integer = 0
            Dim RecordCountIn As Integer = 0

            SQL.Length = 0
            SQL.Append("SELECT COUNT(*) COUNT_A")
            SQL.Append(" FROM MEDIA_ENTRY_TBL")
            SQL.Append(" WHERE BAITAI_KANRI_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 5)).Trim & "'")
            SQL.Append(" AND FURI_DATE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 9)).Trim & "'")
            SQL.Append(" AND DELETE_FLG_ME = '0'")
            SQL.Append(" AND UPLOAD_FLG_ME = '0'")

            If OraReader.DataReader(SQL) = True Then
                RecordCount = OraReader.GetInt("COUNT_A")
            Else
                RecordCount = 0
            End If

            OraReader.Close()

            SQL.Length = 0
            SQL.Append("UPDATE MEDIA_ENTRY_TBL SET")
            If bDeleteFlg = True Then
                SQL.Append(" DELETE_FLG_ME = '1'")
            Else
                SQL.Append(" DELETE_FLG_ME = '0'")
            End If
            SQL.Append(",DELETE_OP_ME = '" & GCom.GetUserID & "'")
            SQL.Append(",UPDATE_DATE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 23)).Trim & "'")
            SQL.Append(" WHERE ITAKU_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 6)).Trim & "'")
            SQL.Append(" AND SYUBETU_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 8)).Trim & "'")
            SQL.Append(" AND FURI_DATE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 9)).Trim & "'")
            SQL.Append(" AND STATION_ENTRY_NO_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 3)).Trim & "'")
            SQL.Append(" AND BAITAI_CODE_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 13)).Trim & "'")
            SQL.Append(" AND ENTRY_NO_ME = '" & GCom.NzStr(GCom.SelectedItem(Me.ListView1, 4)).Trim & "'")

            Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
            If iRet < 0 Then
                Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "���t����у}�X�^�X�V", "���s", Me.MainDB.Message)
                Me.MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            RecordCountIn = 0
            RecordCount = 0

            Me.MainDB.Commit()

            '2018/05/25 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�폜/�폜������̉�ʔ��f�j------------------------ START
            Dim TEMP As String = IIf(bDeleteFlg, "��", "")
            Call GCom.SelectedItem(Me.ListView1, 12, TEMP)
            '2018/05/25 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�폜/�폜������̉�ʔ��f�j------------------------ END

            If bDeleteFlg = True Then
                MSG = String.Format(MSG0016I, "�폜")
            Else
                MSG = String.Format(MSG0016I, "�폜���")
            End If
            MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�폜)��O�G���[", "���s", ex.Message)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�폜)�I��", "����", "")
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
    ''' ���X�g�r���[�J�����N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick
        With CType(sender, ListView)

            If ClickedColumn = e.Column Then
                ' ��������N���b�N�����ꍇ�́C�t���ɂ��� 
                SortOrderFlag = Not SortOrderFlag
            End If

            ' ��ԍ��ݒ�
            ClickedColumn = e.Column

            ' �񐅕������z�u
            Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

            ' �\�[�g
            .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

            ' �\�[�g���s
            .Sort()
        End With
    End Sub

    ''' <summary>
    ''' ���X�g�r���[�_�u���N���b�N�C�x���g�i���ؗp�b�r�u�o�́j
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(ListView1)
    End Sub

#End Region


#Region "�v���C�x�[�g���\�b�h"

    ''' <summary>
    ''' �t�H�[����ɊY���f�[�^�\��
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FormDataLoad()
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Me.MainDB = New CASTCommon.MyOracle

        Try
            Me.ListView1.Items.Clear()

            SQL.Append("SELECT E.FSYORI_KBN_ME,")
            SQL.Append(" E.TORIS_CODE_ME,")
            SQL.Append(" E.TORIF_CODE_ME,")
            SQL.Append(" E.ENTRY_DATE_ME,")
            SQL.Append(" E.FURI_DATE_ME,")
            SQL.Append(" E.ITAKU_KANRI_CODE_ME,")
            SQL.Append(" E.ITAKU_CODE_ME,")
            SQL.Append(" E.STATION_ENTRY_NO_ME,")
            SQL.Append(" E.SYUBETU_CODE_ME,")
            SQL.Append(" E.BAITAI_CODE_ME,")
            SQL.Append(" E.ENTRY_NO_ME,")
            SQL.Append(" E.SYORI_KEN_ME,")
            SQL.Append(" E.SYORI_KIN_ME,")
            SQL.Append(" E.DELETE_FLG_ME,")
            SQL.Append(" E.FORCE_FLG_ME,")
            SQL.Append(" E.UPLOAD_DATE_ME,")
            SQL.Append(" E.UPDATE_OP_ME,")
            SQL.Append(" E.DELETE_OP_ME,")
            SQL.Append(" E.CREATE_DATE_ME,")
            SQL.Append(" E.UPDATE_DATE_ME,")
            SQL.Append(" E.IN_DATE_ME,")
            SQL.Append(" E.STATION_IN_NO_ME,")
            SQL.Append(" E.IN_COUNTER_ME,")
            SQL.Append(" NVL(E.UPLOAD_FLG_ME, 0) UPLOAD_FLG_ME,")
            SQL.Append(" E.CHECK_FLG_ME,")
            SQL.Append(" E.ENTRY_OP_ME ,")
            SQL.Append(" G.ITAKU_NNAME_T")
            SQL.Append(" FROM MEDIA_ENTRY_TBL E,")
            SQL.Append(" (SELECT FSYORI_KBN_T,")
            SQL.Append(" TORIS_CODE_T,")
            SQL.Append(" TORIF_CODE_T,")
            SQL.Append(" ITAKU_NNAME_T")
            SQL.Append(" FROM TORIMAST) G")
            SQL.Append(" WHERE E.FSYORI_KBN_ME = G.FSYORI_KBN_T")
            SQL.Append(" AND E.TORIS_CODE_ME = G.TORIS_CODE_T")
            SQL.Append(" AND E.TORIF_CODE_ME = G.TORIF_CODE_T")
            SQL.Append(" AND E.ENTRY_DATE_ME = '" & Me.SELECT_DATE & "'")
            SQL.Append(" ORDER BY E.STATION_ENTRY_NO_ME ASC")
            SQL.Append(" , E.ENTRY_NO_ME ASC")

            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False

                    Dim LineData(26) As String                 '���t����тs�a�k���[�N

                    LineData(7) = GCom.GetLimitString(GCom.NzStr(OraReader.GetItem("ITAKU_NNAME_T")).Trim, 20) '�ϑ��Җ�

                    LineData(0) = GCom.NzStr(OraReader.GetItem("FSYORI_KBN_ME"))
                    LineData(1) = GCom.NzStr(OraReader.GetItem("TORIS_CODE_ME"))
                    LineData(2) = GCom.NzStr(OraReader.GetItem("TORIF_CODE_ME"))
                    LineData(4) = GCom.NzStr(OraReader.GetItem("ENTRY_NO_ME"))              '�ʔ�
                    LineData(5) = GCom.NzStr(OraReader.GetItem("ITAKU_KANRI_CODE_ME"))      '�����P�[�X�R�[�h
                    LineData(6) = GCom.NzStr(OraReader.GetItem("ITAKU_CODE_ME"))            '�ϑ��҃R�[�h
                    LineData(8) = GCom.NzStr(OraReader.GetItem("SYUBETU_CODE_ME"))          '���
                    LineData(9) = GCom.NzStr(OraReader.GetItem("FURI_DATE_ME"))             '�U�֓�

                    LineData(10) = String.Format("{0:#,##0}", GCom.NzLong(OraReader.GetItem("SYORI_KEN_ME"))) '����
                    LineData(11) = String.Format("{0:#,##0}", GCom.NzLong(OraReader.GetItem("SYORI_KIN_ME"))) '���z

                    If GCom.NzDec(OraReader.GetItem("DELETE_FLG_ME")) = 1 Then              '�폜
                        LineData(12) = "��"
                    Else
                        LineData(12) = ""
                    End If
                    LineData(3) = GCom.NzStr(OraReader.GetItem("STATION_ENTRY_NO_ME"))      '�[���ԍ�
                    LineData(13) = GCom.NzStr(OraReader.GetItem("BAITAI_CODE_ME"))          '�}�̃R�[�h

                    LineData(14) = GCom.NzStr(OraReader.GetItem("FORCE_FLG_ME"))            '�����o�^
                    LineData(15) = GCom.NzStr(OraReader.GetItem("UPLOAD_DATE_ME"))          '�A�b�v���[�h
                    LineData(16) = GCom.NzStr(OraReader.GetItem("UPLOAD_FLG_ME"))           '�A�b�v���[�h
                    LineData(17) = GCom.NzStr(OraReader.GetItem("CHECK_FLG_ME"))            '�ƍ�
                    LineData(18) = GCom.NzStr(OraReader.GetItem("ENTRY_DATE_ME"))           '���t����͓�
                    LineData(19) = GCom.NzStr(OraReader.GetItem("ENTRY_OP_ME"))             'ID(����)
                    LineData(20) = GCom.NzStr(OraReader.GetItem("UPDATE_OP_ME"))            'ID(�ύX�j
                    LineData(21) = GCom.NzStr(OraReader.GetItem("DELETE_OP_ME"))            'ID(�폜�j
                    LineData(22) = GCom.NzStr(OraReader.GetItem("CREATE_DATE_ME"))          '�쐬��
                    LineData(23) = GCom.NzStr(OraReader.GetItem("UPDATE_DATE_ME"))          '�X�V��

                    LineData(24) = GCom.NzStr(OraReader.GetItem("IN_DATE_ME"))              '���ɓ�
                    LineData(25) = GCom.NzStr(OraReader.GetItem("STATION_IN_NO_ME"))        '���ɒ[���ԍ�
                    LineData(26) = GCom.NzStr(OraReader.GetItem("IN_COUNTER_ME"))           '��t�ʔ�

                    Dim LstItem As New ListViewItem(LineData)
                    ListView1.Items.AddRange(New ListViewItem() {LstItem})


                    OraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "�Y���f�[�^�擾", "���s", ex.Message)

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
    ''' �t�H�[����̒l��������
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FormInitializa()

        With ListView1
            .Clear()
            .Columns.Add("�U�֏����敪", 0, HorizontalAlignment.Center)
            .Columns.Add("������", 0, HorizontalAlignment.Center)
            .Columns.Add("����敛", 0, HorizontalAlignment.Center)
            .Columns.Add("�[���ԍ�", 20, HorizontalAlignment.Center)
            .Columns.Add("�ʔ�", 45, HorizontalAlignment.Right)
            .Columns.Add("�����P�[�X�ԍ�", 130, HorizontalAlignment.Center)
            .Columns.Add("�ϑ���CD", 100, HorizontalAlignment.Center)
            .Columns.Add("�ϑ��Җ�", 120, HorizontalAlignment.Left)
            .Columns.Add("���", 50, HorizontalAlignment.Center)
            .Columns.Add("�U�֓�", 80, HorizontalAlignment.Center)
            .Columns.Add("����", 70, HorizontalAlignment.Right)
            .Columns.Add("���z", 120, HorizontalAlignment.Right)
            .Columns.Add("��", 40, HorizontalAlignment.Center)
            .Columns.Add("�}�̃R�[�h", 0, HorizontalAlignment.Center)
            .Columns.Add("�����o�^", 0, HorizontalAlignment.Center)
            .Columns.Add("����۰�ޓ�", 0, HorizontalAlignment.Center)
            .Columns.Add("����۰��", 0, HorizontalAlignment.Center)
            .Columns.Add("�ƍ�FLG", 0, HorizontalAlignment.Center)
            .Columns.Add("���t����͓�", 0, HorizontalAlignment.Center)
            .Columns.Add("�h�c�i���́j", 0, HorizontalAlignment.Center)
            .Columns.Add("�h�c�i�ύX�j", 0, HorizontalAlignment.Center)
            .Columns.Add("�h�c�i�폜�j", 0, HorizontalAlignment.Center)
            .Columns.Add("�쐬��", 0, HorizontalAlignment.Center)
            .Columns.Add("�X�V��", 0, HorizontalAlignment.Center)
            .Columns.Add("���ɓ�", 0, HorizontalAlignment.Center)
            .Columns.Add("�[������", 0, HorizontalAlignment.Center)
            .Columns.Add("��t�ʔ�", 0, HorizontalAlignment.Center)
        End With
    End Sub

#End Region

End Class
