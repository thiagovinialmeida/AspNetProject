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

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync();
            return View(list);
        }

        //GET: CreateSeller
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        //POST: CreateSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = await _departmentService.FindAllAsync() };
                return View(viewModel);
            }

            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        //GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null){ return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if(obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            return View(obj);
        }

        //GET: DeleteSeller
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { RedirectToAction(nameof(Error), new { message = "Id not provided" }); }

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null){ return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            return View(obj);
        }

        //POST: DeleteSeller
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sellerService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        //GET: EditSeller
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return RedirectToAction(nameof(Error), new { message = "Id not provided" }); }

            var obj = await _sellerService.FindByIdAsync(id.Value);
            if (obj == null) { return RedirectToAction(nameof(Error), new { message = "Id not found" }); }

            List<Department> departments = await _departmentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
            return View(viewModel);
        }

        //POST: EditSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid) 
            {
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = await _departmentService.FindAllAsync()};
                return View(viewModel); 
            }
            if (id != seller.Id) { return RedirectToAction(nameof(Error), new { message = "Id mismatch" }); }

            try
            {
                await _sellerService.UpdateAsync(seller);
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
