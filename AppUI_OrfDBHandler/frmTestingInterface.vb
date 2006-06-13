Public Class frmTestingInterface
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

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
    Friend WithEvents txtTransFilePath As System.Windows.Forms.TextBox
    Friend WithEvents gbxTransTableImportTest As System.Windows.Forms.GroupBox
    Friend WithEvents gbxFASTAImportTest As System.Windows.Forms.GroupBox
    Friend WithEvents txtFASTAFilePath As System.Windows.Forms.TextBox
    Friend WithEvents txtConnString As System.Windows.Forms.TextBox
    Friend WithEvents cmdLoadTT As System.Windows.Forms.Button
    Friend WithEvents cmdBrowseTT As System.Windows.Forms.Button
    Friend WithEvents cmdBrowseFF As System.Windows.Forms.Button
    Friend WithEvents cmdLoadFF As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents cmdExportFASTA As System.Windows.Forms.Button
    Friend WithEvents cboCollectionsList As System.Windows.Forms.ComboBox
    Friend WithEvents VisualStyleProvider1 As Skybound.VisualStyles.VisualStyleProvider
    Friend WithEvents gbxConnectionString As System.Windows.Forms.GroupBox
    Friend WithEvents cmdBatchLoadDMS As System.Windows.Forms.Button
    Friend WithEvents gbxOtherStuff As System.Windows.Forms.GroupBox
    Friend WithEvents cmdUpdateArchiveTables As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents lblProgress As System.Windows.Forms.Label
    Friend WithEvents pgbAdminConsole As System.Windows.Forms.ProgressBar
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtTransFilePath = New System.Windows.Forms.TextBox
        Me.cmdLoadTT = New System.Windows.Forms.Button
        Me.cmdBrowseTT = New System.Windows.Forms.Button
        Me.gbxTransTableImportTest = New System.Windows.Forms.GroupBox
        Me.gbxFASTAImportTest = New System.Windows.Forms.GroupBox
        Me.cmdBrowseFF = New System.Windows.Forms.Button
        Me.cmdLoadFF = New System.Windows.Forms.Button
        Me.txtFASTAFilePath = New System.Windows.Forms.TextBox
        Me.txtConnString = New System.Windows.Forms.TextBox
        Me.gbxConnectionString = New System.Windows.Forms.GroupBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Button2 = New System.Windows.Forms.Button
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.cmdBatchLoadDMS = New System.Windows.Forms.Button
        Me.cboCollectionsList = New System.Windows.Forms.ComboBox
        Me.cmdExportFASTA = New System.Windows.Forms.Button
        Me.VisualStyleProvider1 = New Skybound.VisualStyles.VisualStyleProvider
        Me.gbxOtherStuff = New System.Windows.Forms.GroupBox
        Me.cmdUpdateArchiveTables = New System.Windows.Forms.Button
        Me.Button3 = New System.Windows.Forms.Button
        Me.lblProgress = New System.Windows.Forms.Label
        Me.pgbAdminConsole = New System.Windows.Forms.ProgressBar
        Me.gbxTransTableImportTest.SuspendLayout()
        Me.gbxFASTAImportTest.SuspendLayout()
        Me.gbxConnectionString.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.gbxOtherStuff.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtTransFilePath
        '
        Me.txtTransFilePath.Location = New System.Drawing.Point(12, 22)
        Me.txtTransFilePath.Name = "txtTransFilePath"
        Me.txtTransFilePath.Size = New System.Drawing.Size(590, 20)
        Me.txtTransFilePath.TabIndex = 0
        Me.txtTransFilePath.Text = "C:\Documents and Settings\d3k857\My Documents\Visual Studio Projects\Organism Dat" & _
        "abase Handler\Aux_Files\gc.ptr"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.txtTransFilePath, True)
        '
        'cmdLoadTT
        '
        Me.cmdLoadTT.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdLoadTT.Location = New System.Drawing.Point(694, 20)
        Me.cmdLoadTT.Name = "cmdLoadTT"
        Me.cmdLoadTT.TabIndex = 1
        Me.cmdLoadTT.Text = "Load"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdLoadTT, True)
        '
        'cmdBrowseTT
        '
        Me.cmdBrowseTT.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBrowseTT.Location = New System.Drawing.Point(612, 20)
        Me.cmdBrowseTT.Name = "cmdBrowseTT"
        Me.cmdBrowseTT.TabIndex = 3
        Me.cmdBrowseTT.Text = "Browse..."
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdBrowseTT, True)
        '
        'gbxTransTableImportTest
        '
        Me.gbxTransTableImportTest.Controls.Add(Me.cmdBrowseTT)
        Me.gbxTransTableImportTest.Controls.Add(Me.txtTransFilePath)
        Me.gbxTransTableImportTest.Controls.Add(Me.cmdLoadTT)
        Me.gbxTransTableImportTest.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.gbxTransTableImportTest.Location = New System.Drawing.Point(8, 68)
        Me.gbxTransTableImportTest.Name = "gbxTransTableImportTest"
        Me.gbxTransTableImportTest.Size = New System.Drawing.Size(782, 54)
        Me.gbxTransTableImportTest.TabIndex = 5
        Me.gbxTransTableImportTest.TabStop = False
        Me.gbxTransTableImportTest.Text = "Translation Table Import Test"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.gbxTransTableImportTest, True)
        '
        'gbxFASTAImportTest
        '
        Me.gbxFASTAImportTest.Controls.Add(Me.cmdBrowseFF)
        Me.gbxFASTAImportTest.Controls.Add(Me.cmdLoadFF)
        Me.gbxFASTAImportTest.Controls.Add(Me.txtFASTAFilePath)
        Me.gbxFASTAImportTest.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.gbxFASTAImportTest.Location = New System.Drawing.Point(8, 126)
        Me.gbxFASTAImportTest.Name = "gbxFASTAImportTest"
        Me.gbxFASTAImportTest.Size = New System.Drawing.Size(780, 54)
        Me.gbxFASTAImportTest.TabIndex = 6
        Me.gbxFASTAImportTest.TabStop = False
        Me.gbxFASTAImportTest.Text = "FASTA File Import Test"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.gbxFASTAImportTest, True)
        '
        'cmdBrowseFF
        '
        Me.cmdBrowseFF.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBrowseFF.Location = New System.Drawing.Point(614, 20)
        Me.cmdBrowseFF.Name = "cmdBrowseFF"
        Me.cmdBrowseFF.TabIndex = 5
        Me.cmdBrowseFF.Text = "Browse..."
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdBrowseFF, True)
        '
        'cmdLoadFF
        '
        Me.cmdLoadFF.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdLoadFF.Location = New System.Drawing.Point(696, 20)
        Me.cmdLoadFF.Name = "cmdLoadFF"
        Me.cmdLoadFF.TabIndex = 4
        Me.cmdLoadFF.Text = "Load"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdLoadFF, True)
        '
        'txtFASTAFilePath
        '
        Me.txtFASTAFilePath.Location = New System.Drawing.Point(12, 22)
        Me.txtFASTAFilePath.Name = "txtFASTAFilePath"
        Me.txtFASTAFilePath.Size = New System.Drawing.Size(590, 20)
        Me.txtFASTAFilePath.TabIndex = 0
        Me.txtFASTAFilePath.Text = "D:\Org_DB\Shewanella\FASTA\Shewanella_Heme_proteins_2003-11-19.fasta"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.txtFASTAFilePath, True)
        '
        'txtConnString
        '
        Me.txtConnString.Location = New System.Drawing.Point(12, 22)
        Me.txtConnString.Name = "txtConnString"
        Me.txtConnString.Size = New System.Drawing.Size(590, 20)
        Me.txtConnString.TabIndex = 7
        Me.txtConnString.Text = "Data Source=gigasax;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.txtConnString, True)
        '
        'gbxConnectionString
        '
        Me.gbxConnectionString.Controls.Add(Me.txtConnString)
        Me.gbxConnectionString.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.gbxConnectionString.Location = New System.Drawing.Point(8, 10)
        Me.gbxConnectionString.Name = "gbxConnectionString"
        Me.gbxConnectionString.Size = New System.Drawing.Size(780, 54)
        Me.gbxConnectionString.TabIndex = 8
        Me.gbxConnectionString.TabStop = False
        Me.gbxConnectionString.Text = "Connection String"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.gbxConnectionString, True)
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Button2)
        Me.GroupBox2.Controls.Add(Me.TextBox1)
        Me.GroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.GroupBox2.Location = New System.Drawing.Point(8, 188)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(780, 54)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Translation Table Import Test"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.GroupBox2, True)
        '
        'Button2
        '
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Button2.Location = New System.Drawing.Point(696, 20)
        Me.Button2.Name = "Button2"
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Load"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.Button2, True)
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(12, 22)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(590, 20)
        Me.TextBox1.TabIndex = 0
        Me.TextBox1.Text = "1"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.TextBox1, True)
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cmdBatchLoadDMS)
        Me.GroupBox3.Controls.Add(Me.cboCollectionsList)
        Me.GroupBox3.Controls.Add(Me.cmdExportFASTA)
        Me.GroupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.GroupBox3.Location = New System.Drawing.Point(8, 248)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(780, 54)
        Me.GroupBox3.TabIndex = 7
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "FASTA File Import Test"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.GroupBox3, True)
        '
        'cmdBatchLoadDMS
        '
        Me.cmdBatchLoadDMS.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBatchLoadDMS.Location = New System.Drawing.Point(612, 20)
        Me.cmdBatchLoadDMS.Name = "cmdBatchLoadDMS"
        Me.cmdBatchLoadDMS.TabIndex = 9
        Me.cmdBatchLoadDMS.Text = "Batch Load"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdBatchLoadDMS, True)
        '
        'cboCollectionsList
        '
        Me.cboCollectionsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCollectionsList.Location = New System.Drawing.Point(12, 22)
        Me.cboCollectionsList.Name = "cboCollectionsList"
        Me.cboCollectionsList.Size = New System.Drawing.Size(590, 21)
        Me.cboCollectionsList.TabIndex = 5
        '
        'cmdExportFASTA
        '
        Me.cmdExportFASTA.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdExportFASTA.Location = New System.Drawing.Point(696, 20)
        Me.cmdExportFASTA.Name = "cmdExportFASTA"
        Me.cmdExportFASTA.TabIndex = 4
        Me.cmdExportFASTA.Text = "Export"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdExportFASTA, True)
        '
        'gbxOtherStuff
        '
        Me.gbxOtherStuff.Controls.Add(Me.cmdUpdateArchiveTables)
        Me.gbxOtherStuff.Controls.Add(Me.Button3)
        Me.gbxOtherStuff.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.gbxOtherStuff.Location = New System.Drawing.Point(8, 308)
        Me.gbxOtherStuff.Name = "gbxOtherStuff"
        Me.gbxOtherStuff.Size = New System.Drawing.Size(780, 54)
        Me.gbxOtherStuff.TabIndex = 9
        Me.gbxOtherStuff.TabStop = False
        Me.gbxOtherStuff.Text = "Miscellaneous"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.gbxOtherStuff, True)
        '
        'cmdUpdateArchiveTables
        '
        Me.cmdUpdateArchiveTables.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdUpdateArchiveTables.Location = New System.Drawing.Point(566, 18)
        Me.cmdUpdateArchiveTables.Name = "cmdUpdateArchiveTables"
        Me.cmdUpdateArchiveTables.Size = New System.Drawing.Size(120, 23)
        Me.cmdUpdateArchiveTables.TabIndex = 9
        Me.cmdUpdateArchiveTables.Text = "Sync Archive Tables"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdUpdateArchiveTables, True)
        '
        'Button3
        '
        Me.Button3.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Button3.Location = New System.Drawing.Point(696, 18)
        Me.Button3.Name = "Button3"
        Me.Button3.TabIndex = 4
        Me.Button3.Text = "Export"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.Button3, True)
        '
        'lblProgress
        '
        Me.lblProgress.Location = New System.Drawing.Point(8, 368)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(780, 16)
        Me.lblProgress.TabIndex = 11
        Me.lblProgress.Visible = False
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblProgress, True)
        '
        'pgbAdminConsole
        '
        Me.pgbAdminConsole.Location = New System.Drawing.Point(8, 382)
        Me.pgbAdminConsole.Name = "pgbAdminConsole"
        Me.pgbAdminConsole.Size = New System.Drawing.Size(780, 20)
        Me.pgbAdminConsole.TabIndex = 10
        Me.pgbAdminConsole.Visible = False
        '
        'frmTestingInterface
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(796, 408)
        Me.Controls.Add(Me.pgbAdminConsole)
        Me.Controls.Add(Me.gbxOtherStuff)
        Me.Controls.Add(Me.gbxConnectionString)
        Me.Controls.Add(Me.gbxFASTAImportTest)
        Me.Controls.Add(Me.gbxTransTableImportTest)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.lblProgress)
        Me.Name = "frmTestingInterface"
        Me.Text = "Admin Console"
        Me.gbxTransTableImportTest.ResumeLayout(False)
        Me.gbxFASTAImportTest.ResumeLayout(False)
        Me.gbxConnectionString.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.gbxOtherStuff.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    '<System.STAThread()> Public Shared Sub Main()
    '    'System.Windows.Forms.Application.EnableVisualStyles()
    '    Skybound.VisualStyles.VisualStyleProvider.EnableVisualStyles()
    '    System.Windows.Forms.Application.DoEvents()
    '    System.Windows.Forms.Application.Run(New frmTestingInterface)  ' replace frmDecode by the name of your form!!!
    'End Sub


