using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using BetaFit.Infraestructure.Context;
namespace BetaFit.API.Services;
public class OrderTransactionAttribute:ActionFilterAttribute {
 public override async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next){
  if(HttpMethods.IsGet(context.HttpContext.Request.Method)){await next();return;}
  var db=context.HttpContext.RequestServices.GetRequiredService<BetaFitDbContext>();await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
  var result=await next();var status=(result.Result as ObjectResult)?.StatusCode??(result.Result as StatusCodeResult)?.StatusCode??200;
  if(result.Exception==null&&status<400)await tx.CommitAsync();else await tx.RollbackAsync();
 }
}
