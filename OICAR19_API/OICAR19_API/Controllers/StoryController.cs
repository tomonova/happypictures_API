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
    public class StoryController : ApiController
    {
        /// <summary>
        /// This interface returns popular public stories which are not created by user
        /// </summary>
        [HttpGet]
        [Route("api/Stories/DiscoverStories")]
        [ResponseType(typeof(STORy))]
        public IHttpActionResult DiscoverStories(int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var stories = db.STORIES
                        .Include(s => s.IMAGE)
                        .Include(s => s.TAGS)
                        .Where(s => s.PROFILEID != userID && s.SHARED == Status.SHARED)
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
        /// This interface returns all users stories
        /// </summary>
        [HttpGet]
        [Route("api/Stories/GetUserStories")]
        [ResponseType(typeof(STORy))]
        public IHttpActionResult GetUserStories(int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var stories = db.STORIES
                        .Include(s => s.IMAGE)
                        .Include(s => s.TAGS)
                        .Where(s => s.PROFILEID == userID)
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
        /// This interface returns all stories that users liked
        /// </summary>
        [HttpGet]
        [Route("api/Stories/GetUserFavouriteStories")]
        [ResponseType(typeof(STORy))]
        public IHttpActionResult GetUserFavouriteStories(int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var stories = db.STORIES
                        .Include(s => s.IMAGE)
                        .Include(s => s.TAGS)
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
                    var favouriteStories = stories.Where(s => s.FAVOURITE == Status.LIKED);
                    return Ok(favouriteStories);
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// This interface receives a story and inserts it in the database.
        /// Response will have newly created ID of a story
        /// </summary>
        /// <param name="story"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Story/InsertStory")]
        [ResponseType(typeof(STORy))]
        public IHttpActionResult InsertStory(STORy story)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    if (db.IMAGES.Find(story.IMAGE.IDIMAGE) != null)
                    {
                        story.THUMBNAIL = story.IMAGE.IDIMAGE;
                        story.IMAGE = null;
                    }
                    db.STORIES.Add(story);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Content(HttpStatusCode.Created, new { IDSTORY = story.IDSTORY });
            }
        }

        /// <summary>
        /// This interface must receive story ID and the story, it will perform an update of the story. If story does not exist it will not be inserted
        /// </summary>
        [HttpPut]
        [Route("api/Story/UpdateStory")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateImage(int idStory, STORy story)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (idStory != story.IDSTORY)
            {
                return BadRequest("ID of story not correct");
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    if (db.IMAGES.Find(story.IMAGE.IDIMAGE) != null)
                    {
                        story.THUMBNAIL = story.IMAGE.IDIMAGE;
                        story.IMAGE = null;
                    }
                    db.Entry(story).State = EntityState.Modified; ;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (!StoryExists(idStory))
                    {
                        return NotFound();
                    }
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        private bool StoryExists(int idStory)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                return db.STORIES.Count(s => s.IDSTORY == idStory) > 0;
            }
        }
    }
}
