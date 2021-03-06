using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using belt2.Models;

namespace belt2.Controllers
{
    public class DojoActivityCenterController : Controller
    {

        private DojoActivityCenterContext _context;

        public DojoActivityCenterController(DojoActivityCenterContext context)
        {
            _context = context;
        }

        private string getDurationTypeStr(int type)
        {
            if (type == 1)
                return "Hours";
            if (type == 2)
                return "Minutes";
            return "Days";
        }

        private int getDurationNormalized(int type, int duration)
        {
            if (type == 1)
                return duration * 60;
            if (type == 2)
                return duration;
            return 24 * 60 * duration;
        }

        bool isConflict(DateTime requestedDateTime, int requestedDuration, DateTime possibleConfilctDateTime, int possibleDuration)
        {

            DateTime endX = requestedDateTime.AddMinutes(requestedDuration);
            DateTime endY = possibleConfilctDateTime.AddMinutes(possibleDuration);

            DateTime startX = requestedDateTime;
            DateTime startY = possibleConfilctDateTime;

            if (startX < startY && endX < startY)
                return false;
                
            if (startX > endY )
                return false;

            if (startX <= startY && endX <= endY)
                return true;
            if (startX <= startY && endX >= endY)
                return true;

            if (startX >= startY && endX <= endY)
                return true;

            return false;
        }

        // GET: /Home/       
        [HttpGet]
        [Route("Home")]
        public IActionResult DashBoard()
        {

            if (HttpContext.Session.GetObjectFromJson<List<string>>("JoinError") != null)
            {

                ViewBag.JoinError = HttpContext.Session.GetObjectFromJson<List<string>>("JoinError");
                HttpContext.Session.Remove("JoinError");

            }

            int? userLoginId = HttpContext.Session.GetInt32("userId");
            if (userLoginId != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("userName");
                List<EventDetails> AllEvents = _context.Events
                .Include(e => e.User)
                .Include(e => e.Participants)
                .Where(e => e.EventDateTime.AddMinutes(this.getDurationNormalized(e.DurationType, e.Duration)) >= DateTime.Now)
                .Select(e => new EventDetails
                {
                    id = e.id,
                    Name = e.Name,
                    Description = e.Description,
                    CreatedAt = e.CreatedAt,
                    EventDateTime = e.EventDateTime,
                    DurationStr = $"{e.Duration} {this.getDurationTypeStr(e.DurationType)}",
                    userLoginId = (int)userLoginId,
                    CreatorId = e.UserId,
                    Creator = e.User,
                    Participants = e.Participants,
                }).ToList();
                return View(AllEvents);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }



        }


        [HttpGet]
        [Route("new")]
        public IActionResult Create(int EventId)
        {

            ViewBag.Errors = new List<string>();
            if (HttpContext.Session.GetObjectFromJson<object>("ModelState") != null)
            {
                ViewBag.Errors = HttpContext.Session.GetObjectFromJson<object>("ModelState");
                HttpContext.Session.Remove("ModelState");
            }

            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("userName");
                ViewBag.EventId = EventId;
                return View();
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [Route("create")]
        public IActionResult Add(Event newEvent)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {

                if (ModelState.IsValid)
                {
                    ViewBag.UserName = HttpContext.Session.GetString("userName");
                    ViewBag.Errors = new List<string>();
                    newEvent.UserId = (int)userId;
                    _context.Events.Add(newEvent);
                    _context.SaveChanges();
                    int EventId = _context.Events.OrderBy(e => e.id)
                                                .Last(e => e.UserId == (int)userId).id;


                    return RedirectToAction("Acitivity", new { EventId = EventId });
                }
                else
                {
                    ViewBag.Errors = ModelState.Values;
                    HttpContext.Session.SetObjectAsJson("ModelState", ModelState.Values);
                    return View("Create");
                }
            }

            return RedirectToAction("Index", "Home");

        }



        [HttpPost]
        [Route("join/{EventId}")]
        [Route("activity/join/{EventId}")]
        public IActionResult Join(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {
                Event EventToJoin = _context.Events
                        .Single(e => e.id == EventId);

                List<Event> currEvents = _context.Events

                   .Join(_context.Participants.Where(p => p.UserId == (int)userId),
                       e => e.id,
                       p => p.EventId,
                       (e, p) => e)
                   .Where(e => this.isConflict(EventToJoin.EventDateTime, 
                                            this.getDurationNormalized(EventToJoin.DurationType, EventToJoin.Duration),
                                            e.EventDateTime,
                                            this.getDurationNormalized(e.DurationType, e.Duration)
                         )
                           
                   ).ToList();

                if (currEvents.Count > 0)
                {
                    List<string> eventInConflicts = new List<string>();
                    foreach (Event entity in currEvents)
                        eventInConflicts.Add(entity.Name);
                    HttpContext.Session.SetObjectAsJson("JoinError", eventInConflicts);
                }
                else
                {
                    Participant newParticipant = new Participant();

                    newParticipant.UserId = (int)userId;
                    newParticipant.EventId = EventId;
                    _context.Participants.Add(newParticipant);
                    _context.SaveChanges();

                }




                return RedirectToAction("DashBoard");

            }

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [Route("delete/{EventId}")]
        [Route("activity/delete/{EventId}")]
        public IActionResult delete(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {

                _context.Participants.RemoveRange(_context.Participants.Where(p => p.EventId == EventId));


                _context.Events.RemoveRange(_context.Events.Where(e => e.id == EventId));
                _context.SaveChanges();

                return RedirectToAction("DashBoard");

            }

            return RedirectToAction("Index", "Home");

        }


        [HttpPost]
        [Route("leave/{EventId}")]
        [Route("activity/leave/{EventId}")]
        public IActionResult leave(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {

                _context.Participants.RemoveRange(_context.Participants
                .Where(p => p.EventId == EventId && p.UserId == (int)userId));
                _context.SaveChanges();

                return RedirectToAction("DashBoard");

            }

            return RedirectToAction("Index", "Home");

        }



        [HttpGet]
        [Route("activity/{EventId}")]
        public IActionResult Acitivity(int EventId)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("userName");

                List<Event> EventWithIncludes
                 = _context.Events
                .Include(e => e.User)
                .Include(e => e.Participants)
                    .ThenInclude(p => p.User)
                .ToList();


                EventDetails Event = EventWithIncludes
                .Select(e => new EventDetails
                {
                    id = e.id,
                    Name = e.Name,
                    Description = e.Description,
                    CreatedAt = e.CreatedAt,
                    EventDateTime = e.EventDateTime,
                    DurationStr = $"{e.Duration} {e.DurationType}",
                    userLoginId = (int)userId,
                    CreatorId = e.UserId,
                    Creator = e.User,
                    Participants = e.Participants,
                })
                .Single(e => e.id == EventId);

                ViewBag.Event = Event;

                return View();



            }
            return RedirectToAction("Index", "Home");

        }










    }

}
