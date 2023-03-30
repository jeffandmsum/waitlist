using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptPlayground
{
    public class ECCN
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DescriptionNotes { get; set; }
        public string Reasons { get; set; }
        public string ReasonNotes { get; set; }
        public string ItemsControlled { get; set; }
        public string PredictedKeywords { get; set; }
    }
}
