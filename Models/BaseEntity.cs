using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace EvaluationMatricesPOC.Models
{
    [Authorize]
    public class BaseEntity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
