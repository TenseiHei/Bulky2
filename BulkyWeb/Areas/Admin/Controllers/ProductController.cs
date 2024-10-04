using AutoMapper;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility.DTO.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            try
            {
                List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
                return View(objProductList);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Products NotFound";
                return RedirectToAction("Home/Index");
            }
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            try
            {
                ProductVM productVM = new()
                {
                    CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    Product = new Product()
                };
                if (id == null || id == 0)
                {
                    //Create
                    return View(productVM);
                }
                else
                {
                    //Update
                    productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                    return View(productVM);
                }                
            }
            catch (Exception ex)
            {
                TempData["error"] = "Product Could Not Be Created";
                return RedirectToAction("Index");
            }
        }

        [HttpPost] // We are sending information to the server.
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                ProductDto productDto = _mapper.Map<ProductDto>(productVM.Product);
                TempData["success"] = "Product created successfully";
                return Ok(productDto);
                //return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
            //return View();
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


        //#region API CALLS

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        //    return Json(new { data = objProductList });
        //}

        //#endregion
    }
}
