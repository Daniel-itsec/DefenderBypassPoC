Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Environment

Class Form1


    <Flags()>
    Private Enum CryptProtectPromptFlags
        CRYPTPROTECT_PROMPT_ON_UNPROTECT = &H1
        CRYPTPROTECT_PROMPT_ON_PROTECT = &H2
    End Enum

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure DATA_BLOB
        Public cbData As Integer
        Public pbData As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure CRYPTPROTECT_PROMPTSTRUCT
        Public cbSize As Integer
        Public dwPromptFlags As CryptProtectPromptFlags
        Public hwndApp As IntPtr
        Public szPrompt As String
    End Structure

    <DllImport("Crypt32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function CryptUnprotectData(ByRef pDataIn As DATA_BLOB, ByVal szDataDescr As String, ByRef pOptionalEntropy As DATA_BLOB, ByVal pvReserved As IntPtr, ByRef pPromptStruct As CRYPTPROTECT_PROMPTSTRUCT, ByVal dwFlags As Integer, ByRef pDataOut As DATA_BLOB) As Boolean
    End Function

    Private Function Decrypt(ByVal Datas() As Byte) As String
        On Error Resume Next
        Dim inj, Ors As New DATA_BLOB
        Dim Ghandle As GCHandle = GCHandle.Alloc(Datas, GCHandleType.Pinned)
        inj.pbData = Ghandle.AddrOfPinnedObject()
        inj.cbData = Datas.Length
        Ghandle.Free()
        CryptUnprotectData(inj, Nothing, Nothing, Nothing, Nothing, 0, Ors)
        Dim Returned() As Byte = New Byte(Ors.cbData) {}
        Marshal.Copy(Ors.pbData, Returned, 0, Ors.cbData)
        Dim TheString As String = Encoding.UTF8.GetString(Returned)
        Return TheString.Substring(0, TheString.Length - 1)
    End Function

    Private Sub chromedump()
        Try
            Dim a = "Data"
            Dim sqllogin As New SQLiteHandler(GetFolderPath(SpecialFolder.LocalApplicationData) + "\Google\Chrome\User Data\Default\Login " + a)
            sqllogin.ReadTable("logins")
            For i As Integer = 0 To sqllogin.GetRowCount() - 1
                Dim url As String = sqllogin.GetValue(i, "origin_url")
                Dim username As String = sqllogin.GetValue(i, "username_value")
                Dim password_crypted As String = sqllogin.GetValue(i, "password_value")
                Dim password As String = IIf(String.IsNullOrEmpty(password_crypted), "", Decrypt(Encoding.Default.GetBytes(password_crypted)))
                RichTextBox1.Text += username + " | " + password + " | " + url + vbNewLine
            Next
        Catch ex As Exception
            MsgBox("Error getting Chrome passwords", MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        chromedump()
    End Sub

End Class
