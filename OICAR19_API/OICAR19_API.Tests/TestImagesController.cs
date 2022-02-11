using Microsoft.VisualStudio.TestTools.UnitTesting;
using OICAR19_API.Controllers;
using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;

namespace OICAR19_API.Tests
{
    [TestClass]
    public class TestImagesController
    {
        [TestMethod]
        public void GetImages_MustReturnSharedImage()
        {
            var context = new TestImageDbContext();
            context.Images.Add(new IMAGE { IDIMAGE = 12, IMGTIMESTAMP = new DateTime(1999, 11, 12), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/1234546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 22, IMGTIMESTAMP = new DateTime(2010, 11, 12), PROFILEID = 4, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 32, IMGTIMESTAMP = new DateTime(2012, 10, 13), PROFILEID = 6, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 42, IMGTIMESTAMP = new DateTime(2014, 9, 14), PROFILEID = 4, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 52, IMGTIMESTAMP = new DateTime(2016, 8, 15), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });

            var imgController = new ImagesController(context);
            var result = imgController.GetImages() as OkNegotiatedContentResult<List<IMAGE>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(Status.SHARED,result.Content[0].SHARED);
            Assert.AreEqual(2, result.Content.Count);

        }
        [TestMethod]
        public void GetUserImages_MustReturnUsersImage()
        {
            var context = new TestImageDbContext();
            context.Images.Add(new IMAGE { IDIMAGE = 12, IMGTIMESTAMP = new DateTime(1999, 11, 12), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/1234546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 22, IMGTIMESTAMP = new DateTime(2010, 11, 12), PROFILEID = 4, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 32, IMGTIMESTAMP = new DateTime(2012, 10, 13), PROFILEID = 6, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 42, IMGTIMESTAMP = new DateTime(2014, 9, 14), PROFILEID = 4, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 52, IMGTIMESTAMP = new DateTime(2016, 8, 15), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });

            var imgController = new ImagesController(context);
            var result = imgController.GetUserImages(2) as OkNegotiatedContentResult<List<IMAGE>>;
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Content[0].PROFILEID);
            Assert.AreEqual(2, result.Content.Count);

        }

        [TestMethod]
        public void UpdateImage_MustReturnUsersBadRequest()
        {
            var context = new TestImageDbContext();
            var imgController = new ImagesController(context);
            var result = imgController.UpdateImage(2,GetTestImage());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }
        [TestMethod]
        public void UpdateImage_MustReturnOK()
        {
            var context = new TestImageDbContext();
            var imgController = new ImagesController(context);
            var result = imgController.UpdateImage(17, GetTestImage());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void InsertImage_MustReturnOK()
        {
            var context = new TestImageDbContext();
            var imgController = new ImagesController(context);
            var result = imgController.InsertImage(GetTestImage()) as NegotiatedContentResult<IMAGE>;
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Created,result.StatusCode);
        }

        [TestMethod]
        public void DeleteImage_MustReturnNotFound()
        {
            var context = new TestImageDbContext();
            context.Images.Add(new IMAGE { IDIMAGE = 12, IMGTIMESTAMP = new DateTime(1999, 11, 12), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/1234546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 22, IMGTIMESTAMP = new DateTime(2010, 11, 12), PROFILEID = 4, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 32, IMGTIMESTAMP = new DateTime(2012, 10, 13), PROFILEID = 6, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 42, IMGTIMESTAMP = new DateTime(2014, 9, 14), PROFILEID = 4, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 52, IMGTIMESTAMP = new DateTime(2016, 8, 15), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });
            var imgController = new ImagesController(context);
            var result = imgController.DeleteImage(999) as NegotiatedContentResult<string>;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content.ToString(), "Image does not exist");
        }

        [TestMethod]
        public void DeleteImage_MustReturnOK()
        {
            var context = new TestImageDbContext();
            context.Images.Add(new IMAGE { IDIMAGE = 12, IMGTIMESTAMP = new DateTime(1999, 11, 12), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/1234546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 22, IMGTIMESTAMP = new DateTime(2010, 11, 12), PROFILEID = 4, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 32, IMGTIMESTAMP = new DateTime(2012, 10, 13), PROFILEID = 6, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 42, IMGTIMESTAMP = new DateTime(2014, 9, 14), PROFILEID = 4, SHARED = 1, URL = "http://pictures.org/12335546.jpg" });
            context.Images.Add(new IMAGE { IDIMAGE = 52, IMGTIMESTAMP = new DateTime(2016, 8, 15), PROFILEID = 2, SHARED = 0, URL = "http://pictures.org/12335546.jpg" });
            var imgController = new ImagesController(context);
            var result = imgController.DeleteImage(12) as NegotiatedContentResult<string>;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content.ToString(), "Image deleted");
        }

        private IMAGE GetTestImage()
        {
            return new IMAGE {
                IDIMAGE = 17,
                IMGTIMESTAMP = new DateTime(2011, 11, 12),
                PROFILEID = 5,
                SHARED = 0,
                URL = "http://pictures.org/1234546.jpg"
            } ;


        }
    }
}
