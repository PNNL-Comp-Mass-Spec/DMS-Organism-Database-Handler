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
        private string mId;
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

        public string Id
        {
            get => mId;
            set => mId = CleanUpLine(value);
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

        public void Add_IsA_Entry(string isAReference)
        {
            mIsA.Add(isAReference);
        }

        public void Add_XRefAnalog_Entry(string xref)
        {
            mXRefAnalog.Add(xref);
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
        private Regex rEntryHeader;
        private Regex rIdLine;
        private Regex rNameLine;
        private Regex rNameSpaceLine;
        private Regex rDefinitionLine;
        private Regex rCommentLine;
        private Regex rIsObsoleteLine;
        private Regex rExactSynonymLine;
        private Regex rIsALine;
        private Regex rXrefAnalogLine;
        private Regex rRelationshipLine;
        private Regex rSubsetLine;
        #endregion

        // Send it the text block from a single entry
        public GeneOntologyListOBO(List<string> goEntryText)
        {
        }

        public GeneOntologyListOBO()
        {
        }

        protected void ProcessEntry(List<string> entryCollection)
        {
        }

        private void SetupRegexes()
        {
            var reOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;

            rEntryHeader = new Regex(
                @"^\[Term\]$",
                reOptions);
            rIdLine = new Regex(
                @"^(?<tag>id):\s+(?<value>.+)$",
                reOptions);
            rNameLine = new Regex(
                @"^(?<tag>name):\s+(?<value>.+)$",
                reOptions);
            rNameSpaceLine = new Regex(
                @"^(?<tag>namespace):\s+(?<value>.+)$",
                reOptions);
            rDefinitionLine = new Regex(
                @"^(?<tag>def):\s+\""+(?<value>.*)\""\s*\[*(?<xref>.*)\]*\s*",
                reOptions);
            rCommentLine = new Regex(
                @"^(?<tag>comment):\s+(?<value>.+)$",
                reOptions);
            rIsObsoleteLine = new Regex(
                @"^(?<tag>is_obsolete):\s+(?<value>true|false)$",
                reOptions);
            rExactSynonymLine = new Regex(
                @"^(?<tag>exact_synonym):\s+\""+(?<value>.*)\""\s*\[(?<xref>.*)\]S*$",
                reOptions);
            rIsALine = new Regex(
                @"^(?<tag>is_a):\s+(?<value>\S+)\s*\!.*$",
                reOptions);
            rRelationshipLine = new Regex(
                @"^(?<tag>relationship):\s+part_of\s+(?<value>\S+)\s*\!.*$",
                reOptions);
            rXrefAnalogLine = new Regex(
                @"^(?<tag>xref_analog):\s+(?<value>.+)$",
                reOptions);
            rSubsetLine = new Regex(
                @"^(?<tag>subset):\s+(?<value>.+)$",
                reOptions);
        }
    }
}