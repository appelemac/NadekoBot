using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public interface ICustomReaction {
        List<Response> Responses { get; set; }
        //[NotMapped]
        Regex Regex { get; set; }
        string Trigger { get; set; }
        bool IsRegex { get; set; }

    }


    public class CustomGlobalReaction : DbEntity, ICustomReaction
    {
        
        public List<Response> Responses { get; set; }
        [NotMapped]
        public Regex Regex { get; set; }
        public string Trigger { get; set; }
        public bool IsRegex { get; set; }
        public override string ToString() => $"Trigger: {Trigger}\n Regex: {IsRegex}\n Responses: {string.Join("\n-", Responses.Select(r => r.Text))}";
    }

    public class CustomServerReaction : DbEntity, ICustomReaction
    {
        public long ServerId { get; set; }
        [NotMapped]
        public Regex Regex { get; set; }
        public List<Response> Responses { get; set; }
        public string Trigger { get; set; }
        public bool IsRegex { get; set; }
    }

    public class Response : DbEntity
    {
        public string Text { get; set; }
    }
}
