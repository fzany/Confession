using Backend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("test/getusers")]
        public ActionResult<List<UserData>> GetUsers()
        {
            try
            {
                //Store.UserClass.DropUsers();
                List<UserData> list = Store.UserClass.FetchAll();

                //return data
                return Ok(list);
            }
            catch
            {
                return new List<UserData>();
            }
        }

        [HttpGet]
        [Route("test/resetchat")]
        public ActionResult<string> ResetChat()
        {
            try
            {
                Store.Migrate.ResetChat();

                //return data
                return Ok("Done");
            }
            catch
            {
                return BadRequest();
            }

        }

        //[HttpGet]
        //[Route("test/fetchall")]
        //public ActionResult<List<Confess>> FetchAll()
        //{
        //    try
        //    {
        //        //prepare responses
        //        List<Confess> response = Store.Migrate.FetchLite.FetchConfess();

        //        //test for emptiness
        //        if (response.Count() == 0)
        //        {
        //            return Ok(new List<Confess>() { });
        //        }

        //        //return data
        //        return Ok(response);
        //    }
        //    catch
        //    {
        //        return new List<Confess>();
        //    }

        //}
        //[HttpGet]
        //[Route("test/add")]
        //public ActionResult<List<Confess>> Add()
        //{
        //    try
        //    {
        //        Confess confess = new Confess
        //        {
        //            Title = "Test Confess",
        //            Body = "This is a test confess",
        //            Category = "Family",
        //            Owner_Guid = "femi"

        //        };
        //        Store.Migrate.Create.CreateConfess(confess);

        //        return Store.Migrate.FetchLite.FetchConfess();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}

        //[HttpGet]
        //[Route("test/clear")]
        //public ActionResult Clear()
        //{
        //    try
        //    {
        //        //clear LiteDB first
        //        Store.Migrate.ClearLiteDB();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}

        //[HttpGet]
        //[Route("test/migrate")]
        //public ActionResult<MigrationReport> Migrate()
        //{
        //    try
        //    {
        //        Store.Migrate.Create.CreateConfess(Store.Migrate.Fetch.FetchConfess());
        //        Store.Migrate.Create.CreateUser(Store.Migrate.Fetch.FetchUser());
        //        Store.Migrate.Create.CreateComment(Store.Migrate.Fetch.FetchComment());
        //        Store.Migrate.Create.CreateLikes(Store.Migrate.Fetch.FetchLikes());
        //        Store.Migrate.Create.CreateDislikes(Store.Migrate.Fetch.FetchDislikes());
        //        Store.Migrate.Create.CreateSeen(Store.Migrate.Fetch.FetchSeen());

        //        MigrationReport migrationReport = new MigrationReport();

        //        return Ok(migrationReport);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}
        //public class MigrationReport
        //{
        //    public List<Confess> Confesses { get; set; } = Store.Migrate.FetchLite.FetchConfess();
        //    public List<Likes> Likes { get; set; } = Store.Migrate.FetchLite.FetchLikes();
        //    public List<Dislikes> Dislikes { get; set; } = Store.Migrate.FetchLite.FetchDislikes();
        //    public List<Seen> Seens { get; set; } = Store.Migrate.FetchLite.FetchSeen();
        //    public List<User> Users { get; set; } = Store.Migrate.FetchLite.FetchUser();
        //    public List<Comment> Comments { get; set; } = Store.Migrate.FetchLite.FetchComment();
        //}

        //[HttpGet]
        //[Route("test/migratetomongo")]
        //public ActionResult<Migrator> MigrateToMongo()
        //{
        //    try
        //    {
        //        Migrator data = Store.Migrate.MigrateToMongo.LoadMigration();
        //        return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}
    }
}