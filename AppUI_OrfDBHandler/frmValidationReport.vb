Public Class frmValidationReport
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        'Me.m_FileList = errorFileList

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
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
    Friend WithEvents cboFileList As System.Windows.Forms.ComboBox
    Friend WithEvents colErrorDescription As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvwErrorList As System.Windows.Forms.ListView
    Friend WithEvents lblErrorList As System.Windows.Forms.Label
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents gbxValidFileList As System.Windows.Forms.GroupBox
    Friend WithEvents gbxInvalidFileList As System.Windows.Forms.GroupBox
    Friend WithEvents lvwValidList As System.Windows.Forms.ListView
    Friend WithEvents colFileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colOrganism As System.Windows.Forms.ColumnHeader
    Friend WithEvents colCount As System.Windows.Forms.ColumnHeader
    Friend WithEvents pgbListViewLoad As System.Windows.Forms.ProgressBar
    Friend WithEvents colNumOccurences As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmdExportErrorDetails As System.Windows.Forms.Button
    Friend WithEvents VisualStyleProvider1 As Skybound.VisualStyles.VisualStyleProvider
    Friend WithEvents colActualCount As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.cboFileList = New System.Windows.Forms.ComboBox
        Me.cmdExportErrorDetails = New System.Windows.Forms.Button
        Me.lblErrorList = New System.Windows.Forms.Label
        Me.cmdClose = New System.Windows.Forms.Button
        Me.lvwErrorList = New System.Windows.Forms.ListView
        Me.colNumOccurences = New System.Windows.Forms.ColumnHeader
        Me.colErrorDescription = New System.Windows.Forms.ColumnHeader
        Me.gbxValidFileList = New System.Windows.Forms.GroupBox
        Me.lvwValidList = New System.Windows.Forms.ListView
        Me.colFileName = New System.Windows.Forms.ColumnHeader
        Me.colOrganism = New System.Windows.Forms.ColumnHeader
        Me.colCount = New System.Windows.Forms.ColumnHeader
        Me.gbxInvalidFileList = New System.Windows.Forms.GroupBox
        Me.pgbListViewLoad = New System.Windows.Forms.ProgressBar
        Me.VisualStyleProvider1 = New Skybound.VisualStyles.VisualStyleProvider
        Me.colActualCount = New System.Windows.Forms.ColumnHeader
        Me.gbxValidFileList.SuspendLayout()
        Me.gbxInvalidFileList.SuspendLayout()
        Me.SuspendLayout()
        '
        'cboFileList
        '
        Me.cboFileList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboFileList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboFileList.Location = New System.Drawing.Point(12, 25)
        Me.cboFileList.Name = "cboFileList"
        Me.cboFileList.Size = New System.Drawing.Size(448, 21)
        Me.cboFileList.TabIndex = 1
        '
        'cmdExportErrorDetails
        '
        Me.cmdExportErrorDetails.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdExportErrorDetails.Location = New System.Drawing.Point(468, 25)
        Me.cmdExportErrorDetails.Name = "cmdExportErrorDetails"
        Me.cmdExportErrorDetails.Size = New System.Drawing.Size(114, 20)
        Me.cmdExportErrorDetails.TabIndex = 3
        Me.cmdExportErrorDetails.Text = "Export Detailed List"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdExportErrorDetails, True)
        '
        'lblErrorList
        '
        Me.lblErrorList.Location = New System.Drawing.Point(12, 56)
        Me.lblErrorList.Name = "lblErrorList"
        Me.lblErrorList.Size = New System.Drawing.Size(406, 16)
        Me.lblErrorList.TabIndex = 4
        Me.lblErrorList.Text = "Recorded Validation Errors"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblErrorList, True)
        '
        'cmdClose
        '
        Me.cmdClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdClose.Location = New System.Drawing.Point(520, 692)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(84, 24)
        Me.cmdClose.TabIndex = 5
        Me.cmdClose.Text = "Close"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdClose, True)
        '
        'lvwErrorList
        '
        Me.lvwErrorList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwErrorList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colNumOccurences, Me.colErrorDescription})
        Me.lvwErrorList.FullRowSelect = True
        Me.lvwErrorList.GridLines = True
        Me.lvwErrorList.Location = New System.Drawing.Point(12, 73)
        Me.lvwErrorList.MultiSelect = False
        Me.lvwErrorList.Name = "lvwErrorList"
        Me.lvwErrorList.Size = New System.Drawing.Size(572, 254)
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
        Me.gbxValidFileList.Size = New System.Drawing.Size(596, 324)
        Me.gbxValidFileList.TabIndex = 7
        Me.gbxValidFileList.TabStop = False
        Me.gbxValidFileList.Text = "FASTA Files Successfully Uploaded"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.gbxValidFileList, True)
        '
        'lvwValidList
        '
        Me.lvwValidList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwValidList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFileName, Me.colOrganism, Me.colCount, Me.colActualCount})
        Me.lvwValidList.FullRowSelect = True
        Me.lvwValidList.GridLines = True
        Me.lvwValidList.Location = New System.Drawing.Point(12, 27)
        Me.lvwValidList.MultiSelect = False
        Me.lvwValidList.Name = "lvwValidList"
        Me.lvwValidList.Size = New System.Drawing.Size(572, 282)
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
        'gbxInvalidFileList
        '
        Me.gbxInvalidFileList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbxInvalidFileList.Controls.Add(Me.lvwErrorList)
        Me.gbxInvalidFileList.Controls.Add(Me.cmdExportErrorDetails)
        Me.gbxInvalidFileList.Controls.Add(Me.cboFileList)
        Me.gbxInvalidFileList.Controls.Add(Me.lblErrorList)
        Me.gbxInvalidFileList.Location = New System.Drawing.Point(10, 342)
        Me.gbxInvalidFileList.Name = "gbxInvalidFileList"
        Me.gbxInvalidFileList.Size = New System.Drawing.Size(596, 340)
        Me.gbxInvalidFileList.TabIndex = 8
        Me.gbxInvalidFileList.TabStop = False
        Me.gbxInvalidFileList.Text = "FASTA Files Not Uploaded Due to Errors"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.gbxInvalidFileList, True)
        '
        'pgbListViewLoad
        '
        Me.pgbListViewLoad.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pgbListViewLoad.Location = New System.Drawing.Point(10, 696)
        Me.pgbListViewLoad.Name = "pgbListViewLoad"
        Me.pgbListViewLoad.Size = New System.Drawing.Size(496, 18)
        Me.pgbListViewLoad.TabIndex = 9
        Me.pgbListViewLoad.Visible = False
        '
        'colActualCount
        '
        Me.colActualCount.Text = "Actual Count"
        Me.colActualCount.Width = 75
        '
        'frmValidationReport
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(614, 724)
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
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private m_ErrorCollection As ArrayList
    Private m_FileErrorList As Hashtable
    Private m_FileValidList As Hashtable
    Private m_SummarizedFileErrors As Hashtable
    Private m_Organisms As DataTable
    Private m_ErrorListLoaded As Boolean

    Private Sub frmValidationReport_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.FillValidListView()
        Me.BindFileListToComboBox(Me.m_FileErrorList)
        Me.cboFileList.SelectedIndex = 0
        Me.cboFileList.Select()
        Me.cboFileList_SelectedIndexChanged(Me, Nothing)

    End Sub

    Private Sub cmdExportErrorDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExportErrorDetails.Click
        Me.DumpDetailedErrorList(Me.m_ErrorCollection, Me.cboFileList.Text)
    End Sub

    Private Sub cboFileList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboFileList.SelectedIndexChanged
        Me.lvwErrorList.Items.Clear()
        If Not Me.m_ErrorCollection Is Nothing Then
            Me.m_ErrorCollection = DirectCast(Me.m_FileErrorList.Item(Me.cboFileList.Text), ArrayList)
            Dim errorSummary As New Hashtable
            errorSummary = DirectCast(Me.m_SummarizedFileErrors.Item(Me.cboFileList.Text), Hashtable)
            Me.FillErrorListView(errorSummary)
        End If
    End Sub

    Friend WriteOnly Property ErrorSummaryList() As Hashtable
        Set(ByVal Value As Hashtable)
            Me.m_SummarizedFileErrors = Value
        End Set
    End Property

    Friend WriteOnly Property FileErrorList() As Hashtable
        Set(ByVal Value As Hashtable)
            Me.m_FileErrorList = Value
        End Set
    End Property

    Friend WriteOnly Property FileValidList() As Hashtable
        Set(ByVal Value As Hashtable)
            Me.m_FileValidList = Value
        End Set
    End Property

    Friend WriteOnly Property OrganismList() As DataTable
        Set(ByVal Value As DataTable)
            Me.m_Organisms = Value
        End Set
    End Property

    Private Function GetOrganismName(ByVal organismID As Integer) As String
        Dim foundrows() As DataRow
        Dim dr As DataRow

        foundrows = Me.m_Organisms.Select("ID = " & organismID.ToString)

        Return foundrows(0).Item("Display_Name").ToString

    End Function

    Private Sub BindFileListToComboBox(ByVal contents As Hashtable)

        Dim counter As IDictionaryEnumerator
        Dim errorCount As Integer
        RemoveHandler cboFileList.SelectedIndexChanged, AddressOf cboFileList_SelectedIndexChanged

        If Not contents Is Nothing Then
            counter = contents.GetEnumerator

            Me.cboFileList.BeginUpdate()

            While counter.MoveNext = True
                errorCount = DirectCast(Me.m_FileErrorList.Item(counter.Key), ArrayList).Count
                Me.cboFileList.Items.Add(counter.Key) '.ToString & " (" & errorCount & " Errors)")
            End While

            Me.cboFileList.EndUpdate()

        Else
            Me.cboFileList.BeginUpdate()
            Me.cboFileList.Items.Add("-- No Errors --")
            Me.cboFileList.EndUpdate()
        End If

        AddHandler cboFileList.SelectedIndexChanged, AddressOf cboFileList_SelectedIndexChanged

    End Sub

    Private Sub FillErrorListView(ByVal errorSummary As Hashtable)
        Dim li As ListViewItem

        Dim counter As IDictionaryEnumerator

        If Not errorSummary Is Nothing Then
            counter = errorSummary.GetEnumerator

            Me.lvwErrorList.BeginUpdate()
            Me.lvwErrorList.Items.Clear()

            While counter.MoveNext = True
                li = New ListViewItem(counter.Value.ToString)
                li.SubItems.Add(counter.Key.ToString)
                Me.lvwErrorList.Items.Add(li)
            End While

            Me.lvwErrorList.EndUpdate()
        End If

    End Sub

    Private Sub FillValidListView()
        Dim li As ListViewItem
        Dim entry As Protein_Uploader.IUploadProteins.UploadInfo
        Dim FileName As String
        Dim counter As IDictionaryEnumerator

        If Me.m_FileValidList Is Nothing Then
            Me.m_FileValidList = New Hashtable
        End If

        If Me.m_FileValidList.Count > 0 Then
            counter = Me.m_FileValidList.GetEnumerator
        Else
            Exit Sub
        End If

        Me.lvwValidList.BeginUpdate()
        Me.lvwValidList.Items.Clear()

        While counter.MoveNext = True
            entry = DirectCast(counter.Value, Protein_Uploader.IUploadProteins.UploadInfo)
            FileName = System.IO.Path.GetFileName(counter.Key.ToString)
            li = New ListViewItem(FileName)
            li.SubItems.Add(Me.GetOrganismName(entry.OrganismID))
            li.SubItems.Add(entry.ProteinCount.ToString)
            li.SubItems.Add(entry.ExportedProteinCount.ToString)
            Me.lvwValidList.Items.Add(li)
        End While

        Me.lvwValidList.EndUpdate()

    End Sub

    Private Sub DumpDetailedErrorList(ByVal errorList As ArrayList, ByVal FASTAFileName As String)

        Dim SaveDialog As New SaveFileDialog
        Dim fileType As Protein_Importer.IImportProteins.ProteinImportFileTypes
        Dim SelectedSavePath As String
        Dim tmpSelectedProteinList As ArrayList
        Dim currentFileName As String

        Dim errorDetail As ValidateFastaFile.ICustomValidation.udtErrorInfoExtended
        Dim fileErrors As ArrayList

        With SaveDialog
            .Title = "Save Protein Database File"
            .DereferenceLinks = True
            .Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            .FilterIndex = 1
            .RestoreDirectory = True
            .OverwritePrompt = True
            .FileName = System.IO.Path.GetFileNameWithoutExtension(FASTAFileName) & "_Errors"
        End With

        If SaveDialog.ShowDialog = DialogResult.OK Then
            SelectedSavePath = SaveDialog.FileName
        Else
            Exit Sub
        End If

        Dim sw As System.IO.StreamWriter = New System.IO.StreamWriter(SelectedSavePath)

        sw.WriteLine("Protein Name" & ControlChars.Tab & "Line Number" & ControlChars.Tab & "Error Type" & ControlChars.Tab & "Error Message")

        For Each errorDetail In errorList
            sw.WriteLine( _
                errorDetail.ProteinName & ControlChars.Tab & _
                errorDetail.LineNumber & ControlChars.Tab & _
                errorDetail.Type & ControlChars.Tab & _
                errorDetail.MessageText)
        Next

        sw.WriteLine("")

        sw.Flush()
        sw.Close()

        sw = Nothing

        MessageBox.Show("Wrote " & errorList.Count & " errors to " & SaveDialog.FileName, "Detailed Error List", MessageBoxButtons.OK)

    End Sub

    Structure HashToArraylistForCombo
        Sub New(ByVal Key As String, ByVal Value As ArrayList)
            Me.Key = Key.ToString & " (" & Value.Count & " Errors)"
            Me.Value = Value
        End Sub
        Public Key As String
        Public Value As ArrayList
    End Structure


End Class
