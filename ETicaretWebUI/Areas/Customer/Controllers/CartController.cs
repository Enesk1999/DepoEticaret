using ETicaret.Data.Repository;
using ETicaret.Model.Models;
using ETicaret.Model.ViewModels;
using ETicaret.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;

namespace ETicaretWebUI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }    //viewModelim
        public CartController(IUnitOfWork unit)
        {
            unitOfWork = unit;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = unitOfWork.ShoppingCart.GetAll(x => x.AppUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach(var sepetim in ShoppingCartViewModel.ShoppingCartList)
            {
                sepetim.Price = GetUrunAdetFiyat(sepetim);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (sepetim.Price * sepetim.Count);

            }


            return View(ShoppingCartViewModel);
        }



        public IActionResult Ekleme(int sepetId)
        {
            var sepetDb = unitOfWork.ShoppingCart.Get(u => u.Id == sepetId);
            sepetDb.Count += 1;
            unitOfWork.ShoppingCart.Update(sepetDb);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Cikarma(int sepetId)
        {
            var sepetDb = unitOfWork.ShoppingCart.Get(u => u.Id == sepetId);
            if(sepetDb.Count <= 1) //son ürün
            {
                unitOfWork.ShoppingCart.Remove(sepetDb);
            }
            else
            {
                sepetDb.Count -= 1;
                unitOfWork.ShoppingCart.Update(sepetDb);
            }
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Silme(int sepetId)
        {
            var sepetDb = unitOfWork.ShoppingCart.Get(u => u.Id == sepetId);
            unitOfWork.ShoppingCart.Remove(sepetDb);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = unitOfWork.ShoppingCart.GetAll(x => x.AppUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartViewModel.OrderHeader.AppUser = unitOfWork.AppUser.Get(u => u.Id == userId);

            ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.AppUser.Name;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.AppUser.State;
            ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.AppUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.AppUser.City;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.AppUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.AppUser.PostalCode;

            foreach(var sepet in ShoppingCartViewModel.ShoppingCartList)
            {
                sepet.Price = GetUrunAdetFiyat(sepet);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (sepet.Count * sepet.Price);
            }

            return View(ShoppingCartViewModel);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var kullaniciId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //ürün sepet ayarlarlamaları kullanıcı bilgileri ürün bilgileri sepetde ki ürünlerin ayarlamaları
            ShoppingCartViewModel.ShoppingCartList = unitOfWork.ShoppingCart.GetAll(x => x.AppUserId == kullaniciId, includeProperties:"Product");
            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartViewModel.OrderHeader.AppUserId = kullaniciId;

            AppUser appUser = unitOfWork.AppUser.Get(x => x.Id == kullaniciId);

            //Toplam tutarı eklene her yeni ürüne güncelleme
            foreach( var sepet in ShoppingCartViewModel.ShoppingCartList)
            {
                sepet.Price = GetUrunAdetFiyat(sepet);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (sepet.Price * sepet.Count); 
            }

            //Müşteri kullanıcısı
			if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymnentStatusPending;
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            //Firma Kullanıcısı
            else
            {
				ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusApproved;
			}
            unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
            unitOfWork.Save();
            foreach(var sepet in ShoppingCartViewModel.ShoppingCartList)
            {
                OrderDetail detail = new()
                {
                    ProductId = sepet.ProductId,
                    OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = sepet.Price,
                    Count = sepet.Count,
                };
                unitOfWork.OrderDetail.Add(detail);
                unitOfWork.Save();
            }
            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Müşteri kısmı
                //stripe eklediğim yer

                var domain = "https://localhost:7222/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain +$"customer/cart/orderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };
                foreach(var item in ShoppingCartViewModel.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), //10,99  => 1099tl ile işlem yapar
                            Currency = "TRY",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);
                unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });

        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "AppUser");
            //Herhangi bir müşteri alışverişi
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);
				if (session.PaymentStatus.ToLower() == "paid")
				{
					unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
					unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					unitOfWork.Save();
				}


			}
            List<ShoppingCart> shoppings = unitOfWork.ShoppingCart.GetAll(o => o.AppUserId == orderHeader.AppUserId).ToList();
            unitOfWork.ShoppingCart.RemoveRange(shoppings);
            unitOfWork.Save();
            return View(id);
        }

        private double GetUrunAdetFiyat(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
