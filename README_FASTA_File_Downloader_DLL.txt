DLL Files required for FASTA File Downloader DLL standalone usage in Analysis Manager

	- Protein_Exporter.dll
	- Protein_Storage.dll
	- TableManipulationBase.dll
	
Reference all the above in your project, then you can access the FASTA dumper funtionality
through the 'ExportProteinCollectionsIFC.IGetFASTAFromDMS' interface. Instantiate the object
using the 'clsGetFASTAFromDMS' class.

See <http://prismwiki/index.php/IGetFASTAFromDMS> for function descriptions
