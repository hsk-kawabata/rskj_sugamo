Imports System

'�������ʊm�F�\
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    '2010/10/04.Sakon�@�E�v�ǉ��̂��߁A�C���f�b�N�X�ύX +++++++++++++++++++++++++
    Public CsvData(15) As String
    '' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� -------------------->
    '' �U�փR�[�h�A��ƃR�[�h�ǉ��̂��߁ACsvData�̃C���f�b�N�X��12��14�֕ύX
    ''Public CsvData(12) As String
    'Public CsvData(14) As String
    '' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� --------------------<
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    Sub New()
        'CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP001"

        '��`�̖��Z�b�g
        ReportBaseName = "KFJP001_�������ʊm�F�\(����).rpd"

        CsvData(0) = "00010101"                                     '�U�֓�
        CsvData(1) = CASTCommon.Calendar.Now.ToString("yyyyMMdd")   '������
        CsvData(2) = CASTCommon.Calendar.Now.ToString("HHmmss")     '�^�C���X�^���v
        CsvData(3) = "0"                                            '������R�[�h
        CsvData(4) = "0"                                            '����敛�R�[�h
        CsvData(5) = ""                                             '����於
        CsvData(6) = ""                                             '�ϑ��҃R�[�h
        CsvData(7) = ""                                             '�}��
        CsvData(8) = "0"                                            '�˗�����
        CsvData(9) = "0"                                            '�˗����z
        CsvData(10) = "0"                                           '��������
        CsvData(11) = "0"                                           '�������z
        CsvData(12) = ""                                            '���l
        ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� -------------------->
        CsvData(13) = ""                                            '�U�փR�[�h
        CsvData(14) = ""                                            '��ƃR�[�h
        ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� --------------------<

        '2010/10/04.Sakon�@�E�v�ǉ� ++++++++++++++++++++++++++++++++++++++++++
        CsvData(15) = ""
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        '' �^�C�g���s
        'CSVObject.Output("�U�֓�")
        'CSVObject.Output("������")
        'CSVObject.Output("�^�C���X�^���v")
        'CSVObject.Output("������R�[�h")
        'CSVObject.Output("����敛�R�[�h")
        'CSVObject.Output("����於")
        'CSVObject.Output("�ϑ��҃R�[�h")
        'CSVObject.Output("�}��")
        'CSVObject.Output("�˗�����")
        'CSVObject.Output("�˗����z")
        'CSVObject.Output("��������")
        'CSVObject.Output("�������z")
        'CSVObject.Output("���l", True, True)

        Return file
    End Function

    Public Function OutputCSVKekkaSysError(ByVal fsyoriKbn As String, _
                        ByVal aToriS As String, ByVal aToriF As String, _
                        ByVal aTuuban As Integer, ByVal aInfile As String, _
                        ByVal aMSG As String, ByVal oraDB As CASTCommon.MyOracle) As Boolean
        '------------------------------------------------------
        ' �b�r�u�t�@�C���쐬(�G���[��)
        '------------------------------------------------------
        Dim Comm As New CAstBatch.CommData(oraDB)

        Try
            Call Comm.GetTORIMAST(aToriS, aToriF)

            'Itakusyamei = "ERROR"
            CreateCsvFile()

            CsvData(0) = ""
            CsvData(3) = aToriS
            CsvData(4) = aToriF
            
            '�O�s��
            If Not Comm.INFOToriMast.ITAKU_NNAME_T Is Nothing Then
                CsvData(5) = Comm.INFOToriMast.ITAKU_NNAME_T.Trim   '�ϑ��Җ�����
            Else
                CsvData(5) = ""                                     '�����}�X�^�ɑ��݂��Ȃ��ꍇ
            End If

            If Not Comm.INFOToriMast.ITAKU_CODE_T Is Nothing Then
                CsvData(6) = Comm.INFOToriMast.ITAKU_CODE_T         '�ϑ��҃R�[�h�ǉ�
            Else
                CsvData(6) = ""                                     '�����}�X�^�ɑ��݂��Ȃ��ꍇ
            End If

            ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
            '�}�̖����e�L�X�g����擾����

            ' 2016/10/10 ���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(UI_12-1-7) -------------- START
            '�}�̂��t�@�C�����O�ŁA�ϑ��҃R�[�h��ALL1�̏ꍇ�w�t�@�C�����O�x�AALL2�̏ꍇ�wCP�x��\��������
            If Comm.INFOToriMast.BAITAI_CODE_T = "09" Then

                If Comm.INFOToriMast.ITAKU_CODE_T = "2222222222" Then
                    CsvData(7) = "CP"
                Else
                    CsvData(7) = "̧��ݸ�"
                End If
            Else
                CsvData(7) = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_�}�̃R�[�h.TXT"), _
                                           Comm.INFOToriMast.BAITAI_CODE_T)
            End If
            ' 2016/10/10 ���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(UI_12-1-7) -------------- END

            'Select Case Comm.INFOToriMast.BAITAI_CODE_T             '�}��
            '    Case "00"
            '        CsvData(7) = "�`��"
            '    Case "01"
            '        CsvData(7) = "FD3.5"
            '    Case "04"
            '        CsvData(7) = "�˗���"
            '    Case "05"
            '        CsvData(7) = "MT"
            '    Case "06"
            '        CsvData(7) = "CMT"
            '    Case "07"
            '        CsvData(7) = "�w�Z���U"
            '    Case "09"
            '        CsvData(7) = "�`�["
            '        '2012/06/30 �W���Ł@WEB�`���Ή�
            '    Case "10"
            '        CsvData(7) = "WEB�`��"
            '        '2012/06/30 �W���Ł@WEB�`���Ή�
            '    Case "11"
            '        CsvData(7) = "DVD-RAM"
            '    Case "12"
            '        CsvData(7) = "���̑�"
            '    Case "13"
            '        CsvData(7) = "���̑�"
            '    Case "14"
            '        CsvData(7) = "���̑�"
            '        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD -------------------------------------------------->>>>
            '    Case "15"
            '        CsvData(7) = "���̑�"
            '        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD --------------------------------------------------<<<<
            '    Case Else
            '        CsvData(7) = ""
            'End Select
            ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

            CsvData(8) = ""
            CsvData(9) = ""
            CsvData(10) = ""
            '2010/02/04 �ǉ�
            CsvData(11) = ""                                           '�������z
            '===============

            ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� -------------------->
            If Not Comm.INFOToriMast.FURI_CODE_T Is Nothing Then
                CsvData(13) = Comm.INFOToriMast.FURI_CODE_T
            Else
                CsvData(13) = ""
            End If
            If Not Comm.INFOToriMast.KIGYO_CODE_T Is Nothing Then
                CsvData(14) = Comm.INFOToriMast.KIGYO_CODE_T
            Else
                CsvData(14) = ""
            End If
            ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� --------------------<

            '2010/10/04.Sakon�@�E�v�ǉ� ++++++++++++++++++++++++++++++++++++++++++
            Select Case Comm.INFOToriMast.TEKIYOU_KBN_T
                Case "0"
                    CsvData(15) = Comm.INFOToriMast.KTEKIYOU_T
                Case "1"
                    CsvData(15) = Comm.INFOToriMast.NTEKIYOU_T
                Case "2"
                    '2010/10/19 �E�v�����C��--------------------------------------
                    'CsvData(15) = "�f�[�^�E�v"
                    CsvData(15) = "�ϓE�v(�x�X��)"
                Case Else
                    'CsvData(15) = ""
                    CsvData(15) = "�ϓE�v(�\��)"
                    '-------------------------------------------------------------
            End Select
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            If CASTCommon.CAInt32(aToriS) <> 0 Then
                CSVObject.Output(CsvData)
            End If

            '�P�s��
            CsvData(3) = ""
            CsvData(4) = ""
            CsvData(5) = "�����ł��܂���ł��� "
            CsvData(6) = ""
            CsvData(7) = ""
            'CsvData(11) = "1"
            ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� -------------------->
            CsvData(13) = ""
            CsvData(14) = ""
            ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� --------------------<

            '2010/10/04.Sakon�@�E�v�ǉ� ++++++++++++++++++++++++++++++++++++++++++
            CsvData(15) = ""
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            CSVObject.Output(CsvData)

            '�Q�s��
            'CsvData(2) = "0"
            'CsvData(3) = "0"
            CsvData(5) = "�W���u�ʔԁF" & aTuuban.ToString
            CSVObject.Output(CsvData)

            '�R�s��
            CsvData(5) = "�t�@�C�����F" & aInfile
            CSVObject.Output(CsvData)

            '�S�s��
            CsvData(5) = """���e:" & aMSG & """"
            CSVObject.Output(CsvData)

            If ReportExecute() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write("(�������ʊm�F�\���)", "���s", ex.Message)
        End Try
    End Function

    '
    ' �@�\�@ �F �������ʊm�F�\�b�r�u���o�͂���
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCSVKekka(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter    '�p�����[�^���
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '�������
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast          '�˗����׏��

        Try
            '------------------------------------------------------
            ' �b�r�u�t�@�C���쐬(����I����)
            '------------------------------------------------------
            CsvData(0) = InfoPara.FURI_DATE                                     '�U�֓�
            CsvData(3) = InfoTori.TORIS_CODE_T                                  '������R�[�h
            CsvData(4) = InfoTori.TORIF_CODE_T                                  '����敛�R�[�h
            CsvData(5) = InfoTori.ITAKU_NNAME_T                                 '����於
            CsvData(6) = InfoMei.ITAKU_CODE                                     '�ϑ��҃R�[�h

            ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
            '�}�̖����e�L�X�g����擾����

            If InfoTori.BAITAI_CODE_T = "09" Then

                If InfoTori.ITAKU_CODE_T = "2222222222" Then
                    CsvData(7) = "CP"
                Else
                    CsvData(7) = "̧��ݸ�"
                End If
            Else
                CsvData(7) = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_�}�̃R�[�h.TXT"), _
                                           InfoTori.BAITAI_CODE_T)
            End If

            'CsvData(7) = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_�}�̃R�[�h.TXT"), _
            '                                           InfoTori.BAITAI_CODE_T)
            'Select Case InfoTori.BAITAI_CODE_T                                  '�}��
            '    Case "00"
            '        CsvData(7) = "�`��"
            '    Case "01"
            '        CsvData(7) = "FD3.5"
            '    Case "04"
            '        CsvData(7) = "�˗���"
            '    Case "05"
            '        CsvData(7) = "MT"
            '    Case "06"
            '        CsvData(7) = "CMT"
            '    Case "07"
            '        CsvData(7) = "�w�Z���U"
            '    Case "09"
            '        CsvData(7) = "�`�["
            '        '2012/06/30 �W���Ł@WEB�`���Ή�
            '    Case "10"
            '        CsvData(7) = "WEB�`��"
            '    Case "11"
            '        CsvData(7) = "DVD-RAM"
            '    Case "12"
            '        CsvData(7) = "���̑�"
            '    Case "13"
            '        CsvData(7) = "���̑�"
            '    Case "14"
            '        CsvData(7) = "���̑�"
            '        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD -------------------------------------------------->>>>
            '    Case "15"
            '        CsvData(7) = "���̑�"
            '        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD --------------------------------------------------<<<<
            '    Case Else
            '        CsvData(7) = ""
            'End Select
            ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

            CsvData(8) = InfoMei.TOTAL_IRAI_KEN.ToString                        '�˗�����
            CsvData(9) = InfoMei.TOTAL_IRAI_KIN.ToString                        '�˗����z
            '2010.03.05 NHK��0�~�𔲂� start
            If InfoTori.FMT_KBN_T = "01" Then
                CsvData(10) = (InfoMei.TOTAL_KEN2 - InfoMei.TOTAL_IJO_KEN).ToString  '��������
            Else
                CsvData(10) = (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN).ToString  '��������
            End If
            '2010.03.05 NHK��0�~�𔲂� end
            CsvData(11) = (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN).ToString  '�������z
            CsvData(12) = ""                                                    '���l

            '�G���[���b�Z�[�W�͔��l���ɏo�͂���
            If aReadFmt.InfoMeisaiMast.DUPLICATE_KBN = "" Then
                '2010.03.05 NHK��0�~�𔲂� start
                If InfoTori.FMT_KBN_T = "01" Then
                    If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN2 - InfoMei.TOTAL_IJO_KEN) Then
                        CsvData(12) = "�����ُ�"
                    ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                        CsvData(12) = "���z�ُ�"
                    End If
                Else
                    If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN) Then
                        CsvData(12) = "�����ُ�"
                    ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                        CsvData(12) = "���z�ُ�"
                    End If
                End If
                '2010.03.05 NHK��0�~�𔲂� end
            Else
                CsvData(12) = "��d��������"
            End If

            If aReadFmt.TAKOU_ON = True Then
                CsvData(12) += " ���s�L"
            End If

            ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� -------------------->
            CsvData(13) = InfoTori.FURI_CODE_T          '�U�փR�[�h
            CsvData(14) = InfoTori.KIGYO_CODE_T         '��ƃR�[�h
            ' 2010/09/13 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� --------------------<

            '2010/10/04.Sakon�@�E�v�ǉ� ++++++++++++++++++++++++++++++++++++++++++
            Select Case InfoTori.TEKIYOU_KBN_T
                Case "0"
                    CsvData(15) = InfoTori.KTEKIYOU_T
                Case "1"
                    CsvData(15) = InfoTori.NTEKIYOU_T
                Case "2"
                    '2010/10/19 �E�v�����C��--------------------------------------
                    'CsvData(15) = "�f�[�^�E�v"
                    CsvData(15) = "�ϓE�v(�x�X��)"
                Case Else
                    'CsvData(15) = ""
                    CsvData(15) = "�ϓE�v(�\��)"
                    '-------------------------------------------------------------
            End Select
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            CSVObject.Output(CsvData)

        Catch ex As Exception
            BatchLog.Write("(�������ʊm�F�\���)", "���s", ex.Message)
        End Try
    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean
        Try
            '2010/02/03 �ʏ�g�p����v�����^�ɂ��� ===
            'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
            Return MyBase.ReportExecute()
            '=======================================================
        Catch ex As Exception
            BatchLog.Write("(�������ʊm�F�\���)", "���", ex.Message)
        End Try
    End Function
End Class
