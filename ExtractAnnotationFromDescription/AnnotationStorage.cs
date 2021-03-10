using System.Collections.Generic;

namespace ExtractAnnotationFromDescription
{
    internal class AnnotationStorage
    {
        private Dictionary<int, AnnotationGroup> m_AnnotationGroups;
        private Dictionary<string, int> m_GroupNameLookup;
        private readonly SortedSet<string> m_GlobalProteinNameList = new SortedSet<string>();

        public void AddAnnotationGroup(int GroupID, string groupNameToAdd)
        {
            if (m_GroupNameLookup == null)
            {
                m_GroupNameLookup = new Dictionary<string, int>();
            }

            if (m_AnnotationGroups == null)
            {
                m_AnnotationGroups = new Dictionary<int, AnnotationGroup>();
            }

            var newGroup = new AnnotationGroup(groupNameToAdd);
            newGroup.ImportThisGroup = false;
            m_AnnotationGroups.Add(GroupID, newGroup);
            m_GroupNameLookup.Add(groupNameToAdd, GroupID);
        }

        public void ClearAnnotationGroups()
        {
            if (m_AnnotationGroups != null)
            {
                m_AnnotationGroups.Clear();
            }

            if (m_GroupNameLookup != null)
            {
                m_GroupNameLookup.Clear();
            }
        }

        public void AddAnnotation(
            int groupID,
            string PrimaryReferenceName,
            string XRefName)
        {
            var ag = GetGroup(groupID);
            ag.AddAnnotation(PrimaryReferenceName, XRefName);
            m_AnnotationGroups[groupID] = ag;
            if (!m_GlobalProteinNameList.Contains(PrimaryReferenceName))
            {
                m_GlobalProteinNameList.Add(PrimaryReferenceName);
            }
        }

        public void AddDelimiter(
            int groupID,
            string newDelimiter)
        {
            GetGroup(groupID).XRefDelimiter = newDelimiter;
        }

        public void SetAnnotationGroupStatus(int GroupID, bool NewState)
        {
            var group = m_AnnotationGroups[GroupID];
            group.ImportThisGroup = NewState;
            m_AnnotationGroups[GroupID] = group;
            group = null;
        }

        // Controls the import state of the named annotation group
        public void SetAnnotationGroupStatus(string groupNameToUpdate, bool newStateForGroup)
        {
            int groupID = m_GroupNameLookup[groupNameToUpdate];
            SetAnnotationGroupStatus(groupID, newStateForGroup);
        }

        public SortedSet<string> GetAllPrimaryReferences()
        {
            return m_GlobalProteinNameList;
        }

        public int GroupCount
        {
            get { return m_AnnotationGroups.Count; }
        }

        public string get_Delimiter(int GroupID)
        {
            return GetGroup(GroupID).XRefDelimiter;
        }

        public int get_AnnotationAuthorityID(int GroupID)
        {
            return m_AnnotationGroups[GroupID].AnnotationAuthorityID;
        }

        public void set_AnnotationAuthorityID(int GroupID, int value)
        {
            m_AnnotationGroups[GroupID].AnnotationAuthorityID = value;
        }

        public string get_GroupName(int GroupID)
        {
            return GetGroup(GroupID).GroupName;
        }

        public void set_GroupName(int GroupID, string value)
        {
            string oldName;
            var group = GetGroup(GroupID);
            oldName = group.GroupName;
            group.GroupName = value;
            m_AnnotationGroups[GroupID] = group;
            m_GroupNameLookup.Remove(oldName);
            m_GroupNameLookup[value] = GroupID;
        }

        // public HashTable GetAnnotationGroup(string GroupName)
        // {
        //     var groupID = m_GroupNameLookup(GroupName);
        //     return GetAnnotationGroupData(groupID);
        // }

        // public AnnotationGroup GetAnnotationGroupData(int GroupID)
        // {
        //     return GetGroup(GroupID);
        // }

        /// <summary>
        /// Obtain dictionary containing all the added primary reference names as keys
        /// and SortedSets of their corresponding xref names for the specified Annotation group id
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public Dictionary<string, SortedSet<string>> GetAllRawXRefs(int GroupID)
        {
            return GetGroup(GroupID).GetAllXRefs();
        }

        // Returns a SortedSet containing all the xref names for the given
        // primary reference name
        public SortedSet<string> GetXRefs(
            string PrimaryReferenceName,
            int GroupID)
        {
            var group = GetGroup(GroupID);
            return group.GetXRefs(PrimaryReferenceName);
        }

        public AnnotationGroup GetGroup(int groupid)
        {
            AnnotationGroup group;
            group = m_AnnotationGroups[groupid];
            return group;
        }
    }
}