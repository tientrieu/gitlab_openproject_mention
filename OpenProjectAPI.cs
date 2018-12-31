using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

public class OpenProjectAPI
{
  public string OPBaseUrl { get; private set; }

  public string APIKey { get; private set; }

  private HttpClient httpClient;

  public OpenProjectAPI(string opBaseUrl, string apiKey)
  {
    OPBaseUrl = opBaseUrl;
    APIKey = apiKey;

    // Initialize httpClient
    httpClient = new HttpClient();
    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "apikey", APIKey));
    httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
  }

  public async void CreateComment(int workPackageId, string message, bool notify)
  {
    var requestUri = new Uri(GetWorkPackageActivitiesUrl(workPackageId, notify),
                      UriKind.Absolute);
    var comment = new OpenProjectPostCommentModel(message);
    var json = Newtonsoft.Json.JsonConvert.SerializeObject(comment);
    var postContent = new StringContent(json, Encoding.UTF8, "application/json");
    var result = await httpClient.PostAsync(requestUri, postContent);
    var resultString = await result.Content.ReadAsStringAsync();
    Console.WriteLine(resultString);
  }

  private string GetAPIBaseURl()
  {
    string apiBaseUrl = string.Format("{0}/api/v3/", this.OPBaseUrl);
    return apiBaseUrl;
  }

  private string GetWorkPackagesBaseUrl(int workPackageId)
  {
    string workPackageUrl = string.Format("{0}/work_packages/{1}",
                                          GetAPIBaseURl(), workPackageId);
    return workPackageUrl;
  }

  private string GetWorkPackageActivitiesUrl(int workPackageId, bool notify)
  {
    string workPackageActivitiesUrl = string.Format("{0}/activities?notify={1}",
                                                    GetWorkPackagesBaseUrl(workPackageId),
                                                    notify.ToString().ToLower());
    return workPackageActivitiesUrl;
  }

  private class OpenProjectPostCommentModel
  {

    public OpenProjectPostCommentModel(string message)
    {
      this.comment = new Comment(message);
    }

    [JsonProperty("comment")]
    public Comment comment { get; set; }

    public class Comment
    {
      [JsonProperty("raw")]
      public string Raw { get; private set; }

      public Comment(string message)
      {
        Raw = message;
      }
    }
  }
}