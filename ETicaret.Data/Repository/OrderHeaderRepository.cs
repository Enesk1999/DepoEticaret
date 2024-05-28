using ETicaret.Model.Models;
using ETicaretWebUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Data.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext applicationDbContext;
		public OrderHeaderRepository(ApplicationDbContext rr) : base(rr)
		{
			applicationDbContext = rr;
		}

		public void Update(OrderHeader orderHeader)
		{
			applicationDbContext.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderDb = applicationDbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if(orderDb != null){
				orderDb.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderDb.PaymentStatus = paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderDb = applicationDbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);

			if (!string.IsNullOrEmpty(sessionId))
			{
				orderDb.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderDb.PaymentIntentId = paymentIntentId;
				orderDb.PaymentDate = DateTime.Now;
			}
		}
	}
}
