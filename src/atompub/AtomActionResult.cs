using System.Xml;
using Microsoft.AspNet.Mvc;
using Terradue.ServiceModel.Syndication;

namespace sb5.atompub {
  public class AtomActionResult : ActionResult {
    SyndicationFeed feed;

    public AtomActionResult(SyndicationFeed feed) {
      this.feed = feed;
    }

    public override void ExecuteResult(ActionContext context) {
      context.HttpContext.Response.ContentType = "application/atom+xml";
      using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Body)) {
        feed.SaveAsAtom10(writer);
      }
    }

  }

}