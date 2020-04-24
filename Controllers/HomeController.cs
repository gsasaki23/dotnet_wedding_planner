using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wedding_planner.Models;

namespace wedding_planner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingPlannerContext db;
        private int? uid{get {return HttpContext.Session.GetInt32("UserId");}}

        public HomeController(WeddingPlannerContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            // If there is a user already logged in (=it's in session), jump them in
            if (uid != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "is already taken!");
                }
            }
            // Reload with error messages
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }

            // Overwrite password with hashed version
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);

            // Add attributes
            newUser.CreatedAt = DateTime.Now;
            newUser.UpdatedAt = DateTime.Now;
            
            // Add and save
            db.Users.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("Dashboard");
        }
        public IActionResult Login(LoginUser loginUser)
        {
            // Show error messages
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }

            // Get user with matching email from DB
            User dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);
            // If no matching users
            if (dbUser == null)
            {
                ModelState.AddModelError("LoginEmail", "Invalid Email or Password");
            }
            else
            {
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);
                // If passwords don't match
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email or Password");
                }
            }
            // If either no matching email OR email found but no matching PW, show error messages
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            return RedirectToAction("Dashboard");
        }


        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            // If no user signed in, kick them out
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }

            // User to use in page
            User currentUser = db.Users.FirstOrDefault(user => user.UserId == UserId);
            ViewBag.weddings = db.Weddings
                .Include(wedding => wedding.HostUser)
                .Include(wedding => wedding.AllRSVPs)
                .ThenInclude(rsvp => rsvp.User)
                .OrderBy(wedding => wedding.Date)
                .ToList();

            return View(currentUser);
        }

        
        // New Wedding GET and POST
        [HttpGet("/new")]
        public IActionResult NewWedding()
        {
            // If no user signed in, kick them out
            if (uid == null)
            {
                return RedirectToAction("Index");           
            }
            return View();
        }
        [HttpPost("/create")]
        public IActionResult AddWeddingToDB(Wedding newWedding)
        {
            // If no user signed in, kick them out
            if (uid == null)
            {
                return RedirectToAction("Index");           
            }
            if (ModelState.IsValid)
            {
                newWedding.HostUserId = (int)uid;
                db.Add(newWedding);

                

                db.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            // If there were any ModelState invalidations, re-render form with error msg
            return View("NewWedding");
        }

        
        // Wedding Detail
        [HttpGet("/weddings/{weddingId}")]
        public IActionResult Wedding(int weddingId)
        {
            // If no user signed in, kick them out
            if (uid == null)
            {
                return RedirectToAction("Index");           
            }
            Wedding wedding = db.Weddings
                .Include(w => w.HostUser)
                .Include(w => w.AllRSVPs)
                .ThenInclude(rsvp => rsvp.User)
                .FirstOrDefault(w => w.WeddingId == weddingId);
            if (wedding != null)
            {
                return View(wedding);
            }
            return RedirectToAction("Dashboard");
        }


        // DELETE a Wedding
        [HttpPost("/weddings/{weddingId}/delete")]
        public IActionResult Delete(int weddingId)
        {
            // Remove the first wedding found in DB with same id
            db.Weddings.Remove(db.Weddings.FirstOrDefault(w=>w.WeddingId == weddingId));
            db.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("/weddings/{weddingId}/addRSVP")]
        public IActionResult AddRSVP(int weddingId)
        {
            RSVP newRSVP = new RSVP();
            newRSVP.UserId = (int)uid;
            newRSVP.WeddingId = weddingId;
            db.Add(newRSVP);
            db.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("/weddings/{weddingId}/unRSVP")]
        public IActionResult UnRSVP(int weddingId)
        {
            db.RSVPs.Remove(db.RSVPs.FirstOrDefault(w=>w.WeddingId == weddingId && w.UserId == uid));
            db.SaveChanges();
            return RedirectToAction("Dashboard");
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
