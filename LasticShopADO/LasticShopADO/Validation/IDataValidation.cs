using LasticShopAdo.Models;

namespace LasticShopAdo.Validation
{
    internal interface IDataValidation
    {
        public Task<BaseResponse<bool>> EmailValidation(string email);
        public Task<BaseResponse<bool>> PasswordValidation(string email, string password);
    }
}

