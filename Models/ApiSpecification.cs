using System;
using System.Collections.Generic;
using System.Text;

namespace apim_utils.Models
{
    public class ApiSpecification
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiRevision { get; set; }
        public string Description { get; set; }
        public string ServiceUrl { get; set; }
        public string Path { get; set; }
        public List<string> Protocols { get; set; }
        public bool IsCurrent { get; set; }
        public string ApiVersion { get; set; }
        public string ApiVersionSetId { get; set; }
    }

    public class ApiSpecificationCollection
    {
        public List<ApiSpecification> Value { get; set; }
        public int Count { get; set; }
        public object NextLink { get; set; }
    }

    public class ApiSpecificationPayload
    {
        public ApSpecPayloadPropertiesBase Properties { get; set; }

    }

    public abstract class ApSpecPayloadPropertiesBase
    {
        public string Path { get; set; }
        public string [] Protocols { get; set; }
        public bool IsOnline { get; internal set; }
    }

    public class ApSpecCreatePayloadProperties : ApSpecPayloadPropertiesBase
    {
        public string ContentFormat { get; set; }
        public string ContentValue { get; set; }
    }

    public class ApSpecRevisionPayloadProperties : ApSpecPayloadPropertiesBase
    {
        public string ApiRevisionDescription { get; set; }
        public string DisplayName { get; internal set; }
        public string ServiceUrl { get; internal set; }
        public string Description { get; internal set; }
    }
}
