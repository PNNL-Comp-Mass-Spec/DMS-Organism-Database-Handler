using System.Collections.Generic;

namespace AppUI_OrfDBHandler.ExtractAdditionalAnnotations
{
    internal class AnnotationStorage
    {
        private Dictionary<int, AnnotationGroup> mAnnotationGroups;
        private Dictionary<string, int> mGroupNameLookup;
        private readonly SortedSet<string> mGlobalProteinNameList = new SortedSet<string>();

        public void AddAnnotationGroup(int GroupID, string groupNameToAdd)
        {
            if (mGroupNameLookup == null)
            {
                mGroupNameLookup = new Dictionary<string, int>();
            }

            if (mAnnotationGroups == null)
            {
                mAnnotationGroups = new Dictionary<int, AnnotationGroup>();
            }

            var newGroup = new AnnotationGroup(groupNameToAdd);
            newGroup.ImportThisGroup = false;
            mAnnotationGroups.Add(GroupID, newGroup);
            mGroupNameLookup.Add(groupNameToAdd, GroupID);
        }

        public void ClearAnnotationGroups()
        {
            if (mAnnotationGroups != null)
            {
                mAnnotationGroups.Clear();
            }

            if (mGroupNameLookup != null)
            {
                mGroupNameLookup.Clear();
            }
        }

        public void AddAnnotation(
            int groupID,
            string PrimaryReferenceName,
            string XRefName)
        {
            var ag = GetGroup(groupID);
            ag.AddAnnotation(PrimaryReferenceName, XRefName);
            mAnnotationGroups[groupID] = ag;
            if (!mGlobalProteinNameList.Contains(PrimaryReferenceName))
            {
                mGlobalProteinNameList.Add(PrimaryReferenceName);
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
            var group = mAnnotationGroups[GroupID];
            group.ImportThisGroup = NewState;
            mAnnotationGroups[GroupID] = group;
            group = null;
        }

        // Controls the import state of the named annotation group
        public void SetAnnotationGroupStatus(string groupNameToUpdate, bool newStateForGroup)
        {
            int groupID = mGroupNameLookup[groupNameToUpdate];
            SetAnnotationGroupStatus(groupID, newStateForGroup);
        }

        public SortedSet<string> GetAllPrimaryReferences()
        {
            return mGlobalProteinNameList;
        }

        public int GroupCount => mAnnotationGroups.Count;

        public string GetDelimiter(int GroupID)
        {
            return GetGroup(GroupID).XRefDelimiter;
        }

        public int GetAnnotationAuthorityID(int GroupID)
        {
            return mAnnotationGroups[GroupID].AnnotationAuthorityID;
        }

        public void SetAnnotationAuthorityID(int GroupID, int value)
        {
            mAnnotationGroups[GroupID].AnnotationAuthorityID = value;
        }

        public string GetGroupName(int GroupID)
        {
            return GetGroup(GroupID).GroupName;
        }

        public void SetGroupName(int GroupID, string value)
        {
            var group = GetGroup(GroupID);
            var oldName = @group.GroupName;
            group.GroupName = value;
            mAnnotationGroups[GroupID] = group;
            mGroupNameLookup.Remove(oldName);
            mGroupNameLookup[value] = GroupID;
        }

        // public HashTable GetAnnotationGroup(string GroupName)
        // {
        //     var groupID = mGroupNameLookup(GroupName);
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
            var @group = mAnnotationGroups[groupid];
            return group;
        }
    }
}