using System.Xml;
using Microsoft.AspNet.Mvc;
using Terradue.ServiceModel.Syndication;

namespace sb5.atompub {
  public class ServiceDocumentActionResult : ActionResult {
    ServiceDocument doc;

    public ServiceDocumentActionResult(ServiceDocument doc) {
      this.doc = doc;
    }

    public override void ExecuteResult(ActionContext context) {
      // NOTE: atompub spec says "application/atomserv+xml",
      // but it causes browsers to download instead of browse
      // and the tools seem to work with "application/xml" just fine
      //context.HttpContext.Response.ContentType = "application/atomserv+xml";
      context.HttpContext.Response.ContentType = "application/xml";
      using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Body)) {
        doc.GetFormatter().WriteTo(writer);
      }
    }
  }

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