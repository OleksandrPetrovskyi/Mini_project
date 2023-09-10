using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasticShopAdo.Models
{
    internal class BaseResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> Errors { get; set; } = new List<string>();

        public BaseResponse()
        {
            Data = default;
        }

        public BaseResponse(T data)
        {
            Data = data;
        }

        public BaseResponse(bool success, List<string> errors)
        {
            IsSuccess = success;
            Errors = errors;
        }

        public async Task AddError(string error)
        {
            IsSuccess = false;
            Errors.Add(error);
        }
        public async Task AddErrors(List<string> errors)
        {
            IsSuccess = false;

            foreach (string error in errors)
            {
                Errors.Add(error);
            }
        }
    }
}
