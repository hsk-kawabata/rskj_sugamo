Option Explicit On 
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Collections.Specialized

Public Module M_運用試験

#Region "共通変数"

    'ログインユーザ用
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String
    Public GCom As MenteCommon.clsCommon
    Public GOwnerForm As Form

    '互換用
    Public gstrTORIS_CODE As String
    Public gstrTORIF_CODE As String
    Public gstrFURI_DATE As String
    Public gstrSYOKITI As String
    Public gstrSYOKITI_KIN As String
    Public gstrSYOKITI_SIT As String
    Public glngPAGE As Long
    Public gintCHK As Integer
#End Region

End Module


