using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Data.Views;
using MultiFamilyPortal.Data;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Services;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Extensions;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private IBlogContext _context { get; }
        private IIpLookupService _geolocationService { get; }
        private IBlogSubscriberService _email { get; }

        public BlogController(
            IBlogContext context,
            IIpLookupService geolocationService,
            IBlogSubscriberService emailService)
        {
            _context = context;
            _geolocationService = geolocationService;
            _email = emailService;
        }

        [ProducesDefaultResponseType(typeof(PostSummaryResponse))]
        [HttpGet("posts")]
        public async Task<IActionResult> Index(
            [FromQuery(Name = "q")] string query = null,
            [FromQuery] int page = 1,
            [FromQuery] string tag = null,
            [FromQuery] string category = null
            )
        {
            var postQuery = _context.PostSummaryViews
                .Where(x => x.IsPublished && x.Published <= DateTimeOffset.Now);

            if (!string.IsNullOrEmpty(query))
            {
                postQuery = postQuery.Where(x => x.Title.Contains(query));// || x.Description.Contains(query) || x.Content.Contains(query));
            }

            if (!string.IsNullOrEmpty(tag))
            {
                postQuery = postQuery.Where(x => x.Tags.Any(t => t == tag));
            }

            if (!string.IsNullOrEmpty(category))
            {
                postQuery = postQuery.Where(x => x.Categories.Any(c => c == category));
            }

            var pageCount = await _context.GetSettingAsync<int>(PortalSetting.BlogPageLimit);
            var skip = page <= 1 ? 0 : (page - 1) * pageCount;

            var postCount = await postQuery.CountAsync();
            var posts = await postQuery.OrderByDescending(x => x.Published)
                                       .Skip(skip)
                                       .Take(pageCount)
                                       .ToListAsync();

            var topPosts = await _context.Posts
                .Where(x => x.IsPublished && x.Published < DateTimeOffset.Now)
                .OrderByDescending(x => x.Views.Count)
                .Take(5)
                .ToListAsync();

            var totalPages = postCount < pageCount ? 1 : (int)Math.Ceiling(postCount / (double)pageCount);
            var response = new PostSummaryResponse {
                CurrentPage = page,
                TotalPages = totalPages,
                TotalPosts = postCount,
                Category = category,
                Tag = tag,
                HasNext = page + 1 <= totalPages,
                HasPrevious = page - 1 > 0,
                Posts = posts
            };
            return new OkObjectResult(response);
        }

        [HttpGet("categories")]
        [ProducesDefaultResponseType(typeof(IEnumerable<TopicFrequency>))]
        public async Task<IActionResult> GetCategories()
        {
            var values = await _context.Categories.Select(x => new TopicFrequency {
                Name = x.Id,
                Count = x.Posts.Count
            })
                .OrderByDescending(x => x.Count)
                .ToArrayAsync();

            return new OkObjectResult(values);
        }

        [HttpGet("tags")]
        [ProducesDefaultResponseType(typeof(IEnumerable<TopicFrequency>))]
        public async Task<IActionResult> GetTags()
        {
            var values = await _context.Tags
                .Select(x => new TopicFrequency {
                    Name = x.Id,
                    Count = x.Posts.Count
                })
                .OrderByDescending(x => x.Count)
                .ToArrayAsync();

            return new OkObjectResult(values);
        }

        [HttpGet("post/{year}/{month}/{day}/{slug}")]
        [ProducesDefaultResponseType(typeof(PostDetailResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post(
            int month,
            int day,
            int year,
            string slug,
            [FromQuery(Name = "utm_source")] string source = null,
            [FromQuery(Name = "utm_medium")] string medium = null,
            [FromQuery(Name = "utm_campaign")] string campaign = null,
            [FromQuery(Name = "utm_term")] string term = null,
            [FromQuery(Name = "utm_content")] string content = null)
        {
            var post = await _context.Posts
                .Select(x => new PostDetailView {
                    Id = x.Id,
                    AuthorEmail = x.Author.Email,
                    AuthorName = x.Author.DisplayName,
                    IsPublished = x.IsPublished,
                    Published = x.Published,
                    Title = x.Title,
                    Slug = x.Slug,
                    Description = x.Description,
                    Body = x.Content,
                    Categories = x.Categories.Select(c => c.Id),
                    Tags = x.Tags.Select(t => t.Id),
                })
                .Where(x => x.Published.Month == month && x.Published.Day == day && x.Published.Year == year && x.Slug == slug && x.IsPublished && x.Published <= DateTimeOffset.Now)
                .FirstOrDefaultAsync();

            if (post is null)
                return StatusCode(404);

            (var previous, var next) = await GetSuggestedPosts(post);

            var geolocation = await GetGeolocation();
            var userAgentString = Request.Headers["User-Agent"].ToString();
            var clientInfo = UAParser.Parser.GetDefault().Parse(userAgentString);

            var view = new PostView {
                PostId = post.Id,
                Source = source,
                Medium = medium,
                Campaign = campaign,
                Term = term,
                Content = content,
                City = geolocation.City,
                Continent = geolocation.Continent,
                Country = geolocation.Country,
                IpAddress = HttpContext.Connection.RemoteIpAddress,
                Region = geolocation.Region,
                UserAgent = userAgentString,
                UserAgentFamily = clientInfo.UA.Family,
                UserAgentVersion = clientInfo.UA.Version(),
                UserAgentDeviceFamily = clientInfo.Device.Family,
                UserAgentDeviceBrand = clientInfo.Device.Brand,
                UserAgentDeviceModel = clientInfo.Device.Model,
                UserAgentIsSpider = clientInfo.Device.IsSpider,
                UserAgentOSFamily = clientInfo.OS.Family
            };

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                _context.PostViews.Add(view);
                await _context.SaveChangesAsync();
            }

            return new OkObjectResult(new PostDetailResponse {
                Post = post.RenderBody(Request),
                NextPost = next,
                PreviousPost = previous
            });
        }

        [HttpGet("post/permalink/{postId}")]
        [ProducesDefaultResponseType(typeof(PostDetailResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post(Guid postId)
        {
            var post = await _context.Posts
                .Select(x => new PostDetailView {
                    Id = x.Id,
                    AuthorEmail = x.Author.Email,
                    AuthorName = x.Author.DisplayName,
                    IsPublished = x.IsPublished,
                    Published = x.Published,
                    Title = x.Title,
                    Slug = x.Slug,
                    Description = x.Description,
                    Body = x.Content,
                    Categories = x.Categories.Select(c => c.Id),
                    Tags = x.Tags.Select(t => t.Id),
                })
                .Where(x => x.Id == postId)
                .FirstOrDefaultAsync();

            if (post is null)
                return StatusCode(404);

            (var previous, var next) = await GetSuggestedPosts(post);

            return new OkObjectResult(new PostDetailResponse {
                Post = post.RenderBody(Request),
                NextPost = next,
                PreviousPost = previous
            });
        }

        [HttpPost("post/{id}/{slug}/comment")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post(Guid id, string slug, [Bind("Name,Email,Comment")] PostComment postComment)
        {
            var post = await _context.Posts
                .Include(x => x.Author)
                .Where(x => x.Id == id && x.Slug == slug && x.IsPublished && x.Published <= DateTimeOffset.Now)
                .FirstOrDefaultAsync();

            if (post is null)
                return StatusCode(404);

            if (ModelState.IsValid)
            {
                var geolocation = await GetGeolocation();
                var comment = new Comment {
                    PostId = post.Id,
                    Name = postComment.Name,
                    Email = postComment.Email,
                    Content = postComment.Comment,
                    City = geolocation.City,
                    Continent = geolocation.Continent,
                    Country = geolocation.Country,
                    IpAddress = HttpContext.Connection.RemoteIpAddress,
                    Region = geolocation.Region,
                };
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                post.Comments = await _context.Comments.Where(x => x.PostId == post.Id).ToListAsync();

                var commentNotification = new CommentNotification {
                    PostTitle = post.Title,
                    PostAuthorName = post.Author.DisplayName,
                    PostAuthorEmail = post.Author.Email,
                    CommenterEmail = postComment.Email,
                    CommenterName = postComment.Name,
                    Content = postComment.Comment,
                    CommentedOn = DateTime.Now
                };

                var statusCode = await _email.CommentNotificationEmail(commentNotification);
                return StatusCode((int)statusCode);
            }

            return BadRequest();
        }

        private async Task<(PostSuggestionLink previous, PostSuggestionLink next)> GetSuggestedPosts(PostDetailView post)
        {
            var previous = await _context.Posts
                .Where(x => x.IsPublished && x.Published >= DateTimeOffset.Now && x.Published < post.Published)
                .OrderBy(x => x.Published)
                .Select(x => new PostSuggestionLink {
                    Published = x.Published,
                    Slug = x.Slug,
                    Title = x.Title,
                })
                .FirstOrDefaultAsync();

            var next = await _context.Posts
                .Where(x => x.IsPublished && x.Published >= DateTimeOffset.Now && x.Published > post.Published)
                .OrderByDescending(x => x.Published)
                .Select(x => new PostSuggestionLink {
                    Published = x.Published,
                    Slug = x.Slug,
                    Title = x.Title,
                })
                .FirstOrDefaultAsync();

            return (previous, next);
        }

        private async Task<IpData> GetGeolocation()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress;

            return await _geolocationService.LookupAsync(ipAddress, HttpContext.Request.Host.Value);
        }
    }
}
