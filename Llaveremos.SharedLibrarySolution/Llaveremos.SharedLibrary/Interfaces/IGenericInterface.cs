using Llaveremos.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Llaveremos.SharedLibrary.Interfaces
{
    public interface IGenericInterface<T> where T : class
    {
        Task<ApiResponse> CreateAsync(T entity);
        Task<ApiResponse> UpdateAsync(T entity);
        Task<ApiResponse> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetBy(Expression<Func<T, bool>> predicate);
    }
}
