Imports System.Text



Public Class ClsRyousyusyosyo

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

    Private Function PrnRyousyusyosyo(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String, ByVal PrinterName As String) As Boolean

        Dim ret As Boolean = False

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

     
        Try
            'INI�t�@�C�����ݒ� ==========
            '����敪�ǉ�
            Dim PrnKbn As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYUSYO")
            If PrnKbn.ToUpper = "ERR" OrElse PrnKbn = "" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎��؏�����敪 ����:KOKUZEI ����:RYOUSYUSYO")
                Return False
            End If

            '������[��
            Dim Gonouran As String = CASTCommon.GetFSKJIni("KOKUZEI", "GONOURAN")
            If Gonouran.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:������[�� ����:KOKUZEI ����:GONOURAN")
                Return False
            End If

            '�̎����P
            Dim Ryousyu1 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU1")
            If Ryousyu1.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����P ����:KOKUZEI ����:RYOUSYU1")
                Return False
            End If

            '�̎����Q
            Dim Ryousyu2 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU2")
            If Ryousyu2.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����Q ����:PRINT ����:RYOUSYU2")
                Return False
            End If

            '�̎����R
            Dim Ryousyu3 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU3")
            If Ryousyu3.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����R ����:KOKUZEI ����:RYOUSYU3")
                Return False
            End If

            '�̎����S
            Dim Ryousyu4 As String = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYU4")
            If Ryousyu4.ToUpper = "ERR" Then
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�̎����S ����:KOKUZEI ����:RYOUSYU4")
                Return False
            End If
            '==============================

            Dim ClsPrnt As New ClsPrnRyousyusyosyo(PrnKbn)

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

            Dim tuuban As Integer = 0
            Dim zei_code As String = ""

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
                            If tuuban <> 0 Then
                                '�ʔԂ́A�Ŗ������ƂɃJ�E���g�B����R�[�h���ς�����ꍇ�A�ʔԂ�1�ɂ���B
                                If zei_code <> KFmt.KOKUZEI_REC2.KZ6.Trim Then
                                    ZumiCnt = "1"
                                End If
                            End If
                            zei_code = KFmt.KOKUZEI_REC2.KZ6.Trim               '����R�[�h��ޔ�
                            tuuban += 1

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

                        ClsPrnt.OutputCsvData(Gonouran)  '������[��

                        '***�X�֔ԍ�***
                        Dim �X�֔ԍ� As String = ""
                        If KFmt.KOKUZEI_REC3.KZ15.Trim.Length < 4 Then
                            �X�֔ԍ� = KFmt.KOKUZEI_REC3.KZ15.Trim
                        Else
                            �X�֔ԍ� = KFmt.KOKUZEI_REC3.KZ15.PadRight(7).Substring(0, 3) & "-"
                            �X�֔ԍ� &= KFmt.KOKUZEI_REC3.KZ15.PadRight(7).Substring(3)
                        End If
                        ClsPrnt.OutputCsvData(�X�֔ԍ�)

                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '�s�s�於
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '�Z���P
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '�Z���Q
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ26.Trim)  '�Z���R
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ27.Trim)  '�����P
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ28.Trim)  '�����Q
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ29.Trim)  '�����R


                        '*** 2010/3/5 FJH �[�ŎҖ��Q���󂾂�����[�Ŏ҂P���Q�ɁB
                        If KFmt.KOKUZEI_REC3.KZ31.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData("")                           '�[�ŎҖ��P
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ30.Trim)  '�[�ŎҖ��Q
                        Else
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ30.Trim)  '�[�ŎҖ��P
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ31.Trim)  '�[�ŎҖ��Q
                        End If

                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ30.Trim)  '�[�ŎҖ��P
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ31.Trim)  '�[�ŎҖ��Q

                        '***���o�l(���Z�@�֖��̂P�A�Q)***
                        Dim ���o�l As String = KFmt.KOKUZEI_REC2.KZ27.Trim & Space(1) & KFmt.KOKUZEI_REC2.KZ28.Trim
                        ClsPrnt.OutputCsvData(���o�l)  '���o�l


                        Dim ���o�l�Z�� As String = Getsitjyusyo(KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim, _
                                                             KFmt.KOKUZEI_REC2.KZ10.Substring(4).Trim)
                        If ���o�l�Z�� = "err" Then
                            MainLOG.Write("�x�X�Z���̎擾�Ɏ��s���܂���", "���s", "���Z�@�փR�[�h�F" & _
                                     KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim & "-" & KFmt.KOKUZEI_REC2.KZ10.Substring(4))
                            '��荇�����i�߂�
                            ���o�l�Z�� = KFmt.KOKUZEI_REC2.KZ24.Trim & KFmt.KOKUZEI_REC2.KZ25.Trim
                        End If

                        ClsPrnt.OutputCsvData(���o�l�Z��)  '���o�l�Z��

                        '***���o�l�d�b�ԍ�(TEL (�O�O�O�O)�O�O�|�O�O�O�O)***
                        Dim ���o�l�d�b�ԍ� As String = ""
                        Dim ���o�lTELNo As String = GetSitTELNo(KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim, _
                                                             KFmt.KOKUZEI_REC2.KZ10.Substring(4).Trim)
                        If ���o�lTELNo = "err" Then
                            MainLOG.Write("�x�X�d�b�ԍ��̎擾�Ɏ��s���܂���", "���s", "���Z�@�փR�[�h�F" & _
                                      KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim & "-" & KFmt.KOKUZEI_REC2.KZ10.Substring(4))
                            '��荇�����i�߂�
                            ���o�l�d�b�ԍ� = "TEL (" & StrConv(KFmt.KOKUZEI_REC2.KZ21.Substring(0, 4), VbStrConv.Wide) & ")"
                            ���o�l�d�b�ԍ� &= StrConv(KFmt.KOKUZEI_REC2.KZ21.Substring(4, 2), VbStrConv.Wide) & "�|"
                            ���o�l�d�b�ԍ� &= StrConv(KFmt.KOKUZEI_REC2.KZ21.Substring(6).Trim, VbStrConv.Wide)
                        Else
                            ���o�l�d�b�ԍ� = "TEL (" & StrConv(���o�lTELNo.Substring(0, 4), VbStrConv.Wide) & ")"
                            ���o�l�d�b�ԍ� = ���o�l�d�b�ԍ� & StrConv(���o�lTELNo.Substring(5).Trim, VbStrConv.Wide)
                        End If

                        ClsPrnt.OutputCsvData(���o�l�d�b�ԍ�)  '���o�l�d�b�ԍ�


                        '***���o�l�x�X***
                        Dim ���o�l�x�X As String = GetSitName(KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim, _
                                                         KFmt.KOKUZEI_REC2.KZ10.Substring(4).Trim)
                        If ���o�l�x�X = "err" Then
                            MainLOG.Write("�x�X���̎擾�Ɏ��s���܂���", "���s", "���Z�@�փR�[�h�F" & _
                                      KFmt.KOKUZEI_REC2.KZ10.Substring(0, 4).Trim & "-" & KFmt.KOKUZEI_REC2.KZ10.Substring(4))
                            '��荇�����i�߂�
                            ���o�l�x�X = ""
                        End If

                        '*** 2010/3/5 FJH  FSKJ�̃��C�A�E�g�ɍ��킹�� �c�戵�X�͕s�v
                        'ClsPrnt.OutputCsvData("�戵���X  " & ���o�l�x�X)  '���o�l�x�X
                        ClsPrnt.OutputCsvData(���o�l�x�X)  '���o�l�x�X
                        '*** 2010/3/5 FJH  FSKJ�̃��C�A�E�g�ɍ��킹�� �c�戵�X�͕s�v

                        ClsPrnt.OutputCsvData(ZumiCnt.ToString)  '�ʔ�
                        ClsPrnt.OutputCsvData(ZumiCnt.ToString)  '�ԍ�
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ4)  '�ŖڃR�[�h
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ7)  '�[���敪

                        '***�[���敪***
                        Select Case KFmt.KOKUZEI_REC1.KZ7.Trim
                            Case "1"
                                ClsPrnt.OutputCsvData("1")  '�\���[���敪
                                ClsPrnt.OutputCsvData("")  '�[���J�i����
                            Case "2"
                                ClsPrnt.OutputCsvData("2")  '�\���[���敪
                                ClsPrnt.OutputCsvData("")  '�[���J�i����
                            Case "3", "4"
                                ClsPrnt.OutputCsvData("3")  '�\���[���敪

                                '"4"��"8"�̎������L�q����
                                If KFmt.KOKUZEI_REC1.KZ7.Trim = "4" Then
                                    ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ8.Trim)  '�[���J�i����
                                Else
                                    ClsPrnt.OutputCsvData("")  '�[���J�i����
                                End If
                            Case "8"
                                ClsPrnt.OutputCsvData("  ")  '�\���[���敪
                                ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ8.Trim)  '�[���J�i����
                            Case Else
                                ClsPrnt.OutputCsvData("")  '�\���[���敪
                                ClsPrnt.OutputCsvData("")  '�[���J�i����
                        End Select

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

                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC1.KZ5.Trim)  '�N�x
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ7.Trim)  '�Ŗ�����
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC2.KZ6.Trim)  '����R�[�h
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ9.Trim)  '�[�t�Ŋz
                        ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ10.Trim)  '�����q��
                        ClsPrnt.OutputCsvData(FuriDate)  '�U�֓�
                        ClsPrnt.OutputCsvData(Ryousyu1)  '�̎����P
                        ClsPrnt.OutputCsvData(Ryousyu2)  '�̎����Q
                        ClsPrnt.OutputCsvData(Ryousyu3)  '�̎����R
                        ClsPrnt.OutputCsvData(Ryousyu4)  '�̎����S


                        '***2010/3/5 FJH �E���̏Z��

                        If KFmt.KOKUZEI_REC3.KZ24.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData("")  '�s�s�於_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '�s�s�於_B �� �Z��1_B��
                        Else
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '�s�s�於_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '�Z���P_B
                        End If

                        If KFmt.KOKUZEI_REC3.KZ29.Trim.Equals("") Then
                            ClsPrnt.OutputCsvData("")  '�Z���Q_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '�Z���Q_B �� �����R_B
                        Else
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '�Z���Q_B
                            ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ29.Trim)  '�����R_B
                        End If
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ23.Trim)  '�s�s�於_B
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ24.Trim)  '�Z���P_B
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ25.Trim)  '�Z���Q_B
                        'ClsPrnt.OutputCsvData(KFmt.KOKUZEI_REC3.KZ29.Trim)  '�����R_B

                        ClsPrnt.OutputCsvData(tuuban, False, True)   '�����ʔ�

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

            'CSV�����H���邽�ߖ����I��CSV�����
            Call ClsPrnt.CloseCsv()
              Select PrnKbn
                Case "1"    '2��
                    Dim ErrMessage As String = ""
                    Dim CSVName As String = ClsPrnt.FileName
                    '�����Ŏ󂯎����CSV�t�@�C��������g���q(TMP)��A4���ʗp��CSV�t�@�C�����쐬����B
                    If Not MakeCsvFile(CSVName, ErrMessage) Then
                        MainLOG.Write("�̎��؏�", "���s", ErrMessage)
                        Return False
                    Else
                        '�g���qTMP�ō쐬����CSV�t�@�C���Ō��t�@�C�����㏑��
                        Dim TMPFile As String = IO.Path.Combine(IO.Path.GetPathRoot(CSVName), _
                                                                   IO.Path.GetFileNameWithoutExtension(CSVName) & ".TMP")
                        IO.File.Copy(TMPFile, CSVName, True)
                        'TMP�t�@�C���̍폜
                        IO.File.Delete(TMPFile)
                    End If
                Case "2"    '3�܁@
            End Select

            ' ���[�o��
            If ClsPrnt.ReportExecute(PrinterName) = True Then
                MainLOG.Write("�̎��؏�", "����")
            Else
                MainLOG.Write("�̎��؏�", "���s", ClsPrnt.ReportMessage)
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

    Private Function Getsitjyusyo(ByVal KinCode As String, ByVal SitCode As String) As String

        Dim ret As String = "err"

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim sql As New StringBuilder(128)

            sql.Append(" SELECT ")
            sql.Append(" KJYU_N ")
            sql.Append(" FROM SITEN_INFOMAST ")
            sql.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
            sql.Append(" AND SIT_NO_N = '" & SitCode & "'")

            If OraReader.DataReader(sql) Then
                ret = OraReader.GetItem("KJYU_N")
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
    Private Function GetSitTELNo(ByVal KinCode As String, ByVal SitCode As String) As String

        Dim ret As String = "err"

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim sql As New StringBuilder(128)

            sql.Append(" SELECT ")
            sql.Append(" DENWA_N ")
            sql.Append(" FROM SITEN_INFOMAST ")
            sql.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
            sql.Append(" AND SIT_NO_N = '" & SitCode & "'")

            If OraReader.DataReader(sql) Then
                ret = OraReader.GetItem("DENWA_N")
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
    '***************************************************************
    '�@�\:�b�r�u�쐬�i�[�t���E�̎��؏��i���Łj�j
    '***************************************************************
    '����:      
    '   CsvFilePath        �b�r�u�t�@�C���p�X
    '�߂�l:
    '    TRUE       ����I��
    '    FALSE      �ُ�I��
    '���l:   
    '***************************************************************
    Private Function MakeCsvFile(ByVal CsvFileName As String, ByRef ErrMessage As String) As Boolean

        Dim ret As Boolean = False

        Dim sr As IO.StreamReader = Nothing
        Dim sw As IO.StreamWriter = Nothing

        Try
            If Not IO.File.Exists(CsvFileName) Then
                ErrMessage = "CSV�t�@�C����������܂���ł����B:" & CsvFileName
                Exit Try
            End If

            'S-JIS
            sr = New IO.StreamReader(CsvFileName, Text.Encoding.GetEncoding(932))

            Dim CsvFileNameTmp As String = IO.Path.Combine(IO.Path.GetPathRoot(CsvFileName), _
                                                           IO.Path.GetFileNameWithoutExtension(CsvFileName) & ".TMP")
            'S-JIS
            sw = New IO.StreamWriter(CsvFileNameTmp, False, Text.Encoding.GetEncoding(932))

            Dim ret_read As Boolean = False
            Dim WriteLine As New Text.StringBuilder(1024)

            '1�s�ڂ͓ǂ݂Ƃ΂�
            Call sr.ReadLine()

            '2�s���Ǎ�
            Do
                Dim ReadLine1 As String = Nothing
                Dim ReadLine2 As String = Nothing

                'A
                ReadLine1 = sr.ReadLine
                'B
                ReadLine2 = sr.ReadLine

                '1�s�ړǍ�
                If ReadLine1 Is Nothing Then

                    '������Δ�����
                    Exit Do
                Else
                    '�\�ʂ̏�
                    WriteLine.Append(ReadLine1 & ",0,0" & vbCrLf)

                    '2�s�ړǍ�
                    If ReadLine2 Is Nothing Then
                        '2�s�ڂ�������Η��ʂ̏�
                        WriteLine.Append(ReadLine1 & ",1,1" & vbCrLf)

                        ReadLine1 = Nothing
                        ReadLine2 = Nothing

                        '�Ǎ��t���O�𗧂ĂĔ�����
                        ret_read = True

                        Exit Do
                    Else
                        '2�s�ڂ��L��
                        '�\�ʂ̉�
                        WriteLine.Append(ReadLine2 & ",0,0" & vbCrLf)
                        '���ʂ̏�
                        WriteLine.Append(ReadLine1 & ",1,1" & vbCrLf)
                        '���ʂ̉�
                        WriteLine.Append(ReadLine2 & ",1,1" & vbCrLf)
                    End If
                End If

                ReadLine1 = Nothing
                ReadLine2 = Nothing

                '�Ǎ��t���O�𗧂Ă�
                ret_read = True
            Loop

            '����ɓǂ߂Ă���Ώ���
            If ret_read = True Then
                sw.Write(WriteLine)
            End If

            sr.Close()
            sr.Dispose()
            sr = Nothing
            sw.Close()
            sw.Dispose()
            sw = Nothing

            CsvFileName = CsvFileNameTmp

            ret = ret_read

        Catch ex As Exception
            ErrMessage = "CSV�t�@�C���Ǎ��Ɏ��s���܂����B:" & ex.Message

            ret = False
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr.Dispose()
                sr = Nothing
            End If
            If Not sw Is Nothing Then
                sw.Close()
                sw.Dispose()
                sw = Nothing
            End If
        End Try

        Return ret

    End Function

End Class
