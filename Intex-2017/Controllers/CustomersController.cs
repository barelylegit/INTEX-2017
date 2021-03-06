﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Intex_2017.DAL;
using Intex_2017.Models;
using Intex_2017.Models.ViewModels;


namespace Intex_2017.Controllers
{
    public class CustomersController : Controller
    {
        private IntexContext db = new IntexContext();

        // GET: Customers
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        [Authorize(Roles = "SalesAgent, SysAdmin")]
        public ActionResult IndexSalesAgent()
        {
            List<SalesAgentCustomerListViewModel> viewModelList = new List<SalesAgentCustomerListViewModel>();
            List<Customer> customerList = new List<Customer>();

            customerList = db.Customers.ToList();

            foreach (Customer c in customerList)
            {
                SalesAgentCustomerListViewModel viewModel = new SalesAgentCustomerListViewModel();
                viewModel.CustFirstName = c.CustFirstName;
                viewModel.CustLastName = c.CustLastName;
                viewModel.CustUsername = c.CustUsername;
                viewModel.CustCompany = c.CustCompany;
                viewModel.CustPhone = c.CustPhone;
                viewModelList.Add(viewModel);
            }
            return View(viewModelList);
        }

        // GET: Customers/Details/5
        [Authorize(Roles = "SysAdmin, SalesAgent, Customer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
			List<PaymentMethod> customer_Payments = new List<PaymentMethod>();
			customer_Payments = db.PaymentMethods.ToList();
			ViewBag.MyList = customer_Payments.ToList();
            return View();
        }


		// POST: Customers/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustID,CustFirstName,CustLastName,CustAddress1,CustAddress2,CustCity,CustState,CustZip,CustEmail,CustPhone,PaymentMethodID,CustUsername,CustPassword,CustCompany")] Customer customer)
        {
			List<Customer> checkList = new List<Customer>();
			checkList = db.Customers.ToList();
			for (int i = 0; i < checkList.Count; i++)
			{
				if (customer.CustUsername == checkList[i].CustUsername)
				{
					List<PaymentMethod> customer_Payments = new List<PaymentMethod>();
					customer_Payments = db.PaymentMethods.ToList();
					ViewBag.MyList = customer_Payments.ToList();
					return View(customer);
				}
			}
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                if (User.IsInRole("SysAdmin") || (User.IsInRole("SalesAgent")))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var createSuccess = "Your account has been successfully created!";
                    return RedirectToAction("CustomerLogin", "Login", new { message = createSuccess });
                }
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        [Authorize(Roles = "SysAdmin, SalesAgent, Customer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustID,CustFirstName,CustLastName,CustAddress1,CustAddress2,CustCity,CustState,CustZip,CustEmail,CustPhone,PaymentMethodID,CustPassword")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        [Authorize(Roles = "SysAdmin, SalesAgent")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
