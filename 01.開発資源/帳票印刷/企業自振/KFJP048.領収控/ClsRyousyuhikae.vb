Imports System.Text

Public Class ClsRyousyuhikae

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
            If Not PrnRyousyuhikae(TorisCode, TorifCode, FuriDate, PrinterName) Then
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

    Private Function PrnRyousyuhikae(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String, ByVal PrinterName As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim ClsPrnt As New ClsPrnRyousyuhikae
            'INI�t�@�C�����ݒ� ==========
            '�����ɖ�
            Dim JikinkoName As String = CASTCommon.GetFSKJIni("KOKUZEI", "KINKONAME")
            If JikinkoName.ToUpper = "ERR" OrElse JikinkoName = "" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�����ɖ� ����:KOKUZEI ����:KINKONAME")
                Return -300
            End If

            '�̎����P
            Dim Ryousyu1 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU1")
            If Ryousyu1.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����P ����:KOKUZEI ����:RYOUSYU1")
                Return -300
            End If

            '�̎����Q
            Dim Ryousyu2 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU2")
            If Ryousyu2.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����Q ����:KOKUZEI ����:RYOUSYU2")
                Return -300
            End If

            '�̎����R
            Dim Ryousyu3 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU3")
            If Ryousyu3.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����R ����:KOKUZEI ����:RYOUSYU3")
                Return -300
            End If

            '�̎����S
            Dim Ryousyu4 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU4")
            If Ryousyu4.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����S ����:KOKUZEI ����:RYOUSYU4")
                Return -300
            End If
            '==============================

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As New StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" DATA_KBN_K ")
            SQL.Append(",FURI_DATA_K ")
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
                        '�U�֍ς݂݈̂�
                        If OraReader.GetItem("FURIKETU_CODE_K").Trim <> "0" Then
                            Exit Select
                        Else
                            ZumiCnt += 1
                        End If

                        Select Case BK_DATA_KBN
                            Case "2", "3", "4"
                            Case Else
                                MainLOG.Write("���׏��̎擾�Ɏ��s���܂���", "���s", errdetail)

                                OraReader.Close()
                                OraReader = Nothing

                                Exit Try
                        End Select

                        KFmt.KOKUZEI_REC3.Data = OraReader.GetItem("FURI_DATA_K")

                        ClsPrnt.OutputCsvData(JikinkoName) '�����ɖ�
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ4) '�ŖڃR�[�h

                        '***�[���敪***
                        Select Case KFmt.KOKUZEI_REC1.KZ7.Trim
                            Case "1"
                                ClsPrnt.OutputCsvData("") '�[���敪�J�i����
                                ClsPrnt.OutputCsvData("1") '��
                            Case "2"
                                ClsPrnt.OutputCsvData("") '�[���敪�J�i����
                                ClsPrnt.OutputCsvData("2") '��
                            Case "3", "4"
                                '"4"��"8"�̎������L�q����
                                If KFmt.KOKUZEI_REC1.KZ7.Trim = "4" Then
                                    ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ8.Trim)  '�[���J�i����
                                Else
                                    ClsPrnt.OutputCsvData("")  '�[���J�i����
                                End If
                                ClsPrnt.OutputCsvData("3") '��
                            Case "8"
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ8.Trim)  '�[���J�i����
                                ClsPrnt.OutputCsvData("  ") '��
                            Case Else
                                ClsPrnt.OutputCsvData("")  '�[���J�i����
                                ClsPrnt.OutputCsvData("  ") '��
                        End Select

                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ5.Trim)  '�N�x

                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ3.Trim) '�Ŗ����R�[�h(�Ǐ��ԍ�)    '���O���[�v������

                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ7.Trim)  '�Ŗ�����
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ6.Trim)  '����R�[�h
                        ClsPrnt.OutputCsvData(FuriDate) '�U�֓�

                        '***�[�Ŋ���***
                        Select Case KFmt.KOKUZEI_REC1.KZ4 '�ŖڃR�[�h
                            Case "020"
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ6.Trim.Substring(0, 2))  '�[�Ŋ���(��)�N
                                ClsPrnt.OutputCsvData("")  '�[�Ŋ���(��)��
                                ClsPrnt.OutputCsvData("")  '�[�Ŋ���(��)��
                                ClsPrnt.OutputCsvData("")  '�[�Ŋ���(��)�N
                                ClsPrnt.OutputCsvData("")  '�[�Ŋ���(��)��
                                ClsPrnt.OutputCsvData("")  '�[�Ŋ���(��)��
                            Case "300"
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ12.Trim.Substring(0, 2))  '�[�Ŋ���(��)�N
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ12.Trim.Substring(2, 2))  '�[�Ŋ���(��)��
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ12.Trim.Substring(4, 2))  '�[�Ŋ���(��)��
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ13.Trim.Substring(0, 2))  '�[�Ŋ���(��)�N
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ13.Trim.Substring(2, 2))  '�[�Ŋ���(��)��
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ13.Trim.Substring(4, 2))  '�[�Ŋ���(��)��
                            Case Else
                                MainLOG.Write("���׏��̎擾�Ɏ��s���܂���", "���s", errdetail)

                                OraReader.Close()
                                OraReader = Nothing

                                Exit Try
                        End Select


                        ClsPrnt.OutputCsvData(ZumiCnt) '���R�[�h�ԍ�

                        '*** FJH KAKI 2010/3/5 ******************************************
                        '*** 4�sMAX��������̎d�l�������Ȃ̂ŕۗ�************************
                        '*** �Ƃ肠�����l�߂܂�******************************************
                        Dim Icount As Integer = 4
                        Dim i As Integer = 0

                        If Not KFmt.KOKUZEI_REC3.KZ23.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '�s�s�於
                            Icount -= 1
                        End If

                        If Not KFmt.KOKUZEI_REC3.KZ24.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '�Z���P
                            Icount -= 1
                        End If

                        If Not KFmt.KOKUZEI_REC3.KZ25.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '�Z���Q
                            Icount -= 1
                        End If

                        If Not KFmt.KOKUZEI_REC3.KZ26.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ26.Trim)  '�Z���R
                            Icount -= 1
                        End If

                        For i = 0 To Icount - 1
                            ClsPrnt.OutputCsvData("")
                        Next
                        '*** FJH KAKI 2010/3/5 ******************************************


                        '''ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '�s�s�於
                        '''ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '�Z���P
                        '''ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '�Z���Q
                        '''ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ26.Trim)  '�Z���R


                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ30.Trim & Space(1) & KFmt.KOKUZEI_REC3.KZ31.Trim)  '�[�ŎҖ��P�A�Q
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ13.Trim.Substring(0, 4)) '�����ԍ��P
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ13.Trim.Substring(4)) '�����ԍ��Q
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ9.Trim)  '�[�t�Ŋz
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ10.Trim)  '�����q��
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ18.Trim.Substring(0, 4)) '�U�֋��Z�@�փR�[�h
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ18.Trim.Substring(4, 3)) '�U�֎x�X�R�[�h

                        '***���Z�@�֐�����***
                        '��荇������
                        ClsPrnt.OutputCsvData(" ") '���Z�@�֐�����
                        '***���Z�@�֐�����***

                        ClsPrnt.OutputCsvData(FuriDate)  '�U�֓�
                        ClsPrnt.OutputCsvData(Ryousyu1)                '�̎����P
                        ClsPrnt.OutputCsvData(Ryousyu2)                 '�̎����Q
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ27.Trim)  '�̎����R
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ28.Trim, False, True)  '�̎����S
                        ClsPrnt.OutputCsvData(Ryousyu3)  '�̎����R
                        ClsPrnt.OutputCsvData(Ryousyu4, False, True)  '�̎����S

                    Case "4" '���ʋ��Z�@�֓X�ܕʃg�[�^�����R�[�h�i�c�j
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
                    Case "5" '���ʋ��Z�@�֕ʃg�[�^�����R�[�h�i�d�j
                        KFmt.KOKUZEI_REC2.Data = OraReader.GetItem("FURI_DATA_K")
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
            If ClsPrnt.ReportExecute(PrinterName) = True Then
                MainLOG.Write("�̎��T", "����")
            Else
                MainLOG.Write("�̎��T", "���s", ClsPrnt.ReportMessage)
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
