﻿using System.Diagnostics.Metrics;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using System.Linq;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public List<Seller> FindAll()
        {
            return _context.Seller.ToList();
        }
        public Seller FindById(int id)
        {
            return _context.Seller.FirstOrDefault(obj => obj.Id == id);
        }
        public void Insert(Seller obj)
        {
            _context.Add(obj);
            _context.SaveChanges();
        }
        public void Remove(int id)
        {
            var obj = _context.Seller.Find(id);
            _context.Remove(obj);
            _context.SaveChanges();
        }
    }
}
