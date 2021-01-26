Imports clsFUSION.clsMain
Imports System.Drawing.Printing
Imports System.Text
Imports System.IO
Imports CASTCommon.ModPublic
Imports System.Data.OracleClient
Imports System.Runtime.InteropServices
Imports CASTCommon

Public Class KFJMAST101
    Inherits System.Windows.Forms.Form

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST101", "�萔�������t���O�ꊇ�X�V���")
    Private Const msgTitle As String = "�萔�������t���O�ꊇ�X�V���(KFJMAST101)"
    '�\�[�g�I�[�_�[�t���O
    Dim SortOrderFlag As Boolean = True

    '�N���b�N������̔ԍ�
    Dim ClickedColumn As Integer

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Public startFuriDate As String = ""
    Public endFuriDate As String = ""
    Public TorisCode As String = ""
    Public TorifCode As String = ""

#Region " ��ʂ̃��[�h "
    Protected Sub KFJMAST101_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim OraDB As New CASTCommon.MyOracle()
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQL As StringBuilder
        Dim MotherForm As New KFJMAST100

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = String.Concat(TorisCode, TorifCode.PadLeft(2, "0"c))
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            With Me.ListView1
                .Clear()
                .CheckBoxes = True
                .Columns.Add("", 30, HorizontalAlignment.Center)
                .Columns.Add("����溰��", 100, HorizontalAlignment.Center)
                .Columns.Add("����於", 150, HorizontalAlignment.Center)
                .Columns.Add("���ϋ敪", 100, HorizontalAlignment.Center)
                .Columns.Add("�U�ֺ���", 60, HorizontalAlignment.Center)
                .Columns.Add("��ƺ���", 60, HorizontalAlignment.Center)
                .Columns.Add("�U�֓�", 80, HorizontalAlignment.Center)
                .Columns.Add("�萔�������\���", 80, HorizontalAlignment.Center)
                .Columns.Add("�萔�����z", 100, HorizontalAlignment.Right)
            End With

            SQL = New StringBuilder(128)

            SQL.Append("SELECT TORIS_CODE_S")
            SQL.Append(",TORIF_CODE_S")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",KESSAI_KBN_T")
            SQL.Append(",FURI_CODE_S")
            SQL.Append(",KIGYO_CODE_S")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(",TESUU_YDATE_S")
            SQL.Append(",TESUU_KIN_S")
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND TORIS_CODE_T =" & SQ(TorisCode))
            If TorifCode.Trim.Length = 2 Then
                SQL.Append(" AND TORIF_CODE_T =" & SQ(TorifCode))
            End If
            SQL.Append(" AND FURI_DATE_S >= " & SQ(startFuriDate))
            SQL.Append(" AND FURI_DATE_S <= " & SQ(endFuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND KESSAI_FLG_S = '1'")
            SQL.Append(" AND TESUUTYO_FLG_S = '0'")
            SQL.Append(" AND KESSAI_KBN_T <> '99'")
            SQL.Append("ORDER BY TORIS_CODE_S ASC,TORIF_CODE_S ASC,FURI_DATE_S ASC")

            Dim LineColor As Color
            Dim ROW As Integer = 0

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    Dim Data(8) As String
                    Data(1) = String.Concat(New String() {OraReader.GetString("TORIS_CODE_S"), "-", OraReader.GetString("TORIF_CODE_S")})    ' �����R�[�h        
                    Data(2) = OraReader.GetString("ITAKU_NNAME_T").Trim     ' ����於

                    Select Case OraReader.GetString("KESSAI_KBN_T")         ' ���ϋ敪
                        Case "00"
                            Data(3) = "�a����"
                        Case "01"
                            Data(3) = "��������"
                        Case "02"
                            Data(3) = "�ב֐U��"
                        Case "03"
                            Data(3) = "�ב֕t��"
                        Case "04"
                            Data(3) = "�ʒi�o���̂�"
                        Case "05"
                            Data(3) = "���ʊ��"
                        Case "99"
                            Data(3) = "���ϑΏۊO"
                    End Select

                    Data(4) = OraReader.GetString("FURI_CODE_S")            ' �U�փR�[�h
                    Data(5) = OraReader.GetString("KIGYO_CODE_S")           ' ��ƃR�[�h
                    Data(6) = String.Concat(New String() {OraReader.GetString("FURI_DATE_S").Substring(0, 4), _
                                                          "/", OraReader.GetString("FURI_DATE_S").Substring(4, 2), "/" _
                                                          , OraReader.GetString("FURI_DATE_S").Substring(6, 2)})           ' �U�֓�
                    Data(7) = String.Concat(New String() {OraReader.GetString("TESUU_YDATE_S").Substring(0, 4), _
                                      "/", OraReader.GetString("TESUU_YDATE_S").Substring(4, 2), "/" _
                                      , OraReader.GetString("TESUU_YDATE_S").Substring(6, 2)})           ' �萔�������\���
                    Data(8) = OraReader.GetInt64("TESUU_KIN_S").ToString("#,###")          ' �萔�����z

                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                    OraReader.NextRead()

                Loop
            End If

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
    
#Region " �I�� "
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
#End Region

#Region " ���s�{�^�� "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :�쐬�{�^���������̏���
        'Return         :
        'Create         :
        'Update         :
        '=====================================================================================
        Dim MainDB As New CASTCommon.MyOracle()
        Dim SQL As StringBuilder
        Dim CreateCSV As New KFJP057

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)�J�n", "����", "")

            Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
            Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '******************************************
            ' �`�F�b�N�{�b�N�X�̃`�F�b�N
            '******************************************
            '���X�g�ɂP�����\������Ă��Ȃ��Ƃ�
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '���X�g�ɂP���ȏ�\������Ă��邪�A�`�F�b�N����Ă��Ȃ��Ƃ�
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '------------------------------
            '���s�m�F���b�Z�[�W
            '------------------------------
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If
            MainDB.BeginTrans()

            '********************************************
            ' �X�P�W���[���}�X�^�̍X�V
            '********************************************
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName

            For Each item As ListViewItem In nItems
                SQL = New StringBuilder(128)

                If item.Checked = True Then

                    SQL.Append("UPDATE SCHMAST SET TESUUTYO_FLG_S = '1' ")
                    SQL.Append(",TESUU_DATE_S = '" & SyoriDate & "'")
                    SQL.Append(",TESUU_TIME_STAMP_S = '" & String.Concat(SyoriDate, SyoriTime) & "'")
                    SQL.Append(" WHERE TORIS_CODE_S = '" & item.SubItems(1).Text.Replace("-", "").Substring(0, 10) & "' ")
                    SQL.Append(" AND TORIF_CODE_S = '" & item.SubItems(1).Text.Replace("-", "").Substring(10, 2) & "' ")
                    SQL.Append(" AND FURI_DATE_S = '" & item.SubItems(6).Text.Replace("/", "") & "' ")

                    Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)

                    If nRet <= 0 Then
                        MainDB.Rollback()
                        CreateCSV.CloseCsv()
                        MessageBox.Show(String.Format(MSG0027E, "�X�P�W���[���}�X�^", "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If

                    '***���[�ɒǉ����Ă���
                    CreateCSV.OutputCsvData(SyoriDate, True)                                                    '������
                    CreateCSV.OutputCsvData(SyoriTime, True)                                                    '�^�C���X�^���v
                    CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace("-", "").Substring(0, 10), True)      '������R�[�h
                    CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace("-", "").Substring(10, 2), True)      '����敛�R�[�h
                    CreateCSV.OutputCsvData(item.SubItems(2).Text, True)                                        '����於�i�����j
                    CreateCSV.OutputCsvData(item.SubItems(3).Text, True)                                        '���ϋ敪
                    CreateCSV.OutputCsvData(item.SubItems(4).Text, True)                                        '�U�փR�[�h
                    CreateCSV.OutputCsvData(item.SubItems(5).Text, True)                                        '��ƃR�[�h
                    CreateCSV.OutputCsvData(item.SubItems(6).Text.Replace("/", ""), True)                       '�U�֓�
                    CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""), True)                       '�萔�������\���
                    CreateCSV.OutputCsvData(item.SubItems(8).Text.Replace(",", ""), True, True)                 '�萔�����z
                    '***���[�ɒǉ����Ă���

                End If

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
            iRet = ExeRepo.ExecReport("KFJP057.EXE", param)

            If iRet <> 0 Then
                '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                Select Case iRet
                    Case -1
                        MessageBox.Show(MSG0226W.Replace("{0}", "�萔�������t���O�ꊇ�X�V�ꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        MessageBox.Show(MSG0004E.Replace("{0}", "�萔�������t���O�ꊇ�X�V�ꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select

                MainDB.Rollback()

                Return
            End If

            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainDB.Commit()

            Me.ListView1.Items.Clear()

        Catch ex As Exception
            MainDB.Rollback()
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)��O�G���[", "���s", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try
    End Sub

#End Region

#Region " �S�I���{�^�� "
    Private Sub btnAllOn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�I��", "����", "")
        End Try
    End Sub

#End Region

#Region " �S�����{�^�� "
    Private Sub btnAllOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�I��", "����", "")
        End Try
    End Sub

#End Region

#Region " �֐� "

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

#End Region

End Class
