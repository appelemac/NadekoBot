namespace NadekoBot.Models.DB
{
    public class ServerModel : IDataModel
    {
        
        public long ServerId { get; set; }
        public int ChannelCount { get; set; }
        public int MemberCount { get; set; }
        public string ServerName { get; set; }
        public long ServerOwnerId { get; set; }
    }
}
