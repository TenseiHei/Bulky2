using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            try
            {
                List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
                return View(objCategoryList);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Category NotFound";
                return RedirectToAction("Home/Index");
            }
        }

        //[Route("Category/Create/{id}")]
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Category NotFound";
                return RedirectToAction("Index");
            }
        }
        [HttpPost] // We are sending information to the server.
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        [Route("Category/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            try
            {
                Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
                return View(categoryFromDb);
            }
            catch (Exception ex) 
            {
                TempData["error"] = "Category NotFound";
                return RedirectToAction("Index");
            }
        }

        [HttpPost] // We are sending information to the server.
        [Route("Category/Edit/{id}")]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [Route("Category/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
                return View(categoryFromDb);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Category NotFound";
                return RedirectToAction("Index");
            }
        }
        [HttpPost, ActionName("Delete")] // We are sending information to the server.
        [Route("Category/Delete/{id}")]
        public IActionResult DeletePOST(int id)
        {
            Category obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
