Imports System.IO
Imports NucleotideTranslator
Imports Protein_Exporter
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports Protein_Importer
Imports Protein_Uploader
Imports TranslationTableImport

Public Class frmTestingInterface
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
    Friend WithEvents txtTransFilePath As TextBox
    Friend WithEvents gbxTransTableImportTest As GroupBox
    Friend WithEvents gbxFASTAImportTest As GroupBox
    Friend WithEvents txtFASTAFilePath As TextBox
    Friend WithEvents txtConnString As TextBox
    Friend WithEvents cmdLoadTT As Button
    Friend WithEvents cmdBrowseTT As Button
    Friend WithEvents cmdBrowseFF As Button
    Friend WithEvents cmdLoadFF As Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Button2 As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents cmdExportFASTA As Button
    Friend WithEvents cboCollectionsList As ComboBox
    Friend WithEvents gbxConnectionString As GroupBox
    Friend WithEvents cmdBatchLoadDMS As Button
    Friend WithEvents gbxOtherStuff As GroupBox
    Friend WithEvents cmdUpdateArchiveTables As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents lblProgress As Label
    Friend WithEvents pgbAdminConsole As ProgressBar
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtTransFilePath = New TextBox
        Me.cmdLoadTT = New Button
        Me.cmdBrowseTT = New Button
        Me.gbxTransTableImportTest = New GroupBox
        Me.gbxFASTAImportTest = New GroupBox
        Me.cmdBrowseFF = New Button
        Me.cmdLoadFF = New Button
        Me.txtFASTAFilePath = New TextBox
        Me.txtConnString = New TextBox
        Me.gbxConnectionString = New GroupBox
        Me.GroupBox2 = New GroupBox
        Me.Button2 = New Button
        Me.TextBox1 = New TextBox
        Me.GroupBox3 = New GroupBox
        Me.cmdBatchLoadDMS = New Button
        Me.cboCollectionsList = New ComboBox
        Me.cmdExportFASTA = New Button
        Me.gbxOtherStuff = New GroupBox
        Me.cmdUpdateArchiveTables = New Button
        Me.Button3 = New Button
        Me.lblProgress = New Label
        Me.pgbAdminConsole = New ProgressBar
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
        Me.txtTransFilePath.Text = "C:\Documents and Settings\d3k857\My Documents\Visual Studio Projects\Organism Dat" &
        "abase Handler\Aux_Files\gc.ptr"
        '
        'cmdLoadTT
        '
        Me.cmdLoadTT.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdLoadTT.Location = New System.Drawing.Point(694, 20)
        Me.cmdLoadTT.Name = "cmdLoadTT"
        Me.cmdLoadTT.TabIndex = 1
        Me.cmdLoadTT.Text = "Load"
        '
        'cmdBrowseTT
        '
        Me.cmdBrowseTT.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBrowseTT.Location = New System.Drawing.Point(612, 20)
        Me.cmdBrowseTT.Name = "cmdBrowseTT"
        Me.cmdBrowseTT.TabIndex = 3
        Me.cmdBrowseTT.Text = "Browse..."
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
        '
        'cmdBrowseFF
        '
        Me.cmdBrowseFF.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBrowseFF.Location = New System.Drawing.Point(614, 20)
        Me.cmdBrowseFF.Name = "cmdBrowseFF"
        Me.cmdBrowseFF.TabIndex = 5
        Me.cmdBrowseFF.Text = "Browse..."
        '
        'cmdLoadFF
        '
        Me.cmdLoadFF.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdLoadFF.Location = New System.Drawing.Point(696, 20)
        Me.cmdLoadFF.Name = "cmdLoadFF"
        Me.cmdLoadFF.TabIndex = 4
        Me.cmdLoadFF.Text = "Load"
        '
        'txtFASTAFilePath
        '
        Me.txtFASTAFilePath.Location = New System.Drawing.Point(12, 22)
        Me.txtFASTAFilePath.Name = "txtFASTAFilePath"
        Me.txtFASTAFilePath.Size = New System.Drawing.Size(590, 20)
        Me.txtFASTAFilePath.TabIndex = 0
        Me.txtFASTAFilePath.Text = "D:\Org_DB\Shewanella\FASTA\Shewanella_Heme_proteins_2003-11-19.fasta"
        '
        'txtConnString
        '
        Me.txtConnString.Location = New System.Drawing.Point(12, 22)
        Me.txtConnString.Name = "txtConnString"
        Me.txtConnString.Size = New System.Drawing.Size(590, 20)
        Me.txtConnString.TabIndex = 7
        Me.txtConnString.Text = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSP" &
        "I;"
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
        '
        'Button2
        '
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Button2.Location = New System.Drawing.Point(696, 20)
        Me.Button2.Name = "Button2"
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Load"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(12, 22)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(590, 20)
        Me.TextBox1.TabIndex = 0
        Me.TextBox1.Text = "1"
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
        '
        'cmdBatchLoadDMS
        '
        Me.cmdBatchLoadDMS.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBatchLoadDMS.Location = New System.Drawing.Point(612, 20)
        Me.cmdBatchLoadDMS.Name = "cmdBatchLoadDMS"
        Me.cmdBatchLoadDMS.TabIndex = 9
        Me.cmdBatchLoadDMS.Text = "Batch Load"
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
        '
        'cmdUpdateArchiveTables
        '
        Me.cmdUpdateArchiveTables.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdUpdateArchiveTables.Location = New System.Drawing.Point(566, 18)
        Me.cmdUpdateArchiveTables.Name = "cmdUpdateArchiveTables"
        Me.cmdUpdateArchiveTables.Size = New System.Drawing.Size(120, 23)
        Me.cmdUpdateArchiveTables.TabIndex = 9
        Me.cmdUpdateArchiveTables.Text = "Sync Archive Tables"
        '
        'Button3
        '
        Me.Button3.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.Button3.Location = New System.Drawing.Point(696, 18)
        Me.Button3.Name = "Button3"
        Me.Button3.TabIndex = 4
        Me.Button3.Text = "Export"
        '
        'lblProgress
        '
        Me.lblProgress.Location = New System.Drawing.Point(8, 368)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(780, 16)
        Me.lblProgress.TabIndex = 11
        Me.lblProgress.Text = "No Progress Status Yet"
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
        Me.Controls.Add(Me.lblProgress)
        Me.Controls.Add(Me.pgbAdminConsole)
        Me.Controls.Add(Me.gbxOtherStuff)
        Me.Controls.Add(Me.gbxConnectionString)
        Me.Controls.Add(Me.gbxFASTAImportTest)
        Me.Controls.Add(Me.gbxTransTableImportTest)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox3)
        Me.Name = "frmTestingInterface"
        Me.Text = "-"
        Me.gbxTransTableImportTest.ResumeLayout(False)
        Me.gbxFASTAImportTest.ResumeLayout(False)
        Me.gbxConnectionString.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.gbxOtherStuff.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub


