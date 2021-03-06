using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.SqlClient;

namespace OICAR19_API.Controllers
{
    /// <summary>
    /// Controller that returns all story related entities
    /// </summary>
    [Authorize]
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
        public IHttpActionResult InsertStory(int userID,STORy story)
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
                        story.FAVOURITE = 0;
                        story.PROFILEID = userID;
                        story.IMAGE = null;
                    }
                    db.STORIES.Add(story);
                    db.SaveChanges();
                    if (story.SHARED == Status.SHARED)
                    {
                        SqlParameter profileID = new SqlParameter("@profileID", userID);
                        SqlParameter storyID = new SqlParameter("@storyID", story.IDSTORY);
                        SqlParameter admin = new SqlParameter("admin", Status.ADMIN_ACCOUNT);
                        db.Database.ExecuteSqlCommand("exec ShareStory @profileID, @storyID,@admin", profileID, storyID, admin);
                    }
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Content(HttpStatusCode.Created, new { IDSTORY = story.IDSTORY });
            }
        }
        /// <summary>
        /// This endpoint serves as "like" status change for a story
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="storyID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Story/LikeStory")]
        [ResponseType(typeof(void))]
        public IHttpActionResult LikeStory(int userID, int storyID, bool status)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    if (!StoryExists(storyID))
                    {
                        return Content(HttpStatusCode.BadRequest, "Story doesn't exist");
                    }
                    if (status==false && db.LIKES.Any(l=>l.PROFILEID==userID&& l.STORYID==storyID))
                    {
                        db.LIKES.Remove(db.LIKES.Single(l =>l.PROFILEID==userID&&l.STORYID==storyID));
                        db.SaveChanges();
                        return Ok();
                    }
                    if (status==true)
                    {
                        db.LIKES.Add(new LIKE { PROFILEID = userID, STORYID = storyID });
                        db.SaveChanges();
                        return Ok();
                    }

                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return BadRequest();
            }
        }

        /// <summary>
        /// This interface must receive  user ID and the story, it will perform an update of the story.
        /// </summary>
        [HttpPut]
        [Route("api/Story/UpdateStory")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateStory(int userID, STORy story)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!UsersStory(userID,story))
                return Content(HttpStatusCode.BadRequest, "you can only change your stories");

            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    STORy oldStory = db.STORIES
                        .Include(s => s.LIKES)
                        .Include(s => s.STORY_CARD)
                        .Include(s => s.TAGS)
                        .Where(s => s.IDSTORY == story.IDSTORY)
                        .FirstOrDefault();
                    if (oldStory.SHARED==Status.SHARED)
                    {
                        return Content(HttpStatusCode.BadRequest, "You cannot edit shared story");
                    }
                    if (db.IMAGES.Find(story.IMAGE.IDIMAGE) != null)
                    {
                        oldStory.THUMBNAIL = story.IMAGE.IDIMAGE;
                    }
                    SqlParameter profileID = new SqlParameter("@profileID", userID);
                    SqlParameter storyID = new SqlParameter("@storyID", story.IDSTORY);
                    SqlParameter admin = new SqlParameter("admin", Status.ADMIN_ACCOUNT);
                    db.Database.ExecuteSqlCommand("exec DeleteStoryTags @storyID", storyID);
                    oldStory.STORY_CARD = story.STORY_CARD;
                    oldStory.TAGS = story.TAGS;
                    oldStory.NAME = story.NAME;
                    oldStory.DESCRIPTION = story.DESCRIPTION;
                    db.Entry(oldStory).State = EntityState.Modified; ;
                    db.SaveChanges();
                    if (story.SHARED == Status.SHARED)
                    {
                        db.Database.ExecuteSqlCommand("exec ShareStory @profileID, @storyID,@admin", profileID, storyID, admin);
                    }
                }
                catch (Exception ex)
                {
                    if (!StoryExists(story.IDSTORY))
                    {
                        return NotFound();
                    }
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Delete a card
        /// </summary>
        /// <param name="storyID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Story/DeleteStory")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteStory(int storyID, int userID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    STORy oldStory = db.STORIES
                        .Include(s=>s.STORY_CARD)
                        .Include(s=>s.TAGS)
                        .Include(s=>s.LIKES)
                        .Where(s => s.IDSTORY == storyID)
                        .Where(c => c.PROFILEID == userID)
                        .FirstOrDefault();
                    if (oldStory.SHARED == 1)
                    {
                        return Content(HttpStatusCode.BadRequest, "You cannot delete shared story");
                    }
                    var sc = db.STORY_CARD.Where(x => x.STORYID == storyID).ToList();
                    foreach (var item in sc)
                    {
                        db.STORY_CARD.Remove(item);
                    }
                    db.STORIES.Remove(oldStory);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Ok();
            }
        }

        private bool UsersStory(int userID, STORy story)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    STORy checkStory = db.STORIES
                        .Where(s => s.IDSTORY == story.IDSTORY)
                        .FirstOrDefault<STORy>();
                    if (checkStory.PROFILEID==userID)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return false;
            }
        }

        private bool StoryExists(int idStory)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    return db.STORIES.Count(s => s.IDSTORY == idStory) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
