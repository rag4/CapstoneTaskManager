using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Capstone___TaskManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone___TaskManager.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ToDoDBContext _toDoDB;
       
        public ToDoController(ToDoDBContext context)
        {
            _toDoDB = context;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            return View(_toDoDB.ToDoItem.Where(x => x.UserId == userId).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ToDoItem t)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            t.UserId = userId;
            if (ModelState.IsValid)
            {
                _toDoDB.ToDoItem.Add(t);
                _toDoDB.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Delete(int id)
        {
            ToDoItem t = _toDoDB.ToDoItem.Find(id);
            return View(t);
        }

        [HttpPost]
        public IActionResult Delete(ToDoItem t)
        {
            if (ModelState.IsValid)
            {
                _toDoDB.ToDoItem.Remove(t);
                _toDoDB.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Complete(int id)
        {
            ToDoItem f = _toDoDB.ToDoItem.Find(id);
            if(f.Complete == true)
            {
                f.Complete = false;
            }
            else
            {
                f.Complete = true;
            }
            _toDoDB.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Search()
        {
            return View();
        }
        public IActionResult SearchWordResults(string word, string status, string dateChoice, DateTime startDate, DateTime endDate)
        {
            List<ToDoItem> t = _toDoDB.ToDoItem.ToList();

            if(word != null)
            {
                t = _toDoDB.ToDoItem.Where(x => x.Description.Contains(word)).ToList();
            }

            if(status == "yes")
            {
                t = t.Where(x => x.Complete == true).ToList();
            }
            if(status == "no")
            {
                t = t.Where(x => x.Complete == false).ToList();
            }

            if(dateChoice == "yes")
            {
                t = t.Where(x => x.DueDate >= startDate).ToList();
                t = t.Where(x => x.DueDate <= endDate).ToList();
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            t = t.Where(x => x.UserId == userId).ToList();
            return View(t);
        }
    }
}
