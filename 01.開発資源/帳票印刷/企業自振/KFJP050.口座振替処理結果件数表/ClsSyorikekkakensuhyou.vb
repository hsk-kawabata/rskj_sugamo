Imports System.Text

Public Class ClsSyorikekkakensuhyou


    Private MainDB As CASTCommon.MyOracle = Nothing

    Private errdetail As String = ""

    ''�t�@�C���擪���R�[�h�i�`�j
    '<VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=1)
    '<VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪
    '<VBFixedString(19)> Public KZ3 As String        '�_�~�[
    '<VBFixedString(3)> Public KZ4 As String         '�ȖڃR�[�h
    '<VBFixedString(2)> Public KZ5 As String         '�N�x
    '<VBFixedString(2)> Public KZ6 As String         '�ېŔN��
    '<VBFixedString(1)> Public KZ7 As String         '�[���敪
    '<VBFixedString(7)> Public KZ8 As String         '�[���J�i����
    '<VBFixedString(2)> Public KZ9 As String         '����敪
    '<VBFixedString(6)> Public KZ10 As String        '�����N����
    '<VBFixedString(6)> Public KZ11 As String        '�U�֓�
    '<VBFixedString(6)> Public KZ12 As String        '�ېŊ��ԁi���j
    '<VBFixedString(6)> Public KZ13 As String        '�ېŊ��ԁi���j
    '<VBFixedString(325)> Public KZ14 As String      '�_�~�[
    '<VBFixedString(2)> Public KZ15 As String        '�˗��t�@�C���m�n

    ''���ʋ��Z�@�֓X�ܕʖ��̃��R�[�h�i�a�j
    ''���ʋ��Z�@�֓X�ܕʃg�[�^�����R�[�h�i�c�j
    ''���ʋ��Z�@�֕ʃg�[�^�����R�[�h�i�d�j
    '<VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=2)
    '<VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪
    '<VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
    '<VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
    '<VBFixedString(9)> Public KZ5 As String         '�_�~�[
    '<VBFixedString(5)> Public KZ6 As String         '����R�[�h
    '<VBFixedString(10)> Public KZ7 As String        '�Ŗ�����
    '<VBFixedString(7)> Public KZ8 As String         '�_�~�[
    '<VBFixedString(5)> Public KZ9 As String         '�Ŗ����X�֔ԍ�
    '<VBFixedString(7)> Public KZ10 As String        '�戵���Z�@�֔ԍ�
    '<VBFixedString(5)> Public KZ11 As String        '���Z�@�֗X�֔ԍ�
    '<VBFixedString(7)> Public KZ12 As String        '�_�~�[
    '<VBFixedString(6)> Public KZ13 As String        '���t������
    '<VBFixedString(12)> Public KZ14 As String       '���t�����v���z
    '<VBFixedString(6)> Public KZ15 As String        '�U�֔[�t�s�\����
    '<VBFixedString(12)> Public KZ16 As String       '�U�֔[�t�s�\���v
    '<VBFixedString(6)> Public KZ17 As String        '�U�֔[�t����
    '<VBFixedString(12)> Public KZ18 As String       '�U�֔[�t���v���z
    '<VBFixedString(5)> Public KZ19 As String        '�_�~�[
    '<VBFixedString(8)> Public KZ20 As String        '�Ŗ����d�b�ԍ�
    '<VBFixedString(8)> Public KZ21 As String        '���Z�@�֓d�b�ԍ�
    '<VBFixedString(27)> Public KZ22 As String       '�_�~�[
    '<VBFixedString(30)> Public KZ23 As String       '�s�s�於
    '<VBFixedString(30)> Public KZ24 As String       '���ݒn�T
    '<VBFixedString(30)> Public KZ25 As String       '���ݒn�U
    '<VBFixedString(30)> Public KZ26 As String       '����
    '<VBFixedString(30)> Public KZ27 As String       '���Z�@�֖��̇T
    '<VBFixedString(30)> Public KZ28 As String       '���Z�@�֖��̇U
    '<VBFixedString(30)> Public KZ29 As String       '�X�ܖ���
    '<VBFixedString(1)> Public KZ30 As String        '��[�L��
    '<VBFixedString(5)> Public KZ31 As String        '�_�~�[
    '<VBFixedString(2)> Public KZ32 As String        '�˗��t�@�C���m�n

    ''�ʖ��׃��R�[�h�i�b�j
    '<VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=3)
    '<VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪(=91)
    '<VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
    '<VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
    '<VBFixedString(7)> Public KZ5 As String         '�[�ŎҔԍ�
    '<VBFixedString(1)> Public KZ6 As String         '�p���敪
    '<VBFixedString(1)> Public KZ7 As String         '�⊮�\���敪
    '<VBFixedString(1)> Public KZ8 As String         '�U�֌��ʃR�[�h
    '<VBFixedString(10)> Public KZ9 As String        '�[�t�Ŋz
    '<VBFixedString(9)> Public KZ10 As String        '�����q��
    '<VBFixedString(1)> Public KZ11 As String        '�a�����
    '<VBFixedString(7)> Public KZ12 As String        '�����ԍ�
    '<VBFixedString(8)> Public KZ13 As String        '�����ԍ�
    '<VBFixedString(69)> Public KZ14 As String       '�_�~�[
    '<VBFixedString(7)> Public KZ15 As String        '�X�֔ԍ��i7���j
    '<VBFixedString(5)> Public KZ16 As String        '�X�֔ԍ��i5���j
    '<VBFixedString(1)> Public KZ17 As String        '�⊮�\��
    '<VBFixedString(7)> Public KZ18 As String        '�戵���Z�@�֔ԍ�
    '<VBFixedString(7)> Public KZ19 As String        '�_�~�[
    '<VBFixedString(6)> Public KZ20 As String        '�s�O�ǔ�(�[�Ŏ�)
    '<VBFixedString(8)> Public KZ21 As String        '�d�b�ԍ�(�[�Ŏ�)
    '<VBFixedString(2)> Public KZ22 As String        '�_�~�[
    '<VBFixedString(23)> Public KZ23 As String       '�s�s�敪
    '<VBFixedString(23)> Public KZ24 As String       '�Z���T
    '<VBFixedString(23)> Public KZ25 As String       '�Z���U
    '<VBFixedString(23)> Public KZ26 As String       '�Z���V
    '<VBFixedString(23)> Public KZ27 As String       '�����T
    '<VBFixedString(23)> Public KZ28 As String       '�����U
    '<VBFixedString(23)> Public KZ29 As String       '�����V
    '<VBFixedString(23)> Public KZ30 As String       '�[�ŎҖ��T
    '<VBFixedString(23)> Public KZ31 As String       '�[�ŎҖ��U
    '<VBFixedString(5)> Public KZ32 As String        '�[���ԍ�
    '<VBFixedString(3)> Public KZ33 As String        '�����ԍ�
    '<VBFixedString(1)> Public KZ34 As String        '�p���敪
    '<VBFixedString(2)> Public KZ35 As String        '�˗��t�@�C���m�n

    ''�t�@�C�����v���R�[�h�i�e�j
    '<VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=8)
    '<VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪(=91)
    '<VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
    '<VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
    '<VBFixedString(10)> Public KZ5 As String        '�_�~�[
    '<VBFixedString(45)> Public KZ6 As String        '�_�~�[
    '<VBFixedString(6)> Public KZ7 As String         '���t������
    '<VBFixedString(12)> Public KZ8 As String        '���t�����v���z
    '<VBFixedString(6)> Public KZ9 As String         '�U�֔[�t�s�\����
    '<VBFixedString(12)> Public KZ10 As String       '�U�֔[�t�s�\���v���z
    '<VBFixedString(6)> Public KZ11 As String        '�U�֔[�t����
    '<VBFixedString(12)> Public KZ12 As String       '�U�֔[�t���v���z
    '<VBFixedString(264)> Public KZ13 As String      '�_�~�[
    '<VBFixedString(2)> Public KZ14 As String        '�˗��t�@�C���m�n

    ''�t�@�C���G���h���R�[�h�i�f�j
    '<VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=9)
    '<VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪(=91)
    '<VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
    '<VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
    '<VBFixedString(10)> Public KZ5 As String        '�_�~�[
    '<VBFixedString(45)> Public KZ6 As String        '�_�~�[
    '<VBFixedString(6)> Public KZ7 As String         '���t������
    '<VBFixedString(12)> Public KZ8 As String        '���t�����v���z
    '<VBFixedString(6)> Public KZ9 As String         '�U�֔[�t�s�\����
    '<VBFixedString(12)> Public KZ10 As String       '�U�֔[�t�s�\���z
    '<VBFixedString(6)> Public KZ11 As String        '�U�֔[�t����
    '<VBFixedString(12)> Public KZ12 As String       '�U�֔[�t���z
    '<VBFixedString(264)> Public KZ13 As String      '�_�~�[
    '<VBFixedString(2)> Public KZ14 As String        '�˗��t�@�C���m�n

    Public Function Main(ByVal CmdArgs As String) As Boolean

        Dim ret As Boolean = False

        Try

            Dim Cmd As String() = CmdArgs.Split(",")

            Select Case Cmd.Length
                Case 0 To 4
                    MainLOG.Write("Main", "���s", "�����s��")

                    Exit Try
                Case 5 '����
                    MainLOG.Write("Main", "����", CmdArgs)

                Case Else
                    MainLOG.Write("Main", "���s", "�����܂�����")

                    Exit Try
            End Select

            Dim TorisCode As String = Cmd(0)
            Dim TorifCode As String = Cmd(1)
            Dim FuriDate As String = Cmd(2)
            Dim PrinterName As String = Cmd(3)
            Dim PaperName As String = Cmd(4)

            errdetail = "�����R�[�h�F" & TorisCode & "-" & TorifCode & " �U�֓��F" & FuriDate

            MainDB = New CASTCommon.MyOracle

            '���[���
            If Not PrnRyousyusyosyo(TorisCode, TorifCode, FuriDate, PrinterName) Then
                Exit Try
            End If

            MainDB.Close()
            MainDB = Nothing

            ret = True

        Catch ex As Exception
            ret = False
            MainLOG.Write("�z��O�̃G���[���������܂���", "���s", ex.Message & "�F" & ex.StackTrace)
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try

        Return ret

    End Function

    Private Function PrnRyousyusyosyo(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String, ByVal printername As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            'INI�t�@�C�����ݒ� ==========
            '�����ɖ�
            Dim JikinkoName As String = CASTCommon.GetFSKJIni("KOKUZEI", "KINKONAME")
            If JikinkoName.ToUpper = "ERR" OrElse JikinkoName = "" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�����ɖ� ����:PRINT ����:KINKONAME")
                Return -300
            End If
            '==============================
            Dim ClsPrnt As New ClsPrnSyorikekkakensuhyou

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As New StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" DATA_KBN_K ")
            SQL.Append(",FURI_DATA_K ")
            SQL.Append(",FURIKIN_K ")
            SQL.Append(",FURIKETU_CODE_K ")
            SQL.Append(" FROM MEIMAST ")
            SQL.Append(" WHERE TORIS_CODE_K = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & TorifCode & "'")
            SQL.Append(" AND FURI_DATE_K = '" & FuriDate & "'")
            SQL.Append(" ORDER BY RECORD_NO_K")

            Dim name As String = ""

            If Not OraReader.DataReader(SQL) Then
                MainLOG.Write("���׏��̎擾�Ɏ��s���܂���", "���s", errdetail)

                OraReader.Close()
                OraReader = Nothing

                Exit Try
            End If

            name = ClsPrnt.CreateCsvFile()

            Dim KFmt As New CAstFormat.CFormatKokuzei

            Dim RecCnt As Long = 1
            Dim ZumiCnt As Long = 0
            Dim BK_DATA_KBN As String = ""

            '�Ǐ��ԍ�
            Dim KyokusyoBango As String = ""
            Dim KyokusyoBango_bk As String = ""

            '���z�J�E���g�p�ϐ�
            Dim TotalKen As Long = 0
            Dim TotalKin As Long = 0
            Dim FunoKen As Long = 0
            Dim FunoKin As Long = 0
            Dim FuriKen As Long = 0
            Dim FuriKin As Long = 0

            Do While OraReader.EOF = False

                Select Case OraReader.GetItem("DATA_KBN_K").Trim
                    Case "1" '�t�@�C���擪���R�[�h�i�`�j
                        '�擪���R�[�h�`�F�b�N
                        If RecCnt <> 1 Then
                            MainLOG.Write("���׏��̎擾�Ɏ��s���܂���", "���s", errdetail)

                            OraReader.Close()
                            OraReader = Nothing

                            Exit Try
                        End If
                        KFmt.KOKUZEI_REC1.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "2"  '���ʋ��Z�@�֓X�ܕʖ��̃��R�[�h�i�a�j
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "3" '�ʖ��׃��R�[�h�i�b�j
                        KFmt.KOKUZEI_REC3.Data = OraReader.GetItem("FURI_DATA_K")

                        If OraReader.GetItem("FURIKETU_CODE_K") <> 0 Then
                            TotalKen += 1
                            TotalKin += OraReader.GetInt64("FURIKIN_K")
                            FunoKen += 1
                            FunoKin += OraReader.GetInt64("FURIKIN_K")
                        Else
                            TotalKen += 1
                            TotalKin += OraReader.GetInt64("FURIKIN_K")
                            FuriKen += 1
                            FuriKin += OraReader.GetInt64("FURIKIN_K")
                        End If

                    Case "4" '���ʋ��Z�@�֓X�ܕʃg�[�^�����R�[�h�i�c�j
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "5" '���ʋ��Z�@�֕ʃg�[�^�����R�[�h�i�d�j
                        Select Case BK_DATA_KBN
                            Case "2", "3", "4"
                            Case Else
                                MainLOG.Write("���׏��̎擾�Ɏ��s���܂���", "���s", errdetail)

                                OraReader.Close()
                                OraReader = Nothing

                                Exit Try
                        End Select
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")

                        '�f�[�^�o��
                        ClsPrnt.OutputCsvData(Now.ToString("yyyyMMdd")) '������
                        ClsPrnt.OutputCsvData("�������ŋǑ����� �����Ǘ�����") '����於
                        ClsPrnt.OutputCsvData("�d �q �v �Z �� �Q �W") '�����S������
                        ClsPrnt.OutputCsvData(JikinkoName) '�����ɖ�
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ5.Trim)  '�N�x
                        '***�[���敪***
                        Select Case KFmt.KOKUZEI_REC1.KZ7.Trim
                            Case "1"
                                ClsPrnt.OutputCsvData("1")  '��
                            Case "2"
                                ClsPrnt.OutputCsvData("2")  '��
                            Case "3", "4"
                                ClsPrnt.OutputCsvData("3")  '��
                            Case "8"
                                ClsPrnt.OutputCsvData("3")  '��
                        End Select
                        ClsPrnt.OutputCsvData(FuriDate) '�U�֓�
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ7.Trim) '�Ŗ�����
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ3.Trim & "-" & KFmt.KOKUZEI_REC2.KZ6.Trim) '�Ǐ��ԍ���
                        ClsPrnt.OutputCsvData(TotalKen.ToString) '���t������
                        ClsPrnt.OutputCsvData(TotalKin.ToString) '���t�����v���z
                        ClsPrnt.OutputCsvData(FunoKen) '�U�֔[�t�s�\����
                        ClsPrnt.OutputCsvData(FunoKin) '�U�֔[�t�s�\���v���z
                        ClsPrnt.OutputCsvData(FuriKen) '�U�֔[�t����
                        ClsPrnt.OutputCsvData(FuriKin, False, True) '�U�֔[�t���v���z

                        TotalKen = 0
                        TotalKin = 0
                        FunoKen = 0
                        FunoKin = 0
                        FuriKen = 0
                        FuriKin = 0

                    Case "8" '�t�@�C�����v���R�[�h�i�e�j
                        KFmt.KOKUZEI_REC8.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "9" '�t�@�C���G���h���R�[�h�i�f�j
                        KFmt.KOKUZEI_REC9.Data = OraReader.GetItem("FURI_DATA_K")
                    Case Else
                        MainLOG.Write("���׏��̎擾�Ɏ��s���܂���", "���s", errdetail)

                        OraReader.Close()
                        OraReader = Nothing

                        Exit Try
                End Select

                BK_DATA_KBN = OraReader.GetItem("DATA_KBN_K").Trim

                RecCnt += 1

                OraReader.NextRead()
            Loop

            ' ���[�o��
            If ClsPrnt.ReportExecute(printername) = True Then
                MainLOG.Write("�����U�֏������ʌ����\", "����")
            Else
                MainLOG.Write("�����U�֏������ʌ����\", "���s", ClsPrnt.ReportMessage)
                Return False
            End If

            If Not ClsPrnt.HostCsvName Is Nothing AndAlso ClsPrnt.HostCsvName <> "" Then
                Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                DestName &= ClsPrnt.HostCsvName
                IO.File.Copy(ClsPrnt.FileName, DestName, True)
            End If

            ret = True

        Catch ex As Exception
            ret = False
            MainLOG.Write("�z��O�̃G���[���������܂���", "���s", ex.Message & "�F" & ex.StackTrace & Space(1) & errdetail)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function

    Private Function GetSitName(ByVal KinCode As String, ByVal SitCode As String) As String

        Dim ret As String = "err"

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim sql As New StringBuilder(128)

            sql.Append(" SELECT ")
            sql.Append(" SIT_NNAME_N ")
            sql.Append(" FROM TENMAST ")
            sql.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
            sql.Append(" AND SIT_NO_N = '" & SitCode & "'")

            If OraReader.DataReader(sql) Then
                ret = OraReader.GetItem("SIT_NNAME_N")
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            ret = False
            MainLOG.Write("�z��O�̃G���[���������܂���", "���s", ex.Message & "�F" & ex.StackTrace & Space(1) & errdetail)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
End Class
