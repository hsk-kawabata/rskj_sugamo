Imports clsFUSION.clsMain
Imports System.Drawing.Printing
Imports System.Text
Imports System.IO
Imports CASTCommon.ModPublic
Imports System.Data.OracleClient
Imports System.Runtime.InteropServices
Imports CASTCommon

Public Class KFJMAIN030
    Inherits System.Windows.Forms.Form

    Private clsFUSION As New clsFUSION.clsMain

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN030", "�Z���^�[�J�b�g�f�[�^�쐬���")
    Private Const msgTitle As String = "�Z���^�[�J�b�g�f�[�^�쐬���(KFJMAIN030)"
    '�\�[�g�I�[�_�[�t���O
    Dim SortOrderFlag As Boolean = True

    '�N���b�N������̔ԍ�
    Dim ClickedColumn As Integer

    Public WriteOnly Property SetDayCounter() As Integer
        Set(ByVal Value As Integer)
            DayCounter = Value
        End Set
    End Property

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private DayCounter As Integer
    '*** 2008/6/24 kaki**********************
    '*** �J�E���^�[�̍ő吔��ύX         
    'Private Const MaxCounter As Integer = 60
    Private Const MaxCounter As Integer = 600
    '*** 2008/6/24 kaki**********************

    Private htTORI_LIST As Hashtable '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓�����

#Region "�錾"
    Public intSCH_COUNT As Integer
    Public strTORI_CODE(MaxCounter) As String
    Public strFURI_DATE(MaxCounter) As String
    Public strHAISIN_YDATE(MaxCounter) As String
    Public strSOUSIN_KBN(MaxCounter) As String
    Public strITAKU_NNAME(MaxCounter) As String
    Public strKIGYO_CODE(MaxCounter) As String
    Public strFURI_CODE(MaxCounter) As String
    ' 2008.01.16 >>
    Public strJIKOTAKO(MaxCounter) As String        ' ���s�C���s�CSSS
    ' 2008.01.16 <<
    ' 080207 ADD START
    Public strKENSU(MaxCounter) As String
    Public strTAKOU_KBN(MaxCounter) As String    ' ���s�敪�i�[
    ' 080207 ADD END
    Public intMAXCNT As Integer
    Public intMAXLINE As Integer
    Public intPAGE_CNT As Integer
    Public P As Integer
    Public L As Integer

    Public intP_PAGE As Integer


    '���[����p
    Private intGYO As Integer
    Private intMOJI_SIZE As Integer = 5
    Private intP As Integer
    Private intL As Integer

    Private mTakouDensoList As String()
    Private mTakouDensoGaiList As String()

    '2013/10/24 saitou �W���C�� ADD -------------------------------------------------->>>>
    ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- START
    'Private CENTER As String
    'Private KINKOCD As String
    ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- END

    ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- START
    Private INI_COMMON_DEN As String
    Private INI_COMMON_CENTER As String
    Private INI_COMMON_KINKOCD As String
    Private INI_JIFURI_FILEAM As String
    Private INI_JIFURI_FILEPM As String
    Private INI_JIFURI_KITEIKEN As String
    Private INI_JIFURI_BORDER_TIME As String
    Private INI_JIFURI_CCDATACHK As String
    Private INI_JIFURI_BORDERCHK As String
    Private INI_JIFURI_ENTRYCHK As String
    Private INI_JIFURI_CCFNAME As String
    ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- END
    '2017/06/08 �^�X�N�j���� ADD �K���M���Ή��i�w�Z�������Ή��y�W���ŏC���z�j--------------------- START
    Private INI_JYOUKEN As String = ""
    '2017/06/08 �^�X�N�j���� ADD �K���M���Ή��i�w�Z�������Ή��y�W���ŏC���z�j--------------------- END
#End Region

#Region "��ʂ̃��[�h"
    Protected Sub KFJMAIN030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim OraDB As New CASTCommon.MyOracle()
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQL As StringBuilder
        Dim aryCenter As New ArrayList
        Dim cData(10) As String
        Dim NextData As String = Nothing '2009/11/30 �ǉ� ���c�Ɠ�

        ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
        ''2010.02.19 ���E���Ԃ�ini���擾 start
        'Dim strBorder As String = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
        'If strBorder = "err" OrElse strBorder = "" Then
        '    strBorder = "1200"
        'End If
        ''2010.02.19 ���E���Ԃ�ini���擾 end
        ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Me.Visible = False

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�V�X�e�����t�ƃ��[�U����\��
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Select Case DayCounter
                Case 0
                    Me.Label3.Text = "���Z���^�[�J�b�g�f�[�^�쐬��"
                    '                    Me.Label3.Location = New Point(232, 18)
                Case 1
                    Me.Label3.Text = "���Z���^�[�J�b�g���X���z�M�f�[�^�쐬��"
                    '                    Me.Label3.Location = New Point(222, 18)
                Case 2
                    'Me.Label3.Text = "���Z���^�[�J�b�g�����z�M�f�[�^�쐬��"
                    Me.Label3.Text = "�����U���M���O�f�[�^�쐬��"
                    '                    Me.Label3.Location = New Point(262, 18)
            End Select

            txtCycle.Text = "1"

            'INI�t�@�C���̓ǂݍ���
            'gstrLOG_OPENDIR = GCom.GetLogFolder
            ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
            'CENTER = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

            '�g�p���Ă��Ȃ��H2009.10.14
            'If FN_SET_INIFILE() = False Then
            '    MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
            If GetIniInfo() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�h�m�h���擾", "���s", "")
                Return
            End If
            ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

            ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
            '-----------------------------------------------------------
            ' �b�b�f�[�^���݃`�F�b�N
            '-----------------------------------------------------------
            Dim NowTimeStamp As String = System.DateTime.Now.ToString("HHmm")
            If INI_JIFURI_CCDATACHK = "YES" Then
                Dim GetFilesInfo() As String

                Select Case INI_COMMON_CENTER
                    Case "5"
                        '-----------------------------------------------
                        ' ���Z���^�[�̏ꍇ(BORDER_TIME�ɂĐؑ�)
                        '-----------------------------------------------
                        Select Case INI_JIFURI_CCFNAME
                            Case "1"
                                If NowTimeStamp < INI_JIFURI_BORDER_TIME Then
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM)
                                Else
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEPM)
                                End If
                            Case Else
                                If NowTimeStamp < INI_JIFURI_BORDER_TIME Then
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM & "*")
                                Else
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEPM & "*")
                                End If
                        End Select
                    Case Else
                        '-----------------------------------------------
                        ' ���Z���^�[�ȊO�̏ꍇ(FILEAM�g�p)
                        '-----------------------------------------------
                        Select Case INI_JIFURI_CCFNAME
                            Case "1"
                                GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM)
                            Case Else
                                GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM & "*")
                        End Select
                End Select

                If GetFilesInfo.Length > 0 Then
                    If MessageBox.Show(MSG0083I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                        ShowInTaskbar = False
                        Me.Close()
                        Exit Sub
                    End If
                End If
            End If
            ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

            '�x���}�X�^��荞��
            Dim SubSQL As String = " WHERE SUBSTR(YASUMI_DATE_Y, 1, 6)"
            SubSQL &= " IN ('" & String.Format("{0:yyyyMM}", CASTCommon.Calendar.Now.AddMonths(-1)) & "'"
            SubSQL &= ", '" & String.Format("{0:yyyyMM}", CASTCommon.Calendar.Now) & "'"
            SubSQL &= ", '" & String.Format("{0:yyyyMM}", CASTCommon.Calendar.Now.AddMonths(1)) & "')"
            Dim BRet As Boolean = GCom.CheckDateModule(Nothing, CType(1, Short), SubSQL)

            '�\���ϑ���̌����i�X�P�W���[���̌����j
            Dim KEY_HAISIN_YDATE As String = New String("0"c, 8)
            Dim HAISIN_YDATE_FROM As String = New String("0"c, 8)
            BRet = GCom.CheckDateModule(String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now), KEY_HAISIN_YDATE, DayCounter, 1)
            If DayCounter = 0 Then
                Dim nDay As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_OK"))
                If CASTCommon.GetFSKJIni("JIFURI", "HAISIN_OK") = "err" Then
                    nDay = GCom.NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN"))
                End If
                nDay -= GCom.NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN"))
                BRet = GCom.CheckDateModule(KEY_HAISIN_YDATE, HAISIN_YDATE_FROM, nDay, 0)
            Else
                '*** �C�� NISHIDA 2008/12/1 ���U���M���O�͔z�M�\�������Ɠ��������O�̂��̂�ΏۂƂ���B
                'HAISIN_YDATE_FROM = KEY_HAISIN_YDATE
                HAISIN_YDATE_FROM = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)
                '**********************************************************************************
            End If

            '2009/11/30 ���c�Ɠ����擾���� =======================
            BRet = GCom.CheckDateModule(String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now), NextData, 1, 0)
            '=====================================================

            ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
            '-----------------------------------------------------------
            ' �b�b���E���ԃ`�F�b�N
            '-----------------------------------------------------------
            If INI_JIFURI_BORDERCHK = "YES" Then
                Dim CheckFuriDate As String = NextData
                Dim GetMessage As String = String.Empty
                If NowTimeStamp >= INI_JIFURI_BORDER_TIME Then
                    If BorderCheck(CheckFuriDate, GetMessage) = False Then
                        Return
                    Else
                        If GetMessage.Trim <> "" Then
                            MessageBox.Show(GetMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    End If
                End If
            End If

            '-----------------------------------------------------------
            ' �b�b���̃G���g���������`�F�b�N
            '-----------------------------------------------------------
            If INI_JIFURI_ENTRYCHK = "YES" Then
                Dim GetMessage As String = String.Empty
                If EntryCheck(GetMessage) = False Then
                    Return
                Else
                    If GetMessage.Trim <> "" Then
                        MessageBox.Show(GetMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
            ' 2016/06/11 �^�X�N�j���� ADD �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

            SQL = New StringBuilder(128)

            SQL.Append("SELECT * FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_S = '1'")
            If DayCounter = 0 Then
                SQL.Append(" AND HAISIN_YDATE_S BETWEEN '" & KEY_HAISIN_YDATE & "' AND '" & HAISIN_YDATE_FROM & "'")
            Else
                '*** �C�� NISHIDA 2008/12/1 ���U���M���O�͔z�M�\�������Ɠ��������O�̂��́A���A�U�֓����������Ă��Ȃ����̂�ΏۂƂ���B
                SQL.Append(" AND HAISIN_YDATE_S > '" & KEY_HAISIN_YDATE & "'")
                SQL.Append(" AND HAISIN_YDATE_S < '" & HAISIN_YDATE_FROM & "'")
                '**********************************************************************************
            End If
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND FUNOU_FLG_S = '0'")
            SQL.Append(" AND (HAISIN_FLG_S = '0' OR HAISIN_FLG_S = '2')")        '�z�M�t���O��0��2
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '2010/12/24 �M�g�Ή� ���M�敪��L���ɂ���
            SQL.Append(" AND SOUSIN_KBN_S IN('0','1','2')")
            'SQL.Append(" AND SOUSIN_KBN_S IN('0')")
            SQL.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
            SQL.Append(" AND  SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
            '2009/11/30 ���Ԃɂ���ė��c�Ɠ����U�֓����̃f�[�^������
            If Now.ToString("HHmm") >= INI_JIFURI_BORDER_TIME Then
                SQL.Append(" AND  FURI_DATE_S <> " & SQ(NextData))
            End If
            '=====================================================
            '2017/06/08 �^�X�N�j���� ADD �K���M���Ή��i�w�Z�������Ή��y�W���ŏC���z�j--------------------- START
            '�ǉ�������INI�t�@�C������擾����
            If INI_JYOUKEN.Trim <> "" Then
                SQL.Append(String.Format(" {0}", INI_JYOUKEN))
            End If
            '2017/06/08 �^�X�N�j���� ADD �K���M���Ή��i�w�Z�������Ή��y�W���ŏC���z�j--------------------- END
            SQL.Append(" ORDER BY FURI_DATE_S ASC,SOUSIN_KBN_S ASC,TORIS_CODE_S ASC,TORIF_CODE_S ASC")

            htTORI_LIST = New Hashtable '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓�����

            intSCH_COUNT = 0
            'L = 1
            'P = 1
            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓����� ��������
                    Dim key As String = OraReader.GetString("TORIS_CODE_S") & OraReader.GetString("TORIF_CODE_S")

                    If htTORI_LIST.ContainsKey(key) = False Then
                        '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓����� �����܂�

                        strTORI_CODE(intSCH_COUNT) = OraReader.GetString("TORIS_CODE_S") & "-" & OraReader.GetString("TORIF_CODE_S")
                        strFURI_DATE(intSCH_COUNT) = OraReader.GetString("FURI_DATE_S")
                        strFURI_DATE(intSCH_COUNT) = strFURI_DATE(intSCH_COUNT).Substring(0, 4) & "/" & strFURI_DATE(intSCH_COUNT).Substring(4, 2) & "/" & strFURI_DATE(intSCH_COUNT).Substring(6, 2)
                        '2010/12/24 �M�g�Ή� �z�M�\����̍��ڗ��𑗐M�敪�Ɏg�p����
                        If INI_COMMON_CENTER = "0" Then
                            Select Case OraReader.GetString("SOUSIN_KBN_T")
                                Case "0"
                                    strHAISIN_YDATE(intSCH_COUNT) = "�g������"
                                Case "1"
                                    strHAISIN_YDATE(intSCH_COUNT) = "��Ǝ���"
                                Case "2"
                                    strHAISIN_YDATE(intSCH_COUNT) = "�n����"
                                Case Else
                                    strHAISIN_YDATE(intSCH_COUNT) = ""
                            End Select
                        Else
                            strHAISIN_YDATE(intSCH_COUNT) = OraReader.GetString("HAISIN_YDATE_S")
                            strHAISIN_YDATE(intSCH_COUNT) = strHAISIN_YDATE(intSCH_COUNT).Substring(0, 4) & "/" & strHAISIN_YDATE(intSCH_COUNT).Substring(4, 2) & "/" & strHAISIN_YDATE(intSCH_COUNT).Substring(6, 2)
                        End If
                        strSOUSIN_KBN(intSCH_COUNT) = OraReader.GetString("SOUSIN_KBN_T")
                        strITAKU_NNAME(intSCH_COUNT) = OraReader.GetString("ITAKU_NNAME_T")
                        strKIGYO_CODE(intSCH_COUNT) = OraReader.GetString("KIGYO_CODE_T")
                        strFURI_CODE(intSCH_COUNT) = OraReader.GetString("FURI_CODE_T")
                        ' 080207 ADD START
                        strTAKOU_KBN(intSCH_COUNT) = "���s"
                        '080207 ADD END

                        ' 2008.01.16 >>
                        strJIKOTAKO(intSCH_COUNT) = "0" ' ���s��
                        ' 2008.01.16 <<

                        intSCH_COUNT += 1

                        '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓����� ��������
                    End If

                    If INI_COMMON_CENTER = "0" Then
                        Select Case OraReader.GetString("SOUSIN_KBN_T")
                            Case "1", "2"
                                '��������ŕ����U�֓����������ꍇ�A�ŏ��̐U�֓��̂ݑΏۂƂ���
                                If htTORI_LIST.ContainsKey(key) = False Then
                                    htTORI_LIST.Add(key, 0) '2011/01/04 �ďC��
                                End If
                                htTORI_LIST.Item(key) += 1 '2011/01/04 �ďC��
                        End Select
                    End If
                    '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓����� �����܂�

                    OraReader.NextRead()
                Loop

            End If
            OraReader.Close()


            'If DayCounter = 0 Then
            '    ' 2008.1.16 >>
            '    ' ���s�Ώۃf�[�^���o
            '    Dim OraDB As New CASTCommon.MyOracle(gdbcCONNECT)
            '    Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
            '    Dim SQL As StringBuilder
            '    SQL = New StringBuilder(128)
            '    SQL.Append("SELECT ")
            '    SQL.Append(" TORIS_CODE_S")
            '    SQL.Append(",TORIF_CODE_S")
            '    SQL.Append(",FURI_DATE_S")
            '    SQL.Append(",SOUSIN_KBN_T")
            '    SQL.Append(",ITAKU_NNAME_T")
            '    SQL.Append(",KIGYO_CODE_T")
            '    SQL.Append(",FURI_CODE_T")
            '    ' 080207 ADD START
            '    SQL.Append(",HAISIN_T1FLG_S")
            '    SQL.Append(",HAISIN_T1YDATE_S")
            '    SQL.Append(",HAISIN_T2FLG_S")
            '    SQL.Append(",HAISIN_T2YDATE_S")
            '    ' 080207 ADD END
            '    SQL.Append(" FROM SCHMAST, TORIMAST")
            '    SQL.Append(" WHERE")
            '    SQL.Append("     TOUROKU_FLG_S = '1'")
            '    SQL.Append(" AND (")
            '    SQL.Append("      (HAISIN_T1FLG_S = '0' AND HAISIN_T1YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "')")
            '    SQL.Append("     OR")
            '    SQL.Append("      (HAISIN_T2FLG_S = '0' AND HAISIN_T2YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "')")
            '    SQL.Append("     )")
            '    SQL.Append(" AND YUUKOU_FLG_S = '1'")
            '    SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '    SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '    SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '    SQL.Append(" AND TAKO_KBN_T = '1'")
            '    SQL.Append(" AND FMT_KBN_T <> '20' AND FMT_KBN_T <> '21'")
            '    SQL.Append(" ORDER BY TORIS_CODE_S ASC,TORIF_CODE_S ASC")
            '    If OraReader.DataReader(SQL) = True Then
            '        Do Until OraReader.EOF = True
            '            strTORI_CODE(P, L) = OraReader.GetItem("TORIS_CODE_S") & "-" & OraReader.GetItem("TORIF_CODE_S")
            '            strFURI_DATE(P, L) = OraReader.GetItem("FURI_DATE_S")
            '            strFURI_DATE(P, L) = strFURI_DATE(P, L).Substring(0, 4) & "/" & strFURI_DATE(P, L).Substring(4, 2) & "/" & strFURI_DATE(P, L).Substring(6, 2)
            '            strHAISIN_YDATE(P, L) = KEY_HAISIN_YDATE
            '            strHAISIN_YDATE(P, L) = strHAISIN_YDATE(P, L).Substring(0, 4) & "/" & strHAISIN_YDATE(P, L).Substring(4, 2) & "/" & strHAISIN_YDATE(P, L).Substring(6, 2)
            '            strSOUSIN_KBN(P, L) = OraReader.GetItem("SOUSIN_KBN_T")
            '            strITAKU_NNAME(P, L) = OraReader.GetItem("ITAKU_NNAME_T")
            '            strKIGYO_CODE(P, L) = OraReader.GetItem("KIGYO_CODE_T")
            '            strFURI_CODE(P, L) = OraReader.GetItem("FURI_CODE_T")
            '            ' 080207 ADD START
            '            If OraReader.GetItem("HAISIN_T1FLG_S") = "0" And OraReader.GetItem("HAISIN_T1YDATE_S") = Trim(KEY_HAISIN_YDATE) Then
            '                strTAKOU_KBN(P, L) = "���P"
            '            Else
            '                strTAKOU_KBN(P, L) = "���Q"
            '            End If
            '            ' 080207 ADD END

            '            ' 2008.01.16 >>
            '            strJIKOTAKO(P, L) = "1" ' ���s��
            '            ' 2008.01.16 <<

            '            intSCH_COUNT += 1
            '            If L >= 15 Then
            '                L = 1
            '                P = P + 1
            '            Else
            '                L = L + 1
            '            End If

            '            OraReader.NextRead()
            '        Loop
            '    End If
            '    OraReader.Close()
            '    ' 2008.1.16 <<

            '    ' 2008.1.18 >>
            '    ' SSS���s�Ώۃf�[�^���o
            '    OraReader = New CASTCommon.MyOracleReader(OraDB)
            '    SQL = New StringBuilder(128)
            '    SQL.Append("SELECT")
            '    SQL.Append(" TORIS_CODE_S")
            '    SQL.Append(",TORIF_CODE_S")
            '    SQL.Append(",FURI_DATE_S")
            '    SQL.Append(",SOUSIN_KBN_T")
            '    SQL.Append(",ITAKU_NNAME_T")
            '    SQL.Append(",KIGYO_CODE_T")
            '    SQL.Append(",FURI_CODE_T")
            '    ' 080207 ADD START
            '    SQL.Append(",HAISIN_T1FLG_S")
            '    SQL.Append(",HAISIN_T1YDATE_S")
            '    SQL.Append(",HAISIN_T2FLG_S")
            '    SQL.Append(",HAISIN_T2YDATE_S")
            '    SQL.Append(",FMT_KBN_T")
            '    ' 080207 ADD END
            '    SQL.Append(" FROM SCHMAST, TORIMAST")
            '    SQL.Append(" WHERE")
            '    SQL.Append("     TOUROKU_FLG_S = '1'")
            '    SQL.Append(" AND (")
            '    SQL.Append("      (HAISIN_T1FLG_S = '0'")
            '    SQL.Append("       AND HAISIN_T1YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "'")
            '    SQL.Append("       AND FMT_KBN_T IN ('20','21')")
            '    SQL.Append("      )")
            '    SQL.Append("     OR")
            '    SQL.Append("      (HAISIN_T2FLG_S = '0'")
            '    SQL.Append("       AND HAISIN_T2YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "'")
            '    SQL.Append("       AND FMT_KBN_T = '21'")
            '    SQL.Append("      )")
            '    SQL.Append("     )")
            '    SQL.Append(" AND YUUKOU_FLG_S = '1'")
            '    SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '    SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '    SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '    SQL.Append(" ORDER BY TORIS_CODE_S ASC,TORIF_CODE_S ASC")

            '    If OraReader.DataReader(SQL) = True Then
            '        Do Until OraReader.EOF = True
            '            strTORI_CODE(P, L) = OraReader.GetItem("TORIS_CODE_S") & "-" & OraReader.GetItem("TORIF_CODE_S")
            '            strFURI_DATE(P, L) = OraReader.GetItem("FURI_DATE_S")
            '            strFURI_DATE(P, L) = strFURI_DATE(P, L).Substring(0, 4) & "/" & strFURI_DATE(P, L).Substring(4, 2) & "/" & strFURI_DATE(P, L).Substring(6, 2)
            '            strHAISIN_YDATE(P, L) = KEY_HAISIN_YDATE
            '            strHAISIN_YDATE(P, L) = strHAISIN_YDATE(P, L).Substring(0, 4) & "/" & strHAISIN_YDATE(P, L).Substring(4, 2) & "/" & strHAISIN_YDATE(P, L).Substring(6, 2)
            '            strSOUSIN_KBN(P, L) = OraReader.GetItem("SOUSIN_KBN_T")
            '            strITAKU_NNAME(P, L) = OraReader.GetItem("ITAKU_NNAME_T")
            '            strKIGYO_CODE(P, L) = OraReader.GetItem("KIGYO_CODE_T")
            '            strFURI_CODE(P, L) = OraReader.GetItem("FURI_CODE_T")
            '            ' 080207 ADD START
            '            If OraReader.GetItem("HAISIN_T1FLG_S") = "0" And _
            '               OraReader.GetItem("HAISIN_T1YDATE_S") = Trim(KEY_HAISIN_YDATE) And _
            '               (OraReader.GetItem("FMT_KBN_T") = "20" Or OraReader.GetItem("FMT_KBN_T") = "21") Then
            '                strTAKOU_KBN(P, L) = "���"
            '            Else
            '                strTAKOU_KBN(P, L) = "��O"
            '            End If

            '            ' 2008.01.16 >>
            '            strJIKOTAKO(P, L) = "2" ' SSS��
            '            ' 2008.01.16 <<

            '            intSCH_COUNT += 1

            '            If L >= 15 Then
            '                L = 1
            '                P = P + 1
            '            Else
            '                L = L + 1
            '            End If

            '            OraReader.NextRead()
            '        Loop
            '    End If
            '    OraReader.Close()

            '    OraDB.Close()
            '    ' 2008.1.18 <<
            'End If

            If intSCH_COUNT = 0 Then
                'gdbcCONNECT.Close()
                If MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning) = DialogResult.OK Then
                    ShowInTaskbar = False
                    Me.Close()
                    Exit Sub
                End If
            End If
            'gdbcCONNECT.Close()


            'intMAXCNT = P
            'intMAXLINE = L - 1
            'If intMAXLINE = 0 Then
            '    intMAXCNT = intMAXCNT - 1
            '    intMAXLINE = 15
            'End If

            With Me.ListView1
                .Clear()
                .Columns.Add("��", 25, HorizontalAlignment.Left)
                .Columns.Add("����", 40, HorizontalAlignment.Right)
                .Columns.Add("����於", 225, HorizontalAlignment.Left)
                .Columns.Add("����溰��", 100, HorizontalAlignment.Center)
                .Columns.Add("�U�֓�", 80, HorizontalAlignment.Center)
                .Columns.Add("�U�ֺ���", 64, HorizontalAlignment.Center)
                .Columns.Add("��ƺ���", 64, HorizontalAlignment.Center)
                '2010/12/24 �M�g�Ή� �z�M�\����̍��ڗ��𑗐M�敪�Ɏg�p����
                If INI_COMMON_CENTER = "0" Then
                    .Columns.Add("���M�敪", 80, HorizontalAlignment.Center)
                Else
                    .Columns.Add("�z�M�\���", 80, HorizontalAlignment.Center)
                End If
                .Columns.Add("����", 92, HorizontalAlignment.Right)
                .Columns.Add("���z", 0, HorizontalAlignment.Right)
                '   .Columns.Add("�z�M��", 0, HorizontalAlignment.Center)
                .CheckBoxes = True
            End With

            ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- START
            'KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- END

            '2007/10/18
            'If gdbcCONNECT.State = ConnectionState.Closed Then
            '    gdbcCONNECT.ConnectionString = gstrDB_CONNECT
            '    gdbcCONNECT.Open()
            'End If

            mTakouDensoList = GetTakouList(True)
            mTakouDensoGaiList = GetTakouList(False)

            Dim LineColor As Color
            Dim CharColir As Color  '2009/11/30 �ǉ�(�����F)
            Dim ROW As Integer = 0
            For PG As Integer = 0 To intSCH_COUNT - 1 Step 1

                'Dim MaxLines As Integer
                'If PG = intMAXCNT Then

                '    MaxLines = intMAXLINE
                'Else
                '    MaxLines = 15
                'End If

                'For LN As Integer = 1 To MaxLines Step 1

                ' �����Ώی��� �� �J�E���g����
                Dim TaisyouKensu() As Decimal = GetTaisyouKensu(strTORI_CODE(PG), strFURI_DATE(PG), strTAKOU_KBN(PG), OraDB)

                strKENSU(PG) = TaisyouKensu(0).ToString("#,##0")

                '2010/12/24 �M�g�Ή� ���M�敪�ǉ�
                Dim Data() As String = New String(11) {strJIKOTAKO(PG), _
                    (ROW + 1).ToString, strITAKU_NNAME(PG).TrimEnd, strTORI_CODE(PG), _
                    strFURI_DATE(PG), strFURI_CODE(PG), strKIGYO_CODE(PG), _
                    strHAISIN_YDATE(PG), TaisyouKensu(0).ToString("#,##0"), TaisyouKensu(1).ToString("#,##0"), strTAKOU_KBN(PG), strSOUSIN_KBN(PG)}

                If ROW Mod 2 = 0 Then
                    LineColor = Color.White
                Else
                    LineColor = Color.WhiteSmoke
                End If

                '2009/11/30 ���c�Ɠ�=�U�֓��̏ꍇ�ԕ�����
                If NextData = strFURI_DATE(PG).Replace("/", "") Then
                    CharColir = Color.Red
                Else
                    CharColir = Color.Black
                End If
                'Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                Dim vLstItem As New ListViewItem(Data, -1, CharColir, LineColor, Nothing)
                '========================================
                ListView1.Items.AddRange(New ListViewItem() {vLstItem})
                vLstItem.Checked = True

                ROW += 1
                'Next LN
            Next PG

            'gdbcCONNECT.Close()

            'intPAGE_CNT = 1
            'txtPage.Text = Format(intPAGE_CNT, "00") & "/" & Format(intMAXCNT, "00")
            'If intMAXCNT = 0 Then
            '    btnPreGamen.Enabled = False
            '    btnNextGamen.Enabled = False
            'End If

            '2005/03/28 ���Z���^�[�̏ꍇ�T�C�N���Ǘ��������A�����U�ւ̏ꍇ�A���X���ȍ~�U�ւŃt�@�C�������Ǘ�����
            '2011/01/27 �M�g�Ή� �M�g�̏ꍇ���T�C�N���Ǘ����Ȃ�
            If INI_COMMON_CENTER = "5" OrElse INI_COMMON_CENTER = "0" Then
                txtCycle.Text = "1"
                Label25.Enabled = False
                txtCycle.Enabled = False
            End If

            '���S�M������ �T�C�N���Ǘ��}�X�^����T�C�N���ԍ��擾 2007/10/18
            'gstrSSQL = "SELECT MAX(CYCLE_NO_F) FROM FILE_CYCLE"
            'gstrSSQL = gstrSSQL & " WHERE SYORI_DATE_F = '" & Format(CASTCommon.Calendar.Now, "yyyyMMdd") & "'"
            'gstrSSQL = gstrSSQL & " ORDER BY CYCLE_NO_F DESC"

            '2007/10/18
            'If gdbcCONNECT.State = ConnectionState.Closed Then
            '    gdbcCONNECT.ConnectionString = gstrDB_CONNECT
            '    gdbcCONNECT.Open()
            'End If

            'gdbCOMMAND = New OracleClient.OracleCommand
            'gdbCOMMAND.CommandText = gstrSSQL
            'gdbCOMMAND.Connection = gdbcCONNECT

            'gdbrREADER = gdbCOMMAND.ExecuteReader '�Ǎ��̂�
            'If gdbrREADER.Read = False Then
            '    txtSaikuru.Text = "1"
            'Else
            '    If CInt(clsFUSION.fn_chenge_null(gdbrREADER.Item(0), 0)) = 9 Then
            '        txtSaikuru.Text = "1"
            '    Else
            '        txtSaikuru.Text = CStr(CInt(clsFUSION.fn_chenge_null(gdbrREADER.Item(0), 0)) + 1)
            '    End If
            'End If

            'gdbcCONNECT.Close()

            Me.Visible = True

            txtCycle.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not OraDB Is Nothing Then OraDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub
#End Region

#Region "�I���{�^��"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)�J�n", "����", "")

            Me.Close()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)�I��", "����", "")
        End Try

    End Sub
#End Region

#Region "����{�^��"
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        '============================================================================
        'NAME           :btnPrint_Click
        'Parameter      :
        'Description    :����{�^���������̏���
        'Return         :
        'Create         :2004/08/13
        'Update         :
        '============================================================================

        Dim mRet As Integer
        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFJP009


        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", "������")
                Return
            End If

            mRet = MessageBox.Show(MSG0013I.Replace("{0}", "�Z���^�[�J�b�g�f�[�^�쐬�Ώۈꗗ"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If mRet <> DialogResult.OK Then
                Return
            End If

            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems

                CreateCSV.OutputCsvData(item.SubItems(1).Text)        '����
                CreateCSV.OutputCsvData(SyoriDate)        '������
                CreateCSV.OutputCsvData(SyoriTime)        '�^�C���X�^���v
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(0, 10))        '������R�[�h
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(11, 2))        '����敛�R�[�h
                CreateCSV.OutputCsvData(item.SubItems(2).Text)        '����於�i�����j
                CreateCSV.OutputCsvData(item.SubItems(5).Text)        '�U�փR�[�h
                CreateCSV.OutputCsvData(item.SubItems(6).Text)        '��ƃR�[�h
                '2010/12/24 �M�g�Ή� �z�M�\����̍��ڗ��𑗐M�敪�Ɏg�p����
                If INI_COMMON_CENTER = "0" Then
                    CreateCSV.OutputCsvData(item.SubItems(7).Text)                     '���M�敪
                Else
                    CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""))    '�z�M�\���
                End If
                CreateCSV.OutputCsvData(item.SubItems(4).Text.Replace("/", ""))        '�U�֓�
                CreateCSV.OutputCsvData(item.SubItems(8).Text.Replace(",", ""))        '��������
                CreateCSV.OutputCsvData(item.SubItems(9).Text.Replace(",", ""))        '�������z
                CreateCSV.OutputCsvData(item.SubItems(10).Text, False, True)        '�z�M��

            Next
            CreateCSV.CloseCsv()

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C�����A�����R�[�h
            param = GCom.GetUserID & "," & strCSVFileName
            iRet = ExeRepo.ExecReport("KFJP009.EXE", param)

            If iRet <> 0 Then
                '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                Select Case iRet
                    Case -1
                        errMsg = MSG0226W.Replace("{0}", "�Z���^�[�J�b�g�f�[�^�쐬�Ώۈꗗ")
                        MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        errMsg = MSG0004E.Replace("{0}", "�Z���^�[�J�b�g�f�[�^�쐬�Ώۈꗗ")
                        MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Select

               MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�Z���^�[�J�b�g�f�[�^�쐬�Ώۈꗗ���", "���s")
                Return
            Else
                '2009/12/17 ����������b�Z�[�W�ǉ�
                MessageBox.Show(String.Format(MSG0014I, "�Z���^�[�J�b�g�f�[�^�쐬�Ώۈꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                '===============================
            End If

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
        End Try

    End Sub

#End Region

#Region "���s�{�^��"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :�쐬�{�^���������̏���
        'Return         :
        'Create         :2004/08/16
        'Update         :
        '=====================================================================================


        Dim MainDB As New CASTCommon.MyOracle()
        Dim SQL As StringBuilder
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)�J�n", "����", "")

            '2005/03/28 ���Z���^�[�̏ꍇ�T�C�N���Ǘ��������A�����U�ւ̏ꍇ�A���X���ȍ~�U�ւ�
            '�t�@�C�������Ǘ�����
            If INI_COMMON_CENTER <> "5" Then
                '2011/01/27 �M�g�Ή� �M�g�̏ꍇ���T�C�N���Ǘ����Ȃ� ��������
                If INI_COMMON_CENTER = "0" Then
                    If MessageBox.Show(MSG0051I.Replace("�T�C�N���ԍ���{0}�ł�", "�Z���^�[�J�b�g�f�[�^���쐬���܂�"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                        txtCycle.Focus()
                        Exit Sub
                    End If
                Else
                    '2011/01/27 �M�g�Ή� �M�g�̏ꍇ���T�C�N���Ǘ����Ȃ� �����܂�

                    '------------------------------
                    '�T�C�N���ԍ��̃`�F�b�N
                    '------------------------------
                    If txtCycle.Text.Length <> 0 Then
                        Select Case Integer.Parse(txtCycle.Text)
                            Case 1 To 9
                            Case Else
                                MessageBox.Show(MSG0329W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                txtCycle.Focus()
                                Exit Sub
                        End Select
                    Else
                        MessageBox.Show(MSG0330W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtCycle.Focus()
                        Exit Sub
                    End If

                    If MessageBox.Show(MSG0051I.Replace("{0}", txtCycle.Text), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                        txtCycle.Focus()
                        Exit Sub
                    End If
                End If '2011/01/27 �M�g�Ή� If����

            Else    '���Z���^�[
                '2010.02.16 start

                '���ݎ����̎擾
                Dim strTime As String = CASTCommon.Calendar.Now.ToString("HHmm")
                Dim strFile As String
                Dim strErr As String = "err"

                ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
                ''2010.02.19 ���E���Ԃ�ini���擾 start
                'Dim strBorder As String = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
                'If strBorder = "err" OrElse strBorder = "" Then
                '    strBorder = "1200"
                'End If
                ''2010.02.19 ���E���Ԃ�ini���擾 end
                ' 2016/06/11 �^�X�N�j���� DEL �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

                ' 2016/06/11 �^�X�N�j���� CHG �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
                'If strTime < strBorder Then
                '    'INI�t�@�C�����̎擾
                '    '[JIFURI]FILEAM
                '    strFile = CASTCommon.GetFSKJIni("JIFURI", "FILEAM")
                '    If strFile = strErr Then
                '        MessageBox.Show(String.Format(MSG0001E, "�Z���^�[�J�b�g�t�@�C����", "JIFURI", "FILEAM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '        txtCycle.Focus()
                '        Return
                '    End If
                'Else
                '    'INI�t�@�C�����̎擾
                '    '[JIFURI]FILEPM
                '    strFile = CASTCommon.GetFSKJIni("JIFURI", "FILEPM")
                '    If strFile = strErr Then
                '        MessageBox.Show(String.Format(MSG0001E, "�Z���^�[�J�b�g�t�@�C����", "JIFURI", "FILEPM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '        txtCycle.Focus()
                '        Return
                '    End If
                'End If
                If strTime < INI_JIFURI_BORDER_TIME Then
                    strFile = INI_JIFURI_FILEAM
                Else
                    strFile = INI_JIFURI_FILEPM
                End If
                ' 2016/06/11 �^�X�N�j���� CHG �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

                If MessageBox.Show(String.Format(MSG0071I, strFile), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                    txtCycle.Focus()
                    Return
                End If
                '2010.02.16 end
            End If


            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '******************************************
            ' �`�F�b�N�{�b�N�X�̃`�F�b�N
            '******************************************
            '���X�g�ɂP�����\������Ă��Ȃ��Ƃ�
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", "������")
                Return
            Else
                '���X�g�ɂP���ȏ�\������Ă��邪�A�`�F�b�N����Ă��Ȃ��Ƃ�
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", "���I��")
                    Return
                End If
            End If

            '------------------------------
            '�쐬�m�F���b�Z�[�W
            '------------------------------
            Dim sMessage As String
            sMessage = MSG0023I.Replace("{0}", "�Z���^�[�J�b�g�f�[�^�쐬") & vbCrLf
            Dim sJikoFuriDate As String = ""

            ' 2016/06/11 �^�X�N�j���� CHG �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- START
            '2010/09/09.Sakon�@�������擾�A6�����ȏ�Ń��b�Z�[�W�o�� +++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'Dim GoukeiKensuu As Integer
            'Dim KiteiKensuu As String
            'For Each item As ListViewItem In nSelectItems
            '    '2010/12/24 �M�g�Ή� �����J�E���g�͑��M�敪=0�̂�
            '    'If item.SubItems(0).Text = "0" AndAlso item.SubItems(8).Text <> "" Then
            '    If item.SubItems(0).Text = "0" AndAlso item.SubItems(8).Text <> "" AndAlso item.SubItems(11).Text = "0" Then
            '        GoukeiKensuu += CInt(item.SubItems(8).Text)
            '    End If
            'Next
            'KiteiKensuu = CASTCommon.GetFSKJIni("JIFURI", "KITEIKEN")
            'If KiteiKensuu = "err" Then
            '    MessageBox.Show("�h�m�h�t�@�C���擾���s" & vbCrLf & "�Z���^�[�J�b�g�f�[�^���M�K�茏�������ݒ�ł��B", _
            '                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Else
            '    If GoukeiKensuu >= KiteiKensuu Then
            '        If MessageBox.Show("���v���M�������K��l�𒴉߂��Ă��܂��B" & vbCrLf & _
            '                           "�K��l���ƂɃt�@�C���𕪊����ď����𑱍s���܂����H" & vbCrLf & _
            '                           "�K�茏���F" & KiteiKensuu & " ��" & vbCrLf & _
            '                           "���������F" & GoukeiKensuu & " ��", msgTitle, _
            '                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
            '            Return
            '        End If
            '    End If
            'End If
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '-------------------------------------------------------
            ' �Z���^�[�J�b�g���M�����J�E���g�i���M�敪=0 �̂݁j
            '-------------------------------------------------------
            Dim GoukeiKensuu As Integer
            For Each item As ListViewItem In nSelectItems
                If item.SubItems(0).Text = "0" AndAlso item.SubItems(8).Text <> "" AndAlso item.SubItems(11).Text = "0" Then
                    GoukeiKensuu += CInt(item.SubItems(8).Text)
                End If
            Next

            '-------------------------------------------------------
            ' �Z���^�[�J�b�g���M�����`�F�b�N
            '-------------------------------------------------------
            If GoukeiKensuu >= CInt(INI_JIFURI_KITEIKEN.trim) Then
                If MessageBox.Show("���v���M�������K��l�𒴉߂��Ă��܂��B" & vbCrLf & _
                                   "�K��l���ƂɃt�@�C���𕪊����ď����𑱍s���܂����H" & vbCrLf & _
                                   "�K�茏���F" & CInt(INI_JIFURI_KITEIKEN.trim) & " ��" & vbCrLf & _
                                   "���������F" & GoukeiKensuu & " ��", msgTitle, _
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                    Return
                End If
            End If
            ' 2016/06/11 �^�X�N�j���� CHG �yPG�zUI-03-02(RSV2�Ή�<���l�M��>) -------------------- END

            For Each item As ListViewItem In nSelectItems
                If item.SubItems(0).Text = "0" Then
                    sJikoFuriDate = item.SubItems(4).Text.Replace("/", "")
                    Exit For
                End If
            Next
            If sJikoFuriDate <> "" Then
                sMessage &= "�y���s�z" & vbCrLf
                sMessage &= "�U�֓��F" & sJikoFuriDate & vbCrLf
                sMessage &= "�z�M���F" & Format(System.DateTime.Today, "yyyy/MM/dd")
                sMessage &= vbCrLf & vbCrLf
            End If

            'For ii As Integer = 1 To intMAXCNT
            '    For jj As Integer = 1 To 15
            '        If strJIKOTAKO(ii, jj) = "0" Then
            '            sJikoFuriDate = strFURI_DATE(ii, jj)
            '            Exit For
            '        End If
            '    Next jj
            'Next ii
            'If sJikoFuriDate <> "" Then
            '    sMessage &= "�y���s�z" & vbCrLf
            '    sMessage &= "�U�֓��F" & sJikoFuriDate & vbCrLf
            '    sMessage &= "�z�M���F" & Format(System.DateTime.Today, "yyyy/MM/dd")
            '    sMessage &= vbCrLf & vbCrLf
            'End If

            'Dim sTakoFuriDate As String = ""
            'For ii As Integer = 1 To intMAXCNT
            '    For jj As Integer = 1 To 15
            '        If strJIKOTAKO(ii, jj) = "1" Then
            '            sTakoFuriDate = strFURI_DATE(ii, jj)
            '            Exit For
            '        End If
            '    Next jj
            'Next ii
            'If sTakoFuriDate <> "" Then
            '    sMessage &= "�y���s�z" & vbCrLf
            '    sMessage &= "�U�֓��F" & sTakoFuriDate & vbCrLf
            '    sMessage &= "�z�M���F" & Format(System.DateTime.Today, "yyyy/MM/dd")
            '    sMessage &= vbCrLf & vbCrLf
            'End If

            'Dim sSSSFuriDate As String = ""
            'For ii As Integer = 1 To intMAXCNT
            '    For jj As Integer = 1 To 15
            '        If strJIKOTAKO(ii, jj) = "2" Then
            '            sSSSFuriDate = strFURI_DATE(ii, jj)
            '            Exit For
            '        End If
            '    Next jj
            'Next ii
            'If sSSSFuriDate <> "" Then
            '    sMessage &= "�ySSS�z" & vbCrLf
            '    sMessage &= "�U�֓��F" & sSSSFuriDate & vbCrLf
            '    sMessage &= "�z�M���F" & Format(System.DateTime.Today, "yyyy/MM/dd")
            '    sMessage &= vbCrLf & vbCrLf
            'End If
            'If MessageBox.Show(sMessage, "�Z���^�[�J�b�g�f�[�^�쐬", MessageBoxButtons.YesNo) = DialogResult.No Then
            '    Exit Sub
            'End If

            MainDB.BeginTrans()
            '********************************************
            ' �X�P�W���[���}�X�^�̍X�V
            '********************************************
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems
                SQL = New StringBuilder(128)
                If item.SubItems(0).Text = "0" Then

                    If item.Checked = True Then
                        SQL.Append("UPDATE SCHMAST SET HAISIN_FLG_S = '2' ")
                    Else
                        SQL.Append("UPDATE SCHMAST SET HAISIN_FLG_S = '0' ")
                    End If

                    SQL.Append(" WHERE TORIS_CODE_S = '" & item.SubItems(3).Text.Substring(0, 10) & "' ")
                    SQL.Append(" AND TORIF_CODE_S = '" & item.SubItems(3).Text.Substring(11, 2) & "' ")
                    SQL.Append(" AND FURI_DATE_S = '" & item.SubItems(4).Text.Replace("/", "") & "' ")
                    Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                    If nRet = 0 Then
                        MainDB.Rollback()
                        Return
                    ElseIf nRet < 0 Then
                        Throw New Exception(String.Format(MSG0027E, "�X�P�W���[���}�X�^", "�X�V"))
                    End If
                End If
            Next

            '2010/12/24 �M�g�Ή� ���M�敪���ɃW���u�o�^����悤���W�b�N�ύX ��������
            Dim JikoFlg As Boolean = False
            Dim SounsinKbn As New ArrayList
            '���M�敪�����X�g�ɒǉ�
            For Each item As ListViewItem In nSelectItems
                If item.SubItems(0).Text <> "0" Then
                    Exit For
                End If

                If SounsinKbn.Contains(item.SubItems(11).Text) = False Then
                    SounsinKbn.Add(item.SubItems(11).Text)
                End If
            Next
            SounsinKbn.Sort() '�l���Ƀ\�[�g

            For i As Integer = 0 To SounsinKbn.Count - 1
                '2010/12/24 �M�g�Ή� ���M�敪���ɃW���u�o�^����悤���W�b�N�ύX �����܂�
                '------------------------------
                'JOBMAST�ɓo�^
                '------------------------------
                Dim strJOBID As String, strPARAMETA As String

                Dim minFuriDate As String = "99999999"
                Dim maxFuriDate As String = ""

                For Each item As ListViewItem In nSelectItems
                    If item.SubItems(0).Text <> "0" Then
                        Exit For
                    End If

                    ' �ŏ��l

                    If String.Compare(minFuriDate, item.SubItems(4).Text.Replace("/", "")) >= 0 Then
                        minFuriDate = item.SubItems(4).Text.Replace("/", "")
                    End If

                    ' �ő�l
                    If String.Compare(maxFuriDate, item.SubItems(4).Text.Replace("/", "")) <= 0 Then
                        maxFuriDate = item.SubItems(4).Text.Replace("/", "")
                    End If

                Next
                'For i = 1 To intMAXCNT
                '    For j = 1 To 15
                '        If strJIKOTAKO(i, j) <> "0" Then
                '            ' ���s�ȊO�́C�����𔲂���
                '            Exit For
                '        End If

                '        ' �ŏ��l
                '        If String.Compare(minFuriDate, strFURI_DATE(i, j)) >= 0 Then
                '            minFuriDate = strFURI_DATE(i, j)
                '        End If

                '        ' �ő�l
                '        If String.Compare(maxFuriDate, strFURI_DATE(i, j)) <= 0 Then
                '            maxFuriDate = strFURI_DATE(i, j)
                '        End If
                '    Next j
                'Next i

                Dim iRet As Integer
                '2010/12/24 �M�g�Ή� �R�����g�A�E�g
                'Dim JikoFlg As Boolean = False

                If minFuriDate <> "99999999" AndAlso maxFuriDate <> "" Then
                    strJOBID = "J030"
                    strPARAMETA = minFuriDate.Replace("/", "")
                    strPARAMETA &= "," & maxFuriDate.Replace("/", "")
                    '2010/12/24 �M�g�Ή� ���������M�敪���p�����[�^�ݒ肷��
                    'strPARAMETA &= ",0," & txtSaikuru.Text
                    strPARAMETA &= "," & SounsinKbn(i) & "," & txtCycle.Text

                    iRet = MainLOG.SearchJOBMAST(strJOBID, strPARAMETA, MainDB)
                    If iRet = 1 Then
                        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        Return
                    ElseIf iRet = -1 Then
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainDB.Rollback()
                        Return
                    End If
                    If MainLOG.InsertJOBMAST(strJOBID, LW.UserID, strPARAMETA, MainDB) = False Then
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainDB.Rollback()
                        Return
                    End If

                    JikoFlg = True
                    'If clsFUSION.fn_INSERT_JOBMAST(strJOBID, gstrUSER_ID, strPARAMETA) = False Then
                    '    MessageBox.Show("�Z���^�[�J�b�g�f�[�^�쐬�����̋N���Ɏ��s���܂����B" & "�G���[�F" & Err.Description)
                    '    '*** 2008/6/24 kaki ************************************************
                    '    '*** ��ʕ\����MAX(15)�̏ꍇj��16�ƂȂ�A���O�o�͂��G���[�ƂȂ�
                    '    If j = 16 Then
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "�N���p�����^�̓o�^", "���s", "���M�敪�F" & strSOUSIN_KBN(i - 1, j - 1) & " " & "�T�C�N���ԍ��F" & txtSaikuru.Text & "�@" & Err.Description)
                    '    Else
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "�N���p�����^�̓o�^", "���s", "���M�敪�F" & strSOUSIN_KBN(i, j) & " " & "�T�C�N���ԍ��F" & txtSaikuru.Text & "�@" & Err.Description)
                    '    End If
                    '    '*** 2008/6/24 kaki ************************************************
                    '    Exit Sub
                    'Else
                    '    '*** 2008/6/24 kaki ************************************************
                    '    '*** ��ʕ\����MAX(15)�̏ꍇj��16�ƂȂ�A���O�o�͂��G���[�ƂȂ�
                    '    If j = 16 Then
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "�N���p�����^�̓o�^", "����", "���M�敪�F" & strSOUSIN_KBN(i - 1, j - 1) & " " & "�T�C�N���ԍ��F" & txtSaikuru.Text & "�@" & Err.Description)
                    '    Else
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "�N���p�����^�̓o�^", "����", "���M�敪�F" & strSOUSIN_KBN(i, j) & " " & "�T�C�N���ԍ��F" & txtSaikuru.Text & "�@" & Err.Description)
                    '    End If
                    '    '*** 2008/6/24 kaki ************************************************
                    'End If
                End If

            Next '2010/12/24 �M�g�Ή� ForEach����

            ' 2008.01.16 >>
            Dim TakoFlag As Boolean = False
            Dim SSSFlag As Boolean = False

            MainDB.Commit()

            sMessage = MSG0021I.Replace("{0}", "�Z���^�[�J�b�g�f�[�^�쐬")
            sMessage &= vbCrLf
            If JikoFlg = True Then
                'sMessage &= vbCrLf & "  �y�Z���^�[�J�b�g�f�[�^�쐬�����z"
                'MessageBox.Show("�Z���^�[�J�b�g�f�[�^�쐬������" & vbCrLf & "�N���p�����[�^��o�^���܂���", "�Z���^�[�J�b�g�f�[�^�쐬")
            End If
            If TakoFlag = True Then
                'sMessage &= vbCrLf & "  �y���s�f�[�^�쐬�����z"
                'MessageBox.Show("���s�f�[�^�쐬������" & vbCrLf & "�N���p�����[�^��o�^���܂���", "���s�f�[�^�쐬")
            End If
            If SSSFlag = True Then
                'sMessage &= vbCrLf & "  �y�r�r�r�f�[�^�쐬�����z"
                'MessageBox.Show("���s�f�[�^�쐬������" & vbCrLf & "�N���p�����[�^��o�^���܂���", "���s�f�[�^�쐬")
            End If
            '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓����� ��������
            If htTORI_LIST.Count > 0 Then
                '2011/01/04 �ďC�� ��������
                For Each i As Integer In htTORI_LIST.Values
                    If i > 1 Then
                        sMessage &= vbCrLf
                        sMessage &= "����������ŕ����U�֓��̑Ώۂ����݂��邽�߁A�ŏ��̐U�֓��̂݃f�[�^�쐬���܂��B"

                        Exit For
                    End If
                Next
                '2011/01/04 �ďC�� �����܂�
            End If
            '2010/12/24 �M�g�Ή� ��Ǝ����̕����U�֓����� �����܂�

            MessageBox.Show(sMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            btnAction.Enabled = False
            ' 2008.01.16 <<

        Catch ex As Exception
            MainDB.Rollback()
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)��O�G���[", "���s", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)�I��", "����", "")
        End Try

    End Sub
#End Region

    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick

        Try

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
        Catch ex As Exception

        End Try
    End Sub

    Private Function GetTaisyouKensu(ByVal toriCode As String, ByVal furiData As String, ByVal takouKubun As String, ByVal db As CASTCommon.MyOracle) As Decimal()
        Dim SQL As New StringBuilder(128)

        Dim OraReader As New CASTCommon.MyOracleReader(db)

        Try
            SQL.Append("SELECT COUNT(*) KENSU,SUM(FURIKIN_K) AS KIN FROM MEIMAST")
            SQL.Append(" WHERE TORIS_CODE_K = '" & toriCode.Split("-")(0) & "'")
            SQL.Append("   AND TORIF_CODE_K = '" & toriCode.Split("-")(1) & "'")
            SQL.Append("   AND FURI_DATE_K  = '" & furiData.Replace("/", "") & "'")
            SQL.Append("   AND FURIKETU_CODE_K  = '0'")
            Select Case takouKubun
                Case "���s"
                    SQL.Append("   AND KEIYAKU_KIN_K  = '" & INI_COMMON_KINKOCD & "'")
                Case "���P"
                    '
                    If mTakouDensoList.Length > 0 Then
                        SQL.Append("   AND KEIYAKU_KIN_K  IN (" & String.Join(",", mTakouDensoList) & ")")
                    End If
                Case "���Q"
                    '
                    If mTakouDensoList.Length > 0 Then
                        SQL.Append("   AND KEIYAKU_KIN_K  IN (" & String.Join(",", mTakouDensoGaiList) & ")")
                    End If
                Case "���"
                    SQL.Append("   AND KEIYAKU_KIN_K  <> '" & INI_COMMON_KINKOCD & "'")
                    SQL.Append("   AND EXISTS (SELECT KIN_NO_N FROM TENMAST")
                    SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
                    SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
                    SQL.Append("      AND TEIKEI_KBN_N = '1')")
                Case "��O"
                    SQL.Append("   AND KEIYAKU_KIN_K  <> '" & INI_COMMON_KINKOCD & "'")
                    SQL.Append("   AND EXISTS (SELECT KIN_NO_N FROM TENMAST")
                    SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
                    SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
                    SQL.Append("      AND TEIKEI_KBN_N = '0')")
                Case Else
                    SQL.Append("   AND KEIYAKU_KIN_K  = '" & INI_COMMON_KINKOCD & "'")
            End Select

            Static ss As Integer = 1

            If OraReader.DataReader(SQL) = True Then
                Return New Decimal(1) {OraReader.GetInt64("KENSU"), OraReader.GetInt64("KIN")}
            Else
                Return New Decimal(1) {0, 0}
            End If
        Catch ex As Exception
            Throw
        Finally
            OraReader.Close()
        End Try

        Return New Decimal(1) {0, 0}
    End Function

    ' 2016/06/10 �^�X�N�j���� ADD �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- START
    Private Function GetIniInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "�J�n", "")

            '-------------------------------------------------------
            ' �`���t�H���_ [COMMON]-[DEN]
            '-------------------------------------------------------
            INI_COMMON_DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If INI_COMMON_DEN.ToUpper = "ERR" OrElse INI_COMMON_DEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�`���t�H���_", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�`���t�H���_ [COMMON]-[DEN]")
                Return False
            End If

            '-------------------------------------------------------
            ' �w��Z���^�[ [COMMON]-[CENTER]
            '-------------------------------------------------------
            INI_COMMON_CENTER = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If INI_COMMON_CENTER.ToUpper = "ERR" OrElse INI_COMMON_CENTER = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�w��Z���^�[", "COMMON", "CENTER"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�w��Z���^�[ [COMMON]-[CENTER]")
                Return False
            End If

            '-------------------------------------------------------
            ' �����ɃR�[�h [COMMON]-[KINKOCD]
            '-------------------------------------------------------
            INI_COMMON_KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If INI_COMMON_KINKOCD.ToUpper = "ERR" OrElse INI_COMMON_KINKOCD = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�����ɃR�[�h", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�����ɃR�[�h [COMMON]-[KINKOCD]")
                Return False
            End If

            '-------------------------------------------------------
            ' �b�b�f�[�^�t�@�C����(�`�l) [JIFURI]-[FILEAM]
            '-------------------------------------------------------
            INI_JIFURI_FILEAM = CASTCommon.GetFSKJIni("JIFURI", "FILEAM")
            If INI_JIFURI_FILEAM.ToUpper = "ERR" OrElse INI_JIFURI_FILEAM = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�Z���^�[�J�b�g�t�@�C����", "JIFURI", "FILEAM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�b�b�f�[�^�t�@�C����(�`�l) [JIFURI]-[FILEAM]")
                Return False
            End If

            '-------------------------------------------------------
            ' �b�b�f�[�^�t�@�C����(�o�l) [JIFURI]-[FILEPM]
            '-------------------------------------------------------
            INI_JIFURI_FILEPM = CASTCommon.GetFSKJIni("JIFURI", "FILEPM")
            If INI_JIFURI_FILEPM.ToUpper = "ERR" OrElse INI_JIFURI_FILEPM = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�Z���^�[�J�b�g�t�@�C����", "JIFURI", "FILEPM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�b�b�f�[�^�t�@�C����(�o�l) [JIFURI]-[FILEPM]")
                Return False
            End If

            '-------------------------------------------------------
            ' �Z���^�[�J�b�g�f�[�^���M�K�茏�� [JIFURI]-[KITEIKEN]
            '-------------------------------------------------------
            INI_JIFURI_KITEIKEN = CASTCommon.GetFSKJIni("JIFURI", "KITEIKEN")
            If INI_JIFURI_KITEIKEN.ToUpper = "ERR" OrElse INI_JIFURI_KITEIKEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�Z���^�[�J�b�g�f�[�^���M�K�茏��", "JIFURI", "KITEIKEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�Z���^�[�J�b�g�f�[�^���M�K�茏�� [JIFURI]-[KITEIKEN]")
                Return False
            End If

            '-------------------------------------------------------
            ' �b�b���E���� [JIFURI]-[BORDER_TIME]
            '-------------------------------------------------------
            INI_JIFURI_BORDER_TIME = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
            If INI_JIFURI_BORDER_TIME.ToUpper = "ERR" OrElse INI_JIFURI_BORDER_TIME = Nothing OrElse INI_JIFURI_BORDER_TIME = "" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�b�b���E���� [JIFURI]-[BORDER_TIME]")
                INI_JIFURI_BORDER_TIME = "1200"
            End If

            '-------------------------------------------------------
            ' �b�b�f�[�^���݃`�F�b�N [JIFURI]-[CCDATACHK]
            '-------------------------------------------------------
            INI_JIFURI_CCDATACHK = CASTCommon.GetFSKJIni("JIFURI", "CCDATACHK")
            If INI_JIFURI_CCDATACHK.ToUpper = "ERR" OrElse INI_JIFURI_CCDATACHK = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�b�b�f�[�^���݃`�F�b�N [JIFURI]-[CCDATACHK]")
            End If

            '-------------------------------------------------------
            ' �b�b���E���ԃ`�F�b�N [JIFURI]-[BORDERCHK]
            '-------------------------------------------------------
            INI_JIFURI_BORDERCHK = CASTCommon.GetFSKJIni("JIFURI", "BORDERCHK")
            If INI_JIFURI_BORDERCHK.ToUpper = "ERR" OrElse INI_JIFURI_BORDERCHK = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�b�b���E���ԃ`�F�b�N [JIFURI]-[BORDERCHK]")
            End If

            '-------------------------------------------------------
            ' �G���g���������`�F�b�N [JIFURI]-[ENTRYCHK]
            '-------------------------------------------------------
            INI_JIFURI_ENTRYCHK = CASTCommon.GetFSKJIni("JIFURI", "ENTRYCHK")
            If INI_JIFURI_ENTRYCHK.ToUpper = "ERR" OrElse INI_JIFURI_ENTRYCHK = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�G���g���������`�F�b�N [JIFURI]-[ENTRYCHK]")
            End If

            '-------------------------------------------------------
            ' �Z���^�[�J�b�g�t�@�C�����ݒ� [JIFURI]-[CCFNAME]
            '-------------------------------------------------------
            INI_JIFURI_CCFNAME = CASTCommon.GetFSKJIni("JIFURI", "CCFNAME")
            If INI_JIFURI_CCFNAME.ToUpper = "ERR" OrElse INI_JIFURI_CCFNAME = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�Z���^�[�J�b�g�t�@�C�����ݒ� [JIFURI]-[CCFNAME]")
            End If

            '2017/06/08 �^�X�N�j���� ADD �K���M���Ή��i�w�Z�������Ή��y�W���ŏC���z�j--------------------- START
            '-------------------------------------------------------
            ' �Z���^�[�J�b�g�Ώۏ����ǉ� [FORM]-[KFJMAIN030_JYOUKEN]
            '-------------------------------------------------------
            INI_JYOUKEN = CASTCommon.GetRSKJIni("FORM", "KFJMAIN030_JYOUKEN")
            If INI_JYOUKEN.ToUpper = "ERR" OrElse INI_JYOUKEN = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", "�Z���^�[�J�b�g�Ώۏ����ǉ� [FORM]-[KFJMAIN030_JYOUKEN]")
                INI_JYOUKEN = ""
            End If
            '2017/06/08 �^�X�N�j���� ADD �K���M���Ή��i�w�Z�������Ή��y�W���ŏC���z�j--------------------- END

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "����", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�h�m�h���擾", "�I��", "")
        End Try

    End Function

    Private Function BorderCheck(ByVal CheckFuriDate As String, ByRef ReturnMessage As String) As Boolean

        Dim FunctionTitle As String = "�b�b���E���ԃ`�F�b�N"

        Dim SQL As StringBuilder = Nothing
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "�J�n", "")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")
            SQL.Append("   , ITAKU_NNAME_T")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append("   , SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FSYORI_KBN_S   = '1'")
            SQL.Append(" AND TORIS_CODE_T   = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T   = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S    = '" & CheckFuriDate & "'")
            SQL.Append(" AND SOUSIN_KBN_S   = '0'")
            SQL.Append(" AND TOUROKU_FLG_S  = '1'")
            SQL.Append(" AND HAISIN_FLG_S   = '0'")
            SQL.Append(" AND FUNOU_FLG_S    = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S  = '0'")
            SQL.Append(" ORDER BY")
            SQL.Append("     SOUSIN_KBN_S")
            SQL.Append("   , TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim FirstFlg As Integer = 1

            If OraReader.DataReader(SQL) = True Then
                Do While OraReader.EOF = False
                    If FirstFlg = 1 Then
                        Dim MsgFuriDate As String = CheckFuriDate.Substring(0, 4) & "�N" & _
                                       CheckFuriDate.Substring(4, 2) & "��" & _
                                       CheckFuriDate.Substring(6, 2) & "��"
                        ReturnMessage = "�U�֓�:" & MsgFuriDate & "�̐U�փf�[�^�ő��M�ł��Ȃ����̂�����܂��B" & vbCrLf
                        FirstFlg = 0
                    End If

                    ReturnMessage += vbCrLf & _
                                                  "�����R�[�h:" & OraReader.GetString("TORIS_CODE_S") & "-" & OraReader.GetString("TORIF_CODE_S") & _
                                                  " �ϑ��Җ�:" & OraReader.GetString("ITAKU_NNAME_T").Trim
                    OraReader.NextRead()
                Loop
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "����", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "�I��", "")
        End Try

    End Function

    Private Function EntryCheck(ByRef ReturnMessage As String) As Boolean

        Dim FunctionTitle As String = "�G���g���������`�F�b�N"

        Dim SQL As StringBuilder = Nothing
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "�J�n", "")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")
            SQL.Append("   , FURI_DATE_S")
            SQL.Append("   , ITAKU_NNAME_T")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append("   , SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FSYORI_KBN_S   = '1'")
            SQL.Append(" AND TORIS_CODE_T   = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T   = TORIF_CODE_S")
            SQL.Append(" AND BAITAI_CODE_T IN ('04', '09')")
            SQL.Append(" AND UKETUKE_FLG_S  = '1'")
            SQL.Append(" AND TOUROKU_FLG_S  = '0'")
            SQL.Append(" AND HAISIN_FLG_S   = '0'")
            SQL.Append(" AND FUNOU_FLG_S    = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S  = '0'")
            SQL.Append(" ORDER BY")
            SQL.Append("     FURI_DATE_S")
            SQL.Append("   , TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim FirstFlg As Integer = 1

            If OraReader.DataReader(SQL) = True Then


                Do While OraReader.EOF = False

                    If FirstFlg = 1 Then
                        ReturnMessage = "�G���g���f�[�^���͍ςŗ��������s�̃f�[�^�����݂��܂��B" & vbCrLf
                        FirstFlg = 0
                    End If

                    Dim MsgFuriDate As String = OraReader.GetString("FURI_DATE_S").Substring(0, 4) & "�N" & _
                                                OraReader.GetString("FURI_DATE_S").Substring(4, 2) & "��" & _
                                                OraReader.GetString("FURI_DATE_S").Substring(6, 2) & "��"

                    ReturnMessage += vbCrLf & _
                                     "�U�֓�:" & MsgFuriDate & _
                                     " �����R�[�h:" & OraReader.GetString("TORIS_CODE_S") & "-" & OraReader.GetString("TORIF_CODE_S") & _
                                     " �ϑ��Җ�:" & OraReader.GetString("ITAKU_NNAME_T").Trim

                    OraReader.NextRead()
                Loop
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "����", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "�I��", "")
        End Try

    End Function
    ' 2016/06/10 �^�X�N�j���� ADD �yPG�zUI-03-06(RSV2�Ή�<���l�M��>) -------------------- START

    Private Function GetTakouList(ByVal denso As Boolean) As String()
        Dim Arr As New ArrayList

        Dim Apps() As String = GetPrivateProfileSectionNames(Path.Combine(Application.StartupPath, "FURIWAKE.INI"))

        For i As Integer = 0 To Apps.Length - 1
            If denso = True Then
                If CASTCommon.GetIni("FURIWAKE.INI", Apps(i), "Baitai") = "00" Then
                    '*** �C�� mitsu 2009/05/29 SQL�p�u'�v�Ŋ��� ***
                    'Arr.Add(Apps(i))
                    Arr.Add(SQ(Apps(i)))
                    '**********************************************
                End If
            Else
                Dim sRet As String
                sRet = CASTCommon.GetIni("FURIWAKE.INI", Apps(i), "Baitai")
                If sRet <> "00" AndAlso sRet <> "err" Then
                    '*** �C�� mitsu 2009/05/29 SQL�p�u'�v�Ŋ��� ***
                    'Arr.Add(Apps(i))
                    Arr.Add(SQ(Apps(i)))
                    '**********************************************
                End If
            End If
        Next i

        Return CType(Arr.ToArray(GetType(String)), String())
    End Function

    Private Sub btnAllOn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True '((CType(sender, Button).Name.ToUpper = "BTNALLON")) OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�I��", "����", "")

        End Try
    End Sub

    Private Sub btnAllOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False 'OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�I��", "����", "")

        End Try
    End Sub

    '���l�]�����邪ZERO���߂��Ȃ�
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtCycle.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox))
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("���l�]���ҏW1", "���s", ex.ToString)
        End Try
    End Sub

End Class
