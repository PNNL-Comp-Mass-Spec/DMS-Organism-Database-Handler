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

        m_ErrorCollection = New List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)
        m_WarningCollection = New List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)

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

    Private ReadOnly m_ErrorCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)
    Private ReadOnly m_WarningCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)

    Private m_FileErrorList As Dictionary(Of String, List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))        ' Tracks the errors found for each file
    Private m_FileWarningList As Dictionary(Of String, List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))      ' Tracks the warnings found for each file

    ''' <summary>
    ''' Keys are fasta file paths, values are upload info
    ''' </summary>
    Private m_FileValidList As Dictionary(Of String, IUploadProteins.UploadInfo)

    ''' <summary>
    ''' Keys are fasta file names, values are dictionaries of error messages, tracking the count of each error
    ''' </summary>
    Private m_SummarizedFileErrors As Dictionary(Of String, Dictionary(Of String, Integer))

    ''' <summary>
    ''' Keys are fasta file names, values are dictionaries of warning messages, tracking the count of each warning
    ''' </summary>
    Private m_SummarizedFileWarnings As Dictionary(Of String, Dictionary(Of String, Integer))

    Private m_Organisms As DataTable

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

        HandleErrorOrWarningListSelectedIndexChanged(
            cboFileListErrors.Text, lvwErrorList, m_FileErrorList, m_SummarizedFileErrors, m_ErrorCollection)

    End Sub

    Private Sub cboFileListWarnings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFileListWarnings.SelectedIndexChanged

        HandleErrorOrWarningListSelectedIndexChanged(
            cboFileListWarnings.Text, lvwWarningList, m_FileWarningList, m_SummarizedFileWarnings, m_WarningCollection)

    End Sub

    Private Sub HandleErrorOrWarningListSelectedIndexChanged(
      selectedItemText As String,
      objListView As ListView,
      itemListByFile As IReadOnlyDictionary(Of String, List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)),
      summarizedItemList As IReadOnlyDictionary(Of String, Dictionary(Of String, Integer)),
      itemCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))

        objListView.Items.Clear()
        If Not itemListByFile Is Nothing AndAlso itemListByFile.Count > 0 Then
            Dim itemList As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended) = Nothing

            If itemListByFile.TryGetValue(selectedItemText, itemList) Then
                itemCollection.AddRange(itemList)
            Else
                itemCollection.Clear()
            End If
        End If

        If Not summarizedItemList Is Nothing AndAlso summarizedItemList.Count > 0 Then
            Dim itemSummary As Dictionary(Of String, Integer) = Nothing

            If summarizedItemList.TryGetValue(selectedItemText, itemSummary) Then
                FillErrorOrWarningListView(objListView, itemSummary)
            End If

        End If

    End Sub

    Friend WriteOnly Property ErrorSummaryList As Dictionary(Of String, Dictionary(Of String, Integer))
        Set
            m_SummarizedFileErrors = Value
        End Set
    End Property

    Friend WriteOnly Property WarningSummaryList As Dictionary(Of String, Dictionary(Of String, Integer))
        Set
            m_SummarizedFileWarnings = Value
        End Set
    End Property

    Friend WriteOnly Property FileErrorList As Dictionary(Of String, List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))
        Set
            m_FileErrorList = Value
        End Set
    End Property

    Friend WriteOnly Property FileWarningList As Dictionary(Of String, List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))
        Set
            m_FileWarningList = Value
        End Set
    End Property

    Friend WriteOnly Property FileValidList As Dictionary(Of String, IUploadProteins.UploadInfo)
        Set
            m_FileValidList = Value
        End Set
    End Property

    Friend WriteOnly Property OrganismList As DataTable
        Set
            m_Organisms = Value
        End Set
    End Property

    Private Function GetOrganismName(organismID As Integer) As String
        Dim foundRows() As DataRow

        foundRows = m_Organisms.Select("ID = " & organismID.ToString)

        Return foundrows(0).Item("Display_Name").ToString

    End Function

    Private Sub BindFileListToErrorComboBox(contents As Dictionary(Of String, List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)))

        RemoveHandler cboFileListErrors.SelectedIndexChanged, AddressOf cboFileListErrors_SelectedIndexChanged

        If Not contents Is Nothing Then

            cboFileListErrors.BeginUpdate()

            For Each item In contents
                cboFileListErrors.Items.Add(item.Key)
            Next

            cboFileListErrors.EndUpdate()

        Else
            cboFileListErrors.BeginUpdate()
            cboFileListErrors.Items.Add("-- No Errors --")
            cboFileListErrors.EndUpdate()
        End If

        AddHandler cboFileListErrors.SelectedIndexChanged, AddressOf cboFileListErrors_SelectedIndexChanged

    End Sub

    Private Sub BindFileListToWarningComboBox(contents As Dictionary(Of String, List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)))

        RemoveHandler cboFileListWarnings.SelectedIndexChanged, AddressOf cboFileListWarnings_SelectedIndexChanged

        If Not contents Is Nothing Then

            cboFileListWarnings.BeginUpdate()

            For Each item In contents
                cboFileListWarnings.Items.Add(item.Key)
            Next

            cboFileListWarnings.EndUpdate()

        Else
            cboFileListWarnings.BeginUpdate()
            cboFileListWarnings.Items.Add("-- No Warnings --")
            cboFileListWarnings.EndUpdate()
        End If

        AddHandler cboFileListWarnings.SelectedIndexChanged, AddressOf cboFileListWarnings_SelectedIndexChanged

    End Sub

    Private Sub FillErrorOrWarningListView(objListView As ListView, itemSummary As Dictionary(Of String, Integer))
        Dim li As ListViewItem

        If Not itemSummary Is Nothing Then

            objListView.BeginUpdate()
            objListView.Items.Clear()

            For Each item In itemSummary

                li = New ListViewItem(item.Value.ToString())
                li.SubItems.Add(item.Key)
                objListView.Items.Add(li)
            Next

            objListView.EndUpdate()
        End If
    End Sub

    Private Sub FillValidListView()

        If m_FileValidList Is Nothing Then
            m_FileValidList = New Dictionary(Of String, IUploadProteins.UploadInfo)
        End If

        If m_FileValidList.Count = 0 Then
            Exit Sub
        End If

        lvwValidList.BeginUpdate()
        lvwValidList.Items.Clear()

        For Each item In m_FileValidList

            Dim FileName = Path.GetFileName(item.Key)
            Dim uploadInfo = item.Value

            Dim li = New ListViewItem(FileName)
            li.SubItems.Add(GetOrganismName(uploadInfo.OrganismID))
            li.SubItems.Add(uploadInfo.ProteinCount.ToString)
            li.SubItems.Add(uploadInfo.ExportedProteinCount.ToString)

            lvwValidList.Items.Add(li)
        Next

        lvwValidList.EndUpdate()

    End Sub

    Private Sub DumpDetailedErrorOrWarningList(
      errorList As IReadOnlyCollection(Of clsCustomValidateFastaFiles.udtErrorInfoExtended),
      fastaFileName As String,
      messageType As String)

        Dim SaveDialog As New SaveFileDialog

        Dim SelectedSavePath As String

        Dim errorDetail As clsCustomValidateFastaFiles.udtErrorInfoExtended

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
