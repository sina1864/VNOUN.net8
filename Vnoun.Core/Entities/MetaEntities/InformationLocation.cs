using MongoDB.Entities;

namespace Vnoun.Core.Entities.MetaEntities
{
    public class InformationLocation
    {
        [Field("type")]
        public string Type { get; set; }

        [Field("coordinates")]
        public List<double> Coordinates { get; set; }

        [Field("address")]
        public string Address { get; set; }

        [Field("description")]
        public string Description { get; set; }
    }
}
