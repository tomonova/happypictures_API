using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;

namespace OICAR19_API.Controllers
{
    public class SearchController : ApiController
    {
        /// <summary>
        /// Search Story by single TAG
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/SearchStoryByTag")]
        [ResponseType(typeof(STORy))]
        public IHttpActionResult SearchStoryByTag(string tag, int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var stories = db.STORIES
                        .Include(s => s.IMAGE)
                        .Include(s => s.TAGS.Where(t => t.VALUE.Equals(tag)))
                        .Where(s => s.PROFILEID == userID || s.SHARED == Status.SHARED)
                        .ToList();
                    foreach (STORy story in stories)
                    {
                        story.NumberOfLikes = db.LIKES.Where(l => l.STORYID == story.IDSTORY).Count();
                        if (db.LIKES.Find(userID, story.IDSTORY) != null)
                        {
                            story.FAVOURITE = 1;
                        }
                    }
                    return Ok(stories);
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// Search Card by single TAG
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/SearchCardByTag")]
        [ResponseType(typeof(CARD))]
        public IHttpActionResult SearchCardByTag(string tag,int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    return Ok(db.CARDS
                        .Include(c => c.TAGS.Where(t => t.VALUE.Equals(tag)))
                        .Include(c => c.FORMAT.IMAGE)
                        .Include(c => c.FORMAT.IMAGE1)
                        .Include(c => c.FORMAT.IMAGE2)
                        .Where(c => c.SHARED.Equals(Status.SHARED) || c.PROFILEID == userID)
                        .ToList());
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// Search Story by Name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/SearchStoryByName")]
        [ResponseType(typeof(STORy))]
        public IHttpActionResult SearchStoryByName(string name, int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var stories = db.STORIES
                        .Include(s => s.IMAGE)
                        .Include(s => s.TAGS)
                        .Where(s => s.PROFILEID == userID || s.SHARED == Status.SHARED)
                        .Where(s => s.NAME.Equals(name))
                        .ToList();
                    foreach (STORy story in stories)
                    {
                        story.NumberOfLikes = db.LIKES.Where(l => l.STORYID == story.IDSTORY).Count();
                        if (db.LIKES.Find(userID, story.IDSTORY) != null)
                        {
                            story.FAVOURITE = 1;
                        }
                    }
                    return Ok(stories);
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// Search Card by Name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/SearchCardByName")]
        [ResponseType(typeof(CARD))]
        public IHttpActionResult SearchCardByName(string name,int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    return Ok(db.CARDS
                        .Include(c => c.TAGS)
                        .Include(c => c.FORMAT.IMAGE)
                        .Include(c => c.FORMAT.IMAGE1)
                        .Include(c => c.FORMAT.IMAGE2)
                        .Where(c => c.SHARED.Equals(Status.SHARED) || c.PROFILEID == userID)
                        .Where(c => c.NAME.Equals(name))
                        .ToList());
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
    }
}

