using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Silerium.Models;
using System.Linq.Expressions;

namespace Silerium.Data.Configurations
{
    public class OrderStatusConverter : ValueConverter<OrderStatus, string>
    {
        public OrderStatusConverter() : base(
            str => str.ToString(),
            os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os))
        {
        }
    }
}
