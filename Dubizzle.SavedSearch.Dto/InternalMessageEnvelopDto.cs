using System;
using System.Collections.Generic;
using System.Text;

namespace Dubizzle.SavedSearch.Dto
{
    public class InternalMessageEnvelopDto
    {
        public string CorrelationId { get; set; }
        public string Email { get; set; }
        public IList<InternalMessageEnvelopDetailDto> Items { get; set; }
        public ulong Tag { get; set; }
    }

    public class InternalMessageEnvelopDetailDto
    {
        public string Catalogue { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
    }
}
