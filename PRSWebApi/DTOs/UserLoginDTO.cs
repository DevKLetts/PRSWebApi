using System;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace PRSWebApi.DTOs
{
        public class UserLoginDTO
        {
            [Required(ErrorMessage = "Username is required")]
            public string? Username { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string? Password { get; set; }
        }
    
}
