using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder, PriorityLevel? priorityFilter)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var tasks = _context.Tasks.Where(t => t.UserId == user.Id).AsQueryable();

            if (priorityFilter.HasValue)
            {
                tasks = tasks.Where(t => t.Priority == priorityFilter.Value);
            }

            tasks = sortOrder switch
            {
                "priority" => tasks.OrderByDescending(t => t.Priority),
                "date" => tasks.OrderBy(t => t.DueDate ?? t.CreatedDate),
                _ => tasks.OrderByDescending(t => t.CreatedDate)
            };

            var taskList = await tasks.ToListAsync();
            return View(taskList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskItem task)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            task.UserId = user.Id;
            task.CreatedDate = DateTime.Now;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (task.UserId != user.Id) return Unauthorized();

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TaskItem task)
        {
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTask == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (existingTask.UserId != user.Id) return Unauthorized();

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Priority = task.Priority;
            existingTask.DueDate = task.DueDate;
            existingTask.IsCompleted = task.IsCompleted;

            _context.Tasks.Update(existingTask);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (task.UserId != user.Id) return Unauthorized();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
