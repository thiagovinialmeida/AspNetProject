using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewMordels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var list = _sellerService.FindAll();
            return View(list);
        }

        //GET: CreateSeller
        public IActionResult Create()
        {
            var departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        //POST: CreateSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Seller seller)
        {
            if (!ModelState.IsValid) { return View(seller); }

            _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }

        //GET: Details
        public IActionResult Details(int? id)
        {
            if (id == null){ return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            var obj = _sellerService.FindById(id.Value);
            if(obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            return View(obj);
        }

        //GET: DeleteSeller
        public IActionResult Delete(int? id)
        {
            if (id == null) { RedirectToAction(nameof(Error), new { message = "Id not provided" }); }

            var obj = _sellerService.FindById(id.Value);
            if (obj == null){ return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            return View(obj);
        }

        //POST: DeleteSeller
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _sellerService.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        //GET: EditSeller
        public IActionResult Edit(int? id)
        {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); }

            var obj = _sellerService.FindById(id.Value);
            if (obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            List<Department> departments = _departmentService.FindAll();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
            return View(viewModel);
        }

        //POST: EditSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid) { return View(seller); }
            if (id != seller.Id) { return RedirectToAction(nameof(Error), new { message = "Id mismatch" }); }

            try
            {
                _sellerService.Update(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            catch (DbConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        //ERROR
        public IActionResult Error(string message)
        {
            var viewmodel = new ErrorViewModel()
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewmodel);
        }
    }
}
