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
    public class CardsController : ApiController
    {
        /// <summary>
        /// This interface returns all users cards and public(shared) cards
        /// </summary>
        [HttpGet]
        [Route("api/Cards/GetCards")]
        [ResponseType(typeof(CARD))]
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
        [ResponseType(typeof(CARD))]
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
    }
}
