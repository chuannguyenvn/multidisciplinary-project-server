namespace Server.Models;

public class Feed
{
    public string username { get; set; }
    public Owner owner { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int user_id { get; set; }
    public object unit_type { get; set; }
    public object unit_symbol { get; set; }
    public bool history { get; set; }
    public string last_value { get; set; }
    public string visibility { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public bool enabled { get; set; }
    public object wipper_pin_info { get; set; }
    public string key { get; set; }
}

public class Owner
{
    public int id { get; set; }
    public string username { get; set; }
}

public class AdafruitFeedLog
{
    public string username { get; set; }
    public Owner owner { get; set; }
    public int id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public object machine_name { get; set; }
    public object wipper_semver { get; set; }
    public string description { get; set; }
    public string visibility { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public bool is_shared { get; set; }
    public List<Feed> feeds { get; set; }
}