using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class CustomGlobalReaction : DbEntity
    {
        
        public List<Response> Responses { get; set; }
        [NotMapped]
        public Regex Regex { get; set; }
        public string Trigger { get; set; }
        public bool IsRegex { get; set; }
    }

    public class CustomServerReaction : DbEntity
    {
        public long ServerId { get; set; }
        [NotMapped]
        public Regex Regex { get; set; }
        public List<Response> Responses { get; set; }
        public string Trigger { get; set; }
        public bool IsRegex { get; set; }
    }

    public class Response
    {
        public string Text { get; set; }
    }
}
