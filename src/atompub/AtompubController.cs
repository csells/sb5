using System;
using System.Linq;
using System.Xml;
using Microsoft.AspNet.Mvc;
using sb5.atompub.ViewModels;
using Terradue.ServiceModel.Syndication;

// from http://www.asp.net/vnext/overview/aspnet-vnext/create-a-web-api-with-mvc-6
namespace sb5.atompub.Controllers {
  [Route("api/[controller]/[action]")]
  public class AtompubController : Controller {
    // GET: /api/atompub/details
    // TODO: drop this; this is just to enable Url.Action
    [HttpGet("{id}")]
    public IActionResult Details(string id) {
      return null; // TODO
    }

    // GET: /api/atompub/atomdetails
    // TODO: drop this; this is just to enable Url.Action
    [HttpGet("{id}")]
    public IActionResult AtomDetails(string id) {
      return null; // TODO
    }

    // GET: /api/atompub/
    [HttpGet]
    public IActionResult Index() {
      return new ServiceDocumentActionResult(GetServiceDocument(new AtompubViewModel(Request)));
    }

    ServiceDocument GetServiceDocument(AtompubViewModel vm) {
      var workspaces = new Workspace[] {
        new Workspace("Default", new ResourceCollectionInfo[] {
          new ResourceCollectionInfo("Posts",                              new Uri(Url.Action("posts"), UriKind.Relative)) { Categories = { new InlineCategoriesDocument() { IsFixed = false, Scheme = vm.CategoryScheme } } },
          new ResourceCollectionInfo(new TextSyndicationContent("Images"), new Uri(Url.Action("images"), UriKind.Relative), null, new string[] { "image/*" } ),
        }),
      };
      var doc = new ServiceDocument(workspaces);

      var categories = ((InlineCategoriesDocument)doc.Workspaces[0].Collections[0].Categories[0]).Categories;
      foreach (var cat in vm.PostCategories) {
        categories.Add(new SyndicationCategory(cat.Name, null, cat.DisplayName));
      }

      return doc;
    }

    // GET: /api/atompub/posts
    [HttpGet]
    public IActionResult Posts(string category = null, int page = 0) {
      return new AtomActionResult(GetAtomFeed(new AtompubViewModel(Request, category, page)));
    }

    // POST: /api/atompub/posts
    [HttpPost]
    public IActionResult Posts([FromBody] XmlElement body) {
      return CreateAtomPost(body);
    }

    // GET: /api/atompub/images
    [HttpGet("{id}")]
    public IActionResult Images(string id) {
      return null; // TODO
    }

    #region GET posts
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
    #endregion // GET posts

    #region POST posts
    IActionResult CreateAtomPost(XmlElement body) {
      // TODO
      //if (!BasicAuthSingleAdminUserModule.ForceSslAndBasicAuthAsAdmin()) { return null; }

#if false
      // Get post data
      SyndicationItem entry = null;
      using (var reader = XmlReader.Create(Request.InputStream)) {
        entry = SyndicationItem.Load(reader);
      }

      // Create post
      var post = new Post() {
        Categories = entry.Categories.Aggregate("", (cats, cat) => cats.Length == 0 ? cat.Name : cats + "," + cat.Name),
        Content = ((TextSyndicationContent)entry.Content).Text,
        CreationDate = entry.PublishDate < minDate ? DateTime.Now : entry.PublishDate.DateTime,
        Author = entry.Authors.Count > 0 ? entry.Authors[0].Name : null,
        Email = entry.Authors.Count > 0 ? entry.Authors[0].Email : null,
        IsActive = true,
        Title = entry.Title.Text,
        UuidString = Guid.NewGuid().ToString(),
      };

      db.Add(post);
      db.SaveChanges();

      // Return the updated post
      var postLink = GetAtomPostLink(post.Id);
      entry.Id = postLink;
      entry.Authors.Add(new SyndicationPerson() { Name = db.Site.ContactName, Email = db.Site.ContactEmail });

      var sb = new StringBuilder();
      // OMG! WLW crashes if it gets back an XML declaration!
      using (var writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true })) { entry.SaveAsAtom10(writer); }
      var result = new SimpleActionResult() { ResponseOut = sb.ToString(), StatusCode = 201, StatusDescription = "Created" };
      result.Headers.Add("Location", postLink);
      return result;
#endif
      return null;
    }

#endregion // POST

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