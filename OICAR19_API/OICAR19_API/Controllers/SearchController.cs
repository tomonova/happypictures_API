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
                        .Include(s => s.TAGS)
                        .Where(s => s.PROFILEID == userID || s.SHARED == Status.SHARED)
                        .Where(s=>s.TAGS.Any(t=>t.VALUE==tag))
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
                        .Include(c => c.TAGS)
                        .Include(c => c.FORMAT.IMAGE)
                        .Include(c => c.FORMAT.IMAGE1)
                        .Include(c => c.FORMAT.IMAGE2)
                        .Where(c => c.SHARED.Equals(Status.SHARED) || c.PROFILEID == userID)
                        .Where(c=>c.TAGS.Any(t=>t.VALUE==tag))
                        .ToList());
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        ///// <summary>
        ///// Search Story by Name
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="userID"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/Search/SearchStoryByName")]
        //[ResponseType(typeof(STORy))]
        //public IHttpActionResult SearchStoryByName(string name, int userID)
        //{
        //    using (HappyPicturesDbContext db = new HappyPicturesDbContext())
        //    {
        //        try
        //        {
        //            var stories = db.STORIES
        //                .Include(s => s.IMAGE)
        //                .Include(s => s.TAGS)
        //                .Where(s => s.PROFILEID == userID || s.SHARED == Status.SHARED)
        //                .Where(s => s.NAME.Equals(name))
        //                .ToList();
        //            foreach (STORy story in stories)
        //            {
        //                story.NumberOfLikes = db.LIKES.Where(l => l.STORYID == story.IDSTORY).Count();
        //                if (db.LIKES.Find(userID, story.IDSTORY) != null)
        //                {
        //                    story.FAVOURITE = 1;
        //                }
        //            }
        //            return Ok(stories);
        //        }
        //        catch (Exception ex)
        //        {

        //            return Content(HttpStatusCode.BadRequest, ex.Message);
        //        }
        //    }
        //}
        ///// <summary>
        ///// Search Card by Name
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="userID"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/Search/SearchCardByName")]
        //[ResponseType(typeof(CARD))]
        //public IHttpActionResult SearchCardByName(string name,int userID)
        //{
        //    using (HappyPicturesDbContext db = new HappyPicturesDbContext())
        //    {
        //        try
        //        {
        //            return Ok(db.CARDS
        //                .Include(c => c.TAGS)
        //                .Include(c => c.FORMAT.IMAGE)
        //                .Include(c => c.FORMAT.IMAGE1)
        //                .Include(c => c.FORMAT.IMAGE2)
        //                .Where(c => c.SHARED.Equals(Status.SHARED) || c.PROFILEID == userID)
        //                .Where(c => c.NAME.Equals(name))
        //                .ToList());
        //        }
        //        catch (Exception ex)
        //        {

        //            return Content(HttpStatusCode.BadRequest, ex.Message);
        //        }
        //    }
        //}

        /// <summary>
        /// Search Tags on Cards
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/SearchCardTags")]
        [ResponseType(typeof(List<String>))]
        public IHttpActionResult SearchCardTags(int profileID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var tags = db.CARDS
                        .Include(c=>c.TAGS)
                        .Where(x=>x.SHARED==1 || x.PROFILEID==profileID);
                    List<String> listaTagova = new List<string>();
                    foreach (var card in tags)
                    {
                        foreach (var tag in card.TAGS)
                        {
                            listaTagova.Add(tag.VALUE);
                        }
                    }

                    return Ok(listaTagova);
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// Search Tags on Stories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Search/SearchStoryTags")]
        [ResponseType(typeof(List<String>))]
        public IHttpActionResult SearchStoryTags(int profileID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var tags = db.STORIES
                        .Include(s => s.TAGS)
                        .Where(x=>x.SHARED==1||x.PROFILEID==profileID);
                    List<String> listaTagova = new List<string>();
                    foreach (var story in tags)
                    {
                        foreach (var tag in story.TAGS)
                        {
                            listaTagova.Add(tag.VALUE);
                        }
                    }

                    return Ok(listaTagova);
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
    }
}

