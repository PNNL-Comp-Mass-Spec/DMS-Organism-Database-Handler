using System.Collections.Generic;

namespace AppUI_OrfDBHandler.ExtractAdditionalAnnotations
{
    internal class AnnotationGroup
    {
        private Dictionary<string, SortedSet<string>> mAnnotationData;       // key=PrimaryReferenceName, value=SortedSet of xrefs

        public AnnotationGroup(string groupName)
        {
            this.GroupName = groupName;
        }

        public string GroupName { get; set; }

        public int AnnotationAuthorityId { get; set; }

        public string XRefDelimiter { get; set; }

        public bool ImportThisGroup { get; set; }

        public void AddAnnotation(string primaryReferenceName, string xRefName)
        {
            SortedSet<string> xrefList;

            if (mAnnotationData == null)
            {
                mAnnotationData = new Dictionary<string, SortedSet<string>>();
            }

            if (!mAnnotationData.ContainsKey(primaryReferenceName))
            {
                xrefList = new SortedSet<string>();
                xrefList.Add(xRefName);
            }
            else
            {
                xrefList = mAnnotationData[primaryReferenceName.ToString()];
                if (!xrefList.Contains(xRefName))
                {
                    xrefList.Add(xRefName);
                    mAnnotationData[primaryReferenceName.ToString()] = xrefList;
                }
            }
        }

        public Dictionary<string, SortedSet<string>> GetAllXRefs()
        {
            return mAnnotationData;
        }

        public SortedSet<string> GetAllPrimaryReferences()
        {
            var annotationKeys = new SortedSet<string>();
            foreach (var s in mAnnotationData.Keys)
                annotationKeys.Add(s);

            return annotationKeys;
        }

        public SortedSet<string> GetXRefs(string primaryReferenceName)
        {
            var xrefList = mAnnotationData[primaryReferenceName];

            if (XRefDelimiter.Length > 0)
            {
                var newXReflist = new SortedSet<string>();

                foreach (var primeXRef in xrefList)
                {
                    var addnXRefs = primeXRef.Split(XRefDelimiter.ToCharArray());
                    for (var xRefCount = 0; xRefCount < addnXRefs.Length; xRefCount++)
                    {
                        string newItem = addnXRefs[xRefCount].ToString();
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