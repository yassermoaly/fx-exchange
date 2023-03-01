using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Models.Fixer
{
    public record FixerInfo
    {
        public int? Timestamp { get; set; }
        public double? Rate { get; set; }
    }

    public record FixerQuery
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public int? Amount { get; set; }
    }
    public record FixerResponse
    {
        public bool? Success { get; set; }
        public FixerQuery? Query { get; set; }
        public FixerInfo? Info { get; set; }
        public string? Date { get; set; }
        public double? Result { get; set; }
    }
}
