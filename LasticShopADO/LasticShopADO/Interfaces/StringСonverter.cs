using LasticShopAdo.Models;
using System.Text;

namespace LasticShopAdo.Interfaces
{
    internal class StringСonverter
    {
        public string FullProductInfo(Product product)
        {
            var message = new StringBuilder();

            message.Append(@$"      {product.Title}
User ID: {product.UserId}
Lot ID: {product.Id}
Price: {product.Price} {product.Valuta}
Remainder: {product.Balance} {Environment.NewLine}");

            if (string.IsNullOrEmpty(product.Description))
                message.Append($"Description: no description.{Environment.NewLine}");
            else
                message.Append($"Description: {product.Description}{Environment.NewLine}");

            message.Append($"Reviews: {Environment.NewLine}");

            if (product.Reviews == null)
                message.Append($"No reviews.{Environment.NewLine}");
            else
            {
                for (int j = 0; j < product.Reviews.Count; j++)
                {
                    foreach (var review in product.Reviews)
                    {
                        message.Append($@"User {review.FirsName} {review.SecondName} ({review.UserId}) rated the product {review.Evalution} points
Review: {review.Description} {Environment.NewLine}");
                    }
                }
            }

            return message.ToString();
        }
        public string ShortProductInfo(Product product)
        {
            var message = @$"      {product.Title}
User ID: {product.UserId}
Lot ID: {product.Id}
Price: {product.Price} {product.Valuta}
";

            return message;
        }

        public string FullUserToString(User user)
        {
            var message = new StringBuilder(ShortUserToString(user));

            for (int i = 0; i < user.Products.Count; i++)
            {
                var product = user.Products[i];

                message.Append($@"Lot name: {product.Title}
Price {product.Price} {product.Valuta}

");
            }
            return message.ToString();
        }

        public string ShortUserToString(User user)
        {
            var message = string.Empty;

            message = $"User: {user.FirstName} {user.SecondName}{Environment.NewLine}";
            if (user.Products.Count == null)
                message += $"This user is not selling anything{Environment.NewLine}{Environment.NewLine}";
            else
                message += $"Number of lots: {user.Products.Count()}.{Environment.NewLine}{Environment.NewLine}";

            return message;
        }
        public string ListToString(List<string> list)
        {
            var result = Environment.NewLine;

            foreach (var item in list)
            {
                result += $"{item}{Environment.NewLine}";
            }

            return result;
        }
    }
}