#End Region

    Private WithEvents importer As IImportProteins
    Private collectionList As DataTable
    Private m_LastOutputDirectory As String = "D:\outbox\output_test\"
    Private m_AppPath As String = Application.ExecutablePath
    Protected WithEvents m_Syncer As clsSyncFASTAFileArchive
    Protected WithEvents m_Exporter As IGetFASTAFromDMS
    Protected m_FullOutputPath As String

    Private m_TaskMessage As String
    Private m_ProgressMessage As String


    'Private exporter As ExportCollectionsFromDMS.IExportCollectionsFromDMS

    Private Sub frmTestingInterface_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        importer = New clsImportHandler(txtConnString.Text)
        collectionList = importer.LoadProteinCollections()
        With cboCollectionsList
            .DataSource = collectionList
            .DisplayMember = "Display"
            .ValueMember = "Protein_Collection_ID"
        End With

    End Sub

    Private Sub cmdLoadTT_Click(sender As Object, e As EventArgs) Handles cmdLoadTT.Click
        Dim transhandler As ITransTableImport
        transhandler = New clsTransTableHandler(txtConnString.Text)
        transhandler.GetAllTranslationTableEntries(txtTransFilePath.Text)
    End Sub

    Private Sub cmdLoadFF_Click(sender As Object, e As EventArgs) Handles cmdLoadFF.Click
        Dim importHandler As IImportProteins = New clsImportHandler(txtConnString.Text)

        'importHandler.LoadProteins(txtFASTAFilePath.Text, "", Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA, 4, 1)

        'Dim FASTAHandler As Protein_Importer.IReadFASTA
        ''FASTAHandler = New Protein_Importer.FASTAReader
        'Dim sqlData As TableManipulationBase.IGetSQLData
        'sqlData = New TableManipulationBase.clsDBTask(txtConnString.Text, True)



        'Dim SQL As String = "SELECT * FROM " & "T_Proteins"
        'Dim dmsDA As SqlClient.SqlDataAdapter = New SqlClient.SqlDataAdapter(SQL, sqlData.Connection)
        'Dim dmsCB As SqlClient.SqlCommandBuilder = New SqlClient.SqlCommandBuilder(dmsDA)


        'FASTAHandler.ProteinTable = sqlData.GetTable(SQL, dmsDA, dmsCB)

        'FASTAHandler.GetProteinEntries(txtFASTAFilePath.Text)

        'Dim dr As DataRow
        'For Each dr In FASTAHandler.ProteinTable

        'Next


        'dmsDA.Update(FASTAHandler.ProteinTable)


    End Sub

    Private Sub cmdBrowseTT_Click(sender As Object, e As EventArgs) Handles cmdBrowseTT.Click, cmdBrowseFF.Click
        Dim newFilePath As String
        Dim OpenDialog As New OpenFileDialog

        Dim proxy = DirectCast(sender, Button)

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
                txtTransFilePath.Text = newFilePath
            ElseIf proxy.Name.ToString = "cmdBrowseFF" Then
                txtFASTAFilePath.Text = newFilePath
            End If


        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim trans As ITranslateNucleotides =
            New clsTranslateNucleotides(txtConnString.Text)

        trans.LoadMatrix(1)
    End Sub

    Private Sub cmdExportFASTA_Click(sender As Object, e As EventArgs) Handles cmdExportFASTA.Click
        Dim sd As New FolderBrowserDialog

        Dim filePath As String
        Dim r As DialogResult

        'Dim exporter As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS
        'Dim tmpNameList As New ArrayList

        'sd.DefaultExt = ".fasta"
        'sd.FileName = cboCollectionsList.Text + ".fasta"

        sd.SelectedPath = m_LastOutputDirectory

        r = sd.ShowDialog


        If r = DialogResult.OK Then
            filePath = sd.SelectedPath
            m_LastOutputDirectory = filePath

            'tmpNameList.Add(cboCollectionsList.Text.ToString)
            'exporter = New Protein_Exporter.clsGetFASTAFromDMS(txtConnString.Text, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward_sequence)
            'exporter = New Protein_Exporter.clsGetFASTAFromDMS(
            '    txtConnString.Text,
            '    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA,
            '    GetCollectionName(CInt(cboCollectionsList.SelectedValue)) + "_scrambled.fasta")

            'm_Exporter = New Protein_Exporter.clsGetFASTAFromDMS(
            '    txtConnString.Text,
            '    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta,
            '    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)
            m_Exporter = New clsGetFASTAFromDMS(txtConnString.Text)

            'True Legacy fasta file
            'fingerprint = m_Exporter.ExportFASTAFile("na", "na", "HCMV_2003+H_sapiens_IPI_2005-04-04.fasta", filePath)

            'Legacy fasta file with existing protein collection
            'fingerprint = m_Exporter.ExportFASTAFile("Shewanella_2003-12-19", "seq_direction=forward,filetype=fasta", "Shewanella_2003-12-19.fasta", filePath)

            'Legacy fasta file with existing protein collection
            'fingerprint = m_Exporter.ExportFASTAFile("M_Musculus_2007-10-24_IPI,Y_pestis_CO92_2006-05-22,Y_pestis_PestoidesF_2006-05-23,Y_pseudotuberculosis_All_2005-08-25", "seq_direction=forward,filetype=fasta", "na", filePath)

            'Legacy fasta file with existing protein collection
            'fingerprint = m_Exporter.ExportFASTAFile("H_sapiens_IPI_2008-02-07", "seq_direction=decoy", "na", filePath)

            'Legacy fasta file with existing protein collection
            'fingerprint = m_Exporter.ExportFASTAFile("na", "na", "Shewanella_2003-12-19.fasta", filePath)
            m_Exporter.ExportFASTAFile("na", "na", "GOs_Surface_Sargasso_Meso_2009-02-11_24.fasta", filePath)

            'Collection of existing collections
            'fingerprint = m_Exporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=forward,filetype=fasta", "", filePath)
            m_Exporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=reversed,filetype=fasta", "", filePath)

            'fingerprint = m_Exporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=decoy,filetype=fasta", "", filePath)
            m_Exporter.ExportFASTAFile("SAR116_RBH_AA_012809", "seq_direction=forward,filetype=fasta", "", filePath)

            m_Exporter.ExportFASTAFile("Phycomyces_blakesleeanus_v2_filtered_2009-12-16", "seq_direction=forward,filetype=fasta", "", filePath)

            'Protein collection from cbo exported forward
            'fingerprint = m_Exporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=forward,filetype=fasta", "na", filePath)

            'Protein Collection from cbo exported reversed
            ' fingerprint = m_Exporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=reversed,filetype=fasta", "na", filePath)

            'Protein Collection from cbo exported scrambled
            ' fingerprint = m_Exporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=scrambled,filetype=fasta", "na", filePath)

            'fingerprint = exporter.ExportFASTAFile(CInt(cboCollectionsList.SelectedValue), filePath)
            'exporter = New ExportCollectionsFromDMS.clsExportCollectionsFromDMS(txtConnString.Text, ExportCollectionsFromDMS.IExportCollectionsFromDMS.ExportClasses.clsExportProteinsXTFASTA)
            'fingerprint = exporter.Export(CInt(cboCollectionsList.SelectedValue), filePath)


            'outputFI = New System.IO.FileInfo(m_FullOutputPath)
            'destPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(outputFI.FullName), GetCollectionName(CInt(cboCollectionsList.SelectedValue)) + ".fasta")
            'destFI = New System.IO.FileInfo(destPath)
            'If destFI.Exists Then
            '    destFI.Delete()
            'End If
            'outputFI.MoveTo(destPath)
        End If

    End Sub

    Private Function GetCollectionName(ProteinCollectionID As Integer) As String
        Dim foundRows() As DataRow
        foundRows = collectionList.Select("Protein_Collection_ID = " & ProteinCollectionID.ToString)
        Return foundRows(0).Item("FileName").ToString
    End Function

    Private Sub cmdBatchLoadDMS_Click(sender As Object, e As EventArgs)
        Dim fileBatcher As clsBatchUploadFromFileList =
            New clsBatchUploadFromFileList(txtConnString.Text)
        fileBatcher.UploadBatch()
    End Sub

    Private Sub cmdBatchLoadDMS_Click_1(sender As Object, e As EventArgs) Handles cmdBatchLoadDMS.Click

    End Sub

    Private Sub cmdUpdateArchiveTables_Click(sender As Object, e As EventArgs) Handles cmdUpdateArchiveTables.Click

        If m_Syncer Is Nothing Then
            m_Syncer = New clsSyncFASTAFileArchive(txtConnString.Text)
        End If


        'Dim r As DialogResult
        'Dim outputPath As String
        'With f
        '    .RootFolder = Environment.SpecialFolder.MyComputer
        '    .ShowNewFolderButton = True

        '    r = .ShowDialog()
        'End With

        'If r = DialogResult.OK Then
        '    outputPath = f.SelectedPath
        'End If

        'Dim errorCode As Integer

        'errorCode = m_Syncer.SyncCollectionsAndArchiveTables(outputPath)
        m_Syncer.UpdateSHA1Hashes()


    End Sub

    Private Sub StartTask(statusMsg As String) Handles m_Syncer.SyncStart, m_Exporter.FileGenerationStarted
        pgbAdminConsole.Visible = True
        lblProgress.Visible = True

        pgbAdminConsole.Value = 0

        m_TaskMessage = statusMsg
        lblProgress.Text = m_TaskMessage
        Application.DoEvents()

    End Sub

    Private Sub UpdateProgress(statusMsg As String, fractionDone As Double) Handles m_Syncer.SyncProgress, m_Exporter.FileGenerationProgress
        m_ProgressMessage = statusMsg
        Dim percentComplete = CInt(fractionDone * 100)
        If fractionDone > 0 Then
            pgbAdminConsole.Value = CInt(fractionDone * 100)
            lblProgress.Text = m_TaskMessage & " (" & percentComplete & "% completed): " & m_ProgressMessage
        Else
            lblProgress.Text = m_TaskMessage & ": " & m_ProgressMessage
        End If
        Application.DoEvents()
    End Sub

    Private Sub CompletedTask() Handles m_Syncer.SyncComplete
        pgbAdminConsole.Visible = False
        lblProgress.Text = ""
        lblProgress.Visible = False
    End Sub

    Private Sub CompletedTask(fullOutputPath As String) Handles m_Exporter.FileGenerationCompleted
        pgbAdminConsole.Visible = False
        lblProgress.Text =
            "Wrote " +
            Path.GetFileName(fullOutputPath) +
            " to " + Path.GetDirectoryName(fullOutputPath)
        lblProgress.Visible = True
        m_FullOutputPath = fullOutputPath
    End Sub

End Class
