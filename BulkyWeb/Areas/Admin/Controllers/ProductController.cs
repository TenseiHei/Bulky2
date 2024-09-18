using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            try
            {
                List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
                return View(objProductList);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Products NotFound";
                return RedirectToAction("Home/Index");
            }
        }

        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Product Could Not Be Created";
                return RedirectToAction("Index");
            }
        }
        [HttpPost] // We are sending information to the server.
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        [Route("Product/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            try
            {
                Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productFromDb);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Product Does Not Exist";
                return RedirectToAction("Index");
            }
        }

        [HttpPost] // We are sending information to the server.
        [Route("Product/Edit/{id}")]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        [Route("Product/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productFromDb);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Product Does Not Exist";
                return RedirectToAction("Index");
            }
        }
        [HttpPost, ActionName("Delete")] // We are sending information to the server.
        [Route("Product/Delete/{id}")]
        public IActionResult DeletePOST(int id)
        {
            Product obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
