Imports System.Globalization
Imports System.Text

' �������ʊm�F�\
Class ClsPrnSyorikekka
    Inherits CAstReports.ClsReportBase

    Public CsvData(15) As String
    Public CENTER As String

    Sub New(ByVal strCenter As String)
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP008"

        ' ��`�̖��Z�b�g
        '2010/12/24 �M�g�Ή� �M�g�̏ꍇ�͒��[��`�ύX
        If strCenter = "0" Then
            ReportBaseName = "KFJP008_�������ʊm�F�\(�z�M�f�[�^�쐬�E�g������).rpd"
        Else
            ReportBaseName = "KFJP008_�������ʊm�F�\(�z�M�f�[�^�쐬).rpd"
        End If

        'If gstrCENTER = "0" Then
        '    ReportBaseName = "KFJP008_�������ʊm�F�\(�z�M�f�[�^�쐬�E�g������).rpd"
        'Else
        '    ReportBaseName = "KFJP008_�������ʊm�F�\(�z�M�f�[�^�쐬).rpd"
        'End If

        ' ������
        CsvData(0) = "00010101"                         '������
        CsvData(1) = "00010101"                         '�^�C���X�^���v
        CsvData(2) = "00010101"                         ' �U�֓�
        CsvData(3) = "0"                                ' ������R�[�h
        CsvData(4) = "0"                                ' ����敛�R�[�h
        CsvData(5) = ""                                 ' ����於
        CsvData(6) = "0"                                ' �ϑ��҃R�[�h
        CsvData(7) = ""                                 ' ���o���敪
        CsvData(8) = "0"                                ' �U�փR�[�h
        CsvData(9) = "0"                                ' ��ƃR�[�h
        CsvData(10) = "0"                                ' �˗�����
        CsvData(11) = "0"                                ' �˗����z
        CsvData(12) = "0"                                ' ��������
        CsvData(13) = "0"                                ' �������z
        CsvData(14) = ""                                ' ���l
        CsvData(15) = "0"                                ' �`�����v

        CENTER = strCenter

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' �^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("����於")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("���o���敪")
        CSVObject.Output("�U�փR�[�h")
        CSVObject.Output("��ƃR�[�h")
        CSVObject.Output("�˗�����")
        CSVObject.Output("�˗����z")
        CSVObject.Output("��������")
        CSVObject.Output("�������z")
        CSVObject.Output("���l")
        CSVObject.Output("�`�����v", False, True)

        Return file

    End Function

    '
    ' �@�\�@ �F �������ʊm�F�\�b�r�u���o�͂���
    '
    ' ���l�@ �F 
    '
    Public Function OutputCSVKekka(ByVal ary As ArrayList, ByVal densoKen As Integer) As Boolean

        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim sql As StringBuilder

        Try

            Dim i As Integer = 0

            For Each item As String() In ary

                Dim strTakoKbn As String
                '2013/09/20 saitou ���S�M�� MODIFY ----------------------------------------------->>>>
                Dim strFmtKbn As String = String.Empty
                '2013/09/20 saitou ���S�M�� MODIFY -----------------------------------------------<<<<

                oraReader = New CASTCommon.MyOracleReader(oraDB)
                sql = New StringBuilder(128)
                ''-----------------------------------------
                ''�ΏۃX�P�W���[������
                ''-----------------------------------------
                '2013/09/20 saitou ���S�M�� MODIFY ----------------------------------------------->>>>
                '�X���[�G�X�p�Ƀt�H�[�}�b�g�敪���擾����
                sql.Append("SELECT TAKO_KBN_T, FMT_KBN_T FROM SCHMAST,TORIMAST WHERE FURI_DATE_S = '" & item(2) & "'")
                'sql.Append("SELECT TAKO_KBN_T FROM SCHMAST,TORIMAST WHERE FURI_DATE_S = '" & item(2) & "'")
                '2013/09/20 saitou ���S�M�� MODIFY -----------------------------------------------<<<<
                sql.Append(" AND TORIS_CODE_S = '" & item(3) & "' AND TORIF_CODE_S = '" & item(4) & "'")
                sql.Append(" AND TORIS_CODE_S = TORIS_CODE_T AND TORIF_CODE_S = TORIF_CODE_T")

                If oraReader.DataReader(sql) = True Then
                    strTakoKbn = oraReader.GetString("TAKO_KBN_T")
                    '2013/09/20 saitou ���S�M�� MODIFY ----------------------------------------------->>>>
                    strFmtKbn = oraReader.GetString("FMT_KBN_T")
                    '2013/09/20 saitou ���S�M�� MODIFY -----------------------------------------------<<<<
                Else
                    MainLOG.Write("�������ʊm�F�\CSV�쐬", "���s", "���s�敪�擾")
                    Return False
                End If

                oraReader.Close()

                oraReader = New CASTCommon.MyOracleReader(oraDB)
                sql = New StringBuilder(128)

                ''-----------------------------------------
                ''���s�f�[�^����
                ''-----------------------------------------
                Dim takoken As Long
                Dim takokin As Long

                '2017/01/20 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                '�W����(�X���[�G�X���ϖ���)�ł����s�X�P�W���[���}�X�^���쐬���邪�A���쐬�̏����͊��S�łŖ��Ȃ��B
                '����̂��߂ɁA�t�H�[�}�b�g�敪21(���O)�������ɒǉ����Ă����B
                If strFmtKbn.Equals("20") = True OrElse strFmtKbn.Equals("21") = True Then
                    ''2013/09/20 saitou ���S�M�� MODIFY ----------------------------------------------->>>>
                    ''�X���[�G�X�ϑ��҂��܂܂�Ă���ۂɁA�u���쐬����v�ƕ\������Ă��܂��̂����
                    ''���S�̃X���[�G�X�͑��s�X�P�W���[�������݂��Ȃ����߁A���������ƈ˗���������΂ɍ���Ȃ�
                    'If strFmtKbn.Equals("20") = True Then
                    '2017/01/20 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END

                    With sql
                        .Append("select TAKOU_FLG_S from SCHMAST")
                        .Append(" where TORIS_CODE_S = '" & item(3) & "'")
                        .Append(" and TORIF_CODE_S = '" & item(4) & "'")
                        .Append(" and FURI_DATE_S = '" & item(2) & "'")
                    End With

                    Dim strTakouFlg As String = String.Empty

                    If oraReader.DataReader(sql) = True Then
                        strTakouFlg = oraReader.GetString("TAKOU_FLG_S")
                    Else
                        '�X�P�W���[���擾���s�i���̎��_�ŃX�P�W���[�������͂��������j
                        MainLOG.Write("�������ʊm�F�\CSV�쐬", "���s", "�r�r�r�X�P�W���[������")
                        Return False
                    End If

                    oraReader.Close()

                    If strTakouFlg.Equals("0") = True Then
                        '�X���[�G�X�ϑ��҂͂r�r�r���s�f�[�^���쐬�����瑼�s�t���O��1�ɂȂ邽��
                        '�X���[�G�X�Ŗ{���ɖ��쐬�Ȃ̂́A�X���[�G�X�ϑ��҂Ȃ̂ɑ��s�f�[�^���쐬���Ă��Ȃ��Ƃ�
                        item(14) = "�r�r�r���쐬����"
                    ElseIf strTakouFlg.Equals("1") = True Then
                        item(14) = "�r�r�r����"
                    Else
                        item(14) = ""
                    End If

                Else
                    '�X���[�G�X�ȊO�͊����̏���
                    If strTakoKbn = 1 Then '���s�쐬�Ώ�
                        sql.Append("SELECT SUM(SYORI_KEN_U) AS GET_SYORI_KEN,SUM(SYORI_KIN_U) AS GET_SYORI_KIN FROM TAKOSCHMAST")
                        sql.Append(" WHERE TORIS_CODE_U = '" & item(3) & "' AND TORIF_CODE_U = '" & item(4) & "' AND FURI_DATE_U ='" & item(2) & "'")

                        If oraReader.DataReader(sql) = True Then
                            takoken = oraReader.GetInt64("GET_SYORI_KEN")
                            takokin = oraReader.GetInt64("GET_SYORI_KIN")
                        Else
                            MainLOG.Write("�������ʊm�F�\CSV�쐬", "���s", "���s���������擾")
                            Return False
                        End If

                        oraReader.Close()

                        '���l�̐ݒ�
                        If takoken > 0 Then '���s�쐬�Ώۂő��s�쐬�ς�
                            If item(10) <> (item(12) + takoken) Then '���z0�~���U�֌��ʂɕs�\�R�[�h���ݒ肳��Ă�����̂����݂���
                                item(14) = "���s���쐬����"
                            Else
                                item(14) = "���s����"
                            End If
                        Else                '���s�쐬�Ώۂő��s���쐬
                            If item(10) <> (item(12) + takoken) Then '���z0�~���U�֌��ʂɕs�\�R�[�h���ݒ肳��Ă�����̂����݂���
                                item(14) = "���쐬����"
                            End If
                        End If
                    Else
                        If item(10) <> item(12) Then                '���z0�~���U�֌��ʂɕs�\�R�[�h���ݒ肳��Ă�����̂����݂���
                            item(14) = "���쐬����"
                        End If
                    End If

                End If

                'If strTakoKbn = 1 Then '���s�쐬�Ώ�
                '    sql.Append("SELECT SUM(SYORI_KEN_U) AS GET_SYORI_KEN,SUM(SYORI_KIN_U) AS GET_SYORI_KIN FROM TAKOSCHMAST")
                '    sql.Append(" WHERE TORIS_CODE_U = '" & item(3) & "' AND TORIF_CODE_U = '" & item(4) & "' AND FURI_DATE_U ='" & item(2) & "'")

                '    If oraReader.DataReader(sql) = True Then
                '        takoken = oraReader.GetInt64("GET_SYORI_KEN")
                '        takokin = oraReader.GetInt64("GET_SYORI_KIN")
                '    Else
                '        MainLOG.Write("�������ʊm�F�\CSV�쐬", "���s", "���s���������擾")
                '        Return False
                '    End If

                '    oraReader.Close()

                '    '���l�̐ݒ�
                '    If takoken > 0 Then '���s�쐬�Ώۂő��s�쐬�ς�
                '        If item(10) <> (item(12) + takoken) Then '���z0�~���U�֌��ʂɕs�\�R�[�h���ݒ肳��Ă�����̂����݂���
                '            item(14) = "���s���쐬����"
                '        Else
                '            item(14) = "���s����"
                '        End If
                '    Else                '���s�쐬�Ώۂő��s���쐬
                '        If item(10) <> (item(12) + takoken) Then '���z0�~���U�֌��ʂɕs�\�R�[�h���ݒ肳��Ă�����̂����݂���
                '            item(14) = "���쐬����"
                '        End If
                '    End If
                'Else
                '    If item(10) <> item(12) Then                '���z0�~���U�֌��ʂɕs�\�R�[�h���ݒ肳��Ă�����̂����݂���
                '        item(14) = "���쐬����"
                '    End If
                'End If
                '2013/09/20 saitou ���S�M�� MODIFY -----------------------------------------------<<<<

                CsvData(0) = item(0)
                CsvData(1) = item(1)
                CsvData(2) = item(2)
                CsvData(3) = item(3)
                CsvData(4) = item(4)
                CsvData(5) = item(5)
                CsvData(6) = item(6)
                Select Case item(7)
                    Case "1"
                        CsvData(7) = "����"
                    Case "9"
                        CsvData(7) = "�o��"
                End Select
                CsvData(8) = item(8)
                CsvData(9) = item(9)
                CsvData(10) = item(10)
                CsvData(11) = item(11)
                CsvData(12) = item(12)
                CsvData(13) = item(13)
                CsvData(14) = item(14)
                CsvData(15) = densoKen      '�`������
                '2010/12/24 �M�g�Ή� �M�g�̏ꍇ�͑��M�敪�����ڂɒǉ�
                If CENTER = "0" Then
                    ' 2015/12/14 �^�X�N�j���� CHG �yPG�zUI_B-14-04(RSV2�Ή�) -------------------- START
                    'ReDim Preserve CsvData(16)
                    'CsvData(16) = item(15)
                    ReDim Preserve CsvData(19)
                    CsvData(16) = item(15)
                    CsvData(17) = item(16)
                    CsvData(18) = item(17)
                    CsvData(19) = item(18)
                    ' 2015/12/14 �^�X�N�j���� CHG �yPG�zUI_B-14-04(RSV2�Ή�) -------------------- END
                End If
                'If gstrCENTER = "0" Then
                '    ReDim Preserve CsvData(16)
                '    CsvData(16) = item(15)
                'End If

                CSVObject.Output(CsvData)
            Next

            Return True

        Catch ex As Exception
            MainLOG.Write("�������ʊm�F�\CSV�쐬", "���s", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not oraDB Is Nothing Then oraDB.Close()
        End Try
    End Function

    'Public Overloads Overrides Function ReportExecute() As Boolean
    '    Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
    'End Function
End Class
