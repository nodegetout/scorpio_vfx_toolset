using System.Collections.Generic;

namespace com.scorpio.vfxtoolset.Editor.Data
{
    public class AssetDescription
    {
        public string name = "";
        public string path = "";
        public string assetDependencyHash;
        public List<string> dependencies = new List<string>();
        public List<string> references = new List<string>();
        public ReferenceFinderController.AssetState state = ReferenceFinderController.AssetState.NORMAL;
    }
}