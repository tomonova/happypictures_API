using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace OICAR19_API.Controllers
{
    [Authorize]
    public class CardsController : ApiController
    {
       
        /// <summary>
        /// This interface returns all users cards and public(shared) cards
        /// </summary>
        [HttpGet]
        [Route("api/Cards/GetCards")]
        [ResponseType(typeof(List<CARD>))]
        public IHttpActionResult GetCards(int userID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    return Ok(db.CARDS
                        .Include(c => c.STORY_CARD)
                        .Include(c =>c.TAGS)
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
        /// This endpoint will return cards of a story with their respective order in the story.
        /// </summary>
        /// <param name="storyID"></param>
        /// <returns>CARD</returns>
        [HttpGet]
        [Route("api/Cards/GetStoryCards")]
        [ResponseType(typeof(List<CARD>))]
        public IHttpActionResult GetStoryCards(int storyID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    List<CARD> cardsDbSet = db.CARDS
                        .Include(c => c.TAGS)
                        .Include(c => c.FORMAT.IMAGE)
                        .Include(c => c.FORMAT.IMAGE1)
                        .Include(c => c.FORMAT.IMAGE2)
                        .Include(c => c.STORY_CARD).Where(sc=>sc.STORY_CARD.Any(x=>x.STORYID==storyID))
                        .ToList();
                    List<STORY_CARD> scDbSet = db.STORY_CARD.Where(sc=>sc.STORYID==storyID).ToList();
                    foreach (var card in cardsDbSet)
                    {
                        card.CardOrder = db.STORY_CARD
                            .Where(sc => sc.STORYID == storyID)
                            .Where(sc => sc.CARDID == card.IDCARD)
                            .FirstOrDefault().CARD_ORDER;
                    }
                    return Ok(cardsDbSet.OrderBy(c=>c.CardOrder));
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// Insert new card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Cards/InsertCard")]
        [ResponseType(typeof(int))]
        public IHttpActionResult InsertCard(CARD card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    card.FORMAT.IMG1ID = card.FORMAT.IMAGE?.IDIMAGE;
                    card.FORMAT.IMG2ID = card.FORMAT.IMAGE1?.IDIMAGE;
                    card.FORMAT.IMG3ID = card.FORMAT.IMAGE2?.IDIMAGE;
                    card.FORMAT.IMAGE = null;
                    card.FORMAT.IMAGE1 = null;
                    card.FORMAT.IMAGE2 = null;
                    db.CARDS.Add(card);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Content(HttpStatusCode.Created, new { idCard = card.IDCARD });
            }
        }

        /// <summary>
        /// Delete a card
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Cards/DeleteCard")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteCard(int cardID, int userID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    CARD oldCard = db.CARDS
                        .Include(c=>c.FORMAT)
                        .Include(c=>c.TAGS)
                        .Where(c => c.IDCARD == cardID)
                        .Where(c=>c.PROFILEID==userID)
                        .FirstOrDefault();
                    if (oldCard.SHARED==1)
                    {
                        return Content(HttpStatusCode.BadRequest, "You cannot delte shared card");
                    }
                    var sc = db.STORY_CARD.Where(x => x.CARDID == cardID).ToList();
                    foreach (var item in sc)
                    {
                        db.STORY_CARD.Remove(item);
                    }
                    db.CARDS.Remove(oldCard);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Ok();
            }
        }

        /// <summary>
        /// Edit existing card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Cards/EditCard")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditCard(CARD card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    CARD oldCard = db.CARDS.Where(c => c.IDCARD == card.IDCARD).FirstOrDefault();
                    if (oldCard.SHARED==Status.SHARED)
                    {
                        return Content(HttpStatusCode.BadRequest, "You cannot edit shared card");
                    }
                    card.FORMATID = oldCard.FORMATID;
                    oldCard.FORMAT.IDFORMAT = oldCard.FORMAT.IDFORMAT;
                    oldCard.FORMAT.IMG1ID = card.FORMAT.IMAGE?.IDIMAGE;
                    oldCard.FORMAT.IMG2ID = card.FORMAT.IMAGE1?.IDIMAGE;
                    oldCard.FORMAT.IMG3ID = card.FORMAT.IMAGE2?.IDIMAGE;
                    oldCard.FORMAT.COLOR = card.FORMAT.COLOR;
                    oldCard.FORMAT.FONT_SIZE = card.FORMAT.FONT_SIZE;
                    oldCard.FORMAT.FONT_FORMAT = card.FORMAT.FONT_SIZE;
                    oldCard.FORMAT.LAYOUT = card.FORMAT.LAYOUT;
                    oldCard.TEXT = card.TEXT;
                    if (card.SHARED==1)
                    {
                        PROFILE profile = db.PROFILES.Where(p => p.EMAIL == Status.ADMIN_ACCOUNT).FirstOrDefault();
                        oldCard.PROFILEID = profile.IDPROFILE;
                        oldCard.SHARED = 1;
                    }
                    SqlParameter cardID = new SqlParameter("@cardID", card.IDCARD);
                    db.Database.ExecuteSqlCommand("exec DeleteCardTags @cardID", cardID);
                    oldCard.TAGS = card.TAGS;
                    oldCard.NAME = card.NAME;
                    db.Entry(oldCard).State=EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Ok();
            }
        }
        /// <summary>
        /// Manage cards of a story
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Cards/ManageStoryCards")]
        [ResponseType(typeof(void))]
        public IHttpActionResult ManageStoryCards(int storyId, List<CARD> storyCards)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                int cardOrder = 1;
                try
                {
                    STORy story = db.STORIES.Where(s => s.IDSTORY == storyId).FirstOrDefault();
                    //if (story==null || story.SHARED==Status.SHARED)
                    //{
                    //    return Content(HttpStatusCode.BadRequest, "You can't edit shared story or the story doesn't exist");
                    //}
                    var cards_story = db.STORY_CARD.Where(sc => sc.STORYID == storyId);
                    db.STORY_CARD.RemoveRange(cards_story);
                    foreach (var item in storyCards)
                    {
                        STORY_CARD sc = new STORY_CARD
                        {
                            STORYID = storyId,
                            CARDID = item.IDCARD,
                            CARD_ORDER = cardOrder
                        };
                        cardOrder++;
                        db.STORY_CARD.Add(sc);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Ok();
            }
        }
    }
}
