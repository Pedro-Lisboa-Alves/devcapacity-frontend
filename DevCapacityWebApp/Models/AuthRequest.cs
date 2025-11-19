using System;

namespace DevCapacityWebApp.Models
{
    public class AuthRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Model { get; set; } // Adicionado para compatibilidade com a API
    }
}
