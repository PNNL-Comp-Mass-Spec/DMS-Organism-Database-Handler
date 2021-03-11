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
                xrefList = new SortedSet<string> {xRefName};
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
                var newXRefList = new SortedSet<string>();

                foreach (var primeXRef in xrefList)
                {
                    var additionalXRefs = primeXRef.Split(XRefDelimiter.ToCharArray());
                    for (var xRefCount = 0; xRefCount < additionalXRefs.Length; xRefCount++)
                    {
                        var newItem = additionalXRefs[xRefCount].ToString();
                        if (!newXRefList.Contains(newItem))
                        {
                            newXRefList.Add(newItem);
                        }
                    }
                }

                xrefList = newXRefList;
            }

            return xrefList;
        }
    }
}