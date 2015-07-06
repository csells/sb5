using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace sb5.atompub.ViewModels {
  class AtompubViewModel {
    public string About { get; internal set; }
    public string CategoryScheme { get; internal set; }
    public string ContactEmail { get; internal set; }
    public string ContactName { get; internal set; }
    public string ContactUrl { get; internal set; }
    public string CopyrightNotice { get; internal set; }
    public string FeedId { get; internal set; }
    public string FeedTitle { get; internal set; }
    public Uri ImageUrl { get; internal set; } // TODO: new Uri(string.Format("{0}/content/images/sellsbrothers_feed_logo.jpg", Request.Url.GetLeftPart(UriPartial.Authority)))
    public IEnumerable<PostCategory> PostCategories { get; internal set; }
    public IEnumerable<Post> Posts { get; internal set; }

    public AtompubViewModel(HttpRequest request, string category = null, int page = 0) {
      // TODO: this is from the site description
      About = "About";
      CategoryScheme = "http://sellsbrothers.com/todo/categoryscheme";
      ContactEmail = "todo@email.com";
      ContactName = "TodO Mally";
      ContactUrl = "http://example.com/TODO";
      CopyrightNotice = "Copyright (c) now-forever TODO Corp.";
      FeedId = "feedid";
      FeedTitle = "feedtitle";
      ImageUrl = new Uri(request.Scheme + "://" + request.Host + "/sellsbrothers_feed_logo.jpg");
      PostCategories = new PostCategory[] { new PostCategory { Name = "postcat-name-1", DisplayName = "postcat-display-1" }, new PostCategory { Name = "postcat-name-2", DisplayName = "postcat-display-2" }, };
      Posts = new Post[] {
        new Post { Author = "Chris Sells", Content = "<h1>testing</h1><p>123...</p>", CreationDate = new DateTime(1995, 1, 1, 12, 0, 0), Id = "1", Title = "Test 1" },
        new Post { Author = "Chris Sells", Content = "<h1>testing</h1><p>321...</p>", CreationDate = new DateTime(1995, 1, 1, 12, 0, 1), Id = "2", Title = "Test 2" },
        new Post { Author = "Chris Sells", Content = "<h1>testing</h1><p>abc...</p>", CreationDate = new DateTime(1995, 1, 1, 12, 0, 2), Id = "3", Title = "Test 3" },
      };
    }

  }

  public class PostCategory {
    public string DisplayName { get; internal set; }
    public string Name { get; internal set; }
  }

  class Post {
    public string Author { get; internal set; }
    public string Categories { get; internal set; }
    public string Content { get; internal set; }
    public DateTime CreationDate { get; internal set; }
    public string Email { get; internal set; }
    public string Id { get; internal set; }
    public string Title { get; internal set; }
  }
}
