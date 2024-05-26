using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SyndicationFeed.Atom;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Data;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Extensions;

namespace MultiFamilyPortal.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("feed")]
    public class FeedController : ControllerBase
    {
        private IBlogContext _db { get; }

        public FeedController(IBlogContext db)
        {
            _db = db;
        }

        [HttpGet("rss")]
        public async Task<IActionResult> Rss([FromQuery(Name = "category")] string filter)
        {
            return await GetFeed("rss", filter);
        }

        [HttpGet("atom")]
        public async Task<IActionResult> Atom([FromQuery(Name = "category")] string filter)
        {
            return await GetFeed("atom", filter);
        }

        private async Task<IActionResult> GetFeed(string type, [FromQuery(Name = "category")] string filter)
        {
            //Response.ContentType = "application/xml";
            var host = new Uri($"{Request.Scheme}://{Request.Host}");
            var sb = new StringBuilder();
            using var xmlWriter = XmlWriter.Create(
                sb,
                new XmlWriterSettings { Async = true, Indent = true, Encoding = new UTF8Encoding(false) });

            var query = _db.Posts.Include(x => x.Author)
                                 .Include(x => x.Categories)
                                 .Where(x => x.IsPublished && x.Published < DateTimeOffset.Now);
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Categories.Any(x => x.Id == filter));

            var posts = await query.OrderByDescending(x => x.Published)
                                   .Take(10)
                                   .ToListAsync();
            var authors = await _db.Users.Where(x => x.Posts.Any(x => x.IsPublished && x.Published < DateTimeOffset.Now)).ToArrayAsync();
            var writer = await GetWriter(
                type,
                xmlWriter, posts.Any() ?
                posts.Select(x => x.Published).Distinct().Max() :
                DateTimeOffset.Now,
                authors);

            foreach (var post in posts)
            {
                var url = new Uri(host, $"blog/post/{post.Published.Month}/{post.Published.Day}/{post.Published.Year}/{post.Slug}");
                var item = new AtomEntry {
                    Title = post.Title,
                    Description = post.Content,
                    Id = url.ToString(),
                    Published = post.Published,
                    LastUpdated = post.LastModified,
                    ContentType = "html",
                };

                if (post.Categories is null)
                    post.Categories = new List<Category>();

                foreach (var category in post.Categories)
                {
                    item.AddCategory(new SyndicationCategory(category.Id));
                }

                if(post.Author != null)
                    item.AddContributor(post.Author.Syndicate());

                item.AddLink(new SyndicationLink(new Uri(item.Id)));
                await writer.Write(item);
            }

            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();
            var feed = sb.ToString();
            return Content(feed, "application/xml");
        }

        private async Task<ISyndicationFeedWriter> GetWriter(string type, XmlWriter xmlWriter, DateTimeOffset updated, IEnumerable<SiteUser> authors)
        {
            var host = $"{Request.Scheme}://{Request.Host}/";

            var siteTitle = _db.GetSetting<string>(PortalSetting.SiteTitle);
            var rssDescription = _db.GetSetting<string>(PortalSetting.RssDescription);
            if (type?.Equals("rss", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                var rss = new RssFeedWriter(xmlWriter);
                await rss.WriteTitle(siteTitle);
                await rss.WriteDescription(rssDescription);
                await rss.WriteGenerator("AvantiPoint");
                await rss.WriteValue("link", host);
                await rss.WriteLanguage(CultureInfo.GetCultureInfo("en-US"));
                await rss.WriteCopyright($"Copyright {DateTime.Now.Year} {siteTitle}");
                return rss;
            }

            var atom = new AtomFeedWriter(xmlWriter);
            await atom.WriteTitle(siteTitle);
            await atom.WriteId(host);
            await atom.WriteSubtitle(rssDescription);
            await atom.WriteGenerator(siteTitle, host, "1.0");
            await atom.WriteRights($"Copyright {DateTime.Now.Year} {siteTitle}");
            await atom.WriteValue("updated", updated.ToString("u", CultureInfo.InvariantCulture));
            foreach (var author in authors)
            {
                await atom.WriteValue("contributor", new { name = $"{author.FirstName} {author.LastName}".Trim() });
            }
            return atom;
        }
    }
}
