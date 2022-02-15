using OICAR19_API.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace OICAR19_API.Controllers
{
    [Authorize]
    public class ImagesController : ApiController
    {
        [Authorize]
        /// <summary>
        /// This interface returns all public (shared) images
        /// </summary>
        [HttpGet]
        [Route("api/Images/GetImages")]
        [ResponseType(typeof(IMAGE))]
        public IHttpActionResult GetImages()
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    return Ok(db.IMAGES
                        .Where(i => i.SHARED.Equals(Status.SHARED))
                        .ToList());
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// This interface returns all user images
        /// </summary>
        [HttpGet]
        [Route("api/Images/GetUserImages")]
        [ResponseType(typeof(IMAGE))]
        public IHttpActionResult GetUserImages(int userId)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    var img = db.IMAGES.Where(i => i.PROFILEID.Equals(userId)).ToList();
                    return Ok(img);
                }
                catch (Exception ex)
                {

                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
            }
        }
        /// <summary>
        /// This interface must receive image ID and the image, it will perform an update of the image. If Image does not exist it will not be inserted
        /// </summary>
        [HttpPut]
        [Route("api/Images/UpdateImage")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateImage(int idImage, IMAGE image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(idImage != image.IDIMAGE)
            {
                return BadRequest("ID of image not correct");
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    db.Entry(image).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (!ImageExists(idImage))
                    {
                        return NotFound();
                    }
                    return Content(HttpStatusCode.BadRequest,ex.Message);
                }
                return StatusCode(HttpStatusCode.OK);
            }
        }
        /// <summary>
        /// This interface receives an image and inserts it in the database.
        /// Response will have newly created ID of an image
        /// </summary>
        /// <param name="image"></param>
        [HttpPost]
        [Route("api/Images/InsertImage")]
        [ResponseType(typeof(IMAGE))]
        public IHttpActionResult InsertImage(IMAGE image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    db.IMAGES.Add(image);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return Content(HttpStatusCode.Created,new { idImage = image.IDIMAGE });
            }
        }
        /// <summary>
        /// This interface receives id of an image and trys to delete it.
        /// If Image wass shared, as agreed by license agreement it will not be deleted, it is transfered to Happy Pictures account and will remain shared
        /// </summary>
        [HttpDelete]
        [Route("api/Images/DeleteImage")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteImage(int idImage)
        {
            using(HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                try
                {
                    IMAGE image = db.IMAGES.Find(idImage);
                    if (image == null)
                    {
                        return Content(HttpStatusCode.NotFound, "Image does not exist");
                    }
                    if (!ImageExists(idImage))
                    {
                        return NotFound();
                    }
                    if (image.SHARED == Status.NOT_SHARED)
                    {
                        db.IMAGES.Remove(image);
                        db.SaveChanges();
                        return Content(HttpStatusCode.OK, "Image deleted");
                    }
                    if (image.SHARED==Status.SHARED)
                    {
                        int adminID = db.GetAdminID(Status.ADMIN_ACCOUNT).FirstOrDefault().GetValueOrDefault();
                        image.PROFILEID = adminID;
                        db.SaveChanges();
                        return Content(HttpStatusCode.OK, "Image is shared, under license agreement deletion is not performed. Owner of Image was changed");
                    }
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex.Message);
                }
                return BadRequest();
            }
        }

        private bool ImageExists(int idImage)
        {
            using (HappyPicturesDbContext db = new HappyPicturesDbContext())
            {
                return db.IMAGES.Count(e => e.IDIMAGE == idImage) > 0;
            }
        }
    }
}
