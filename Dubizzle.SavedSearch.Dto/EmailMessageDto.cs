using System;
using System.Collections.Generic;
using System.Text;

namespace Dubizzle.SavedSearch.Dto
{
    public class EmailMessageDto
    {
        public string CorrelationId { get; set; }
        public IList<string> Recepients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
