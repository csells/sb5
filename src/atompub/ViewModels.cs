using System;
using System.Collections.Generic;

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

    // TODO
    public AtompubViewModel() {
      PostCategories = new PostCategory[0];
      Posts = new Post[] {
        new Post { Author = "Chris Sells", Content = "<h1>testing</h1><p>123...</p>", CreationDate = new DateTime(1995, 1, 1, 12, 0, 0), Id = "1", Title = "Test 1" },
        new Post { Author = "Chris Sells", Content = "<h1>testing</h1><p>321...</p>", CreationDate = new DateTime(1995, 1, 1, 12, 0, 1), Id = "2", Title = "Test 2" },
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
