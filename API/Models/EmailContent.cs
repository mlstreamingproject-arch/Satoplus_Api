using System;

namespace MeuProxySsl.Models
{
    public class EmailContent
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Tittle { get; set; }
        public string Greetings { get; set; }
        public string MainText { get; set; }
        public string SecondaryText { get; set; }
        public string AuxiliarText { get; set; }
        public string ButtonText { get; set; }
        public string Link { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateOn { get; set; }
    }
}
