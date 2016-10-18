Imports Microsoft.Win32
Imports System.IO
Imports System.Threading
Imports System.Xml

Public Class Form1

    Private Delegate Sub addNewItem(ByRef i As String)
    Private Delegate Sub updateItem(ByRef i As String, ByRef j As String)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim foldbox As New FolderBrowserDialog
        Dim ret = foldbox.ShowDialog()
        If ret = DialogResult.OK Then
            TextBox1.Text = foldbox.SelectedPath
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim rValue = Registry.ClassesRoot.OpenSubKey("wbpfile").OpenSubKey("Shell").OpenSubKey("Open").OpenSubKey("Command")
        Dim text As String = DirectCast(rValue.GetValue(""), String)
        TextBox1.Text = text.Replace("""", "").Replace("%1", "").Replace("WBPInstaller.exe", "").Trim + "WBP\"

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim th As New Thread(AddressOf nThread)
        th.Start()
    End Sub

    Private Sub addListItem(ByRef i As String)
        ListBox1.Items.Add(i.Clone())
    End Sub
    Private Sub updateListItem(ByRef i As String, ByRef j As String)
        Dim idx = ListBox1.Items.IndexOf(i)
        ListBox1.SelectedIndex = idx
        ListBox1.Items(ListBox1.SelectedIndex) = j
    End Sub

    Private Sub nThread()
        Dim filelist As New List(Of File)
        Dim FI As FileInfo()
        Dim DI = New DirectoryInfo(TextBox1.Text)
        Dim procStr As String = ""
        Dim fkstr As String = ""
        FI = DI.GetFiles("*.wbp")
        For Each f As FileInfo In FI
            procStr = "Fucking "
            Using zipToOpen = New FileStream(f.FullName, FileMode.Open)
                Using archive = New ZipArchive(zipToOpen, ZipArchiveMode.Update)
                    Using tStream As Stream = archive.GetEntry("wbpinfo").Open()
                        procStr += f.FullName + "..."
                        fkstr = procStr
                        BeginInvoke(New addNewItem(AddressOf addListItem), procStr)
                        Debug.WriteLine(fkstr)
                        Dim AnalyzeXML = New XmlDocument
                        AnalyzeXML.Load(tStream)
                        tStream.Seek(0, 0)
                        Dim tval = AnalyzeXML.SelectSingleNode("/Package/Version").InnerText
                        If tval = TextBox2.Text Then
                            AnalyzeXML.SelectSingleNode("/Package/Version").InnerText = TextBox3.Text
                            AnalyzeXML.Save(tStream)
                            procStr += "fucked."
                        Else
                            procStr += "not fucked."
                        End If
                        BeginInvoke(New updateItem(AddressOf updateListItem), fkstr, procStr)
                        Debug.WriteLine(procStr)
                    End Using
                End Using
            End Using
        Next
    End Sub
End Class
