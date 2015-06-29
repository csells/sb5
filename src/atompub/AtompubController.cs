using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Terradue.ServiceModel.Syndication;
using sb5.atompub.ViewModels;

// from http://www.asp.net/vnext/overview/aspnet-vnext/create-a-web-api-with-mvc-6
namespace sb5.atompub.Controllers {
  [Route("api/[controller]")]
  public class AtompubController : Controller {
    // GET: /api/atompub/details
    // TODO: drop this; this is just to enable Url.Action and Url.Link
    [HttpGet("{id}")]
    public IActionResult Details(string id) {
      return null; // TODO
    }

    // GET: /api/atompub/atomdetails
    // TODO: drop this; this is just to enable Url.Action and Url.Link
    [HttpGet("{id}")]
    public IActionResult AtomDetails(string id) {
      return null; // TODO
    }

    // GET: /api/atompub
    [HttpGet]
    public IActionResult Index() {
      return new AtomActionResult(GetAtomFeed(new AtompubViewModel()));
    }

    SyndicationFeed GetAtomFeed(AtompubViewModel vm) {
      var posts = vm.Posts.ToArray();
      var feed = new SyndicationFeed() {
        Authors = { new SyndicationPerson(vm.ContactEmail, vm.ContactName, vm.ContactUrl) },
        ImageUrl = vm.ImageUrl,
        Copyright = new TextSyndicationContent(vm.CopyrightNotice),
        Description = new TextSyndicationContent(vm.About),
        Id = vm.FeedId,
        //Language = "en-us", // TODO: make this work
        LastUpdatedTime = posts.Length == 0 ? DateTime.Now : posts[0].CreationDate,
        Title = new TextSyndicationContent(vm.FeedTitle),
        Items = posts.Select(p => GetAtomItem(vm, p)),
      };

      foreach (var cat in vm.PostCategories) {
        feed.Categories.Add(new SyndicationCategory(cat.Name, vm.CategoryScheme, cat.DisplayName));
      }

      return feed;
    }

    SyndicationItem GetAtomItem(AtompubViewModel vm, Post post) {
      string authorEmail = post.Email;
      string authorName = post.Author;
      if (string.IsNullOrWhiteSpace(authorEmail) && string.IsNullOrWhiteSpace(authorName)) {
        authorEmail = vm.ContactEmail;
        authorName = vm.ContactName;
      }

      var entry = new SyndicationItem() {
        Content = new TextSyndicationContent(post.Content, TextSyndicationContentKind.Html),
        Id = GetPostLink(post.Id),
        LastUpdatedTime = post.CreationDate,
        Links = {
          new SyndicationLink(new Uri(GetAtomPostLink(post.Id)), "edit", null, null, 0),
          new SyndicationLink(new Uri(GetPostLink(post.Id)), "alternate", null, "text/html", 0),
        },
        PublishDate = post.CreationDate,
        Title = new TextSyndicationContent(post.Title),
        Authors = { new SyndicationPerson(authorEmail, authorName, null) },
      };

      // Split each item category with embedded commas
      if (post.Categories != null && post.Categories.Contains(',')) {
        foreach (var category in post.Categories.Split(',')) {
          entry.Categories.Add(new SyndicationCategory(category));
        }
      }

      return entry;
    }

    // Helpers
    string GetIdLink(string action, string id) {
      // MVC 2: var link = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action(action, new { id = id });

      // porting MVC 2:
      var requestUrl = new Uri(Request.Scheme + "://" + Request.Host + Request.Path);
      var authority = requestUrl.GetLeftPart(UriPartial.Authority);
      var link = authority + Url.Action(action, new { id = id });

      link = link.Replace("https://", "http://"); // fix the issue with images going in with "https" links
      return link;
    }

    string GetPostLink(string id) {
      return GetIdLink("Details", id);
    }

    string GetAtomPostLink(string id) {
      return GetIdLink("AtomDetails", id);
    }

  }

}