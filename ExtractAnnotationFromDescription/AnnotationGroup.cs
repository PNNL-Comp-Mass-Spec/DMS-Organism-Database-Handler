using System.Collections.Generic;

namespace ExtractAnnotationFromDescription
{
    internal class AnnotationGroup
    {
        private Dictionary<string, SortedSet<string>> m_AnnotationData;       // key=PrimaryReferenceName, value=SortedSet of xrefs
        private string m_Delimiter;

        public AnnotationGroup(string GroupName)
        {
            this.GroupName = GroupName;
        }

        public string GroupName { get; set; }

        public int AnnotationAuthorityID { get; set; }

        public string XRefDelimiter
        {
            get => m_Delimiter;
            set => m_Delimiter = value;
        }

        public bool ImportThisGroup { get; set; }

        public void AddAnnotation(string PrimaryReferenceName, string XRefName)
        {
            SortedSet<string> xrefList;

            if (m_AnnotationData == null)
            {
                m_AnnotationData = new Dictionary<string, SortedSet<string>>();
            }

            if (!m_AnnotationData.ContainsKey(PrimaryReferenceName))
            {
                xrefList = new SortedSet<string>();
                xrefList.Add(XRefName);
            }
            else
            {
                xrefList = m_AnnotationData[PrimaryReferenceName.ToString()];
                if (!xrefList.Contains(XRefName))
                {
                    xrefList.Add(XRefName);
                    m_AnnotationData[PrimaryReferenceName.ToString()] = xrefList;
                }
            }
        }

        public Dictionary<string, SortedSet<string>> GetAllXRefs()
        {
            return m_AnnotationData;
        }

        public SortedSet<string> GetAllPrimaryReferences()
        {
            var annotationKeys = new SortedSet<string>();
            foreach (var s in m_AnnotationData.Keys)
                annotationKeys.Add(s);

            return annotationKeys;
        }

        public SortedSet<string> GetXRefs(string PrimaryReferenceName)
        {
            var xrefList = m_AnnotationData[PrimaryReferenceName];

            if (m_Delimiter.Length > 0)
            {
                string[] addnXRefs;
                int XRefCount;
                var newXReflist = new SortedSet<string>();

                foreach (var primeXRef in xrefList)
                {
                    addnXRefs = primeXRef.Split(m_Delimiter.ToCharArray());
                    for (XRefCount = 0; XRefCount < addnXRefs.Length; XRefCount++)
                    {
                        string newItem = addnXRefs[XRefCount].ToString();
                        if (!newXReflist.Contains(newItem))
                        {
                            newXReflist.Add(newItem);
                        }
                    }
                }

                xrefList = newXReflist;
            }

            return xrefList;
        }
    }
}