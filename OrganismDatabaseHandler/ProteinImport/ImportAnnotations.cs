using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OrganismDatabaseHandler.ProteinImport
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
        private string mID;
        private string mName;
        private string mNamespace;
        private string mDefinition;
        private string mComment;
        private bool mIsObsolete;
        private ArrayList mExactSynonym;
        private ArrayList mIsA;
        private ArrayList mXRefAnalog;
        private ArrayList mRelationship;
        private ArrayList mSubSet;

        public string ID
        {
            get => mID;
            set => mID = CleanUpLine(value);
        }

        public string Name
        {
            get => mName;
            set => mName = CleanUpLine(value);
        }

        public string NameSpace
        {
            get => mNamespace;
            set => mNamespace = CleanUpLine(value);
        }

        public string Definition
        {
            get => mDefinition;
            set => mDefinition = CleanUpLine(value);
        }

        public string Comment
        {
            get => mComment;
            set => mComment = CleanUpLine(value);
        }

        public bool IsObsolete
        {
            get => mIsObsolete;
            set => mIsObsolete = value;
        }

        public ArrayList ExactSynonymList => mExactSynonym;

        public ArrayList IsA_List => mIsA;

        public ArrayList XRefAnalog_List => mXRefAnalog;

        public ArrayList Relationship => mRelationship;

        public ArrayList SubSet => mSubSet;

        public void Add_ExactSynonymEntry(string synonym)
        {
            mExactSynonym.Add(CleanUpLine(synonym));
        }

        public void Add_IsA_Entry(string IsAReference)
        {
            mIsA.Add(IsAReference);
        }

        public void Add_XRefAnalog_Entry(string XRef)
        {
            mXRefAnalog.Add(XRef);
        }

        public void Add_RelationShip_Entry(string relationshipEntry)
        {
            mRelationship.Add(relationshipEntry);
        }

        public void Add_Subset_Entry(string subsetEntry)
        {
            mSubSet.Add(subsetEntry);
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