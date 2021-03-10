using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Protein_Importer
{
    public interface IImportAnnotations
    {

        // Structure GOInfoCategories
        // End Structure

    }

    public abstract class ImportAnnotationsBase
    {
        public ImportAnnotationsBase()
        {
        }
    }

    public class GeneOntologyEntry
    {
        private string m_ID;
        private string m_Name;
        private string m_Namespace;
        private string m_Definition;
        private string m_Comment;
        private bool m_IsObsolete;
        private ArrayList m_ExactSynonym;
        private ArrayList m_IsA;
        private ArrayList m_XRefAnalog;
        private ArrayList m_Relationship;
        private ArrayList m_SubSet;

        public string ID
        {
            get => m_ID;
            set => m_ID = CleanUpLine(value);
        }

        public string Name
        {
            get => m_Name;
            set => m_Name = CleanUpLine(value);
        }

        public string NameSpace
        {
            get => m_Namespace;
            set => m_Namespace = CleanUpLine(value);
        }

        public string Definition
        {
            get => m_Definition;
            set => m_Definition = CleanUpLine(value);
        }

        public string Comment
        {
            get => m_Comment;
            set => m_Comment = CleanUpLine(value);
        }

        public bool IsObsolete
        {
            get => m_IsObsolete;
            set => m_IsObsolete = value;
        }

        public ArrayList ExactSynonym_List => m_ExactSynonym;

        public ArrayList IsA_List => m_IsA;

        public ArrayList XRefAnalog_List => m_XRefAnalog;

        public ArrayList Relationship => m_Relationship;

        public ArrayList SubSet => m_SubSet;

        public void Add_ExactSynonym_Entry(string synonym)
        {
            m_ExactSynonym.Add(CleanUpLine(synonym));
        }

        public void Add_IsA_Entry(string IsAReference)
        {
            m_IsA.Add(IsAReference);
        }

        public void Add_XRefAnalog_Entry(string XRef)
        {
            m_XRefAnalog.Add(XRef);
        }

        public void Add_RelationShip_Entry(string relationshipEntry)
        {
            m_Relationship.Add(relationshipEntry);
        }

        public void Add_Subset_Entry(string subsetEntry)
        {
            m_SubSet.Add(subsetEntry);
        }

        private string CleanUpLine(string entryLine)
        {
            string tmpEntryLine = entryLine.Replace("\\", "");
            return tmpEntryLine;
        }
    }

    public class GeneOntologyListOBO
    {

        #region "Regular Expressions"
        private Regex r_entryHeader;
        private Regex r_IDLine;
        private Regex r_NameLine;
        private Regex r_NameSpaceLine;
        private Regex r_DefinitionLine;
        private Regex r_CommentLine;
        private Regex r_IsObsoleteLine;
        private Regex r_ExactSynonymLine;
        private Regex r_IsALine;
        private Regex r_XRefAnalogLine;
        private Regex r_RelationshipLine;
        private Regex r_SubsetLine;
        #endregion

        // Send it the text block from a single entry
        public GeneOntologyListOBO(List<string> GOEntryText)
        {
        }

        public GeneOntologyListOBO()
        {
        }

        protected void ProcessEntry(List<string> EntryCollection)
        {
        }

        private void SetupRegexes()
        {
            var reOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;

            r_entryHeader = new Regex(
                @"^\[Term\]$",
                reOptions);
            r_IDLine = new Regex(
                @"^(?<tag>id):\s+(?<value>.+)$",
                reOptions);
            r_NameLine = new Regex(
                @"^(?<tag>name):\s+(?<value>.+)$",
                reOptions);
            r_NameSpaceLine = new Regex(
                @"^(?<tag>namespace):\s+(?<value>.+)$",
                reOptions);
            r_DefinitionLine = new Regex(
                @"^(?<tag>def):\s+\""+(?<value>.*)\""\s*\[*(?<xref>.*)\]*\s*",
                reOptions);
            r_CommentLine = new Regex(
                @"^(?<tag>comment):\s+(?<value>.+)$",
                reOptions);
            r_IsObsoleteLine = new Regex(
                @"^(?<tag>is_obsolete):\s+(?<value>true|false)$",
                reOptions);
            r_ExactSynonymLine = new Regex(
                @"^(?<tag>exact_synonym):\s+\""+(?<value>.*)\""\s*\[(?<xref>.*)\]S*$",
                reOptions);
            r_IsALine = new Regex(
                @"^(?<tag>is_a):\s+(?<value>\S+)\s*\!.*$",
                reOptions);
            r_RelationshipLine = new Regex(
                @"^(?<tag>relationship):\s+part_of\s+(?<value>\S+)\s*\!.*$",
                reOptions);
            r_XRefAnalogLine = new Regex(
                @"^(?<tag>xref_analog):\s+(?<value>.+)$",
                reOptions);
            r_SubsetLine = new Regex(
                @"^(?<tag>subset):\s+(?<value>.+)$",
                reOptions);
        }
    }
}