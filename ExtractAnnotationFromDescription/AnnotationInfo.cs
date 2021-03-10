using System;
using System.Collections.Generic;

namespace ExtractAnnotationFromDescription
{
    [Obsolete("Unused")]
    internal class AnnotationInfo
    {
        private readonly Dictionary<int, AnnotationDetails> m_AnnotationDetails;

        private NameLookups m_AuthorityLookup;
        private NameLookups m_AnnotationGroupLookup;

        public AnnotationInfo()
        {
            m_AnnotationDetails = new Dictionary<int, AnnotationDetails>();
            m_AuthorityLookup = new NameLookups();
            m_AnnotationGroupLookup = new NameLookups();
        }

        public void AddPrimaryAnnotation(int proteinID,
            string protName, string description,
            int refID, int namingAuthorityID)
        {
            m_AnnotationDetails.Add(proteinID,
                new AnnotationDetails(
                        protName, description, refID,
                        proteinID));
        }

        public void AddAdditionalAnnotation(
            int ProteinID,
            string NewName,
            int AnnotationGroupID)
        {
            AnnotationDetails tmpDetails;

            tmpDetails = m_AnnotationDetails[ProteinID];
            tmpDetails.AddNewName(AnnotationGroupID, NewName);
        }

        public void AddAuthorityNameToLookup(
            int AuthorityID, string authName)
        {
            m_AuthorityLookup.AddName(AuthorityID, authName);
        }

        public void AddAnnotationGroupLookup(
            int AnnotationGroupCode,
            int AuthorityID)
        {
            m_AnnotationGroupLookup.AddName(
                AnnotationGroupCode,
                m_AuthorityLookup.GetName(AuthorityID));
        }

        public string get_ProteinName(
            int ProteinID,
            int AnnotationGroupCode)
        {
            var details = m_AnnotationDetails[ProteinID];
            return details.get_Name(AnnotationGroupCode);
        }

        public int get_ReferenceID(int ProteinID, int AnnotationGroupCode)
        {
            var details = m_AnnotationDetails[ProteinID];
            return details.ReferenceID;
        }

        public string get_AuthorityName(int AnnotationGroupCode)
        {
            return m_AuthorityLookup.GetName(AnnotationGroupCode);
        }

        public struct NameLookups
        {
            private Dictionary<int, string> Names;

            public void AddName(
                int ID,
                string Name)
            {
                if (Names == null)
                {
                    Names = new Dictionary<int, string>();
                }

                Names.Add(ID, Name);
            }

            public string GetName(int ID)
            {
                if (Names.ContainsKey(ID))
                {
                    return Names[ID];
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
            internal int ReferenceID;
            internal int ProteinID;
            internal int NamingAuthorityID;
            internal Dictionary<int, string> Names;

            // Key is AnnotationGroupID, Value is Name

            public AnnotationDetails(
                string PrimaryName,
                string Description,
                int ReferenceID,
                int ProteinID)
            {
                this.Description = Description;
                this.ReferenceID = ReferenceID;
                this.ProteinID = ProteinID;

                Names = new Dictionary<int, string>();
                Names.Add(0, PrimaryName);
            }

            public void AddNewName(int annotationGroupId, string annotationName)
            {
                if (!Names.ContainsValue(annotationName))
                {
                    Names.Add(annotationGroupId, annotationName);
                }
            }

            public string PrimaryName => Names[0].ToString();

            public string get_Name(int annotationGroupCode)
            {
                return Names[annotationGroupCode];
            }
        }
    }
}