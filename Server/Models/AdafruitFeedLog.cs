namespace Server.Models;

public class AdafruitFeedLog
{
    public string Id { get; set; }
    public string Value { get; set; }
    public int FeedId { get; set; }
    public string FeedKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedEpoch { get; set; }
    public DateTime Expiration { get; set; }
}