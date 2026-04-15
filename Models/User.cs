using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace EvaluationMatricesPOC.Models
{
    [Authorize]
    public class User : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