#End Region

    Private WithEvents importer As Protein_Importer.IImportProteins
    Private collectionList As DataTable
    Private m_LastOutputDirectory As String = "D:\outbox\output_test\"
    Private m_AppPath As String = Application.ExecutablePath
    Protected WithEvents m_Syncer As clsSyncFASTAFileArchive
    Protected WithEvents m_Exporter As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS
    Protected m_FullOutputPath As String

    Private m_TaskMessage As String
    Private m_ProgressMessage As String


    'Private exporter As ExportCollectionsFromDMS.IExportCollectionsFromDMS

    Private Sub frmTestingInterface_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        importer = New Protein_Importer.clsImportHandler(Me.txtConnString.Text)
        collectionList = importer.LoadProteinCollections()
        With Me.cboCollectionsList
            .DataSource = collectionList
            .DisplayMember = "Display"
            .ValueMember = "Protein_Collection_ID"
        End With

    End Sub

    Private Sub cmdLoadTT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoadTT.Click
        Dim transhandler As TranslationTableImport.ITransTableImport
        transhandler = New TranslationTableImport.clsTransTableHandler(Me.txtConnString.Text)
        transhandler.GetAllTranslationTableEntries(Me.txtTransFilePath.Text)
    End Sub

    Private Sub cmdLoadFF_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoadFF.Click
        Dim importHandler As Protein_Importer.IImportProteins = New Protein_Importer.clsImportHandler(Me.txtConnString.Text)

        'importHandler.LoadProteins(Me.txtFASTAFilePath.Text, "", Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA, 4, 1)

        'Dim FASTAHandler As Protein_Importer.IReadFASTA
        ''FASTAHandler = New Protein_Importer.FASTAReader
        'Dim sqlData As TableManipulationBase.IGetSQLData
        'sqlData = New TableManipulationBase.clsDBTask(Me.txtConnString.Text, True)



        'Dim SQL As String = "SELECT * FROM " & "T_Proteins"
        'Dim dmsDA As SqlClient.SqlDataAdapter = New SqlClient.SqlDataAdapter(SQL, sqlData.Connection)
        'Dim dmsCB As SqlClient.SqlCommandBuilder = New SqlClient.SqlCommandBuilder(dmsDA)


        'FASTAHandler.ProteinTable = sqlData.GetTable(SQL, dmsDA, dmsCB)

        'FASTAHandler.GetProteinEntries(Me.txtFASTAFilePath.Text)

        'Dim dr As DataRow
        'For Each dr In FASTAHandler.ProteinTable

        'Next


        'dmsDA.Update(FASTAHandler.ProteinTable)


    End Sub

    Private Sub cmdBrowseTT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowseTT.Click, cmdBrowseFF.Click
        Dim newFilePath As String
        Dim OpenDialog As New OpenFileDialog

        Dim proxy As Button = DirectCast(sender, Button)

        With OpenDialog
            .Title = "Open Translation Definitions File"
            .DereferenceLinks = False
            .InitialDirectory = "D:\Org_DB\"
            .Filter = "All files (*.*)|*.*"
            .FilterIndex = 1
            .RestoreDirectory = True
            .Multiselect = False
        End With

        If OpenDialog.ShowDialog = DialogResult.OK Then
            newFilePath = OpenDialog.FileName
            If proxy.Name.ToString = "cmdBrowseTT" Then
                Me.txtTransFilePath.Text = newFilePath
            ElseIf proxy.Name.ToString = "cmdBrowseFF" Then
                Me.txtFASTAFilePath.Text = newFilePath
            End If


        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim trans As NucleotideTranslator.ITranslateNucleotides = _
            New NucleotideTranslator.clsTranslateNucleotides(Me.txtConnString.Text)

        trans.LoadMatrix(1)
    End Sub

    Private Sub cmdExportFASTA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExportFASTA.Click
        Dim sd As New FolderBrowserDialog

        Dim filePath As String
        Dim r As DialogResult
        Dim fingerprint As String
        Dim outputFI As System.IO.FileInfo
        Dim destFI As System.IO.FileInfo
        'Dim exporter As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS
        'Dim tmpNameList As New ArrayList

        'sd.DefaultExt = ".fasta"
        'sd.FileName = Me.cboCollectionsList.Text + ".fasta"

        sd.SelectedPath = Me.m_LastOutputDirectory

        r = sd.ShowDialog
        Dim destPath As String

        If r = DialogResult.OK Then
            filePath = sd.SelectedPath
            Me.m_LastOutputDirectory = filePath

            'tmpNameList.Add(Me.cboCollectionsList.Text.ToString)
            'exporter = New Protein_Exporter.clsGetFASTAFromDMS(Me.txtConnString.Text, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward_sequence)
            'exporter = New Protein_Exporter.clsGetFASTAFromDMS( _
            '    Me.txtConnString.Text, _
            '    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA, _
            '    Me.GetCollectionName(CInt(Me.cboCollectionsList.SelectedValue)) + "_scrambled.fasta")

            'Me.m_Exporter = New Protein_Exporter.clsGetFASTAFromDMS( _
            '    Me.txtConnString.Text, _
            '    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta, _
            '    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)
            Me.m_Exporter = New Protein_Exporter.clsGetFASTAFromDMS(Me.txtConnString.Text)
            'fingerprint = Me.m_Exporter.ExportFASTAFile("na", "na", "HCMV_2003+H_sapiens_IPI_2005-04-04.fasta", filePath)
            fingerprint = Me.m_Exporter.ExportFASTAFile("na", "na", "Shewanella_2003-12-19.fasta", filePath)
            'fingerprint = Me.m_Exporter.ExportFASTAFile(Me.GetCollectionName(CInt(Me.cboCollectionsList.SelectedValue)), "seq_direction=forward,filetype=fasta", "na", filePath)
            'fingerprint = Me.m_Exporter.ExportFASTAFile("PCQ_ETJ_2004-01-21", "seq_direction=forward,filetype=fasta", "na", filePath)
            'fingerprint = Me.m_Exporter.ExportFASTAFile(Me.GetCollectionName(CInt(Me.cboCollectionsList.SelectedValue)), "seq_direction=reversed,filetype=fasta", "na", filePath)

            'fingerprint = exporter.ExportFASTAFile(CInt(Me.cboCollectionsList.SelectedValue), filePath)
            'exporter = New ExportCollectionsFromDMS.clsExportCollectionsFromDMS(Me.txtConnString.Text, ExportCollectionsFromDMS.IExportCollectionsFromDMS.ExportClasses.clsExportProteinsXTFASTA)
            'fingerprint = exporter.Export(CInt(Me.cboCollectionsList.SelectedValue), filePath)


            'outputFI = New System.IO.FileInfo(Me.m_FullOutputPath)
            'destPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(outputFI.FullName), Me.GetCollectionName(CInt(Me.cboCollectionsList.SelectedValue)) + ".fasta")
            'destFI = New System.IO.FileInfo(destPath)
            'If destFI.Exists Then
            '    destFI.Delete()
            'End If
            'outputFI.MoveTo(destPath)
        End If

    End Sub

    Private Function GetCollectionName(ByVal ProteinCollectionID As Integer) As String
        Dim foundRows() As DataRow
        foundRows = Me.collectionList.Select("Protein_Collection_ID = " & ProteinCollectionID.ToString)
        Return foundRows(0).Item("FileName").ToString
    End Function

    Private Sub cmdBatchLoadDMS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim fileBatcher As Protein_Uploader.clsBatchUploadFromFileList = _
            New Protein_Uploader.clsBatchUploadFromFileList(Me.txtConnString.Text)
        fileBatcher.UploadBatch()
    End Sub

    Private Sub cmdBatchLoadDMS_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBatchLoadDMS.Click

    End Sub

    Private Sub cmdUpdateArchiveTables_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdateArchiveTables.Click
        Dim f As FolderBrowserDialog = New FolderBrowserDialog
        Dim r As DialogResult
        Dim outputPath As String

        If Me.m_Syncer Is Nothing Then
            Me.m_Syncer = New clsSyncFASTAFileArchive(Me.txtConnString.Text)
        End If



        'With f
        '    .RootFolder = Environment.SpecialFolder.MyComputer
        '    .ShowNewFolderButton = True

        '    r = .ShowDialog()
        'End With

        'If r = DialogResult.OK Then
        '    outputPath = f.SelectedPath
        'End If

        'Dim errorCode As Integer

        'errorCode = Me.m_Syncer.SyncCollectionsAndArchiveTables(outputPath)
        Me.m_Syncer.UpdateSHA1Hashes()


    End Sub

    Private Sub StartTask(ByVal StatusMsg As String) Handles m_Syncer.SyncStart, m_Exporter.FileGenerationStarted
        Me.pgbAdminConsole.Visible = True
        Me.lblProgress.Visible = True

        Me.pgbAdminConsole.Value = 0

        Me.m_TaskMessage = StatusMsg
        Me.lblProgress.Text = Me.m_TaskMessage
        Application.DoEvents()

    End Sub

    Private Sub UpdateProgress(ByVal StatusMsg As String, ByVal fractionDone As Double) Handles m_Syncer.SyncProgress, m_Exporter.FileGenerationProgress
        Me.m_ProgressMessage = StatusMsg
        Dim percentComplete As Integer = CInt(fractionDone * 100)
        If fractionDone > 0 Then
            Me.pgbAdminConsole.Value = CInt(fractionDone * 100)
            Me.lblProgress.Text = Me.m_TaskMessage & " (" & percentComplete & "% completed): " & Me.m_ProgressMessage
        Else
            Me.lblProgress.Text = Me.m_TaskMessage & ": " & Me.m_ProgressMessage
        End If
        Application.DoEvents()
    End Sub

    Private Sub CompletedTask() Handles m_Syncer.SyncComplete
        Me.pgbAdminConsole.Visible = False
        Me.lblProgress.Text = ""
        Me.lblProgress.Visible = False
    End Sub

    Private Sub CompletedTask(ByVal fullOutputPath As String) Handles m_Exporter.FileGenerationCompleted
        Me.pgbAdminConsole.Visible = False
        Me.lblProgress.Text = _
            "Wrote " + _
            System.IO.Path.GetFileName(fullOutputPath) + _
            " to " + System.IO.Path.GetDirectoryName(fullOutputPath)
        Me.lblProgress.Visible = False
        Me.m_FullOutputPath = fullOutputPath
    End Sub

End Class
