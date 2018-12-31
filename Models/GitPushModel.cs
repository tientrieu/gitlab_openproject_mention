using System.Collections.Generic;
using Newtonsoft.Json;

namespace gitlab_openproject.Models
{
  public class GitPushModel
  {
    [JsonProperty("object_kind")]
    public string ObjectKind { get; set; }

    [JsonProperty("commits")]
    public List<Commit> Commits { get; set; }

    [JsonProperty("repository")]
    public RepositoryModel Repository { get; set; }

    public class RepositoryModel
    {

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("homepage")]
      public string HomePage { get; set; }
    }

    public class Commit
    {
      [JsonProperty("id")]
      public string Id { get; set; }

      [JsonProperty("message")]
      public string Message { get; set; }

      [JsonProperty("url")]
      public string Url { get; set; }

      [JsonProperty("author")]
      public CommitAuthor Author { get; set; }

      public class CommitAuthor
      {
        public string Name { get; set; }

        public string Email { get; set; }
      }
    }
  }
}