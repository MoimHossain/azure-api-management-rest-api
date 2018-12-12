using System;
using System.Collections.Generic;
using System.Text;

namespace apim_utils.Models
{
    public class VersionSchemeProperties
    {
        public string DisplayName { get; set; }
        public object Description { get; set; }
        public string VersioningScheme { get; set; }
        public string VersionQueryName { get; set; }
        public string VersionHeaderName { get; set; }
    }

    public class VersionScheme
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public VersionSchemeProperties Properties { get; set; }
    }

    public class VersionSchemeCollection
    {
        public List<VersionScheme> Value { get; set; }
    }
}
