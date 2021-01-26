Option Explicit On 
Option Strict On

Imports System
Imports System.IO
Imports System.Data.OracleClient
Imports System.Collections
Imports CASTCommon

Public Class KFJMAIN130

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Dim SortOrderFlag As Boolean = True
    Dim ClickedColumn As Integer

    Private MyOwnerForm As Form

    Private Structure CodeNameInf
        Dim Code As Integer
        Dim Name As String
    End Structure
    Private BAITAI_CODE_INF() As CodeNameInf
    Private SYUBETU_CODE_INF() As CodeNameInf
    Private CHECK_KBN_INF() As CodeNameInf
    Private CHECK_FLG_INF() As CodeNameInf

    Private Const msgTitle As String = "���ƍ��󋵉��(KFJMAIN130)"
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN130", "���ƍ��󋵉��")
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure
    Private LW As LogWrite

    Private Structure MEDIA_ENTRY_TBL
        Dim FSYORI_KBN As String
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim ITAKU_CODE As String
        Dim ITAKU_NNAME As String
        Dim BAITAI_KANRI_CODE As String
        Dim IN_DATE As String
        Dim STATION_IN_NO As String
        Dim IN_COUNTER As String
        Dim ENTRY_DATE As String
        Dim STATION_ENTRY_NO As String
        Dim ENTRY_NO As String
        Dim BAITAI_CODE As String
        Dim SYUBETU_CODE As String
        Dim FURI_DATE As String
        Dim SYORI_KEN As String
        Dim SYORI_KIN As String
        Dim CHECK_KBN As String
        Dim CHECK_FLG As String
        Dim CREATE_DATE As String
        Dim UPDATE_DATE As String
    End Structure
    Private DT As MEDIA_ENTRY_TBL

    '��ʋN���C�x���g����
    Private Sub KFJMAIN130_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            Call GetCodeName(BAITAI_CODE_INF, "Common_�}�̃R�[�h.TXT")
            Call GetCodeName(SYUBETU_CODE_INF, "KFJMAST010_���.TXT")
            Call GetCodeName(CHECK_KBN_INF, "KFJMAST011_�ƍ��v�ۋ敪.TXT")
            Call GetCodeName(CHECK_FLG_INF, "KFJMAIN130_�ƍ�����.TXT")

            Call MeForm_Load_Action(ListView1)
            Call MeForm_Load_Action(ListView2)

            ListView2.Size = New Size(732, 196)

            If ListView2.Items.Count > 0 Then
                Application.DoEvents()
                ListView2.Items(0).Selected = True
            End If

            If ListView1.Items.Count > 0 Then
                Application.DoEvents()
                ListView1.Items(0).Selected = True
            End If

            '�x�����擾
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(�x�����擾)�I��", "���s", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.GroupBox1.Enabled = True
            Me.GroupBox2.Enabled = True
            Me.GroupBox3.Enabled = False
            Me.GroupBox1.Visible = True
            Me.GroupBox2.Visible = True
            Me.GroupBox3.Visible = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub

    '��ʕ\��(�ĕ\��)�C�x���g����
    Private Sub KFJOTHER0910MG_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job2 = "���ƍ���(�}��)���"
    End Sub

    '�߂�{�^������
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
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

    '
    ' �@�@�\ : ��ʏ�����
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - ListView
    '
    ' ���@�l : �ꗗ�\���ĕ`��
    '    
    Private Sub MeForm_Load_Action(ByVal ListView As ListView)
        Try
            With ListView
                .Clear()
                Me.SuspendLayout()

                Select Case ListView.Name.ToUpper
                    Case "ListView1".ToUpper
                        .Columns.Add("�����敪", 0, HorizontalAlignment.Left)
                        .Columns.Add("������R�[�h", 0, HorizontalAlignment.Left)
                        .Columns.Add("����敛�R�[�h", 0, HorizontalAlignment.Left)
                        .Columns.Add("�ϑ��҃R�[�h", 90, HorizontalAlignment.Left)
                        .Columns.Add("�ϑ��Җ�", 125, HorizontalAlignment.Left)
                        .Columns.Add("�}�̊Ǘ��R�[�h", 0, HorizontalAlignment.Left)
                        .Columns.Add("���ɓ�", 0, HorizontalAlignment.Center)
                        .Columns.Add("���ɒ[��", 0, HorizontalAlignment.Center)
                        .Columns.Add("���ɒʔ�", 0, HorizontalAlignment.Right)
                        .Columns.Add("���t����͓�", 0, HorizontalAlignment.Center)
                        .Columns.Add("�[��", 40, HorizontalAlignment.Center)
                        .Columns.Add("�ʔ�", 50, HorizontalAlignment.Right)
                        .Columns.Add("�}��CD", 0, HorizontalAlignment.Center)
                        .Columns.Add("�}��", 60, HorizontalAlignment.Left)
                        .Columns.Add("���", 40, HorizontalAlignment.Center)
                        .Columns.Add("��ʖ���", 0, HorizontalAlignment.Left)
                        .Columns.Add("�U�֓�", 80, HorizontalAlignment.Center)
                        .Columns.Add("�˗�����", 60, HorizontalAlignment.Right)
                        .Columns.Add("�˗����z", 110, HorizontalAlignment.Right)
                        .Columns.Add("�ƍ�KBN", 0, HorizontalAlignment.Center)
                        .Columns.Add("�ƍ���", 80, HorizontalAlignment.Left)
                        .Columns.Add("�ƍ�FLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("�ƍ�����", 65, HorizontalAlignment.Left)
                        .Columns.Add("�쐬��", 0, HorizontalAlignment.Left)
                        .Columns.Add("�X�V��", 0, HorizontalAlignment.Left)
                        .Columns.Add("����۰��FLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("����۰�ޓ�", 0, HorizontalAlignment.Center)
                    Case "ListView2".ToUpper
                        .Columns.Add("���s���t", 0, HorizontalAlignment.Left)
                        .Columns.Add("���s����", 0, HorizontalAlignment.Left)
                        .Columns.Add("���", 40, HorizontalAlignment.Center)
                        .Columns.Add("�ϑ��҃R�[�h", 80, HorizontalAlignment.Center)
                        .Columns.Add("�ϑ��Җ�", 125, HorizontalAlignment.Left)
                        .Columns.Add("������R�[�h", 0, HorizontalAlignment.Left)
                        .Columns.Add("����敛�R�[�h", 0, HorizontalAlignment.Left)
                        .Columns.Add("�U�֓�", 80, HorizontalAlignment.Center)
                        .Columns.Add("�T�C�N���ԍ�.", 0, HorizontalAlignment.Center)
                        .Columns.Add("�������ݓ�", 0, HorizontalAlignment.Center)
                        .Columns.Add("����", 60, HorizontalAlignment.Right)
                        .Columns.Add("���z", 110, HorizontalAlignment.Right)
                        .Columns.Add("�}��", 60, HorizontalAlignment.Left)
                        .Columns.Add("�G���[�R�[�h", 0, HorizontalAlignment.Right)
                        .Columns.Add("�ƍ�����", 100, HorizontalAlignment.Left)
                        .Columns.Add("�ʔ�", 0, HorizontalAlignment.Right)
                        .Columns.Add("�����敪", 0, HorizontalAlignment.Left)
                        .Columns.Add("�J�i�ϑ��Җ�", 0, HorizontalAlignment.Left)
                        .Columns.Add("��tFLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("�ƍ�FLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("FMT", 0, HorizontalAlignment.Center)
                        .Columns.Add("BAITAI", 0, HorizontalAlignment.Center)
                        .Columns.Add("CODE", 0, HorizontalAlignment.Center)
                End Select
            End With

            Dim SQL As String
            Dim ROW As Integer
            Dim nRet As Integer
            Dim Temp As Decimal
            Dim onDate As DateTime
            Dim onText(2) As Integer
            Dim LineColor As Color
            Dim REC As OracleDataReader = Nothing

            Select Case ListView.Name.ToUpper
                Case "ListView1".ToUpper

                    '�}�̓��o�Ɏ���TBL���ŏ㕔�̈��`��
                    SQL = "SELECT E.FSYORI_KBN_ME"
                    SQL &= ", E.TORIS_CODE_ME"
                    SQL &= ", E.TORIF_CODE_ME"
                    SQL &= ", E.ITAKU_CODE_ME"
                    SQL &= ", V.ITAKU_NNAME_T"
                    SQL &= ", E.ITAKU_KANRI_CODE_ME"
                    SQL &= ", E.IN_DATE_ME"
                    SQL &= ", E.STATION_IN_NO_ME"
                    SQL &= ", E.IN_COUNTER_ME"
                    SQL &= ", E.ENTRY_DATE_ME"
                    SQL &= ", E.STATION_ENTRY_NO_ME"
                    SQL &= ", E.ENTRY_NO_ME"
                    SQL &= ", E.BAITAI_CODE_ME"
                    SQL &= ", E.SYUBETU_CODE_ME"
                    SQL &= ", E.FURI_DATE_ME"
                    SQL &= ", E.SYORI_KEN_ME"
                    SQL &= ", E.SYORI_KIN_ME"
                    SQL &= ", E.CHECK_KBN_ME"
                    SQL &= ", E.CHECK_FLG_ME"
                    SQL &= ", E.CREATE_DATE_ME"
                    SQL &= ", E.UPDATE_DATE_ME"
                    SQL &= ", E.UPLOAD_FLG_ME"
                    SQL &= ", E.UPLOAD_DATE_ME"
                    SQL &= " FROM MEDIA_ENTRY_TBL"
                    SQL &= " E"
                    SQL &= ", (SELECT FSYORI_KBN_T"
                    SQL &= ", TORIS_CODE_T"
                    SQL &= ", TORIF_CODE_T"
                    SQL &= ", ITAKU_NNAME_T"
                    SQL &= ", ITAKU_KNAME_T"
                    SQL &= " FROM TORIMAST"
                    SQL &= ") V"
                    SQL &= " WHERE E.FSYORI_KBN_ME = V.FSYORI_KBN_T"
                    SQL &= " AND E.TORIS_CODE_ME = V.TORIS_CODE_T"
                    SQL &= " AND E.TORIF_CODE_ME = V.TORIF_CODE_T"
                    SQL &= " AND NVL(E.DELETE_FLG_ME, '0') = '0'"
                    SQL &= " AND NVL(E.CHECK_KBN_ME, '0') = '1'"
                    SQL &= " AND NOT NVL(E.CHECK_FLG_ME, '0') = '1'"
                    SQL &= " AND NVL(E.FURI_DATE_ME, '00000000') >= TO_CHAR(SYSDATE, 'yyyymmdd')"
                    SQL &= " ORDER BY V.ITAKU_KNAME_T ASC"
                    SQL &= ", E.TORIS_CODE_ME ASC"
                    SQL &= ", E.TORIF_CODE_ME ASC"
                    SQL &= ", E.FURI_DATE_ME ASC"

                    If GCom.SetDynaset(SQL, REC) Then

                        ROW = 0
                        Do While REC.Read
                            Dim Data(26) As String

                            Data(0) = GCom.NzDec(REC.Item("FSYORI_KBN_ME"), "")        '�����敪
                            Data(1) = GCom.NzDec(REC.Item("TORIS_CODE_ME"), "")        '������R�[�h
                            Data(2) = GCom.NzDec(REC.Item("TORIF_CODE_ME"), "")        '����敛�R�[�h
                            Data(3) = GCom.NzStr(REC.Item("ITAKU_CODE_ME")).Trim       '�ϑ��҃R�[�h
                            Data(4) = GCom.NzStr(REC.Item("ITAKU_NNAME_T")).Trim       '�ϑ��Җ�
                            Data(5) = GCom.NzDec(REC.Item("ITAKU_KANRI_CODE_ME"), "") '�}�̊Ǘ��R�[�h

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("IN_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(6) = String.Format("{0:yyyy.MM.dd}", onDate)   '���ɓ�
                            End If
                            Data(7) = GCom.NzDec(REC.Item("STATION_IN_NO_ME"), "")     '���ɒ[��
                            Data(8) = GCom.NzDec(REC.Item("IN_COUNTER_ME"), "")        '���ɒʔ�

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("ENTRY_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(9) = String.Format("{0:yyyy.MM.dd}", onDate)   '���t����͓�
                            End If
                            Data(10) = GCom.NzDec(REC.Item("STATION_ENTRY_NO_ME"), "") '���t����͒[��
                            Data(11) = GCom.NzDec(REC.Item("ENTRY_NO_ME"), "")         '���t����͒ʔ�

                            Data(12) = GCom.NzDec(REC.Item("BAITAI_CODE_ME"), "")      '�}�̃R�[�h
                            Data(13) = GetCodeName(GCom.NzInt(Data(12)), BAITAI_CODE_INF)   '�}�̖���

                            Data(14) = GCom.NzDec(REC.Item("SYUBETU_CODE_ME"), "")     '��ʃR�[�h
                            Data(15) = GetCodeName(GCom.NzInt(Data(14)), SYUBETU_CODE_INF)  '��ʖ���

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("FURI_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(16) = String.Format("{0:yyyy.MM.dd}", onDate)  '�U�֓�
                            End If

                            Temp = GCom.NzDec(REC.Item("SYORI_KEN_ME"))
                            Data(17) = String.Format("{0:#,##0}", Temp)             '�˗�����

                            Temp = GCom.NzDec(REC.Item("SYORI_KIN_ME"))
                            Data(18) = String.Format("{0:#,##0}", Temp)             '�˗����z

                            Data(19) = GCom.NzDec(REC.Item("CHECK_KBN_ME"), "")        '�ƍ��敪
                            Data(20) = GetCodeName(GCom.NzInt(Data(19)), CHECK_KBN_INF) '�ƍ��敪����

                            Data(21) = GCom.NzDec(REC.Item("CHECK_FLG_ME"), "")        '�ƍ�����
                            Data(22) = GetCodeName(GCom.NzInt(Data(21)), CHECK_FLG_INF) '�ƍ����ʖ���

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("CREATE_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(23) = String.Format("{0:yyyy.MM.dd}", onDate)  '�쐬��
                            End If
                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("UPDATE_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(24) = String.Format("{0:yyyy.MM.dd}", onDate)  '�X�V��
                            End If

                            Data(25) = GCom.NzDec(REC.Item("UPLOAD_FLG_ME"), "")       '�A�b�v���[�hFLG
                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("UPLOAD_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(26) = String.Format("{0:yyyy.MM.dd}", onDate)  '�A�b�v���[�h��
                            End If

                            If ROW Mod 2 = 0 Then
                                LineColor = Color.White
                            Else
                                LineColor = Color.WhiteSmoke
                            End If

                            Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                            ListView.Items.AddRange(New ListViewItem() {vLstItem})

                            ROW += 1
                        Loop
                    End If
                Case "ListView2".ToUpper
                    SQL = ""

                    '�ƍ��G���[TBL���ŉ����̈��`��
                    SQL = "SELECT MATCHING_DATE"            '���s���t
                    SQL &= ", MATCHING_TIME"                '���s����
                    SQL &= ", FSYORI_KBN"                   '�����敪
                    SQL &= ", TORIS_CODE"                   '������R�[�h
                    SQL &= ", TORIF_CODE"                   '����敛�R�[�h
                    SQL &= ", FURI_DATE"                    '�U�֓�
                    SQL &= ", CYCLE_NO"                     '�T�C�N���ԍ�
                    SQL &= ", IN_DATE"                      '���Ƃ����ݓ�
                    SQL &= ", IN_BAITAI_CODE"               '�}�̃R�[�h
                    SQL &= ", IN_ITAKU_CODE"                '�ϑ��҃R�[�h
                    SQL &= ", IN_FURI_DATE"                 '�U�֓�
                    SQL &= ", IN_KEN"                       '����
                    SQL &= ", IN_KIN"                       '���z
                    SQL &= ", ERR_CODE"                     '�G���[�R�[�h
                    SQL &= ", ERR_TEXT"                     '�G���[����
                    SQL &= ", SEQ"                          '�ʔ�
                    SQL &= ", ITAKU_NNAME"
                    SQL &= ", ITAKU_KNAME"
                    SQL &= ", SYUBETU_CODE"
                    SQL &= ", UKETUKE_FLG"
                    SQL &= ", SYOUGOU_FLG"
                    SQL &= ", FMT_KBN"
                    SQL &= ", CODE_KBN"

                    '�X�P�W���[���}�X�^����̖��ƍ����
                    SQL &= " FROM (SELECT '00000000' MATCHING_DATE" '���s���t
                    SQL &= ", '000000' MATCHING_TIME"               '���s����
                    SQL &= ", E.FSYORI_KBN_S FSYORI_KBN"            '�����敪
                    SQL &= ", E.TORIS_CODE_S TORIS_CODE"            '������R�[�h
                    SQL &= ", E.TORIF_CODE_S TORIF_CODE"            '����敛�R�[�h
                    SQL &= ", E.FURI_DATE_S FURI_DATE"              '�U�֓�
                    SQL &= ", E.MOTIKOMI_SEQ_S CYCLE_NO"            '�T�C�N���ԍ�
                    SQL &= ", E.UKETUKE_DATE_S IN_DATE"             '���Ƃ����ݓ�
                    SQL &= ", E.BAITAI_CODE_S IN_BAITAI_CODE"       '�}�̃R�[�h
                    SQL &= ", E.ITAKU_CODE_S IN_ITAKU_CODE"         '�ϑ��҃R�[�h
                    SQL &= ", E.FURI_DATE_S IN_FURI_DATE"           '�U�֓�
                    SQL &= ", E.SYORI_KEN_S IN_KEN"                 '����
                    SQL &= ", E.SYORI_KIN_S IN_KIN"                 '���z
                    SQL &= ", NULL ERR_CODE"                        '�G���[�R�[�h
                    '�G���[����
                    SQL &= ", DECODE(SYOUGOU_FLG_S, '0', '���ƍ�', '9', '���v�[�Ȃ�', ' ') ERR_TEXT"
                    SQL &= ", NULL SEQ"                             '�ʔ�
                    SQL &= ", V.ITAKU_NNAME_T ITAKU_NNAME"
                    SQL &= ", V.ITAKU_KNAME_T ITAKU_KNAME"
                    SQL &= ", V.SYUBETU_T SYUBETU_CODE"
                    SQL &= ", E.UKETUKE_FLG_S UKETUKE_FLG"
                    SQL &= ", E.SYOUGOU_FLG_S SYOUGOU_FLG"
                    SQL &= ", V.FMT_KBN_T FMT_KBN"
                    SQL &= ", V.CODE_KBN_T CODE_KBN"
                    SQL &= " FROM SCHMAST_VIEW E"
                    SQL &= ", (SELECT FSYORI_KBN_T"
                    SQL &= ", TORIS_CODE_T"
                    SQL &= ", TORIF_CODE_T"
                    SQL &= ", ITAKU_NNAME_T"
                    SQL &= ", ITAKU_KNAME_T"
                    SQL &= ", SYUBETU_T"
                    SQL &= ", FMT_KBN_T"
                    SQL &= ", CODE_KBN_T"
                    SQL &= " FROM TORIMAST_VIEW"
                    SQL &= " WHERE SYOUGOU_KBN_T = '1'"
                    SQL &= ") V"
                    SQL &= " WHERE E.FSYORI_KBN_S = V.FSYORI_KBN_T"
                    SQL &= " AND E.TORIS_CODE_S = V.TORIS_CODE_T"
                    SQL &= " AND E.TORIF_CODE_S = V.TORIF_CODE_T"
                    SQL &= " AND NVL(E.UKETUKE_FLG_S, '0') = '1'"
                    SQL &= " AND NVL(E.SYOUGOU_FLG_S, '0') IN ('0', '9')"
                    SQL &= " AND NVL(E.TYUUDAN_FLG_S, '0') = '0'  AND E.TOUROKU_FLG_S = '0'"
                    SQL &= ")"

                    '***�C�� 2009.02.24 �U�֓�����������̂��̂̂ݕ\�� 2009.02.24 start
                    SQL &= " WHERE NVL(FURI_DATE, '00000000') >= TO_CHAR(SYSDATE, 'yyyymmdd')"
                    '***�C�� 2009.02.24 �U�֓�����������̂��̂̂ݕ\�� 2009.02.24 end

                    SQL &= " ORDER BY ITAKU_KNAME ASC"
                    SQL &= ", FSYORI_KBN ASC"
                    SQL &= ", TORIS_CODE ASC"
                    SQL &= ", TORIF_CODE ASC"
                    SQL &= ", FURI_DATE ASC"
                    SQL &= ", CYCLE_NO ASC"

                    If GCom.SetDynaset(SQL, REC) Then

                        Dim CurrentString As String = ""
                        Dim PreviousString As String = ""

                        ROW = 0
                        Do While REC.Read
                            Dim Data(22) As String

                            Data(0) = GCom.NzDec(REC.Item("MATCHING_DATE"), "")     '���s���t
                            Data(1) = GCom.NzDec(REC.Item("MATCHING_TIME"), "")     '���s����
                            Data(2) = GCom.NzDec(REC.Item("SYUBETU_CODE"), "")      '�����敪
                            Data(3) = GCom.NzDec(REC.Item("IN_ITAKU_CODE"), "")     '�ϑ��҃R�[�h
                            Data(4) = GCom.NzStr(REC.Item("ITAKU_NNAME")).Trim      '�ϑ��Җ�
                            Data(5) = GCom.NzDec(REC.Item("TORIS_CODE"), "")        '������R�[�h
                            Data(6) = GCom.NzDec(REC.Item("TORIF_CODE"), "")        '����敛�R�[�h

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("IN_FURI_DATE"), ""))
                            If Not onDate = Nothing Then
                                Data(7) = String.Format("{0:yyyy.MM.dd}", onDate)   '�U�֓�
                            End If

                            Data(8) = GCom.NzDec(REC.Item("CYCLE_NO"), "")          '�T�C�N���ԍ�

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("IN_DATE"), ""))
                            If Not onDate = Nothing Then
                                Data(9) = String.Format("{0:yyyy.MM.dd}", onDate)   '���Ƃ����ݓ�
                            End If

                            Temp = GCom.NzDec(REC.Item("IN_KEN"), 0)
                            Data(10) = String.Format("{0:#,##0}", Temp)             '����

                            Temp = GCom.NzDec(REC.Item("IN_KIN"), 0)
                            Data(11) = String.Format("{0:#,##0}", Temp)             '���z

                            nRet = GCom.NzInt(REC.Item("IN_BAITAI_CODE"))
                            Data(12) = GetCodeName(nRet, BAITAI_CODE_INF)           '�}��

                            Data(13) = GCom.NzStr(REC.Item("ERR_CODE")).Trim        '�G���[�R�[�h
                            Data(14) = GCom.NzStr(REC.Item("ERR_TEXT")).Trim        '�ƍ�����
                            Data(15) = GCom.NzDec(REC.Item("SEQ"), "")              '�ʔ�
                            Data(16) = GCom.NzDec(REC.Item("FSYORI_KBN"), "")       '�����敪
                            Data(17) = GCom.NzStr(REC.Item("ITAKU_KNAME")).Trim     '�J�i�ϑ��Җ�
                            Data(18) = GCom.NzDec(REC.Item("UKETUKE_FLG"), "")      '��tFLG
                            Data(19) = GCom.NzDec(REC.Item("SYOUGOU_FLG"), "")      '�ƍ�FLG
                            Data(20) = GCom.NzDec(REC.Item("FMT_KBN"), "")          'FMT_KBN
                            Data(21) = nRet.ToString.PadLeft(2, "0"c)               '�}�̃R�[�h
                            Data(22) = GCom.NzDec(REC.Item("CODE_KBN"), "")         '�R�[�h�敪

                            '���ꃌ�R�[�h�̔r��
                            CurrentString = Data(17)            '�J�i�ϑ��Җ�
                            CurrentString &= "-" & Data(16)     '�����敪
                            CurrentString &= "-" & Data(5)      '������R�[�h
                            CurrentString &= "-" & Data(6)      '����敛�R�[�h
                            CurrentString &= "-" & Data(7)      '�U�֓�
                            CurrentString &= "-" & Data(8)      '�T�C�N���ԍ�

                            If Not CurrentString = PreviousString Then

                                If ROW Mod 2 = 0 Then
                                    LineColor = Color.White
                                Else
                                    LineColor = Color.WhiteSmoke
                                End If

                                Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                                ListView.Items.AddRange(New ListViewItem() {vLstItem})

                                ROW += 1
                                PreviousString = CurrentString
                            End If
                        Loop
                    End If
            End Select
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ꗗ�����`�揈��", "���s", ex.Message)
        Finally
            Me.ResumeLayout()
        End Try
    End Sub

    '��t���ύX�{�^������
    Private Sub CmdUkeUpdate_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUkeUpdate.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(��t���ύX)�J�n", "����", "")

            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '�R���g���[���̎g�p��
            Me.GroupBox1.Enabled = False
            Me.GroupBox2.Enabled = False
            Me.GroupBox3.Enabled = True
            Me.GroupBox2.Visible = False
            Me.GroupBox3.Visible = True

            '���e�ݒ�
            Me.UpdateInitializa()
            Me.txtFuriDateY.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(��t���ύX)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(��t���ύX)�I��", "����", "")
        End Try
    End Sub

    '
    ' �@�@�\ : �}�̃R�[�h����~�ς���
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �~�ϔz��ϐ�
    ' �@�@�@   ARG2 - �e�L�X�g�t�@�C����
    '
    ' ���@�l : �R���{�{�b�N�X���ʊ֐�
    '    
    Private Sub GetCodeName(ByRef avCodeName() As CodeNameInf, ByVal avFileName As String)
        Dim FL As StreamReader = Nothing
        Try
            Dim Index As Integer = 0

            Dim FileName As String = GCom.SET_PATH(GCom.GetTXTFolder) & avFileName
            If System.IO.File.Exists(FileName) Then

                FL = New StreamReader(FileName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

                Dim LineData As String = FL.ReadLine
                Do While Not LineData Is Nothing

                    Dim Data() As String = LineData.Split(","c)
                    If Data.Length >= 2 Then

                        ReDim Preserve avCodeName(Index)
                        avCodeName(Index).Code = GCom.NzInt(Data(0))
                        avCodeName(Index).Name = GCom.NzStr(Data(1)).Trim

                        Index += 1
                    End If

                    LineData = FL.ReadLine
                Loop
            End If
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̃R�[�h���~�Ϗ���", "���s", ex.Message)
        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Sub

    '
    ' �@�@�\ : �R�[�h��񂩂疼�̂�Ԃ�
    '
    ' �߂�l : �Ώۖ��̒l
    '
    ' ������ : ARG1 - Code�l
    ' �@�@�@   ARG2 - �L���z��
    '
    ' ���@�l : �~�ϔz��̎Q��
    '    
    Private Function GetCodeName(ByVal avIntValue As Integer, ByVal avCodeName() As CodeNameInf) As String
        Try
            For Each Temp As CodeNameInf In avCodeName

                If avIntValue = Temp.Code Then

                    Return Temp.Name
                End If
            Next
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̕ԋp����", "���s", ex.Message)
        End Try

        Return ""
    End Function

    '
    ' �@�@�\ : ���̏�񂩂�R�[�h��Ԃ�
    '
    ' �߂�l : �R�[�h�l
    '
    ' ������ : ARG1 - ���̒l
    ' �@�@�@   ARG2 - �L���z��
    '
    ' ���@�l : �~�ϔz��̎Q��
    '    
    Private Function GetCodeName(ByVal avStrValue As String, ByVal avCodeName() As CodeNameInf) As Integer
        Try
            For Each Temp As CodeNameInf In avCodeName

                If avStrValue = Temp.Name Then

                    Return Temp.Code
                End If
            Next
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�R�[�h�ԋp����", ex.Message)
        End Try

        Return Nothing
    End Function

    '���ؗpCSV�o��&�\��
    Private Sub LetMonitor_ListView(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles ListView1.DoubleClick, ListView2.DoubleClick

        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    '�ꗗ�\���̈�̃\�[�g
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
        Handles ListView1.ColumnClick, ListView2.ColumnClick

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

    '
    ' �@�@�\ : �Γ�������ʂւ̑J�ڔ���
    '
    ' �߂�l : DialogResult.OK = �J�ڂ���, DialogResult.Cancel = �J�ڂ��Ȃ�
    '
    ' ������ : ARG1 - �Ǝ����b�Z�[�W
    ' �@�@�@   ARG2 - �L���z��(�l)
    '
    ' ���@�l : ���ʉ�
    '
    Private Function CheckMessage(ByVal avMSG As String, ByRef StrData() As String) As DialogResult
        Try
            Dim MSG As String = ""

            If ListView2.Items.Count <= 0 Then
                Return DialogResult.Cancel
            Else
                If GCom.GetListViewHasRow(Me.ListView2) <= 0 Then
                    MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return DialogResult.Cancel
                End If
            End If

            Dim StrName() As String = {"�ϑ��Җ�", "�}��", "���", "�ϑ��҃R�[�h", _
                                       "������R�[�h", "����敛�R�[�h", "�U�֓�", "����", "���z"}

            Dim BAITAI As String = GCom.NzStr(GCom.SelectedItem(ListView2, 12))
            BAITAI = "( " & GetCodeName(BAITAI, BAITAI_CODE_INF).ToString.PadLeft(2, "0"c) & " ) " & BAITAI

            Dim SYUBETU As String = GCom.NzDec(GCom.SelectedItem(ListView2, 2), "")
            SYUBETU = "( " & SYUBETU & " ) " & GetCodeName(GCom.NzInt(SYUBETU), SYUBETU_CODE_INF)

            Dim Data() As String = {GCom.NzStr(GCom.SelectedItem(ListView2, 4)).Trim, _
                        BAITAI, SYUBETU, _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 3), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 5), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 6), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 7), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 10), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 11), "")}
            StrData = Data

            MSG = ""
            Dim Index As Integer = 0
            For Each Temp As String In StrName
                Do While Temp.Length < 7
                    Temp &= "�@"
                Loop
                MSG &= Temp & ControlChars.Tab & StrData(Index) & Space(8) & ControlChars.Cr
                Index += 1
            Next
            MSG &= ControlChars.Cr & avMSG & Space(8)
            Return MessageBox.Show(MSG, GCom.GLog.Job1, _
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Y��������ʂւ̑J�ڔ���", "���s", ex.Message)
        End Try
    End Function


    Private Sub UpdateInitializa()
        With DT
            .FSYORI_KBN = GCom.SelectedItem(Me.ListView1, 0).ToString
            .TORIS_CODE = GCom.SelectedItem(Me.ListView1, 1).ToString
            .TORIF_CODE = GCom.SelectedItem(Me.ListView1, 2).ToString
            .ITAKU_CODE = GCom.SelectedItem(Me.ListView1, 3).ToString
            .ITAKU_NNAME = GCom.SelectedItem(Me.ListView1, 4).ToString
            .BAITAI_KANRI_CODE = GCom.SelectedItem(Me.ListView1, 5).ToString
            .IN_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 6), "")
            .STATION_IN_NO = GCom.SelectedItem(Me.ListView1, 7).ToString
            .IN_COUNTER = GCom.SelectedItem(Me.ListView1, 8).ToString
            .ENTRY_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 9), "")
            .STATION_ENTRY_NO = GCom.SelectedItem(Me.ListView1, 10).ToString
            .ENTRY_NO = GCom.SelectedItem(Me.ListView1, 11).ToString
            .BAITAI_CODE = GCom.SelectedItem(Me.ListView1, 12).ToString
            .SYUBETU_CODE = GCom.SelectedItem(Me.ListView1, 14).ToString
            .FURI_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 16), "")
            .SYORI_KEN = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 17), "")
            .SYORI_KIN = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 18), "")
            .CHECK_KBN = GCom.SelectedItem(Me.ListView1, 19).ToString
            .CHECK_FLG = GCom.SelectedItem(Me.ListView1, 21).ToString
            .CREATE_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 23), "")
            .UPDATE_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 24), "")
        End With

        '�ϑ��҃R�[�h�^��
        Me.lblItakuCode.Text = DT.ITAKU_CODE
        Me.lblItakuName.Text = Space(1) & GCom.GetLimitString(DT.ITAKU_NNAME, 48)

        '���(���)�R�[�h�^����
        Select Case DT.SYUBETU_CODE
            Case "91" : Me.lblSyubetu.Text = "91�F���U"
            Case "21" : Me.lblSyubetu.Text = "21�F���U"
            Case "11" : Me.lblSyubetu.Text = "11�F���^"
            Case "12" : Me.lblSyubetu.Text = "12�F�ܗ^"
            Case Else : Me.lblSyubetu.Text = DT.SYUBETU_CODE
        End Select

        '�U�֓�
        Dim onDate As DateTime = GCom.SET_DATE(DT.FURI_DATE)
        If Not onDate = Nothing Then
            Me.txtFuriDateY.Text = onDate.Year.ToString.PadLeft(4, "0"c)
            Me.txtFuriDateM.Text = onDate.Month.ToString.PadLeft(2, "0"c)
            Me.txtFuriDateD.Text = onDate.Day.ToString.PadLeft(2, "0"c)
        Else
            Me.txtFuriDateY.Text = New String("0"c, 4)
            Me.txtFuriDateM.Text = New String("0"c, 2)
            Me.txtFuriDateD.Text = New String("0"c, 2)
        End If

        '����
        Me.txtKen.Text = String.Format("{0:#,##0}", GCom.NzDec(DT.SYORI_KEN))

        '���z
        Me.txtKin.Text = String.Format("{0:#,##0}", GCom.NzDec(DT.SYORI_KIN))

        '�ʔ�
        Dim Temp As String = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 9), "")
        Temp &= " - "
        Temp &= GCom.NzDec(GCom.SelectedItem(Me.ListView1, 10), "")
        Temp &= " - "
        Temp &= GCom.NzDec(GCom.SelectedItem(Me.ListView1, 11), "")
        Me.lblSEQ.Text = Temp

    End Sub

    ''' <summary>
    ''' �X�V�{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUpdate.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")

            '--------------------------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '--------------------------------------------------
            If Me.CheckTextBox() = False Then
                Return
            End If

            '--------------------------------------------------
            '�X�V�`�F�b�N
            '--------------------------------------------------
            'Dim NEW_SYUBETU_CODE As String
            Dim NEW_FURI_DATE As String
            Dim NEW_SYORI_KEN As String
            Dim NEW_SYORI_KIN As String

            NEW_FURI_DATE = String.Concat(New String() {Me.txtFuriDateY.Text, Me.txtFuriDateM.Text, Me.txtFuriDateD.Text})
            NEW_SYORI_KEN = Me.txtKen.Text.Replace(",", "")
            NEW_SYORI_KIN = Me.txtKin.Text.Replace(",", "")

            If DT.FURI_DATE = NEW_FURI_DATE AndAlso _
                DT.SYORI_KEN = NEW_SYORI_KEN AndAlso _
                DT.SYORI_KIN = NEW_SYORI_KIN Then
                MessageBox.Show(MSG0040I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.txtFuriDateY.Focus()
                Return
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '--------------------------------------------------
            '�X�V����
            '--------------------------------------------------
            Dim Ret As Integer
            Dim SQL As String
            Dim Temp As String
            Dim SQLCode As Integer

            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            SQL = "UPDATE MEDIA_ENTRY_TBL"
            '��ʂ͍X�V���Ȃ�
            'SQL &= " SET SYUBETU_CODE_ME = '" & String.Format("{0:00}", GCom.NzInt(NEW_SYUBETU_CODE)) & "'"
            SQL &= " SET FURI_DATE_ME = '" & NEW_FURI_DATE & "'"
            SQL &= ", SYORI_KEN_ME = " & NEW_SYORI_KEN
            SQL &= ", SYORI_KIN_ME = " & NEW_SYORI_KIN
            SQL &= ", UPDATE_OP_ME = '" & GCom.GetUserID & "'"
            SQL &= ", UPDATE_DATE_ME = TO_CHAR(SYSDATE, 'yyyymmddHH24MIss')"
            With DT
                SQL &= " WHERE FSYORI_KBN_ME = '" & .FSYORI_KBN & "'"
                SQL &= " AND TORIS_CODE_ME = '" & .TORIS_CODE & "'"
                SQL &= " AND TORIF_CODE_ME = '" & .TORIF_CODE & "'"
                SQL &= " AND FURI_DATE_ME = '" & .FURI_DATE & "'"
                SQL &= " AND ENTRY_DATE_ME = '" & .ENTRY_DATE & "'"
                SQL &= " AND STATION_ENTRY_NO_ME = '" & .STATION_ENTRY_NO & "'"
                SQL &= " AND BAITAI_CODE_ME = '" & .BAITAI_CODE & "'"
                SQL &= " AND ENTRY_NO_ME = " & .ENTRY_NO
            End With

            Try
                '�}�̓��o�Ɏ��тs�a�k�̍X�V
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)

            Catch ex As Exception
                Ret = 0
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�[�u���X�V", "���s", ex.Message)

            Finally
                Select Case SQLCode
                    Case 0
                        Try
                            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                            Temp = NEW_FURI_DATE.Substring(0, 4)
                            Temp &= "." & NEW_FURI_DATE.Substring(4, 2)
                            Temp &= "." & NEW_FURI_DATE.Substring(6)
                            Call GCom.SelectedItem(Me.ListView1, 16, Temp)                  '�U�֓�
                            Temp = String.Format("{0:#,##0}", GCom.NzDec(NEW_SYORI_KEN))
                            Call GCom.SelectedItem(Me.ListView1, 17, Temp)                  '�˗�����
                            Temp = String.Format("{0:#,##0}", GCom.NzDec(NEW_SYORI_KIN))
                            Call GCom.SelectedItem(Me.ListView1, 18, Temp)                  '�˗����z

                            Me.CmdCancel.PerformClick()
                        Catch ex As Exception
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�[�u���R�~�b�g", "���s", ex.Message)
                        End Try

                    Case Else
                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                        MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End Try

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�V", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �L�����Z���{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdCancel.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�L�����Z��)�J�n", "����", "")

            '�R���g���[���̎g�p��
            Me.GroupBox1.Enabled = True
            Me.GroupBox2.Enabled = True
            Me.GroupBox3.Enabled = False
            Me.GroupBox2.Visible = True
            Me.GroupBox3.Visible = False

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�L�����Z��", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�L�����Z��)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X�̓��̓`�F�b�N�����܂��B
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Private Function CheckTextBox() As Boolean
        Try
            '�U�֓��i�N�j�`�F�b�N
            If Me.txtFuriDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '�U�֓��i���j�`�F�b�N
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

            '�U�֓��i���j�`�F�b�N
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

            '�ߋ����`�F�b�N
            WORK_DATE = String.Concat(New String() {Me.txtFuriDateY.Text, Me.txtFuriDateM.Text, Me.txtFuriDateD.Text})
            If WORK_DATE <= String.Format("{0:yyyyMMdd}", GCom.GetSysDate) Then
                MessageBox.Show(MSG0387W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
            Select Case WORK_DATE
                Case Is > GCom.GetSysDate.AddMonths(1).AddDays(-1).ToString("yyyyMMdd")     '�ꃖ���ȏ��`�F�b�N
                    If MessageBox.Show(MSG0090I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                        Me.txtFuriDateY.Focus()
                        Return False
                    End If

                Case Is < GCom.GetSysDate.AddMonths(-1).AddDays(1).ToString("yyyyMMdd")     '�ꃖ���ȏ�O�`�F�b�N
                    If MessageBox.Show(MSG0090I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                        Me.txtFuriDateY.Focus()
                        Return False
                    End If
            End Select

            '�����`�F�b�N
            If Me.txtKen.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "�˗�����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKen.Focus()
                Return False
            End If

            '���z�`�F�b�N
            If Me.txtKin.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "�˗����z"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKin.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�{�b�N�X���̓`�F�b�N", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X���@���f�C�e�B���O�C�x���g�i�[���p�f�B���O�j
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�{�b�N�X���@���f�C�e�B���O�C�x���g", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
