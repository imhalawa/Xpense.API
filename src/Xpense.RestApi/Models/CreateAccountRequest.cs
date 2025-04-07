using System.ComponentModel.DataAnnotations;
using Xpense.Core.Features.Accounts.Commands;

namespace Xpense.RestApi.Models
{
   public class CreateAccountRequest
   {
      [Required]
      public required string Name { get; set; }
      
      [Required]
      public required decimal Balance { get; set; }

      public CreateAccountCommand ToCommand(){
         return new CreateAccountCommand(Name, Balance);
      }
   }
}