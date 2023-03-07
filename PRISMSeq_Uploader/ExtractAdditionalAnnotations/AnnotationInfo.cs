using System;
using System.Collections.Generic;

namespace PRISMSeq_Uploader.ExtractAdditionalAnnotations
{
    [Obsolete("Unused")]
    internal class AnnotationInfo
    {
        private readonly Dictionary<int, AnnotationDetails> mAnnotationDetails;

        private NameLookups mAuthorityLookup;
        private NameLookups mAnnotationGroupLookup;

        public AnnotationInfo()
        {
            mAnnotationDetails = new Dictionary<int, AnnotationDetails>();
            mAuthorityLookup = new NameLookups();
            mAnnotationGroupLookup = new NameLookups();
        }

        public void AddPrimaryAnnotation(int proteinId,
            string protName, string description,
            int refId, int namingAuthorityId)
        {
            mAnnotationDetails.Add(proteinId,
                new AnnotationDetails(
                        protName, description, refId,
                        proteinId));
        }

        public void AddAdditionalAnnotation(
            int proteinId,
            string newName,
            int annotationGroupId)
        {
            var tmpDetails = mAnnotationDetails[proteinId];
            tmpDetails.AddNewName(annotationGroupId, newName);
        }

        public void AddAuthorityNameToLookup(
            int authorityId, string authName)
        {
            mAuthorityLookup.AddName(authorityId, authName);
        }

        public void AddAnnotationGroupLookup(
            int annotationGroupCode,
            int authorityId)
        {
            mAnnotationGroupLookup.AddName(
                annotationGroupCode,
                mAuthorityLookup.GetName(authorityId));
        }

        public string GetProteinName(
            int proteinId,
            int annotationGroupCode)
        {
            var details = mAnnotationDetails[proteinId];
            return details.GetAnnotationName(annotationGroupCode);
        }

        public int GetProteinReferenceId(int proteinId, int annotationGroupCode)
        {
            return GetProteinReferenceId(proteinId);
        }

        public int GetProteinReferenceId(int proteinId)
        {
            var details = mAnnotationDetails[proteinId];
            return details.ReferenceId;
        }

        public string GetAnnotationAuthorityName(int annotationGroupCode)
        {
            return mAuthorityLookup.GetName(annotationGroupCode);
        }

        public struct NameLookups
        {
            private Dictionary<int, string> names;

            public void AddName(
                int id,
                string name)
            {
                if (names == null)
                {
                    names = new Dictionary<int, string>();
                }

                names.Add(id, name);
            }

            public string GetName(int id)
            {
                if (names.ContainsKey(id))
                {
                    return names[id];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public class AnnotationDetails
        {
            internal string Description;
            internal int ReferenceId;
            internal int ProteinId;
            internal Dictionary<int, string> Names;

            // Key is AnnotationGroupID, Value is Name

            public AnnotationDetails(
                string primaryName,
                string description,
                int referenceId,
                int proteinId)
            {
                this.Description = description;
                this.ReferenceId = referenceId;
                this.ProteinId = proteinId;

                Names = new Dictionary<int, string> {{0, primaryName}};
            }

            public void AddNewName(int annotationGroupId, string annotationName)
            {
                if (!Names.ContainsValue(annotationName))
                {
                    Names.Add(annotationGroupId, annotationName);
                }
            }

            public string PrimaryName => Names[0];

            public string GetAnnotationName(int annotationGroupCode)
            {
                return Names[annotationGroupCode];
            }
        }
    }
}