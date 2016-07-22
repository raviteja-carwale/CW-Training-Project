using System.Web.Script.Serialization;
using log4net;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Training.BusinessLogic;
using Training.DTO;
using Training.Entities;
using Training.Utilities;

namespace Training.WebServices.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserProfileController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/v1/UserProfiles?pageNo={pno}
        [HttpGet]
        [Route("api/v1/UserProfiles")]
        public IHttpActionResult UserProfilePage(int pageNo = 1, string orderFormat = "1A")
        {
            try
            {
                if (pageNo > 0)
                {
                    Log.Info(string.Format("Client IP: {2}, User Profiles Requested. Page: {0}, OrderFormat: {1}.", pageNo, orderFormat, HttpContext.Current.Request.UserHostAddress));
                    DataProcessor dp = new DataProcessor();
                    List<UserProfile> userList = dp.GetUserProfiles(pageNo, orderFormat);
                    int count = dp.GetNumberOfUsers();
                    return Ok(new UsersList { count = count, userList = userList });
                }
                else
                {
                    Log.Error(string.Format("Client IP: {2}, Invaild request User Profiles Requested. Page: {0}, OrderFormat: {1}.", pageNo, orderFormat, HttpContext.Current.Request.UserHostAddress));
                    return BadRequest("Invalid Page Number");
                }
            }
            catch (Exception e)
            {
                Log.Fatal(string.Format("Client IP: {0}, Server Exception occured", HttpContext.Current.Request.UserHostAddress), e);
                return InternalServerError();
            }
        }

        // POST api/v1/UserProfiles
        [HttpPost]
        [Route("api/v1/UserProfiles")]
        public IHttpActionResult NewUserProfile([FromBody]UserProfile newUser)
        {
            try
            {
                if (!Utils.ValidUser(newUser))
                    return BadRequest("Invalid Form data");
                if (!Utils.ValidName(newUser.FirstName))
                    return BadRequest("Invalid first name");
                if (!Utils.ValidName(newUser.LastName))
                    return BadRequest("Invalid last name");
                if (!Utils.ValidDate(newUser.DateOfBirth))
                    return BadRequest("Invalid Date of Birth");
                if (!Utils.ValidGender(newUser.Gender))
                    return BadRequest("Invalid Gender");
                DataProcessor dp = new DataProcessor();
                newUser.Id = dp.InsertUserProfile(newUser);
                if (newUser.Id == 0)
                {
                    Log.Error(string.Format("Client IP: {1}, Invaild User Profile: {0}", new JavaScriptSerializer().Serialize(newUser), HttpContext.Current.Request.UserHostAddress));
                    return Conflict();
                }
                Log.Info(string.Format("Client IP: {1}, User Profile Inserted: {0}", new JavaScriptSerializer().Serialize(newUser), HttpContext.Current.Request.UserHostAddress));
                return Created(Request.RequestUri + "/" + newUser.Id, new { id = newUser.Id });
            }
            catch (Exception e)
            {
                Log.Fatal(string.Format("Client IP: {0}, Server Exception occured", HttpContext.Current.Request.UserHostAddress), e);
                return InternalServerError();
            }
        }

        // PUT api/v1/UserProfiles/{userId}
        [HttpPut]
        [Route("api/v1/UserProfiles/{userId}")]
        public IHttpActionResult UpdateUserProfile(int userId, [FromBody]UserProfile user)
        {
            try
            {
                if (!Utils.ValidUser(user))
                    return BadRequest("Invalid Form data");
                if (!Utils.ValidName(user.FirstName))
                    return BadRequest("Invalid first name");
                if (!Utils.ValidName(user.LastName))
                    return BadRequest("Invalid last name");
                if (!Utils.ValidDate(user.DateOfBirth))
                    return BadRequest("Invalid Date of Birth");
                if (!Utils.ValidGender(user.Gender))
                    return BadRequest("Invalid Gender");
                if (user.Id == userId)
                {
                    DataProcessor dp = new DataProcessor();
                    int resultCode = dp.UpdateUserProfile(user);
                    if (resultCode == -1)
                    {   // Duplicate entry
                        Log.Error(string.Format("Client IP: {1}, Invaild User Profile: {0}", new JavaScriptSerializer().Serialize(user), HttpContext.Current.Request.UserHostAddress));
                        return Conflict();
                    }
                    else if (resultCode == 0)
                    {   // Id not found
                        Log.Error(string.Format("Client IP: {1}, Invaild User Profile: {0}", new JavaScriptSerializer().Serialize(user), HttpContext.Current.Request.UserHostAddress));
                        return NotFound();
                    }
                    Log.Info(string.Format("Client IP: {1}, User Profile updated: {0}", new JavaScriptSerializer().Serialize(user), HttpContext.Current.Request.UserHostAddress));
                    return Ok();    // Updated success
                }
                return BadRequest("Request ID doesn't match ID in form data");
            }
            catch (Exception e)
            {
                Log.Fatal(string.Format("Client IP: {0}, Server Exception occured", HttpContext.Current.Request.UserHostAddress), e);
                return InternalServerError();
            }
        }

        // DELETE api/v1/UserProfiles/{userId}
        [HttpDelete]
        [Route("api/v1/UserProfiles/{userId}")]
        public IHttpActionResult Delete(int userId)
        {
            try
            {
                if (userId > 0)
                {
                    DataProcessor dp = new DataProcessor();
                    int resultCode = dp.RemoveUserProfile(userId);
                    if (resultCode == 0)
                    {
                        Log.Info(string.Format("Client IP: {1}, User Profile Deleted: {0}", userId, HttpContext.Current.Request.UserHostAddress));
                        return Ok();
                    }
                    return NotFound();
                }
                else
                {
                    return BadRequest("Invalid User ID");
                }
            }
            catch (Exception e)
            {
                Log.Fatal(string.Format("Client IP: {0}, Server Exception occured", HttpContext.Current.Request.UserHostAddress), e);
                return InternalServerError();
            }
        }
    }
}