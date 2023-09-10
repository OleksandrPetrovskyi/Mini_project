using LasticShopAdo.Models;

namespace LasticShopAdo.Repository
{
    internal interface IShopRepository
    {
        public Task<BaseResponse<User>> GetUserByEmail(string email);
        public Task<BaseResponse<User>> AccountLogin(string email);
        public Task<BaseResponse<User>> GetUserById(int userId);
        public Task<BaseResponse<Product>> GetProductById(int productId);
        public Task<BaseResponse<List<Product>>> GetProductsByCategory(string Category);
        public Task<BaseResponse<int>> CreateNewProduct(Product product);
        public Task<BaseResponse<int>> CreateNewUser(User user);
        public Task<BaseResponse<bool>> CreatNewReview(Review review);
        public Task<BaseResponse<List<string>>> GetProductCategories();
        public Task<BaseResponse<bool>> BuyProductById(Product product, int quantity);
    }
}

