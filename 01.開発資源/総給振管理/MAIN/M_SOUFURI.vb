Option Explicit On 
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Collections.Specialized

Public Module M_SOUFURI

#Region "���ʕϐ�"
    '���O�C�����[�U�p
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String

    Public GCom As MenteCommon.clsCommon

    Public GOwnerForm As Form

    '�݊��p
    'Public gstrTORIS_CODE As String
    'Public gstrTORIF_CODE As String
    'Public gstrFURI_DATE As String
    'Public gstrSYOKITI As String
    'Public gstrSYOKITI_KIN As String
    'Public gstrSYOKITI_SIT As String
    'Public glngPAGE As Long
    'Public gintCHK As Integer
#End Region

#Region "�N���X by mitsu"
    'CSV��n�������̒��[�����p
    Public Class KFSPxxx
        Inherits CAstReports.ClsReportBase

        Sub New(ByVal reportname As String)
            ' CSV�t�@�C���Z�b�g
            InfoReport.ReportName = reportname
        End Sub

        Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
            CSVObject.Output(data, dq, crlf)
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class

#End Region

#Region "�֐� by mitsu"
    Public Function GetStringDictionary(ByVal filename As String) As System.Collections.Specialized.StringDictionary
        '�L�[�A�l�̃e�L�X�g��ǂ��StringDictionary�ɂ��ĕԂ��֐�
        Dim sd As StringDictionary = New StringDictionary

        Try
            Dim FileNameFull As String = Path.Combine(GCom.GetTXTFolder, filename)

            If File.Exists(FileNameFull) Then
                Using sr As StreamReader = New StreamReader(FileNameFull, Encoding.GetEncoding(932))
                    While sr.Peek > -1
                        Dim s() As String = sr.ReadLine().Split(","c)
                        If sd.ContainsKey(s(0).Trim) = False Then
                            sd.Add(s(0).Trim, s(1).Trim)
                        End If
                    End While
                End Using
            End If

        Catch
            Throw
        End Try

        Return sd
    End Function
#End Region

End Module


