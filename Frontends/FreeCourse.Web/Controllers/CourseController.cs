﻿using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public CourseController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
        {
            _catalogService = catalogService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _sharedIdentityService.GetUserId;
            var courses = await _catalogService.GetAllCourseByUserIdAsync(userId);
            return View(courses);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _catalogService.GetAllCategoryAsync();

            //Kullanıcı seçenek üzerinden ekleyebilmesi için
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateVM courseCreateVM)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");

            //Model kontrol
            if (!ModelState.IsValid)
            {
                return View();
            }

            //UserId'yi ekliyoruz. Authorize ile işaretlediğimiz için alacağız token üzerinden.
            courseCreateVM.UserId = _sharedIdentityService.GetUserId;
            await _catalogService.CreateCourseAsync(courseCreateVM);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(string id)
        {
            var course = await _catalogService.GetByCourseIdAsycn(id);

            if (course == null)
            {
                //to do mesaj gösterilebilir
                return RedirectToAction(nameof(Index));
            }

            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name", course.Id);

            CourseUpdateVM courseUpdateVM = new()
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Price = course.Price,
                Feature = course.Feature,
                CategoryId = course.CategoryId,
                UserId = course.UserId,
                Picture = course.Picture
            };

            return View(courseUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CourseUpdateVM courseUpdateVM)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name", courseUpdateVM.Id);

            if (!ModelState.IsValid)
            {
                return View();
            }

            await _catalogService.UpdateCourseAsync(courseUpdateVM);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _catalogService.DeleteCourseAsycn(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
