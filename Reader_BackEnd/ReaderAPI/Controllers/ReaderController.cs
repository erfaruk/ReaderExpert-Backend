using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ReaderAPI.Models.Database;
using ReaderAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReaderAPI.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("[controller]")]
    public class ReaderController : Controller
    {
        
        [HttpGet]
        [Route("[action]")]
        public IActionResult List()
        {
            using var context = new ReaderExpertContext();

            var readers = (from p in context.Pitreaders
                           join t in context.Transponders on p.KeyId equals t.KeyId
                           into transponderJoin
                           from transponder in transponderJoin.DefaultIfEmpty()
                           join u in context.Users on transponder.UserId equals u.UserId // Örnek ilişki, uygun olanı kullanın
                           into userJoin
                           from user in userJoin.DefaultIfEmpty()
                           select new
                           {
                               p.ReaderId,
                               p.Ipaddress,
                               p.Name,
                               p.Location,
                               p.Apitoken,
                               p.ServerId,
                               p.BlockListId,
                               p.Port,
                               p.Fingerprint,
                               p.Status,
                               p.IsKeyIn,
                               p.KeyId,
                               p.Permission,
                               keySecurityId = transponder.SecurityId,
                               keyOrderNo = transponder.OrderNo,
                               keySerialNo = transponder.SerialNo,
                               keyStartTime = transponder.StartTime,
                               keyEndDate = transponder.EndDate,
                               keyUserId = transponder.UserId,
                               userName = user.Name!=null? user.Name + " " + user.Surname : null ,
                               isActiveUser = user.Active
                           }).ToList();
            return Ok(readers);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Users()
        {
            using var context = new ReaderExpertContext();
            var readers = context.Users.
                Select(c => new
                {
                    c.UserId,
                    c.Name,
                    c.Surname,
                    c.Role,
                    c.Email,
                    c.PhoneNumber,
                    c.Company,
                    c.Description,
                    c.Active,
                }).ToList();
            return Ok(readers);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Keys()
        {
            using var context = new ReaderExpertContext();
            var readers = context.Transponders.
                Select(c => new
                {
                    c.KeyId,
                    c.UserId,
                    c.OrderNo,
                    c.SerialNo,
                    c.SecurityId,
                    c.StartTime,
                    c.EndDate
                }).ToList();
            return Ok(readers);
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult KeyDetail(int keyId)
        {
            using var context = new ReaderExpertContext();
            var key = context.Transponders.Find(keyId);
            if (key == null)
            {
                return NotFound(new { Message = "Customer Not Found" });
            }
            return Ok(new
            {
                key.OrderNo,
                key.SerialNo,
                key.SecurityId,
                key.UserId,
            });
            
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult UserDetail(int userId)
        {
            using var context = new ReaderExpertContext();
            var user = context.Users.Find(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User Not Found" });
            }
            return Ok(new
            {
                user.Name,
                user.Surname,
                user.Email,
                user.Active,
                user.Role
            });

        }

        
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddUser([FromBody] UserAdd model)
        {
            try
            {
                using var context = new ReaderExpertContext();
                var user = new User 
                {
                   Name=model.Name,
                   Surname=model.Surname,
                   Email=model.Email,
                   Active=model.Active,
                   Role=model.Role,
                   PhoneNumber=model.PhoneNumber,
                   Company=model.Company,
                   Description=model.Description
                };

                context.Users.Add(user);

                context.SaveChanges();

                return Ok();

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddKey([FromBody] KeyAdd model)
        {
            try
            {
                using var context = new ReaderExpertContext();
                var key = new Transponder
                {
                   SecurityId= model.SecurityId,
                   SerialNo=model.SerialNo,
                   OrderNo=model.OrderNo,
                   StartTime=model.StartTime,
                   EndDate=model.EndDate,
                   UserId=model.UserId,
                };

                context.Transponders.Add(key);

                context.SaveChanges();

                return Ok();

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
