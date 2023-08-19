using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Silerium.Models;
using System.Linq.Expressions;

namespace Silerium.Data.Configurations
{
    public class EnumConverter<T> : ValueConverter<T, string>
    {
        public EnumConverter() : base(
            str => str.ToString(),
            os => (T)Enum.Parse(typeof(T), os))
        {
        }
    }
}
