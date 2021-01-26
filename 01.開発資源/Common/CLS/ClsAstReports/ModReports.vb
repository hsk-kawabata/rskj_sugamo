Option Explicit On 

Imports System.IO
Imports System.Drawing.Printing.PrinterSettings
Imports System.Runtime.InteropServices

Friend Module ModReports
    ' ÉvÉäÉìÉ^èÓïÒéÊìæóp
    Public Class WINAPI

        Declare Auto Function GetPrinter Lib "winspool.drv" (ByVal hPrinter As _
        IntPtr, ByVal Level As Integer, ByRef pPrinter As Byte, ByVal cbBuf _
        As Integer, ByRef pcbNeeded As Integer) As Boolean

        Declare Auto Function lstrcpy Lib "Kernel32.Lib" Alias "lstrcpyA" _
        (<OutAttribute(), MarshalAs(UnmanagedType.LPStr)> ByVal lpString1 As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpString2 As String) As Long

        Declare Auto Function ClosePrinter Lib "winspool.drv" Alias "ClosePrinter" (ByVal hPrinter As IntPtr) As Long

        Public Declare Function EnumJobs Lib "winspool.drv" Alias "EnumJobsA" _
        (ByVal hPrinter As IntPtr, _
        ByVal FirstJob As Int32, _
        ByVal NoJobs As Int32, _
        ByVal Level As Int32, _
        ByVal pJob As Byte(), _
        ByVal cdBuf As Int32, _
        ByRef pcbNeeded As Int32, _
        ByRef pcReturned As Int32) _
        As Long

        Declare Function OpenPrinter Lib "winspool.drv" Alias "OpenPrinterA" (ByVal pPrinterName As String, _
        ByRef phPrinter As IntPtr, ByVal pDefault As PRINTER_DEFAULTS) As Long
    End Class

    'Constants for the PRINTER_DEFAULTS structure
    Public Const PRINTER_ACCESS_USE As Long = &H8
    Public Const PRINTER_ACCESS_ADMINISTER As Long = &H4

    'Constants for the DEVMODE structure
    Public Const CCHDEVICENAME As Long = 32
    Public Const CCHFORMNAME As Long = 32
    Public API As New WINAPI
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> Structure SYSTEMTIME
        Public wYear As Short
        Public wMonth As Short
        Public wDayOfWeek As Short
        Public wDay As Short
        Public wHour As Short
        Public wMinute As Short
        Public wSecond As Short
        Public wMilliseconds As Short
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> Structure JOB_INFO_2
        Public PrinterJobId As Integer
        Public pPrinterName As Integer
        Public PrinterName As Integer
        Public PrinterUserName As Integer
        Public PrinterDocument As Integer
        Public PrinterNotifyName As Integer
        Public PrinterDatatype As Integer
        Public PrintProcessor As Integer
        Public PrinterParameters As Integer
        Public PrinterDriverName As Integer
        Public PrinterDevMode As Integer
        Public PrinterStatus As Integer
        Public PrinterSecurityDescriptor As Integer
        Public pStatus As Integer
        Public PrinterPriority As Integer
        Public Position As Integer
        Public StartTime As Integer
        Public UntilTime As Integer
        Public TotalPages As Integer
        Public Size As Integer
        Public Submitted As SYSTEMTIME
        Public time As Integer
        Public PagesPrinted As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> Public Structure PRINTER_INFO_2
        Public pServerName As Integer
        Public pPrinterName As Integer
        Public pShareName As Integer
        Public pPortName As Integer
        Public pDriverName As Integer
        Public pComment As Integer
        Public pLocation As Integer
        Public pDevMode As Integer
        Public pSepFile As Integer
        Public pPrintProcessor As Integer
        Public pDatatype As Integer
        Public pParameters As Integer
        Public pSecurityDescriptor As Integer
        Public Attributes As Integer
        Public Priority As Integer
        Public DefaultPriority As Integer
        Public StartTime As Integer
        Public UntilTime As Integer
        Public Status As Integer
        Public cJobs As Integer
        Public AveragePPM As Integer
    End Structure

    Public Function Pointer_to_String(ByVal Add As Long) As String
        Dim Temp_var As String
        Temp_var = New String(CChar(""), 512)
        Dim x As Long
        x = WINAPI.lstrcpy(Temp_var, Add.ToString)
        If (InStr(1, Temp_var, Chr(0)) = 0) Then
            Pointer_to_String = ""
        Else
            Pointer_to_String = Left(Temp_var, InStr(1, Temp_var, Chr(0)) - 1)
        End If
    End Function

    Public Function DatatoDeserial(ByVal datas() As Byte, ByVal type_to_change As Type, _
        ByVal NumJub As Integer) As Object
        'Returns the size of the JOB_INFO_2 structure
        Dim Data_to_Size As Integer = Marshal.SizeOf(type_to_change)
        If Data_to_Size > datas.Length Then
            Return Nothing
        End If

        Dim buffer As IntPtr = Marshal.AllocHGlobal(Data_to_Size)
        Dim startindex As Integer
        Dim i As Integer
        For i = 0 To NumJub - 1
            If i = 0 Then
                startindex = 0
            Else
                startindex = startindex + Data_to_Size
            End If
        Next
        'Copy data from the datas array to the unmanaged memory pointer.
        Marshal.Copy(datas, startindex, buffer, Data_to_Size)

        'Marshal data from the buffer pointer to a managed object.
        Dim result_obj As Object = Marshal.PtrToStructure(buffer, type_to_change)
        'Free the memory that is allocated from the unmanaged memory.
        Marshal.FreeHGlobal(buffer)
        Return result_obj
    End Function

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
    Structure PRINTER_DEFAULTS
        Public pDatatype As String
        Public pDevMode As Long
        Public DesiredAccess As Long
    End Structure

    'Define the printer status constants.
    Public Const ERROR_INSUFFICIENT_BUFFER As Long = 122
    Public Const PRINTER_STATUS_BUSY As Long = &H200
    Public Const PRINTER_STATUS_DOOR_OPEN As Long = &H400000
    Public Const PRINTER_STATUS_ERROR As Long = &H2
    Public Const PRINTER_STATUS_INITIALIZING As Long = &H8000
    Public Const PRINTER_STATUS_IO_ACTIVE As Long = &H100
    Public Const PRINTER_STATUS_MANUAL_FEED As Long = &H20
    Public Const PRINTER_STATUS_NO_TONER As Long = &H40000
    Public Const PRINTER_STATUS_NOT_AVAILABLE As Long = &H1000
    Public Const PRINTER_STATUS_OFFLINE As Long = &H80
    Public Const PRINTER_STATUS_OUT_OF_MEMORY As Long = &H200000
    Public Const PRINTER_STATUS_OUTPUT_BIN_FULL As Long = &H800
    Public Const PRINTER_STATUS_PAGE_PUNT As Long = &H80000
    Public Const PRINTER_STATUS_PAPER_JAM As Long = &H8
    Public Const PRINTER_STATUS_PAPER_OUT As Long = &H10
    Public Const PRINTER_STATUS_PAPER_PROBLEM As Long = &H40
    Public Const PRINTER_STATUS_PAUSED As Long = &H1
    Public Const PRINTER_STATUS_PENDING_DELETION As Long = &H4
    Public Const PRINTER_STATUS_PRINTING As Long = &H400
    Public Const PRINTER_STATUS_PROCESSING As Long = &H4000
    Public Const PRINTER_STATUS_TONER_LOW As Long = &H20000
    Public Const PRINTER_STATUS_USER_INTERVENTION As Long = &H100000
    Public Const PRINTER_STATUS_WAITING As Long = &H2000
    Public Const PRINTER_STATUS_WARMING_UP As Long = &H10000
    'Define the job status constants.
    Public Const JOB_STATUS_PAUSED As Long = &H1
    Public Const JOB_STATUS_ERROR As Long = &H2
    Public Const JOB_STATUS_DELETING As Long = &H4
    Public Const JOB_STATUS_SPOOLING As Long = &H8
    Public Const JOB_STATUS_PRINTING As Long = &H10
    Public Const JOB_STATUS_OFFLINE As Long = &H20
    Public Const JOB_STATUS_PAPEROUT As Long = &H40
    Public Const JOB_STATUS_PRINTED As Long = &H80
    Public Const JOB_STATUS_DELETED As Long = &H100
    Public Const JOB_STATUS_BLOCKED_DEVQ As Long = &H200
    Public Const JOB_STATUS_USER_INTERVENTION As Long = &H400
    Public Const JOB_STATUS_RESTART As Long = &H800

    Public Function GetString(ByVal PtrStr As Long) As String
        Dim StrBuff As String
        StrBuff = New String(CChar(""), 256)
        'Determine if a zero address is used.
        If PtrStr = 0 Then
            GetString = " "
            Exit Function
        End If

        'Copy data from PtrStr to the buffer.

        Dim PtrInt As IntPtr = New IntPtr(PtrStr)
        StrBuff = Marshal.PtrToStringAuto(PtrInt)

        'Remove any trailing nulls from the string.
        GetString = StripNulls(StrBuff)
    End Function

    Public Function StripNulls(ByVal OriginalStr As String) As String

        'Remove any trailing nulls from the input string.
        If (InStr(OriginalStr, Chr(0)) > 0) Then
            OriginalStr = Left(OriginalStr, InStr(OriginalStr, Chr(0)) - 1)
        End If
        'Return the modified string.
        StripNulls = OriginalStr
    End Function

    Public Function CheckPrinterStatus(ByVal PI2Status As Long) As String
        Dim tempStr As String
        If PI2Status = 0 Then   ' Return the "Ready" status.
            CheckPrinterStatus = "Printer Status = Ready" & vbCrLf
        Else
            tempStr = ""
            'Determine the printer state.
            If (PI2Status And PRINTER_STATUS_BUSY) = PRINTER_STATUS_BUSY Then
                tempStr = tempStr & "Busy  "
            End If
            If (PI2Status And PRINTER_STATUS_DOOR_OPEN) = PRINTER_STATUS_DOOR_OPEN Then
                tempStr = tempStr & "Printer Door Open  "
            End If
            If (PI2Status And PRINTER_STATUS_ERROR) = PRINTER_STATUS_ERROR Then
                tempStr = tempStr & "Printer Error  "
            End If
            If (PI2Status And PRINTER_STATUS_INITIALIZING) = PRINTER_STATUS_INITIALIZING Then
                tempStr = tempStr & "Initializing  "
            End If
            If (PI2Status And PRINTER_STATUS_IO_ACTIVE) = PRINTER_STATUS_IO_ACTIVE Then
                tempStr = tempStr & "I/O Active  "
            End If
            If (PI2Status And PRINTER_STATUS_MANUAL_FEED) = PRINTER_STATUS_MANUAL_FEED Then
                tempStr = tempStr & "Manual Feed  "
            End If
            If (PI2Status And PRINTER_STATUS_NO_TONER) = PRINTER_STATUS_NO_TONER Then
                tempStr = tempStr & "No Toner  "
            End If
            If (PI2Status And PRINTER_STATUS_NOT_AVAILABLE) = PRINTER_STATUS_NOT_AVAILABLE Then
                tempStr = tempStr & "Not Available  "
            End If
            If (PI2Status And PRINTER_STATUS_OFFLINE) = PRINTER_STATUS_OFFLINE Then
                tempStr = tempStr & "Off Line  "
            End If
            If (PI2Status And PRINTER_STATUS_OUT_OF_MEMORY) = PRINTER_STATUS_OUT_OF_MEMORY Then
                tempStr = tempStr & "Out of Memory  "
            End If
            If (PI2Status And PRINTER_STATUS_OUTPUT_BIN_FULL) = PRINTER_STATUS_OUTPUT_BIN_FULL Then
                tempStr = tempStr & "Output Bin Full  "
            End If
            If (PI2Status And PRINTER_STATUS_PAGE_PUNT) = PRINTER_STATUS_PAGE_PUNT Then
                tempStr = tempStr & "Page Punt  "
            End If
            If (PI2Status And PRINTER_STATUS_PAPER_JAM) = PRINTER_STATUS_PAPER_JAM Then
                tempStr = tempStr & "Paper Jam  "
            End If
            If (PI2Status And PRINTER_STATUS_PAPER_OUT) = PRINTER_STATUS_PAPER_OUT Then
                tempStr = tempStr & "Paper Out  "
            End If
            If (PI2Status And PRINTER_STATUS_OUTPUT_BIN_FULL) = PRINTER_STATUS_OUTPUT_BIN_FULL Then
                tempStr = tempStr & "Output Bin Full  "
            End If
            If (PI2Status And PRINTER_STATUS_PAPER_PROBLEM) = PRINTER_STATUS_PAPER_PROBLEM Then
                tempStr = tempStr & "Page Problem  "
            End If
            If (PI2Status And PRINTER_STATUS_PAUSED) = PRINTER_STATUS_PAUSED Then
                tempStr = tempStr & "Paused  "
            End If
            If (PI2Status And PRINTER_STATUS_PENDING_DELETION) = PRINTER_STATUS_PENDING_DELETION Then
                tempStr = tempStr & "Pending Deletion  "
            End If
            If (PI2Status And PRINTER_STATUS_PRINTING) = PRINTER_STATUS_PRINTING Then
                tempStr = tempStr & "Printing  "
            End If
            If (PI2Status And PRINTER_STATUS_PROCESSING) = PRINTER_STATUS_PROCESSING Then
                tempStr = tempStr & "Processing  "
            End If
            If (PI2Status And PRINTER_STATUS_TONER_LOW) = PRINTER_STATUS_TONER_LOW Then
                tempStr = tempStr & "Toner Low  "
            End If
            If (PI2Status And PRINTER_STATUS_USER_INTERVENTION) = PRINTER_STATUS_USER_INTERVENTION Then
                tempStr = tempStr & "User Intervention  "
            End If
            If (PI2Status And PRINTER_STATUS_WAITING) = PRINTER_STATUS_WAITING Then
                tempStr = tempStr & "Waiting  "
            End If
            If (PI2Status And PRINTER_STATUS_WARMING_UP) = PRINTER_STATUS_WARMING_UP Then
                tempStr = tempStr & "Warming Up  "
            End If
            If Len(tempStr) = 0 Then
                tempStr = "Unknown Status of " & PI2Status
            End If
            'Return the status.
            CheckPrinterStatus = "Printer Status = " & tempStr & vbCrLf
        End If
    End Function

    Function GetPrinterInfo(ByVal printername As String) As PRINTER_INFO_2
        Dim hPrinter As IntPtr
        Dim ByteBuf As Long
        Dim BytesNeeded As Int32
        Dim PI2 As New PRINTER_INFO_2
        Dim intCount As Integer
        Dim JI2(intCount) As JOB_INFO_2
        Dim PrinterInfo() As Byte
        'Dim JobInfo() As Byte
        Dim result As Long
        Dim LastError As Long
        'Dim tempStr As String
        'Dim NumJI2 As Int32
        Dim pDefaults As PRINTER_DEFAULTS = Nothing

        'Set the access security setting that you want.
        pDefaults.DesiredAccess = PRINTER_ACCESS_USE

        'Call the API to obtain a handle to the printer.
        'If an error occurs, display the error.
        result = WINAPI.OpenPrinter(printername, hPrinter, pDefaults)
        If result = 0 Then
            Return Nothing
        End If

        'Initialize the BytesNeeded variable.
        BytesNeeded = 0

        'Clear the error object.
        Err.Clear()

        'Determine the buffer size that is required to obtain the printer information.
        Dim ret As Boolean = WINAPI.GetPrinter(hPrinter, 2, 0&, 0, BytesNeeded)
        If ret = True Then
            result = 1
        Else
            result = 0
        End If
        'Display the error message that you receive when you call the GetPrinter function, 
        'and then close the printer handle.
        If Marshal.GetLastWin32Error() <> ERROR_INSUFFICIENT_BUFFER Then
            'CheckPrinter = " > GetPrinter Failed on initial call! <"
            WINAPI.ClosePrinter(hPrinter)
            Return Nothing
        End If
        ReDim PrinterInfo(BytesNeeded)
        ByteBuf = BytesNeeded

        'Call the GetPrinter function to obtain the status.
        If WINAPI.GetPrinter(hPrinter, 2, PrinterInfo(0), CType(ByteBuf, Integer), _
          BytesNeeded) = True Then
            result = 1
        Else
            result = 0
        End If

        'Check for any errors.
        If result = 0 Then
            'Get the error.
            LastError = Marshal.GetLastWin32Error()

            'Display the error message, and then close the printer handle.
            'CheckPrinter = "Could not get Printer Status!  Error = " _
            '   & LastError
            WINAPI.ClosePrinter(hPrinter)
            Return Nothing
        End If

        'Copy the contents of the printer status byte array into a
        'PRINTER_INFO_2 structure.
        PI2 = CType(DatatoDeserial(PrinterInfo, GetType(PRINTER_INFO_2), 1), PRINTER_INFO_2)
        Dim PrinterStr As String = CheckPrinterStatus(PI2.Status)

        'Add the printer name, the driver, and the port to the text box.
        PrinterStr = PrinterStr & "Printer Name = " & _
          GetString(PI2.pPrinterName) & vbCrLf
        PrinterStr = PrinterStr & "Printer Driver Name = " & _
          GetString(PI2.pDriverName) & vbCrLf
        PrinterStr = PrinterStr & "Printer Port Name = " & _
          GetString(PI2.pPortName) & vbCrLf

        'Close the printer handle.
        winAPI.ClosePrinter(hPrinter)

        Return PI2
    End Function
End Module

