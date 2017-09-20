Imports System.IO
Imports Protein_Uploader
Imports ValidateFastaFile

Public Class frmValidationReport
    Inherits Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call


    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Friend WithEvents colErrorDescription As ColumnHeader
    Friend WithEvents lvwErrorList As ListView
    Friend WithEvents lblErrorList As Label
    Friend WithEvents cmdClose As Button
    Friend WithEvents gbxValidFileList As GroupBox
    Friend WithEvents gbxInvalidFileList As GroupBox
    Friend WithEvents lvwValidList As ListView
    Friend WithEvents colFileName As ColumnHeader
    Friend WithEvents colOrganism As ColumnHeader
    Friend WithEvents colCount As ColumnHeader
    Friend WithEvents pgbListViewLoad As ProgressBar
    Friend WithEvents colNumOccurences As ColumnHeader
    Friend WithEvents cmdExportErrorDetails As Button
    Friend WithEvents colActualCount As ColumnHeader
    Friend WithEvents fraFastaFileWarnings As GroupBox
    Friend WithEvents lvwWarningList As ListView
    Friend WithEvents cmdExportWarningDetails As Button
    Friend WithEvents cboFileListWarnings As ComboBox
    Friend WithEvents lblWarning As Label
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents cboFileListErrors As ComboBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.cboFileListErrors = New ComboBox
        Me.cmdExportErrorDetails = New Button
        Me.lblErrorList = New Label
        Me.cmdClose = New Button
        Me.lvwErrorList = New ListView
        Me.colNumOccurences = New ColumnHeader
        Me.colErrorDescription = New ColumnHeader
        Me.gbxValidFileList = New GroupBox
        Me.lvwValidList = New ListView
        Me.colFileName = New ColumnHeader
        Me.colOrganism = New ColumnHeader
        Me.colCount = New ColumnHeader
        Me.colActualCount = New ColumnHeader
        Me.gbxInvalidFileList = New GroupBox
        Me.pgbListViewLoad = New ProgressBar
        Me.fraFastaFileWarnings = New GroupBox
        Me.lvwWarningList = New ListView
        Me.ColumnHeader1 = New ColumnHeader
        Me.ColumnHeader2 = New ColumnHeader
        Me.cmdExportWarningDetails = New Button
        Me.cboFileListWarnings = New ComboBox
        Me.lblWarning = New Label
        Me.gbxValidFileList.SuspendLayout()
        Me.gbxInvalidFileList.SuspendLayout()
        Me.fraFastaFileWarnings.SuspendLayout()
        Me.SuspendLayout()
        '
        'cboFileListErrors
        '
        Me.cboFileListErrors.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboFileListErrors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboFileListErrors.Location = New System.Drawing.Point(12, 28)
        Me.cboFileListErrors.Name = "cboFileListErrors"
        Me.cboFileListErrors.Size = New System.Drawing.Size(448, 21)
        Me.cboFileListErrors.TabIndex = 1
        '
        'cmdExportErrorDetails
        '
        Me.cmdExportErrorDetails.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdExportErrorDetails.Location = New System.Drawing.Point(468, 28)
        Me.cmdExportErrorDetails.Name = "cmdExportErrorDetails"
        Me.cmdExportErrorDetails.Size = New System.Drawing.Size(114, 20)
        Me.cmdExportErrorDetails.TabIndex = 3
        Me.cmdExportErrorDetails.Text = "Export Detailed List"
        '
        'lblErrorList
        '
        Me.lblErrorList.Location = New System.Drawing.Point(12, 56)
        Me.lblErrorList.Name = "lblErrorList"
        Me.lblErrorList.Size = New System.Drawing.Size(406, 16)
        Me.lblErrorList.TabIndex = 4
        Me.lblErrorList.Text = "Recorded Validation Errors"
        '
        'cmdClose
        '
        Me.cmdClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdClose.Location = New System.Drawing.Point(520, 626)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(84, 24)
        Me.cmdClose.TabIndex = 5
        Me.cmdClose.Text = "Close"
        '
        'lvwErrorList
        '
        Me.lvwErrorList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwErrorList.Columns.AddRange(New ColumnHeader() {Me.colNumOccurences, Me.colErrorDescription})
        Me.lvwErrorList.FullRowSelect = True
        Me.lvwErrorList.GridLines = True
        Me.lvwErrorList.Location = New System.Drawing.Point(12, 76)
        Me.lvwErrorList.MultiSelect = False
        Me.lvwErrorList.Name = "lvwErrorList"
        Me.lvwErrorList.Size = New System.Drawing.Size(572, 127)
        Me.lvwErrorList.Sorting = System.Windows.Forms.SortOrder.Descending
        Me.lvwErrorList.TabIndex = 6
        Me.lvwErrorList.View = System.Windows.Forms.View.Details
        '
        'colNumOccurences
        '
        Me.colNumOccurences.Text = "Error Count"
        Me.colNumOccurences.Width = 80
        '
        'colErrorDescription
        '
        Me.colErrorDescription.Text = "Error Description"
        Me.colErrorDescription.Width = 488
        '
        'gbxValidFileList
        '
        Me.gbxValidFileList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbxValidFileList.Controls.Add(Me.lvwValidList)
        Me.gbxValidFileList.Location = New System.Drawing.Point(10, 10)
        Me.gbxValidFileList.Name = "gbxValidFileList"
        Me.gbxValidFileList.Size = New System.Drawing.Size(596, 146)
        Me.gbxValidFileList.TabIndex = 7
        Me.gbxValidFileList.TabStop = False
        Me.gbxValidFileList.Text = "FASTA Files Successfully Uploaded"
        '
        'lvwValidList
        '
        Me.lvwValidList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwValidList.Columns.AddRange(New ColumnHeader() {Me.colFileName, Me.colOrganism, Me.colCount, Me.colActualCount})
        Me.lvwValidList.FullRowSelect = True
        Me.lvwValidList.GridLines = True
        Me.lvwValidList.Location = New System.Drawing.Point(12, 30)
        Me.lvwValidList.MultiSelect = False
        Me.lvwValidList.Name = "lvwValidList"
        Me.lvwValidList.Size = New System.Drawing.Size(572, 101)
        Me.lvwValidList.TabIndex = 7
        Me.lvwValidList.View = System.Windows.Forms.View.Details
        '
        'colFileName
        '
        Me.colFileName.Text = "File Name"
        Me.colFileName.Width = 313
        '
        'colOrganism
        '
        Me.colOrganism.Text = "Organism"
        Me.colOrganism.Width = 100
        '
        'colCount
        '
        Me.colCount.Text = "Protein Count"
        Me.colCount.Width = 80
        '
        'colActualCount
        '
        Me.colActualCount.Text = "Actual Count"
        Me.colActualCount.Width = 75
        '
        'gbxInvalidFileList
        '
        Me.gbxInvalidFileList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbxInvalidFileList.Controls.Add(Me.lvwErrorList)
        Me.gbxInvalidFileList.Controls.Add(Me.cmdExportErrorDetails)
        Me.gbxInvalidFileList.Controls.Add(Me.cboFileListErrors)
        Me.gbxInvalidFileList.Controls.Add(Me.lblErrorList)
        Me.gbxInvalidFileList.Location = New System.Drawing.Point(10, 400)
        Me.gbxInvalidFileList.Name = "gbxInvalidFileList"
        Me.gbxInvalidFileList.Size = New System.Drawing.Size(596, 216)
        Me.gbxInvalidFileList.TabIndex = 8
        Me.gbxInvalidFileList.TabStop = False
        Me.gbxInvalidFileList.Text = "FASTA Files Not Uploaded Due to Errors"
        '
        'pgbListViewLoad
        '
        Me.pgbListViewLoad.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pgbListViewLoad.Location = New System.Drawing.Point(10, 630)
        Me.pgbListViewLoad.Name = "pgbListViewLoad"
        Me.pgbListViewLoad.Size = New System.Drawing.Size(496, 18)
        Me.pgbListViewLoad.TabIndex = 9
        Me.pgbListViewLoad.Visible = False
        '
        'fraFastaFileWarnings
        '
        Me.fraFastaFileWarnings.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.fraFastaFileWarnings.Controls.Add(Me.lvwWarningList)
        Me.fraFastaFileWarnings.Controls.Add(Me.cmdExportWarningDetails)
        Me.fraFastaFileWarnings.Controls.Add(Me.cboFileListWarnings)
        Me.fraFastaFileWarnings.Controls.Add(Me.lblWarning)
        Me.fraFastaFileWarnings.Location = New System.Drawing.Point(8, 166)
        Me.fraFastaFileWarnings.Name = "fraFastaFileWarnings"
        Me.fraFastaFileWarnings.Size = New System.Drawing.Size(596, 224)
        Me.fraFastaFileWarnings.TabIndex = 10
        Me.fraFastaFileWarnings.TabStop = False
        Me.fraFastaFileWarnings.Text = "Fasta File Warnings"
        '
        'lvwWarningList
        '
        Me.lvwWarningList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwWarningList.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lvwWarningList.FullRowSelect = True
        Me.lvwWarningList.GridLines = True
        Me.lvwWarningList.Location = New System.Drawing.Point(12, 77)
        Me.lvwWarningList.MultiSelect = False
        Me.lvwWarningList.Name = "lvwWarningList"
        Me.lvwWarningList.Size = New System.Drawing.Size(572, 136)
        Me.lvwWarningList.Sorting = System.Windows.Forms.SortOrder.Descending
        Me.lvwWarningList.TabIndex = 6
        Me.lvwWarningList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Warning Count"
        Me.ColumnHeader1.Width = 80
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Warning Description"
        Me.ColumnHeader2.Width = 488
        '
        'cmdExportWarningDetails
        '
        Me.cmdExportWarningDetails.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdExportWarningDetails.Location = New System.Drawing.Point(468, 29)
        Me.cmdExportWarningDetails.Name = "cmdExportWarningDetails"
        Me.cmdExportWarningDetails.Size = New System.Drawing.Size(114, 20)
        Me.cmdExportWarningDetails.TabIndex = 3
        Me.cmdExportWarningDetails.Text = "Export Detailed List"
        '
        'cboFileListWarnings
        '
        Me.cboFileListWarnings.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboFileListWarnings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboFileListWarnings.Location = New System.Drawing.Point(12, 29)
        Me.cboFileListWarnings.Name = "cboFileListWarnings"
        Me.cboFileListWarnings.Size = New System.Drawing.Size(448, 21)
        Me.cboFileListWarnings.TabIndex = 1
        '
        'lblWarning
        '
        Me.lblWarning.Location = New System.Drawing.Point(12, 56)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(406, 16)
        Me.lblWarning.TabIndex = 4
        Me.lblWarning.Text = "Recorded Validation Warnings"
        '
        'frmValidationReport
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(614, 658)
        Me.Controls.Add(Me.fraFastaFileWarnings)
        Me.Controls.Add(Me.pgbListViewLoad)
        Me.Controls.Add(Me.gbxInvalidFileList)
        Me.Controls.Add(Me.gbxValidFileList)
        Me.Controls.Add(Me.cmdClose)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(362, 416)
        Me.Name = "frmValidationReport"
        Me.Text = "FASTA File Validation Failure Report"
        Me.gbxValidFileList.ResumeLayout(False)
        Me.gbxInvalidFileList.ResumeLayout(False)
        Me.fraFastaFileWarnings.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private m_ErrorCollection As ArrayList      ' Array of type ValidateFastaFile.ICustomValidation.udtErrorInfoExtended
    Private m_WarningCollection As ArrayList    ' Array of type ValidateFastaFile.ICustomValidation.udtErrorInfoExtended

    Private m_FileErrorList As Hashtable        ' Tracks the errors found for each file
    Private m_FileWarningList As Hashtable      ' Tracks the warnings found for each file

    Private m_FileValidList As Hashtable
    Private m_SummarizedFileErrors As Hashtable
    Private m_SummarizedFileWarnings As Hashtable

    Private m_Organisms As DataTable
    Private m_ErrorListLoaded As Boolean

    Private Sub frmValidationReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        FillValidListView()
        BindFileListToErrorComboBox(m_FileErrorList)
        BindFileListToWarningComboBox(m_FileWarningList)

        If cboFileListErrors.Items.Count > 0 Then
            cboFileListErrors.SelectedIndex = 0
            cboFileListErrors.Select()
            cboFileListErrors_SelectedIndexChanged(Me, Nothing)
        End If

        If cboFileListWarnings.Items.Count > 0 Then
            cboFileListWarnings.SelectedIndex = 0
            cboFileListWarnings.Select()
            cboFileListWarnings_SelectedIndexChanged(Me, Nothing)
        End If

    End Sub

    Private Sub cmdExportErrorDetails_Click(sender As Object, e As EventArgs) Handles cmdExportErrorDetails.Click
        If m_ErrorCollection Is Nothing OrElse m_ErrorCollection.Count = 0 Then
            MessageBox.Show("Error list is empty; nothing to export", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DumpDetailedErrorOrWarningList(m_ErrorCollection, cboFileListErrors.Text, "Error")
        End If
    End Sub

    Private Sub cmdExportWarningDetails_Click(sender As Object, e As EventArgs) Handles cmdExportWarningDetails.Click
        If m_WarningCollection Is Nothing OrElse m_WarningCollection.Count = 0 Then
            MessageBox.Show("Warning list is empty; nothing to export", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DumpDetailedErrorOrWarningList(m_WarningCollection, cboFileListWarnings.Text, "Warning")
        End If

    End Sub

    Private Sub cboFileListErrors_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFileListErrors.SelectedIndexChanged
        Me.lvwErrorList.Items.Clear()
        If Not Me.m_FileErrorList Is Nothing AndAlso Me.m_FileErrorList.Count > 0 Then
            Me.m_ErrorCollection = DirectCast(Me.m_FileErrorList.Item(Me.cboFileListErrors.Text), ArrayList)
        End If

        If Not m_SummarizedFileErrors Is Nothing AndAlso m_SummarizedFileErrors.Count > 0 Then
            Dim errorSummary = DirectCast(Me.m_SummarizedFileErrors.Item(Me.cboFileListErrors.Text), Hashtable)
            Me.FillErrorListView(errorSummary)
        End If
    End Sub

    Private Sub cboFileListWarnings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFileListWarnings.SelectedIndexChanged
        Me.lvwWarningList.Items.Clear()
        If Not Me.m_FileWarningList Is Nothing AndAlso Me.m_FileWarningList.Count > 0 Then
            Me.m_WarningCollection = DirectCast(Me.m_FileWarningList.Item(Me.cboFileListWarnings.Text), ArrayList)
        End If

        If Not m_SummarizedFileWarnings Is Nothing AndAlso m_SummarizedFileWarnings.Count > 0 Then
            Dim WarningSummary = DirectCast(Me.m_SummarizedFileWarnings.Item(Me.cboFileListWarnings.Text), Hashtable)
            Me.FillWarningListView(WarningSummary)
        End If
    End Sub

    Friend WriteOnly Property ErrorSummaryList() As Hashtable
        Set(Value As Hashtable)
            Me.m_SummarizedFileErrors = Value
        End Set
    End Property

    Friend WriteOnly Property WarningSummaryList() As Hashtable
        Set(Value As Hashtable)
            Me.m_SummarizedFileWarnings = Value
        End Set
    End Property

    Friend WriteOnly Property FileErrorList() As Hashtable
        Set(Value As Hashtable)
            Me.m_FileErrorList = Value
        End Set
    End Property

    Friend WriteOnly Property FileWarningList() As Hashtable
        Set(Value As Hashtable)
            Me.m_FileWarningList = Value
        End Set
    End Property

    Friend WriteOnly Property FileValidList() As Hashtable
        Set(Value As Hashtable)
            Me.m_FileValidList = Value
        End Set
    End Property

    Friend WriteOnly Property OrganismList() As DataTable
        Set(Value As DataTable)
            Me.m_Organisms = Value
        End Set
    End Property

    Private Function GetOrganismName(organismID As Integer) As String
        Dim foundrows() As DataRow

        foundrows = Me.m_Organisms.Select("ID = " & organismID.ToString)

        Return foundrows(0).Item("Display_Name").ToString

    End Function

    Private Sub BindFileListToErrorComboBox(contents As Hashtable)

        Dim errorCount As Integer
        RemoveHandler cboFileListErrors.SelectedIndexChanged, AddressOf cboFileListErrors_SelectedIndexChanged

        If Not contents Is Nothing Then
            Dim counter = contents.GetEnumerator

            Me.cboFileListErrors.BeginUpdate()

            While counter.MoveNext()
                errorCount = DirectCast(Me.m_FileErrorList.Item(counter.Key), ArrayList).Count
                Me.cboFileListErrors.Items.Add(counter.Key) '.ToString & " (" & errorCount & " Errors)")
            End While

            Me.cboFileListErrors.EndUpdate()

        Else
            Me.cboFileListErrors.BeginUpdate()
            Me.cboFileListErrors.Items.Add("-- No Errors --")
            Me.cboFileListErrors.EndUpdate()
        End If

        AddHandler cboFileListErrors.SelectedIndexChanged, AddressOf cboFileListErrors_SelectedIndexChanged

    End Sub

    Private Sub BindFileListToWarningComboBox(contents As Hashtable)

        Dim WarningCount As Integer
        RemoveHandler cboFileListWarnings.SelectedIndexChanged, AddressOf cboFileListWarnings_SelectedIndexChanged

        If Not contents Is Nothing Then
            Dim counter = contents.GetEnumerator

            Me.cboFileListWarnings.BeginUpdate()

            While counter.MoveNext()
                WarningCount = DirectCast(Me.m_FileWarningList.Item(counter.Key), ArrayList).Count
                Me.cboFileListWarnings.Items.Add(counter.Key)
            End While

            cboFileListWarnings.EndUpdate()

        Else
            cboFileListWarnings.BeginUpdate()
            cboFileListWarnings.Items.Add("-- No Warnings --")
            cboFileListWarnings.EndUpdate()
        End If

        AddHandler cboFileListWarnings.SelectedIndexChanged, AddressOf cboFileListWarnings_SelectedIndexChanged

    End Sub

    Private Sub FillErrorListView(errorSummary As Hashtable)
        FillErrorOrWarningListView(Me.lvwErrorList, errorSummary)
    End Sub

    Private Sub FillWarningListView(warningSummary As Hashtable)
        FillErrorOrWarningListView(Me.lvwWarningList, warningSummary)
    End Sub

    Private Sub FillErrorOrWarningListView(objListview As ListView, itemSummary As Hashtable)
        Dim li As ListViewItem

        If Not itemSummary Is Nothing Then
            Dim counter = itemSummary.GetEnumerator

            objListview.BeginUpdate()
            objListview.Items.Clear()

            While counter.MoveNext()
                li = New ListViewItem(counter.Value.ToString)
                li.SubItems.Add(counter.Key.ToString)
                objListview.Items.Add(li)
            End While

            objListview.EndUpdate()
        End If
    End Sub

    Private Sub FillValidListView()
        Dim li As ListViewItem
        Dim entry As IUploadProteins.UploadInfo
        Dim FileName As String

        If Me.m_FileValidList Is Nothing Then
            Me.m_FileValidList = New Hashtable
        End If

        If Me.m_FileValidList.Count = 0 Then
            Exit Sub
        End If

        Dim counter = Me.m_FileValidList.GetEnumerator

        Me.lvwValidList.BeginUpdate()
        Me.lvwValidList.Items.Clear()

        While counter.MoveNext()
            entry = DirectCast(counter.Value, IUploadProteins.UploadInfo)
            FileName = Path.GetFileName(counter.Key.ToString)
            li = New ListViewItem(FileName)
            li.SubItems.Add(Me.GetOrganismName(entry.OrganismID))
            li.SubItems.Add(entry.ProteinCount.ToString)
            li.SubItems.Add(entry.ExportedProteinCount.ToString)
            Me.lvwValidList.Items.Add(li)
        End While

        Me.lvwValidList.EndUpdate()

    End Sub

    Private Sub DumpDetailedErrorOrWarningList(errorList As ArrayList, FASTAFileName As String, strMessageType As String)

        Dim SaveDialog As New SaveFileDialog

        Dim SelectedSavePath As String

        Dim errorDetail As ICustomValidation.udtErrorInfoExtended

        Dim intErrorCount = 0

        If String.IsNullOrWhiteSpace(messageType) Then
            messageType = "Error"
        End If

        With SaveDialog
            .Title = "Save Protein Database File"
            .DereferenceLinks = True
            .Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            .FilterIndex = 1
            .RestoreDirectory = True
            .OverwritePrompt = True
            .FileName = Path.GetFileNameWithoutExtension(fastaFileName) & "_" & messageType
        End With

        If SaveDialog.ShowDialog = DialogResult.OK Then
            SelectedSavePath = SaveDialog.FileName
        Else
            Exit Sub
        End If

        Using sw = New StreamWriter(New FileStream(SelectedSavePath, FileMode.Create, FileAccess.Write, FileShare.Read))


            sw.WriteLine("Protein Name" & ControlChars.Tab &
                     "Line Number" & ControlChars.Tab &
                     "Message Type" & ControlChars.Tab &
                     "Message")

            If Not errorList Is Nothing AndAlso errorList.Count > 0 Then
                For Each errorDetail In errorList
                    sw.WriteLine(
                        errorDetail.ProteinName & ControlChars.Tab &
                        errorDetail.LineNumber & ControlChars.Tab &
                        errorDetail.Type & ControlChars.Tab &
                        errorDetail.MessageText)

                    intErrorCount += 1
                Next
            End If

            sw.WriteLine("")

        End Using

        MessageBox.Show("Wrote " & intErrorCount.ToString & " " & messageType & "s to " & SaveDialog.FileName, "Detailed " & messageType & " List", MessageBoxButtons.OK)

    End Sub

End Class
