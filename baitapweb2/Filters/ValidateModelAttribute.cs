using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace baitapweb2.Filters // Cập nhật namespace cho đúng với cấu trúc dự án của bạn
{
    // Kế thừa từ ActionFilterAttribute
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        // Ghi đè phương thức này để thực thi logic trước khi Action Method chạy
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Kiểm tra ModelState
            if (!context.ModelState.IsValid)
            {
                // Nếu không hợp lệ, trả về lỗi 400 Bad Request ngay lập tức
                // Lỗi Bad Request này sẽ chứa chi tiết các lỗi validation (ModelState)
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}