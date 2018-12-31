using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using gitlab_openproject.Models;

namespace gitlab_openproject.Controllers
{
  public class HomeController : Controller
  {
    private const string WorkPackagePattern = @"#(\d+)";

    private IConfiguration configuration;

    public HomeController(IConfiguration config) {
      this.configuration = config;
    }

    [HttpPost]
    public string Index()
    {
      var reader = new StreamReader(Request.Body);
      var bodyString = reader.ReadToEnd();
      GitPushModel gitEvent = null;
      try
      {
        gitEvent = JsonConvert.DeserializeObject<GitPushModel>(bodyString);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return "error: " + ex.Message;
      }
      if (gitEvent == null)
      {
        return "error: gitEvent == null";
      }
      if (!gitEvent.ObjectKind.Equals("push"))
      {
        return "invalid object kind. ObjectKind = " + gitEvent.ObjectKind;
      }
      if (gitEvent.Commits == null || gitEvent.Commits.Count <= 0)
      {
        return "no commit";
      }
      // Load config
      var opBaseUrl = configuration.GetValue<string>("opSettings:opBaseUrl");
      var apiKey = configuration.GetValue<string>("opSettings:apiKey");
      var openProjectAPI = new OpenProjectAPI(opBaseUrl, apiKey);
      foreach (var commit in gitEvent.Commits)
      {
        var regex = new Regex(WorkPackagePattern);
        var matches = regex.Matches(commit.Message);
        List<int> wpIds = new List<int>();
        // Get all work package int in commit
        foreach (Match m in matches)
        {
          try
          {
            // Group 0: will be '#'
            // Group 1: will be {number}
            var value = m.Groups[1].Value;
            wpIds.Add(Convert.ToInt32(value));
          }
          catch { }
        }
        if (wpIds.Count <= 0) {
          return "no mention";
        }
        // Create comment
        foreach (int id in wpIds) {
            string authorInfo = "someone";
            if (commit.Author != null && !string.IsNullOrEmpty(commit.Author.Name)) {
                authorInfo = commit.Author.Name;
                if (!string.IsNullOrEmpty(commit.Author.Email))
                    authorInfo = string.Format("{0} ({1})", authorInfo, commit.Author.Email);
            }
            string commitInfo = string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>",
                                              commit.Url, commit.Message);
            string respostoryInfo = string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>",
                                                  gitEvent.Repository.HomePage,
                                                  gitEvent.Repository.Name);
            var commentMessage =
              string.Format("{0} mentioned this work package in a commit {1} of {2}",
                            authorInfo, commitInfo, respostoryInfo);
            openProjectAPI.CreateComment(id, commentMessage, true);
        }
      }
      return "ok";
    }
  }
}
