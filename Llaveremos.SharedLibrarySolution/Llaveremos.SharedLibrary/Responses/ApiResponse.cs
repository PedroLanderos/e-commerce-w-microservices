using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Llaveremos.SharedLibrary.Responses
{
    public record ApiResponse(bool Flag = false, string Message = null!);
}
