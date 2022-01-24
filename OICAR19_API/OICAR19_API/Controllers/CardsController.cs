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
        [HttpGet]
        [Route("api/Cards/GetStoryCards")]
        [ResponseType(typeof(CARD))]
        public IHttpActionResult GetStoryCards(int storyID)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    
                    //var cards = db.CARDS.SqlQuery("exec GetStoryCards @param1", new SqlParameter("param1", storyID)).ToList();
                    //var test = db.Database.ExecuteSqlCommand("exec GetStoryCards @param1", new SqlParameter("param1", storyID));

                    List<CARD> cardsDbSet = db.CARDS
                        .Include(c => c.TAGS)
                        .Include(c => c.FORMAT.IMAGE)
                        .Include(c => c.FORMAT.IMAGE1)
                        .Include(c => c.FORMAT.IMAGE2)
                        .Include(c => c.STORY_CARD).Where(sc=>sc.STORY_CARD.Any(x=>x.STORYID==storyID))
                        .ToList();
                    List<STORY_CARD> scDbSet = db.STORY_CARD.Where(sc=>sc.STORYID==storyID).ToList();
                    //foreach (var item in cardsDbSet)
                    //{
                    //    if (item.)
                    //    {

                    //    }
                    //}


                    return Ok(cardsDbSet);
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
    }
}
