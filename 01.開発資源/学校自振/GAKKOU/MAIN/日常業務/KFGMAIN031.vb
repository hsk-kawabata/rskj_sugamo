Option Explicit On
Option Strict Off

Imports System.IO
Imports System.Text
Imports System.Drawing.Printing
Imports CASTCommon

Public Class KFGMAIN031

    Private Int_Spd_Count As Integer
    Private Str_ZenginFile As String
    Private Str_Syori_Date(1) As String
    Private Str_Baitai_Code As String
    Private Lng_Ijyo_Count As Long
    Private Lng_Err_Count As Long
    Private Lng_Trw_Count As Long
    Private Str_Sch_Flg(1) As String
    Private Lng_RecordNo As Long
    Private Bln_ErrEnd_Flg As Boolean
    Private Bln_ErrEndFinal_Flg As Boolean

    '�G���[���X�g�쐬����n�p�p�����[�^
    Private Structure ErrListInfo
        Dim Int_Err_Gakunen_Code As Integer
        Dim Int_Err_Class_Code As Integer
        Dim Str_Err_Seito_No As String
        Dim Int_Err_Tuuban As Integer
        Dim Str_Err_Itaku_Name As String
        Dim Str_Err_Tkin_No As String
        Dim Str_Err_Tsit_No As String
        Dim Str_Err_Kamoku As String
        Dim Str_Err_Kouza As String
        Dim Str_Err_Keiyaku_No As String
        Dim Str_Err_Keiyaku_Name As String
        Dim Lng_Err_Furikae_Kingaku As Long
        Dim Str_Err_Msg As String
    End Structure
    Dim intLIST_COUNT As Integer
    Private ErrList As ErrListInfo

    '2006/10/11
    Private lngJIKOU_SAKI As Long = 0
    Private blnDATA_CREATE_FLG As Boolean = False

    Private blnGAK_FLG As Boolean = False
    Private MainDB As CASTCommon.MyOracle
    Private Const msgTitle As String = "�����U�փf�[�^�쐬���(KFGMAIN031)"
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN031", "�����U�փf�[�^�쐬���")
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    ' 2017/02/21 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/02/21 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

    Private Sub KFGMAIN031_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '#####################################
        '���O�̏����ɕK�v�ȏ��̎擾
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Dim FuriDate As String = ""
            FuriDate = Me.lblFuriDateY.Text
            FuriDate = Me.lblFuriDateM.Text
            FuriDate = Me.lblFuriDateD.Text

            MainDB = New CASTCommon.MyOracle

            ' 2017/02/21 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/02/21 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

            '�U�����ɑΉ������X�P�W���[�����ꗗ�̐ݒ�
            If fn_SetDataGridView(FuriDate) = False Then
                Me.Close()
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    Private Sub btnChkAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChkAll.Click
        Try
            With Me.DataGridView
                For Cnt As Integer = 0 To .Rows.Count - 1
                    If .Rows(Cnt).Cells(7).Value.ToString.Trim = "��" Then
                    Else
                        .Rows(Cnt).Cells(0).Value = True
                    End If
                Next
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)", "���s", ex.ToString)
        End Try
    End Sub
    Private Sub btnClrChkAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClrChkAll.Click
        Try
            With Me.DataGridView
                For Cnt As Integer = 0 To .Rows.Count - 1
                    If .Rows(Cnt).Cells(7).Value.ToString.Trim = "��" Then
                    Else
                        .Rows(Cnt).Cells(0).Value = False
                    End If
                Next
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)", "���s", ex.ToString)
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreate.Click

        Str_Syori_Date(0) = Format(Now, "yyyyMMdd")
        Str_Syori_Date(1) = Format(Now, "yyyyMMddHHmmss")

        Dim FuriDate As String = ""
        FuriDate = Me.lblFuriDateY.Text
        FuriDate = Me.lblFuriDateM.Text
        FuriDate = Me.lblFuriDateD.Text

        STR_COMMAND = "�쐬"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = STR_FURIKAE_DATE(1)

        Try
            Bln_ErrEnd_Flg = False
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)�J�n", "����", "")
            '���̓`�F�b�N
            Dim SelectCheck As Boolean = False
            With Me.DataGridView
                For Cnt As Integer = 0 To .Rows.Count - 1
                    If CBool(.Rows(Cnt).Cells(0).Value) = True Then
                        SelectCheck = True
                        Exit For
                    End If
                Next
            End With

            If SelectCheck = False Then
                MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Try
            End If

            '�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0015I, "�����U�փf�[�^�쐬"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Exit Try
            End If

            lngJIKOU_SAKI = 0

            '2011/06/16 �W���ŏC�� �������J�[�\���ύX------------------START
            Cursor.Current = Cursors.WaitCursor()
            '2011/06/16 �W���ŏC�� �������J�[�\���ύX------------------END
            '�����U�փf�[�^�쐬
            If Not fn_CreateFurikaeData() Then
                Exit Sub
            End If

            '��ʍĕ`��
            If fn_SetDataGridView(FuriDate) = False Then
                Exit Sub
            End If

            If Bln_ErrEndFinal_Flg = False Then
                If Lng_Ijyo_Count = 0 Then
                    If lngJIKOU_SAKI = 0 Then
                        '�������b�Z�[�W
                        MessageBox.Show(G_MSG0005I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        '�������b�Z�[�W
                        MessageBox.Show(String.Format(MSG0016I, "�����U�փf�[�^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                    '2011/06/16 �W���ŏC�� �C���v�b�g�G���[����̏ꍇ���b�Z�[�W�o��------------------START
                Else
                    MessageBox.Show(G_MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    '2011/06/16 �W���ŏC�� �C���v�b�g�G���[����̏ꍇ���b�Z�[�W�o��------------------END
                End If
            Else
                Call MessageBox.Show(String.Format(MSG0034E, "�����U�փf�[�^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            btnEnd.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)�I��", "����", "")
        End Try

    End Sub
    Private Function fn_SetDataGridView(ByVal FuriDate As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As MyOracleReader = Nothing

        Try

            Dim SQL As New StringBuilder(128)
            '�X�P�W���[���}�X�^���
            '�U�֓����U�֓��i��ʕ\���j
            '�U�֓��o�敪���Qor�R�̏ꍇ
            '(���׃t���O���P)
            '�`�F�b�N�t���O���P
            '���f�t���O���O
            '�ȏ�̏����Œ��o
            SQL.Append(" SELECT ")
            SQL.Append(" G_SCHMAST.* ")
            SQL.Append(",GAKKOU_NNAME_G ")
            SQL.Append(",TKIN_NO_T ")
            SQL.Append(",TSIT_NO_T ")
            SQL.Append(",KAMOKU_T ")
            SQL.Append(",KOUZA_T ")
            SQL.Append(",SFURI_SYUBETU_T ")
            SQL.Append(",MEISAI_OUT_T ")
            SQL.Append(",FILE_NAME_T ")
            SQL.Append(",MEISAI_KBN_T ")
            SQL.Append(",BAITAI_CODE_T ")
            SQL.Append(" FROM G_SCHMAST ")
            SQL.Append(" LEFT JOIN GAKMAST1 ON ")
            SQL.Append(" GAKKOU_CODE_S = GAKKOU_CODE_G ")
            SQL.Append(" LEFT JOIN GAKMAST2 ON ")
            SQL.Append(" GAKKOU_CODE_S = GAKKOU_CODE_T ")
            SQL.Append(" WHERE GAKUNEN_CODE_G =1 ")
            SQL.Append(" AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
            If STR_KINGAKU_CHK <> "0" Then
                SQL.Append(" AND CHECK_FLG_S ='1' ")
            End If
            'SQL.Append(" AND CHECK_FLG_S ='1' ")
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END
            SQL.Append(" AND TYUUDAN_FLG_S ='0' ")
            '���ޭ�ً敪2(����)�͏����Ɋ܂܂Ȃ���
            SQL.Append(" AND SCH_KBN_S <> '2' ")
            SQL.Append(" ORDER BY GAKKOU_CODE_S ")

            OraReader = New MyOracleReader(MainDB)

            '���z�m�F�ς݂̃f�[�^�����݂��Ȃ��ꍇ�͎����(GFJMAST0301G)���I������
            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Try
            Else
                Dim TMP_GakkoCode As String = ""
                '�s�J�E���g
                Dim RowsCnt As Long = 0

                '�X�P�W���[������������w�Z�͒P��̍s�Ƃ��ĕ\������
                While OraReader.EOF = False
                    If TMP_GakkoCode.Trim <> OraReader.GetItem("GAKKOU_CODE_S").Trim Then
                        If (OraReader.GetItem("FURI_KBN_S") = "2" OrElse OraReader.GetItem("FURI_KBN_S") = "3") _
                        AndAlso OraReader.GetItem("ENTRI_FLG_S") <> "1" Then
                            '(FURI_KBN = 2 OR 3) AND ENTRI_FLG_S = 1
                            '�ł���Ή������Ȃ�
                        Else
                            With DataGridView
                                '�ǉ��s
                                Dim RowItem As New DataGridViewRow
                                RowItem.CreateCells(DataGridView)

                                RowsCnt += 1

                                RowItem.Cells(0).Value = False '�`�F�b�N�{�b�N�X
                                RowItem.Cells(1).Value = OraReader.GetItem("GAKKOU_NNAME_G") '�w�Z��
                                RowItem.Cells(2).Value = OraReader.GetItem("GAKKOU_CODE_S") '�w�Z�R�[�h
                                RowItem.Cells(3).Value = Mid(Trim(OraReader.GetItem("FURI_DATE_S")), 7, 2) '�U�֓��i���j
                                RowItem.Cells(4).Value = OraReader.GetItem("NENGETUDO_S").Trim '�����N��
                                RowItem.Cells(5).Value = GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_BAITAI_TXT, OraReader.GetItem("BAITAI_CODE_S").Trim) '�}��
                                RowItem.Cells(6).Value = GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_NYUSYUTU_TXT, OraReader.GetItem("FURI_KBN_S").Trim) '���o��

                                '������
                                Select Case CInt(OraReader.GetItem("DATA_FLG_S"))
                                    Case 0
                                        '��
                                        RowItem.Cells(7).Value = ""

                                        '�I����
                                        RowItem.ReadOnly = True
                                        RowItem.Cells(0).ReadOnly = False

                                        For cnt As Integer = 0 To RowItem.Cells.Count - 1 Step 1
                                            RowItem.Cells(cnt).Style.BackColor = System.Drawing.Color.White
                                        Next
                                    Case 1
                                        '��
                                        RowItem.Cells(7).Value = "��"

                                        '�I��s��
                                        RowItem.ReadOnly = True
                                        RowItem.Cells(0).ReadOnly = True
                                        '�w�i�����F�ɂ���
                                        For cnt As Integer = 0 To RowItem.Cells.Count - 1 Step 1
                                            RowItem.Cells(cnt).Style.BackColor = System.Drawing.Color.Yellow
                                        Next
                                End Select

                                '�U�֓��o�敪
                                RowItem.Cells(8).Value = CInt(OraReader.GetItem("FURI_KBN_S"))

                                Select Case CInt(OraReader.GetItem("FURI_KBN_S"))
                                    Case 0, 3
                                        RowItem.Cells(9).Value = "D" & OraReader.GetItem("GAKKOU_CODE_S") & "01.DAT"
                                    Case 1
                                        RowItem.Cells(9).Value = "D" & OraReader.GetItem("GAKKOU_CODE_S") & "02.DAT"
                                    Case 2
                                        RowItem.Cells(9).Value = "D" & OraReader.GetItem("GAKKOU_CODE_S") & "03.DAT"
                                End Select

                                '�\�[�g��
                                RowItem.Cells(10).Value = CInt(OraReader.GetItem("MEISAI_OUT_T"))

                                '�ۑ���̧�ٖ���(�w�Z�}�X�^�ɓo�^����Ă�����e)
                                '�����t�@�C�������o�^����Ă��Ȃ��ꍇ��S�{�w�Z�R�[�h�{.dat�Ƃ���
                                If IsDBNull(OraReader.GetItem("FILE_NAME_T")) = True Then
                                    RowItem.Cells(11).Value = "S" & OraReader.GetItem("GAKKOU_CODE_S") & ".DAT"
                                Else
                                    If CStr(OraReader.GetItem("FILE_NAME_T")).Trim = "" Then
                                        RowItem.Cells(11).Value = "S" & OraReader.GetItem("GAKKOU_CODE_S") & ".DAT"
                                    Else
                                        RowItem.Cells(11).Value = OraReader.GetItem("FILE_NAME_T")
                                    End If
                                End If

                                '�U�֗\�薾�ו\����敪
                                If IsDBNull(OraReader.GetItem("MEISAI_KBN_T")) = True Then
                                    RowItem.Cells(12).Value = 0
                                Else
                                    RowItem.Cells(12).Value = OraReader.GetItem("MEISAI_KBN_T")
                                End If

                                '�s�ǉ�
                                .Rows.Add(RowItem)
                            End With
                        End If

                        '2017/03/14 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        TMP_GakkoCode = OraReader.GetItem("GAKKOU_CODE_S")
                        '2017/03/14 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ END

                    End If

                    OraReader.NextRead()
                End While
            End If

            OraReader.Close()
            OraReader = Nothing

            ret = True

        Catch ex As Exception
            Call MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(��ʏ�����)", "���s", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
    Private Function fn_GetKinkoName(ByVal KinCode As String, ByVal TenCode As String) As String()

        Dim KinInfo As String() = {"", "", "", ""}

        Dim Orareader As MyOracleReader = Nothing

        Try
            If KinCode.Trim = "" OrElse TenCode.Trim = "" Then
                Return Nothing
            End If

            Dim SQL As New StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" KIN_KNAME_N ")
            SQL.Append(",KIN_NNAME_N ")
            SQL.Append(",SIT_KNAME_N ")
            SQL.Append(",SIT_NNAME_N ")
            SQL.Append(" FROM TENMAST ")
            SQL.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
            SQL.Append(" AND SIT_NO_N = '" & TenCode & "'")

            Orareader = New MyOracleReader(MainDB)

            If Orareader.DataReader(SQL) Then
                KinInfo(0) = Orareader.GetItem("KIN_KNAME_N")
                KinInfo(1) = Orareader.GetItem("KIN_NNAME_N")
                KinInfo(2) = Orareader.GetItem("SIT_KNAME_N")
                KinInfo(3) = Orareader.GetItem("SIT_NNAME_N")
            Else
                Return KinInfo
            End If

            Orareader.Close()
            Orareader = Nothing

        Catch ex As Exception
            Return KinInfo
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
        End Try

        Return KinInfo

    End Function
    Private Function fn_CreateFurikaeData() As Boolean

        Dim ret As Boolean = False

        Dim JobID As String = ""
        Dim Param As String = ""


        Try
            Dim strPARA() As String
            Dim strGAKKOU_P() As String
            Dim strFURI_DATE_P() As String
            Dim lngPARA_CNT As Long = 0

            ReDim strPARA(0)
            ReDim strGAKKOU_P(0)
            ReDim strFURI_DATE_P(0)

            Lng_Ijyo_Count = 0

            Bln_ErrEndFinal_Flg = False

            '�G���[���X�g�폜
            '�g�����U�N�V�����J�n
            MainDB.BeginTrans()


            '�����J�n�O�Ɉ�U����������B
            If MainDB.ExecuteNonQuery(" DELETE FROM G_IJYOLIST ") = -1 Then
                MainDB.Rollback()
                Call MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            Else
                MainDB.Commit()
            End If

            '�����U�փf�[�^���P�����I������Ă��Ȃ��ꍇ�G���[
            With Me.DataGridView
                For iCount As Integer = 0 To (.Rows.Count - 1)
                    '2005/03/15
                    intLIST_COUNT = iCount
                    '�`�F�b�N���L���ɂȂ��Ă���w�Z�̐U�փf�[�^���쐬
                    If CBool(.Rows(iCount).Cells(0).Value) = True Then
                        Str_Baitai_Code = GFUNC_NAME_TO_CODE2(STR_TXT_PATH & STR_BAITAI_TXT, .Rows(iCount).Cells(5).Value.ToString)
                        LW.ToriCode = .Rows(iCount).Cells(2).Value.ToString
                        LW.FuriDate = STR_FURIKAE_DATE(1)
                        Select Case CInt(.Rows(iCount).Cells(8).Value)
                            Case 0
                                '���U
                                Call Sub_Insert_Meisai1(.Rows(iCount).Cells(2).Value.ToString, _
                                                         .Rows(iCount).Cells(4).Value.ToString, _
                                                         .Rows(iCount).Cells(9).Value.ToString, _
                                                         .Rows(iCount).Cells(10).Value.ToString, _
                                                         .Rows(iCount).Cells(11).Value.ToString)
                            Case 1
                                '�ĐU
                                Call Sub_Insert_Meisai2(.Rows(iCount).Cells(2).Value.ToString, _
                                                         .Rows(iCount).Cells(4).Value.ToString, _
                                                         .Rows(iCount).Cells(9).Value.ToString, _
                                                         .Rows(iCount).Cells(10).Value.ToString, _
                                                         .Rows(iCount).Cells(11).Value.ToString)
                        End Select

                        Lng_Ijyo_Count += Lng_Err_Count

                        '�����P�ʂŃG���[���������Ȃ��ꍇ������������s
                        If (Lng_Err_Count = 0 OrElse Lng_Ijyo_Count = Lng_Trw_Count) And blnDATA_CREATE_FLG = True Then
                            '��ɂ��̃t���O�͑��s���쐬�����ɐ����f�[�^���쐬���悤�Ƃ������Ƀt���O��ON�ɂȂ�
                            If Bln_ErrEnd_Flg = False Then
                                '����t���O���쐬�Ώۂ̊w�Z�̂�
                                Select Case CInt(.Rows(iCount).Cells(12).Value)
                                    Case 1, 2
                                        '�����U�֗\��W�v�\

                                        Dim nRet As Integer
                                        Dim Param2 As String
                                        '����o�b�`�Ăяo��
                                        Dim ExeRepo As New CAstReports.ClsExecute
                                        ExeRepo.SetOwner = Me

                                        '�p�����[�^�ݒ�F���O�C�����A�w�Z�R�[�h�A������
                                        Param2 = GCom.GetUserID & "," & Trim(.Rows(iCount).Cells(2).Value) & "," & Str_Syori_Date(1)

                                        nRet = ExeRepo.ExecReport("KFGP005.EXE", Param2)

                                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                                        Select Case nRet
                                            Case 0
                                            Case Else
                                                '������s���b�Z�[�W
                                                MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��W�v�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        End Select


                                        '�����U�֗\��ꗗ�\
                                        ExeRepo = New CAstReports.ClsExecute
                                        ExeRepo.SetOwner = Me

                                        '�p�����[�^�ݒ�F���O�C�����A�w�Z�R�[�h�A�������A�U�֓��A���[����敪(1:�X�ԃ\�[�g�Ȃ� 2:�X�ԃ\�[�g����)�A���[�\�[�g��
                                        Param2 = GCom.GetUserID & "," & Trim(.Rows(iCount).Cells(2).Value) & "," _
                                                 & Str_Syori_Date(1) & "," & STR_FURIKAE_DATE(1) & "," & GCom.NzInt(.Rows(iCount).Cells(12).Value) & "," _
                                                 & Trim(.Rows(iCount).Cells(10).Value)

                                        nRet = ExeRepo.ExecReport("KFGP003.EXE", Param2)

                                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                                        Select Case nRet
                                            Case 0
                                            Case Else
                                                '������s���b�Z�[�W
                                                MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        End Select



                                End Select
                            Else
                                Bln_ErrEndFinal_Flg = True
                            End If


                            If Bln_ErrEnd_Flg = False Then

                                '------------------------------------------------
                                '�W���u�}�X�^�ɓo�^
                                '------------------------------------------------
                                JobID = "J010" '�o�^

                                Dim ToriCode As String = ""
                                Select Case CInt(.Rows(iCount).Cells(8).Value)
                                    Case 0
                                        ToriCode = Trim(.Rows(iCount).Cells(2).Value) & "01"
                                    Case 1
                                        ToriCode = Trim(.Rows(iCount).Cells(2).Value) & "02"
                                End Select
                                Param = ToriCode
                                'FURI_DATE
                                Param &= "," & STR_FURIKAE_DATE(1)
                                'CODE_KBN
                                Param &= "," & "0"
                                'FMT_KBN
                                Param &= "," & "00"
                                'BAITAI_CODE 
                                Param &= "," & "07"
                                'LABEL_KBN 
                                Param &= "," & "0"

                                ' �W���u�p�����[�^��z��ɕۑ� 2005/11/02
                                ReDim Preserve strPARA(lngPARA_CNT)
                                ReDim Preserve strGAKKOU_P(lngPARA_CNT)
                                ReDim Preserve strFURI_DATE_P(lngPARA_CNT)

                                strPARA(lngPARA_CNT) = Param
                                strGAKKOU_P(lngPARA_CNT) = Trim(.Rows(iCount).Cells(2).Value)
                                strFURI_DATE_P(lngPARA_CNT) = STR_FURIKAE_DATE(1)
                                lngPARA_CNT = lngPARA_CNT + 1
                            End If

                        End If
                    End If

                    blnDATA_CREATE_FLG = False
                Next iCount
            End With
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            If Bln_ErrEnd_Flg = False Then '��Ǝ��U�A�g

                '�W���u�p�����[�^����C�ɓo�^
                Dim iCNT As Integer = 0
                If strPARA(0) <> Nothing OrElse strGAKKOU_P(0) <> Nothing OrElse strFURI_DATE_P(0) <> Nothing Then  '�p�����[�^������Ƃ��̂ݓo�^
                    For iCNT = LBound(strPARA) To UBound(strPARA)
                        If fn_JOBMAST_TOUROKU_CHECK(JobID, GCom.GetUserID, strPARA(iCNT)) = False Then
                        Else
                            If fn_INSERT_JOBMAST(JobID, GCom.GetUserID, strPARA(iCNT)) = False Then
                                MainLOG.Write(LW.UserID, strGAKKOU_P(iCNT), strFURI_DATE_P(iCNT), "(�p�����[�^�o�^)", "���s", Err.Description)
                            Else
                                MainLOG.Write(LW.UserID, strGAKKOU_P(iCNT), strFURI_DATE_P(iCNT), "(�p�����[�^�o�^)", "����", "")
                            End If
                        End If
                    Next
                End If
            End If

            '�װؽĈ��
            '�������ɃG���[���X�g�������P���ȏ㔭�������ꍇ���s
            If Lng_Ijyo_Count <> 0 Then
                '�C���v�b�g�G���[���X�g
                Dim nRet As Integer
                Dim Param2 As String
                '����o�b�`�Ăяo��
                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me

                '�p�����[�^�ݒ�F���O�C����
                Param2 = GCom.GetUserID

                nRet = ExeRepo.ExecReport("KFGP001.EXE", Param2)

                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "�C���v�b�g�G���[���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End If

            '�s��S�č폜
            For Cnt As Integer = 0 To DataGridView.RowCount - 1
                Me.DataGridView.Rows.Remove(DataGridView.Rows(0))
            Next

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�փf�[�^�쐬)", "���s", ex.ToString)
        End Try

        Return ret

    End Function
    Private Sub Sub_Insert_Meisai1(ByVal pGakkou_Code As String, _
                                    ByVal pSeikyuNentuki As String, _
                                    ByVal pZengin_FileName As String, _
                                    ByVal pSort_Kbn As String, _
                                    ByVal pSave_FileName As String)

        Dim SQL As New StringBuilder(128)

        Dim lTotal_Kensuu As Long
        Dim lTotal_Kingaku As Long
        Dim lThrrowCount As Long
        Dim sGakkou_Name As String = ""
        Dim sFile_Name As String = ""
        Dim iSaifuri_Syubetu As Integer

        STR_LOG_GAKKOU_CODE = pGakkou_Code

        Dim sw As StreamWriter = Nothing

        Dim ret As Boolean = False

        Try
            'BeginTrans
            MainDB.BeginTrans()

            SQL.Length = 0
            SQL.Append(" DELETE  FROM G_MEIMAST")
            SQL.Append(" WHERE GAKKOU_CODE_M = '" & pGakkou_Code & "'")
            SQL.Append(" AND FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'")
            SQL.Append(" AND FURI_KBN_M = '" & 0 & "'")
            'SQL.append( " AND TKIN_NO_M = '" & Str_Jikou_Ginko_Code & "'"'���S�M������ ���s���܂� 2007/09/04

            If MainDB.ExecuteNonQuery(SQL) = -1 Then
                Exit Try
            End If

            '�Ȃ��ꍇ�̓f�B���N�g���쐬
            If Not Directory.Exists(STR_DAT_PATH) Then
                MkDir(STR_DAT_PATH)
            End If

            '�S��t�@�C���쐬
            Str_ZenginFile = STR_DAT_PATH & pZengin_FileName

            sw = New StreamWriter(Str_ZenginFile, False, System.Text.Encoding.GetEncoding(932), 120)

            '**********************************************
            '�w�b�_�[���R�[�h�쐬
            '**********************************************
            If Not MakeH_RecordSyofuri(pGakkou_Code, pSeikyuNentuki, sGakkou_Name, iSaifuri_Syubetu, sFile_Name, sw) Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�b�_���R�[�h�쐬)", "���s", Err.Description)
                Exit Sub
            End If

            '**********************************************
            '�f�[�^���R�[�h�쐬
            '**********************************************
            Dim iret As Integer = MakeD_RecordSyofuri(pGakkou_Code, pSeikyuNentuki, iSaifuri_Syubetu, lTotal_Kensuu, lTotal_Kingaku, lThrrowCount, sw)

            Select Case iret
                Case 0
                    '����

                    '**********************************************
                    '�g���[�����R�[�h�쐬
                    '**********************************************
                    If Not MakeT_Record(lTotal_Kensuu, lTotal_Kingaku, sw) Then
                        Exit Sub
                    End If

                    '**********************************************
                    '�G���h���R�[�h�쐬
                    '**********************************************
                    If Not MakeE_Record(sw) Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�G���h���R�[�h�쐬)", "���s", Err.Description)
                        Exit Sub
                    End If

                    '�P���ł��ُ탊�X�g�ɒǉ�������w�Z�̓X�P�W���[���̍X�V�Ȍ�̏����͍s��Ȃ�
                    '���ēx�A���������s�ł���悤�ɂ����
                    '��O�Ƃ��āA���Z�@�ւɑ��݂��Ȃ����Z�@�ւ̓o�^���̓G���[���X�P�W���[���̍X�V�������s��
                    If Lng_Err_Count = 0 OrElse Lng_Err_Count = lThrrowCount Then

                        sw.Close()
                        sw = Nothing

                        '2010.02.04 �s�v
                        'Select Case GFUNC_FD_Copy3(Me, sGakkou_Name, Str_ZenginFile, pSave_FileName, Str_Baitai_Code)
                        '    Case 0
                        '        blnDATA_CREATE_FLG = True
                        '        '����I��
                        '        Call GSUB_LOG(1, "�����U�փf�[�^�쐬")
                        '    Case 1
                        '        lngJIKOU_SAKI -= 1
                        '        blnDATA_CREATE_FLG = False
                        '        '�L�����Z��
                        '        Exit Sub
                        '    Case Else
                        '        lngJIKOU_SAKI -= 1
                        '        blnDATA_CREATE_FLG = False
                        '        '�G���[
                        '        Call GSUB_LOG(0, "�S��̧��FD�ۑ��G���[")
                        '        Exit Sub
                        'End Select

                        blnDATA_CREATE_FLG = True
                        '����I��
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U�փf�[�^�쐬)", "����", "")

                        '�����U�փf�[�^�쐬�t���O�X�V
                        '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
                        If STR_KINGAKU_CHK <> "0" Then
                            '���тƖ��ׂ̏W�v�`�F�b�N
                            If CompareJissekiMastGMeiMastSyofuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then
                                Bln_ErrEnd_Flg = True
                                Exit Sub
                            End If
                        End If
                        'If CompareJissekiMastGMeiMastSyofuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then
                        '    Bln_ErrEnd_Flg = True
                        '    Exit Sub
                        'End If
                        '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END

                        '�X�P�W���[���X�V
                        If fn_Update_Schedule(pGakkou_Code, pSeikyuNentuki, "0") = False Then
                            Exit Sub
                        End If
                    End If
                Case 1
                    'GoTo JISSEKI_UPDATE
                    '�����U�փf�[�^�쐬�t���O�X�V
                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
                    If STR_KINGAKU_CHK <> "0" Then
                        '���тƖ��ׂ̏W�v�`�F�b�N
                        If CompareJissekiMastGMeiMastSyofuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then
                            Bln_ErrEnd_Flg = True
                            Exit Sub
                        End If
                    End If
                    'If CompareJissekiMastGMeiMastSyofuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then
                    '    Bln_ErrEnd_Flg = True
                    '    Exit Sub
                    'End If
                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END

                    '�X�P�W���[���X�V
                    If fn_Update_Schedule(pGakkou_Code, pSeikyuNentuki, "0") = False Then
                        Exit Sub
                    End If
                Case Else
                    '�ُ�
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^���R�[�h�쐬���s)", "���s", Err.Description)
                    Exit Sub
            End Select

            If Not sw Is Nothing Then
                sw.Close()
                sw = Nothing
            End If

            Lng_Trw_Count += lThrrowCount

            ret = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���ׂP�쐬)", "���s", ex.ToString)
        Finally
            If Not sw Is Nothing Then
                sw.Close()
                sw = Nothing
            End If
            If ret = True Then
                MainDB.Commit()
            Else
                MainDB.Rollback()
            End If
        End Try

    End Sub
    Private Sub Sub_Insert_Meisai2(ByVal pGakkou_Code As String, _
                                    ByVal pSeikyuNentuki As String, _
                                    ByVal pZengin_FileName As String, _
                                    ByVal pSort_Kbn As String, _
                                    ByVal pSave_FileName As String)

        Dim lTotal_Kensuu As Long
        Dim lTotal_Kingaku As Long
        Dim lThrrowCount As Long
        Dim sGakkou_Name As String = ""
        Dim sFile_Name As String = ""
        Dim iSaifuri_Syubetu As Integer

        STR_LOG_GAKKOU_CODE = pGakkou_Code

        Dim sw As StreamWriter = Nothing

        '�ŏ��ɏ����Ώۂ����肷��
        Try
            Dim SQL As New StringBuilder(128)
            SQL.Append("SELECT SFURI_SYUBETU_T")
            SQL.Append(" FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T = '" & pGakkou_Code & "'")

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

            If Not OraReader.DataReader(SQL) Then
            Else
                '------------------------------------------
                '�ĐU�w��m�F
                '   �ĐU���(�ĐU�L��) = 1 OR 2 �����Ώ�
                '   �ĐU���(�ĐU����) = 0 OR 3 �������s��Ȃ�
                '-------------------------------------------
                Select Case OraReader.GetItem("SFURI_SYUBETU_T").ToString.Trim
                    Case "0", "3"
                        OraReader.Close()
                        OraReader = Nothing
                        Exit Sub
                    Case Else
                        '���s
                End Select
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ĐU�敪�擾)", "���s", ex.ToString)
            Exit Sub
        End Try

        Dim ret As Boolean = False

        Try
            'BeginTrans
            MainDB.BeginTrans()

            '�Ȃ��ꍇ�̓f�B���N�g���쐬
            If Not Directory.Exists(STR_DAT_PATH) Then
                MkDir(STR_DAT_PATH)
            End If

            '�S��t�@�C���쐬
            Str_ZenginFile = STR_DAT_PATH & pZengin_FileName

            sw = New StreamWriter(Str_ZenginFile, False, System.Text.Encoding.GetEncoding(932), 120)

            '**********************************************
            '�w�b�_�[���R�[�h�쐬
            '**********************************************
            If Not MakeH_RecordSaifuri(pGakkou_Code, pSeikyuNentuki, sGakkou_Name, iSaifuri_Syubetu, sFile_Name, sw) Then
                Exit Sub
            End If

            '**********************************************
            '�f�[�^���R�[�h�쐬
            '**********************************************
            Dim iret As Integer = MakeD_RecordSaifuri(pGakkou_Code, pSeikyuNentuki, iSaifuri_Syubetu, lTotal_Kensuu, lTotal_Kingaku, lThrrowCount, sw)

            Select Case iret
                Case 0
                    '**********************************************
                    '�g���[�����R�[�h�쐬
                    '**********************************************
                    If Not MakeT_Record(lTotal_Kensuu, lTotal_Kingaku, sw) Then
                        Exit Sub
                    End If

                    '**********************************************
                    '�G���h���R�[�h�쐬
                    '**********************************************
                    If Not MakeE_Record(sw) Then
                        Exit Sub
                    End If

                    '��O�Ƃ��āA���Z�@�ւɑ��݂��Ȃ����Z�@�ւ̓o�^���̓G���[���X�P�W���[���̍X�V�������s��
                    If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then

                        '��Ǝ��U�ƘA�g�� 2005/03/14�m�F���b�Z�[�W��\�����Ȃ�
                        Select Case GFUNC_FD_Copy3(Me, sGakkou_Name, Str_ZenginFile, pSave_FileName, Str_Baitai_Code)
                            Case 0
                                blnDATA_CREATE_FLG = True
                                '����I��
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U�փf�[�^�쐬)", "����", "")
                            Case 1
                                lngJIKOU_SAKI -= 1
                                blnDATA_CREATE_FLG = False
                                '�L�����Z��
                                Exit Sub
                            Case Else
                                lngJIKOU_SAKI -= 1
                                blnDATA_CREATE_FLG = False
                                '�G���[
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S��̧��FD�ۑ�)", "���s", Err.Description)
                                Exit Sub
                        End Select

                        '�����U�փf�[�^�쐬�t���O�X�V
                        '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
                        If STR_KINGAKU_CHK <> "0" Then
                            '���тƖ��ׂ̏W�v�`�F�b�N
                            If CompareJissekiMastGMeiMastSaifuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then
                                Bln_ErrEnd_Flg = True
                                Exit Sub
                            End If
                        End If
                        'If CompareJissekiMastGMeiMastSaifuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then
                        '    Bln_ErrEnd_Flg = True
                        '    Exit Sub
                        'End If
                        '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END

                        '�X�P�W���[���X�V
                        If fn_Update_Schedule(pGakkou_Code, pSeikyuNentuki, "1") = False Then
                            Exit Sub
                        End If

                    End If
                Case 1
                    '�����U�փf�[�^�쐬�t���O�X�V
                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
                    If STR_KINGAKU_CHK <> "0" Then
                        '���тƖ��ׂ̏W�v�`�F�b�N
                        If CompareJissekiMastGMeiMastSaifuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then

                            Bln_ErrEnd_Flg = True

                            Exit Sub
                        End If
                    End If
                    'If CompareJissekiMastGMeiMastSaifuri(pGakkou_Code, pSeikyuNentuki, STR_FURIKAE_DATE(1)) = False Then

                    '    Bln_ErrEnd_Flg = True

                    '    Exit Sub
                    'End If
                    '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END

                    '�X�P�W���[���X�V
                    If fn_Update_Schedule(pGakkou_Code, pSeikyuNentuki, "1") = False Then
                        Exit Sub
                    End If
                Case Else

            End Select

            Lng_Trw_Count += lThrrowCount

            ret = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���ׂQ�쐬)", "���s", ex.ToString)
        Finally
            If Not sw Is Nothing Then
                sw.Close()
                sw = Nothing
            End If
            If ret = True Then
                MainDB.Commit()
            Else
                MainDB.Rollback()
            End If
        End Try
    End Sub
    Private Function MakeInsertIjyoListSQL(ByVal pGakkou_Code As String) As String

        Dim SQL As New StringBuilder(128)
        With ErrList
            SQL.Length = 0
            SQL.Append(" INSERT INTO G_IJYOLIST")
            SQL.Append(" values(")
            SQL.Append("'" & Str_Syori_Date(0) & "'")
            SQL.Append("," & .Int_Err_Gakunen_Code)
            SQL.Append("," & .Int_Err_Class_Code)
            SQL.Append(",'" & .Str_Err_Seito_No & "'")
            SQL.Append("," & .Int_Err_Tuuban)
            SQL.Append("," & Lng_Err_Count)
            SQL.Append(",'" & pGakkou_Code & "'")
            SQL.Append(",'" & STR_FURIKAE_DATE(1) & "'")
            SQL.Append(",'" & .Str_Err_Itaku_Name & "'")
            SQL.Append("," & Lng_RecordNo)
            SQL.Append(",'" & .Str_Err_Tkin_No & "'")
            SQL.Append(",'" & .Str_Err_Tsit_No & "'")
            SQL.Append(",'" & .Str_Err_Kamoku & "'")
            SQL.Append(",'" & .Str_Err_Kouza & "'")
            SQL.Append(",'" & .Str_Err_Keiyaku_No & "'")
            SQL.Append(",'" & .Str_Err_Keiyaku_Name.PadRight(40).Substring(0, 30) & "'")
            SQL.Append("," & .Lng_Err_Furikae_Kingaku)
            SQL.Append(",'" & .Str_Err_Msg & "'")
            SQL.Append(",'" & Format(Now, "yyyyMMddHHmmss") & "'") '�^�C���X�^���v 2006/11/21
            SQL.Append(",'" & Space(50) & "'")
            SQL.Append(",'" & Space(50) & "'")
            SQL.Append(" )")
        End With

        Return SQL.ToString

    End Function
    Private Function fn_Update_Schedule(ByVal pGakkou_Code As String, ByVal pNengetu As String, ByVal pFuri_Kbn As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim OraReader2 As CASTCommon.MyOracleReader = Nothing

        Try
            Dim iG_Flg(9) As Integer
            Dim bG_Flg As Boolean

            '�X�P�W���[���`�F�b�N
            '�N�ԂƓ��ʂŐU�֓�������̃X�P�W���[�������݂����
            '�܂��͑��݂���X�P�W���[���̃v���C�}���L�[����S�擾����
            '�X�P�W���[���敪�ƍĐU���i���U�֓��͓���ׁ̈A���̂Q���ڂŏW�v���邱�ƂŃX�P�W���[���P�ʂ̏W�v���擾�\�j
            Dim SQL As New StringBuilder(128)
            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append(" SCH_KBN_S")
            SQL.Append(",SFURI_DATE_S")
            SQL.Append(" FROM SEITOMASTVIEW , G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_O = GAKKOU_CODE_S")
            SQL.Append(" AND FURI_KBN_S ='" & pFuri_Kbn & "'")
            SQL.Append(" AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")
            SQL.Append(" AND GAKKOU_CODE_O = '" & pGakkou_Code & "'")
            SQL.Append(" AND TUKI_NO_O = '" & pNengetu.Substring(4, 2) & "'")
            SQL.Append(" AND FURIKAE_O = '0'")
            SQL.Append(" AND KAIYAKU_FLG_O <> '9'")
            SQL.Append(" AND SCH_KBN_S <> '2'")
            SQL.Append(" GROUP BY SCH_KBN_S , SFURI_DATE_S")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If Not OraReader.DataReader(SQL) Then
            End If

            While OraReader.EOF = False
                '�擾�����v���C�}�����ŒP��̃X�P�W���[�����擾
                '���W�v����X�P�W���[���̑Ώۊw�N���擾�����
                blnGAK_FLG = False
                If fn_GetGakunen2(pGakkou_Code, OraReader.GetItem("SCH_KBN_S"), OraReader.GetItem("SFURI_DATE_S"), iG_Flg) = False Then
                    'GoTo NextWhile:
                Else
                    bG_Flg = False

                    '�ĐU
                    SQL.Length = 0
                    SQL.Append(" SELECT ")
                    SQL.Append(" SUM(1) AS KENSUU")
                    SQL.Append(",SUM(SEIKYU_KIN_M) AS KINGAKU")
                    SQL.Append(" FROM G_MEIMAST , G_SCHMAST")
                    SQL.Append(" WHERE GAKKOU_CODE_M = GAKKOU_CODE_S")
                    SQL.Append(" AND FURI_DATE_M = FURI_DATE_S")
                    SQL.Append(" AND FURI_KBN_S ='" & pFuri_Kbn & "'")
                    '���s���̍ĐU�f�[�^�͐��SAIFURI_SUMI_M ='1'�ƂȂ��Ă��邽�߃R�����g 2006/10/05
                    SQL.Append(" AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")
                    SQL.Append(" AND SFURI_DATE_S ='" & OraReader.GetItem("SFURI_DATE_S") & "'")
                    SQL.Append(" AND GAKKOU_CODE_M = '" & pGakkou_Code & "'")

                    '���s���݂̂̌����ƍ��v�𒊏o�ˎ��s�{���s���̌����E���z�𒊏o 2006/10/05
                    SQL.Append(" AND SCH_KBN_S = '" & OraReader.GetItem("SCH_KBN_S") & "'")
                    If blnGAK_FLG = True Then
                        SQL.Append(" AND (")
                        For i As Integer = 1 To 9 Step 1
                            If iG_Flg(i) = 1 Then
                                If bG_Flg = True Then
                                    SQL.Append(" OR ")
                                End If
                                SQL.Append(" GAKUNEN_CODE_M=" & i)
                                bG_Flg = True
                            End If
                        Next
                        SQL.Append(" )")
                    End If

                    OraReader2 = New CASTCommon.MyOracleReader(MainDB)

                    If OraReader2.DataReader(SQL) Then

                        If IsDBNull(OraReader2.GetItem("KENSUU")) = False And _
                           IsDBNull(OraReader2.GetItem("KINGAKU")) = False Then

                            '�w��w�N��ALL0���ǂ��� 2006/12/26
                            Dim blnSITEI_GAK As Boolean = False
                            For i As Integer = 1 To 9 Step 1
                                If iG_Flg(i) = 1 Then
                                    blnSITEI_GAK = True
                                    Exit For
                                End If
                            Next

                            '�X�P�W���[���}�X�^�U�փf�[�^�쐬�����X�V
                            SQL.Length = 0
                            SQL.Append(" UPDATE G_SCHMAST SET ")
                            SQL.Append(" DATA_DATE_S='" & Str_Syori_Date(0) & "'")
                            SQL.Append(",DATA_FLG_S='1'")
                            '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
                            If STR_KINGAKU_CHK = "0" Then
                                SQL.Append(",CHECK_DATE_S='" & Str_Syori_Date(0) & "'")
                                SQL.Append(",CHECK_FLG_S='1'")
                            End If
                            '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END
                            SQL.Append(",TIME_STAMP_S='" & Str_Syori_Date(1) & "'")
                            If blnSITEI_GAK = True Then '�w��w�N��ALL0�̏ꍇ�͏��������E���z��0�Ƃ��� 2006/12/26
                                SQL.Append(",SYORI_KEN_S=" & CLng(OraReader2.GetItem("KENSUU")))
                                SQL.Append(",SYORI_KIN_S=" & CLng(OraReader2.GetItem("KINGAKU")))
                            Else
                                SQL.Append(",SYORI_KEN_S = 0")
                                SQL.Append(",SYORI_KIN_S = 0")
                            End If
                            SQL.Append(" WHERE GAKKOU_CODE_S = '" & pGakkou_Code & "'")
                            SQL.Append(" AND NENGETUDO_S = '" & pNengetu & "'")
                            SQL.Append(" AND FURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'")
                            SQL.Append(" AND SFURI_DATE_S ='" & OraReader.GetItem("SFURI_DATE_S") & "'")
                            SQL.Append(" AND SCH_KBN_S = '" & OraReader.GetItem("SCH_KBN_S") & "'")
                            SQL.Append(" AND FURI_KBN_S = '" & pFuri_Kbn & "'")

                            If MainDB.ExecuteNonQuery(SQL) = -1 Then
                                Exit Try
                            End If

                            If pFuri_Kbn = "1" Then
                                '----------------------
                                '�X�P�W���[���}�X�^�̍X�V(���U�̽��ޭ�ق̍ĐU�쐬�ς݃t���O�X�V)
                                '----------------------
                                SQL.Length = 0
                                SQL.Append(" UPDATE  G_SCHMAST SET ")
                                SQL.Append(" SAIFURI_FLG_S = '1'")
                                SQL.Append(" WHERE GAKKOU_CODE_S = '" & pGakkou_Code & "'")
                                SQL.Append(" AND NENGETUDO_S = '" & pNengetu & "'")
                                SQL.Append(" AND SCH_KBN_S = '" & OraReader.GetItem("SCH_KBN_S") & "'")
                                SQL.Append(" AND FURI_KBN_S = '0'")
                                SQL.Append(" AND SFURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")

                                If MainDB.ExecuteNonQuery(SQL) = -1 Then
                                    Exit Try
                                End If
                            End If
                        End If
                    End If

                    OraReader2.Close()
                    OraReader2 = Nothing
                End If

                'NextWhile:
                OraReader.NextRead()
            End While

            OraReader.Close()
            OraReader = Nothing

            ret = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���X�V)", "���s", ex.ToString)
        End Try

        Return ret

    End Function
    Private Function fn_GetGakunen(ByVal pGakkou_Code As String, ByRef pSiyou_gakunen() As Integer) As Integer

        Dim ret As Integer = -1

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim iMaxGakunen As Integer

            ReDim pSiyou_gakunen(9)

            '�I�����ꂽ�w�Z�̎w��U�֓��Œ��o
            '(�S�X�P�W���[���敪���Ώ�)
            Dim SQL As New StringBuilder(128)
            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append(" SCH_KBN_S")
            For Cnt As Integer = 1 To 9
                SQL.Append(", GAKUNEN" & Cnt & "_FLG_S")
                pSiyou_gakunen(Cnt) = 0
            Next
            SQL.Append(", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T")
            SQL.Append(" FROM G_SCHMAST")
            SQL.Append(" LEFT JOIN GAKMAST2 ON ")
            SQL.Append(" GAKKOU_CODE_S = GAKKOU_CODE_T")
            SQL.Append(" WHERE GAKKOU_CODE_S ='" & pGakkou_Code & "'")
            SQL.Append(" AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
            If STR_KINGAKU_CHK <> "0" Then
                SQL.Append(" AND CHECK_FLG_S ='1' ")
            End If
            'SQL.Append(" AND CHECK_FLG_S ='1' ")
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END
            SQL.Append(" AND TYUUDAN_FLG_S ='0'")
            '���ޭ�ً敪2(����)�͏����Ɋ܂܂Ȃ���
            SQL.Append(" AND SCH_KBN_S <> '2'")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If Not OraReader.DataReader(SQL) Then
                Exit Try
            Else
                While OraReader.EOF = False
                    iMaxGakunen = CInt(OraReader.GetItem("SIYOU_GAKUNEN_T"))
                    For Cnt As Integer = 1 To iMaxGakunen
                        Select Case CInt(OraReader.GetItem("GAKUNEN" & Cnt & "_FLG_S"))
                            Case 1
                                pSiyou_gakunen(Cnt) = OraReader.GetItem("GAKUNEN" & Cnt & "_FLG_S")
                        End Select
                    Next

                    OraReader.NextRead()
                End While
            End If

            OraReader.Close()
            OraReader = Nothing

            '�g�p�w�N�S�ĂɊw�N�t���O������ꍇ�͑S�w�N�ΏۂƂ��Ĉ���
            '�w�N
            For Cnt As Integer = 1 To iMaxGakunen
                If pSiyou_gakunen(Cnt) <> 1 Then
                    ret = iMaxGakunen
                    Exit Try
                End If
            Next

            ret = 0

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�N�擾)", "���s", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
    Private Function fn_GetGakunen2(ByVal pGakkou_Code As String, _
                                       ByVal pSch_Kbn As String, _
                                       ByVal pSFuri_Date As String, _
                                       ByRef pSiyou_gakunen() As Integer) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim iMaxGakunen As Integer

            ReDim pSiyou_gakunen(9)

            '�I�����ꂽ�w�Z�ɑ��݂���X�P�W���[�����
            '�ĐU�֓��܂ł𒊏o�����Ƃ��A
            '�X�P�W���[�����ɏW�v��������w�N���擾����
            Dim SQL As New StringBuilder(128)
            SQL.Append(" SELECT ")
            SQL.Append(" SCH_KBN_S")
            For Cnt As Integer = 1 To 9
                SQL.Append(", GAKUNEN" & Cnt & "_FLG_S")
                pSiyou_gakunen(Cnt) = 0
            Next
            SQL.Append(", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T")
            SQL.Append(" FROM G_SCHMAST")
            SQL.Append(" LEFT JOIN GAKMAST2 ON ")
            SQL.Append(" GAKKOU_CODE_S = GAKKOU_CODE_T")
            SQL.Append(" WHERE GAKKOU_CODE_S ='" & pGakkou_Code & "'")
            SQL.Append(" AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")
            SQL.Append(" AND SFURI_DATE_S ='" & pSFuri_Date & "'")
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- START
            If STR_KINGAKU_CHK <> "0" Then
                SQL.Append(" AND CHECK_FLG_S ='1' ")
            End If
            'SQL.Append(" AND CHECK_FLG_S ='1' ")
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�w�Z�@�����U�֋��z�m�F������INI���j--------- END
            SQL.Append(" AND TYUUDAN_FLG_S ='0'")
            SQL.Append(" AND SCH_KBN_S = '" & pSch_Kbn & "'")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If Not OraReader.DataReader(SQL) Then
                Exit Try
            End If

            While OraReader.EOF = False
                iMaxGakunen = CInt(OraReader.GetItem("SIYOU_GAKUNEN_T"))
                For Cnt As Integer = 1 To iMaxGakunen
                    Select Case CInt(OraReader.GetItem("GAKUNEN" & Cnt & "_FLG_S"))
                        Case 1
                            pSiyou_gakunen(Cnt) = OraReader.GetItem("GAKUNEN" & Cnt & "_FLG_S")
                            blnGAK_FLG = True
                        Case 0
                            pSiyou_gakunen(Cnt) = "0"
                    End Select
                Next

                OraReader.NextRead()
            End While

            OraReader.Close()
            OraReader = Nothing

            ret = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�N�擾)", "���s", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function

    Private Function CompareJissekiMastGMeiMastSyofuri(ByVal GakkouCode As String, ByVal SeikyuNentuki As String, ByVal FurikaeDate As String) As Boolean
        Return CompareJissekiMastGMeiMast(GakkouCode, SeikyuNentuki, FurikaeDate, False)
    End Function
    Private Function CompareJissekiMastGMeiMastSaifuri(ByVal GakkouCode As String, ByVal SeikyuNentuki As String, ByVal FurikaeDate As String) As Boolean
        Return CompareJissekiMastGMeiMast(GakkouCode, SeikyuNentuki, FurikaeDate, True)
    End Function
    Private Function CompareJissekiMastGMeiMast(ByVal pGakkouCode As String, ByVal pSeikyuNentuki As String, ByVal pFurikaeDate As String, ByVal SfuriFlg As Boolean) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Dim ret As Boolean = False

        Try
            '���׃}�X�^���W�v
            Dim SQL As New StringBuilder(128)
            SQL.Length = 0
            SQL.Append(" SELECT COUNT(1) AS KENSU,SUM(SEIKYU_KIN_M) AS KINGAKU")
            SQL.Append(" FROM G_MEIMAST")
            SQL.Append(" WHERE GAKKOU_CODE_M= '" & pGakkouCode & "'")
            SQL.Append(" AND SEIKYU_TAISYOU_M = '" & pSeikyuNentuki & "'")
            SQL.Append(" AND FURI_DATE_M = '" & pFurikaeDate & "' ")
            If SfuriFlg = False Then
                SQL.Append(" AND FURI_KBN_M = '0' ") '���U
            Else
                SQL.Append(" AND FURI_KBN_M = '1' ") '�ĐU
            End If
            SQL.Append(" AND SEIKYU_KIN_M > 0 ")

            Dim MeiCount As Long
            Dim MeiKingaku As Double

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) Then
                MeiCount = OraReader.GetItem("KENSU")

                '2011/06/16 �W���ŏC�� if���C��------------------START
                'If IsDBNull(OraReader.GetItem("KINGAKU")) Then
                If IsDBNull(OraReader.GetItem("KINGAKU")) OrElse OraReader.GetItem("KINGAKU") = "" Then
                    '2011/06/16 �W���ŏC�� if���C��------------------END
                    MeiKingaku = 0
                Else
                    MeiKingaku = CDbl(OraReader.GetItem("KINGAKU"))
                End If
            Else
                Exit Try
            End If

            OraReader.Close()
            OraReader = Nothing

            SQL.Length = 0
            SQL.Append(" SELECT SUM(SEIKYU_KEN_F) AS KENSU,SUM(SEIKYU_KIN_F) AS KINGAKU")
            SQL.Append(" FROM JISSEKIMAST")
            SQL.Append(" WHERE GAKKOU_CODE_F= '" & pGakkouCode & "' ")
            SQL.Append(" AND FURI_DATE_F = '" & pFurikaeDate & "' ")
            SQL.Append(" GROUP BY GAKKOU_CODE_F,SEIKYU_NENGETU_F,FURI_DATE_F")

            Dim JissekiCount As Long
            Dim JissekiKingaku As Double

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) Then
                JissekiCount = OraReader.GetItem("KENSU")
                '2011/06/16 �W���ŏC�� if���C��------------------START
                'If IsDBNull(OraReader.GetItem("KINGAKU")) Then
                If IsDBNull(OraReader.GetItem("KINGAKU")) OrElse OraReader.GetItem("KINGAKU") = "" Then
                    '2011/06/16 �W���ŏC�� if���C��------------------END
                    JissekiKingaku = 0
                Else
                    JissekiKingaku = CDbl(OraReader.GetItem("KINGAKU"))
                End If
            Else
                Exit Try
            End If

            OraReader.Close()
            OraReader = Nothing

            Select Case True
                Case MeiCount <> JissekiCount
                    '��������v���܂���
                    Call MessageBox.Show(G_MSG0015W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                Case MeiKingaku <> JissekiKingaku
                    '���z����v���܂���
                    Call MessageBox.Show(G_MSG0016W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
            End Select

            ret = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���у}�X�^��r)", "���s", ex.ToString)
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
    Private Function fn_CheckMeisai(ByRef pErrInfo As ErrListInfo) As Boolean

        Dim ret As Integer = -1

        Dim OraReader As MyOracleReader = Nothing

        Try
            With pErrInfo
                '���Z�@�֖����̓`�F�b�N
                If .Str_Err_Tkin_No.Trim = "" OrElse .Str_Err_Tsit_No.Trim = "" Then
                    .Str_Err_Msg = "�ϑ����Z�@�ւ������͂ł��B"

                    Exit Try
                Else
                    '���݃`�F�b�N
                    Dim SQL As New StringBuilder(128)
                    SQL.Append(" SELECT * FROM TENMAST")
                    SQL.Append(" WHERE KIN_NO_N = '" & .Str_Err_Tkin_No & "'")
                    SQL.Append(" AND SIT_NO_N = '" & .Str_Err_Tsit_No & "'")

                    OraReader = New MyOracleReader(MainDB)

                    If Not OraReader.DataReader(SQL) Then
                        .Str_Err_Msg = "���Z�@�փ}�X�^�ɓo�^����Ă��܂���B"
                        '���Z�@�ֈُ��1��Ԃ�
                        ret = 1

                        OraReader.Close()
                        OraReader = Nothing

                        Exit Try
                    End If
                End If

                '�����ԍ��K�茅�`�F�b�N
                If .Str_Err_Kouza.Trim.Length <> 7 Then
                    .Str_Err_Msg = "�����ԍ��̌����V���ȊO�ł�"
                    Exit Try
                End If

                '�����ԍ�����ALLZERO , ALL9 �`�F�b�N
                If .Str_Err_Kouza.Trim = "0000000" Then
                    .Str_Err_Msg = "�����ԍ���ALLZERO�l�͐ݒ�ł��܂���"
                    Exit Try
                End If

                If .Str_Err_Kouza.Trim = "9999999" Then
                    .Str_Err_Msg = "�����ԍ���ALL9�l�͐ݒ�ł��܂���"
                    Exit Try
                End If
            End With

            '���펞��0��Ԃ�
            ret = 0

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���׃`�F�b�N)", "���s", ex.ToString)
            ret = -1
        End Try

        Return ret

    End Function
    Private Function PFUNC_Query_String(ByVal SortKey As Integer, ByVal GakkoCode As String, ByVal Sort As Integer) As String

        PFUNC_Query_String = ""

        Dim SQL As New StringBuilder(128)

        SQL.Append(" SELECT TENMAST.SIT_NO_N")
        SQL.Append(",GAKMAST1.GAKKOU_CODE_G")
        SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
        SQL.Append(",HIMOMAST.HIMOKU_NAME01_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME02_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME04_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME06_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME07_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME08_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME09_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME10_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME11_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME12_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME13_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME14_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME15_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME03_H")
        SQL.Append(",HIMOMAST.HIMOKU_NAME05_H")
        SQL.Append(",TENMAST.KIN_NO_N")
        SQL.Append(",G_SCHMAST.FURI_DATE_S")
        SQL.Append(",SEITOMASTVIEW.GAKUNEN_CODE_O")
        SQL.Append(",SEITOMASTVIEW.SEITO_NO_O")
        SQL.Append(",SEITOMASTVIEW.SEITO_KNAME_O")
        SQL.Append(",SEITOMASTVIEW.SEITO_NNAME_O")
        SQL.Append(",SEITOMASTVIEW.MEIGI_KNAME_O")
        SQL.Append(",TENMAST.SIT_NNAME_N")
        SQL.Append(",SEITOMASTVIEW.TYOUSI_FLG_O")
        SQL.Append(",G_SCHMAST.FUNOU_FLG_S")
        SQL.Append(",G_MEIMAST.TKAMOKU_M")
        SQL.Append(",G_MEIMAST.GAKKOU_CODE_M ")
        SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
        SQL.Append(",G_MEIMAST.HIMOKU1_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU2_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU3_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU4_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU5_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU6_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU7_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU8_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU9_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU10_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU11_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU12_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU13_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU14_KIN_M")
        SQL.Append(",G_MEIMAST.HIMOKU15_KIN_M")
        SQL.Append(",G_MEIMAST.TUUBAN_M")
        SQL.Append(",G_MEIMAST.NENDO_M")
        SQL.Append(",G_MEIMAST.CLASS_CODE_M")
        SQL.Append(",G_MEIMAST.SEITO_NO_M")
        SQL.Append(",G_MEIMAST.TKOUZA_M")
        SQL.Append(" FROM KZFMAST.G_MEIMAST G_MEIMAST ")
        SQL.Append(",KZFMAST.SEITOMASTVIEW SEITOMASTVIEW ")
        SQL.Append(",KZFMAST.G_SCHMAST G_SCHMAST ")
        SQL.Append(",KZFMAST.HIMOMAST HIMOMAST ")
        SQL.Append(",KZFMAST.GAKMAST1 GAKMAST1 ")
        SQL.Append(",KZFMAST.TENMAST TENMAST ")
        SQL.Append(" WHERE ")
        SQL.Append(" ((((((G_MEIMAST.GAKKOU_CODE_M=SEITOMASTVIEW.GAKKOU_CODE_O) ")
        SQL.Append(" AND  (G_MEIMAST.NENDO_M=SEITOMASTVIEW.NENDO_O)) ")
        SQL.Append(" AND  (G_MEIMAST.GAKUNEN_CODE_M=SEITOMASTVIEW.GAKUNEN_CODE_O)) ")
        SQL.Append(" AND  (G_MEIMAST.CLASS_CODE_M=SEITOMASTVIEW.CLASS_CODE_O)) ")
        SQL.Append(" AND  (G_MEIMAST.SEITO_NO_M=SEITOMASTVIEW.SEITO_NO_O)) ")
        SQL.Append(" AND  (G_MEIMAST.TUUBAN_M=SEITOMASTVIEW.TUUBAN_O)) ")
        SQL.Append(" AND  (((G_MEIMAST.GAKKOU_CODE_M=G_SCHMAST.GAKKOU_CODE_S) ")
        SQL.Append(" AND  (G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S)) ")
        SQL.Append(" AND  (G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S)) ")
        SQL.Append(" AND  ((((SEITOMASTVIEW.GAKKOU_CODE_O=HIMOMAST.GAKKOU_CODE_H (+)) ")
        SQL.Append(" AND  (SEITOMASTVIEW.GAKUNEN_CODE_O=HIMOMAST.GAKUNEN_CODE_H (+))) ")
        SQL.Append(" AND  (SEITOMASTVIEW.HIMOKU_ID_O=HIMOMAST.HIMOKU_ID_H (+))) ")
        SQL.Append(" AND  (SEITOMASTVIEW.TUKI_NO_O=HIMOMAST.TUKI_NO_H (+))) ")
        SQL.Append(" AND  ((SEITOMASTVIEW.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G (+)) ")
        SQL.Append(" AND  (SEITOMASTVIEW.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G (+))) ")
        SQL.Append(" AND  ((SEITOMASTVIEW.TKIN_NO_O=TENMAST.KIN_NO_N (+)) ")
        SQL.Append(" AND  (SEITOMASTVIEW.TSIT_NO_O=TENMAST.SIT_NO_N (+))) ")
        SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = '" & GakkoCode & "'")

        SQL.Append(" AND G_MEIMAST.YOBI1_M = '" & Str_Syori_Date(1) & "'")
        SQL.Append(" AND SEITOMASTVIEW.TUKI_NO_O = '" & Mid(Trim(STR_FURIKAE_DATE(1)), 5, 2) & "'")

        SQL.Append(" AND ")
        SQL.Append(" ((G_MEIMAST.GAKUNEN_CODE_M=1 AND G_SCHMAST.GAKUNEN1_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=2 AND G_SCHMAST.GAKUNEN2_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=3 AND G_SCHMAST.GAKUNEN3_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=4 AND G_SCHMAST.GAKUNEN4_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=5 AND G_SCHMAST.GAKUNEN5_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=6 AND G_SCHMAST.GAKUNEN6_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=7 AND G_SCHMAST.GAKUNEN7_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=8 AND G_SCHMAST.GAKUNEN8_FLG_S='1') OR ")
        SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=9 AND G_SCHMAST.GAKUNEN9_FLG_S='1')) ")
        SQL.Append(" ORDER BY ")

        If Sort = 2 Then
            SQL.Append(" G_MEIMAST.TKIN_NO_M , G_MEIMAST.TSIT_NO_M , ")
        End If

        Select Case CInt(SortKey)
            Case 0
                '�w�N,�N���X,���k�ԍ�
                SQL.Append(" G_MEIMAST.GAKUNEN_CODE_M ASC ")
                SQL.Append(",G_MEIMAST.CLASS_CODE_M ASC ")
                SQL.Append(",G_MEIMAST.SEITO_NO_M ASC ")
                SQL.Append(",G_MEIMAST.NENDO_M ASC ")
                SQL.Append(",G_MEIMAST.TUUBAN_M ASC ")
            Case 1
                '���w�N�x,�ʔ�
                SQL.Append(" G_MEIMAST.GAKUNEN_CODE_M ASC ")
                SQL.Append(",G_MEIMAST.CLASS_CODE_M ASC ")
                SQL.Append(",G_MEIMAST.NENDO_M ASC ")
                SQL.Append(",G_MEIMAST.TUUBAN_M ASC ")
            Case 2
                '����������(���k��(��))
                SQL.Append(" G_MEIMAST.GAKUNEN_CODE_M ASC ")
                SQL.Append(",G_MEIMAST.CLASS_CODE_M ASC ")
                SQL.Append(",SEITOMASTVIEW.SEITO_KNAME_O ASC ")
                SQL.Append(",G_MEIMAST.NENDO_M ASC ")
                SQL.Append(",G_MEIMAST.TUUBAN_M ASC ")
        End Select

        PFUNC_Query_String = SQL.ToString

    End Function
    Public Function GFUNC_FD_Copy3(ByVal pForm As Form, _
                                  ByVal pTitleName As String, _
                                  ByVal pSouceFilePath As String, _
                                  ByVal pInitialFileName As String, _
                                  ByVal pBaitai As String) As Integer
        '2005/03/14
        Dim sPath As String = ""
        Dim sBuff As String = ""

        Dim oDlg As New SaveFileDialog

        On Error Resume Next

        GFUNC_FD_Copy3 = -1

        '--------------------
        'FD�ۑ�
        '--------------------
        '--------------------
        '�U�փf�[�^�ۑ���p�X
        '--------------------
        Select Case pBaitai
            Case "0"
                sPath = STR_IFL_PATH
            Case "1"
                sPath = "A:\"
        End Select

        If sPath = "" Then
            'ini�t�@�C����TAKIRAIFL�i�[���񂪐ݒ肳��Ă��܂���
            MessageBox.Show(String.Format(MSG0001E, "�˗��t�@�C���i�[��", "GCOMMON", "IRAIFL"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        Select Case StrConv(Mid(sPath, 1, 1), vbProperCase)
            Case "A", "B"
                '�e�c�ǂݎ�菈��
                '�m�F���b�Z�[�W�\��
                If MessageBox.Show(G_MSG0002I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> Windows.Forms.DialogResult.OK Then
                    GFUNC_FD_Copy3 = 1
                    Exit Function
                End If
        End Select
        Select Case StrConv(Mid(sPath, 1, 1), vbProperCase)
            Case "A", "B"

                With oDlg
                    '.Filter = STR_DLG_FILTER_NAME & " (" & STR_DLG_FILTER & ")|" & STR_DLG_FILTER

                    .Filter = "���ׂẴt�@�C�� (*.*)|*.*"

                    .FilterIndex = 1
                    .InitialDirectory = sPath

                    '.DefaultExt = STR_DEF_FILE_KBN
                    If Trim(pInitialFileName) <> "" Then
                        .FileName = pInitialFileName
                    End If
                    .Title = "[" & pTitleName & "]�U�փf�[�^�ۑ�"
                    .ShowDialog()
                    sBuff = .FileName
                End With


                If sBuff = pInitialFileName Or Err.Number <> 0 Then
                    '�L�����Z���̂Ƃ��́A�Z�b�g�����t�@�C�����̂ݕԂ�����
                    GFUNC_FD_Copy3 = 1
                    Exit Function
                End If

                If Dir(sBuff, vbNormal) <> "" Then Kill(sBuff)

                FileCopy(pSouceFilePath, sBuff)

                If Err.Number <> 0 Then
                    '�t�@�C���ۑ����s
                    Exit Function
                End If
            Case Else
                With DataGridView
                    Select Case CInt(.Rows(intLIST_COUNT).Cells(8).Value)
                        Case 0
                            '���U
                            sBuff = sPath & "G" & Trim(.Rows(intLIST_COUNT).Cells(2).Value) & "01.DAT"
                        Case 1
                            '�ĐU
                            sBuff = sPath & "G" & Trim(.Rows(intLIST_COUNT).Cells(2).Value) & "02.DAT"
                    End Select
                End With

                FileCopy(pSouceFilePath, sBuff)
                If Err.Number <> 0 Then
                    '�t�@�C���ۑ����s
                    Exit Function
                End If
        End Select

        GFUNC_FD_Copy3 = 0

    End Function

    Private Function MakeH_RecordSyofuri(ByVal pGakkou_Code As String, _
                                    ByVal pSeikyuNentuki As String, _
                                    ByRef GakkouName As String, _
                                    ByRef SaifuriSyubetu As Integer, _
                                    ByRef FileName As String, _
                                    ByVal sw As StreamWriter) As Boolean

        Return MakeH_Record(pGakkou_Code, pSeikyuNentuki, GakkouName, SaifuriSyubetu, FileName, False, sw)

    End Function
    Private Function MakeH_RecordSaifuri(ByVal pGakkou_Code As String, _
                                   ByVal pSeikyuNentuki As String, _
                                   ByRef GakkouName As String, _
                                   ByRef SaifuriSyubetu As Integer, _
                                   ByRef FileName As String, _
                                   ByVal sw As StreamWriter) As Boolean

        Return MakeH_Record(pGakkou_Code, pSeikyuNentuki, GakkouName, SaifuriSyubetu, FileName, True, sw)

    End Function
    Private Function MakeH_Record(ByVal pGakkou_Code As String, _
                                    ByVal pSeikyuNentuki As String, _
                                    ByRef GakkouName As String, _
                                    ByRef SaifuriSyubetu As Integer, _
                                    ByRef FileName As String, _
                                    ByVal SaiFuriFlg As Boolean, _
                                    ByVal sw As StreamWriter) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As New StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" GAKMAST2.*")
            SQL.Append(",GAKKOU_KNAME_G , GAKKOU_NNAME_G")
            SQL.Append(" FROM ")
            SQL.Append(" GAKMAST1")
            SQL.Append(",GAKMAST2")
            SQL.Append(",G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
            SQL.Append(" AND GAKKOU_CODE_G = GAKKOU_CODE_S")
            SQL.Append(" AND NENGETUDO_S = '" & pSeikyuNentuki & "'")
            SQL.Append(" AND GAKKOU_CODE_G = '" & pGakkou_Code & "'")
            If SaiFuriFlg = False Then
                SQL.Append(" AND FURI_KBN_S = '0'")
            Else
                SQL.Append(" AND FURI_KBN_S = '1'")
                SQL.Append(" AND GAKUNEN_CODE_G = 1")
                SQL.Append(" AND SCH_KBN_S <> '2'")
            End If

            If Not OraReader.DataReader(SQL) Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�փf�[�^�쐬)", "���s", "�����U�փf�[�^�O��")
                Exit Try
            Else
                Dim ArrayKinkoInf(3) As String
                '�w�b�_�[���R�[�h�\���̂ɒl���Z�b�g
                ArrayKinkoInf = fn_GetKinkoName(OraReader.GetItem("TKIN_NO_T"), OraReader.GetItem("TSIT_NO_T"))
                With gZENGIN_REC1                                                      '�S��f�[�^�쐬(�w�b�_)
                    .ZG1 = "1"                                                         '�f�[�^�敪(=1)       
                    .ZG2 = "91"                                                        '��ʃR�[�h
                    .ZG3 = "0" 'JIS�Ȃ̂�"0"�w�� 2006/10/04                            '�R�[�h�敪
                    .ZG4 = OraReader.GetItem("ITAKU_CODE_T")                         '�U���˗��l�R�[�h
                    .ZG5 = OraReader.GetItem("GAKKOU_KNAME_G")                       '�U���˗��l��
                    .ZG6 = Mid(STR_FURIKAE_DATE(1), 5, 4)                              '�戵��
                    .ZG7 = OraReader.GetItem("TKIN_NO_T")                            '�d����s����
                    .ZG8 = ArrayKinkoInf(0)                                                '�d����s��
                    .ZG9 = OraReader.GetItem("TSIT_NO_T")                            '�d���x�X����
                    .ZG10 = ArrayKinkoInf(2)                                               '�d���x�X��
                    .ZG11 = CASTCommon.ConvertKamoku2TO1(OraReader.GetItem("KAMOKU_T"))     '�a�����
                    .ZG12 = Format(CInt(OraReader.GetItem("KOUZA_T")), "0000000")    '�����ԍ�
                    .ZG13 = Space(17)                                                  '�_�~�[
                End With

                SaifuriSyubetu = CInt(OraReader.GetItem("SFURI_SYUBETU_T"))
                GakkouName = Trim(OraReader.GetItem("GAKKOU_NNAME_G"))
                If IsDBNull(OraReader.GetItem("FILE_NAME_T")) = True Then
                    FileName = ""
                Else
                    FileName = Trim(OraReader.GetItem("FILE_NAME_T"))
                End If

                OraReader.Close()
                OraReader = Nothing

                sw.Write(gZENGIN_REC1.Data)

                ret = True
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�b�_�[���擾)", "���s", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
    Private Function MakeD_RecordSyofuri(ByVal GakkouCode As String, _
                                         ByVal SeikyuNentuki As String, _
                                         ByRef SaifuriSyubetu As Integer, _
                                         ByRef TotalKensuu As Long, _
                                         ByRef TotalKingaku As Long, _
                                         ByRef ThrrowCount As Long, _
                                         ByVal sw As StreamWriter) As Integer

        Dim ret As Integer = -1

        Dim SQL As New StringBuilder(128)

        Try
            Dim sJyuyouka_No As String = ""
            Dim lRecordCount As Long = 0
            Dim bLoopFlg As Boolean = False
            Dim iGakunen_Flag() As Integer = Nothing
            Dim lFurikae_Kingaku As Long = 0
            Dim bFlg As Boolean = False

            '�X�P�W���[�����N�ԁA���ʂ���
            Select Case fn_GetGakunen(GakkouCode, iGakunen_Flag)
                Case -1
                    '�G���[
                    Exit Try
                Case 0
                    '�S�w�N���Ώ�
                    bFlg = False
                Case Else
                    '����w�N�݂̂��Ώ�
                    bFlg = True
            End Select

            '���k�}�X�^�擾
            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append(" SEITOMASTVIEW.*")
            SQL.Append(",TKIN_NO_T , TSIT_NO_T , KAMOKU_T , KOUZA_T")
            SQL.Append(" FROM SEITOMASTVIEW , GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_O = GAKKOU_CODE_T")
            SQL.Append(" AND GAKKOU_CODE_O = '" & GakkouCode & "'")
            SQL.Append(" AND TUKI_NO_O = '" & Mid(SeikyuNentuki, 5, 2) & "'")
            SQL.Append(" AND FURIKAE_O = '0'")
            SQL.Append(" AND KAIYAKU_FLG_O <> '9'")
            If bFlg = True Then
                SQL.Append(" AND (")
                For Cnt As Integer = 1 To 9
                    If iGakunen_Flag(Cnt) = 1 Then
                        If bLoopFlg = True Then
                            SQL.Append(" OR ")
                        End If
                        SQL.Append(" GAKUNEN_CODE_O=" & Cnt)
                        bLoopFlg = True
                    End If
                Next
                SQL.Append(" )")
            End If
            '�U���ް��͋��ɁE�X�ԁE�ȖځE�����ԍ��E�w�N(�~��)�E������ 2006/10/04
            SQL.Append(" ORDER BY ")
            SQL.Append(" TKIN_NO_O ASC")
            SQL.Append(",TSIT_NO_O ASC")
            SQL.Append(",KAMOKU_O ASC")
            SQL.Append(",KOUZA_O ASC")
            SQL.Append(",GAKUNEN_CODE_O DESC")

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

            If Not OraReader.DataReader(SQL) Then
                '0���̏ꍇ�͎��у}�X�^��UPDATE�����邽�ߔ����Ȃ�
                'Call GSUB_LOG(0, "�f�[�^���R�[�h�쐬���s")
                'Exit Try
            End If

            ThrrowCount = 0
            TotalKensuu = 0
            TotalKingaku = 0
            Lng_Err_Count = 0

            '2010/11/10�@�O�N�x���͎����z���Ȃ��i���o���Ȃ��j-----
            Dim strFURIDATE As String
            Dim strFURINEN As String
            Dim strFURITUKI As String
            Dim strPRE_FURINEN As String
            strFURIDATE = SeikyuNentuki
            strFURINEN = strFURIDATE.Substring(0, 4)
            strFURITUKI = strFURIDATE.Substring(4, 2)
            strPRE_FURINEN = CStr(CInt(strFURINEN) - 1)
            '-----------------------------------------------------

            While OraReader.EOF = False

                lFurikae_Kingaku = 0
                '�������z
                For Cnt As Integer = 1 To 15                                                                             '�U�֋��z���v
                    If Not IsDBNull(OraReader.GetItem("KINGAKU" & Format(Cnt, "00") & "_O")) Then
                        lFurikae_Kingaku += CLng(OraReader.GetItem("KINGAKU" & Format(Cnt, "00") & "_O"))
                    End If
                Next

                '�U�֕s�\���쐬
                Select Case SaifuriSyubetu
                    Case 2, 3
                        SQL.Length = 0
                        '�U�֕s�\�@�����U�֖��׃f�[�^�쐬
                        SQL.Append(" SELECT * FROM G_MEIMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_M = '" & GakkouCode & "'")
                        ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
                        'SQL.Append(" AND FURIKETU_CODE_M <> 0")
                        SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
                        ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
                        SQL.Append(" AND SAIFURI_SUMI_M = '0'")
                        SQL.Append(" AND SEIKYU_KIN_M > 0")
                        SQL.Append(" AND NENDO_M = '" & OraReader.GetItem("NENDO_O") & "'")
                        SQL.Append(" AND TUUBAN_M = " & OraReader.GetItem("TUUBAN_O"))
                        '2010/11/10 �����ƗՎ��o�������O��----------------
                        SQL.Append(" AND FURI_KBN_M <> '2'")    '����
                        SQL.Append(" AND FURI_KBN_M <> '3'")    '�Վ��o��
                        '-------------------------------------------------
                        '2010/11/10�@�O�N�x���͎����z���Ȃ��i���o���Ȃ��j------------------------------
                        '2011/06/16 �W���ŏC�� SQL���C��------------------START
                        If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                            SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401'  ")
                        ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                            SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401'  ")
                        End If
                        'If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                        '    SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401""'  ")
                        'ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                        '    SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401""'  ")
                        'End If
                        '2011/06/16 �W���ŏC�� SQL���C��------------------END
                        '-------------------------------------------------------------------------------
                        '�U���ް��͋��ɁE�X�ԁE�ȖځE�����ԍ��E�w�N(�~��)�E������ 2006/10/04
                        SQL.Append(" ORDER BY ")
                        SQL.Append(" TKIN_NO_M ASC ")
                        SQL.Append(",TSIT_NO_M ASC ")
                        SQL.Append(",TKAMOKU_M ASC ")
                        SQL.Append(",TKOUZA_M ASC ")
                        SQL.Append(",SEIKYU_TUKI_M ASC ")
                        SQL.Append(",GAKUNEN_CODE_M DESC")

                        Dim SaifuriReader As New CASTCommon.MyOracleReader(MainDB)

                        If Not SaifuriReader.DataReader(SQL) Then
                            '0���̏ꍇ�͎��у}�X�^��UPDATE�����邽�ߔ����Ȃ�
                            'Call GSUB_LOG(0, "�f�[�^���R�[�h�쐬���s")
                            'Exit Try
                        End If

                        While SaifuriReader.EOF = False
                            '�S��f�[�^�쐬(�ُ펞�Ɉُ탊�X�g�փp�����[�^��ۑ����邽��)

                            '���v�Ɣԍ�
                            sJyuyouka_No = SaifuriReader.GetItem("NENDO_M") & Format(CInt(SaifuriReader.GetItem("TUUBAN_M")), "0000") & Mid(SaifuriReader.GetItem("SEIKYU_TUKI_M"), 5, 2)
                            Dim ArrayKinkoInf(3) As String
                            ArrayKinkoInf = fn_GetKinkoName(SaifuriReader.GetItem("TKIN_NO_M"), _
                                                            SaifuriReader.GetItem("TSIT_NO_M"))

                            gZENGIN_REC2.ZG1 = "2"                                                                  '�f�[�^�敪(=2)
                            gZENGIN_REC2.ZG2 = SaifuriReader.GetItem("TKIN_NO_M")                              '��d����s�ԍ�
                            gZENGIN_REC2.ZG3 = ArrayKinkoInf(0)                                                         '��d����s���@
                            gZENGIN_REC2.ZG4 = SaifuriReader.GetItem("TSIT_NO_M")                              '��d���x�X�ԍ�
                            gZENGIN_REC2.ZG5 = ArrayKinkoInf(2)                                                         '��d���x�X��
                            gZENGIN_REC2.ZG6 = Space(4)                                                             '��`�������ԍ�
                            gZENGIN_REC2.ZG7 = CASTCommon.ConvertKamoku2TO1(SaifuriReader.GetItem("TKAMOKU_M"))  '�a�����
                            gZENGIN_REC2.ZG8 = SaifuriReader.GetItem("TKOUZA_M")                               '�����ԍ�
                            gZENGIN_REC2.ZG9 = Mid(SaifuriReader.GetItem("TMEIGI_KNM_M"), 1, 30)               '���l
                            gZENGIN_REC2.ZG10 = Format(CLng(SaifuriReader.GetItem("SEIKYU_KIN_M")), "0000000000")    '�U�����z
                            gZENGIN_REC2.ZG11 = "0"                                                                 '�V�K�R�[�h
                            gZENGIN_REC2.ZG12 = sJyuyouka_No                                                         '�ڋq�R�[�h�P
                            gZENGIN_REC2.ZG13 = "0" & Space(9)      '�X�P�W���[���敪��ݒ�                          '�ڋq�R�[�h�Q
                            gZENGIN_REC2.ZG14 = "0"                                                                 '�U���w��敪
                            gZENGIN_REC2.ZG15 = Space(8)                                                            '�_�~�[

                            '���דo�^�p�U�փf�[�^(�S��t�H�[�}�b�g�̃f�[�^���R�[�h�P�Q�OBYTE)
                            sw.Write(gZENGIN_REC2.Data)

                            ErrList.Int_Err_Gakunen_Code = SaifuriReader.GetItem("GAKUNEN_CODE_M")
                            ErrList.Int_Err_Class_Code = SaifuriReader.GetItem("CLASS_CODE_M")
                            ErrList.Str_Err_Seito_No = SaifuriReader.GetItem("SEITO_NO_M")
                            ErrList.Int_Err_Tuuban = SaifuriReader.GetItem("TUUBAN_M")
                            ErrList.Str_Err_Itaku_Name = ""
                            ErrList.Str_Err_Tkin_No = SaifuriReader.GetItem("TKIN_NO_M")
                            ErrList.Str_Err_Tsit_No = SaifuriReader.GetItem("TSIT_NO_M")
                            ErrList.Str_Err_Kamoku = SaifuriReader.GetItem("TKAMOKU_M")
                            ErrList.Str_Err_Kouza = SaifuriReader.GetItem("TKOUZA_M")
                            ErrList.Str_Err_Keiyaku_No = ""
                            ErrList.Str_Err_Keiyaku_Name = SaifuriReader.GetItem("TMEIGI_KNM_M")
                            ErrList.Lng_Err_Furikae_Kingaku = SaifuriReader.GetItem("SEIKYU_KIN_M")

                            '�����U�֖��׃f�[�^�쐬
                            SQL.Length = 0
                            SQL.Append(" INSERT INTO G_MEIMAST")
                            SQL.Append(" values(")
                            SQL.Append("'" & GakkouCode & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("NENDO_M") & "'")
                            SQL.Append(",'" & STR_FURIKAE_DATE(1) & "'")
                            SQL.Append("," & SaifuriReader.GetItem("GAKUNEN_CODE_M"))
                            SQL.Append("," & SaifuriReader.GetItem("CLASS_CODE_M"))
                            SQL.Append(",'" & SaifuriReader.GetItem("SEITO_NO_M") & "'")
                            SQL.Append("," & SaifuriReader.GetItem("TUUBAN_M"))
                            SQL.Append(",'" & SaifuriReader.GetItem("ITAKU_KIN_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("ITAKU_SIT_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("ITAKU_KAMOKU_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("ITAKU_KOUZA_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("TKIN_NO_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("TSIT_NO_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("TKAMOKU_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("TKOUZA_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("TMEIGI_KNM_M") & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("KTEKIYO_M") & "'") '�s�\�����̓E�v���Z�b�g
                            SQL.Append(",'" & sJyuyouka_No & "0" & "'") '�敪�ǉ�
                            SQL.Append(",'" & gZENGIN_REC2.Data & "'")
                            SQL.Append("," & lRecordCount)
                            SQL.Append(",'" & SaifuriReader.GetItem("SEIKYU_TUKI_M") & "'")
                            SQL.Append(",'" & SeikyuNentuki & "'")
                            SQL.Append(",'" & SaifuriReader.GetItem("HIMOKU_ID_M") & "'")
                            SQL.Append("," & SaifuriReader.GetItem("SEIKYU_KIN_M"))
                            For Cnt As Integer = 1 To 15
                                SQL.Append("," & SaifuriReader.GetItem("HIMOKU" & Cnt & "_KIN_M"))
                            Next
                            SQL.Append(",0")
                            '2005/03/15
                            '�ĐU�쐬�σt���O�������o�^���͖����͂������̂�0:���쐬��
                            '�쐬����
                            SQL.Append(",'0'")
                            SQL.Append(",'0'")
                            SQL.Append(",'" & Str_Syori_Date(1) & "'")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(",' '")
                            SQL.Append(")")

                            Select Case fn_CheckMeisai(ErrList)
                                Case -1
                                    '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^�Ȃ� , �X�P�W���[���X�V�Ȃ�)
                                    Lng_Err_Count += 1

                                    If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                        Exit Try
                                    End If
                                Case 0
                                    '����I��
                                    If MainDB.ExecuteNonQuery(SQL) = -1 Then
                                        '�ُ탊�X�g�ǉ�
                                        Lng_Err_Count += 1
                                        ErrList.Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                                        If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                            Exit Try
                                        End If
                                    Else
                                        TotalKingaku += SaifuriReader.GetItem("SEIKYU_KIN_M")
                                        TotalKensuu += 1

                                        lRecordCount += 1
                                    End If
                                Case Else
                                    '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^���� , �X�P�W���[���X�V����)
                                    ThrrowCount += 1
                                    If MainDB.ExecuteNonQuery(SQL) = -1 Then
                                        '�ُ탊�X�g�ǉ�
                                        Lng_Err_Count += 1
                                        ErrList.Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                                        If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                            Exit Try
                                        End If
                                    Else
                                        TotalKingaku += SaifuriReader.GetItem("SEIKYU_KIN_M")
                                        TotalKensuu += 1

                                        lRecordCount += 1
                                    End If

                                    Lng_Err_Count += 1

                                    If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                        Exit Try
                                    End If
                            End Select

                            SaifuriReader.NextRead()
                        End While

                        SaifuriReader.Close()
                        SaifuriReader = Nothing
                End Select

                '���v�Ɣԍ�
                sJyuyouka_No = OraReader.GetItem("NENDO_O") & Format(CInt(OraReader.GetItem("TUUBAN_O")), "0000") & Mid(SeikyuNentuki, 5, 2)

                ErrList.Int_Err_Gakunen_Code = OraReader.GetItem("GAKUNEN_CODE_O")
                ErrList.Int_Err_Class_Code = OraReader.GetItem("CLASS_CODE_O")
                ErrList.Str_Err_Seito_No = OraReader.GetItem("SEITO_NO_O")
                ErrList.Int_Err_Tuuban = OraReader.GetItem("TUUBAN_O")
                ErrList.Str_Err_Itaku_Name = ""
                ErrList.Str_Err_Tkin_No = OraReader.GetItem("TKIN_NO_O")
                ErrList.Str_Err_Tsit_No = OraReader.GetItem("TSIT_NO_O")
                ErrList.Str_Err_Kamoku = OraReader.GetItem("KAMOKU_O")
                ErrList.Str_Err_Kouza = OraReader.GetItem("KOUZA_O")
                ErrList.Str_Err_Keiyaku_No = ""
                ErrList.Str_Err_Keiyaku_Name = Mid(OraReader.GetItem("MEIGI_KNAME_O"), 1, 30)

                ErrList.Lng_Err_Furikae_Kingaku = lFurikae_Kingaku

                If lFurikae_Kingaku > 0 Then
                    Dim ArrayKinkoInf(3) As String
                    ArrayKinkoInf = fn_GetKinkoName(OraReader.GetItem("TKIN_NO_O"), OraReader.GetItem("TSIT_NO_O"))

                    '�S��f�[�^�쐬(����)
                    '�f�[�^�敪(=2)
                    '��d����s�ԍ�
                    '��d����s���@
                    '��d���x�X�ԍ�
                    '��d���x�X��
                    '��`�������ԍ�
                    '�a�����
                    '�����ԍ�
                    '���l
                    '�U�����z
                    '�V�K�R�[�h
                    '�ڋq�R�[�h�P
                    '�ڋq�R�[�h�Q
                    '�U���w��敪
                    '�_�~�[
                    gZENGIN_REC2.ZG1 = "2"
                    gZENGIN_REC2.ZG2 = OraReader.GetItem("TKIN_NO_O")
                    gZENGIN_REC2.ZG3 = ArrayKinkoInf(0)
                    gZENGIN_REC2.ZG4 = OraReader.GetItem("TSIT_NO_O")
                    gZENGIN_REC2.ZG5 = ArrayKinkoInf(2)
                    gZENGIN_REC2.ZG6 = Space(4)
                    gZENGIN_REC2.ZG7 = CASTCommon.ConvertKamoku2TO1(OraReader.GetItem("KAMOKU_O"))
                    gZENGIN_REC2.ZG8 = OraReader.GetItem("KOUZA_O")
                    gZENGIN_REC2.ZG9 = Mid(OraReader.GetItem("MEIGI_KNAME_O"), 1, 30)
                    gZENGIN_REC2.ZG10 = Format(lFurikae_Kingaku, "0000000000")
                    gZENGIN_REC2.ZG11 = "0"
                    gZENGIN_REC2.ZG12 = sJyuyouka_No
                    gZENGIN_REC2.ZG13 = "0" & Space(9)      '�X�P�W���[���敪��ݒ�
                    gZENGIN_REC2.ZG14 = "0"
                    gZENGIN_REC2.ZG15 = Space(8)

                    '���דo�^�p�U�փf�[�^(�S��t�H�[�}�b�g�̃f�[�^���R�[�h�P�Q�OBYTE)
                    Call sw.Write(gZENGIN_REC2.Data)

                    '�����U�֖��׍쐬
                    SQL.Length = 0
                    SQL.Append(" INSERT INTO G_MEIMAST")
                    SQL.Append(" VALUES (")
                    SQL.Append("'" & GakkouCode & "'")
                    SQL.Append(",'" & OraReader.GetItem("NENDO_O") & "'")
                    SQL.Append(",'" & STR_FURIKAE_DATE(1) & "'")
                    SQL.Append("," & OraReader.GetItem("GAKUNEN_CODE_O"))
                    SQL.Append("," & OraReader.GetItem("CLASS_CODE_O"))
                    SQL.Append(",'" & OraReader.GetItem("SEITO_NO_O") & "'")
                    SQL.Append("," & OraReader.GetItem("TUUBAN_O"))
                    SQL.Append(",'" & OraReader.GetItem("TKIN_NO_T") & "'")
                    SQL.Append(",'" & OraReader.GetItem("TSIT_NO_T") & "'")
                    SQL.Append(",'" & OraReader.GetItem("KAMOKU_T") & "'")
                    SQL.Append(",'" & OraReader.GetItem("KOUZA_T") & "'")
                    SQL.Append(",'" & OraReader.GetItem("TKIN_NO_O") & "'")
                    SQL.Append(",'" & OraReader.GetItem("TSIT_NO_O") & "'")
                    SQL.Append(",'" & OraReader.GetItem("KAMOKU_O") & "'")
                    SQL.Append(",'" & OraReader.GetItem("KOUZA_O") & "'")
                    SQL.Append(",'" & OraReader.GetItem("MEIGI_KNAME_O") & "'")
                    SQL.Append(",'" & Mid(SeikyuNentuki, 5, 2) & "�����'")
                    SQL.Append(",'" & sJyuyouka_No & "0" & Space(9) & "'") '�敪�ǉ�
                    SQL.Append(",'" & gZENGIN_REC2.Data & "'")
                    SQL.Append("," & lRecordCount)
                    SQL.Append(",'" & SeikyuNentuki & "'")
                    SQL.Append(",'" & SeikyuNentuki & "'")
                    SQL.Append(",'" & OraReader.GetItem("HIMOKU_ID_O") & "'")
                    SQL.Append("," & lFurikae_Kingaku)
                    For Cnt As Integer = 1 To 15
                        SQL.Append("," & OraReader.GetItem("KINGAKU" & Format(Cnt, "00") & "_O"))
                    Next
                    SQL.Append(",0")
                    '�ĐU�쐬�σt���O�������o�^���͖����͂������̂�0:���쐬��
                    '�쐬����
                    'SQL.append( ",' '"
                    SQL.Append(",'0'")
                    SQL.Append(",'0'")
                    SQL.Append(",'" & Str_Syori_Date(1) & "'")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(",' '")
                    SQL.Append(")")

                    Select Case fn_CheckMeisai(ErrList)
                        Case -1
                            '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^�Ȃ� , �X�P�W���[���X�V�Ȃ�)
                            Lng_Err_Count += 1

                            If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                Exit Try
                            End If
                        Case 0
                            '����I��
                            If MainDB.ExecuteNonQuery(SQL) = -1 Then
                                '�ُ탊�X�g�ǉ�
                                Lng_Err_Count += 1
                                ErrList.Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                                If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                    Exit Try
                                End If
                            Else
                                TotalKingaku += lFurikae_Kingaku
                                TotalKensuu += 1

                                lRecordCount += 1
                            End If
                        Case Else
                            '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^���� , �X�P�W���[���X�V����)
                            ThrrowCount += 1
                            If MainDB.ExecuteNonQuery(SQL) = -1 Then
                                '�ُ탊�X�g�ǉ�
                                Lng_Err_Count += 1
                                ErrList.Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                                If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                    Exit Try
                                End If
                            Else
                                TotalKingaku += lFurikae_Kingaku
                                TotalKensuu += 1

                                lRecordCount += 1
                            End If

                            Lng_Err_Count += 1

                            If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                Exit Try
                            End If
                    End Select
                End If

                OraReader.NextRead()
            End While

            OraReader.Close()
            OraReader = Nothing

            '���������O���̏ꍇ�I��
            '2011/07/04 �W���ŏC�� �쐬����0���ł��G���[���������݂����ꍇ�A�������s ------------------START
            If TotalKensuu = 0 AndAlso Lng_Err_Count = 0 Then
                'If TotalKensuu = 0 Then
                '2011/07/04 �W���ŏC�� �쐬����0���ł��G���[���������݂����ꍇ�A�������s ------------------END
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�փf�[�^�쐬)", "���s", "�S��̧��0��")
                MessageBox.Show(String.Format(G_MSG0007I, GakkouCode), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return 1 '����Q
            Else
                lngJIKOU_SAKI += 1
            End If

            If Lng_Err_Count = 0 Or Lng_Err_Count = ThrrowCount Then
                Select Case SaifuriSyubetu
                    Case 2, 3
                        'iGakunen_Flag
                        bLoopFlg = False

                        '�擾�����s�\�U�փf�[�^�̍ĐU�쐬�ς݋敪���X�V����
                        '----------------------
                        '�U�֍쐬�ϐݒ�
                        '----------------------
                        SQL.Length = 0
                        SQL.Append(" UPDATE G_MEIMAST SET ")
                        SQL.Append(" SAIFURI_SUMI_M ='1'")
                        SQL.Append(" WHERE")
                        SQL.Append(" (GAKKOU_CODE_M , NENDO_M , TUUBAN_M)")
                        SQL.Append(" = (")
                        SQL.Append(" SELECT ")
                        SQL.Append(" GAKKOU_CODE_O , NENDO_O , TUUBAN_O")
                        SQL.Append(" FROM SEITOMASTVIEW")
                        SQL.Append(" WHERE GAKKOU_CODE_M = GAKKOU_CODE_O")
                        SQL.Append(" AND NENDO_M = NENDO_O")
                        SQL.Append(" AND TUUBAN_M = TUUBAN_O")
                        SQL.Append(" AND KAIYAKU_FLG_O <> '9'")
                        SQL.Append(" AND FURIKAE_O = '0'")
                        '�N�x�Œ��o���Ă�
                        '���k�}�X�^�͂P���k�Ɍ����i�P�Q�j�����R�[�h�����݂����
                        '�P���ׂ��P�Q���ׂ܂ő�������̂�h�����߂ɒǉ�
                        SQL.Append(" AND TUKI_NO_O = '" & SeikyuNentuki.Substring(4, 2) & "'")
                        SQL.Append(" )")
                        SQL.Append(" AND GAKKOU_CODE_M = '" & GakkouCode & "'")
                        ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
                        'SQL.Append(" AND FURIKETU_CODE_M <> 0")
                        SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
                        ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
                        SQL.Append(" AND SAIFURI_SUMI_M = '0'")
                        'SQL.append( " AND"
                        'SQL.append( " TKIN_NO_M = '" & Str_Jikou_Ginko_Code & "'"'���S�M������ ���s���܂� 2007/09/04
                        SQL.Append(" AND SEIKYU_KIN_M > 0")
                        '2010/11/10 �����ƗՎ��o�������O��---------------
                        SQL.Append(" AND FURI_KBN_M <> '2'") '����
                        SQL.Append(" AND FURI_KBN_M <> '3'") '�Վ��o��
                        '------------------------------------------------
                        '2010/11/10�@�O�N�x���͎����z���Ȃ��i���o���Ȃ��j------------------------------
                        '2011/06/16 �W���ŏC�� SQL���C��------------------START
                        If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                            SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401'  ")
                        ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                            SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401'  ")
                        End If
                        'If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                        '    SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401""'  ")
                        'ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                        '    SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401""'  ")
                        'End If
                        '2011/06/16 �W���ŏC�� SQL���C��------------------END
                        '------------------------------------------------------------------------------
                        '�w��w�N�̏����ǉ�
                        If bFlg = True Then
                            SQL.Append(" AND (")
                            For Cnt As Integer = 1 To 9
                                If iGakunen_Flag(Cnt) = 1 Then
                                    If bLoopFlg = True Then
                                        SQL.Append(" OR ")
                                    End If
                                    SQL.Append(" GAKUNEN_CODE_M = " & Cnt)
                                    bLoopFlg = True
                                End If
                            Next
                            SQL.Append(" )")
                        End If

                        'EXCUTE
                        If MainDB.ExecuteNonQuery(SQL) = -1 Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�֍쐬�ϐݒ�)", "���s", Err.Description)
                            Exit Try
                        End If
                End Select
            End If

            Return 0 '����P

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^���R�[�h�쐬)", "���s", ex.ToString)
        End Try

        Return ret

    End Function
    Private Function MakeD_RecordSaifuri(ByVal GakkouCode As String, _
                                         ByVal SeikyuNentuki As String, _
                                         ByRef SaifuriSyubetu As Integer, _
                                         ByRef TotalKensuu As Long, _
                                         ByRef TotalKingaku As Long, _
                                         ByRef ThrrowCount As Long, _
                                         ByVal sw As StreamWriter) As Integer

        Dim ret As Boolean = -1

        Dim SQL As New StringBuilder(128)

        Try
            Dim sJyuyouka_No As String = ""
            Dim lRecordCount As Long = 0
            Dim bLoopFlg As Boolean = False
            Dim iGakunen_Flag() As Integer = Nothing
            Dim lFurikae_Kingaku As Long = 0
            Dim SyofuriDate As String = ""
            Dim bFlg As Boolean = False

            '�X�P�W���[�����N�ԁA���ʂ��� �ǉ� 2005/12/10
            Select Case fn_GetGakunen(GakkouCode, iGakunen_Flag)
                Case -1
                    '�G���[
                    Exit Function
                Case 0
                    '�S�w�N���Ώ�
                    bFlg = False
                Case Else
                    '����w�N�݂̂��Ώ�
                    bFlg = True
            End Select

            '2010/11/10�@�O�N�x���͎����z���Ȃ��i���o���Ȃ��j-----------------
            Dim strFURIDATE As String
            Dim strFURINEN As String
            Dim strFURITUKI As String
            Dim strPRE_FURINEN As String
            strFURIDATE = SeikyuNentuki
            strFURINEN = strFURIDATE.Substring(0, 4)
            strFURITUKI = strFURIDATE.Substring(4, 2)
            strPRE_FURINEN = CStr(CInt(strFURINEN) - 1)
            '-----------------------------------------------------------

            '�ĐU��ʁ��P
            '�X�P�W���[���}�X�^��菉�U���̎擾
            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append(" FURI_DATE_S")
            SQL.Append(" FROM G_SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append(" GAKKOU_CODE_S = '" & GakkouCode & "'")
            SQL.Append(" AND NENGETUDO_S = '" & SeikyuNentuki & "'")
            SQL.Append(" AND FURI_KBN_S = '0'")
            SQL.Append(" AND SCH_KBN_S <> '2'")
            SQL.Append(" AND SFURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'")
            '�w��w�N�̏����ǉ� 2005/12/08
            If bFlg = True Then
                SQL.Append(" AND (")
                For iLcount As Integer = 1 To 9
                    If iGakunen_Flag(iLcount) = 1 Then
                        If bLoopFlg = True Then
                            SQL.Append(" AND ")
                        End If

                        SQL.Append(" GAKUNEN" & iLcount & "_FLG_S = '1'")
                        bLoopFlg = True
                    End If
                Next iLcount

                SQL.Append(" )")
            End If

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

            If Not OraReader.DataReader(SQL) Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���U���擾)", "���s", Err.Description)
                OraReader.Close()
                OraReader = Nothing
                Exit Try
            Else
                SyofuriDate = OraReader.GetItem("FURI_DATE_S").Trim
                OraReader.Close()
                OraReader = Nothing
            End If

            '�s�\�U�փf�[�^�̎擾
            bLoopFlg = False
            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append(" G_MEIMAST.*")
            SQL.Append(" FROM ")
            SQL.Append(" G_MEIMAST")
            SQL.Append(",SEITOMASTVIEW")
            SQL.Append(" WHERE GAKKOU_CODE_M = GAKKOU_CODE_O")
            SQL.Append(" AND NENDO_M = NENDO_O")
            SQL.Append(" AND TUUBAN_M = TUUBAN_O")
            SQL.Append(" AND GAKKOU_CODE_M = '" & GakkouCode & "'")
            'SQL.append(" AND TKIN_NO_M = '" & Str_Jikou_Ginko_Code & "'"'���S�M���͑��s���܂߂� 2007/09/04
            ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            'SQL.Append(" AND FURIKETU_CODE_M <> 0")
            SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
            ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
            SQL.Append(" AND SAIFURI_SUMI_M = '0'")
            SQL.Append(" AND SEIKYU_KIN_M > 0")
            SQL.Append(" AND KAIYAKU_FLG_O <> '9'")
            SQL.Append(" AND FURIKAE_O = '0'")
            '2010/11/10 �����ƗՎ��o�������O��------------
            SQL.Append(" AND FURI_KBN_M <> '2'") '����
            SQL.Append(" AND FURI_KBN_M <> '3'") '�Վ��o��
            '---------------------------------------------
            '2010/11/10�@�O�N�x���͎����z���Ȃ��i���o���Ȃ��j----------------------------------
            '2011/06/16 �W���ŏC�� SQL���C��------------------START
            If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401'  ")
            ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401'  ")
            End If
            'If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
            '    SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401""'  ")
            'ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
            '    SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401""'  ")
            'End If
            '2011/06/16 �W���ŏC�� SQL���C��------------------END
            '----------------------------------------------------------------------------------
            '�N�x�Œ��o���Ă�
            '���k�}�X�^�͂P���k�Ɍ����i�P�Q�j�����R�[�h�����݂����
            '�P���ׂ��P�Q���ׂ܂ő�������̂�h�����߂ɒǉ�
            SQL.Append(" AND TUKI_NO_O = '" & SeikyuNentuki.Substring(4, 2) & "'")

            Select Case SaifuriSyubetu
                Case 1
                    '���׃}�X�^��蓖���ŕs�\�ƂȂ������U�f�[�^���擾����
                    '���׃}�X�^�̐U�֓��͎擾�������U�����g�p����
                    SQL.Append(" AND FURI_DATE_M = '" & SyofuriDate & "'")
                    '2011/06/16 �W���ŏC�� ���z���敪�ύX�Ή� ------------------START
                    'SQL.Append(" AND SEIKYU_TUKI_M ='" & SeikyuNentuki & "'")
                    '�ĐU�̏ꍇ�́A���U�ŐU�ւ��s�������ׂ��W�v
                    SQL.Append(" AND SEIKYU_TAISYOU_M ='" & SeikyuNentuki & "'")
                    '2011/06/16 �W���ŏC�� ���z���敪�ύX�Ή� ------------------END
                Case 2
                    '�ĐU��ʁ��Q
                    '�ߋ��ɕs�\�ƂȂ����S�U�փf�[�^���擾����
            End Select
            '�w��w�N�̏���
            If bFlg = True Then
                SQL.Append(" AND (")
                For iLcount As Integer = 1 To 9
                    If iGakunen_Flag(iLcount) = 1 Then
                        If bLoopFlg = True Then
                            SQL.Append(" OR ")
                        End If
                        SQL.Append(" GAKUNEN_CODE_M = " & iLcount)
                        bLoopFlg = True
                    End If
                Next iLcount
                SQL.Append(" )")
            End If

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If Not OraReader.DataReader(SQL) Then
                '0���̏ꍇ�͎��у}�X�^��UPDATE�����邽�ߔ����Ȃ�
                'Call GSUB_LOG(0, "�f�[�^���R�[�h�쐬���s")
                'Exit Try
            End If

            ThrrowCount = 0
            lRecordCount = 1
            TotalKensuu = 0
            TotalKingaku = 0
            Lng_Err_Count = 0

            '�擾�����U�փf�[�^�̍ĐU�p�̐U�փf�[�^���쐬����
            While OraReader.EOF = False
                ErrList.Int_Err_Gakunen_Code = OraReader.GetItem("GAKUNEN_CODE_M")
                ErrList.Int_Err_Class_Code = OraReader.GetItem("CLASS_CODE_M")
                ErrList.Str_Err_Seito_No = OraReader.GetItem("SEITO_NO_M")
                ErrList.Int_Err_Tuuban = OraReader.GetItem("TUUBAN_M")
                ErrList.Str_Err_Itaku_Name = ""
                ErrList.Str_Err_Tkin_No = OraReader.GetItem("TKIN_NO_M")
                ErrList.Str_Err_Tsit_No = OraReader.GetItem("TSIT_NO_M")
                ErrList.Str_Err_Kamoku = OraReader.GetItem("TKAMOKU_M")
                ErrList.Str_Err_Kouza = OraReader.GetItem("TKOUZA_M")
                ErrList.Str_Err_Keiyaku_No = ""
                ErrList.Str_Err_Keiyaku_Name = OraReader.GetItem("TMEIGI_KNM_M")
                ErrList.Lng_Err_Furikae_Kingaku = OraReader.GetItem("SEIKYU_KIN_M")

                '�O��U�֑S��f�[�^
                gZENGIN_REC2.Data = CStr(OraReader.GetItem("FURI_DATA_M"))
                '���v�Ɣԍ��ɃX�P�W���[���敪���Z�b�g
                gZENGIN_REC2.ZG13 = "1" & Space(9)
                Call sw.Write(gZENGIN_REC2.Data)

                '�����U�֖��׃f�[�^�쐬
                SQL.Length = 0
                SQL.Append(" INSERT INTO ")
                SQL.Append(" G_MEIMAST")
                SQL.Append(" VALUES (")
                SQL.Append(" '" & GakkouCode & "'")
                SQL.Append(",'" & OraReader.GetItem("NENDO_M") & "'")
                SQL.Append(",'" & STR_FURIKAE_DATE(1) & "'")
                SQL.Append("," & OraReader.GetItem("GAKUNEN_CODE_M"))
                SQL.Append("," & OraReader.GetItem("CLASS_CODE_M"))
                SQL.Append(",'" & OraReader.GetItem("SEITO_NO_M") & "'")
                SQL.Append("," & OraReader.GetItem("TUUBAN_M"))
                SQL.Append(",'" & OraReader.GetItem("ITAKU_KIN_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("ITAKU_SIT_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("ITAKU_KAMOKU_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("ITAKU_KOUZA_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("TKIN_NO_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("TSIT_NO_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("TKAMOKU_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("TKOUZA_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("TMEIGI_KNM_M") & "'")
                SQL.Append(",'" & OraReader.GetItem("KTEKIYO_M") & "'")
                SQL.Append(",'" & Trim(OraReader.GetItem("JUYOUKA_NO_M")).Substring(0, 10) & "1" & Space(9) & "'")
                SQL.Append(",'" & gZENGIN_REC2.Data & "'")
                SQL.Append("," & lRecordCount)
                SQL.Append(",'" & OraReader.GetItem("SEIKYU_TUKI_M") & "'")
                Select Case SaifuriSyubetu
                    Case 1
                        SQL.Append(",'" & SeikyuNentuki & "'")
                    Case 2
                        SQL.Append(",'" & OraReader.GetItem("SEIKYU_TAISYOU_M") & "'")
                End Select
                SQL.Append(",'" & OraReader.GetItem("HIMOKU_ID_M") & "'")
                SQL.Append("," & OraReader.GetItem("SEIKYU_KIN_M"))
                For iLoopCount As Integer = 1 To 15
                    SQL.Append("," & OraReader.GetItem("HIMOKU" & iLoopCount & "_KIN_M"))
                Next iLoopCount
                SQL.Append(",0")
                SQL.Append(",'0'")
                SQL.Append(",'1'")
                SQL.Append(",'" & Str_Syori_Date(1) & "'")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(",' '")
                SQL.Append(")")

                Select Case fn_CheckMeisai(ErrList)
                    Case -1
                        '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^�Ȃ� , �X�P�W���[���X�V�Ȃ�)
                        Lng_Err_Count += 1

                        If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                            Exit Try
                        End If
                    Case 0
                        '����I��
                        'EXCUTE
                        If MainDB.ExecuteNonQuery(SQL) = -1 Then
                            '�ُ탊�X�g�ǉ�
                            Lng_Err_Count += 1
                            ErrList.Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                            If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                Exit Try
                            End If
                        Else
                            TotalKingaku += OraReader.GetItem("SEIKYU_KIN_M")
                            TotalKensuu += 1

                            lRecordCount += 1
                        End If
                    Case Else
                        '�G���[�L(�ُ탊�X�g�ɒǉ� , ���ׂɓo�^���� , �X�P�W���[���X�V����)
                        ThrrowCount += 1
                        If MainDB.ExecuteNonQuery(SQL) = -1 Then
                            '�ُ탊�X�g�ǉ�
                            Lng_Err_Count += 1
                            ErrList.Str_Err_Msg = "�f�[�^�x�[�X�G���[�ł�"

                            If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                                Exit Try
                            End If
                        Else
                            TotalKingaku += OraReader.GetItem("SEIKYU_KIN_M")
                            TotalKensuu += 1

                            lRecordCount += 1
                        End If

                        Lng_Err_Count += 1

                        If MainDB.ExecuteNonQuery(MakeInsertIjyoListSQL(GakkouCode)) = -1 Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ُ탊�X�g�쐬)", "���s", Err.Description)
                            Exit Try
                        End If
                End Select

                OraReader.NextRead()
            End While

            '���������O���̏ꍇ�I��
            '2011/07/04 �W���ŏC�� �쐬����0���ł��G���[���������݂����ꍇ�A�������s ------------------START
            If TotalKensuu = 0 AndAlso Lng_Err_Count = 0 Then
                'If TotalKensuu = 0 Then
                '2011/07/04 �W���ŏC�� �쐬����0���ł��G���[���������݂����ꍇ�A�������s ------------------END
                MessageBox.Show(String.Format(G_MSG0007I, GakkouCode), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                'GoTo JISSEKI_UPDATE2
                Return 1
            Else
                lngJIKOU_SAKI += 1
            End If

            If Lng_Err_Count = 0 Or Lng_Err_Count = ThrrowCount Then

                '�擾�����s�\�U�փf�[�^�̍ĐU�쐬�ς݋敪���X�V����
                '----------------------
                '�U�֍쐬�ϐݒ�
                '----------------------
                bLoopFlg = False
                SQL.Length = 0
                SQL.Append(" UPDATE G_MEIMAST SET ")
                SQL.Append(" SAIFURI_SUMI_M ='1'")
                SQL.Append(" WHERE (GAKKOU_CODE_M , NENDO_M , TUUBAN_M)")
                SQL.Append(" = (")
                SQL.Append(" SELECT GAKKOU_CODE_O , NENDO_O , TUUBAN_O")
                SQL.Append(" FROM SEITOMASTVIEW")
                SQL.Append(" WHERE GAKKOU_CODE_M = GAKKOU_CODE_O")
                SQL.Append(" AND NENDO_M = NENDO_O")
                SQL.Append(" AND TUUBAN_M = TUUBAN_O")
                SQL.Append(" AND KAIYAKU_FLG_O <> '9'")
                SQL.Append(" AND FURIKAE_O = '0'")
                SQL.Append(" AND TUKI_NO_O = '" & SeikyuNentuki.Substring(4, 2) & "'")
                SQL.Append(" )")
                SQL.Append(" AND GAKKOU_CODE_M = '" & GakkouCode & "'")
                ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
                'SQL.Append(" AND FURIKETU_CODE_M <> 0")
                SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
                ' 2017/02/21 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
                SQL.Append(" AND SAIFURI_SUMI_M = '0'")
                'SQL.append( " AND TKIN_NO_M = '" & Str_Jikou_Ginko_Code & "'"'���S�M������ ���s���܂� 2007/09/04
                SQL.Append(" AND SEIKYU_KIN_M > 0")
                SQL.Append(" AND FURI_DATE_M = '" & SyofuriDate & "'")
                '2010/11/10 �����ƗՎ��o�������O��------------
                SQL.Append(" AND FURI_KBN_M <> '2'") '����
                SQL.Append(" AND FURI_KBN_M <> '3'") '�Վ��o��
                '---------------------------------------------
                '2010/11/10�@�O�N�x���͎����z���Ȃ��i���o���Ȃ��j------------------------------
                '2011/06/16 �W���ŏC�� SQL���C��------------------START
                If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                    SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401'  ")
                ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                    SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401'  ")
                End If
                'If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                '    SQL.Append(" AND FURI_DATE_M >= '" & strFURINEN & "0401""'  ")
                'ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                '    SQL.Append(" AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401""'  ")
                'End If
                '2011/06/16 �W���ŏC�� SQL���C��------------------END
                '------------------------------------------------------------------------------
                Select Case SaifuriSyubetu
                    Case 1
                        SQL.Append(" AND FURI_KBN_M = '0'")
                        SQL.Append(" AND SEIKYU_TUKI_M = '" & SeikyuNentuki & "'")
                End Select
                '�w��w�N�̏����ǉ�
                If bFlg = True Then
                    SQL.Append(" AND (")
                    For iLcount As Integer = 1 To 9
                        If iGakunen_Flag(iLcount) = 1 Then
                            If bLoopFlg = True Then
                                SQL.Append(" OR ")
                            End If
                            SQL.Append(" GAKUNEN_CODE_M = " & iLcount)
                            bLoopFlg = True
                        End If
                    Next iLcount
                    SQL.Append(" )")
                End If

                If MainDB.ExecuteNonQuery(SQL) = -1 Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�֍쐬�ϐݒ�)", "���s", Err.Description)
                    Exit Try
                End If
            End If

            Return 0

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^���R�[�h�쐬)", "���s", ex.ToString)
        End Try

        Return ret

    End Function
    Private Function MakeT_Record(ByVal TotalKensu As Long, ByVal TotalKingaku As Long, ByVal sw As StreamWriter) As Boolean

        Try
            '�S��f�[�^�쐬(�g���[���[�s��W�v���ʣ)
            '�쐬���������Ƌ��z��ݒ�
            '�O���̕s�\���͍ĐU��ʂ�0�ȊO�̏ꍇ�̂ݏW�v���ʂɉ��Z�����
            '(�S��f�[�^�쐬(����)�������l)
            With gZENGIN_REC8
                .ZG1 = "8"
                .ZG2 = Format(TotalKensu, "000000")
                .ZG3 = Format(TotalKingaku, "000000000000")
                .ZG4 = "000000"
                .ZG5 = "000000000000"
                .ZG6 = "000000"
                .ZG7 = "000000000000"
                .ZG8 = Space(65)
            End With

            Call sw.Write(gZENGIN_REC8.Data)

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�g���[�����R�[�h�쐬)", "���s", ex.ToString)
        End Try

        Return False

    End Function
    Private Function MakeE_Record(ByVal sw As StreamWriter) As Boolean

        Try
            '�S��f�[�^�쐬(�I���s)
            '�f�[�^�敪
            '�_�~�[
            With gZENGIN_REC9
                .ZG1 = "9"
                .ZG2 = Space(119)
            End With

            Call sw.Write(gZENGIN_REC9.Data)

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�G���h���R�[�h�쐬)", "���s", ex.ToString)
        End Try

        Return False

    End Function

    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView.RowPostPaint
        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' �s�w�b�_�̃Z���̈���A�s�ԍ���`�悷�钷���`�Ƃ���
        ' �i�������E�[��4�h�b�g�̂����Ԃ��󂯂�j
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' ��L�̒����`���ɍs�ԍ����c�����������E�l�ŕ`�悷��
        ' �t�H���g��F�͍s�w�b�_�̊���l���g�p����
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub

End Class
