Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic



' ��t���ו\
Class ClsPrnUketukeMeisai
    Inherits CAstReports.ClsReportBase

    Private ToriSCode As String         '������R�[�h
    Private ToriFCode As String         '����敛�R�[�h
    Private FuriDate As String          '�U�֓�
    Private Tuuban As Long = 0          '�ʔ�

    Private DataOK As Boolean = False

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP003"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP003_��t���ו\.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        '' �^�C�g���s
        'CSVObject.Output("         ")
        'CSVObject.Output("������")
        'CSVObject.Output("�U�֓�")
        'CSVObject.Output("������R�[�h")
        'CSVObject.Output("����敛�R�[�h")
        'CSVObject.Output("����於")
        'CSVObject.Output("�戵���Z�@�փR�[�h")
        'CSVObject.Output("�戵���Z�@�֖�")
        'CSVObject.Output("�戵�X�܃R�[�h")
        'CSVObject.Output("�戵�x�X��")
        'CSVObject.Output("�����R�[�h")
        'CSVObject.Output("�G���[���b�Z�[�W")
        'CSVObject.Output("�������Z�@�փR�[�h")
        'CSVObject.Output("�������Z�@�֖�")
        'CSVObject.Output("�����X��")
        'CSVObject.Output("�����x�X��")
        'CSVObject.Output("�Ȗ�")
        'CSVObject.Output("���������ԍ�")
        'CSVObject.Output("���l")
        'CSVObject.Output("���z")
        'CSVObject.Output("���l", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F ��t���ו\���f�[�^�ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal aReadFmt As CAstFormat.CFormat)

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            BatchLog.Write("0000000000-00", "00000000", "���O�C��(�J�n)", "����")

            Dim OraReader As CASTCommon.MyOracleReader
            Dim SQL As New StringBuilder(128)
            Dim InfoPara As CAstBatch.CommData.stPARAMETER = aReadFmt.ToriData.INFOParameter    '�p�����[�^���
            Dim InfoTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast      '�������
            Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast                  '�˗����׏��
            Dim sort As String = aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T                   '�j�d�x�f�[�^

            '�f�[�^���P���ł��o�͂���΂n�j�t���O�𗧂Ă�
            DataOK = True

            'If sort = "1" Then
            '    CSVObject.Output(InfoMei.KEIYAKU_KIN & InfoMei.KEIYAKU_SIT & InfoMei.RECORD_NO.ToString.PadLeft(6))
            'Else
            '    CSVObject.Output("".PadLeft(12))
            'End If

            ToriSCode = InfoTori.TORIS_CODE_T
            ToriFCode = InfoTori.TORIF_CODE_T
            FuriDate = InfoMei.FURIKAE_DATE

            Tuuban += 1

            '----------------------------------------------------
            '�f�[�^�o��
            '----------------------------------------------------
            CSVObject.Output(InfoMei.FURIKAE_DATE)                          '�U�֓�
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '������
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    '�^�C�v�X�^���v
            CSVObject.Output(InfoTori.TORIS_CODE_T)                         '������R�[�h
            CSVObject.Output(InfoTori.TORIF_CODE_T)                         '����敛�R�[�h
            CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '����於
            CSVObject.Output(InfoTori.TKIN_NO_T)                            '�戵���Z�@�փR�[�h
            CSVObject.Output(InfoTori.TKIN_NNAME_N, True)                   '�戵���Z�@�֖�
            CSVObject.Output(InfoTori.TSIT_NO_T)                            '�戵�x�X�R�[�h
            CSVObject.Output(InfoTori.TSIT_NNAME_N, True)                   '�戵�x�X��
            CSVObject.Output(InfoTori.SYUBETU_T)                            '���

            Select Case InfoPara.CODE_KBN                                   '�R�[�h�敪
                Case "0"
                    CSVObject.Output("JIS")
                Case "1"
                    CSVObject.Output("JIS���L(120)")
                Case "2"
                    CSVObject.Output("JIS���L(119)")
                Case "3"
                    CSVObject.Output("JIS���L(118)")
                Case "4"
                    CSVObject.Output("EBCDIC")
            End Select

            ' 2010/09/09 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� -------------------->
            CSVObject.Output(InfoTori.FURI_CODE_T)                          '�U�փR�[�h
            CSVObject.Output(InfoTori.KIGYO_CODE_T)                         '��ƃR�[�h
            ' 2010/09/09 TASK)saitou �U��/��ƃR�[�h�󎚑Ή� --------------------<

            'CSVObject.Output(Tuuban)                                        '�ʔ�

            If aReadFmt.InErrorArray.Count = 0 Then                         '�G���[���b�Z�[�W
                CSVObject.Output("")
            Else
                CSVObject.Output(CType(aReadFmt.InErrorArray(0), CAstFormat.CFormat.INPUTERROR).ERRINFO, True)
            End If

            OraReader = aReadFmt.GetTENMAST(InfoMei.KEIYAKU_KIN, InfoMei.KEIYAKU_SIT)

            If OraReader Is Nothing OrElse OraReader.EOF = True OrElse _
                (Not OraReader Is Nothing AndAlso OraReader.GetItem("SIT_N") = "NG") Then
                CSVObject.Output(InfoMei.KEIYAKU_KIN)                       '���Z�@�փR�[�h
                CSVObject.Output("")                                        '���Z�@�֖�
                CSVObject.Output(InfoMei.KEIYAKU_SIT)                       '�x�X�R�[�h
                CSVObject.Output("")                                        '�x�X��
            Else
                CSVObject.Output(InfoMei.KEIYAKU_KIN)                       '���Z�@�փR�[�h
                CSVObject.Output(OraReader.GetValue(0), True)               '���Z�@�֖�
                CSVObject.Output(InfoMei.KEIYAKU_SIT)                       '�x�X�R�[�h
                CSVObject.Output(OraReader.GetValue(1), True)               '�x�X��
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If

            CSVObject.Output(InfoMei.KEIYAKU_KAMOKU)                        '�Ȗ�
            CSVObject.Output(InfoMei.KEIYAKU_KOUZA, True)                   '�����ԍ�
            CSVObject.Output(InfoMei.KEIYAKU_KNAME, True)                   '�a���Җ�
            CSVObject.Output(InfoMei.FURIKIN.ToString)                      '���z
            CSVObject.Output("", True)                                      '���l

            '2010/11/05 ���R�[�h�ԍ���ǉ��i�X�ʁE���R�[�hNo���\�[�g�p�j--------------------------------------------
            '���y�[�W�����ݒ�(�X�ԃ\�[�g�̏ꍇ�͋��Z�@�ց^�x�X�R�[�h��ݒ�)
            'If InfoTori.UMEISAI_KBN_T = "1" Then
            '    CSVObject.Output(InfoMei.KEIYAKU_KIN & _
            '                     InfoMei.KEIYAKU_SIT, False, True)          '���y�[�W�L�[(�X�ԃ\�[�g)
            'Else
            '    CSVObject.Output("0000000", False, True)                    '���y�[�W�L�[(���̑�)
            'End If

            If InfoTori.UMEISAI_KBN_T = "1" Then
                CSVObject.Output(InfoMei.KEIYAKU_KIN & _
                                 InfoMei.KEIYAKU_SIT)          '���y�[�W�L�[(�X�ԃ\�[�g)
            Else
                CSVObject.Output("0000000")                    '���y�[�W�L�[(���̑�)
            End If

            CSVObject.Output(InfoMei.RECORD_NO.ToString.PadLeft(6), False, True)    '���R�[�h�ԍ��o��
            '-------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            BatchLog.Write("(��t���ו\���)", "���s", ex.Message)
        Finally
        End Try
    End Sub

    Public Sub SortData(ByVal SortParam As String)
        '---------------------------------------------------------------------------------------
        '�f�[�^�\�[�g����
        '---------------------------------------------------------------------------------------
        Try
            ' 2016/06/10 �^�X�N�j���� CHG �yPG�zUI-03-01(RSV2�Ή�<���l�M��>) -------------------- START
            ''�\�[�g���F������R�[�h�A����敛�R�[�h�A���Z�@�փR�[�h�A�x�X�R�[�h
            ''2010/11/05 ���R�[�h�ԍ�����ǉ�
            'Call SortFile("3.10sjia 4.2sjia 15.4sjia 17.3sjia 25.6sjia")
            ' ''2010/09/21.Sakon�@��ƃR�[�h�E�U�փR�[�h�ǉ��Ή�
            ''Call SortFile("3.10sjia 4.2sjia 15.4sjia 17.3sjia")
            ''Call SortFile("3.10sjia 4.2sjia 13.4sjia 15.3sjia")
            ''Call SortFile("0.12sjia")
            Call SortFile(SortParam)
            ' 2016/06/10 �^�X�N�j���� CHG �yPG�zUI-03-01(RSV2�Ή�<���l�M��>) -------------------- END
        Catch ex As Exception
            BatchLog.Write("(��t���ו\���)�\�[�g", "���s", ex.Message)
        Finally
        End Try
    End Sub

    '
    ' �@�\�@ �F ������s
    '
    ' ���l�@ �F 
    '
    Public Overloads Overrides Function ReportExecute() As Boolean
        Try
            MyBase.CloseCsv()
            '2010/02/03 �ʏ�g�p����v�����^�ɂ��� ===
            'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
            Return MyBase.ReportExecute()
            '=======================================================
        Catch ex As System.Exception
            BatchLog.Write("(��t���ו\���)������s", "���s", ex.Message)
        End Try
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class

