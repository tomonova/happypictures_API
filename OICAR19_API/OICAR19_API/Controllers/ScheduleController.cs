using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;

namespace OICAR19_API.Controllers
{
    /// <summary>
    /// Controler for schedule management
    /// </summary>
    public class ScheduleController : ApiController
    {
        /// <summary>
        /// Return schedule with images for a chosen date
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="date">date format yyyy-mm-dd</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Schedules/GetSchedule")]
        [ResponseType(typeof(SCHEDULE))]
        public IHttpActionResult GetSchedule(int userID, DateTime date)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    SCHEDULE schedule = db.SCHEDULES
                        .Where(s => s.PROFILEID == userID)
                        .Where(s => s.SCHEDULE_DATE == date)
                        .Include(si => si.SCHEDULE_IMAGE.Select(i=>i.IMAGE))
                        .FirstOrDefault();
                    return Ok(schedule);
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// Insert a new scheudle
        /// </summary>
        /// <param name="schedule">When insertin provide the following: PROFILEID, SCHEDULE_DATE and SCHEDULE_IMAGE
        /// for SCHEDULE_IMAGE properties provide only IMAGEID and POSITION</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Schedules/InsertSchedule")]
        [ResponseType(typeof(int))]
        public IHttpActionResult InsertSchedule(SCHEDULE schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    foreach (var item in schedule.SCHEDULE_IMAGE)
                    {
                        item.IMAGE = null;
                        if (item.IMAGEID==0)
                        {
                            return Content(HttpStatusCode.BadRequest, "IMAGEID is mandatory");
                        }
                    }
                    db.SCHEDULES.Add(schedule);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Content(HttpStatusCode.Created, new { idSchedule = schedule.IDSCHEDULE });
            }
        }
        /// <summary>
        /// Edit an existing schedule
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/Schedules/EditSchedule")]
        [ResponseType(typeof(void))]
        public IHttpActionResult EditSchedule(SCHEDULE schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    if (!ScheduleExists(schedule.IDSCHEDULE))
                    {
                        return Content(HttpStatusCode.NotFound,"Schedule does not exist");
                    }
                    SCHEDULE existingEntity = db.SCHEDULES
                        .Where(s => s.IDSCHEDULE == schedule.IDSCHEDULE)
                        .Include(si => si.SCHEDULE_IMAGE.Select(i => i.IMAGE))
                        .FirstOrDefault();
                    if (existingEntity!=null)
                    {
                        db.Entry(existingEntity).CurrentValues.SetValues(schedule);
                        foreach (var existingChild in existingEntity.SCHEDULE_IMAGE.ToList())
                        {
                            if (!schedule.SCHEDULE_IMAGE.Any(c=>c.IDSCHEDULE_IMAGE==existingChild.IDSCHEDULE_IMAGE))
                            {
                                db.SCHEDULE_IMAGE.Remove(existingChild);
                            }
                        }
                    }
                    foreach (var childSchedule in schedule.SCHEDULE_IMAGE)
                    {
                        var existingChild = existingEntity.SCHEDULE_IMAGE
                            .Where(c => c.IDSCHEDULE_IMAGE == childSchedule.IDSCHEDULE_IMAGE && c.IDSCHEDULE_IMAGE != default(int))
                            .SingleOrDefault();
                        if (existingChild!=null)
                        {
                            db.Entry(existingChild).CurrentValues.SetValues(childSchedule);
                        }
                        else
                        {
                            var newSI = new SCHEDULE_IMAGE
                            {
                                IMAGEID = childSchedule.IMAGEID,
                                POSITION = childSchedule.POSITION,
                                SCHEDULEID = schedule.IDSCHEDULE

                            };
                            existingEntity.SCHEDULE_IMAGE.Add(newSI);
                        }
                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        /// <summary>
        /// Delete a schedule
        /// </summary>
        /// <param name="idSchedule"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Schedules/DeleteSchedule")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteSchedule(int idSchedule)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    SCHEDULE schedule = db.SCHEDULES
                                        .Where(s => s.IDSCHEDULE == idSchedule)
                                        .Include(si => si.SCHEDULE_IMAGE)
                                        .FirstOrDefault();
                    if (schedule == null)
                    {
                        return Content(HttpStatusCode.NotFound, "Schedule does not exist");
                    }

                    db.SCHEDULE_IMAGE.RemoveRange(schedule.SCHEDULE_IMAGE);
                    db.SCHEDULES.Remove(schedule);
                        db.SaveChanges();
                        return Content(HttpStatusCode.OK, "Schedule deleted");
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        private bool ScheduleExists(int idschedule)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                return db.SCHEDULES.Count(e => e.IDSCHEDULE == idschedule) > 0;
            }
        }
    }
}
