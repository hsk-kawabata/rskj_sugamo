Imports System
Imports System.Threading

Module ModMain

    Function Main(ByVal CmdArgs() As String) As Integer

        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            If CmdArgs.Length <> 2 Then
                Console.Write("JobCheck：パラメタエラー")
                Return 255
            End If

            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim sql As String
            sql = "select max(tuuban_j) from jobmast where " & _
                  "jobid_j='" & CmdArgs(0) & "' and " & _
                  "touroku_date_j='" & CmdArgs(1) & "' " & _
                  "group by touroku_date_j"

            If oraReader.DataReader(sql) = False Then
                Console.Write("JobCheck：該当ジョブなし")
                Return 255
            End If

            Dim tuuban As String = oraReader.GetValue(0)

            oraReader.Close()
            oraReader = Nothing
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            sql = "select status_j from jobmast where " & _
                  "tuuban_j='" & tuuban & "' and " & _
                  "touroku_date_j='" & CmdArgs(1) & "' and " & _
                  "(status_j='2' or status_j='7')"

            While (oraReader.DataReader(sql) = False)
                oraReader.Close()
                oraReader = Nothing
                oraReader = New CASTCommon.MyOracleReader(MainDB)

                Thread.Sleep(100)
            End While

            Dim status As String = oraReader.GetValue(0)

            Console.Write(status)
            Return 0

        Catch ex As Exception
            Console.Write(ex.Tostring)
            Return 255

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If

            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
            End If

        End Try

    End Function

End Module
