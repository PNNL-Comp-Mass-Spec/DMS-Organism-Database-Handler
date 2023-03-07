using System.Collections.Generic;

namespace PRISMSeq_Uploader.ExtractAdditionalAnnotations
{
    internal class AnnotationStorage
    {
        private Dictionary<int, AnnotationGroup> mAnnotationGroups;
        private Dictionary<string, int> mGroupNameLookup;
        private readonly SortedSet<string> mGlobalProteinNameList = new SortedSet<string>();

        public void AddAnnotationGroup(int groupId, string groupNameToAdd)
        {
            if (mGroupNameLookup == null)
            {
                mGroupNameLookup = new Dictionary<string, int>();
            }

            if (mAnnotationGroups == null)
            {
                mAnnotationGroups = new Dictionary<int, AnnotationGroup>();
            }

            var newGroup = new AnnotationGroup(groupNameToAdd) {ImportThisGroup = false};
            mAnnotationGroups.Add(groupId, newGroup);
            mGroupNameLookup.Add(groupNameToAdd, groupId);
        }

        public void ClearAnnotationGroups()
        {
            mAnnotationGroups?.Clear();

            mGroupNameLookup?.Clear();
        }

        public void AddAnnotation(
            int groupId,
            string primaryReferenceName,
            string xRefName)
        {
            var ag = GetGroup(groupId);
            ag.AddAnnotation(primaryReferenceName, xRefName);
            mAnnotationGroups[groupId] = ag;
            if (!mGlobalProteinNameList.Contains(primaryReferenceName))
            {
                mGlobalProteinNameList.Add(primaryReferenceName);
            }
        }

        public void AddDelimiter(
            int groupId,
            string newDelimiter)
        {
            GetGroup(groupId).XRefDelimiter = newDelimiter;
        }

        public void SetAnnotationGroupStatus(int groupId, bool newState)
        {
            var group = mAnnotationGroups[groupId];
            group.ImportThisGroup = newState;
            mAnnotationGroups[groupId] = group;
            group = null;
        }

        // Controls the import state of the named annotation group
        public void SetAnnotationGroupStatus(string groupNameToUpdate, bool newStateForGroup)
        {
            var groupId = mGroupNameLookup[groupNameToUpdate];
            SetAnnotationGroupStatus(groupId, newStateForGroup);
        }

        public SortedSet<string> GetAllPrimaryReferences()
        {
            return mGlobalProteinNameList;
        }

        public int GroupCount => mAnnotationGroups.Count;

        public string GetDelimiter(int groupId)
        {
            return GetGroup(groupId).XRefDelimiter;
        }

        public int GetAnnotationAuthorityId(int groupId)
        {
            return mAnnotationGroups[groupId].AnnotationAuthorityId;
        }

        public void SetAnnotationAuthorityId(int groupId, int value)
        {
            mAnnotationGroups[groupId].AnnotationAuthorityId = value;
        }

        public string GetGroupName(int groupId)
        {
            return GetGroup(groupId).GroupName;
        }

        public void SetGroupName(int groupId, string value)
        {
            var group = GetGroup(groupId);
            var oldName = @group.GroupName;
            group.GroupName = value;
            mAnnotationGroups[groupId] = group;
            mGroupNameLookup.Remove(oldName);
            mGroupNameLookup[value] = groupId;
        }

        // public HashTable GetAnnotationGroup(string groupName)
        // {
        //     var groupId = mGroupNameLookup(groupName);
        //     return GetAnnotationGroupData(groupId);
        // }

        // public AnnotationGroup GetAnnotationGroupData(int groupId)
        // {
        //     return GetGroup(groupId);
        // }

        /// <summary>
        /// Obtain dictionary containing all the added primary reference names as keys
        /// and SortedSets of their corresponding xref names for the specified Annotation group id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Dictionary<string, SortedSet<string>> GetAllRawXRefs(int groupId)
        {
            return GetGroup(groupId).GetAllXRefs();
        }

        // Returns a SortedSet containing all the xref names for the given
        // primary reference name
        public SortedSet<string> GetXRefs(
            string primaryReferenceName,
            int groupId)
        {
            var group = GetGroup(groupId);
            return group.GetXRefs(primaryReferenceName);
        }

        public AnnotationGroup GetGroup(int groupId)
        {
            var @group = mAnnotationGroups[groupId];
            return group;
        }
    }
}